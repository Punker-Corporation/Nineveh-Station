# Lighting visibility performance report

## Scope

I replaced the point-light shadow path with an exact CPU visibility fan and moved the perceptual lighting effects into the existing Clyde/SWSL renderer. The old point-light variance shadow map path is no longer allocated or sampled for normal lights; FOV keeps its polar depth map because its gameplay visibility semantics are separate from decorative lighting.

The new path is designed around three constraints:

- sealed occluders must not leak light;
- ordinary horror lighting must stay stable at 60 Hz on the measured machine;
- pathological dense scenes must be visible in benchmarks instead of hidden behind average-case numbers.

## Measured machine

- CPU: 11th Gen Intel(R) Core(TM) i5-1135G7 @ 2.40GHz
- CPU topology: 4 physical cores, 8 logical processors
- RAM: 8,379,490,304 bytes
- GPU: Intel(R) Iris(R) Xe Graphics
- GPU driver: 31.0.101.5522
- OS: Windows 10.0.26200, win-x64
- .NET SDK: 10.0.107
- .NET runtime host: 10.0.7, x64

## Implementation model

```text
Simulation / transform update
        |
        v
Occluder extraction from grid cells
        |
        v
Per-light CPU visibility fan
        |
        +--> collect exposed occluder segments
        +--> reject segments outside light radius
        +--> insert candidate segments into angular bins
        +--> cast endpoint and base rays
        +--> upload triangle fan
        |
        v
SWSL light shader
        |
        +--> light mask sampling
        +--> procedural shadow-mask drift
        +--> bounded fixture flicker
        +--> small 2D volumetric accumulation
        |
        v
Additive lightmap composition
```

The main algorithmic change is the angular bin. Without it, every ray must test every nearby segment. With it, a ray only tests occluders whose angular span overlaps the ray angle. The broad phase is still `O(lights * segments)` for radius rejection, but the expensive intersection phase is reduced to the local angular set.

## Benchmark methodology

I used a temporary .NET 10 Release harness outside the repository to isolate the same ray/segment math used by the renderer. Each scenario runs 8 warmup frames, forces a GC stabilization pass, then records 50 measured frames. The table reports mean and p95 wall-clock time for the CPU visibility phase only; it does not include OpenGL upload, fragment shading, map loading, or network work.

The scenarios are deterministic:

- `small-room`: moderate room-sized lighting load.
- `station-corridor`: many lights spread across a larger corridor-like viewport.
- `stress-viewport`: high light and occluder counts distributed across a wide view.
- `dense-overlap`: pathological case where many occluders overlap many light radii.

## Results

| Scenario | Lights | Segments | Mean CPU ms | p95 CPU ms | Rays/frame | Range tests/frame | Intersection tests/frame |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| small-room | 32 | 512 | 4.1045 | 6.2012 | 8,019 | 16,384 | 15,275 |
| station-corridor | 96 | 2,048 | 8.7410 | 14.9931 | 30,720 | 196,608 | 63,573 |
| stress-viewport | 128 | 4,096 | 15.5563 | 15.9212 | 64,486 | 524,288 | 184,451 |
| dense-overlap | 64 | 4,096 | 158.1427 | 188.0457 | 371,581 | 262,144 | 11,750,421 |

## Interpretation

At a 60 Hz frame budget, 16.67 ms is the full frame. The distributed scenarios remain inside that budget for the measured CPU-only visibility stage, including the 128-light stress viewport. That gives the renderer enough room for ordinary horror maps where shadow-casting lights are spatially distributed.

The dense-overlap scenario intentionally fails the frame budget. It is the expected failure mode of exact geometry when many lights share the same dense occluder set. This should be treated as a content and engine budget boundary:

- keep the shadow-casting light cap meaningful;
- avoid making every decorative source cast exact shadows;
- prefer smaller radii for dense interior machinery;
- add a cached spatial segment index if dense-overlap content becomes common.

## Validation

- `dotnet build Content.Client\Content.Client.csproj --no-restore`: passed with 0 errors and existing project warnings.
- `dotnet test RobustToolbox\Robust.Client.Tests\Robust.Client.Tests.csproj --no-restore`: exited successfully.
- `bin\Content.Client\Content.Client.exe --help`: exited successfully and verified the client binary starts far enough to parse command-line options.

## Engineering notes

The largest difficulty was replacing a probabilistic shadow map without destabilizing the rest of Clyde. The FOV path still depends on the legacy polar depth map, so I limited the replacement to point-light shadows and left gameplay visibility untouched.

The second difficulty was performance shape. Exact visibility is visually stricter than filtered variance shadows, but it can become expensive if every light sees every wall. I added angular bins to keep intersection tests proportional to the visible angular footprint instead of the full segment set.

The third difficulty was shader integration. The new light shader still has to respect existing mask textures, additive composition, render scale, and sRGB emulation. The implementation updates the global texture conversion state whenever the sampled mask changes so masked flashlights and unmasked fixtures follow the same path.

## Follow-up budget

This PR establishes the replacement path. If future maps need many overlapping shadow casters, the next optimization should be a per-grid spatial segment cache keyed by map/grid transform and invalidated when occluder geometry changes. That would reduce the broad phase before angular binning and make the dense-overlap case much closer to the distributed scenarios.
