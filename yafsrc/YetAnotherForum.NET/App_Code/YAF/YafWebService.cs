using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

/// <summary>
/// Summary description for YafForumWebService
/// </summary>
[WebService( Namespace = "http://yetanotherforum.net/services" )]
[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
public class YafWebService : System.Web.Services.WebService
{
	public YafWebService()
	{
		//Uncomment the following line if using designed components 
		//InitializeComponent(); 
	}

	[WebMethod]
	public long CreateNewTopic(string token, int forumid, int userid, string username, string subject, string post, string ip, int priority, int flags )
	{
		// validate token...
		if ( token != YafContext.Current.BoardSettings.WebServiceToken )
			throw new Exception( "Invalid Secure Web Service Token: Operation Failed" );

		long messageId = 0;
		string subjectEncoded = Server.HtmlEncode(subject);

		return DB.topic_save(forumid, subjectEncoded, post, userid, priority, null,username, ip, null, null, flags, ref messageId);
	}
}

