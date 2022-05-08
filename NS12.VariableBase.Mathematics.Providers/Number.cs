using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.Algorithms;
using NS12.VariableBase.Mathematics.Providers.Operators;
using System.IO;
using System.Diagnostics;
using NS12.VariableBase.Mathematics.Common.Interfaces;

[assembly: InternalsVisibleTo("Math.Tests")]
namespace NS12.VariableBase.Mathematics.Providers
{
    public struct Number : IEquatable<Number>, IComparable<Number>, IDisposable
    {
        public static IOperator<Number> Operator = new NumberOperator(new BasicMathAlgorithm());
        //public static IStorageRepository StorageRepository = new AzureBlobStorageRepository();

        public static bool IsBottom(Number number)
        {
            return number.Size == 1 && number.Segments[0] == 0;
        }

        public static bool IsFirst(Number number)
        {
            return number.Size == 1 && number.Segments[0] == 1;
        }

        public static bool IsBottom(NumberSegments segments)
        {
            return segments.Size == 1 && segments[0] == 0;
        }

        public static bool IsFirst(NumberSegments segments)
        {
            return segments.Size == 1 && segments[0] == 1;
        }

        public Fraction Fragment { get; set; }

        public decimal First { get; set; }

        public bool IsNegative { get; set; }

        public bool? Even { get; set; }

        public bool IsOdd()
        {
            if (!Even.HasValue)
            {
                Even = Operator.IsEven(this);
            }
            return !Even.Value;
        }


        public bool IsEven()
        {
            if (!Even.HasValue)
            {
                Even = Operator.IsEven(this);
            }
            return Even.Value;
        }

        public IMathEnvironment<Number> Environment { get; set; }

        public NumberSegments Segments { get; set; }
        public int Size { get; private set; }

        public Number(IMathEnvironment<Number> environment, NumberSegments segments, NumberSegments numerator, NumberSegments denominator, bool isNegative)
        {
            if (segments == default(NumberSegments) || segments.Size == 0)
            {
                segments = new NumberSegments(new decimal[] { 0 });
            }

            Environment = environment;

            IsNegative = isNegative;

            Segments = segments;

            Size = segments.Length;

            if (numerator != default(NumberSegments) && denominator != default(NumberSegments))
            {
                Fragment = new Fraction(environment, numerator, denominator);
            }
            else
            {
                Fragment = default;
            }

            First = Segments[Segments.Size - 1];
            if (First == 0 && Segments.Size > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }

            Even = null;
        }


