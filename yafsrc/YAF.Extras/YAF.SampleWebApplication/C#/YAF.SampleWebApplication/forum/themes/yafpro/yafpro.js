jQuery(document).ready(function () {
    jQuery(".yafnet .forumRow, .yafnet .forumRow_Alt, .yafnet .topicRow, .yafnet .topicRow_Alt").hover(function () {
        jQuery(this).find("img").stop().fadeTo("fast", 0.5);
    }, function () {
        jQuery(this).find("img").stop().fadeTo("fast", 1.0);
    });

    jQuery(".avatarimage").stop().fadeTo("fast", 0.5);

    jQuery(".yafnet .postContainer, .yafnet .postContainer_Alt").hover(function () {
        jQuery(this).find(".avatarimage").stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).find(".avatarimage").stop().fadeTo("fast", 0.5);
    });
});