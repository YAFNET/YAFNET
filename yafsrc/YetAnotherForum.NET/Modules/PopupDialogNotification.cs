/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Modules
{
    #region Using

    using System;
    using System.Web;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The popup dialog notification.
    /// </summary>
    public class PopupDialogNotification : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets ShowModalFunction.
        /// </summary>
        public string ShowModalFunction => $"ShowPopupDialogNotification{this.ClientID}";

        #endregion

        #region Methods

        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // add js for client-side error settings...
            var javaScriptFunction =
                $@"function {this.ShowModalFunction}(newErrorStr, newErrorType, script) {{ if (newErrorStr != null && newErrorStr != """") {{
                      var iconFA = '';

                      if (newErrorType == 'warning') {{
                          iconFA = 'fa fa-exclamation-triangle';
                      }}
                      else if (newErrorType == 'danger') {{
                          iconFA = 'fa fa-exclamation-triangle';
                      }}
                      else if (newErrorType == 'info') {{
                          iconFA = 'fa fa-info-circle';
                      }}
                      else if (newErrorType == 'success') {{
                          iconFA = 'fa fa-check';
                      }}

                      if (script != null && script != """")
                      {{
                         $('.modal-backdrop').remove();
                         $('#' + script).modal('show');
                      }}

                      $.notify({{
                                   message: newErrorStr,
                                   icon: iconFA
                            }},
                            {{
                                  type: newErrorType,
                                  element: 'body', position: null, placement: {{ from: 'top', align: 'center' }}, delay: {this.Get<YafBoardSettings>().MessageNotifcationDuration} * 1000
                        }});}} }}";

            // Override Notification Setting if Mobile Device is used
            if (this.Get<YafBoardSettings>().NotifcationNativeOnMobile
                && this.Get<HttpRequestBase>().Browser.IsMobileDevice)
            {
                // Show as Modal Dialog
                javaScriptFunction =
                    $@"function {this.ShowModalFunction}(newErrorStr) {{  if (newErrorStr != null && newErrorStr != """") {{
                                                    alert(newErrorStr);
                      }} }}";
            }

            YafContext.Current.PageElements.RegisterJsBlock(thisControl: this, name: this.ShowModalFunction, script: javaScriptFunction);

            base.OnPreRender(e: e);
        }

        #endregion
    }
}