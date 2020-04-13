/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Utilities
{
    #region Using

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.Context;
    using YAF.Core.Context.Start;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Contains the Java Script Blocks
    /// </summary>
    public static class JavaScriptBlocks
    {
        #region Properties

        /// <summary>
        ///   Gets the script for changing the album title.
        /// </summary>
        /// <returns>
        ///   the change album title js.
        /// </returns>
        [NotNull]
        public static string ChangeAlbumTitleJs =>
            $@"function changeAlbumTitle(albumId, txtTitleId){{
                     var newTitleTxt = {Config.JQueryAlias}('#' + txtTitleId).val();
            {Config.JQueryAlias}.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/Album/ChangeAlbumTitle',
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify({{ AlbumId: albumId, NewTitle: newTitleTxt  }}),
                    dataType: 'json',
                    success: changeTitleSuccess,
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
               }}";

        /// <summary>
        ///   Gets the script for changing the image caption.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static string ChangeImageCaptionJs =>
            $@"function changeImageCaption(imageID, txtTitleId){{
                        var newImgTitleTxt = {Config.JQueryAlias}('#' + txtTitleId).val();
              {Config.JQueryAlias}.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/Album/ChangeImageCaption',
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify({{ ImageId: imageID, NewCaption: newImgTitleTxt  }}),
                    dataType: 'json',
                    success: changeTitleSuccess,
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
               }}";

        /// <summary>
        ///   Gets the MomentJS Load JS.
        /// </summary>
        public static string MomentLoadJs =>
            $@"function loadTimeAgo() {{
            
		     moment.locale('{(BoardContext.Current.CultureUser.IsSet()
                                  ? BoardContext.Current.CultureUser.Substring(0, 2)
                                  : BoardContext.Current.Get<BoardSettings>().Culture.Substring(0, 2))}');

             {Config.JQueryAlias}('abbr.timeago').each(function() {{
                  {Config.JQueryAlias}(this).html(function(index, value) {{
                                          return moment(value).fromNow();
                  }});
                  {Config.JQueryAlias}(this).removeClass('timeago');
            }});

            Prism.highlightAll();
			      }}
                   Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadTimeAgo);";

        /// <summary>
        ///   Gets the script for album/image title/image callback.
        /// </summary>
        /// <returns>
        ///   the callback success js.
        /// </returns>
        [NotNull]
        public static string AlbumCallbackSuccessJs =>
            @"function changeTitleSuccess(res){
                  spnTitleVar = document.getElementById('spnTitle' + res.Id);
                  txtTitleVar =  document.getElementById('txtTitle' + res.Id);
                  spnTitleVar.firstChild.nodeValue = res.NewTitle;
                  txtTitleVar.disabled = false;
                  spnTitleVar.style.display = 'inline';
                  txtTitleVar.style.display='none';}";

        /// <summary>
        /// Gets the multi quote callback success JS.
        /// </summary>
        [NotNull]
        public static string MultiQuoteCallbackSuccessJs =>
            $@"function multiQuoteSuccess(res){{
                  var multiQuoteButton = {Config.JQueryAlias}('#' + res.Id).parent('span');
                  multiQuoteButton.removeClass(multiQuoteButton.attr('class')).addClass(res.NewTitle);
                  {Config.JQueryAlias}(document).scrollTop(multiQuoteButton.offset().top - 20);
                      }}";

        /// <summary>
        /// Gets the multi quote button JS.
        /// </summary>
        [NotNull]
        public static string MultiQuoteButtonJs =>
            $@"function handleMultiQuoteButton(button, msgId, tpId){{
                var multiQuoteButton = {{}};
                    multiQuoteButton.ButtonId = button.id;
                    multiQuoteButton.IsMultiQuoteButton = button.checked;
                    multiQuoteButton.MessageId = msgId;
                    multiQuoteButton.TopicId = tpId;
                    multiQuoteButton.ButtonCssClass = {Config.JQueryAlias}('#' + button.id).parent('span').attr('class');
 
                {Config.JQueryAlias}.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/MultiQuote/HandleMultiQuote',
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    data: JSON.stringify(multiQuoteButton),
                    dataType: 'json',
                    success: multiQuoteSuccess,
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
        }}";

        /// <summary>
        /// Gets Board Tags JavaScript
        /// </summary>
        /// <param name="inputId">
        /// The input Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public static string GetBoardTagsJs(string inputId) =>
            $@" $(""#{inputId}"").tagsinput({{
        typeahead: {{
            source: function () {{
                var ajaxUrl = ""{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/Tags/GetBoardTags"";
                return $.ajax({{
                    url: ajaxUrl,
                    type: 'POST',
                    dataType: 'JSON',
                   // data: 'query=' + query,
                    success: function (data) {{
                        return data;
                    }}
                }});
            }}
        }},
        freeInput: true
    }});

    $(""input"").on('itemAdded', function (event) {{
        setTimeout(function () {{
            $("">input[type=text]"", "".bootstrap-tagsinput"").val("""");
        }}, 1);
    }});";

        #endregion

        #region Public Methods

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
            return $@"function showTexBox(spnTitleId) {{
    {{
        spnTitleVar = document.getElementById('spnTitle' + spnTitleId.substring(8));
        txtTitleVar = document.getElementById('txtTitle' + spnTitleId.substring(8));
        if (spnTitleVar.firstChild != null) txtTitleVar.setAttribute('value', spnTitleVar.firstChild.nodeValue);
        if (spnTitleVar.firstChild.nodeValue == '{albumEmptyTitle}' || spnTitleVar.firstChild.nodeValue == '{imageEmptyCaption}') {{
            {{
                txtTitleVar.value = '';
                spnTitleVar.firstChild.nodeValue = '';
            }}
        }}
        txtTitleVar.style.display = 'inline';
        spnTitleVar.style.display = 'none';
        txtTitleVar.focus();
    }}
}}

function resetBox(txtTitleId, isAlbum) {{
    {{
        spnTitleVar = document.getElementById('spnTitle' + txtTitleId.substring(8));
        txtTitleVar = document.getElementById(txtTitleId);
        txtTitleVar.style.display = 'none';
        txtTitleVar.disabled = false;
        spnTitleVar.style.display = 'inline';
        if (spnTitleVar.firstChild != null) txtTitleVar.value = spnTitleVar.firstChild.nodeValue;
        if (spnTitleVar.firstChild.nodeValue == '') {{
            {{
                txtTitleVar.value = '';
                if (isAlbum) spnTitleVar.firstChild.nodeValue = '{albumEmptyTitle}';
                else spnTitleVar.firstChild.nodeValue = '{imageEmptyCaption}';
            }}
        }}
    }}
}}

function checkKey(event, handler, id, isAlbum) {{
    {{
        if ((event.keyCode == 13) || (event.which == 13)) {{
            {{
                if (event.preventDefault) event.preventDefault();
                event.cancel = true;
                event.returnValue = false;
                if (spnTitleVar.firstChild.nodeValue != txtTitleVar.value) {{
                    {{
                        handler.disabled = true;
                        if (isAlbum == true) changeAlbumTitle(id, handler.id);
                        else changeImageCaption(id, handler.id);
                    }}
                }} else resetBox(handler.id, isAlbum);
            }}
        }} else if ((event.keyCode == 27) || (event.which == 27)) resetBox(handler.id, isAlbum);
    }}
}}

function blurTextBox(txtTitleId, id, isAlbum) {{
    {{
        spnTitleVar = document.getElementById('spnTitle' + txtTitleId.substring(8));
        txtTitleVar = document.getElementById(txtTitleId);
        if (spnTitleVar.firstChild != null) {{
            {{
                if (spnTitleVar.firstChild.nodeValue != txtTitleVar.value) {{
                    {{
                        txtTitleVar.disabled = true;
                        if (isAlbum == true) changeAlbumTitle(id, txtTitleId);
                        else changeImageCaption(id, txtTitleId);
                    }}
                }} else resetBox(txtTitleId, isAlbum);
            }}
        }} else resetBox(txtTitleId, isAlbum);
    }}
}}";
        }

        /// <summary>
        /// Blocks the UI JS.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="buttonId">The button identifier.</param>
        /// <returns>
        /// The block UI execute JS.
        /// </returns>
        public static string BlockUiExecuteJs([NotNull] string messageId, [NotNull] string buttonId)
        {
            return $@"{Config.JQueryAlias}(document).ready(function() {{
                      {Config.JQueryAlias}('{buttonId}').click(function() {{ 
                                   {Config.JQueryAlias}.blockUI({{ 
                                                 message: {Config.JQueryAlias}('#{messageId}') }});
                       }});
                      }});";
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
            return $@"function blockUIMessage() {{ 
                                   {Config.JQueryAlias}.blockUI({{ 
                                                 message: {Config.JQueryAlias}('#{messageId}') }});
                       }};";
        }

        /// <summary>
        /// Generates a BootStrap DateTimePicker Script
        /// </summary>
        /// <param name="dateFormat">Localized Date Format</param>
        /// <param name="culture">Current Culture</param>
        /// <returns>
        /// The Load JS.
        /// </returns>
        public static string DatePickerLoadJs([NotNull] string dateFormat, [NotNull] string culture)
        {
            var cultureJs = string.Empty;

            dateFormat = dateFormat.ToUpper();

            if (culture.IsSet())
            {
                cultureJs = $", locale: '{culture}'";
            }

            return $@"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadDatePicker);
                  function loadDatePicker() {{	
                                 {Config.JQueryAlias}(document).ready(function() {{ 
                                {Config.JQueryAlias}('.datepickerinput').datetimepicker({{
                                                        format: '{dateFormat}'{cultureJs},icons:{{
                                                        time: 'fa fa-clock fa-fw',
                                                        date: 'fa fa-calendar fa-fw',
                                                        up: 'fa fa-chevron-up fa-fw',
                                                        down: 'fa fa-chevron-down fa-fw',
                                                        previous: 'fa fa-chevron-left fa-fw',
                                                        next: 'fa fa-chevron-right fa-fw',
                                                        today: 'fa fa-sun fa-fw',
                                                        clear: 'fa fa-trash fa-fw',
                                                        close: 'fa fa-times fa-fw'
        }}}}); }});}} ";
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
            return BootstrapTabsLoadJs(tabId, hiddenId, string.Empty);
        }

        /// <summary>
        /// Gets the Bootstrap Tab Load JS.
        /// </summary>
        /// <param name="tabId">The tab Id.</param>
        /// <param name="hiddenId">The hidden field id.</param>
        /// <param name="onClickEvent">The on click event.</param>
        /// <returns>
        /// Returns the the Bootstrap Tab Load JS string
        /// </returns>
        public static string BootstrapTabsLoadJs([NotNull] string tabId, string hiddenId, string onClickEvent)
        {
            return $@"{Config.JQueryAlias}(document).ready(function() {{
            var selectedTab = {Config.JQueryAlias}(""#{hiddenId}"");
            var tabId = selectedTab.val() != """" ? selectedTab.val() : ""View1"";
            {Config.JQueryAlias}('#{tabId} a[href=""#' + tabId + '""]').tab('show');
            {Config.JQueryAlias}(""#{tabId} a"").click(function() {{
                var tab = {Config.JQueryAlias}(this).attr(""href"").substring(1);
                if (!tab.startsWith(""avascript""))
{{
                selectedTab.val({Config.JQueryAlias}(this).attr(""href"").substring(1));
}}
                {onClickEvent}
            }});
                           }});";
        }

        /// <summary>
        /// Gets the Bootstrap Tab Load JS.
        /// </summary>
        /// <param name="tabId">
        /// The tab Id.
        /// </param>
        /// <param name="hiddenId">
        /// The hidden field id.
        /// </param>
        /// <returns>
        /// Returns the the Bootstrap Tab Load JS string
        /// </returns>
        public static string BootstrapTabLoadJs([NotNull] string tabId, string hiddenId)
        {
            return $@"{Config.JQueryAlias}(document).ready(function() {{
            var selectedTab = {Config.JQueryAlias}(""#{hiddenId}"");
            var tabId = selectedTab.val() != """" ? selectedTab.val() : ""View1"";
            {Config.JQueryAlias}('#{tabId} a[href=""#' + tabId + '""]').tab('show');
            {Config.JQueryAlias}(""#{tabId} a"").click(function() {{
                var tab = {Config.JQueryAlias}(this).attr(""href"").substring(1);
                if (!tab.startsWith(""avascript""))
{{
                selectedTab.val({Config.JQueryAlias}(this).attr(""href"").substring(1));
}}
            }});
                           }});";
        }

        /// <summary>
        /// The drop down toggle JS.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DropDownToggleJs()
        {
            return $@"document.addEventListener('DOMContentLoaded', (event) => {{
                {Config.JQueryAlias}(function() {{
                {Config.JQueryAlias}('.dropdown-menu').on('click', function(e) {{
                    if (e.target.type == 'button')
                        {Config.JQueryAlias}().dropdown('toggle')
                    else
                        e.stopPropagation();
                }});
                {Config.JQueryAlias}(window).on('click', function() {{
                    if (!{Config.JQueryAlias}('.dropdown-menu').is (':hidden')) {{
                        {Config.JQueryAlias}().dropdown('toggle')
                     }}
                 }});
                 }});
                }});";
        }

        /// <summary>
        /// Generated the load Script for the Table Sorter Plugin
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// Returns the Java Script that loads table Sorter
        /// </returns>
        public static string LoadTableSorter([NotNull] string selector, [CanBeNull] string options)
        {
            var widgets =
                ", widgets: ['zebra', 'reflow'], widgetOptions: { reflow_className: 'ui-table-reflow',reflow_headerAttrib: 'data-name',reflow_dataAttrib: 'data-title'}";

            return $@"{Config.JQueryAlias}(document).ready(function() {{
                        {Config.JQueryAlias}('{selector}').tablesorter( 
                                          {(options.IsSet() ? $"{{ theme: 'bootstrap', {options}{widgets} }}" : "{{ theme: 'bootstrap'{widgets} }}")} );
                    }});";
        }

        /// <summary>
        /// Generated the load Script for the Table Sorter Plugin (with Pager)
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="options">The options.</param>
        /// <param name="pagerSelector">The pager selector.</param>
        /// <returns>
        /// Returns the Java Script that loads table Sorter
        /// </returns>
        public static string LoadTableSorter(
            [NotNull] string selector,
            [CanBeNull] string options,
            [NotNull] string pagerSelector)
        {
            var widgets =
                ", widgets: ['zebra', 'reflow'], widgetOptions: { reflow_className: 'ui-table-reflow',reflow_headerAttrib: 'data-name',reflow_dataAttrib: 'data-title'}";

            return $@"{Config.JQueryAlias}(document).ready(function() {{
                        {Config.JQueryAlias}('{selector}').tablesorter( {(options.IsSet() ? $"{{ {options},theme : 'bootstrap'{widgets} }}" : "{{ theme : 'bootstrap'{widgets}}}")} )
                                  .tablesorterPager({{
                                                     container: $('{pagerSelector}')
                                                     }});
                    }});";
        }

        /// <summary>
        /// Generated the load Script for the DataGrid table fix
        /// </summary>
        /// <param name="selector">The id of the DataGrid.</param>
        /// <returns>
        /// Returns the Java Script
        /// </returns>
        public static string FixGridTable([NotNull] string selector)
        {
            var widgets =
                ", widgets: ['zebra', 'reflow'], widgetOptions: { reflow_className: 'ui-table-reflow',reflow_headerAttrib: 'data-name',reflow_dataAttrib: 'data-title'}";

            return $@"{Config.JQueryAlias}(document).ready(function() {{
                        var table = document.getElementById('{selector}'); 
                        if(table != null) {{
                            var head = document.createElement('THEAD');
                            head.appendChild(table.rows[0]);
                            table.insertBefore(head, table.childNodes[0]); 
                        }}
                        {Config.JQueryAlias}('#{selector}').tablesorter({{ theme: 'bootstrap'{widgets}}});
                    }});";
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
            return $@"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadGotoAnchor);
            function loadGotoAnchor() {{
               document.getElementById('{anchor}').scrollIntoView();
			      }}";
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
            return $@"{Config.JQueryAlias}(document).ready(function() {{  
                                {Config.JQueryAlias}('{openLink}').click(function () {{ 
                                        {Config.JQueryAlias}('{dialogId}').modal('show')   
                                }}); 
                   }});";
        }

        /// <summary>
        /// script for the add Favorite Topic button
        /// </summary>
        /// <param name="untagButtonHtml">
        /// HTML code for the "un Tag As Favorite" button
        /// </param>
        /// <returns>
        /// The add Favorite Topic JS.
        /// </returns>
        public static string AddFavoriteTopicJs([NotNull] string untagButtonHtml)
        {
            return $@"function addFavoriteTopic(topicID){{ 
            {Config.JQueryAlias}.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/FavoriteTopic/AddFavoriteTopic/' + topicID,
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    success: function(response) {{
                              {Config.JQueryAlias}('#dvFavorite1').html({untagButtonHtml});
                              {Config.JQueryAlias}('#dvFavorite2').html({untagButtonHtml});
                    }},
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
                          
                 }}";
        }

        /// <summary>
        /// script for the remove Favorite Topic button
        /// </summary>
        /// <param name="tagButtonHtml">
        /// HTML code for the "Tag As a Favorite" button
        /// </param>
        /// <returns>
        /// The remove Favorite Topic JS.
        /// </returns>
        public static string RemoveFavoriteTopicJs([NotNull] string tagButtonHtml)
        {
            return $@"function removeFavoriteTopic(topicID){{ 
            {Config.JQueryAlias}.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/FavoriteTopic/RemoveFavoriteTopic/' + topicID,
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    success: function(response) {{
                              {Config.JQueryAlias}('#dvFavorite1').html({tagButtonHtml});
                              {Config.JQueryAlias}('#dvFavorite2').html({tagButtonHtml});
                    }},
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
                          
                 }}";
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
            return $@"function addThanks(messageID){{ 
            {Config.JQueryAlias}.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/ThankYou/AddThanks/' + messageID,
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    success: function(response) {{
                              {Config.JQueryAlias}('#dvThanksInfo' + response.MessageID).html(response.ThanksInfo);
                              {Config.JQueryAlias}('#dvThankBox' + response.MessageID).html({removeThankBoxHtml});

                              {Config.JQueryAlias}('.thanks-popover').popover({{
                                     template: '<div class=""popover"" role=""tooltip""><div class=""arrow""></div><h3 class=""popover-header""></h3><div class=""popover-body popover-body-scrollable""></div></div>'}});
                    }},
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
                          
                 }}";
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
            return $@"function removeThanks(messageID){{ 
            $.ajax({{
                    url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/ThankYou/RemoveThanks/' + messageID,
                    type: 'POST',
                    contentType: 'application/json;charset=utf-8',
                    success: function(response) {{
                              $('#dvThanksInfo' + response.MessageID).html(response.ThanksInfo);
                              $('#dvThankBox' + response.MessageID).html({addThankBoxHtml});
                    }},
                    error: function(x, e)  {{
                             console.log('An Error has occured!');
                             console.log(x.responseText);
                             console.log(x.status);
                    }}
                 }});
                          
                 }}";
        }

        #endregion

        /// <summary>
        /// The CKEditor Load JS.
        /// </summary>
        /// <param name="editorId">
        /// The editor Id.
        /// </param>
        /// <param name="editorLanguage">
        /// The editor language.
        /// </param>
        /// <param name="maxCharacters">
        /// The max characters.
        /// </param>
        /// <param name="themeCssUrl">
        /// The theme CSS url.
        /// </param>
        /// <param name="forumCssUrl">
        /// The forum CSS url.
        /// </param>
        /// <param name="toolbar">
        /// The toolbar.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public static string CKEditorLoadJs(
            [NotNull] string editorId,
            [NotNull] string editorLanguage,
            [NotNull] int maxCharacters,
            [NotNull] string themeCssUrl,
            [NotNull] string forumCssUrl,
            [NotNull] string toolbar)
        {
            return $@"{Config.JQueryAlias}(document).ready(function() {{
                      var yafCKEditor = {Config.JQueryAlias}(""#{editorId}"").ckeditor({{
                          extraPlugins: ""bbcode,mentions,highlight,bbcodeselector,syntaxhighlight,emoji,wordcount,autolink,albumsbrowser,attachments,quote,codemirror"",
                          removePlugins: 'bidi,dialogadvtab,div,filebrowser,flash,format,forms,horizontalrule,iframe,liststyle,pagebreak,showborders,stylescombo,table,tabletools,templates',
		                  toolbar: [{toolbar}],
		                  entities_greek: false,
                          entities_latin: false,
                          language: '{editorLanguage}',
                          disableObjectResizing: true,
		                  fontSize_sizes: ""30/30%;50/50%;100/100%;120/120%;150/150%;200/200%;300/300%"",
		                  forcePasteAsPlainText: true,
		                  contentsCss: [""{themeCssUrl}"", ""{forumCssUrl}""],
                          autosave:
                          {{
                              saveDetectionSelectors: ""a[id*='_PostReply'],a[id*='Cancel'],a[id*='_Preview']""
                          }},
                          codemirror: {{mode: ""bbcode"",  theme: ""monokai""}},
                          wordcount:
                          {{
                              maxCharCount: {maxCharacters},showParagraphs: false,showWordCount: false,showCharCount: true,countHTML: true
                          }},
		                  mentions: [ {{ feed:  CKEDITOR.basePath.replace('Scripts/ckeditor/', '') + 'resource.ashx?users={{encodedQuery}}',
                                         itemTemplate: '<li data-id=""{{id}}""><i class=""fas fa-user pr-1""></i><strong class=""username"">{{name}}</strong></li>',
		                                 outputTemplate: '@[userlink]{{name}}[/userlink]'
          		                      }} ]
                          }});

                      {Config.JQueryAlias}(""a[id*='_PostReply'],a[id*='_Save'],a[id*='_Preview']"").click(function () {{
                          yafCKEditor.editor.updateElement();
                      }});
                  }});

                  CKEDITOR.on('instanceReady', function (ev) {{
                     ev.editor.document.on('drop', function (event) {{
                       {Config.JQueryAlias}('.EditorDiv').yafFileUpload(""send"", {{files: event.data.$.dataTransfer.files}});
                     }});
                  }});";
        }

        /// <summary>
        /// The CKEditor Load JS.
        /// </summary>
        /// <param name="editorId">
        /// The editor Id.
        /// </param>
        /// <param name="editorLanguage">
        /// The editor language.
        /// </param>
        /// <param name="maxCharacters">
        /// The max characters.
        /// </param>
        /// <param name="themeCssUrl">
        /// The theme CSS url.
        /// </param>
        /// <param name="forumCssUrl">
        /// The forum CSS url.
        /// </param>
        /// <param name="toolbar">
        /// The toolbar.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        public static string CKEditorBasicLoadJs(
            [NotNull] string editorId,
            [NotNull] string editorLanguage,
            [NotNull] int maxCharacters,
            [NotNull] string themeCssUrl,
            [NotNull] string forumCssUrl,
            [NotNull] string toolbar)
        {
            return $@"{Config.JQueryAlias}(document).ready(function() {{
                      var yafCKEditor = {Config.JQueryAlias}(""#{editorId}"").ckeditor({{
                          extraPlugins: ""bbcode,mentions,wordcount,autolink,quote,codemirror"",
                          removePlugins: 'autosave,bidi,dialogadvtab,div,filebrowser,flash,format,forms,horizontalrule,iframe,liststyle,pagebreak,showborders,stylescombo,table,tabletools,templates',
		                  toolbar: [{toolbar}],
		                  entities_greek: false,
                          entities_latin: false,
                          language: '{editorLanguage}',
                          disableObjectResizing: true,
		                  forcePasteAsPlainText: true,
		                  contentsCss: [""{themeCssUrl}"", ""{forumCssUrl}""],
                          wordcount:
                          {{
                              maxCharCount: {maxCharacters},showParagraphs: false,showWordCount: false,showCharCount: true,countHTML: true
                          }},
                          codemirror: {{mode: ""bbcode"",  theme: ""monokai""}},
		                  mentions: [ {{ feed:  CKEDITOR.basePath.replace('Scripts/ckeditor/', '') + 'resource.ashx?users={{encodedQuery}}',
                                         itemTemplate: '<li data-id=""{{id}}""><i class=""fas fa-user pr-1""></i><strong class=""username"">{{name}}</strong></li>',
		                                 outputTemplate: '@[userlink]{{name}}[/userlink]'
          		                      }} ]
                          }});

                          {Config.JQueryAlias}(""a[id*='_QuickReplyDialog'],a[id*='_SignatureEdit']"").click(function () {{
                              yafCKEditor.editor.updateElement();
                          }});
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
        /// <param name="forumId">
        /// The forum identifier.
        /// </param>
        /// <param name="boardId">
        /// The board identifier.
        /// </param>
        /// <param name="imageMaxWidth">
        /// The image Max Width.
        /// </param>
        /// <param name="imageMaxHeight">
        /// The image Max Height.
        /// </param>
        /// <param name="editorId">
        /// The editor Id.
        /// </param>
        /// <returns>
        /// Returns the FileUpload Java Script.
        /// </returns>
        [NotNull]
        public static string FileAutoUploadLoadJs(
            [NotNull] string acceptedFileTypes,
            [NotNull] int maxFileSize,
            [NotNull] string fileUploaderUrl,
            [NotNull] int forumId,
            [NotNull] int boardId,
            [NotNull] int imageMaxWidth,
            [NotNull] int imageMaxHeight,
            [NotNull] string editorId)
        {
            return $@"{Config.JQueryAlias}('.EditorDiv').yafFileUpload({{
                url: '{fileUploaderUrl}',
                acceptFileTypes: new RegExp('(\.|\/)(' + '{acceptedFileTypes}' + ')', 'i'),
                imageMaxWidth: {imageMaxWidth},
                imageMaxHeight: {imageMaxHeight},
                autoUpload: true,
                disableImageResize: /Android(?!.*Chrome)|Opera/
                .test(window.navigator && navigator.userAgent),
                dataType: 'json',
                {(maxFileSize > 0 ? $"maxFileSize: {maxFileSize}," : string.Empty)}
                done: function (e, data) {{
                    var ckEditor = CKEDITOR.instances.{editorId}; 
                    ckEditor.insertHtml( '[attach]' + data.result[0].fileID + '[/attach]' );
                }},
                formData: {{
                    forumID: '{forumId}',
                    boardID: '{boardId}',
                    userID: '{BoardContext.Current.PageUserID}',
                    uploadFolder: '{BoardFolders.Current.Uploads}',
                    allowedUpload: true
                }},
                dropZone: {Config.JQueryAlias}('.EditorDiv'),
                pasteZone: {Config.JQueryAlias}('.EditorDiv')
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
        /// <param name="forumId">
        /// The forum identifier.
        /// </param>
        /// <param name="boardId">
        /// The board identifier.
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
            [NotNull] int forumId,
            [NotNull] int boardId,
            [NotNull] int imageMaxWidth,
            [NotNull] int imageMaxHeight)
        {
            return $@"{Config.JQueryAlias}(function() {{

            {Config.JQueryAlias}('#fileupload').yafFileUpload({{
                url: '{fileUploaderUrl}',
                acceptFileTypes: new RegExp('(\.|\/)(' + '{acceptedFileTypes}' + ')', 'i'),
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
                    CKEDITOR.tools.insertAttachment(data.result[0].fileID);
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
                    forumID: '{forumId}',
                    boardID: '{boardId}',
                    userID: '{BoardContext.Current.PageUserID}',
                    uploadFolder: '{BoardFolders.Current.Uploads}',
                    allowedUpload: true
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
        /// <param name="forumDropDownId">The forum drop down identifier.</param>
        /// <returns>Returns the select2 topics load JS.</returns>
        [NotNull]
        public static string SelectTopicsLoadJs([NotNull] string forumDropDownId)
        {
            return $@"{Config.JQueryAlias}('.TopicsSelect2Menu').select2({{
            ajax: {{
                url: '{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}/Topic/GetTopics',
                type: 'POST',
                dataType: 'json',
                minimumInputLength: 0,
                data: function(params) {{
                      var query = {{
                          ForumId : {Config.JQueryAlias}('#{forumDropDownId}').val(),
                          UserId: 0,
                          PageSize: 0,
                          Page : params.page || 0,
                          SearchTerm : params.term || ''
                      }}
                      return query;
                }},
                error: function(x, e)  {{
                       console.log('An Error has occured!');
                       console.log(x.responseText);
                       console.log(x.status);
                }},
                processResults: function(data, params) {{
                    params.page = params.page || 0;

                    var resultsperPage = 15 * 2;

                    var total = params.page == 0 ? data.Results.length : resultsperPage;

                    return {{
                        results: data.Results,
                        pagination: {{
                            more: total < data.Total
                        }}
                    }}
                }}
            }},
            width: 'style',
            theme: 'bootstrap4',
            allowClear: true,
            cache: true,
            {BoardContext.Current.Get<ILocalization>().GetText("SELECT_LOCALE_JS")}
        }});";
        }

        /// <summary>
        /// Gets the Selected Quoting Java Script
        /// </summary>
        /// <param name="postUrl">The post URL.</param>
        /// <param name="toolTipText">The tool tip text.</param>
        /// <returns>Returns the the Selected Quoting Java Script</returns>
        [NotNull]
        public static string SelectedQuotingJs([NotNull] string postUrl, string toolTipText)
        {
            return $@"{Config.JQueryAlias}('.selectionQuoteable').each(function () {{
                         var $this = jQuery(this);
                         $this.selectedQuoting({{ URL: '{postUrl}', ToolTip: '{toolTipText}' }});
                     }});";
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
            return $@"{Config.JQueryAlias}(document).ready(function() {{

    {Config.JQueryAlias}('#{confirmPasswordClientId}').on('keyup', function(e) {{
        var password = {Config.JQueryAlias}('#{passwordClientId}').val();
        var passwordConfirm = {Config.JQueryAlias}('#{confirmPasswordClientId}').val();

        if(password == '' && passwordConfirm == '') {{
            {Config.JQueryAlias}('#passwordStrength').removeClass().empty();
            {Config.JQueryAlias}('#passwordStrength').parent().parent('.post').hide();

            return false;
        }}
        else
        {{
             if(password != passwordConfirm) {{
    		    {Config.JQueryAlias}('#passwordStrength').removeClass().addClass('alert alert-danger').html('<p><i class=""fas fa-exclamation-circle""></i> {notMatchText}</p>');
                {Config.JQueryAlias}('#passwordStrength').parent().parent('.post').show();
        	    return false;
    	     }}
             else {{
                {Config.JQueryAlias}('#passwordStrength').removeClass().empty();
                {Config.JQueryAlias}('#passwordStrength').parent().parent('.post').hide();
             }}
         }}
    }});

    {Config.JQueryAlias}('#{passwordClientId}').on('keyup', function(e) {{

        var password = {Config.JQueryAlias}('#{passwordClientId}').val();
        var passwordConfirm = {Config.JQueryAlias}('#{confirmPasswordClientId}').val();

        if(password == '' && passwordConfirm == '')
        {{
            {Config.JQueryAlias}('#passwordStrength').removeClass().empty();
            {Config.JQueryAlias}('#passwordStrength').parent().parent('.post').hide();

            return false;
        }}

        var strongRegex = new RegExp(""^(?=.{{8,}})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$"", ""g"");

        var mediumRegex = new RegExp(""^(?=.{{7,}})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$"", ""g"");

        var okRegex = new RegExp(""(?=.{{{minimumChars},}}).*"", ""g"");

        if (okRegex.test(password) === false) {{
            {Config.JQueryAlias}('#passwordStrength').removeClass().addClass('alert alert-danger').html('<p><i class=""fas fa-exclamation-circle""></i> {passwordMinText}</p>');

        }} else if (strongRegex.test(password)) {{
            {Config.JQueryAlias}('#passwordStrength').removeClass().addClass('alert alert-info').html('<p><i class=""fas fa-exclamation-circle""></i> {passwordGoodText}</p>');
        }} else if (mediumRegex.test(password)) {{
            {Config.JQueryAlias}('#passwordStrength').removeClass().addClass('alert alert-warning').html('<p><i class=""fas fa-exclamation-circle""></i> {passwordStrongerText}</p>');
        }} else {{
            {Config.JQueryAlias}('#passwordStrength').removeClass().addClass('alert alert-danger').html('<p><i class=""fas fa-exclamation-circle""></i> {passwordWeakText}</p>');
        }}

        {Config.JQueryAlias}('#passwordStrength').parent().parent('.post').show();

        return true;
    }});
}});";
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
            return $"{Config.JQueryAlias}('#{clientId}').modal('show');";
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
            return "getSearchResultsData(0);";
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
            return $@"{Config.JQueryAlias}('.{cssClass}').popover({{
                           html: true,
                           content: ""{content}"",
                           trigger: ""focus hover""
                    }});";
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
            return $@"{Config.JQueryAlias}('{cssClass}').popover({{
                           title: '{title}',
                           html: true,
                           trigger: '{trigger}',
                           template: '<div class=""popover"" role=""tooltip""><h3 class=""popover-header""></h3><div class=""arrow""></div><div class=""popover-body""></div></div>'
                }});
                {Config.JQueryAlias}('{cssClass}').on('inserted.bs.popover', function () {{
                      {Config.JQueryAlias}('.popover-timeago').each(function() {{
                  {Config.JQueryAlias}(this).html(function(index, value) {{
                                          return moment(value).fromNow();
                  }});
                  {Config.JQueryAlias}(this).removeClass('popover-timeago');
            }});
                }})";
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
            return $@"{Config.JQueryAlias}('.forum-mods-popover').popover({{
                           title: '{title}',
                           html: true,
                           trigger: 'focus hover',
                           template: '<div class=""popover"" role=""tooltip""><h3 class=""popover-header""></h3><div class=""arrow""></div><div class=""popover-body popover-body-scrollable""></div></div>'
                }});";
        }
    }
}