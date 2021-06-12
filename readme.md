# Castaway Game Engine

[![.NET](https://github.com/LiamCoalStudio/castaway/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LiamCoalStudio/castaway/actions/workflows/dotnet.yml)

## What is Castaway?

Castaway is a game engine using OpenGL. You can use it to make 3D graphics
applications with .NET Core. It supports custom shaders, `Level`s,
`Quaternion`s, and a whole bunch more. Castaway isn't ready though, there
is still a lot of work to be done, and my programming is almost certainly
not bug free.

If you find an issue with Castaway, report it! Feel free to not use the
issue templates if you don't want to, I don't really care as long as the
report is descriptive. Include an image and/or stacktrace if applicable.
_(This un-restrictiveness (is that even a word?) on issue templates is caused
in part by me not using the issue templates myself. :])_

## Using Castaway

Castaway has a NuGet package on GitHub Packages. To use this, you will first
have to set this up on your system.

[Instructions here! Click me!](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry)

Then, just install the `Castaway.Base` package from the source you added.

Something like:
```bash
dotnet add <ProjectFile> package -s <SourceName> Castaway.Base
```

This will give you a usable install of castaway.
