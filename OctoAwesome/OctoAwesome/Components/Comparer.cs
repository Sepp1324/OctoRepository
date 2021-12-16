using System;

namespace OctoAwesome.Components
{
    public struct Comparer<TOriginal, TSelected> : IEquatable<Comparer<TOriginal, TSelected>>, IEquatable<TOriginal>
    {
        public TOriginal Value { get; set; }

        private readonly Func<TOriginal, TSelected, bool> compareFunc;

        public Comparer(TOriginal value, Func<TOriginal, TSelected, bool> compareFunc)
        {
            Value = value;
            this.compareFunc = compareFunc;
        }

        public override bool Equals(object obj)
        {
            return obj is Comparer<TOriginal, TSelected> comparer && Equals(comparer)
                   || obj is TSelected rigth && Equals(rigth)
                   || obj is TOriginal left && Equals(left);
        }

        public bool Equals(Comparer<TOriginal, TSelected> other)
        {
            return Equals(other.Value);
        }

        public bool Equals(TOriginal other)
        {
            return Equals(Value, other);
        }

        public bool Equals(TSelected other)
        {
            return compareFunc(Value, other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Comparer<TOriginal, TSelected> left, Comparer<TOriginal, TSelected> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Comparer<TOriginal, TSelected> left, Comparer<TOriginal, TSelected> right)
        {
            return !(left == right);
        }

        public static bool operator ==(TOriginal left, Comparer<TOriginal, TSelected> right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(TOriginal left, Comparer<TOriginal, TSelected> right)
        {
            return !(left == right);
        }

        public static bool operator ==(Comparer<TOriginal, TSelected> left, TOriginal right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Comparer<TOriginal, TSelected> left, TOriginal right)
        {
            return !(left == right);
        }

        public static bool operator ==(TSelected left, Comparer<TOriginal, TSelected> right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(TSelected left, Comparer<TOriginal, TSelected> right)
        {
            return !(left == right);
        }

        public static bool operator ==(Comparer<TOriginal, TSelected> left, TSelected right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Comparer<TOriginal, TSelected> left, TSelected right)
        {
            return !(left == right);
        }

        public static implicit operator TOriginal(Comparer<TOriginal, TSelected> comparer)
        {
            return comparer.Value;
        }

        public static implicit operator Comparer<TOriginal, TSelected>(
            (TOriginal value, Func<TOriginal, TSelected, bool> comparer) tuple)
        {
            return new(tuple.value, tuple.comparer);
        }

        public static implicit operator (TOriginal value, Func<TOriginal, TSelected, bool> comparer)(
            Comparer<TOriginal, TSelected> comparer)
        {
            return (comparer.Value, comparer.compareFunc);
        }
    }
}