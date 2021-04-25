#nullable enable
using System.IO;

namespace PirateSLC
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            foreach (var s in args)
            {
                Compiler.CompileOut(File.ReadAllText(s));
            }
        }
    }
}