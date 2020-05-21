using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data
{
    /// <summary>
    /// Implements the <see cref="IEqualityComparer{T}"/> interface by comparing two keys generated from the objects being compared via a delegate.
    /// </summary>
    /// <typeparam name="TSource">The type of the objects to compare.</typeparam>
    /// <typeparam name="TKey">The type of the key value to be compared.</typeparam>
    public class LambdaEqualityComparer<TSource, TKey> : IEqualityComparer<TSource>
    {
        //======================================================
        //      Private fields and constants
        //======================================================
        private Func<TSource, TKey> keySelector;

        //======================================================
        //      Constructors and finalizers
        //======================================================
        public LambdaEqualityComparer(Func<TSource, TKey> keySelector)
        {
            this.keySelector = keySelector;
        }

        //======================================================
        //      Public methods
        //======================================================
        public bool Equals(TSource x, TSource y)
        {
            if (x == null || y == null)
                return x == null && y == null;

            return Equals(keySelector(x), keySelector(y));

        }

        public int GetHashCode(TSource obj)
        {
            if (obj == null)
                return int.MinValue;

            var k = keySelector(obj);
            if (k == null)
                return int.MaxValue;

            return k.GetHashCode();
        }

        //======================================================
        //      Properties
        //======================================================

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.Data
