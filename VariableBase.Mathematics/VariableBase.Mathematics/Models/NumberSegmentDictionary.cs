using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Models
{
    public class NumberSegmentDictionary : Dictionary<Decimal, NumberSegmentDictionary>
    {

        public enum NumberTypes
        {
            Unknown,
            Composite,
            Prime,
            Fibonacci,
            FibonacciPrime
        }


        public NumberSegmentDictionary Parent;

        public NumberTypes NumberType;

        public NumberSegmentDictionary(NumberSegmentDictionary parent){
            this.Parent = parent;
        }
        
        public void Add(NumberSegments segments, NumberTypes numberType, NumberSegmentDictionary parent = default(NumberSegmentDictionary))
        {
            NumberSegmentDictionary lastItem;
            if (this.Parent == default(NumberSegmentDictionary))
            {
                if (!this.ContainsKey(segments.Length))
                {
                    this[segments.Length] = new NumberSegmentDictionary(this);
                }
                lastItem = this[segments.Length];
            }
            else
            {
                lastItem = this.Parent;
            }


            for (var i = segments.Length - 1; i >= 0; i--)
            {
                if (!lastItem.ContainsKey(segments[i]))
                {
                    lastItem.Add(segments[i], new NumberSegmentDictionary(lastItem));
                }

                lastItem = lastItem[segments[i]];
            }

            lastItem.NumberType = numberType;
        }

        

        public NumberTypes GetNumberType(NumberSegments segments)
        {
            NumberSegmentDictionary lastItem;
            if (this.Parent == default(NumberSegmentDictionary))
            {
                if (!this.ContainsKey(segments.Length))
                {
                    return NumberTypes.Unknown;
                }
                lastItem = this[segments.Length];
            }
            else
            {
                lastItem = this.Parent;
            }


            for (var i = segments.Length - 1; i >= 0; i--)
            {
                if (!lastItem.ContainsKey(segments[i]))
                {
                    return NumberTypes.Unknown;
                }

                lastItem = lastItem[segments[i]];
            }

            return lastItem.NumberType;
        }

        public Boolean Contains(NumberSegments segments)
        {

            NumberSegmentDictionary lastItem;
            if (this.Parent == default(NumberSegmentDictionary))
            {
                if (!this.ContainsKey(segments.Length))
                {
                    return false;
                }
                lastItem = this[segments.Length];
            }
            else
            {
                lastItem = this.Parent;
            }


            for (var i = segments.Length - 1; i >= 0; i--)
            {
                if (!lastItem.ContainsKey(segments[i]))
                {
                    return false;
                }

                lastItem = lastItem[segments[i]];
            }

            return true;
        }

        public List<NumberSegments> ToList(NumberTypes numberType)
        {
            List<NumberSegments> numberSegments = new List<NumberSegments>();

            if (this.NumberType == numberType)
            {
                NumberSegmentDictionary parent  = this.Parent;
                var segments = new List<Decimal>();
                while (parent != default(NumberSegmentDictionary))
                {
                    segments.Add((Decimal)parent.FirstOrDefault(x => x.Value == this).Key);
                }
                numberSegments.Add(new NumberSegments(segments));
            }
            
            for (var i = 0; i < this.Count; i++)
            {
                List<NumberSegments> childList = this[i].ToList(numberType);
                if (childList != default(List<NumberSegments>))
                {
                    numberSegments.AddRange(childList);
                }
            }

            return numberSegments;
        }

        public override String ToString()
        {
            return this.Count + "," + String.Join(",", this.ToList().Count());
        }
    }
}
