using Sync.Tools.ConfigGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property, AllowMultiple = false)]
    public class HideAttribute : Attribute
    {
    }
}
