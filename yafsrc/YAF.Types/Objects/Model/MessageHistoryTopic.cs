/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Types.Objects.Model
{
    using System;

    public class MessageHistoryTopic
    {
        public string EditReason { get; set; }

        public DateTime Edited { get; set; }

        public int EditedBy { get; set; }

        public int Flags { get; set; }

        public string IP { get; set; }

        public bool? IsModeratorChanged { get; set; }

        public int MessageID { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string UserStyle { get; set; }

        public DateTime? Suspended { get; set; }

        public int ForumID { get; set; }

        public int TopicID { get; set; }

        public string Topic { get; set; }

        public DateTime Posted { get; set; }

        public string MessageIP { get; set; }
    }
}
