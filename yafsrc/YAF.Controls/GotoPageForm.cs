/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The goto page forum event args.
    /// </summary>
    public class GotoPageForumEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GotoPageForumEventArgs" /> class.
        /// </summary>
        /// <param name="gotoPage">
        ///     The goto page.
        /// </param>
        public GotoPageForumEventArgs(int gotoPage)
        {
            this.GotoPage = gotoPage;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets GotoPage.
        /// </summary>
        public int GotoPage { get; set; }

        #endregion
    }

    /// <summary>
    ///     The goto page form.
    /// </summary>
    public class GotoPageForm : BaseControl
    {
        #region Fields

        /// <summary>
        ///     The _goto button.
        /// </summary>
        private readonly Button gotoButton = new Button();

        /// <summary>
        ///     The _goto text box.
        /// </summary>
        private readonly TextBox gotoTextBox = new TextBox();

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
        ///     Gets GotoButtonClientID.
        /// </summary>
        [NotNull]
        public string GotoButtonClientID => this.gotoButton.ClientID;

        /// <summary>
        ///     Gets or sets GotoPageValue.
        /// </summary>
        public int GotoPageValue
        {
            get
            {
                return this.gotoPageValue;
            }

            set
            {
                this.gotoPageValue = value;
            }
        }

        /// <summary>
        ///     Gets GotoTextBoxClientID.
        /// </summary>
        [NotNull]
        public string GotoTextBoxClientID => this.gotoTextBox.ClientID;

        #endregion

        #region Methods

        /// <summary>
        ///     The build form.
        /// </summary>
        protected void BuildForm()
        {
            var headerLabel = new HtmlGenericControl("label");

            headerLabel.Controls.Add(new Literal { Text = this.GetText("COMMON", "GOTOPAGE_HEADER") });

            this.Controls.Add(headerLabel);

            var inputGroup = new HtmlGenericControl("div");

            inputGroup.Attributes.Add("class", "input-group");

            // text box...
            this.gotoTextBox.ID = this.GetExtendedID("GotoTextBox");
            this.gotoTextBox.CssClass = "form-control";

            inputGroup.Controls.Add(this.gotoTextBox);

            var groupBtn = new HtmlGenericControl("span");
            groupBtn.Attributes.Add("class", "input-group-btn");

            this.gotoButton.ID = this.GetExtendedID("GotoButton");
            this.gotoButton.CssClass = "btn btn-primary";
            this.gotoButton.CausesValidation = false;
            this.gotoButton.UseSubmitBehavior = false;
            this.gotoButton.Click += this.GotoButtonClick;
            this.gotoButton.Text = this.GetText("COMMON", "GO");

            groupBtn.Controls.Add(this.gotoButton);

            inputGroup.Controls.Add(groupBtn);

            this.Controls.Add(inputGroup);
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
        ///     The on init.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
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
            base.Render(writer);
        }

        #endregion
    }
}