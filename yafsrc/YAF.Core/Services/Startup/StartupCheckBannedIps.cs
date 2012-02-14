/* YetAnotherForum.NET
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
namespace YAF.Core.Services
{
  #region Using

  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The yaf check banned ips.
  /// </summary>
  public class StartupCheckBannedIps : BaseStartupService
  {
    #region Constants and Fields

    /// <summary>
    ///   The _init var name.
    /// </summary>
    protected const string _initVarName = "YafCheckBannedIps_Init";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupCheckBannedIps"/> class.
    /// </summary>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    /// <param name="httpResponseBase">
    /// The http response base.
    /// </param>
    /// <param name="httpRequestBase">
    /// The http request base.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public StartupCheckBannedIps(
      [NotNull] IDataCache dataCache, 
      [NotNull] HttpResponseBase httpResponseBase, 
      [NotNull] HttpRequestBase httpRequestBase, [NotNull] ILogger logger)
    {
      this.DataCache = dataCache;
      this.HttpResponseBase = httpResponseBase;
      this.HttpRequestBase = httpRequestBase;
      this.Logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets or sets HttpRequestBase.
    /// </summary>
    public HttpRequestBase HttpRequestBase { get; set; }

    /// <summary>
    ///   Gets or sets HttpResponseBase.
    /// </summary>
    public HttpResponseBase HttpResponseBase { get; set; }

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    ///   Gets InitVarName.
    /// </summary>
    [NotNull]
    protected override string InitVarName
    {
      get
      {
        return "YafCheckBannedIps_Init";
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The run service.
    /// </returns>
    protected override bool RunService()
    {
      var bannedIPs =
        this.DataCache.GetOrSet(
          Constants.Cache.BannedIP, () => LegacyDb.bannedip_list(YafContext.Current.PageBoardID, null)).AsEnumerable();

      // check for this user in the list...
      if (
        bannedIPs.Any(
          row => IPHelper.IsBanned(row.Field<string>("Mask"), this.HttpRequestBase.ServerVariables["REMOTE_ADDR"])))
      {
        // we're done...
        this.Logger.Info(@"Ending Response for Banned User at IP ""{0}""", this.HttpRequestBase.ServerVariables["REMOTE_ADDR"]);
        this.HttpResponseBase.End();
        return false;
      }

      return true;
    }

    #endregion
  }
}