namespace Content.Server._Nineveh.GameTicking.Components;

/// <summary>
/// Hard-caps a round to a fixed real-world duration.
/// </summary>
[RegisterComponent, Access(typeof(NinevehRoundTimerRuleSystem))]
public sealed partial class NinevehRoundTimerRuleComponent : Component
{
    /// <summary>
    /// How long the round is allowed to run before it is forcefully ended.
    /// </summary>
    [DataField("roundDuration", required: true)]
    public TimeSpan RoundDuration = TimeSpan.FromHours(3);

    /// <summary>
    /// The absolute game time when this rule started.
    /// </summary>
    [ViewVariables]
    public TimeSpan StartTime;

    /// <summary>
    /// The absolute game time when the round should end.
    /// </summary>
    [ViewVariables]
    public TimeSpan EndTime;

    /// <summary>
    /// Prevents trying to end the round more than once if multiple updates occur before teardown.
    /// </summary>
    [ViewVariables]
    public bool RoundEnding;
}
