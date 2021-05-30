using System;
using Castaway.Window;
using GLFW;

namespace Castaway.OpenGL
{
    public class GLWindow : BaseGLFWWindow
    {
        public uint Vao = uint.MaxValue;
        
        protected override void SetupWindowHints()
        {
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, GL.MaxSupportedVersion.Major);
            Glfw.WindowHint(Hint.ContextVersionMinor, GL.MaxSupportedVersion.Minor);
            if(GL.MaxSupportedVersion.Major == 3 && GL.MaxSupportedVersion.Minor >= 2 || GL.MaxSupportedVersion.Major > 3) 
                Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            else Glfw.WindowHint(Hint.OpenglProfile, Profile.Any);
        }

        protected override void UseContext()
        {
            Glfw.MakeContextCurrent(Window);
        }
    }
}