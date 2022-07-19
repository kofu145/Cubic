using System;

namespace Cubic.Utilities;

public static class CubicMath
{
    public static float ToRadians(float degrees) => degrees * (MathF.PI / 180);

    public static float ToDegrees(float radians) => radians * (180 / MathF.PI);

    public static float Clamp(float value, float min, float max) => value <= min ? min : value >= max ? max : value;
    
    public static int Clamp(int value, int min, int max) => value <= min ? min : value >= max ? max : value;

    public static float Lerp(float value1, float value2, float multiplier) => value1 + multiplier * (value2 - value1);

    public static int GreatestCommonFactor(int value1, int value2)
    {
        if (value2 > value1)
        {
            int val = value2 - value1;
            if (val <= 0)
                return value1;
            return val > value1 ? GreatestCommonFactor(value1, val) : GreatestCommonFactor(val, value1);
        }
        else
        {
            int val = value1 - value2;
            if (val <= 0)
                return value2;
            return val > value2 ? GreatestCommonFactor(value2, val) : GreatestCommonFactor(val, value2);
        }
    }
}