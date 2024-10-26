﻿using YAF.Lucene.Net.Diagnostics;
using YAF.Lucene.Net.Index;
using System;
using System.Diagnostics;

namespace YAF.Lucene.Net.Codecs.Lucene46
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

    using Directory = YAF.Lucene.Net.Store.Directory;
    using DocValuesType = YAF.Lucene.Net.Index.DocValuesType;
    using FieldInfo = YAF.Lucene.Net.Index.FieldInfo;
    using FieldInfos = YAF.Lucene.Net.Index.FieldInfos;
    using IndexFileNames = YAF.Lucene.Net.Index.IndexFileNames;
    using IndexOptions = YAF.Lucene.Net.Index.IndexOptions;
    using IndexOutput = YAF.Lucene.Net.Store.IndexOutput;
    using IOContext = YAF.Lucene.Net.Store.IOContext;
    using IOUtils = YAF.Lucene.Net.Util.IOUtils;

    /// <summary>
    /// Lucene 4.6 FieldInfos writer.
    /// <para/>
    /// @lucene.experimental 
    /// </summary>
    /// <seealso cref="Lucene46FieldInfosFormat"/>
    internal sealed class Lucene46FieldInfosWriter : FieldInfosWriter
    {
        /// <summary>
        /// Sole constructor. </summary>
        public Lucene46FieldInfosWriter()
        {
        }

        public override void Write(Directory directory, string segmentName, string segmentSuffix, FieldInfos infos, IOContext context)
        {
            string fileName = IndexFileNames.SegmentFileName(segmentName, segmentSuffix, Lucene46FieldInfosFormat.EXTENSION);
            IndexOutput output = directory.CreateOutput(fileName, context);
            bool success = false;
            try
            {
                CodecUtil.WriteHeader(output, Lucene46FieldInfosFormat.CODEC_NAME, Lucene46FieldInfosFormat.FORMAT_CURRENT);
                output.WriteVInt32(infos.Count);
                foreach (FieldInfo fi in infos)
                {
                    IndexOptions indexOptions = fi.IndexOptions;
                    sbyte bits = 0x0;
                    if (fi.HasVectors)
                    {
                        bits |= Lucene46FieldInfosFormat.STORE_TERMVECTOR;
                    }
                    if (fi.OmitsNorms)
                    {
                        bits |= Lucene46FieldInfosFormat.OMIT_NORMS;
                    }
                    if (fi.HasPayloads)
                    {
                        bits |= Lucene46FieldInfosFormat.STORE_PAYLOADS;
                    }
                    if (fi.IsIndexed)
                    {
                        bits |= Lucene46FieldInfosFormat.IS_INDEXED;
                        // LUCENENET specific - to avoid boxing, changed from CompareTo() to IndexOptionsComparer.Compare()
                        if (Debugging.AssertsEnabled) Debugging.Assert(IndexOptionsComparer.Default.Compare(indexOptions, IndexOptions.DOCS_AND_FREQS_AND_POSITIONS) >= 0 || !fi.HasPayloads);
                        if (indexOptions == IndexOptions.DOCS_ONLY)
                        {
                            bits |= Lucene46FieldInfosFormat.OMIT_TERM_FREQ_AND_POSITIONS;
                        }
                        else if (indexOptions == IndexOptions.DOCS_AND_FREQS_AND_POSITIONS_AND_OFFSETS)
                        {
                            bits |= Lucene46FieldInfosFormat.STORE_OFFSETS_IN_POSTINGS;
                        }
                        else if (indexOptions == IndexOptions.DOCS_AND_FREQS)
                        {
                            bits |= Lucene46FieldInfosFormat.OMIT_POSITIONS;
                        }
                    }
                    output.WriteString(fi.Name);
                    output.WriteVInt32(fi.Number);
                    output.WriteByte((byte)bits);

                    // pack the DV types in one byte
                    var dv = DocValuesByte(fi.DocValuesType);
                    var nrm = DocValuesByte(fi.NormType);
                    if (Debugging.AssertsEnabled) Debugging.Assert((dv & (~0xF)) == 0 && (nrm & (~0x0F)) == 0);
                    var val = (byte)(0xff & ((nrm << 4) | (byte)dv));
                    output.WriteByte(val);
                    output.WriteInt64(fi.DocValuesGen);
                    output.WriteStringStringMap(fi.Attributes);
                }
                CodecUtil.WriteFooter(output);
                success = true;
            }
            finally
            {
                if (success)
                {
                    output.Dispose();
                }
                else
                {
                    IOUtils.DisposeWhileHandlingException(output);
                }
            }
        }

        private static sbyte DocValuesByte(DocValuesType type)
        {
            if (type == DocValuesType.NONE)
            {
                return 0;
            }
            else if (type == DocValuesType.NUMERIC)
            {
                return 1;
            }
            else if (type == DocValuesType.BINARY)
            {
                return 2;
            }
            else if (type == DocValuesType.SORTED)
            {
                return 3;
            }
            else if (type == DocValuesType.SORTED_SET)
            {
                return 4;
            }
            else
            {
                throw AssertionError.Create();
            }
        }
    }
}