/* Yet Another Forum.net
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
namespace YAF.Core.BBCode
{
  #region Using

    using System.Collections.Generic;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// Gets an instance of replace rules and uses
  ///   caching if possible.
  /// </summary>
  public class ProcessReplaceRulesProvider : IHaveServiceLocator, IReadOnlyProvider<IProcessReplaceRules>
  {
    #region Constants and Fields

    /// <summary>
    ///   The _inject services.
    /// </summary>
    private readonly IInjectServices _injectServices;

    /// <summary>
    /// The _object store.
    /// </summary>
    private readonly IObjectStore _objectStore;

    /// <summary>
    ///   The _unique flags.
    /// </summary>
    private readonly IEnumerable<bool> _uniqueFlags;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessReplaceRulesProvider"/> class.
    /// </summary>
    /// <param name="objectStore">
    /// </param>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="injectServices">
    /// The inject services.
    /// </param>
    /// <param name="uniqueFlags">
    /// The unique Flags.
    /// </param>
    public ProcessReplaceRulesProvider(
      [NotNull] IObjectStore objectStore, 
      [NotNull] IServiceLocator serviceLocator, 
      [NotNull] IInjectServices injectServices, 
      [NotNull] IEnumerable<bool> uniqueFlags)
    {
      this.ServiceLocator = serviceLocator;
      this._objectStore = objectStore;
      this._injectServices = injectServices;
      this._uniqueFlags = uniqueFlags;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   The Instance of this provider.
    /// </summary>
    /// <returns>
    /// </returns>
    public IProcessReplaceRules Instance
    {
      get
      {
        return this._objectStore.GetOrSet(
          Constants.Cache.ReplaceRules.FormatWith(this._uniqueFlags.ToIntOfBits()), 
          () =>
            {
              var processRules = new ProcessReplaceRules();

              // inject
              this._injectServices.Inject(processRules);

              return processRules;
            });
      }
    }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion
  }
}