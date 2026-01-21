using Arcade.Core;
using Arcade.Visual;

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
    void SetCentralWidget(IWidget widget);
}

public class GuiService(ILayerView layer) : IGuiService
{
    IWidget? _centralWidget = null;

    public void SetCentralWidget(IWidget widget)
    {
        _centralWidget = widget;
    }

    public void FrameTick(IFrameTickService frameTickService)
    {
        _centralWidget?.Update(layer.Origin, (int)layer.Size.X, (int)layer.Size.Y);
        _centralWidget?.FrameTick(frameTickService);
    }

    public void Draw(IRenderer renderer)
    {
        _centralWidget?.Draw(renderer);
    }
}
