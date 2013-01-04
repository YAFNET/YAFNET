jQuery(document).ready(function () {
    jQuery(".yafnet .yafcssbigbutton").fadeTo("fast", 0.6);
    jQuery(".yafnet .yaflittlebutton").fadeTo("fast", 0.6);
	jQuery(".yafnet .MultiQuoteButton").fadeTo("fast", 0.6);
    jQuery(".yafnet .yafcssbigbutton").hover(function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 0.5);
    });
    jQuery(".yafnet .yaflittlebutton").hover(function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 0.5);
    });
	jQuery(".yafnet .MultiQuoteButton").hover(function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 0.5);
    });
    jQuery(".yafnet .forumRow").hover(function () {
        jQuery(this).stop().fadeTo("fast", 0.6);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    });
    jQuery(".yafnet .forumRow_Alt").hover(function () {
        jQuery(this).stop().fadeTo("fast", 0.6);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    });
    jQuery(".yafnet .topicRow").hover(function () {
        jQuery(this).stop().fadeTo("fast", 0.6);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    });
    jQuery(".yafnet .topicRow_Alt").hover(function () {
        jQuery(this).stop().fadeTo("fast", 0.6);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    });


});