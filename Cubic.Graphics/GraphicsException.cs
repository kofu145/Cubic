using System;
using System.Runtime.Serialization;

namespace Cubic.Graphics;

public class GraphicsException : Exception
{
    public GraphicsException() { }
    protected GraphicsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public GraphicsException(string message) : base(message) { }
    public GraphicsException(string message, Exception innerException) : base(message, innerException) { }
}