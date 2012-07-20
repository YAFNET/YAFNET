jQuery(document).ready(function () {
    
    jQuery(".yafnet .yafcssbigbutton").fadeTo("fast", 0.7);
    jQuery(".yafnet .yaflittlebutton").fadeTo("fast", 0.7);
    jQuery(".yafnet .MultiQuoteButton").fadeTo("fast", 0.7);
    
    jQuery(".yafnet .yafcssbigbutton").hover(function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 0.7);
    });
    jQuery(".yafnet .yaflittlebutton").hover(function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 0.7);
    });
    jQuery(".yafnet .MultiQuoteButton").hover(function () {
        jQuery(this).stop().fadeTo("fast", 1.0);
    }, function () {
        jQuery(this).stop().fadeTo("fast", 0.7);
    }); 
    
    /* commented out due to very laggy response in most browsers on not so 'fresh' hardware 
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
    */
    /* much speedier, more responsive all the way around */
    
    jQuery(".yafnet .forumRow").hover(function () { jQuery(this).addClass("forumRow_hover"); }, function() { jQuery(this).removeClass("forumRow_hover"); });
    jQuery(".yafnet .forumRow_Alt").hover(function () { jQuery(this).addClass("forumRow_Alt_hover"); }, function() { jQuery(this).removeClass("forumRow_Alt_hover"); });
    jQuery(".yafnet .topicRow").hover(function () { jQuery(this).addClass("forumRow_Alt_hover"); }, function() { jQuery(this).removeClass("forumRow_Alt_hover"); });
    jQuery(".yafnet .topicRow_Alt").hover(function () { jQuery(this).addClass("forumRow_Alt_hover"); }, function() { jQuery(this).removeClass("forumRow_Alt_hover"); });    

});