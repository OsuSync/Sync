using Sync.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using static Sync.Tools.IO;

namespace Sync
{
    public static class Program
    {
        static void Main(string[] args)
        {
            (new StartupHelper(args)).Start();
        }
    }
}
