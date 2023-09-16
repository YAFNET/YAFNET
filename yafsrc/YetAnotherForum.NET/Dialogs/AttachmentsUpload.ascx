<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Dialogs.AttachmentsUpload" CodeBehind="AttachmentsUpload.ascx.cs" %>


<div class="modal fade" id="UploadDialog" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
          <h5 class="modal-title" id="myModalLabel">
              <%= this.Get<ILocalization>().GetText("ATTACHMENTS", "UPLOAD_TITLE") %>
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <h4>
            <YAF:LocalizedLabel ID="SelectFile"
                                LocalizedTag="SELECT_FILE"
                                LocalizedPage="ATTACHMENTS" runat="server" />
        </h4>
        <div>
              <div id="drop-area" class="card link-light bg-dark mb-3 text-center">
                  <div class="card-body">
                      <form class="my-form">
                          <p class="card-text"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "DROP_HERE") %></p>

                          <span class="btn btn-success fileinput-button m-1">
                              <i class="fa fa-plus fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedPage="ATTACHMENTS" LocalizedTag="ADD_FILES" />
                              <input type="file" id="fileElem" multiple>
                          </span>
                      </form>
                  </div>
                  
              </div>

            <div class="progress m-2" role="progressbar" id="progress-bar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">
                <div class="progress-bar" style="width: 0%"></div>
            </div>

            <ul class="list-group" id="gallery"></ul>
        </div>
      </div>
      <div class="modal-footer">
          <asp:PlaceHolder ID="UploadNodePlaceHold" runat="server">
              <YAF:Alert runat="server" Type="warning"><asp:Label ID="UploadNote" runat="server"></asp:Label></YAF:Alert>
          </asp:PlaceHolder>
        <div class="alert alert-info" role="alert">
                <strong><YAF:LocalizedLabel ID="ExtensionTitle"
                                            LocalizedTag="ALLOWED_EXTENSIONS" runat="server" /></strong>
            &nbsp;
            <asp:Label ID="ExtensionsList" runat="server"></asp:Label>
        </div>
      </div>
    </div>
  </div>
</div>
