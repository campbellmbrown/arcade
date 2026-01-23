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
}
