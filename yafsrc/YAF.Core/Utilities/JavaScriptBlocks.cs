/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
namespace YAF.Core.Utilities;

using System.Collections.Generic;
using System.Text;

using YAF.Core.Context.Start;

/// <summary>
/// Contains the Java Script Blocks
/// </summary>
public static class JavaScriptBlocks
{
    /// <summary>
    ///   Gets the script for changing the image caption.
    /// </summary>
    /// <returns></returns>
    [NotNull]
    public static string ChangeImageCaptionJs =>
        $$"""
          function changeImageCaption(imageId, txtTitleId) {
              const newImgTitleTxt = document.getElementById(txtTitleId).value;

              fetch("{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Album/ChangeImageCaption",
                      {
                          method: "POST",
                          body: JSON.stringify({ ImageId: imageId, NewCaption: newImgTitleTxt }),
                          headers: {
                              "Accept": "application/json",
                              "Content-Type": "application/json;charset=utf-8"
                          }
                      }).then(res => res.json())
                  .then(response => {
                      changeTitleSuccess(response);
                  }).catch(function(error) {
                      errorLog(error);
                  });
          }
          
          """;

    /// <summary>
    ///   Gets the script for album/image title/image callback.
    /// </summary>
    /// <returns>
    ///   the callback success js.
    /// </returns>
    [NotNull]
    public static string AlbumCallbackSuccessJs =>
        """
        function changeTitleSuccess(res){
                          txtTitleVar =  document.getElementById("txtTitle" + res.Id);
                          spnTitleVar = document.getElementById("spnTitle" + res.Id);
                          txtTitleVar =  document.getElementById("txtTitle" + res.Id);
                          spnTitleVar.firstChild.nodeValue = res.NewTitle;
                          txtTitleVar.disabled = false;
                          spnTitleVar.style.display = 'inline';
                          txtTitleVar.style.display='none';}
        """;

    /// <summary>
    /// Gets the multi quote callback success JS.
    /// </summary>
    [NotNull]
    public static string MultiQuoteCallbackSuccessJs =>
        """
          function multiQuoteSuccess(res) {
              const multiQuoteButton = document.getElementById(res.Id).parentNode;
              multiQuoteButton.setAttribute("class", res.NewTitle);
          }
          """;

    /// <summary>
    /// Gets the multi quote button JS.
    /// </summary>
    [NotNull]
    public static string MultiQuoteButtonJs =>
        $$"""
           function handleMultiQuoteButton(button, msgId, tpId) {
               const multiQuoteButton = {};
               multiQuoteButton.ButtonId = button.id;
               multiQuoteButton.IsMultiQuoteButton = button.checked;
               multiQuoteButton.MessageId = msgId;
               multiQuoteButton.TopicId = tpId;
               multiQuoteButton.ButtonCssClass = document.getElementById(button.id).parentNode.className;
           
               fetch("{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/MultiQuote/HandleMultiQuote",
                       {
                           method: "POST",
                           body: JSON.stringify(multiQuoteButton),
                           headers: {
                               "Accept": "application/json",
                               "Content-Type": "application/json;charset=utf-8"
                           }
                       }).then(res => res.json())
                   .then(response => {
                       multiQuoteSuccess(response);
                   }).catch(function(error) {
                       errorLog(error);
                   });
           }
           """;

    /// <summary>
    /// Gets Board Tags JavaScript
    /// </summary>
    /// <param name="inputId">
    /// The input Id.
    /// </param>
    /// <param name="hiddenId">
    /// the hidden id
    /// </param>
    /// <param name="existingTags">
    /// the existing topic tags id
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string GetBoardTagsJs(string inputId, string hiddenId, IEnumerable<string> existingTags = null)
    {
        var tags = new StringBuilder();

        if (!existingTags.NullOrEmpty())
        {
            existingTags.ForEach(tag => tags.Append($"{Config.JQueryAlias}('#{inputId}').append($('<option>', {{value:'{tag}', text:'{tag}', selected: true}}));"));
        }
        
        return $$"""
                 {{tags}}{{Config.JQueryAlias}}("#{{inputId}}").select2({
                             tags: true,
                             tokenSeparators: [',', ' '],
                             ajax: {
                                 url: '{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Tags/GetBoardTags',
                                 type: 'POST',
                                 dataType: 'json',
                                 data: function(params) {
                                     var query = {
                                         ForumId: 0,
                                         UserId: 0,
                                         PageSize: 15,
                                         Page: params.page || 0,
                                         SearchTerm: params.term || ''
                                     }
                                     return query;
                                 },
                                 error: errorLog,
                                 processResults: function(data, params) {
                                     params.page = params.page || 0;
                 
                                     var resultsPerPage = 15 * 2;
                 
                                     var total = params.page == 0 ? data.Results.length : resultsPerPage;
                 
                                     return {
                                         results: data.Results,
                                         pagination: {
                                             more: total < data.Total
                                         }
                                     }
                                 }
                             },
                             allowClearing: false,
                             width: '100%',
                             theme: 'bootstrap-5',
                             {{BoardContext.Current.Get<ILocalization>().GetText("SELECT_LOCALE_JS")}}
                         }).on("select2:select", function (e) {
                                   $("#{{hiddenId}}").val($(this).select2('data').map(x => x.text).join());
                         });
                 """;
    }

    /// <summary>
    /// The cookie consent JS.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string CookieConsentJs()
    {
        return """
               function addConsentCookie(name, value, days) {
                       var expires;
               
                       if (days) {
                           var date = new Date();
                           date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                           expires = "; expires=" + date.toGMTString();
                       } else {
                           expires = "";
                       }
                       document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + expires + "; path=/";
                   }
               """;
    }

