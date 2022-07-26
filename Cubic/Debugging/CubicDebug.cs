using System;

namespace Cubic.Debugging;

public static class CubicDebug
{
    public static event OnWriteLineDelegate OnWriteLine;

    public static event OnWriteDelegate OnWrite;

    public static void WriteLine(object input)
    {
        string text = $"[{DateTime.Now}]: {input}";
        Console.WriteLine(text);
        System.Diagnostics.Debug.WriteLine(text);
        OnWriteLine?.Invoke(text);
    }
    
    public static void Write(object input)
    {
        string text = $"[{DateTime.Now}]: {input}";
        Console.Write(text);
        System.Diagnostics.Debug.Write(text);
        OnWrite?.Invoke(text);
    }
    
    public delegate void OnWriteLineDelegate(string text);

    public delegate void OnWriteDelegate(string text);
}