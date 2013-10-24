jQuery(document).ready(function () {
    jQuery('.menuMyList li a').not('.menuMyList li .Unread a').text('');
    jQuery(".loggedInUser").insertBefore('.menuMyList');
	
	if(typeof tipTip == "undefined") {
        jQuery.getScript('themes/GreenGrey/js/jquery.tipTip.minified.js', function() {
            jQuery(".yafnet img, .yafnet input,.yafnet  a").not('.Facebook-HoverCard, .Twitter-HoverCard').tipTip();
        });
    }
	
    jQuery(function () {
        jQuery('img.avatarimage').onImagesLoaded(function (_this) {
            jQuery(_this).wrap(function () {
                    return '<span class="AvatarWrap" style="position:relative; display:inline-block; background:url(' + jQuery(_this).attr('src') + ') no-repeat center center; width: auto; height: auto;" />';
                });
                jQuery(_this).css("opacity", "0");
        });
    });
});

jQuery.fn.onImagesLoaded = function (_cb) { return this.each(function () { var $imgs = (this.tagName.toLowerCase() === 'img') ? $(this) : $('img', this), _cont = this, i = 0, _done = function () { if (typeof _cb === 'function') _cb(_cont); }; if ($imgs.length) { $imgs.each(function () { var _img = this, _checki = function (e) { if ((_img.complete) || (_img.readyState == 'complete' && e.type == 'readystatechange')) { if (++i === $imgs.length) _done(); } else if (_img.readyState === undefined) { $(_img).attr('src', $(_img).attr('src')); } }; $(_img).bind('load readystatechange', function (e) { _checki(e); }); _checki({ type: 'readystatechange' }); });
} else _done();}); };