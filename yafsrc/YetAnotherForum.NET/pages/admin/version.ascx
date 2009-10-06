<%@ Control Language="c#" CodeFile="version.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="adminmenu1" runat="server">
    <table width="100%" cellspacing="0" cellpadding="0" class="content">
        <tr>
            <td class="post">
                <table width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td class="header1">
                            Version Check
                        </td>
                    </tr>
                    <tr class="post">
                        <td>
                            <p>
                                You are running YetAnotherForum.NET version <b>
                                    <%#YafForumInfo.AppVersionName%></b> (Date: <%#YafServices.DateTime.FormatDateShort( YafForumInfo.AppVersionDate ) %>).</p>
                            <p>
                                The latest final version available is <b>
                                    <%#LastVersion%></b> released <b>
                                        <%#LastVersionDate%></b> .</p>
                            <p runat="server" id="Upgrade" visible="false">
                                You can download the latest version from <a target="_top" href="http://sourceforge.net/project/showfiles.php?group_id=90539">
                                    here</a>.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
