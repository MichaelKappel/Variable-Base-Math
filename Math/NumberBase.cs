using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Math
{
    public abstract class  NumberBase
    {
        public Boolean IsNegative
        {
            get;
            protected set;
        }

        public MathEnvironment Environment
        {
            get;
            protected set;
        }

        public ReadOnlyCollection<Char> Segments
        {
            get;
            protected set;
        }

        protected NumberBase(MathEnvironment environment, Char number, Boolean isNegative)
            : this(environment, new Char[] { number }, isNegative)
        {

        }

        protected NumberBase(MathEnvironment environment, Char[] number, Boolean isNegative)
        {
            this.Environment = environment;
            this.Segments = new ReadOnlyCollection<Char>(number);
            this.IsNegative = isNegative;
            this.FirstChar = Segments[Segments.Count - 1];
        }

        protected NumberBase(MathEnvironment environment, List<Char> number, Boolean isNegative)
        {
            this.Environment = environment;
            this.Segments = new ReadOnlyCollection<Char>(number);
            this.IsNegative = isNegative;
            this.FirstChar = Segments[Segments.Count - 1];
        }

        protected NumberBase(MathEnvironment environment, ReadOnlyCollection<Char> number, Boolean isNegative)
        {
            this.Environment = environment;
            this.Segments = number;
            this.IsNegative = isNegative;
            this.FirstChar = Segments[Segments.Count - 1];
        }

        
        public Char FirstChar
        {
            get;
            protected set;
        }

        public Boolean IsBottom()
        {
            foreach (Char segment in Segments)
            {
                if (segment != this.Environment.Bottom)
                {
                    return false;
                }
            }
            return true;
        }

        public override String ToString()
        {
            String result = null;
            foreach (Char segment in this.Segments.Reverse())
            {
                result += segment;
            }
            if (this.IsNegative)
            {
                result = "-" + result;
            }
            return result;
        }
    }
}