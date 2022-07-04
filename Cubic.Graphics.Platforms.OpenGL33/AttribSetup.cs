using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Cubic.Graphics.Platforms.OpenGL33;

internal struct AttribSetup
{
    public uint TotalSize;

    public int[] Sizes;

    public int[] Locations;

    public VertexAttribPointerType[] Types;

    public AttribSetup(uint totalSize, List<int> locations, List<int> sizes, List<VertexAttribPointerType> types)
    {
        TotalSize = totalSize;
        Locations = locations.ToArray();
        Sizes = sizes.ToArray();
        Types = types.ToArray();
    }
}