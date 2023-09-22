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
    /// <param name="inputId">The input Id.</param>
    /// <param name="hiddenId">the hidden id</param>
    /// <returns>The <see cref="string" />.</returns>
    [NotNull]
    public static string GetBoardTagsJs(string inputId, string hiddenId)
    {
        return $$"""
                  var tagsSelect = new Choices('#{{inputId}}', {
                          allowHTML: false,
                          addChoices: true,
                          shouldSort: false,
                          removeItemButton: true,
                          placeholder: false,
                          classNames: { containerOuter: "choices w-100" },
                          resetScrollPosition: false,
                          callbackOnCreateTemplates: createTagsSelectTemplates
                        });
                        
                        var query = {
                      TopicId: {{BoardContext.Current.PageTopicID}},
                      PageSize: 0,
                      Page: 0,
                      SearchTerm: ""
                  };
                  
                  tagsSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Tags/GetBoardTags") });
                  
                  const hiddenField = document.getElementById("{{hiddenId}}");
                  
                  if (hiddenField.value.length > 0) {
                      tagsSelect.setValue(hiddenField.value.split(','));
                  }

                  ["addItem","removeItem"].forEach(function (e) {
                      tagsSelect.passedElement.element.addEventListener(e, function (event) {
                          hiddenField.value = tagsSelect.getValue().map(x => x.value).join();
                      });
                  });
                 

                  tagsSelect.passedElement.element.addEventListener("search", function (event) {
                  
                      if (event.detail.value > 2) {
                          var query = {
                              ForumId: 0,
                              TopicId: 0,
                              PageSize: 15,
                              Page: 0,
                              SearchTerm: event.detail.value
                          };
                          tagsSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Tags/GetBoardTags") }, "value", "label", true);
                      }
                  });

                  tagsSelect.passedElement.element.addEventListener("showDropdown", function () {
                      var listBox = tagsSelect.choiceList.element;
                      listBox.addEventListener("scroll", function () {
                  
                          const scrollableHeight = listBox.scrollHeight - listBox.clientHeight
                  
                          if (listBox.scrollTop >= scrollableHeight) {
                              const resultsPerPage = 15 * 2,
                                  choices = tagsSelect._currentState.choices,
                  
                                  lastItem = choices[choices.length - 1],
                  
                                  currentPage = lastItem.customProperties.page,
                  
                                  total = lastItem.customProperties.page == 0
                                      ? tagsSelect._currentState.choices.length
                                      : resultsPerPage;
                  
                  
                              if (total < lastItem.customProperties.total) {
                                  var query = {
                                      ForumId: 0,
                                      TopicId: 0,
                                      PageSize: 15,
                                      Page: currentPage + 1,
                                      SearchTerm: ""
                                  };
                  
                                  tagsSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Tags/GetBoardTags") }, "value", "label", false);
                              }
                          }
                      });
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
        return $$$"""
                  var {{{editorId}}}=new yafEditor("{{{editorId}}}", "{{{urlTitle}}}", "{{{urlDescription}}}", "{{{urlImageTitle}}}", "{{{urlImageDescription}}}", "{{{description}}}");
                                    function setStyle(style,option) {
                                             {{{editorId}}}.FormatText(style,option);
                                    }
                                    function insertAttachment(id,url) {
                                             {{{editorId}}}.FormatText("attach", id);
                                    }
                                    
                  mentions({id: '{{{editorId}}}',
                           lookup: 'user',
                           url:'{{{BoardInfo.ForumClientFileRoot}}}resource.ashx?users={q}',
                           onclick: function (data) {{{{editorId}}}.FormatText("userlink", data.name);}});
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
    public static string CodeMirrorSqlLoadJs([NotNull] string editorId, [NotNull] string mime)
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
    /// Gets the Editor File Auto Upload Java Script.
    /// </summary>
    /// <param name="fileUploaderUrl">
    /// The file uploader URL.
    /// </param>
    /// <returns>
    /// Returns the FileUpload Java Script.
    /// </returns>
    [NotNull]
    public static string FileAutoUploadLoadJs([NotNull] string fileUploaderUrl)
    {
        return $$"""
                 (function () {
                     "use strict";
                     const eventHandlers = zone => {
                         ["dragenter", "dragover", "dragleave", "drop"].forEach(event => {
                             zone.addEventListener(event, (e) => {
                                 e.preventDefault();
                                 e.stopPropagation();
                             }, false);
                             document.body.addEventListener(event, (e) => {
                                 e.preventDefault();
                                 e.stopPropagation();
                             }, false);
                         });
                 
                         zone.addEventListener("drop", (e) => {
                             filesUpload(e.dataTransfer.files);
                         }, false);
                     }
                 
                     document.addEventListener("DOMContentLoaded",
                         function() {
                             const dropZone = document.querySelector(".BBCodeEditor");
                             eventHandlers(dropZone);
                         });
                 
                     const filesUpload = files => {
                 
                         if (!files) return;
                 
                         const url = "{{fileUploaderUrl}}";
                 
                         const formData = new FormData();
                 
                         for (let x = 0; x < files.length; x++) {
                             formData.append("'file" + x + "'", files[x]);
                         }
                 
                         fetch(url, {
                             method: "POST",
                             body: formData,
                             mode: "cors"
                         })
                             .then(response => response.json())
                             .then(data => {
                                 if (data.length) {
                                     if (data[0].error) {
                                         const _ = new Notify({
                                                 title: "{{BoardContext.Current.BoardSettings.Name}}",
                                                 message: data[0].error,
                                                 icon: "fa fa-exclamation-triangle"
                                             },
                                             {
                                                 type: "danger",
                                                 element: "body",
                                                 position: null,
                                                 placement: { from: "top", align: "center" },
                                                 delay: {{BoardContext.Current.BoardSettings.MessageNotifcationDuration}} * 1000
                                             });
                 
                                     } else {
                                         insertAttachment(data[0].fileID, data[0].fileID);
                                     }
                                 } else {
                                     console.error("error");
                                 }
                             })
                             .catch(error => {
                                 console.error("error: ", error);
                             });
                     }
                 })();
                 """;
    }

    /// <summary>
    /// Gets the FileUpload Java Script.
    /// </summary>
    /// <param name="fileUploaderUrl">
    /// The file uploader URL.
    /// </param>
    /// <returns>
    /// Returns the FileUpload Java Script.
    /// </returns>
    [NotNull]
    public static string FileUploadLoadJs([NotNull] string fileUploaderUrl)
    {
        return $$"""
                 document.addEventListener("DOMContentLoaded", function() {
                     const fileUploader = new FileUploader({
                         dropZone: "drop-area",
                         url: "{{fileUploaderUrl}}",
                         fileInput: ".fileinput-button",
                         errorTitle: "{{BoardContext.Current.BoardSettings.Name}}",
                         errorDelay: {{BoardContext.Current.BoardSettings.MessageNotifcationDuration}}
                     });
                 });
                 """;
    }

    /// <summary>
    /// select topics load JS.
    /// </summary>
    /// <param name="topicDropDownId">
    /// The topic drop down Id.
    /// </param>
    /// <param name="topicHiddenId">
    /// the selected topic Id hidden field
    /// </param>
    /// <param name="forumDropDownId">
    /// The forum drop down identifier.
    /// </param>
    /// <param name="placeHolder">
    /// The select place holder.
    /// </param>
    /// <returns>
    /// Returns the select topics load JS.
    /// </returns>
    [NotNull]
    public static string SelectTopicsLoadJs(
        [NotNull] string topicDropDownId,
        [NotNull] string topicHiddenId,
        [NotNull] string forumDropDownId,
        [NotNull] string placeHolder)
    {
        return $$"""
                 var topicsSelect = new Choices("#{{topicDropDownId}}", {
                     allowHTML: false,
                     shouldSort: false,
                     classNames: { containerOuter: "choices w-100" },
                     placeholderValue: "{{placeHolder}}",
                     resetScrollPosition: false
                 });

                 var query = {
                     ForumId: document.getElementById('{{forumDropDownId}}').value,
                     TopicId: {{BoardContext.Current.PageTopicID}},
                     PageSize: 0,
                     Page: 0,
                     SearchTerm: ""
                 };
                 topicsSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Topic/GetTopics") });

                 topicsSelect.passedElement.element.addEventListener("choice", function (event) {
                     document.getElementById("{{topicHiddenId}}").value = event.detail.choice.value;
                 });

                 topicsSelect.passedElement.element.addEventListener("search", function (event) {
                 
                     if (event.detail.value > 2) {
                         var query = {
                             ForumId: document.getElementById('{{forumDropDownId}}').value,
                             TopicId: {{BoardContext.Current.PageTopicID}},
                             PageSize: 15,
                             Page: 0,
                             SearchTerm: event.detail.value
                         };
                         topicsSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Topic/GetTopics") }, "value", "label", true);
                     }
                 });

                 topicsSelect.passedElement.element.addEventListener("showDropdown", function () {
                     var listBox = topicsSelect.choiceList.element;
                     listBox.addEventListener("scroll", function () {
                 
                         const scrollableHeight = listBox.scrollHeight - listBox.clientHeight
                 
                         if (listBox.scrollTop >= scrollableHeight) {
                             const resultsPerPage = 15 * 2,
                                 choices = topicsSelect._currentState.choices,
                 
                                 lastItem = choices[choices.length - 1],
                 
                                 currentPage = lastItem.customProperties.page,
                 
                                 total = lastItem.customProperties.page == 0
                                     ? topicsSelect._currentState.choices.length
                                     : resultsPerPage;
                 
                 
                             if (total < lastItem.customProperties.total) {
                                 var query = {
                                     ForumId: document.getElementById('{{forumDropDownId}}').value,
                                     TopicId: {{BoardContext.Current.PageTopicID}},
                                     PageSize: 15,
                                     Page: currentPage + 1,
                                     SearchTerm: ""
                                 };
                 
                                 topicsSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Topic/GetTopics") }, "value", "label", false);
                             }
                         }
                     });
                 });
                 """;
    }

    /// <summary>
    /// select forum load JS.
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
    /// Returns the select topics load JS.
    /// </returns>
    [NotNull]
    public static string SelectForumsLoadJs(
        [NotNull] string forumDropDownId,
        [NotNull] string placeHolder,
        bool forumLink,
        bool allForumsOption,
        [CanBeNull] string selectedHiddenId = null)
    {
        // forum link
        var forumLinkJs = forumLink
                              ? """
                                forumsSelect.passedElement.element.addEventListener("choice", function (event) {
                                    var json;
                                    
                                    console.log(event);
                                
                                    if (event.detail.choice.customProperties) {
                                        try {
                                            json = JSON.parse(event.detail.choice.customProperties);
                                        } catch (e) {
                                            json = event.detail.choice.customProperties;
                                        }
                                
                                        if (json.url !== undefined) {
                                            window.location = json.url;
                                        }
                                    }
                                });
                                """
                              : string.Empty;

        // selected forum id
        var selectHiddenValue = selectedHiddenId.IsSet() ? $"document.getElementById('{selectedHiddenId}').value" : "0";

        var selectHiddenJs = selectedHiddenId.IsSet()
                                 ? $$"""
                                     forumsSelect.passedElement.element.addEventListener("choice", function (event) {
                                         document.getElementById("{{selectedHiddenId}}").value = event.detail.choice.value;
                                     });
                                     """
                                 : string.Empty;

        // all forums option
        var allForumsOptionJs = allForumsOption ? "AllForumsOption: true," : string.Empty;

        var placeholderValue = allForumsOption ? string.Empty : $"""placeholderValue: "{placeHolder}",""";

        return $$"""
                 var forumsSelect = new Choices("#{{forumDropDownId}}", {
                     allowHTML: true,
                     shouldSort: false,
                     classNames: { containerOuter: "choices w-100 choices-forum" },
                     {{placeholderValue}}
                     resetScrollPosition: false,
                     callbackOnCreateTemplates: createForumSelectTemplates
                 });

                 {{forumLinkJs}}

                 var forumQuery = {
                     {{allForumsOptionJs}}
                     PageSize: 0,
                     Page: 0,
                     SearchTerm: ""
                 };

                 forumsSelect.setChoices(function () {
                     return loadForumChoiceOptions(forumQuery, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Forum/GetForums", {{selectHiddenValue}}) });

                 {{selectHiddenJs}}

                 forumsSelect.passedElement.element.addEventListener("search", function (event) {
                 
                     if (event.detail.value > 2) {
                         var query = {
                             {{allForumsOptionJs}}
                             PageSize: 15,
                             Page: 0,
                             SearchTerm: event.detail.value
                         };
                         forumsSelect.setChoices(function () {
                             return loadForumChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Forum/GetForums", {{selectHiddenValue}}) },
                                 "value", "label", true);
                     }
                 });

                 forumsSelect.passedElement.element.addEventListener("showDropdown", function () {
                     var listBox = forumsSelect.choiceList.element;
                     listBox.addEventListener("scroll", function () {
                 
                         const scrollableHeight = listBox.scrollHeight - listBox.clientHeight
                 
                         if (listBox.scrollTop >= scrollableHeight) {
                             const resultsPerPage = 15 * 2,
                                 choices = forumsSelect._currentState.choices,
                 
                                 lastItem = choices[choices.length - 1],
                 
                                 currentPage = lastItem.customProperties.page,
                 
                                 total = lastItem.customProperties.page == 0
                                     ? forumsSelect._currentState.choices.length
                                     : resultsPerPage;
                 
                             if (total < lastItem.customProperties.total) {
                                 var query = {
                                     {{allForumsOptionJs}}
                                     PageSize: 15,
                                     Page: currentPage + 1,
                                     SearchTerm: ""
                                 };
                 
                                 forumsSelect.setChoices(function () {
                                     return loadForumChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/Forum/GetForums", {{selectHiddenValue}}) },
                                         "value", "label", false);
                             }
                         }
                     });
                 });
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
                 document.addEventListener('DOMContentLoaded', function() {
                     document.querySelectorAll(".hc-user").forEach(pop => {
                 	    userCardContent(pop, {{BoardContext.Current.BoardSettings.HoverCardOpenDelay}});
                 	});
                 });
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
    /// Select user load JS.
    /// </summary>
    /// <param name="selectClientId">
    /// The id of the select
    /// </param>
    /// <param name="hiddenUserId">
    /// The hidden id to store the selected user id value
    /// </param>
    /// <param name="placeHolder">
    /// The place Holder.
    /// </param>
    /// <returns>
    /// Returns the select user load JS.
    /// </returns>
    [NotNull]
    public static string SelectUsersLoadJs(
        [NotNull] string selectClientId,
        [NotNull] string hiddenUserId,
        [NotNull] string placeHolder)
    {
        return $$"""
                 if (document.getElementById("{{selectClientId}}") != null) {

                 var userSelect = new Choices("#{{selectClientId}}", {
                     allowHTML: false,
                     shouldSort: false,
                     classNames: { containerOuter: "choices w-100" },
                     placeholderValue: "{{placeHolder}}",
                     resetScrollPosition: false
                 });

                 var query = {
                     ForumId: 0,
                     TopicId: 0,
                     PageSize: 0,
                     Page: 0,
                     SearchTerm: ""
                 };
                 userSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/User/GetUsers") });

                 userSelect.passedElement.element.addEventListener("choice", function (event) {
                     document.getElementById("{{hiddenUserId}}").value = event.detail.choice.value;
                 });

                 userSelect.passedElement.element.addEventListener("search", function (event) {
                 
                     if (event.detail.value > 2) {
                         var query = {
                             ForumId: 0,
                             TopicId: 0,
                             PageSize: 15,
                             Page: 0,
                             SearchTerm: event.detail.value
                         };
                         userSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/User/GetUsers") }, "value", "label", true);
                     }
                 });

                 userSelect.passedElement.element.addEventListener("showDropdown", function () {
                     var listBox = userSelect.choiceList.element;
                     listBox.addEventListener("scroll", function () {
                 
                         const scrollableHeight = listBox.scrollHeight - listBox.clientHeight
                 
                         if (listBox.scrollTop >= scrollableHeight) {
                             const resultsPerPage = 15 * 2,
                                 choices = userSelect._currentState.choices,
                 
                                 lastItem = choices[choices.length - 1],
                 
                                 currentPage = lastItem.customProperties.page,
                 
                                 total = lastItem.customProperties.page == 0
                                     ? userSelect._currentState.choices.length
                                     : resultsPerPage;
                 
                 
                             if (total < lastItem.customProperties.total) {
                                 var query = {
                                     ForumId: 0,
                                     TopicId: 0,
                                     PageSize: 15,
                                     Page: currentPage + 1,
                                     SearchTerm: ""
                                 };
                 
                                 userSelect.setChoices(function () { return loadChoiceOptions(query, "{{BoardInfo.ForumClientFileRoot}}{{WebApiConfig.UrlPrefix}}/User/GetUsers") }, "value", "label", false);
                             }
                         }
                     });
                 });
                 }
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