using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class ReflectListAttribute : ListAttribute
    {
        public Type Type;
        public string ValueListName;
        public override string[] ValueList => Type.GetProperty(ValueListName, BindingFlags.Static | BindingFlags.Public)?.GetValue(null) as string[];

        public ReflectListAttribute()
        {
            NoCheck = true;
        }
    }
}
