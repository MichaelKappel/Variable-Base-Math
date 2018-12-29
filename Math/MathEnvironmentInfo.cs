using System;
using System.Collections.Generic;
using System.Text;

namespace Math
{
    public class MathEnvironmentInfo: IEquatable<MathEnvironmentInfo>
    {
        public MathEnvironmentInfo(String rawKey)
        {
            this.SetKey(rawKey);
        }

        public UInt64 GetIndex(Char arg)
        {
            return (UInt64)this.Key.IndexOf(arg);
        }

        public void SetKey(String rawKey)
        {
            this.Key = new List<Char>();
            foreach (Char segment in rawKey.ToCharArray())
            {
                if (!Key.Contains(segment))
                {
                    Key.Add(segment);
                }
            }
            this.Bottom = this.Key[0];
            this.Top = this.Key[this.Key.Count - 1];
            this.Base = (UInt64)this.Key.Count;
        }

        public Char Bottom
        {
            get;
            protected set;
        }
        public Number BottomNumber
        {
            get { return new Number(this, new List<Char> { this.Bottom }); }
        }

        public Char Top
        {
            get;
            protected set;
        }

        public Number TopNumber
        {
            get { return new Number(this, new List<Char> { this.Top }); }
        }

        public UInt64 Base {
            get;
            protected set;
        }

        public List<Char> Key
        {
            get;
            protected set;
        }

        public override string ToString()
        {
            String result = null;
            foreach (Char segment in this.Key)
            {
                if (result != null)
                {
                    result += '|';
                }
                result += segment;
            }
            return String.Format("[BASE:\"{0}\"|Bottom:\"{1}\"|Top:\"{2}]\"", this.Base, this.Bottom, this.Top) + result;
        }

        public Boolean Equals(MathEnvironmentInfo other)
        {
            if (this.Base != other.Base)
            {
                return false;
            }

            if (this.Key.Count != other.Key.Count)
            {
                return false;
            }

            for (var i=0; i < this.Key.Count; i++)
            {
                if (this.Key[i] != other.Key[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
