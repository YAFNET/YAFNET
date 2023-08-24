document.addEventListener("DOMContentLoaded", function () {

    const myModal = new bootstrap.Modal(".yafWizard",
        {
            backdrop: "static",
            keyboard: false
        });

    myModal.show();

    [].forEach.call(document.querySelectorAll(".btn-primary,.btn-info"),
        function(e) {
            e.addEventListener("click",
                function() {
                    // code…
                    e.innerHTML =
                        "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading...";
                });
        });
})