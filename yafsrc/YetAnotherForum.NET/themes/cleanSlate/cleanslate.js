$(document).ready(function () {
    $(".yafnet .forumRow, .yafnet .forumRow_Alt, .yafnet .topicRow, .yafnet .topicRow_Alt").hover(function () {
        $(this).find("img").stop().fadeTo("fast", 0.5);
    }, function () {
        $(this).find("img").stop().fadeTo("fast", 1.0);
    });

    $(".avatarimage").stop().fadeTo("fast", 0.5);

    $(".yafnet .postContainer, .yafnet .postContainer_Alt").hover(function () {
        $(this).find(".avatarimage").stop().fadeTo("fast", 1.0);
    }, function () {
        $(this).find(".avatarimage").stop().fadeTo("fast", 0.5);
    });


});