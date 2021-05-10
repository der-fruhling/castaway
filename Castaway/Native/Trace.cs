namespace Castaway.Native
{
    public static class Trace
    {
        public static void Enable() => CawNative.cawSetTrace(true);
        public static void Disable() => CawNative.cawSetTrace(false);
        public static void Write(string s) => CawNative.cawWriteTrace(s);
    }
}