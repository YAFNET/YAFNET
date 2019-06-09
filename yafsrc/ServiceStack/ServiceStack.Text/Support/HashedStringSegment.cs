using System;

#if NETSTANDARD2_0
using Microsoft.Extensions.Primitives;
#endif

namespace ServiceStack.Text.Support
{
    public class HashedStringSegment
    {
        public StringSegment Value { get; }
        private readonly int hash;

        public HashedStringSegment(StringSegment value)
        {
            this.Value = value;
            this.hash = ComputeHashCode(value);
        }

        public HashedStringSegment(string value) : this(new StringSegment(value))
        {
        }

        public override bool Equals(object obj)
        {
            return this.Value.Equals(((HashedStringSegment)obj).Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => this.hash;

        public static int ComputeHashCode(StringSegment value)
        {
            var length = value.Length;
            if (length == 0)
                return 0;

            var offset = value.Offset;
            var hash = 37 * length;

            var c1 = char.ToUpperInvariant(value.Buffer[offset]);
            hash += 53 * c1;

            if (length > 1)
            {
                var c2 = char.ToUpperInvariant(value.Buffer[offset + length - 1]);
                hash += 37 * c2;
            }

            return hash;
        }
    }
}