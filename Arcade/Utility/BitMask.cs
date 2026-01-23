using Arcade.World;

namespace Arcade.Utility;

public enum BitMaskType
{
    Bits4,
    Bits8,
}

public static class BitMask
{
    static readonly Dictionary<int, int> _8BitRemap = new()
    {
        { 2, 1 },
        { 8, 2 },
        { 10, 3 },
        { 11, 4 },
        { 16, 5 },
        { 18, 6 },
        { 22, 7 },
        { 24, 8 },
        { 26, 9 },
        { 27, 10 },
        { 30, 11 },
        { 31, 12 },
        { 64, 13 },
        { 66, 14 },
        { 72, 15 },
        { 74, 16 },
        { 75, 17 },
        { 80, 18 },
        { 82, 19 },
        { 86, 20 },
        { 88, 21 },
        { 90, 22 },
        { 91, 23 },
        { 94, 24 },
        { 95, 25 },
        { 104, 26 },
        { 106, 27 },
        { 107, 28 },
        { 120, 29 },
        { 122, 30 },
        { 123, 31 },
        { 126, 32 },
        { 127, 33 },
        { 208, 34 },
        { 210, 35 },
        { 214, 36 },
        { 216, 37 },
        { 218, 38 },
        { 219, 39 },
        { 222, 40 },
        { 223, 41 },
        { 248, 42 },
        { 250, 43 },
        { 251, 44 },
        { 254, 45 },
        { 255, 46 },
        { 0, 47 }
    };

    public static int FindValue(BitMaskType bitMaskType, IEnumerable<ITile> tiles, ITile tile)
    {
        return bitMaskType switch
        {
            BitMaskType.Bits4 => FindValueWith4Bits(tiles, tile),
            BitMaskType.Bits8 => FindValueWith8Bits(tiles, tile),
            _ => 0,
        };
    }

    /// <summary>
    /// Finds the value of the Tile, considering only the directly connected 4 tiles.
    /// </summary>
    /// <remarks>
    /// The Tiles have the following values:
    /// <code>
    ///     [1]
    /// [8] [ ] [2]
    ///     [4]
    /// </code>
    /// If a neighboring tile is present, it contributes to the value. Therefore, there are 16 unique textures.
    /// </remarks>
    static int FindValueWith4Bits(IEnumerable<ITile> tiles, ITile tile)
    {
        bool above = tiles.Any(gs => gs.XIdx == tile.XIdx && gs.YIdx == tile.YIdx - 1);
        bool right = tiles.Any(gs => gs.XIdx == tile.XIdx + 1 && gs.YIdx == tile.YIdx);
        bool below = tiles.Any(gs => gs.XIdx == tile.XIdx && gs.YIdx == tile.YIdx + 1);
        bool left = tiles.Any(gs => gs.XIdx == tile.XIdx - 1 && gs.YIdx == tile.YIdx);
        int val = above ? 1 : 0;
        val += right ? 2 : 0;
        val += below ? 4 : 0;
        val += left ? 8 : 0;
        return val;
    }

    /// <summary>
    /// Finds the value of the Tile, considering all surrounding 8 tiles.
    /// </summary>
    /// <remarks>
    /// The Tiles have the following values:
    /// <code>
    /// [ 1 ] [ 2 ] [ 4 ]
    /// [ 8 ] [   ] [ 16]
    /// [ 32] [ 64] [128]
    /// </code>
    /// There is something else to consider though: When a corner is present, without ANY of it's horizontal or vertical
    /// neighbors, it doesn't contribute towards the value. E.g. if There was the top-left (1), top-middle (2), and
    /// middle-right (16), then this would look identical to a value of 18 (2 + 16). So the 1 isn't counted. Therefore,
    /// there are only 47 unique textures. A map is used to convert the large numbers into this range.
    /// </remarks>
    private static int FindValueWith8Bits(IEnumerable<ITile> tiles, ITile tile)
    {
        bool above = tiles.Any(gs => gs.XIdx == tile.XIdx && gs.YIdx == tile.YIdx - 1);
        bool right = tiles.Any(gs => gs.XIdx == tile.XIdx + 1 && gs.YIdx == tile.YIdx);
        bool below = tiles.Any(gs => gs.XIdx == tile.XIdx && gs.YIdx == tile.YIdx + 1);
        bool left = tiles.Any(gs => gs.XIdx == tile.XIdx - 1 && gs.YIdx == tile.YIdx);
        bool aboveLeft = tiles.Any(gs => gs.XIdx == tile.XIdx - 1 && gs.YIdx == tile.YIdx - 1);
        bool aboveRight = tiles.Any(gs => gs.XIdx == tile.XIdx + 1 && gs.YIdx == tile.YIdx - 1);
        bool belowLeft = tiles.Any(gs => gs.XIdx == tile.XIdx - 1 && gs.YIdx == tile.YIdx + 1);
        bool belowRight = tiles.Any(gs => gs.XIdx == tile.XIdx + 1 && gs.YIdx == tile.YIdx + 1);

        int val = (aboveLeft && above && left) ? 1 : 0;
        val += above ? 2 : 0;
        val += (aboveRight && above && right) ? 4 : 0;
        val += left ? 8 : 0;
        val += right ? 16 : 0;
        val += (belowLeft && below && left) ? 32 : 0;
        val += below ? 64 : 0;
        val += (belowRight && below && right) ? 128 : 0;

        return _8BitRemap[val];
    }
}
