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

namespace YAF.Types.Exceptions
{
    using System;
    using YAF.Types.Extensions;

    /// <summary>
    /// The No TopicRow Exception
    /// </summary>
    public class NoTopicRowException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoTopicRowException"/> class.
        /// </summary>
        /// <param name="topicRowID">The topic row identifier.</param>
        public NoTopicRowException(int? topicRowID)
            : base("No topic row found for topic row id [{0}]".FormatWith(topicRowID ?? 0))
        {
        }
    }
}