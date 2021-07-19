namespace Castaway.Components
{
    public class Events : IContainer
    {
        public delegate void TestDelegate();

        [Publish("test")] public event TestDelegate TestEvent;

        public virtual void InvokeTest() => TestEvent?.Invoke();
    }
}