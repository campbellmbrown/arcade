using Microsoft.Xna.Framework;

namespace Arcade.World;

public interface ITile
{
    int XIdx { get; set; }
    int YIdx { get; set; }
    Vector2 Position { get; }
    Vector2 Center { get; }

    bool IsSamePosition(ITile other);
    bool IsSamePosition(int otherXIdx, int otherYIdx);
}

public class Tile(int xIdx, int yIdx) : ITile
{
    public const int Size = 22;

    public int XIdx { get; set; } = xIdx;
    public int YIdx { get; set; } = yIdx;

    public Vector2 Position => Size * new Vector2(XIdx, YIdx);
    public Vector2 Center => Position + new Vector2(Size / 2f);

    public bool IsSamePosition(ITile other)
    {
        return (XIdx == other.XIdx) && (YIdx == other.YIdx);
    }

    public bool IsSamePosition(int otherXIdx, int otherYIdx)
    {
        return (XIdx == otherXIdx) && (YIdx == otherYIdx);
    }
}
