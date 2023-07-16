/*!
 * .NET Port of jQuery Plugin netStack
 * License : Apache 2
 * Author : Ingo Herbote
 * Url: https://github.com/elmahio/netStack.js
 *
 *
 * Original 
 * A simple and easy jQuery plugin for highlighting .NET stack traces
 * License : Apache 2
 * Author : https://elmah.io
 * Url: https://github.com/elmahio/netStack.js
 *
 */

namespace YAF.Core.Utilities.StringUtils;

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// The stack trace beautify.
/// </summary>
public partial class StackTraceBeautify
{
    /// <summary>
    /// The pretty print.
    /// </summary>
    private readonly bool prettyPrint;

    /// <summary>
    /// The frame CSS class.
    /// </summary>
    private readonly string frameCssClass;

    /// <summary>
    /// The type CSS class.
    /// </summary>
    private readonly string typeCssClass;

    /// <summary>
    /// The method CSS class.
    /// </summary>
    private readonly string methodCssClass;

    /// <summary>
    /// The params list CSS class.
    /// </summary>
    private readonly string paramsListCssClass;

    /// <summary>
    /// The param type CSS class.
    /// </summary>
    private readonly string paramTypeCssClass;

    /// <summary>
    /// The param name CSS class.
    /// </summary>
    private readonly string paramNameCssClass;

    /// <summary>
    /// The file CSS class.
    /// </summary>
    private readonly string fileCssClass;

    /// <summary>
    /// The line CSS class.
    /// </summary>
    private readonly string lineCssClass;

    /// <summary>
    /// The languages.
    /// </summary>
    private readonly List<Language> languages;

    /// <summary>
    /// Initializes a new instance of the <see cref="StackTraceBeautify"/> class.
    /// </summary>
    public StackTraceBeautify()
    {
        this.prettyPrint = true;
        this.frameCssClass = "st-frame";
        this.typeCssClass = "st-type";
        this.methodCssClass = "st-method";
        this.paramsListCssClass = "st-frame-params";
        this.paramTypeCssClass = "st-param-type";
        this.paramNameCssClass = "st-param-name";
        this.fileCssClass = "st-file";
        this.lineCssClass = "st-line";

        this.languages = new List<Language>
                             {
                                 new () { Name = "english", At = "at", In = "in", Line = "line" },
                                 new () { Name = "danish", At = "ved", In = "i", Line = "linje" },
                                 new () { Name = "german", At = "bei", In = "in", Line = "Zeile" },
                                 new () { Name = "russian", At = "в", In = "в", Line = "строка" }
                             };
    }

