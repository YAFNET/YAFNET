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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration.Provider;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

namespace YAFProviders.Passthru
{
	class YAFMembershipPassThru : System.Web.Security.MembershipProvider
	{
		System.Web.Security.MembershipProvider _realProvider;

		public override void Initialize( string name, NameValueCollection config )
		{
			string realProviderName = config ["passThru"];


			if ( realProviderName == null || realProviderName.Length < 1 )
				throw new ProviderException( "Pass Thru provider name has not been specified in the web.config" );

			// Remove passThru configuration attribute
			config.Remove( "passThru" );

			// Check for further attributes
			if ( config.Count > 0 )
			{
				// Throw Provider error as no more attributes were expected
				throw new ProviderException( "Unrecognised Attribute on the Membership PassThru Provider" );
			}

			// Initialise the "Real" membership provider
			_realProvider = System.Web.Security.Membership.Providers [realProviderName];
		}

		public override string ApplicationName
		{
			get
			{
				return _realProvider.ApplicationName;
			}
			set
			{
				_realProvider.ApplicationName = value;
			}
		}

		public override bool ChangePassword( string username, string oldPassword, string newPassword )
		{
			return _realProvider.ChangePassword( username, oldPassword, newPassword );
		}

		public override bool ChangePasswordQuestionAndAnswer( string username, string password, string newPasswordQuestion, string newPasswordAnswer )
		{
			return _realProvider.ChangePasswordQuestionAndAnswer( username, password, newPasswordQuestion, newPasswordAnswer );
		}

		public override System.Web.Security.MembershipUser CreateUser( string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status )
		{
			return _realProvider.CreateUser( username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status );
		}

		public override bool DeleteUser( string username, bool deleteAllRelatedData )
		{
			return _realProvider.DeleteUser( username, deleteAllRelatedData );
		}

		public override bool EnablePasswordReset
		{
			get { return _realProvider.EnablePasswordReset; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return _realProvider.EnablePasswordRetrieval; }
		}

		public override System.Web.Security.MembershipUserCollection FindUsersByEmail( string emailToMatch, int pageIndex, int pageSize, out int totalRecords )
		{
			return _realProvider.FindUsersByEmail( emailToMatch, pageIndex, pageSize, out totalRecords );
		}

		public override System.Web.Security.MembershipUserCollection FindUsersByName( string usernameToMatch, int pageIndex, int pageSize, out int totalRecords )
		{
			return _realProvider.FindUsersByName( usernameToMatch, pageIndex, pageSize, out totalRecords );
		}

		public override System.Web.Security.MembershipUserCollection GetAllUsers( int pageIndex, int pageSize, out int totalRecords )
		{
			return _realProvider.GetAllUsers( pageIndex, pageSize, out totalRecords );
		}

		public override int GetNumberOfUsersOnline()
		{
			return _realProvider.GetNumberOfUsersOnline();
		}

		public override string GetPassword( string username, string answer )
		{
			return _realProvider.GetPassword( username, answer );
		}

		public override System.Web.Security.MembershipUser GetUser( string username, bool userIsOnline )
		{
			return _realProvider.GetUser( username, userIsOnline );
		}

		public override System.Web.Security.MembershipUser GetUser( object providerUserKey, bool userIsOnline )
		{
			return _realProvider.GetUser( providerUserKey, userIsOnline );
		}

		public override string GetUserNameByEmail( string email )
		{
			return _realProvider.GetUserNameByEmail( email );
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return _realProvider.MaxInvalidPasswordAttempts; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return _realProvider.MinRequiredNonAlphanumericCharacters; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return _realProvider.MinRequiredPasswordLength; }
		}

		public override int PasswordAttemptWindow
		{
			get { return _realProvider.PasswordAttemptWindow; }
		}

		public override System.Web.Security.MembershipPasswordFormat PasswordFormat
		{
			get { return _realProvider.PasswordFormat; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return _realProvider.PasswordStrengthRegularExpression; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return _realProvider.RequiresQuestionAndAnswer; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return _realProvider.RequiresUniqueEmail; }
		}

		public override string ResetPassword( string username, string answer )
		{
			return _realProvider.ResetPassword( username, answer );
		}

		public override bool UnlockUser( string userName )
		{
			return _realProvider.UnlockUser( userName );
		}

		public override void UpdateUser( System.Web.Security.MembershipUser user )
		{
			_realProvider.UpdateUser( user );
		}

		public override bool ValidateUser( string username, string password )
		{
			return _realProvider.ValidateUser( username, password );
		}
	}
}
