using System.IO;
using System.Reflection;

namespace Cubic.Content;

public static class ContentManager
{
    public static byte[] LoadEmbeddedResource(string assemblyName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyName);
        using MemoryStream memStr = new MemoryStream();
        stream.CopyTo(memStr);
        return memStr.GetBuffer();
    }
}