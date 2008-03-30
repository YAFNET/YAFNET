/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Globalization;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for register.
	/// </summary>
	public partial class register : YAF.Classes.Base.ForumPage
	{

		public register()
			: base("REGISTER")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Ederon : guess we don't need to test CanLogin anymore
			if ( /*!CanLogin ||*/ PageContext.BoardSettings.DisableRegistrations)
				YafBuildLink.AccessDenied();

			if (!IsPostBack)
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.forum));
				PageLinks.AddLink(GetText("TITLE"));

				// handle the CreateUser Step localization
				Control createUserTemplateRef = CreateUserWizard1.CreateUserStep.ContentTemplateContainer;

				CompareValidator passwordNoMatch = (CompareValidator)createUserTemplateRef.FindControl("PasswordCompare");
				RequiredFieldValidator usernameRequired = (RequiredFieldValidator)createUserTemplateRef.FindControl("UserNameRequired");
				RequiredFieldValidator passwordRequired = (RequiredFieldValidator)createUserTemplateRef.FindControl("PasswordRequired");
				RequiredFieldValidator confirmPasswordRequired = (RequiredFieldValidator)createUserTemplateRef.FindControl("ConfirmPasswordRequired");
				RequiredFieldValidator emailRequired = (RequiredFieldValidator)createUserTemplateRef.FindControl("EmailRequired");
				RequiredFieldValidator questionRequired = (RequiredFieldValidator)createUserTemplateRef.FindControl("QuestionRequired");
				RequiredFieldValidator answerRequired = (RequiredFieldValidator)createUserTemplateRef.FindControl("AnswerRequired");
				Button createUser = (Button)createUserTemplateRef.FindControl("StepNextButton");

				usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText("NEED_USERNAME");
				passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText("NEED_PASSWORD");
				confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = GetText("RETYPE_PASSWORD");
				passwordNoMatch.ToolTip = passwordNoMatch.ErrorMessage = GetText("NEED_MATCH");
				emailRequired.ToolTip = emailRequired.ErrorMessage = GetText("NEED_EMAIL");
				questionRequired.ToolTip = questionRequired.ErrorMessage = GetText("NEED_QUESTION");
				answerRequired.ToolTip = answerRequired.ErrorMessage = GetText("NEED_ANSWER");
				createUser.ToolTip = createUser.Text = GetText("CREATE_USER");

				// handle other steps localization
				((Button)FindWizardControl("ProfileNextButton")).Text = GetText("SAVE");
				((Button)FindWizardControl("ContinueButton")).Text = GetText("CONTINUE");

				// get the time zone data source
				DropDownList timeZones = ((DropDownList)FindWizardControl("TimeZones"));
				timeZones.DataSource = YafStaticData.TimeZones();

				if (!PageContext.BoardSettings.EmailVerification)
				{
					// automatically log in created users
					CreateUserWizard1.LoginCreatedUser = true;
					CreateUserWizard1.DisableCreatedUser = false;
					// success notification localization
					((Literal)FindWizardControl("AccountCreated")).Text = YAF.Classes.UI.BBCode.MakeHtml(GetText("ACCOUNT_CREATED"), true, false);
				}
				else
				{
					CreateUserWizard1.LoginCreatedUser = false;
					CreateUserWizard1.DisableCreatedUser = true;
					// success notification localization
					((Literal)FindWizardControl("AccountCreated")).Text = YAF.Classes.UI.BBCode.MakeHtml(GetText("ACCOUNT_CREATED_VERIFICATION"), true, false);
				}

				if (PageContext.BoardSettings.EnableCaptchaForRegister)
				{
					Session["CaptchaImageText"] = General.GetCaptchaString();
					Image imgCaptcha = (Image)createUserTemplateRef.FindControl("imgCaptcha");
					HtmlTableRow tr_captcha1 = (HtmlTableRow)createUserTemplateRef.FindControl("tr_captcha1");
					HtmlTableRow tr_captcha2 = (HtmlTableRow)createUserTemplateRef.FindControl("tr_captcha2");

					imgCaptcha.ImageUrl = String.Format("{0}resource.ashx?c=1", YafForumInfo.ForumRoot);
					tr_captcha1.Visible = true;
					tr_captcha2.Visible = true;
				}

				CreateUserWizard1.FinishDestinationPageUrl = YafForumInfo.ForumURL;

				DataBind();

				timeZones.Items.FindByValue("0").Selected = true;
			}
		}

		/// <summary>
		/// Find Wizard Control - Find a control in a wizard
		/// </summary>
		/// <param name="id">ID of target control</param>
		/// <returns>A control reference, if found, null, if not</returns>
		protected Control FindWizardControl(string id)
		{
			Control ctrlRtn = null;

			for (int i = 0; i < CreateUserWizard1.WizardSteps.Count; i++)
			{
				for (int j = 0; j <
				CreateUserWizard1.WizardSteps[i].Controls.Count; j++)
				{
					ctrlRtn = FindWizardControl((Control)CreateUserWizard1.WizardSteps[i].Controls[j], id);
					if (ctrlRtn != null) break;
				}
				if (ctrlRtn != null) break;
			}

			return ctrlRtn;

		} // end protected Control FindWizardControl(string id)

		/// <summary>
		/// Find Wizard Control - Find a control in a wizard, is recursive
		/// </summary>
		/// <param name="srcCtrl">Source/Root Control</param>
		/// <param name="id">ID of target control</param>
		/// <returns>A Control, if found; null, if not</returns>
		protected Control FindWizardControl(Control srcCtrl, string id)
		{
			Control ctrlRtn = srcCtrl.FindControl(id);

			if (ctrlRtn == null)
			{
				if (srcCtrl.HasControls())
				{
					int nbrCtrls = srcCtrl.Controls.Count;
					for (int i = 0; i < nbrCtrls; i++)
					{
						// Check all child controls of srcCtrl
						ctrlRtn = FindWizardControl(srcCtrl.Controls[i], id);
						if (ctrlRtn != null) break;
					} // endfor (int i=0; i < nbrCtrls; i++)
				} // endif (HasControls)

			} // endif (ctrlRtn == null)

			return ctrlRtn;
		} // end protected Control FindWizardControl(Control srcCtrl, string id)

		protected void CreateUserWizard1_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
		{

		}

		protected void CreateUserWizard1_NextButtonClick(object sender, WizardNavigationEventArgs e)
		{
			if (e.CurrentStepIndex == 2)
			{
				// this is the "Profile Information" step. Save the data to their profile (+ defaults).
				DropDownList timeZones = ((DropDownList)FindWizardControl("TimeZones"));
				TextBox locationTextBox = ((TextBox)FindWizardControl("Location"));
				TextBox homepageTextBox = ((TextBox)FindWizardControl("Homepage"));

				MembershipUser user = Membership.GetUser(CreateUserWizard1.UserName);

				// setup/save the profile
				YafUserProfile userProfile = PageContext.GetProfile(CreateUserWizard1.UserName);

				userProfile.Location = locationTextBox.Text.Trim();
				userProfile.Homepage = homepageTextBox.Text.Trim();

				userProfile.Save();

				// save the time zone...
				YAF.Classes.Data.DB.user_save(UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), PageContext.PageBoardID, null, null, Convert.ToInt32(timeZones.SelectedValue), null, null, null, null, null);
			}
		}

		protected void CreateUserWizard1_ContinueButtonClick(object sender, EventArgs e)
		{
			// redirect to the main forum URL
			YafBuildLink.Redirect(ForumPages.forum);
		}

		protected void CreateUserWizard1_CreateUserError(object sender, CreateUserErrorEventArgs e)
		{
			string createUserError = "";
			// find the type of error
			switch (e.CreateUserError)
			{
				case MembershipCreateStatus.DuplicateEmail:
					createUserError = GetText("ALREADY_REGISTERED");
					break;
				case MembershipCreateStatus.DuplicateUserName:
					createUserError = GetText("ALREADY_REGISTERED");
					break;
				case MembershipCreateStatus.InvalidEmail:
					createUserError = GetText("BAD_EMAIL");
					break;
				case MembershipCreateStatus.InvalidPassword:
					createUserError = GetText("BAD_PASSWORD");
					break;
				case MembershipCreateStatus.InvalidQuestion:
					createUserError = GetText("INVALID_QUESTION");
					break;
				case MembershipCreateStatus.InvalidUserName:
					createUserError = GetText("INVALID_USERNAME");
					break;
				case MembershipCreateStatus.InvalidAnswer:
					createUserError = GetText("INVALID_ANSWER");
					break;
				case MembershipCreateStatus.InvalidProviderUserKey:
					createUserError = "Invalid provider user key.";
					break;
				case MembershipCreateStatus.DuplicateProviderUserKey:
					createUserError = "Duplicate provider user key.";
					break;
				case MembershipCreateStatus.ProviderError:
					createUserError = "Provider Error";
					break;
				case MembershipCreateStatus.UserRejected:
					createUserError = "User creation failed: Reason is defined by the provider.";
					break;
			}
			PageContext.AddLoadMessage( createUserError );
			//Display the failure message in a client-side alert box
			//Page.ClientScript.RegisterStartupScript(Page.GetType(), "CreateUserError", String.Format("alert('{0}');", createUserError.Replace("'", "\'")), true);
		}

		protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
		{
			MembershipUser user = Membership.GetUser(CreateUserWizard1.UserName);

			// setup inital roles (if any) for this user
			RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, CreateUserWizard1.UserName);

			// create the user in the YAF DB as well as sync roles...
			int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

			// create empty profile just so they have one
			YafUserProfile userProfile = PageContext.GetProfile(CreateUserWizard1.UserName);
			// setup their inital profile information
			userProfile.Save();

			if (userID == null)
			{
				// something is seriously wrong here -- redirect to failure...
				YafBuildLink.Redirect(ForumPages.info, "i=7");
			}

			// handle e-mail verification if needed
			if (PageContext.BoardSettings.EmailVerification)
			{
				// get the user email
				TextBox emailTextBox = (TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email");
				string email = emailTextBox.Text.Trim();

				string hashinput = DateTime.Now.ToString() + email + Security.CreatePassword(20);
				string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

				// save verification record...
				YAF.Classes.Data.DB.checkemail_save(userID, hash, user.Email);

				YafTemplateEmail verifyEmail = new YafTemplateEmail( "VERIFYEMAIL" );

				string subject = String.Format(GetText("VERIFICATION_EMAIL_SUBJECT"), PageContext.BoardSettings.Name);

				verifyEmail.TemplateParams ["{link}"] = String.Format( "{1}{0}", YafBuildLink.GetLinkNotEscaped( ForumPages.approve, "k={0}", hash ), YafForumInfo.ServerURL );
				verifyEmail.TemplateParams ["{key}"] = hash;
				verifyEmail.TemplateParams ["{forumname}"] = PageContext.BoardSettings.Name;
				verifyEmail.TemplateParams ["{forumlink}"] = String.Format("{0}", YafForumInfo.ForumURL);

				verifyEmail.SendEmail( new System.Net.Mail.MailAddress( email, user.UserName ), subject, true );
			}
		}

		protected void CreateUserWizard1_CreatingUser(object sender, LoginCancelEventArgs e)
		{
			// trim username on postback
			CreateUserWizard1.UserName = CreateUserWizard1.UserName.Trim();

			// username cannot contain semi-colon
			if (CreateUserWizard1.UserName.Contains(";"))
			{
				PageContext.AddLoadMessage(GetText("BAD_USERNAME"));
				e.Cancel = true;
				return;
			}

			Control createUserTemplateRef = CreateUserWizard1.CreateUserStep.ContentTemplateContainer;
			TextBox tbCaptcha = (TextBox)createUserTemplateRef.FindControl("tbCaptcha");

			// verify captcha if enabled
			if (PageContext.BoardSettings.EnableCaptchaForRegister && Session["CaptchaImageText"].ToString() != tbCaptcha.Text.Trim())
			{
				PageContext.AddLoadMessage(GetText("BAD_CAPTCHA"));
				e.Cancel = true;
			}
		}


		public override bool IsProtected
		{
			get { return false; }
		}
	}
}
