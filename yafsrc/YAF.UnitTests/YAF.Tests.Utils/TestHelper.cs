/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
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

namespace YAF.Tests.Utils
{
    using System.Text.RegularExpressions;

    using WatiN.Core;
    using WatiN.Core.Native.Windows;

    using YAF.Utils;

    /// <summary>
    /// Test Helper Class
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// Registers the standard test user.
        /// </summary>
        /// <param name="browser">The <paramref name="browser"/> instance.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// If User was Registered or not
        /// </returns>
        public static bool RegisterStandardTestUser(IE browser, string userName, string password)
        {
            browser.GoTo("{0}yaf_register.aspx".FormatWith(TestConfig.TestForumUrl));

            var email = "{0}@test.com".FormatWith(userName.ToLower());

            browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            // Check if Registrations are Disabled
            if (browser.ContainsText("You tried to enter an area where you didn't have access"))
            {
                return false;
            }

            // Accept the Rules
            if (browser.ContainsText("Forum Rules"))
            {
                browser.Button(Find.ById("forum_ctl04_Login1_LoginButton")).Click();
                browser.Refresh();
            }

            if (browser.ContainsText("Security Image"))
            {
                return false;
            }

            // Fill the Register Page
            browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_UserName"))).TypeText(
                userName);

            if (browser.ContainsText("Display Name"))
            {
                browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_DisplayName"))).TypeText(userName);
            }

            browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Password"))).TypeText(password);
            browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_ConfirmPassword"))).TypeText(password);
            browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Email"))).TypeText(email);
            browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Question"))).TypeText(password);
            browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_Answer"))).TypeText(password);

            ////browser.TextField(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_tbCaptcha"))).TypeText(captcha);

            // Create User
            browser.Button(Find.ById(new Regex("CreateUserWizard1_CreateUserStepContainer_StepNextButton"))).Click();

            if (!browser.ContainsText("Forum Preferences"))
            {
                return false;
            }

            browser.Button(Find.ById(new Regex("ProfileNextButton"))).Click();

            return browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists;
        }

        /// <summary>
        /// Logins the user.
        /// </summary>
        /// <param name="browser">The <paramref name="browser"/> instance.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userPassword">The user password.</param>
        /// <returns>If User login was successfully or not</returns>
        public static bool LoginUser(IE browser, string userName, string userPassword)
        {
            // Login User
            browser.GoTo("{0}yaf_login.aspx".FormatWith(TestConfig.TestForumUrl));

            // Check If User is already Logged In
            if (browser.Link(Find.ById(new Regex("_LogOutButton"))).Exists)
            {
                browser.Link(Find.ById("forum_ctl01_LogOutButton")).Click();

                browser.Button(Find.ById("forum_ctl02_OkButton")).Click();
            }

            browser.GoTo("{0}yaf_login.aspx".FormatWith(TestConfig.TestForumUrl));

            browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            browser.TextField(Find.ById(new Regex("Login1_UserName"))).TypeText(userName);
            browser.TextField(Find.ById(new Regex("Login1_Password"))).TypeText(userPassword);

            browser.Button(Find.ById(new Regex("LoginButton"))).ClickNoWait();

            browser.GoTo(TestConfig.TestForumUrl);

            return browser.Link(Find.ById(new Regex("LogOutButton"))).Exists;
        }
    }
}
