
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
            Byte,
            UInt16,
            UInt32,
            UInt64
        }

        public NumberSegments(IList<bool> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.Boolean;
            this.BooleanSegments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }
        

        public NumberSegments(IList<byte> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.Byte;
            this.ByteSegments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        public NumberSegments(IList<ushort> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        public NumberSegments(IList<char> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments.Select(x => (ushort)x).ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        public NumberSegments(IList<uint> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt32;
            this.UInt32Segments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        public NumberSegments(IList<ulong> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        public NumberSegments(IList<decimal> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments.Select(x => (ulong)x).ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        public NumberSegments(bool[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.Boolean;
            this.BooleanSegments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        public NumberSegments(ushort[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        public NumberSegments(uint[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt32;
            this.UInt32Segments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        public NumberSegments(ulong[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        public NumberSegments(decimal[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments.Select(x => (ulong)x).ToArray();
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        public NumberSegments(Decimal baseSize, IList<Char> number)
            :this(baseSize, number.ToArray())
        {
           
        }
        
        public NumberSegments(Decimal baseSize, Char[] segments)
        {
            if (baseSize <= 2)
            {
                this.NumberSegmentType = NumberSegmentTypes.Boolean;
                this.BooleanSegments = segments.Select(x => (x == 0) ? false : true).ToArray();
            }
            else if (baseSize <= 255)
            {
                this.NumberSegmentType = NumberSegmentTypes.Byte;
                this.ByteSegments = segments.Select(x => (Byte)x).ToArray();
            }
            else if (baseSize <= 65535)
            {
                this.NumberSegmentType = NumberSegmentTypes.UInt16;
                this.UInt16Segments = segments.Select(x => (ushort)x).ToArray();
            }
            else if (baseSize <= 4294967295)
            {
                this.NumberSegmentType = NumberSegmentTypes.UInt32;
                this.UInt32Segments = segments.Select(x => (UInt32)x).ToArray();
            }
            else if (baseSize <= 18446744073709551615)
            {
                this.NumberSegmentType = NumberSegmentTypes.UInt64;
                this.UInt64Segments = segments.Select(x => (UInt64)x).ToArray();
            }

            this.Size = segments.Length;
            this.Length = segments.Length;
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
                if (this.NumberSegmentType == NumberSegmentTypes.Boolean && this.BooleanSegments != null)
                {
                    return this.BooleanSegments[(uint)i] ? 1 : 0;
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.Byte && this.ByteSegments != null)
                {
                    return this.ByteSegments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt16 && this.UInt16Segments != null)
                {
                    return this.UInt16Segments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt32 && this.UInt32Segments != null)
                {
                    return this.UInt32Segments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt64 && this.UInt64Segments != null)
                {
                    return this.UInt64Segments[(uint)i];
                }
                else
                {
                    throw new Exception("NumberSegmentType Unknown");
                }
            }
             
        }

        public decimal this[decimal i]
        {
            get
            {
                if (this.NumberSegmentType == NumberSegmentTypes.Boolean && this.BooleanSegments != null)
                {
                    return this.BooleanSegments[(uint)i] ? 1 : 0;
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.Byte && this.ByteSegments != null)
                {
                    return this.ByteSegments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt16 && this.UInt16Segments != null)
                {
                    return this.UInt16Segments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt32 && this.UInt32Segments != null)
                {
                    return this.UInt32Segments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt64 && this.UInt64Segments != null)
                {
                    return this.UInt64Segments[(uint)i];
                }
                else
                {
                    throw new Exception("NumberSegmentType Unknown");
                }
            }
        }

        public decimal this[double i]
        {
            get
            {
                if (this.NumberSegmentType == NumberSegmentTypes.Boolean && this.BooleanSegments != null)
                {
                    return BooleanSegments[(uint)i] ? 1 : 0;
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.Byte && this.ByteSegments != null)
                {
                    return this.ByteSegments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt16 && this.UInt16Segments != null)
                {
                    return UInt16Segments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt32 && this.UInt32Segments != null)
                {
                    return UInt32Segments[(uint)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt64 && this.UInt64Segments != null)
                {
                    return UInt64Segments[(uint)i];
                }
                else
                {
                    throw new Exception("NumberSegmentType Unknown");
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.NumberSegmentType == NumberSegmentTypes.Boolean && this.BooleanSegments != null)
            {
                return this.BooleanSegments.GetEnumerator();
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.Byte && this.ByteSegments != null)
            {
                return this.ByteSegments.GetEnumerator();
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt16 && this.UInt16Segments != null)
            {
                return this.UInt16Segments.GetEnumerator();
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt32 && this.UInt32Segments != null)
            {
                return this.UInt32Segments.GetEnumerator();
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt64 && this.UInt64Segments != null)
            {
                return this.UInt64Segments.GetEnumerator();
            }
            else
            {
                throw new Exception("NumberSegmentType Unknown");
            }
        }

        public IEnumerator<decimal> GetEnumerator()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    return this.BooleanSegments.Select(x => x ? 1 : (decimal)0).GetEnumerator();
                case NumberSegmentTypes.Byte:
                    return this.ByteSegments.Select(x => (decimal)x).GetEnumerator();
                case NumberSegmentTypes.UInt16:
                    return this.UInt16Segments.Select(x => (decimal)x).GetEnumerator();
                case NumberSegmentTypes.UInt32:
                    return this.UInt32Segments.Select(x => (decimal)x).GetEnumerator();
                case NumberSegmentTypes.UInt64:
                    return this.UInt64Segments.Select(x => (decimal)x).GetEnumerator();
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        public decimal[] GetSegments()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    return this.BooleanSegments.Select(x => x ? 0 : (decimal)1).ToArray();
                case NumberSegmentTypes.Byte:
                    return this.ByteSegments.Select(x => (decimal)x).ToArray();
                case NumberSegmentTypes.UInt16:
                    return this.UInt16Segments.Select(x => (decimal)x).ToArray();
                case NumberSegmentTypes.UInt32:
                    return this.UInt32Segments.Select(x => (decimal)x).ToArray();
                case NumberSegmentTypes.UInt64:
                    return this.UInt64Segments.Select(x => (decimal)x).ToArray();
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        protected bool[]? BooleanSegments { get; set; }

        protected byte[]? ByteSegments { get; set; }

        protected ushort[]? UInt16Segments { get; set; }

        protected uint[]? UInt32Segments { get; set; }

        protected ulong[]? UInt64Segments { get; set; }

        public void Dispose()
        {
            switch (NumberSegmentType)
            {
                case NumberSegmentTypes.Boolean:
                    BooleanSegments = null;
                    break;
                case NumberSegmentTypes.UInt16:
                    UInt16Segments = null;
                    break;
                case NumberSegmentTypes.UInt32:
                    UInt32Segments = null;
                    break;
                case NumberSegmentTypes.UInt64:
                    UInt64Segments = null;
                    break;
                default:
                    throw new Exception("NumberSegmentType Unknown");
            }
        }

        public void CopyTo(decimal[] segments)
        {
            if (this.NumberSegmentType == NumberSegmentTypes.Boolean && this.BooleanSegments != null)
            {
                int i2 = this.BooleanSegments.Length - 1;
                for (var i = segments.Length - 1; i >= 0; i--)
                {
                    if (i2 == 0)
                    {
                        break;
                    }
                    segments[i] = BooleanSegments[i2] ? 1 : 0;
                    i2--;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.Byte && this.ByteSegments != null)
            {
                int ib = this.ByteSegments.Length - 1;
                for (var i = segments.Length - 1; i >= 0; i--)
                {
                    if (ib == 0)
                    {
                        break;
                    }
                    segments[i] = this.ByteSegments[ib];
                    ib--;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt16 && this.UInt16Segments != null)
            {
                int i16 = this.UInt16Segments.Length - 1;
                for (var i = segments.Length - 1; i >= 0; i--)
                {
                    if (i16 == 0)
                    {
                        break;
                    }
                    segments[i] = this.UInt16Segments[i16];
                    i16--;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt32 && this.UInt32Segments != null)
            {
                int i1 = this.UInt32Segments.Length - 1;
                for (var i = segments.Length - 1; i >= 0; i--)
                {
                    if (i1 == 0)
                    {
                        break;
                    }
                    segments[i] = this.UInt32Segments[i1];
                    i1--;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt64 && this.UInt64Segments != null)
            {
                int i64 = this.UInt64Segments.Length - 1;
                for (var i = segments.Length - 1; i >= 0; i--)
                {
                    segments[i] = this.UInt64Segments[i64];

                    if (i64 == 0)
                    {
                        break;
                    }
                    i64--;
                }
            }
            else
            {
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
