using System;
using Castaway.Core;
using Castaway.Exec;
using Castaway.Level;
using Castaway.Math;

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
    private static void Main(string[] args)
    {
        var level = new Level();
        var @ref = level.Create();
        
        var o = @ref.Object;
        o.Add<TestController>();
        @ref.Object = o;
        
        Events.PostInit += level.Activate;
        Events.CloseNormally += level.Deactivate;
        Cast.Start();
    }
}
