
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
}