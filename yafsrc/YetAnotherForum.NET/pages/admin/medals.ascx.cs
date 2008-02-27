using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using System.Text;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Administration inferface for managing medals.
	/// </summary>
	public partial class medals : YAF.Classes.Base.AdminPage
	{
		#region Construcotrs & Overridden Methods

		/// <summary>
		/// Default constructor.
		/// </summary>
		public medals() : base("ADMIN_MEDALS") { }


		/// <summary>
		/// Creates page links for this page.
		/// </summary>
		protected override void CreatePageLinks()
		{
			// forum index
			PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			// administration index
			PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
			// currect page
			PageLinks.AddLink("Medals", "");
		}


		#endregion


		#region Event Handlers

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load(object sender, EventArgs e)
		{
			// this needs to be done just once, not during postbacks
			if (!IsPostBack)
			{
				// create page links
				CreatePageLinks();

				// bind data
				BindData();
			}
		}


		/// <summary>
		/// Handles on load event for delete button.
		/// </summary>
		protected void Delete_Load(object sender, System.EventArgs e)
		{
			General.AddOnClickConfirmDialog(sender, "Delete this Medal?");
		}


		/// <summary>
		/// Handles click on new medal button.
		/// </summary>
		protected void NewMedal_Click(object sender, System.EventArgs e)
		{
			// redirect to medal edit page
			YafBuildLink.Redirect(ForumPages.admin_editmedal);
		}


		/// <summary>
		/// Handles item command of medal list repeater.
		/// </summary>
		protected void MedalList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "edit":
					// edit medal
					YafBuildLink.Redirect(ForumPages.admin_editmedal, "m={0}", e.CommandArgument);
					break;
				case "delete":
					// delete medal
					YAF.Classes.Data.DB.medal_delete(e.CommandArgument);
					// re-bind data
					BindData();
					break;
				case "moveup":
					YAF.Classes.Data.DB.medal_resort(PageContext.PageBoardID, e.CommandArgument, -1);
					BindData();
					break;
				case "movedown":
					YAF.Classes.Data.DB.medal_resort(PageContext.PageBoardID, e.CommandArgument, 1);
					BindData();
					break;
			}
		}

		#endregion


		#region Data Binding & Formatting

		/// <summary>
		/// Bind data for this control.
		/// </summary>
		private void BindData()
		{
			// list medals for this board
			MedalList.DataSource = YAF.Classes.Data.DB.medal_list(PageContext.PageBoardID, null);

			// bind data to controls
			DataBind();
		}


		/// <summary>
		/// Formats HTML output to display image representation of a medal.
		/// </summary>
		/// <returns>HTML markup with image representation of a medal.</returns>
		protected string RenderImages(object data)
		{
			StringBuilder output=new StringBuilder(250);

			DataRowView dr = (DataRowView)data;

			// image of medal
			output.AppendFormat(
				"<img src=\"{0}images/medals/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" align=\"top\" />",
				YafForumInfo.ForumRoot,
				dr["SmallMedalURL"],
				dr["SmallMedalWidth"],
				dr["SmallMedalHeight"],
				"Medal image as it'll be displayed in user box."
				);

			// if available, create also ribbon bar image of medal
			if (dr["SmallRibbonURL"] != null)
			{
				output.AppendFormat(
					" &nbsp; <img src=\"{0}images/medals/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" align=\"top\" />",
					YafForumInfo.ForumRoot,
					dr["SmallRibbonURL"],
					dr["SmallRibbonWidth"],
					dr["SmallRibbonHeight"],
					"Ribbon bar image as it'll be displayed in user box."
					);
			}

			return output.ToString();
		}

		#endregion
	}

}