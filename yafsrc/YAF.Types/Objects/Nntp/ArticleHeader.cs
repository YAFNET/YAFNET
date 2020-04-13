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

    /// <summary>
    /// The article header.
    /// </summary>
    public class ArticleHeader
    {
        /// <summary>
        /// Gets or sets ReferenceIds.
        /// </summary>
        public string[] ReferenceIds { get; set; }

        /// <summary>
        /// Gets or sets an Article Time Zone offset to UTC.
        /// </summary>
        public int TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets Subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets Date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets From.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets Sender.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets PostingHost.
        /// </summary>
        public string PostingHost { get; set; }

        /// <summary>
        /// Gets or sets LineCount.
        /// </summary>
        public int LineCount { get; set; }
    }
}