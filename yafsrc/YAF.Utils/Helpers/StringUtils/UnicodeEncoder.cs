/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Utils.Helpers.StringUtils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Special Encoding class to handle Unicode Chars
    /// </summary>
    public class UnicodeEncoder
    {
        /// <summary>
        /// The HTML unicode's
        /// </summary>
        private readonly List<HtmlCode> htmlUniCodes = new List<HtmlCode>();

        /// <summary>
        /// The simple list of codes that are needed
        /// </summary>
        private readonly List<HtmlCode> htmlSimpleCodes = new List<HtmlCode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeEncoder"/> class.
        /// </summary>
        public UnicodeEncoder()
        {
            // All Unicode Chars
            this.htmlUniCodes.Add(new HtmlCode("&#192;", "À"));
            this.htmlUniCodes.Add(new HtmlCode("&#193;", "Á"));
            this.htmlUniCodes.Add(new HtmlCode("&#194;", "Â"));
            this.htmlUniCodes.Add(new HtmlCode("&#195;", "Ã"));
            this.htmlUniCodes.Add(new HtmlCode("&#196;", "Ä"));
            this.htmlUniCodes.Add(new HtmlCode("&#197;", "Å"));
            this.htmlUniCodes.Add(new HtmlCode("&#198;", "Æ"));
            this.htmlUniCodes.Add(new HtmlCode("&#199;", "Ç"));
            this.htmlUniCodes.Add(new HtmlCode("&#200;", "È"));
            this.htmlUniCodes.Add(new HtmlCode("&#201;", "É"));
            this.htmlUniCodes.Add(new HtmlCode("&#202;", "Ê"));
            this.htmlUniCodes.Add(new HtmlCode("&#203;", "Ë"));
            this.htmlUniCodes.Add(new HtmlCode("&#204;", "Ì"));
            this.htmlUniCodes.Add(new HtmlCode("&#205;", "Í"));
            this.htmlUniCodes.Add(new HtmlCode("&#206;", "Î"));
            this.htmlUniCodes.Add(new HtmlCode("&#207;", "Ï"));
            this.htmlUniCodes.Add(new HtmlCode("&#208;", "Ð"));
            this.htmlUniCodes.Add(new HtmlCode("&#209;", "Ñ"));
            this.htmlUniCodes.Add(new HtmlCode("&#210;", "Ò"));
            this.htmlUniCodes.Add(new HtmlCode("&#211;", "Ó"));
            this.htmlUniCodes.Add(new HtmlCode("&#212;", "Ô"));
            this.htmlUniCodes.Add(new HtmlCode("&#213;", "Õ"));
            this.htmlUniCodes.Add(new HtmlCode("&#214;", "Ö"));
            this.htmlUniCodes.Add(new HtmlCode("&#216;", "Ø"));
            this.htmlUniCodes.Add(new HtmlCode("&#217;", "Ù"));
            this.htmlUniCodes.Add(new HtmlCode("&#218;", "Ú"));
            this.htmlUniCodes.Add(new HtmlCode("&#219;", "Û"));
            this.htmlUniCodes.Add(new HtmlCode("&#220;", "Ü"));
            this.htmlUniCodes.Add(new HtmlCode("&#221;", "Ý"));
            this.htmlUniCodes.Add(new HtmlCode("&#222;", "Þ"));
            this.htmlUniCodes.Add(new HtmlCode("&#223;", "ß"));
            this.htmlUniCodes.Add(new HtmlCode("&#224;", "à"));
            this.htmlUniCodes.Add(new HtmlCode("&#225;", "á"));
            this.htmlUniCodes.Add(new HtmlCode("&#226;", "â"));
            this.htmlUniCodes.Add(new HtmlCode("&#227;", "ã"));
            this.htmlUniCodes.Add(new HtmlCode("&#228;", "ä"));
            this.htmlUniCodes.Add(new HtmlCode("&#229;", "å"));
            this.htmlUniCodes.Add(new HtmlCode("&#230;", "æ"));
            this.htmlUniCodes.Add(new HtmlCode("&#231;", "ç"));
            this.htmlUniCodes.Add(new HtmlCode("&#232;", "è"));
            this.htmlUniCodes.Add(new HtmlCode("&#233;", "é"));
            this.htmlUniCodes.Add(new HtmlCode("&#234;", "ê"));
            this.htmlUniCodes.Add(new HtmlCode("&#235;", "ë"));
            this.htmlUniCodes.Add(new HtmlCode("&#236;", "ì"));
            this.htmlUniCodes.Add(new HtmlCode("&#237;", "í"));
            this.htmlUniCodes.Add(new HtmlCode("&#238;", "î"));
            this.htmlUniCodes.Add(new HtmlCode("&#239;", "ï"));
            this.htmlUniCodes.Add(new HtmlCode("&#240;", "ð"));
            this.htmlUniCodes.Add(new HtmlCode("&#241;", "ñ"));
            this.htmlUniCodes.Add(new HtmlCode("&#242;", "ò"));
            this.htmlUniCodes.Add(new HtmlCode("&#243;", "ó"));
            this.htmlUniCodes.Add(new HtmlCode("&#244;", "ô"));
            this.htmlUniCodes.Add(new HtmlCode("&#245;", "õ"));
            this.htmlUniCodes.Add(new HtmlCode("&#246;", "ö"));
            this.htmlUniCodes.Add(new HtmlCode("&#248;", "ø"));
            this.htmlUniCodes.Add(new HtmlCode("&#249;", "ù"));
            this.htmlUniCodes.Add(new HtmlCode("&#250;", "ú"));
            this.htmlUniCodes.Add(new HtmlCode("&#251;", "û"));
            this.htmlUniCodes.Add(new HtmlCode("&#252;", "ü"));
            this.htmlUniCodes.Add(new HtmlCode("&#253;", "ý"));
            this.htmlUniCodes.Add(new HtmlCode("&#254;", "þ"));
            this.htmlUniCodes.Add(new HtmlCode("&#255;", "ÿ"));

            // All relevant Chars to prevent XSS
            this.htmlSimpleCodes.Add(new HtmlCode("&quot;", "\""));
            this.htmlSimpleCodes.Add(new HtmlCode("&#39;", "\\"));
            this.htmlSimpleCodes.Add(new HtmlCode("&lt;", "<"));
            this.htmlSimpleCodes.Add(new HtmlCode("&gt;", ">"));
            this.htmlSimpleCodes.Add(new HtmlCode("&#255;", "ÿ"));
        }

        /// <summary>
        /// Encodes only the relevant Symbols to prevent XSS
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>Returns the Encoded string</returns>
        public string XSSEncode(string inputString)
        {
            return this.htmlSimpleCodes.Aggregate(
                inputString,
                (current, htmlCode) => current.Replace(htmlCode.Symbol, htmlCode.HTMLNumber));
        }

        /// <summary>
        /// Encodes the specified input string, and decodes Unicode chars
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>Returns the Encoded string, with the decodes unicode chars</returns>
        public string EncodeWithoutUnicode(string inputString)
        {
            inputString = WebUtility.HtmlEncode(inputString);

            return this.htmlUniCodes.Aggregate(
                inputString,
                (current, htmlUniCode) => current.Replace(htmlUniCode.HTMLNumber, htmlUniCode.Symbol));
        }

        /// <summary>
        /// The Html Code Class
        /// </summary>
        public class HtmlCode
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HtmlCode"/> class.
            /// </summary>
            /// <param name="htmlNumber">The HTML number.</param>
            /// <param name="symbol">The symbol.</param>
            public HtmlCode(string htmlNumber, string symbol)
            {
                this.HTMLNumber = htmlNumber;
                this.Symbol = symbol;
            }

            /// <summary>
            /// Gets or sets the symbol.
            /// </summary>
            /// <value>
            /// The symbol.
            /// </value>
            public string Symbol { get; set; }

            /// <summary>
            /// Gets or sets the HTML number.
            /// </summary>
            /// <value>
            /// The HTML number.
            /// </value>
            public string HTMLNumber { get; set; }
        }
    }
}
