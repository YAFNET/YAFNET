using System;
using System.Collections.Generic;
using System.Text;
using CookComputing.XmlRpc;

namespace YAF.Classes.Data
{
	public class MetaWeblog : XmlRpcClientProtocol
	{
		public MetaWeblog(string metaWeblogServiceUrl)
		{
			this.Url = metaWeblogServiceUrl;
		}

		/// <summary>
		/// Posts a new entry to a blog.
		/// </summary>
		/// <param name="blogid">The blogid, not sure if this is needed.  I know subtext doesn't need it, but not sure of others.</param>
		/// <param name="username">The name the user uses to login</param>
		/// <param name="password">The user’s password.</param>
		/// <param name="content">The content.</param>
		/// <param name="publish">If false, this is a draft post.</param>
		/// <returns>The postid of the newly-created post.</returns>
		[XmlRpcMethod("metaWeblog.newPost")]
		public string newPost(string blogid, string username, string password, Post content, bool publish)
		{
			// TODO: We'll most likely want to keep the returned postid with the message that's posted to the forum.
			// That way, if the user edits/deletes we can also make the appropriate change to their blog as well. See
			// editPost and deletePost method's below.
			return (string)this.Invoke("newPost", new object[] { blogid, username, password, content, publish });
		}
		/// <summary>
		/// Posts a new entry to a blog.
		/// </summary>
		/// <param name="blogid">The blogid, not sure if this is needed.  I know subtext doesn't need it, but not sure of others.</param>
		/// <param name="username">The name the user uses to login</param>
		/// <param name="password">The user’s password.</param>
		/// <param name="title">The title.</param>
		/// <param name="description">The description.</param>
		/// <returns>The postid of the newly-created post.</returns>
		[XmlRpcMethod("metaWeblog.newPost")]
		public string newPost(string blogid, string username, string password, string title, string description)
		{
			Post post = new Post();
			post.title = title;
			post.description = description;
			post.dateCreated = DateTime.Now;
			return this.newPost(blogid, username, password, post, true);
		}

		#region Only implement these when we collect the blog's PostID within the YAF system
		/// <summary> 
		/// Edits an existing entry on a blog. 
		/// </summary> 
		/// <param name="postid">The ID of the post to update.</param> 
		/// <param name="username">Username to login to the blog</param> 
		/// <param name="password">Password to login to the blog</param> 
		/// <param name="post"> A struct representing the content to update. </param> 
		/// <param name="publish"> If false, this is a draft post.</param> 
		/// <returns>Always returns true.</returns> 
		[XmlRpcMethod("metaWeblog.editPost")]
		public bool editPost(string postid, string username, string password, Post content, bool publish)
		{
			throw new Exception("Not yet implemented");
			//return (bool)this.Invoke("editPost", new object[] { postid, username, password, content, publish });
		}

		/// <summary> 
		/// Deletes a post from the blog. 
		/// </summary> 
		/// <param name="appKey">This value is ignored.</param> 
		/// <param name="postid">The ID of the post to update.</param> 
		/// <param name="username">Username to login to the blog</param> 
		/// <param name="password">Password to login to the blog</param> 
		/// <param name="publish">This value is ignored.</param> 
		/// <returns>Always returns true.</returns> 
		[XmlRpcMethod("blogger.deletePost")]
		public bool deletePost(string appKey, string postid, string username, string password, bool publish)
		{
			throw new Exception("Not yet implemented");
			//return (bool)this.Invoke("deletePost", new object[] { appKey, postid, username, password, publish });
		}
		#endregion

		/// <summary> 
		/// This struct represents the information about a category
		/// </summary> 
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public struct Category
		{
			public string description;
			public string title;
		}

		/// <summary> 
		/// This struct represents the information about a post
		/// </summary> 
		[XmlRpcMissingMapping(MappingAction.Ignore)]
		public struct Post
		{
			[XmlRpcMissingMapping(MappingAction.Error)]
			[XmlRpcMember(Description = "Required when posting.")]
			public DateTime dateCreated;
			[XmlRpcMissingMapping(MappingAction.Error)]
			[XmlRpcMember(Description = "Required when posting.")]
			public string description;
			[XmlRpcMissingMapping(MappingAction.Error)]
			[XmlRpcMember(Description = "Required when posting.")]
			public string title;
			[XmlRpcMember("categories", Description = "Contains categories for the post.")]
			public string[] categories;
			public string link;
			public string permalink;
			[XmlRpcMember(
				Description = "Not required when posting. Depending on server may "
				+ "be either string or integer. "
				+ "Use Convert.ToInt32(postid) to treat as integer or "
				+ "Convert.ToString(postid) to treat as string")]
			public object postid;
			public string userid;
		}
	}
}
