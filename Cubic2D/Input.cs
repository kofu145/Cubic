using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cubic2D.Windowing;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cubic2D;

public static class Input
{
    private static readonly HashSet<KeyState> _keyStates = new HashSet<KeyState>();
    private static readonly HashSet<MouseState> _mouseStates = new HashSet<MouseState>();

    private static readonly HashSet<Keys> _keysHeld = new HashSet<Keys>();
    private static readonly HashSet<Keys> _frameKeys = new HashSet<Keys>();

    private static readonly HashSet<MouseButtons> _buttonsHeld = new HashSet<MouseButtons>();
    private static readonly HashSet<MouseButtons> _frameButtons = new HashSet<MouseButtons>();

    /// <summary>
    /// Get an array of all keyboard keys currently held down.
    /// </summary>
    public static Keys[] KeysHeld => _keysHeld.ToArray();

    /// <summary>
    /// Get an array of all mouse buttons currently held down.
    /// </summary>
    public static MouseButtons[] MouseButtonsHeld => _buttonsHeld.ToArray();

    /// <summary>
    /// Check if the given key is held down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the key is held down.</returns>
    public static bool KeyDown(Keys key) => _keysHeld.Contains(key);

    /// <summary>
    /// Check if the given key was pressed <b>in this current frame.</b>
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the key was pressed in this current frame.</returns>
    public static bool KeyPressed(Keys key) => _frameKeys.Contains(key);

    /// <summary>
    /// Check if the given key is <b>not</b> held down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the key is <b>not</b> held down.</returns>
    public static bool KeyReleased(Keys key) => !_keysHeld.Contains(key);

