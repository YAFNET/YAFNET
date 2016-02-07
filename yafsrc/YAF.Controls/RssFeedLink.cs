/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
  #region Using

    using System;
    using System.Web.UI;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

    /// <summary>
  /// The RSS feed link (with optional icon)
  /// </summary>
  public class RssFeedLink : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _attribute collection.
    /// </summary>
    protected AttributeCollection _attributeCollection;

    /// <summary>
    ///   The _localized label.
    /// </summary>
    protected LocalizedLabel _localizedLabel = new LocalizedLabel();

    /// <summary>
    ///   The _theme image.
    /// </summary>
    protected ThemeImage _themeImage = new ThemeImage();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "RssFeedLink" /> class.
    /// </summary>
    public RssFeedLink()
    {
      this.Load += this.RssFeedLink_Load;
      this._attributeCollection = new AttributeCollection(this.ViewState);
      this._localizedLabel.LocalizedTag = "RSSFEED";
      this.ImageThemeTag = "RSSFEED";
      this._themeImage.CssClass = "RssFeedIcon";
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or Sets additional rss feed url parameters.
    /// </summary>
    public string AdditionalParameters
    {
      get
      {
        return this.ViewState.ToTypeOrDefault("AdditionalParameters", string.Empty);
      }

      set
      {
        this.ViewState["AdditionalParameters"] = value;
      }
    }

    /// <summary>
    ///   Gets Attributes.
    /// </summary>
    public AttributeCollection Attributes
    {
      get
      {
        return this._attributeCollection;
      }
    }

    /// <summary>
    ///   Defaults to "rssfeedlink"
    /// </summary>
    [CanBeNull]
    public string CssClass
    {
      get
      {
        return (this.ViewState["CssClass"] != null) ? this.ViewState["CssClass"] as string : "rssfeedlink";
      }

      set
      {
        this.ViewState["CssClass"] = value;
      }
    }

    /// <summary>
    ///   Defaults to "Forum"
    /// </summary>
    public YafRssFeeds FeedType
    {
      get
      {
        return (this.ViewState["FeedType"] != null)
                 ? this.ViewState["FeedType"].ToEnum<YafRssFeeds>()
                 : YafRssFeeds.Forum;
      }

      set
      {
        this.ViewState["FeedType"] = value;
      }
    }

    /// <summary>
    ///   ThemePage for the optional button image
    /// </summary>
    public string ImageThemePage
    {
      get
      {
        return this._themeImage.ThemePage;
      }

      set
      {
        this._themeImage.ThemePage = value;
      }
    }

    /// <summary>
    ///   ThemeTag for the optional button image
    /// </summary>
    public string ImageThemeTag
    {
      get
      {
        return this._themeImage.ThemeTag;
      }

      set
      {
        this._themeImage.ThemeTag = value;
      }
    }

    /// <summary>
    ///   Gets or Sets if this is a link to atom feed. (Default <see langword = "false" />)
    /// </summary>
    public bool IsAtomFeed
    {
      get
      {
        return this.ViewState.ToTypeOrDefault("IsAtomFeed", false);
      }

      set
      {
        this.ViewState["IsAtomFeed"] = value;
      }
    }

    /// <summary>
    ///   Gets or Sets if a spacer should be inserted after the link. (Default <see langword = "false" />)
    /// </summary>
    public bool ShowSpacerAfter
    {
      get
      {
        return this.ViewState.ToTypeOrDefault("ShowSpacerAfter", false);
      }

      set
      {
        this.ViewState["ShowSpacerAfter"] = value;
      }
    }

    /// <summary>
    ///   Gets or Sets if a spacer should be inserted before the link. (Default <see langword = "false" />)
    /// </summary>
    public bool ShowSpacerBefore
    {
      get
      {
        return this.ViewState.ToTypeOrDefault("ShowSpacerBefore", false);
      }

      set
      {
        this.ViewState["ShowSpacerBefore"] = value;
      }
    }

    /// <summary>
    ///   Localized Page for the optional button text
    /// </summary>
    public string TextLocalizedPage
    {
      get
      {
        return this._localizedLabel.LocalizedPage;
      }

      set
      {
        this._localizedLabel.LocalizedPage = value;
      }
    }

    /// <summary>
    ///   Localized Tag for the optional button text
    /// </summary>
    public string TextLocalizedTag
    {
      get
      {
        return this._localizedLabel.LocalizedTag;
      }

      set
      {
        this._localizedLabel.LocalizedTag = value;
      }
    }

    /// <summary>
    ///   Localized Page for the optional link description (title)
    /// </summary>
    [CanBeNull]
    public string TitleLocalizedPage
    {
      get
      {
        return (this.ViewState["TitleLocalizedPage"] != null)
                 ? this.ViewState["TitleLocalizedPage"] as string
                 : String.Empty;
      }

      set
      {
        this.ViewState["TitleLocalizedPage"] = value;
      }
    }

    /// <summary>
    ///   Localized Tag for the optional link description (title)
    /// </summary>
    [CanBeNull]
    public string TitleLocalizedTag
    {
      get
      {
        return (this.ViewState["TitleLocalizedTag"] != null)
                 ? this.ViewState["TitleLocalizedTag"] as string
                 : string.Empty;
      }

      set
      {
        this.ViewState["TitleLocalizedTag"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get localized title.
    /// </summary>
    /// <returns>
    /// The get localized title.
    /// </returns>
    protected string GetLocalizedTitle()
    {
      if (this.Site != null && this.Site.DesignMode && this.TitleLocalizedTag.IsSet())
      {
        return "[TITLE:{0}]".FormatWith(this.TitleLocalizedTag);
      }
      else if (this.TitleLocalizedPage.IsSet() && this.TitleLocalizedTag.IsSet())
      {
        return this.GetText(this.TitleLocalizedPage, this.TitleLocalizedTag);
      }
      else if (this.TitleLocalizedTag.IsSet())
      {
        return this.GetText(this.TitleLocalizedTag);
      }

      return null;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter output)
    {
      if (this.Visible)
      {
        // get the title...
        string title = this.GetLocalizedTitle();

        output.BeginRender();

        if (this.ShowSpacerBefore)
        {
          output.Write(" |&nbsp;".FormatWith());
        }

        output.WriteBeginTag("a");
        output.WriteAttribute("id", this.ClientID);

        if (this.CssClass.IsSet())
        {
          output.WriteAttribute("class", this.CssClass);
        }

        if (title.IsSet())
        {
          output.WriteAttribute("title", title);
        }

        output.WriteAttribute(
          "href", 
          YafBuildLink.GetLink(
            ForumPages.rsstopic, 
            "pg={0}&ft={1}{2}", 
            this.FeedType.ToInt(), 
            this.IsAtomFeed ? 1 : 0, 
            this.AdditionalParameters.IsNotSet() ? string.Empty : "&{0}".FormatWith(this.AdditionalParameters)));

        // handle additional attributes (if any)
        if (this._attributeCollection.Count > 0)
        {
          // add attributes...
          foreach (string key in this._attributeCollection.Keys)
          {
            if (key.ToLower().StartsWith("on") || key.ToLower() == "rel" || key.ToLower() == "target")
            {
              // only write javascript attributes -- and a few other attributes...
              output.WriteAttribute(key, this._attributeCollection[key]);
            }
          }
        }

        output.Write(HtmlTextWriter.TagRightChar);

        output.WriteBeginTag("span");
        output.Write(HtmlTextWriter.TagRightChar);

        // render the optional controls (if any)
        base.Render(output);
        output.WriteEndTag("span");

        output.WriteEndTag("a");

        if (this.ShowSpacerAfter)
        {
          output.Write("&nbsp;| ".FormatWith());
        }

        output.EndRender();
      }
    }

    /// <summary>
    /// The rss icon link_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void RssFeedLink_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // render the text if available
      if (this._localizedLabel.LocalizedTag.IsSet())
      {
        this.Controls.Add(this._localizedLabel);
      }

      if (this._themeImage.ThemeTag.IsSet())
      {
        // add the theme image...
        this.Controls.Add(this._themeImage);
      }
    }

    #endregion
  }
}