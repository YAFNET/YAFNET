jQuery(document).ready(function() {
	
	jQuery("img, input, a").tipTip();
	
  // TipTip
  if(!jQuery.browser.msie)
  {
	 jQuery("img.avatarimage").load(function() {
		 jQuery(this).wrap('<span class="' + jQuery(this).attr('class') + '" style="background:url(' + jQuery(this).attr('src') + ') no-repeat ;background-size:100%; width: auto; height: auto;" />');jQuery(this).css("opacity","0");
	 });
  }
  else
  {
	 if(jQuery.browser.version >= 9)
  {
	   jQuery("img.avatarimage").load(function() {
		 jQuery(this).wrap('<span class="' + jQuery(this).attr('class') + '" style="background:url(' + jQuery(this).attr('src') + ') no-repeat ;background-size:100%; width:auto; height: auto;" />');jQuery(this).css("opacity","0");
	 });
  }
  }
 
});