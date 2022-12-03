/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Tests.Utils.Extensions;

using System.Linq;
using System.Threading;

using netDumbster.smtp;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

/// <summary>
/// Test Helper Class
/// </summary>
public static class RegisterLoginExtensions
{
    /// <summary>
    /// Log Current user Out
    /// </summary>
    /// <param name="driver">
    /// The driver.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool LogOutUser(this ChromeDriver driver)
    {
        var js = (IJavaScriptExecutor)driver;
        js.ExecuteScript("window.location.href = $(\"a[id*='_LogOutButton']\").attr('href');");

        Thread.Sleep(500);

        driver.FindElement(By.ClassName("bootbox-accept")).ClickAndWait();

        return driver.PageSource.Contains("LoginLink");
    }

    /// <summary>
    /// Registers the standard test user.
    /// </summary>
    /// <param name="driver">
    /// The <paramref name="driver"/> instance.
    /// </param>
    /// <param name="simpleSmtpServer">
    /// </param>
    /// <param name="userName">
    /// Name of the user.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// If User was Registered or not
    /// </returns>
    public static bool RegisterUser(
        this ChromeDriver driver,
        SimpleSmtpServer simpleSmtpServer,
        string userName,
        string password)
    {
        return RegisterUser(driver, simpleSmtpServer, userName, $"{userName.ToLower()}@test.com", password);
    }

    /// <summary>
    /// Logins the user.
    /// </summary>
    /// <param name="driver">The <paramref name="driver"/> instance.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="userPassword">The user password.</param>
    /// <returns>If User login was successfully or not</returns>
    public static bool LoginUser(this ChromeDriver driver, string userName, string userPassword)
    {
        // Check If User is already Logged In
        if (driver.ElementExists(By.XPath("//*[contains(@id, '_LogOutButton')]")))
        {
            driver.LogOutUser();
        }

        driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}Account/login");

        driver.FindElement(By.XPath("//*[contains(@id, '_UserName')]")).SendKeys(userName);
        driver.FindElement(By.XPath("//*[contains(@id, '_Password')]")).SendKeys(userPassword);

        driver.FindElement(By.XPath("//*[contains(@id, '_LoginButton')]")).ClickAndWait();

        return driver.ElementExists(By.XPath("//a[contains(@id,'LogOutButton')]"));
    }

    /// <summary>
    /// Registers the standard test user.
    /// </summary>
    /// <param name="driver">
    /// The <paramref name="driver"/> instance.
    /// </param>
    /// <param name="simpleSmtpServer">
    /// The simple Smtp Server.
    /// </param>
    /// <param name="userName">
    /// Name of the user.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// If User was Registered or not
    /// </returns>
    private static bool RegisterUser(this IWebDriver driver, SimpleSmtpServer simpleSmtpServer, string userName, string email, string password)
    {
        driver.Navigate().GoToUrl($"{TestConfig.TestForumUrl}Account/Register");

        // Check if Registrations are Disabled
        if (driver.PageSource.Contains("You tried to enter an area where you didn't have access"))
        {
            return false;
        }

        // Accept the Rules
        if (driver.PageSource.Contains("Forum Rules"))
        {
            driver.Navigate().Refresh();
        }

        if (driver.PageSource.Contains("Security Image") || driver.PageSource.Contains("Sicherheitscode"))
        {
            return false;
        }

        // Fill the Register Page
        driver.FindElement(By.XPath("//input[contains(@id,'_UserName')]")).SendKeys(
            userName);

        if (driver.PageSource.Contains("Display Name") || driver.PageSource.Contains("Anzeigename"))
        {
            driver.FindElement(By.XPath("//input[contains(@id,'_DisplayName')]")).SendKeys(userName);
        }

        driver.FindElement(By.XPath("//input[contains(@id,'_Password')]")).SendKeys(password);
        driver.FindElement(By.XPath("//input[contains(@id,'_ConfirmPassword')]")).SendKeys(password);
        driver.FindElement(By.XPath("//input[contains(@id,'_Email')]")).SendKeys(email);

        // Create User
        driver.FindElement(By.XPath("//a[contains(@id,'_CreateUser')]")).ClickAndWait();

        var confirmEmail = simpleSmtpServer.ReceivedEmail.FirstOrDefault();

        var confirmUrl = StringUtils.GetLinks(confirmEmail.MessageParts[0].BodyData).FirstOrDefault();

        driver.Navigate().GoToUrl(confirmUrl);

        return driver.ElementExists(By.XPath("//*[contains(@id, '_LogOutButton')]"));
    }
}