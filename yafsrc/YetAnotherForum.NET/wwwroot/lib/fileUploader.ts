// File Uploader based on: https://www.smashingmagazine.com/2018/01/drag-drop-file-uploader-vanilla-js/

import Notify from '@w8tcha/bootstrap-notify';

const _global = (window /* browser */ || global /* node */) as any;

interface IFileUploaderConfig {
    dropZone: string | HTMLElement;
    url?: string;
    fileInput?: HTMLInputElement;
    errorDelay: number;
    errorTitle: string;
}

class FileUploader {
    private readonly config: IFileUploaderConfig;
    private readonly progressBar: HTMLElement | null;
    private readonly dropArea: HTMLElement;
    private uploadProgress: number[];

    constructor(args: Partial<IFileUploaderConfig>) {
        if (!(this instanceof FileUploader)) {
            throw new Error('FileUploader must be instantiated with `new`.');
        }

        this.config = {
            dropZone: 'js-file-uploader',
            url: undefined,
            fileInput: undefined,
            errorDelay: 1000,
            errorTitle: '',
            ...args
        };

        this.dropArea = typeof this.config.dropZone === 'string'
            ? document.getElementById(this.config.dropZone) as HTMLElement
            : this.config.dropZone;

        if (!this.dropArea) {
            throw new Error('The drop zone element is undefined.');
        }

        this.progressBar = document.getElementById('progress-bar');
        this.uploadProgress = [];

        // Prevent default drag behaviors
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            this.dropArea.addEventListener(eventName, this.preventDefaults, false);
            document.body.addEventListener(eventName, this.preventDefaults, false);
        });

        const fileInput = document.getElementById('fileElem') as HTMLInputElement;
        fileInput.addEventListener('change', (e: Event) => {
            this.handleFiles((e.target as HTMLInputElement).files!);
        });

        this.dropArea.addEventListener('dragenter', () => this.highlight());
        this.dropArea.addEventListener('dragover', () => this.highlight());
        this.dropArea.addEventListener('dragleave', () => this.unhighlight());

        // Handle dropped files
        this.dropArea.addEventListener('drop', (e) => this.handleDrop(e));
    }

    private preventDefaults(e: Event): void {
        e.preventDefault();
        e.stopPropagation();
    }

    private highlight(): void {
        this.dropArea.classList.add('border-danger', 'border-2');
    }

    private unhighlight(): void {
        this.dropArea.classList.remove('border-danger');
    }

    private bytesToSize(bytes: number, separator = ' '): string {
        const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        if (bytes === 0) return 'n/a';
        const i = Math.floor(Math.log(bytes) / Math.log(1024));
        return i === 0
            ? `${bytes}${separator}${sizes[i]}`
            : `${(bytes / (1024 ** i)).toFixed(1)}${separator}${sizes[i]}`;
    }

    private handleDrop(e: DragEvent): void {
        const files = e.dataTransfer?.files;
        if (files) this.handleFiles(files);
    }

    private initializeProgress(numFiles: number): void {
        if (this.progressBar) {
            this.progressBar.setAttribute('aria-valuenow', '0');
            (this.progressBar.querySelector('.progress-bar') as HTMLElement).style.width = '0%';
        }

        this.uploadProgress = new Array(numFiles).fill(0);
    }

    private updateProgress(fileNumber: number, percent: number): void {
        this.uploadProgress[fileNumber] = percent;
        const total = this.uploadProgress.reduce((tot, curr) => tot + curr, 0) / this.uploadProgress.length;

        if (this.progressBar) {
            this.progressBar.setAttribute('aria-valuenow', total.toString());
            (this.progressBar.querySelector('.progress-bar') as HTMLElement).style.width = `${total}%`;
        }
    }

    private handleFiles(files: FileList): void {
        this.initializeProgress(files.length);

        Array.from(files).forEach((file, index) => {
            this.previewFile(file);
            this.uploadFile(file, index);
        });
    }

    private previewFile(file: File): void {
        const listItem = document.createElement('li');
        listItem.className = 'list-group-item list-group-item-action';

        // Build container div
        const containerDiv = document.createElement('div');
        containerDiv.className = 'd-flex w-100 justify-content-between';

        // Title + preview
        const titleH5 = document.createElement('h5');
        titleH5.className = 'mb-1';

        const previewSpan = document.createElement('span');
        previewSpan.className = 'preview';
        titleH5.appendChild(previewSpan);

        // File size
        const sizeSmall = document.createElement('small');
        sizeSmall.className = 'text-body-secondary size';
        sizeSmall.textContent = this.bytesToSize(file.size);

        containerDiv.appendChild(titleH5);
        containerDiv.appendChild(sizeSmall);

        // File name paragraph (escaped)
        const nameP = document.createElement('p');
        nameP.className = 'name';
        nameP.textContent = file.name;

        listItem.appendChild(containerDiv);
        listItem.appendChild(nameP);

        // Fill preview
        if (file.type.match('image.*')) {
            const img = document.createElement('img');
            img.classList.add('img-thumbnail');

            const reader = new FileReader();
            reader.onloadend = () => { img.src = reader.result as string; };
            reader.readAsDataURL(file);

            previewSpan.appendChild(img);
        } else {
            const icon = document.createElement('i');
            icon.className = 'fa-regular fa-file';
            previewSpan.appendChild(icon);
        }

        document.getElementById('gallery')?.appendChild(listItem);
    }

    private uploadFile(file: File, index: number): void {
        const url = this.config.url;
        if (!url) return;

        const xhr = new XMLHttpRequest();
        const formData = new FormData();
        xhr.open('POST', url, true);

        xhr.upload.addEventListener('progress', (e) => {
            this.updateProgress(index, (e.loaded * 100) / e.total || 100);
        });

        xhr.onreadystatechange = () => {
	        if (xhr.readyState === 4 && xhr.status === 200) {
		        this.updateProgress(index, 100);

		        const response = JSON.parse(xhr.response) as any;

		        if (response[0].error) {
			        const _ = new Notify({
					        title: this.config.errorTitle,
					        message: response[0].error,
					        icon: 'fa fa-exclamation-triangle'
				        },
				        {
					        type: 'danger',
					        element: 'body',
					        placement: { from: 'top', align: 'center' },
					        delay: this.config.errorDelay * 1000
				        });
		        } else {
			        _global.insertAttachment(response[0].fileID, response[0].fileID);
		        }

	        } else if (xhr.readyState === 4) {
		        console.error(xhr);
	        }
        };

        formData.append('file', file);
        xhr.send(formData);
    }
}

_global.FileUploader = FileUploader;