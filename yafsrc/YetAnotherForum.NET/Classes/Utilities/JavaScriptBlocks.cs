/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using Classes.Core;

  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for JavaScriptBlocks
  /// </summary>
  public static class JavaScriptBlocks
  {
    public static string LoadGotoAnchor(string anchor)
    {
      return
        @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadGotoAnchor);
            function loadGotoAnchor() {{
               window.location = window.location + ""#{0}""
			      }}".FormatWith(anchor);
    }

    /// <summary>
    /// Gets ToggleMessageJs.
    /// </summary>
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
    /// Gets DisablePageManagerScrollJs.
    /// </summary>
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
    /// Gets TimeagoLoadJs.
    /// </summary>
    public static string TimeagoLoadJs
    {
      get
      {
        return
          @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadTimeAgo);
            function loadTimeAgo() {				      	
            " + YafContext.Current.Localization.GetText("TIMEAGO_JS") + @"
              jQuery('abbr.timeago').timeago();	
			      }";
      }
    }

    /// <summary>
    /// Gets LightBoxLoadJs.
    /// </summary>
    public static string LightBoxLoadJs
    {
      get
      {
        return
          @"jQuery(document).ready(function() { 
					jQuery.Lightbox.construct({
						show_linkback:	false,
						show_helper_text: false,
				text: {
					image:		'" +
          YafContext.Current.Localization.GetText("IMAGE_TEXT") + @"',
					close:    '" + YafContext.Current.Localization.GetText("CLOSE_TEXT") +
          @"',
					download:    '" + YafContext.Current.Localization.GetText("IMAGE_DOWNLOAD") + @"'
					}
				});
			});";
      }
    }

    /// <summary>
    /// Gets SyntaxHighlightLoadJs.
    /// </summary>
    public static string SyntaxHighlightLoadJs
    {
        get
        {
            return
                 @"jQuery(document).ready(function() {
					SyntaxHighlighter.all()});";
        }
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
    public static string YafModalDialogLoadJs(string openLink, string dialogId)
    {
        return
                @"jQuery(document).ready(function() {{jQuery('{0}').YafModalDialog({{Dialog : '{1}',ImagePath : '{2}'}}); }});".FormatWith(openLink, dialogId, YafForumInfo.GetURLToResource("images/"));
    }

      /// <summary>
      /// Generates jQuery UI DatePicker Script
      /// </summary>
      /// <param name="fieldId">
      /// The Id of the Control to Bind the DatePicker
      /// </param>
      /// <param name="dateFormat">
      /// Localized Date Format</param>
      /// <param name="altDateFormat">
      /// The Alt Date Format
      /// </param>
      /// <param name="culture">
      /// Current Culture
      /// </param>
      /// <returns>
      /// The load js.
      /// </returns>
      public static string DatePickerLoadJs(string fieldId, string dateFormat, string altDateFormat, string culture)
      {
        string cultureJs = string.Empty;

        dateFormat = dateFormat.ToLower();

        dateFormat = dateFormat.Replace("yyyy", "yy");

        if (!string.IsNullOrEmpty(culture))
        {
            cultureJs = @"jQuery('#{0}').datepicker(jQuery.datepicker.regional['{1}']);".FormatWith(fieldId, culture);
        }

        return
            @"jQuery(document).ready(function() {{jQuery('#{0}').datepicker({{changeMonth:true,changeYear:true,maxDate:'+0d',dateFormat:'{1}',altFormat:'{2}'}}); {3} }});"
                .FormatWith(fieldId, dateFormat, altDateFormat, cultureJs);
    }

    /// <summary>
    /// If asynchronous callback encounters any problem, this javascript function will be called.
    /// </summary>
    /// <returns></returns>
    public static string asynchCallFailedJs
    {
      get
      {
        return "function CallFailed(res){{alert('Error Occurred');}}";
      }
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
    public static string BlockUIExecuteJs(string elementId)
    {
      return @"jQuery(document).ready(function() {{ 
            jQuery.blockUI({{ message: jQuery('#{0}') }}); 
        }});".FormatWith(elementId);
    }

    /// <summary>
    /// script for the addThanks button
    /// </summary>
    /// <param name="RemoveThankBoxHTML">
    /// HTML code for the "Remove Thank Note" button
    /// </param>
    /// <returns>
    /// The add thanks js.
    /// </returns>
    public static string addThanksJs(string RemoveThankBoxHTML)
    {
      return
        ("function addThanks(messageID){{YAF.Controls.ThankYou.AddThanks(messageID, addThanksSuccess, CallFailed);}}" +
         "function addThanksSuccess(res){{if (res.value != null) {{" +
         "var dvThanks=document.getElementById('dvThanks' + res.value.MessageID); dvThanks.innerHTML=res.value.Thanks;" +
         "dvThanksInfo=document.getElementById('dvThanksInfo' + res.value.MessageID); dvThanksInfo.innerHTML=res.value.ThanksInfo;" +
         "dvThankbox=document.getElementById('dvThankBox' + res.value.MessageID); dvThankbox.innerHTML={0};}}}}").FormatWith(RemoveThankBoxHTML);
    }

    /// <summary>
    /// script for the removeThanks button
    /// </summary>
    /// <param name="AddThankBoxHTML">
    /// The Add Thank Box HTML.
    /// </param>
    /// <returns>
    /// The remove thanks js.
    /// </returns>
    public static string removeThanksJs(string AddThankBoxHTML)
    {
      return
        ("function removeThanks(messageID){{YAF.Controls.ThankYou.RemoveThanks(messageID, removeThanksSuccess, CallFailed);}}" +
         "function removeThanksSuccess(res){{if (res.value != null) {{" +
         "var dvThanks=document.getElementById('dvThanks' + res.value.MessageID); dvThanks.innerHTML=res.value.Thanks;" +
         "dvThanksInfo=document.getElementById('dvThanksInfo' + res.value.MessageID); dvThanksInfo.innerHTML=res.value.ThanksInfo;" +
         "dvThankbox=document.getElementById('dvThankBox' + res.value.MessageID); dvThankbox.innerHTML={0};}}}}").FormatWith(AddThankBoxHTML);
    }

      /// <summary>
      /// script for the add Favorite Topic button
      /// </summary>
      /// <param name="UntagButtonHTML">HTML code for the "Untag As Favorite" button
      /// </param>
      /// <returns>
      /// The add Favorite Topic js.
      /// </returns>
      public static string addFavoriteTopicJs(string UntagButtonHTML)
    {
        return
          ("function addFavoriteTopic(topicID){{YAF.Classes.Core.YafFavoriteTopic.AddFavoriteTopic(topicID, addFavoriteTopicSuccess, CallFailed);}};" +
           "function addFavoriteTopicSuccess(res){{" +
           "var dvFavorite1=document.getElementById('dvFavorite1'); dvFavorite1.innerHTML={0};" +
           "var dvFavorite2=document.getElementById('dvFavorite2'); dvFavorite2.innerHTML={0};}}").FormatWith(UntagButtonHTML);
    }

    /// <summary>
    /// script for the remove Favorite Topic button
    /// </summary>
    /// <param name="TagButtonHTML">
    /// HTML code for the "Tag As a Favorite" button
    /// </param>
    /// <returns>
    /// The remove Favorite Topic js.
    /// </returns>
    public static string removeFavoriteTopicJs(string TagButtonHTML)
    {
        return
          ("function removeFavoriteTopic(topicID){{YAF.Classes.Core.YafFavoriteTopic.RemoveFavoriteTopic(topicID, removeFavoriteTopicSuccess, CallFailed);}};" +
           "function removeFavoriteTopicSuccess(res){{" +
           "var dvFavorite1=document.getElementById('dvFavorite1'); dvFavorite1.innerHTML={0};" +
           "var dvFavorite2=document.getElementById('dvFavorite2'); dvFavorite2.innerHTML={0};}}").FormatWith(TagButtonHTML);
    }

      /// <summary>
      /// Javascript events for Album pages.
      /// </summary>
    public static string AlbumEventsJs(string AlbumEmptyTitle, string ImageEmptyCaption)
      {
              return ("function showTexBox(spnTitleId){{" + "spnTitleVar = document.getElementById('spnTitle' + spnTitleId.substring(8));" +
                      "txtTitleVar = document.getElementById('txtTitle'+spnTitleId.substring(8));" +
                      "if (spnTitleVar.firstChild != null) txtTitleVar.setAttribute('value',spnTitleVar.firstChild.nodeValue);" +
                      "if(spnTitleVar.firstChild.nodeValue == '{0}' || spnTitleVar.firstChild.nodeValue == '{1}'){{txtTitleVar.value='';spnTitleVar.firstChild.nodeValue='';}}" +
                      "txtTitleVar.style.display = 'inline'; spnTitleVar.style.display = 'none'; txtTitleVar.focus();}}" + 
                     
                      "function resetBox(txtTitleId, isAlbum) {{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);" +
                      "txtTitleVar.style.display = 'none';txtTitleVar.disabled = false;spnTitleVar.style.display = 'inline';" +
                      "if (spnTitleVar.firstChild != null)txtTitleVar.value = spnTitleVar.firstChild.nodeValue;if (spnTitleVar.firstChild.nodeValue==''){{txtTitleVar.value='';if (isAlbum) spnTitleVar.firstChild.nodeValue='{0}';else spnTitleVar.firstChild.nodeValue='{1}';}}}}" +

                      "function checkKey(event, handler, id, isAlbum){{" + 
                      "if ((event.keyCode == 13) || (event.which == 13)){{" +
                      "if (event.preventDefault) event.preventDefault(); event.cancel=true; event.returnValue=false; " +
                      "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{" +
                      "handler.disabled = true; if (isAlbum == true)changeAlbumTitle(id, handler.id); else changeImageCaption(id,handler.id);}}" +
                      "else resetBox(handler.id, isAlbum);}}" +
                      "else if ((event.keyCode == 27) || (event.which == 27))resetBox(handler.id, isAlbum);}}" +
                     
                      "function blurTextBox(txtTitleId, id , isAlbum){{spnTitleVar = document.getElementById('spnTitle'+txtTitleId.substring(8));txtTitleVar = document.getElementById(txtTitleId);" +
                      "if (spnTitleVar.firstChild != null){{" + 
                      "if(spnTitleVar.firstChild.nodeValue != txtTitleVar.value){{" +
                      "txtTitleVar.disabled = true; if (isAlbum == true)changeAlbumTitle(id, txtTitleId); else changeImageCaption(id,txtTitleId);}}" +
                      "else resetBox(txtTitleId, isAlbum);}}" + 
                      "else resetBox(txtTitleId, isAlbum);}}").FormatWith(AlbumEmptyTitle, ImageEmptyCaption);
      }

      /// <summary>
      /// script for changing the album title.
      /// </summary>
      /// <returns>
      /// the change album title js.
      /// </returns>
      public static string ChangeAlbumTitleJs
      {
          get
          {
          return
              "function changeAlbumTitle(albumId, txtTitleId){{" +
              "YAF.Classes.Core.YafAlbum.ChangeAlbumTitle(albumId, document.getElementById(txtTitleId).value, changeTitleSuccess, CallFailed);}}";
          }
      }

      /// <summary>
      /// script for changing the image caption.
      /// </summary>
      /// <returns></returns>
      public static string ChangeImageCaptionJs
      {
          get
          {
              return
              "function changeImageCaption(imageID, txtTitleId){{" +
              "YAF.Classes.Core.YafAlbum.ChangeImageCaption(imageID, document.getElementById(txtTitleId).value, changeTitleSuccess, CallFailed);}}";
          }
      }

      /// <summary>
      /// script for album/image title/image callback.
      /// </summary>
      /// <returns>
      /// the callback success js.
      /// </returns>
      public static string AlbumCallbackSuccessJS
      {
          get
          {
              return
                  "function changeTitleSuccess(res){{" +
                  "spnTitleVar = document.getElementById('spnTitle' + res.value.Id);" +
                  "txtTitleVar = document.getElementById('txtTitle' + res.value.Id);" +
                  "spnTitleVar.firstChild.nodeValue = res.value.NewTitle;" +
                  "txtTitleVar.disabled = false;" +
                  "spnTitleVar.style.display = 'inline';" +
                  "txtTitleVar.style.display='none';}}";
          }
      }
  }
}