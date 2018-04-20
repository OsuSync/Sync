using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    public class GuiLanguageElement : LanguageElement
    {
        public GuiLanguageElement(string defaultVal) : base(defaultVal)
        {
        }

        public static implicit operator GuiLanguageElement(string val)
        {
            return new GuiLanguageElement(val);
        }

        public static implicit operator string(GuiLanguageElement element)
        {
            return element.ToString();
        }
    }
}
