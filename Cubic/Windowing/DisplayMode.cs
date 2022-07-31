using System;
using System.Drawing;
using Cubic.Utilities;

namespace Cubic.Windowing;

public struct DisplayMode : IEquatable<DisplayMode>
{
    /// <summary>
    /// The resolution of this display mode in pixels.
    /// </summary>
    public readonly Size Resolution;

    /// <summary>
    /// The refresh rate of this display mode in Hz.
    /// </summary>
    public readonly int RefreshRate;

    /// <summary>
    /// The aspect ratio of this display mode. This value <b>is not</b> 100% accurate to the actual aspect ratio, however it
    /// is more accurate to the end user (for example, 1366x768 will show as 16:9 even though it is not quite 16:9 in
    /// reality). Use <see cref="AccurateAspectRatio"/> for an accurate aspect ratio.
    /// </summary>
    public readonly Size AspectRatio;
    
    /// <summary>
    /// The accurate aspect ratio of this display mode. This value <b>is</b> accurate to the actual aspect ratio of the
    /// resolution. Use <see cref="AspectRatio"/> for a "good enough" aspect ratio calculation.
    /// </summary>
    public readonly Size AccurateAspectRatio;

    public DisplayMode(Size resolution, int refreshRate)
    {
        Resolution = resolution;
        RefreshRate = refreshRate;

        AspectRatio = Utils.CalculateAspectRatio(resolution);
        AccurateAspectRatio = resolution / CubicMath.GreatestCommonFactor(resolution.Width, resolution.Height);
    }

    public bool Equals(DisplayMode other)
    {
        return Resolution.Equals(other.Resolution) && RefreshRate == other.RefreshRate;
    }

    public override bool Equals(object obj)
    {
        return obj is DisplayMode other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Resolution, RefreshRate);
    }

    public static bool operator ==(DisplayMode disp1, DisplayMode disp2)
    {
        return disp1.Equals(disp2);
    }

    public static bool operator !=(DisplayMode disp1, DisplayMode disp2)
    {
        return !(disp1 == disp2);
    }
}