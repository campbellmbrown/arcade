using Arcade.Utility;
using Arcade.World;

using NUnit.Framework;

namespace Arcade.Tests.Utility;

public class BitMaskTests
{
    [SetUp]
    public void Setup()
    {
    }

    const int CenterXIdx = 15;
    const int CenterYIdx = 20;

    static readonly object[] BitMask4TestCases =
    {
        new object[] { @". . .
                         .   .
                         . . .", 0 },
        new object[] { @". # .
                         .   .
                         . . .", 1 },
        new object[] { @". . .
                         .   #
                         . . .", 2 },
        new object[] { @". # .
                         .   #
                         . . .", 3 },
        new object[] { @". . .
                         .   .
                         . # .", 4 },
        new object[] { @". # .
                         .   .
                         . # .", 5 },
        new object[] { @". . .
                         .   #
                         . # .", 6 },
        new object[] { @". # .
                         .   #
                         . # .", 7 },
        new object[] { @". . .
                         #   .
                         . . .", 8 },
        new object[] { @". # .
                         #   .
                         . . .", 9 },
        new object[] { @". . .
                         #   #
                         . . .", 10 },
        new object[] { @". # .
                         #   #
                         . . .", 11 },
        new object[] { @". . .
                         #   .
                         . # .", 12 },
        new object[] { @". # .
                         #   .
                         . # .", 13 },
        new object[] { @". . .
                         #   #
                         . # .", 14 },
        new object[] { @". # .
                         #   #
                         . # .", 15 },
    };

    [TestCaseSource(nameof(BitMask4TestCases))]
    public void BitMask4(string encodedPattern, int expected)
    {
        // Given:
        var tiles = GetListFromEncodedPattern(encodedPattern);
        Tile tile = new(CenterXIdx, CenterYIdx);

        // When:
        var value = BitMask.FindValue(BitMaskType.Bits4, tiles, tile);

        // When/then:
        Assert.That(value, Is.EqualTo(expected));
    }

    static readonly object[] BitMask8TestCases =
    {
        new object[] { @". . .
                         .   .
                         . . .", 47 },
        new object[] { @"# . .
                         .   .
                         . . .", 47 },
        new object[] { @". # .
                         .   .
                         . . .", 1 },
        new object[] { @"# # .
                         .   .
                         . . .", 1 },
        new object[] { @". . #
                         .   .
                         . . .", 47 },
        new object[] { @"# . #
                         .   .
                         . . .", 47 },
        new object[] { @". # #
                         .   .
                         . . .", 1 },
        new object[] { @"# # #
                         .   .
                         . . .", 1 },
        new object[] { @". . .
                         #   .
                         . . .", 2 },
        new object[] { @"# . .
                         #   .
                         . . .", 2 },
        new object[] { @". # .
                         #   .
                         . . .", 3 },
        new object[] { @"# # .
                         #   .
                         . . .", 4 },
        new object[] { @". . #
                         #   .
                         . . .", 2 },
        new object[] { @"# . #
                         #   .
                         . . .", 2 },
        new object[] { @". # #
                         #   .
                         . . .", 3 },
        new object[] { @"# # #
                         #   .
                         . . .", 4 },
        new object[] { @". . .
                         .   #
                         . . .", 5 },
        new object[] { @"# . .
                         .   .
                         . . .", 47 },
        new object[] { @". # .
                         .   #
                         . . .", 6 },
        new object[] { @"# # .
                         .   #
                         . . .", 6 },
        new object[] { @". . #
                         .   #
                         . . .", 5 },
        new object[] { @"# . #
                         .   #
                         . . .", 5 },
        new object[] { @". # #
                         .   #
                         . . .", 7 },
        new object[] { @"# # #
                         .   #
                         . . .", 7 },
        new object[] { @". . .
                         #   #
                         . . .", 8 },
        new object[] { @"# . .
                         #   #
                         . . .", 8 },
        new object[] { @". # .
                         #   #
                         . . .", 9 },
        new object[] { @"# # .
                         #   #
                         . . .", 10 },
        new object[] { @". . #
                         #   #
                         . . .", 8 },
        new object[] { @"# . #
                         #   #
                         . . .", 8 },
        new object[] { @". # #
                         #   #
                         . . .", 11 },
        new object[] { @"# # #
                         #   #
                         . . .", 12 },
        new object[] { @". . .
                         .   .
                         # . .", 47 },
        new object[] { @"# . .
                         .   .
                         # . .", 47 },
        new object[] { @". # .
                         .   .
                         # . .", 1 },
        new object[] { @"# # .
                         .   .
                         # . .", 1 },
        new object[] { @". . #
                         .   .
                         # . .", 47 },
        new object[] { @"# . #
                         .   .
                         # . .", 47 },
        new object[] { @". # #
                         .   .
                         # . .", 1 },
        new object[] { @"# # #
                         .   .
                         # . .", 1 },
        new object[] { @". . .
                         #   .
                         # . .", 2 },
        new object[] { @"# . .
                         #   .
                         # . .", 2 },
        new object[] { @". # .
                         #   .
                         # . .", 3 },
        new object[] { @"# # .
                         #   .
                         # . .", 4 },
        new object[] { @". . #
                         #   .
                         # . .", 2 },
        new object[] { @"# . #
                         #   .
                         # . .", 2 },
        new object[] { @". # #
                         #   .
                         # . .", 3 },
        new object[] { @"# # #
                         #   .
                         # . .", 4 },
        new object[] { @". . .
                         .   #
                         # . .", 5 },
        new object[] { @"# . .
                         .   .
                         # . .", 47 },
        new object[] { @". # .
                         .   #
                         # . .", 6 },
        new object[] { @"# # .
                         .   #
                         # . .", 6 },
        new object[] { @". . #
                         .   #
                         # . .", 5 },
        new object[] { @"# . #
                         .   #
                         # . .", 5 },
        new object[] { @". # #
                         .   #
                         # . .", 7 },
        new object[] { @"# # #
                         .   #
                         # . .", 7 },
        new object[] { @". . .
                         #   #
                         # . .", 8 },
        new object[] { @"# . .
                         #   #
                         # . .", 8 },
        new object[] { @". # .
                         #   #
                         # . .", 9 },
        new object[] { @"# # .
                         #   #
                         # . .", 10 },
        new object[] { @". . #
                         #   #
                         # . .", 8 },
        new object[] { @"# . #
                         #   #
                         # . .", 8 },
        new object[] { @". # #
                         #   #
                         # . .", 11 },
        new object[] { @"# # #
                         #   #
                         # . .", 12 },
        new object[] { @". . .
                         .   .
                         . # .", 13 },
        new object[] { @"# . .
                         .   .
                         . # .", 13 },
        new object[] { @". # .
                         .   .
                         . # .", 14 },
        new object[] { @"# # .
                         .   .
                         . # .", 14 },
        new object[] { @". . #
                         .   .
                         . # .", 13 },
        new object[] { @"# . #
                         .   .
                         . # .", 13 },
        new object[] { @". # #
                         .   .
                         . # .", 14 },
        new object[] { @"# # #
                         .   .
                         . # .", 14 },
        new object[] { @". . .
                         #   .
                         . # .", 15 },
        new object[] { @"# . .
                         #   .
                         . # .", 15 },
        new object[] { @". # .
                         #   .
                         . # .", 16 },
        new object[] { @"# # .
                         #   .
                         . # .", 17 },
        new object[] { @". . #
                         #   .
                         . # .", 15 },
        new object[] { @"# . #
                         #   .
                         . # .", 15 },
        new object[] { @". # #
                         #   .
                         . # .", 16 },
        new object[] { @"# # #
                         #   .
                         . # .", 17 },
        new object[] { @". . .
                         .   #
                         . # .", 18 },
        new object[] { @"# . .
                         .   .
                         . # .", 13 },
        new object[] { @". # .
                         .   #
                         . # .", 19 },
        new object[] { @"# # .
                         .   #
                         . # .", 19 },
        new object[] { @". . #
                         .   #
                         . # .", 18 },
        new object[] { @"# . #
                         .   #
                         . # .", 18 },
        new object[] { @". # #
                         .   #
                         . # .", 20 },
        new object[] { @"# # #
                         .   #
                         . # .", 20 },
        new object[] { @". . .
                         #   #
                         . # .", 21 },
        new object[] { @"# . .
                         #   #
                         . # .", 21 },
        new object[] { @". # .
                         #   #
                         . # .", 22 },
        new object[] { @"# # .
                         #   #
                         . # .", 23 },
        new object[] { @". . #
                         #   #
                         . # .", 21 },
        new object[] { @"# . #
                         #   #
                         . # .", 21 },
        new object[] { @". # #
                         #   #
                         . # .", 24 },
        new object[] { @"# # #
                         #   #
                         . # .", 25 },
        new object[] { @". . .
                         .   .
                         # # .", 13 },
        new object[] { @"# . .
                         .   .
                         # # .", 13 },
        new object[] { @". # .
                         .   .
                         # # .", 14 },
        new object[] { @"# # .
                         .   .
                         # # .", 14 },
        new object[] { @". . #
                         .   .
                         # # .", 13 },
        new object[] { @"# . #
                         .   .
                         # # .", 13 },
        new object[] { @". # #
                         .   .
                         # # .", 14 },
        new object[] { @"# # #
                         .   .
                         # # .", 14 },
        new object[] { @". . .
                         #   .
                         # # .", 26 },
        new object[] { @"# . .
                         #   .
                         # # .", 26 },
        new object[] { @". # .
                         #   .
                         # # .", 27 },
        new object[] { @"# # .
                         #   .
                         # # .", 28 },
        new object[] { @". . #
                         #   .
                         # # .", 26 },
        new object[] { @"# . #
                         #   .
                         # # .", 26 },
        new object[] { @". # #
                         #   .
                         # # .", 27 },
        new object[] { @"# # #
                         #   .
                         # # .", 28 },
        new object[] { @". . .
                         .   #
                         # # .", 18 },
        new object[] { @"# . .
                         .   .
                         # # .", 13 },
        new object[] { @". # .
                         .   #
                         # # .", 19 },
        new object[] { @"# # .
                         .   #
                         # # .", 19 },
        new object[] { @". . #
                         .   #
                         # # .", 18 },
        new object[] { @"# . #
                         .   #
                         # # .", 18 },
        new object[] { @". # #
                         .   #
                         # # .", 20 },
        new object[] { @"# # #
                         .   #
                         # # .", 20 },
        new object[] { @". . .
                         #   #
                         # # .", 29 },
        new object[] { @"# . .
                         #   #
                         # # .", 29 },
        new object[] { @". # .
                         #   #
                         # # .", 30 },
        new object[] { @"# # .
                         #   #
                         # # .", 31 },
        new object[] { @". . #
                         #   #
                         # # .", 29 },
        new object[] { @"# . #
                         #   #
                         # # .", 29 },
        new object[] { @". # #
                         #   #
                         # # .", 32 },
        new object[] { @"# # #
                         #   #
                         # # .", 33 },
        new object[] { @". . .
                         .   .
                         . . #", 47 },
        new object[] { @"# . .
                         .   .
                         . . #", 47 },
        new object[] { @". # .
                         .   .
                         . . #", 1 },
        new object[] { @"# # .
                         .   .
                         . . #", 1 },
        new object[] { @". . #
                         .   .
                         . . #", 47 },
        new object[] { @"# . #
                         .   .
                         . . #", 47 },
        new object[] { @". # #
                         .   .
                         . . #", 1 },
        new object[] { @"# # #
                         .   .
                         . . #", 1 },
        new object[] { @". . .
                         #   .
                         . . #", 2 },
        new object[] { @"# . .
                         #   .
                         . . #", 2 },
        new object[] { @". # .
                         #   .
                         . . #", 3 },
        new object[] { @"# # .
                         #   .
                         . . #", 4 },
        new object[] { @". . #
                         #   .
                         . . #", 2 },
        new object[] { @"# . #
                         #   .
                         . . #", 2 },
        new object[] { @". # #
                         #   .
                         . . #", 3 },
        new object[] { @"# # #
                         #   .
                         . . #", 4 },
        new object[] { @". . .
                         .   #
                         . . #", 5 },
        new object[] { @"# . .
                         .   .
                         . . #", 47 },
        new object[] { @". # .
                         .   #
                         . . #", 6 },
        new object[] { @"# # .
                         .   #
                         . . #", 6 },
        new object[] { @". . #
                         .   #
                         . . #", 5 },
        new object[] { @"# . #
                         .   #
                         . . #", 5 },
        new object[] { @". # #
                         .   #
                         . . #", 7 },
        new object[] { @"# # #
                         .   #
                         . . #", 7 },
        new object[] { @". . .
                         #   #
                         . . #", 8 },
        new object[] { @"# . .
                         #   #
                         . . #", 8 },
        new object[] { @". # .
                         #   #
                         . . #", 9 },
        new object[] { @"# # .
                         #   #
                         . . #", 10 },
        new object[] { @". . #
                         #   #
                         . . #", 8 },
        new object[] { @"# . #
                         #   #
                         . . #", 8 },
        new object[] { @". # #
                         #   #
                         . . #", 11 },
        new object[] { @"# # #
                         #   #
                         . . #", 12 },
        new object[] { @". . .
                         .   .
                         # . #", 47 },
        new object[] { @"# . .
                         .   .
                         # . #", 47 },
        new object[] { @". # .
                         .   .
                         # . #", 1 },
        new object[] { @"# # .
                         .   .
                         # . #", 1 },
        new object[] { @". . #
                         .   .
                         # . #", 47 },
        new object[] { @"# . #
                         .   .
                         # . #", 47 },
        new object[] { @". # #
                         .   .
                         # . #", 1 },
        new object[] { @"# # #
                         .   .
                         # . #", 1 },
        new object[] { @". . .
                         #   .
                         # . #", 2 },
        new object[] { @"# . .
                         #   .
                         # . #", 2 },
        new object[] { @". # .
                         #   .
                         # . #", 3 },
        new object[] { @"# # .
                         #   .
                         # . #", 4 },
        new object[] { @". . #
                         #   .
                         # . #", 2 },
        new object[] { @"# . #
                         #   .
                         # . #", 2 },
        new object[] { @". # #
                         #   .
                         # . #", 3 },
        new object[] { @"# # #
                         #   .
                         # . #", 4 },
        new object[] { @". . .
                         .   #
                         # . #", 5 },
        new object[] { @"# . .
                         .   .
                         # . #", 47 },
        new object[] { @". # .
                         .   #
                         # . #", 6 },
        new object[] { @"# # .
                         .   #
                         # . #", 6 },
        new object[] { @". . #
                         .   #
                         # . #", 5 },
        new object[] { @"# . #
                         .   #
                         # . #", 5 },
        new object[] { @". # #
                         .   #
                         # . #", 7 },
        new object[] { @"# # #
                         .   #
                         # . #", 7 },
        new object[] { @". . .
                         #   #
                         # . #", 8 },
        new object[] { @"# . .
                         #   #
                         # . #", 8 },
        new object[] { @". # .
                         #   #
                         # . #", 9 },
        new object[] { @"# # .
                         #   #
                         # . #", 10 },
        new object[] { @". . #
                         #   #
                         # . #", 8 },
        new object[] { @"# . #
                         #   #
                         # . #", 8 },
        new object[] { @". # #
                         #   #
                         # . #", 11 },
        new object[] { @"# # #
                         #   #
                         # . #", 12 },
        new object[] { @". . .
                         .   .
                         . # #", 13 },
        new object[] { @"# . .
                         .   .
                         . # #", 13 },
        new object[] { @". # .
                         .   .
                         . # #", 14 },
        new object[] { @"# # .
                         .   .
                         . # #", 14 },
        new object[] { @". . #
                         .   .
                         . # #", 13 },
        new object[] { @"# . #
                         .   .
                         . # #", 13 },
        new object[] { @". # #
                         .   .
                         . # #", 14 },
        new object[] { @"# # #
                         .   .
                         . # #", 14 },
        new object[] { @". . .
                         #   .
                         . # #", 15 },
        new object[] { @"# . .
                         #   .
                         . # #", 15 },
        new object[] { @". # .
                         #   .
                         . # #", 16 },
        new object[] { @"# # .
                         #   .
                         . # #", 17 },
        new object[] { @". . #
                         #   .
                         . # #", 15 },
        new object[] { @"# . #
                         #   .
                         . # #", 15 },
        new object[] { @". # #
                         #   .
                         . # #", 16 },
        new object[] { @"# # #
                         #   .
                         . # #", 17 },
        new object[] { @". . .
                         .   #
                         . # #", 34 },
        new object[] { @"# . .
                         .   .
                         . # #", 13 },
        new object[] { @". # .
                         .   #
                         . # #", 35 },
        new object[] { @"# # .
                         .   #
                         . # #", 35 },
        new object[] { @". . #
                         .   #
                         . # #", 34 },
        new object[] { @"# . #
                         .   #
                         . # #", 34 },
        new object[] { @". # #
                         .   #
                         . # #", 36 },
        new object[] { @"# # #
                         .   #
                         . # #", 36 },
        new object[] { @". . .
                         #   #
                         . # #", 37 },
        new object[] { @"# . .
                         #   #
                         . # #", 37 },
        new object[] { @". # .
                         #   #
                         . # #", 38 },
        new object[] { @"# # .
                         #   #
                         . # #", 39 },
        new object[] { @". . #
                         #   #
                         . # #", 37 },
        new object[] { @"# . #
                         #   #
                         . # #", 37 },
        new object[] { @". # #
                         #   #
                         . # #", 40 },
        new object[] { @"# # #
                         #   #
                         . # #", 41 },
        new object[] { @". . .
                         .   .
                         # # #", 13 },
        new object[] { @"# . .
                         .   .
                         # # #", 13 },
        new object[] { @". # .
                         .   .
                         # # #", 14 },
        new object[] { @"# # .
                         .   .
                         # # #", 14 },
        new object[] { @". . #
                         .   .
                         # # #", 13 },
        new object[] { @"# . #
                         .   .
                         # # #", 13 },
        new object[] { @". # #
                         .   .
                         # # #", 14 },
        new object[] { @"# # #
                         .   .
                         # # #", 14 },
        new object[] { @". . .
                         #   .
                         # # #", 26 },
        new object[] { @"# . .
                         #   .
                         # # #", 26 },
        new object[] { @". # .
                         #   .
                         # # #", 27 },
        new object[] { @"# # .
                         #   .
                         # # #", 28 },
        new object[] { @". . #
                         #   .
                         # # #", 26 },
        new object[] { @"# . #
                         #   .
                         # # #", 26 },
        new object[] { @". # #
                         #   .
                         # # #", 27 },
        new object[] { @"# # #
                         #   .
                         # # #", 28 },
        new object[] { @". . .
                         .   #
                         # # #", 34 },
        new object[] { @"# . .
                         .   .
                         # # #", 13 },
        new object[] { @". # .
                         .   #
                         # # #", 35 },
        new object[] { @"# # .
                         .   #
                         # # #", 35 },
        new object[] { @". . #
                         .   #
                         # # #", 34 },
        new object[] { @"# . #
                         .   #
                         # # #", 34 },
        new object[] { @". # #
                         .   #
                         # # #", 36 },
        new object[] { @"# # #
                         .   #
                         # # #", 36 },
        new object[] { @". . .
                         #   #
                         # # #", 42 },
        new object[] { @"# . .
                         #   #
                         # # #", 42 },
        new object[] { @". # .
                         #   #
                         # # #", 43 },
        new object[] { @"# # .
                         #   #
                         # # #", 44 },
        new object[] { @". . #
                         #   #
                         # # #", 42 },
        new object[] { @"# . #
                         #   #
                         # # #", 42 },
        new object[] { @". # #
                         #   #
                         # # #", 45 },
        new object[] { @"# # #
                         #   #
                         # # #", 46 },
    };

    [TestCaseSource(nameof(BitMask8TestCases))]
    public void BitMask8(string encodedPattern, int expected)
    {
        // Given:
        var tiles = GetListFromEncodedPattern(encodedPattern);
        Tile tile = new(CenterXIdx, CenterYIdx);

        // When:
        var value = BitMask.FindValue(BitMaskType.Bits8, tiles, tile);

        // When/then:
        Assert.That(value, Is.EqualTo(expected));
    }

    static List<ITile> GetListFromEncodedPattern(string encodedPattern)
    {
        var withoutWhitespace = new string(encodedPattern.Where(c => !char.IsWhiteSpace(c)).ToArray());
        Assert.That(withoutWhitespace.Length, Is.EqualTo(8));

        List<ITile> fakeTiles = [];
        if (withoutWhitespace[0] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx - 1, CenterYIdx - 1));
        }
        if (withoutWhitespace[1] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx, CenterYIdx - 1));
        }
        if (withoutWhitespace[2] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx + 1, CenterYIdx - 1));
        }
        if (withoutWhitespace[3] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx - 1, CenterYIdx));
        }
        if (withoutWhitespace[4] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx + 1, CenterYIdx));
        }
        if (withoutWhitespace[5] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx - 1, CenterYIdx + 1));
        }
        if (withoutWhitespace[6] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx, CenterYIdx + 1));
        }
        if (withoutWhitespace[7] == '#')
        {
            fakeTiles.Add(new Tile(CenterXIdx + 1, CenterYIdx + 1));
        }

        return fakeTiles;
    }
}
