# FEAR-AUDIO Performance Report

This report records the local validation pass for the psychoacoustic audio overhaul.
All measurements were taken on the development machine used for this branch.

## Machine Profile

| Device | Value |
| --- | --- |
| CPU | 11th Gen Intel(R) Core(TM) i5-1135G7 @ 2.40GHz |
| CPU topology | 4 physical cores, 8 logical processors |
| Memory | 8.38 GB installed physical memory |
| GPU | Intel(R) Iris(R) Xe Graphics |
| GPU driver | 31.0.101.5522 |

## Validation Results

| Check | Result | Wall time |
| --- | --- | --- |
| `dotnet build Robust.Client/Robust.Client.csproj --no-restore` | Passed, 0 errors | 8.67 s |
| `dotnet build Content.Shared/Content.Shared.csproj --no-restore` | Passed, 0 errors | 58.41 s |
| `dotnet build Content.Client/Content.Client.csproj --no-restore` | Passed, 0 errors | 41.99 s |
| `dotnet build Content.Server/Content.Server.csproj --no-restore` | Passed, 0 errors | 75.79 s |
| `dotnet test Robust.Shared.Tests/Robust.Shared.Tests.csproj --filter Psychoacoustic` | Passed | 1.18 s |
| `Content.Client.exe --help` | Started and exited normally | 0.128 s |
| `Content.Server.exe --help` | Started and exited normally | 0.130 s |

Incremental build validation after the executables were generated:

| Check | Result | Wall time |
| --- | --- | --- |
| `dotnet build Content.Client/Content.Client.csproj --no-restore --verbosity:minimal` | Passed, 0 errors | 12.75 s |
| `dotnet build Content.Server/Content.Server.csproj --no-restore --verbosity:minimal` | Passed, 0 errors | 13.08 s |

## Runtime Cost Model

The hot path added by this branch is source-local and scalar. It is evaluated once per active source update and is
bounded by `PsychoacousticAudioModel.MaxActiveVoices = 128`.

Per active source, the model performs:

- One inverse-square/log-compressed attenuation calculation.
- One material-weighted occlusion scalar.
- One profile switch for pitch/gain/priority.
- No heap allocation.
- No file I/O.
- No lock acquisition.

The asymptotic cost remains `O(active voices)`. The voice allocator caps active work at 128 voices and steals the
lowest-priority source when the cap is saturated. Priority is ordered toward player-critical and threat-critical
events before ambience and music, matching the horror mix policy.

## Psychoacoustic Safety Bounds

The model intentionally clamps all perceptual modifiers:

- Distance gain is clamped to `[0, 2]` before source profile gain, then final gain is clamped to `[0, 2.5]`.
- Pitch is clamped to `[0.5, 2]`.
- Occlusion is clamped to `[0, 8]`.
- Priority is clamped to `[0, 1]`.
- FearLayer infrasound carrier gain is authored at subliminal gain space through the shared dB-to-gain conversion.

This prevents runaway gain, pitch explosions, and pathological priority escalation during dense scenes.

## Scientific Design Notes

The branch does not attempt to synthesize unsafe sound pressure levels. Instead, it exposes bounded DSP intent as
engine parameters: nonlinear vocalization, infrasonic carrier routing, roughness stingers, looming threat cues,
whisper occlusion, breath/cough intimacy, suppressed weapon coloration, ballistic crack emphasis, structural
transmission, vent resonance, and procedural music tension.

The goal is reproducible horror bias rather than uncontrolled amplitude. The perceptual hooks are deterministic,
serializable, network-safe, and cheap enough to run in the existing audio update cadence.

