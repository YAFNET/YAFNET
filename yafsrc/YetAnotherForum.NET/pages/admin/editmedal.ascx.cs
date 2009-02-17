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
using System.IO;
using YAF.Classes.Data;
using System.Drawing;

namespace YAF.Pages.Admin
{
	public partial class editmedal : YAF.Classes.Base.AdminPage
	{
		#region Constructors & Overriden Methods

		/// <summary>
		/// Default constructor.
		/// </summary>
		public editmedal() : base("ADMIN_EDITMEDAL") { }


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
			PageLinks.AddLink("Edit Medal", "");
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

			// set previews
			SetPreview(MedalImage, MedalPreview);
			SetPreview(RibbonImage, RibbonPreview);
			SetPreview(SmallMedalImage, SmallMedalPreview);
			SetPreview(SmallRibbonImage, SmallRibbonPreview);
		}


		/// <summary>
		/// Handles click on cancel button.
		/// </summary>
		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			// go back to medals administration
			YafBuildLink.Redirect(ForumPages.admin_medals);
		}


		/// <summary>
		/// Handles save button click.
		/// </summary>
		protected void Save_Click(object sender, System.EventArgs e)
		{
			if (MedalImage.SelectedIndex <= 0)
			{
				PageContext.AddLoadMessage("Medal image must be specified!");
				return;
			}
			else if (SmallMedalImage.SelectedIndex <= 0)
			{
				PageContext.AddLoadMessage("Small medal image must be specified!");
				return;
			}

			// data
			object medalID = null;
			object imageURL, smallImageURL, ribbonURL = null, smallRibbonURL = null;
			object ribbonWidth = null, ribbonHeight = null;
			Size imageSize;
			MedalFlags flags = new MedalFlags(0);

			// retrieve medal ID, use null if we are creating new one
			if (Request.QueryString["m"] != null) medalID = Request.QueryString["m"];

			// flags
			flags.ShowMessage = ShowMessage.Checked;
			flags.AllowRibbon = AllowRibbon.Checked;
			flags.AllowReOrdering = AllowReOrdering.Checked;
			flags.AllowHiding = AllowHiding.Checked;

			// get medal images
			imageURL = MedalImage.SelectedValue;
			smallImageURL = SmallMedalImage.SelectedValue;
			if (RibbonImage.SelectedIndex > 0) ribbonURL = RibbonImage.SelectedValue;
			if (SmallRibbonImage.SelectedIndex > 0)
			{
				smallRibbonURL = SmallRibbonImage.SelectedValue;

				imageSize = GetImageSize(smallRibbonURL.ToString());
				ribbonWidth = imageSize.Width;
				ribbonHeight = imageSize.Height;
			}

			// get size of small image
			imageSize = GetImageSize(smallImageURL.ToString());			

			// save medal
			DB.medal_save(PageContext.PageBoardID, medalID, Name.Text, Description.Text, Message.Text, Category.Text, imageURL, ribbonURL,
					smallImageURL, smallRibbonURL, imageSize.Width, imageSize.Height, ribbonWidth, ribbonHeight, SortOrder.Text, flags.BitValue);

			// go back to medals administration
			YafBuildLink.Redirect( ForumPages.admin_medals );
		}


		/// <summary>
		/// Adds javascript popup to remove group link button.
		/// </summary>
		protected void GroupRemove_Load(object sender, EventArgs e)
		{
			General.AddOnClickConfirmDialog(sender, "Remove medal from this group?");
		}


		/// <summary>
		/// Adds javascript popup to remove user link button.
		/// </summary>
		protected void UserRemove_Load(object sender, EventArgs e)
		{
			General.AddOnClickConfirmDialog(sender, "Remove medal from this user?");
		}


		/// <summary>
		/// Handles clear button click event.
		/// </summary>
		protected void Clear_Click(object sender, EventArgs e)
		{
			// clear drop down
			UserNameList.Items.Clear();
			// hide it and show empty UserName text box
			UserNameList.Visible = false;
			UserName.Text = null;
			UserName.Visible = true;
			UserID.Text = null;
			// show find users and all users (if user is admin)
			FindUsers.Visible = true;
			// clear button is not necessary now
			Clear.Visible = false;
		}


		/// <summary>
		/// Handles find users button click event.
		/// </summary>
		protected void FindUsers_Click(object sender, EventArgs e)
		{
			// try to find users by user name
			using (DataTable dt = DB.user_find(PageContext.PageBoardID, true, UserName.Text, null))
			{
				if (dt.Rows.Count > 0)
				{
					// we found a user(s)
					UserNameList.DataSource = dt;
					UserNameList.DataValueField = "UserID";
					UserNameList.DataTextField = "Name";
					UserNameList.DataBind();

					// hide To text box and show To drop down
					UserNameList.Visible = true;
					UserName.Visible = false;
					// find is no more needed
					FindUsers.Visible = false;
					// we need clear button displayed now
					Clear.Visible = true;
				}
			}
		}


		/// <summary>
		/// Handles click on GroupList repeaters item command link buttton.
		/// </summary>
		protected void GroupList_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "edit":
					// load group-medal to the controls
					using (DataTable dt = DB.group_medal_list(e.CommandArgument, Request.QueryString["m"]))
					{
						// prepare editing interface
						AddGroup_Click(null, e);
						// tweak it for editing
						GroupMedalEditTitle.Text = "Edit Medal of a Group";
						AvailableGroupList.Enabled = false;

						// we are intereseted inly in first row
						DataRow r = dt.Rows[0];

						// load data to controls
						AvailableGroupList.SelectedIndex = -1;
						AvailableGroupList.Items.FindByValue(r["GroupID"].ToString()).Selected = true;
						GroupMessage.Text = r["MessageEx"].ToString();
						GroupSortOrder.Text = r["SortOrder"].ToString();
						GroupOnlyRibbon.Checked = (bool)r["OnlyRibbon"];
						GroupHide.Checked = (bool)r["Hide"];
					}
					break;
				case "remove":
					DB.group_medal_delete(e.CommandArgument, Request.QueryString["m"]);
					BindData();
					break;
			}
		}


		/// <summary>
		/// Handles click on UserList repeaters item command link buttton.
		/// </summary>
		protected void UserList_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "edit":
					// load user-medal to the controls
					using (DataTable dt = DB.user_medal_list(e.CommandArgument, Request.QueryString["m"]))
					{
						// prepare editing interface
						AddUser_Click(null, e);
						// tweak it for editing
						UserMedalEditTitle.Text = "Edit Medal of a User";
						UserName.Enabled = false;
						FindUsers.Visible = false;

						// we are intereseted inly in first row
						DataRow r = dt.Rows[0];

						// load data to controls
						UserID.Text = r["UserID"].ToString();
						UserName.Text = r["UserName"].ToString();
						UserMessage.Text = r["MessageEx"].ToString();
						UserSortOrder.Text = r["SortOrder"].ToString();
						UserOnlyRibbon.Checked = (bool)r["OnlyRibbon"];
						UserHide.Checked = (bool)r["Hide"];
					}
					break;
				case "remove":
					// delete user-medal
					DB.user_medal_delete(e.CommandArgument, Request.QueryString["m"]);
					BindData();
					break;
			}
		}


		/// <summary>
		/// Handles click on add user button.
		/// </summary>
		/// <remarks>Shows user-medal adding/editing controls.</remarks>
		protected void AddUser_Click(object sender, EventArgs e)
		{
			// set title
			UserMedalEditTitle.Text = "Add Medal to a User";

			// clear controls
			UserID.Text = null;
			UserName.Text = null;
			UserNameList.Items.Clear();
			UserMessage.Text = null;
			UserOnlyRibbon.Checked = false;
			UserHide.Checked = false;
			UserSortOrder.Text = SortOrder.Text;

			// set controls visibility and availability
			UserName.Enabled = true;
			UserName.Visible = true;
			UserNameList.Visible = false;
			FindUsers.Visible = true;
			Clear.Visible = false;

			// show editing controls and hide row with add user button
			AddUserRow.Visible = false;
			AddUserPanel.Visible = true;

			// focus on save button
			AddUserSave.Focus();

			// disable global save button to prevent confusion
			Save.Enabled = false;
		}


		/// <summary>
		/// Handles click on save user button.
		/// </summary>
		protected void AddUserSave_Click(object sender, EventArgs e)
		{
			// test if there is specified unsername/user id
			if (String.IsNullOrEmpty(UserID.Text) && String.IsNullOrEmpty(UserNameList.SelectedValue) &&
				String.IsNullOrEmpty(UserName.Text))
			{
				// no username, nor userID specified
				PageContext.AddLoadMessage("Please specify valid user!");
				return;
			}
			else if (String.IsNullOrEmpty(UserNameList.SelectedValue) && String.IsNullOrEmpty(UserID.Text))
			{
				// only username is specified, we must find id for it
				using (DataTable dt = DB.user_find(PageContext.PageBoardID, true, UserName.Text, null))
				{
					if (dt.Rows.Count > 1)
					{
						// more than one user is avalilable for this username
						PageContext.AddLoadMessage("Ambiguous user name specified!");
						return;
					}
					else if (dt.Rows.Count < 1)
					{
						// no user found
						PageContext.AddLoadMessage("Please specify valid user!");
						return;
					}
					else
					{
						// save id to the control
						UserID.Text = dt.Rows[0]["UserID"].ToString();
					}
				}
			}
			else if (String.IsNullOrEmpty(UserID.Text))
			{
				// user is selected in dropdown, we must get id to UserID control
				UserID.Text = UserNameList.SelectedValue;
			}

			// save user, if there is no message specified, pass null
			DB.user_medal_save(UserID.Text, Request.QueryString["m"], String.IsNullOrEmpty(UserMessage.Text) ? null : UserMessage.Text, UserHide.Checked, UserOnlyRibbon.Checked, UserSortOrder.Text, null);

			// disable/hide edit controls
			AddUserCancel_Click(sender, e);

			// re-bind data
			BindData();
		}


		/// <summary>
		/// Hides user add/edit controls.
		/// </summary>
		protected void AddUserCancel_Click(object sender, EventArgs e)
		{
			// set visibility
			AddUserRow.Visible = true;
			AddUserPanel.Visible = false;

			// re-enable global save button
			Save.Enabled = true;
		}


		/// <summary>
		/// Handles click on add group button.
		/// </summary>
		/// <remarks>Shows user-medal adding/editing controls.</remarks>
		protected void AddGroup_Click(object sender, EventArgs e)
		{
			// set title
			GroupMedalEditTitle.Text = "Add Medal to a Group";

			// clear controls
			AvailableGroupList.SelectedIndex = -1;
			GroupMessage.Text = null;
			GroupOnlyRibbon.Checked = false;
			GroupHide.Checked = false;
			GroupSortOrder.Text = SortOrder.Text;

			// set controls visibility and availability
			AvailableGroupList.Enabled = true;

			// show editing controls and hide row with add user button
			AddGroupRow.Visible = false;
			AddGroupPanel.Visible = true;

			// focus on save button
			AddGroupSave.Focus();

			// disable global save button to prevent confusion
			Save.Enabled = false;
		}


		/// <summary>
		/// Handles click on save group button.
		/// </summary>
		protected void AddGroupSave_Click(object sender, EventArgs e)
		{
			// test if there is specified unsername/user id
			if (AvailableGroupList.SelectedIndex<0)
			{
				// no group selected
				PageContext.AddLoadMessage("Please select user group!");
				return;
			}

			// save group, if there is no message specified, pass null
			DB.group_medal_save(AvailableGroupList.SelectedValue, Request.QueryString["m"], String.IsNullOrEmpty(GroupMessage.Text) ? null : GroupMessage.Text, GroupHide.Checked, GroupOnlyRibbon.Checked, GroupSortOrder.Text);

			// disable/hide edit controls
			AddGroupCancel_Click(sender, e);

			// re-bind data
			BindData();
		}


		/// <summary>
		/// Hides group add/edit controls.
		/// </summary>
		protected void AddGroupCancel_Click(object sender, EventArgs e)
		{
			// set visibility
			AddGroupRow.Visible = true;
			AddGroupPanel.Visible = false;

			// re-enable global save button
			Save.Enabled = true;
		}
		
		#endregion


		#region Data Bidining & Formatting

		/// <summary>
		/// Bind data for this control.
		/// </summary>
		private void BindData()
		{
			// load available images from images/medals folder
			using (DataTable dt = new DataTable("Files"))
			{
				// create structure
				dt.Columns.Add("FileID", typeof(long));
				dt.Columns.Add("FileName", typeof(string));
				dt.Columns.Add("Description", typeof(string));

				// add blank row
				DataRow dr = dt.NewRow();
				dr["FileID"] = 0;
				dr["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
				dr["Description"] = "Select Medal Image";
				dt.Rows.Add(dr);

				// add files from medals folder
				DirectoryInfo dir = new DirectoryInfo(Request.MapPath(String.Format("{0}images/medals", YafForumInfo.ForumFileRoot)));
				FileInfo[] files = dir.GetFiles("*.*");

				long nFileID = 1;

				foreach (FileInfo file in files)
				{
					string sExt = file.Extension.ToLower();

					// do just images
					if (sExt != ".png" && sExt != ".gif" && sExt != ".jpg")
						continue;

					dr = dt.NewRow();
					dr["FileID"] = nFileID++;
					dr["FileName"] = file.Name;
					dr["Description"] = file.Name;
					dt.Rows.Add(dr);
				}

				// medal image
				MedalImage.DataSource = dt;
				MedalImage.DataValueField = "FileName";
				MedalImage.DataTextField = "Description";

				// ribbon bar image
				RibbonImage.DataSource = dt;
				RibbonImage.DataValueField = "FileName";
				RibbonImage.DataTextField = "Description";

				// small medal image
				SmallMedalImage.DataSource = dt;
				SmallMedalImage.DataValueField = "FileName";
				SmallMedalImage.DataTextField = "Description";

				// small ribbon bar image
				SmallRibbonImage.DataSource = dt;
				SmallRibbonImage.DataValueField = "FileName";
				SmallRibbonImage.DataTextField = "Description";
			}

			// bind data to controls
			DataBind();

			// load existing medal if we are editing one
			if (Request.QueryString["m"] != null)
			{
				// load users and groups who has been assigned this medal
				UserList.DataSource = DB.user_medal_list(null, Request.QueryString["m"]);
				UserList.DataBind();
				GroupList.DataSource = DB.group_medal_list(null, Request.QueryString["m"]);
				GroupList.DataBind();

				// enable adding users/groups
				AddUserRow.Visible = true;
				AddGroupRow.Visible = true;

				using (DataTable dt = YAF.Classes.Data.DB.medal_list(Request.QueryString["m"]))
				{
					// get data row
					DataRow row = dt.Rows[0];

					// load flags
					MedalFlags flags = new MedalFlags(row["Flags"]);

					// set controls
					Name.Text = row["Name"].ToString();
					Description.Text = row["Description"].ToString();
					Message.Text = row["Message"].ToString();
					Category.Text = row["Category"].ToString();
					SortOrder.Text = row["SortOrder"].ToString();
					ShowMessage.Checked = flags.ShowMessage;
					AllowRibbon.Checked = flags.AllowRibbon;
					AllowHiding.Checked = flags.AllowHiding;
					AllowReOrdering.Checked = flags.AllowReOrdering;

					// select images
					SelectImage(MedalImage, MedalPreview, row["MedalURL"]);
					SelectImage(RibbonImage, RibbonPreview, row["RibbonURL"]);
					SelectImage(SmallMedalImage, SmallMedalPreview, row["SmallMedalURL"]);
					SelectImage(SmallRibbonImage, SmallRibbonPreview, row["SmallRibbonURL"]);
				}

				using (DataTable dt = DB.group_list(PageContext.PageBoardID, null))
				{
					// get data row
					DataRow row = dt.Rows[0];

					AvailableGroupList.DataSource = dt;
					AvailableGroupList.DataTextField = "Name";
					AvailableGroupList.DataValueField = "GroupID";
					AvailableGroupList.DataBind();
				}
			}
			else
			{
				// set all previews on blank image
				MedalPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
				RibbonPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
				SmallMedalPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
				SmallRibbonPreview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
			}
		}


		/// <summary>
		/// Creates link to group editing admin interface.
		/// </summary>
		protected string FormatGroupLink(object data)
		{
			DataRowView dr = (DataRowView)data;

			return String.Format("<a href=\"{1}\">{0}</a>",
				dr["GroupName"],
				YafBuildLink.GetLink(ForumPages.admin_editgroup, "i={0}", dr["GroupID"])
				);
		}


		/// <summary>
		/// Creates link to user editing admin interface.
		/// </summary>
		protected string FormatUserLink(object data)
		{
			DataRowView dr = (DataRowView)data;

			return String.Format("<a href=\"{1}\">{0}</a>",
				dr["UserName"],
				YafBuildLink.GetLink(ForumPages.admin_edituser, "u={0}", dr["UserID"])
				);
		}


		/// <summary>
		/// Select image in dropdown list and sets appropriate preview.
		/// </summary>
		/// <param name="list">DropDownList where to search.</param>
		/// <param name="preview">Preview image.</param>
		/// <param name="imageURL">URL to seach for.</param>
		private void SelectImage(DropDownList list, HtmlImage preview, object imageURL) { SelectImage(list, preview, imageURL.ToString()); }
		/// <summary>
		/// Select image in dropdown list and sets appropriate preview.
		/// </summary>
		/// <param name="list">DropDownList where to search.</param>
		/// <param name="preview">Preview image.</param>
		/// <param name="imageURL">URL to seach for.</param>
		private void SelectImage(DropDownList list, HtmlImage preview, string imageURL)
		{
			// try to find item in a list
			ListItem item = list.Items.FindByText(imageURL);
			
			if (item != null)
			{
				// select found item
				item.Selected = true;
				// set preview image
				preview.Src = String.Format("{0}images/medals/{1}", YafForumInfo.ForumRoot, imageURL);
			}
			else
			{
				// if we found nothing, set blank image as preview
				preview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
			}
		}


		/// <summary>
		/// Set onchange event for image selector DropDown to set preview image.
		/// </summary>
		/// <param name="imageSelector">DropDownList with image file listed.</param>
		/// <param name="imagePreview">Image for showing preview.</param>
		private void SetPreview(WebControl imageSelector, HtmlControl imagePreview)
		{
			// create javascript
			imageSelector.Attributes["onchange"] = String.Format(
				"getElementById('{1}').src='{0}images/medals/' + this.value",
				YafForumInfo.ForumRoot,
				imagePreview.ClientID
				);
		}


		/// <summary>
		/// Gets size of image located in medals directory.
		/// </summary>
		/// <param name="filename">Name of file.</param>
		/// <returns>Size of image.</returns>
		private Size GetImageSize(string filename)
		{
			using (System.Drawing.Image img = System.Drawing.Image.FromFile(
						Server.MapPath(String.Format("{0}images/medals/{1}", YafForumInfo.ForumFileRoot, filename))))
			{
				return img.Size;
			}
		}

		#endregion
	}
}