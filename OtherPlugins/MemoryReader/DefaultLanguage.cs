using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;

namespace MemoryReader
{
    public class DefaultLanguage : I18nProvider
    {
        public static LanguageElement LANG_OSU_NOT_FOUND = "没有发现 OSU! 进程";
        public static LanguageElement LANG_ADDRESS_NOT_FOUND = "没有找到ThreadStack0 Base Address";

        public static LanguageElement LANG_NOT_FOUND = "未找到";
        public static LanguageElement LANG_BEATMAP_NOT_FOUND = "memoryReader无法获取beatmap地址";
    }
}
