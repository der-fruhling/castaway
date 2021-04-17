using System;
using Castaway.Core;
using Castaway.Exec;
using Castaway.Input;
using Castaway.Levels;
using Castaway.Math;
using Castaway.Render;
using static Castaway.Assets.AssetManager;
using static Castaway.Math.Matrix4;

[ControllerInfo(Name = "Camera Controller")]
internal class CameraController : Controller
{
    private Vector3 _currentForward = null!;
    
    public override void OnUpdate()
    {
        _currentForward ??= parent.ForwardVector;
        base.OnUpdate();
        // Lighting.Add(new Lighting.DirectionalLightConfig(
        //     new Vector3(1, 1, 1),
        //     new Vector3(1, 1, 1),
        //     new Vector3(1, 1, 1),
        //     parent.ForwardVector));
        // Lighting.Add(new Lighting.PointLightConfig(
        //     new Vector3(1, 1, 1),
        //     new Vector3(1, 1, 1),
        //     new Vector3(1, 1, 1),
        //     parent.Position,
        //     1,
        //     0.09f,
        //     0.032f));
        Lighting.Add(new Lighting.SpotlightConfig(
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 1),
            parent.Position,
            _currentForward = Vector3.Lerp(_currentForward, parent.ForwardVector, .15f),
            MathF.Cos(CMath.Radians(27.5f)),
            MathF.Cos(CMath.Radians(32.5f))));
    }
}

[RequiresModules(
    CModule.Assets, 
    CModule.Render, 
    CModule.Mesh, 
    CModule.Serializable, 
    CModule.Input, 
    CModule.Level)]
[Entrypoint]
internal class ProgramEntrypoint
{
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
        _shaderHandle.SetUniform("spotlight.cutOff", MathF.Cos(CMath.Radians(32.5f)));
        _shaderHandle.SetUniform("spotlight.outerCutOff", MathF.Cos(CMath.Radians(40f)));
        _shaderHandle.SetUniform("spotlight.ambient", 3, 1f, 1f, 1f);
        _shaderHandle.SetUniform("spotlight.diffuse", 3, 1f, 1f, 1f);
        _shaderHandle.SetUniform("spotlight.specular", 3, 1f, 1f, 1f);

        _level = Get<Level>(Index("/test.lvl"));
        if (_level == null) throw new ApplicationException("Failed to load level");

        Events.CloseNormally += _level.Deactivate;
        _level.Activate();
    }

    // ReSharper disable UnusedMember.Global
    private static float _speed = .025f;

    public static void SpacebarHandler(LevelObject m, Keys k) => m.Position.Y += _speed;
    public static void LeftShiftHandler(LevelObject m, Keys k) => m.Position.Y -= _speed;
    public static void WHandler(LevelObject m, Keys k) => m.Position += RotateYDeg(m.Rotation.Y) * RotateXDeg(m.Rotation.X) * new Vector3(0, 0, _speed);
    public static void SHandler(LevelObject m, Keys k) => m.Position -= RotateYDeg(m.Rotation.Y) * RotateXDeg(m.Rotation.X) * new Vector3(0, 0, _speed);
    public static void DHandler(LevelObject m, Keys k) => m.Position += RotateYDeg(m.Rotation.Y) * RotateXDeg(m.Rotation.X) * new Vector3(_speed, 0, 0);
    public static void AHandler(LevelObject m, Keys k) => m.Position -= RotateYDeg(m.Rotation.Y) * RotateXDeg(m.Rotation.X) * new Vector3(_speed, 0, 0);

    public static void SpeedUp(LevelObject m, Keys k) => _speed = .05f;
    public static void SlowDown(LevelObject m, Keys k) => _speed = .025f;
    
    public static void UpHandler(LevelObject m, Keys k) => m.Rotation.X -= 3f;
    public static void DownHandler(LevelObject m, Keys k) => m.Rotation.X += 3f;
    public static void LeftHandler(LevelObject m, Keys k) => m.Rotation.Y -= 3f;
    public static void RightHandler(LevelObject m, Keys k) => m.Rotation.Y += 3f;
    // ReSharper restore UnusedMember.Global
}
