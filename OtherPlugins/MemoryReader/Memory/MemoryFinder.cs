using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryReader.Memory
{
    class MemoryFinder
    {
        private Process m_osu_process=null;
        private IntPtr m_threadstack0_address=IntPtr.Zero;
        
        public MemoryFinder(Process osu)
        {
            m_osu_process = osu;
            m_threadstack0_address = GetThreadStack0Address();
        }

        public double GetMemoryDouble(List<Int32> offsets,bool use_threadstack0=true)
        {
            int i;
            int size;
            byte[] tmp;
            IntPtr next_ptr = IntPtr.Zero;

            if (m_osu_process == null)
                throw new OsuProcessNoFoundException();

            if (use_threadstack0)
            {
                if (m_threadstack0_address == IntPtr.Zero)
                    throw new ThreadStackNoFoundException();
                next_ptr = m_threadstack0_address;
            }

            for (i=0;i<offsets.Count()-1;++i)
            {
                tmp = ReadProcessMemory((IntPtr)((Int32)next_ptr + offsets[i]), 4, out size);
                next_ptr = (IntPtr)BitConverter.ToInt32(tmp,0);
            }

            tmp = ReadProcessMemory((IntPtr)((Int32)next_ptr + offsets[i]), 8, out size);
            return BitConverter.ToDouble(tmp,0);
        }

        public Int32 GetMemoryInt(List<Int32> offsets,bool use_threadstack0=true)
        {
            int i;
            int size;
            byte[] tmp;
            IntPtr next_ptr=IntPtr.Zero;

            if (m_osu_process == null)
                throw new OsuProcessNoFoundException();

            if (use_threadstack0)
            {
                if (m_threadstack0_address == IntPtr.Zero)
                    throw new ThreadStackNoFoundException();
                next_ptr = m_threadstack0_address;
            }

            for (i = 0; i < offsets.Count() - 1; ++i)
            {
                tmp = ReadProcessMemory((IntPtr)((Int32)next_ptr + offsets[i]), 4, out size);
                next_ptr = (IntPtr)BitConverter.ToInt32(tmp,0);
            }

            tmp = ReadProcessMemory((IntPtr)((Int32)next_ptr + offsets[i]),4, out size);
            return BitConverter.ToInt32(tmp,0);
        }

        private byte[] ReadProcessMemory(IntPtr address,uint length,out int size)
        {
            byte[] buffer = new byte[length];

            IntPtr ptrBytesRead;
            Win32API.ReadProcessMemory(m_osu_process.Handle, address, buffer, length, out ptrBytesRead);

            size = ptrBytesRead.ToInt32();

            return buffer;
        }

        private IntPtr GetThreadStackTop(IntPtr hThread)
        {
            Win32API.THREAD_BASIC_INFORMATION tbi = new Win32API.THREAD_BASIC_INFORMATION();

            IntPtr size;
            int status=Win32API.NtQueryInformationThread(hThread, Win32API.THREADINFOCLASS.ThreadBasicInformation,out tbi, 28, out size);

            if (status >= 0)
            {
                int unused = 0;
                byte[] stacktop=ReadProcessMemory((IntPtr)((Int32)tbi.TebBaseAddress + 4), 4, out unused);
                return (IntPtr)BitConverter.ToInt32(stacktop,0);
            }
            return IntPtr.Zero;
        }

        private IntPtr GetThreadStack0Address()
        {
            Win32API.MODULEINFO mi= new Win32API.MODULEINFO();
            IntPtr hThread=Win32API.OpenThread(Win32API.ThreadAccess.GET_CONTEXT | Win32API.ThreadAccess.QUERY_INFORMATION, false, (UInt32)m_osu_process.Threads[0].Id);
            Win32API.GetModuleInformation(m_osu_process.Handle, Win32API.GetModuleHandle("kernel32.dll"),out mi,12);
            IntPtr stacktop = GetThreadStackTop(hThread);

            Win32API.CloseHandle(hThread);
            

            if (stacktop!=IntPtr.Zero)
            {
                IntPtr size;
                byte[] buf = new byte[4096];
                Win32API.ReadProcessMemory(m_osu_process.Handle, (IntPtr)((Int32)stacktop - 4096), buf, 4096, out size);
                for(int i=4096/4-1;i>=0;--i)
                {
                    Int32 tmp = BitConverter.ToInt32(buf, i * 4);
                    if (tmp > (Int32)mi.lpBaseOfDll && tmp <= (Int32)mi.lpBaseOfDll + (Int32)mi.SizeOfImage)
                    {
                        return (IntPtr)((Int32)stacktop - 4096 + i * 4);
                    }
                }
            }
            return IntPtr.Zero;
        }
    }

    class OsuProcessNoFoundException : Exception
    {
        public override string Message
        {
            get
            {
                return "没有发现 OSU! 进程";
            }
        }
    }
    class ThreadStackNoFoundException : Exception
    {
        public override string Message
        {
            get
            {
                return "没有找到ThreadStack0 Base Address";
            }
        }
    }

}
