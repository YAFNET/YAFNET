/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
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
