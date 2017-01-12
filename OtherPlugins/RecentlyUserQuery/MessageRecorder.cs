using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlyUserQuery
{
    public class Record
    {
        public Record(int id,string userName,string message)
        {
            this.id = id;
            this.userName = userName;
            this.message = message;
        }

        public int id=-1;
        public string userName="???", message="";
    }

    public class MessageRecorder
    {
        private List<Record> historyList = new List<Record>();
   
        private int capacity = 15;

        public int Capacity
        {
            get { return capacity; }
            set
            {
                capacity = value<0?0:value;
                ChangeUpdate();
            }
        }

        private bool isRecording = true;

        public bool IsRecording
        {
            get { return isRecording; }
            set { isRecording = value; }
        }
        
        public List<Record> GetHistoryList()
        {
            return historyList;
        }

        private void ChangeUpdate()
        {
            while (Capacity < historyList.Count)
                historyList.RemoveAt(0);
        }

        public void Update(string userName,string message)
        {
            historyList.Add(new Record(UserIdGenerator.GetId(userName), userName, message));
            ChangeUpdate();
        }

        public void Clear()
        {
            historyList.Clear();
        }

        public string ProcessCommonCommand(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();
            int value = 0;
            switch (args[1])
            {
                case "--status":
                    return (string.Format("MessageRecord status: {0} | recordCount/Capacity : {1}/{2}", IsRecording ? "running" : "stopped", GetHistoryList().Count, Capacity));

                case "--disable":
                    IsRecording = false;
                    Clear();
                    UserIdGenerator.Clear();
                    return ("消息记录器已禁用，数据已清除");

                case "--start":
                    IsRecording = true;
                    return ("消息记录器开启");

                case "--realloc":
                    if (args.Length < 3)
                        return ("MessageRecord: 错误的指令");
                    else
                    {
                        value = Convert.ToInt32(args[2]);
                        Capacity = value;
                        return ("消息记录器现在可记录" + Capacity + "条历史记录");
                    }

                case "--i": //鸽一会
                    return ("咕咕咕~");

                case "--u": //鸽一会
                    return ("咕咕咕~");

                case "--recently":
                    return (EnumRecentUser().Result);

                default:
                    return ("未知命令");
            }
        }

        private async Task<string> EnumRecentUser()
        {
            var task = new Task<string>(() =>
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                foreach (var record in GetHistoryList())
                    result[record.userName] = (record.id);
                StringBuilder sb = new StringBuilder();
                foreach (var pair in result)
                    sb.AppendFormat("{0}->{1} || ", pair.Value, pair.Key);

                return sb.ToString();
            });
            task.Start();
            return await task;
        }
    }
}
