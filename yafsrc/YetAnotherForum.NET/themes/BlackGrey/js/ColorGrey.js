
function pageLoad(){

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
  jQuery("img.avatarimage").load(function() {jQuery(this).wrap('<span class="' + jQuery(this).attr('class') + '" style="background:url(' + jQuery(this).attr('src') + ') no-repeat ;background-size:100%; width: ' + jQuery(this).width() + 'px; height: ' + jQuery(this).height() + 'px;" />');jQuery(this).css("opacity","0");});
  
}