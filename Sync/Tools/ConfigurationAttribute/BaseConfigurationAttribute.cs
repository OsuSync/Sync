using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigurationAttribute
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public abstract class BaseConfigurationAttribute : Attribute
    {
        public bool RequireRestart { get; set; } = false;

        public bool Hide { get; set; }
        public bool NoCheck { get; set; }

        public virtual string CheckFailedFormatMessage { get; set; } = "Parse error:{0}";
        public abstract bool Check(string value);
        public void CheckFailedNotify(object obj) => IO.CurrentIO.WriteColor($"[Config]{string.Format(CheckFailedFormatMessage, obj.ToString())}", ConsoleColor.Red);
    }
}
