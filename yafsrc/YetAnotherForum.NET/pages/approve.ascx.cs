/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Pages
{
	// YAF.Pages
	#region Using

	using System;
	using System.Data;
	using System.Web.Security;

	using YAF.Classes.Data;
	using YAF.Core;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// Summary description for approve.
	/// </summary>
	public partial class approve : ForumPage
	{
		#region Constructors and Destructors

		/// <summary>
		///   Initializes a new instance of the <see cref = "approve" /> class.
		/// </summary>
		public approve()
			: base("APPROVE")
		{
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets a value indicating whether IsProtected.
		/// </summary>
		public override bool IsProtected
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// The validate key_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		public void ValidateKey_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			DataTable dt = LegacyDb.checkemail_update(this.key.Text);
			DataRow row = dt.Rows[0];
			string dbEmail = row["Email"].ToString();

			bool keyVerified = (row["ProviderUserKey"] == DBNull.Value) ? false : true;

			this.approved.Visible = keyVerified;
			this.error.Visible = !keyVerified;

			if (!keyVerified)
			{
				return;
			}

			// approve and update e-mail in the membership as well...
			MembershipUser user = UserMembershipHelper.GetMembershipUserByKey(row["ProviderUserKey"]);
			if (!user.IsApproved)
			{
				user.IsApproved = true;
			}

			// update the email if anything was returned...
			if (user.Email != dbEmail && dbEmail != string.Empty)
			{
				user.Email = dbEmail;
			}

			// tell the provider to update...
			this.Get<MembershipProvider>().UpdateUser(user);

			// now redirect to main site...
			this.PageContext.LoadMessage.AddSession(this.GetText("EMAIL_VERIFIED"));

			// default redirect -- because if may not want to redirect to login.
			YafBuildLink.Redirect(ForumPages.forum);
		}

		#endregion

		#region Methods

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
			if (this.IsPostBack)
			{
				return;
			}

			this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

			this.ValidateKey.Text = this.GetText("validate");
			if (this.Request.QueryString["k"] != null)
			{
				this.key.Text = this.Request.QueryString["k"];
				this.ValidateKey_Click(sender, e);
			}
			else
			{
				this.approved.Visible = false;
				this.error.Visible = !this.approved.Visible;
			}
		}

		#endregion
	}
}