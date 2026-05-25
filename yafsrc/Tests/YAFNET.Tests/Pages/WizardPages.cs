/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Tests.Pages;

/// <summary>Step 1 – Welcome page.</summary>
public sealed class WelcomePage(IPage page) : BasePage(page)
{
    private ILocator WelcomeHeading  => this.Page.Locator("h4").First;

    public async Task NavigateAsync(string baseUrl)
    {
        await this.Page.GotoAsync($"{baseUrl}");
    }

    public async Task<string> GetWelcomeTextAsync()
    {
        return await this.WelcomeHeading.InnerTextAsync();
    }
}

/// <summary>Step 2 – Database test page.</summary>
public sealed class DatabasePage(IPage page) : BasePage(page)
{
    private ILocator FromEmail => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter the from Email Address" });

    private ILocator ToEmail => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter the to Email Address" });

    private ILocator TestSmtp => this.Page.Locator(".btn-info");

    public async Task FillMailTestingAsync(
        string forumEmail = "forum@example.com",
        string toEmail = "test@test.com")
    {
        await this.FromEmail.FillAsync(forumEmail);
        await this.ToEmail.FillAsync(toEmail);
    }

    public async Task TestSmtpAsync()
    {
        await this.TestSmtp.ClickAsync();
    }
}

/// <summary>Step 3 – Initialize database page.</summary>
public sealed class InitDatabasePage(IPage page) : BasePage(page);

/// <summary>Step 4 – Create board page.</summary>
public sealed class CreateBoardPage(IPage page) : BasePage(page)
{
    private ILocator WelcomeHeading => this.Page.Locator("h4").First;
    private ILocator BoardNameInput => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter the name of the new" });
    private ILocator BoardEmailInput => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Forum Email Address" });
    private ILocator SuperUserNameInput => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Enter the name of the super" });
    private ILocator EmailInput => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email Address", Exact = true });
    private ILocator PasswordInput => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Password", Exact = true });
    private ILocator ConfirmInput => this.Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Verify Password" });

    public async Task FillBoardDetailsAsync(
        string boardName = "Testboard created by Unit Test",
        string forumEmail = "forum@example.com",
        string username = "admin",
        string email = "admin@example.com",
        string password = "P@ssword123!")
    {
        await this.BoardNameInput.FillAsync(boardName);
        await this.BoardEmailInput.FillAsync(forumEmail);
        await this.SuperUserNameInput.FillAsync(username);
        await this.EmailInput.FillAsync(email);
        await this.PasswordInput.FillAsync(password);
        await this.ConfirmInput.FillAsync(password);
    }

    public async Task<string> GetWelcomeTextAsync()
    {
        return await this.WelcomeHeading.InnerTextAsync();
    }
}

/// <summary>Final page – installation complete.</summary>
public sealed class CompletePage(IPage page) : BasePage(page)
{
    private ILocator SuccessMessage => this.Page.Locator("h4").First;
    private ILocator GoToAppButton => this.Page.Locator(".btn-success");

    public async Task<string> GetSuccessMessageAsync()
    {
        return await this.SuccessMessage.InnerTextAsync();
    }

    public async Task ClickGoToAppAsync()
    {
        await this.GoToAppButton.ClickAsync();
    }
}
