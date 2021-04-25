namespace Castaway.Core
{
    /// <summary>
    /// Contains all events run in the event loop, and the event loop
    /// itself.
    /// </summary>
    public static class Events
    {
        public delegate void GenericHandler();
        public delegate bool BoolExpression();

        public static event GenericHandler Init;
        public static event GenericHandler PreInit;
        public static event GenericHandler PostInit;
        public static event GenericHandler Update;
        public static event GenericHandler PreUpdate;
        public static event GenericHandler PostUpdate;
        public static event GenericHandler Draw;
        public static event GenericHandler PreDraw;
        public static event GenericHandler PostDraw;
        public static event GenericHandler Finish;
        public static event GenericHandler CloseNormally;

        /// <summary>
        /// Used by <see cref="Loop"/> to determine whether it should keep
        /// running.
        /// </summary>
        public static BoolExpression ShouldClose = () => false;

        /// <summary>
        /// Starts the event loop.
        /// </summary>
        public static void Loop()
        {
            PreInit?.Invoke();
            Init?.Invoke();
            PostInit?.Invoke();
            while (!ShouldClose())
            {
                PreUpdate?.Invoke();
                Update?.Invoke();
                PostUpdate?.Invoke();
                
                PreDraw?.Invoke();
                Draw?.Invoke();
                PostDraw?.Invoke();
                
                Finish?.Invoke();
            }
            CloseNormally?.Invoke();
        }
    }
}