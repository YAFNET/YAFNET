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
	using System.Drawing;
	using System.Web.UI.WebControls;
	using YAF.Classes;
	using YAF.Classes.Core;
	using YAF.Classes.Data;
	using YAF.Classes.Utils;

	/// <summary>
	/// Summary description for forums.
	/// </summary>
	public partial class accessmasks : AdminPage
	{
		/* Construction */
		#region Overridden Methods

		/// <summary>
		/// Creates navigation page links on top of forum (breadcrumbs).
		/// </summary>
		protected override void CreatePageLinks()
		{
			// board index
			this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			// administration index
			this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
			// current page label (no link)
			this.PageLinks.AddLink("Access Masks", string.Empty);
		}

		#endregion


		/* Event Handlers */

		#region Page Events

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
				// create links
				CreatePageLinks();

				// bind data
				BindData();
			}
		}
		
		#endregion

		#region Controls Events

		/// <summary>
		/// The list_ item command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void List_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "edit":
					// redirect to editing page
					YafBuildLink.Redirect(ForumPages.admin_editaccessmask, "i={0}", e.CommandArgument);
					break;
				case "delete":
					// attmempt to delete access masks
					if (DB.accessmask_delete(e.CommandArgument))
					{
						// remove cache of forum moderators
						PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
						BindData();
					}
					else
					{
						// used masks cannot be deleted
						PageContext.AddLoadMessage("You cannot delete this access mask because it is in use.");
					}

					// quit switch
					break;
			}
		}


		/// <summary>
		/// The delete_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Delete_Load(object sender, EventArgs e)
		{
			// add on click confirm dialog
			ControlHelper.AddOnClickConfirmDialog(sender, "Delete this access mask?");
		}


		/// <summary>
		/// The new_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void New_Click(object sender, EventArgs e)
		{
			// redirect to page for access mask creation
			YafBuildLink.Redirect(ForumPages.admin_editaccessmask);
		}

		#endregion


		/* Methods */
		#region Data Binding

		/// <summary>
		/// The bind data.
		/// </summary>
		private void BindData()
		{
			// list all access masks for this boeard
			this.List.DataSource = DB.accessmask_list(PageContext.PageBoardID, null);
			DataBind();
		}

		#endregion

		#region Formatting

		/// <summary>
		/// Format access mask setting color formatting.
		/// </summary>
		/// <param name="enabled">
		/// The enabled.
		/// </param>
		/// <returns>
		/// Set access mask flags are rendered red, rest black.
		/// </returns>
		protected Color GetItemColor(bool enabled)
		{
			// show enabled flag red
			if (enabled) return Color.Red;
			// unset flag black
			else return Color.Black;
		}

		#endregion

		#region Data Checking Helper

		/// <summary>
		/// The bit set.
		/// </summary>
		/// <param name="_o">
		/// The _o.
		/// </param>
		/// <param name="bitmask">
		/// The bitmask.
		/// </param>
		/// <returns>
		/// The bit set.
		/// </returns>
		protected bool BitSet(object _o, int bitmask)
		{
			var i = (int)_o;
			return (i & bitmask) != 0;
		}

		#endregion
	}
}