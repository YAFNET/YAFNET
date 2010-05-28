$(document).ready(function () {
    $(".yafnet .yafcssbigbutton").fadeTo("fast", 0.6);
    $(".yafnet .yaflittlebutton").fadeTo("fast", 0.6);
    $(".yafnet .yafcssbigbutton").hover(function () {
        $(this).stop().fadeTo("fast", 1.0);
    }, function () {
        $(this).stop().fadeTo("fast", 0.5);
    });
    $(".yafnet .yaflittlebutton").hover(function () {
        $(this).stop().fadeTo("fast", 1.0);
    }, function () {
        $(this).stop().fadeTo("fast", 0.5);
    });
    $(".yafnet .forumRow").hover(function () {
        $(this).stop().fadeTo("fast", 0.6);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });
    $(".yafnet .forumRow_Alt").hover(function () {
        $(this).stop().fadeTo("fast", 0.6);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });
    $(".yafnet .topicRow").hover(function () {
        $(this).stop().fadeTo("fast", 0.6);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });
    $(".yafnet .topicRow_Alt").hover(function () {
        $(this).stop().fadeTo("fast", 0.6);
    }, function () {
        $(this).stop().fadeTo("fast", 1.0);
    });


});