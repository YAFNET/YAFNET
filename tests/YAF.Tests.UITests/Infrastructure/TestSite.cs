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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.UITests.Infrastructure;

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

/// <summary>
/// The forum for the UI tests: a copy of the built site (YetAnotherForum.NET, built with YAF.NET-SqlServer.slnx)
/// with its own LocalDB database, run by IIS Express. The mails are written as files into the mail folder.
/// </summary>
public sealed class TestSite : IDisposable
{
    /// <summary>
    /// The folders of the site that are not copied.
    /// </summary>
    private static readonly string[] ExcludedFolders = ["node_modules", "obj", ".vs", "App_Data"];

    private Process iisExpress;

    private TestSite(string siteFolder, string mailFolder)
    {
        this.SiteFolder = siteFolder;
        this.MailFolder = mailFolder;
    }

    /// <summary>
    /// Gets the folder of the site copy.
    /// </summary>
    public string SiteFolder { get; }

    /// <summary>
    /// Gets the folder the mails of the forum are written to.
    /// </summary>
    public string MailFolder { get; }

    /// <summary>
    /// Gets the connection string of the test database.
    /// </summary>
    public static string ConnectionString =>
        $@"Data Source={TestConfig.LocalDbInstance};Initial Catalog={TestConfig.DatabaseName};Integrated Security=True";

    /// <summary>
    /// Copies the site, creates an empty database and starts IIS Express.
    /// </summary>
    /// <returns>Returns the started site.</returns>
    public static async Task<TestSite> StartAsync()
    {
        var source = FindSiteSource();

        if (!File.Exists(Path.Combine(source, "bin", "YAF.dll")))
        {
            throw new InvalidOperationException(
                $"The forum is not built ({source}\\bin\\YAF.dll is missing), build yafsrc\\YAF.NET-SqlServer.slnx first.");
        }

        var root = Path.Combine(Path.GetTempPath(), "yaf-uitests");
        var siteFolder = Path.Combine(root, "site");
        var mailFolder = Path.Combine(root, "mails");

        // an IIS Express of an aborted run still uses the site folder and the port
        StopLeftOverIisExpress(siteFolder);

        DeleteFolder(siteFolder);
        DeleteFolder(mailFolder);

        CopyFolder(source, siteFolder);
        Directory.CreateDirectory(mailFolder);
        Directory.CreateDirectory(Path.Combine(siteFolder, "App_Data"));

        PatchWebConfig(Path.Combine(siteFolder, "web.config"), mailFolder);

        RecreateDatabase();

        var site = new TestSite(siteFolder, mailFolder);

        site.StartIisExpress();

        await site.WaitUntilRunningAsync();

        return site;
    }

    /// <summary>
    /// Gets the mails sent by the forum (the newest first).
    /// </summary>
    /// <returns>Returns the mail files.</returns>
    public FileInfo[] GetMails()
    {
        return [.. new DirectoryInfo(this.MailFolder).GetFiles("*.eml").OrderByDescending(f => f.CreationTimeUtc)];
    }

    /// <summary>
    /// Stops IIS Express and removes the database and the site copy (unless they should be kept).
    /// </summary>
    public void Dispose()
    {
        try
        {
            if (this.iisExpress is { HasExited: false })
            {
                this.iisExpress.Kill();
                this.iisExpress.WaitForExit(10000);
            }
        }
        catch (InvalidOperationException)
        {
            // already stopped
        }

        this.iisExpress?.Dispose();

        if (TestConfig.Keep)
        {
            return;
        }

        try
        {
            DropDatabase();
        }
        catch (SqlException)
        {
            // the next run creates the database again
        }

        DeleteFolder(this.SiteFolder);
    }

