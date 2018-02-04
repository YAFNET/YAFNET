<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editgroup"
    CodeBehind="editgroup.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITGROUP" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITGROUP" />
                </div>
                <div class="card-body">
            <h4>
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="ROLE_NAME" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox CssClass="form-control" ID="Name" runat="server" />
                <asp:RequiredFieldValidator ID="postNameRequired" runat="server" Display="Dynamic" ControlToValidate="Name" ErrorMessage="Role name is required."></asp:RequiredFieldValidator>
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:CheckBox ID="IsStartX" runat="server" CssClass="form-control"></asp:CheckBox>
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="FORUM_MOD" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:CheckBox ID="IsModeratorX" runat="server" CssClass="form-control"></asp:CheckBox>
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="IS_ADMIN" LocalizedPage="ADMIN_EDITGROUP" />                
            </h4>
            <p>
                <asp:CheckBox ID="IsAdminX" runat="server" CssClass="form-control"></asp:CheckBox>
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="PMMESSAGES" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox ID="PMLimit" Text="0" runat="server" CssClass="form-control" TextMode="Number" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox  CssClass="form-control" ID="Description" runat="server" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="SIGNATURE_LENGTH" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox ID="UsrSigChars" runat="server"  Text="128" CssClass="form-control" TextMode="Number" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODES" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox CssClass="form-control" ID="UsrSigBBCodes" runat="server" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITGROUP" />                
            </h4>
            <p>
                <asp:TextBox CssClass="form-control" ID="UsrSigHTMLTags" runat="server" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="ALBUM_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox ID="UsrAlbums" runat="server" Text="0" CssClass="form-control" TextMode="Number" />
           </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox ID="UsrAlbumImages" runat="server" Text="0" CssClass="form-control" TextMode="Number" />
           </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="PRIORITY" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:TextBox ID="Priority" MaxLength="5" Text="0" runat="server" CssClass="form-control" TextMode="Number" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="STYLE" LocalizedPage="ADMIN_EDITGROUP" />            
            </h4>
            <p>
                <asp:TextBox ID="StyleTextBox" TextMode="MultiLine" runat="server" CssClass="form-control" />
            </p><hr />
        <asp:PlaceHolder runat="server" visible="false" id="IsGuestTR">
            <h4>
                <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="IS_GUEST" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
         <asp:CheckBox ID="IsGuestX" runat="server" CssClass="form-control"></asp:CheckBox>
                </p><hr />
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" id="NewGroupRow">
            <h4>
                <YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="INITIAL_MASK" LocalizedPage="ADMIN_EDITGROUP" />
            </h4>
            <p>
                <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindDataAccessMaskId" CssClass="custom-select" />
            </p>
        </asp:PlaceHolder>
        <asp:Repeater ID="AccessList" runat="server">
            <ItemTemplate>
                
                    <h4>
                        <asp:Label ID="ForumID" Visible="false" runat="server" Text='<%# this.Eval( "ForumID") %>'></asp:Label><%# this.Eval( "ForumName") %></h4>
                        <small><em>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="BOARD"  LocalizedPage="ADMIN_EDITGROUP" />
                        <%# this.Eval( "BoardName") %>
                        <br />
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="CATEGORY"  LocalizedPage="ADMIN_EDITGROUP" />
                        <%# this.Eval( "CategoryName") %>
                        </em></small>
                    <p>
                        <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindDataAccessMaskId" CssClass="custom-select"
                            OnPreRender="SetDropDownIndex" value='<%# this.Eval("AccessMaskID") %>' />
                        ...
                    </p>
                    <hr />
              
            </ItemTemplate>
        </asp:Repeater>
               
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="Save" runat="server" OnClick="SaveClick" Type="Primary"
                                    Icon="save" TextLocalizedTag="SAVE"></YAF:ThemeButton>&nbsp;
                    <YAF:ThemeButton ID="Cancel" runat="server" OnClick="CancelClick" Type="Secondary"
                                    Icon="times" TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
