using System;
using System.Diagnostics.CodeAnalysis;
using static Castaway.Native.CawNative;

namespace Castaway.Render
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public unsafe class Window : IDisposable
    {
        private window _window;

        public class Config : IDisposable
        {
            internal readonly window_conf* Caw;

            public Config(uint width, uint height, string title)
            {
                Caw = cawNewWindowConf();
                Caw->width = width;
                Caw->height = height;
                Caw->title = title;
            }

            public Config AnyGLVersion()
            { 
                Caw->glv = cawGLVersionAny();
                return this;
            }

            public Config GLVersion(uint major, uint minor)
            {
                Caw->glv = cawGLVersion2(major, minor);
                return this;
            }

            public Config Fullscreen()
            {
                Caw->fullscreen = true;
                return this;
            }

            public void Dispose()
            {
                cawDestroyWindowConf(Caw);
                GC.SuppressFinalize(this);
            }
        }

        public Window(Config c)
        {
            _window = cawOpenWindow(c.Caw);
        }

        public void Dispose()
        {
            Start();
            Close();
            GC.SuppressFinalize(this);
        }

        public bool ShouldBeOpen
        {
            get { fixed(window* w = &_window) return !cawWindowShouldClose(w); }
        }

        public Action<DrawContext> Render { private get; set; }

        public void Start()
        {
            while (ShouldBeOpen)
            {
                using(var d = new DrawContext()) Render!(d);
                fixed(window* w = &_window) cawFinishRender(w);
            }
        }

        public void Close()
        {
            fixed(window* w = &_window) cawCloseWindow(w);
        }
    }
}