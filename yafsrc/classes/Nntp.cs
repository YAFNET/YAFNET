using System;
using System.Data;

namespace yaf.classes
{
	/// <summary>
	/// Summary description for Nntp.
	/// </summary>
	public class Nntp : System.Net.Sockets.TcpClient
	{
		public void Connect(string hostname) 
		{
			Connect(hostname,119);
			string response = ReadLine();
			if(response.Substring(0,3)!="200")
				throw new Exception(response);
		}

		public void Disconnect() 
		{
			Write("QUIT");
			Close();
		}

		private string ReadToEnd() 
		{
			string output = "";
			do 
			{
				string sLine = ReadLine();
				if(sLine==".")
					break;

				output += sLine + "\r\n";
			} while(true);
			return output;
		}

		private string ReadLine() 
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			byte []serverbuff = new Byte[1024];
			System.Net.Sockets.NetworkStream stream = GetStream();
			int count = 0;
			while (true)
			{
				byte []buff = new Byte[2];
				int bytes = stream.Read( buff, 0, 1 );
				if (bytes == 1)
				{
					if(buff[0]!='\r' && buff[0]!='\n')
						serverbuff[count++] = buff[0];

					if (buff[0] == '\n')
						break;
				}
				else
					break;
			};

			return System.Text.Encoding.ASCII.GetString(serverbuff,0,count);
		}

		private string Write(string input) 
		{
			byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(input + "\r\n");
			GetStream().Write(sendBytes,0,sendBytes.Length);
			return ReadLine();
		}

		static public void ReadArticles() 
		{
			int nUserID = 2;	// Guest - TODO

			Nntp nntp = null;
			string hostname = null;
			try 
			{
				// Only those not updated in the last 30 minutes
				using(DataTable dtForums = DB.nntpforum_list(30,null)) 
				{
					foreach(DataRow drForum in dtForums.Rows) 
					{
						if(hostname!=drForum["Address"].ToString().ToLower()) 
						{
							if(nntp!=null) 
							{
								nntp.Disconnect();
								nntp.Dispose(true);
							}
							nntp = new Nntp();
							hostname = drForum["Address"].ToString().ToLower();
							nntp.Connect(hostname);
						}

						string sLine = nntp.Write(String.Format("GROUP {0}",drForum["GroupName"]));
						if(sLine.Substring(0,3)!="211")
							throw new Exception(sLine);

						string[] buffer = sLine.Split();

						int nLastMessage = int.Parse(buffer[3]);
						//Console.WriteLine(String.Format("Last message: {0}",nLastMessage));
						int nCurrentMessage = (int)drForum["LastMessageNo"];
						if((nCurrentMessage==0) || (nLastMessage - 500 > nCurrentMessage))
							nCurrentMessage = nLastMessage - 500;

						nCurrentMessage++;

						int nForumID	= (int)drForum["ForumID"];
						int nCount		= 0;

						for(;nCurrentMessage<=nLastMessage;nCurrentMessage++) 
						{
							sLine = nntp.Write(String.Format("ARTICLE {0}",nCurrentMessage));
							if(sLine.Substring(0,3)!="220")
							{
								//throw new Exception(sLine);
								continue;
							}

							string article  = nntp.ReadToEnd();
							string sThread	= "";
							string sSubject	= "nntp subject";
							string sFrom	= "nntp from";

							int pos = article.IndexOf("\r\n\r\n");
							if(pos<0) continue;

							buffer = System.Text.RegularExpressions.Regex.Split(article, "\r\n\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
							string sHead	= article.Substring(0,pos);
							//string sBody	= System.Web.HttpContext.Current.Server.HtmlEncode(article);
							string sBody	= System.Web.HttpContext.Current.Server.HtmlEncode(article.Substring(pos+4));
							string sMsgID	= "";
							string sDate	= "";

							string[] headers = System.Text.RegularExpressions.Regex.Split(sHead, "\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
							foreach(string sHeader in headers) 
							{
								if(sHeader.ToLower().StartsWith("subject: ")) 
									sSubject = sHeader.Substring(9);
								else if(sHeader.ToLower().StartsWith("message-id: "))
									sMsgID = sHeader.Substring(12).ToLower().Trim();
								else if(sHeader.ToLower().StartsWith("references: "))
									sThread = sHeader.Substring(12).ToLower().Trim();
								else if(sHeader.ToLower().StartsWith("from: "))
									sFrom = sHeader.Substring(6).Trim();
								else if(sHeader.ToLower().StartsWith("date: "))
									sDate = sHeader.Substring(6).Trim();
							}
							if(sThread=="")
								sThread = sMsgID;

							pos = sThread.IndexOf('>');
							if(pos>0) sThread = sThread.Substring(0,pos+1);
							//sBody = "Thread: *" + System.Web.HttpContext.Current.Server.HtmlEncode(sThread) + "*\r\n\r\n" + sBody;
							sBody = String.Format("Date: {0}\r\n\r\n",sDate) + sBody;
							sBody = String.Format("From: {0}\r\n",System.Web.HttpContext.Current.Server.HtmlEncode(sFrom)) + sBody;
							sThread = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sThread,"md5");

							DataTable dt = DB.nntptopic_list(sThread);
							if(dt.Rows.Count>0) 
							{
								// insert new message
								long nMessageID = 0;
								DB.message_save(dt.Rows[0]["TopicID"],nUserID,sBody,sFrom,System.Web.HttpContext.Current.Request.UserHostAddress,ref nMessageID);
							} 
							else 
							{
								// insert new topic
								long nMessageID = 0;
								long nTopicID = DB.topic_save(nForumID,sSubject,sBody,nUserID,0,null,sFrom,System.Web.HttpContext.Current.Request.UserHostAddress,ref nMessageID);
								DB.nntptopic_save(drForum["NntpForumID"],sThread,nTopicID);
							}
							if(++nCount>10) break;
						}
						DB.nntpforum_update(drForum["NntpForumID"],nCurrentMessage);
					}
				}
			}
			finally 
			{
				if(nntp!=null) 
				{
					nntp.Disconnect();
					nntp.Dispose(true);
				}
			}
		}
	}
}
