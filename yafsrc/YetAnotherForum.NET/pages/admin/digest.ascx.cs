/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Pages.Admin
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The digest.
  /// </summary>
  public partial class digest : AdminPage
  {
    #region Methods

    /// <summary>
    /// The force send_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForceSend_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafContext.Current.BoardSettings.ForceDigestSend = true;
      ((YafLoadBoardSettings)YafContext.Current.BoardSettings).SaveRegistry();

      this.PageContext.AddLoadMessage("Digest Send Scheduled for Immediate Sending");
    }

    /// <summary>
    /// The generate digest_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void GenerateDigest_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.DigestHtmlPlaceHolder.Visible = true;
      this.DigestFrame.Attributes["src"] = this.Get<YafDigest>().GetDigestUrl(
        this.PageContext.PageUserID, this.PageContext.PageBoardID);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Digest", string.Empty);

        this.LastDigestSendLabel.Text = this.PageContext.BoardSettings.LastDigestSend.IsNotSet()
                                          ? "Never"
                                          : this.PageContext.BoardSettings.LastDigestSend;

        this.DigestEnabled.Text = this.PageContext.BoardSettings.AllowDigestEmail ? "True" : "False";
      }
    }

    /// <summary>
    /// The test send_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void TestSend_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.TextSendEmail.Text.IsNotSet())
      {
        this.PageContext.AddLoadMessage("Please enter a valid email address to send a test.");
      }

      try
      {
        // create and send a test digest to the email provided...
        var digestHtml = this.Get<YafDigest>().GetDigestHtml(this.PageContext.PageUserID, this.PageContext.PageBoardID);

        // send....
        this.Get<YafDigest>().SendDigest(
          digestHtml, 
          this.PageContext.BoardSettings.Name, 
          this.TextSendEmail.Text.Trim(), 
          "Digest Send Test", 
          this.SendMethod.SelectedItem.Text == "Queued");

        this.PageContext.AddLoadMessage("Sent via {0} successfully.".FormatWith(this.SendMethod.SelectedItem.Text));
      }
      catch (Exception ex)
      {
        this.PageContext.AddLoadMessage("Exception Getting Digest: " + ex);
      }
    }

    #endregion
  }
}