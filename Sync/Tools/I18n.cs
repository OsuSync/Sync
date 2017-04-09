using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    class IniLanguageFile
    {
        private Dictionary<string, string> lang;

        public IniLanguageFile(Dictionary<string, string> collections)
        {
            lang = collections;
        }

        public IniLanguageFile(string FilePath, bool Mode)
        {

        }

        public IniLanguageFile(string content)
        {

        }
    }

    class I18nProvider
    {
        public string Name { get; private set; }
        public Dictionary<string, string> Langs { get; private set; }

    }

    static class I18n
    {
        static Dictionary<string, I18nProvider> providers = new Dictionary<string, I18nProvider>();
        static void RegisterProvider(I18nProvider provider)
        {
            providers.Add(provider.Name, provider);
        }

        static string GetLanguage(string Key)
        {
            foreach(var item in providers)
            {
                foreach (var p in item.Value.Langs)
                {
                    if (p.Key == Key) return p.Value;
                }
            }

            return string.Empty;
        }

        static string GetLanguage(I18nProvider provider, string Key)
        {
            return provider.Langs[Key];
        }

        static I18nProvider Find(string Name)
        {
            return providers[Name];
        }
    }
}
