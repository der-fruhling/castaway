# Castaway Game Engine

## What is Castaway?

Castaway is a game engine using OpenGL. You can use it to make 3D graphics
applications with .NET Core (C#). Castaway is still _heavily_ a work in
progress, so things might move around or be deleted, and most features are
not supported at all.

## Using Castaway

At the moment, almost everything in Castaway has to be created through an
instance of the `IGraphics<...>` interface, which also happens to be an
absolute mess of type parameters. The OpenGL API is implemented in
`Castaway.OpenGL.OpenGL`, with various `struct`s as it's object types.

First set up OpenGL, and save it's result to a variable:

```c#
var g = OpenGL.Setup();
```

Once this is done, create a window by calling the `CreateWindowWindowed`
method on `g`, and activate it with `Bind`.

```c#
var window = g.CreateWindowWindowed("Test", 800, 600);
g.Bind(window);
```

Creating a render loop is also easy, `g` provides a `WindowShouldBeOpen`
method, which returns `true` for as long as the user hasn't closed the
window, though a window can still stay open after this has happened.

At the start of your loop, you should call `StartFrame`, and at the end,
call `FinishFrame`.

```c#
while(g.WindowShouldBeOpen(window))
{
    g.StartFrame();
    // Rendering code would go here.
    g.FinishFrame(window);
}
```

Here's the full example:

```c#
using Castaway.OpenGL;
using Castaway.Rendering;

class Example
{
    static void Main()
    {
        var g = OpenGL.Setup();
        var window = g.CreateWindowWindowed("Test", 800, 600);
        g.Bind(window);

        while (g.WindowShouldBeOpen(window))
        {
            g.StartFrame();
            // Render code.
            g.FinishFrame(window);
        }
    }
}
```
