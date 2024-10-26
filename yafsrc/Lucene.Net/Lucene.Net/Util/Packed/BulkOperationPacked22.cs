﻿// this file has been automatically generated, DO NOT EDIT

using J2N.Numerics;

namespace YAF.Lucene.Net.Util.Packed
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
    /// Efficient sequential read/write of packed integers.
    /// </summary>
    internal sealed class BulkOperationPacked22 : BulkOperationPacked
    {
        public BulkOperationPacked22()
            : base(22)
        {
        }

        public override void Decode(long[] blocks, int blocksOffset, int[] values, int valuesOffset, int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                long block0 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(block0.TripleShift(42));
                values[valuesOffset++] = (int)((block0.TripleShift(20)) & 4194303L);
                long block1 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block0 & 1048575L) << 2) | (block1.TripleShift(62)));
                values[valuesOffset++] = (int)((block1.TripleShift(40)) & 4194303L);
                values[valuesOffset++] = (int)((block1.TripleShift(18)) & 4194303L);
                long block2 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block1 & 262143L) << 4) | (block2.TripleShift(60)));
                values[valuesOffset++] = (int)((block2.TripleShift(38)) & 4194303L);
                values[valuesOffset++] = (int)((block2.TripleShift(16)) & 4194303L);
                long block3 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block2 & 65535L) << 6) | (block3.TripleShift(58)));
                values[valuesOffset++] = (int)((block3.TripleShift(36)) & 4194303L);
                values[valuesOffset++] = (int)((block3.TripleShift(14)) & 4194303L);
                long block4 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block3 & 16383L) << 8) | (block4.TripleShift(56)));
                values[valuesOffset++] = (int)((block4.TripleShift(34)) & 4194303L);
                values[valuesOffset++] = (int)((block4.TripleShift(12)) & 4194303L);
                long block5 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block4 & 4095L) << 10) | (block5.TripleShift(54)));
                values[valuesOffset++] = (int)((block5.TripleShift(32)) & 4194303L);
                values[valuesOffset++] = (int)((block5.TripleShift(10)) & 4194303L);
                long block6 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block5 & 1023L) << 12) | (block6.TripleShift(52)));
                values[valuesOffset++] = (int)((block6.TripleShift(30)) & 4194303L);
                values[valuesOffset++] = (int)((block6.TripleShift(8)) & 4194303L);
                long block7 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block6 & 255L) << 14) | (block7.TripleShift(50)));
                values[valuesOffset++] = (int)((block7.TripleShift(28)) & 4194303L);
                values[valuesOffset++] = (int)((block7.TripleShift(6)) & 4194303L);
                long block8 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block7 & 63L) << 16) | (block8.TripleShift(48)));
                values[valuesOffset++] = (int)((block8.TripleShift(26)) & 4194303L);
                values[valuesOffset++] = (int)((block8.TripleShift(4)) & 4194303L);
                long block9 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block8 & 15L) << 18) | (block9.TripleShift(46)));
                values[valuesOffset++] = (int)((block9.TripleShift(24)) & 4194303L);
                values[valuesOffset++] = (int)((block9.TripleShift(2)) & 4194303L);
                long block10 = blocks[blocksOffset++];
                values[valuesOffset++] = (int)(((block9 & 3L) << 20) | (block10.TripleShift(44)));
                values[valuesOffset++] = (int)((block10.TripleShift(22)) & 4194303L);
                values[valuesOffset++] = (int)(block10 & 4194303L);
            }
        }

        public override void Decode(byte[] blocks, int blocksOffset, int[] values, int valuesOffset, int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                int byte0 = blocks[blocksOffset++] & 0xFF;
                int byte1 = blocks[blocksOffset++] & 0xFF;
                int byte2 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = (byte0 << 14) | (byte1 << 6) | (byte2.TripleShift(2));
                int byte3 = blocks[blocksOffset++] & 0xFF;
                int byte4 = blocks[blocksOffset++] & 0xFF;
                int byte5 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = ((byte2 & 3) << 20) | (byte3 << 12) | (byte4 << 4) | (byte5.TripleShift(4));
                int byte6 = blocks[blocksOffset++] & 0xFF;
                int byte7 = blocks[blocksOffset++] & 0xFF;
                int byte8 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = ((byte5 & 15) << 18) | (byte6 << 10) | (byte7 << 2) | (byte8.TripleShift(6));
                int byte9 = blocks[blocksOffset++] & 0xFF;
                int byte10 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = ((byte8 & 63) << 16) | (byte9 << 8) | byte10;
            }
        }

        public override void Decode(long[] blocks, int blocksOffset, long[] values, int valuesOffset, int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                long block0 = blocks[blocksOffset++];
                values[valuesOffset++] = block0.TripleShift(42);
                values[valuesOffset++] = (block0.TripleShift(20)) & 4194303L;
                long block1 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block0 & 1048575L) << 2) | (block1.TripleShift(62));
                values[valuesOffset++] = (block1.TripleShift(40)) & 4194303L;
                values[valuesOffset++] = (block1.TripleShift(18)) & 4194303L;
                long block2 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block1 & 262143L) << 4) | (block2.TripleShift(60));
                values[valuesOffset++] = (block2.TripleShift(38)) & 4194303L;
                values[valuesOffset++] = (block2.TripleShift(16)) & 4194303L;
                long block3 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block2 & 65535L) << 6) | (block3.TripleShift(58));
                values[valuesOffset++] = (block3.TripleShift(36)) & 4194303L;
                values[valuesOffset++] = (block3.TripleShift(14)) & 4194303L;
                long block4 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block3 & 16383L) << 8) | (block4.TripleShift(56));
                values[valuesOffset++] = (block4.TripleShift(34)) & 4194303L;
                values[valuesOffset++] = (block4.TripleShift(12)) & 4194303L;
                long block5 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block4 & 4095L) << 10) | (block5.TripleShift(54));
                values[valuesOffset++] = (block5.TripleShift(32)) & 4194303L;
                values[valuesOffset++] = (block5.TripleShift(10)) & 4194303L;
                long block6 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block5 & 1023L) << 12) | (block6.TripleShift(52));
                values[valuesOffset++] = (block6.TripleShift(30)) & 4194303L;
                values[valuesOffset++] = (block6.TripleShift(8)) & 4194303L;
                long block7 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block6 & 255L) << 14) | (block7.TripleShift(50));
                values[valuesOffset++] = (block7.TripleShift(28)) & 4194303L;
                values[valuesOffset++] = (block7.TripleShift(6)) & 4194303L;
                long block8 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block7 & 63L) << 16) | (block8.TripleShift(48));
                values[valuesOffset++] = (block8.TripleShift(26)) & 4194303L;
                values[valuesOffset++] = (block8.TripleShift(4)) & 4194303L;
                long block9 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block8 & 15L) << 18) | (block9.TripleShift(46));
                values[valuesOffset++] = (block9.TripleShift(24)) & 4194303L;
                values[valuesOffset++] = (block9.TripleShift(2)) & 4194303L;
                long block10 = blocks[blocksOffset++];
                values[valuesOffset++] = ((block9 & 3L) << 20) | (block10.TripleShift(44));
                values[valuesOffset++] = (block10.TripleShift(22)) & 4194303L;
                values[valuesOffset++] = block10 & 4194303L;
            }
        }

        public override void Decode(byte[] blocks, int blocksOffset, long[] values, int valuesOffset, int iterations)
        {
            for (int i = 0; i < iterations; ++i)
            {
                long byte0 = blocks[blocksOffset++] & 0xFF;
                long byte1 = blocks[blocksOffset++] & 0xFF;
                long byte2 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = (byte0 << 14) | (byte1 << 6) | (byte2.TripleShift(2));
                long byte3 = blocks[blocksOffset++] & 0xFF;
                long byte4 = blocks[blocksOffset++] & 0xFF;
                long byte5 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = ((byte2 & 3) << 20) | (byte3 << 12) | (byte4 << 4) | (byte5.TripleShift(4));
                long byte6 = blocks[blocksOffset++] & 0xFF;
                long byte7 = blocks[blocksOffset++] & 0xFF;
                long byte8 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = ((byte5 & 15) << 18) | (byte6 << 10) | (byte7 << 2) | (byte8.TripleShift(6));
                long byte9 = blocks[blocksOffset++] & 0xFF;
                long byte10 = blocks[blocksOffset++] & 0xFF;
                values[valuesOffset++] = ((byte8 & 63) << 16) | (byte9 << 8) | byte10;
            }
        }
    }
}