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
  using System.Data;

  #endregion

  /// <summary>
  /// The typed poll group.
  /// </summary>
  [Serializable]
  public class TypedPollGroup
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedPollGroup"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedPollGroup(DataRow row)
    {
      this.Question = row.Field<string>("Question");
      this.PollGroupID = row.Field<int?>("PollGroupID");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedPollGroup"/> class.
    /// </summary>
    /// <param name="question">
    /// The question.
    /// </param>
    /// <param name="pollgroupid">
    /// The pollgroupid.
    /// </param>
    public TypedPollGroup(string question, int? pollgroupid)
    {
      this.Question = question;
      this.PollGroupID = pollgroupid;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets PollGroupID.
    /// </summary>
    public int? PollGroupID { get; set; }

    /// <summary>
    /// Gets or sets Question.
    /// </summary>
    public string Question { get; set; }

    #endregion
  }
}