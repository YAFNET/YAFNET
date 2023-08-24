<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.ReportPost" CodeBehind="ReportPost.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card text-bg-light mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="comment"
                                LocalizedTag="REPORTPOST_TITLE"/>
            </div>
            <div class="card-body">
                <YAF:MessagePostData ID="MessagePreview" runat="server"
                                     ShowAttachments="false"
                                     ShowSignature="false" />
            </div>
            <div class="card-footer">
                <small class="text-body-secondary">
                    <YAF:LocalizedLabel ID="PostedByLabel" runat="server"
                                        LocalizedTag="POSTEDBY" />
                    <YAF:UserLink ID="UserLink1" runat="server"/>
                    <YAF:Icon runat="server"
                              IconName="calendar-day"
                              IconNameBadge="clock" />
                    <YAF:DisplayDateTime runat="server" ID="Posted"></YAF:DisplayDateTime>
                </small>
            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="exclamation-triangle"
                                LocalizedTag="HEADER"/>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <h6 class="card-subtitle mb-2 text-body-secondary">
                        <YAF:LocalizedLabel ID="EnterReportTextLabel" runat="server"
                                            LocalizedTag="ENTER_TEXT" />
                    </h6>
                    <asp:PlaceHolder id="EditorLine" runat="server">
                        <asp:TextBox runat="server" ID="Report"
                                     CssClass="form-control"
                                     required="required" />
                        <div class="invalid-feedback">
                            <YAF:LocalizedLabel runat="server"
                                                LocalizedTag="NEED_REASON" />
                        </div>
                    </asp:PlaceHolder>
                </div>
                <div class="d-sm-none d-md-block">
                    <YAF:Alert runat="server" Type="info" >
                        <YAF:Icon runat="server"
                                  IconName="info-circle" />
                        <YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server"
                                            LocalizedTag="MAXNUMBEROF"/>
                    </YAF:Alert>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="btnReport" runat="server"
                                 TextLocalizedTag="SEND" TitleLocalizedTag="SEND_TITLE"
                                 CausesValidation="True"
                                 OnClick="ReportClick"
                                 Icon="paper-plane" ReturnConfirmTag="CONFIRM_REPORTPOST"/>
                <YAF:ThemeButton ID="btnCancel" runat="server"
                                 TextLocalizedTag="CANCEL" TitleLocalizedTag="CANCEL_TITLE"
                                 OnClick="CancelClick"
                                 Type="Secondary"
                                 Icon="times"/>
            </div>
        </div>
    </div>
</div>