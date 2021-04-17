# Castaway Game Engine

Castaway is a modular 2D / 3D game engine.

## Building

To build Castaway, you can use Visual Studio or VS compatible IDEs (such as
JetBrains Rider).

## Using

### `Entrypoint`s

`Entrypoint` classes are where Castaway start looking for any
initialization code. They exist to differentiate normal classes from
startup classes.

Here's an example:
```c#
using System;
using Castaway.Exec;

// Entrypoint classes are looked through
[Entrypoint]
public class ReadmeTest
{
    // Entrypoint methods are run on startup.
    [Entrypoint]
    public void Entrypoint()
    {
        // Print if working.
        Console.WriteLine("Entrypoint hit!");
    }
}
```

Compile that into an assembly, and pass the path to that file as an
argument to Castaway.Exec. If done correctly, you should see:
```text
Entrypoint hit!
```
After it starts running, it will enter a loop. Just Ctrl-C to stop
it.

### The Event Loop

Events are an important part of Castaway. There are many events:
```c#
/* in Castaway.Core */
class Events
{
    event Init;
    event PreInit;
    event PostInit;
    event Update;
    event PreUpdate;
    event PostUpdate;
    event Draw;
    event PreDraw;
    event PostDraw;
    event Finish;
    event CloseNormally;
}
```

Any of these events can be subscribed to (or unsubscribed from) at
any time. Here's an example:

```c#
using System;
using Castaway.Core;
using Castaway.Exec;

[Entrypoint]
public class ReadmeTest
{
    [Entrypoint]
    public void Entrypoint()
    {
        Events.Init += () => Console.WriteLine("Init!");
    }
    
    // Alternate syntax for event handlers.
    [EventHandler(EventType.Init)]
    public void Init()
    {
        Console.WriteLine("Other init!");
    }
}
```

Run the same way as before. You should see:
```text
Init!
Other init!
```

### Using Modules

Not every module is initialized before your `Entrypoint` is run. There's an
attribute you can use that will tell `Castaway.Exec` which modules the class
needs set up before it can safely run. Such classes do not have to be
`Entrypoint`s. Here's an example:

```c#
using System;
using Castaway.Exec;

[Entrypoint]
// Enables the Assets module.
[RequiresModules(CModule.Assets)]
public class ReadmeTest
{
    // Assets module will be loaded.
    // Entrypoint methods cannot use features from modules.
}
```

### Assets

Assets are also important. They store the data that your game will use.

First, create a new folder called `Assets`. In that folder, create a file
called `Asset.txt`. In that file, put:
```text
Hello, World!
```

Then, run this:
```c#
using System;
using Castaway.Assets;
using Castaway.Exec;

[Entrypoint]
[RequiresModules(CModule.Assets)]
public class ReadmeTest
{
    [EventHandler(EventType.Init)]
    public void Init()
    {
        // Get the index of the asset.
        var assetIndex = AssetManager.Index("/Asset.txt");

        // Get the data contained in the asset.
        // `.txt` files load to string objects.
        var assetData = AssetManager.Get<string>(assetIndex);
        
        // Print
        Console.WriteLine(assetData);
    }
}
```

You should see:
```text
Hello, World!
```

### More examples

Take a look at the 
[examples repo](https://www.github.com/LiamCoalStudio/castaway-examples),
or try the `Test` project in the solution.
