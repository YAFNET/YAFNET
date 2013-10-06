/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core
{
    using System;
    using System.IO;
    using System.Web.Hosting;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Models;

    /// <summary>
    ///     The file helpers.
    /// </summary>
    public static class FileHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deletes an attachment from the file system. No exceptions are handled in this function.
        /// </summary>
        /// <param name="attachment">
        /// The attachment. 
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool DeleteFile([NotNull] this Attachment attachment)
        {
            CodeContracts.VerifyNotNull(attachment, "attachment");

            string uploadDir = HostingEnvironment.MapPath(string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            string fileName = string.Format("{0}/{1}.{2}.yafupload", uploadDir, attachment.MessageID, attachment.FileName);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);

                return true;
            }

            return false;
        }

        #endregion
    }
}