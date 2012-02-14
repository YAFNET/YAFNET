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
    using System;
    using System.Web;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Xunit;

    using YAF.Core;
    using YAF.Types.Interfaces;

    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    /// <summary>
    /// The container tests.
    /// </summary>
    [TestClass]
    public class ContainerTests
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// The container is available from yaf context not in request.
        /// </summary>
        [TestMethod]
        [Description("The container is available from yaf context not in request.")]
        public void Container_Is_Available_From_YafContext_Not_In_Request()
        {
            var serviceLocator = YafContext.Current.ServiceLocator;

            Exception exception = Record.Exception(() => serviceLocator.Get<HttpRequestBase>());

            Assert.AreNotEqual(null, exception, "Exception should not be Null");
        }

        /// <summary>
        /// The container is available to send digest in background.
        /// </summary>
        [TestMethod]
        [Description("The container is available to send digest in background.")]
        public void Container_Is_Available_To_Send_Digest_In_Background()
        {
            var sendTask = new DigestSendTask();

            YafContext.Current.ServiceLocator.Get<IInjectServices>().Inject(sendTask);

            sendTask.RunOnce();
        }
    }
}