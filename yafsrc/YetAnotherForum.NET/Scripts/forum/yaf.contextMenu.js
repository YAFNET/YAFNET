jQuery(document).ready(function () {
    $(".list-group-item-menu, .message").each(function () {

        var contextMenu = $(this).find(".context-menu");

        $(this).on("contextmenu", function (e) {
            $(".context-menu").removeClass("show").hide();

            contextMenu.css({
                display: "block"
            }).addClass("show").offset({ left: e.pageX, top: e.pageY });
            return false;
        }).on("click", function () {
            contextMenu.removeClass("show").hide();
        });

        $(this).find(".context-menu a").on("click", function () {
            contextMenu.removeClass("show").hide();
        });

        $("body").click(function () {
            contextMenu.removeClass("show").hide();
        });

    });
});