/* MIT License

 * Copyright (c) 2020 Michael
 * https://github.com/michaelvs97/AspNetCore.ReCaptcha

 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace YAF.Web.TagHelpers;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Web.ReCaptcha;

[HtmlTargetElement("recaptcha")]
public class ReCaptchaTagHelper : TagHelper
{
    public string Text { get; set; } = "Submit";

    public string ClassName { get; set; } = string.Empty;

    public string Size { get; set; } = "normal";

    public string Theme { get; set; } = "light";

    public string Action { get; set; } = "homepage";

    public string Language { get; set; } = null;

    public string Id { get; set; } = "recaptcha";

    public string Badge { get; set; } = "bottomright";

    public string Callback { get; set; } = null;

    public string ErrorCallback { get; set; } = null;

    public string ExpiredCallback { get; set; } = null;

    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagMode = TagMode.StartTagAndEndTag;
        output.TagName = null;

        if (string.IsNullOrEmpty(this.Id))
            throw new ArgumentException("id can't be null");

        if (this.Id.ToLower() == "submit")
            throw new ArgumentException("id can't be named submit");

        this.Language ??= this.ViewContext.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture
            .TwoLetterISOLanguageName;

        var settings = BoardContext.Current.Get<BoardSettings>();

        var content = ReCaptchaGenerator.ReCaptchaV2(
            settings.RecaptchaPublicKey,
            this.Size,
            this.Theme,
            this.Language,
            this.Callback,
            this.ErrorCallback,
            this.ExpiredCallback);

        output.Content.AppendHtml(content);
    }
}