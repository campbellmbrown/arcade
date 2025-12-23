using Arcade.Input;
using Arcade.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Arcade.Gui;

public class PushButton : Widget, IClickable
{
    const int TEXTURE_CORNER_PIXELS = 3;

    readonly Texture2D _texture;
    readonly IWidget _widget;
    readonly List<Action> _actions = [];

    int _chunkWidth;
    int _chunkHeight;
    int _numHorizontalFullChunks;
    int _numVerticalFullChunks;
    int _partialVerticalChunkWidth;
    int _partialHorizontalChunkHeight;

    public PushButton(Texture2D texture, IWidget widget)
    {
        _texture = texture;
        _widget = widget;
    }

    public RectangleF ClickArea => new(Position, new Size2(Width, Height));

    public void AddAction(Action action)
    {
        _actions.Add(action);
    }

    public void OnClick()
    {
        foreach (Action action in _actions)
        {
            action();
        }
    }

    public override int GetContentWidth() => _widget.MeasureWidth();
    public override int GetContentHeight() => _widget.MeasureHeight();

    public override void Update(Vector2 position, int availableWidth, int availableHeight)
    {
        Width = GetContentWidth();
        Height = GetContentHeight();
        base.Update(position, availableWidth, availableHeight);
        _widget.Update(Position, Width, Height);

        int widthExcludingEnds = Width - 2 * TEXTURE_CORNER_PIXELS;
        int heightExcludingEnds = Height - 2 * TEXTURE_CORNER_PIXELS;

        _chunkWidth = _texture.Width - 2 * TEXTURE_CORNER_PIXELS;
        _numHorizontalFullChunks = widthExcludingEnds / _chunkWidth;
        _partialVerticalChunkWidth = widthExcludingEnds % _chunkWidth;

        _chunkHeight = _texture.Height - 2 * TEXTURE_CORNER_PIXELS;
        _numVerticalFullChunks = heightExcludingEnds / _chunkHeight;
        _partialHorizontalChunkHeight = heightExcludingEnds % _chunkHeight;
    }

