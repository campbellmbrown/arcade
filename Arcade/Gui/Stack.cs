using Arcade.Core;
using Arcade.Visual;

namespace Arcade.Gui;

public abstract class Stack : Widget
{
    public int Spacing { get; init; } = 0;
    public int Margin { get; init; } = 0;

    protected List<IWidget> Widgets { get; } = [];

    public void AddWidget(IWidget widget)
    {
        Widgets.Add(widget);
    }

    public override void FrameTick(IFrameTickService frameTickService)
    {
        base.FrameTick(frameTickService);
        foreach (var widget in Widgets)
        {
            widget.FrameTick(frameTickService);
        }
    }

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);
        foreach (var widget in Widgets)
        {
            widget.Draw(renderer);
        }
    }
}
