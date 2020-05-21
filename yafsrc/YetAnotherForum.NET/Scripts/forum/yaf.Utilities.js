// Helper Function for Select2
function formatState(state) {
    if (!state.id) {
        return state.text;
    }
    if ($($(state.element).data("content")).length === 0) {
        return state.text;
    }

    var $state = $($(state.element).data("content"));
    return $state;
}

// --
function doClick(buttonName, e) {
    var key;

    if (window.event) {
        key = window.event.keyCode;
    } else {
        key = e.which;
    }

    if (key == 13) {
        var btn = document.getElementById(buttonName);
        if (btn != null) {
            e.preventDefault();
            btn.click();
            event.keyCode = 0;
        }
    }
}

// Confirm Dialog
$(document).on("click",
    "[data-toggle=\"confirm\"]",
    function (e) {
        e.preventDefault();
        var link = $(this).attr("href");
        var text = $(this).data("title");
        var blockUI = $(this).data("confirm-event");
        bootbox.confirm({
                centerVertical: true,
                message: text,
                buttons: {
                    confirm: {
                        label: '<i class="fa fa-check"></i> ' + $(this).data("yes"),
                        className: "btn-success"
                    },
                    cancel: {
                        label: '<i class="fa fa-times"></i> ' + $(this).data("no"),
                        className: "btn-danger"
                    }
                },
                callback: function (confirmed) {
                    if (confirmed) {
                        document.location.href = link;

                        if (typeof blockUI !== "undefined") {
                            window[blockUI]();
                        }
                    }
                }
            }
        );
    }
);

$(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
        $(".scroll-top:hidden").stop(true, true).fadeIn();
    } else {
        $(".scroll-top").stop(true, true).fadeOut();
    }
});
$(function () {
    $(".btn-scroll").click(function () {
        $("html,body").animate({ scrollTop: $("header").offset().top }, "1000");
        return false;
    });
});

// Toggle password visibility 
$(document).ready(function () {
    $("#PasswordToggle").on("click", function (event) {
        event.preventDefault();
        var pass = $("input[id*='Password']");
        if (pass.attr("type") === "text") {
            pass.attr("type", "password");
            $("#PasswordToggle i").addClass("fa-eye-slash");
            $("#PasswordToggle i").removeClass("fa-eye");
        } else if (pass.attr("type") === "password") {
            pass.attr("type", "text");
            $("#PasswordToggle i").removeClass("fa-eye-slash");
            $("#PasswordToggle i").addClass("fa-eye");
        }
    });
});