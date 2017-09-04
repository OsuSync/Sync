using System;

namespace Sync.Tools
{
    public struct StringElement
    {
        public string perfix;
        public string str;
        public string suffix;

        public StringElement(string perfix, string text, string suffix)
        {
            this.perfix = perfix;
            str = text;
            if (str == null) str = string.Empty;
            this.suffix = suffix;
        }

        public StringElement(string perfix, string text)
        {
            this.perfix = perfix;
            str = text;
            if (str == null) str = string.Empty;
            suffix = string.Empty;
        }

        public StringElement(string source)
        {
            str = source;
            if (str == null) str = string.Empty;
            perfix = string.Empty;
            suffix = string.Empty;
        }

        public string Result
        {
            get
            {
                return str.Length == 0 ? String.Empty : perfix + str + suffix;
            }
        }

        public string RawText
        {
            get { return str; }
        }

        public string Perfix
        {
            get { return perfix; }
        }

        public string Suffix
        {
            get { return suffix; }
        }

        public static StringElement operator +(StringElement op1, StringElement op2)
        {
            if (op1 == null) return op2;
            if (op2 == null) return op1;
            return new StringElement(op1.perfix + op2.perfix, op1.str + op2.str, op1.suffix + op2.suffix);
        }

        public static string operator +(StringElement op1, string op2)
        {

            return op1.Result + op2;
        }

        public static implicit operator StringElement(string e)
        {
            return new StringElement(e);
        }

        public static implicit operator string(StringElement e)
        {
            return e.Result;
        }

        public override string ToString()
        {
            return Result;
        }
    }
}
