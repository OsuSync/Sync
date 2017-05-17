using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public class FtpClient
    {
        NetworkCredential credential;

        string ftpLink;

        public FtpClient(string ftplink,string user,string password)
        {
            credential = new NetworkCredential("update", "updatesync!!!");
            ftpLink = ftplink;
        }

        public List<string> GetFilesList(string path)
        {
            path = path[0] == '/' ? path.Substring(1) : path;

            List<string> result = new List<string>();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpLink + path);
            request.Credentials = credential;
            request.Proxy = null;
            request.EnableSsl = false;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        result.Add(reader.ReadLine());
                    };
                };
            }
            return result;
        }

        public byte[] GetFile(string fullPath)
        {
            List<string> result = new List<string>();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpLink + fullPath);
            request.Credentials = credential;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            MemoryStream memory = null;
            int bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            int recvByte;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (memory = new MemoryStream())
                {
                    using (Stream stream = (response.GetResponseStream()))
                    {
                        while (true)
                        {
                            recvByte = stream.Read(buffer, 0, bufferSize);
                            if (recvByte == 0)
                                return memory.ToArray();
                            memory.Write(buffer, 0, recvByte);
                        }
                    }
                };
            }
        }
    }
}
