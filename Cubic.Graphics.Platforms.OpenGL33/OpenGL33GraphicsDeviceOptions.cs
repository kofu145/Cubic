using System;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33GraphicsDeviceOptions : GraphicsDeviceOptions
{
    private DepthTest _depthTest;
    private bool _depthEnabled;
    private bool _depthMask;
    private CullFace _face;
    private CullDirection _dir;

    public override bool DepthMask
    {
        get => _depthMask;
        set
        {
            _depthMask = value;
            Gl.DepthMask(value);
        }
    }

    public override CullFace CullFace
    {
        get => _face;
        set
        {
            _face = value;
            if (value == CullFace.None)
                Gl.Disable(EnableCap.CullFace);
            else
            {
                Gl.Enable(EnableCap.CullFace);
                Gl.CullFace(value == CullFace.Front ? CullFaceMode.Front : CullFaceMode.Back);
            }
        }
    }

    public override CullDirection CullDirection
    {
        get => _dir;
        set
        {
            _dir = value;
            Gl.FrontFace(value == CullDirection.Clockwise ? FrontFaceDirection.CW : FrontFaceDirection.Ccw);
        }
    }
    
    public override DepthTest DepthTest
    {
        get => _depthTest;
        set
        {
            _depthTest = value;
            if (value == DepthTest.Disable && _depthEnabled)
            {
                _depthEnabled = false;
                Gl.Disable(EnableCap.DepthTest);
            }
            else if (!_depthEnabled)
            {
                _depthEnabled = true;
                Gl.Enable(EnableCap.DepthTest);
                Gl.DepthFunc(value switch
                {
                    DepthTest.Disable => DepthFunction.Never,
                    DepthTest.Always => DepthFunction.Always,
                    DepthTest.Equal => DepthFunction.Equal,
                    DepthTest.GreaterEqual => DepthFunction.Gequal,
                    DepthTest.Greater => DepthFunction.Greater,
                    DepthTest.LessEqual => DepthFunction.Lequal,
                    DepthTest.Less => DepthFunction.Less,
                    DepthTest.NotEqual => DepthFunction.Notequal,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                });
            }
        }
    }

    public override bool EnableScissor
    {
        get => Gl.IsEnabled(EnableCap.ScissorTest);
        set
        {
            if (value)
                Gl.Enable(EnableCap.ScissorTest);
            else
                Gl.Disable(EnableCap.ScissorTest);
        }
    }
}