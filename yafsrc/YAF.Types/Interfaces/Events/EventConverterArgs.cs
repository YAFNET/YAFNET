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
namespace YAF.Types
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The event converter args.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public class EventConverterArgs<T> : EventArgs
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="EventConverterArgs{T}"/> class.
    /// </summary>
    /// <param name="eventData">
    /// The event data.
    /// </param>
    public EventConverterArgs(T eventData)
    {
      this.EventData = eventData;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets EventData.
    /// </summary>
    public T EventData { get; set; }

    #endregion
  }
}