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

namespace YAF.Core.Services;

using System.Web;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
///  ThankYou Class to handle Thanks
/// </summary>
public class ThankYou : IThankYou, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThankYou"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public ThankYou(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Creates an instance of the thank you object from the current information.
    /// </summary>
    /// <param name="username">
    /// The Current Username
    /// </param>
    /// <param name="textTag">
    /// Button Text
    /// </param>
    /// <param name="titleTag">
    /// Button  Title
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    public ThankYouInfo CreateThankYou(
        string username,
        string textTag,
        string titleTag,
        int messageId)
    {
        return new()
                   {
                       MessageID = messageId,
                       ThanksInfo = this.Get<IThankYou>().ThanksInfo(username, messageId, false),
                       Text = this.Get<ILocalization>().GetText("BUTTON", textTag),
                       Title = this.Get<ILocalization>().GetText("BUTTON", titleTag)
                   };
    }

    /// <summary>
    /// Creates an instance of the thank you object from the current information.
    /// </summary>
    /// <param name="username">
    /// The Current Username
    /// </param>
    /// <param name="textTag">
    /// Button Text
    /// </param>
    /// <param name="titleTag">
    /// Button  Title
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    public ThankYouInfo GetThankYou(
        string username,
        string textTag,
        string titleTag,
        int messageId)
    {
        return new()
                   {
                       MessageID = messageId,
                       ThanksInfo = this.Get<IThankYou>().ThanksInfo(username, messageId, true),
                       Text = this.Get<ILocalization>().GetText("BUTTON", textTag),
                       Title = this.Get<ILocalization>().GetText("BUTTON", titleTag)
                   };
    }

    /// <summary>
    /// This method returns a string which shows how many times users have
    ///   thanked the message with the provided messageID. Returns an empty string.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="messageId">
    /// The Message ID.
    /// </param>
    /// <param name="thanksInfoOnly">
    /// The thank Info Only.
    /// </param>
    /// <returns>
    /// The thanks number.
    /// </returns>
    public string ThanksInfo(string username, int messageId, bool thanksInfoOnly)
    {
        var thanksNumber = this.GetRepository<Thanks>().Count(t => t.MessageID == messageId);

        if (thanksNumber == 0)
        {
            return "&nbsp;";
        }

        var thanksText = this.Get<ILocalization>()
            .GetTextFormatted("THANKSINFO", thanksNumber, username);

        var thanks = this.GetThanks(messageId);

        return thanksInfoOnly
                   ? thanks.Replace("\"", "'").Replace("<ol>", string.Empty).Replace("</ol>", string.Empty)
                   : $"""
                      <a class="btn btn-link thanks-popover"
                                                 data-bs-toggle="popover"
                                                 data-bs-trigger="click hover"
                                                 data-bs-html="true"
                                                 title="{thanksText}"
                                                 data-bs-content="{thanks.Replace("\"", "'")}">
                                                     <i class="fa fa-heart" style= "color:#e74c3c"></i>&nbsp;+{thanksNumber}</a>
                      """;
    }

    /// <summary>
    /// This method returns a string containing the HTML code for
    ///   showing the the post footer. the HTML content is the name of users
    ///   who thanked the post and the date they thanked.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// The get thanks.
    /// </returns>
    private string GetThanks(int messageId)
    {
        var filler = new StringBuilder();

        var thanks = this.GetRepository<Thanks>().MessageGetThanksList(messageId);

        filler.Append("<ol>");

        thanks.ForEach(
            dr =>
                {
                    var name = HttpUtility.HtmlEncode(dr.Item2.DisplayOrUserName());

                    filler.AppendFormat(
                        """<li class="list-inline-item"><a id="{0}" href="{1}"><u>{2}</u></a>""",
                        dr.Item2.ID,
                        this.Get<LinkBuilder>().GetUserProfileLink(dr.Item2.ID, name),
                        name);

                    if (this.Get<BoardSettings>().ShowThanksDate)
                    {
                        filler.AppendFormat(
                            " {0}",
                            this.Get<ILocalization>().GetTextFormatted(
                                "ONDATE",
                                this.Get<IDateTimeService>().FormatDateShort(dr.Item1.ThanksDate)));
                    }

                    filler.Append("</li>");
                });

        filler.Append("</ol>");

        return filler.ToString();
    }
}