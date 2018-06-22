using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    internal class CommandParser<T>
    {
        //storage the commands parse to the command exector
        string[] args;
        List<Action<T>> parsedArgumentActions = new List<Action<T>>();
        List<Action<T>> parsedExecuteActions = new List<Action<T>>();
        Dictionary<char, Func<string, Action<T>>> parseArgumentBinder;
        Dictionary<char, Func<string, Action<T>>> parseActionBinder;
        int parseSharedIndex = 0;
        internal CommandParser(string[] args)
        {
            this.args = args;
            this.parseArgumentBinder = new Dictionary<char, Func<string, Action<T>>>();
            this.parseActionBinder = new Dictionary<char, Func<string, Action<T>>>();
            ParseCommands();
        }
        internal CommandParser(string[] args, Dictionary<char, Func<string, Action<T>>> arguments, Dictionary<char, Func<string, Action<T>>> actions)
        {
            this.args = args;
            this.parseArgumentBinder = arguments;
            this.parseActionBinder = actions;
            ParseCommands();
        }
        //Parse commands form args to Actions
        public void ParseCommands()
        {
            try
            {
                foreach (var item in args)
                {
                    ParseStringBlock($"{item} ");
                }
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Wrong argument");
            }
        }
        private void ParseStringBlock(string value)
        {
            if (value.StartsWith("-"))
            {
                ParseArgumentArgs(value.Substring(1));
            }
            else
            {
                ParseExecuteArgs(value);
            }
        }
        private void ParseArgumentArgs(string value)
        {
            for (parseSharedIndex = 0; parseSharedIndex < value.Length - 1; parseSharedIndex++)
            {
                parsedArgumentActions.Add(parseArgumentBinder[value[parseSharedIndex]](value));

            }
        }
        private void ParseExecuteArgs(string value)
        {
            if (value.Length > 0)
            {
                parsedExecuteActions.Add(parseActionBinder[value[0]](value));
            }
        }
        #region ParseTools
        private string ReadLiteralString(string value)
        {
            string result = string.Empty;
            //is quote ", read until next quote "
            char current = value[++parseSharedIndex];
            while (current != '"')
            {
                result += current;
                current = value[++parseSharedIndex];
            }
            return result;
        }

        private string ReadNumberToString(string value)
        {
            string result = string.Empty;
            char current = value[parseSharedIndex++];
            while (char.IsDigit(current))
            {
                result += current;
                current = value[parseSharedIndex++];
            }
            parseSharedIndex -= 2;
            return result;
        }
        private string GetSubArgument(string value)
        {
            char peek = value[++parseSharedIndex];
            if (peek == '"') return ReadLiteralString(value);
            if (char.IsDigit(peek)) return ReadNumberToString(value);
            return string.Empty;
        }
        #endregion

        public void ExecuteActionOn(T instance)
        {
            foreach (var item in parsedArgumentActions)
            {
                item.Invoke(instance);
            }
            foreach (var item in parsedExecuteActions)
            {
                item.Invoke(instance);
            }
        }
    }
}
