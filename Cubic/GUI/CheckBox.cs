using System;
using System.Drawing;
using System.Numerics;
using Cubic.Render;
using Cubic.Utilities;

namespace Cubic.GUI;

public class CheckBox : UIElement
{
    public bool Checked;
    public string Text;
    public uint TextSize;
    
    public CheckBox(Anchor anchor, Rectangle position, bool isChecked = false, string text = "", uint size = 24,
        bool captureMouse = true, bool ignoreReferenceResolution = false, Point? index = null) : base(anchor, position,
        captureMouse, ignoreReferenceResolution, index)
    {
        Checked = isChecked;
        Text = text;
        TextSize = size;
    }

    protected internal override void Update(ref bool mouseCaptured)
    {
        base.Update(ref mouseCaptured);

        if (Pressed)
            Checked = !Checked;
    }

    protected internal override void Draw(GraphicsMachine graphics)
    {
        base.Draw(graphics);
        
        Rectangle rect = Position;
        UI.CalculatePos(Anchor, ref rect, IgnoreReferenceResolution, Offset, Viewport);

        Color color = Theme.RectColor;
        if (Hovering)
            color = Theme.HoverColor;
        if (Clicked)
            color = Theme.ClickColor;

        uint textSize = (uint) (TextSize * UI.GetReferenceMultiplier());
        Size size = Theme.Font.MeasureString(textSize, Text);
        Theme.Font.Draw(graphics.SpriteRenderer, textSize, Text, new Vector2(rect.X + rect.Width / 2 + rect.Width / 2 + 5, rect.Y + rect.Height / 2), Color.Black, 0,
            new Vector2(0, size.Height / 2), Vector2.One);
        
        graphics.SpriteRenderer.DrawBorderRectangle(rect.Location.ToVector2(), rect.Size.ToVector2(), Theme.BorderWidth,
            Theme.BorderColor, color, 0, Vector2.Zero);

        int padding = Theme.CheckBoxPadding;
        
        if (Checked)
        {
            graphics.SpriteRenderer.DrawRectangle(rect.Location.ToVector2() + new Vector2(padding),
                rect.Size.ToVector2() - new Vector2(padding * 2), Theme.CheckedColor, 0, Vector2.Zero);
        }
    }
}