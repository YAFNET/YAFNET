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

// Confirm Dialog
$(document).on("click",
    "[data-toggle=\"confirm\"]",
    function (e) {
        e.preventDefault();
        var link = $(this).attr("href");
        var text = $(this).data("title");
        var title = $(this).html();
        var blockUI = $(this).data("confirm-event");
        bootbox.confirm({
            centerVertical: true,
                title: title,
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
document.addEventListener("DOMContentLoaded", function () {

    if (document.body.contains(document.getElementById("PasswordToggle"))) {
        var passwordToggle = document.getElementById("PasswordToggle");
        var icon = passwordToggle.querySelector("i");
        var pass = document.querySelector("input[id*='Password']");
        passwordToggle.addEventListener("click", function (event) {
            event.preventDefault();
            if (pass.getAttribute("type") === "text") {
                pass.setAttribute("type", "password");
                icon.classList.add("fa-eye-slash");
                icon.classList.remove("fa-eye");
            } else if (pass.getAttribute("type") === "password") {
                pass.setAttribute("type", "text");
                icon.classList.remove("fa-eye-slash");
                icon.classList.add("fa-eye");
            }
        });
    }
})