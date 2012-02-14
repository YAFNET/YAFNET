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
			
			$($(this)).click(function() {          
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
					  
					 var cnt = $("#" + MainDialogId + " .DialogContent").contents();
		             $("#" + MainDialogId).replaceWith(cnt);
					 
					 $(settings.Dialog + '#ModalDialog' + ' #' + DialogId + 'Close').remove();
					 $(settings.Dialog + '#ModalDialog_overlay').remove();
					
					 return false;
		 };
					 
		// allow jQuery chaining
		return this;
	};
	
	$.fn.YafModalDialog.Show = function(settings) {
		
		settings = $.extend( {Dialog: "#MessageBox", ImagePath: "images/"}, settings);
		
		if ($('#LoginBox').is(':visible'))
		{
			$.fn.YafModalDialog.Close({ Dialog: '#LoginBox' });
		}

        var top = getPageScroll()[1] + (getPageHeight() / 100);
		var left =  $(window).width() / 2 - 205;

		var cookieScroll = readCookie('ScrollPosition');
	    if (cookieScroll != null)
		{
	      eraseCookie('ScrollPosition');
		  top = 0;
		  top = parseInt(cookieScroll) + 100;
		}
		
		var DialogId = settings.Dialog;
		DialogId = DialogId.replace("#", "");
		
		var MainDialogId = DialogId + 'Box';
		
		$(settings.Dialog).wrapInner("<div id=\"" + MainDialogId +"\" class=\"ModalDialog\" style=\"top: "+  top + "px; display: block; left: " + left +"px; \"><div class=\"popup\"><div class=\"DialogContent\">");
				$('#' + MainDialogId + ' .popup').after("<a href=\"javascript:void(0);\" class=\"close\" id=\"" + DialogId + "Close\"><img src=\"" + settings.ImagePath + "closelabel.png\" title=\"close\" class=\"close_image\"></a>");
			    $(settings.Dialog).after("<div id=\"" + MainDialogId +  "_overlay\" class=\"ModalDialog_hide ModalDialog_overlayBG\" style=\"display: none; opacity: 0.2; \"></div>");
				
				$(settings.Dialog).fadeIn('normal');
				 
				// IE FIX
				$('#' + MainDialogId + '_overlay').css('filter', 'alpha(opacity=20)');
				
                $('#' + MainDialogId + '_overlay').fadeIn('normal');
				
				$(document).bind('keydown.' + DialogId, function(e) {
					if (e.keyCode == 27) 
					{
						$.fn.YafModalDialog.Close(settings);
					}
					return true;
				});
	    $('#' + DialogId + 'Close').click(function(){
						$.fn.YafModalDialog.Close(settings);
				});
};
	
function getPageScroll(){var xScroll,yScroll;if(self.pageYOffset){yScroll=self.pageYOffset;xScroll=self.pageXOffset;
                                             }else if(document.documentElement&&document.documentElement.scrollTop){yScroll=document.documentElement.scrollTop;xScroll=document.documentElement.scrollLeft;
                                             }else if(document.body){yScroll=document.body.scrollTop;xScroll=document.body.scrollLeft;
                                             }return new Array(xScroll,yScroll);
}function getPageHeight(){var windowHeight;if(self.innerHeight){windowHeight=self.innerHeight;
                                           }else if(document.documentElement&&document.documentElement.clientHeight){windowHeight=document.documentElement.clientHeight;
                                           }else if(document.body){windowHeight=document.body.clientHeight;
                                           }return windowHeight;
}

})(jQuery);

function createCookie(name,value){var expires="";document.cookie=name+"="+value+expires+"; path=/";
}function readCookie(name){var nameEQ=name+"=";var ca=document.cookie.split(';');for(var i=0;i<ca.length;i++){var c=ca[i];while(c.charAt(0)==' ')c=c.substring(1,c.length);if(c.indexOf(nameEQ)==0)return c.substring(nameEQ.length,c.length);
                                                                                 }return null;
}function eraseCookie(name){createCookie(name,"",-1);
}