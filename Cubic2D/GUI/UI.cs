using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Cubic2D.Render;
using Cubic2D.Utilities;

namespace Cubic2D.GUI;

public static partial class UI
{
    private static Rectangle[] _lastElementPositions;
    private static List<Rectangle> _elementPositions;
    private static List<(Rectangle, Color, int)> _rectangles;
    private static List<(string text, uint size, Vector2 pos, Color color, bool centerOrigin, bool ignoreParams, int id)> _texts;
    private static List<char> _charBuffer;
    private static int _hoveringID;
    private static int _currentID;
    private static Point _textCursorPos;

    private static bool _mouseButtonHeld;
    private static bool _clicked;

    private static Size _framebufferSize;

    public static UITheme Theme;

    static UI()
    {
        _rectangles = new List<(Rectangle, Color, int)>();
        _texts = new List<(string, uint, Vector2, Color, bool, bool, int)>();
        _elementPositions = new List<Rectangle>();
        Theme = new UITheme();
        _charBuffer = new List<char>();
        _textCursorPos = Point.Empty;
        Input.TextInput += TextEntered;
    }

    private static void TextEntered(char character)
    {
        _charBuffer.Add(character);
    }

    private static bool MouseHovering(Rectangle pos)
    {
        Vector2 mPos = Input.MousePosition;
        return pos.Contains(new Point((int) mPos.X, (int) mPos.Y)) && _currentID + 1 == _hoveringID;
    }

    private static bool ElementClicked(Rectangle pos)
    {
        Vector2 mPos = Input.MousePosition;
        return _clicked && pos.Contains(new Point((int) mPos.X, (int) mPos.Y)) && _currentID == _hoveringID;
    }

    private static void AddElement(Rectangle pos, bool captureMouse = true)
    {
        if (captureMouse)
            _elementPositions.Add(pos);
        _currentID++;
    }

    private static void CalculatePos(Anchor anchor, ref Rectangle rect)
    {
        Vector2 origin;
        
        switch (anchor)
        {
            case Anchor.TopLeft:
                origin = Vector2.Zero;
                break;
            case Anchor.TopCenter:
                rect.X += _framebufferSize.Width / 2;
                origin = new Vector2(rect.Size.Width / 2, 0);
                break;
            case Anchor.TopRight:
                rect.X += _framebufferSize.Width;
                origin = new Vector2(rect.Size.Width, 0);
                break;
            case Anchor.CenterLeft:
                rect.Y += _framebufferSize.Height / 2;
                origin = new Vector2(0, rect.Size.Height / 2);
                break;
            case Anchor.Center:
                rect.X += _framebufferSize.Width / 2;
                rect.Y += _framebufferSize.Height / 2;
                origin = rect.Size.ToVector2() / 2;
                break;
            case Anchor.CenterRight:
                rect.X += _framebufferSize.Width;
                rect.Y += _framebufferSize.Height / 2;
                origin = new Vector2(rect.Size.Width, rect.Size.Height / 2);
                break;
            case Anchor.BottomLeft:
                rect.Y += _framebufferSize.Height;
                origin = new Vector2(0, rect.Size.Height);
                break;
            case Anchor.BottomCenter:
                rect.X += _framebufferSize.Width / 2;
                rect.Y += _framebufferSize.Height;
                origin = new Vector2(rect.Size.Width / 2, rect.Size.Height);
                break;
            case Anchor.BottomRight:
                rect.X += _framebufferSize.Width;
                rect.Y += _framebufferSize.Height;
                origin = rect.Size.ToVector2();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
        }

        rect.X -= (int) origin.X;
        rect.Y -= (int) origin.Y;
    }

    private static bool IsFocused(int id)
    {
        // TODO
        return true;
    }

    internal static void Update()
    {
        _lastElementPositions = _elementPositions.ToArray();
        _rectangles.Clear();
        _texts.Clear();
        _elementPositions.Clear();
        _hoveringID = -1;
        _currentID = -1;
        _clicked = false;

        Vector2 mousePos = Input.MousePosition;
        for (int i = _lastElementPositions.Length - 1; i >= 0; i--)
        {
            if (!_lastElementPositions[i].Contains(new Point((int) mousePos.X, (int) mousePos.Y))) continue;
            _hoveringID = i;
            break;
        }

        if (Input.MouseButtonDown(MouseButtons.Left))
            _mouseButtonHeld = true;
        if (Input.MouseButtonReleased(MouseButtons.Left) && _mouseButtonHeld)
        {
            _clicked = true;
            _mouseButtonHeld = false;
        }
    }

    internal static void Draw(SpriteRenderer renderer)
    {
        // Update the framebuffer size per frame to allow the anchoring system to work.
        _framebufferSize = renderer.FramebufferSize;
        
        // I'd rather not do this in draw but I have to due to the way the updating works
        _charBuffer.Clear();
        
        // UI is drawn on a different spritebatch, and always drawn on top.
        renderer.Begin();
        
        // Explanation of what's going on below
        // ----------------------------------------
        // It first loops through all the rectangles, and draws those. Then text, and draws those too.
        // However one thing you should notice is that they are drawn separately. All rectangles are drawn before any
        // text is drawn. Okay, no big deal. "But what about if the elements overlap!" I hear you not asking.
        // Good question. You see, this presents a problem - since text is drawn after all rectangles are drawn, this
        // will actually cause any text that should be hidden by an overlapping rectangle will be drawn on top of it
        // instead. Bummer.
        // Fortunately, this is where my new depth sorting system comes in! Since all elements here are drawn in the
        // same batch session, we can use the depth sorting algorithm to determine what gets drawn where.
        // Each tuple here is assigned an "ID". Every element has a unique ID. Above, this is used to determine which
        // element the mouse is hovering over. But, we use it here to determine depth.
        // We actually use the ID of the item as its depth (although we negate it since we want each element drawing
        // in front of the last one). Now, since the sprite renderer sorts by depth, this means that it will automagically
        // sort all rectangles and text into the correct order, meaning it displays correctly! It uses a few more draw
        // calls, which is a shame, but modern GPUs can handle thousands of them, so it's really not a concern.

        foreach ((Rectangle, Color, int) cRect in _rectangles)
        {
            Rectangle rect = cRect.Item1;
            Color col = cRect.Item2;
            renderer.Draw(Texture2D.Blank, new Vector2(rect.X, rect.Y), null, col, 0, Vector2.Zero,
                new Vector2(rect.Width, rect.Height), SpriteFlipMode.None, -cRect.Item3);
        }

        foreach ((string text, uint size, Vector2 pos, Color color, bool centerOrigin, bool ignoreParams, int id) in _texts)
        {
            Vector2 origin = Vector2.Zero;
            if (centerOrigin)
                origin = Theme.Font.MeasureString(size, text).ToVector2() / 2;
            origin.X = (int) origin.X;
            origin.Y = (int) origin.Y;
            Theme.Font.Draw(renderer, size, text, pos, color, 0, origin, Vector2.One, -id, ignoreParams: ignoreParams);
        }

        renderer.End();
    }
}