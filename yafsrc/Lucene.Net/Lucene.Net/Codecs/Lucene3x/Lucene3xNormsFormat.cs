using System;
using System.Runtime.CompilerServices;

namespace YAF.Lucene.Net.Codecs.Lucene3x
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

    using SegmentReadState = YAF.Lucene.Net.Index.SegmentReadState;
    using SegmentWriteState = YAF.Lucene.Net.Index.SegmentWriteState;

    /// <summary>
    /// Lucene3x ReadOnly <see cref="NormsFormat"/> implementation.
    /// <para/>
    /// @lucene.experimental
    /// </summary>
    [Obsolete("(4.0) this is only used to read indexes created before 4.0.")]
    internal class Lucene3xNormsFormat : NormsFormat
    {
        public override DocValuesConsumer NormsConsumer(SegmentWriteState state)
        {
            throw UnsupportedOperationException.Create("this codec can only be used for reading");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override DocValuesProducer NormsProducer(SegmentReadState state)
        {
            return new Lucene3xNormsProducer(state.Directory, state.SegmentInfo, state.FieldInfos, state.Context);
        }
    }
}