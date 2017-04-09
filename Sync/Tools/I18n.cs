using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    class IniLanguageFile
    {
        private Dictionary<string, string> lang = new Dictionary<string, string>();

        public IniLanguageFile(Dictionary<string, string> collections)
        {

        }
    }

    class I18nProvider
    {
        public string Name { get; private set; }

    }

    static class I18n
    {
        static Dictionary<string, I18nProvider> providers = new Dictionary<string, I18nProvider>();
        static void RegisterProvider(I18nProvider provider)
        {
            providers.Add(provider.Name, provider);
        }

        static I18nProvider Find(string Name)
        {
            return providers[Name];
        }
    }
}
