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
    "[data-bs-toggle=\"confirm\"]",
    function (e, confirmed) {
        if (!confirmed) {
            e.preventDefault();
            var button = $(this);

            var text = button.data("title");
            var yes = button.data("yes");
            var no = button.data("no");

            var title = button.html();

            // Add spinner
            button.html(
                "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading..."
            );

            bootboxConfirm(button, title, text,yes, no, function (r) {
                if (r) {
                    button.trigger('click', true);
                }
            });
        }
    }
);

$(".dropdown-menu button").on("click", function (e, confirmed) {
    if (!confirmed) {

        var button = $(e.currentTarget);
        if (button.data("bs-toggle") !== undefined && button.data("bs-toggle") == "confirm") {


            e.preventDefault();

            var text = button.data("title");
            var title = button.html();
            var yes = button.data("yes");
            var no = button.data("no");

            bootboxConfirm(button, title, text, yes, no, function (r) {
                if (r) {
                    button.trigger("click", true);
                }
            });
        }
    }

});

var bootboxConfirm = function (button, title, message, yes, no, callback) {
    var options = {
        message: message,
        centerVertical: true,
        title: title
    };
    options.buttons = {
        cancel: {
            label: '<i class="fa fa-times"></i> ' + no,
            className: "btn-danger",
            callback: function (result) {
                callback(false);
                button.html(title);
            }
        },
        main: {
            label: '<i class="fa fa-check"></i> ' + yes,
            className: "btn-success",
            callback: function (result) {
                callback(true);
            }
        }
    };
    bootbox.dialog(options);
};


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