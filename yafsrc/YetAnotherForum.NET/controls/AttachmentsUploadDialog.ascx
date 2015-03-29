<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AttachmentsUploadDialog"
    CodeBehind="AttachmentsUploadDialog.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>

<div class="UploadDialog" title='<%= this.Get<ILocalization>().GetText("ATTACHMENTS", "UPLOAD_TITLE") %>' style="display: none;">
    <fieldset>
        <p><strong><YAF:LocalizedLabel ID="SelectFile" LocalizedTag="SELECT_FILE" LocalizedPage="ATTACHMENTS" runat="server" /></strong></p>
        <div>
            <div id="fileupload">
                      <div class="fileupload-buttonbar">
                          <div id="dropzone" class="fade-ui ui-widget-header"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "DROP_HERE") %></div>
                          <div class="ui-widget">
                              <div class="ui-state-error ui-corner-all uploadCompleteWarning" style="padding: 0 .7em; margin-bottom: 15px;display:none">
                                  <p><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em"></span>
                                      <%= this.Get<ILocalization>().GetText("ATTACHMENTS", "COMPLETE_WARNING") %>
                                  </p>
                              </div>
                          </div>
                          <div class="fileupload-buttons">
                              <span class="fileinput-button">
                                  <YAF:LocalizedLabel id="AddFiles" runat="server" LocalizedPage="ATTACHMENTS" LocalizedTag="ADD_FILES" />
                                  <input type="file" name="files[]" multiple>
                              </span>
                              <button type="submit" class="start"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "START_UPLOADS") %></button>
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
                                      <button class="cancel"><%= this.Get<ILocalization>().GetText("COMMON", "CANCEL") %></button>
                                  {% } %}
                              </td>
                          </tr>
                      {% } %}
                  </script>
                  
                  <script id="template-download" type="text/x-tmpl">
                  </script>
        </div>
        <div style="padding: 5px;margin:5px">
            <asp:PlaceHolder ID="UploadNodePlaceHold" runat="server">
                <br />
                <em><asp:Label ID="UploadNote" runat="server"></asp:Label></em><br/><br />
            </asp:PlaceHolder>
            <strong><YAF:LocalizedLabel ID="ExtensionTitle" LocalizedTag="ALLOWED_EXTENSIONS" runat="server" />:</strong>&nbsp;<asp:Label ID="ExtensionsList" runat="server"></asp:Label>
        </div>
    </fieldset>
</div>
