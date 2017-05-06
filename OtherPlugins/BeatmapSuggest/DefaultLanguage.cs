using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;

namespace BeatmapSuggest
{
    public class DefaultLanguage:I18nProvider
    {
        public static LanguageElement LANG_GET_BEATMAP_FAILED = "获取谱面{0}信息失败,原因:{1}";
        public static LanguageElement LANG_SUGGEST_MEG = "{0} want you to play the beatmap [{1} {2}] || [{3} dl] || [{4} mirror]";
        public static LanguageElement LANG_NOT_FOUND_ERR = "找不到匹配的内容或者id并不是有效的beatmapSetId";
        public static LanguageElement LANG_UNKNOWN_TITLE = "<unk title>";
        public static LanguageElement LANG_GET_BEATMAP_TIME_OUT = "获取谱面{0}信息超时,TaskStatus{1}";  
    }
}
