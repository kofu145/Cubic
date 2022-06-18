using System.Drawing;
using Cubic.Render.Text;

namespace Cubic.GUI;

public struct UITheme
{
    public Font Font;

    public Color BorderColor;

    public int BorderWidth;

    public Color RectColor;

    public Color HoverColor;

    public Color HoverTextColor;

    public Color ClickColor;

    public Color TextColor;

    public int CheckBoxPadding;

    public Color CheckedColor;

    public Color WindowColor;

    public Color WindowBorder;

    public int WindowBorderWidth;

    public Color AccentTextColor;

    public Color SelectionColor;
    
    public UITheme()
    {
        Font = UI.DefaultFont;
        
        BorderColor = Color.Black;
        BorderWidth = 1;
        RectColor = Color.GhostWhite;
        HoverColor = Color.DarkGray;
        HoverTextColor = default;
        ClickColor = Color.LightGray;
        TextColor = Color.Black;
        CheckBoxPadding = 5;
        CheckedColor = Color.DimGray;
        WindowColor = Color.White;
        WindowBorder = Color.Black;
        WindowBorderWidth = 1;
        AccentTextColor = Color.DimGray;
        SelectionColor = Color.CornflowerBlue;
    }
}