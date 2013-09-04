/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Utilities
{
    #region Using

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
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
               return
                   @"{1}(document).ready(function() {{
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
                return
                    @"function changeTitleSuccess(res){{
                  spnTitleVar = document.getElementById('spnTitle' + res.d.Id);
                  txtTitleVar =  document.getElementById('txtTitle' + res.d.Id);
                  spnTitleVar.firstChild.nodeValue = res.d.NewTitle;
                  txtTitleVar.disabled = false;
                  spnTitleVar.style.display = 'inline';
                  txtTitleVar.style.display='none';}}";
            }
        }

        /// <summary>
        ///   Gets Pagination Load Js.
        /// </summary>
        [NotNull]
        public static string PaginationLoadJs
        {
            get
            {
                return
                    @"function pageselectCallback(page_index, jq){{
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
            }});"
                        .FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets CeeBox Load Js.
        /// </summary>
        [NotNull]
        public static string CeeBoxLoadJs
        {
            get
            {
                return
                    @"{0}(document).ready(function() {{ 
					{0}('.ceebox').ceebox({{titles:true}});}});".FormatWith(
                        Config.JQueryAlias);
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
                return
                    @"function multiQuoteSuccess(res){{
                  var multiQuoteButton = {0}('#' + res.d.Id).parent('span');
                  multiQuoteButton.removeClass(multiQuoteButton.attr('class')).addClass(res.d.NewTitle);
                  {0}(document).scrollTop(multiQuoteButton.offset().top - 20);
                      }}"
                        .FormatWith(Config.JQueryAlias);
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
                    @"function handleMultiQuoteButton(button, msgId){{
                     var messageId = msgId;var cssClass = {1}('#' + button.id).parent('span').attr('class');
                     {1}.PageMethod('{0}YafAjax.asmx', 'HandleMultiQuote', multiQuoteSuccess, CallFailed, 'buttonId', button.id, 'multiquoteButton', button.checked, 'messageId', messageId, 'buttonCssClass', cssClass);}}"
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
                return
                    @"
	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}
