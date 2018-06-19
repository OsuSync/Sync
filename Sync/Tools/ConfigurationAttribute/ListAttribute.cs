using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigurationAttribute
{
    public class ListAttribute : BaseConfigurationAttribute
    {
        public virtual string[] ValueList { get; set; } = new string[] { };

        public bool IgnoreCase { get; set; } = false;

        public bool AllowMultiSelect { get; set; } = false;
        public char SplitSeparator { get; set; } = ',';

        public override bool Check(string val)
        {
            var m_val = IgnoreCase ? val.ToLower() : val;

            if ((ValueList.Length != 0))
            {
                if (AllowMultiSelect)
                {
                    foreach (var str in m_val.Split(new[] { SplitSeparator }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!ContainValue(m_val))
                        {
                            CheckFailedNotify(val);
                            return false;
                        }
                    }
                }
                else
                {
                    if (!ContainValue(m_val))
                    {
                        CheckFailedNotify(val);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ContainValue(string content)
        {
            return ValueList.Where((str) => (IgnoreCase ? str.ToLower() : str) == content).Count() != 0;
        }
    }
}
