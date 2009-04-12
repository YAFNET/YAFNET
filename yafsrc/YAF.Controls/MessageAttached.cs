/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	public class MessageAttached : BaseControl
	{
		public MessageAttached()
			: base()
		{

		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.BeginRender();
			writer.WriteBeginTag( "div" );
			writer.WriteAttribute( "id", this.ClientID );
			writer.Write( HtmlTextWriter.TagRightChar );

			if ( MessageID != 0 )
			{
				RenderAttachedFiles( writer );
			}

			base.Render( writer );

			writer.WriteEndTag( "div" );
			writer.EndRender();
		}

		protected void RenderAttachedFiles( HtmlTextWriter writer )
		{
			string [] aImageExtensions = { "jpg", "gif", "png", "bmp" };

			string stats = PageContext.Localization.GetText( "ATTACHMENTINFO" );
			string strFileIcon = PageContext.Theme.GetItem( "ICONS", "ATTACHED_FILE" );

			using ( DataTable attachListDT = YAF.Classes.Data.DB.attachment_list( this.MessageID, null, null ) )
			{
				// show file then image attachments...
				int tmpDisplaySort = 0;

				writer.Write( @"<div class=""fileattach smallfont"">" );

				while ( tmpDisplaySort <= 1 )
				{
					bool bFirstItem = true;

					foreach ( DataRow dr in attachListDT.Rows )
					{
						string strFilename = Convert.ToString( dr ["FileName"] ).ToLower();
						bool bShowImage = false;

						// verify it's not too large to display
						// Ederon : 02/17/2009 - made it board setting
						if ( Convert.ToInt32( dr ["Bytes"] ) <= PageContext.BoardSettings.PictureAttachmentDisplayTreshold )
						{
							// is it an image file?
							for ( int i = 0; i < aImageExtensions.Length; i++ )
							{
								if ( strFilename.ToLower().EndsWith( aImageExtensions [i] ) )
								{
									bShowImage = true;
									break;
								}
							}
						}
						if ( bShowImage && tmpDisplaySort == 1 )
						{
							if ( bFirstItem )
							{
								writer.Write( @"<div class=""imgtitle"">" );
								writer.Write( String.Format(PageContext.Localization.GetText( "IMAGE_ATTACHMENT_TEXT" ), Convert.ToString( UserName )) );
								writer.Write( "</div>" );
								bFirstItem = false;
							}
							// Ederon : download rights
							if ( PageContext.ForumDownloadAccess || PageContext.ForumModeratorAccess )
							{
								// user has rights to download, show him image
								writer.Write( String.Format(@"<div class=""attachedimg""><img src=""{0}resource.ashx?a={1}"" alt=""{2}"" /></div>", YafForumInfo.ForumRoot, dr ["AttachmentID"], HtmlEncode( dr ["FileName"] ) ) );
							}
							else
							{
								int kb = ( 1023 + ( int )dr ["Bytes"] ) / 1024;
								// user doesn't have rights to download, don't show him image
								writer.Write( @"<div class=""attachedfile"">" );
								writer.Write( String.Format(@"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>", strFileIcon, dr ["FileName"], String.Format( stats, kb, dr ["Downloads"] )) );
								writer.Write( @"</div>" );
							}
						}
						else if ( !bShowImage && tmpDisplaySort == 0 )
						{
							if ( bFirstItem )
							{
								writer.Write( String.Format(@"<div class=""filetitle"">{0}</div>", PageContext.Localization.GetText( "ATTACHMENTS" )) );
								bFirstItem = false;
							}
							// regular file attachment
							int kb = ( 1023 + ( int )dr ["Bytes"] ) / 1024;

							writer.Write( @"<div class=""attachedfile"">" );

							// Ederon : download rights
							if ( PageContext.ForumDownloadAccess || PageContext.ForumModeratorAccess )
							{
								writer.Write( String.Format(@"<img border=""0"" alt="""" src=""{0}"" /> <a href=""{1}resource.ashx?a={2}"">{3}</a> <span class=""attachmentinfo"">{4}</span>", strFileIcon, YafForumInfo.ForumRoot, dr ["AttachmentID"], dr ["FileName"], String.Format( stats, kb, dr ["Downloads"] )) );
							}
							else
							{
								writer.Write( String.Format(@"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>", strFileIcon, dr ["FileName"], String.Format( stats, kb, dr ["Downloads"] )) );
							}

							writer.Write( @"</div>" );
						}
					}
					// now show images
					tmpDisplaySort++;
				}

				writer.Write( @"</div>" );
			}
		}

		private int _messageId = 0;
		public int MessageID
		{
			get { return _messageId; }
			set { _messageId = value; }
		}

		private string _userName = string.Empty;
		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}
	}
}