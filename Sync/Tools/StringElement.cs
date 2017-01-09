using System;

namespace Sync.Tools
{
    class StringElement
    {
        private string perfix { get; set; }
        private string str { get; set; }
        private string suffix { get; set; }

        public StringElement(string perfix, string text, string suffix)
        {
            this.perfix = perfix;
            str = text;
            this.suffix = suffix;
        }

        public StringElement(string perfix, string text)
        {
            this.perfix = perfix;
            str = text;
        }

        public StringElement(string source)
        {
            str = source;
        }

        public StringElement()
        {
            str = String.Empty;
        }

        public void setText(string source)
        {
            str = source;
        }

        public void setPerfix(string perfix)
        {
            this.perfix = perfix;
        }

        public void setSuffix(string suffix)
        {
            this.suffix = suffix;
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

        public static implicit operator StringElement(string e)
        {
            return new StringElement(e);
        }

        public static implicit operator string(StringElement e)
        {
            return e.Result;
        }
    }
}
