using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cubic.Graphics;
using Cubic.Graphics.Platforms.GLES20;
using Cubic.Graphics.Platforms.OpenGL33;
using Cubic.Render;
using Cubic.Windowing;
using ImGuiNET;
using Buffer = Cubic.Graphics.Buffer;
using Shader = Cubic.Render.Shader;
using Texture = Cubic.Render.Texture;

namespace Cubic.Extensions.Imgui;

public class ImGuiRenderer : IDisposable
{
    private int _windowWidth;
    private int _windowHeight;

    private bool _frameBegun;

    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private uint _vboSize;
    private uint _eboSize;

    private Shader _shader;

    private Texture2D _fontTexture;

    public Vector2 Scale;

    private readonly List<char> _pressedChars;

    private Keys[] _keysList;

    private Dictionary<string, ImFontPtr> _fonts;

    private uint _stride;
    private ShaderLayout[] _layouts;

    public ImGuiRenderer(CubicGame game)
    {
        game.GameBeforeUpdate += Update;
        game.GameDraw += Render;
        
        Scale = Vector2.One;
        _fonts = new Dictionary<string, ImFontPtr>();

        _windowWidth = game.Graphics.Viewport.Width;
        _windowHeight = game.Graphics.Viewport.Height;
            
        CubicGraphics.GraphicsDevice.ViewportResized += WindowOnResize;
        Input.TextInput += PressChar;

        _pressedChars = new List<char>();
        _keysList = (Keys[])Enum.GetValues(typeof(Keys));

        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceResources();
        SetKeyMappings();

        SetPerFrameImGuiData(1f / 60f);
        
        ImGui.NewFrame();
        _frameBegun = true;
    }

    private void WindowOnResize(Rectangle viewport)
    {
        _windowWidth = viewport.Width;
        _windowHeight = viewport.Height;
    }

    private void CreateDeviceResources()
    {
        _vboSize = 10000;
        _eboSize = 2000;

        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, _vboSize);
        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, _eboSize);

        RecreateFontDeviceTexture();

        const string vertexSource = @"
in vec2 aPosition;
in vec2 aTexCoords;
in vec4 aColor;

out vec4 frag_color;
out vec2 frag_texCoords;

uniform mat4 uProjection;

void main()
{
gl_Position = uProjection * vec4(aPosition, 0, 1);
frag_color = aColor;
frag_texCoords = aTexCoords;
}";

        const string fragmentSource = @"
in vec4 frag_color;
in vec2 frag_texCoords;

out vec4 out_color;

uniform sampler2D uTexture;

