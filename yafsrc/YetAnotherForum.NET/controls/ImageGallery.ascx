<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.ImageGallery" CodeBehind="ImageGallery.ascx.cs" %>

<div id="blueimp-gallery" class="blueimp-gallery "data-use-bootstrap-modal="true">
    <div class="slides"></div>
    <div class="modal fade">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"></h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body next"></div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary float-left prev">
                        <i class="fa fa-chevron-left fa-fw"></i>
                        Previous
                    </button>
                    <button type="button" class="btn btn-primary next">
                        Next
                        <i class="fa fa-chevron-right fa-fw"></i>
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>