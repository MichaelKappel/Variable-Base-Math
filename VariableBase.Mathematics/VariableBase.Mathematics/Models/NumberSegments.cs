using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics.Models
{
    public class NumberSegments: IEnumerable<Decimal>, IEnumerator<Decimal>
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

        internal NumberSegments(IList<Boolean> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.Boolean;
            this.BooleanSegments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        internal NumberSegments(IList<UInt16> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        internal NumberSegments(IList<Char> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments.Select(x => (UInt16)x).ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        internal NumberSegments(IList<UInt32> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt32;
            this.UInt32Segments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        internal NumberSegments(IList<UInt64> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments.ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        internal NumberSegments(IList<Decimal> segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments.Select(x => (UInt64)x).ToArray();
            this.Size = segments.Count;
            this.Length = segments.Count;
        }

        internal NumberSegments(Boolean[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.Boolean;
            this.BooleanSegments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        internal NumberSegments(UInt16[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }
        internal NumberSegments(Char[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt16;
            this.UInt16Segments = segments.Select(x => (UInt16)x).ToArray();
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        internal NumberSegments(UInt32[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt32;
            this.UInt32Segments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        internal NumberSegments(UInt64[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments;
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        internal NumberSegments(Decimal[] segments)
        {
            this.NumberSegmentType = NumberSegmentTypes.UInt64;
            this.UInt64Segments = segments.Select(x => (UInt64)x).ToArray();
            this.Size = segments.Length;
            this.Length = segments.Length;
        }

        public Decimal Size
        {
            get;
            protected set;
        }

        public Int32 Length
        {
            get;
            protected set;
        }

        public Decimal this[Int32 i]
        {
            get
            {
                if (this.NumberSegmentType == NumberSegmentTypes.Boolean)
                {
                    return this.BooleanSegments[(UInt32)i] ? 1 : 0;
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt16)
                {
                    return (Decimal)this.UInt16Segments[(UInt32)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt32)
                {
                    return (Decimal)this.UInt32Segments[(UInt32)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt64)
                {
                    return (Decimal)this.UInt64Segments[(UInt32)i];
                }
                throw new Exception("NumberSegmentType Unknown");
            }
        }

        public Decimal this[Decimal i]
        {
            get
            {
                if (this.NumberSegmentType == NumberSegmentTypes.Boolean)
                {
                    return this.BooleanSegments[(UInt32)i]?1:0;
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt16)
                {
                    return (Decimal)this.UInt16Segments[(UInt32)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt32)
                {
                    return (Decimal)this.UInt32Segments[(UInt32)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt64)
                {
                    return (Decimal)this.UInt64Segments[(UInt32)i];
                }
                throw new Exception("NumberSegmentType Unknown");
            }
        }

        public Decimal this[Double i]
        {
            get
            {
                if (this.NumberSegmentType == NumberSegmentTypes.Boolean)
                {
                    return this.BooleanSegments[(UInt32)i] ? 1 : 0;
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt16)
                {
                    return (Decimal)this.UInt16Segments[(UInt32)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt32)
                {
                    return (Decimal)this.UInt32Segments[(UInt32)i];
                }
                else if (this.NumberSegmentType == NumberSegmentTypes.UInt64)
                {
                    return (Decimal)this.UInt64Segments[(UInt32)i];
                }
                else
                {
                    throw new Exception("NumberSegmentType Unknown");
                }
            }
        }

        public IEnumerator<Decimal> GetEnumerator()
        {
            if (this.NumberSegmentType == NumberSegmentTypes.Boolean)
            {
                foreach (Boolean item in this.BooleanSegments)
                {
                    yield return item ? 1 : 0;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt16)
            {
                foreach (Decimal item in this.UInt16Segments)
                {
                    yield return item;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt32)
            {
                foreach (Decimal item in this.UInt32Segments)
                {
                    yield return item;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt64)
            {
                foreach (Decimal item in this.UInt64Segments)
                {
                    yield return item;
                }
            }
            else
            {
                throw new Exception("NumberSegmentType Unknown");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.NumberSegmentType == NumberSegmentTypes.Boolean)
            {
                foreach (Boolean item in this.BooleanSegments)
                {
                    yield return item ? 1 : 0;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt16)
            {
                foreach (Decimal item in this.UInt16Segments)
                {
                    yield return item;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt32)
            {
                foreach (Decimal item in this.UInt32Segments)
                {
                    yield return item;
                }
            }
            else if (this.NumberSegmentType == NumberSegmentTypes.UInt64)
            {
                foreach (Decimal item in this.UInt64Segments)
                {
                    yield return item;
                }
            }
            else
            {
                throw new Exception("NumberSegmentType Unknown");
            }
        }

        protected Boolean[] BooleanSegments { get; set; }

        protected UInt16[] UInt16Segments { get; set; }

        protected UInt32[] UInt32Segments { get; set; }

        protected UInt64[] UInt64Segments { get; set; }

        public decimal Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal void CopyTo(decimal[] segments, decimal times)
        {
            throw new NotImplementedException();
        }
    }
}
