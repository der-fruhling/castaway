using System;
using System.Runtime.InteropServices;
using Castaway.Native;
using static Castaway.Native.CawNative;

namespace Castaway
{
    public static unsafe class Cast
    {
        public static void Init()
        {
            CawNative.Init();
            cawInit();
            ErrorHandlers.SetDefault();
        }

        public static void GetVersion(out int m, out int d, out int y, out int b)
        {
            int i, j, k, l;
            cawVersion(&i, &j, &k, &l);
            m = i;
            d = j;
            y = k;
            b = l;
        }

        public static string GetVersion()
        {
            return Marshal.PtrToStringAnsi((IntPtr) cawVersionStr());
        }
    }
}