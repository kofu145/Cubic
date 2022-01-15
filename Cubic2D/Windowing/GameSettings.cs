using System.Drawing;

namespace Cubic2D.Windowing;

public struct GameSettings
{
    /// <summary>
    /// The initial size in pixels (resolution) of the window. (Default: 1280x720)
    /// </summary>
    public Size Size;

    /// <summary>
    /// The initial title of the window. (Default: "Cubic2D Window")
    /// </summary>
    public string Title;

    /// <summary>
    /// The initial <see cref="WindowMode"/> of the window. (Default: Windowed)
    /// </summary>
    public WindowMode WindowMode;
    
    /// <summary>
    /// If true, the window will be resizable. (Default: false)
    /// </summary>
    public bool Resizable;

    /// <summary>
    /// If true, the graphics device will attempt to sync to vertical refresh. (Default: true)
    /// </summary>
    public bool VSync;

    /// <summary>
    /// The starting location of the window. If you <b>do not</b> set this value, the window will start centered on
    /// the primary monitor.
    /// </summary>
    public Point Location;

    public GameSettings()
    {
        Size = new Size(1280, 720);
        Title = "Cubic2D Window";
        WindowMode = WindowMode.Windowed;
        Resizable = false;
        VSync = true;
        Location = new Point(-1, -1);
    }
}