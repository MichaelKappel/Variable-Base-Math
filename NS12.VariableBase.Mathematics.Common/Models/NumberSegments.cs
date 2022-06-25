
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Models
{
    public class NumberSegments : IEnumerable<decimal>
    {

        public NumberSegmentTypes NumberSegmentType { get; protected set; }

        public enum NumberSegmentTypes
        {
            Unknown,
            Boolean,
            UInt16,
            UInt32,
            UInt64
        }

        public NumberSegments(IList<bool> segments)
        {
            NumberSegmentType = NumberSegmentTypes.Boolean;
            BooleanSegments = segments.ToArray();
            Size = segments.Count;
            Length = segments.Count;
        }

        public NumberSegments(IList<ushort> segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt16;
            UInt16Segments = segments.ToArray();
            Size = segments.Count;
            Length = segments.Count;
        }

        public NumberSegments(IList<char> segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt16;
            UInt16Segments = segments.Select(x => (ushort)x).ToArray();
            Size = segments.Count;
            Length = segments.Count;
        }

        public NumberSegments(IList<uint> segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt32;
            UInt32Segments = segments.ToArray();
            Size = segments.Count;
            Length = segments.Count;
        }

        public NumberSegments(IList<ulong> segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt64;
            UInt64Segments = segments.ToArray();
            Size = segments.Count;
            Length = segments.Count;
        }

        public NumberSegments(IList<decimal> segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt64;
            UInt64Segments = segments.Select(x => (ulong)x).ToArray();
            Size = segments.Count;
            Length = segments.Count;
        }

        public NumberSegments(bool[] segments)
        {
            NumberSegmentType = NumberSegmentTypes.Boolean;
            BooleanSegments = segments;
            Size = segments.Length;
            Length = segments.Length;
        }

        public NumberSegments(ushort[] segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt16;
            UInt16Segments = segments;
            Size = segments.Length;
            Length = segments.Length;
        }

        public NumberSegments(char[] segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt16;
            UInt16Segments = segments.Select(x => (ushort)x).ToArray();
            Size = segments.Length;
            Length = segments.Length;
        }

        public NumberSegments(String number)
        {
            NumberSegmentType = NumberSegmentTypes.UInt16;
            UInt16Segments = number.ToCharArray().Select(x => (ushort)x).ToArray();
            Size = number.Length;
            Length = number.Length;
        }

        public NumberSegments(uint[] segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt32;
            UInt32Segments = segments;
            Size = segments.Length;
            Length = segments.Length;
        }

        public NumberSegments(ulong[] segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt64;
            UInt64Segments = segments;
            Size = segments.Length;
            Length = segments.Length;
        }

        public NumberSegments(decimal[] segments)
        {
            NumberSegmentType = NumberSegmentTypes.UInt64;
            UInt64Segments = segments.Select(x => (ulong)x).ToArray();
            Size = segments.Length;
            Length = segments.Length;
        }

        public decimal Size
        {
            get;
            protected set;
        }

        public int Length
        {
            get;
            protected set;
        }

        public decimal this[int i]
        {
            get
            {
                switch (NumberSegmentType)
                {
                    case NumberSegmentTypes.Boolean:
                        return BooleanSegments[(uint)i] ? 1 : 0;
                    case NumberSegmentTypes.UInt16:
                        return UInt16Segments[(uint)i];
                    case NumberSegmentTypes.UInt32:
                        return UInt32Segments[(uint)i];
                    case NumberSegmentTypes.UInt64:
                        return UInt64Segments[(uint)i];
                    default:
                        throw new Exception("NumberSegmentType Unknown");
                }
            }
        }

        public decimal this[decimal i]
        {
            get
            {
                switch (NumberSegmentType)
                {
                    case NumberSegmentTypes.Boolean:
                        return BooleanSegments[(uint)i] ? 1 : 0;
                    case NumberSegmentTypes.UInt16:
                        return UInt16Segments[(uint)i];
                    case NumberSegmentTypes.UInt32:
                        return UInt32Segments[(uint)i];
                    case NumberSegmentTypes.UInt64:
                        return UInt64Segments[(uint)i];
                    default:
                        throw new Exception("NumberSegmentType Unknown");
                }
            }
        }

        public decimal this[double i]
        {
            get
            {
                switch (NumberSegmentType)
                {
                    case NumberSegmentTypes.Boolean:
                        return BooleanSegments[(uint)i] ? 1 : 0;
                    case NumberSegmentTypes.UInt16:
                        return UInt16Segments[(uint)i];
                    case NumberSegmentTypes.UInt32:
                        return UInt32Segments[(uint)i];
                    case NumberSegmentTypes.UInt64:
                        return UInt64Segments[(uint)i];
                    default:
                        throw new Exception("NumberSegmentType Unknown");
                }
            }
        }

        public IEnumerator<decimal> GetEnumerator()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    return BooleanSegments.Select(x => x ? 1 : (decimal)0).GetEnumerator();
                case NumberSegmentTypes.UInt16:
                    return UInt16Segments.Select(x => (decimal)x).GetEnumerator();
                case NumberSegmentTypes.UInt32:
                    return UInt32Segments.Select(x => (decimal)x).GetEnumerator();
                case NumberSegmentTypes.UInt64:
                    return UInt64Segments.Select(x => (decimal)x).GetEnumerator();
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    return BooleanSegments.GetEnumerator();
                case NumberSegmentTypes.UInt16:
                    return UInt16Segments.GetEnumerator();
                case NumberSegmentTypes.UInt32:
                    return UInt32Segments.GetEnumerator();
                case NumberSegmentTypes.UInt64:
                    return UInt64Segments.GetEnumerator();
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        public decimal[] GetSegments()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    return BooleanSegments.Select(x => x ? 0 : (decimal)1).ToArray();
                case NumberSegmentTypes.UInt16:
                    return UInt16Segments.Select(x => (decimal)x).ToArray();
                case NumberSegmentTypes.UInt32:
                    return UInt32Segments.Select(x => (decimal)x).ToArray();
                case NumberSegmentTypes.UInt64:
                    return UInt64Segments.Select(x => (decimal)x).ToArray();
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        protected bool[] BooleanSegments { get; set; }

        protected ushort[] UInt16Segments { get; set; }

        protected uint[] UInt32Segments { get; set; }

        protected ulong[] UInt64Segments { get; set; }

        public void Dispose()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    BooleanSegments = default;
                    break;
                case NumberSegmentTypes.UInt16:
                    UInt16Segments = default;
                    break;
                case NumberSegmentTypes.UInt32:
                    UInt32Segments = default;
                    break;
                case NumberSegmentTypes.UInt64:
                    UInt64Segments = default;
                    break;
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        public void CopyTo(decimal[] segments)
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    int i2 = BooleanSegments.Length - 1;
                    for (var i = segments.Length - 1; i >= 0; i--)
                    {
                        if (i2 == 0)
                        {
                            break;
                        }
                        segments[i] = BooleanSegments[i2] ? 1 : 0;
                        i2--;
                    }
                    break;
                case NumberSegmentTypes.UInt16:
                    int i16 = UInt16Segments.Length - 1;
                    for (var i = segments.Length - 1; i >= 0; i--)
                    {
                        if (i16 == 0)
                        {
                            break;
                        }
                        segments[i] = UInt16Segments[i16];
                        i16--;
                    }
                    break;
                case NumberSegmentTypes.UInt32:
                    int i1 = UInt32Segments.Length - 1;
                    for (var i = segments.Length - 1; i >= 0; i--)
                    {
                        if (i1 == 0)
                        {
                            break;
                        }
                        segments[i] = UInt32Segments[i1];
                        i1--;
                    }
                    break;
                case NumberSegmentTypes.UInt64:
                    int i64 = UInt64Segments.Length - 1;
                    for (var i = segments.Length - 1; i >= 0; i--)
                    {
                        segments[i] = UInt64Segments[i64];

                        if (i64 == 0)
                        {
                            break;
                        }
                        i64--;
                    }
                    break;
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        public string ToCsv()
        {
            return string.Join(',', this.Reverse());
        }

        public override string ToString()
        {
            return GetDisplayValue();
        }

        public string GetDisplayValue()
        {
            string result = string.Empty;
            if (Length > 100)
            {
                result = string.Format("{0}e{1}", this[Length - 1], Length);
            }
            else
            {
                result = string.Join(" ", this.Select(x => x.ToString()).Reverse());
            }
            return result;
        }
    }
}
