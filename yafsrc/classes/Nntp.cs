using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace yaf.classes
{
	public class NntpException : Exception 
	{
		public NntpException(string message) : base(message) 
		{
		}
	}

	public class Nntp : System.Net.Sockets.TcpClient
	{
		public void Connect(string hostname) 
		{
			Connect(hostname,119);
			string response = ReadLine();
			if(response.Substring(0,3)!="200")
				throw new NntpException(response);
		}

		public void Disconnect() 
		{
			Write("QUIT");
			Close();
		}

		private string ReadLine() 
		{
			return ReadLine(DefaultEncoding);
		}

		static private System.Text.Encoding DefaultEncoding 
		{
			get 
			{
				//return System.Text.Encoding.ASCII;
				return System.Text.Encoding.GetEncoding("ISO-8859-1");
			}
		}

		private string ReadLine(System.Text.Encoding enc) 
		{
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

			string sLine = enc.GetString(serverbuff,0,count);
			Console.WriteLine(sLine);
			return sLine;
		}
		private static string GetHeader(string headers,string tofind) 
		{
			Match m = Regex.Match(headers,"^"+tofind,RegexOptions.IgnoreCase|RegexOptions.Multiline);
			if(m.Success) 
			{
				int pos = m.Index + 14;
				for(;pos<headers.Length;pos++) 
				{
					if(headers[pos]=='\n' && headers[pos+1]!=' ' && headers[pos+1]!='\t')
						break;

				}
				string res = headers.Substring(m.Index,pos-m.Index);
				res = res.Replace("\r\n","").Substring(tofind.Length+1).Trim();

				// Decode inline QP
				string re = @"\=\?(.*?)\?([bq])\?(.*?)\?\=";
				MatchCollection m2 = Regex.Matches(res,re,RegexOptions.IgnoreCase);
				if(m2.Count>0) 
				{
					for(int i=m2.Count-1;i>=0;i--) 
					{
						Encoding	enc;
						string		charset	= m2[i].Groups[1].Value;
						string		enctype	= m2[i].Groups[2].Value;
						string		text	= m2[i].Groups[3].Value;

						if(enctype.ToLower()!="q")
							throw new NntpException("Unsupported inline QP: " + enctype);

						try 
						{
							enc = Encoding.GetEncoding(charset);
						}
						catch(Exception) 
						{
							enc = DefaultEncoding;
						}

						byte[] tmp = DefaultEncoding.GetBytes(text);
						byte[] newstr = new byte[tmp.Length];
						int count = 0;
						for(int j=0;j<tmp.Length;j++) 
						{
							if(tmp[j]=='=') 
							{
								int ch = int.Parse(text.Substring(j+1,2),System.Globalization.NumberStyles.HexNumber);
								newstr[count++] = (byte)ch;
								j += 2;
							}
							else if(tmp[j]=='_') 
							{
								newstr[count++] = (byte)' ';
							}
							else 
							{
								newstr[count++] = tmp[j];
							}
						}
						res = enc.GetString(newstr,0,count);
					}
				}
				return res;
			}
			return null;
		}

		public string ReadArticle() 
		{
			StringBuilder article = new StringBuilder(10000);

			Encoding enc = DefaultEncoding;
			bool bInHeader = true;
			while(true) 
			{
				string line = ReadLine(enc);
				if(line==".")
					break;

				if(bInHeader) 
				{
					if(line.Length==0) 
					{
						// Finished getting header, see if we can find body encoding
						bInHeader = false;
						string headers = article.ToString();
					
						string s = GetHeader(headers,"Content-Type:");
						if(s!=null) 
						{
							string[] parts = s.Split(';');
							foreach(string part in parts) 
							{
								s = part.ToLower().Trim();
								if(s.StartsWith("charset=")) 
								{
									try 
									{
										enc = Encoding.GetEncoding(s.Substring(8).Trim());
									}
									catch(Exception) 
									{
										enc = DefaultEncoding;
									}
								}
							}
						}
					}
				}

				article.Append(line);
				article.Append("\r\n");
			}
			return article.ToString();
		}

		public string Write(string input) 
		{
			byte[] sendBytes = DefaultEncoding.GetBytes(input + "\r\n");
			GetStream().Write(sendBytes,0,sendBytes.Length);
			return ReadLine();
		}

		#region Classes
		public class GroupInfo 
		{
			#region Internals
			private	string	m_sName;
			private int		m_nNum;
			private int		m_nLow;
			private int		m_nHi;

			public GroupInfo(string response) 
			{
				string[] buffer = response.Split();
				m_nNum	= int.Parse(buffer[1]);
				m_nLow	= int.Parse(buffer[2]);
				m_nHi	= int.Parse(buffer[3]);
				m_sName	= buffer[4];
			}
			#endregion

			#region Public Properties
			public string Name 
			{
				get 
				{
					return m_sName;
				}
			}
			public int Count
			{
				get 
				{
					return m_nNum;
				}
			}
			public int First
			{
				get 
				{
					return m_nLow;
				}
			}
			public int Last 
			{
				get 
				{
					return m_nHi;
				}
			}
			#endregion
		}

		public class ArticleInfo 
		{
			#region Internals
			private	string	m_sHeader, m_sBody;
			private string	m_sFrom, m_sSubject, m_sThread, m_sDate;
			
			public ArticleInfo(string body) 
			{
				int pos = body.IndexOf("\r\n\r\n");
				if(pos<0)
					throw new Exception("Header and body not separated.");
				
				m_sHeader	= body.Substring(0,pos+2);
				m_sBody		= body.Substring(pos+4);
				m_sFrom		= GetHeader(m_sHeader,"From:");
				m_sSubject	= GetHeader(m_sHeader,"Subject:");
				m_sDate		= GetHeader(m_sHeader,"Date:");
				m_sThread	= GetHeader(m_sHeader,"References:");
				if(m_sThread==null)
					m_sThread	= GetHeader(m_sHeader,"Message-ID:");

				pos = m_sThread.IndexOf('>');
				m_sThread = m_sThread.Substring(0,pos+1).Trim().ToLower();
				m_sThread = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(m_sThread,"md5");
			}
			#endregion

			#region Public properties
			public string From
			{
				get 
				{
					return m_sFrom;
				}
			}
			public string FromName 
			{
				get 
				{
					string name = m_sFrom;
					int pos1, pos2;
					// Name <email>
					pos1 = name.IndexOf('<');
					pos2 = name.IndexOf('>');
					if(pos1>0 && pos2>pos1) 
					{
						name = name.Substring(0,pos1-1).Trim();
						if(name[0]=='"' && name[name.Length-1]=='"')
							name = name.Substring(1,name.Length-2);
						return name;
					}
					// email (Name)
					pos1 = name.IndexOf('(');
					pos2 = name.IndexOf(')');
					if(pos1>0 && pos2>pos1) 
					{
						name = name.Substring(pos1+1,pos2-pos1-2).Trim();
						return name;
					}
					return name;
				}
			}
			public string Subject 
			{
				get 
				{
					return m_sSubject;
				}
			}
			public string Body
			{
				get 
				{
					return m_sBody;
				}
			}
			public string Thread 
			{
				get 
				{
					return m_sThread;
				}
			}
			public string Date 
			{
				get 
				{
					return m_sDate;
				}
			}
			#endregion
		}
		#endregion

		public GroupInfo Group(string group) 
		{
			string response = Write("GROUP " + group);
			if(!response.StartsWith("211")) 
				throw new NntpException(response);
				
			return new GroupInfo(response);
		}
		public ArticleInfo Article(int article) 
		{
			string response = Write("ARTICLE " + article.ToString());
			if(!response.StartsWith("220"))
				throw new NntpException(response);

			return new ArticleInfo(ReadArticle());
		}

		static public void ReadArticles() 
		{
			int nUserID = DB.user_guest();	// Use guests user-id

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

						GroupInfo group = nntp.Group(drForum["GroupName"].ToString());
						int nLastMessageNo = (int)drForum["LastMessageNo"];
						int nCurrentMessage = nLastMessageNo;
						if(nCurrentMessage==0)
							nCurrentMessage = group.Last - 500;

						nCurrentMessage++;

						int nForumID	= (int)drForum["ForumID"];
						int nCount		= 0;

						for(;nCurrentMessage<group.Last;nCurrentMessage++) 
						{
							try 
							{
								ArticleInfo article = nntp.Article(nCurrentMessage);

								string	sBody		= article.Body;
								string	sThread		= article.Thread;
								string	sSubject	= article.Subject;
								string	sFrom		= article.FromName;
								string	sDate		= article.Date;
							
								sBody = String.Format("Date: {0}\r\n\r\n",sDate) + sBody;

								sBody = System.Web.HttpContext.Current.Server.HtmlEncode(sBody);
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
								nLastMessageNo = nCurrentMessage;
								// We don't wanna retrieve articles forever...
								if(++nCount>25) break;
							}
							catch(NntpException) 
							{
							}
						}
						DB.nntpforum_update(drForum["NntpForumID"],nLastMessageNo);
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
