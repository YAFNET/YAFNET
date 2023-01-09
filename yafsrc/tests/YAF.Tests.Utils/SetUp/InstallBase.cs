/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Utils.SetUp;

using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Xml;

using ICSharpCode.SharpZipLib.Zip;

using netDumbster.smtp;

using NUnit.Framework;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using YAF.Tests.Utils.Extensions;
using YAF.Types.Extensions;

/// <summary>
/// The InstallBase Class
/// </summary>
public class InstallBase
{
    /// <summary>
    /// Gets the IE instance.
    /// </summary>
    /// <value>
    /// The IE instance.
    /// </value>
    public ChromeDriver ChromeDriver { get; private set; }

    /// <summary>
    /// Gets or sets the SMTP server.
    /// </summary>
    /// <value>
    /// The SMTP server.
    /// </value>
    private SimpleSmtpServer SmtpServer { get; set; }

    /// <summary>
    /// The Web site Set up.
    /// </summary>
    public void SetUp()
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

        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Create folder
        Directory.CreateDirectory(TestConfig.InstallPhysicalPath);

        var installZip = string.Empty;

        switch (TestConfig.PackageLocation)
        {
            case "Local":
                {
                    installZip = Path.Combine(currentPath, TestConfig.LocalReleasePackageFile);
                }

                break;
            case "GitHub":
                {
                    installZip = Path.Combine(TestConfig.InstallPhysicalPath, "YAF-BIN.zip");

                    // download latest release
                    var webClient = new WebClient();
                    webClient.DownloadFile(TestConfig.ReleaseDownloadUrl, installZip);
                    webClient.Dispose();
                }

                break;
        }

        // Extract Install Package
        var fastZip = new FastZip();

        fastZip.ExtractZip(
            installZip, applicationPath, FastZip.Overwrite.Always, null, null, null, false);

        // Rename Web.config
        File.Copy(
            Path.Combine(applicationPath, "recommended.web.config"), Path.Combine(applicationPath, "web.config"));

        // Create Website in IIS
        IISManager.CreateIISApplication(TestConfig.TestApplicationName, applicationPath);

        // Setup Mail Config
        var xmlMailConfig = new XmlDocument();

        xmlMailConfig.Load(Path.Combine(applicationPath, "mail.config"));

        var xNetworkNode = xmlMailConfig.CreateNode(XmlNodeType.Element, "network", string.Empty);
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
            
        // Copy db.config
        File.Copy(Path.Combine(currentPath, @"..\..\..\testfiles\db.config"), Path.Combine(applicationPath, "db.config"), true);

        // Inject Custom.sql file
        File.Copy(Path.Combine(currentPath, @"..\..\..\testfiles\custom.sql"), Path.Combine(applicationPath, "install\\custom.sql"));

        Directory.CreateDirectory(Path.Combine(applicationPath, "App_Data"));

        // Setup DB
        DBManager.AttachDatabase(TestConfig.TestDatabase);
            
        if (TestConfig.UseTestMailServer)
        {
            // Launch Mail Server
            this.SmtpServer = SimpleSmtpServer.Start(TestConfig.TestMailPort.ToType<int>());
        }

