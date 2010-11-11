//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.Collections;
  using System.Text;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The name value attribute collection.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public class NameValueAttributeCollection<T> : CollectionBase, IStateManager
    where T : NameValueAttribute
  {
    #region Constants and Fields

    /// <summary>
    /// The _ tracking.
    /// </summary>
    private bool _Tracking;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsTrackingViewState.
    /// </summary>
    bool IStateManager.IsTrackingViewState
    {
      get
      {
        return this._Tracking;
      }
    }

    #endregion

    #region Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public T this[string name]
    {
      get
      {
        foreach (T attr in this.InnerList)
        {
          if (attr.Name.ToLower() == name.ToLower())
          {
            return attr;
          }
        }

        return null;
      }
    }

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    public T this[int index]
    {
      get
      {
        return this.InnerList[index] as T;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="attr">
    /// The attr.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public void Add(T attr)
    {
      if (!this.Contains(attr))
      {
        this.InnerList.Add(attr);
      }
      else
      {
        throw new Exception("The attribute is exists!");
      }

      if (((IStateManager)this).IsTrackingViewState)
      {
        ((IStateManager)attr).TrackViewState();
        attr.SetViewStateDirty();
      }
    }

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void Add(string name, string value)
    {
      var instance = Activator.CreateInstance<T>();
      instance.Name = name;
      instance.Value = value;
      this.Add(instance);
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The contains.
    /// </returns>
    public bool Contains(string name)
    {
      return this[name] != null;
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="attribute">
    /// The attribute.
    /// </param>
    /// <returns>
    /// The contains.
    /// </returns>
    public bool Contains(T attribute)
    {
      return this.InnerList.Contains(attribute);
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="attr">
    /// The attr.
    /// </param>
    public void Remove(T attr)
    {
      this.InnerList.Remove(attr);
    }

    /// <summary>
    /// The to json string.
    /// </summary>
    /// <returns>
    /// The to json string.
    /// </returns>
    public string ToJSONString()
    {
      var jsonStr = new StringBuilder();
      foreach (T attr in this.InnerList)
      {
        if (jsonStr.Length > 0)
        {
          jsonStr.Append(",");
        }

        jsonStr.Append(attr.Name + ":\"" + attr.Value + "\"");
      }

      if (jsonStr.Length > 0)
      {
        return "{" + jsonStr + "}";
      }
      else
      {
        return "{}";
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IStateManager

    /// <summary>
    /// The load view state.
    /// </summary>
    /// <param name="state">
    /// The state.
    /// </param>
    void IStateManager.LoadViewState(object state)
    {
      var bags = state as object[];
      foreach (object bag in bags)
      {
        if (bag != null)
        {
          var attr = Activator.CreateInstance<T>();
          ((IStateManager)attr).LoadViewState(bag);
          this.Add(attr);
        }
      }
    }

    /// <summary>
    /// The save view state.
    /// </summary>
    /// <returns>
    /// The save view state.
    /// </returns>
    object IStateManager.SaveViewState()
    {
      var bags = new object[this.Count];
      int index = 0;
      foreach (NameValueAttribute item in this.InnerList)
      {
        bags[index++] = ((IStateManager)item).SaveViewState();
      }

      return bags;
    }

    /// <summary>
    /// The track view state.
    /// </summary>
    void IStateManager.TrackViewState()
    {
      this._Tracking = true;
      foreach (NameValueAttribute item in this.InnerList)
      {
        ((IStateManager)item).TrackViewState();
      }
    }

    #endregion

    #endregion
  }
}