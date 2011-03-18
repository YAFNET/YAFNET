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
        /// The container_ is_ available_ from_ yaf context_ not_ in_ request.
        /// </summary>
        [TestMethod]
        public void Container_Is_Available_From_YafContext_Not_In_Request()
        {
            var serviceLocator = YafContext.Current.ServiceLocator;

            Exception exception = Record.Exception(() => serviceLocator.Get<HttpRequestBase>());

            Assert.AreNotEqual(null, exception, "Exception should not be Null");
        }

        /// <summary>
        /// The container_ is_ available_ to_ send_ digest_ in_ background.
        /// </summary>
        [TestMethod]
        public void Container_Is_Available_To_Send_Digest_In_Background()
        {
            var sendTask = new DigestSendTask();

            YafContext.Current.ServiceLocator.Get<IInjectServices>().Inject(sendTask);

            sendTask.RunOnce();
        }
    }
}