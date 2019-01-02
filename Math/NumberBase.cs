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

        public NumberBase(MathEnvironment environment, Char number, Boolean isNegative)
            : this(environment, new Char[] { number }, isNegative)
        {

        }

        public NumberBase(MathEnvironment environment, Char[] number, Boolean isNegative)
        {
            this.Environment = environment;
            List<Char> numberSegments = number.ToList();
            this.Validate(numberSegments);
            this.Segments = new ReadOnlyCollection<Char>(numberSegments);
            this.IsNegative = isNegative;

        }
       
        public NumberBase(MathEnvironment environment, String rawNumber, Boolean isNegative)
        {
            this.Environment = environment;
            List<Char> numberSegments = rawNumber.ToCharArray().Reverse().ToList();

            this.Validate(numberSegments);
            this.Segments = new ReadOnlyCollection<Char>(numberSegments);
            this.IsNegative = isNegative;
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

        public void Validate(List<Char> numberSegments)
        {
            while (numberSegments.Count > 1 && numberSegments[numberSegments.Count - 1] == Environment.Bottom)
            {
                numberSegments.RemoveAt(numberSegments.Count - 1);
            }

            foreach (Char segment in numberSegments)
            {
                if (!this.Environment.Key.Contains(segment))
                {
                    throw new Exception(String.Format("Invalid Number {0} not found in {1}", segment, this.Environment));
                }
            }
        }


        public override String ToString()
        {
            String result = null;
            foreach (Char segment in this.Segments.ToArray().Reverse())
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