namespace Arcade.Input;

// TODO: improve documentation

public class InputEvent
{
    /// <summary>
    /// Indicates whether the hover event was consumed by any hoverable.
    /// </summary>
    /// <remarks>
    /// This is the lowest priority consumption flag. If either <see cref="ClickHoverConsumed"/> or
    /// <see cref="ScrollHoverConsumed"/> is true, this should also be true.
    /// </remarks>
    /// <value>True if the hover event was consumed, false otherwise.</value>
    public bool HoverConsumed { get; set; } = false;

    /// <summary>
    /// Indicates whether the left click event was consumed.
    /// </summary>
    /// <value>True if the left click event was consumed, false otherwise.</value>
    public bool LeftClickConsumed { get; set; } = false;

    /// <summary>
    /// Indicates whether the scroll event was consumed.
    /// </summary>
    /// <value>True if the scroll event was consumed, false otherwise.</value>
    public bool ScrollConsumed { get; set; } = false;
}
