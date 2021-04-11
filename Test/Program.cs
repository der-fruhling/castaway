using System;
using Castaway.Assets;
using Castaway.Exec;
using Castaway.Render;

[RequiresModules(CModule.Assets, CModule.Render)]
[Entrypoint]
internal class ProgramEntrypoint
{
    private ShaderHandle _shaderHandle;

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
     */
    [EventHandler(EventType.Draw)]
    public void Draw()
    {
        // This function draws a full screen RGB rectangle.
        // (definitely does not include white on a corner)
        
        // Create a new VBO
        var vbo = new VBO();
        
        // Bottom left triangle
        vbo.Add(-1, -1, r: 1, g: 0, b: 0);
        vbo.Add(-1, 1, r: 0, g: 1, b: 0);
        vbo.Add(1, -1, r: 0, g: 0, b: 1);
        
        // Top right triangle.
        vbo.Add(1, 1, r: 1, g: 1, b: 1); // nothing to see here
        vbo.Add(-1, 1, r: 0, g: 1, b: 0);
        vbo.Add(1, -1, r: 0, g: 0, b: 1);
        
        // Draw!
        vbo.Draw();
    }

    /*
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
        _shaderHandle = AssetManager.Get<LoadedShader>("/test.shdr")?.ToHandle();
        
        // If the loader couldn't load the shader correctly, null can be
        // returned. It is much more likely that an exception will just
        // be thrown instead, so this doesn't catch much.
        if (_shaderHandle == null) throw new ApplicationException("Shader failed to read");
        
        // Activates the shader.
        _shaderHandle.Use();
    }
}
