﻿using J2N.Numerics;
using System.Runtime.CompilerServices;

namespace YAF.Lucene.Net.Util
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

    using DocIdSetIterator = YAF.Lucene.Net.Search.DocIdSetIterator;

    /// <summary>
    /// An iterator to iterate over set bits in an <see cref="OpenBitSet"/>.
    /// this is faster than <see cref="OpenBitSet.NextSetBit(long)"/> for iterating over the complete set of bits,
    /// especially when the density of the bits set is high.
    /// </summary>
    public class OpenBitSetIterator : DocIdSetIterator
    {
        // hmmm, what about an iterator that finds zeros though,
        // or a reverse iterator... should they be separate classes
        // for efficiency, or have a common root interface?  (or
        // maybe both?  could ask for a SetBitsIterator, etc...

        internal readonly long[] arr;
        internal readonly int words;
        private int i = -1;
        private long word;
        private int wordShift;
        private int indexArray;
        private int curDocId = -1;

        public OpenBitSetIterator(OpenBitSet obs)
            : this(obs.GetBits(), obs.NumWords)
        {
        }

        public OpenBitSetIterator(long[] bits, int numWords)
        {
            arr = bits;
            words = numWords;
        }

        // 64 bit shifts
        private void Shift()
        {
            if ((int)word == 0)
            {
                wordShift += 32;
                word = word.TripleShift(32);
            }
            if ((word & 0x0000FFFF) == 0)
            {
                wordShift += 16;
                word = word.TripleShift(16);
            }
            if ((word & 0x000000FF) == 0)
            {
                wordShift += 8;
                word = word.TripleShift(8);
            }
            indexArray = BitUtil.BitList((byte)word);
        }

        //*** alternate shift implementations
        // // 32 bit shifts, but a long shift needed at the end
        // private void shift2() {
        //  int y = (int)word;
        //  if (y==0) {wordShift +=32; y = (int)(word >>>32); }
        //  if ((y & 0x0000FFFF) == 0) { wordShift +=16; y>>>=16; }
        //  if ((y & 0x000000FF) == 0) { wordShift +=8; y>>>=8; }
        //  indexArray = bitlist[y & 0xff];
        //  word >>>= (wordShift +1);
        // }
        //
        // private void shift3() {
        //  int lower = (int)word;
        //  int lowByte = lower & 0xff;
        //  if (lowByte != 0) {
        //    indexArray=bitlist[lowByte];
        //    return;
        //  }
        //  shift();
        // }
        // *****

        public override int NextDoc()
        {
            if (indexArray == 0)
            {
                if (word != 0)
                {
                    word = word.TripleShift(8);
                    wordShift += 8;
                }

                while (word == 0)
                {
                    if (++i >= words)
                    {
                        return curDocId = NO_MORE_DOCS;
                    }
                    word = arr[i];
                    wordShift = -1; // loop invariant code motion should move this
                }

                // after the first time, should I go with a linear search, or
                // stick with the binary search in shift?
                Shift();
            }

            int bitIndex = (indexArray & 0x0f) + wordShift;
            indexArray = indexArray.TripleShift(4);
            // should i<<6 be cached as a separate variable?
            // it would only save one cycle in the best circumstances.
            return curDocId = (i << 6) + bitIndex;
        }

        public override int Advance(int target)
        {
            indexArray = 0;
            i = target >> 6;
            if (i >= words)
            {
                word = 0; // setup so next() will also return -1
                return curDocId = NO_MORE_DOCS;
            }
            wordShift = target & 0x3f;
            word = arr[i].TripleShift(wordShift);
            if (word != 0)
            {
                wordShift--; // compensate for 1 based arrIndex
            }
            else
            {
                while (word == 0)
                {
                    if (++i >= words)
                    {
                        return curDocId = NO_MORE_DOCS;
                    }
                    word = arr[i];
                }
                wordShift = -1;
            }

            Shift();

            int bitIndex = (indexArray & 0x0f) + wordShift;
            indexArray = indexArray.TripleShift(4);
            // should i<<6 be cached as a separate variable?
            // it would only save one cycle in the best circumstances.
            return curDocId = (i << 6) + bitIndex;
        }

        public override int DocID => curDocId;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long GetCost()
        {
            return words / 64;
        }
    }
}