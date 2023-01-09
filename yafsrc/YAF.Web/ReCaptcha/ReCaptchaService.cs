/* MIT License

 * Copyright (c) 2020 Michael
 * https://github.com/michaelvs97/AspNetCore.ReCaptcha

 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace YAF.Web.ReCaptcha;

using System.Net.Http;
using System.Text.Json;

using Microsoft.Extensions.Options;

/// <summary>
/// Class ReCaptchaService.
/// Implements the <see cref="YAF.Types.Interfaces.IReCaptchaService" />
/// </summary>
/// <seealso cref="YAF.Types.Interfaces.IReCaptchaService" />
internal class ReCaptchaService : IReCaptchaService
{
    private readonly HttpClient client;

    private readonly ReCaptchaSettings reCaptchaSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReCaptchaService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="reCaptchaSettings">The re captcha settings.</param>
    public ReCaptchaService(HttpClient httpClient, IOptions<ReCaptchaSettings> reCaptchaSettings)
    {
        this.client = httpClient;
        this.reCaptchaSettings = reCaptchaSettings.Value;
    }

    /// <inheritdoc />
    public async Task<bool> VerifyAsync(string reCaptchaResponse)
    {
        var obj = await this.GetVerifyResponseAsync(reCaptchaResponse);

        /*if (this._reCaptchaSettings.Version == ReCaptchaVersion.V3)
        {
            return obj.Success && obj.Score >= this._reCaptchaSettings.ScoreThreshold;
        }*/

        return obj.Success;
    }

    /// <inheritdoc />
    public async Task<ReCaptchaResponse> GetVerifyResponseAsync(string reCaptchaResponse)
    {
        var body = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                 {
                                                     ["secret"] = BoardContext.Current.Get<BoardSettings>().RecaptchaPrivateKey,
                                                     ["response"] = reCaptchaResponse
                                                 });

        var url =
            $"https://{(!this.reCaptchaSettings.UseRecaptchaNet ? "www.google.com/recaptcha" : "www.recaptcha.net")}/api/siteverify";

        var result = await this.client.PostAsync(url, body);

        var stringResult = await result.Content.ReadAsStringAsync();

        var obj = JsonSerializer.Deserialize<ReCaptchaResponse>(stringResult);

        return obj;
    }
}