void main()
{
out_color = frag_color * texture(uTexture, frag_texCoords);
}";

        _shader = new Shader(vertexSource, fragmentSource);

        _stride = (uint) Unsafe.SizeOf<ImDrawVert>();
        _layouts = new[]
        {
            new ShaderLayout("aPosition", 2, AttribType.Float),
            new ShaderLayout("aTexCoords", 2, AttribType.Float),
            new ShaderLayout("aColor", 4, AttribType.Byte, true)
        };
    }

    private void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

        _fontTexture = new Texture2D(width, height, false);
        _fontTexture.SetData(pixels, 0, 0, width, height);

        switch (CubicGraphics.GraphicsDevice.CurrentApi)
        {
            case GraphicsApi.OpenGL33:
                io.Fonts.SetTexID((IntPtr) ((OpenGl33Texture) _fontTexture.InternalTexture).Handle);
                break;
            case GraphicsApi.GLES20:
                io.Fonts.SetTexID((IntPtr) ((Gles20Texture) _fontTexture.InternalTexture).Handle);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        io.Fonts.ClearTexData();
    }

    private void Render(CubicGame game, CubicGraphics graphics)
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
        }
    }

    private void Update(CubicGame game, CubicGraphics graphics)
    {
        if (_frameBegun)
            ImGui.Render();

        SetPerFrameImGuiData(Time.DeltaTime);
        UpdateImGuiInput();

        _frameBegun = true;
        
        ImGui.NewFrame();
    }

    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(_windowWidth / Scale.X, _windowHeight / Scale.Y);
        io.DisplayFramebufferScale = Scale;
        io.DeltaTime = deltaSeconds;
    }

    private void UpdateImGuiInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        io.MouseDown[0] = Input.MouseButtonDown(MouseButtons.Left);
        io.MouseDown[1] = Input.MouseButtonDown(MouseButtons.Right);
        io.MouseDown[2] = Input.MouseButtonDown(MouseButtons.Middle);

        io.MousePos = Input.MousePosition / Scale;

        io.MouseWheel = Input.ScrollWheelDelta.Y;
        io.MouseWheelH = Input.ScrollWheelDelta.X;

        foreach (Keys key in _keysList)
        {
            if ((int) key > 0)
                io.KeysDown[(int) key] = Input.KeyDown(key);
        }
        
        foreach (char c in _pressedChars)
            io.AddInputCharacter(c);
        _pressedChars.Clear();
        
        io.KeyCtrl = Input.KeyDown(Keys.LeftControl) || Input.KeyDown(Keys.RightControl);
        io.KeyAlt = Input.KeyDown(Keys.LeftAlt) || Input.KeyDown(Keys.RightAlt);
        io.KeyShift = Input.KeyDown(Keys.LeftShift) || Input.KeyDown(Keys.RightShift);
        io.KeySuper = Input.KeyDown(Keys.LeftSuper) || Input.KeyDown(Keys.RightSuper);
    }

    private void PressChar(char chr)
    {
        _pressedChars.Add(chr);
    }

    private static void SetKeyMappings()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
    }

    private void RenderImDrawData(ImDrawDataPtr drawData)
    {
        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (drawData.CmdListsCount == 0)
            return;
        
        uint totalVbSize = (uint) (drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        if (totalVbSize > _vboSize)
        {
            _vboSize = (uint) Math.Max(_vboSize * 1.5f, totalVbSize);
            
            _vertexBuffer.Resize(_vboSize);
        }

        uint totalIbSize = (uint) (drawData.TotalIdxCount * sizeof(ushort));
        if (totalIbSize > _eboSize)
        {
            _eboSize = (uint) Math.Max(_eboSize * 1.5f, totalIbSize);
            
            _indexBuffer.Resize(_eboSize);
        }

        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdListsRange[i];
            
            _vertexBuffer.Update<ImDrawVert>((int) vertexOffsetInVertices, (uint) cmdList.VtxBuffer.Size, cmdList.VtxBuffer.Data);
            _indexBuffer.Update<ushort>((int) indexOffsetInElements, (uint) cmdList.IdxBuffer.Size, cmdList.IdxBuffer.Data);

            vertexOffsetInVertices += (uint) cmdList.VtxBuffer.Size;
            indexOffsetInElements += (uint) cmdList.IdxBuffer.Size;
        }

        ImGuiIOPtr io = ImGui.GetIO();

        Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1, 1);
        _shader.Set("uProjection", mvp, false);
        _shader.Set("uTexture", 0);

        drawData.ScaleClipRects(io.DisplayFramebufferScale);

        GraphicsDevice device = CubicGraphics.GraphicsDevice;
        CullFace lastCull = device.Options.CullFace;
        DepthTest lastTest = device.Options.DepthTest;
        device.Options.CullFace = CullFace.None;
        device.Options.DepthTest = DepthTest.Disable;

        int vtxOffset = 0;
        int idxOffset = 0;

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdListsRange[n];
            for (int cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
            {
                ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdI];
                if (pcmd.UserCallback != IntPtr.Zero)
                    throw new NotImplementedException();
                
                device.SetTexture(0, pcmd.TextureId);
                device.SetShader(_shader.InternalProgram);

                Vector2 clipOff = drawData.DisplayPos;
                Vector4 clipRect = pcmd.ClipRect;
                device.Scissor = new Rectangle((int) (clipRect.X - clipOff.X), (int) (clipRect.Y - clipOff.Y),
                    (int) (clipRect.Z - clipOff.X - clipRect.X), (int) (clipRect.W - clipOff.Y - clipRect.Y));

                device.SetVertexBuffer(_vertexBuffer, _stride, _layouts);
                device.SetIndexBuffer(_indexBuffer);

                device.Draw(pcmd.ElemCount, idxOffset * sizeof(ushort), vtxOffset);

                idxOffset += (int) pcmd.ElemCount;
            }

            vtxOffset += cmdList.VtxBuffer.Size;
        }

        device.Options.DepthTest = lastTest;
        device.Options.CullFace = lastCull;
    }
    
    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _fontTexture.Dispose();
        _shader.Dispose();
        GC.SuppressFinalize(this);
    }

    public void AddFont(string name, string path, int size)
    {
        if (_fonts.ContainsKey(name))
            return;
        _fonts.Add(name, ImGui.GetIO().Fonts.AddFontFromFileTTF(path, size));
        RecreateFontDeviceTexture();
    }

    public void SetFont(string name)
    {
        ImGui.PushFont(_fonts[name]);
    }

    public void ResetFont()
    {
        ImGui.PopFont();
    }

    public IntPtr TextureToImGui(Texture texture)
    {
        switch (CubicGraphics.GraphicsDevice.CurrentApi)
        {
            case GraphicsApi.OpenGL33:
                return (IntPtr) ((OpenGl33Texture) texture.InternalTexture).Handle;
            case GraphicsApi.GLES20:
                return (IntPtr) ((Gles20Texture) texture.InternalTexture).Handle;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}