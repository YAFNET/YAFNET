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

using YAF.Types.Interfaces.Data;

namespace YAF.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services.Import;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    /// <summary>
    ///     The install upgrade service.
    /// </summary>
    public class InstallUpgradeService : IHaveServiceLocator
    {
        #region Constants

        /// <summary>
        ///     The _bbcode import.
        /// </summary>
        private const string _BbcodeImport = "bbCodeExtensions.xml";

        /// <summary>
        ///     The _file import.
        /// </summary>
        private const string _FileImport = "fileExtensions.xml";

        /// <summary>
        ///     The Topic Status Import File.
        /// </summary>
        private const string _TopicStatusImport = "TopicStatusList.xml";

        #endregion

        #region Fields

        /// <summary>
        ///     The _messages.
        /// </summary>
        private readonly List<string> _messages = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallUpgradeService"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="raiseEvent">
        /// The raise Event.
        /// </param>
        public InstallUpgradeService(IServiceLocator serviceLocator, IRaiseEvent raiseEvent, IDbAccess dbAccess)
        {
            this.RaiseEvent = raiseEvent;
            this.DbAccess = dbAccess;
            this.ServiceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is forum installed.
        /// </summary>
        public bool IsForumInstalled
        {
            get
            {
                try
                {
                    var boards = this.GetRepository<Board>().ListTyped();
                    return boards.Any();
                }
                catch
                {
                    // failure... no boards.    
                }

                return false;
            }
        }

        /// <summary>
        ///     Gets the messages.
        /// </summary>
        public string[] Messages
        {
            get
            {
                return this._messages.ToArray();
            }
        }

        /// <summary>
        ///     Gets or sets the raise event.
        /// </summary>
        public IRaiseEvent RaiseEvent { get; set; }

        public IDbAccess DbAccess { get; set; }

        /// <summary>
        ///     Gets or sets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets PageBoardID.
        /// </summary>
        private int PageBoardID
        {
            get
            {
                try
                {
                    return int.Parse(Config.BoardID);
                }
                catch
                {
                    return 1;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize forum.
        /// </summary>
        /// <param name="forumName">
        /// The forum name.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="forumEmail">
        /// The forum email.
        /// </param>
        /// <param name="adminUserName">
        /// The admin user name.
        /// </param>
        /// <param name="adminEmail">
        /// The admin email.
        /// </param>
        /// <param name="adminProviderUserKey">
        /// The admin provider user key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public void InitializeForum(
            string forumName, string timeZone, string culture, string forumEmail, string adminUserName, string adminEmail, object adminProviderUserKey)
        {
            DataTable cult = StaticDataHelper.Cultures();
            string langFile = "english.xml";

            foreach (DataRow drow in cult.Rows.Cast<DataRow>().Where(drow => drow["CultureTag"].ToString() == culture))
            {
                langFile = (string)drow["CultureFile"];
            }

            LegacyDb.system_initialize(
                forumName, 
                timeZone, 
                culture, 
                langFile, 
                forumEmail, 
                string.Empty, 
                adminUserName, 
                adminEmail, 
                adminProviderUserKey, 
                Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

            LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);
            LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

            // vzrus: uncomment it to not keep install/upgrade objects in db for a place and better security
            // YAF.Classes.Data.DB.system_deleteinstallobjects();
            this.ImportStatics();
        }

        /// <summary>
        /// Tests database connection. Can probably be moved to DB class.
        /// </summary>
        /// <param name="exceptionMessage">
        /// The exception message.
        /// </param>
        /// <returns>
        /// The test database connection.
        /// </returns>
        public bool TestDatabaseConnection([NotNull] out string exceptionMessage)
        {
            return this.DbAccess.TestConnection(out exceptionMessage);
        }

        /// <summary>
        /// The upgrade database.
        /// </summary>
        /// <param name="fullText">
        /// The full text.
        /// </param>
        /// <param name="upgradeExtensions">
        /// The upgrade Extensions.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpgradeDatabase(bool fullText, bool upgradeExtensions)
        {
            this._messages.Clear();
            {
                // try
                this.FixAccess(false);

                foreach (string script in this.DbAccess.Information.Scripts)
                {
                    this.ExecuteScript(script, true);
                }

                this.FixAccess(true);

                int prevVersion = LegacyDb.GetDBVersion();

                LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

                // Ederon : 9/7/2007
                // resync all boards - necessary for propr last post bubbling
                this.GetRepository<Board>().Resync();

                this.RaiseEvent.RaiseIssolated(new AfterUpgradeDatabaseEvent(prevVersion, YafForumInfo.AppVersion), null);

                //// upgrade providers...
                // 
                if (this.IsForumInstalled && (prevVersion < 30 || upgradeExtensions))
                {
                    this.ImportStatics();
                }

                if (this.IsForumInstalled && prevVersion < 42)
                {
                    // un-html encode all topic subject names...
                    LegacyDb.unencode_all_topics_subjects(HttpUtility.HtmlDecode);
                }

                if (this.IsForumInstalled && prevVersion < 49)
                {
                    // Reset The UserBox Template
                    this.Get<YafBoardSettings>().UserBox = Constants.UserBox.DisplayTemplateDefault;

                    ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();
                }

                // vzrus: uncomment it to not keep install/upgrade objects in DB and for better security 
                // DB.system_deleteinstallobjects();
            }

            // attempt to apply fulltext support if desired
            if (fullText && this.DbAccess.Information.FullTextScript.IsSet())
            {
                try
                {
                    this.ExecuteScript(this.DbAccess.Information.FullTextScript, false);
                }
                catch (Exception x)
                {
                    // just a warning...
                    this._messages.Add("Warning: FullText Support wasn't installed: {0}".FormatWith(x.Message));
                }
            }

            // run custom script...
            this.ExecuteScript("custom/custom.sql", true);

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute script.
        /// </summary>
        /// <param name="scriptFile">
        /// The script file.
        /// </param>
        /// <param name="useTransactions">
        /// The use transactions.
        /// </param>
        private void ExecuteScript([NotNull] string scriptFile, bool useTransactions)
        {
            string script;
            string fileName = this.Get<HttpRequestBase>().MapPath(scriptFile);

            try
            {
                script = "{0}\r\n".FormatWith(File.ReadAllText(fileName));
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (Exception x)
            {
                throw new IOException("Failed to read {0}".FormatWith(fileName), x);
            }

            LegacyDb.system_initialize_executescripts(script, scriptFile, useTransactions);
        }

        /// <summary>
        /// The fix access.
        /// </summary>
        /// <param name="bGrant">
        /// The b grant.
        /// </param>
        private void FixAccess(bool bGrant)
        {
            LegacyDb.system_initialize_fixaccess(bGrant);
        }

        /// <summary>
        ///     The import statics.
        /// </summary>
        private void ImportStatics()
        {
            var loadWrapper = new Action<string, Action<Stream>>(
                (file, streamAction) =>
                    {
                        var fullFile = this.Get<HttpRequestBase>().MapPath(file);

                        if (!File.Exists(fullFile))
                        {
                            return;
                        }

                        // import into board...
                        using (var stream = new StreamReader(fullFile))
                        {
                            streamAction(stream.BaseStream);
                            stream.Close();
                        }
                    });

            this.Get<IRaiseEvent>().Raise(new ImportStaticDataEvent(this.PageBoardID));

            // load default bbcode if available...
            loadWrapper(_BbcodeImport, s => DataImport.BBCodeExtensionImport(this.PageBoardID, s));

            // load default extensions if available...
            loadWrapper(_FileImport, s => DataImport.FileExtensionImport(this.PageBoardID, s));

            // load default topic status if available...
            loadWrapper(_TopicStatusImport, s => DataImport.TopicStatusImport(this.PageBoardID, s));
        }

        #endregion
    }
}