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
		public void ConnectNntp(string hostname,int port) 
		{
			Connect(hostname,port);
			string response = ReadLine();
			if(response.Substring(0,3)!="200" && response.Substring(0,3)!="201")
				throw new NntpException(response);
		}

		public void Disconnect() 
		{
			if(Active)
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
			byte []buff = new Byte[2];
			while (true)
			{
				int bytes = stream.Read( buff, 0, 1 );
				if (bytes == 1)
				{
					if(buff[0]!='\r' && buff[0]!='\n')
						serverbuff[count++] = buff[0];

					if (buff[0] == '\n')
						break;

					if(count>1000 && buff[0]!='\r')
						break;
				}
				else
					break;
			};

			string sLine = enc.GetString(serverbuff,0,count);
#if DEBUG
			///Console.WriteLine(sLine);
#endif
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
					if(headers[pos]=='\n' && pos<headers.Length-1 && headers[pos+1]!=' ' && headers[pos+1]!='\t')
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
						if(name.Length>2 && name[0]=='"' && name[name.Length-1]=='"')
							name = name.Substring(1,name.Length-2);
						
						return name.Replace("\\","");
					}
					// email (Name)
					pos1 = name.IndexOf('(');
					pos2 = name.IndexOf(')');
					if(pos1>0 && pos2>pos1) 
					{
						name = name.Substring(pos1+1,pos2-pos1-1).Trim();
						return name.Replace("\\","");
					}
					return name.Replace("\\","");
				}
			}
			public string FromEmail
			{
				get 
				{
					string name = m_sFrom.Trim();
					int pos1, pos2;
					// Name <email>
					pos1 = name.IndexOf('<');
					pos2 = name.IndexOf('>');
					if(pos1>=0 && pos2>pos1) 
					{
						name = name.Substring(pos1+1,pos2-pos1-1).Trim();
						return name;
					}
					// email (Name)
					pos1 = name.IndexOf('(');
					pos2 = name.IndexOf(')');
					if(pos1>0 && pos2>pos1) 
					{
						name = name.Substring(0,pos1-1).Trim();
						return name;
					}
					if(Utils.IsValidEmail(name))
						return name;

					//throw new ApplicationException(string.Format("Wrong name format '{0}'",name));
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
			public string DateString
			{
				get 
				{
					return m_sDate;
				}
			}
			public DateTime Date 
			{
				get 
				{
					try
					{
						string sDate = DateString;
						if(sDate==null)
							return DateTime.Now;

						sDate = sDate.Trim();
						if(sDate.Length==32 || sDate.Length==29 || true) 
						{
							// Tue, 23 Sep 2003 13:21:00 -07:00 (32 bytes)
							// Tue, 23 Sep 2003 13:21:00 GMT (29 bytes)
							// Tue, 23 Sep 2003 3:21:00 GMT
							// Tue, 3 Sep 2003 13:21 GMT
							// Tue, 23 Sep 2003 13:21:00

							int offset = sDate.IndexOf(',') + 1;
							string[] s = sDate.Substring(offset).Trim().Split(' ');
							if(s.Length>=4) 
							{
								try 
								{
									int	day		= int.Parse(s[0]);
									int	month	= GetMonth(s[1]);
									int	year	= int.Parse(s[2]);
									string[] t = s[3].Split(':');
									if(t.Length<2) t = s[3].Split('.');
									int	hour	= int.Parse(t[0]);
									int	min		= int.Parse(t[1]);
									int	sec		= t.Length>2 ? int.Parse(t[2]) : 0;

									DateTime date = new DateTime(year,month,day,hour,min,sec);
									if(s.Length>4)
										date += GetTimeOffset(s[4]);
									return date;
								}
								catch(Exception x) 
								{
									throw new Exception(sDate,x);
								}
							}
						}
						throw new Exception(sDate);
					}
					catch(Exception) 
					{
						return DateTime.Now;
					}
				}
			}
			static public int GetMonth(string mon) 
			{
				string[] months = {"jan","feb","mar","apr","may","jun","jul","aug","sep","oct","nov","dec"};
				mon = mon.Trim().ToLower();
				for(int i=0;i<12;i++)
					if(months[i]==mon)
						return i+1;

				throw new Exception(mon);
			}
			static public TimeSpan GetTimeOffset(string timezone) 
			{
				timezone = timezone.Trim().ToLower();
				if(timezone.Length>=4 && (timezone[0]=='+' || timezone[0]=='-'))
				{
					int hour = 0;
					int min = 0;
					if(timezone.Length==5)
					{
						hour = int.Parse(timezone.Substring(0,3));
						min = int.Parse(timezone.Substring(3,2));
					} 
					else if(timezone.Length==4) 
					{
						try 
						{
							hour = int.Parse(timezone.Substring(0,2));
							min = int.Parse(timezone.Substring(2,2));
						}
						catch(Exception) 
						{
							hour = 0;
							min = 0;
						}
					} 
					else if(timezone[0]=='+' || timezone[0]=='-') 
					{
						try
						{
							hour = int.Parse(timezone.Substring(0,3));
							min = int.Parse(timezone.Substring(3,2));
						}
						catch(Exception) 
						{
							hour = 0;
							min = 0;
						}
					}
					else 
					{
						throw new Exception(timezone);
					}
					return new TimeSpan(hour,min,0);
				}
				switch(timezone) 
				{
					case "gmt":
					case "ut":
						return new TimeSpan(0,0,0);
					case "adt":
						return new TimeSpan(-3,0,0);
					case "ast":
					case "edt":
					case "gmt-4":
						return new TimeSpan(-4,0,0);
					case "est":
						return new TimeSpan(-5,0,0);
					case "cdt":
					case "utc-5:00":
						return new TimeSpan(-5,0,0);
					case "cst":
					case "mdt":
					case "gmt-6":
						return new TimeSpan(-6,0,0);
					case "mst":
						return new TimeSpan(-7,0,0);
					case "pdt":
						return new TimeSpan(-7,0,0);
					case "pst":
						return new TimeSpan(-8,0,0);
					case "akdt":
						return new TimeSpan(-8,0,0);
					case "akst":
						return new TimeSpan(-9,0,0);
					case "hst":
						return new TimeSpan(-10,0,0);
				}
				throw new ApplicationException(string.Format("Unknown timezone: {0}",timezone));
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

		static public int ReadArticles(object boardID,int nLastUpdate,int nTimeToRun,bool bCreateUsers) 
		{
			int			nUserID			= DB.user_guest();	// Use guests user-id
			string		sHostAddress	= System.Web.HttpContext.Current.Request.UserHostAddress;
			DataTable	dtSystem		= DB.registry_list("TimeZone");
			TimeSpan	tsLocal			= new TimeSpan(0,Convert.ToInt32(dtSystem.Rows[0]["Value"]),0);
			DateTime	dtStart			= DateTime.Now;
			int			nArticleCount	= 0;

			Nntp nntp = null;
			string hostname = null;
			int port = 119;
			try 
			{
				// Only those not updated in the last 30 minutes
				using(DataTable dtForums = DB.nntpforum_list(boardID,nLastUpdate,null,true)) 
				{
					foreach(DataRow drForum in dtForums.Rows) 
					{
						if(hostname!=drForum["Address"].ToString().ToLower() || port!=(int)drForum["Port"]) 
						{
							if(nntp!=null) 
							{
								nntp.Disconnect();
								nntp.Dispose(true);
							}
							nntp = new Nntp();
							hostname = drForum["Address"].ToString().ToLower();
							port = (int)drForum["Port"];
							nntp.ConnectNntp(hostname,port);
						}

						GroupInfo group = nntp.Group(drForum["GroupName"].ToString());
						int nLastMessageNo = (int)drForum["LastMessageNo"];
						int nCurrentMessage = nLastMessageNo;
						// If this is first retrieve for this group, only fetch last 50
						if(nCurrentMessage==0)
							nCurrentMessage = group.Last - 50;

						nCurrentMessage++;

						int			nForumID	= (int)drForum["ForumID"];

						for(;nCurrentMessage<=group.Last;nCurrentMessage++) 
						{
							try 
							{
								ArticleInfo article = nntp.Article(nCurrentMessage);

								string		sBody		= article.Body;
								string		sThread		= article.Thread;
								string		sSubject	= article.Subject;
								string		sFrom		= article.FromName;
								string		sDate		= article.DateString;
								DateTime	dtDate		= article.Date - tsLocal;

								if(dtDate.Year<1950 || dtDate>DateTime.Now)
									dtDate = DateTime.Now;
							
								sBody = String.Format("Date: {0}\r\n\r\n",sDate) + sBody;
								sBody = String.Format("Date parsed: {0}\r\n",dtDate) + sBody;

								if(bCreateUsers)
									nUserID = DB.user_nntp(boardID,sFrom,article.FromEmail);

								sBody = System.Web.HttpContext.Current.Server.HtmlEncode(sBody);
								DB.nntptopic_savemessage(drForum["NntpForumID"],sSubject,sBody,nUserID,sFrom,sHostAddress,dtDate,sThread);
								nLastMessageNo = nCurrentMessage;
								nArticleCount++;
								// We don't wanna retrieve articles forever...
								// Total time x seconds for all groups
								if((DateTime.Now - dtStart).TotalSeconds>nTimeToRun)
									break;
							}
							catch(NntpException) 
							{
							}
						}
						DB.nntpforum_update(drForum["NntpForumID"],nLastMessageNo,nUserID);
						// Total time x seconds for all groups
						if((DateTime.Now - dtStart).TotalSeconds>nTimeToRun)
							break;
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
			return nArticleCount;
		}
	}
}
