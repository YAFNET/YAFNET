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
namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The i format message extensions.
  /// </summary>
  public static class IFormatMessageExtensions
  {
    #region Public Methods

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="formatMessage">
    /// The format Message.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <returns>
    /// The formatted message.
    /// </returns>
    public static string FormatMessage([NotNull] this IFormatMessage formatMessage, [NotNull] string message, [NotNull] MessageFlags messageFlags)
    {
      return formatMessage.FormatMessage(message, messageFlags, false, DateTime.UtcNow);
    }

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="formatMessage">
    /// The format Message.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target blank override.
    /// </param>
    /// <returns>
    /// The formated message.
    /// </returns>
    public static string FormatMessage([NotNull] this IFormatMessage formatMessage, 
      [NotNull] string message, 
      [NotNull] MessageFlags messageFlags, 
      bool targetBlankOverride)
    {
      return formatMessage.FormatMessage(message, messageFlags, targetBlankOverride, DateTime.UtcNow);
    }

    #endregion
  }
}