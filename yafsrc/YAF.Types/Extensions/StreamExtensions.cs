/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Extensions
{
    using System.IO;

    /// <summary>
    /// The stream extensions.
    /// </summary>
    public static class StreamExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts a Stream to a String.
        /// </summary>
        /// <param name="theStream">
        /// </param>
        /// <returns>
        /// The stream to string. 
        /// </returns>
        public static string AsString(this Stream theStream)
        {
            var reader = new StreamReader(theStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// The copy stream.
        /// </summary>
        /// <param name="input">
        /// The input. 
        /// </param>
        /// <param name="output">
        /// The output. 
        /// </param>
        public static void CopyTo(this Stream input, Stream output)
        {
            var buffer = new byte[1024];
            int count = buffer.Length;

            while (count > 0)
            {
                count = input.Read(buffer, 0, count);
                if (count > 0)
                {
                    output.Write(buffer, 0, count);
                }
            }
        }

        /// <summary>
        /// Reads the stream into a byte array.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] ToArray([NotNull] this Stream stream)
        {
            CodeContracts.VerifyNotNull(stream, "stream");

            var data = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(data, 0, (int)stream.Length);

            return data;
        }

        #endregion
    }
}