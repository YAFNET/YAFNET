/* Yet Another Forum.NET
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Xml.Serialization;
using DNA.UI.JQuery;
using YAF.Classes;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for AdminMenu.
	/// </summary>
	public class AdminMenu : BaseControl
	{
		private DNA.UI.JQuery.Accordion _accordian = new Accordion();
		private YafMenu _menuDef = null;

		public AdminMenu()
		{

		}

		protected override void OnLoad( EventArgs e )
		{
			PageContext.PageElements.RegisterJQuery();

			string defFile = "YAF.Controls.AdminMenu.AdminMenuDef.xml";

			// load menu definition...
      var deserializer = new XmlSerializer(typeof(YafMenu));
			using(Stream resourceStream = Assembly.GetAssembly( this.GetType() ).GetManifestResourceStream( defFile ))
			{
				if ( resourceStream != null ) _menuDef = (YafMenu) deserializer.Deserialize( resourceStream );
			}

			_accordian.ID = GetExtendedID( "DNA_Accordian" );
			_accordian.CssClass = "adminMenuAccordian";

			int viewIndex = 0;

			// build menu...
			foreach ( var value in _menuDef.Items )
			{
				bool addView = true;

				if ( Convert.ToBoolean( value.HostAdminOnly ) && !PageContext.IsHostAdmin )
				{
					addView = false;
				}

				if ( addView )
				{
					var view = new NavView { Text = value.Title };
					_accordian.Views.Add( view );

					// add items...
					BuildUrlList( view, value.YafMenuItem );

					//// select the view that has the current page...
					string currentPage = PageContext.ForumPageType.ToString();

					if ( value.YafMenuItem.Any( x => x.ForumPage == currentPage ) )
					{
						// select this view...
						_accordian.SelectedIndex = viewIndex;
					}

					viewIndex++;
				}
			}

			this.Controls.Add( _accordian );
		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.BeginRender();

			// render the contents of the admin menu....
			writer.WriteLine( String.Format( @"<div id=""{0}"">", this.ClientID ) );
			writer.WriteLine( @"<table class=""adminContainer""><tr>" );
			writer.WriteLine( @"<td class=""adminMenu"" valign=""top"">" );
			_accordian.RenderControl( writer );
			_accordian.Visible = false;
			writer.WriteLine( @"</td>" );

			// contents of the admin page...
			writer.WriteLine( @"<td class=""adminContent"">" );

			base.RenderChildren( writer );

			writer.WriteLine( @"</td></tr></table>" );
			writer.WriteLine( "</div>" );

			_accordian.Visible = true;

			writer.EndRender();
		}

		/// <summary>
		/// Builds a Url List
		/// </summary>
		/// <param name="view"></param>
		/// <param name="listItems"></param>
		/// <returns></returns>
		protected void BuildUrlList( NavView view, YafMenuYafMenuSectionYafMenuItem[] listItems )
		{
			if ( listItems.Length > 0 )
			{
				view.ItemCssClass = "YafMenuItem";
				view.ItemIconClass = "YafMenuItemIcon";

				// add each YafMenuItem to the NavView...
				foreach( var item in listItems )
				{
					bool isVisible = true;

					if ( !String.IsNullOrEmpty(item.Debug) && Convert.ToBoolean( item.Debug ) == true )
					{
						isVisible = false;
#if DEBUG
						// only visible with debug...
						isVisible = true;
#endif
					}

					if ( !isVisible ) continue;

					string url = string.Empty;

					if ( !String.IsNullOrEmpty(item.Link) )
					{
						// direct link...
						url = item.Link.Replace( "~", YafForumInfo.ForumRoot );
					}
					else if ( !String.IsNullOrEmpty( item.ForumPage ) )
					{
						// internal "page" link...
						url = YafBuildLink.GetLink( (ForumPages) Enum.Parse( typeof ( ForumPages ), item.ForumPage ) );
					}

					if ( !String.IsNullOrEmpty( item.Image ) )
					{
						// add icon...
						view.AddItem( String.Format( @"<img alt="""" src=""{1}"" /> {0}", item.Title, YafForumInfo.GetURLToResource( String.Format( "icons/{0}.png", item.Image ) ) ), url );
					}
					else
					{
						// just add the item regular style..
						view.AddItem( item.Title, url );	
					}
				}
			}
		}
	}
}
