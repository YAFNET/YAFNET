using System;
using System.Web;
using System.Web.UI;

namespace yaf
{
	public enum Pages 
	{
		forum,
		topics,
		posts,
		profile,
		activeusers,
		moderate,
		postmessage,
		mod_forumuser,
		attachments,
		pmessage,
		movetopic,
		emailtopic,
		printtopic,
		members,
		cp_inbox,
		cp_profile,
		cp_editprofile,
		cp_signature,
		cp_subscriptions,
		cp_message,
		login,
		approve,
		info,
		rules,
		register,
		search,
		active,
		logout,
		moderate_index,
		moderate_forum,
		error
	}

	/// <summary>
	/// Summary description for Forum.
	/// </summary>
	public class Forum : System.Web.UI.UserControl
	{
		public Forum()
		{
			this.Load += new EventHandler(Forum_Load);
		}

		private void Forum_Load(object sender,EventArgs e) 
		{
			Pages page;
			string m_baseDir = Config.ConfigSection["root"];

			switch(Request.QueryString["g"])
			{
				default:
					//throw new ApplicationException(Request.QueryString["g"]);
				case "forum":
				case null:
					page = Pages.forum;
					break;
				case "moderate":
					page = Pages.moderate;
					break;
				case "topics":
					page = Pages.topics;
					break;
				case "posts":
					page = Pages.posts;
					break;
				case "movetopic":
					page = Pages.movetopic;
					break;
				case "profile":
					page = Pages.profile;
					break;
				case "cp_profile":
					page = Pages.cp_profile;
					break;
				case "cp_editprofile":
					page = Pages.cp_editprofile;
					break;
				case "postmessage":
					page = Pages.postmessage;
					break;
				case "activeusers":
					page = Pages.activeusers;
					break;
				case "attachments":
					page = Pages.attachments;
					break;
				case "approve":
					page = Pages.approve;
					break;
				case "rules":
					page = Pages.rules;
					break;
				case "register":
					page = Pages.register;
					break;
				case "logout":
					page = Pages.logout;
					break;
				case "login":
					page = Pages.login;
					break;
				case "members":
					page = Pages.members;
					break;
				case "printtopic":
					page = Pages.printtopic;
					break;
				case "pmessage":
					page = Pages.pmessage;
					break;
				case "cp_inbox":
					page = Pages.cp_inbox;
					break;
				case "cp_message":
					page = Pages.cp_message;
					break;
				case "emailtopic":
					page = Pages.emailtopic;
					break;
				case "cp_signature":
					page = Pages.cp_signature;
					break;
				case "cp_subscriptions":
					page = Pages.cp_subscriptions;
					break;
				case "active":
					page = Pages.active;
					break;
				case "mod_forumuser":
					page = Pages.mod_forumuser;
					break;
				case "info":
					page = Pages.info;
					break;
				case "moderate_index":
					page = Pages.moderate_index;
					break;
				case "moderate_forum":
					page = Pages.moderate_forum;
					break;
				case "search":
					page = Pages.search;
					break;
				case "error":
					page = Pages.error;
					break;
			}

			string src = string.Format("{0}pages/{1}.ascx",m_baseDir,page);
			if(src.IndexOf("/moderate_")>=0)
				src = src.Replace("/moderate_","/moderate/");

			try
			{
				pages.ForumPage ctl = (pages.ForumPage)LoadControl(src);
				ctl.ForumControl = this;
				this.Controls.Add(ctl);
			}
			catch(System.IO.FileNotFoundException)
			{
				throw new ApplicationException("Failed to load " + src + ".");
			}
		}

		private	yaf.Header	m_header	= null;
		private yaf.Footer	m_footer	= null;

		public yaf.Header Header
		{
			set
			{
				m_header = value;
			}
			get
			{
				return m_header;
			}
		}

		public yaf.Footer Footer
		{
			set
			{
				m_footer = value;
			}
			get
			{
				return m_footer;
			}
		}

		static public string GetLink(Pages page)
		{
			return Config.UrlBuilder.BuildUrl(string.Format("g={0}",page));
		}

		static public string GetLink(Pages page,string format,params object[] args)
		{
			return Config.UrlBuilder.BuildUrl(string.Format("g={0}&{1}",page,string.Format(format,args)));
		}

		static public void Redirect(Pages page)
		{
			System.Web.HttpContext.Current.Response.Redirect(GetLink(page));
		}

		static public void Redirect(Pages page,string format,params object[] args)
		{
			System.Web.HttpContext.Current.Response.Redirect(GetLink(page,format,args));
		}
	}
}
