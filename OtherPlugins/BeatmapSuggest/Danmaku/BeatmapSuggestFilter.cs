using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sync.MessageFilter;
using Sync.Source;
using Sync.Plugins;
using System.Threading;
using System.Net;
using Sync;
using System.IO;
using Sync.Tools;
using static BeatmapSuggest.DefaultLanguage;

namespace BeatmapSuggest.Danmaku
{
    class BeatmapSuggestFilter : IFilter, ISourceDanmaku
    {
        private MessageDispatcher msgManager = null;

        private const string suggestCommand = "?suggest";

        const int timeout = 6000;//ms

        static Regex titleRegex = new Regex(@"Title.+\<a\s+href='/p/beatmaplist\?q=(?<beatmapName>.+)'>\1\</a\>");

        public void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText;
            int beatmapSetId = 0;
            if (message.StartsWith(suggestCommand))
            {
                msg.cancel = true;
                if (msgManager == null)
                    return; //没完全初始化，发送不了信息

                if (Int32.TryParse(message.Substring(suggestCommand.Length).Trim(), out beatmapSetId))
                {
                    SendSuggestMessage(beatmapSetId, msg.user.RawText);
                }
            }
        }

        private async void SendSuggestMessage(int beatmapSetId, string userName)
        {
            string beatmapName = String.Empty;
            try
            {
                beatmapName = await GetBeatmapName(beatmapSetId);
            }
            catch (Exception e)
            {
                Console.WriteLine(LANG_GET_BEATMAP_FAILED, beatmapSetId, e.Message);
                return;
            }
            CBaseDanmuku danmaku = new CBaseDanmuku();
            /*
            StringBuilder sb = new StringBuilder();
            sb.Append(userName).Append(" want you to play the beatmap [").Append(GetLink(beatmapSetId)).Append(" ").Append(beatmapName).Append("] || [")
                .Append(GetDownloadLink(beatmapSetId)).Append(" dl] || [").Append(GetMirrorDownloadLink(beatmapSetId)).Append(" mirror]");
            */
            string message = string.Format(LANG_SUGGEST_MEG,userName,GetLink(beatmapSetId),beatmapName,GetDownloadLink(beatmapSetId),GetMirrorDownloadLink(beatmapSetId));
            danmaku.danmuku = /*sb.ToString()*/message;
            msgManager.RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
        }

        private string GetLink(int beatmapSetId)
        {
            return "https://osu.ppy.sh/s/" + beatmapSetId;
        }

        private string GetDownloadLink(int beatmapSetId)
        {
            return "https://osu.ppy.sh/d/" + beatmapSetId;
        }

        private string GetMirrorDownloadLink(int beatmapSetId)
        {
            return "http://osu.mengsky.net/api/download/" + beatmapSetId;
        }

        private async Task<string> GetBeatmapName(int beatmapSetId)
        {
            var timeoutCancellation = new CancellationTokenSource();

            var task = new Task<string>(() =>
            {
                string url = GetLink(beatmapSetId);
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                StreamReader reader = null;
                try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                    response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        reader = new StreamReader(response.GetResponseStream());
                        string buffer = reader.ReadToEnd();
                        Match result = titleRegex.Match(buffer);
                        if (result.Success)
                            return result.Groups["beatmapName"].Value;
                        else
                            throw new Exception(LANG_NOT_FOUND_ERR);
                    }
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (response != null)
                        response.Close();
                }
                return "<unk title>";
            }, timeoutCancellation.Token);

            timeoutCancellation.Token.Register(() => {
                if (!task.IsCompleted || task.IsFaulted)
                    Console.WriteLine(LANG_GET_BEATMAP_TIME_OUT, beatmapSetId, task.Status.ToString());
            });

            task.Start();

            timeoutCancellation.CancelAfter(timeout);

            return await task;
        }

        public void SetFilterManager(MessageDispatcher manager)
        {
            msgManager = manager;
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}
