namespace YAF.Core.Services
{
  using System;
  using System.Web;

  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  /// <summary>
  /// The panel session state.
  /// </summary>
  public class PanelSessionState : IPanelSessionState
  {
    #region Indexers

    /// <summary>
    ///   Gets panel session state.
    /// </summary>
    /// <param name = "panelID">panelID</param>
    /// <returns></returns>
    public CollapsiblePanelState this[[NotNull] string panelID]
    {
      // Ederon : 7/14/2007
      get
      {
        string sessionPanelID = "panelstate_" + panelID;

        // try to get panel state from session state first
        if (YafContext.Current.Get<HttpSessionStateBase>()[sessionPanelID] != null)
        {
          return (CollapsiblePanelState)YafContext.Current.Get<HttpSessionStateBase>()[sessionPanelID];
        }
          
          
        // if no panel state info is in session state, try cookie
        if (YafContext.Current.Get<HttpRequestBase>().Cookies[sessionPanelID] != null)
        {
          try
          {
            // we must convert string to int, better get is safe
            if (YafContext.Current.Get<HttpRequestBase>() != null)
            {
              return (CollapsiblePanelState)int.Parse(YafContext.Current.Get<HttpRequestBase>().Cookies[sessionPanelID].Value);
            }
          }
          catch
          {
            // in case cookie has wrong value
            if (YafContext.Current.Get<HttpRequestBase>() != null)
            {
              YafContext.Current.Get<HttpRequestBase>().Cookies.Remove(sessionPanelID); // scrap wrong cookie
            }

            return CollapsiblePanelState.None;
          }
        }

        return CollapsiblePanelState.None;
      }
      // Ederon : 7/14/2007
      set
      {
        string sessionPanelID = "panelstate_" + panelID;

        YafContext.Current.Get<HttpSessionStateBase>()[sessionPanelID] = value;

        // create persistent cookie with visibility setting for panel
        var c = new HttpCookie(sessionPanelID, ((int)value).ToString()) {Expires = DateTime.UtcNow.AddYears(1)};
        YafContext.Current.Get<HttpResponseBase>().SetCookie(c);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The toggle panel state.
    /// </summary>
    /// <param name="panelID">
    /// The panel id.
    /// </param>
    /// <param name="defaultState">
    /// The default state.
    /// </param>
    public void TogglePanelState([NotNull] string panelID, CollapsiblePanelState defaultState)
    {
      CollapsiblePanelState currentState = this[panelID];

      if (currentState == CollapsiblePanelState.None)
      {
        currentState = defaultState;
      }

      if (currentState == CollapsiblePanelState.Collapsed)
      {
        this[panelID] = CollapsiblePanelState.Expanded;
      }
      else if (currentState == CollapsiblePanelState.Expanded)
      {
        this[panelID] = CollapsiblePanelState.Collapsed;
      }
    }

    #endregion
  }
}