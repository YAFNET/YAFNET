/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for JavaScriptBlocks
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
                  @"window.fbAsyncInit = function() {{
                       FB.init({{
                             appId: '{0}',
                             status: true, // check login status
                             cookie: true, // enable cookies to allow the server to access the session
                             xfbml: true  // parse XFBML
                            }}); 
                     }};
                      (function() {{
                              var e = document.createElement('script');
                              e.src = document.location.protocol + '//connect.facebook.net/en_US/all.js';
                              e.async = true;
                              document.getElementById('fb-root').appendChild(e);
                       }}());".FormatWith(Config.FacebookAPIKey);
            }
        }

        /// <summary>
        ///   Gets the script for login callback.
        /// </summary>
        /// <returns>
        ///   the callback success js.
        /// </returns>
        [NotNull]
        public static string LoginCallSuccessJS
        {
            get
            {
                return
                    @"function loginCallSuccess(res){{
                    if (res.d != null && res.d == ""OK"") {{
                    window.location = ""{0}"";
                    }}
                    else {{
                      // Show MessageBox
                      {1}('span[id$=_YafPopupErrorMessageInner]').html(res.d);
                      {1}().YafModalDialog.Show({{Dialog : '#' + {1}('div[id$=_YafForumPageErrorPopup1]').attr('id'),ImagePath : '/yaf/resources/images/'}});
                    }} }}".FormatWith(YafBuildLink.GetLink(ForumPages.forum), Config.JQueryAlias);
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
        ///   Gets CeeBox Load Js.
        /// </summary>
        [NotNull]
        public static string CeeBoxLoadJs
        {
            get
            {
                return @"{0}(document).ready(function() {{ 
					{0}('.ceebox').ceebox({{titles:true}});
			}});".FormatWith(Config.JQueryAlias);
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

                // YAF.Classes.Core.YafAlbum.ChangeAlbumTitle(albumId, document.getElementById(txtTitleId).value, changeTitleSuccess, CallFailed);}}";
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

                // YAF.Classes.Core.YafAlbum.ChangeImageCaption(imageID, document.getElementById(txtTitleId).value, changeTitleSuccess, CallFailed);}}";
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
                return @"{0}(document).ready(function() {{
					SyntaxHighlighter.all()}});".FormatWith(Config.JQueryAlias);
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
                      {{ {0}('#' + divId).toggle(); }}".FormatWith(Config.JQueryAlias);
            }
        }

        /// <summary>
        ///  Gets the If asynchronous callback encounters any problem, this javascript function will be called.
        /// </summary>
        [NotNull]
        public static string asynchCallFailedJs
        {
            get
            {
                return "function CallFailed(res){{alert('Error Occurred');}}";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the script for logging in thru facebook.
        /// </summary>
        /// <param name="remberMeId">
        /// The rember Me Checkbox Client Id.
        /// </param>
        /// <returns>
        /// The facebook login js.
        /// </returns>
        [NotNull]
        public static string FacebookLoginJs(string remberMeId)
        {
            return
                @"function LoginUser() {{

                    var Remember = {1}('#{2}').is(':checked');

                    FB.api('/me', function (response) {{
                    
                    {1}.PageMethod('{0}YafAjax.asmx', 'LoginFacebookUser', loginCallSuccess, LoginCallFailed,
                              'id', response.id,
                              'name', response.name,
                              'first_name', response.first_name,
                              'last_name', response.last_name,
                              'link', response.link,
                              'username', response.username,
                              'birthday', response.birthday,
                              'hometown', response.hometown === undefined ? '' : response.hometown.name,
                              'gender', response.gender,
                              'email', response.email,
                              'timezone', response.timezone,
                              'locale', response.locale,
                              'remember', Remember);
                     }});}}".FormatWith(YafForumInfo.ForumClientFileRoot, Config.JQueryAlias, remberMeId);
        }

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
        public static string FacebookPostJs(string message, string description, string link, string picture, string caption)
        {
            return
                @"function postToFacebook() {{

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
              ("function showTexBox(spnTitleId){{" +
               "spnTitleVar = document.getElementById('spnTitle' + spnTitleId.substring(8));" +
               "txtTitleVar = document.getElementById('txtTitle'+spnTitleId.substring(8));" +
               "if (spnTitleVar.firstChild != null) txtTitleVar.setAttribute('value',spnTitleVar.firstChild.nodeValue);" +
               "if(spnTitleVar.firstChild.nodeValue == '{0}' || spnTitleVar.firstChild.nodeValue == '{1}'){{txtTitleVar.value='';spnTitleVar.firstChild.nodeValue='';}}" +
               "txtTitleVar.style.display = 'inline'; spnTitleVar.style.display = 'none'; txtTitleVar.focus();}}" +
               "function resetBox(txtTitleId, isAlbum) {{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);" +
               "txtTitleVar.style.display = 'none';txtTitleVar.disabled = false;spnTitleVar.style.display = 'inline';" +
               "if (spnTitleVar.firstChild != null)txtTitleVar.value = spnTitleVar.firstChild.nodeValue;if (spnTitleVar.firstChild.nodeValue==''){{txtTitleVar.value='';if (isAlbum) spnTitleVar.firstChild.nodeValue='{0}';else spnTitleVar.firstChild.nodeValue='{1}';}}}}" +
               "function checkKey(event, handler, id, isAlbum){{" + "if ((event.keyCode == 13) || (event.which == 13)){{" +
               "if (event.preventDefault) event.preventDefault(); event.cancel=true; event.returnValue=false; " +
               "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{" +
               "handler.disabled = true; if (isAlbum == true)changeAlbumTitle(id, handler.id); else changeImageCaption(id,handler.id);}}" +
               "else resetBox(handler.id, isAlbum);}}" +
               "else if ((event.keyCode == 27) || (event.which == 27))resetBox(handler.id, isAlbum);}}" +
               "function blurTextBox(txtTitleId, id , isAlbum){{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);" +
               "if (spnTitleVar.firstChild != null){{" + "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{" +
               "txtTitleVar.disabled = true; if (isAlbum == true)changeAlbumTitle(id, txtTitleId); else changeImageCaption(id,txtTitleId);}}" +
               "else resetBox(txtTitleId, isAlbum);}}" + "else resetBox(txtTitleId, isAlbum);}}").FormatWith(
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
              @"{1}(document).ready(function() {{ {1}.blockUI({{ message: {1}('#{0}') }}); }});"
                .FormatWith(elementId, Config.JQueryAlias);
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
        public static string DatePickerLoadJs([NotNull] string fieldId, [NotNull] string dateFormat, [NotNull] string culture)
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
                  function loadDatePicker() {{	{3}(document).ready(function() {{ {3}('#{0}').datepicker({{changeMonth:true,changeYear:true,maxDate:'+0d',dateFormat:'{1}',}}); {2} }});}} "
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
        public static string SpellCheckerLoadJs([NotNull] string editorClientId, [NotNull]string spellCheckButtonId, [CanBeNull]string cultureCode, [NotNull]string spellCorrectTxt)
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
                ".FormatWith(Config.JQueryAlias, editorClientId, spellCheckButtonId, cultureCode, spellCorrectTxt, YafForumInfo.ForumClientFileRoot);
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
            return
              @"function pageLoad() {{ {0}('#{1}').msDropDown(); }} "
                .FormatWith(Config.JQueryAlias, dropDownId);
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
        public static string JqueryUITabsLoadJs([NotNull] string tabId, [NotNull] string hiddenId, [CanBeNull] string hiddenTabId, [CanBeNull] string postbackJs, bool hightTransition, bool addSelectedFunction)
        {
            string heightTransitionJs = hightTransition ? ", fx:{height:'toggle'}" : string.Empty;

            string selectFunctionJs = addSelectedFunction
                                          ? ", select: function(event, ui) {{ {0}('#{1}').val(ui.index);{0}('#{2}').val(ui.panel.id);{3} }}".FormatWith(
                                              Config.JQueryAlias, hiddenId, hiddenTabId, postbackJs)
                                          : string.Empty;

            return
              @"{3}(document).ready(function() {{
					{3}('#{0}').tabs(
                    {{
            show: function() {{
                var sel = {3}('#{0}').tabs('option', 'selected');
                {3}('#{1}').val(sel);
            }},
            selected: {3}('#{1}').val()
            {2}
            {4}
        }});
                    }});"
                .FormatWith(tabId, hiddenId, heightTransitionJs, Config.JQueryAlias, selectFunctionJs);
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
        public static string ReputationProgressChangeJs([NotNull]string generateReputationBar, [NotNull]string userId)
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

        #endregion
    }
}