<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.smilies_edit" CodeBehind="smilies_edit.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SMILIES_EDIT" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-smile-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SMILIES_EDIT" />
</div>
                <div class="card-body">
            <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CODE" LocalizedPage="ADMIN_SMILIES_EDIT" />
                </h4>
            <p>
                <asp:TextBox ID="Code" runat="server" CssClass="form-control" /></p>
            <hr />
            <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ICON" LocalizedPage="ADMIN_SMILIES_EDIT" />
            </h4>
            <p>
                <YAF:ImageListBox ID="Icon" AutoPostBack="true" OnTextChanged="ChangePreview" runat="server" CssClass="selectpicker" />
            </p>
            <hr />
            <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="EMOTION" LocalizedPage="ADMIN_SMILIES_EDIT" />
            </h4>
            <p>
                <asp:TextBox ID="Emotion" runat="server" CssClass="form-control" /></p>
            <hr />
            <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_SMILIES_EDIT" />
            </h4>
            <p>
                <asp:TextBox ID="SortOrder" runat="server" Text="0" MaxLength="3" CssClass="form-control" TextMode="Number" /></p>
                </div>
                <div class="card-footer text-lg-center">
                <asp:LinkButton ID="save" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="save_Click" />&nbsp;
                <asp:LinkButton ID="cancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="cancel_Click"  />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
