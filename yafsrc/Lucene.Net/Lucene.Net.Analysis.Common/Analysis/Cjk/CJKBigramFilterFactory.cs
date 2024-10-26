// Lucene version compatibility level 4.8.1
using YAF.Lucene.Net.Analysis.Util;
using System;
using System.Collections.Generic;

namespace YAF.Lucene.Net.Analysis.Cjk
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
    /// Factory for <see cref="CJKBigramFilter"/>.
    /// <code>
    /// &lt;fieldType name="text_cjk" class="solr.TextField"&gt;
    ///   &lt;analyzer&gt;
    ///     &lt;tokenizer class="solr.StandardTokenizerFactory"/&gt;
    ///     &lt;filter class="solr.CJKWidthFilterFactory"/&gt;
    ///     &lt;filter class="solr.LowerCaseFilterFactory"/&gt;
    ///     &lt;filter class="solr.CJKBigramFilterFactory" 
    ///       han="true" hiragana="true" 
    ///       katakana="true" hangul="true" outputUnigrams="false" /&gt;
    ///   &lt;/analyzer&gt;
    /// &lt;/fieldType&gt;
    /// </code>
    /// </summary>
    public class CJKBigramFilterFactory : TokenFilterFactory
    {
        private readonly CJKScript flags;
        private readonly bool outputUnigrams;

        /// <summary>
        /// Creates a new <see cref="CJKBigramFilterFactory"/> </summary>
        public CJKBigramFilterFactory(IDictionary<string, string> args)
              : base(args)
        {
            CJKScript flags = 0;
            if (GetBoolean(args, "han", true))
            {
                flags |= CJKScript.HAN;
            }
            if (GetBoolean(args, "hiragana", true))
            {
                flags |= CJKScript.HIRAGANA;
            }
            if (GetBoolean(args, "katakana", true))
            {
                flags |= CJKScript.KATAKANA;
            }
            if (GetBoolean(args, "hangul", true))
            {
                flags |= CJKScript.HANGUL;
            }
            this.flags = flags;
            this.outputUnigrams = GetBoolean(args, "outputUnigrams", false);
            if (args.Count > 0)
            {
                throw new ArgumentException(string.Format(J2N.Text.StringFormatter.CurrentCulture, "Unknown parameters: {0}", args));
            }
        }

        public override TokenStream Create(TokenStream input)
        {
            return new CJKBigramFilter(input, flags, outputUnigrams);
        }
    }
}