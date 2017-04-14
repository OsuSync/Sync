using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    /*
    //class LangSet
    //{
    //    private Dictionary<Guid, string> Langs = new Dictionary<Guid, string>();

    //    Type t;
    //    public LangSet(Type parent)
    //    {
    //        t = parent;
    //    }

    //    internal virtual string this[Guid uid]
    //    {
    //        get
    //        {
    //            return Langs[uid];
    //        }
    //    }

    //    internal virtual void Add(Guid uid, string defaultValue)
    //    {
    //        Langs.Add(uid, defaultValue);
    //    }
    //}

    //class IniLangSet : LangSet
    //{
    //    string FilePath = string.Empty;
    //    public IniLangSet(Type parent, string FilePath) : base(parent)
    //    {
    //        this.FilePath = FilePath;
    //    }

    //    internal override string this[Guid uid]
    //    {
    //        get { return ConfigurationIO.IniReadValue(FilePath, uid.ToString(), "Language"); }
    //    }

    //    internal override void Add(Guid uid, string defaultValue)
    //    {
    //        ConfigurationIO.Write(FilePath, uid.ToString(), defaultValue, "Language");
    //    }
    //}


    class DefaultLanguage : Plugins.Plugin
    {
        internal DefaultLanguage() : base("DefaultLang", "Deliay")
        {
        }
    }
    */

    public class DefaultI18n : I18nProvider
    {
        public static LanguageElement LANG_Loading = "读取中....";
        public static LanguageElement LANG_Plugins = "已载入 {0:D} 个 插件";
        public static LanguageElement LANG_Sources = "已载入 {0:D} 个 直播源";
        public static LanguageElement LANG_Error = "不能初始化连接器，请确认是否已经安装直播源.";
        public static LanguageElement LANG_Commands = "已载入 {0:D} 个 命令";
        public static LanguageElement LANG_Filters = "已载入 {0:D} 个 过滤器";
        public static LanguageElement LANG_Ready = "准备就绪。";

        public static LanguageElement LANG_Start = "开始工作....";
        public static LanguageElement LANG_Stopping = "停止工作...";
        public static LanguageElement LANG_Restarting = "重新开始工作...";

        public static LanguageElement LANG_LoadingPlugin = "载入 {0:S} 中...";
        public static LanguageElement LANG_LoadPluginErr = "不能载入 {0:S} ({1:S})";
        public static LanguageElement LANG_NotPluginErr = "插件{0:S} 非osuSync插件 ({1:S})";

        public static LanguageElement LANG_NotConfig = "请配置 'config.ini' 后再开始进行同步操作。";
        public static LanguageElement LANG_NoSource = "无法找到任何直播源！请安装一个直播源。";
        public static LanguageElement LANG_MissSource = "找不到默认匹配的直播源，直接使用第一个。";
        public static LanguageElement LANG_SetSource = "设置 {0:S} 为直播弹幕源";
        public static LanguageElement LANG_SupportSend = "提示:当前弹幕源支持游戏内发送到弹幕源的功能，请输入login [用户名] [密码] 来登录!(用户名、密码二者可选输入)";
        public static LanguageElement LANG_CertLength = "Certification长度: {0:D}";
        public static LanguageElement LANG_CertExist = "提示：当前已有登录Certification记录，如需覆盖，请输入login [用户名] [密码]进行覆盖！（用户名密码可选输入）";

        public static LanguageElement LANG_UnknowCommand = "未知命令！ 请输入help查看命令列表。";
        public static LanguageElement LANG_CommandFail = "命令执行失败！ 请输入help查看命令列表。";

        public static LanguageElement LANG_ConfigFile = "配置文件";
    }


    interface I18nProvider
    {
    }

    public class LanguageElement
    {
        private string value;

        public LanguageElement(string defaultVal)
        {
            value = defaultVal;
        }

        public static implicit operator LanguageElement(string val)
        {
            return new LanguageElement(val);
        }

        public static implicit operator string(LanguageElement element)
        {
            return element.value;
        }
    }

    class I18n
    {
        public static string CurrentLang { get { return System.Globalization.CultureInfo.CurrentCulture.Name; } }
        string Base { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        string LangFolder { get { return Path.Combine(Base, "Language"); } }
        string SelectLangFolder {  get { return Path.Combine(LangFolder, SelectLang); } }
        public string SelectLang;

        public I18n(string CultureName)
        {
            SelectLang = CultureName;
            if (!Directory.Exists(LangFolder)) Directory.CreateDirectory(LangFolder);
            if (!Directory.Exists(SelectLangFolder)) Directory.CreateDirectory(SelectLangFolder);
        }

        public void ApplyLanguage<T>(T instance) where T : I18nProvider
        {
            string LangFile = Path.Combine(SelectLangFolder, instance.ToString()) + ".lang";
            foreach (FieldInfo item in typeof(T).GetFields())
            {
                if(item.FieldType.Equals(typeof(LanguageElement)))
                {
                    string value = ConfigurationIO.IniReadValue(LangFile, item.Name, SelectLang);
                    if (value == "")
                    {
                        value = item.GetValue(instance) as LanguageElement;
                        ConfigurationIO.Write(LangFile, item.Name, value, SelectLang);
                    }
                    item.SetValue(instance, new LanguageElement(value));
                }
            }
        }


    }
}
