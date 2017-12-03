<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.smilies" CodeBehind="smilies.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <asp:UpdatePanel ID="SmilesUpdatePanel" runat="server">
        <ContentTemplate>
                <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SMILIES" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <YAF:Pager ID="Pager" runat="server" />
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-smile-o fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SMILIES" />
                    </div>
                <div class="card-body">
                 <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
            <table class="table">
                <asp:Repeater runat="server" ID="List">
                    <HeaderTemplate>
                        <tr>
                            <thead>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ORDER" LocalizedPage="ADMIN_SMILIES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CODE" LocalizedPage="ADMIN_SMILIES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="SMILEY" LocalizedPage="ADMIN_SMILIES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="EMOTION" LocalizedPage="ADMIN_SMILIES" />
                            </th>
                            <th>
                                &nbsp;
                            </th>
                            </thead>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# this.Eval("SortOrder") %>
                            </td>
                            <td>
                                <%# this.Eval("Code") %>
                            </td>
                            <td>
                                <img src="<%# YafForumInfo.ForumClientFileRoot + YafBoardFolders.Current.Emoticons %>/<%# this.Eval("Icon") %>" alt="<%# this.Eval("Icon") %>" />
                            </td>
                            <td>
                                <%# this.Eval("Emoticon") %>
                            </td>
                            <td>
                                <span class="pull-right">
                                <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="btn btn-info btn-sm"
                                    CommandName='edit' CommandArgument='<%# this.Eval( "ID") %>'
                                    TitleLocalizedTag="EDIT"
                                    Icon="edit"
                                    TextLocalizedTag="EDIT"
                                    runat="server">
                                </YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButtonMoveUp" CssClass="btn btn-warning btn-sm"
                            CommandName='moveup' CommandArgument='<%# this.Eval("ID") %>'
					        TitleLocalizedTag="MOVE_UP"
                            TitleLocalizedPage="ADMIN_SMILIES"
					        Icon="level-up"
					        TextLocalizedTag="MOVE_UP"
                            TextLocalizedPage="ADMIN_SMILIES"
					        runat="server"/>
					    <YAF:ThemeButton ID="ThemeButtonMoveDown" CssClass="btn btn-warning btn-sm"
					        CommandName='movedown' CommandArgument='<%# this.Eval("ID") %>'
					        TitleLocalizedTag="MOVE_DOWN"
                            TitleLocalizedPage="ADMIN_SMILIES"
					        Icon="level-down"
					        TextLocalizedTag="MOVE_DOWN"
                            TextLocalizedPage="ADMIN_SMILIES"
					        runat="server" />

                                <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "ID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
                                    </span>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></div>
                </div>
                <div class="card-footer text-lg-center">
                                <asp:LinkButton runat="server" CommandName="add" CssClass="btn btn-primary" OnLoad="addLoad"> </asp:LinkButton>
                                &nbsp;
                                <asp:LinkButton runat="server" CommandName="import" CssClass="btn btn-info" OnLoad="importLoad"></asp:LinkButton>
                    </FooterTemplate>
                </asp:Repeater>                </div>
            </div>
             <YAF:Pager ID="Pager1" runat="server" LinkedPager="Pager" />
        </div>
    </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
