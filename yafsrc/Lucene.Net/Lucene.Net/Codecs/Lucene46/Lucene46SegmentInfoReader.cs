using System.Collections.Generic;

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

    using ChecksumIndexInput = YAF.Lucene.Net.Store.ChecksumIndexInput;
    using CorruptIndexException = YAF.Lucene.Net.Index.CorruptIndexException;
    using Directory = YAF.Lucene.Net.Store.Directory;
    using IndexFileNames = YAF.Lucene.Net.Index.IndexFileNames;
    using IOContext = YAF.Lucene.Net.Store.IOContext;
    using IOUtils = YAF.Lucene.Net.Util.IOUtils;
    using SegmentInfo = YAF.Lucene.Net.Index.SegmentInfo;

    /// <summary>
    /// Lucene 4.6 implementation of <see cref="SegmentInfoReader"/>.
    /// <para/>
    /// @lucene.experimental
    /// </summary>
    /// <seealso cref="Lucene46SegmentInfoFormat"/>
    public class Lucene46SegmentInfoReader : SegmentInfoReader
    {
        /// <summary>
        /// Sole constructor. </summary>
        public Lucene46SegmentInfoReader()
        {
        }

        public override SegmentInfo Read(Directory dir, string segment, IOContext context)
        {
            string fileName = IndexFileNames.SegmentFileName(segment, "", Lucene46SegmentInfoFormat.SI_EXTENSION);
            ChecksumIndexInput input = dir.OpenChecksumInput(fileName, context);
            bool success = false;
            try
            {
                int codecVersion = CodecUtil.CheckHeader(input, Lucene46SegmentInfoFormat.CODEC_NAME, Lucene46SegmentInfoFormat.VERSION_START, Lucene46SegmentInfoFormat.VERSION_CURRENT);
                string version = input.ReadString();
                int docCount = input.ReadInt32();
                if (docCount < 0)
                {
                    throw new CorruptIndexException("invalid docCount: " + docCount + " (resource=" + input + ")");
                }
                bool isCompoundFile = input.ReadByte() == SegmentInfo.YES;
                IDictionary<string, string> diagnostics = input.ReadStringStringMap();
                ISet<string> files = input.ReadStringSet();

                if (codecVersion >= Lucene46SegmentInfoFormat.VERSION_CHECKSUM)
                {
                    CodecUtil.CheckFooter(input);
                }
                else
                {
#pragma warning disable 612, 618
                    CodecUtil.CheckEOF(input);
#pragma warning restore 612, 618
                }

                SegmentInfo si = new SegmentInfo(dir, version, segment, docCount, isCompoundFile, null, diagnostics);
                si.SetFiles(files);

                success = true;

                return si;
            }
            finally
            {
                if (!success)
                {
                    IOUtils.DisposeWhileHandlingException(input);
                }
                else
                {
                    input.Dispose();
                }
            }
        }
    }
}