    /// <summary>
    /// Check if any of the given keys are held down.
    /// </summary>
    /// <param name="keys">The keys to check.</param>
    /// <returns>True if any of the given keys are held down.</returns>
    public static bool KeysDown(params Keys[] keys)
    {
        foreach (Keys key in keys)
        {
            if (_keysHeld.Contains(key))
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// Check if any of the given keys were pressed <b>in this current frame.</b>
    /// </summary>
    /// <param name="keys">The keys to check.</param>
    /// <returns>True if any of the given keys were pressed in this current frame.</returns>
    public static bool KeysPressed(params Keys[] keys)
    {
        foreach (Keys key in keys)
        {
            if (_frameKeys.Contains(key))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the given mouse button is held down.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>True if the mouse button is held down.</returns>
    public static bool MouseButtonDown(MouseButtons button) => _buttonsHeld.Contains(button);

    /// <summary>
    /// Check if the given mouse button was pressed <b>in this current frame.</b>
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>True if the mouse button was pressed in this current frame.</returns>
    public static bool MouseButtonPressed(MouseButtons button) => _frameButtons.Contains(button);

    /// <summary>
    /// Check if the given mouse button is <b>not</b> held down.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>True if the mouse button is <b>not</b> held down.</returns>
    public static bool MouseButtonReleased(MouseButtons button) => !_buttonsHeld.Contains(button);
    
    /// <summary>
    /// Check if any of the given mouse buttons are held down.
    /// </summary>
    /// <param name="buttons">The mouse buttons to check.</param>
    /// <returns>True if any of the given mouse buttons are held down.</returns>
    public static bool MouseButtonsDown(params MouseButtons[] buttons)
    {
        foreach (MouseButtons button in buttons)
        {
            if (_buttonsHeld.Contains(button))
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// Check if any of the given mouse buttons were pressed <b>in this current frame.</b>
    /// </summary>
    /// <param name="buttons">The mouse buttons to check.</param>
    /// <returns>True if any of the given mouse buttons were pressed in this current frame.</returns>
    public static bool MouseButtonsPressed(params MouseButtons[] buttons)
    {
        foreach (MouseButtons button in buttons)
        {
            if (_frameButtons.Contains(button))
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// The current position of the mouse cursor on screen, relative to the top-left of the window.
    /// </summary>
    public static Vector2 MousePosition { get; private set; }
    
    /// <summary>
    /// The change in scroll since the last frame.
    /// </summary>
    public static Vector2 ScrollWheelDelta { get; private set; }

    internal static unsafe void Update(GameWindow window)
    {
        _keyStates.Clear();
        _mouseStates.Clear();
        _frameKeys.Clear();
        _frameButtons.Clear();

        GLFW.PollEvents();

        foreach (KeyState state in _keyStates)
        {
            if (state.Pressed)
            {
                if (_keysHeld.Add(state.Key))
                    _frameKeys.Add(state.Key);
            }
            else
            {
                _keysHeld.Remove(state.Key);
                _frameKeys.Remove(state.Key);
            }
        }

        foreach (MouseState state in _mouseStates)
        {
            if (state.Pressed)
            {
                if (_buttonsHeld.Add(state.Button))
                    _frameButtons.Add(state.Button);
            }
            else
            {
                _buttonsHeld.Remove(state.Button);
                _frameButtons.Remove(state.Button);
            }
        }

        GLFW.GetCursorPos(window.Handle, out double x, out double y);
        MousePosition = new Vector2((float) x, (float) y);
    }

    internal static unsafe void KeyCallback(Window* window, OpenTK.Windowing.GraphicsLibraryFramework.Keys key,
        int scanCode, InputAction action, KeyModifiers mods)
    {
        if (action != InputAction.Repeat)
            _keyStates.Add(new KeyState((Keys) key, action == InputAction.Press));
    }

    internal static unsafe void MouseCallback(Window* window, MouseButton button, InputAction action, KeyModifiers mods)
    {
        if (action != InputAction.Repeat)
            _mouseStates.Add(new MouseState((MouseButtons) button, action == InputAction.Press));
    }

    private struct KeyState
    {
        public Keys Key;
        public bool Pressed;

        public KeyState(Keys key, bool pressed)
        {
            Key = key;
            Pressed = pressed;
        }
    }
    
    private struct MouseState
    {
        public MouseButtons Button;
        public bool Pressed;

        public MouseState(MouseButtons button, bool pressed)
        {
            Button = button;
            Pressed = pressed;
        }
    }

    public static unsafe void ScrollCallback(Window* window, double offsetx, double offsety)
    {
        Console.WriteLine(offsety);
    }
}

public enum Keys
{
    Unknown,
    LeftShift,
    RightShift,
    LeftControl,
    RightControl,
    LeftAlt,
    RightAlt,
    LeftSuper,
    RightSuper,
    Menu,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12,
    F13,
    F14,
    F15,
    F16,
    F17,
    F18,
    F19,
    F20,
    F21,
    F22,
    F23,
    F24,
    F25,
    F26,
    F27,
    F28,
    F29,
    F30,
    F31,
    F32,
    F33,
    F34,
    F35,
    Up,
    Down,
    Left,
    Right,
    Enter,
    Escape,
    Space,
    Tab,
    Backspace,
    Insert,
    Delete,
    PageUp,
    PageDown,
    Home,
    End,
    CapsLock,
    ScrollLock,
    PrintScreen,
    Pause,
    NumLock,
    Clear,
    Sleep,
    Keypad0,
    Keypad1,
    Keypad2,
    Keypad3,
    Keypad4,
    Keypad5,
    Keypad6,
    Keypad7,
    Keypad8,
    Keypad9,
    KeypadDivide,
    KeypadMultiply,
    KeypadSubtract,
    KeypadAdd,
    KeypadDecimal,
    KeypadEnter,
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    Num0,
    Num1,
    Num2,
    Num3,
    Num4,
    Num5,
    Num6,
    Num7,
    Num8,
    Num9,
    Tilde,
    Minus,
    Plus,
    LeftBracket,
    RightBracket,
    Semicolon,
    Quote,
    Comma,
    Period,
    Slash,
    Backslash,
    NonUsBackslash
}

public enum MouseButtons
{
    Left,
    Middle,
    Right,
    Button1,
    Button2,
    Button3,
    Button4,
    Button5,
    Button6,
    Button7,
    Button8,
    Button9
}