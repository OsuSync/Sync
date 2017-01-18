using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryReader.Memory
{
    public static class Win32API
    {

        public static class Helper
        {
            public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
                return stuff;
            }
        }

        [Flags]
        public enum ProcessAccessType
        {
            PROCESS_TERMINATE = (0x0001),
            PROCESS_CREATE_THREAD = (0x0002),
            PROCESS_SET_SESSIONID = (0x0004),
            PROCESS_VM_OPERATION = (0x0008),
            PROCESS_VM_READ = (0x0010),
            PROCESS_VM_WRITE = (0x0020),
            PROCESS_DUP_HANDLE = (0x0040),
            PROCESS_CREATE_PROCESS = (0x0080),
            PROCESS_SET_QUOTA = (0x0100),
            PROCESS_SET_INFORMATION = (0x0200),
            PROCESS_QUERY_INFORMATION = (0x0400)
        }

        [Flags]
        public enum THREADINFOCLASS
        {
            ThreadBasicInformation,
        };

        [StructLayout(LayoutKind.Sequential)]
        struct CLIENT_ID
        {
            public IntPtr UniqueProcess;//ptr
            public IntPtr UniqueThread;//ptr
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct THREAD_BASIC_INFORMATION
        {
            public Int32 ExitStatus;
            public IntPtr TebBaseAddress;//ptr 4
            private CLIENT_ID clientId;
            public IntPtr AffinityMask;//ptr 4
            public Int32 Priority;
            public Int32 BasePriority;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEINFO
        {
            public IntPtr lpBaseOfDll;//ptr
            public UInt32 SizeOfImage;
            public IntPtr EntryPoint;//ptr
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

      //  [DllImport("kernel32.dll")] 不需要
      // public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);

        [DllImport("psapi.dll")]
        public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule,out MODULEINFO lpmodinfo, uint cb);

        [DllImport("ntdll.dll")]
        public static extern Int32 NtQueryInformationThread(IntPtr hThread,THREADINFOCLASS ThreadInformationClass,out THREAD_BASIC_INFORMATION ThreadInformation, UInt32 ThreadInformationLength,out IntPtr ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle,uint dwThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
