// File Uploader based on: https://www.smashingmagazine.com/2018/01/drag-drop-file-uploader-vanilla-js/

var FileUploader = (function () {
    function fileUploader(args) {
        var self = this;

        if (!(this instanceof fileUploader)) {
            throw "FileUploader must be instantiated with `new`.";
        }

        this.config = {
            dropZone: "js-file-uploader",
            url: undefined,
            fileInput: undefined,
            errorDelay: 1000,
            errorTitle: ""
        };

        for (let p in args) {
            this.config[p] = args[p];
        }

        const dropArea = typeof (this.config.dropZone) == "string" ? document.getElementById(this.config.dropZone) : this.config.dropZone;

        if (!dropArea) {
            throw "the drop zone element is undefined.";
        }

        this.progressBar = document.getElementById("progress-bar");
        this.dropArea = dropArea;
        this.uploadProgress = [];

        // Prevent default drag behaviors
        ["dragenter", "dragover", "dragleave", "drop"].forEach(eventName => {
                dropArea.addEventListener(eventName, self.preventDefaults, false);
            document.body.addEventListener(eventName, self.preventDefaults, false);
        });

        document.querySelector(this.config.fileInput).addEventListener("onchange",
            (e) => {
                self.handleDrop(e, self);
            }); 

        dropArea.ondragenter = function () {
            self.highlight(self);
        };

        dropArea.ondragover = function () {
            self.highlight(self);
        };

        dropArea.ondragleave = function () {
            self.unhighlight(self);
        };

        dropArea.ondrop = function () {
            self.unhighlight(self);
        };

        // Handle dropped files
        dropArea.addEventListener("drop", (e) => {
            self.handleDrop(e, self);
        }, false);
    }

    fileUploader.prototype.unhighlight = function (self) {
        self.dropArea.classList.remove("border-danger");
    };

    fileUploader.prototype.highlight = function (self) {
        self.dropArea.classList.add("border-danger");
        self.dropArea.classList.add("border-2");
    };

    fileUploader.prototype.preventDefaults = function (e) {
        e.preventDefault();
        e.stopPropagation();
    };

    fileUploader.prototype.bytesToSize = function (bytes, separator = " ") {
        const sizes = ["Bytes", "KB", "MB", "GB", "TB"];
        if (bytes === 0) return "n/a";
        const i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)), 10);
        if (i === 0) return `${bytes}${separator}${sizes[i]}`;
        return `${(bytes / (1024 * i)).toFixed(1)}${separator}${sizes[i]}`;
    };

    fileUploader.prototype.handleDrop = function (e, self) {
        const dt = e.dataTransfer,
            files = dt.files;

        self.handleFiles(self, files);
    };

    fileUploader.prototype.initializeProgress = function (self, numFiles) {
        
        self.progressBar.setAttribute("aria-valuenow", 0);
        self.progressBar.querySelector(".progress-bar").style.width = "0%";

        self.uploadProgress = [];

        for (let i = numFiles; i > 0; i--) {
            self.uploadProgress.push(0);
        }
    };

    fileUploader.prototype.updateProgress = function (self, fileNumber, percent) {

        self.uploadProgress[fileNumber] = percent;
        const total = self.uploadProgress.reduce((tot, curr) => tot + curr, 0) / self.uploadProgress.length;

        self.progressBar.setAttribute("aria-valuenow", total);
        self.progressBar.querySelector(".progress-bar").style.width = total + "%";
    };

    fileUploader.prototype.handleFiles = function (self, files) {

        files = [...files];
        self.initializeProgress(self, files.length);

        files.forEach(file => {
            self.previewFile(self, file);
        });

        files.forEach(function(item, index) {
            self.uploadFile(self, item, index);
        });

        // close dialog
        files = null;
        document.getElementById("gallery").replaceChildren();

        bootstrap.Modal.getInstance("#UploadDialog").hide();

        if (document.querySelector("#fileupload .alert-danger") != null) {
            bootstrap.Alert.getInstance("#fileupload .alert-danger").close();
        }

        const pageSize = 5, pageNumber = 0;
        getPaginationData(pageSize, pageNumber, false);
    };

    fileUploader.prototype.previewFile = function (self, file) {

        const listItem = document.createElement("li");

        listItem.className = "list-group-item list-group-item-action";

        listItem.innerHTML = `<div class="d-flex w-100 justify-content-between">
                                 <h5 class="mb-1">
                                     <span class="preview"></span>
                                 </h5>
                                 <small class="text-body-secondary size">${self.bytesToSize(file.size)}</small>
                             </div>
                             <div class="mb-1">
                                 <p class="name">${file.name}</p>
                             </div>
                             <small class="text-body-secondary">
                             <div class="btn-group" role="group">
                             </div></small>`;

        if (file.type.match("image.*")) {
            const img = document.createElement("img"),
                reader = new FileReader();

            img.classList.add("img-thumbnail");

            reader.readAsDataURL(file);
            reader.onloadend = function () {
                img.src = reader.result;
            };

            listItem.querySelector(".preview").appendChild(img);
        } else {
            const icon = document.createElement("i");

            icon.className = "fa-regular fa-file";

            listItem.querySelector(".preview").appendChild(icon);
        }

        document.getElementById("gallery").appendChild(listItem);
    };

    fileUploader.prototype.uploadFile = function (self, file, i) {

        const url = self.config.url;
        var xhr = new XMLHttpRequest();
        const formData = new FormData();
        xhr.open("POST", url, true);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");

        // Update progress (can be used to show progress indicator)
        xhr.upload.addEventListener("progress", function (e) {
            self.updateProgress(self, i, (e.loaded * 100.0 / e.total) || 100);
        });

        xhr.addEventListener("readystatechange", function () {
            if (xhr.readyState === 4 && xhr.status === 200) {

                self.updateProgress(self, i, 100);

                const response = JSON.parse(xhr.response);

                if (response[0].error) {
                    const _ = new Notify({
                            title: self.config.errorTitle,
                            message: response[0].error,
                            icon: "fa fa-exclamation-triangle"
                        },
                        {
                            type: "danger",
                            element: "body",
                            position: null,
                            placement: { from: "top", align: "center" },
                            delay: self.config.errorDelay * 1000
                        });
                } else {
                    insertAttachment(response[0].fileID, response[0].fileID);
                }
            }
            else if (xhr.readyState === 4 && xhr.status !== 200) {
                console.log(xhr);
            }
        });

        formData.append("file", file);
        xhr.send(formData);
    };

    return fileUploader;
})();