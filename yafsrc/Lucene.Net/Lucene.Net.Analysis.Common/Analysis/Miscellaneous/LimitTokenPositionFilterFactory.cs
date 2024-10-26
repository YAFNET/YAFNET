// Lucene version compatibility level 4.8.1
using YAF.Lucene.Net.Analysis.Util;
using System;
using System.Collections.Generic;

namespace YAF.Lucene.Net.Analysis.Miscellaneous
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
    /// Factory for <see cref="LimitTokenPositionFilter"/>. 
    /// <code>
    /// &lt;fieldType name="text_limit_pos" class="solr.TextField" positionIncrementGap="100"&gt;
    ///   &lt;analyzer&gt;
    ///     &lt;tokenizer class="solr.WhitespaceTokenizerFactory"/&gt;
    ///     &lt;filter class="solr.LimitTokenPositionFilterFactory" maxTokenPosition="3" consumeAllTokens="false" /&gt;
    ///   &lt;/analyzer&gt;
    /// &lt;/fieldType&gt;</code>
    /// <para>
    /// The <see cref="consumeAllTokens"/> property is optional and defaults to <c>false</c>.  
    /// See <see cref="LimitTokenPositionFilter"/> for an explanation of its use.
    /// </para>
    /// </summary>
    public class LimitTokenPositionFilterFactory : TokenFilterFactory
    {
        public const string MAX_TOKEN_POSITION_KEY = "maxTokenPosition";
        public const string CONSUME_ALL_TOKENS_KEY = "consumeAllTokens";
        private readonly int maxTokenPosition;
        private readonly bool consumeAllTokens;

        /// <summary>
        /// Creates a new <see cref="LimitTokenPositionFilterFactory"/> </summary>
        public LimitTokenPositionFilterFactory(IDictionary<string, string> args)
            : base(args)
        {
            maxTokenPosition = RequireInt32(args, MAX_TOKEN_POSITION_KEY);
            consumeAllTokens = GetBoolean(args, CONSUME_ALL_TOKENS_KEY, false);
            if (args.Count > 0)
            {
                throw new ArgumentException(string.Format(J2N.Text.StringFormatter.CurrentCulture, "Unknown parameters: {0}", args));
            }
        }

        public override TokenStream Create(TokenStream input)
        {
            return new LimitTokenPositionFilter(input, maxTokenPosition, consumeAllTokens);
        }
    }
}