    /// <summary>
    /// Java Script events for Album pages.
    /// </summary>
    /// <param name="albumEmptyTitle">
    /// The Album Empty Title.
    /// </param>
    /// <param name="imageEmptyCaption">
    /// The Image Empty Caption.
    /// </param>
    /// <returns>
    /// The album events JS.
    /// </returns>
    public static string AlbumEventsJs([NotNull] string albumEmptyTitle, [NotNull] string imageEmptyCaption)
    {
        return $$"""
                  function showTexBox(spnTitleId) {
                      {
                          const spnTitleVar = document.getElementById("spnTitle" + spnTitleId.substring(8)),
                              txtTitleVar = document.getElementById("txtTitle" + spnTitleId.substring(8));
                  
                          if (spnTitleVar.firstChild != null) txtTitleVar.setAttribute("value", spnTitleVar.firstChild.nodeValue);
                          if (spnTitleVar.firstChild.nodeValue == "{{albumEmptyTitle}}" || spnTitleVar.firstChild.nodeValue == "{{imageEmptyCaption}}") {
                              {
                                  txtTitleVar.value = "";
                                  spnTitleVar.firstChild.nodeValue = "";
                              }
                          }
                          txtTitleVar.style.display = "inline";
                          spnTitleVar.style.display = "none";
                          txtTitleVar.focus();
                      }
                  }
                  
                  function resetBox(txtTitleId, isAlbum) {
                      {
                          const spnTitleVar = document.getElementById("spnTitle" + txtTitleId.substring(8)),
                              txtTitleVar = document.getElementById(txtTitleId);
                  
                          txtTitleVar.style.display = "none";
                          txtTitleVar.disabled = false;
                          spnTitleVar.style.display = "inline";
                          if (spnTitleVar.firstChild != null) txtTitleVar.value = spnTitleVar.firstChild.nodeValue;
                          if (spnTitleVar.firstChild.nodeValue == "") {
                              {
                                  txtTitleVar.value = "";
                                  if (isAlbum) spnTitleVar.firstChild.nodeValue = "{{albumEmptyTitle}}";
                                  else spnTitleVar.firstChild.nodeValue = "{{imageEmptyCaption}}";
                              }
                          }
                      }
                  }
                  
                  function checkKey(event, handler, id, isAlbum) {
                      {
                          if ((event.keyCode == 13) || (event.which == 13)) {
                              {
                                  if (event.preventDefault) event.preventDefault();
                                  event.cancel = true;
                                  event.returnValue = false;
                                  if (spnTitleVar.firstChild.nodeValue != txtTitleVar.value) {
                                      {
                                          handler.disabled = true;
                                          if (isAlbum == true) changeAlbumTitle(id, handler.id);
                                          else changeImageCaption(id, handler.id);
                                      }
                                  } else resetBox(handler.id, isAlbum);
                              }
                          } else if ((event.keyCode == 27) || (event.which == 27)) resetBox(handler.id, isAlbum);
                      }
                  }
                  
                  function blurTextBox(txtTitleId, id, isAlbum) {
                      {
                          const spnTitleVar = document.getElementById("spnTitle" + txtTitleId.substring(8)),
                              txtTitleVar = document.getElementById(txtTitleId);
                  
                          if (spnTitleVar.firstChild != null) {
                              {
                                  if (spnTitleVar.firstChild.nodeValue != txtTitleVar.value) {
                                      {
                                          txtTitleVar.disabled = true;
                                          if (isAlbum == true) changeAlbumTitle(id, txtTitleId);
                                          else changeImageCaption(id, txtTitleId);
                                      }
                                  } else resetBox(txtTitleId, isAlbum);
                              }
                          } else resetBox(txtTitleId, isAlbum);
                      }
                  }
                  """;
    }

    /// <summary>
    /// Blocks the UI JS
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string BlockUiFunctionJs([NotNull] string messageId)
    {
        return $$"""
                  var modal = new bootstrap.Modal(document.getElementById("{{messageId}}"),
                      {
                          backdrop: "static",
                          keyboard: false
                      });
                  
                  modal.show();
                                       
                  """;
    }

    /// <summary>
    /// Gets the Bootstrap Tab Load JS.
    /// </summary>
    /// <param name="tabId">The tab Id.</param>
    /// <param name="hiddenId">The hidden field id.</param>
    /// <returns>
    /// Returns the the Bootstrap Tab Load JS string
    /// </returns>
    public static string BootstrapTabsLoadJs([NotNull] string tabId, string hiddenId)
    {
        return $$"""
                  document.addEventListener("DOMContentLoaded", function () {
                      const selectedTab = document.getElementById("{{hiddenId}}"),
                          tabId = selectedTab.value !== "" ? selectedTab.value : "View1",
                          tab = new bootstrap.Tab('#{{tabId}} a[href="#' + tabId + '"]');
                  
                      tab.show();
                  
                      document.querySelectorAll('[data-bs-toggle="tab"]').forEach(tabEl => {
                          tabEl.addEventListener("click",
                              (e) => {
                                  var tabLink = e.target.href.split('#');
                  
                                  if (tabLink.length > 1) {
                                      selectedTab.value = tabLink[1];
                                  }
                              });
                      });
                  });
                  """;
    }

    /// <summary>
    /// The drop down toggle JS.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string DropDownToggleJs()
    {
        return """
                 document.querySelectorAll(".dropdown-menu").forEach(dropdown => {
                 
                     dropdown.addEventListener("click", (e) => {
                         if (e.target.type === "button") {
                             new bootstrap.Dropdown(e.target).show();
                         }
                         else {
                             e.stopPropagation();
                         }
                     });
                 });
                 """;
    }

