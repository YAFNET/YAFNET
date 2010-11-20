$(document).ready(function() {
	
	$("img, input").tipTip();
	
  // TipTip
  if(!$.browser.msie)
  {
	 $("img.avatarimage").load(function() {
		 $(this).wrap('<span class="' + $(this).attr('class') + '" style="background:url(' + $(this).attr('src') + ') no-repeat ;background-size:100%; width: ' + $(this).width() + 'px; height: ' + $(this).height() + 'px;" />');$(this).css("opacity","0");
	 });
  }
  else
  {
	 if($.browser.version >= 9)
  {
	   $("img.avatarimage").load(function() {
		 $(this).wrap('<span class="' + $(this).attr('class') + '" style="background:url(' + $(this).attr('src') + ') no-repeat ;background-size:100%; width: ' + $(this).width() + 'px; height: ' + $(this).height() + 'px;" />');$(this).css("opacity","0");
	 });
  }
  }
 
});