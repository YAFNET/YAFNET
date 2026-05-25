#nullable enable

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

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace YAF.Tests.Utils;

/// <summary>
/// Manages a single docker-compose stack lifecycle.
/// Instantiated per test (or per parameterized group) with a specific compose file,
/// so the same test suite can run against different database backends.
/// </summary>
public sealed class DockerComposeFixture
{
    /// <summary>Base URL Playwright will navigate to.</summary>
    public string AppBaseUrl { get; }

    private readonly static TimeSpan StartupTimeout = TimeSpan.FromSeconds(700);

    private readonly HttpClient http;

    private readonly string composeDirectory;
    private readonly string composeFile;

    /// <param name="baseUrl">URL Playwright will navigate to, e.g. "http://localhost:8001"</param>
    /// <param name="composeFile">
    ///   the docker compose file name.
    ///   Example: "docker-compose-SqlServer.yml"
    /// </param>
    /// <param name="composeDirectory">
    ///   Directory that contains the compose files.
    ///   Defaults to auto-discovery (walks up from the test assembly until it finds docker-compose.yml).
    /// </param>
    public DockerComposeFixture(
        string baseUrl,
        string composeFile,
        string? composeDirectory = null)
    {
        this.http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        this.AppBaseUrl = baseUrl;
        this.composeFile = composeFile;
        this.composeDirectory = composeDirectory
                                 ?? Environment.GetEnvironmentVariable("COMPOSE_DIR")
                                 ?? FindSolutionRoot(composeFile);
    }

    public async Task StartAsync()
    {
        await this.RunComposeAsync("up --build -d");
        await this.WaitForAppAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await this.RunComposeAsync("down -v --remove-orphans");
        this.http.Dispose();
    }

    private async Task WaitForAppAsync()
    {
        var deadline = DateTime.UtcNow + StartupTimeout;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await this.http.GetAsync(this.AppBaseUrl);
                if (response.IsSuccessStatusCode ||
                    (int)response.StatusCode is 301 or 302 or 307 or 308)
                {
                    return;
                }
            }
            catch
            {
                // Not ready yet
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        await this.RunComposeAsync("logs --no-color", captureOutput: true);
        throw new TimeoutException(
            $"App did not become reachable at {this.AppBaseUrl} within {StartupTimeout.TotalSeconds}s.");
    }

    private Task RunComposeAsync(string arguments, bool captureOutput = false)
    {
        try
        {
            TestContext.Progress.WriteLine("🧪 Start docker");

            var fullArgs = $"compose -f {this.composeFile} {arguments}";

            var psi = new ProcessStartInfo("docker", fullArgs)
            {
                WorkingDirectory = this.composeDirectory,
                RedirectStandardOutput = captureOutput,
                RedirectStandardError = captureOutput,
                UseShellExecute = false
            };

            var tcs = new TaskCompletionSource<bool>();

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            process.Exited += (_, _) =>
            {
                if (process.ExitCode == 0)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(new Exception(
                        $"'docker {fullArgs}' exited with code {process.ExitCode}"));
                }
            };

            process.Start();

            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    private static string FindSolutionRoot(string composeFile)
    {
        var dir = new DirectoryInfo(
            Path.GetDirectoryName(
                typeof(DockerComposeFixture).Assembly.Location)!);

        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, composeFile)))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate docker-compose.yml. Set the COMPOSE_DIR environment variable to the directory containing it.");
    }
}