    /// <summary>
    /// Get the Stack Trace, sanitize it, and split it into lines
    /// </summary>
    /// <param name="stackTrace">
    /// The stack trace.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string Beautify(string stackTrace)
    {
        var sanitizedStack = stackTrace.Replace("<", "&lt;").Replace(">", "&gt;");

        var lines = sanitizedStack.Split('\n');
        var lang = string.Empty;
        var clone = new StringBuilder();

        // search for language
        foreach (var line in lines)
        {
            if (lang != string.Empty)
            {
                continue;
            }

            if (EnglishRegex().IsMatch(line)) 
            {
                lang = "english";
            }
            else if (DanishRegex().IsMatch(line))
            {
                lang = "danish";
            }
            else if (GermanRegex().IsMatch(line))
            {
                lang = "german";
            }
            else if (RussianRegex().IsMatch(line))
            {
                lang = "russian";
            }
        }

        if (lang == string.Empty)
        {
            return stackTrace;
        }

        var selectedLanguage = Search(lang, this.languages);

        // Pretty print result if is set to true
        if (this.prettyPrint)
        {
            sanitizedStack = FormatException(sanitizedStack, selectedLanguage.At);
            lines = sanitizedStack.Split('\n');
        }

        for (int i = 0, j = lines.Length; i < j; ++i)
        {
            var line = lines[i];
            var li = line;

            var hli = new Regex($@"(\S*){selectedLanguage.At} .*\)");

            if (hli.IsMatch(line))
            {
                // Frame
                var regFrame = new Regex($@"(\S*){selectedLanguage.At} .*\)");
                var partsFrame = regFrame.Match(line).Value;

                partsFrame = partsFrame.Replace($"{selectedLanguage.At} ", string.Empty);

                // Frame -> ParameterList
                var regParamList = new Regex("\\(.*\\)");

                var partsParamList = regParamList.Match(line).Value;

                // Frame -> Params
                var partsParams = partsParamList.Replace("(", string.Empty).Replace(")", string.Empty);
                var arrParams = partsParams.Split(',');
                var parameterList = new StringBuilder();

                for (var index = 0; index < arrParams.Length; index++)
                {
                    var parameter = arrParams[index];
                    if (parameter.IsNotSet())
                    {
                        continue;
                    }

                    var cleanedParameter = parameter.TrimStart().Split(' ');

                    if (cleanedParameter[0].IsNotSet())
                    {
                        continue;
                    }

                    if (cleanedParameter.Length <= 1)
                    {
                        continue;
                    }

                    var paramType = cleanedParameter[0];
                    var paramName = cleanedParameter[1];

                    var theParam =
                        $"<span class=\"{this.paramTypeCssClass}\">{paramType}</span> <span class=\"{this.paramNameCssClass}\">{paramName}</span>";

                    parameterList.Append(index + 1 < arrParams.Length ? $"{theParam}, " : $"{theParam}");
                }

                var stringParamComplete = $"<span class=\"{this.paramsListCssClass}\">({parameterList})</span>";

                // Frame -> Type & Method
                var partsTypeMethod = partsFrame.Replace(partsParamList, string.Empty).Replace("\r", string.Empty);
                var arrTypeMethod = partsTypeMethod.Split('.').ToList();
                var method = arrTypeMethod.Last();

                var type = partsTypeMethod.Replace($".{method}", string.Empty);
                var stringTypeMethod =
                    $"<span class=\"{this.typeCssClass}\">{type}</span>.<span class=\"{this.methodCssClass}\">{method}</span>";

                // Construct Frame
                var newPartsFrame = partsFrame.Replace(partsParamList, stringParamComplete)
                    .Replace(partsTypeMethod, stringTypeMethod);

                // Line
                var regLine = new Regex($"(:{selectedLanguage.Line}.*)");

                var partsLine = regLine.Match(line).Value;
                partsLine = partsLine.Replace(":", string.Empty).Replace("\r", string.Empty);

                // File => (!) text requires multiline to exec regex, otherwise it will return null.
                var regFile = new Regex($"({selectedLanguage.In}\\s.*)", RegexOptions.Multiline);
                var partsFile = regFile.Match(line).Value;
                partsFile = partsFile.Replace($"{selectedLanguage.In} ", string.Empty)
                    .Replace($":{partsLine}", string.Empty);

                li = li.Replace(partsFrame, $"<span class=\"{this.frameCssClass}\">{newPartsFrame}</span>");

                if (partsFile.IsSet())
                {
                    li = li.Replace(partsFile, $"<span class=\"{this.fileCssClass}\">{partsFile}</span>");
                }

                if (partsLine.IsSet())
                {
                    li = li.Replace(partsLine, $"<span class=\"{this.lineCssClass}\">{partsLine}</span>");
                }

                li = li.Replace("&lt;", "<span>&lt;</span>").Replace("&gt;", "<span>&gt;</span>");

                clone.Append(lines.Length - 1 == i ? li : $"{li}");
            }
            else
            {
                if (line.Trim().IsNotSet())
                {
                    continue;
                }

                li = line;

                clone.Append(lines.Length - 1 == i ? li : $"{li}");
            }
        }

        return clone.ToString();
    }

    /// <summary>
    /// Get the Language
    /// </summary>
    /// <param name="languageName">
    /// The language name.
    /// </param>
    /// <param name="languages">
    /// The languages.
    /// </param>
    /// <returns>
    /// The <see cref="Language"/>.
    /// </returns>
    private static Language Search(string languageName, IReadOnlyCollection<Language> languages)
    {
        var language = languages.FirstOrDefault(x => x.Name == languageName);

        return language ?? languages.FirstOrDefault(x => x.Name == "english");
    }

    /// <summary>
    /// Format exception.
    /// </summary>
    /// <param name="exceptionMessage">
    /// The exception message.
    /// </param>
    /// <param name="languageAt">
    /// The language At.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private static string FormatException(string exceptionMessage, string languageAt)
    {
        var regex = new Regex(@"(-{3}\s)(.*?)(-{3})");
        var regex2 = new Regex($@"(\s){languageAt} ([^-:]*?)\((.*?)\)");

        var result = regex.IsMatch(exceptionMessage) ? regex.Replace(exceptionMessage, string.Empty) : exceptionMessage;

        if (regex2.IsMatch(result))
        {
            result = regex.Replace(result, string.Empty);
        }

        return result;
    }

    [GeneratedRegex("(\\s*)at .*\\)")]
    private static partial Regex EnglishRegex();

    [GeneratedRegex("(\\s*)ved .*\\)")]
    private static partial Regex DanishRegex();

    [GeneratedRegex("(\\s*)bei .*\\)")]
    private static partial Regex GermanRegex();

    [GeneratedRegex("(\\s*)в .*\\)")]
    private static partial Regex RussianRegex();
}

/// <summary>
/// The language.
/// </summary>
public class Language
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the at.
    /// </summary>
    public string At { get; set; }

    /// <summary>
    /// Gets or sets the in.
    /// </summary>
    public string In { get; set; }

    /// <summary>
    /// Gets or sets the line.
    /// </summary>
    public string Line { get; set; }
}