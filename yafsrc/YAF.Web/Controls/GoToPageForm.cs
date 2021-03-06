﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Web.EventsArgs;

    #endregion

    /// <summary>
    ///     The goto page form.
    /// </summary>
    public class GotoPageForm : BaseControl
    {
        #region Fields

        /// <summary>
        ///     The _goto button.
        /// </summary>
        private readonly Button gotoButton = new();

        /// <summary>
        ///     The _goto text box.
        /// </summary>
        private readonly TextBox gotoTextBox = new();

        /// <summary>
        ///     The _goto page value.
        /// </summary>
        private int gotoPageValue;

        #endregion

        #region Public Events

        /// <summary>
        ///     The goto page click.
        /// </summary>
        public event EventHandler<GotoPageForumEventArgs> GotoPageClick;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets GotoPageValue.
        /// </summary>
        public int GotoPageValue
        {
            get => this.gotoPageValue;

            set => this.gotoPageValue = value;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The build form.
        /// </summary>
        protected void BuildForm()
        {
            var headerLabel = new Label
                                  {
                                      Text = this.GetText("COMMON", "GOTOPAGE_HEADER"),
                                      AssociatedControlID = this.gotoTextBox.ID
                                  };

            this.Controls.Add(headerLabel);

            var toolbar = new HtmlGenericControl("div");

            toolbar.Attributes.Add("class", "btn-toolbar");
            toolbar.Attributes.Add("role", "toolbar");

            var inputGroup = new HtmlGenericControl("div");

            inputGroup.Attributes.Add("class", "input-group me-2 w-50");

            // text box...
            this.gotoTextBox.ID = this.GetExtendedID("GotoTextBox");
            this.gotoTextBox.CssClass = "form-control form-pager";
            this.gotoTextBox.TextMode = TextBoxMode.Number;

            inputGroup.Controls.Add(this.gotoTextBox);

            toolbar.Controls.Add(inputGroup);

            this.gotoButton.ID = this.GetExtendedID("GotoButton");
            this.gotoButton.CssClass = "btn btn-primary";
            this.gotoButton.CausesValidation = false;
            this.gotoButton.UseSubmitBehavior = false;
            this.gotoButton.Click += this.GotoButtonClick;
            this.gotoButton.Text = this.GetText("COMMON", "GO");

            toolbar.Controls.Add(this.gotoButton);

            this.Controls.Add(toolbar);
        }

        /// <summary>
        ///     The goto button click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected void GotoButtonClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.GotoPageClick != null)
            {
                // attempt to parse the page value...
                if (int.TryParse(this.gotoTextBox.Text.Trim(), out this.gotoPageValue))
                {
                    // valid, fire the event...
                    this.GotoPageClick(this, new GotoPageForumEventArgs(this.GotoPageValue));
                }
            }

            // clear the old value...
            this.gotoTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            this.BuildForm();
        }

        /// <summary>
        ///     The on load.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnLoad([NotNull] EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        ///     The render.
        /// </summary>
        /// <param name="writer">
        ///     The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            this.PageContext.PageElements.RegisterJsBlock(
                nameof(JavaScriptBlocks.DropDownToggleJs),
                JavaScriptBlocks.DropDownToggleJs());

            base.Render(writer);
        }

        #endregion
    }
}