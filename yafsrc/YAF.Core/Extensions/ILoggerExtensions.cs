/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2025 Ingo Herbote
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
namespace YAF.Core.Extensions;

using System;

using YAF.Types.Constants;

/// <summary>
///     The Logger extensions.
/// </summary>
public static class ILoggerExtensions
{
    /// <summary>
    /// The log.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="eventType">
    /// The event type.
    /// </param>
    public static void Log(
        this ILoggerService logger, 
        int? userId, 
        object source, 
        Exception exception, 
        EventLogTypes eventType = EventLogTypes.Error)
    {
        var sourceDescription = "unknown";

        if (source is Type)
        {
            sourceDescription = source.GetType().FullName;
        }
        else if (source != null)
        {
            sourceDescription = source.ToString().Truncate(50);
        }

        string message;

        try
        {
            message = $"Exception at URL: {BoardContext.Current.Get<HttpRequestBase>().Url}";
        }
        catch (Exception)
        {
            message = "Exception";
        }
            
        logger.Log(
            message,
            eventType,
            userId,
            sourceDescription,
            exception);
    }
}