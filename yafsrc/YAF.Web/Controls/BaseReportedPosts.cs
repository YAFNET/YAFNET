/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
/// Shows a Reporters for reported posts
/// </summary>
public class BaseReportedPosts : BaseUserControl
{
    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public virtual int MessageID { get; set; }

    /// <summary>
    ///   Gets or sets Resolved.
    /// </summary>
    [CanBeNull]
    public virtual string Resolved { get; set; }

    /// <summary>
    ///   Gets or sets ResolvedBy. It returns UserID as string value
    /// </summary>
    [CanBeNull]
    public virtual int? ResolvedBy { get; set; }

    /// <summary>
    ///   Gets or sets ResolvedDate.
    /// </summary>
    [CanBeNull]
    public virtual string ResolvedDate { get; set; }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        var reportersList = this.GetRepository<User>().MessageReporters(this.MessageID);

        if (!reportersList.Any())
        {
            return;
        }

        writer.BeginRender();

        reportersList.ForEach(
            reporter =>
                {
                    writer.WriteLine(@"<div class=""alert alert-secondary"" role=""alert"">");

                    string howMany = null;
                    if (reporter.Item1.ReportedNumber > 1)
                    {
                        howMany = $"({this.GetTextFormatted("REPORTED_TIMES", reporter.Item1.ReportedNumber)})";
                    }

                    var reporterName = reporter.Item2.DisplayOrUserName();

                    // If the message was previously resolved we have not null string
                    // and can add an info about last user who resolved the message
                    if (this.ResolvedDate.IsSet() && DateTime.Parse(this.ResolvedDate) > DateTime.MinValue)
                    {
                        var resolvedBy = this.GetRepository<User>().GetById(
                            this.ResolvedBy.Value);

                        var resolvedByName = resolvedBy.DisplayOrUserName();

                        writer.Write(
                            @"<span class=""fw-bold me-2"">{0}</span><a href=""{1}"">{2}</a> : {3}",
                            this.GetText("RESOLVEDBY"),
                            this.Get<LinkBuilder>().GetUserProfileLink(this.ResolvedBy.ToType<int>(), resolvedByName),
                            resolvedByName,
                            this.Get<IDateTimeService>().FormatDateTimeTopic(this.ResolvedDate));
                    }

                    writer.Write(
                        @"<span class=""fw-bold mx-2"">{3}</span><a href=""{1}"" class=""me-2"">{0}</a><em>{2}</em>",
                        reporterName,
                        this.Get<LinkBuilder>().GetUserProfileLink(reporter.Item2.ID, reporterName),
                        howMany,
                        this.GetText("REPORTEDBY"));

                    writer.Write(
                        @"<a class=""btn btn-secondary btn-sm ms-2"" href=""{1}""><i class=""fa fa-envelope fa-fw""></i>&nbsp;{2} {0}</a>",
                        reporterName,
                        this.Get<LinkBuilder>().GetLink(
                            ForumPages.PostPrivateMessage,
                            new { u = reporter.Item2.ID, r = this.MessageID }),
                        this.GetText("REPLYTO"));

                    var reportString = reporter.Item1.ReportText.Trim().Split('|');

                    reportString.ForEach(
                        t =>
                            {
                                var textString = t.Split("??".ToCharArray());

                                writer.Write(
                                    @"<p class=""card-text"">@<span class=""ps-1"">{0}</span></p>",
                                    this.Get<IDateTimeService>().FormatDateTimeTopic(textString[0]));

                                writer.Write(@"<p class=""card-text"">");

                                writer.Write(textString.Length > 2 ? textString[2] : t);

                                writer.WriteLine(@"</p>");
                            });

                    writer.Write("</div>");
                });

        // render controls...
        base.Render(writer);

        writer.EndRender();
    }
}