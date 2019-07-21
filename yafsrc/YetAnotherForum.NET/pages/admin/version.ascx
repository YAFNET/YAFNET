<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version"
    CodeBehind="version.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />
    <div class="row">
    <div class="col-xl-12">
         <h1><yaf:LocalizedLabel id="LocalizedLabel1" runat="server" 
                                 LocalizedTag="TITLE" 
                                 Localizedpage="ADMIN_VERSION"></yaf:LocalizedLabel></h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header"> <i class="fa fa-info fa-fw"></i>&nbsp;
                <yaf:LocalizedLabel id="LocalizedLabel2"
                runat="server" localizedtag="TITLE" localizedpage="ADMIN_VERSION"></yaf:LocalizedLabel>
            </div>
            <div class="card-body">
                <asp:placeholder runat="server" id="UpgradeVersionHolder" visible="false">
                    <YAF:Alert runat="server" Type="info">
                        <yaf:LocalizedLabel id="Upgrade" runat="server" localizedtag="UPGRADE_VERSION"
                                            localizedpage="ADMIN_VERSION"></yaf:LocalizedLabel>
                    </YAF:Alert>
                </asp:placeholder>
                <p class="card-text">
                    <img src="~/Content/Images/YafLogoSmall.png" alt="YAF.NET" class="p-5 float-left"
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

