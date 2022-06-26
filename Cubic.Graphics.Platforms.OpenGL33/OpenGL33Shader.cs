using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;
using static Cubic.Graphics.Platforms.OpenGL33.OpenGL33GraphicsDevice;

namespace Cubic.Graphics.Platforms.OpenGL33;

public class OpenGL33Shader : Shader
{
    public uint Handle;
    public Dictionary<string, int> UniformLocations;

    public override void SetUniform(string uniformName, bool value)
    {
        Gl.UseProgram(Handle);
        Gl.Uniform1(UniformLocations[uniformName], value ? 1 : 0);
    }

    public override void SetUniform(string uniformName, int value)
    {
        Gl.UseProgram(Handle);
        Gl.Uniform1(UniformLocations[uniformName], value);
    }

    public override void SetUniform(string uniformName, float value)
    {
        Gl.UseProgram(Handle);
        Gl.Uniform1(UniformLocations[uniformName], value);
    }

    public override void SetUniform(string uniformName, Vector2 value)
    {
        Gl.UseProgram(Handle);
        Gl.Uniform2(UniformLocations[uniformName], ref value);
    }

    public override void SetUniform(string uniformName, Vector3 value)
    {
        Gl.UseProgram(Handle);
        Gl.Uniform3(UniformLocations[uniformName], ref value);
    }

    public override void SetUniform(string uniformName, Vector4 value)
    {
        Gl.UseProgram(Handle);
        Gl.Uniform4(UniformLocations[uniformName], ref value);
    }

    public override void SetUniform(string uniformName, Color color)
    {
        Gl.UseProgram(Handle);
        Vector4 normalized = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        Gl.Uniform4(UniformLocations[uniformName], ref normalized);
    }

    public override unsafe void SetUniform(string uniformName, Matrix4x4 matrix, bool transpose = true)
    {
        Gl.UseProgram(Handle);
        Gl.UniformMatrix4(UniformLocations[uniformName], 1, transpose, (float*) &matrix);
    }

    internal OpenGL33Shader(ShaderAttachment[] attachments)
    {
        Handle = Gl.CreateProgram();
        
        for (int i = 0; i < attachments.Length; i++)
        {
            ShaderAttachment attachment = attachments[i];
            ShaderType sType = attachment.Attachment switch
            {
                AttachmentType.Vertex => ShaderType.VertexShader,
                AttachmentType.Fragment => ShaderType.FragmentShader,
                _ => throw new ArgumentOutOfRangeException(nameof(attachment.Attachment), attachment.Attachment, null)
            };
                
            string finalCode = attachment.Code.Insert(0, "#version 330 core\n");

            uint shader = Gl.CreateShader(sType);
            attachments[i].TempHandle = shader;
            Gl.ShaderSource(shader, finalCode);
            Gl.CompileShader(shader);
            Gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            if (status != (int) GLEnum.True)
                throw new GraphicsException($"Error compiling shader.\n\n{Gl.GetShaderInfoLog(shader)}");
            
            Gl.AttachShader(Handle, shader);
        }
        
        Gl.LinkProgram(Handle);
        for (int i = 0; i < attachments.Length; i++)
        {
            Gl.DetachShader(Handle, attachments[i].TempHandle);
            Gl.DeleteShader(attachments[i].TempHandle);
        }
        Gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int pStatus);
        if (pStatus != (int) GLEnum.True)
            throw new GraphicsException($"Error linking program.\n\n{Gl.GetProgramInfoLog(Handle)}");

        Dictionary<string, int> uLocations = new Dictionary<string, int>();
        Gl.GetProgram(Handle, ProgramPropertyARB.ActiveUniforms, out int numUniforms);
        for (uint i = 0; i < numUniforms; i++)
        {
            string name = Gl.GetActiveUniform(Handle, i, out _, out _);
            int location = Gl.GetUniformLocation(Handle, name);
            uLocations.Add(name, location);
        }

        UniformLocations = uLocations;
    }

    public override void Dispose()
    {
        Gl.DeleteShader(Handle);
    }
}