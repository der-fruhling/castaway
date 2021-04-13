namespace Castaway.Level
{
    /// <summary>
    /// References an object by index and level. 
    /// </summary>
    /// <typeparam name="T">Type of the object this is referencing.</typeparam>
    public struct ObjectRef<T> where T : LevelObject
    {
        public readonly int Index;
        public readonly Level Level;

        /// <summary>
        /// Allows getting and setting the object behind this reference.
        /// </summary>
        public T Object
        {
            get => Level.Get<T>(Index);
            set => Level.Set(Index, value);
        }
        
        internal ObjectRef(int index, Level level)
        {
            Index = index;
            Level = level;
        }

        /// <summary>
        /// Converts this reference to type <typeparamref name="T"/> to a
        /// reference to type <typeparamref name="T1"/>
        /// </summary>
        /// <typeparam name="T1">New object type.</typeparam>
        /// <returns></returns>
        public ObjectRef<T1> To<T1>() where T1 : LevelObject => new ObjectRef<T1>(Index, Level);
    }
}