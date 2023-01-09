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

namespace YAF.Types.Interfaces;

using System.Threading.Tasks;

using YAF.Types.Objects;

/// <summary>
/// The ReCaptchaService interface.
/// </summary>
public interface IReCaptchaService
{
    /// <summary>
    /// Verifies provided ReCaptcha Response.
    /// </summary>
    /// <param name="reCaptchaResponse">ReCaptcha Response as given by the widget.</param>
    /// <returns>Returns whether the recaptcha validation was successful or not.</returns>
    Task<bool> VerifyAsync(string reCaptchaResponse);

    /// <summary>
    /// Verifies provided ReCaptcha Response.
    /// </summary>
    /// <param name="reCaptchaResponse">ReCaptcha Response as given by the widget.</param>
    /// <returns>Returns result of the verification of the ReCaptcha Response.</returns>
    Task<ReCaptchaResponse> GetVerifyResponseAsync(string reCaptchaResponse);
}