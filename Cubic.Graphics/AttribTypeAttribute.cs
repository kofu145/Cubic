using System;

namespace Cubic.Graphics;

public class AttribTypeAttribute : Attribute
{
    public readonly AttribType Type;

    public AttribTypeAttribute(AttribType type)
    {
        Type = type;
    }
}