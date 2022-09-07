/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Pages.Admin;

using System.Net.Mail;

/// <summary>
/// Manage Forum Digest Sending
/// </summary>
public partial class Digest : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Digest"/> class.
    /// </summary>
    public Digest()
        : base("ADMIN_DIGEST", ForumPages.Admin_Digest)
    {
    }

    /// <summary>
    /// Force Sending the Current Digest
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ForceSendClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PageBoardContext.BoardSettings.ForceDigestSend = true;
        this.Get<BoardSettingsService>().SaveRegistry(this.PageBoardContext.BoardSettings);

        this.PageBoardContext.Notify(this.GetText("ADMIN_DIGEST", "MSG_FORCE_SEND"), MessageTypes.success);
    }

    /// <summary>
    /// Generate a test Digest
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void GenerateDigestClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.DigestHtmlPlaceHolder.Visible = true;
        this.DigestFrame.Attributes["src"] = this.Get<IDigest>()
            .GetDigestUrl(this.PageBoardContext.PageUserID, this.PageBoardContext.BoardSettings, true);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.LastDigestSendLabel.Text = this.PageBoardContext.BoardSettings.LastDigestSend.IsNotSet()
                                            ? this.GetText("ADMIN_DIGEST", "DIGEST_NEVER")
                                            : Convert.ToDateTime(
                                                this.PageBoardContext.BoardSettings.LastDigestSend,
                                                CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

        this.DigestEnabled.Text = this.PageBoardContext.BoardSettings.AllowDigestEmail
                                      ? this.GetText("COMMON", "YES")
                                      : this.GetText("COMMON", "NO");
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // forum index
        this.PageBoardContext.PageLinks.AddRoot();

        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_DIGEST", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Send Test Digest
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void TestSendClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.TextSendEmail.Text.IsSet())
        {
            try
            {
                // create and send a test digest to the email provided...
                var digestHtml = this.Get<IDigest>().GetDigestHtml(
                    this.PageBoardContext.PageUser,
                    this.PageBoardContext.BoardSettings,
                    true);

                // send....
                var message = this.Get<IDigest>().CreateDigestMessage(
                    string.Format(this.GetText("DIGEST", "SUBJECT"), this.PageBoardContext.BoardSettings.Name),
                    digestHtml,
                    new MailAddress(this.PageBoardContext.BoardSettings.ForumEmail, this.PageBoardContext.BoardSettings.Name),
                    this.TextSendEmail.Text.Trim(),
                    "Digest Send Test");

                this.Get<IMailService>().SendAll(new List<MailMessage> { message });

                this.PageBoardContext.Notify(
                    this.GetTextFormatted("MSG_SEND_SUC", "Direct"),
                    MessageTypes.success);
            }
            catch (Exception ex)
            {
                this.PageBoardContext.Notify(
                    string.Format(this.GetText("ADMIN_DIGEST", "MSG_SEND_ERR"), ex),
                    MessageTypes.danger);
            }
        }
        else
        {
            this.PageBoardContext.Notify(this.GetText("ADMIN_DIGEST", "MSG_VALID_MAIL"), MessageTypes.danger);
        }
    }
}