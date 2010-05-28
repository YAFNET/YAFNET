$(document).ready(function () {
    $(".yafnet .forumRow").hover(function () {
        $(this).stop().fadeTo("fast", 0.5);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });
    $(".yafnet .forumRow_Alt").hover(function () {
        $(this).stop().fadeTo("fast", 0.5);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });
    $(".yafnet .topicRow").hover(function () {
        $(this).stop().fadeTo("fast", 0.5);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });
    $(".yafnet .topicRow_Alt").hover(function () {
        $(this).stop().fadeTo("fast", 0.5);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });

    $(".avatarimage").stop().fadeTo("fast", 0.5);

    $(".yafnet .postContainer, .yafnet .postContainer_Alt").hover(function () {
        $(this).find(".avatarimage").stop().fadeTo("fast", 1.0);
    }, function () {
        $(this).find(".avatarimage").stop().fadeTo("fast", 0.5);
    });


});