    public override void Draw(IRenderer renderer)
    {
        renderer.SpriteBatch.DrawRectangle(new RectangleF(Position.X, Position.Y, Width, Height), Color.White, 1, 0.5f);
        Rectangle topLeftRect = new(0, 0, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS);
        Rectangle topRightRect = new(_texture.Width - TEXTURE_CORNER_PIXELS, 0, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS);
        Rectangle bottomLeftRect = new(0, _texture.Height - TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS);
        Rectangle bottomRightRect = new(_texture.Width - TEXTURE_CORNER_PIXELS, _texture.Height - TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS);
        Rectangle chunkRect = new(TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _chunkWidth, _chunkHeight);
        Rectangle topEdgeChunkRect = new(TEXTURE_CORNER_PIXELS, 0, _chunkWidth, TEXTURE_CORNER_PIXELS);
        Rectangle bottomEdgeChunkRect = new(TEXTURE_CORNER_PIXELS, _texture.Height - TEXTURE_CORNER_PIXELS, _chunkWidth, TEXTURE_CORNER_PIXELS);
        Rectangle partialHorizontalChunkRect = new(TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _chunkWidth, _partialHorizontalChunkHeight);
        Rectangle partialVerticalChunkRect = new(TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _partialVerticalChunkWidth, _chunkHeight);
        Rectangle partialRemainingChunkRect = new(TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _partialVerticalChunkWidth, _partialHorizontalChunkHeight);

        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X, Position.Y), topLeftRect, Color.White);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X + Width - TEXTURE_CORNER_PIXELS, Position.Y), topRightRect, Color.White);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X, Position.Y + Height - TEXTURE_CORNER_PIXELS), bottomLeftRect, Color.White);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X + Width - TEXTURE_CORNER_PIXELS, Position.Y + Height - TEXTURE_CORNER_PIXELS), bottomRightRect, Color.White);

        // Draw the middle chunks
        Vector2 chunkDrawPosition = new(Position.X + TEXTURE_CORNER_PIXELS, Position.Y + TEXTURE_CORNER_PIXELS);
        for (int yIdx = 0; yIdx < _numVerticalFullChunks; yIdx++)
        {
            for (int xIdx = 0; xIdx < _numHorizontalFullChunks; xIdx++)
            {
                renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, chunkRect, Color.White);
                chunkDrawPosition.X += _chunkWidth;
            }
            chunkDrawPosition.Y += _chunkHeight;
        }

        // Draw the top edge chunks
        chunkDrawPosition = new(Position.X + TEXTURE_CORNER_PIXELS, Position.Y);
        for (int xIdx = 0; xIdx < _numHorizontalFullChunks; xIdx++)
        {
            renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, topEdgeChunkRect, Color.White);
            chunkDrawPosition.X += _chunkWidth;
        }

        // Draw the bottom edge chunks
        chunkDrawPosition = new(Position.X + TEXTURE_CORNER_PIXELS, Position.Y + Height - TEXTURE_CORNER_PIXELS);
        for (int xIdx = 0; xIdx < _numHorizontalFullChunks; xIdx++)
        {
            renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, bottomEdgeChunkRect, Color.White);
            chunkDrawPosition.X += _chunkWidth;
        }

        // Draw the left edge chunks
        chunkDrawPosition = new(Position.X, Position.Y + TEXTURE_CORNER_PIXELS);
        for (int yIdx = 0; yIdx < _numVerticalFullChunks; yIdx++)
        {
            renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, new Rectangle(0, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _chunkHeight), Color.White);
            chunkDrawPosition.Y += _chunkHeight;
        }

        // Draw the right edge chunks
        chunkDrawPosition = new(Position.X + Width - TEXTURE_CORNER_PIXELS, Position.Y + TEXTURE_CORNER_PIXELS);
        for (int yIdx = 0; yIdx < _numVerticalFullChunks; yIdx++)
        {
            renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, new Rectangle(_texture.Width - TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _chunkHeight), Color.White);
            chunkDrawPosition.Y += _chunkHeight;
        }

        // Draw the partial horizontal chunks
        chunkDrawPosition = new(Position.X + TEXTURE_CORNER_PIXELS, Position.Y + TEXTURE_CORNER_PIXELS + _numVerticalFullChunks * _chunkHeight);
        for (int xIdx = 0; xIdx < _numHorizontalFullChunks; xIdx++)
        {
            renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, partialHorizontalChunkRect, Color.White);
            chunkDrawPosition.X += _chunkWidth;
        }

        // Draw the partial vertical chunks
        chunkDrawPosition = new(Position.X + TEXTURE_CORNER_PIXELS + _numHorizontalFullChunks * _chunkWidth, Position.Y + TEXTURE_CORNER_PIXELS);
        for (int yIdx = 0; yIdx < _numVerticalFullChunks; yIdx++)
        {
            renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, partialVerticalChunkRect, Color.White);
            chunkDrawPosition.Y += _chunkHeight;
        }

        // Draw the partial remaining chunk
        renderer.SpriteBatch.Draw(_texture, chunkDrawPosition, partialRemainingChunkRect, Color.White);

        // Draw the remaining chunk on the left edge
        Rectangle remainingLeftRect = new(0, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _partialHorizontalChunkHeight);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X, Position.Y + TEXTURE_CORNER_PIXELS + _numVerticalFullChunks * _chunkHeight), remainingLeftRect, Color.White);

        // Draw the remaining chunk on the right edge
        Rectangle remainingRightRect = new(_texture.Width - TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, TEXTURE_CORNER_PIXELS, _partialHorizontalChunkHeight);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X + Width - TEXTURE_CORNER_PIXELS, Position.Y + TEXTURE_CORNER_PIXELS + _numVerticalFullChunks * _chunkHeight), remainingRightRect, Color.White);

        // Draw the remaining chunk on the top edge
        Rectangle remainingTopRect = new(TEXTURE_CORNER_PIXELS, 0, _partialVerticalChunkWidth, TEXTURE_CORNER_PIXELS);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X + TEXTURE_CORNER_PIXELS + _numHorizontalFullChunks * _chunkWidth, Position.Y), remainingTopRect, Color.White);

        // Draw the remaining chunk on the bottom edge
        Rectangle remainingBottomRect = new(TEXTURE_CORNER_PIXELS, _texture.Height - TEXTURE_CORNER_PIXELS, _partialVerticalChunkWidth, TEXTURE_CORNER_PIXELS);
        renderer.SpriteBatch.Draw(_texture, new Vector2(Position.X + TEXTURE_CORNER_PIXELS + _numHorizontalFullChunks * _chunkWidth, Position.Y + Height - TEXTURE_CORNER_PIXELS), remainingBottomRect, Color.White);

        _widget.Draw(renderer);
        base.Draw(renderer);
    }
}
