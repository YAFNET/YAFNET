/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utilities
{
    #region Using

    using System;

    using CookComputing.XmlRpc;

    using YAF.Types;

    #endregion

    /// <summary>
    /// The meta weblog.
    /// </summary>
    public class MetaWeblog : XmlRpcClientProtocol
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaWeblog"/> class.
        /// </summary>
        /// <param name="metaWeblogServiceUrl">
        /// The meta weblog service url.
        /// </param>
        public MetaWeblog([NotNull] string metaWeblogServiceUrl)
        {
            this.Url = metaWeblogServiceUrl;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes a post from the blog.
        /// </summary>
        /// <param name="appKey">
        /// This value is ignored.
        /// </param>
        /// <param name="postid">
        /// The ID of the post to update.
        /// </param>
        /// <param name="username">
        /// Username to login to the blog
        /// </param>
        /// <param name="password">
        /// Password to login to the blog
        /// </param>
        /// <param name="publish">
        /// This value is ignored.
        /// </param>
        /// <returns>
        /// Always returns true.
        /// </returns>
        [XmlRpcMethod("blogger.deletePost")]
        public bool deletePost(
            [NotNull] string appKey,
            [NotNull] string postid,
            [NotNull] string username,
            [NotNull] string password,
            bool publish)
        {
            throw new Exception("Not yet implemented");

            // return (bool)this.Invoke("deletePost", new object[] { appKey, postid, username, password, publish });
        }

        /// <summary>
        /// Edits an existing entry on a blog.
        /// </summary>
        /// <param name="postid">
        /// The ID of the post to update.
        /// </param>
        /// <param name="username">
        /// Username to login to the blog
        /// </param>
        /// <param name="password">
        /// Password to login to the blog
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="publish">
        /// If false, this is a draft post.
        /// </param>
        /// <returns>
        /// Always returns true.
        /// </returns>
        [XmlRpcMethod("metaWeblog.editPost")]
        public bool editPost(
            [NotNull] string postid,
            [NotNull] string username,
            [NotNull] string password,
            Post content,
            bool publish)
        {
            return (bool)this.Invoke("editPost", new object[] { postid, username, password, content, publish });
        }

        /// <summary>
        /// Edits an existing entry on a blog.
        /// </summary>
        /// <param name="postid">
        /// The ID of the post to update.
        /// </param>
        /// <param name="username">
        /// Username to login to the blog
        /// </param>
        /// <param name="password">
        /// Password to login to the blog
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void editPost(
            [NotNull] string postid,
            [NotNull] string username,
            [NotNull] string password,
            [NotNull] string subject,
            [NotNull] string message)
        {
            Post post = this.getPost(postid, username, password);
            post.title = subject;
            post.description = message;
            this.editPost(postid, username, password, post, true);
        }

        /// <summary>
        /// Get a specific entry from the blog.
        /// </summary>
        /// <param name="postid">
        /// The ID of the post to get. 
        /// </param>
        /// <param name="username">
        /// Username to login to the blog
        /// </param>
        /// <param name="password">
        /// Password to login to the blog
        /// </param>
        /// <returns>
        /// Returns a specific entry from a blog.
        /// </returns>
        [XmlRpcMethod("metaWeblog.getPost")]
        public Post getPost([NotNull] string postid, [NotNull] string username, [NotNull] string password)
        {
            return (Post)this.Invoke("getPost", new object[] { postid, username, password });
        }

        /// <summary>
        /// Posts a new entry to a blog.
        /// </summary>
        /// <param name="blogid">
        /// The blogid, not sure if this is needed.  I know subtext doesn't need it, but not sure of others.
        /// </param>
        /// <param name="username">
        /// The name the user uses to login
        /// </param>
        /// <param name="password">
        /// The user�s password.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="publish">
        /// If false, this is a draft post.
        /// </param>
        /// <returns>
        /// The postid of the newly-created post.
        /// </returns>
        [XmlRpcMethod("metaWeblog.newPost")]
        public string newPost(
            [NotNull] string blogid,
            [NotNull] string username,
            [NotNull] string password,
            Post content,
            bool publish)
        {
            // TODO: We'll most likely want to keep the returned postid with the message that's posted to the forum.
            // That way, if the user edits/deletes we can also make the appropriate change to their blog as well. See
            // editPost and deletePost method's below.
            return (string)this.Invoke("newPost", new object[] { blogid, username, password, content, publish });
        }

        /// <summary>
        /// Posts a new entry to a blog.
        /// </summary>
        /// <param name="blogid">
        /// The blogid, not sure if this is needed.  I know subtext doesn't need it, but not sure of others.
        /// </param>
        /// <param name="username">
        /// The name the user uses to login
        /// </param>
        /// <param name="password">
        /// The user�s password.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The postid of the newly-created post.
        /// </returns>
        [XmlRpcMethod("metaWeblog.newPost")]
        public string newPost(
            [NotNull] string blogid,
            [NotNull] string username,
            [NotNull] string password,
            [NotNull] string subject,
            [NotNull] string message)
        {
            var post = new Post { title = subject, description = message, dateCreated = DateTime.UtcNow };
            return this.newPost(blogid, username, password, post, true);
        }

        #endregion

        /// <summary>
        /// This struct represents the information about a category
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct Category
        {
            #region Constants and Fields

            /// <summary>
            ///   The description.
            /// </summary>
            public string description;

            /// <summary>
            ///   The title.
            /// </summary>
            public string title;

            #endregion
        }

        /// <summary>
        /// This struct represents the information about a post
        /// </summary>
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct Post
        {
            #region Constants and Fields

            /// <summary>
            ///   The categories.
            /// </summary>
            [XmlRpcMember("categories", Description = "Contains categories for the post.")]
            public string[] categories;

            /// <summary>
            ///   The date created.
            /// </summary>
            public DateTime dateCreated;

            /// <summary>
            ///   The description.
            /// </summary>
            [XmlRpcMissingMapping(MappingAction.Error)]
            [XmlRpcMember(Description = "Required when posting.")]
            public string description;

            /// <summary>
            ///   The link.
            /// </summary>
            public string link;

            /// <summary>
            ///   The permalink.
            /// </summary>
            public string permalink;

            /// <summary>
            ///   The postid.
            /// </summary>
            [XmlRpcMember(
                Description =
                    "Not required when posting. Depending on server may " + "be either string or integer. "
                    + "Use Convert.ToInt32(postid) to treat as integer or "
                    + "Convert.ToString(postid) to treat as string")]
            public object postid;

            /// <summary>
            ///   The title.
            /// </summary>
            [XmlRpcMissingMapping(MappingAction.Error)]
            [XmlRpcMember(Description = "Required when posting.")]
            public string title;

            /// <summary>
            ///   The userid.
            /// </summary>
            public string userid;

            #endregion
        }
    }
}