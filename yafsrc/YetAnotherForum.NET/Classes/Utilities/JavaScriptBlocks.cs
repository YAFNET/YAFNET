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
namespace YAF.Utilities
{
    #region Using

    using YAF.Classes;
    using YAF.Core;
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
        ///   Gets the script for album/image title/image callback.
        /// </summary>
        /// <returns>
        ///   the callback success js.
        /// </returns>
        [NotNull]
        public static string AlbumCallbackSuccessJS =>
            @"function changeTitleSuccess(res){{
                  spnTitleVar = document.getElementById('spnTitle' + res.d.Id);
                  txtTitleVar =  document.getElementById('txtTitle' + res.d.Id);
                  spnTitleVar.firstChild.nodeValue = res.d.NewTitle;
                  txtTitleVar.disabled = false;
                  spnTitleVar.style.display = 'inline';
                  txtTitleVar.style.display='none';}}";

        /// <summary>
        /// Gets the multi quote callback success JS.
        /// </summary>
        [NotNull]
        public static string MultiQuoteCallbackSuccessJS =>
            @"function multiQuoteSuccess(res){{
                  var multiQuoteButton = {0}('#' + res.d.Id).parent('span');
                  multiQuoteButton.removeClass(multiQuoteButton.attr('class')).addClass(res.d.NewTitle);
                  {0}(document).scrollTop(multiQuoteButton.offset().top - 20);
                      }}".FormatWith(Config.JQueryAlias);

        /// <summary>
        /// Gets the multi quote button JS.
        /// </summary>
        [NotNull]
        public static string MultiQuoteButtonJs =>
            @"function handleMultiQuoteButton(button, msgId, tpId){{
                     var messageId = msgId,topicId = tpId, cssClass = {1}('#' + button.id).parent('span').attr('class');
                     {1}.PageMethod('{0}YafAjax.asmx', 'HandleMultiQuote', multiQuoteSuccess, CallFailed, 'buttonId', button.id, 'multiquoteButton', button.checked, 'messageId', messageId, 'topicId', topicId, 'buttonCssClass', cssClass);}}"
                .FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);

        /// <summary>
        ///   Gets the script for changing the album title.
        /// </summary>
        /// <returns>
        ///   the change album title js.
        /// </returns>
        [NotNull]
        public static string ChangeAlbumTitleJs =>
            @"function changeAlbumTitle(albumId, txtTitleId){{
                     var albId = albumId;var newTitleTxt = {1}('#' + txtTitleId).val();
                     {1}.PageMethod('{0}YafAjax.asmx', 'ChangeAlbumTitle', changeTitleSuccess, CallFailed, 'albumID', albId, 'newTitle', newTitleTxt);}}"
                .FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);

        /// <summary>
        ///   Gets the script for changing the image caption.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static string ChangeImageCaptionJs =>
            @"function changeImageCaption(imageID, txtTitleId){{
              var imgId = imageID;var newImgTitleTxt = {1}('#' + txtTitleId).val();
              {1}.PageMethod('{0}YafAjax.asmx', 'ChangeImageCaption', changeTitleSuccess, CallFailed, 'imageID', imgId, 'newCaption', newImgTitleTxt);}}"
                .FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);

        /// <summary>
        ///   Gets DisablePageManagerScrollJs.
        /// </summary>
        [NotNull]
        public static string DisablePageManagerScrollJs =>
            @"
	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}
