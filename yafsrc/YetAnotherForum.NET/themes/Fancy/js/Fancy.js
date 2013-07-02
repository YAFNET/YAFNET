function pageLoad() {
    jQuery('.yafnet').hide();
    jQuery('.content, .yafPageLink').addClass('contentShadow roundShadow');
    jQuery('#DivIconLegend, #DivPageAccess').addClass('content contentShadow roundShadow');
    jQuery('.menuContainer').addClass('contentShadow raisedShadow');
    jQuery('.menuMyList .myProfile a').text('.');
    jQuery('.menuMyList .myPm a').text('%');
    jQuery('.menuMyList .myBuddies a').text(',');
    jQuery('.menuMyList .myAlbums a').text('p');
    jQuery('.menuMyList .myTopics a').text('9');
    jQuery('.menuMyList .menuAccount a').text('E');
    jQuery(".loggedInUser").insertBefore('.menuMyList');
    jQuery("img, input, a").tipTip();
    jQuery(function() {
        jQuery('img.avatarimage').onImagesLoaded(function(_this) {
            jQuery(_this).wrap(function() {
                return '<span class="AvatarWrap" style="position:relative; display:inline-block; background:url(' + jQuery(_this).attr('src') + ') no-repeat center center; width: auto; height: auto;" />';
            });
            jQuery(_this).css("opacity", "0");
        });
    });

    if (jQuery("input:checkbox").not('.MultiQuoteButton input')) {
        jQuery("input:checkbox").not('.MultiQuoteButton input').uniform();
    }

    jQuery("input:radio, input:file").not('.MultiQuoteButton input').uniform();
    jQuery('.QuickSearchButton').text('A');
    jQuery('select').not('[id*="ProfileEditor_Country"]').each(function() {
        jQuery(this).msDropDown();
    });

    jQuery('.dd').each(function() {
        var width = jQuery(this).width();
        jQuery(this).find('.ddChild').eq(0).css({
            'width': width - 12
        });
        jQuery(this).find('.ddTitleText').eq(0).css({
            'width': width - 30
        });
        jQuery(this).find('.arrow').eq(0).text('l');
    });

    jQuery('.yafnet').show();
}

jQuery.fn.onImagesLoaded = function(_cb) {
    return this.each(function() {
        var $imgs = (this.tagName.toLowerCase() === 'img') ? $(this) : $('img', this), _cont = this, i = 0, _done = function() { if (typeof _cb === 'function') _cb(_cont); };
        if ($imgs.length) {
            $imgs.each(function() {
                var _img = this, _checki = function(e) {
                    if ((_img.complete) || (_img.readyState == 'complete' && e.type == 'readystatechange')) {
                        if (++i === $imgs.length) _done();
                    } else if (_img.readyState === undefined) {
                        $(_img).attr('src', $(_img).attr('src'));
                    }
                };
                $(_img).bind('load readystatechange', function(e) { _checki(e); });
                _checki({ type: 'readystatechange' });
            });
        } else _done();
    });
};