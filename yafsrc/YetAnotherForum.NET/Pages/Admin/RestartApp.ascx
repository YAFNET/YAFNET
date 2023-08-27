<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.RestartApp" Codebehind="RestartApp.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconType="text-secondary"
                                    IconName="sync"
                                    LocalizedPage="ADMIN_RESTARTAPP"></YAF:IconHeader>
                </div>
                <div class="card-body text-center">
                    <p class="card-text">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="INFO" 
                                            LocalizedPage="ADMIN_RESTARTAPP" />
                    </p>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Restart" runat="server" 
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

