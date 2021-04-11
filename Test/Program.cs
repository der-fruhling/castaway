using System;
using Castaway.Assets;
using Castaway.Exec;
using Castaway.Render;

[RequiresModules(CModule.Assets, CModule.Render)]
[Entrypoint]
internal class ProgramEntrypoint
{
    private ShaderHandle _shaderHandle;

    [EventHandler(EventType.Draw)]
    public void Draw()
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

    [EventHandler(EventType.Init)]
    public void Init()
    {
        _shaderHandle = AssetManager.Get<LoadedShader>("/test.shdr")?.ToHandle();
        if (_shaderHandle == null) throw new ApplicationException("Shader failed to read");
        _shaderHandle.Use();
    }
}
