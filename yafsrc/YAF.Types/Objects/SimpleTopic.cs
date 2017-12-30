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
namespace YAF.Types.Objects
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The simple topic.
  /// </summary>
  [Serializable]
  public class SimpleTopic
  {
    #region Properties

    /// <summary>
    /// Gets or sets CreatedDate.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets FirstMessage.
    /// </summary>
    public string FirstMessage { get; set; }

    /// <summary>
    /// Gets or sets Forum.
    /// </summary>
    public SimpleForum Forum { get; set; }

    /// <summary>
    /// Gets or sets LastMessage.
    /// </summary>
    public string LastMessage { get; set; }

    /// <summary>
    /// Gets or sets LastMessageID.
    /// </summary>
    public int LastMessageID { get; set; }

    /// <summary>
    /// Gets or sets LastPostDate.
    /// </summary>
    public DateTime LastPostDate { get; set; }

    /// <summary>
    /// Gets or sets LastUserID.
    /// </summary>
    public int LastUserID { get; set; }

    /// <summary>
    /// Gets or sets LastUserName.
    /// </summary>
    public string LastUserName { get; set; }

    /// <summary>
    /// Gets or sets Replies.
    /// </summary>
    public int Replies { get; set; }

    /// <summary>
    /// Gets or sets StartedUserID.
    /// </summary>
    public int StartedUserID { get; set; }

    /// <summary>
    /// Gets or sets StartedUserName.
    /// </summary>
    public string StartedUserName { get; set; }

    /// <summary>
    /// Gets or sets Subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets TopicID.
    /// </summary>
    public int TopicID { get; set; }

    #endregion
  }
}