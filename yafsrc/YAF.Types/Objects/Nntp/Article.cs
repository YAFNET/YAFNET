/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Objects.Nntp
{
    using System;
    using System.Collections;

    /// <summary>
    /// The article.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Gets or sets MessageId.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets ArticleId.
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets Header.
        /// </summary>
        public ArticleHeader Header { get; set; }

        /// <summary>
        /// Gets or sets Body.
        /// </summary>
        public ArticleBody Body { get; set; }

        /// <summary>
        /// Gets or sets LastReply.
        /// </summary>
        public DateTime LastReply { get; set; }

        /// <summary>
        /// Gets or sets Children.
        /// </summary>
        public ArrayList Children { get; set; }

        /// <summary>
        /// Gets or sets the mime part.
        /// </summary>
        public MIMEPart MimePart { get; set; }
    }
}