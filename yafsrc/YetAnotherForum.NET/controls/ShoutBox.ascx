<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShoutBox.ascx.cs" Inherits="YAF.Controls.ShoutBox" %>

<script type="text/javascript">
	function insertsmiley(code, path) {
		InsertSmileyForShoutBox(code, path);
	}

	function InsertSmileyForShoutBox(code, path) {
		InsertStringAtCurrentCursorPositionOrOverwriteSelectedText(document.getElementById('<%=messageTextBox.ClientID %>'), code)
	}

	function InsertStringAtCurrentCursorPositionOrOverwriteSelectedText(control, insertionText) {
		control.focus();
		if (control.value == '') {
			control.value += insertionText;
		}
		else
			control.value += ' ' + insertionText;
	}

	function openwin() {
		var hostname = window.location.hostname
		window.open("<%=YafForumInfo.ForumBaseUrl %>popup.aspx?g=shoutbox", "mywindow", "location=0,status=0,scrollbars=0,resizable=1,width=350,height=475");
		return false;
	}
</script>

<script type="text/javascript">

	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}

</script>

<asp:Panel ID="shoutBoxPanel" DefaultButton="btnButton" runat="server" Visible="false">
	<%--The Interval property is defined in milliseconds. 3000 milliseconds = 3 seconds--%>
	<asp:Timer ID="shoutBoxRefreshTimer" OnTick="ShoutBoxRefreshTimer_Tick" Interval="3000"
		runat="server">
	</asp:Timer>
	<asp:UpdatePanel ID="shoutBoxUpdatePanel" UpdateMode="conditional" runat="server">
		<ContentTemplate>
			<table border="0" class="content" cellspacing="1" cellpadding="0" width="100%">
				<tr>
					<td class="header1" colspan="2">
						<YAF:CollapsibleImage ID="CollapsibleImageShoutBox" runat="server" BorderWidth="0" Style="vertical-align: middle" DefaultState="Collapsed"
                        PanelID='ShoutBoxPanel' AttachedControlID="shoutBoxPlaceHolder" />&nbsp;&nbsp;					
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="SHOUTBOX" LocalizedTag="TITLE"></YAF:LocalizedLabel>
					</td>
				</tr>
				<asp:PlaceHolder ID="shoutBoxPlaceHolder" runat="server">
					<tr>
						<td class="header2" colspan="2">
							<span><YAF:LocalizedLabel ID="lblMemberchat" runat="server" LocalizedPage="SHOUTBOX" LocalizedTag="HEADING"></YAF:LocalizedLabel></span>
						</td>
					</tr>
					<tr>
						<td class="post" style="padding-left: 5px; margin: 0" colspan="2">
							<div class="post" style="overflow-y: scroll; height: 150px; width: 100%; padding: 0;
								margin: 0">
								<asp:UpdatePanel ID="shoutBoxChatUpdatePanel" UpdateMode="conditional" runat="server">
									<Triggers>
										<asp:AsyncPostBackTrigger ControlID="shoutBoxRefreshTimer" />
									</Triggers>
									<ContentTemplate>
										<asp:Repeater ID="shoutBoxRepeater" runat="server">
											<ItemTemplate>
												<div style="padding: 0; margin: 0">
													<i><asp:Label ID="dateLabel" runat="server" Text='<%# YafServices.DateTime.FormatDateTimeTopic( ((System.Data.DataRowView)Container.DataItem)["Date"] ) %>' /></i>
													<b><YAF:UserLink ID="UserLink1" runat="server" BlankTarget="true" UserName='<%# Convert.ToString(((System.Data.DataRowView)Container.DataItem)["UserName"]) %>' UserID='<%# Convert.ToInt32(((System.Data.DataRowView)Container.DataItem)["UserID"]) %>'></YAF:UserLink></b>: 
													<asp:Label ID="messageLabel" runat="server" Text='<%# ((System.Data.DataRowView)Container.DataItem)["Message"] %>' />
												</div>
											</ItemTemplate>
										</asp:Repeater>
									</ContentTemplate>
								</asp:UpdatePanel>
							</div>
						</td>
					</tr>
					<asp:PlaceHolder ID="phShoutText" runat="server" Visible="true">
						<tr id="shoutBoxFooter" runat="server">
							<td class="footer1" style="padding-left: 5px;">
								<asp:TextBox ID="messageTextBox" Width="99%" MaxLength="150" Visible="true" runat="server" />
							</td>
							<td class="footer1" style="text-align: center;white-space:nowrap;">
								<asp:Button ID="btnButton" OnClick="btnButton_Click" CssClass="pbutton" Text="Submit" Visible="true"
									runat="server" />
								<asp:Button ID="btnClear" OnClick="btnClear_Click" CssClass="pbutton" Text="Clear" Visible="false" runat="server" />
							</td>
						</tr>
						<tr>
							<%--<td colspan="2" class="post" style="overflow-y: scroll; height: 10px; width: 99%; padding: 0px 0px 0px 5px; margin: 0;">--%>
							<td class="post" style="padding-left: 5px; margin: 0;">
								<asp:Repeater ID="smiliesRepeater" runat="server">
									<ItemTemplate>
										<asp:ImageButton ID="ImageButton1" ImageUrl='<%# YafForumInfo.ForumRoot + "images/emoticons/" + Eval("Icon") %>'
											ToolTip='<%# Eval("Code") %>' OnClientClick='<%# FormatSmiliesOnClickString(Eval("Code").ToString(),Eval("Icon").ToString()) %>'
											runat="server" />
									</ItemTemplate>
								</asp:Repeater>
							</td>
							<td id="tdFlyOut" runat="server" class="post" style="text-align: center;">
								<asp:Button ID="btnFlyOut" OnClientClick="openwin()" CssClass="pbutton" Text="FlyOut" runat="server" />								
							</td>
						</tr>
					</asp:PlaceHolder>
				</asp:PlaceHolder>
			</table>
		</ContentTemplate>
	</asp:UpdatePanel>
	<br />
</asp:Panel>
