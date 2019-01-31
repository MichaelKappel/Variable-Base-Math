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
            Prime
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
                NumberSegmentDictionary topLevelNumberSegment;
                if (this.TryGetValue(segments.Length, out topLevelNumberSegment))
                {
                    lastItem = topLevelNumberSegment;
                }
                else
                {
                    lastItem = new NumberSegmentDictionary(this);
                    this.Add(segments.Length, lastItem);
                }
            }
            else
            {
                lastItem = this.Parent;
            }
            
            for (var i = segments.Length - 1; i >= 0; i--)
            {
                NumberSegmentDictionary childSegment;
                if (lastItem.TryGetValue(segments[i], out childSegment))
                {
                    lastItem = childSegment;
                }
                else
                {
                    childSegment = new NumberSegmentDictionary(lastItem);
                    lastItem.Add(segments[i], childSegment);
                }
            }

            lastItem.NumberType = numberType;
        }

        

        public NumberTypes GetNumberType(NumberSegments segments)
        {
            NumberSegmentDictionary lastItem;
            if (this.Parent == default(NumberSegmentDictionary))
            {
                NumberSegmentDictionary topLevelNumberSegment;
                if (this.TryGetValue(segments.Length, out topLevelNumberSegment))
                {
                    lastItem = topLevelNumberSegment;
                }
                else
                {
                    return NumberTypes.Unknown;
                }
            }
            else
            {
                lastItem = this.Parent;
            }
            
            for (var i = segments.Length - 1; i >= 0; i--)
            {
                NumberSegmentDictionary childSegment;
                if (lastItem.TryGetValue(segments[i], out childSegment))
                {
                    lastItem = childSegment;
                }
                else
                { 
                    return NumberTypes.Unknown;
                }
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
