/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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
        /// Gets Facebook Init Js.
        /// </summary>
        public static string FacebookInitJs
        {
            get
            {
                return @"{1}(document).ready(function() {{
                         {1}.getScript('//connect.facebook.net/en_US/all.js', function(data, textStatus, jqxhr) {{
                            FB.init({{appId:'{0}',status:true,cookie:true,xfbml:true}});
                         }});
                     }});".FormatWith(Config.FacebookAPIKey, Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets the script for album/image title/image callback.
        /// </summary>
        /// <returns>
        ///   the callback success js.
        /// </returns>
        [NotNull]
        public static string AlbumCallbackSuccessJS
        {
            get
            {
                return @"function changeTitleSuccess(res){{
                  spnTitleVar = document.getElementById('spnTitle' + res.d.Id);
                  txtTitleVar =  document.getElementById('txtTitle' + res.d.Id);
                  spnTitleVar.firstChild.nodeValue = res.d.NewTitle;
                  txtTitleVar.disabled = false;
                  spnTitleVar.style.display = 'inline';
                  txtTitleVar.style.display='none';}}";
            }
        }

        /// <summary>
        ///   Gets Pagination Load JS.
        /// </summary>
        [NotNull]
        public static string PaginationLoadJs
        {
            get
            {
                return @"function pageselectCallback(page_index, jq){{
                var new_content = {0}('#SmiliesPagerHidden div.result:eq('+page_index+')').clone();
                {0}('#SmiliesPagerResult').empty().append(new_content);
                return false;
            }}
           
            {0}(document).ready(function(){{      
                var num_entries = {0}('#SmiliesPagerHidden div.result').length;
                {0}('#SmiliesPager').pagination(num_entries, {{
                    callback: pageselectCallback,
                    items_per_page:1,
					num_display_entries: 3,
					num_edge_entries: 1,
                    prev_class: 'smiliesPagerPrev',
					next_class: 'smiliesPagerNext',
					prev_text: '&laquo;',
					next_text: '&raquo;'
                }});
            }});".FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets CeeBox Load JS.
        /// </summary>
        [NotNull]
        public static string CeeBoxLoadJs
        {
            get
            {
                return @"{0}(document).ready(function() {{ 
					{0}('.ceebox').ceebox({{titles:true}});}});".FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        /// Gets the multi quote callback success JS.
        /// </summary>
        [NotNull]
        public static string MultiQuoteCallbackSuccessJS
        {
            get
            {
                return @"function multiQuoteSuccess(res){{
                  var multiQuoteButton = {0}('#' + res.d.Id).parent('span');
                  multiQuoteButton.removeClass(multiQuoteButton.attr('class')).addClass(res.d.NewTitle);
                  {0}(document).scrollTop(multiQuoteButton.offset().top - 20);
                      }}".FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        /// Gets the multi quote button JS.
        /// </summary>
        [NotNull]
        public static string MultiQuoteButtonJs
        {
            get
            {
                return
                    @"function handleMultiQuoteButton(button, msgId, tpId){{
                     var messageId = msgId,topicId = tpId, cssClass = {1}('#' + button.id).parent('span').attr('class');
                     {1}.PageMethod('{0}YafAjax.asmx', 'HandleMultiQuote', multiQuoteSuccess, CallFailed, 'buttonId', button.id, 'multiquoteButton', button.checked, 'messageId', messageId, 'topicId', topicId, 'buttonCssClass', cssClass);}}"
                        .FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets the script for changing the album title.
        /// </summary>
        /// <returns>
        ///   the change album title js.
        /// </returns>
        [NotNull]
        public static string ChangeAlbumTitleJs
        {
            get
            {
                return
                    @"function changeAlbumTitle(albumId, txtTitleId){{
                     var albId = albumId;var newTitleTxt = {1}('#' + txtTitleId).val();
                     {1}.PageMethod('{0}YafAjax.asmx', 'ChangeAlbumTitle', changeTitleSuccess, CallFailed, 'albumID', albId, 'newTitle', newTitleTxt);}}"
                        .FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets the script for changing the image caption.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static string ChangeImageCaptionJs
        {
            get
            {
                return
                    @"function changeImageCaption(imageID, txtTitleId){{
              var imgId = imageID;var newImgTitleTxt = {1}('#' + txtTitleId).val();
              {1}.PageMethod('{0}YafAjax.asmx', 'ChangeImageCaption', changeTitleSuccess, CallFailed, 'imageID', imgId, 'newCaption', newImgTitleTxt);}}"
                        .FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets DisablePageManagerScrollJs.
        /// </summary>
        [NotNull]
        public static string DisablePageManagerScrollJs
        {
            get
            {
                return @"
	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}
";
            }
        }

        /// <summary>
        ///   Gets Repuatation Progress Load JS.
        /// </summary>
        [NotNull]
        public static string RepuatationProgressLoadJs
        {
            get
            {
                return @"{0}(document).ready(function() {{
					{0}('.ReputationBar').progressbar({{
			            create: function(event, ui) {{
			                    ChangeReputationBarColor({0}(this).attr('data-percent'),{0}(this).attr('data-text'), this);
			                    }}
		             }});}});".FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets TimeagoLoadJs.
        /// </summary>
        public static string TimeagoLoadJs
        {
            get
            {
                return @" if( typeof(CKEDITOR) == 'undefined') {{
            function loadTimeAgo() {{	
            {2}.timeago.settings.refreshMillis = {1};			      	
            {0}
		    {2}('abbr.timeago').timeago();	
			      }} 
                   Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadTimeAgo);
                   }};".FormatWith(
                    YafContext.Current.Get<ILocalization>().GetText("TIMEAGO_JS"),
                    YafContext.Current.Get<YafBoardSettings>().RelativeTimeRefreshTime,
                    Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets ToggleMessageJs.
        /// </summary>
        [NotNull]
        public static string ToggleMessageJs
        {
            get
            {
                return @"
                      function toggleMessage(divId)
                      {{ {0}('#' + divId).toggle(); }}".FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        ///  Gets the If asynchronous callback encounters any problem, this javascript function will be called.
        /// </summary>
        [NotNull]
        public static string AsynchCallFailedJs
        {
            get
            {
                return "function CallFailed(res){{alert('Error Occurred');}}";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The Post to Facebook Js
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <param name="picture">
        /// The picture.
        /// </param>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <returns>
        /// Returns the The Post to Facebook Js
        /// </returns>
        [NotNull]
        public static string FacebookPostJs(
            string message,
            string description,
            string link,
            string picture,
            string caption)
        {
            return @"function postToFacebook() {{

                   FB.getLoginStatus(function(response) {{
  if (response.status === 'connected') {{
    
    FB.ui(
                                {{ method: 'feed', name: '{0}', link: '{2}', picture: '{3}', caption: '{4}', description: '{1}', message: '{0}'
                                }},
                                function(response) {{
                                  if (response && response.post_id) {{
                                    alert('Post was published.');
                                  }} else {{
                                    alert('Post was not published.');
                                  }}
                                }});
  }} else {{
     FB.login(function(response) {{
                       if (response.authResponse) {{
                             FB.ui(
                                {{ method: 'feed', name: '{0}', link: '{2}', picture: '{3}', caption: '{4}', description: '{1}', message: '{0}'
                                }},
                                function(response) {{
                                  if (response && response.post_id) {{
                                    alert('Post was published.');
                                  }} else {{
                                    alert('Post was not published.');
                                  }}
                                }});
                             }}else {{
                                 alert('Not Logged in on Facebook!');
                                }}
                             }});
  }}
 }});

                         }}".FormatWith(message, description, link, picture, caption);
        }

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
            return
                ("function showTexBox(spnTitleId){{"
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
                 + "else resetBox(txtTitleId, isAlbum);}}" + "else resetBox(txtTitleId, isAlbum);}}").FormatWith(
                     albumEmptyTitle,
                     imageEmptyCaption);
        }

        /// <summary>
        /// Blocks the UI js.
        /// </summary>
        /// <param name="elementId">The element Id.</param>
        /// <returns>
        /// The block ui execute js.
        /// </returns>
        public static string BlockUIExecuteJs([NotNull] string elementId)
        {
            return
                @"{1}(document).ready(function() {{ {1}.blockUI({{ message: {1}('#{0}') }}); }});".FormatWith(
                    elementId,
                    Config.JQueryAlias);
        }

        /// <summary>
        /// Generates jQuery UI DatePicker Script
        /// </summary>
        /// <param name="fieldId">
        /// The Id of the Control to Bind the DatePicker
        /// </param>
        /// <param name="dateFormat">
        /// Localized Date Format
        /// </param>
        /// <param name="culture">
        /// Current Culture
        /// </param>
        /// <returns>
        /// The Load JS.
        /// </returns>
        public static string DatePickerLoadJs(
            [NotNull] string fieldId,
            [NotNull] string dateFormat,
            [NotNull] string culture)
        {
            var cultureJs = string.Empty;

            dateFormat = dateFormat.ToLower();

            dateFormat = dateFormat.Replace("yyyy", "yy");

            if (culture.IsSet())
            {
                cultureJs = @"{2}('#{0}').datepicker('option', {2}.datepicker.regional['{1}']);".FormatWith(
                    fieldId,
                    culture,
                    Config.JQueryAlias);
            }

            return
                @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadDatePicker);
                  function loadDatePicker() {{	{3}(document).ready(function() {{ {3}('#{0}').datepicker({{showButtonPanel: true,changeMonth:true,changeYear:true,yearRange: '-100:+0',maxDate:'+0d',dateFormat:'{1}',}}); {2} }});}} "
                    .FormatWith(fieldId, dateFormat, cultureJs, Config.JQueryAlias);
        }

        /// <summary>
        /// Generates jQuery Select Menu (with icons) load JS
        /// </summary>
        /// <param name="dropDownId">
        /// The drop Down client Id.
        /// </param>
        /// <returns>
        /// The Load JS.
        /// </returns>
        public static string SelectMenuWithIconsJs([NotNull] string dropDownId)
        {
            return @"Sys.Application.add_load(render_IconSelectMenu);function render_IconSelectMenu() {{ 
                                    {0}('#{1}').iconselectmenu({{
            change: function() {{
                if (typeof ({0}(this).attr('onchange')) !== 'undefined') {{
                            __doPostBack({0}(this).attr('name'),'');
                        }}
        }}
        }}).iconselectmenu('menuWidget').addClass('ui-menu-icons customicon'); 
                     }} ".FormatWith(Config.JQueryAlias, dropDownId);
        }

        /// <summary>
        /// Gets the jQuery-UI Tabs Load JS.
        /// </summary>
        /// <param name="tabId">The tab Id.</param>
        /// <param name="hiddenId">The hidden Id.</param>
        /// <param name="hightTransition">Height Transition</param>
        /// <returns>
        /// Returns the the jQuery-UI Tabs Load JS string
        /// </returns>
        public static string JqueryUITabsLoadJs([NotNull] string tabId, [NotNull] string hiddenId, bool hightTransition)
        {
            return JqueryUITabsLoadJs(tabId, hiddenId, string.Empty, string.Empty, hightTransition, true);
        }

        /// <summary>
        /// Gets the jQuery-UI Tabs Load JS.
        /// </summary>
        /// <param name="tabId">The tab Id.</param>
        /// <param name="hiddenId">The hidden Id.</param>
        /// <param name="hiddenTabId">The hidden tab identifier.</param>
        /// <param name="hightTransition">Height Transition</param>
        /// <returns>
        /// Returns the the jQuery-UI Tabs Load JS string
        /// </returns>
        public static string JqueryUITabsLoadJs(
            [NotNull] string tabId,
            [NotNull] string hiddenId,
            [NotNull] string hiddenTabId,
            bool hightTransition)
        {
            return JqueryUITabsLoadJs(tabId, hiddenId, hiddenTabId, string.Empty, hightTransition, true);
        }

        /// <summary>
        /// Gets the jQuery-UI Tabs Load JS.
        /// </summary>
        /// <param name="tabId">
        /// The tab Id.
        /// </param>
        /// <param name="hiddenId">
        /// The hidden Id.
        /// </param>
        /// <param name="hiddenTabId">
        /// The hidden tab id.
        /// </param>
        /// <param name="postbackJs">
        /// The PostBack JS.
        /// </param>
        /// <param name="hightTransition">
        /// Height Transition
        /// </param>
        /// <param name="addSelectedFunction">
        /// The add Selected Function.
        /// </param>
        /// <returns>
        /// Returns the the jQuery-UI Tabs Load JS string
        /// </returns>
        public static string JqueryUITabsLoadJs(
            [NotNull] string tabId,
            [NotNull] string hiddenId,
            [CanBeNull] string hiddenTabId,
            [CanBeNull] string postbackJs,
            bool hightTransition,
            bool addSelectedFunction)
        {
            string heightTransitionJs = hightTransition ? ", fx:{height:'toggle'}" : string.Empty;

            string selectFunctionJs = addSelectedFunction
                                          ? ", beforeActivate: function(event, ui) {{ {0}('#{1}').val(ui.newTab.index());{0}('#{2}').val(ui.newPanel.selector.replace('#', ''));{3} }}"
                                                .FormatWith(Config.JQueryAlias, hiddenId, hiddenTabId, postbackJs)
                                          : string.Empty;

            return @"{3}(document).ready(function() {{
					{3}('#{0}').tabs(
                    {{
            show: function() {{
                var sel = {3}('#{0}').tabs('option', 'active');

                {3}('#{1}').val(sel);
            }},
            active: {3}('#{1}').val()
            {2}
            {4}
        }});
                    }});".FormatWith(tabId, hiddenId, heightTransitionJs, Config.JQueryAlias, selectFunctionJs);
        }

        /// <summary>
        /// Loads the spinner widget for time correction.
        /// </summary>
        /// <returns>Returns the spinner widget for time correction.</returns>
        public static string LoadSpinnerWidgetForTimeCorrection()
        {
            return @"{0}(document).ready(function() {{
                        if (typeof (jQuery.fn.spinner) !== 'undefined') {{
                            {0}('.NumericServerTimeCorrection').spinner({{min: -720, max: 720}});
                        }}
                    }});".FormatWith(Config.JQueryAlias);
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
                options.IsSet() ? "{{ {0} }}".FormatWith(options) : string.Empty);
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
        public static string YafModalDialogLoadJs([NotNull] string openLink, [NotNull] string dialogId)
        {
            return
                @"{3}(document).ready(function() {{{3}('{0}').YafModalDialog({{Dialog : '{1}',ImagePath : '{2}'}}); }});"
                    .FormatWith(openLink, dialogId, YafForumInfo.GetURLToContent("images/"), Config.JQueryAlias);
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
            return @"function addFavoriteTopic(topicID){{ var topId = topicID; {2}.PageMethod('{1}YafAjax.asmx', 'AddFavoriteTopic', addFavoriteTopicSuccess, CallFailed, 'topicId', topId);}}
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
            return @"function addThanks(messageID){{ var messId = messageID;{2}.PageMethod('{1}YafAjax.asmx', 'AddThanks', addThanksSuccess, CallFailed, 'msgID', messId);}}
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
            return @"function removeFavoriteTopic(topicID){{ var topId = topicID;{2}.PageMethod('{1}YafAjax.asmx', 'RemoveFavoriteTopic', removeFavoriteTopicSuccess, CallFailed, 'topicId', topId);}}
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
            return @"function removeThanks(messageID){{ var messId = messageID;{2}.PageMethod('{1}YafAjax.asmx', 'RemoveThanks', removeThanksSuccess, CallFailed, 'msgID', messId);}}
          function removeThanksSuccess(res){{if (res.d != null) {{
                   {2}('#dvThanks' + res.d.MessageID).html(res.d.Thanks);
                   {2}('#dvThanksInfo' + res.d.MessageID).html(res.d.ThanksInfo);
                   {2}('#dvThankBox' + res.d.MessageID).html({0});}}}}".FormatWith(
                addThankBoxHTML,
                YafForumInfo.ForumClientFileRoot,
                Config.JQueryAlias);
        }

        /// <summary>
        /// The Reputation Progress Bar Change Js Code
        /// </summary>
        /// <param name="generateReputationBar">The generate reputation bar.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Returns the JS Code string</returns>
        [NotNull]
        public static string ReputationProgressChangeJs([NotNull] string generateReputationBar, [NotNull] string userId)
        {
            return @"{0}(document).ready(function() {{
                    {0}('.AddReputation_{1}').remove();
                    {0}('.RemoveReputation_{1}').remove();
                    {0}('.ReputationUser_{1}').replaceWith('{2}');
					{0}('.ReputationBar').progressbar({{
			            create: function(event, ui) {{
			                    ChangeReputationBarColor({0}(this).attr('data-percent'),{0}(this).attr('data-text'), this);
			                    }}
		             }});}});".FormatWith(Config.JQueryAlias, userId, generateReputationBar);
        }

        /// <summary>
        /// Toggle Event Log Item Js Scripts
        /// used to show/hide event log item
        /// </summary>
        /// <param name="showText">The show text.</param>
        /// <param name="hideText">The hide text.</param>
        /// <returns>Toggle Event Log Item Js</returns>
        [NotNull]
        public static string ToggleEventLogItemJs([NotNull] string showText, [NotNull] string hideText)
        {
            return @"function toggleEventLogItem(detailId) {{
                           var show = '{1}';var hide = '{2}';
	                       {0}('#Show'+ detailId).text({0}('#Show'+ detailId).text() == show ? hide : show);
                           {0}('#eventDetails' + detailId).slideToggle('slow'); return false;
                  }}".FormatWith(Config.JQueryAlias, showText, hideText);
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
            return
                "{0}('{1}').hovercard({{{2}width: 350,loadingHTML: '{3}',errorHTML: '{4}', delay: {5} }});".FormatWith(
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
        /// <returns>Returns the FileUpload Java Script.</returns>
        [NotNull]
        public static string FileUploadLoadJs(
            [NotNull] string acceptedFileTypes,
            [NotNull] int maxFileSize,
            [NotNull] string fileUploaderUrl,
            int forumID)
        {
            return @"{0}(function() {{

            {0}('#fileupload').fileupload({{
                url: '{3}',
                acceptFileTypes: new RegExp('(\.|\/)(' + '{2}' + ')', 'i'),
                dataType: 'json',
                {1}
                done: function (e, data) {{
                    setStyle('attach', data.result[0].fileID);
                    {0}('#fileupload').find('.files tr:first').remove();
                    
                    if ({0}('#fileupload').find('.files tr').length == 0) {{
                        {0}('.UploadDialog').dialog('close');
                    }}
                }},
                formData: {{
                    forumID: '{4}',
                    allowedUpload: true
                }},
                dropZone: {0}('#dropzone')
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
                forumID);
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
                         $this.selectionSharer({{ URL: '{1}', ToolTip: '{2}' }});
                     }});".FormatWith(Config.JQueryAlias, postURL, toolTipText);
        }

#region BootStrap Script Blocks

        /// <summary>
        /// Gets the tool tip load script block.
        /// </summary>
        /// <value>
        /// The tool tip load script block.
        /// </value>
        public static string ToolTipLoadScriptBlock
        {
            get
            {
                return "{0}('img, input, a').tooltip();".FormatWith(Config.JQueryAlias);
            }
        }

        #endregion
    }
}