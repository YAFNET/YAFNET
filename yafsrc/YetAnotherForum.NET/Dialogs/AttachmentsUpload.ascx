<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Dialogs.AttachmentsUpload" CodeBehind="AttachmentsUpload.ascx.cs" %>
<%@ Import Namespace="YAF.Configuration" %>


<div class="modal fade" id="UploadDialog" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
          <h5 class="modal-title">
              <%= this.Get<ILocalization>().GetText("ATTACHMENTS", "TITLE") %>
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
	      <div class="card mb-3">
		      <div class="card" id="drop-area">
			      <div class="card-body">
				      <h5 class="card-title">
					      <YAF:LocalizedLabel ID="SelectFile"
					                          LocalizedTag="UPLOAD_TITLE"
					                          LocalizedPage="ATTACHMENTS" runat="server" />
				      </h5>
				      <div class="my-form link-light bg-dark text-center rounded">
					      <p class="card-text"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "DROP_HERE") %></p>

					      <span class="btn btn-success fileinput-button m-1">
						      <i class="fa fa-plus"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedPage="ATTACHMENTS" LocalizedTag="ADD_FILES" />
						      <input type="file" id="fileElem" multiple>
					      </span>
				      </div>
				      <div class="progress m-2" role="progressbar" id="progress-bar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100">
					      <div class="progress-bar" style="width: 0%"></div>
				      </div>

				      <ul class="list-group" id="gallery"></ul>
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
	      <div class="card">
		      <div class="card-body">
			      <h5 class="card-title"><%= this.Get<ILocalization>().GetText("ATTACHMENTS", "CURRENT_UPLOADS") %></h5>
			      <form>
				      <div id="AttachmentsListPager"></div>
				      <div id="PostAttachmentLoader" class="text-center">
					      <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
				      </div>
				      <div id="AttachmentsListBox">
					      <div id="PostAttachmentListPlaceholder" data-url="<%= BoardInfo.ForumClientFileRoot %>" data-notext="<%= this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS") %>" style="clear: both; ">
						      <div class="container">
							      <div class="AttachmentList row"></div>
						      </div>
					      </div>
				      </div>
			      </form>
		      </div>
	      </div>
      </div>
	    <div class="modal-footer">
		    <YAF:ThemeButton runat="server" ID="Close"
		                     DataDismiss="modal"
		                     TextLocalizedTag="CLOSE_TEXT"
		                     Type="Secondary"
		                     Icon="times">
		    </YAF:ThemeButton>
	    </div>
    </div>
  </div>
</div>
						                                                                                                                                   