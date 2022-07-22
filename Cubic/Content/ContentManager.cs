using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Render;
using Cubic.Utilities;

namespace Cubic.Content;

public static class ContentManager
{
    private static Dictionary<string, object> _loadedCache;

    private static Dictionary<string, Bitmap> _bitmapsQueue;
    private static EventWaitHandle _waitHandle;

    static ContentManager()
    {
        _loadedCache = new Dictionary<string, object>();
        _bitmapsQueue = new Dictionary<string, Bitmap>();
        _waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
    }

    /*public static async Task LoadAsync(string name)
    {
        await Task.Run(() =>
        {
            Bitmap b = new Bitmap(name);
            _bitmapsQueue.Add(name, b);
        });
    }*/

    public static async Task LoadFilesAsync(string[] paths, Action<string, int> loadAction = null)
    {
        await Task.Run(() =>
        {
            int count = 0;
            foreach (string path in paths)
            {
                Bitmap b = new Bitmap(path);
                _waitHandle.WaitOne();
                _bitmapsQueue.Add(path, b);
                count++;
                loadAction?.Invoke(path, (int) ((count / (float) paths.Length) * 100));
            }

            _waitHandle.WaitOne();
        });
    }

    public static byte[] LoadEmbeddedResource(string assemblyName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyName);
        using MemoryStream memStr = new MemoryStream();
        stream.CopyTo(memStr);
        return memStr.GetBuffer();
    }

    public static void Update()
    {
        _waitHandle.Reset();
        foreach ((string name, Bitmap b) in _bitmapsQueue)
                _loadedCache.Add(name, new Texture2D(b));
        _bitmapsQueue.Clear();
        _waitHandle.Set();
    }
}