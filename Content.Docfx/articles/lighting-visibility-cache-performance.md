# Lighting visibility cache performance report

## Scope

This report measures the second stage of the experimental 2D lighting pipeline. The first stage replaced filtered point-light shadow maps with exact CPU visibility fans. This stage changes the cost model again by adding:

- a spatial bucket index for occluder segments;
- a persistent geometry signature;
- a temporal cache for already-built light visibility fans;
- a lower-cost interleaved volumetric shader path.

The goal is not merely to make one loop faster. The goal is to avoid doing visibility work at all when the light/geometry state has not changed, and to reduce the candidate set when a rebuild is unavoidable.

## Measured machine

- CPU: 11th Gen Intel(R) Core(TM) i5-1135G7 @ 2.40GHz
- CPU topology: 4 physical cores, 8 logical processors
- RAM: 8,379,490,304 bytes
- GPU: Intel(R) Iris(R) Xe Graphics
- GPU driver: 31.0.101.5522
- OS: Windows 10.0.26200, win-x64
- .NET SDK: 10.0.107
- .NET runtime host: 10.0.7, x64

## Algorithmic change

The first exact-visibility implementation had two dominant costs:

1. each light scanned the full viewport occluder list before radius rejection;
2. each frame rebuilt every light fan even if the station geometry and the light transform were unchanged.

The new version splits the problem into three layers.

```text
Occluder extraction
        |
        v
Stable geometry hash
        |
        +-- unchanged: keep spatial buckets and fan cache
        |
        +-- changed: rebuild spatial buckets and clear fan cache
        |
        v
Per-light visibility
        |
        +-- cache hit: upload cached fan vertices
        |
        +-- cache miss:
              query spatial buckets overlapping light radius
              run exact segment/ray construction only on candidates
              store resulting fan vertices
```

The fan cache key is intentionally conservative: geometry version, exact light position bits, exact radius bits, and exact mask rotation bits. This avoids temporal ghosting. If a light flickers by changing radius, it rebuilds. If a flashlight rotates, it rebuilds. If a wall changes, the geometry version invalidates every fan.

## Shader change

The volumetric path was also reduced. The previous light shader marched 8 samples for every shaded fragment. The new path:

- discards fragments beyond the light radius before noise and volumetric work;
- uses 5 interleaved ray-march steps;
- jitters the first sample per fragment to hide banding at lower sample counts;
- keeps the same bounded flicker and procedural mask drift model.

This is a deliberate temporal sampling tradeoff: fewer deterministic samples plus stable spatial jitter usually reads smoother in motion than more expensive aligned steps.

## Benchmark methodology

I used a temporary .NET 10 Release harness outside the repository. It mirrors the renderer's spatial bucket query, angular binning, ray/segment math, and exact cache key behavior.

Each scenario records:

- `cold`: cache is cleared before each frame; geometry buckets remain available;
- `hot`: same lights and same geometry are rendered repeatedly, so every fan is a cache hit;
- `moved`: lights move slightly every frame, forcing fan rebuilds while still using the spatial index.

The benchmark runs warmup passes before measurement. Cold uses 30 samples. Hot and moved use 50 samples.

## Results

| Scenario | Lights | Segments | Cold mean ms | Cold p95 ms | Hot mean ms | Hot p95 ms | Moved mean ms | Moved p95 ms | Cold fan builds | Hot cache hits | Moved intersections |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| small-room | 32 | 512 | 4.2291 | 5.9845 | 0.0049 | 0.0052 | 3.2607 | 5.2643 | 32 | 32 | 15,237 |
| station-corridor | 96 | 2,048 | 7.2831 | 17.4491 | 0.0034 | 0.0039 | 4.5224 | 5.6652 | 96 | 96 | 63,542 |
| stress-viewport | 128 | 4,096 | 9.9622 | 11.4729 | 0.0008 | 0.0009 | 9.6689 | 10.2885 | 128 | 128 | 184,250 |
| dense-overlap | 64 | 4,096 | 143.8960 | 146.1046 | 0.0004 | 0.0006 | 148.9272 | 172.7370 | 64 | 64 | 11,751,781 |

## Comparison to the first exact path

The previous report measured the first exact path with p95 values:

- small-room: 6.2012 ms;
- station-corridor: 14.9931 ms;
- stress-viewport: 15.9212 ms;
- dense-overlap: 188.0457 ms.

The new hot-cache path makes static lighting effectively free on the CPU visibility side. That is the important result for horror maps: most environmental lights, emergency fixtures, lamps, and room-scale practicals do not move every frame.

For moving lights, the distributed scenarios improve because the spatial index avoids scanning the entire segment set. The dense-overlap case remains expensive when lights move because it is geometrically pathological: most lights really do overlap most occluders, so the exact algorithm must process a large angular set.

## Interpretation

The system now has three performance tiers:

- static lights: cache hits dominate; CPU visibility work is amortized across frames;
- moving distributed lights: spatial buckets reduce rebuild cost;
- moving dense-overlap lights: still bounded by real geometric complexity.

This is the intended shape for a horror renderer. Static darkness and static practical lighting should be cheap. Player flashlights and moving threat lights should pay only for the local geometry they touch. A stress case where dozens of large lights cover a dense wall field remains the correct place to spend future work.

## Remaining research direction

The next theoretical step is a perceptual scheduler:

- prioritize lights by screen coverage, luminance, threat proximity, and camera salience;
- rebuild low-salience fans at lower temporal frequency;
- keep exact fan data but update it on a visibility budget instead of rebuilding all misses immediately;
- blend old/new fans only for soft perceptual effects, never for hard sealed-wall visibility.

That would turn the dense-overlap case from a per-frame worst case into a budgeted multi-frame convergence problem while preserving exactness for the highest-salience lights.
