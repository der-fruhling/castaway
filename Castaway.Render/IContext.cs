using System;

namespace Castaway.Render
{
    public interface IContext
    {
        public struct KeyPress
        {
            public uint Key, Action;

            public KeyPress(uint key, uint action)
            {
                Key = key;
                Action = action;
            }
        }

        public event EventHandler<KeyPress> KeyHandler;
        
        public void MakeCurrent();
        public void Close();
    }
}