using System;
using System.Runtime.InteropServices;

namespace Castaway.Native
{
    public unsafe class FreeOp : IDisposable
    {
        private readonly void* _ptr;

        public FreeOp(void* ptr) => _ptr = ptr;

        private void ReleaseUnmanagedResources()
        {
            Marshal.FreeHGlobal((IntPtr) _ptr);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~FreeOp()
        {
            ReleaseUnmanagedResources();
        }
    }
    
    public static unsafe class Memory
    {
        public static void* Alloc(nint size) => 
            Marshal.AllocHGlobal(size).ToPointer();

        public static T* Alloc<T>(nint count = 1) where T : unmanaged =>
            (T*) Alloc(sizeof(T) * count);

        public static void Free(void* ptr) =>
            Marshal.FreeHGlobal(new IntPtr(ptr));

        public static void Expand(void* ptr, nint size) =>
            Marshal.ReAllocHGlobal(new IntPtr(ptr), size);

        public static void Expand<T>(void* ptr, nint count) where T : unmanaged =>
            Expand(ptr, sizeof(T) * count);

        public static FreeOp Alloc(out void* ptr, nint size) =>
            new(ptr = Alloc(size));
        
        public static FreeOp Alloc<T>(out T* ptr, nint count) where T : unmanaged =>
            new(ptr = Alloc<T>(count));

        public static void Copy(void* dest, void* src, nint size)
        {
            byte* a = (byte*) src, b = (byte*) dest;
            for (nint i = 0; i < size; i++) b[i] = a[i];
        }

        public static void Copy<T>(T* dest, void* src, nint count = 1) where T : unmanaged =>
            Copy((void*)dest, src, count * sizeof(T));

        public static void Copy<T>(T* dest, T[] src) where T : unmanaged
        {
            fixed(T* s = src) Copy(dest, s, src.Length);
        }

        public static void Free(params void*[] ptrs)
        {
            foreach (var ptr in ptrs) Free(ptr);
        }
    }
}