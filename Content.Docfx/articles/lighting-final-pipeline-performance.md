# Final lighting pipeline performance report

## Scope

This report closes the current lighting workstream by measuring the final state of the exact point-light visibility pipeline after the previous spatial-cache stage. The implementation now uses:

- exact CPU-built visibility fans for hard, leak-free point-light shadows;
- a spatial bucket index for viewport occluder segments;
- a conservative temporal cache keyed by geometry version and light state;
- persistent GPU vertex buffers for cached visibility fans;
- persistent GPU vertex array objects for cached visibility fan layout;
- a perceptual base-sample scheduler that keeps silhouette rays exact while reducing round-light perimeter cost for low-salience lights;
- explicit teardown for cached GPU visibility resources before the graphics context is shut down.

The goal of this final stage is to remove the remaining per-frame hot-path upload and vertex-layout work introduced by exact visibility. Static lights in static geometry should now amortize both CPU fan construction and GPU fan upload.

## Measured machine

- CPU: 11th Gen Intel(R) Core(TM) i5-1135G7 @ 2.40GHz
- CPU topology: 4 physical cores, 8 logical processors
- RAM: 8,379,490,304 bytes
- GPU: Intel(R) Iris(R) Xe Graphics
- GPU driver: 31.0.101.5522
- OS target observed during local builds: win-x64
- .NET SDK used by build: 10.0.107

## Final pipeline model

```text
Viewport wall extraction
        |
        v
Occluder segment stream
        |
        v
Stable geometry hash
        |
        +-- changed:
        |     rebuild spatial buckets
        |     delete cached fan VAOs/VBOs
        |     advance geometry version
        |
        +-- unchanged:
              preserve spatial buckets
              preserve cached fan VAOs/VBOs

Per light
        |
        v
Perceptual base ring schedule
        |
        v
Cache lookup by:
  geometry version + light position bits + radius bits + mask rotation bits + base sample count
        |
        +-- hit:
        |     bind cached VAO
        |     draw triangle fan
        |
        +-- miss:
              query spatial buckets inside light radius
              add endpoint rays for every local occluder segment
              ray-cast exact nearest intersection per angle
              upload fan once into a static VBO
              capture layout once into a VAO
              draw cached fan
```

The key distinction is that the hard-shadow boundary is still driven by occluder endpoint rays. The adaptive base ring only controls how finely an unobstructed circular light perimeter is tessellated. This allows low-impact lights to use 32 perimeter samples, mid-impact lights to use 64, and large or visually dominant lights to keep 96, without allowing sealed geometry to leak.

## GPU resource lifetime

The previous cache stage kept CPU-side fan vertices and reuploaded them on cache hits. That solved ray-casting cost but still spent driver time on buffer updates. The final stage stores each cached fan as a static VBO and captures its vertex layout in a VAO. A cache hit no longer calls buffer reallocation or vertex attribute setup.

Resource deletion is explicit:

- geometry hash changes clear the fan cache;
- LRU eviction deletes the evicted VAO/VBO pair;
- renderer shutdown clears the cache before the GL context is shut down.

This matters because a horror lighting pass can keep many static lights alive for long periods. A cache that is fast but leaks GPU objects would be unacceptable for long sessions.

## Benchmark methodology

I used a temporary .NET 10 Release harness outside the repository. It models the renderer's spatial buckets, angular bins, exact segment intersection, temporal cache key, and final base-sample scheduler. It measures the CPU-side visibility workload because that is the deterministic part available without an attached graphics profiler.

The scenarios are:

- `small-room`: 32 lights, 512 occluder segments;
- `station-corridor`: 96 lights, 2048 occluder segments;
- `stress-viewport`: 128 lights, 4096 occluder segments;
- `dense-overlap`: 64 lights, 4096 segments deliberately packed into overlapping light radii.

The measured modes are:

