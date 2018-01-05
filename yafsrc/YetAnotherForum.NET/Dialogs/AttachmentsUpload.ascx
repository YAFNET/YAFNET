<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Dialogs.AttachmentsUpload" CodeBehind="AttachmentsUpload.ascx.cs" %>

<div class="UploadDialog modal" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
          <h5 class="modal-title" id="myModalLabel"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "UPLOAD_TITLE") %></h5>
          <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body" style="min-height:485px">
        <h4><YAF:LocalizedLabel ID="SelectFile" LocalizedTag="SELECT_FILE" LocalizedPage="ATTACHMENTS" runat="server" /></h4>
        <div>
            <div id="fileupload">
                      <div class="fileupload-buttonbar">
                          <div id="dropzone" class="card text-white bg-dark border-danger mb-3">
                              <div class="card-body">
                                  <p class="card-text"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "DROP_HERE") %></p>
                              </div>
                          </div>
                          <div class="alert alert-danger uploadCompleteWarning" role="alert" style="display: none">
                              <%= this.Get<ILocalization>().GetText("ATTACHMENTS", "COMPLETE_WARNING") %>
                          </div>
                          <div class="fileupload-buttons">
                              <span class="btn btn-success fileinput-button">
                                  <i class="fa fa-plus fa-fw"></i>&nbsp;<YAF:LocalizedLabel id="AddFiles" runat="server" LocalizedPage="ATTACHMENTS" LocalizedTag="ADD_FILES" />
                                  <input type="file" name="files[]" multiple>
                              </span>
                              <button type="submit" class="btn btn-primary start">
                                  <i class="fa fa-upload fa-fw"></i><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "START_UPLOADS") %>
                              </button>
                              <span class="fileupload-process"></span>
                          </div>
                          <div class="fileupload-progress fade-ui" style="display:none">
                              <div class="progress" role="progressbar" aria-valuemin="0" aria-valuemax="100"></div>
                              <div class="progress-extended">&nbsp;</div>
                          </div>
                      </div>
                      <div id="UploadFilesBox">
                          <table role="presentation" class="table-striped UploadFiles"><tbody class="files"></tbody></table>
                      </div>
                  </div>
                  <script id="template-upload" type="text/x-tmpl">
                      {% for (var i=0, file; file=o.files[i]; i++) { %}
                          <tr class="template-upload fade-ui">
                              <td>
                                  <span class="preview"></span>
                              </td>
                              <td>
                                  <p class="name">{%=file.name%}</p>
                                  <strong class="error"></strong>
                                  <p class="size">Processing...</p>
                                  <div class="progress"></div>
                              </td>
                              <td>
                                  {% if (!i && !o.options.autoUpload) { %}
                <button class="start" disabled style="display:none">Start</button>
            {% } %}
                      {% if (!i) { %}
                                      <button class="btn btn-danger cancel"><i class="fa fa-times fa-fw"></i>&nbsp;<%= this.Get<ILocalization>().GetText("COMMON", "CANCEL") %></button>
                                  {% } %}
                              </td>
                          </tr>
                      {% } %}
                  </script>

                  <script id="template-download" type="text/x-tmpl">
                  </script>
        </div>

            <asp:PlaceHolder ID="UploadNodePlaceHold" runat="server">
                <div class="alert alert-info" role="alert">
                    <asp:Label ID="UploadNote" runat="server"></asp:Label>
                </div>
            </asp:PlaceHolder>
      </div>
      <div class="modal-footer">
        <div class="alert alert-info" role="alert">
                <strong><YAF:LocalizedLabel ID="ExtensionTitle" LocalizedTag="ALLOWED_EXTENSIONS" runat="server" />:</strong>&nbsp;<asp:Label ID="ExtensionsList" runat="server"></asp:Label>
        </div>
      </div>
    </div>
  </div>
</div>
