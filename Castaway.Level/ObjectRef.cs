namespace Castaway.Level
{
    public struct ObjectRef<T> where T : LevelObject
    {
        internal int Index;
        public Level Level { get; internal set; }

        public T Object
        {
            get => Level.Get<T>(Index);
            set => Level.Set(Index, value);
        }
        
        public ObjectRef(int index, Level level)
        {
            Index = index;
            Level = level;
        }

        public ObjectRef<T1> To<T1>() where T1 : LevelObject => new ObjectRef<T1>(Index, Level);
    }
}