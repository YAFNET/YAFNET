/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

  using YAF.Core;
  using YAF.Types;
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

      /*get
          {
              return
                  "function changeTitleSuccess(res){{" +
                  "spnTitleVar = document.getElementById('spnTitle' + res.value.Id);" +
                  "txtTitleVar = document.getElementById('txtTitle' + res.value.Id);" +
                  "spnTitleVar.firstChild.nodeValue = res.value.NewTitle;" +
                  "txtTitleVar.disabled = false;" +
                  "spnTitleVar.style.display = 'inline';" +
                  "txtTitleVar.style.display='none';}}";
          }*/
    }

    /// <summary>
    ///   Gets Pagination Load Js.
    /// </summary>
    [NotNull]
    public static string PaginationLoadJs
    {
        get
        {
            return @"function pageselectCallback(page_index, jq){
                var new_content = jQuery('#SmiliesPagerHidden div.result:eq('+page_index+')').clone();
                jQuery('#SmiliesPagerResult').empty().append(new_content);
                return false;
            }
           
            jQuery(document).ready(function(){      
                var num_entries = jQuery('#SmiliesPagerHidden div.result').length;
                jQuery('#SmiliesPager').pagination(num_entries, {
                    callback: pageselectCallback,
                    items_per_page:1,
					num_display_entries: 3,
					num_edge_entries: 1,
                    prev_class: 'smiliesPagerPrev',
					next_class: 'smiliesPagerNext',
					prev_text: '&laquo;',
					next_text: '&raquo;'
                });
            });";
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
        return @"jQuery(document).ready(function() { 
					jQuery('.ceebox').ceebox({titles:true});
			});";
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
                     var albId = albumId;var newTitleTxt = jQuery('#' + txtTitleId).val();
                     jQuery.PageMethod('{0}YafAjax.asmx', 'ChangeAlbumTitle', changeTitleSuccess, CallFailed, 'albumID', albId, 'newTitle', newTitleTxt);}}".FormatWith(YafForumInfo.ForumClientFileRoot);

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
              var imgId = imageID;var newImgTitleTxt = jQuery('#' + txtTitleId).val();
              jQuery.PageMethod('{0}YafAjax.asmx', 'ChangeImageCaption', changeTitleSuccess, CallFailed, 'imageID', imgId, 'newCaption', newImgTitleTxt);}}".FormatWith(YafForumInfo.ForumClientFileRoot);

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
        return @"jQuery(document).ready(function() {
					SyntaxHighlighter.all()});";
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
            jQuery.timeago.settings.refreshMillis = {1};			      	
            {0}
              jQuery('abbr.timeago').timeago();	
			      }}".FormatWith(YafContext.Current.Get<ILocalization>().GetText("TIMEAGO_JS"), YafContext.Current.BoardSettings.RelativeTimeRefreshTime);
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
{
    if(divId != null)
    {
        var o = $get(divId);

        if(o != null)
        {
            o.style.display = (o.style.display == ""none"" ? ""block"" : ""none"");
        }
    }
}
";
      }
    }

    /// <summary>
    ///   If asynchronous callback encounters any problem, this javascript function will be called.
    /// </summary>
    /// <returns></returns>
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
    /// Javascript events for Album pages.
    /// </summary>
    /// <param name="AlbumEmptyTitle">
    /// The Album Empty Title.
    /// </param>
    /// <param name="ImageEmptyCaption">
    /// The Image Empty Caption.
    /// </param>
    /// <returns>
    /// The album events js.
    /// </returns>
    public static string AlbumEventsJs([NotNull] string AlbumEmptyTitle, [NotNull] string ImageEmptyCaption)
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
           AlbumEmptyTitle, ImageEmptyCaption);
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
        @"jQuery(document).ready(function() {{ 
            jQuery.blockUI({{ message: jQuery('#{0}') }}); 
        }});"
          .FormatWith(elementId);
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
    /// <param name="altDateFormat">
    /// The Alt Date Format
    /// </param>
    /// <param name="culture">
    /// Current Culture
    /// </param>
    /// <returns>
    /// The load js.
    /// </returns>
    public static string DatePickerLoadJs([NotNull] string fieldId, [NotNull] string dateFormat, [NotNull] string altDateFormat, [NotNull] string culture)
    {
      string cultureJs = string.Empty;

      dateFormat = dateFormat.ToLower();

      dateFormat = dateFormat.Replace("yyyy", "yy");

      if (!string.IsNullOrEmpty(culture))
      {
        cultureJs = @"jQuery('#{0}').datepicker('option', jQuery.datepicker.regional['{1}']);".FormatWith(
          fieldId, culture);
      }

      return
        @"jQuery(document).ready(function() {{jQuery('#{0}').datepicker({{changeMonth:true,changeYear:true,maxDate:'+0d',dateFormat:'{1}',altFormat:'{2}'}}); {3} }}); "
          .FormatWith(fieldId, dateFormat, altDateFormat, cultureJs);
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
      string heightTransitionJs = string.Empty;

      if (hightTransition)
      {
        heightTransitionJs = ", fx:{height:'toggle'}";
      }

      return
        @"jQuery(document).ready(function() {{
					jQuery('#{0}').tabs(
                    {{
            show: function() {{
                var sel = jQuery('#{0}').tabs('option', 'selected');
                jQuery('#{1}').val(sel);
            }},
            selected: jQuery('#{1}').val()
            {2}
        }});
                    }});"
          .FormatWith(tabId, hiddenId, heightTransitionJs);
    }

    /// <summary>
    /// Load Go to Anchor
    /// </summary>
    /// <param name="anchor">
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
        @"jQuery(document).ready(function() {{jQuery('{0}').YafModalDialog({{Dialog : '{1}',ImagePath : '{2}'}}); }});".
          FormatWith(openLink, dialogId, YafForumInfo.GetURLToResource("images/"));
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
        @"function addFavoriteTopic(topicID){{ var topId = topicID; jQuery.PageMethod('{1}YafAjax.asmx', 'AddFavoriteTopic', addFavoriteTopicSuccess, CallFailed, 'topicId', topId);}}
          function addFavoriteTopicSuccess(res){{if (res.d != null) {{
                   jQuery('#dvFavorite1').html({0});
                   jQuery('#dvFavorite2').html({0});}}}}"
          .FormatWith(untagButtonHTML, YafForumInfo.ForumClientFileRoot);

      /*("function addFavoriteTopic(topicID){{YAF.Classes.Core.IFavoriteTopic.AddFavoriteTopic(topicID, addFavoriteTopicSuccess, CallFailed);}};" +
         "function addFavoriteTopicSuccess(res){{" +
         "jQuery('#dvFavorite1').html({0});" +
         "jQuery('#dvFavorite2').html({0});}}").FormatWith(UntagButtonHTML);*/
    }

    /// <summary>
    /// script for the addThanks button
    /// </summary>
    /// <param name="removeThankBoxHTML">
    /// HTML code for the "Remove Thank Note" button
    /// </param>
    /// <returns>
    /// The add thanks js.
    /// </returns>
    public static string AddThanksJs([NotNull] string removeThankBoxHTML)
    {
      return
        @"function addThanks(messageID){{ var messId = messageID;jQuery.PageMethod('{1}YafAjax.asmx', 'AddThanks', addThanksSuccess, CallFailed, 'msgID', messId);}}
          function addThanksSuccess(res){{if (res.d != null) {{
                   jQuery('#dvThanks' + res.d.MessageID).html(res.d.Thanks);
                   jQuery('#dvThanksInfo' + res.d.MessageID).html(res.d.ThanksInfo);
                   jQuery('#dvThankBox' + res.d.MessageID).html({0});}}}}"
          .FormatWith(removeThankBoxHTML, YafForumInfo.ForumClientFileRoot);

      /*   @"function addThanks(messageID){{YAF.Controls.ThankYou.AddThanks(messageID, addThanksSuccess, CallFailed);}}
          function addThanksSuccess(res){{if (res.value != null) {{
                   jQuery('#dvThanks' + res.value.MessageID).html(res.value.Thanks);
                   jQuery('#dvThanksInfo' + res.value.MessageID).html(res.value.ThanksInfo);
                   jQuery('#dvThankBox' + res.value.MessageID).html({0});}}}}".FormatWith(RemoveThankBoxHTML);*/
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
        @"function removeFavoriteTopic(topicID){{ var topId = topicID;jQuery.PageMethod('{1}YafAjax.asmx', 'RemoveFavoriteTopic', removeFavoriteTopicSuccess, CallFailed, 'topicId', topId);}}
          function removeFavoriteTopicSuccess(res){{if (res.d != null) {{
                   jQuery('#dvFavorite1').html({0});
                   jQuery('#dvFavorite2').html({0});}}}}"
          .FormatWith(tagButtonHTML, YafForumInfo.ForumClientFileRoot);

      /*("function removeFavoriteTopic(topicID){{YAF.Classes.Core.IFavoriteTopic.RemoveFavoriteTopic(topicID, removeFavoriteTopicSuccess, CallFailed);}};" +
           "function removeFavoriteTopicSuccess(res){{" +
           "jQuery('#dvFavorite1').html({0});" +
           "jQuery('#dvFavorite2').html({0});}}").FormatWith(TagButtonHTML);*/
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
        @"function removeThanks(messageID){{ var messId = messageID;jQuery.PageMethod('{1}YafAjax.asmx', 'RemoveThanks', removeThanksSuccess, CallFailed, 'msgID', messId);}}
          function removeThanksSuccess(res){{if (res.d != null) {{
                   jQuery('#dvThanks' + res.d.MessageID).html(res.d.Thanks);
                   jQuery('#dvThanksInfo' + res.d.MessageID).html(res.d.ThanksInfo);
                   jQuery('#dvThankBox' + res.d.MessageID).html({0});}}}}"
          .FormatWith(addThankBoxHTML, YafForumInfo.ForumClientFileRoot);

      /*@"function removeThanks(messageID){{YAF.Controls.ThankYou.RemoveThanks(messageID, removeThanksSuccess, CallFailed);}}
         function removeThanksSuccess(res){{if (res.value != null) {{
         jQuery('#dvThanks' + res.value.MessageID).html(res.value.Thanks);
         jQuery('#dvThanksInfo' + res.value.MessageID).html(res.value.ThanksInfo);
         jQuery('#dvThankBox' + res.value.MessageID).html(0});}}}}".FormatWith(AddThankBoxHTML);*/
    }

    #endregion
  }
}