/*
 * YafModalDialog for YAF.NET based on Facebox http://famspam.com/facebox/ by Chris Wanstrath [ chris@ozmm.org ]
 * version: 1.0 (11/20/2010)
 * @requires jQuery v1.4.4 or later
*
 * Licensed under the MIT:
 *   http://www.opensource.org/licenses/mit-license.php
 */
  
(function($) {
	// jQuery plugin definition
	$.fn.YafModalDialog = function(settings) {
		settings = $.extend( {Dialog: "'YafModalDialog", ImagePath: "images/"}, settings);
		// traverse all nodes
		this.each(function() {
		
			$($(this)).click(function(e) {          
				e.preventDefault();
				
				$(settings.Dialog).wrapInner("<div id=\"ModalDialog\" style=\"top: 64.8px; display: block; left: 431px; \"><div class=\"popup\"><div class=\"DialogContent\">");
				$('#ModalDialog .popup').after("<a href=\"#\" class=\"close\"><img src=\"" + settings.ImagePath + "closelabel.png\" title=\"close\" class=\"close_image\"></a>");
			    $(settings.Dialog).after("<div id=\"ModalDialog_overlay\" class=\"ModalDialog_hide ModalDialog_overlayBG\" style=\"display: none; opacity: 0.2; \"></div>");
				
				$('#ModalDialog').css({
					top:	getPageScroll()[1] + (getPageHeight() / 10),
					left:	$(window).width() / 2 - 205
			     });
	  
	  
				$(settings.Dialog).fadeIn('normal');
                $("#ModalDialog_overlay").fadeIn('normal');
				
				$(document).bind('keydown.yafmodaldialog', function(e) {
					if (e.keyCode == 27) 
					{
						CloseDialog();
					}
					return true
					})
					
			     $('.close').click(function(){
				   CloseDialog();
				});
					
            });
				
				 function CloseDialog() {
					 $(settings.Dialog).hide();
                     $("#ModalDialog_overlay").fadeOut();
					 $(document).unbind('keydown.yafmodaldialog');
					 
					 var cnt = $("#ModalDialog .popup").contents()
					 $("#ModalDialog .popup").replaceWith(cnt);
					 
					 var cnt = $("#ModalDialog .DialogContent").contents()
					 $("#ModalDialog .DialogContent").replaceWith(cnt);
					 
					  var cnt = $("#ModalDialog").contents()
					 $("#ModalDialog").replaceWith(cnt);
					 
					 $('#ModalDialog .close').remove();
					 $('#ModalDialog_overlay').remove();
					
					 return false
					 };
					 
					 
			
		});
		// allow jQuery chaining
		return this;
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