using System;

namespace Castaway.Core
{
    public class ControllerInfoAttribute : Attribute
    {
        public string Name, Icon = "";
    }
}