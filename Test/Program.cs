using System;
using Castaway.Core;
using Castaway.Exec;
using Castaway.Input;
using Castaway.Levels;
using Castaway.Math;
using Castaway.Render;
using static Castaway.Assets.AssetManager;

[RequiresModules(CModule.Assets, CModule.Render, CModule.Mesh, CModule.Serializable)]
[Entrypoint]
internal class ProgramEntrypoint
{
    private class MovementController : Controller
    {
        public override void OnUpdate()
        {
            base.OnUpdate();
            const float speed = .05f;
            const float lookSpeed = 1f;

            var movement = Vector3.Zero;
            if (InputSystem.Keyboard.IsPressed(Keys.W)) movement.Z += speed;
            if (InputSystem.Keyboard.IsPressed(Keys.S)) movement.Z -= speed;
            if (InputSystem.Keyboard.IsPressed(Keys.D)) movement.X += speed;
            if (InputSystem.Keyboard.IsPressed(Keys.A)) movement.X -= speed;

            if (InputSystem.Keyboard.IsPressed(Keys.Up))    parent.Rotation.X += lookSpeed;
            if (InputSystem.Keyboard.IsPressed(Keys.Down))  parent.Rotation.X -= lookSpeed;
            if (InputSystem.Keyboard.IsPressed(Keys.Left))  parent.Rotation.Y += lookSpeed;
            if (InputSystem.Keyboard.IsPressed(Keys.Right)) parent.Rotation.Y -= lookSpeed;

            parent.Position += Matrix4.RotateDeg(-parent.Rotation) * -movement;
        }
    }

    private ShaderHandle _shaderHandle;
    private int _shaderHandleAsset;
    private Level _level;

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
        ShaderManager.SetUniform(_shaderHandle, "lightPos", 0f, 0, 0);

        _level = Get<Level>(Index("/test.lvl"));
        if (_level == null) throw new ApplicationException("Failed to load level");
        
        Events.CloseNormally += _level.Deactivate;
        _level.Activate();
    }
}
