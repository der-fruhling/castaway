# Castaway Game Engine

[![.NET](https://github.com/LiamCoalStudio/castaway/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LiamCoalStudio/castaway/actions/workflows/dotnet.yml)

## What is Castaway?

Castaway is a game engine using OpenGL. You can use it to make 3D graphics
applications with .NET Core. It supports custom shaders, `Level`s,
`Quaternion`s, and a whole bunch more. Castaway isn't ready though, there
is still a lot of work to be done, and my programming is almost certainly
not bug free.

## Using Castaway

Castaway has a number of NuGet packages published to GitHub packages. You
will need `Castaway.OpenGL` to use castaway. It includes an OpenGL implementation
for the Castaway Graphics API, as well as depending on everything else you need.

Here's an example:

```c#
using Castaway.Assets;
using Castaway.OpenGL;
using Castaway.Rendering;

// The Imports attribute is a workaround to the compiler discarding some
// referenced assemblies unless they are used. API Implementations are
// only created through reflection and therefore the compiler does not
// know that it's being used, and will discard the reference.
//
// Referencing OpenGLImpl here uses a type from the OpenGL implementation,
// which avoids this whole mess. This is the only purpose Imports serves.
[Imports(typeof(OpenGLImpl))]
internal static class Program
{
    private static void Main(string[] args)
    {
        // Initialize Assets
        AssetLoader.Init();

        // Create window.
        using var window = new Window(800, 600, "Example", false);
        window.Bind();

        // Get Graphics implementation. The one used here will depend on your
        // systems capabilities, up to OpenGL 4.2. The minimum for OpenGL is
        // version 3.2.
        var g = window.GL;
        
        // ** Any initialization magic would happen right here. **

        // Show window and start render loop.
        window.Visible = true;
        while (!window.ShouldClose)
        {
            g.StartFrame();
            // ** Rendering and Updateing would go right here. **
            g.FinishFrame(window);
        }
        
        // ** Object destruction would go right here. **
        
        // (`window` is automatically destroyed because of the `using` on it's
        //  declaration. If `using` is omitted, call discard on the object
        //  *before* Main exits.)
    }
}
```
