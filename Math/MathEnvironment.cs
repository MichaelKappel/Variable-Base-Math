using System;
using System.Collections.Generic;
using System.Text;

namespace Math
{
    public class MathEnvironment: IEquatable<MathEnvironment>
    {
        public MathEnvironment(String rawKey)
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
            this.First = this.Key[1];
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
            get { return new Number(this, new Char[] { this.Bottom }); }
        }

        public WholeNumber BottomWholeNumber
        {
            get { return new WholeNumber(this, new Char[] { this.Bottom }); }
        }


        public Number PowerOfFirstNumber
        {
            get { return new Number(this, new Char[] { this.First, this.Bottom }); }
        }

        public WholeNumber PowerOfFirstWholeNumber
        {
            get { return new WholeNumber(this, new Char[] { this.First, this.Bottom }); }
        }

        public Char First
        {
            get;
            protected set;
        }

        public Number FirstNumber
        {
            get { return new Number(this, new Char[] { this.First }); }
        }

        public WholeNumber FirstWholeNumber
        {
            get { return new WholeNumber(this, new Char[] { this.First }); }
        }
        
        public Char Top
        {
            get;
            protected set;
        }

        public Number TopNumber
        {
            get { return new Number(this, new Char[] { this.Top }); }
        }

        public WholeNumber ToptWholeNumber
        {
            get { return new WholeNumber(this, new Char[] { this.Top }); }
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

        public Boolean Equals(MathEnvironment other)
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
