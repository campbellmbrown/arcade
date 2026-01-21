namespace Arcade.Gui;

public class ButtonGroup
{
    readonly List<IButton> buttons = [];

    public bool RequireSelection { get; private set; } = false;

    bool _transitioning = false;

    /// <summary>
    /// Create a new button group.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="requireSelection"/> is true but no <paramref name="initialSelection"/> is provided.</exception>
    /// <param name="requireSelection">Whether at least one button must be selected at all times.</param>
    /// <param name="initialSelection">The button to be initially selected. This button will be added to the group.</param>
    public ButtonGroup(bool requireSelection = false, IButton? initialSelection = null)
    {
        RequireSelection = requireSelection;
        if (RequireSelection && initialSelection == null)
        {
            throw new ArgumentException("Initial selection must be provided when RequireSelection is true.", nameof(initialSelection));
        }
        if (initialSelection != null)
        {
            Add(initialSelection);
            initialSelection.IsChecked = true;
        }
    }

    /// <summary>
    /// Add a button to the group.
    /// </summary>
    /// <remarks>
    /// The button's <see cref="IButton.IsCheckable"/> property will be set to true.
    /// </remarks>
    /// <param name="button"></param>
    public void Add(IButton button)
    {
        if (buttons.Contains(button))
        {
            return;
        }

        // If the new button is checked and any other button is already checked, uncheck the others
        if (button.IsChecked)
        {
            _transitioning = true;
            try
            {
                foreach (var other in buttons)
                {
                    if (other.IsChecked)
                    {
                        other.IsChecked = false;
                    }
                }
            }
            finally
            {
                _transitioning = false;
            }
        }

        button.IsCheckable = true;
        buttons.Add(button);
        button.CheckedChanged += isChecked => OnButtonCheckedChanged(button, isChecked);
    }

    void OnButtonCheckedChanged(IButton changedButton, bool isChecked)
    {
        if (_transitioning)
        {
            // The change was triggered by me, ignore to avoid recursion
            return;
        }

        _transitioning = true;
        try
        {
            if (isChecked)
            {
                // Uncheck all other buttons
                foreach (var button in buttons)
                {
                    if (button != changedButton && button.IsChecked)
                    {
                        button.IsChecked = false;
                    }
                }
            }
            else if (RequireSelection)
            {
                // Revert the change to ensure at least one button remains selected
                changedButton.IsChecked = true;
            }
        }
        finally
        {
            _transitioning = false;
        }
    }
}
