<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.mod_forumuser" Codebehind="mod_forumuser.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-secret fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                                 LocalizedTag="TITLE" />
            </div>
            <div class="card-body text-center">
                <form>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserName">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" />
                        </asp:Label>
                        <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                        <asp:DropDownList runat="server" ID="ToList"
                                          Visible="false" 
                                          CssClass="standardSelectMenu" />
                        <asp:Button runat="server" ID="FindUsers" 
                                    OnClick="FindUsers_Click" 
                                    CssClass="btn btn-secondary btn-sm" />
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="AccessMaskID">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCESSMASK" />
                        </asp:Label>
                        <asp:DropDownList runat="server" ID="AccessMaskID" CssClass="standardSelectMenu" />
                    </div>
                </form>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="Update"
                                 OnClick="Update_Click"
                                 TextLocalizedTag="UPDATE"
                                 Type="Primary"
                                 Icon="save"/>
                <YAF:ThemeButton runat="server" ID="Cancel"
                                 OnClick="Cancel_Click"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary"
                                 Icon="times"/>

            </div>
        </div>
    </div>
</div>