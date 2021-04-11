using System;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Exec;
using Castaway.Level;
using Castaway.Render;
using static Castaway.Render.VertexAttribInfo.AttribValue;

[ControllerInfo(Name = "Test Controller", Icon = "/test.controller.png")]
public class TestController : Controller
{
    public override void OnBegin()
    {
        base.OnBegin();
        Console.WriteLine("OnBegin");
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Console.WriteLine("OnEnd");
    }

    public override void OnDraw()
    {
        base.OnDraw();
        Console.WriteLine("OnDraw");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Console.WriteLine("OnUpdate");
    }
}

internal static class Program
{
    private static ShaderHandle _shaderHandle;
    
    private static void Main(string[] args)
    {
        var level = new Level();
        var @ref = level.Create();
        
        var o = @ref.Object;
        o.Add<TestController>();
        @ref.Object = o;
        
        Events.PostInit += level.Activate;
        Events.Init += () =>
        {
            _shaderHandle = AssetManager.Get<LoadedShader>("/test.shdr")?.ToHandle();
            if (_shaderHandle == null) throw new ApplicationException("Shader failed to read");
            _shaderHandle.Use();
        };
        Events.Draw += () =>
        {
            var vbo = new VBO();
            vbo.Add(0, 0, r: 1, g: 0, b: 0);
            vbo.Add(0, 1, r: 0, g: 1, b: 0);
            vbo.Add(1, 0, r: 0, g: 0, b: 1);
            vbo.Add(1, 1, r: 1, g: 1, b: 1);
            vbo.Add(0, 1, r: 0, g: 1, b: 0);
            vbo.Add(1, 0, r: 0, g: 0, b: 1);
            vbo.Draw();
        };
        Events.CloseNormally += level.Deactivate;
        Cast.Start();
    }
}
