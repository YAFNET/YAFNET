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
  using System.Linq;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for settings.
  /// </summary>
  public partial class hostsettings : AdminPage
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
      if (!PageContext.IsHostAdmin)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Host Settings", string.Empty);

        BindData();
      }

      var txtBoxes =
        ControlHelper.ControlListRecursive(this, (c) => (c.GetType() == typeof(TextBox) && ((TextBox) c).TextMode == TextBoxMode.SingleLine)).Cast<TextBox>().
          ToList();

      // default to 100% width...
      txtBoxes.ForEach(x => x.Width = Unit.Percentage(100));

      // vzrus : 13/5/2010
      ControlHelper.AddStyleAttributeWidth(ServerTimeCorrection, "25px");
      ControlHelper.AddAttributeMaxWidth(ServerTimeCorrection, "3");

      ControlHelper.AddStyleAttributeWidth(ImageAttachmentResizeHeight, "50px");

      ControlHelper.AddStyleAttributeWidth(MaxPostSize, "50px");
      ControlHelper.AddAttributeMaxWidth(UserNameMaxLength, "5"); 

      ControlHelper.AddStyleAttributeWidth(UserNameMaxLength, "25px");
      ControlHelper.AddAttributeMaxWidth(UserNameMaxLength, "3");

      ControlHelper.AddStyleAttributeWidth(ActiveListTime, "40px");

      ControlHelper.AddStyleAttributeWidth(PictureAttachmentDisplayTreshold, "100px");
      ControlHelper.AddAttributeMaxWidth(PictureAttachmentDisplayTreshold, "11"); 

      // Ederon : 7/1/2007
      ControlHelper.AddStyleAttributeWidth(SmiliesPerRow, "25px");
      ControlHelper.AddAttributeMaxWidth(SmiliesPerRow, "2");
      ControlHelper.AddStyleAttributeWidth(SmiliesColumns, "25px");
      ControlHelper.AddAttributeMaxWidth(SmiliesColumns, "2");

      
      ControlHelper.AddStyleAttributeWidth(ImageAttachmentResizeWidth, "50px");
      ControlHelper.AddStyleAttributeWidth(DisableNoFollowLinksAfterDay, "100px");

      // Ederon : 7/14/2007
      ControlHelper.AddStyleAttributeSize(UserBox, "350px", "100px");
      ControlHelper.AddStyleAttributeSize(AdPost, "400px", "150px");

      // CheckCache
      CheckCache();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {     
      ForumEditor.DataSource = PageContext.EditorModuleManager.GetEditorsTable();

      // TODO: vzrus: UseFullTextSearch check box is data layer specific and can be hidden by YAF.Classes.Data.DB.FullTextSupported  property.
      DataBind();

      // load Board Setting collection information...
      var settingCollection = new YafBoardSettingCollection(PageContext.BoardSettings);

      // handle checked fields...
      foreach (string name in settingCollection.SettingsBool.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is CheckBox && settingCollection.SettingsBool[name].CanRead)
        {
          // get the value from the property...
          ((CheckBox) control).Checked = (bool) Convert.ChangeType(settingCollection.SettingsBool[name].GetValue(PageContext.BoardSettings, null), typeof(bool));
        }
      }

      // handle string fields...
      foreach (string name in settingCollection.SettingsString.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is TextBox && settingCollection.SettingsString[name].CanRead)
        {
          // get the value from the property...
          ((TextBox) control).Text =
            (string) Convert.ChangeType(settingCollection.SettingsString[name].GetValue(PageContext.BoardSettings, null), typeof(string));
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsString[name].CanRead)
        {
          ListItem listItem =
            ((DropDownList) control).Items.FindByValue(settingCollection.SettingsString[name].GetValue(PageContext.BoardSettings, null).ToString());

          if (listItem != null)
          {
            listItem.Selected = true;
          }
        }
      }

      // handle int fields...
      foreach (string name in settingCollection.SettingsInt.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is TextBox && settingCollection.SettingsInt[name].CanRead)
        {
          // get the value from the property...
          ((TextBox) control).Text = settingCollection.SettingsInt[name].GetValue(PageContext.BoardSettings, null).ToString();
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsInt[name].CanRead)
        {
          ListItem listItem =
            ((DropDownList) control).Items.FindByValue(settingCollection.SettingsInt[name].GetValue(PageContext.BoardSettings, null).ToString());

          if (listItem != null)
          {
            listItem.Selected = true;
          }
        }
      }

      // handle double fields...
      foreach (string name in settingCollection.SettingsDouble.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is TextBox && settingCollection.SettingsDouble[name].CanRead)
        {
          // get the value from the property...
          ((TextBox) control).Text = settingCollection.SettingsDouble[name].GetValue(PageContext.BoardSettings, null).ToString();
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsDouble[name].CanRead)
        {
          ListItem listItem =
            ((DropDownList) control).Items.FindByValue(settingCollection.SettingsDouble[name].GetValue(PageContext.BoardSettings, null).ToString());

          if (listItem != null)
          {
            listItem.Selected = true;
          }
        }
      }

      // special field handling...
      AvatarSize.Text = (PageContext.BoardSettings.AvatarSize != 0) ? PageContext.BoardSettings.AvatarSize.ToString() : string.Empty;
      MaxFileSize.Text = (PageContext.BoardSettings.MaxFileSize != 0) ? PageContext.BoardSettings.MaxFileSize.ToString() : string.Empty;

      SQLVersion.Text = HtmlEncode(PageContext.BoardSettings.SQLVersion);
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
      // write all the settings back to the settings class

      // load Board Setting collection information...
      var settingCollection = new YafBoardSettingCollection(PageContext.BoardSettings);

      // handle checked fields...
      foreach (string name in settingCollection.SettingsBool.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is CheckBox && settingCollection.SettingsBool[name].CanWrite)
        {
          settingCollection.SettingsBool[name].SetValue(PageContext.BoardSettings, ((CheckBox) control).Checked, null);
        }
      }

      // handle string fields...
      foreach (string name in settingCollection.SettingsString.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is TextBox && settingCollection.SettingsString[name].CanWrite)
        {
          settingCollection.SettingsString[name].SetValue(PageContext.BoardSettings, ((TextBox) control).Text.Trim(), null);
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsString[name].CanWrite)
        {
          settingCollection.SettingsString[name].SetValue(PageContext.BoardSettings, Convert.ToString(((DropDownList) control).SelectedItem.Value), null);
        }
      }

      // handle int fields...
      foreach (string name in settingCollection.SettingsInt.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is TextBox && settingCollection.SettingsInt[name].CanWrite)
        {
          string value = ((TextBox) control).Text.Trim();
          int i = 0;

          if (String.IsNullOrEmpty(value))
          {
            i = 0;
          }
          else
          {
            int.TryParse(value, out i);
          }

          settingCollection.SettingsInt[name].SetValue(PageContext.BoardSettings, i, null);
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsInt[name].CanWrite)
        {
          settingCollection.SettingsInt[name].SetValue(PageContext.BoardSettings, Convert.ToInt32(((DropDownList) control).SelectedItem.Value), null);
        }
      }

      // handle double fields...
      foreach (string name in settingCollection.SettingsDouble.Keys)
      {
        Control control = ControlHelper.FindControlRecursive(this.HostSettingsTabs, name);

        if (control != null && control is TextBox && settingCollection.SettingsDouble[name].CanWrite)
        {
          string value = ((TextBox) control).Text.Trim();
          double i = 0;

          if (String.IsNullOrEmpty(value))
          {
            i = 0;
          }
          else
          {
            double.TryParse(value, out i);
          }

          settingCollection.SettingsDouble[name].SetValue(PageContext.BoardSettings, i, null);
        }
        else if (control != null && control is DropDownList && settingCollection.SettingsDouble[name].CanWrite)
        {
          settingCollection.SettingsDouble[name].SetValue(PageContext.BoardSettings, Convert.ToDouble(((DropDownList) control).SelectedItem.Value), null);
        }
      }

      // save the settings to the database
      ((YafLoadBoardSettings) PageContext.BoardSettings).SaveRegistry();

      // reload all settings from the DB
      this.PageContext.BoardSettings = null;

      YafBuildLink.Redirect(ForumPages.admin_admin);
    }

    /// <summary>
    /// The forum statistics cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ForumStatisticsCacheReset_Click(object sender, EventArgs e)
    {
        this.RemoveCacheKey(Constants.Cache.BoardStats);
    }

    /// <summary>
    /// The active discussions cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ActiveDiscussionsCacheReset_Click(object sender, EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.ActiveDiscussions);
      this.RemoveCacheKey(Constants.Cache.ForumActiveDiscussions);
    }

    /// <summary>
    /// The board moderators cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BoardModeratorsCacheReset_Click(object sender, EventArgs e)
    {
        this.RemoveCacheKey(Constants.Cache.ForumModerators);
    }

    /// <summary>
    /// The board categories cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void BoardCategoriesCacheReset_Click(object sender, EventArgs e)
    {
      this.RemoveCacheKey(Constants.Cache.ForumCategory);
    }

    /// <summary>
    /// The user lazy data cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserLazyDataCacheReset_Click(object sender, EventArgs e)
    {
       this.RemoveCacheKey(Constants.Cache.ActiveUserLazyData);
    }

    /// <summary>
    /// The replace rules cache reset_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ReplaceRulesCacheReset_Click(object sender, EventArgs e)
    {
      ReplaceRulesCreator.ClearCache();
      this.CheckCache();
    }

    /// <summary>
    /// The reset cache all_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ResetCacheAll_Click(object sender, EventArgs e)
    {
      // clear all cache keys
      this.PageContext.Cache.Clear();

      this.CheckCache();
    }

    /// <summary>
    /// The remove cache key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    private void RemoveCacheKey(string key)
    {
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(key));
      this.CheckCache();
    }

    /// <summary>
    /// The check cache key.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <returns>
    /// The check cache key.
    /// </returns>
    private bool CheckCacheKey(string key)
    {
        return this.PageContext.Cache[YafCache.GetBoardCacheKey(key)] != null;
    }

    /// <summary>
    /// The check cache.
    /// </summary>
    private void CheckCache()
    {
        this.ForumStatisticsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.BoardStats);
        this.ActiveDiscussionsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ActiveDiscussions) || CheckCacheKey(Constants.Cache.ForumActiveDiscussions);
        this.BoardModeratorsCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ForumModerators);
        this.BoardCategoriesCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ForumCategory);
        this.ActiveUserLazyDataCacheReset.Enabled = this.CheckCacheKey(Constants.Cache.ActiveUserLazyData);
        this.ResetCacheAll.Enabled = this.PageContext.Cache.Count > 0;
    }
  }
}