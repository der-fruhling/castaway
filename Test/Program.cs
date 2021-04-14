using System;
using Castaway.Core;
using Castaway.Exec;
using Castaway.Input;
using Castaway.Level;
using Castaway.Level.Controllers.Renderers;
using Castaway.Level.Controllers.Rendering;
using Castaway.Mesh;
using Castaway.Render;
using static Castaway.Assets.AssetManager;

[RequiresModules(CModule.Assets, CModule.Render, CModule.Mesh)]
[Entrypoint]
internal class ProgramEntrypoint
{
    private class Movement2DController : Controller
    {
        public override void OnUpdate()
        {
            base.OnUpdate();
            const float speed = .05f;
            if (InputSystem.Keyboard.IsPressed(Keys.W)) parent.Position.Y += speed;
            if (InputSystem.Keyboard.IsPressed(Keys.S)) parent.Position.Y -= speed;
            if (InputSystem.Keyboard.IsPressed(Keys.D)) parent.Position.X += speed;
            if (InputSystem.Keyboard.IsPressed(Keys.A)) parent.Position.X -= speed;
        }
    }

    private ShaderHandle _shaderHandle;
    private int _shaderHandleAsset;
    private readonly Level _level = new Level();

    /*
     * The [EventHandler(...)] attribute allows defining a method in an
     * Entrypoint class as an event handler. It takes a parameter of
     * enum EventType.
     *
     * Here are all the usable events.
     * |  PreInit(), Init(), PostInit()
     * |  while window is open {
     * |      PreUpdate(), Update(), PostUpdate()
     * |      PreDraw(), Draw(), PostDraw()
     * |      Finish()
     * |  }
     * 
     * Not specifying which event to handle will automatically parse
     * the name of the method as an EventType. *Invalid EventTypes
     * generated this way will crash the program.*
     */
    [EventHandler]
    public void Init()
    {
        // Shader files:
        // /test.shdr - File containing `/test.shdr_d`, which is a directory
        //              containing all other shader files.
        // /test.shdr_d/shader.vsh - Vertex shader, allows transforming vertices.
        // /test.shdr_d/shader.fsh - Fragment shader, chooses colors to draw
        //                           for each pixel.
        // /test.shdr_d/shader.csh - Configuration file linking custom shaders
        //                           into the engine.
        _shaderHandleAsset = Index("/test.shdr");
        _shaderHandle = Get<LoadedShader>(_shaderHandleAsset)?.ToHandle();
        
        // If the loader couldn't load the shader correctly, null can be
        // returned. It is much more likely that an exception will just
        // be thrown instead, so this doesn't catch much.
        if (_shaderHandle == null) throw new ApplicationException("Shader failed to read");
        
        // Activates the shader.
        _shaderHandle.Use();

        var camera = _level.Create(new PerspectiveCameraController(0));
        camera.Object.Position.Z -= .75f;

        var m = Get<STLMesh>(Index("/test.stl"));
        foreach (var vertex in m!.Vertices)
        {
            Console.WriteLine(vertex);
        }
        
        var square = _level.Create(
            new MeshRenderer(m),
            new TransformController(),
            new Movement2DController());
        square.Object.Position.Z += 4f;
        
        Events.CloseNormally += _level.Deactivate;
        _level.Activate();
    }
}