        internal Number(Fraction fraction)
        {
            if (fraction.Numerator.Environment != fraction.Numerator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = fraction.Numerator.Environment;

            Environment = environment;

            if (fraction.Denominator == 0)
            {
                Segments = environment.GetNumber(0).Segments;
                Fragment = default;
            }
            else if (Operator.IsGreaterThan(fraction.Denominator, fraction.Numerator))
            {
                Segments = new NumberSegments(new decimal[] { 0 });
                Fragment = fraction;
            }
            else
            {

                Number numerator = fraction.Numerator;
                Number denominator = fraction.Denominator;

                if (denominator.Fragment != default || denominator.Fragment != default)
                {
                    var aFraction = default(Fraction);
                    if (numerator.Fragment != default)
                    {
                        Number aDividend = Operator.Add(Operator.Multiply(numerator, numerator.Fragment.Denominator), numerator.Fragment.Numerator);
                        aFraction = new Fraction(aDividend, environment.GetNumber(1));
                    }
                    else
                    {
                        aFraction = new Fraction(numerator.Environment, numerator.Segments, environment.GetNumber(1).Segments);
                    }

                    var bFraction = default(Fraction);
                    if (denominator.Fragment != default)
                    {
                        Number bDividend = Operator.Add(Operator.Multiply(denominator, denominator.Fragment.Denominator), denominator.Fragment.Numerator);
                        bFraction = new Fraction(bDividend, environment.GetNumber(1));

                    }
                    else if (aFraction != default)
                    {
                        bFraction = new Fraction(denominator.Environment, denominator.Segments, environment.GetNumber(1).Segments);
                    }

                    Fraction fractionResult = aFraction / bFraction;
                    numerator = fractionResult.Numerator;
                    denominator = fractionResult.Denominator;
                }

                Number resultSegments = Operator.Divide(numerator, denominator);
                Segments = resultSegments.Segments;
                if (resultSegments.Fragment == default)
                {
                    Fragment = default;
                }
                else
                {
                    Fragment = new Fraction(resultSegments.Fragment.Numerator, resultSegments.Fragment.Denominator);
                }
            }
            Size = Segments.Length;
            IsNegative = false;
            First = Segments[Segments.Length - 1];


            Even = null;
        }


        internal Number(IMathEnvironment<Number> environment, NumberSegments segments, Fraction fragment, bool isNegative)
        {
            Environment = environment;

#if DEBUG
            foreach (decimal segment in segments)
            {
                if (segment > Environment.Base)
                {
                    throw new Exception("Bad number segment larger than base");
                }
                else if (segment < 0)
                {
                    throw new Exception("Bad number segment less than zero");
                }
            }
#endif

            IsNegative = isNegative;

            Segments = segments;

            Fragment = fragment;

            Size = segments.Length;

            First = Segments[Segments.Size - 1];
            if (First == 0 && Segments.Size > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }

            Even = null;
        }

        #region operator overrides
        public static bool operator <(Number a, Number b)
        {
            return Operator.Compare(a, b) < 0;
        }

        public static bool operator <=(Number a, Number b)
        {
            return Operator.Compare(a, b) <= 0;
        }
        public static bool operator >(Number a, Number b)
        {
            return Operator.Compare(a, b) > 0;
        }

        public static bool operator >=(Number a, Number b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(Number a, Number b)
        {
            return Operator.Compare(a, b) == 0;
        }

        public static bool operator !=(Number a, Number b)
        {
            return Operator.Compare(a, b) != 0;
        }

        public static Number operator +(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Add Number");
#endif
            return Operator.Add(a, b);
        }

        public static Number operator -(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Subtract Number");
#endif
            return Operator.Subtract(a, b);
        }

        public static Number operator *(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Multiply Number");
#endif
            return Operator.Multiply(a, b);
        }


        public static Number operator /(Number a, Number b)
        {
#if DEBUG
            Console.WriteLine("Divide Number");
#endif
            return Operator.Divide(a, b);
        }


        public static Number operator %(Number a, Number b)
        {
            Number totalResult = Operator.Divide(a, b);
            return new Number(a.Environment, new NumberSegments(new decimal[] { 0 }), totalResult.Fragment, totalResult.IsNegative);
        }

        #endregion

        //public bool SaveFile(string folderName, string fileName)
        //{
        //    try
        //    {

        //        IMathEnvironment<Number> environment = Environment;
        //        string numberString = new string(Segments.Select(x => environment.Key[(int)x]).Reverse().ToArray());


        //        StorageRepository.Create(folderName, fileName, numberString);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Write("SaveFile ERROR: " + ex.Message);
        //        return false;
        //    }
        //}

        internal Number Copy()
        {
            return new Number(Environment, Segments, Fragment, IsNegative);
        }

        internal Number AsNegativeNumber()
        {
            return new Number(Environment, Segments, Fragment, true);
        }

        internal Number AsPositiveNumber()
        {
            return new Number(Environment, Segments, Fragment, false);
        }

        internal Number Floor()
        {
            return new Number(Environment, Segments, null, IsNegative);
        }

        public static implicit operator decimal(Number d)
        {
            return d.Segments[0];
        }

        public static implicit operator long(Number d)
        {
            return (long)d.Segments[0];
        }


        public static implicit operator ulong(Number d)
        {
            return (ulong)d.Segments[0];
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Segments.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object other)
        {
            return Operator.Compare(this, (Number)other) == 0;
        }

        public bool Equals(Number other)
        {
            return Operator.Compare(this, other) == 0;
        }

        public int CompareTo(Number other)
        {
            return Operator.Compare(this, other);
        }

        public Number Convert(IMathEnvironment<Number> environment)
        {
            return Operator.Convert(environment, this);
        }

        public Number Square()
        {
            return Operator.Square(this);
        }

        public Number SquareRoot()
        {
            return Operator.SquareRoot(this);
        }

        public string GetCharArray(IMathEnvironment<Number> environment = default)
        {
            string result = string.Empty;
            if (IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment<Number>) && environment != Environment)
            {
                resultSegments = Convert(environment).Segments;
            }
            else
            {
                environment = Environment;
                resultSegments = Segments;
            }

            result += string.Concat(resultSegments.Select(x => environment.Key[(int)x]).Reverse());

            if (Fragment != default)
            {
                result = string.Format("{0} {1}", result, Fragment);
            }

            return result;
        }

        public string GetDecimalArray(IMathEnvironment<Number> environment = default)
        {
            string result = string.Empty;
            if (IsNegative)
            {
                result = "-" + result;
            }

            NumberSegments resultSegments;
            if (environment != default(IMathEnvironment<Number>) && environment != Environment)
            {
                resultSegments = Convert(environment).Segments;
            }
            else
            {
                resultSegments = Segments;
            }

            result += string.Join(',', resultSegments.Reverse());

            if (Fragment != default)
            {
                result = string.Format("{0} {1}", result, Fragment);
            }

            return result;
        }
        public string GetDisplayValue()
        {
            //return "Disabled";
            string result = string.Empty;

            if (IsNegative)
            {
                result = "-" + result;
            }

            result = string.Join("", Environment.ConvertToString(Segments));

            if (Fragment != default)
            {
                result = string.Format("{0} {1}", result, Fragment);
            }

            return result;
        }

        public string ToString(IMathEnvironment<Number> environment)
        {
            return GetDecimalArray(environment);
        }

        public override string ToString()
        {
            return GetDisplayValue();
        }

        public Number ConvertToBase10()
        {
            return Operator.ConvertToBase10(this);
        }

        public void Dispose()
        {
            Fragment = default;

            First = 0;

            IsNegative = false;

            Environment = default;

            Segments.Dispose();
        }
    }
}