jQuery(document).ready(function () {
    jQuery(".yafnet .forumRow, .yafnet .forumRow_Alt, .yafnet .topicRow, .yafnet .topicRow_Alt").hover(function () {
        jQuery(this).find("img").stop().fadeTo("fast", 0.5);
    }, function () {
        jQuery(this).find("img").stop().fadeTo("fast", 1.0);
    });

    jQuery(".yafnet .postContainer .avatarimage, .yafnet .postContainer_Alt .avatarimage").stop().fadeTo("fast", 0.5);

    jQuery(".yafnet .postContainer, .yafnet .postContainer_Alt").hover(function () {
        jQuery(this).find(".avatarimage").stop(true, true).fadeTo("fast", 1.0);
        jQuery(this).find('.postPosted .postedRight').stop(true, true).fadeIn('fast');
        jQuery(this).find('.postInfoBottom div').stop(true, true).fadeIn('fast');
    }, function () {
        jQuery(this).find(".avatarimage").stop().fadeTo("fast", 0.5);
        jQuery(this).find('.postPosted .postedRight').stop(true, true).fadeOut('slow');
        jQuery(this).find('.postInfoBottom div').stop(true, true).fadeOut('slow');
    });

    jQuery('.yafnet .postPosted .postedRight').hide();
    jQuery('.yafnet .postInfoBottom div').hide();
});