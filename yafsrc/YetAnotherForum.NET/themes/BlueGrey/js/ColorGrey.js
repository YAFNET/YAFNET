$(document).ready(function() {
	
	  $("a, img, input").tipTip();
	
  // TipTip
  if(!$.browser.msie)
  {
	 $("img.avatarimage").wrap('<span class="' + $("img.avatarimage").attr('class') + '" style="background:url(' + $("img.avatarimage").attr('src') + ') no-repeat ;background-size:100%; width: ' + $("img.avatarimage").width() + 'px; height: ' + $("img.avatarimage").height() + 'px;" />');$("img.avatarimage").css("opacity","0");
  }
  else
  {
	 if($.browser.version >= 9)
  {
	   $("img.avatarimage").wrap('<span class="' + $("img.avatarimage").attr('class') + '" style="background:url(' + $("img.avatarimage").attr('src') + ') no-repeat ;background-size:100%; width: ' + $("img.avatarimage").width() + 'px; height: ' + $("img.avatarimage").height() + 'px;" />');$("img.avatarimage").css("opacity","0");
  }
  }
 
});