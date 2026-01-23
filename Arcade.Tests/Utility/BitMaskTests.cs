using System.Collections.Generic;
using System.Linq;

using Arcade.Utility;
using Arcade.World;

using Moq;

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
        var fakeTiles = GetListFromEncodedPattern(encodedPattern);
        Mock<ITile> centerMock = new();
        centerMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx);
        centerMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx);

        // When:
        var value = BitMask.FindValue(BitMask.BitMaskType.Bits4, fakeTiles, centerMock.Object);

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
        var fakeTiles = GetListFromEncodedPattern(encodedPattern);
        Mock<ITile> centerMock = new();
        centerMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx);
        centerMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx);

        // When:
        var value = BitMask.FindValue(BitMask.BitMaskType.Bits8, fakeTiles, centerMock.Object);

        // When/then:
        Assert.That(value, Is.EqualTo(expected));
    }

    static List<ITile> GetListFromEncodedPattern(string encodedPattern)
    {
        var withoutWhitespace = new string(encodedPattern.Where(c => !char.IsWhiteSpace(c)).ToArray());
        Assert.That(withoutWhitespace.Length, Is.EqualTo(8));

        List<ITile> fakeTiles = new();
        if (withoutWhitespace[0] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx - 1);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx - 1);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[1] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx - 1);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[2] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx + 1);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx - 1);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[3] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx - 1);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[4] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx + 1);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[5] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx - 1);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx + 1);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[6] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx + 1);
            fakeTiles.Add(gtileMock.Object);
        }
        if (withoutWhitespace[7] == '#')
        {
            Mock<ITile> gtileMock = new();
            gtileMock.Setup(gtile => gtile.XIdx).Returns(CenterXIdx + 1);
            gtileMock.Setup(gtile => gtile.YIdx).Returns(CenterYIdx + 1);
            fakeTiles.Add(gtileMock.Object);
        }

        return fakeTiles;
    }
}
