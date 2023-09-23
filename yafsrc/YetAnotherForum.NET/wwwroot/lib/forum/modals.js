document.addEventListener("DOMContentLoaded",
    function() {
        var placeholderElement = document.getElementById("modal-placeholder");

        document.querySelectorAll('button[data-bs-toggle="ajax-modal"],a[data-bs-toggle="ajax-modal"]').forEach(button => {
                button.addEventListener("click",
                    (event) => {
                        event.preventDefault();
                        const url = button.dataset.url;

                        fetch(url).then(res => res.text()).then(data => {
                            placeholderElement.innerHTML = data;

                            loadModal(window.dialog = new bootstrap.Modal(placeholderElement.querySelector(".modal")),
                                placeholderElement);
                        }).catch(error => {
                            console.log(error);
                        });
                    });
            });

    });

function loadModal(modal, placeholderElement) {
    modal.show();

    modal._element.addEventListener("shown.bs.modal",
        event => {
            if (event.target.id === "LoginBox") {
                var form = document.querySelector(".modal.show").querySelector("form");
                form.addEventListener("submit", function (e) {
                    if (!form.checkValidity()) {
                        e.preventDefault();
                        e.stopPropagation();
                    }

                    form.classList.add("was-validated");
                }, false);
            } else {
                dialogFunctions(event);
            }
            
        });

    if (placeholderElement.querySelector('[data-bs-save="modal"]') != null) {
        placeholderElement.querySelector('[data-bs-save="modal"]').addEventListener("click",
            (event) => {
                event.preventDefault();

                var form = document.querySelector(".modal.show").querySelector("form");
                const actionUrl = form.action;

                fetch(actionUrl, {
                    method: "POST",
                    body: serialize(form, {
                        hash: true
                    }),
                    headers: {
                        'Accept': "application/json",
                        'Content-Type': "application/json;charset=utf-8",
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                }).then(res => res.json())
                    .then(response =>
                    {
                        if (response) {
                            if (response.messageType) {
                                showModalNotify(response.messageType, response.message, ".modal.show form");
                            } else {
                                window.location.href = response;
                            }
                        } else {
                            window.location.href = window.location.pathname + window.location.search;
                        }
                    });
            });
    }

    if (placeholderElement.querySelector('[data-bs-save="editModal"]') != null) {
        placeholderElement.querySelector('[data-bs-save="editModal"]').addEventListener("click",
            (event) => {
                event.preventDefault();
                event.stopPropagation();

                var form = document.querySelector(".modal.show").querySelector("form");
                const actionUrl = form.action;

                form.classList.add("was-validated");

                if (!form.checkValidity()) {

                    return;
                }

                fetch(actionUrl, {
                        method: "POST",
                        body: serialize(form, {
                            hash: true
                        }),
                        headers: {
                            'Accept': "application/json",
                            'Content-Type': "application/json;charset=utf-8",
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        }
                    }).then(res => res.json())
                    .then(response => {
                        if (response) {
                            if (response.messageType) {
                                showModalNotify(response.messageType, response.message, ".modal.show form");
                            } else {
                                window.location.href = response;
                            }
                        } else {
                            window.location.href = window.location.pathname + window.location.search;
                        }
                    }).catch(function () {
                        window.location.href = window.location.pathname + window.location.search;
                    });
            });
    }

    if (placeholderElement.querySelector('[data-bs-save="importModal"]') != null) {
        placeholderElement.querySelector('[data-bs-save="importModal"]').addEventListener("click",
            (event) => {
                event.preventDefault();
                event.stopPropagation();

                var form = document.querySelector(".modal.show").querySelector("form");
                const actionUrl = form.action, formData = new FormData(),
                    fileInput = document.getElementById("Import");

                formData.append("file", fileInput.files[0]);

                fetch(actionUrl, {
                        method: "POST",
                    body: formData,
                        headers: {
                            'Accept': "application/json",
                            'Content-Type': "application/json;charset=utf-8",
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        }
                    }).then(res => res.json())
                    .then(response => {
                        if (response) {
                            if (response.messageType) {
                                showModalNotify(response.messageType, response.message, ".modal.show form");
                            } else {
                                window.location.href = response;
                            }
                        } else {
                            window.location.href = window.location.pathname + window.location.search;
                        }
                    }).catch(function () {
                        window.location.href = window.location.pathname + window.location.search;
                    });
            });
    }
}