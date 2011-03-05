/* DotNetNuke - http://www.dotnetnuke.com
// Copyright (c) DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
*/ 

namespace YAF.DotNetNuke.DnnUtils
{
    using System;
    using System.IO;
    using System.Linq;
    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Entities.Portals;
    using global::DotNetNuke.Security.Permissions;
    using global::DotNetNuke.Services.FileSystem;

    /// <summary>
    /// Imported and converted DNN Source, so it can be used in dnn from version 05.03.00 to current
    /// </summary>
    public class DnnFileSystem
    {
        #region "Enums"

        /// <summary>
        /// Enum UserFolder Element
        /// </summary>
        private enum EnumUserFolderElement
        {
            /// <summary>
            /// Is Root
            /// </summary>
            Root = 0,

            /// <summary>
            /// Is SubFolder
            /// </summary>
            SubFolder = 1
        }
        #endregion

        /// <summary>
        /// Add the dnn User Folder
        /// </summary>
        /// <param name="_portalSettings">
        /// The _ portal settings.
        /// </param>
        /// <param name="parentFolder">
        /// The parent folder.
        /// </param>
        /// <param name="storageLocation">
        /// The storage location.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public static void AddUserFolder(PortalSettings _portalSettings, string parentFolder, int storageLocation, int userID)
        {
            int portalId = _portalSettings.PortalId;
            string parentFolderName = _portalSettings.HomeDirectoryMapPath;

            string rootFolder = GetUserFolderPathElement(userID, EnumUserFolderElement.Root);
            string subFolder = GetUserFolderPathElement(userID, EnumUserFolderElement.SubFolder);

            // create root folder
            string folderPath = Path.Combine(Path.Combine(parentFolderName, "Users"), rootFolder);
            DirectoryInfo dinfoNew = new DirectoryInfo(folderPath);

            if (!dinfoNew.Exists)
            {
                dinfoNew.Create();
                AddFolder(portalId, folderPath.Substring(parentFolderName.Length).Replace("\\", "/"), storageLocation);
            }

            // create two-digit subfolder
            folderPath = Path.Combine(folderPath, subFolder);
            dinfoNew = new DirectoryInfo(folderPath);

            if (!dinfoNew.Exists)
            {
                dinfoNew.Create();
                AddFolder(portalId, folderPath.Substring(parentFolderName.Length).Replace("\\", "/"), storageLocation);
            }

            // create folder from UserID
            folderPath = Path.Combine(folderPath, userID.ToString());
            dinfoNew = new DirectoryInfo(folderPath);

            if (dinfoNew.Exists)
            {
                return;
            }

            dinfoNew.Create();
            int folderID = AddFolder(portalId, folderPath.Substring(parentFolderName.Length).Replace("\\", "/"), storageLocation);

            // Give user Read Access to this folder
            FolderInfo folder = new FolderController().GetFolderInfo(portalId, folderID);
            foreach (FolderPermissionInfo folderPermission in
                from PermissionInfo permission in PermissionController.GetPermissionsByFolder()
                where permission.PermissionKey.ToUpper() == "READ" || permission.PermissionKey.ToUpper() == "WRITE"
                select new FolderPermissionInfo(permission)
                {
                    FolderID = folder.FolderID,
                    UserID = userID,
                    RoleID = Null.NullInteger,
                    AllowAccess = true
                })
            {
                folder.FolderPermissions.Add(folderPermission);
            }

            FolderPermissionController.SaveFolderPermissions(folder);
        }

        /// <summary>
        /// Add new Dnn Folder
        /// </summary>
        /// <param name="portalId">
        /// The portal id.
        /// </param>
        /// <param name="relativePath">
        /// The relative path.
        /// </param>
        /// <param name="storageLocation">
        /// The storage location.
        /// </param>
        /// <returns>
        /// Folder Id of the New Folder
        /// </returns>
        private static int AddFolder(int portalId, string relativePath, int storageLocation)
        {
            FolderController objFolderController = new FolderController();
            bool isProtected = FileSystemUtils.DefaultProtectedFolders(relativePath);
            int folderID = objFolderController.AddFolder(portalId, relativePath, storageLocation, isProtected, false);
            
            if (portalId != Null.NullInteger)
            {
                FileSystemUtils.SetFolderPermissions(portalId, folderID, relativePath);
            }

            return folderID;
        }

        /// <summary>
        /// Returns Root and SubFolder elements of User Folder path
        /// </summary>
        /// <param name="userID">
        /// The User ID.
        /// </param>
        /// <param name="mode">
        /// The Mode.
        /// </param>
        /// <returns>
        /// The get user folder path element.
        /// </returns>
        private static string GetUserFolderPathElement(int userID, EnumUserFolderElement mode)
        {
            const int SUBFOLDER_SEED_LENGTH = 2;
            const int BYTE_OFFSET = 255;
            string element = string.Empty;

            switch (mode)
            {
                case EnumUserFolderElement.Root:
                    element = (Convert.ToInt32(userID) & BYTE_OFFSET).ToString("000");

                    break;
                case EnumUserFolderElement.SubFolder:
                    element = userID.ToString("00").Substring(userID.ToString("00").Length - SUBFOLDER_SEED_LENGTH, SUBFOLDER_SEED_LENGTH);

                    break;
            }

            return element;
        }
    }
}