    /// <summary>
    /// The collapse toggle JS.
    /// </summary>
    /// <param name="hideText">
    /// The hide Text.
    /// </param>
    /// <param name="showText">
    /// The show Text.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string CollapseToggleJs([NotNull] string hideText, [NotNull] string showText)
    {
        return $$"""
                  document.addEventListener("DOMContentLoaded", function () {
                      document.querySelectorAll('a[data-bs-toggle="collapse"]').forEach(button => {
                          if (button.getAttribute("aria-expanded") === "false") {
                              button.innerHTML = '<i class="fa fa-caret-square-up fa-fw"></i>&nbsp;{{hideText}}';
                          } else {
                              button.innerHTML = '<i class="fa fa-caret-square-down fa-fw"></i>&nbsp;{{showText}}';
                          }
                      });
                  });
                  """;
    }

    /// <summary>
    /// Load Go to Anchor
    /// </summary>
    /// <param name="anchor">
    /// The anchor.
    /// </param>
    /// <returns>
    /// The load goto anchor.
    /// </returns>
    public static string LoadGotoAnchor([NotNull] string anchor)
    {
        return $$"""
                  Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadGotoAnchor);
                  function loadGotoAnchor() {
                      const htmlElement = document.querySelector("html");
                      htmlElement.style.scrollBehavior = "auto";
                  
                      document.getElementById("{{anchor}}").scrollIntoView();
                  
                      htmlElement.style.scrollBehavior = "smooth";
                  }
                  """;
    }

    /// <summary>
    /// Generates Modal Dialog Script
    /// </summary>
    /// <param name="openLink">
    /// The Open Link, that opens the Modal Dialog.
    /// </param>
    /// <param name="dialogId">
    /// The Id or CSS Class of the Dialog Content
    /// </param>
    /// <returns>
    /// The YAF modal dialog Load JS.
    /// </returns>
    public static string LoginBoxLoadJs([NotNull] string openLink, [NotNull] string dialogId)
    {
        return $$"""
                  document.addEventListener("DOMContentLoaded", function () {
                      document.querySelectorAll("{{openLink}}").forEach(button => {
                          button.addEventListener("click", () => {
                              const loginModal = new bootstrap.Modal(document.getElementById("{{dialogId}}"));
                              loginModal.show();
                          });
                      });
                  });
                  """;
    }

    /// <summary>
    /// script for the addThanks button
    /// </summary>
    /// <param name="removeThankBoxHtml">
    /// HTML code for the "Remove Thank" button
    /// </param>
    /// <returns>
    /// The add thanks JS.
    /// </returns>
    public static string AddThanksJs([NotNull] string removeThankBoxHtml)
    {
        return $$"""
                  function addThanks(messageId) {
                      fetch("{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/ThankYou/AddThanks/" + messageId,
                              {
                                  method: "POST",
                                  headers: {
                                      "Accept": "application/json",
                                      "Content-Type": "application/json;charset=utf-8"
                                  }
                              }).then(res => res.json())
                          .then(response => {
                  
                              document.getElementById('dvThanksInfo' + response.MessageID).innerHTML = response.ThanksInfo;
                              document.getElementById('dvThankBox' + response.MessageID).innerHTML = {{removeThankBoxHtml}};
                  
                              document.querySelectorAll(".thanks-popover").forEach(pop => {
                                  const popover = new bootstrap.Popover(pop,
                                      {
                                          template:
                                              '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
                                      });
                              });
                          }).catch(function (error) {
                              errorLog(error);
                          });
                  }
                  """;
    }

    /// <summary>
    /// script for the removeThanks button
    /// </summary>
    /// <param name="addThankBoxHtml">
    /// The Add Thank Box HTML.
    /// </param>
    /// <returns>
    /// The remove thanks JS.
    /// </returns>
    public static string RemoveThanksJs([NotNull] string addThankBoxHtml)
    {
        return $$"""
                  function removeThanks(messageId) {
                      fetch("{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/ThankYou/RemoveThanks/" + messageId,
                              {
                                  method: "POST",
                                  headers: {
                                      "Accept": "application/json",
                                      "Content-Type": "application/json;charset=utf-8"
                                  }
                              }).then(res => res.json())
                          .then(response => {
                  
                              document.getElementById("dvThanksInfo" + response.MessageID).innerHTML = response.ThanksInfo;
                              document.getElementById("dvThankBox" + response.MessageID).innerHTML = {{addThankBoxHtml}};
                          }).catch(function(error) {
                              errorLog(error);
                          });
                  }
                  """;
    }

    /// <summary>
    /// Creates the yaf editor js.
    /// </summary>
    /// <param name="editorId">The editor identifier.</param>
    /// <param name="urlTitle">The URL title.</param>
    /// <param name="urlDescription">The URL description.</param>
    /// <param name="urlImageTitle">The URL image title.</param>
    /// <param name="urlImageDescription">The URL image description.</param>
    /// <param name="description">The description.</param>
    /// <returns>System.String.</returns>
    [NotNull]
    public static string CreateYafEditorJs(
        [NotNull] string editorId,
        [NotNull] string urlTitle,
        [NotNull] string urlDescription,
        [NotNull] string urlImageTitle,
        [NotNull] string urlImageDescription,
        [NotNull] string description)
    {
        return $$"""
                 var {{editorId}}=new yafEditor("{{editorId}}", "{{urlTitle}}", "{{urlDescription}}", "{{urlImageTitle}}", "{{urlImageDescription}}", "{{description}}");
                                   function setStyle(style,option) {
                                            {{editorId}}.FormatText(style,option);
                                   }
                                   function insertAttachment(id,url) {
                                            {{editorId}}.FormatText("attach", id);
                                   }
                                   
                                   {{Config.JQueryAlias}}(".BBCodeEditor").suggest("@",
                                           {
                             data: function(q) {
                                 if (q && q.length > 3) {
                                     return {{Config.JQueryAlias}}.getJSON("{{BoardInfo.ForumClientFileRoot}}resource.ashx?users=" + q);
                                            }
                                           },
                                           map: function(user)
                                          {
                     return {
                         value: "[userlink]" + user.name + "[/userlink]",
                         text: "<i class='fas fa-user me-2'></i><strong>" + user.name + "</strong>"
                     };
                 }
                 });

                 """;
    }

