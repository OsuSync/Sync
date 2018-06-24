using Sync.Tools.Builtin;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    static class Updater
    {
        public const string SourceEXEName = "Sync.exe";
        public const string UpdateEXEName = "Sync_update.exe";
        public const string UpdateArg = "--update";
        public static readonly string CurrentEXEName = Path.GetFileName(Process.GetCurrentProcess().Modules[0].FileName);
        public static readonly string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string CurrentFullEXEPath = Path.Combine(CurrentPath, CurrentEXEName);
        public static readonly string CurrentFullSourceEXEPath = Path.Combine(CurrentPath, SourceEXEName);
        public static readonly string CurrentFullUpdateEXEPath = Path.Combine(CurrentPath, UpdateEXEName);
        public static bool IsUpdated = false;
        internal static PluginCommand update;

        public static bool ApplyUpdate(bool needUpdate)
        {
            if (CurrentEXEName == SourceEXEName)
            {
                if (needUpdate)
                {
                    Process.GetProcessesByName(UpdateEXEName).FirstOrDefault(p => p.MainModule.FileName.Contains(CurrentFullUpdateEXEPath))?.Kill();
                    File.Delete(CurrentFullUpdateEXEPath);
                    IsUpdated = true;
                }
                else if (File.Exists(CurrentFullUpdateEXEPath))
                {
                    try
                    {
                        Process.Start(CurrentFullUpdateEXEPath);
                        return true;
                    }
                    catch (Exception e)
                    {
                        File.Delete(Path.Combine(CurrentPath, UpdateEXEName));
                        IO.CurrentIO.WriteColor($"Apply update fail! Can't launch ({e.Message})", ConsoleColor.Red);
                        return false;
                    }
                }
            }
            else if (CurrentEXEName == UpdateEXEName)
            {
                Process.GetProcessesByName(SourceEXEName).FirstOrDefault(p => p.MainModule.FileName.Contains(CurrentFullSourceEXEPath))?.Kill();
                File.Delete(CurrentFullSourceEXEPath);
                File.Copy(CurrentFullUpdateEXEPath, CurrentFullSourceEXEPath);
                Process.Start(CurrentFullSourceEXEPath, UpdateArg);
                return true;
            }

            return false;
        }
    }
}
