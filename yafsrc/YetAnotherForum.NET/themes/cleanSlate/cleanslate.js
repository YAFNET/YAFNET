(function($) {
    $(document).ready(function () {
        $('.yafnet .forumRow, .yafnet .forumRow_Alt, .yafnet .topicRow, .yafnet .topicRow_Alt').hover(function () {
            $(this).find('img').stop().fadeTo('fast', 0.5);
        }, function() {
            $(this).find('img').stop().fadeTo('fast', 1.0);
        });

        $('.yafnet .postContainer .avatarimage, .yafnet .postContainer_Alt .avatarimage').stop().fadeTo('fast', 0.5);

        $('.yafnet .postContainer, .yafnet .postContainer_Alt').hover(function () {
            var $select = $(this);
            $select.find('.avatarimage').stop(true, true).fadeTo('fast', 1.0);
            $select.find('.postPosted .postedRight, .postInfoBottom .displayPostFooter').stop(true, true).delay(500).fadeIn('fast');
        }, function() {
            var $select = $(this);
            $select.find('.avatarimage').stop().fadeTo('fast', 0.5);
            $select.find('.postPosted .postedRight, .postInfoBottom .displayPostFooter').stop(true, true).delay(500).hide(0);
        });

        $('.yafnet').find('.postPosted .postedRight, .postInfoBottom .displayPostFooter').hide();
    });
})(jQuery);