namespace Arcade.Core;

public interface IFrameTickable
{
    void FrameTick(IFrameTickService frameTickService);
}
