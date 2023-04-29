<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Dialogs.QuickReply" CodeBehind="QuickReply.ascx.cs" %>

<div class="modal fade" id="QuickReplyDialog" tabindex="-1" role="dialog" aria-labelledby="quickReplyLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
          <h5 class="modal-title" id="myModalLabel">
              <%= this.Get<ILocalization>().GetText("QUICKREPLY") %>
          </h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
          </button>
      </div>
      <div class="modal-body">
          <div id="QuickReplyLine" runat="server">
          </div>
          <div>
              <asp:PlaceHolder runat="server" ID="QuickReplyWatchTopic">
                  <div class="form-check mt-3">
                      <asp:CheckBox ID="TopicWatch" runat="server"/>
                      <asp:Label runat="server" CssClass="custom-control-label" 
                                 AssociatedControlID="TopicWatch">
                          <YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" 
                                              LocalizedTag="TOPICWATCH" />
                      </asp:Label>
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