    /// <summary>
    /// Finds the folder of the site (yafsrc\YetAnotherForum.NET) above the test folder.
    /// </summary>
    /// <returns>Returns the folder.</returns>
    private static string FindSiteSource()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "yafsrc", "YetAnotherForum.NET");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("The folder yafsrc\\YetAnotherForum.NET was not found.");
    }

    private static void StopLeftOverIisExpress(string siteFolder)
    {
        using var searcher = new System.Management.ManagementObjectSearcher(
            "SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'iisexpress.exe'");

        foreach (var process in searcher.Get())
        {
            var commandLine = process["CommandLine"]?.ToString() ?? string.Empty;

            if (commandLine.IndexOf(siteFolder, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            try
            {
                using var leftOver = Process.GetProcessById(Convert.ToInt32(process["ProcessId"]));
                leftOver.Kill();
                leftOver.WaitForExit(10000);
            }
            catch (ArgumentException)
            {
                // already stopped
            }
        }
    }

    private static void CopyFolder(string source, string target)
    {
        Directory.CreateDirectory(target);

        foreach (var file in Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true);
        }

        foreach (var folder in Directory.GetDirectories(source))
        {
            var name = Path.GetFileName(folder);

            if (ExcludedFolders.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            CopyFolder(folder, Path.Combine(target, name));
        }
    }

    private static void DeleteFolder(string folder)
    {
        for (var attempt = 0; attempt < 5 && Directory.Exists(folder); attempt++)
        {
            try
            {
                Directory.Delete(folder, true);
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(1000);
            }
        }
    }

    /// <summary>
    /// The site copy uses the test database, writes the mails into the mail folder and shows the errors.
    /// </summary>
    private static void PatchWebConfig(string webConfig, string mailFolder)
    {
        var document = XDocument.Load(webConfig);
        var configuration = document.Root!;

        var connectionStrings = configuration.Element("connectionStrings")!;
        connectionStrings.RemoveNodes();
        connectionStrings.Add(
            new XElement(
                "add",
                new XAttribute("name", "yafnet"),
                new XAttribute("connectionString", ConnectionString),
                new XAttribute("providerName", "System.Data.SqlClient")));

        var mailSettings = configuration.Element("system.net")?.Element("mailSettings");

        if (mailSettings == null)
        {
            mailSettings = new XElement("mailSettings");
            GetOrAdd(configuration, "system.net").Add(mailSettings);
        }

        mailSettings.RemoveNodes();
        mailSettings.Add(
            new XElement(
                "smtp",
                new XAttribute("deliveryMethod", "SpecifiedPickupDirectory"),
                new XAttribute("from", TestConfig.ForumEmail),
                new XElement("specifiedPickupDirectory", new XAttribute("pickupDirectoryLocation", mailFolder))));

        // the errors of the pages are shown (and written into the test output)
        var systemWeb = GetOrAdd(configuration, "system.web");
        var customErrors = GetOrAdd(systemWeb, "customErrors");
        customErrors.SetAttributeValue("mode", "Off");

        document.Save(webConfig);
    }

    private static XElement GetOrAdd(XElement parent, string name)
    {
        var element = parent.Element(name);

        if (element != null)
        {
            return element;
        }

        element = new XElement(name);
        parent.Add(element);

        return element;
    }

    private static void RecreateDatabase()
    {
        DropDatabase();

        ExecuteOnMaster($"CREATE DATABASE [{TestConfig.DatabaseName}]");
    }

    private static void DropDatabase()
    {
        ExecuteOnMaster(
            $"""
             IF DB_ID('{TestConfig.DatabaseName}') IS NOT NULL
             BEGIN
                 ALTER DATABASE [{TestConfig.DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                 DROP DATABASE [{TestConfig.DatabaseName}];
             END
             """);
    }

    private static void ExecuteOnMaster(string sql)
    {
        using var connection = new SqlConnection(
            $@"Data Source={TestConfig.LocalDbInstance};Initial Catalog=master;Integrated Security=True");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 120;
        command.ExecuteNonQuery();
    }

    private void StartIisExpress()
    {
        var iisExpressPath = new[]
                                 {
                                     Path.Combine(
                                         Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                         "IIS Express",
                                         "iisexpress.exe"),
                                     Path.Combine(
                                         Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                                         "IIS Express",
                                         "iisexpress.exe")
                                 }.FirstOrDefault(File.Exists)
                             ?? throw new FileNotFoundException("IIS Express (iisexpress.exe) is not installed.");

        this.iisExpress = new Process
                              {
                                  StartInfo = new ProcessStartInfo
                                                  {
                                                      FileName = iisExpressPath,
                                                      Arguments =
                                                          $"/path:\"{this.SiteFolder}\" /port:{TestConfig.Port} /clr:v4.0 /systray:false",
                                                      UseShellExecute = false,
                                                      CreateNoWindow = true,
                                                      RedirectStandardOutput = true,
                                                      RedirectStandardError = true
                                                  }
                              };

        this.iisExpress.OutputDataReceived += (_, e) => Log(e.Data);
        this.iisExpress.ErrorDataReceived += (_, e) => Log(e.Data);

        this.iisExpress.Start();
        this.iisExpress.BeginOutputReadLine();
        this.iisExpress.BeginErrorReadLine();
    }

    /// <summary>
    /// Waits until the site answers (the first request compiles the site).
    /// </summary>
    private async Task WaitUntilRunningAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };

        var until = DateTime.UtcNow.AddMinutes(5);

        while (DateTime.UtcNow < until)
        {
            if (this.iisExpress.HasExited)
            {
                throw new InvalidOperationException($"IIS Express stopped with the exit code {this.iisExpress.ExitCode}.");
            }

            try
            {
                using var response = await client.GetAsync($"{TestConfig.BaseUrl}install/default.aspx");

                // the site does not start (e.g. an assembly of the bin folder does not fit, a build was running)
                if ((int)response.StatusCode >= 500)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var title = System.Text.RegularExpressions.Regex.Match(content, "<title>(.*?)</title>").Groups[1].Value;

                    throw new InvalidOperationException(
                        $"The forum failed with {(int)response.StatusCode}: {System.Net.WebUtility.HtmlDecode(title)}");
                }

                return;
            }
            catch (HttpRequestException)
            {
                await Task.Delay(1000);
            }
        }

        throw new TimeoutException($"The forum did not start at {TestConfig.BaseUrl}.");
    }

    private static void Log(string line)
    {
        // only the messages of IIS Express, not every request
        if (!string.IsNullOrEmpty(line) && !line.Contains(TestConfig.BaseUrl))
        {
            TestContext.Progress.WriteLine($"[iisexpress] {line}");
        }
    }
}
