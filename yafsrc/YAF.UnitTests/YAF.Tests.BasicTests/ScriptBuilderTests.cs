/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
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

namespace YAF.Tests.BasicTests
{
    using Autofac;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using YAF.Core;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    ///  The script builder tests.
    /// </summary>
    [TestClass]
    public class ScriptBuilderTests : IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptBuilderTests"/> class.
        /// </summary>
        public ScriptBuilderTests()
        {
            GlobalContainer.Container.Resolve<IInjectServices>().Inject(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        [Inject]
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// The test script building assumptions.
        /// </summary>
        [TestMethod]
        [Description("The test script building assumptions.")]
        public void Test_ScriptBuilding_Assumptions()
        {
            var sb = this.Get<IScriptBuilder>();

            var str =
                sb.CreateStatement().AddSelectorFormat("'Blah{0}'", 10).Dot().AddCall("html", "donkey's").Dot().AddCall(
                    "click", sb.CreateFunction().AddCall("alert", "It's clicked!").End()).End().Build();

            Assert.AreEqual(
                @"jQuery('Blah10').html(""donkey\'s"").click(function(){{ alert(""It\'s clicked!"");{0} }});{0}".FormatWith("\r\n"),
                str,
                @"The Script should be : jQuery('Blah10').html(""donkey\'s"").click(function(){{ alert(""It\'s clicked!"");{0} }});{0}");
        }
    }
}
