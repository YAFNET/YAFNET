<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Avatar" Codebehind="Avatar.ascx.cs" %>


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
                <YAF:IconHeader runat="server"
                                IconName="user-tie"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <asp:DataList runat="server" ID="directories" Width="100%" RepeatColumns="5" OnItemDataBound="Directories_Bind"
                              OnItemCommand="ItemCommand">
                    <ItemStyle BorderWidth="1" BorderStyle="Dotted" Width="20%" />
                    <ItemTemplate>
                        <asp:LinkButton ID="dirName" runat="server" CommandName="directory" />
                    </ItemTemplate>
                </asp:DataList>
                <asp:DataList runat="server" ID="files" Width="100%" RepeatColumns="5" OnItemDataBound="Files_Bind" OnItemCommand="ItemCommand">
                    <ItemStyle BorderWidth="1" BorderStyle="Dotted" CssClass="post" Width="20%" />
                    <HeaderTemplate>
                        <asp:LinkButton ID="up" runat="server" CommandName="directory" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Literal ID="fname" runat="server" />
                    </ItemTemplate>
                </asp:DataList>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="btnCancel" runat="server"
                                 TextLocalizedTag="CANCEL" TitleLocalizedTag="CANCEL_TITLE" 
                                 OnClick="BtnCancel_Click"
                                 Type="Secondary"
                                 Icon="reply"/>
            </div>
        </div>
    </div>
</div>