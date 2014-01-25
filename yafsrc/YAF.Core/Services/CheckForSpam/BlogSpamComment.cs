/* Based on BlogSpam.net API http://blogspamnetapi.codeplex.com/
 * 
 * The MIT License (MIT)
 * -------------------------------------
 * Copyright (c) 2011 Code Gecko
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace YAF.Core.Services.CheckForSpam
{
    using CookComputing.XmlRpc;

    /// <summary>
    /// The Blog Spam Comment.
    /// </summary>
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct BlogSpamComment
    {
        #region Properties

        /// <summary>
        /// Gets or sets agent.
        /// </summary>
        public string agent { get; set; }

        /// <summary>
        /// Gets or sets comment.
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember]
        public string comment { get; set; }

        /// <summary>
        /// Gets or sets email.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Gets or sets ip.
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Error)]
        [XmlRpcMember]
        public string ip { get; set; }

        /// <summary>
        /// Gets or sets link.
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets options.
        /// </summary>
        public string options { get; set; }

        /// <summary>
        /// Gets or sets site.
        /// </summary>
        public string site { get; set; }

        /// <summary>
        /// Gets or sets subject.
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        /// Gets or sets version.
        /// </summary>
        public string version { get; set; }

        #endregion
    }
}