";
            }
        }

        /// <summary>
        ///   Gets SyntaxHighlightLoadJs.
        /// </summary>
        [NotNull]
        public static string SyntaxHighlightLoadJs
        {
            get
            {
                return
                    @"{0}(document).ready(function() {{
					SyntaxHighlighter.all()}});".FormatWith(
                        Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets Repuatation Progress Load Js.
        /// </summary>
        [NotNull]
        public static string RepuatationProgressLoadJs
        {
            get
            {
                return
                    @"{0}(document).ready(function() {{
					{0}('.ReputationBar').progressbar({{
			            create: function(event, ui) {{
			                    ChangeReputationBarColor({0}(this).attr('data-percent'),{0}(this).attr('data-text'), this);
			                    }}
		             }});}});"
                        .FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        ///   Gets TimeagoLoadJs.
        /// </summary>
        public static string TimeagoLoadJs
        {
            get
            {
                return
                    @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadTimeAgo);
            function loadTimeAgo() {{	
            {2}.timeago.settings.refreshMillis = {1};			      	
            {0}
              {2}('abbr.timeago').timeago();	
			      }}"
                        .FormatWith(
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
                return
                    @"
                      function toggleMessage(divId)
                      {{ {0}('#' + divId).toggle(); }}"
                        .FormatWith(Config.JQueryAlias);
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
            string message, string description, string link, string picture, string caption)
        {
            return
                @"function postToFacebook() {{

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

                         }}"
                    .FormatWith(message, description, link, picture, caption);
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
                 +
                 "if (spnTitleVar.firstChild != null) txtTitleVar.setAttribute('value',spnTitleVar.firstChild.nodeValue);"
                 +
                 "if(spnTitleVar.firstChild.nodeValue == '{0}' || spnTitleVar.firstChild.nodeValue == '{1}'){{txtTitleVar.value='';spnTitleVar.firstChild.nodeValue='';}}"
                 + "txtTitleVar.style.display = 'inline'; spnTitleVar.style.display = 'none'; txtTitleVar.focus();}}"
                 +
                 "function resetBox(txtTitleId, isAlbum) {{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);"
                 +
                 "txtTitleVar.style.display = 'none';txtTitleVar.disabled = false;spnTitleVar.style.display = 'inline';"
                 +
                 "if (spnTitleVar.firstChild != null)txtTitleVar.value = spnTitleVar.firstChild.nodeValue;if (spnTitleVar.firstChild.nodeValue==''){{txtTitleVar.value='';if (isAlbum) spnTitleVar.firstChild.nodeValue='{0}';else spnTitleVar.firstChild.nodeValue='{1}';}}}}"
                 + "function checkKey(event, handler, id, isAlbum){{"
                 + "if ((event.keyCode == 13) || (event.which == 13)){{"
                 + "if (event.preventDefault) event.preventDefault(); event.cancel=true; event.returnValue=false; "
                 + "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{"
                 +
                 "handler.disabled = true; if (isAlbum == true)changeAlbumTitle(id, handler.id); else changeImageCaption(id,handler.id);}}"
                 + "else resetBox(handler.id, isAlbum);}}"
                 + "else if ((event.keyCode == 27) || (event.which == 27))resetBox(handler.id, isAlbum);}}"
                 +
                 "function blurTextBox(txtTitleId, id , isAlbum){{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);"
                 + "if (spnTitleVar.firstChild != null){{"
                 + "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{"
                 +
                 "txtTitleVar.disabled = true; if (isAlbum == true)changeAlbumTitle(id, txtTitleId); else changeImageCaption(id,txtTitleId);}}"
                 + "else resetBox(txtTitleId, isAlbum);}}" + "else resetBox(txtTitleId, isAlbum);}}").FormatWith(
                     albumEmptyTitle, imageEmptyCaption);
        }

        /// <summary>
        /// Requires {0} formatted elementId.
        /// </summary>
        /// <param name="elementId">
        /// The element Id.
        /// </param>
        /// <returns>
        /// The block ui execute js.
        /// </returns>
        public static string BlockUIExecuteJs([NotNull] string elementId)
        {
            return
                @"{1}(document).ready(function() {{ {1}.blockUI({{ message: {1}('#{0}') }}); }});".FormatWith(
                    elementId, Config.JQueryAlias);
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
        /// The load js.
        /// </returns>
        public static string DatePickerLoadJs(
            [NotNull] string fieldId, [NotNull] string dateFormat, [NotNull] string culture)
        {
            string cultureJs = string.Empty;

            dateFormat = dateFormat.ToLower();

            dateFormat = dateFormat.Replace("yyyy", "yy");

            if (!string.IsNullOrEmpty(culture))
            {
                cultureJs = @"{2}('#{0}').datepicker('option', {2}.datepicker.regional['{1}']);".FormatWith(
                    fieldId, culture, Config.JQueryAlias);
            }

            return
                @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadDatePicker);
                  function loadDatePicker() {{	{3}(document).ready(function() {{ {3}('#{0}').datepicker({{showButtonPanel: true,changeMonth:true,changeYear:true,maxDate:'+0d',dateFormat:'{1}',}}); {2} }});}} "
                    .FormatWith(fieldId, dateFormat, cultureJs, Config.JQueryAlias);
        }

        /// <summary>
        /// The spell checker load js.
        /// </summary>
        /// <param name="editorClientId">
        /// The Editor client Id.
        /// </param>
        /// <param name="spellCheckButtonId">
        /// The spell Check Button Id.
        /// </param>
        /// <param name="cultureCode">
        /// The culture Code.
        /// </param>
        /// <param name="spellCorrectTxt">
        /// The spell Correct Info Warning Text.
        /// </param>
        /// <returns>
        /// The load js.
        /// </returns>
        [NotNull]
        public static string SpellCheckerLoadJs(
            [NotNull] string editorClientId,
            [NotNull] string spellCheckButtonId,
            [CanBeNull] string cultureCode,
            [NotNull] string spellCorrectTxt)
        {
            return
                @"{0}(document).ready(function() {{ {0}('#{1}').spellchecker({{lang: ""{3}"", engine: ""google"", url: ""{5}YafAjax.asmx/SpellCheck""}}); }});
                {0}('#{2}').click(function(e){{
                    e.preventDefault();
                     var text = {0}('#{1}').val();
                     if (text == '') {{  
                         {0}('#{1}').spellchecker('remove');
                         alert('{4}');
                     }}
                     else
                    {{                           
                    {0}('#{1}').spellchecker('check', function(result){{
                    if (result) {{
                                   {0}('#{1}').spellchecker('remove');
                                   alert('{4}');
                                 }}
                  }});
                 }}
                }});
                "
                    .FormatWith(
                        Config.JQueryAlias,
                        editorClientId,
                        spellCheckButtonId,
                        cultureCode,
                        spellCorrectTxt,
                        YafForumInfo.ForumClientFileRoot);
        }

        /// <summary>
        /// Generates jQuery dropdown load js
        /// </summary>
        /// <param name="dropDownId">
        /// The drop Down client Id.
        /// </param>
        /// <returns>
        /// The load js.
        /// </returns>
        public static string DropDownLoadJs([NotNull] string dropDownId)
        {
            return @"Sys.Application.add_load(test);function test() {{ {0}('#{1}').msDropDown(); }} ".FormatWith(Config.JQueryAlias, dropDownId);
        }

        /// <summary>
        /// Gets JqueryUITabsLoadJs.
        /// </summary>
        /// <param name="tabId">
        /// The tab Id.
        /// </param>
        /// <param name="hiddenId">
        /// The hidden Id.
        /// </param>
        /// <param name="hightTransition">
        /// Height Transition
        /// </param>
        /// <returns>
        /// The jquery ui tabs load js.
        /// </returns>
        public static string JqueryUITabsLoadJs([NotNull] string tabId, [NotNull] string hiddenId, bool hightTransition)
        {
            return JqueryUITabsLoadJs(tabId, hiddenId, null, null, hightTransition, false);
        }

        /// <summary>
        /// Gets JqueryUITabsLoadJs.
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
        /// The postback Js.
        /// </param>
        /// <param name="hightTransition">
        /// Height Transition
        /// </param>
        /// <param name="addSelectedFunction">
        /// The add Selected Function.
        /// </param>
        /// <returns>
        /// The jquery ui tabs load js.
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

            return
                @"{3}(document).ready(function() {{
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
                    }});"
                    .FormatWith(tabId, hiddenId, heightTransitionJs, Config.JQueryAlias, selectFunctionJs);
        }

        /// <summary>
        /// Generated the load Script for the Spinner Widget
        /// </summary>
        /// <returns>
        /// Returns the Java Script that loads table Sorter
        /// </returns>
        public static string LoadSpinnerWidget()
        {
            return @"{0}(document).ready(function() {{
                        if (typeof (jQuery.fn.spinner) !== 'undefined') {{
                            {0}('.Numeric').spinner();
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
        public static string LoadTableSorter([NotNull]string selector, [CanBeNull]string options)
        {
            return @"{0}(document).ready(function() {{
                        {0}('{1}').tablesorter( {2} );
                    }});".FormatWith(Config.JQueryAlias, selector, options.IsSet() ? "{{ {0} }}".FormatWith(options) : string.Empty);
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
            return
                @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadGotoAnchor);
            function loadGotoAnchor() {{
               window.location.hash = ""{0}"";               
			      }}"
                    .FormatWith(anchor);
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
        /// The yaf modal dialog load js.
        /// </returns>
        public static string YafModalDialogLoadJs([NotNull] string openLink, [NotNull] string dialogId)
        {
            return
                @"{3}(document).ready(function() {{{3}('{0}').YafModalDialog({{Dialog : '{1}',ImagePath : '{2}'}}); }});"
                    .FormatWith(openLink, dialogId, YafForumInfo.GetURLToResource("images/"), Config.JQueryAlias);
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
                   {2}('#dvFavorite2').html({0});}}}}"
                    .FormatWith(untagButtonHTML, YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
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
                   {2}('#dvThankBox' + res.d.MessageID).html({0});}}}}"
                    .FormatWith(removeThankBoxHTML, YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
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
                   {2}('#dvFavorite2').html({0});}}}}"
                    .FormatWith(tagButtonHTML, YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
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
                   {2}('#dvThankBox' + res.d.MessageID).html({0});}}}}"
                    .FormatWith(addThankBoxHTML, YafForumInfo.ForumClientFileRoot, Config.JQueryAlias);
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
            return
                @"{0}(document).ready(function() {{
                    {0}('.AddReputation_{1}').remove();
                    {0}('.RemoveReputation_{1}').remove();
                    {0}('.ReputationUser_{1}').replaceWith('{2}');
					{0}('.ReputationBar').progressbar({{
			            create: function(event, ui) {{
			                    ChangeReputationBarColor({0}(this).attr('data-percent'),{0}(this).attr('data-text'), this);
			                    }}
		             }});}});"
                    .FormatWith(Config.JQueryAlias, userId, generateReputationBar);
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
            return
                @"function toggleEventLogItem(detailId) {{
                           var show = '{1}';var hide = '{2}';
	                       {0}('#Show'+ detailId).text({0}('#Show'+ detailId).text() == show ? hide : show);
                           {0}('#eventDetails' + detailId).slideToggle('slow'); return false;
                  }}"
                    .FormatWith(Config.JQueryAlias, showText, hideText);
        }

        #endregion

        /// <summary>
        /// Renders the Hover card load js.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="type">The type.</param>
        /// <param name="loadingHtml">The loading HTML.</param>
        /// <param name="errorHtml">The error HTML.</param>
        /// <returns>Returns the js String</returns>
        [NotNull]
        public static string HoverCardLoadJs(
            [NotNull] string clientId, [NotNull] string type, [NotNull] string loadingHtml, [NotNull] string errorHtml)
        {
            return
                "{0}('{1}').hovercard({{{2}width: 350,loadingHTML: '{3}',errorHTML: '{4}', delay: {5} }});".FormatWith(
                    Config.JQueryAlias,
                    clientId,
                    !string.IsNullOrEmpty(type) ? "show{0}Card: true,".FormatWith(type) : string.Empty,
                    loadingHtml,
                    errorHtml,
                    YafContext.Current.Get<YafBoardSettings>().HoverCardOpenDelay);
        }
    }
}