    /// <summary>
    /// The CodeMirror SQL Load JS.
    /// </summary>
    /// <param name="editorId">
    /// The editor Id.
    /// </param>
    /// <param name="mime">
    /// The mime.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string CodeMirrorSqlLoadJs(
        [NotNull] string editorId,
        [NotNull] string mime)
    {
        return $$$"""
                 window.onload = function() {
                   window.editor = CodeMirror.fromTextArea(document.getElementById('{{{editorId}}}'), {
                     mode: "{{{mime}}}",
                     indentWithTabs: true,
                     smartIndent: true,
                     lineNumbers: true,
                     matchBrackets : true,
                     theme: "monokai",
                     autofocus: true,
                     extraKeys: {"Ctrl-Space": "autocomplete"},
                     hintOptions: {tables: {
                       users: ["name", "score", "birthDate"],
                       countries: ["name", "population", "size"]
                     }}
                   });
                 };
                 """;
    }

    /// <summary>
    /// Gets the FileUpload Java Script.
    /// </summary>
    /// <param name="acceptedFileTypes">
    /// The accepted file types.
    /// </param>
    /// <param name="maxFileSize">
    /// Maximum size of the file.
    /// </param>
    /// <param name="fileUploaderUrl">
    /// The file uploader URL.
    /// </param>
    /// <param name="imageMaxWidth">
    /// The image Max Width.
    /// </param>
    /// <param name="imageMaxHeight">
    /// The image Max Height.
    /// </param>
    /// <returns>
    /// Returns the FileUpload Java Script.
    /// </returns>
    [NotNull]
    public static string FileAutoUploadLoadJs(
        [NotNull] string acceptedFileTypes,
        [NotNull] int maxFileSize,
        [NotNull] string fileUploaderUrl,
        [NotNull] int imageMaxWidth,
        [NotNull] int imageMaxHeight)
    {
        return $@"{Config.JQueryAlias}('.BBCodeEditor').yafFileUpload({{
                url: '{fileUploaderUrl}',
                acceptFileTypes: /(\.|\/)({acceptedFileTypes})$/i,
                imageMaxWidth: {imageMaxWidth},
                imageMaxHeight: {imageMaxHeight},
                autoUpload: true,
                disableImageResize: /Android(?!.*Chrome)|Opera/
                .test(window.navigator && navigator.userAgent),
                dataType: 'json',
                {(maxFileSize > 0 ? $"maxFileSize: {maxFileSize}," : string.Empty)}
                done: function (e, data) {{
                    insertAttachment( data.result[0].fileID, data.result[0].fileID );
                }},
                formData: {{
                    userID: '{BoardContext.Current.PageUserID}'
                }},
                dropZone: {Config.JQueryAlias}('.BBCodeEditor'),
                pasteZone: {Config.JQueryAlias}('.BBCodeEditor')
            }});";
    }

    /// <summary>
    /// Gets the FileUpload Java Script.
    /// </summary>
    /// <param name="acceptedFileTypes">
    /// The accepted file types.
    /// </param>
    /// <param name="maxFileSize">
    /// Maximum size of the file.
    /// </param>
    /// <param name="fileUploaderUrl">
    /// The file uploader URL.
    /// </param>
    /// <param name="imageMaxWidth">
    /// The image Max Width.
    /// </param>
    /// <param name="imageMaxHeight">
    /// The image Max Height.
    /// </param>
    /// <returns>
    /// Returns the FileUpload Java Script.
    /// </returns>
    [NotNull]
    public static string FileUploadLoadJs(
        [NotNull] string acceptedFileTypes,
        [NotNull] int maxFileSize,
        [NotNull] string fileUploaderUrl,
        [NotNull] int imageMaxWidth,
        [NotNull] int imageMaxHeight)
    {
        return $@"{Config.JQueryAlias}(function() {{

            {Config.JQueryAlias}('#fileupload').yafFileUpload({{
                url: '{fileUploaderUrl}',
                acceptFileTypes: /(\.|\/)({acceptedFileTypes})$/i,
                imageMaxWidth: {imageMaxWidth},
                imageMaxHeight: {imageMaxHeight},
                disableImageResize: /Android(?!.*Chrome)|Opera/
                .test(window.navigator && navigator.userAgent),
                dataType: 'json',
                {(maxFileSize > 0 ? $"maxFileSize: {maxFileSize}," : string.Empty)}
                start: function (e) {{
                    {Config.JQueryAlias}('#fileupload .alert-danger').toggle();
                }},
                done: function (e, data) {{
                    insertAttachment(data.result[0].fileID, data.result[0].fileID);
                    {Config.JQueryAlias}('#fileupload').find('.files li:first').remove();

                    if ({Config.JQueryAlias}('#fileupload').find('.files li').length == 0) {{
                     {Config.JQueryAlias}('#UploadDialog').modal('hide');
                     {Config.JQueryAlias}('#fileupload .alert-danger').toggle();

                        var pageSize = 5;
                        var pageNumber = 0;
                        getPaginationData(pageSize, pageNumber, false);
                    }}
                }},
                formData: {{
                    userID: '{BoardContext.Current.PageUserID}'
                }},
                dropZone: {Config.JQueryAlias}('#UploadDialog')
            }});
            {Config.JQueryAlias}(document).bind('dragover', function (e) {{
                var dropZone = {Config.JQueryAlias}('#dropzone'),
                    timeout = window.dropZoneTimeout;
                if (!timeout) {{
                    dropZone.addClass('ui-state-highlight');
                }} else {{
                    clearTimeout(timeout);
                }}
                var found = false,
                    node = e.target;
                do {{
                    if (node === dropZone[0]) {{
                        found = true;
                        break;
                    }}
                    node = node.parentNode;
                }} while (node != null);
                if (found) {{
                    dropZone.addClass('ui-widget-content');
                }} else {{
                    dropZone.removeClass('ui-widget-content');
                }}
                window.dropZoneTimeout = setTimeout(function () {{
                    window.dropZoneTimeout = null;
                    dropZone.removeClass('ui-state-highlight ui-widget-content');
                }}, 100);
            }});
        }});";
    }

    /// <summary>
    /// select2 topics load JS.
    /// </summary>
    /// <param name="topicsId">
    /// The topics Id.
    /// </param>
    /// <param name="forumDropDownId">
    /// The forum drop down identifier.
    /// </param>
    /// <param name="selectedHiddenId">
    /// The topic selected Hidden Id.
    /// </param>
    /// <returns>
    /// Returns the select2 topics load JS.
    /// </returns>
    [NotNull]
    public static string SelectTopicsLoadJs([NotNull] string topicsId, [NotNull] string forumDropDownId, [NotNull] string selectedHiddenId)
    {
        return $$"""
                 {{Config.JQueryAlias}}('#{{topicsId}}').select2({
                             ajax: {
                                 url: '{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Topic/GetTopics',
                                 type: 'POST',
                                 dataType: 'json',
                                 minimumInputLength: 1,
                                 data: function(params) {
                                       var query = {
                                           ForumId : {{Config.JQueryAlias}}('#{{forumDropDownId}}').val(),
                                           TopicId: {{BoardContext.Current.PageTopicID}},
                                           PageSize: 0,
                                           Page : params.page || 0,
                                           SearchTerm : params.term || ''
                                       }
                                       return query;
                                 },
                                 error: errorLog,
                                 processResults: function(data, params) {
                                     params.page = params.page || 0;
                 
                                     var resultsPerPage = 15 * 2;
                 
                                     var total = params.page == 0 ? data.Results.length : resultsPerPage;
                 
                                     return {
                                         results: data.Results,
                                         pagination: {
                                             more: total < data.Total
                                         }
                                     }
                                 }
                             },
                             allowClearing: false,
                             width: '100%',
                             theme: 'bootstrap-5',
                             cache: true,
                             {{BoardContext.Current.Get<ILocalization>().GetText("SELECT_LOCALE_JS")}}
                         }).on('select2:select', function(e){
                                    if (e.params.data.Total) {
                                                                  {{Config.JQueryAlias}}('#{{selectedHiddenId}}').val(e.params.data.Results[0].children[0].id);
                                                              } else {
                                                                  {{Config.JQueryAlias}}('#{{selectedHiddenId}}').val(e.params.data.id);
                                                              }
                             });
                 """;
    }

    /// <summary>
    /// select2 forum load JS.
    /// </summary>
    /// <param name="forumDropDownId">
    /// The forum drop down identifier.
    /// </param>
    /// <param name="placeHolder">
    /// The place Holder.
    /// </param>
    /// <param name="forumLink">
    /// Go to Forum on select
    /// </param>
    /// <param name="allForumsOption">
    /// Add All Forums option
    /// </param>
    /// <param name="selectedHiddenId">
    /// The selected Hidden Id.
    /// </param>
    /// <returns>
    /// Returns the select2 topics load JS.
    /// </returns>
    [NotNull]
    public static string SelectForumsLoadJs([NotNull] string forumDropDownId, [NotNull] string placeHolder, bool forumLink, bool allForumsOption, [CanBeNull] string selectedHiddenId = null)
    {
        var selectHiddenValue = selectedHiddenId.IsSet()
                                    ? $$"""
                                        if (e.params.data.Total) {
                                                                                         {{Config.JQueryAlias}}('#{{selectedHiddenId}}').val(e.params.data.Results[0].children[0].id);
                                                                                     } else {
                                                                                         {{Config.JQueryAlias}}('#{{selectedHiddenId}}').val(e.params.data.id);
                                                                                     }
                                        """
                                    : string.Empty;

        var forumSelect = selectedHiddenId.IsSet() ? $@"var forumsListSelect = {Config.JQueryAlias}('#{forumDropDownId}');
            var forumId = {Config.JQueryAlias}('#{selectedHiddenId}').val();

            {Config.JQueryAlias}.ajax({{
                url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/Forum/GetForum/' + forumId,
                type: 'POST',
                dataType: 'json'
            }}).then(function (data) {{
                                if (data.Total > 0) {{
                                var result = data.Results[0].children[0];
                                       
                                var option = new Option(result.text, result.id, true, true);
                                forumsListSelect.append(option).trigger('change');

                                forumsListSelect.trigger({{
                                    type: 'select2:select',
                                    params: {{
                                        data: data
                                    }}
                                }});}}
            }});" : string.Empty;

        var allForumsOptionJs = allForumsOption ? "AllForumsOption: true," : string.Empty;

        return $$"""
                 {{Config.JQueryAlias}}('#{{forumDropDownId}}').select2({
                             ajax: {
                                 url: '{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Forum/GetForums',
                                 type: 'POST',
                                 dataType: 'json',
                                 data: function(params) {
                                       var query = {
                                           PageSize: 0,
                                           {{allForumsOptionJs}}
                                           Page : params.page || 0,
                                           SearchTerm : params.term || ''
                                       }
                                       return query;
                                 },
                                 error: errorLog,
                                 processResults: function(data, params) {
                                     params.page = params.page || 0;
                 
                                     var resultsPerPage = 15 * 2;
                 
                                     var total = params.page == 0 ? data.Results.length : resultsPerPage;
                 
                                         return {
                                         results: data.Results,
                                         pagination: {
                                             more: total < data.Total
                                         }
                                     }
                                 }
                             },
                             placeholder: '{{placeHolder}}',
                             minimumInputLength: 0,
                             allowClearing: false,
                             dropdownAutoWidth: true,
                             templateResult: function (option) {
                                                 if (option.loading) {
                                                     return option.text;
                                                 }
                 	                            if (option.id) {
                 	                                var $container = {{Config.JQueryAlias}}("<span class='select2-image-select-icon'><i class='fas fa-comments fa-fw text-secondary me-1'></i>" + option.text + "</span>");
                                                     return $container;
                 	                            } else {
                                                     var $container = {{Config.JQueryAlias}}("<span class='select2-image-select-icon'><i class='fas fa-folder fa-fw text-warning me-1'></i>" + option.text + "</span>");
                                                     return $container;
                 	                            }
                 	        },
                             templateSelection: function (option) {
                 	                               if (option.id) {
                 	                               var $container = {{Config.JQueryAlias}}("<span class='select2-image-select-icon'><i class='fas fa-comments fa-fw text-secondary me-1'></i>" + option.text + "</span>");
                                                        return $container;
                 	                               } else {
                 	                                   return option.text;
                 	                               }
                 	        },
                             width: '100%',
                             theme: 'bootstrap-5',
                             cache: true,
                             {{BoardContext.Current.Get<ILocalization>().GetText("SELECT_LOCALE_JS")}}
                             }).on('select2:select', function(e){
                                    {{(forumLink ? "window.location = e.params.data.url;" : "")}}
                                    {{selectHiddenValue}}
                             });
                 
                             {{forumSelect}}
                             
                 """;
    }

    /// <summary>
    /// Gets the Passwords strength checker Java Script.
    /// </summary>
    /// <param name="passwordClientId">The password client identifier.</param>
    /// <param name="confirmPasswordClientId">The confirm password client identifier.</param>
    /// <param name="minimumChars">The minimum chars.</param>
    /// <param name="notMatchText">The not match text.</param>
    /// <param name="passwordMinText">The password minimum text.</param>
    /// <param name="passwordGoodText">The password good text.</param>
    /// <param name="passwordStrongerText">The password stronger text.</param>
    /// <param name="passwordWeakText">The password weak text.</param>
    /// <returns>Returns the Passwords strength checker Java Script</returns>
    [NotNull]
    public static string PasswordStrengthCheckerJs(
        [NotNull] string passwordClientId,
        [NotNull] string confirmPasswordClientId,
        [NotNull] int minimumChars,
        [NotNull] string notMatchText,
        [NotNull] string passwordMinText,
        [NotNull] string passwordGoodText,
        [NotNull] string passwordStrongerText,
        [NotNull] string passwordWeakText)
    {
        return $$"""
                   document.addEventListener("DOMContentLoaded",
                   function() {
                       var password = document.getElementById("{{passwordClientId}}"),
                           passwordConfirm = document.getElementById("{{confirmPasswordClientId}}"),
                           progressBar = document.getElementById("progress-password");
                   
                       password.addEventListener("keyup",
                           () => {
                               checkPassword();
                           });
                       passwordConfirm.addEventListener("keyup",
                           () => {
                               checkPassword();
                           });
                   
                       function checkPassword() {
                           const invalid = document.getElementById("PasswordInvalid");
                   
                           if (password.value !== "" && passwordConfirm.value !== "" && password.value === passwordConfirm.value) {
                               invalid.style.display = "none";
                   
                               password.classList.remove("is-invalid");
                               passwordConfirm.classList.remove("is-invalid");
                           } else {
                               invalid.style.display = "block";
                               invalid.innerText = "{{notMatchText}}";
                   
                               password.classList.add("is-invalid");
                               passwordConfirm.classList.add("is-invalid");
                           }
                   
                           const strongRegex = new RegExp("^(?=.{8,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g"),
                               mediumRegex =
                                   new RegExp(
                                       "^(?=.{7,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$",
                                       "g"),
                               okRegex = new RegExp("(?=.{{{minimumChars}},}).*", "g");
                   
                           document.getElementById("passwordStrength").classList.remove("d-none");
                   
                           const passwordHelp = document.getElementById("passwordHelp");
                   
                           if (okRegex.test(password.value) === false) {
                               passwordHelp.innerText = "{{passwordMinText}}";
                               progressBar.className = "progress-bar bg-danger w-25";
                           } else if (strongRegex.test(password.value)) {
                               passwordHelp.innerText = "{{passwordGoodText}}";
                   
                               progressBar.className = "progress-bar bg-success w-100";
                           } else if (mediumRegex.test(password.value)) {
                               passwordHelp.innerText = "{{passwordStrongerText}}";
                               progressBar.className = "progress-bar bg-warning w-75";
                           } else {
                               passwordHelp.innerText = "{{passwordWeakText}}";
                               progressBar.classList.add("progress-bar bg-warning w-50");
                           }
                       }
                   
                       const form1 = document.querySelector("form");
                   
                       // Validate on submit
                       form1.addEventListener("submit",
                           function(event) {
                               if (form1.checkValidity() === false) {
                                   event.preventDefault();
                                   event.stopPropagation();
                               }
                               form1.classList.add("was-validated");
                           },
                           false);
                   
                       // Validate on input:
                       form1.querySelectorAll(".form-control").forEach(input => {
                           input.addEventListener(("input"),
                               () => {
                                   if (input.checkValidity()) {
                                       input.classList.remove("is-invalid");
                                       input.classList.add("is-valid");
                                   } else {
                                       input.classList.remove("is-valid");
                                       input.classList.add("is-invalid");
                                   }
                               });
                       });
                   });
                   """;
    }

    /// <summary>
    /// Renders Modal open JS.
    /// </summary>
    /// <param name="clientId">The client id.</param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string OpenModalJs([NotNull] string clientId)
    {
        return $"var myModal = new bootstrap.Modal(document.getElementById(\"{clientId}\"), null);myModal.show();";
    }

    /// <summary>
    /// Gets the Do Search java script.
    /// </summary>
    /// <returns>
    /// Returns the do Search Java script String
    /// </returns>
    [NotNull]
    public static string DoSearchJs()
    {
        return "document.addEventListener(\"DOMContentLoaded\", function() { getSearchResultsData(0);});";
    }

    /// <summary>
    /// Renders the Forum Icon Legend Popover JS.
    /// </summary>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="cssClass">
    /// The CSS Class.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string ForumIconLegendPopoverJs([NotNull] string content, [NotNull] string cssClass)
    {
        return $$"""
                 Sys.Application.add_load(function(){var popoverTriggerIconList = [].slice.call(document.querySelectorAll(".{{cssClass}}"));
                                       var popoverIconList = popoverTriggerIconList.map(function(popoverTriggerEl) {
                                            return new bootstrap.Popover(popoverTriggerEl,{
                                            html: true,
                                            content: "{{content}}",
                                            trigger: "focus"
                                            });
                                     });});
                 """;
    }

    /// <summary>
    /// Renders the Topic Link Popover JS.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="cssClass">
    /// The CSS Class.
    /// </param>
    /// <param name="trigger">
    /// The trigger.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string TopicLinkPopoverJs([NotNull] string title, [NotNull] string cssClass, [NotNull] string trigger)
    {
        return $$"""
                 Sys.Application.add_load(function(){
                                       var popoverTriggerModsList = [].slice.call(document.querySelectorAll('{{cssClass}}'));
                                       var popoverModsList = popoverTriggerModsList.map(function(popoverTriggerEl) {
                                            return new bootstrap.Popover(popoverTriggerEl,{
                                            title: "{{title}}",
                                            html: true,
                                            trigger: "{{trigger}}",
                                            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>'
                                            });
                                 });
                                 });
                 """;
    }

    /// <summary>
    /// The forum mods popover JS.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string ForumModsPopoverJs([NotNull] string title)
    {
        return $$"""
                 Sys.Application.add_load(function(){
                                       var popoverTriggerModsList = [].slice.call(document.querySelectorAll('.forum-mods-popover'));
                                       var popoverModsList = popoverTriggerModsList.map(function(popoverTriggerEl) {
                                            return new bootstrap.Popover(popoverTriggerEl,{
                                            title: '{{title}}',
                                            html: true,
                                            trigger: 'focus',
                                            template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><h3 class="popover-header"></h3><div class="popover-body popover-body-scrollable"></div></div>'
                                            });
                                 });
                 
                        });
                 """;
    }

    /// <summary>
    /// The Hover Card Load JS.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    [NotNull]
    public static string HoverCardJs()
    {
        return $$"""
                 if (typeof(jQuery.fn.hovercard) != 'undefined'){
                                       {{Config.JQueryAlias}}('.hc-user').hovercard({
                                                       delay: {{BoardContext.Current.BoardSettings.HoverCardOpenDelay}},
                                                       width: 350,
                                                       loadingHTML: '{{BoardContext.Current.Get<ILocalization>().GetText("DEFAULT", "LOADING_HOVERCARD").ToJsString()}}',
                                                       errorHTML: '{{BoardContext.Current.Get<ILocalization>().GetText("DEFAULT", "ERROR_HOVERCARD").ToJsString()}}',
                                                       pointsText: '{{BoardContext.Current.Get<ILocalization>().GetText("REPUTATION").ToJsString()}}',
                                                       postsText: '{{BoardContext.Current.Get<ILocalization>().GetText("POSTS").ToJsString()}}'
                                       });
                                  }
                 """;
    }

    /// <summary>
    /// Form Validator JS.
    /// </summary>
    /// <param name="buttonClientId">
    /// The button Client Id.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string FormValidatorJs([NotNull] string buttonClientId)
    {
        return $$"""
                  (function () {
                      "use strict";
                      window.addEventListener("load", function () {
                          var form = document.forms[0];
                  
                          const test = document.getElementById("{{buttonClientId}}");
                          test.addEventListener("click", function (event) {
                              if (form.checkValidity() === false) {
                                  event.preventDefault();
                                  event.stopPropagation();
                              }
                              form.classList.add("was-validated");
                          }, false);
                  
                      }, false);
                  })();
                  """;
    }

    /// <summary>
    /// Click Button on Enter Key JS.
    /// </summary>
    /// <param name="buttonClientId">
    /// The button Client Id.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string ClickOnEnterJs([NotNull] string buttonClientId)
    {
        return $$"""
                   if (event.which || event.keyCode) {
                       if ((event.which == 13) || (event.keyCode == 13)) {
                           document.getElementById("{{buttonClientId}}").click();
                           return false;
                       }
                   } else {
                       return true;
                   };
                   """;
    }

    /// <summary>
    /// Opens the BootBox Confirm Dialog JS.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="yes">
    /// The yes.
    /// </param>
    /// <param name="no">
    /// The no.
    /// </param>
    /// <param name="link">
    /// The link.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string BootBoxConfirmJs(
        [NotNull] string title,
        [NotNull] string text,
        [NotNull] string yes,
        [NotNull] string no,
        [NotNull] string link)
    {
        return $$"""
                  document.addEventListener("DOMContentLoaded", function () {
                      bootbox.confirm({
                              centerVertical: true,
                              title: "{{title}}",
                              message: "{{text}}",
                              buttons: {
                                  confirm: {
                                      label: '<i class="fa fa-check"></i> ' + "{{yes}}",
                                      className: "btn-success"
                                  },
                                  cancel: {
                                      label: '<i class="fa fa-times"></i> ' + "{{no}}",
                                      className: "btn-danger"
                                  }
                              },
                              callback: function (confirmed) {
                                  if (confirmed) {
                                      document.location.href = "{{link}}";
                                  }
                              }
                          }
                      );
                  });
                  """;
    }

    /// <summary>
    /// Opens the BootBox Prompt Dialog JS.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="cancel">
    /// The cancel.
    /// </param>
    /// <param name="ok">
    /// The ok.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string BootBoxPromptJs(
        [NotNull] string title,
        [NotNull] string message,
        [NotNull] string cancel,
        [NotNull] string ok,
        [NotNull] string value)
    {
        return $$"""
                   bootbox.prompt({
                       title: "{{title}}",
                       message: "{{message}}",
                       value: "{{value}}",
                       buttons: { cancel: { label: "{{cancel}}" }, confirm: { label: "{{ok}}" } },
                       callback: function() {}
                   });
                   """;
    }

    /// <summary>
    /// select2 user load JS.
    /// </summary>
    /// <param name="parentId">
    /// The id of the modal
    /// </param>
    /// <param name="selectClientId">
    /// The id of the select
    /// </param>
    /// <param name="hiddenUserId">
    /// The hidden id to store the selected user id value
    /// </param>
    /// <returns>
    /// Returns the select2 user load JS.
    /// </returns>
    [NotNull]
    public static string SelectUsersLoadJs(
        [NotNull] string parentId,
        [NotNull] string selectClientId,
        [NotNull] string hiddenUserId)
    {
        return $$"""
                 {{Config.JQueryAlias}}('#{{selectClientId}}').select2({
                             ajax: {
                                 url: '{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/User/GetUsers',
                                 type: 'POST',
                                 dataType: 'json',
                                 contentType: 'application/json',
                                 minimumInputLength: 0,
                                 data: function(params) {
                                       var query = {
                                           ForumId : 0,
                                           UserId: 0,
                                           PageSize: 0,
                                           Page : params.page || 0,
                                           SearchTerm : params.term || ''
                                       }
                                       return JSON.stringify(query);
                                 },
                                 error: errorLog,
                                 processResults: function(data, params) {
                                     params.page = params.page || 0;
                 
                                     var resultsPerPage = 15 * 2;
                 
                                     var total = params.page == 0 ? data.Results.length : resultsPerPage;
                 
                                     return {
                                         results: data.Results,
                                         pagination: {
                                             more: total < data.Total
                                         }
                                     }
                                 }
                             },
                             dropdownParent: {{Config.JQueryAlias}}("#{{parentId}}"),
                             theme: 'bootstrap-5',
                             allowClearing: false,
                             placeholder: '{{BoardContext.Current.Get<ILocalization>().GetText("ADD_USER")}}',
                             cache: true,
                             width: '100%',
                             {{BoardContext.Current.Get<ILocalization>().GetText("SELECT_LOCALE_JS")}}
                         });
                               
                              {{Config.JQueryAlias}}('#{{selectClientId}}').on('select2:select', function (e) {
                                 if (e.params.data.Total) {
                                                                  {{Config.JQueryAlias}}('#{{hiddenUserId}}').val(e.params.data.Results[0].children[0].id);
                                                              } else {
                                                                  {{Config.JQueryAlias}}('#{{hiddenUserId}}').val(e.params.data.id);
                                                              }
                             });
                 """;
    }

    /// <summary>
    /// The Logout Dialog Load JS.
    /// </summary>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="yes">
    /// The yes.
    /// </param>
    /// <param name="no">
    /// The no.
    /// </param>
    /// <param name="link">
    /// The link.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string LogOutJs(
        [NotNull] string title,
        [NotNull] string text,
        [NotNull] string yes,
        [NotNull] string no,
        [NotNull] string link)
    {
        return $$"""
                  function LogOutClick() {
                      bootbox.confirm({
                              centerVertical: true,
                              title: "{{title}}",
                              message: "{{text}}",
                              buttons: {
                                  confirm: {
                                      label: '<i class="fa fa-check"></i> ' + "{{yes}}",
                                      className: "btn-success"
                                  },
                                  cancel: {
                                      label: '<i class="fa fa-times"></i> ' + "{{no}}",
                                      className: "btn-danger"
                                  }
                              },
                              callback: function (confirmed) {
                                  if (confirmed) {
                                      document.location.href = "{{link}}";
                                  }
                              }
                          }
                      );
                  }
                  """;
    }

    /// <summary>
    /// Renders the Load More on Scrolling JS.
    /// </summary>
    /// <param name="buttonClientId">
    /// The button Client Id.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string LoadMoreOnScrolling([NotNull] string buttonClientId)
    {
        return $$"""
                  window.addEventListener("scroll", () => {
                      const { scrollTop, clientHeight, scrollHeight } = document.documentElement;
                      if ((scrollTop + clientHeight) >= scrollHeight) {
                          const btn = document.getElementById("{{buttonClientId}}");
                          if (btn != null) {
                              btn.click();
                          }
                      }
                  });
                  """;
    }

    /// <summary>
    /// Renders toggleSelection Function JS.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// Returns the JS String
    /// </returns>
    [NotNull]
    public static string ToggleDiffSelectionJs([NotNull] string message)
    {
        return $$"""
                 function toggleSelection(source) {
                     if (document.querySelector("input[id*='Compare']:checked").length > 2) {
                     source.checked = false;
                     bootbox.alert("{{message}}");
                     }
                 }
                 """;
    }
}