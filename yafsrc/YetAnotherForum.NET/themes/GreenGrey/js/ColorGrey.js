
jQuery(document).ready(function() {
	
  // TipTip
  if(!$.browser.msie)
  {
    $("a, img, input").tipTip();
  }
  else
  {
	  // $ Corner Test
	  /*$('.section').corner();
      $('.yaflittlebutton').corner();
	  $('.yafcssimagebutton').corner();
      $('.yafcssbigbutton').corner();
	  $('.pbutton').corner();
	  $('.yafPageLink').corner();*/
  }
  $("img.avatarimage").load(function() {$(this).wrap('<span class="' + $(this).attr('class') + '" style="background:url(' + $(this).attr('src') + ') no-repeat ;background-size:100%; width: ' + $(this).width() + 'px; height: ' + $(this).height() + 'px;" />');$(this).css("opacity","0");});
  

});