#nullable enable
using System.IO;

namespace Castaway.PSLC
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