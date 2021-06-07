using System;

namespace Castaway
{
    public struct ExecDispose : IDisposable
    {
        public Action Action;

        public ExecDispose(Action action)
        {
            Action = action;
        }

        public void Dispose()
        {
            Action();
        }

        public static implicit operator ExecDispose(Action a) => new(a);
    }
}