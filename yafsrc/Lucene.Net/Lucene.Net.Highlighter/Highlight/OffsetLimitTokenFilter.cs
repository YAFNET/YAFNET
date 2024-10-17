﻿using YAF.Lucene.Net.Analysis;
using YAF.Lucene.Net.Analysis.TokenAttributes;

namespace YAF.Lucene.Net.Search.Highlight
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
    /// This <see cref="TokenFilter"/> limits the number of tokens while indexing by adding up the
    /// current offset.
    /// </summary>
    public sealed class OffsetLimitTokenFilter : TokenFilter
    {
        private int offsetCount;
        private readonly IOffsetAttribute offsetAttrib;
        private readonly int offsetLimit;

        public OffsetLimitTokenFilter(TokenStream input, int offsetLimit) : base(input)
        {
            this.offsetLimit = offsetLimit;
            offsetAttrib = GetAttribute<IOffsetAttribute>();
        }

        public override bool IncrementToken()
        {
            if (offsetCount < offsetLimit && m_input.IncrementToken())
            {
                int offsetLength = offsetAttrib.EndOffset - offsetAttrib.StartOffset;
                offsetCount += offsetLength;
                return true;
            }
            return false;
        }

        public override void Reset()
        {
            base.Reset();
            offsetCount = 0;
        }
    }
}
