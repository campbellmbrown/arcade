using Arcade.Core;
using Arcade.Visual;
using Microsoft.Xna.Framework;

namespace Arcade.Gui;

public enum GuiPlacement
{
    TopLeft,
    TopMiddle,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomMiddle,
    BottomRight,
}


public interface IGuiService : IVisual, IFrameTickable
{
    void AddWidget(IWidget widget, GuiPlacement placement = GuiPlacement.TopLeft);
}

public class GuiService(ILayerView layerView) : IGuiService
{
    readonly List<Tuple<IWidget, GuiPlacement>> _widgets = [];

    public void AddWidget(IWidget widget, GuiPlacement placement = GuiPlacement.TopLeft)
    {
        _widgets.Add(new Tuple<IWidget, GuiPlacement>(widget, placement));
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        foreach (var (widget, placement) in _widgets)
        {
            widget.Update(GetWidgetPosition(placement, widget));
            widget.FrameTick(frameTickService);
        }
    }

    public void Draw(IRenderer renderer)
    {
        foreach (var (widget, _) in _widgets)
        {
            widget.Draw(renderer);
        }
    }

    Vector2 GetWidgetPosition(GuiPlacement placement, IWidget widget)
    {
        var offset = placement switch
        {
            GuiPlacement.TopLeft => Vector2.Zero,
            GuiPlacement.TopMiddle => new Vector2((layerView.Size.X - widget.Width) / 2f, 0f),
            GuiPlacement.TopRight => new Vector2(layerView.Size.X - widget.Width, 0f),
            GuiPlacement.CenterLeft => new Vector2(0, (layerView.Size.Y - widget.Height) / 2f),
            GuiPlacement.Center => new Vector2(layerView.Size.X - widget.Width, layerView.Size.Y - widget.Height) / 2f,
            GuiPlacement.CenterRight => new Vector2(layerView.Size.X - widget.Width, (layerView.Size.Y - widget.Height) / 2f),
            GuiPlacement.BottomLeft => new Vector2(0, layerView.Size.Y - widget.Height),
            GuiPlacement.BottomMiddle => new Vector2((layerView.Size.X - widget.Width) / 2f, layerView.Size.Y - widget.Height),
            GuiPlacement.BottomRight => new Vector2(layerView.Size.X - widget.Width, layerView.Size.Y - widget.Height),
            _ => throw new ArgumentException("Grid placement not supported."),
        };
        return layerView.Origin + offset;
    }
}
