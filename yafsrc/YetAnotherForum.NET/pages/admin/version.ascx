<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version"
    CodeBehind="version.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
    <div class="row">
    <div class="col-xl-12">
         <h1><yaf:localizedlabel id="LocalizedLabel1" runat="server" localizedtag="TITLE" localizedpage="ADMIN_VERSION"></yaf:localizedlabel></h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header"> <i class="fa fa-info fa-fw"></i>&nbsp;
                <yaf:localizedlabel id="LocalizedLabel2"
                runat="server" localizedtag="TITLE" localizedpage="ADMIN_VERSION"></yaf:localizedlabel>
            </div>
            <div class="card-body">
                <asp:placeholder runat="server" id="UpgradeVersionHolder" visible="false">
                    <YAF:Alert runat="server" Type="info">
                        <yaf:localizedlabel id="Upgrade" runat="server" localizedtag="UPGRADE_VERSION"
                                            localizedpage="ADMIN_VERSION"></yaf:localizedlabel>
                    </YAF:Alert>
                </asp:placeholder>
                <p class="card-text">
                    <img src="~/Content/Images/YafLogoSmall.png" alt="YAF.NET" style="float: left; padding: 10px;"
                    runat="server" /></p>
                    <p class="card-text">
                        <asp:label id="RunningVersion" runat="server"></asp:label>
                    </p>
                    <p class="card-text">
                        <asp:label id="LatestVersion" runat="server"></asp:label>
                    </p>
            </div>
        </div>
    </div>
</div>

