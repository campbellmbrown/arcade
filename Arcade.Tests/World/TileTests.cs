using System.Security.Cryptography.X509Certificates;

using Arcade.World;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Arcade.Tests.World;

public class TileTests
{
    [Test]
    public void PositionCalculatedFromIndices()
    {
        // Given
        var tile = new Tile(3, 5);

        // Then
        Assert.That(tile.Position, Is.EqualTo(new Vector2(66, 110)));
    }

    [Test]
    public void CenterCalculatedFromPosition()
    {
        // Given
        var tile = new Tile(3, 5);

        // Then
        Assert.That(tile.Center, Is.EqualTo(new Vector2(77, 121)));
    }

    [TestCase(2, 4, 2, 4, true)]
    [TestCase(2, 4, 3, 4, false)]
    public void IsSamePosition(int x1, int y1, int x2, int y2, bool expected)
    {
        // Given
        var tile1 = new Tile(x1, y1);
        var tile2 = new Tile(x2, y2);

        // Then
        Assert.That(tile1.IsSamePosition(tile2), Is.EqualTo(expected));
        Assert.That(tile1.IsSamePosition(x2, y2), Is.EqualTo(expected));
    }
}
