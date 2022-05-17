/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Web.Editors;

using ServiceStack.OrmLite;

/// <summary>
/// The CKEditor BBCode editor (Sql Mode).
/// </summary>
public class CKEditorBBCodeEditorSql : CKEditor
{
    /// <summary>
    ///   Gets Description.
    /// </summary>
    [NotNull]
    public override string Description => "CKEditor (BBCode) - Sql";

    /// <summary>
    ///   Gets ModuleId.
    /// </summary>
    public override string ModuleId => "6";

    /// <summary>
    ///   Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode => true;

    /// <summary>
    ///   Gets a value indicating whether UsesHTML.
    /// </summary>
    public override bool UsesHTML => false;

    /// <summary>
    /// The allows uploads.
    /// </summary>
    public override bool AllowsUploads => false;

    /// <summary>
    /// The register CKEditor custom JS.
    /// </summary>
    protected override void RegisterCKEditorCustomJS()
    {
        var language = BoardContext.Current.PageUser.Culture.IsSet()
                           ? BoardContext.Current.PageUser.Culture.Substring(0, 2)
                           : this.PageBoardContext.BoardSettings.Culture.Substring(0, 2);

        if (ValidationHelper.IsNumeric(language))
        {
            language = this.PageBoardContext.BoardSettings.Culture.Substring(0, 2);
        }

        var serverName = OrmLiteConfig.DialectProvider.SQLServerName();

        var mime = serverName switch
            {
                "Microsoft SQL Server" => "text/x-mssql",
                "MySQL" => "text/x-mysql",
                "PostgreSQL" => "text/x-pgsql",
                _ => "text/x-sql"
            };

        BoardContext.Current.PageElements.RegisterJsBlock(
            "ckeditorinitbbcodeSql",
            JavaScriptBlocks.CKEditorSqlLoadJs(
                this.TextAreaControl.ClientID,
                language,
                this.MaxCharacters,
                this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css"),
                BoardInfo.GetURLToContent("forum.min.css"),
                mime));
    }
}