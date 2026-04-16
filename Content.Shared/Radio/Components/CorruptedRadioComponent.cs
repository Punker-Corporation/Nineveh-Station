using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio;

[RegisterComponent, NetworkedComponent]
public sealed partial class CorruptedRadioComponent : Component
{
    [DataField("corruptionChance")]
    public float CorruptionChance = 0.70f;

    [DataField("spoofChance")]
    public float SpoofChance = 0.25f;

    [DataField("deadAirChance")]
    public float DeadAirChance = 0.18f;

    [DataField("whisperChance")]
    public float WhisperChance = 0.08f;

    [DataField("whisperMessages")]
    public List<string> WhisperMessages = new()
    {
        "...v...ocê...",
        "...me...ou...ve...",
        "...atrás...",
        "...não...",
        "...s...o...z...i...n...h...o...",
        "...cuidado...",
        "...f...r...i...o...",
        "...peso...",
        "...v...a...z...i...o...",
        "...eles...",
        "...es...tá...tica...",
        "...por quê?...",
        "...silên...cio...",
        "...olhos...",
        "...mentira...",
        "...voz...",
        "...nunca...",
        "...morte...",
        "...a...ju...da...",
        "...sem...pre..."
    };

    [DataField("spoofNames")]
    public List<string> SpoofNames = new()
    {
        "???",
        "Desconhecido",
        "Sinal Fraco",
        "Estática",
        "Ninguém",
        "000-0000",
        "Vazio",
        "...",
        "Erro",
        "Desconexão"
    };

    [DataField("corruptionReplacements")]
    public string CorruptionReplacements = "!@#$%&*_?.";

    [DataField("corruptionIntensity")]
    public float CorruptionIntensity = 0.35f;

    [DataField("infrasoundChance")]
    public float InfrasoundChance = 0.03f;

    [DataField("semanticSatiationThreshold")]
    public int SemanticSatiationThreshold = 2;

    [DataField("silenceChance")]
    public float SilenceChance = 0.05f;

    [DataField("jamaisVuChance")]
    public float JamaisVuChance = 0.10f;
}
