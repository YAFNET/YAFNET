using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace yaf
{
	/// <summary>
	/// Summary description for Smtp.
	/// </summary>
	public class Smtp : System.Net.Sockets.TcpClient
	{
		#region Declarations
		const int			_maxReceiveSize	= 1024;

		const int			_sendTimeout	= 60000;	// milliseconds
		const int			_receiveTimeout	= 60000;	// milliseconds
		const int			_pollTimeout	= 100000;	// in microseconds

		const string		CRLF			= "\r\n";

		string				_server			= null;
		string				_username		= null;
		string				_userpass		= null;
		const int			_port			= 25;
		string				_lastline		= "";
		Encoding			m_SrcCharset	= System.Text.Encoding.Default;
		Encoding			m_DestCharset;
		#endregion

		#region Constructors
		public Smtp(string server)
		{
			_server = server;

			// CharSet
			if( m_SrcCharset.HeaderName.Equals( m_SrcCharset.BodyName ) )
			{
				// No need for conversion
				m_DestCharset = m_SrcCharset;
			}
			else
			{
				// Src and dest charsets are different. Need to convert
				m_DestCharset = System.Text.Encoding.GetEncoding( m_SrcCharset.BodyName );
			}

		}

		public Smtp(string server,string username,string userpass) : this(server)
		{
			_username = username;
			_userpass = userpass;
		}
		#endregion

		public void SendMail(string from,string to,string subject,string message) 
		{
			if(!Active)
				OpenConnection();

			if(SendCmd(String.Format("MAIL FROM:<{0}>",from))!=250)
				Error();

			if(SendCmd(String.Format("RCPT TO:<{0}>",to))!=250)
				Error();

			if(SendCmd("DATA")!=354)
				Error();

			StringBuilder text = new StringBuilder();

			// Headers
			text.AppendFormat("Subject: {0}\r\n",subject);
			text.AppendFormat("From: {0}\r\n",from);
			text.AppendFormat("To: {0}\r\n",to);
			text.AppendFormat("Date: {0:r}\r\n",System.DateTime.Now.ToUniversalTime());
			text.Append("MIME-Version: 1.0\r\n");
			text.AppendFormat("Content-Type: text/plain; charset=\"{0}\"\r\n",m_DestCharset.HeaderName);
			text.AppendFormat("Content-Transfer-Encoding: {0}\r\n","8bit");
			text.AppendFormat("X-Mailer: {0}\r\n",System.Reflection.Assembly.GetCallingAssembly().FullName);
			text.Append("\r\n");
			
			byte[] srcData = m_SrcCharset.GetBytes(text.ToString());
			byte[] dstData;
			
			// see if we need to convert header
			if( m_DestCharset != m_SrcCharset )
				dstData = Encoding.Convert( m_SrcCharset, m_DestCharset, srcData );
			else
				dstData = srcData;
			
			GetStream().Write(dstData,0,dstData.Length);
			
			// Body
			string[] sLines = System.Text.RegularExpressions.Regex.Split(message, "\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			text.Length = 0;
			foreach(string sLine in sLines) 
			{
				if(sLine==".")
					text.Append("..");
				else
					text.Append(sLine);
				text.Append("\r\n");
			}

			srcData = m_SrcCharset.GetBytes(text.ToString());
			// see if we need to convert body
			if( m_DestCharset != m_SrcCharset )
				dstData = Encoding.Convert( m_SrcCharset, m_DestCharset, srcData );
			else
				dstData = srcData;

			if(false) 
			{
				//Send(Convert.ToBase64String(_encoding.GetBytes(body.ToString().ToCharArray())));
			} 
			else 
			{
				GetStream().Write(dstData,0,dstData.Length);
			}
			
			// End of message
			if(SendCmd(".")!=250)
				Error();
		}

		#region Private methods and properties
		private void OpenConnection()
		{
			// the amount of time it will remain after closing if data remains to be sent
			LingerState		= new LingerOption(true, 10);
			// set sockets timeouts
			SendTimeout		= _sendTimeout;
			ReceiveTimeout	= _receiveTimeout;

			// connect to host
			Connect(_server,_port);
			if(!Active)
				throw new Exception("Incorrect server address: " + _server);

			Receive();

			if(_username!=null && _userpass!=null) 
			{
				if(SendCmd("EHLO " + _server)!=250)
					Error();

				if(SendCmd("AUTH LOGIN")!=334)
					Error();

				if(SendCmd(Convert.ToBase64String(Encoding.Default.GetBytes(_username)))!=334)
					Error();

				if(SendCmd(Convert.ToBase64String(Encoding.Default.GetBytes(_userpass)))!=235)
					Error();
			} 
			else 
			{
				if(SendCmd("HELO " + _server)!=250)
					Error();
			}
		}
		#endregion

		#region Send and receive commands
		private long SendCmd(string command) 
		{
			Send(command);
			_lastline = Receive();
			return long.Parse(_lastline.Substring(0,3));
		}

		private void Send(string command) 
		{
			Trace("C: " + command);
			command += CRLF;
            
			byte[] WriteBuffer = System.Text.Encoding.ASCII.GetBytes(command);
			GetStream().Write(WriteBuffer,0,WriteBuffer.Length);
		}	

		public void Quit()
		{
			SendCmd("QUIT");
			Close();
		}

		private string Receive()
		{
			StringBuilder tmp = new StringBuilder();
			Encoding cenc = Encoding.ASCII;
			do
			{
				Byte[] buf = new Byte[_maxReceiveSize];
				int recv = Client.Receive(buf, SocketFlags.None);
				if (recv == 0) 
				{
					if (tmp.Length > 3) 
						break;

					Close();
					throw new Exception("Failed to receive");
				}
				tmp.Append(cenc.GetChars(buf, 0, recv));
			} while(Client.Poll(_pollTimeout, SelectMode.SelectRead));

			Trace("S: " + tmp.ToString().Trim());
			return tmp.ToString().Trim();
		}
		#endregion

		#region Errors and debugging
		private void Error() 
		{
			throw new Exception(_lastline);
		}

		private void Trace(string str) 
		{
		}
		#endregion
	}
}