";

        /// <summary>
        ///   Gets TimeagoLoadJs.
        /// </summary>
        public static string TimeagoLoadJs =>
            @" if( typeof(CKEDITOR) == 'undefined') {{
            function loadTimeAgo() {{
            
		     moment.locale('{1}');
            {0}('abbr.timeago').html(function(index, value) {{
                 
            return moment(value).fromNow(true);
            }});

            Prism.highlightAll();
			      }}
                   Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadTimeAgo);
                   }};".FormatWith(
                Config.JQueryAlias,
                YafContext.Current.CultureUser.IsSet()
                    ? YafContext.Current.CultureUser.Substring(0, 2)
                    : YafContext.Current.Get<YafBoardSettings>().Culture.Substring(0, 2));

        /// <summary>
        ///   Gets ToggleMessageJs.
        /// </summary>
        [NotNull]
        public static string ToggleMessageJs =>
            @"
                      function toggleMessage(divId)
                      {{ {0}('#' + divId).toggle(); }}".FormatWith(Config.JQueryAlias);

        /// <summary>
        ///  Gets the If asynchronous callback encounters any problem, this javascript function will be called.
        /// </summary>
        [NotNull]
        public static string AsynchCallFailedJs => "function CallFailed(res){{alert('Error Occurred');}}";

        #endregion

        #region Public Methods

        /// <summary>
        /// Javascript events for Album pages.
        /// </summary>
        /// <param name="albumEmptyTitle">
        /// The Album Empty Title.
        /// </param>
        /// <param name="imageEmptyCaption">
        /// The Image Empty Caption.
        /// </param>
        /// <returns>
        /// The album events js.
        /// </returns>
        public static string AlbumEventsJs([NotNull] string albumEmptyTitle, [NotNull] string imageEmptyCaption)
        {
            return ("function showTexBox(spnTitleId){{"
                    + "spnTitleVar = document.getElementById('spnTitle' + spnTitleId.substring(8));"
                    + "txtTitleVar = document.getElementById('txtTitle'+spnTitleId.substring(8));"
                    + "if (spnTitleVar.firstChild != null) txtTitleVar.setAttribute('value',spnTitleVar.firstChild.nodeValue);"
                    + "if(spnTitleVar.firstChild.nodeValue == '{0}' || spnTitleVar.firstChild.nodeValue == '{1}'){{txtTitleVar.value='';spnTitleVar.firstChild.nodeValue='';}}"
                    + "txtTitleVar.style.display = 'inline'; spnTitleVar.style.display = 'none'; txtTitleVar.focus();}}"
                    + "function resetBox(txtTitleId, isAlbum) {{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);"
                    + "txtTitleVar.style.display = 'none';txtTitleVar.disabled = false;spnTitleVar.style.display = 'inline';"
                    + "if (spnTitleVar.firstChild != null)txtTitleVar.value = spnTitleVar.firstChild.nodeValue;if (spnTitleVar.firstChild.nodeValue==''){{txtTitleVar.value='';if (isAlbum) spnTitleVar.firstChild.nodeValue='{0}';else spnTitleVar.firstChild.nodeValue='{1}';}}}}"
                    + "function checkKey(event, handler, id, isAlbum){{"
                    + "if ((event.keyCode == 13) || (event.which == 13)){{"
                    + "if (event.preventDefault) event.preventDefault(); event.cancel=true; event.returnValue=false; "
                    + "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{"
                    + "handler.disabled = true; if (isAlbum == true)changeAlbumTitle(id, handler.id); else changeImageCaption(id,handler.id);}}"
                    + "else resetBox(handler.id, isAlbum);}}"
                    + "else if ((event.keyCode == 27) || (event.which == 27))resetBox(handler.id, isAlbum);}}"
                    + "function blurTextBox(txtTitleId, id , isAlbum){{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);"
                    + "if (spnTitleVar.firstChild != null){{"
                    + "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{"
                    + "txtTitleVar.disabled = true; if (isAlbum == true)changeAlbumTitle(id, txtTitleId); else changeImageCaption(id,txtTitleId);}}"
                    + "else resetBox(txtTitleId, isAlbum);}}" + "else resetBox(txtTitleId, isAlbum);}}")
                .FormatWith(albumEmptyTitle, imageEmptyCaption);
        }

        /// <summary>
        /// Blocks the UI js.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="buttonId">The button identifier.</param>
        /// <returns>
        /// The block ui execute js.
        /// </returns>
        public static string BlockUIExecuteJs([NotNull] string messageId, [NotNull] string buttonId)
        {
            return @"{1}(document).ready(function() {{
                      {1}('{2}').click(function() {{ {1}.blockUI({{ message: {1}('#{0}') }});
                       }});
                      }});".FormatWith(messageId, Config.JQueryAlias, buttonId);
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
                cultureJs = ", locale: '{0}'".FormatWith(culture);
            }

            return @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadDatePicker);
                  function loadDatePicker() {{	{2}(document).ready(function() {{ {2}('.datepickerinput').datetimepicker({{format: '{0}'{1},icons:{{
            time: 'fa fa-clock fa-fw',
            date: 'fa fa-calendar fa-fw',
            up: 'fa fa-chevron-up fa-fw',
            down: 'fa fa-chevron-down fa-fw',
            previous: 'fa fa-chevron-left fa-fw',
            next: 'fa fa-chevron-right fa-fw',
            today: 'fa fa-sun fa-fw',
            clear: 'fa fa-trash fa-fw',
            close: 'fa fa-times fa-fw'
        }}}}); }});}} ".FormatWith(dateFormat, cultureJs, Config.JQueryAlias);
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
            return @"{2}(document).ready(function() {{
            var selectedTab = {2}(""#{1}"");
            var tabId = selectedTab.val() != """" ? selectedTab.val() : ""View1"";
            {2}('#{0} a[href=""#' + tabId + '""]').tab('show');
            {2}(""#{0} a"").click(function() {{
                selectedTab.val({2}(this).attr(""href"").substring(1));
                {3}
            }});
                           }});".FormatWith(tabId, hiddenId, Config.JQueryAlias, onClickEvent);
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
            return @"{0}(document).ready(function() {{
                        {0}('{1}').tablesorter( {2} );
                    }});".FormatWith(
                Config.JQueryAlias,
                selector,
                options.IsSet() ? "{{ theme: 'bootstrap', {0} }}".FormatWith(options) : "{{ theme: 'bootstrap' }}");
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
            return @"{0}(document).ready(function() {{
                        {0}('{1}').tablesorter( {2} )
                                  .tablesorterPager({{
                                                     container: $('{3}')
                                                     }});
                    }});".FormatWith(
                Config.JQueryAlias,
                selector,
                options.IsSet() ? "{{ {0} }}".FormatWith(options) : string.Empty,
                pagerSelector);
        }

        /// <summary>
        /// Loads the touch spin.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static string LoadTouchSpin([NotNull] string selector, [CanBeNull] string options)
        {
            return @"{0}(document).ready(function() {{
                        {0}('{1}').TouchSpin( {2} );
                    }});".FormatWith(
                Config.JQueryAlias,
                selector,
                options.IsSet() ? "{{ {0} }}".FormatWith(options) : string.Empty);
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
            return @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadGotoAnchor);
            function loadGotoAnchor() {{
               document.getElementById('{0}').scrollIntoView();
			      }}".FormatWith(anchor);
        }

        /// <summary>
        /// Generates Modal Dialog Script
        /// </summary>
        /// <param name="openLink">
        /// The Open Link, that opens the Modal Dialog.
        /// </param>
        /// <param name="dialogId">
        /// The Id or Css Class of the Dialog Content
        /// </param>
        /// <returns>
        /// The yaf modal dialog Load JS.
        /// </returns>
        public static string LoginBoxLoadJs([NotNull] string openLink, [NotNull] string dialogId)
        {
            return
                @"{2}(document).ready(function() {{  {2}('{0}').click(function () {{ {2}('{1}').modal('show')   }}); }});"
                    .FormatWith(openLink, dialogId, Config.JQueryAlias);
        }

        /// <summary>
        /// script for the add Favorite Topic button
        /// </summary>
        /// <param name="untagButtonHTML">
        /// HTML code for the "Untag As Favorite" button
        /// </param>
        /// <returns>
        /// The add Favorite Topic js.
        /// </returns>
        public static string AddFavoriteTopicJs([NotNull] string untagButtonHTML)
        {
            return
                @"function addFavoriteTopic(topicID){{ var topId = topicID; {2}.PageMethod('{1}YafAjax.asmx', 'AddFavoriteTopic', addFavoriteTopicSuccess, CallFailed, 'topicId', topId);}}
          function addFavoriteTopicSuccess(res){{if (res.d != null) {{
                   {2}('#dvFavorite1').html({0});
                   {2}('#dvFavorite2').html({0});}}}}".FormatWith(
                    untagButtonHTML,
                    YafForumInfo.ForumClientFileRoot,
                    Config.JQueryAlias);
        }

        /// <summary>
        /// script for the addThanks button
        /// </summary>
        /// <param name="removeThankBoxHTML">
        /// HTML code for the "Remove Thank" button
        /// </param>
        /// <returns>
        /// The add thanks js.
        /// </returns>
        public static string AddThanksJs([NotNull] string removeThankBoxHTML)
        {
            return
                @"function addThanks(messageID){{ var messId = messageID;{2}.PageMethod('{1}YafAjax.asmx', 'AddThanks', addThanksSuccess, CallFailed, 'msgID', messId);}}
          function addThanksSuccess(res){{if (res.d != null) {{
                   {2}('#dvThanks' + res.d.MessageID).html(res.d.Thanks);
                   {2}('#dvThanksInfo' + res.d.MessageID).html(res.d.ThanksInfo);
                   {2}('#dvThankBox' + res.d.MessageID).html({0});}}}}".FormatWith(
                    removeThankBoxHTML,
                    YafForumInfo.ForumClientFileRoot,
                    Config.JQueryAlias);
        }

        /// <summary>
        /// script for the remove Favorite Topic button
        /// </summary>
        /// <param name="tagButtonHTML">
        /// HTML code for the "Tag As a Favorite" button
        /// </param>
        /// <returns>
        /// The remove Favorite Topic js.
        /// </returns>
        public static string RemoveFavoriteTopicJs([NotNull] string tagButtonHTML)
        {
            return
                @"function removeFavoriteTopic(topicID){{ var topId = topicID;{2}.PageMethod('{1}YafAjax.asmx', 'RemoveFavoriteTopic', removeFavoriteTopicSuccess, CallFailed, 'topicId', topId);}}
          function removeFavoriteTopicSuccess(res){{if (res.d != null) {{
                   {2}('#dvFavorite1').html({0});
                   {2}('#dvFavorite2').html({0});}}}}".FormatWith(
                    tagButtonHTML,
                    YafForumInfo.ForumClientFileRoot,
                    Config.JQueryAlias);
        }

        /// <summary>
        /// script for the removeThanks button
        /// </summary>
        /// <param name="addThankBoxHTML">
        /// The Add Thank Box HTML.
        /// </param>
        /// <returns>
        /// The remove thanks js.
        /// </returns>
        public static string RemoveThanksJs([NotNull] string addThankBoxHTML)
        {
            return
                @"function removeThanks(messageID){{ var messId = messageID;{2}.PageMethod('{1}YafAjax.asmx', 'RemoveThanks', removeThanksSuccess, CallFailed, 'msgID', messId);}}
          function removeThanksSuccess(res){{if (res.d != null) {{
                   {2}('#dvThanks' + res.d.MessageID).html(res.d.Thanks);
                   {2}('#dvThanksInfo' + res.d.MessageID).html(res.d.ThanksInfo);
                   {2}('#dvThankBox' + res.d.MessageID).html({0});}}}}".FormatWith(
                    addThankBoxHTML,
                    YafForumInfo.ForumClientFileRoot,
                    Config.JQueryAlias);
        }

        #endregion

        /// <summary>
        /// Renders the Hover card Load JS.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="type">The type.</param>
        /// <param name="loadingHtml">The loading HTML.</param>
        /// <param name="errorHtml">The error HTML.</param>
        /// <returns>
        /// Returns the JS String
        /// </returns>
        [NotNull]
        public static string HoverCardLoadJs(
            [NotNull] string clientId,
            [NotNull] string type,
            [NotNull] string loadingHtml,
            [NotNull] string errorHtml)
        {
            return "{0}('{1}').hovercard({{{2}width: 350,loadingHTML: '{3}',errorHTML: '{4}', delay: {5} }});"
                .FormatWith(
                    Config.JQueryAlias,
                    clientId,
                    type.IsSet() ? "show{0}Card: true,".FormatWith(type) : string.Empty,
                    loadingHtml,
                    errorHtml,
                    YafContext.Current.Get<YafBoardSettings>().HoverCardOpenDelay);
        }

        /// <summary>
        /// Renders the Hover card Load JS.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="type">The type.</param>
        /// <param name="loadingHtml">The loading HTML.</param>
        /// <param name="errorHtml">The error HTML.</param>
        /// <param name="twitterUrl">The twitter URL.</param>
        /// <returns>
        /// Returns the JS String
        /// </returns>
        [NotNull]
        public static string HoverCardLoadJs(
            [NotNull] string clientId,
            [NotNull] string type,
            [NotNull] string loadingHtml,
            [NotNull] string errorHtml,
            [NotNull] string twitterUrl)
        {
            return
                "{0}('{1}').hovercard({{{2}width: 350,loadingHTML: '{3}',errorHTML: '{4}', delay: {5}, twitterURL: '{6}' }});"
                    .FormatWith(
                        Config.JQueryAlias,
                        clientId,
                        type.IsSet() ? "show{0}Card: true,".FormatWith(type) : string.Empty,
                        loadingHtml,
                        errorHtml,
                        YafContext.Current.Get<YafBoardSettings>().HoverCardOpenDelay,
                        twitterUrl);
        }

        /// <summary>
        /// Gets the FileUpload Java Script.
        /// </summary>
        /// <param name="acceptedFileTypes">The accepted file types.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        /// <param name="fileUploaderUrl">The file uploader URL.</param>
        /// <param name="forumID">The forum identifier.</param>
        /// <param name="boardID">The board identifier.</param>
        /// <returns>
        /// Returns the FileUpload Java Script.
        /// </returns>
        [NotNull]
        public static string FileUploadLoadJs(
            [NotNull] string acceptedFileTypes,
            [NotNull] int maxFileSize,
            [NotNull] string fileUploaderUrl,
            [NotNull] int forumID,
            [NotNull] int boardID)
        {
            return @"{0}(function() {{

            {0}('#fileupload').yafFileUpload({{
                url: '{3}',
                acceptFileTypes: new RegExp('(\.|\/)(' + '{2}' + ')', 'i'),
                dataType: 'json',
                {1}
                start: function (e) {{
                    {0}('.uploadCompleteWarning').toggle();
                }},
                done: function (e, data) {{
                    insertAttachment(data.result[0].fileID, data.result[0].fileID);
                    {0}('#fileupload').find('.files tr:first').remove();

                    if ({0}('#fileupload').find('.files tr').length == 0) {{
                        {0}('.UploadDialog').modal('hide');

                        var pageSize = 5;
                        var pageNumber = 0;
                        getPaginationData(pageSize, pageNumber, false);
                    }}
                }},
                formData: {{
                    forumID: '{4}',
                    boardID: '{5}',
                    userID: '{6}',
                    uploadFolder: '{7}',
                    allowedUpload: true
                }},
                dropZone: {0}('.UploadDialog')
            }});
            {0}(document).bind('dragover', function (e) {{
                var dropZone = {0}('#dropzone'),
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
        }});".FormatWith(
                Config.JQueryAlias,
                maxFileSize > 0 ? "maxFileSize: {0},".FormatWith(maxFileSize) : string.Empty,
                acceptedFileTypes,
                fileUploaderUrl,
                forumID,
                boardID,
                YafContext.Current.PageUserID,
                YafBoardFolders.Current.Uploads);
        }

        /// <summary>
        /// Selects the topics load js.
        /// </summary>
        /// <param name="forumDropDownID">The forum drop down identifier.</param>
        /// <returns></returns>
        [NotNull]
        public static string SelectTopicsLoadJs([NotNull] string forumDropDownID)
        {
            return @"{0}('.TopicsSelect2Menu').select2({{
            ajax: {{
                url: '{2}YafAjax.asmx/GetTopics',
                type: 'POST',
                dataType: 'json',
                minimumInputLength: 0,
                data: function(params) {{
                    return {{
                        'forumID': {0}('#{1}').val(),
                        'page': params.page || 0,
                        'searchTerm': params.term || ''
                    }};
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
            allowClear: true,
            cache: true,
            width: '350px',
            {3}
        }});".FormatWith(
                Config.JQueryAlias,
                forumDropDownID,
                YafForumInfo.ForumClientFileRoot,
                YafContext.Current.Get<ILocalization>().GetText("SELECT_LOCALE_JS"));
        }

        /// <summary>
        /// Gets the Selected Quoting Java Script
        /// </summary>
        /// <param name="postURL">The post URL.</param>
        /// <param name="toolTipText">The tool tip text.</param>
        /// <returns>Returns the the Selected Quoting Java Script</returns>
        [NotNull]
        public static string SelectedQuotingJs([NotNull] string postURL, string toolTipText)
        {
            return @"{0}('.selectionQuoteable').each(function () {{
                         var $this = jQuery(this);
                         $this.selectedQuoting({{ URL: '{1}', ToolTip: '{2}' }});
                     }});".FormatWith(Config.JQueryAlias, postURL, toolTipText);
        }

        /// <summary>
        /// Gets the Passwords strength checker Java Script.
        /// </summary>
        /// <param name="passwordClientID">The password client identifier.</param>
        /// <param name="confirmPasswordClientID">The confirm password client identifier.</param>
        /// <param name="minimumChars">The minimum chars.</param>
        /// <param name="notMatchText">The not match text.</param>
        /// <param name="passwordMinText">The password minimum text.</param>
        /// <param name="passwordGoodText">The password good text.</param>
        /// <param name="passwordStrongerText">The password stronger text.</param>
        /// <param name="passwordWeakText">The password weak text.</param>
        /// <returns>Returns the Passwords strength checker Java Script</returns>
        [NotNull]
        public static string PasswordStrengthCheckerJs(
            [NotNull] string passwordClientID,
            [NotNull] string confirmPasswordClientID,
            [NotNull] int minimumChars,
            [NotNull] string notMatchText,
            [NotNull] string passwordMinText,
            [NotNull] string passwordGoodText,
            [NotNull] string passwordStrongerText,
            [NotNull] string passwordWeakText)
        {
            return @"{0}(document).ready(function() {{

    {0}('#{2}').on('keyup', function(e) {{
        var password = {0}('#{1}').val();
        var passwordConfirm = {0}('#{2}').val();

        if(password == '' && passwordConfirm == '') {{
            {0}('#passwordStrength').removeClass().empty();
            {0}('#passwordStrength').parent().parent('.post').hide();

            return false;
        }}
        else
        {{
             if(password != passwordConfirm) {{
    		    {0}('#passwordStrength').removeClass().addClass('ui-state-error ui-corner-all').html('<p><span class=""PasswordStrengthMessageIcon ui-icon ui-icon-alert"" />{4}</p>');
                {0}('#passwordStrength').parent().parent('.post').show();
        	    return false;
    	     }}
             else {{
                {0}('#passwordStrength').removeClass().empty();
                {0}('#passwordStrength').parent().parent('.post').hide();
             }}
         }}
    }});

    {0}('#{1}').on('keyup', function(e) {{

        var password = {0}('#{1}').val();
        var passwordConfirm = {0}('#{2}').val();

        if(password == '' && passwordConfirm == '')
        {{
            {0}('#passwordStrength').removeClass().empty();
            {0}('#passwordStrength').parent().parent('.post').hide();

            return false;
        }}

        var strongRegex = new RegExp(""^(?=.{{8,}})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$"", ""g"");

        var mediumRegex = new RegExp(""^(?=.{{7,}})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$"", ""g"");

        var okRegex = new RegExp(""(?=.{{{3},}}).*"", ""g"");

        if (okRegex.test(password) === false) {{
            {0}('#passwordStrength').removeClass().addClass('ui-state-error ui-corner-all').html('<p><span class=""PasswordStrengthMessageIcon ui-icon ui-icon-alert"" />{5}</p>');

        }} else if (strongRegex.test(password)) {{
            {0}('#passwordStrength').removeClass().addClass('ui-state-default ui-corner-all').html('<p><span class=""PasswordStrengthMessageIcon ui-icon ui-icon-check"" />{6}</p>');
        }} else if (mediumRegex.test(password)) {{
            {0}('#passwordStrength').removeClass().addClass('ui-state-highlight ui-corner-all').html('<p><span class=""PasswordStrengthMessageIcon ui-icon ui-icon-info"" />{7}</p>');
        }} else {{
            {0}('#passwordStrength').removeClass().addClass('ui-state-error ui-corner-all').html('<p><span class=""PasswordStrengthMessageIcon ui-icon ui-icon-alert"" />{8}</p>');
        }}

        {0}('#passwordStrength').parent().parent('.post').show();

        return true;
    }});
}});".FormatWith(
                Config.JQueryAlias,
                passwordClientID,
                confirmPasswordClientID,
                minimumChars,
                notMatchText,
                passwordMinText,
                passwordGoodText,
                passwordStrongerText,
                passwordWeakText);
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
            return "{0}('#{1}').modal('show');".FormatWith(Config.JQueryAlias, clientId);
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
            return "getSeachResultsData(0);";
        }
    }
}