<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.restartapp" Codebehind="restartapp.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="TITLE" LocalizedPage="ADMIN_RESTARTAPP" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-sync fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                                              LocalizedTag="TITLE" 
                                                                              LocalizedPage="ADMIN_RESTARTAPP" />
                </div>
                <div class="card-body text-center">
                    <p class="card-text">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="INFO" 
                                            LocalizedPage="ADMIN_RESTARTAPP" />
                    </p>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="RestartApp" runat="server" 
                                     Type="Primary" 
                                     OnClick="RestartAppClick" 
                                     TextLocalizedTag="TITLE"
                                     TextLocalizedPage="ADMIN_RESTARTAPP"
                                     Icon="sync">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>

