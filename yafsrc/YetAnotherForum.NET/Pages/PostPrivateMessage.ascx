<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PostPrivateMessage" Codebehind="PostPrivateMessage.ascx.cs" %>

<%@ Register TagPrefix="YAF" TagName="AttachmentsUploadDialog" Src="../Dialogs/AttachmentsUpload.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
<div class="col">
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-envelope-open-text fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                                                  LocalizedTag="TITLE" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder id="PreviewRow" runat="server" visible="false">
                    <div class="form-group">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="previewtitle" />
                        <YAF:MessagePost ID="PreviewMessagePost" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="ToRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="To">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="to" />
                        </asp:Label>
                        <asp:TextBox ID="To" runat="server" CssClass="form-control" />
                        <asp:DropDownList runat="server" ID="ToList" Visible="false" CssClass="select2-select" />
                        <div class="btn-group mt-3">
                        <YAF:ThemeButton runat="server" ID="FindUsers" 
                                         Size="Small" 
                                         OnClick="FindUsers_Click"
                                         TextLocalizedTag="FINDUSERS"
                                         Type="Secondary"
                                         Icon="search"/>
                        <YAF:ThemeButton runat="server" ID="AllUsers" 
                                         Size="Small" 
                                         OnClick="AllUsers_Click"
                                         TextLocalizedTag="ALLUSERS"
                                         Type="Secondary"
                                         Icon="users"/>
                        <YAF:ThemeButton runat="server" ID="AllBuddies" 
                                         Size="Small" 
                                         OnClick="AllBuddies_Click"
                                         TextLocalizedTag="ALLBUDDIES"
                                         Type="Secondary"
                                         Icon="user-friends"/>
                        <YAF:ThemeButton runat="server" ID="Clear" 
                                         Size="Small" 
                                         OnClick="Clear_Click" 
                                         Visible="false"
                                         TextLocalizedTag="CLEAR"
                                         Type="Secondary"
                                         Icon="times"/>
                        </div>
                        <asp:Label ID="MultiReceiverInfo" runat="server" Visible="false" />
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="PmSubjectTextBox">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="subject" />
                    </asp:Label>
                    <asp:TextBox ID="PmSubjectTextBox" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <asp:Label runat="server">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="message" />
                    </asp:Label>
                    <asp:PlaceHolder id="EditorLine" runat="server">
                        <!-- editor goes here -->
                    </asp:PlaceHolder>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="Preview" runat="server"
                                 TextLocalizedTag="PREVIEW" 
                                 OnClick="Preview_Click"
                                 Icon="image"
                                 Type="Secondary"/>
                <YAF:ThemeButton ID="Save" runat="server" 
                                 TextLocalizedTag="SAVE"
                                 OnClick="Save_Click"
                                 Icon="save"/>
                <YAF:ThemeButton ID="Cancel" runat="server"
                                 TextLocalizedTag="CANCEL"
                                 OnClick="Cancel_Click"
                                 Icon="times"
                                 Type="Secondary"/>
            </div>
        </div>
    </div>
</div>

</div>
</div>
<YAF:AttachmentsUploadDialog ID="UploadDialog" runat="server"></YAF:AttachmentsUploadDialog>