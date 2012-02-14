/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

namespace YAF.Tests.UserTests
{
    using System.IO;
    using System.Net;
    using System.Xml;
    using ICSharpCode.SharpZipLib.Zip;
    using netDumbster.smtp;
    using NUnit.Framework;
    using WatiN.Core;
    using WatiN.Core.Native.Windows;
    using YAF.Tests.Utils;
    using YAF.Utils;

    /// <summary>
    /// Testing a YAF Installation
    /// </summary>
    [SetUpFixture]
    public class TestSetup
    {
        /// <summary>
        /// The Browser Instance
        /// </summary>
        public static IE IEInstance;

        /// <summary>
        /// The Test Mail Server
        /// </summary>
        public static SimpleSmtpServer SmtpServer;

        /// <summary>
        /// Download YAF, Create Application and setup db
        /// </summary>
        [SetUp]
        public void Preparations()
        {
            if (TestConfig.UseExistingInstallation)
            {
                return;
            }

            var applicationPath = Path.Combine(TestConfig.InstallPhysicalPath, TestConfig.TestApplicationName);

            // Delete folder
            if (Directory.Exists(applicationPath))
            {
                Directory.Delete(applicationPath, true);
            }

            // Create folder
            Directory.CreateDirectory(TestConfig.InstallPhysicalPath);

            var installZip = string.Empty;

            switch (TestConfig.PackageLocation)
            {
                case "Local":
                    {
                        installZip = TestConfig.LocalReleasePackageFile;
                    }

                    break;
                case "CodePlex":
                    {
                        installZip = Path.Combine(TestConfig.InstallPhysicalPath, "YAF-BIN.zip");

                        // download latest release
                        var webClient = new WebClient();
                        webClient.DownloadFile(TestConfig.ReleaseDownloadUrl, installZip);
                        webClient.Dispose();
                    }

                    break;
            }

            // Exctract Install Package
            var fastZip = new FastZip();

            fastZip.ExtractZip(
                installZip, applicationPath, FastZip.Overwrite.Always, null, null, "YetAnotherForum.NET", false);

            foreach (
                string dirPath in
                    Directory.GetDirectories(
                        Path.Combine(applicationPath, "YetAnotherForum.NET"), "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(
                    dirPath.Replace(Path.Combine(applicationPath, "YetAnotherForum.NET"), applicationPath));
            }

            foreach (
                string newPath in
                    Directory.GetFiles(
                        Path.Combine(applicationPath, "YetAnotherForum.NET"), "*.*", SearchOption.AllDirectories))
            {
                File.Move(
                    newPath, newPath.Replace(Path.Combine(applicationPath, "YetAnotherForum.NET"), applicationPath));
            }

            Directory.Delete(Path.Combine(applicationPath, "YetAnotherForum.NET"), true);

            // Rename Web.config
            File.Copy(
                Path.Combine(applicationPath, "recommended-NET-web.config"),
                Path.Combine(applicationPath, "web.config"));

            // Create Website in IIS
            IISManager.CreateIISApplication(TestConfig.TestApplicationName, applicationPath);

            // Add Config Password
            XmlDocument xmlAppConfig = new XmlDocument();

            xmlAppConfig.Load(Path.Combine(applicationPath, "app.config"));

            XmlNode xNode = xmlAppConfig.CreateNode(XmlNodeType.Element, "add", string.Empty);
            XmlAttribute xKey = xmlAppConfig.CreateAttribute("key");
            XmlAttribute xValue = xmlAppConfig.CreateAttribute("value");

            xKey.Value = "YAF.ConfigPassword";
            xValue.Value = TestConfig.ConfigPassword;
            xNode.Attributes.Append(xKey);
            xNode.Attributes.Append(xValue);
            xmlAppConfig.GetElementsByTagName("appSettings")[0].InsertAfter(
                xNode, xmlAppConfig.GetElementsByTagName("appSettings")[0].LastChild);

            xmlAppConfig.Save(Path.Combine(applicationPath, "app.config"));

            // Setup Mail Config
            var xmlMailConfig = new XmlDocument();

            xmlMailConfig.Load(Path.Combine(applicationPath, "mail.config"));

            XmlNode xNetworkNode = xmlMailConfig.CreateNode(XmlNodeType.Element, "network", string.Empty);
            var xhost = xmlMailConfig.CreateAttribute("host");
            var xport = xmlMailConfig.CreateAttribute("port");
            var xpassword = xmlMailConfig.CreateAttribute("password");
            var xuserName = xmlMailConfig.CreateAttribute("userName");

            xhost.Value = TestConfig.TestMailHost;
            xport.Value = TestConfig.TestMailPort;
            xpassword.Value = TestConfig.TestMailPassword;
            xuserName.Value = TestConfig.TestMailUserName;

            xNetworkNode.Attributes.Append(xhost);
            xNetworkNode.Attributes.Append(xport);
            xNetworkNode.Attributes.Append(xpassword);
            xNetworkNode.Attributes.Append(xuserName);

            xmlMailConfig.GetElementsByTagName("smtp")[0].InsertAfter(
                xNetworkNode, xmlMailConfig.GetElementsByTagName("smtp")[0].LastChild);

            xmlMailConfig.GetElementsByTagName("smtp")[0].Attributes["from"].Value = TestConfig.TestForumMail;

            xmlMailConfig.Save(Path.Combine(applicationPath, "mail.config"));

            // Inject Custom.sql file
            File.Copy(@"..\..\testfiles\custom.sql", Path.Combine(applicationPath, "install\\custom.sql"));

            if (TestConfig.UseTestMailServer)
            {
                // Launch Mail Server
                SmtpServer = SimpleSmtpServer.Start(TestConfig.TestMailPort.ToType<int>());
            }

            // Setup DB
            DBManager.AttachDatabase(TestConfig.TestDatabase, Path.Combine(applicationPath, "App_Data\\Database.mdf"));

            this.SetupWebsite();
        }

