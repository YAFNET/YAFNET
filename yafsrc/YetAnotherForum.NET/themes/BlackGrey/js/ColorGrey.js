
jQuery(document).ready(function() {
	
  // TipTip
  if(!jQuery.browser.msie)
  {
    jQuery("a, img, input").tipTip();
  }
  else
  {
	  // jQuery Corner Test
	  /*jQuery('.section').corner();
      jQuery('.yaflittlebutton').corner();
	  jQuery('.yafcssimagebutton').corner();
      jQuery('.yafcssbigbutton').corner();
	  jQuery('.pbutton').corner();
	  jQuery('.yafPageLink').corner();*/
  }
  jQuery("img.avatarimage").wrap('<span class="' + jQuery("img.avatarimage").attr('class') + '" style="background:url(' + jQuery("img.avatarimage").attr('src') + ') no-repeat ;background-size:100%; width: ' + jQuery("img.avatarimage").width() + 'px; height: ' + jQuery("img.avatarimage").height() + 'px;" />');jQuery("img.avatarimage").css("opacity","0");
});