- `cold`: cache cleared before each frame, spatial buckets kept;
- `hot`: same lights and same geometry, all fans are cache hits;
- `moved`: lights shift slightly each frame, forcing rebuilds while keeping the spatial index.

## Results

| Scenario | Lights | Segments | Cold mean | Cold p95 | Hot mean | Hot p95 | Moved mean | Moved p95 | Cold rays | Moved rays | Moved intersections | Low | Medium | High |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| small-room | 32 | 512 | 7.4089 ms | 16.5559 ms | 0.0364 ms | 0.1034 ms | 4.0359 ms | 12.4031 ms | 7000 | 7028 | 13910 | 0 | 32 | 0 |
| station-corridor | 96 | 2048 | 7.9565 ms | 9.8046 ms | 0.0053 ms | 0.0068 ms | 4.9564 ms | 5.8649 ms | 27740 | 27656 | 59514 | 0 | 96 | 0 |
| stress-viewport | 128 | 4096 | 11.7230 ms | 12.1994 ms | 0.0011 ms | 0.0012 ms | 11.4205 ms | 12.3818 ms | 64486 | 64553 | 184250 | 0 | 0 | 128 |
| dense-overlap | 64 | 4096 | 174.5883 ms | 195.7576 ms | 0.0011 ms | 0.0012 ms | 177.9945 ms | 207.7262 ms | 371581 | 371578 | 11751781 | 0 | 0 | 64 |

## Interpretation

The hot-cache path is now effectively amortized. For unchanged lights and unchanged wall geometry, the CPU-side visibility stage falls to near-zero time in this harness. In the renderer, the remaining work is the actual draw call, light shader evaluation, blending, and fill-rate cost.

The small-room and corridor cases show the intended practical behavior: moving lights still rebuild exact fans, but spatial buckets prevent every light from scanning every segment. The scheduler classified those lights as medium-salience and used 64 base samples while preserving endpoint rays.

The stress-viewport case keeps all lights high-salience because every light is large or dominant. This is intentional: the scheduler should not visibly degrade the lights that players are most likely to inspect.

The dense-overlap case is the geometric worst case. When many lights overlap the same dense occluder field, exact visibility must test many local segment candidates. The final cache solves static overlap, but not constantly moving dense overlap. That behavior is correct for an exact solver and should be handled at content/layout level or with a later multi-frame budget scheduler if a scene deliberately creates that pathological case.

## Build and executable validation

The final branch was validated with:

- `dotnet build Robust.Client\Robust.Client.csproj --no-restore`
- `dotnet build Content.Client\Content.Client.csproj --no-restore`
- `dotnet restore Content.Server\Content.Server.csproj`
- `dotnet build Content.Server\Content.Server.csproj --no-restore`
- `dotnet test Robust.Client.Tests\Robust.Client.Tests.csproj --no-restore --logger "console;verbosity=minimal"`
- `bin\Content.Client\Content.Client.exe --help`
- `bin\Content.Server\Content.Server.exe --help`

The client and server builds completed with zero errors. Existing warnings remain in the broader codebase, including analyzer warnings, obsolete API usage, and package audit warnings in server dependencies. They are not introduced by this lighting pipeline change.

## Closure criteria

This stage closes the lighting session technically by addressing the major loose ends left after exact visibility became viable:

- hard-shadow correctness is preserved by endpoint-driven CPU visibility;
- static-light performance is no longer dominated by repeated CPU fan construction;
- hot-cache rendering no longer performs repeated fan VBO uploads;
- vertex attribute layout for cached fans is no longer rebuilt every draw;
- cache invalidation is conservative and tied to geometry/light state;
- GPU objects owned by the visibility cache have deterministic deletion paths;
- client and server executables were rebuilt and smoke-tested.

The field-of-view path remains semantically isolated from lighting because gameplay visibility and light visibility have different correctness contracts. The lighting pass now has its own exact visibility cache without changing gameplay FOV behavior, which avoids a rendering optimization becoming a gameplay regression.
