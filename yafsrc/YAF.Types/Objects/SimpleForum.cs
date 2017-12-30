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
  using System.Collections.Generic;

  #endregion

  /// <summary>
  /// The simple forum.
  /// </summary>
  [Serializable]
  public class SimpleForum
  {
    #region Constants and Fields

    /// <summary>
    /// The _topics.
    /// </summary>
    private List<SimpleTopic> _topics = new List<SimpleTopic>();

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Topics.
    /// </summary>
    public List<SimpleTopic> Topics
    {
      get
      {
        return this._topics;
      }

      set
      {
        this._topics = value;
      }
    }

    #endregion
  }
}