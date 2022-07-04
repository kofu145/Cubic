using System;
using System.Drawing;
using System.Numerics;

namespace Cubic.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    /// <summary>
    /// This event is invoked whenever the viewport is resized (which often occurs when the window size changes.)
    /// </summary>
    public abstract event OnViewportResized ViewportResized;
    
    /// <summary>
    /// Set various graphics options, including depth tests and blend functions.
    /// </summary>
    public abstract GraphicsDeviceOptions Options { get; protected set; }

    /// <summary>
    /// Get or set the viewport region.
    /// </summary>
    public abstract Rectangle Viewport { get; set; }
    
    /// <summary>
    /// Get or set the scissor region.
    /// </summary>
    public abstract Rectangle Scissor { get; set; }
    
    /// <summary>
    /// Get the current graphics API being used
    /// </summary>
    public abstract GraphicsApi CurrentApi { get; }
    
    /// <summary>
    /// Create a graphics buffer with the given <see cref="BufferType"/> and size.
    /// </summary>
    /// <param name="type">The buffer's type (vertex buffer, index buffer, etc.)</param>
    /// <param name="size">The initial size in bytes of this buffer.</param>
    /// <returns>The created buffer.</returns>
    public abstract Buffer CreateBuffer(BufferType type, uint size);

    /// <summary>
    /// Create a texture.
    /// </summary>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <param name="format">The format of this texture (RGB, RGBA, etc).</param>
    /// <param name="sample">The sample type of this texture (linear, nearest)</param>
    /// <param name="mipmap">Whether or not to generate mipmaps for this texture automatically. You can manually generate mipmaps by calling <see cref="Texture.GenerateMipmaps()"/></param>
    /// <param name="usage">The usage of this texture (Texture, Cubemap, etc)</param>
    /// <param name="wrap">The wrapping of this texture (Wrap, Clamp, etc)</param>
    /// <param name="anisotropicLevel">The number of anisotropic texture levels should be generated for this texture, if mipmapping is used.</param>
    /// <returns>The created texture.</returns>
    public abstract Texture CreateTexture(uint width, uint height, PixelFormat format,
        TextureSample sample = TextureSample.Linear, bool mipmap = true, TextureUsage usage = TextureUsage.Texture, TextureWrap wrap = TextureWrap.Repeat, uint anisotropicLevel = 0);

    /// <summary>
    /// Create a framebuffer that can have <see cref="Texture"/>s attached to it.
    /// </summary>
    /// <returns>The created framebuffer.</returns>
    public abstract Framebuffer CreateFramebuffer();

    /// <summary>
    /// Create a shader with the given shader attachments.
    /// </summary>
    /// <param name="attachments">The attachments this shader should have.</param>
    /// <returns>The created shader.</returns>
    public abstract Shader CreateShader(params ShaderAttachment[] attachments);

    /// <summary>
    /// Get a region of pixels on the screen. Pass in <see cref="Viewport"/> to get the entire viewport.
    /// </summary>
    /// <param name="region">The region of pixels.</param>
    /// <returns>An RGBA byte array of pixels.</returns>
    public abstract byte[] GetPixels(Rectangle region);

    /// <summary>
    /// Clear the screen with the given color.
    /// </summary>
    /// <param name="color">The color to clear the screen with.</param>
    public abstract void Clear(Color color);

    /// <summary>
    /// Clear the screen with the given color.
    /// </summary>
    /// <param name="color">The <b>normalized</b> color.</param>
    public abstract void Clear(Vector4 color);

    /// <summary>
    /// Set the shader that will be used on next draw.
    /// </summary>
    /// <param name="program">The shader to set.</param>
    public abstract void SetShader(Shader program);

    /// <summary>
    /// Set the vertex buffer that will be used on next draw. This overload will automatically determine the best layout
    /// to use for the given buffer type and shader. This will work correctly 90% of the time.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer.</param>
    public abstract void SetVertexBuffer(Buffer vertexBuffer);

    /// <summary>
    /// Set the vertex buffer that will be used on next draw. Use this overload if you need to manually define the layout
    /// used in the current vertex shader. You may need to do this if the automatic overload does not correctly determine
    /// the layout itself.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer to set.</param>
    /// <param name="stride">The stride the layout should use.</param>
    /// <param name="layout">The layout of the shader.</param>
    public abstract void SetVertexBuffer(Buffer vertexBuffer, uint stride, params ShaderLayout[] layout);

    /// <summary>
    /// Set the index buffer that will be used on next draw.
    /// </summary>
    /// <param name="indexBuffer">The index buffer to set.</param>
    public abstract void SetIndexBuffer(Buffer indexBuffer);

    /// <summary>
    /// Set the texture that will be used on next draw in the given slot.
    /// </summary>
    /// <param name="slot">The slot the texture should occupy.</param>
    /// <param name="texture">The texture to use.</param>
    public abstract void SetTexture(uint slot, Texture texture);

    // TODO: Remove this later
    /// <summary>
    /// RESERVED FOR IMGUI
    /// </summary>
    public abstract void SetTexture(uint slot, IntPtr texture);

    /// <summary>
    /// Set the framebuffer that will be rendered to.
    /// </summary>
    /// <param name="framebuffer">The framebuffer to render to.</param>
    public abstract void SetFramebuffer(Framebuffer framebuffer);

    /// <summary>
    /// Draw the set vertex and index buffer, using triangles.
    /// </summary>
    /// <param name="count">The number of indices to draw.</param>
    public abstract void Draw(uint count);

    /// <summary>
    /// Draw the set vertex and index buffer, using triangles.
    /// </summary>
    /// <param name="count">The number of indices to draw.</param>
    /// <param name="indices">A pointer to where the indices are stored.</param>
    public abstract void Draw(uint count, int indices);

    /// <summary>
    /// Draw the set vertex and index buffer, using triangles and a base vertex.
    /// </summary>
    /// <param name="count">The number of indices to draw.</param>
    /// <param name="indices">A pointer to where the indices are stored.</param>
    /// <param name="baseVertex">The base vertex to use.</param>
    public abstract void Draw(uint count, int indices, int baseVertex);

    public abstract void Dispose();
    
    public delegate void OnViewportResized(Rectangle viewport);
}