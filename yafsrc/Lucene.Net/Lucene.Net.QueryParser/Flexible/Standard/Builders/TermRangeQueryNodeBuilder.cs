﻿using YAF.Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using YAF.Lucene.Net.QueryParsers.Flexible.Core.Util;
using YAF.Lucene.Net.QueryParsers.Flexible.Standard.Nodes;
using YAF.Lucene.Net.QueryParsers.Flexible.Standard.Processors;
using YAF.Lucene.Net.Search;

namespace YAF.Lucene.Net.QueryParsers.Flexible.Standard.Builders
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
    /// Builds a <see cref="TermRangeQuery"/> object from a <see cref="TermRangeQueryNode"/>
    /// object.
    /// </summary>
    public class TermRangeQueryNodeBuilder : IStandardQueryBuilder
    {
        public TermRangeQueryNodeBuilder()
        {
            // empty constructor
        }

        public virtual Query Build(IQueryNode queryNode)
        {
            TermRangeQueryNode rangeNode = (TermRangeQueryNode)queryNode;
            FieldQueryNode upper = (FieldQueryNode)rangeNode.UpperBound;
            FieldQueryNode lower = (FieldQueryNode)rangeNode.LowerBound;

            string field = StringUtils.ToString(rangeNode.Field);
            string lowerText = lower.GetTextAsString();
            string upperText = upper.GetTextAsString();

            if (lowerText.Length == 0)
            {
                lowerText = null;
            }

            if (upperText.Length == 0)
            {
                upperText = null;
            }

            TermRangeQuery rangeQuery = TermRangeQuery.NewStringRange(field, lowerText, upperText, rangeNode
                .IsLowerInclusive, rangeNode.IsUpperInclusive);

            MultiTermQuery.RewriteMethod method = (MultiTermQuery.RewriteMethod)queryNode
                .GetTag(MultiTermRewriteMethodProcessor.TAG_ID);
            if (method != null)
            {
                rangeQuery.MultiTermRewriteMethod = method;
            }

            return rangeQuery;
        }
    }
}
