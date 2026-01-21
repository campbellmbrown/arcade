namespace Arcade.Gui;

public class ButtonGroup
{
    readonly List<ButtonBase> buttons = [];

    public bool RequireSelection { get; private set; } = false;

    /// <summary>
    /// Create a new button group.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="requireSelection"/> is true but no <paramref name="initialSelection"/> is provided.</exception>
    /// <param name="requireSelection">Whether at least one button must be selected at all times.</param>
    /// <param name="initialSelection">The button to be initially selected. This button will be added to the group.</param>
    public ButtonGroup(bool requireSelection = false, ButtonBase? initialSelection = null)
    {
        RequireSelection = requireSelection;
        if (RequireSelection && initialSelection == null)
        {
            throw new ArgumentException("Initial selection must be provided when RequireSelection is true.", nameof(initialSelection));
        }
        if (initialSelection != null)
        {
            Add(initialSelection);
            initialSelection.Check(raiseEvent: false);
        }
    }

    /// <summary>
    /// Add a button to the group.
    /// </summary>
    /// <remarks>
    /// The button's <see cref="ButtonBase.IsCheckable"/> property will be set to true.
    /// </remarks>
    /// <param name="button"></param>
    public void Add(ButtonBase button)
    {
        if (!buttons.Contains(button))
        {
            button.IsCheckable = true;
            button.CheckedChanged += isChecked => OnButtonCheckedChanged(button, isChecked);
            buttons.Add(button);
        }
    }

    void OnButtonCheckedChanged(ButtonBase changedButton, bool isChecked)
    {
        if (isChecked)
        {
            // Uncheck all other buttons
            foreach (var button in buttons)
            {
                if (button != changedButton && button.IsChecked)
                {
                    // Don't raise the event to avoid recursion
                    button.Uncheck(raiseEvent: false);
                }
            }
        }
        else if (RequireSelection)
        {
            // Revert the change to ensure at least one button remains selected
            // Don't raise the event to avoid recursion
            changedButton.Check(raiseEvent: false);
        }
    }
}
