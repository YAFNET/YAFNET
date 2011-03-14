/*
 * YafModalDialog by Ingo Herbote  for YAF.NET based on Facebox http://famspam.com/facebox/ by Chris Wanstrath [ chris@ozmm.org ]
 * version: 1.01 (12/04/2010)
 * @requires jQuery v1.4.4 or later
 *
 * Licensed under the MIT:
 *   http://www.opensource.org/licenses/mit-license.php
 */
  
;(function($) {
	// jQuery plugin definition
	$.fn.YafModalDialog = function(settings) {

		settings = $.extend( {Dialog: "#MessageBox", ImagePath: "images/"}, settings);
		
		var DialogId = settings.Dialog;
		DialogId = DialogId.replace("#", "");
		
		var MainDialogId = DialogId + 'Box';

		// traverse all nodes
		this.each(function() {
			
			$($(this)).click(function(e) {          
				$.fn.YafModalDialog.Show(settings);
            });
					 
		});
		// allow jQuery chaining
		return this;
	};
	
	// jQuery plugin definition
	$.fn.YafModalDialog.Close = function(settings) {

		 settings = $.extend( {Dialog: "#MessageBox", ImagePath: "images/"}, settings);
		 
		var DialogId = settings.Dialog;
		DialogId = DialogId.replace("#", "");
		
		var MainDialogId = DialogId + 'Box';

		 CloseDialog();
		 
		 function CloseDialog() {
					 $(settings.Dialog).hide();
                     $('#' + MainDialogId + '_overlay').fadeOut();
					 $(document).unbind('keydown.' + DialogId);
					 
					  $('#' + MainDialogId + '_overlay').remove();
					  
					 var cnt = $("#" + MainDialogId + " .DialogContent").contents()
					 $("#" + MainDialogId).replaceWith(cnt);
					 
					 $(settings.Dialog + '#ModalDialog' + ' #' + DialogId + 'Close').remove();
					 $(settings.Dialog + '#ModalDialog_overlay').remove();
					
					 return false
					 };
					 
		// allow jQuery chaining
		return this;
	};
	
	$.fn.YafModalDialog.Show = function(settings) {
		
		settings = $.extend( {Dialog: "#MessageBox", ImagePath: "images/"}, settings);
		
		var top = getPageScroll()[1] + (getPageHeight() / 10);
		var left =  $(window).width() / 2 - 205;
		
		var DialogId = settings.Dialog;
		DialogId = DialogId.replace("#", "");
		
		var MainDialogId = DialogId + 'Box';
		
		$(settings.Dialog).wrapInner("<div id=\"" + MainDialogId +"\" class=\"ModalDialog\" style=\"top: "+  top + "px; display: block; left: " + left +"px; \"><div class=\"popup\"><div class=\"DialogContent\">");
				$('#' + MainDialogId + ' .popup').after("<a href=\"javascript:void(0);\" class=\"close\" id=\"" + DialogId + "Close\"><img src=\"" + settings.ImagePath + "closelabel.png\" title=\"close\" class=\"close_image\"></a>");
			    $(settings.Dialog).after("<div id=\"" + MainDialogId +  "_overlay\" class=\"ModalDialog_hide ModalDialog_overlayBG\" style=\"display: none; opacity: 0.2; \"></div>");
				
				$(settings.Dialog).fadeIn('normal');
                $('#' + MainDialogId + '_overlay').fadeIn('normal');
				
				$(document).bind('keydown.' + DialogId, function(e) {
					if (e.keyCode == 27) 
					{
						$.fn.YafModalDialog.Close(settings);
					}
					return true
					})
					
			     $('#' + DialogId + 'Close').click(function(){
						$.fn.YafModalDialog.Close(settings);
				});
};
	
	// getPageScroll() by quirksmode.com
  function getPageScroll() {
    var xScroll, yScroll;
    if (self.pageYOffset) {
      yScroll = self.pageYOffset;
      xScroll = self.pageXOffset;
    } else if (document.documentElement && document.documentElement.scrollTop) {	 // Explorer 6 Strict
      yScroll = document.documentElement.scrollTop;
      xScroll = document.documentElement.scrollLeft;
    } else if (document.body) {// all other Explorers
      yScroll = document.body.scrollTop;
      xScroll = document.body.scrollLeft;
    }
    return new Array(xScroll,yScroll)
  }
  
  // Adapted from getPageSize() by quirksmode.com
  function getPageHeight() {
    var windowHeight
    if (self.innerHeight) {	// all except Explorer
      windowHeight = self.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
      windowHeight = document.documentElement.clientHeight;
    } else if (document.body) { // other Explorers
      windowHeight = document.body.clientHeight;
    }
    return windowHeight
  }
})(jQuery);