        this.SetupWebsite();
    }

    /// <summary>
    /// Tears down.
    /// </summary>
    public void TearDown()
    {
        if (TestConfig.UseTestMailServer)
        {
            // Stop Mail Server
            this.SmtpServer?.Stop();
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

        this.ChromeDriver.Close();
    }

    /// <summary>
    /// Setups the Test website.
    /// </summary>
    private void SetupWebsite()
    {
        this.ChromeDriver = new ChromeDriver();

        this.ChromeDriver.Navigate().GoToUrl($"{TestConfig.TestForumUrl}install/default.aspx");

        Thread.Sleep(5000);

        // Enter Config Password
        this.ChromeDriver.FindElement(By.Id("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

        this.ChromeDriver.FindElement(By.Id("InstallWizard_StepNavigationTemplateContainerID_StepPreviousButton")).Click();

        this.ChromeDriver.FindElement(By.Id("InstallWizard_rblYAFDatabase_1")).Click();

        // Enter YAF Database Connection
        var serverInput = this.ChromeDriver.FindElement(By.Id("InstallWizard_Parameter1_Value"), 300);
        serverInput.Clear();
        serverInput.SendKeys(TestConfig.DatabaseServer);

        this.ChromeDriver.FindElement(By.Id("InstallWizard_Parameter2_Value")).SendKeys(TestConfig.TestDatabase);

        // Test Database Connection
        this.ChromeDriver.FindElement(By.Id("InstallWizard_btnTestDBConnection")).Click();

        Assert.IsTrue(this.ChromeDriver.PageSource.Contains("Connection Succeeded"), "Database Connection Is Wrong");

        this.ChromeDriver.FindElement(By.Id("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

        // Test Mail Setup
        this.ChromeDriver.FindElement(By.Id("InstallWizard_txtTestFromEmail")).SendKeys(TestConfig.TestForumMail);

        this.ChromeDriver.FindElement(By.Id("InstallWizard_txtTestToEmail")).SendKeys("receiver@there.com");

        this.ChromeDriver.FindElement(By.Id("InstallWizard_btnTestSmtp")).Click();

        Assert.IsTrue(
            this.ChromeDriver.PageSource.Contains("Mail Sent. Verify it's received at your entered email address."),
            "Mail Send Failed");

        if (TestConfig.UseTestMailServer)
        {
            var mail = this.SmtpServer.ReceivedEmail[0];

            Assert.AreEqual("receiver@there.com", mail.ToAddresses[0].ToString(), "Receiver does not match");
            Assert.IsTrue(mail.FromAddress.ToString().Contains(TestConfig.TestForumMail), "Sender does not match");
            Assert.AreEqual(
                "Test Email From Yet Another Forum.NET", mail.Headers["Subject"], "Subject does not match");

            Assert.AreEqual(
                "The email sending appears to be working from your YAF installation.",
                mail.MessageParts[0].BodyData,
                "Body does not match");
        }

        // Now continue to Initialize Database
        this.ChromeDriver.FindElement(By.Id("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

        // Initialize Database
        this.ChromeDriver.FindElement(By.Id("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

        Thread.Sleep(3000);

        Assert.IsTrue(this.ChromeDriver.PageSource.Contains("Create Board"));

        // Board Settings
        this.ChromeDriver.FindElement(By.Id("InstallWizard_TheForumName")).SendKeys(TestConfig.TestApplicationName);
        this.ChromeDriver.FindElement(By.Id("InstallWizard_ForumEmailAddress")).SendKeys(TestConfig.TestForumMail);

        // Admin User
        this.ChromeDriver.FindElement(By.Id("InstallWizard_UserName")).SendKeys(TestConfig.AdminUserName);
        this.ChromeDriver.FindElement(By.Id("InstallWizard_AdminEmail")).SendKeys(TestConfig.TestForumMail);
        this.ChromeDriver.FindElement(By.Id("InstallWizard_Password1")).SendKeys(TestConfig.AdminPassword);
        this.ChromeDriver.FindElement(By.Id("InstallWizard_Password2")).SendKeys(TestConfig.AdminPassword);
        this.ChromeDriver.FindElement(By.Id("InstallWizard_SecurityQuestion")).SendKeys(TestConfig.AdminPassword);
        this.ChromeDriver.FindElement(By.Id("InstallWizard_SecurityAnswer")).SendKeys(TestConfig.AdminPassword);

        this.ChromeDriver.FindElement(By.Id("InstallWizard_StepNavigationTemplateContainerID_StepNextButton")).Click();

        Thread.Sleep(3000);

        Assert.IsTrue(this.ChromeDriver.PageSource.Contains("Setup Finished"));

        this.ChromeDriver.FindElement(By.Id("InstallWizard_FinishNavigationTemplateContainerID_FinishButton")).Click();

        Thread.Sleep(3000);

        Assert.IsTrue(this.ChromeDriver.PageSource.Contains("Welcome Guest!"), "Installation failed");
    }
}