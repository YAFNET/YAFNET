<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Dialogs.QuickReply" CodeBehind="QuickReply.ascx.cs" %>


<div class="modal fade" id="QuickReplyDialog" tabindex="-1" role="dialog" aria-labelledby="quickReplyLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
          <h5 class="modal-title" id="myModalLabel"><%= this.Get<ILocalization>().GetText("QUICKREPLY") %></h5>
          <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
          <div id="QuickReplyLine" runat="server">
          </div>
          <div id="CaptchaDiv" visible="false" runat="server">
              <div class="form-group">
                  <label for="<%= this.imgCaptcha.ClientID %>">
                      <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="Captcha_Image" />
                  </label>
                  <asp:Image ID="imgCaptcha" runat="server" AlternateText="Captcha" CssClass="form-control" />
              </div>
              <div class="form-group">
                  <label for="<%= this.tbCaptcha.ClientID %>">
                      <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="Captcha_Enter" />
                  </label>
                  <asp:TextBox ID="tbCaptcha" runat="server" CssClass="form-control" />
              </div>
          </div>
          <div>
              <asp:PlaceHolder runat="server" ID="QuickReplyWatchTopic">
                  <div class="custom-control custom-checkbox">
                      <asp:CheckBox ID="TopicWatch" runat="server" CssClass="custom-control-input" />
                      <label class="custom-control-label" for='<%= this.TopicWatch.ClientID %>'>
                          <YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" LocalizedTag="TOPICWATCH" />
                      </label>
                  </div>
              </asp:PlaceHolder>
          </div>
      </div>
      <div class="modal-footer">
          <YAF:ThemeButton runat="server" ID="Reply"
                           Type="Primary"
                           OnClick="QuickReplyClick"
                           TextLocalizedTag="SAVE" TextLocalizedPage="POSTMESSAGE"
                           Icon="reply">
          </YAF:ThemeButton>
      </div>
    </div>
  </div>
</div>
