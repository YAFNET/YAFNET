﻿// Lucene version compatibility level 4.8.1
using YAF.Lucene.Net.Index;
using YAF.Lucene.Net.Search;
using System.Collections;

namespace YAF.Lucene.Net.Queries.Function.ValueSources
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
    /// Returns the value of <see cref="IndexReader.MaxDoc"/>
    /// for every document. This is the number of documents
    /// including deletions.
    /// </summary>
    public class MaxDocValueSource : ValueSource
    {
        public virtual string Name => "maxdoc";

        public override string GetDescription()
        {
            return Name + "()";
        }

        public override void CreateWeight(IDictionary context, IndexSearcher searcher)
        {
            context["searcher"] = searcher;
        }

        public override FunctionValues GetValues(IDictionary context, AtomicReaderContext readerContext)
        {
            var searcher = (IndexSearcher)context["searcher"];
            return new ConstInt32DocValues(searcher.IndexReader.MaxDoc, this);
        }

        public override bool Equals(object o)
        {
            return this.GetType() == o.GetType();
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }
    }
}