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
namespace YAF.Types.Attributes
{
  using System;

  /// <summary>
  /// This attribute is used to represent a string value
  /// for a value in an enum.
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum)]
  public class AltStringValueAttribute : Attribute
  {
    #region Properties

    /// <summary>
    /// Holds the stringvalue for a value in an enum.
    /// </summary>
    public string AltStringValue
    {
      get;
      protected set;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AltStringValueAttribute"/> class. 
    /// Constructor used to init a StringValue Attribute
    /// </summary>
    /// <param name="value">
    /// </param>
    public AltStringValueAttribute(string value)
    {
      AltStringValue = value;
    }

    #endregion
  }
}