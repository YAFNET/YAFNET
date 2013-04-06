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

namespace YAF.Tests.CoreTests.Formatting
{
    using System;

    using Autofac;

    using NUnit.Framework;

    using YAF.Core;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces;

    /// <summary>
    ///  The Format Date Time tests.
    /// </summary>
    [TestFixture]
    public class FormatDateTimeTests : IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDateTimeTests"/> class.
        /// </summary>
        public FormatDateTimeTests()
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

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        /// <summary>
        /// Format date to long test.
        /// </summary>
        [Test]
        [Description("Format date to long test.")]
        public void Format_Date_To_Long_Test()
        {
            var currentDateTime = new DateTime(2012, 08, 19, 20, 20, 20);

            var dateTimeString = this.Get<IDateTime>().FormatDateLong(currentDateTime);

            Assert.AreEqual(
                "Sonntag, 19. August 2012(UTC)",
                dateTimeString,
                dateTimeString);
        }
    }
}