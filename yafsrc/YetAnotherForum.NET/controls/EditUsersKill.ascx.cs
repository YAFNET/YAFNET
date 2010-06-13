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
namespace YAF.Controls
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web.Security;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The edit users kill.
  /// </summary>
  public partial class EditUsersKill : BaseUserControl
  {
    /// <summary>
    /// The _all posts by user.
    /// </summary>
    private DataTable _allPostsByUser = null;

    /// <summary>
    /// Gets CurrentUserID.
    /// </summary>
    protected long? CurrentUserID
    {
      get
      {
        return PageContext.QueryIDs["u"];
      }
    }

    /// <summary>
    /// Gets AllPostsByUser.
    /// </summary>
    public DataTable AllPostsByUser
    {
      get
      {
        if (this._allPostsByUser == null)
        {
          this._allPostsByUser = DB.post_alluser(PageContext.PageBoardID, CurrentUserID, PageContext.PageUserID, null);
        }

        return this._allPostsByUser;
      }
    }

    /// <summary>
    /// Gets IPAddresses.
    /// </summary>
    public List<string> IPAddresses
    {
      get
      {
        return AllPostsByUser.GetColumnAsList<string>("IP").OrderBy(x => x).Distinct().ToList();
      }
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
    protected void Page_Load(object sender, EventArgs e)
    {
      // init ids...
      PageContext.QueryIDs = new QueryStringIDHelper("u", true);

      // this needs to be done just once, not during postbacks
      if (!IsPostBack)
      {
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(CurrentUserID);
        var userData = new CombinedUserDataHelper(user, (int) CurrentUserID.Value);

          this.ViewPostsLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.search, "postedby={0}", !userData.IsGuest ? userData.Membership.UserName : (!userData.DisplayName.IsNotSet() ? userData.DisplayName : userData.UserName));

        // bind data
        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      // load ip address history for user...
      this.IpAddresses.Text = IPAddresses.ToDelimitedString("<br />");

      // show post count...
      this.PostCount.Text = AllPostsByUser.Rows.Count.ToString();

      DataBind();
    }

    /// <summary>
    /// The ban user ips.
    /// </summary>
    private void BanUserIps()
    {
      var ips = IPAddresses;
      var allIps = DB.bannedip_list(PageContext.PageBoardID, null).GetColumnAsList<string>("Mask").ToList();

      // remove all IPs from ips if they already exist in allIps...
      ips.RemoveAll(allIps.Contains);

      // ban user ips...
      string name = UserMembershipHelper.GetDisplayNameFromID(this.CurrentUserID == null? -1: (int)this.CurrentUserID);

      if (string.IsNullOrEmpty(name))
      {
          name = UserMembershipHelper.GetUserNameFromID(this.CurrentUserID == null ? -1 : (int)this.CurrentUserID);
      } 
        
      IPAddresses.ForEach(x => DB.bannedip_save(null, PageContext.PageBoardID, x,
          string.Format(@"User <a id=""usr{0}"" href=""{1}"">{2}</a>  is banned by IP", this.CurrentUserID, YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.CurrentUserID), Server.HtmlEncode(name)), this.PageContext.PageUserID));

      // clear cache of banned IPs for this board
      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BannedIP));
    }

    /// <summary>
    /// The delete posts.
    /// </summary>
    private void DeletePosts()
    {
      // delete posts...
      var messageIds = (from m in AllPostsByUser.AsEnumerable()
                        select m.Field<int>("MessageID")).Distinct().ToList();

      messageIds.ForEach(x => DB.message_delete(x, true, string.Empty, 1, true));
    }

    /// <summary>
    /// The kill_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Kill_OnClick(object sender, EventArgs e)
    {
      if (this.BanIps.Checked)
      {
        BanUserIps();
      }

      DeletePosts();

      MembershipUser user = UserMembershipHelper.GetMembershipUserById(CurrentUserID);
      PageContext.AddLoadMessage(String.Format("User {0} Killed!", user.UserName));

      // update the displayed data...
      BindData();
    }
  }
}