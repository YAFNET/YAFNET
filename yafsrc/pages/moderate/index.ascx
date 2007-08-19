<%@ Control Language="c#" Codebehind="index.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.moderate.index" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="4">
                <%# GetText("MODERATE_DEFAULT", "FORUMS")%></td>
        </tr>
        <asp:Repeater ID="CategoryList" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="header2">
                        <%# Eval( "Name") %>
                    </td>
                    <td class="header2" width="15%" align="center">
                        <%# GetText("MODERATE_DEFAULT", "UNAPPROVED")%>
                    </td>
                    <td class="header2" width="15%" align="center">
                        <%# GetText("MODERATE_DEFAULT", "REPORTED")%>
                    </td>
                    <td class="header2" width="15%" align="center">
                        <%# GetText("MODERATE_DEFAULT", "REPORTEDSPAM")%>
                    </td>
                    
                </tr>
                <asp:Repeater ID="ForumList" runat="server" OnItemCommand="ForumList_ItemCommand"
                    DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
                    
                    <ItemTemplate>
                        <tr class="post">
                            <td align="left">
                                <b>
                                    <%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %>
                                </b>
                                <br />
                                <%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
                            </td>
                            <td align="center">
                                <asp:LinkButton ID="ViewUnapprovedPostsBtn" runat='server' CommandName='viewunapprovedposts' CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"MessageCount\"]") %>'></asp:LinkButton>
                            </td>
                            <td align="center">
                               <asp:LinkButton ID="ViewReportedPostsBtn" runat='server' CommandName='viewreportedposts' CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"ReportCount\"]") %>'></asp:LinkButton>
                            </td>
                            <td align="center">
                               <asp:LinkButton ID="ViewReportedSpamBtn" runat='server' CommandName='viewreportedspam' CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"SpamCount\"]") %>'></asp:LinkButton>
                            </td>
                            
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>

    </table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
