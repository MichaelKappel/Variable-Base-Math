using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Math
{
    public class Numbers
    {
        public MathEnvironmentInfo Environment
        {
            get;
            set;
        }

        public Numbers(MathEnvironmentInfo environment)
        {
            this.Environment = environment;
            this.NumbersSegments = new List<char[]>();
        }

        public Numbers(params Number[] numberInfoArray)
            : this(numberInfoArray[0].Environment)
        {
            foreach(Number number in numberInfoArray)
            {
                this.Add(number);
            }
        }

        public Numbers(MathEnvironmentInfo environment, params String[] numberArrays)
            :this(environment)
        {
            this.Add(numberArrays.Select(x => new Number(environment, x)).ToArray());
        }

        public void Add(params Number[] numberArrays)
        {
            foreach (Number numberArray in numberArrays)
            {
                if (numberArray.Environment.ToString() != this.Environment.ToString())
                {
                    throw new Exception(String.Format("Invalid Environment Number {0} failed in Numbers Info {1}", numberArray.Environment, this.Environment));
                }
                else
                {
                    this.NumbersSegments.Add(numberArray.Segments);
                }
            }
        }

        public Number GetNumberAtIndex(Int64 index)
        {
            return new Number(this.Environment, this.NumbersSegments[(Int32)index]);
        }
        
        public List<Char[]> NumbersSegments { get; set; }

        public override string ToString()
        {
            String result = null;
            foreach (Char[] segment in this.NumbersSegments)
            {
                result += segment.ToString();
            }
            return result;
        }
    }
}