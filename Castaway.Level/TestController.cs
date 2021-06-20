using System;
using System.Diagnostics;

namespace Castaway.Level
{
    [DebuggerDisplay("Test1={" + nameof(Test1) + "}, Test2={" + nameof(Test2) + "}")]
    [ControllerName("Test")]
    public class TestController : Controller
    {
        [LevelSerialized("Test1")]
        public int Test1;
        
        [LevelSerialized("Test2")]
        public int Test2;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            Console.WriteLine($"Init() = {ToString()}");
        }

        public override void OnDestroy(LevelObject parent)
        {
            base.OnDestroy(parent);
            Console.WriteLine($"Destroy() = {ToString()}");
        }

        public override string ToString()
        {
            return $"{nameof(TestController)}{{{nameof(Test1)}: {Test1}, {nameof(Test2)}: {Test2}}}";
        }
    }
}