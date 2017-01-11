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
    }
}
