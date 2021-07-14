﻿using System;
using System.Diagnostics;
using Castaway.Assets;
using Castaway.Base;
using Castaway.Input;
using Castaway.Level;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.OpenGL.Controllers;
using Castaway.Rendering;
using GLFW;
using Window = Castaway.Rendering.Window;

namespace Test
{
    [Imports(typeof(OpenGLImpl), typeof(ShaderController))]
    internal class Program : IApplication
    {
        private Level? _level;

        private Stopwatch _stopwatch = new();

        private Window? _window;
#pragma warning disable 649
        // ReSharper disable once InconsistentNaming
        private Graphics g = null!;
#pragma warning restore 649

        public bool ShouldStop => _window?.ShouldClose ?? true;

        public void Init()
        {
            // Perform global initialization
            AssetLoader.Init();

            _window = new Window(800, 600, "name", false);
            _window.Bind();

            g = _window.GL;

            _level = new Level(AssetLoader.Loader!.GetAssetByName("/test_level.xml"));

            _level.Start();
            _window.Visible = true;

            InputSystem.Mouse.RawInput = true;

            _stopwatch.Start();
        }

        public void StartFrame()
        {
            g.StartFrame();
        }

        public void Render()
        {
            _level?.Render();
        }

        public void Update()
        {
            _level?.Update();
            if (_level is null) return;
            var s = _stopwatch.Elapsed.TotalSeconds * Math.PI * 2;
            const double hpi = Math.PI / 2;
            _level["Object"].Position =
                ProcessDance(_level["Object"].Position, Math.Sin(s));
            _level["Object Also"].Position =
                ProcessDance(_level["Object Also"].Position, Math.Sin(s + hpi));
            _level["Object Also Also"].Position =
                ProcessDance(_level["Object Also Also"].Position, Math.Sin(s + hpi * 2));
            _level["Object Also Also Also"].Position =
                ProcessDance(_level["Object Also Also Also"].Position, Math.Sin(s + hpi * 3));
            var rot = Quaternion.DegreesRotation(0, .75, 0);
            _level["Object"].Position = rot * _level["Object"].Position;
            _level["Object Also"].Position = rot * _level["Object Also"].Position;
            _level["Object Also Also"].Position = rot * _level["Object Also Also"].Position;
            _level["Object Also Also Also"].Position = rot * _level["Object Also Also Also"].Position;
            _level["Object"].Rotation = (_level["Object"].Rotation * rot).Normalize();
            _level["Object Also"].Rotation = (_level["Object Also"].Rotation * rot).Normalize();
            _level["Object Also Also"].Rotation = (_level["Object Also Also"].Rotation * rot).Normalize();
            _level["Object Also Also Also"].Rotation = (_level["Object Also Also Also"].Rotation * rot).Normalize();
        }

        public void EndFrame()
        {
            if (_window is null) return;
            if (InputSystem.Keyboard.WasJustPressed(Keys.J))
                CastawayGlobal.GetLogger().Debug(
                    "(Last Frame) {Time}, {FrameTime}, {Change} @ {Rate}fps",
                    CastawayGlobal.RealFrameTime,
                    CastawayGlobal.FrameTime,
                    g.FrameChange,
                    CastawayGlobal.Framerate);
            g.FinishFrame(_window);
            if (InputSystem.Gamepad.Valid && InputSystem.Gamepad.Start || InputSystem.Keyboard.IsDown(Keys.Escape))
                _window.ShouldClose = true;
        }

        public void Recover(RecoverableException e)
        {
            g.Clear();
            if (_window is not null) g.FinishFrame(_window);
        }

        public void Dispose()
        {
            if (_window is null) return;
            _window.Visible = false;
            _level?.End();
            _window.Dispose();
        }

        private static int Main()
        {
            return CastawayGlobal.Run<Program>();
        }

        private Vector3 ProcessDance(Vector3 orig, double value) => new(orig.X, value, orig.Z);
    }
}