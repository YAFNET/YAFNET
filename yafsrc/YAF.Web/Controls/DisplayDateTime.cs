﻿/* Yet Another Forum.NET
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
namespace YAF.Web.Controls;

/// <summary>
/// The display date time.
/// </summary>
public class DisplayDateTime : BaseControl
{
    /// <summary>
    ///   The ControlHtml.
    /// </summary>
    private const string ControlHtml = @"<abbr class=""timeago"" title=""{1}"" data-bs-toggle=""tooltip"" data-bs-html=""true"">{0}</abbr>";

    /// <summary>
    ///   Gets or sets DateTime.
    /// </summary>
    public object DateTime
    {
        get => this.ViewState["DateTime"];

        set => this.ViewState["DateTime"] = value;
    }

    /// <summary>
    ///   Gets or sets Format.
    /// </summary>
    public DateTimeFormat Format
    {
        get => this.ViewState["Format"]?.ToString().ToEnum<DateTimeFormat>() ?? DateTimeFormat.Both;

        set => this.ViewState["Format"] = value;
    }

    /// <summary>
    ///   Gets AsDateTime.
    /// </summary>
    protected DateTime AsDateTime
    {
        get
        {
            if (this.DateTime == null)
            {
                return DateTimeHelper.SqlDbMinTime();
            }

            try
            {
                return Convert.ToDateTime(this.DateTime, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                // not useable...
            }

            return DateTimeHelper.SqlDbMinTime();
        }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        if (!this.Visible || this.DateTime == null)
        {
            return;
        }

        var formattedDatetime = this.Get<IDateTimeService>().Format(this.Format, this.DateTime);

        writer.Write(
            ControlHtml,
            this.PageBoardContext.BoardSettings.ShowRelativeTime
                ? this.AsDateTime.ToRelativeTime()
                : formattedDatetime,
            formattedDatetime);

        writer.WriteLine();
    }
}