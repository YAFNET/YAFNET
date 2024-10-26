using J2N.Numerics;
using System.Runtime.CompilerServices;

namespace YAF.Lucene.Net.Util // from org.apache.solr.util rev 555343
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// A variety of high efficiency bit twiddling routines.
    /// <para/>
    /// @lucene.internal
    /// </summary>
    public static class BitUtil // LUCENENET specific - made static
    {
        private static readonly sbyte[] BYTE_COUNTS = new sbyte[] {
            0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
            4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8
        }; // table of bits/byte

        // The General Idea: instead of having an array per byte that has
        // the offsets of the next set bit, that array could be
        // packed inside a 32 bit integer (8 4 bit numbers).  That
        // should be faster than accessing an array for each index, and
        // the total array size is kept smaller (256*sizeof(int))=1K
        /// <summary>
        /// the python code that generated bitlist
        /// <code>
        /// def bits2int(val):
        /// arr=0
        /// for shift in range(8,0,-1):
        ///  if val &amp; 0x80:
        ///    arr = (arr &lt;&lt; 4) | shift
        ///  val = val &lt;&lt; 1
        /// return arr
        ///
        /// def int_table():
        ///  tbl = [ hex(bits2int(val)).strip('L') for val in range(256) ]
        ///  return ','.join(tbl)
        /// </code>
        /// </summary>
        private static readonly int[] BIT_LISTS = new int[] {
            0x0, 0x1, 0x2, 0x21, 0x3, 0x31, 0x32, 0x321, 0x4, 0x41, 0x42, 0x421, 0x43,
            0x431, 0x432, 0x4321, 0x5, 0x51, 0x52, 0x521, 0x53, 0x531, 0x532, 0x5321,
            0x54, 0x541, 0x542, 0x5421, 0x543, 0x5431, 0x5432, 0x54321, 0x6, 0x61, 0x62,
            0x621, 0x63, 0x631, 0x632, 0x6321, 0x64, 0x641, 0x642, 0x6421, 0x643,
            0x6431, 0x6432, 0x64321, 0x65, 0x651, 0x652, 0x6521, 0x653, 0x6531, 0x6532,
            0x65321, 0x654, 0x6541, 0x6542, 0x65421, 0x6543, 0x65431, 0x65432, 0x654321,
            0x7, 0x71, 0x72, 0x721, 0x73, 0x731, 0x732, 0x7321, 0x74, 0x741, 0x742,
            0x7421, 0x743, 0x7431, 0x7432, 0x74321, 0x75, 0x751, 0x752, 0x7521, 0x753,
            0x7531, 0x7532, 0x75321, 0x754, 0x7541, 0x7542, 0x75421, 0x7543, 0x75431,
            0x75432, 0x754321, 0x76, 0x761, 0x762, 0x7621, 0x763, 0x7631, 0x7632,
            0x76321, 0x764, 0x7641, 0x7642, 0x76421, 0x7643, 0x76431, 0x76432, 0x764321,
            0x765, 0x7651, 0x7652, 0x76521, 0x7653, 0x76531, 0x76532, 0x765321, 0x7654,
            0x76541, 0x76542, 0x765421, 0x76543, 0x765431, 0x765432, 0x7654321, 0x8,
            0x81, 0x82, 0x821, 0x83, 0x831, 0x832, 0x8321, 0x84, 0x841, 0x842, 0x8421,
            0x843, 0x8431, 0x8432, 0x84321, 0x85, 0x851, 0x852, 0x8521, 0x853, 0x8531,
            0x8532, 0x85321, 0x854, 0x8541, 0x8542, 0x85421, 0x8543, 0x85431, 0x85432,
            0x854321, 0x86, 0x861, 0x862, 0x8621, 0x863, 0x8631, 0x8632, 0x86321, 0x864,
            0x8641, 0x8642, 0x86421, 0x8643, 0x86431, 0x86432, 0x864321, 0x865, 0x8651,
            0x8652, 0x86521, 0x8653, 0x86531, 0x86532, 0x865321, 0x8654, 0x86541,
            0x86542, 0x865421, 0x86543, 0x865431, 0x865432, 0x8654321, 0x87, 0x871,
            0x872, 0x8721, 0x873, 0x8731, 0x8732, 0x87321, 0x874, 0x8741, 0x8742,
            0x87421, 0x8743, 0x87431, 0x87432, 0x874321, 0x875, 0x8751, 0x8752, 0x87521,
            0x8753, 0x87531, 0x87532, 0x875321, 0x8754, 0x87541, 0x87542, 0x875421,
            0x87543, 0x875431, 0x875432, 0x8754321, 0x876, 0x8761, 0x8762, 0x87621,
            0x8763, 0x87631, 0x87632, 0x876321, 0x8764, 0x87641, 0x87642, 0x876421,
            0x87643, 0x876431, 0x876432, 0x8764321, 0x8765, 0x87651, 0x87652, 0x876521,
            0x87653, 0x876531, 0x876532, 0x8765321, 0x87654, 0x876541, 0x876542,
            0x8765421, 0x876543, 0x8765431, 0x8765432, unchecked((int)0x87654321)
        };

        /// <summary>
        /// Return the number of bits sets in <paramref name="b"/>. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitCount(byte b)
        {
            return BYTE_COUNTS[b & 0xFF];
        }

        /// <summary>
        /// Return the list of bits which are set in <paramref name="b"/> encoded as followed:
        /// <code>(i >>> (4 * n)) &amp; 0x0F</code> is the offset of the n-th set bit of
        /// the given byte plus one, or 0 if there are n or less bits set in the given
        /// byte. For example <code>bitList(12)</code> returns 0x43:
        /// <list type="bullet">
        ///     <item><description><code>0x43 &amp; 0x0F</code> is 3, meaning the the first bit set is at offset 3-1 = 2,</description></item>
        ///     <item><description><code>(0x43 >>> 4) &amp; 0x0F</code> is 4, meaning there is a second bit set at offset 4-1=3,</description></item>
        ///     <item><description><code>(0x43 >>> 8) &amp; 0x0F</code> is 0, meaning there is no more bit set in this byte.</description></item>
        /// </list>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitList(byte b)
        {
            return BIT_LISTS[b & 0xFF];
        }

        // The pop methods used to rely on bit-manipulation tricks for speed but it
        // turns out that it is faster to use the Long.bitCount method (which is an
        // intrinsic since Java 6u18) in a naive loop, see LUCENE-2221

        /// <summary>
        /// Returns the number of set bits in an array of <see cref="long"/>s. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Pop_Array(long[] arr, int wordOffset, int numWords)
        {
            long popCount = 0;
            for (int i = wordOffset, end = wordOffset + numWords; i < end; ++i)
            {
                popCount += arr[i].PopCount();
            }
            return popCount;
        }

        /// <summary>
        /// Returns the popcount or cardinality of the two sets after an intersection.
        /// Neither array is modified.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Pop_Intersect(long[] arr1, long[] arr2, int wordOffset, int numWords)
        {
            long popCount = 0;
            for (int i = wordOffset, end = wordOffset + numWords; i < end; ++i)
            {
                popCount += (arr1[i] & arr2[i]).PopCount();
            }
            return popCount;
        }

        /// <summary>
        /// Returns the popcount or cardinality of the union of two sets.
        /// Neither array is modified.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Pop_Union(long[] arr1, long[] arr2, int wordOffset, int numWords)
        {
            long popCount = 0;
            for (int i = wordOffset, end = wordOffset + numWords; i < end; ++i)
            {
                popCount += (arr1[i] | arr2[i]).PopCount();
            }
            return popCount;
        }

        /// <summary>
        /// Returns the popcount or cardinality of A &amp; ~B.
        /// Neither array is modified.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Pop_AndNot(long[] arr1, long[] arr2, int wordOffset, int numWords)
        {
            long popCount = 0;
            for (int i = wordOffset, end = wordOffset + numWords; i < end; ++i)
            {
                popCount += (arr1[i] & ~arr2[i]).PopCount();
            }
            return popCount;
        }

        /// <summary>
        /// Returns the popcount or cardinality of A ^ B
        /// Neither array is modified.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Pop_Xor(long[] arr1, long[] arr2, int wordOffset, int numWords)
        {
            long popCount = 0;
            for (int i = wordOffset, end = wordOffset + numWords; i < end; ++i)
            {
                popCount += (arr1[i] ^ arr2[i]).PopCount();
            }
            return popCount;
        }

        /// <summary>
        /// Returns the next highest power of two, or the current value if it's already a power of two or zero </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextHighestPowerOfTwo(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        /// <summary>
        /// Returns the next highest power of two, or the current value if it's already a power of two or zero </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NextHighestPowerOfTwo(long v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v |= v >> 32;
            v++;
            return v;
        }
    }
}