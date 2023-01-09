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

/// <summary>
/// The re captcha generator.
/// </summary>
internal static class ReCaptchaGenerator
{
    /// <summary>
    /// The re captcha v 2.
    /// </summary>
    /// <param name="siteKey">
    /// The site key.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <param name="theme">
    /// The theme.
    /// </param>
    /// <param name="language">
    /// The language.
    /// </param>
    /// <param name="callback">
    /// The callback.
    /// </param>
    /// <param name="errorCallback">
    /// The error callback.
    /// </param>
    /// <param name="expiredCallback">
    /// The expired callback.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent ReCaptchaV2(string siteKey, string size, string theme, string language, string callback, string errorCallback, string expiredCallback)
    {
        var content = new HtmlContentBuilder();
        content.AppendFormat(@"<div class=""g-recaptcha"" data-sitekey=""{0}""", siteKey);

        if (!string.IsNullOrEmpty(size))
        {
            content.AppendFormat(@" data-size=""{0}""", size);
        }

        if (!string.IsNullOrEmpty(theme))
        {
            content.AppendFormat(@" data-theme=""{0}""", theme);
        }

        if (!string.IsNullOrEmpty(callback))
        {
            content.AppendFormat(@" data-callback=""{0}""", callback);
        }

        if (!string.IsNullOrEmpty(errorCallback))
        {
            content.AppendFormat(@" data-error-callback=""{0}""", errorCallback);
        }

        if (!string.IsNullOrEmpty(expiredCallback))
        {
            content.AppendFormat(@" data-expired-callback=""{0}""", expiredCallback);
        }

        content.AppendFormat("></div>");
        content.AppendLine();
        content.AppendFormat(@"<script src=""https://www.google.com/recaptcha/api.js?hl={0}"" defer></script>", language);

        return content;
    }
}