        /// <summary>
        /// The Tear Down
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (TestConfig.UseTestMailServer && SmtpServer != null)
            {
                // Stop Mail Server
                SmtpServer.Stop();
            }

            if (TestConfig.UseExistingInstallation)
            {
                return;
            }

            var applicationPath = Path.Combine(TestConfig.InstallPhysicalPath, TestConfig.TestApplicationName);

            // Recycle App Pool
            IISManager.RecycleApplicationPool(TestConfig.TestApplicationName);

            // Delete App from IIS
            IISManager.DeleteIISApplication(TestConfig.TestApplicationName);

            // Detach Database
            DBManager.DropDatabase(TestConfig.TestDatabase);

            // Delete Files
            if (File.Exists(Path.Combine(TestConfig.InstallPhysicalPath, "YAF-BIN.zip")))
            {
                File.Delete(Path.Combine(TestConfig.InstallPhysicalPath, "YAF-BIN.zip"));
            }

            Directory.Delete(applicationPath, true);

            IEInstance.Close();
        }

        /// <summary>
        /// Setups the Test website.
        /// </summary>
        private void SetupWebsite()
        {
            IEInstance = new IE();

            IEInstance.GoTo("{0}install/default.aspx".FormatWith(TestConfig.TestForumUrl));

            IEInstance.ShowWindow(NativeMethods.WindowShowStyle.Maximize);

            IEInstance.WaitForComplete(5000);

            // Enter Config Password
            IEInstance.TextField(Find.ById("InstallWizard_txtEnteredPassword")).TypeText(TestConfig.ConfigPassword);
            IEInstance.Button(Find.ById("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

            IEInstance.Button(Find.ById("InstallWizard_StepNavigationTemplateContainerID_StepPreviousButton")).Click();

            IEInstance.RadioButton(Find.ById("InstallWizard_rblYAFDatabase_1")).Click();

            // Enter YAF Database Connection
            IEInstance.TextField(Find.ById("InstallWizard_Parameter1_Value")).TypeText(TestConfig.DatabaseServer);

            IEInstance.TextField(Find.ById("InstallWizard_Parameter2_Value")).TypeText(TestConfig.TestDatabase);

            // Test Database Conncection
            IEInstance.Button(Find.ById("InstallWizard_btnTestDBConnection")).Click();

            Assert.IsTrue(IEInstance.ContainsText("Connection Succeeded"), "Database Connection Is Wrong");

            IEInstance.Button(Find.ById("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

            // Test Mail Setup
            IEInstance.TextField(Find.ById("InstallWizard_txtTestFromEmail")).TypeText(TestConfig.TestForumMail);

            IEInstance.TextField(Find.ById("InstallWizard_txtTestToEmail")).TypeText("receiver@there.com");

            IEInstance.Button(Find.ById("InstallWizard_btnTestSmtp")).Click();

            Assert.IsTrue(
                IEInstance.ContainsText("Mail Sent. Verify it's received at your entered email address."),
                "Mail Send Failed");

            if (TestConfig.UseTestMailServer)
            {
                SmtpMessage mail = SmtpServer.ReceivedEmail[0];

                Assert.AreEqual("receiver@there.com", mail.ToAddresses[0].ToString(), "Receiver does not match");
                Assert.IsTrue(mail.FromAddress.ToString().Contains(TestConfig.TestForumMail), "Sender does not match");
                Assert.AreEqual(
                    "Test Email From Yet Another Forum.NET", mail.Headers["Subject"], "Subject does not match");

                Assert.AreEqual(
                    "The email sending appears to be working from your YAF installation.",
                    mail.MessageParts[0].BodyView,
                    "Body does not match");
            }

            // Now continue to Initialize Database
            IEInstance.Button(Find.ById("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

            // Initialize Database
            IEInstance.Button(Find.ById("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).ClickNoWait();

            IEInstance.WaitUntilContainsText("Create Board", 300);

            Assert.IsTrue(IEInstance.ContainsText("Create Board"));

            // Board Settings
            IEInstance.TextField(Find.ById("InstallWizard_TheForumName")).TypeText(TestConfig.TestApplicationName);
            IEInstance.TextField(Find.ById("InstallWizard_ForumEmailAddress")).TypeText(TestConfig.TestForumMail);

            // Admin User
            IEInstance.TextField(Find.ById("InstallWizard_UserName")).TypeText(TestConfig.AdminUserName);
            IEInstance.TextField(Find.ById("InstallWizard_AdminEmail")).TypeText(TestConfig.TestForumMail);
            IEInstance.TextField(Find.ById("InstallWizard_Password1")).TypeText(TestConfig.AdminPassword);
            IEInstance.TextField(Find.ById("InstallWizard_Password2")).TypeText(TestConfig.AdminPassword);
            IEInstance.TextField(Find.ById("InstallWizard_SecurityQuestion")).TypeText(TestConfig.AdminPassword);
            IEInstance.TextField(Find.ById("InstallWizard_SecurityAnswer")).TypeText(TestConfig.AdminPassword);

            IEInstance.Button(Find.ById("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).ClickNoWait();

            IEInstance.WaitUntilContainsText("Setup/Upgrade Finished", 300);

            Assert.IsTrue(IEInstance.ContainsText("Setup/Upgrade Finished"));

            IEInstance.Button(Find.ById("InstallWizard_FinishNavigationTemplateContainerID_FinishButton")).ClickNoWait();

            IEInstance.WaitUntilContainsText("Welcome Guest!", 300);

            Assert.IsTrue(IEInstance.ContainsText("Welcome Guest!"), "Installation failed");
        }
    }
}
