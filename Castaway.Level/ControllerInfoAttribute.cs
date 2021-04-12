using System;

namespace Castaway.Level
{
    /// <summary>
    /// An attribute that programs can use to gather some data about a
    /// controller, such as a human readable name or an icon.
    /// </summary>
    public class ControllerInfoAttribute : Attribute
    {
        public string Name, Icon = "";
    }
}