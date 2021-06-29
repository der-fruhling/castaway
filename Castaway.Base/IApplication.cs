using System;

namespace Castaway.Base
{
    public interface IApplication : IDisposable
    {
        public bool ShouldStop { get; }
        public void Init();
        public void StartFrame();
        public void Render();
        public void Update();
        public void EndFrame();
        public void Recover(RecoverableException e);
    }
}