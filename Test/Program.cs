using System;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Exec;
using Castaway.Render;

internal static class Program
{
    private static ShaderHandle _shaderHandle;
    
    private static void Main(string[] args)
    {
        Events.Init += Init;
        Events.Draw += Draw;
        Cast.Start().Wait();
    }

    private static void Draw()
    {
        var vbo = new VBO();
        vbo.Add(-1, -1, r: 1, g: 0, b: 0);
        vbo.Add(-1, 1, r: 0, g: 1, b: 0);
        vbo.Add(1, -1, r: 0, g: 0, b: 1);
        vbo.Add(1, 1, r: 1, g: 1, b: 1);
        vbo.Add(-1, 1, r: 0, g: 1, b: 0);
        vbo.Add(1, -1, r: 0, g: 0, b: 1);
        vbo.Draw();
    }

    private static void Init()
    {
        _shaderHandle = AssetManager.Get<LoadedShader>("/test.shdr")?.ToHandle();
        if (_shaderHandle == null) throw new ApplicationException("Shader failed to read");
        _shaderHandle.Use();
    }
}
