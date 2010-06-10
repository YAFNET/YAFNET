/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for settings.
  /// </summary>
  public partial class boardsettings : AdminPage
  {
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
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Board Settings", string.Empty);

        // create list boxes by populating datasources from Data class
        this.Theme.DataSource = StaticDataHelper.Themes();
        this.Theme.DataTextField = "Theme";
        this.Theme.DataValueField = "FileName";

        this.Language.DataSource = StaticDataHelper.Cultures();
        this.Language.DataTextField = "CultureNativeName";
        this.Language.DataValueField = "CultureTag";       

        this.ShowTopic.DataSource = StaticDataHelper.TopicTimes();
        this.ShowTopic.DataTextField = "TopicText";
        this.ShowTopic.DataValueField = "TopicValue";

        this.FileExtensionAllow.DataSource = StaticDataHelper.AllowDisallow();
        this.FileExtensionAllow.DataTextField = "Text";
        this.FileExtensionAllow.DataValueField = "Value";

        BindData();

        // Get first default full culture from a language file tag.
        string langFileCulture = StaticDataHelper.CultureDefaultFromFile(PageContext.BoardSettings.Language);
        
        SetSelectedOnList(ref this.Theme, PageContext.BoardSettings.Theme);

        // If 2-letter language code is the same we return Culture, else we return  a default full culture from language file
        SetSelectedOnList(ref this.Language, (langFileCulture.Substring(0,2) == PageContext.BoardSettings.Culture ? PageContext.BoardSettings.Culture :langFileCulture));      
        SetSelectedOnList(ref this.ShowTopic, PageContext.BoardSettings.ShowTopicsDefault.ToString());
        SetSelectedOnList(ref this.FileExtensionAllow, PageContext.BoardSettings.FileExtensionAreAllowed ? "0" : "1");

        this.NotificationOnUserRegisterEmailList.Text = PageContext.BoardSettings.NotificationOnUserRegisterEmailList;
        this.AllowThemedLogo.Checked = PageContext.BoardSettings.AllowThemedLogo;
      }
    }

    /// <summary>
    /// The set selected on list.
    /// </summary>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    private void SetSelectedOnList(ref DropDownList list, string value)
    {
      ListItem selItem = list.Items.FindByValue(value);

      if (selItem != null)
      {
        selItem.Selected = true;
      }
      else if (list.Items.Count > 0)
      {
        // select the first...
        list.SelectedIndex = 0;
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      DataRow row;
      using (DataTable dt = DB.board_list(PageContext.PageBoardID))
      {
        row = dt.Rows[0];
      }

      DataBind();
      this.Name.Text = (string) row["Name"];
      this.AllowThreaded.Checked = SqlDataLayerConverter.VerifyBool(row["AllowThreaded"]);
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click(object sender, EventArgs e)
    {

        System.Data.DataTable cult = StaticDataHelper.Cultures();
        string langFile = "en-US";
        foreach (System.Data.DataRow drow in cult.Rows)
        {
            if (drow["CultureTag"].ToString() == this.Language.SelectedValue)
            {
                langFile = (string)drow["CultureFile"];
            }
        }

      DB.board_save(PageContext.PageBoardID, langFile,this.Language.SelectedValue, this.Name.Text, this.AllowThreaded.Checked);

      PageContext.BoardSettings.Language = langFile;
      PageContext.BoardSettings.Culture = this.Language.SelectedValue;
      PageContext.BoardSettings.Theme = this.Theme.SelectedValue;      
      PageContext.BoardSettings.ShowTopicsDefault = Convert.ToInt32(this.ShowTopic.SelectedValue);
      PageContext.BoardSettings.AllowThemedLogo = this.AllowThemedLogo.Checked;
      PageContext.BoardSettings.FileExtensionAreAllowed = Convert.ToInt32(this.FileExtensionAllow.SelectedValue) == 0 ? true : false;
      PageContext.BoardSettings.NotificationOnUserRegisterEmailList = this.NotificationOnUserRegisterEmailList.Text.Trim();

      // save the settings to the database
      ((YafLoadBoardSettings) PageContext.BoardSettings).SaveRegistry();

      // Reload forum settings
      PageContext.BoardSettings = null;

      // Clearing cache with old users permissions data to get new default styles...
      this.PageContext.Cache.Remove((x) => x.StartsWith(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData)));
      YafBuildLink.Redirect(ForumPages.admin_admin);
    }
  }
}