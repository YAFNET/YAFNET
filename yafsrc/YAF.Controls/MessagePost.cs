/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	/// Shows a Message Post
	/// </summary>
	public class MessagePost : BaseControl
	{
		static private RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
		static private string _rgxModule = @"\<YafModuleFactoryInvocation ClassName=\""(?<classname>(.*?))\""\>(?<inner>(.*?))\</YafModuleFactoryInvocation\>";

		public MessagePost()
			: base()
		{

		}

		protected override void OnPreRender( EventArgs e )
		{
			if ( !String.IsNullOrEmpty( this.Signature ) )
			{
				MessageSignature sig = new MessageSignature();
				sig.Signature = this.Signature;
				this.Controls.Add( sig );
			}

			base.OnPreRender( e );
		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.BeginRender();
			writer.WriteBeginTag( "div" );
			writer.WriteAttribute( "id", this.ClientID );
			writer.Write( HtmlTextWriter.TagRightChar );

			RenderMessage( writer );

			// render controls...
			base.Render( writer );

			writer.WriteEndTag( "div" );
			writer.EndRender();
		}

		virtual protected void RenderMessage( HtmlTextWriter writer )
		{
			if ( this.MessageFlags.IsDeleted )
			{
				// deleted message text...
				RenderDeletedMessage( writer );
			}
			else if ( this.MessageFlags.NotFormatted )
			{
				writer.Write( this.Message );
			}
			else
			{
				string formattedMessage = FormatMsg.FormatMessage( this.Message, this.MessageFlags );

				if ( this.MessageFlags.IsBBCode )
					RenderModulesInBBCode( writer, formattedMessage );
				else
					writer.Write( formattedMessage );
			}
		}

		virtual protected void RenderDeletedMessage( HtmlTextWriter writer )
		{
			// if message was deleted then write that instead of real body
			if ( MessageFlags.IsDeleted )
			{
				if ( IsModeratorChanged )
				{
					// deleted by mod
					writer.Write( PageContext.Localization.GetText( "POSTS", "MESSAGEDELETED_MOD" ) );
				}
				else
				{
					// deleted by user
					writer.Write( PageContext.Localization.GetText( "POSTS", "MESSAGEDELETED_USER" ) );
				}
			}
		}

		virtual protected void RenderModulesInBBCode( HtmlTextWriter writer, string message )
		{
			XmlDocument xmlDoc = new XmlDocument();
			Regex _regExSearch = new Regex( _rgxModule, _options );
			Match m = _regExSearch.Match( message );

			while ( m.Success )
			{
				// load this module info into the xml document...
				xmlDoc.LoadXml( m.Groups [0].Value );
				XmlNode mainNode = xmlDoc.SelectSingleNode( "YafModuleFactoryInvocation" );
				string className = mainNode.Attributes ["ClassName"].InnerText;

				// get all parameters as a name/value dictionary
				Dictionary<string, string> paramDic = new Dictionary<string, string>();
				XmlNodeList paramList = xmlDoc.SelectNodes( "/YafModuleFactoryInvocation/Parameters/*" );

				foreach ( XmlNode paramNode in paramList )
				{
					paramDic.Add( paramNode.Attributes ["Name"].InnerText, paramNode.InnerText );
				}

				// render what is before the control...
				writer.Write( message.Substring( 0, m.Groups [0].Index ) );

				// create/render the control...
				Type module = System.Web.Compilation.BuildManager.GetType( className, true, false );
				YAF.Modules.YafBBCodeControl customModule = ( YAF.Modules.YafBBCodeControl )Activator.CreateInstance( module );
				// assign parameters...
				customModule.Parameters = paramDic;
				// render this control...
				customModule.RenderControl( writer );

				// now we are just concerned with what is after...
				message =  message.Substring( m.Groups [0].Index + m.Groups [0].Length );

				m = _regExSearch.Match( message );
			}

			// render anything remaining...
			writer.Write( message );
		}

		virtual public string Signature
		{
			get
			{
				if ( ViewState ["Signature"] != null )
					return ViewState ["Signature"].ToString();

				return null;
			}
			set { ViewState ["Signature"] = value; }
		}

		virtual public string Message
		{
			get
			{
				if ( ViewState ["Message"] != null )
					return ViewState ["Message"].ToString();

				return null;
			}
			set { ViewState ["Message"] = value; }
		}

		virtual public bool IsModeratorChanged
		{
			get
			{
				if ( ViewState ["IsModeratorChanged"] != null )
					return Convert.ToBoolean(ViewState ["IsModeratorChanged"]);

				return false;
			}
			set { ViewState ["IsModeratorChanged"] = value; }
		}

		virtual public MessageFlags MessageFlags
		{
			get
			{
				if ( ViewState ["MessageFlags"] == null )
				{
					ViewState ["MessageFlags"] = new MessageFlags( 0 );
				}
				
				return ViewState ["MessageFlags"] as MessageFlags;
			}
			set { ViewState ["MessageFlags"] = value; }
		}
	}
}