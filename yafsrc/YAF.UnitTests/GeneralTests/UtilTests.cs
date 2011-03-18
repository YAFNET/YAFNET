/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.UnitTests.GeneralTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using YAF.Utils;

    /// <summary>
    /// The role membership tests.
    /// </summary>
    [TestClass]
    public class UtilTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Simple URL Parameter Parser Test
        /// </summary>
        [TestMethod]
        public void SimpleURLParameterParserTest()
        {
            var parser = new SimpleURLParameterParser("g=forum&t=1&url=&pg=43#cool");

            Assert.AreEqual(true, parser.HasAnchor, "Url should have an Anchor");
            Assert.AreEqual("cool", parser.Anchor, "The Url should be called : cool");
            Assert.AreEqual("forum", parser.Parameters["g"], "The Paramer g should equals forum");
            Assert.AreEqual("url=&pg=43", parser.CreateQueryString(new[] { "g", "t" }), "The Paramert g should equals forum");
            Assert.AreEqual("43", parser.Parameters[3], "The Parameter Index 3 should be 43");
        }
    }
}
