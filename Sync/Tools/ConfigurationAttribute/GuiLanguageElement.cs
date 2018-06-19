using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigurationAttribute
{
    public struct GuiLanguageElement
    {
        private LanguageElement element;

        public GuiLanguageElement(string defaultVal)
        {
            element = new LanguageElement(defaultVal);
        }

        public static implicit operator GuiLanguageElement(string val)
        {
            return new GuiLanguageElement(val);
        }

        public static implicit operator string(GuiLanguageElement element)
        {
            return element.ToString();
        }

        public override string ToString()
        {
            return element.ToString();
        }
    }
}
