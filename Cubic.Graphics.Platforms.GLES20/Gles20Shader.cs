using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text.RegularExpressions;
using Silk.NET.OpenGLES;
using static Cubic.Graphics.Platforms.GLES20.Gles20GraphicsDevice;

namespace Cubic.Graphics.Platforms.GLES20;

public class Gles20Shader : Shader
{
    public uint Handle;
    public Dictionary<string, int> UniformLocations;
    public ShaderLayout[] Layout;
    public uint Stride;

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
        Matrix4x4 tm = matrix;
        if (transpose)
            tm = Matrix4x4.Transpose(matrix);
        Gl.UniformMatrix4(UniformLocations[uniformName], 1, false, (float*) &tm);
    }

    internal Gles20Shader(ShaderAttachment[] attachments)
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

            #region mini transpiler
            
            string finalCode = attachment.Code.Insert(0, "#version 100\nprecision highp float;\n");

            switch (attachment.Attachment)
            {
                case AttachmentType.Vertex:
                    string[] splitLines = finalCode.Split('\n');
                    for (int l = 0; l < splitLines.Length; l++)
                    {
                        if (splitLines[l].StartsWith("in "))
                        {
                            splitLines[l] = splitLines[l].Remove(0, 3);
                            splitLines[l] = splitLines[l].Insert(0, "attribute ");
                        }
                        else if (splitLines[l].StartsWith("out "))
                        {
                            splitLines[l] = splitLines[l].Remove(0, 4);
                            splitLines[l] = splitLines[l].Insert(0, "varying ");
                        }
                    }

                    finalCode = string.Join('\n', splitLines);
                    break;
                case AttachmentType.Fragment:
                    string[] splitLinesFrag = finalCode.Split('\n');
                    for (int l = 0; l < splitLinesFrag.Length; l++)
                    {
                        if (splitLinesFrag[l].StartsWith("in "))
                        {
                            splitLinesFrag[l] = splitLinesFrag[l].Remove(0, 3);
                            splitLinesFrag[l] = splitLinesFrag[l].Insert(0, "varying ");
                        }
                        else if (splitLinesFrag[l].StartsWith("out "))
                        {
                            splitLinesFrag[l] = splitLinesFrag[l].Remove(0, splitLinesFrag[l].Length);
                        }

                        if (splitLinesFrag[l].Contains("out_color"))
                        {
                            splitLinesFrag[l] = splitLinesFrag[l].Replace("out_color", "gl_FragColor");
                        }
                        if (splitLinesFrag[l].Contains("texture"))
                            splitLinesFrag[l] = splitLinesFrag[l].Replace("texture", "texture2D");
                    }

                    finalCode = string.Join('\n', splitLinesFrag);
                    break;
            }
            
            #endregion

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
        
        List<ShaderLayout> layouts = new List<ShaderLayout>();
        Gl.GetProgram(Handle, GLEnum.ActiveAttributes, out int numAttributes);
        uint stride = 0;
        for (uint i = 0; i < numAttributes; i++)
        {
            string name = Gl.GetActiveAttrib(Handle, i, out int size, out AttributeType type);
            switch (type)
            {
                case AttributeType.Int:
                    size *= 1;
                    break;
                case AttributeType.UnsignedInt:
                    size *= 1;
                    break;
                case AttributeType.Float:
                    size *= 1;
                    break;
                case AttributeType.Double:
                    size *= 1;
                    break;
                case AttributeType.FloatVec2:
                    size *= 2;
                    break;
                case AttributeType.FloatVec3:
                    size *= 3;
                    break;
                case AttributeType.FloatVec4:
                    size *= 4;
                    break;
                case AttributeType.IntVec2:
                    size *= 2;
                    break;
                case AttributeType.IntVec3:
                    size *= 3;
                    break;
                case AttributeType.IntVec4:
                    size *= 4;
                    break;
                case AttributeType.Bool:
                    size *= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            stride += (uint) size * 4;
            layouts.Add(new ShaderLayout(name, size, AttribType.Float));
        }

        Stride = stride;
        Layout = layouts.ToArray();
    }

    public override void Dispose()
    {
        Gl.DeleteShader(Handle);
    }
}