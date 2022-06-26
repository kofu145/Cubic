using System.Collections.Generic;

namespace Cubic.Graphics.Platforms.OpenGL33;

public struct AttribSetup
{
    public uint TotalSize;

    public int[] Sizes;

    public AttribSetup(uint totalSize, List<int> sizes)
    {
        TotalSize = totalSize;
        Sizes = sizes.ToArray();
    }
}