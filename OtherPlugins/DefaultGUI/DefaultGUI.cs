using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync;
using System.Threading;
using System.Windows.Forms;
using Sync.Command;
using static DefaultGUI.Language;
using Sync.Source;

namespace DefaultGUI
{
    [Serializable]
    public class DefaultGUI : Plugin
    {
        public const string PLUGIN_NAME = "Default GUI";
        public const string PLUGIN_AUTHOR = "Deliay";

        public frmDefault frmUI;
        public Thread frmThread;

        public static bool InputFlag = true;

        public static SyncHost hoster = null;

        public DefaultGUI() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {
            Sync.Tools.I18n.Instance.ApplyLanguage(new Language());
            frmUI = new frmDefault();
            frmThread = new Thread(ShowForm);
            frmThread.SetApartmentState(ApartmentState.STA);
            frmThread.Name = "STAThreadForm";

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(evt => Task.Run(() => { hoster = evt.Host; frmUI.ready(); }));

            EventBus.BindEvent<PluginEvents.InitCommandEvent>(evt => Task.Run(() => {
                evt.Commands.Dispatch.bind("gui", (t) =>
                {
                    frmUI.ShowMe(); return true;
                }, UI_DISPLAY);
            }));

            SourceEvents.Instance.BindEvent<StartSyncEvent>(evt => Task.Run(() => frmUI.UpdateStautsAuto()));

            SourceEvents.Instance.BindEvent<StopSyncEvent>(evt => Task.Run(() => frmUI.UpdateStautsAuto()));

            frmThread.Start();
        }

        [STAThread]
        public void ShowForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(frmUI);
        }
    }
}
