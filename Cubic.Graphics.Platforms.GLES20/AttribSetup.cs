using System.Collections.Generic;
using Silk.NET.OpenGLES;

namespace Cubic.Graphics.Platforms.GLES20;

internal struct AttribSetup
{
    public uint TotalSize;

    public int[] Sizes;

    public VertexAttribPointerType[] Types;

    public AttribSetup(uint totalSize, List<int> sizes, List<VertexAttribPointerType> types)
    {
        TotalSize = totalSize;
        Sizes = sizes.ToArray();
        Types = types.ToArray();
    }
}