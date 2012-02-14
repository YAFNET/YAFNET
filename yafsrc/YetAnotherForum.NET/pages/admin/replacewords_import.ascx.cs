/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Pages.Admin
{
	#region Using

	using System;
	using System.Data;
	using System.Linq;

	using YAF.Classes.Data;
	using YAF.Core;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The replacewords_import.
	/// </summary>
	public partial class replacewords_import : AdminPage
	{
		#region Methods

		/// <summary>
		/// The cancel_ on click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Cancel_OnClick([NotNull] object sender, [NotNull] EventArgs e)
		{
			YafBuildLink.Redirect(ForumPages.admin_replacewords);
		}

		/// <summary>
		/// The import_ on click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Import_OnClick([NotNull] object sender, [NotNull] EventArgs e)
		{
			// import selected file (if it's the proper format)...
			if (!this.importFile.PostedFile.ContentType.StartsWith("text"))
			{
				this.PageContext.AddLoadMessage(
						this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED_FAILEDX").FormatWith("Invalid upload format specified: " + this.importFile.PostedFile.ContentType));

				return;
			}

			try
			{
				// import replace words...
				var dsReplaceWords = new DataSet();
				dsReplaceWords.ReadXml(this.importFile.PostedFile.InputStream);

				if (dsReplaceWords.Tables["YafReplaceWords"] != null &&
						dsReplaceWords.Tables["YafReplaceWords"].Columns["badword"] != null &&
						dsReplaceWords.Tables["YafReplaceWords"].Columns["goodword"] != null)
				{
					int importedCount = 0;

					DataTable replaceWordsList = LegacyDb.replace_words_list(this.PageContext.PageBoardID, null);

					// import any extensions that don't exist...
					foreach (DataRow row in
						dsReplaceWords.Tables["YafReplaceWords"].Rows.Cast<DataRow>().Where(row => replaceWordsList.Select("badword = '{0}' AND goodword = '{1}'".FormatWith(row["badword"], row["goodword"])).Length == 0))
					{
						// add this...
						LegacyDb.replace_words_save(this.PageContext.PageBoardID, null, row["badword"], row["goodword"]);
						importedCount++;
					}

					this.PageContext.LoadMessage.AddSession(
						importedCount > 0
							? this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED").FormatWith(importedCount)
							: this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_NOTHING"));

					YafBuildLink.Redirect(ForumPages.admin_replacewords);
				}
				else
				{
					this.PageContext.AddLoadMessage(this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED_FAILED"));
				}
			}
			catch (Exception x)
			{
				this.PageContext.AddLoadMessage(this.GetText("ADMIN_REPLACEWORDS_IMPORT", "MSG_IMPORTED_FAILEDX").FormatWith(x.Message));
			}
		}

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
		{
			if (this.IsPostBack)
			{
				return;
			}

			this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
			this.PageLinks.AddLink(this.GetText("ADMIN_REPLACEWORDS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_replacewords));
			this.PageLinks.AddLink(this.GetText("ADMIN_REPLACEWORDS_IMPORT", "TITLE"), string.Empty);

			this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
					this.GetText("ADMIN_ADMIN", "Administration"),
					this.GetText("ADMIN_REPLACEWORDS", "TITLE"),
					this.GetText("ADMIN_REPLACEWORDS_IMPORT", "TITLE"));

			this.Import.Text = this.GetText("COMMON", "IMPORT");
			this.cancel.Text = this.GetText("COMMON", "CANCEL");
		}

		#endregion
	}
}