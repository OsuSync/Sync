using RecentlyUserQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BanManagerPlugin.Ban
{
    enum BanAccessType
    {
        NOTBANNED,//没被ban的可以发言给osu!irc
        WHITE_LIST,//只有白名单的人可以发言
        ALL//全都可以发言
    }

    class BanInfo
    {
        public class Rule
        {
            public Rule(string expr, int id)
            {
                this.id = id;
                expression = expr;
                regex = new Regex(expr);
            }
            public int id;
            public string expression;
            public Regex regex;
        }

        int rule_id_gerenator = 0;

        string splitString = @"_@!#_";

        List<string> banUserName = new List<string>();

        public List<string> GetBanUserList()
        {
            return banUserName;
        }

        List<Rule> banRuleRegex = new List<Rule>();

        public List<Rule> GetBanRuleRegexList()
        {
            return banRuleRegex;
        }

        List<string> whitelistUserName = new List<string>();

        public List<string> GetWhiteListUserList()
        {
            return whitelistUserName;
        }

        List<Rule> whitelistRuleRegex = new List<Rule>();

        public List<Rule> GetWhiteListRuleRegexList()
        {
            return whitelistRuleRegex;
        }

        BanAccessType accessType = BanAccessType.NOTBANNED;

        private int GenRuleId()
        {
            return rule_id_gerenator++;
        }

        /// <summary>
        /// 添加用户名到禁烟黑名单
        /// </summary>
        /// <param name="userName">用户名</param>
        public void AddBanUserName(string userName)
        {
            RemoveWhiteListUserName(userName);
            banUserName.Add(userName);
        }

        /// <summary>
        /// 添加禁言规则，符合正则表达式匹配的用户名都被加入黑名单
        /// </summary>
        /// <param name="ruleRegexExpr">正则表达式</param>
        /// <returns></returns>
        public int AddBanRuleRegex(string ruleRegexExpr)
        {
            Rule rule = new Rule(ruleRegexExpr, GenRuleId());
            banRuleRegex.Add(rule);
            return rule.id;
        }

        private int AddBanRuleRegex(string ruleRegexExpr, int id)
        {
            banRuleRegex.Add(new Rule(ruleRegexExpr, id));
            return id;
        }

        /// <summary>
        /// 添加用户名到白名单
        /// </summary>
        /// <param name="userName">用户名</param>
        public void AddWhiteListUserName(string userName)
        {
            RemoveBanUserName(userName);
            whitelistUserName.Add(userName);
        }

        /// <summary>
        /// 将某个用户从黑名单中移除
        /// </summary>
        /// <param name="userName">用户名</param>
        public void RemoveBanUserName(string userName)
        {
            if (banUserName.Contains(userName))
                banUserName.Remove(userName);
        }

        /// <summary>
        /// 将某个用户从白名单中移除
        /// </summary>
        /// <param name="userName">用户名</param>
        public void RemoveWhiteListUserName(string userName)
        {
            if (whitelistUserName.Contains(userName))
                whitelistUserName.Remove(userName);
        }

        /// <summary>
        /// 添加白名单规则，符合正则表达式匹配的用户名都被加入白名单
        /// </summary>
        /// <param name="ruleRegexExpr">正则表达式</param>
        /// <returns></returns>
        public int AddWhiteListRuleRegex(string ruleRegexExpr)
        {
            Rule rule = new Rule(ruleRegexExpr, GenRuleId());
            whitelistRuleRegex.Add(rule);
            return rule.id;
        }

        public void RemoveWhiteListRuleRegex(int ruleId)
        {
            for(int i = 0; i < whitelistRuleRegex.Count; i++)
            {
                if (whitelistRuleRegex[i].id == ruleId)
                {
                    whitelistRuleRegex.RemoveAt(i);
                    break;
                }
                    
            }
        }

        public void RemovBanListRuleRegex(int ruleId)
        {
            for (int i = 0; i < banRuleRegex.Count; i++)
            {
                if (banRuleRegex[i].id == ruleId)
                {
                    banRuleRegex.RemoveAt(i);
                    break;
                }

            }
        }

        private int AddWhiteListRuleRegex(string ruleRegexExpr, int id)
        {
            whitelistRuleRegex.Add(new Rule(ruleRegexExpr, id));
            return id;
        }

        public void AddWhiteListId(int id)
        {
            string userName = GetUserName(id);
            if (userName.Length != 0)
                AddWhiteListUserName(userName);
        }

        public void RemoveWhiteListId(int id)
        {
            string userName = GetUserName(id);
            if (userName.Length != 0)
                RemoveWhiteListUserName(userName);
        }

        public void AddBanId(int id)
        {
            string userName = GetUserName(id);
            if (userName.Length != 0)
                AddBanUserName(userName);
        }

        public void RemoveBanId(int id)
        {
            string userName = GetUserName(id);
            if (userName.Length != 0)
                RemoveBanUserName(userName);
        }

        /// <summary>
        /// 将当前过滤控制器的内容格式化打包，获得的字符串可以通过LoadFromFormattedString()回复 
        /// </summary>
        /// <returns></returns>
        public string SaveAsFormattedString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var userName in banUserName)
            {
                sb.AppendFormat("0{0}{1}\n", splitString, userName);
            }
            foreach (var rule in banRuleRegex)
            {
                sb.AppendFormat("1{0}{1}{0}{2}\n", splitString, rule.id, rule.expression);
            }
            foreach (var userName in whitelistUserName)
            {
                sb.AppendFormat("2{0}{1}\n", splitString, userName);
            }
            foreach (var rule in whitelistRuleRegex)
            {
                sb.AppendFormat("3{0}{1}{0}{2}\n", splitString, rule.id, rule.expression);
            }
            return sb.ToString();//todo
        }

        /// <summary>
        /// 按照字符串内容加载内容
        /// </summary>
        /// <param name="formatString">字符串内容，通常由SaveAsFormattedString()得到的</param>
        public void LoadFromFormattedString(string formatString)
        {
            //StringReader reader = new StringReader(formatString);
            int position = 0;
            string content = "";
            int max_id = 0;
            int current_id = 0;
            char ch;
            while (position < formatString.Length)
            {
                ch = formatString[position];
                content += ch;
                if (ch == '\n')
                {
                    current_id = RecoverObject(content);
                    if (max_id < current_id)
                        max_id = current_id;
                    content = "";
                }
                position++;
            }

            rule_id_gerenator = max_id;
        }

        protected int RecoverObject(string context)
        {
            if (context.Length == 0)
                return -1;
            string[] splitResult;
            int maxId = 0, current_id;
            splitResult = context.Split(context.ToCharArray(), StringSplitOptions.None);
            if (splitResult.Length < 2)
                return -1;
            if (splitResult[splitResult.Length - 1][splitResult[splitResult.Length - 1].Length - 1] == '\n')
                splitResult[splitResult.Length - 1] = splitResult[splitResult.Length - 1].Substring(0, splitResult[splitResult.Length - 1].Length - 1);
            switch (context[0])
            {
                case '0':
                    {
                        AddBanUserName(splitResult[1]);
                        break;
                    }
                case '1':
                    {
                        current_id = Convert.ToInt32(splitResult[1]);
                        AddBanRuleRegex(splitResult[2], current_id);
                        if (maxId < current_id)
                            maxId = current_id;
                        break;
                    }
                case '2':
                    {
                        AddWhiteListUserName(splitResult[1]);
                        break;
                    }
                case '3':
                    {
                        current_id = Convert.ToInt32(splitResult[1]);
                        AddWhiteListRuleRegex(splitResult[2], current_id);
                        if (maxId < current_id)
                            maxId = current_id;
                        break;
                    }
                default:
                    break;
            }
            return maxId;
        }

        /// <summary>
        /// 判断用户是否被ban
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public bool IsBanned(string userName)
        {
            if (accessType == BanAccessType.ALL)
                return false;
            if (accessType == BanAccessType.WHITE_LIST)
            {
                foreach (var enumUserName in whitelistUserName)
                    if (enumUserName.CompareTo(userName) == 0)
                        return false;
                foreach (var enumRule in whitelistRuleRegex)
                    if (enumRule.regex.IsMatch(userName))
                        return false;
                return true;
            }
            if (accessType == BanAccessType.NOTBANNED)
            {
                foreach (var enumUserName in banUserName)
                    if (enumUserName.CompareTo(userName) == 0)
                        return true;
                foreach (var enumRule in banRuleRegex)
                    if (enumRule.regex.IsMatch(userName))
                        return true;
                return false;
            }
            return false;
        }

        public bool IsAllow(string userName)
        {
            return !IsBanned(userName);
        }

        private string GetUserName(int id)
        {
            return UserIdGenerator.GetUserName(id);
        }
    }
}
