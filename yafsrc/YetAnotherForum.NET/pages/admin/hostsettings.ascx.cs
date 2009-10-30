/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Editors;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for settings.
	/// </summary>
	public partial class hostsettings : YAF.Classes.Core.AdminPage
	{
		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.IsHostAdmin )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Host Settings", "" );

				BindData();
			}

			var txtBoxes =
				ControlHelper.ControlListRecursive( this,
				                                    ( c ) =>
				                                    (c.GetType() == typeof ( TextBox ) &&
				                                     ((TextBox) c).TextMode == TextBoxMode.SingleLine) ).Cast<TextBox>().ToList();
			// default to 100% width...
			txtBoxes.ForEach( x => x.Width = Unit.Percentage( 100 ) );

			// Ederon : 7/1/2007
			ControlHelper.AddStyleAttributeWidth( SmiliesPerRow, "25px" );
			ControlHelper.AddStyleAttributeWidth( SmiliesColumns, "25px" );
			ControlHelper.AddStyleAttributeWidth( ImageAttachmentResizeWidth, "50px" );
			ControlHelper.AddStyleAttributeWidth( DisableNoFollowLinksAfterDay, "100px" );

			// Ederon : 7/14/2007
			ControlHelper.AddStyleAttributeSize( UserBox, "350px", "100px" );
			ControlHelper.AddStyleAttributeSize( AdPost, "400px", "150px" );

			// CheckCache
			CheckCache();
		}

		private void BindData()
		{
			TimeZoneRaw.DataSource = StaticDataHelper.TimeZones();
			ForumEditor.DataSource = PageContext.EditorModuleManager.GetEditorsTable();
			// TODO: vzrus: UseFullTextSearch check box is data layer specific and can be hidden by YAF.Classes.Data.DB.FullTextSupported  property.
			DataBind();

			// load Board Setting collection information...
			YafBoardSettingCollection settingCollection = new YafBoardSettingCollection( PageContext.BoardSettings );

			// handle checked fields...
			foreach ( string name in settingCollection.SettingsBool.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is CheckBox && settingCollection.SettingsBool[name].CanRead )
				{
					// get the value from the property...
					( (CheckBox)control ).Checked =
						(bool)
						Convert.ChangeType( settingCollection.SettingsBool[name].GetValue( PageContext.BoardSettings, null ),
																typeof( bool ) );
				}
			}

			// handle string fields...
			foreach ( string name in settingCollection.SettingsString.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is TextBox && settingCollection.SettingsString[name].CanRead )
				{
					// get the value from the property...
					( (TextBox)control ).Text =
						(string)
						Convert.ChangeType( settingCollection.SettingsString[name].GetValue( PageContext.BoardSettings, null ),
																typeof( string ) );
				}
				else if ( control != null && control is DropDownList && settingCollection.SettingsString[name].CanRead )
				{
					ListItem listItem = ( (DropDownList)control ).Items.FindByValue(
						settingCollection.SettingsString[name].GetValue( PageContext.BoardSettings, null ).ToString() );

					if ( listItem != null ) listItem.Selected = true;
				}
			}

			// handle int fields...
			foreach ( string name in settingCollection.SettingsInt.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is TextBox && settingCollection.SettingsInt[name].CanRead )
				{
					// get the value from the property...
					( (TextBox)control ).Text =
						settingCollection.SettingsInt[name].GetValue( PageContext.BoardSettings, null ).ToString();
				}
				else if ( control != null && control is DropDownList && settingCollection.SettingsInt[name].CanRead )
				{
					ListItem listItem = ( (DropDownList)control ).Items.FindByValue(
						settingCollection.SettingsInt[name].GetValue( PageContext.BoardSettings, null ).ToString() );

					if ( listItem != null ) listItem.Selected = true;
				}
			}

			// handle double fields...
			foreach ( string name in settingCollection.SettingsDouble.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is TextBox && settingCollection.SettingsDouble[name].CanRead )
				{
					// get the value from the property...
					( (TextBox)control ).Text =
						settingCollection.SettingsDouble[name].GetValue( PageContext.BoardSettings, null ).ToString();
				}
				else if ( control != null && control is DropDownList && settingCollection.SettingsDouble[name].CanRead )
				{
					ListItem listItem = ( (DropDownList)control ).Items.FindByValue(
						settingCollection.SettingsDouble[name].GetValue( PageContext.BoardSettings, null ).ToString() );

					if ( listItem != null ) listItem.Selected = true;
				}
			}

			// special field handling...
			AvatarSize.Text = ( PageContext.BoardSettings.AvatarSize != 0 ) ? PageContext.BoardSettings.AvatarSize.ToString() : "";
			MaxFileSize.Text = ( PageContext.BoardSettings.MaxFileSize != 0 ) ? PageContext.BoardSettings.MaxFileSize.ToString() : "";

			SQLVersion.Text = HtmlEncode( PageContext.BoardSettings.SQLVersion );
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			// write all the settings back to the settings class

			// load Board Setting collection information...
			YafBoardSettingCollection settingCollection = new YafBoardSettingCollection( PageContext.BoardSettings );

			// handle checked fields...
			foreach ( string name in settingCollection.SettingsBool.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is CheckBox && settingCollection.SettingsBool[name].CanWrite )
				{
					settingCollection.SettingsBool[name].SetValue( PageContext.BoardSettings, ( (CheckBox)control ).Checked, null );
				}
			}

			// handle string fields...
			foreach ( string name in settingCollection.SettingsString.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is TextBox && settingCollection.SettingsString[name].CanWrite )
				{
					settingCollection.SettingsString[name].SetValue( PageContext.BoardSettings, ( (TextBox)control ).Text.Trim(), null );
				}
				else if ( control != null && control is DropDownList && settingCollection.SettingsString[name].CanWrite )
				{
					settingCollection.SettingsString[name].SetValue( PageContext.BoardSettings,
																													 Convert.ToString( ( (DropDownList)control ).SelectedItem.Value ), null );
				}
			}

			// handle int fields...
			foreach ( string name in settingCollection.SettingsInt.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is TextBox && settingCollection.SettingsInt[name].CanWrite )
				{
					string value = ( (TextBox)control ).Text.Trim();
					int i = 0;

					if ( String.IsNullOrEmpty( value ) ) i = 0;
					else int.TryParse( value, out i );

					settingCollection.SettingsInt[name].SetValue( PageContext.BoardSettings, i, null );
				}
				else if ( control != null && control is DropDownList && settingCollection.SettingsInt[name].CanWrite )
				{
					settingCollection.SettingsInt[name].SetValue( PageContext.BoardSettings,
																													 Convert.ToInt32( ( (DropDownList)control ).SelectedItem.Value ), null );
				}
			}

			// handle double fields...
			foreach ( string name in settingCollection.SettingsDouble.Keys )
			{
				Control control = ControlHelper.FindControlRecursive( HostSettingsTabs, name );

				if ( control != null && control is TextBox && settingCollection.SettingsDouble[name].CanWrite )
				{
					string value = ( (TextBox)control ).Text.Trim();
					double i = 0;

					if ( String.IsNullOrEmpty( value ) ) i = 0;
					else double.TryParse( value, out i );

					settingCollection.SettingsDouble[name].SetValue( PageContext.BoardSettings, i, null );
				}
				else if ( control != null && control is DropDownList && settingCollection.SettingsDouble[name].CanWrite )
				{
					settingCollection.SettingsDouble[name].SetValue( PageContext.BoardSettings,
																													 Convert.ToDouble( ( (DropDownList)control ).SelectedItem.Value ), null );
				}
			}

			// save the settings to the database
			( (YafLoadBoardSettings)PageContext.BoardSettings ).SaveRegistry();

			// reload all settings from the DB
			PageContext.BoardSettings = null;

			YafBuildLink.Redirect( ForumPages.admin_admin );
		}

		protected void ForumStatisticsCacheReset_Click( object sender, System.EventArgs e )
		{
			RemoveCacheKey( Constants.Cache.BoardStats );
		}

		protected void ActiveDiscussionsCacheReset_Click( object sender, System.EventArgs e )
		{
			RemoveCacheKey( Constants.Cache.ActiveDiscussions );
			RemoveCacheKey( Constants.Cache.ForumActiveDiscussions );
		}

		protected void BoardModeratorsCacheReset_Click( object sender, System.EventArgs e )
		{
			RemoveCacheKey( Constants.Cache.ForumModerators );
		}

		protected void BoardCategoriesCacheReset_Click( object sender, System.EventArgs e )
		{
			RemoveCacheKey( Constants.Cache.ForumCategory );
		}

		protected void ReplaceRulesCacheReset_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
			CheckCache();
		}

		protected void ResetCacheAll_Click( object sender, System.EventArgs e )
		{
			// clear all cache keys
			PageContext.Cache.Clear();

			CheckCache();
		}

		private void RemoveCacheKey( string key )
		{
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( key ) );
			CheckCache();
		}

		private bool CheckCacheKey( string key )
		{
			return PageContext.Cache[YafCache.GetBoardCacheKey( key )] != null;
		}

		private void CheckCache()
		{
			ForumStatisticsCacheReset.Enabled = CheckCacheKey( Constants.Cache.BoardStats );
			ActiveDiscussionsCacheReset.Enabled = CheckCacheKey( Constants.Cache.ActiveDiscussions ) || CheckCacheKey( Constants.Cache.ForumActiveDiscussions );
			BoardModeratorsCacheReset.Enabled = CheckCacheKey( Constants.Cache.ForumModerators );
			BoardCategoriesCacheReset.Enabled = CheckCacheKey( Constants.Cache.ForumCategory );
			ResetCacheAll.Enabled = PageContext.Cache.Count > 0;
		}
	}
}
