using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Math.Base
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
            : this(environment, isNegative)
        {
            this.Segments = new ReadOnlyCollection<Char>(number);
            this.OnInit();
        }

        protected NumberBase(MathEnvironment environment, List<Char> number, Boolean isNegative)
            : this(environment, isNegative)
        {
            this.Segments = new ReadOnlyCollection<Char>(number);
            this.OnInit();
        }

        protected NumberBase(MathEnvironment environment, ReadOnlyCollection<Char> number, Boolean isNegative)
            : this(environment, isNegative)
        {
            this.Segments = number;
            this.OnInit();
        }

        private NumberBase(MathEnvironment environment, Boolean isNegative)
        {
            this.Environment = environment;
            this.IsNegative = isNegative;
        }

        public void OnInit()
        {
            this.FirstChar = this.Segments[this.Segments.Count - 1];
            if (this.FirstChar != this.Environment.Bottom)
            {
                this.DecimalPlaces = (UInt64)this.Segments.Count;
            }
            else if (this.Segments.Count > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }
        }

        public UInt64 DecimalPlaces
        {
            get;
            set;
        }

        public Char FirstChar
        {
            get;
            protected set;
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