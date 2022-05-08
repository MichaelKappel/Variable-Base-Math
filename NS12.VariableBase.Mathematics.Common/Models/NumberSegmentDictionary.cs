
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Models
{
    public class NumberSegmentDictionary : Dictionary<decimal, NumberSegmentDictionary>
    {

        public enum NumberTypes
        {
            Unknown,
            Composite,
            Prime
        }

        public NumberSegmentDictionary Parent;

        public NumberTypes NumberType;

        public NumberSegmentDictionary(NumberSegmentDictionary parent)
        {
            Parent = parent;
        }

        public void Add(NumberSegments segments, NumberTypes numberType, NumberSegmentDictionary parent = default)
        {
            NumberSegmentDictionary lastItem;
            if (Parent == default(NumberSegmentDictionary))
            {
                NumberSegmentDictionary topLevelNumberSegment;
                if (TryGetValue(segments.Length, out topLevelNumberSegment))
                {
                    lastItem = topLevelNumberSegment;
                }
                else
                {
                    lastItem = new NumberSegmentDictionary(this);
                    Add(segments.Length, lastItem);
                }
            }
            else
            {
                lastItem = Parent;
            }

            for (var i = segments.Length - 1; i >= 0; i--)
            {
                try
                {
                    AddAt(segments[i], numberType, lastItem);
                }
                catch (Exception)
                {
                    AddAt(segments[i], numberType, lastItem);
                }
            }

            lastItem.NumberType = numberType;
        }

        public NumberSegmentDictionary AddAt(decimal segment, NumberTypes numberType, NumberSegmentDictionary lastItem)
        {
            NumberSegmentDictionary childSegment;
            if (lastItem.TryGetValue(segment, out childSegment))
            {
                lastItem = childSegment;
            }
            else
            {
                childSegment = new NumberSegmentDictionary(lastItem);
                lastItem.Add(segment, childSegment);
            }
            return lastItem;
        }

        public NumberTypes GetNumberType(NumberSegments segments)
        {
            NumberSegmentDictionary lastItem;
            if (Parent == default(NumberSegmentDictionary))
            {
                NumberSegmentDictionary topLevelNumberSegment;
                if (TryGetValue(segments.Length, out topLevelNumberSegment))
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
                lastItem = Parent;
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

        public bool Contains(NumberSegments segments)
        {

            NumberSegmentDictionary lastItem;
            if (Parent == default(NumberSegmentDictionary))
            {
                if (!ContainsKey(segments.Length))
                {
                    return false;
                }
                lastItem = this[segments.Length];
            }
            else
            {
                lastItem = Parent;
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

            if (NumberType == numberType)
            {
                NumberSegmentDictionary parent = Parent;
                var segments = new List<decimal>();
                while (parent != default(NumberSegmentDictionary))
                {
                    segments.Add(parent.FirstOrDefault(x => x.Value == this).Key);
                }
                numberSegments.Add(new NumberSegments(segments));
            }

            for (var i = 0; i < Count; i++)
            {
                List<NumberSegments> childList = this[i].ToList(numberType);
                if (childList != default(List<NumberSegments>))
                {
                    numberSegments.AddRange(childList);
                }
            }

            return numberSegments;
        }

        public override string ToString()
        {
            return Count + "," + string.Join(",", this.ToList().Count());
        }
    }
}
