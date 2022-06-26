using System;
using System.Drawing;
using System.Numerics;
using Cubic.Graphics;
using Cubic.Graphics.Platforms.OpenGL33;
using Cubic.Utilities;
using Cubic.Windowing;
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

        GraphicsDevice = new OpenGl33GraphicsDevice(new GlfwContext(GameWindow.GLFW, window.Handle));

        VSync = settings.VSync;

        Viewport = new Rectangle(0, 0, window.Size.Width, window.Size.Height);
        
        SpriteRenderer = new SpriteRenderer(GraphicsDevice);

        GraphicsDevice.Options.DepthTest = DepthTest.LessEqual;
        GraphicsDevice.Options.EnableScissor = true;
        GraphicsDevice.Options.CullDirection = CullDirection.CounterClockwise;
        GraphicsDevice.Options.CullFace = CullFace.Back;


        /*Gl.Enable(EnableCap.Blend);
        Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        Gl.Enable(EnableCap.CullFace);
        Gl.CullFace(CullFaceMode.Back);
        Gl.FrontFace(FrontFaceDirection.CW);
        
        Gl.Enable(EnableCap.DepthTest);
        Gl.DepthFunc(DepthFunction.Lequal);
        
        Gl.Enable(EnableCap.ScissorTest);
        Scissor = Viewport;
        
        if (settings.MsaaSamples > 0)
            Gl.Enable(EnableCap.Multisample);*/
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