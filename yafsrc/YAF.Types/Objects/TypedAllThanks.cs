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
  /// The typed all thanks.
  /// </summary>
  [Serializable]
  public class TypedAllThanks
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedAllThanks"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedAllThanks(DataRow row)
    {
      this.MessageID = row.Field<int?>("MessageID");
      this.FromUserID = row.Field<int?>("FromUserID");
      this.ThanksDate = row.Field<DateTime?>("ThanksDate");
      this.ThanksFromUserNumber = row.Field<int?>("ThanksFromUserNumber");
      this.ThanksToUserNumber = row.Field<int?>("ThanksToUserNumber");
      this.ThanksToUserPostsNumber = row.Field<int?>("ThanksToUserPostsNumber");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedAllThanks"/> class.
    /// </summary>
    /// <param name="messageid">
    /// The messageid.
    /// </param>
    /// <param name="fromuserid">
    /// The fromuserid.
    /// </param>
    /// <param name="thanksdate">
    /// The thanksdate.
    /// </param>
    /// <param name="thanksfromusernumber">
    /// The thanksfromusernumber.
    /// </param>
    /// <param name="thankstousernumber">
    /// The thankstousernumber.
    /// </param>
    /// <param name="thankstouserpostsnumber">
    /// The thankstouserpostsnumber.
    /// </param>
    public TypedAllThanks(
      int? messageid, 
      int? fromuserid, 
      DateTime? thanksdate, 
      int? thanksfromusernumber, 
      int? thankstousernumber, 
      int? thankstouserpostsnumber)
    {
      this.MessageID = messageid;
      this.FromUserID = fromuserid;
      this.ThanksDate = thanksdate;
      this.ThanksFromUserNumber = thanksfromusernumber;
      this.ThanksToUserNumber = thankstousernumber;
      this.ThanksToUserPostsNumber = thankstouserpostsnumber;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets FromUserID.
    /// </summary>
    public int? FromUserID { get; set; }

    /// <summary>
    /// Gets or sets MessageID.
    /// </summary>
    public int? MessageID { get; set; }

    /// <summary>
    /// Gets or sets ThanksDate.
    /// </summary>
    public DateTime? ThanksDate { get; set; }

    /// <summary>
    /// Gets or sets ThanksFromUserNumber.
    /// </summary>
    public int? ThanksFromUserNumber { get; set; }

    /// <summary>
    /// Gets or sets ThanksToUserNumber.
    /// </summary>
    public int? ThanksToUserNumber { get; set; }

    /// <summary>
    /// Gets or sets ThanksToUserPostsNumber.
    /// </summary>
    public int? ThanksToUserPostsNumber { get; set; }

    #endregion
  }
}