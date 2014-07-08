/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
  using System.ComponentModel;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Core;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// Rendered DIV container
  /// </summary>
  public class Container : Control, INamingContainer
  {
    #region Properties

    /// <summary>
    ///   Css Clas for surrounding DIV
    /// </summary>
    [Browsable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("CssClass")]
    public string CSSClass { get; set; }

    /// <summary>
    ///   Contents to render within the container
    /// </summary>
    [Browsable(false)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [TemplateContainer(typeof(PlaceHolder))]
    [DefaultValue(typeof(ITemplate), "")]
    [Description("Contents")]
    public ITemplate Contents { get; set; }

    /// <summary>
    ///   Hide Text required for expanding/collapsing container
    /// </summary>
    [Browsable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("HideText")]
    public string HideText { get; set; }

    /// <summary>
    ///   If true, renders SLIDING doors technique additional DIV tags
    /// </summary>
    [Browsable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("RoundedCorners")]
    public bool RoundedCorners { get; set; }

    /// <summary>
    ///   Hide Text required for expanding/collapsing container
    /// </summary>
    [Browsable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("ShowText")]
    public string ShowText { get; set; }

    /// <summary>
    ///   If present renders a h2 html tag
    /// </summary>
    [Browsable(true)]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Description("Title")]
    public string Title { get; set; }

    /// <summary>
    ///   Client ID for Contents DIV - Required for Javascript
    /// </summary>
    private string ContentsClientID
    {
      get
      {
        return this.ClientID + "_content";
      }
    }

    /// <summary>
    ///   Client ID for HTML Link - Required for Javascript
    /// </summary>
    private string LinkClientID
    {
      get
      {
        return this.ClientID + "_expandLink";
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The data bind.
    /// </summary>
    public override void DataBind()
    {
      this.CreateChildControls();
      this.ChildControlsCreated = true;
      base.DataBind();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
      // Render all Contents in the Contents Template
      var templateControl = new PlaceHolder();
      if (this.Contents != null)
      {
        this.Contents.InstantiateIn(templateControl);
      }

      this.Controls.Add(templateControl);
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      base.OnInit(e);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      base.OnPreRender(e);
    }

    /// <summary>
    /// Renders surrounding div
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      if (!this.Visible)
      {
        return;
      }

      writer.BeginRender();

      writer.WriteBeginTag("div");
      writer.WriteAttribute("id", this.ClientID);
      if (this.CSSClass.IsSet())
      {
        // Make sure CSS Class is not empty before rendering attribute
        writer.WriteAttribute("class", this.CSSClass);
      }

      writer.Write(">");
      writer.WriteLine();

      if (this.RoundedCorners)
      {
        this.RenderRoundedCorners(writer); // Render additional DIVs for Sliding doors technique
      }
      else
      {
        this.RenderContents(writer); // Render contents div and Contents
      }

      writer.WriteLine();
      writer.WriteEndTag("div");
      writer.WriteLine();

      writer.EndRender();
    }

    /// <summary>
    /// Renders contents div and childcontrols
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderContents([NotNull] HtmlTextWriter writer)
    {
      // Expandable DIV
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "expandablePanel");
      writer.Write(">");
      writer.WriteLine();

      // Container DIV
      writer.WriteBeginTag("div");
      writer.WriteAttribute("id", this.ContentsClientID);
      writer.WriteAttribute("class", "contents");
      writer.Write(">");
      writer.WriteLine();

      if (this.Title.IsSet())
      {
        writer.WriteFullBeginTag("h2");
        writer.Write(this.Title);
        writer.WriteEndTag("h2");
        writer.WriteLine();
      }

      base.Render(writer);

      writer.WriteEndTag("div");
      writer.WriteLine();

      // Footer DIV
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "footer");
      writer.Write(">");
      writer.WriteLine();
      writer.WriteLine(this.ExpandLink()); // Render Show/Hide
      writer.WriteLine();
      writer.WriteEndTag("div");
      writer.WriteLine();

      // end expandable div
      writer.WriteEndTag("div");
      writer.WriteLine();
    }

    /// <summary>
    /// Renders surrounding DIV for Sliding Doors Rounded Corners technique
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderRoundedCorners([NotNull] HtmlTextWriter writer)
    {
      // Write Begining div
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "roundedHeader");
      writer.Write(">");
      writer.Write("<div class=\"rightCorner\"></div>");
      writer.WriteEndTag("div");

      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "roundedContents");
      writer.Write(">");
      this.RenderContents(writer); // Render all Content Controls
      writer.WriteEndTag("div");

      // Write End div
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "roundedFooter");
      writer.Write(">");
      writer.Write("<div class=\"rightCorner\"></div>");
      writer.WriteEndTag("div");
    }

    /// <summary>
    /// Summary description for ForumJump.
    /// </summary>
    /// <returns>
    /// The expand link.
    /// </returns>
    private string ExpandLink()
    {
      string jsHref = "javascript:toggleContainer('{0}', '{1}', '{2}', '{3}');".FormatWith(
        this.ContentsClientID, this.LinkClientID, this.ShowText, this.HideText);
      string link = "<a id=\"{0}\" href=\"{1}\">{2}</a>".FormatWith(this.LinkClientID, jsHref, this.ShowText);
      return link;
    }

    #endregion
  }
}