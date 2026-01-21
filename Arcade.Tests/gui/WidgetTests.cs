using Arcade.Gui;

using Microsoft.Xna.Framework;

using NUnit.Framework;

namespace Arcade.Tests.Gui;

class WidgetImplementation : Widget
{
    public WidgetImplementation(int width = 0, int height = 0)
    {
        Width = width;
        Height = height;
    }
}

public class WidgetTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(Alignment.Left | Alignment.Right)]
    [TestCase(Alignment.Left | Alignment.HCenter)]
    [TestCase(Alignment.Left | Alignment.HStretch)]
    [TestCase(Alignment.Right | Alignment.HStretch)]
    [TestCase(Alignment.Right | Alignment.HCenter)]
    [TestCase(Alignment.Top | Alignment.Bottom)]
    [TestCase(Alignment.Top | Alignment.VCenter)]
    [TestCase(Alignment.Top | Alignment.VStretch)]
    [TestCase(Alignment.Bottom | Alignment.VStretch)]
    [TestCase(Alignment.Bottom | Alignment.VCenter)]
    [TestCase(Alignment.Center | Alignment.Left)]
    [TestCase(Alignment.Center | Alignment.Right)]
    [TestCase(Alignment.Center | Alignment.HStretch)]
    [TestCase(Alignment.Center | Alignment.Top)]
    [TestCase(Alignment.Center | Alignment.Bottom)]
    [TestCase(Alignment.Center | Alignment.VStretch)]
    [TestCase(Alignment.Center | Alignment.Stretch)]
    [TestCase(Alignment.Stretch | Alignment.Left)]
    [TestCase(Alignment.Stretch | Alignment.Right)]
    [TestCase(Alignment.Stretch | Alignment.HCenter)]
    [TestCase(Alignment.Stretch | Alignment.Top)]
    [TestCase(Alignment.Stretch | Alignment.Bottom)]
    [TestCase(Alignment.Stretch | Alignment.VCenter)]
    public void Update_InvalidAlignment_ThrowsArgumentException(Alignment alignment)
    {
        // Arrange
        WidgetImplementation widget;

        // Act
        void Act() => widget = new() { Alignment = alignment };

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    const int AVAILABLE_WIDTH = 10;
    const int AVAILABLE_HEIGHT = 20;
    const int WIDTH = 6;
    const int HEIGHT = 12;

    [TestCase(Alignment.Top | Alignment.Left, 0, 0)]
    [TestCase(Alignment.Top | Alignment.Right, 4, 0)]
    [TestCase(Alignment.Top | Alignment.HCenter, 2, 0)]
    [TestCase(Alignment.Top | Alignment.HStretch, 0, 0)]
    [TestCase(Alignment.Bottom | Alignment.Left, 0, 8)]
    [TestCase(Alignment.Bottom | Alignment.Right, 4, 8)]
    [TestCase(Alignment.Bottom | Alignment.HCenter, 2, 8)]
    [TestCase(Alignment.Bottom | Alignment.HStretch, 0, 8)]
    [TestCase(Alignment.VCenter | Alignment.Left, 0, 4)]
    [TestCase(Alignment.VCenter | Alignment.Right, 4, 4)]
    [TestCase(Alignment.VCenter | Alignment.HCenter, 2, 4)]
    [TestCase(Alignment.VCenter | Alignment.HStretch, 0, 4)]
    [TestCase(Alignment.VStretch | Alignment.Left, 0, 0)]
    [TestCase(Alignment.VStretch | Alignment.Right, 4, 0)]
    [TestCase(Alignment.VStretch | Alignment.HCenter, 2, 0)]
    [TestCase(Alignment.VStretch | Alignment.HStretch, 0, 0)]
    [TestCase(Alignment.Top, 0, 0)] // Becomes Top | Left
    [TestCase(Alignment.Bottom, 0, 8)] // Becomes Bottom | Left
    [TestCase(Alignment.VCenter, 0, 4)] // Becomes VCenter | Left
    [TestCase(Alignment.VStretch, 0, 0)] // Becomes VStretch | Left
    [TestCase(Alignment.Left, 0, 4)] // Becomes VCenter | Left
    [TestCase(Alignment.Right, 4, 4)] // Becomes VCenter | Right
    [TestCase(Alignment.HCenter, 2, 4)] // Becomes VCenter | HCenter
    [TestCase(Alignment.HStretch, 0, 4)] // Becomes VCenter | VStretch
    [TestCase(Alignment.Center, 2, 4)] // Implicit VCenter | HCenter
    [TestCase(Alignment.Stretch, 0, 0)] // Implicit VStretch | HStretch
    [TestCase(Alignment.None, 0, 4)] // Implicit VCenter | Left
    public void Update_WidthAndHeightAreSetBasedOnAlignment(Alignment alignment, int expectedX, int expectedY)
    {
        // Arrange
        WidgetImplementation widget = new(WIDTH, HEIGHT) { Alignment = alignment };
        Vector2 position = Vector2.Zero;

        // Act
        widget.Update(position, AVAILABLE_WIDTH, AVAILABLE_HEIGHT);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(widget.Position.X, Is.EqualTo(expectedX));
            Assert.That(widget.Position.Y, Is.EqualTo(expectedY));
        });
    }
}
