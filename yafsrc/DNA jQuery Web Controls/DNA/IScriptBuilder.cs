//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA
{
  /// <summary>
  /// The i script builder.
  /// </summary>
  public interface IScriptBuilder
  {
    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsBuilded.
    /// </summary>
    bool IsBuilded { get; }

    /// <summary>
    /// Gets a value indicating whether IsPrepared.
    /// </summary>
    bool IsPrepared { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The build.
    /// </summary>
    void Build();

    /// <summary>
    /// The get applicaiton init script.
    /// </summary>
    /// <returns>
    /// The get applicaiton init script.
    /// </returns>
    string GetApplicaitonInitScript();

    /// <summary>
    /// The get application load script.
    /// </summary>
    /// <returns>
    /// The get application load script.
    /// </returns>
    string GetApplicationLoadScript();

    /// <summary>
    /// The get document ready script.
    /// </summary>
    /// <returns>
    /// The get document ready script.
    /// </returns>
    string GetDocumentReadyScript();

    /// <summary>
    /// The prepare.
    /// </summary>
    void Prepare();

    /// <summary>
    /// The reset.
    /// </summary>
    void Reset();

    #endregion
  }
}