/*!
 * netStack v1.0.14
 * A simple and easy jQuery plugin for highlighting .NET stack traces
 * License : Apache 2
 * Author : https://elmah.io
 */
(function($) {
    "use strict";

    $.fn.netStack = function(options) {

        function search(nameKey, myArray){
            for (var i=0; i < myArray.length; i++) {
                if (myArray[i].name === nameKey) {
                    return myArray[i];
                }
            }
        }

        function formatException(exceptionMessage, at_language){
            var result = exceptionMessage || "";
            var searchReplaces = [
                {
                    find: new RegExp(" "+at_language, "g"),
                    repl: "\r\n   "+at_language},
                {
                    find: new RegExp(" ---> ", "g"),
                    repl: "\r\n ---> "},
                {
                    find: new RegExp("\\) "+at_language+" ", "g"),
                    repl: "\r\n "+at_language+" "},
                {
                    find:/ --- End of inner exception stack trace ---/g,
                    repl: "\r\n   --- End of inner exception stack trace ---"
                }
            ]
            searchReplaces.forEach(function(item){
                result = result.replace(item.find, item.repl);
            });
            return result;
        };

        var settings = $.extend({

            // Default values for classes
            prettyprint: false,
            frame: "st-frame",
            type: "st-type",
            method: "st-method",
            paramsList: "st-frame-params",
            paramType: "st-param-type",
            paramName: "st-param-name",
            file: "st-file",
            line: "st-line"

        }, options);

        var languages = [
            { name: "english", at: "at", in: "in", line: "line" },
            { name: "danish", at: "ved", in: "i", line: "linje" },
            { name: "german", at: "bei", in: "in", line: "Zeile" }
        ];

        return this.each(function() {

            // Get the stacktrace, sanitize it, and split it into lines

            var stacktrace = $(this).text(),
                sanitizedStack = stacktrace.replace(/</g, "&lt;").replace(/>/g, "&gt;"),
                lines = sanitizedStack.split("\n"),
                lang = "",
                clone = "";

            // search for language
            for (var i = 0, j = lines.length; i < j; ++i) {
                if(lang === "") {
                    var line = lines[i];
                    var english = new RegExp("\\bat .*\\)"),
                        danish = new RegExp("\\bved .*\\)"),
                        german = new RegExp("\\bbei .*\\)");

                    if(english.test(lines[i])) {
                        lang = "english";
                    } else if (danish.test(lines[i])) {
                        lang = "danish";
                    } else if (german.test(lines[i])) {
                        lang = "german";
                    }
                }
            }

            if (lang === "") return;

            var selectedLanguage = search(lang, languages);

            // Pritty print result if is set to true
            if (settings.prettyprint) {
                sanitizedStack = formatException(sanitizedStack, selectedLanguage["at"]);
                lines = sanitizedStack.split("\n");
            }

            for (var i = 0, j = lines.length; i < j; ++i) {

                var li = lines[i],
                    hli = new RegExp("\\b"+selectedLanguage["at"]+" .*\\)");

                if (hli.test(lines[i])) {

                    // Frame
                    var regFrame = new RegExp("\\b"+selectedLanguage["at"]+" .*?\\)"),
                        partsFrame = String(regFrame.exec(lines[i]));
                    partsFrame = partsFrame.replace(selectedLanguage["at"]+" ", "");

                    // Frame -> ParameterList
                    var regParamList = new RegExp("\\(.*\\)"),
                        partsParamList = String(regParamList.exec(lines[i]));

                    // Frame -> Params
                    var partsParams = partsParamList.replace("(", "").replace(")", ""),
                        arrParams = partsParams.split(", "),
                        stringParam = "";

                    for (var x = 0, y = arrParams.length; x < y; ++x) {
                        var theParam = "",
                            param = arrParams[x].split(" "),
                            paramType = param[0],
                            paramName = param[1];

                        if (param[0] !== "null" && param[0] !== "") {
                            theParam = '<span class="' + settings.paramType + '">' + paramType + "</span>" + " " + '<span class="' + settings.paramName + '">' + paramName + "</span>";
                            stringParam += String(theParam) + ", ";
                        }
                    }

                    stringParam = stringParam.replace(/,\s*$/, "");
                    stringParam = '<span class="' + settings.paramsList + '">' + "(" + stringParam + ")" + "</span>";

                    // Frame -> Type & Method
                    var partsTypeMethod = partsFrame.replace(partsParamList, ""),
                        arrTypeMethod = partsTypeMethod.split("."),
                        method = arrTypeMethod.pop(),
                        type = partsTypeMethod.replace("." + method, ""),
                        stringTypeMethod = '<span class="' + settings.type + '">' + type + "</span>." + '<span class="' + settings.method + '">' + method + "</span>";

                    // Construct Frame
                    var newPartsFrame = partsFrame.replace(partsParamList, stringParam).replace(partsTypeMethod, stringTypeMethod);

                    // Line
                    var regLine = new RegExp("\\b:"+selectedLanguage["line"]+".*"),
                        partsLine = String(regLine.exec(lines[i]));
                    partsLine = partsLine.replace(":", "");

                    // File => (!) text requires multiline to exec regex, otherwise it will return null.
                    var regFile = new RegExp("\\b"+selectedLanguage["in"]+"\\s.*$", "m"),
                        partsFile = String(regFile.exec(lines[i]));
                    partsFile = partsFile.replace(selectedLanguage["in"]+" ", "").replace(":" + partsLine, "");

                    li = li.replace(partsFrame, '<span class="' + settings.frame + '">' + newPartsFrame + "</span>")
                        .replace(partsFile, '<span class="' + settings.file + '">' + partsFile + "</span>")
                        .replace(partsLine, '<span class="' + settings.line + '">' + partsLine + "</span>");

                    li = li.replace(/&lt;/g, "<span>&lt;</span>").replace(/&gt;/g, "<span>&gt;</span>");

                    if (lines.length - 1 == i) {
                        clone += li;
                    } else {
                        clone += li + "\n";
                    }
                } else {
                    if((lines[i].trim()).length) {
                        li = lines[i];

                        if (lines.length - 1 == i) {
                            clone += li;
                        } else {
                            clone += li + "\n";
                        }
                    }
                }
            }

            return $(this).html(clone);

        });

    };

}(jQuery));
