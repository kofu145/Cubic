using System;
using System.Drawing;
using System.Numerics;
using Cubic.Graphics;
using Cubic.Graphics.Platforms.GLES20;
using Cubic.Graphics.Platforms.OpenGL33;
using Cubic.Utilities;
using Cubic.Windowing;
using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;

namespace Cubic.Render;

public class CubicGraphics : IDisposable
{
    private GameWindow _window;

    public readonly SpriteRenderer SpriteRenderer;

    public static GraphicsDevice GraphicsDevice { get; private set; }

    public bool VSync
    {
        set => GameWindow.GLFW.SwapInterval(value ? 1 : 0);
    }

    public Rectangle Viewport
    {
        get => GraphicsDevice.Viewport;
        set => GraphicsDevice.Viewport = value;
    }

    public Rectangle Scissor
    {
        get => GraphicsDevice.Scissor;
        set => GraphicsDevice.Scissor = value;
    }

    public void SetRenderTarget(RenderTarget target)
    {
        if (target == null)
        {
            GraphicsDevice.SetFramebuffer(null);
            Viewport = new Rectangle(0, 0, _window.Size.Width, _window.Size.Height);
            Scissor = Viewport;
            return;
        }

        GraphicsDevice.SetFramebuffer(target.Framebuffer);
        Viewport = new Rectangle(0, 0, target.Size.Width, target.Size.Height);
        Scissor = Viewport;
    }

    public void Clear(Vector4 clearColor)
    {
        GraphicsDevice.Clear(clearColor);
    }

    public void Clear(Color clearColor)
    {
        GraphicsDevice.Clear(clearColor);
    }

    internal unsafe CubicGraphics(GameWindow window, GameSettings settings)
    {
        _window = window;

        window.Resize += WindowResized;

        IGLContext context = new GlfwContext(GameWindow.GLFW, window.Handle);

        switch (settings.GraphicsApi)
        {
            case GraphicsApi.Default:
                break;
            case GraphicsApi.OpenGL33:
                GraphicsDevice = new OpenGl33GraphicsDevice(context);
                break;
            case GraphicsApi.GLES20:
                GraphicsDevice = new Gles20GraphicsDevice(context);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        VSync = settings.VSync;

        Viewport = new Rectangle(0, 0, window.Size.Width, window.Size.Height);
        
        SpriteRenderer = new SpriteRenderer(GraphicsDevice);

        GraphicsDevice.Options.DepthTest = DepthTest.LessEqual;
        GraphicsDevice.Options.EnableScissor = true;
        GraphicsDevice.Options.CullDirection = CullDirection.CounterClockwise;
        GraphicsDevice.Options.CullFace = CullFace.Back;
    }
    

    internal void PrepareFrame(Vector4 clearColor)
    {
        Scissor = Viewport;
        Clear(clearColor);
    }

    internal unsafe void PresentFrame()
    {
        GameWindow.GLFW.SwapBuffers(_window.Handle);
    }
    
    private void WindowResized(Size size)
    {
        // Resize viewport.
        Viewport = new Rectangle(0, 0, size.Width, size.Height);
    }

    public Bitmap Capture(Rectangle region)
    {
        return new Bitmap(region.Width, region.Height, GraphicsDevice.GetPixels(region));
    }

    public Bitmap Capture() => Capture(Viewport);

    public void Dispose()
    {
        SpriteRenderer.Dispose();
        // Dispose of the command list and graphics device, remove delegate for window resizing.
        _window.Resize -= WindowResized;
    }

    public delegate void OnResize(Size size);
}