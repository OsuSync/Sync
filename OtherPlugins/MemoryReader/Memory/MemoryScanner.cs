using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryReader.Memory
{
    class MemoryScanner
    {
        public Int32 BeginAddress { get; set; }
        public Int32 EndAddress { get; set; }
        public Int32 InterVal { get; set; }
        public UInt32 BufferSize { get; set; }

        public delegate bool AddressFilterDelegate(Int32 address,Int32 target);
        public AddressFilterDelegate AddressFilter { set; get; }
        private Process m_osu_process;

        public MemoryScanner(Process osu_process)
        {
            if (Setting.OsuPath == "")
            {
                Setting.OsuPath = osu_process.MainModule.FileName.Replace(@"\osu!.exe","");
                Setting.SaveSetting();
            }
            m_osu_process = osu_process;
        }

        public List<Int32> Scan()
        {
            List<Int32> res=new List<Int32>();
            byte[] buffer;
            int size = 0;

            for(Int32 i=BeginAddress;i<EndAddress;i+=InterVal)
            {
                buffer=ReadProcessMemory((IntPtr)i, BufferSize, out size);

                for(int k=0;k< BufferSize; k+=4)
                {
                    Int32 num=BitConverter.ToInt32(buffer, k);
                    if (AddressFilter(i,num))
                        res.Add(i);
                }
            }
            return res;
        }

        private byte[] ReadProcessMemory(IntPtr address, uint length, out int size)
        {
            byte[] buffer = new byte[length];

            IntPtr ptrBytesRead;
            Win32API.ReadProcessMemory(m_osu_process.Handle, address, buffer, length, out ptrBytesRead);

            size = ptrBytesRead.ToInt32();

            return buffer;
        }
    }
}
