<px:PXSmartPanel ID="panelTeamsContact" runat="server"  Caption="Teams Contact" Width="400px" Height="290px" AutoRepaint="true"
	CaptionVisible="true" Key="TeamsContactCard" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" 
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
	<px:PXFormView ID="formTeamsContact" runat="server" DataSourceID="ds" DataMember="TeamsContactCard" AllowCollapse="false" Width="100%" >
		<ContentStyle BackColor="Transparent" BorderStyle="None" />
		<Template>
			<px:PXImageView ID="edTeamsMemberPhoto" runat="server" DataField="PhotoFileName" Width="64" Height="64" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" />
			<px:PXTextEdit ID="edDisplayName" runat="server" DataField="DisplayName" SuppressLabel="True" Width="190px" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="XXS" ControlSize="XXS" />
			<px:PXButton ID="edStatusIconContactOffline" runat="server" CommandSourceID="ds" CommandName="StatusIconContactOffline" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@offline" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconContactAvailable" runat="server" CommandSourceID="ds" CommandName="StatusIconContactAvailable" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@available" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconContactBusy" runat="server" CommandSourceID="ds" CommandName="StatusIconContactBusy" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@busy" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconContactAway" runat="server" CommandSourceID="ds" CommandName="StatusIconContactAway" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@away" />
			</px:PXButton>
			<px:PXTextEdit ID="edStatus" runat="server" DataField="TeamsStatus"  ControlSize="M" CssClass="teamsStatusLabel" SuppressLabel="True" Width="132px" />
			<px:PXLayoutRule runat="server" Merge="False" StartRow="True" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edJobTitle" runat="server" DataField="JobTitle" Style="margin-top:20px" />
			<px:PXTextEdit ID="edCompanyName" runat="server" DataField="CompanyName" />
			<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" />
			<px:PXTextEdit ID="edMobilePhone" runat="server" DataField="MobilePhone" />
		</Template>
	</px:PXFormView>

	<div style="padding-left:74px; text-align: center;">
		<px:PXButton ID="edContactChat" runat="server" CommandSourceID="ds" CommandName="ContactChat" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Chat">
			<Images Normal="svg:teams@teams_chat" />
		</px:PXButton>
		<px:PXButton ID="edContactCall" runat="server" CommandSourceID="ds" CommandName="ContactCall" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Call">
			<Images Normal="svg:teams@teams_call" />
		</px:PXButton>
		<px:PXButton ID="edContactMeeting" runat="server" CommandSourceID="ds" CommandName="ContactMeeting" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Meeting">
			<Images Normal="svg:teams@teams_event" />
		</px:PXButton>
	</div>
</px:PXSmartPanel>

<px:PXSmartPanel ID="panelTeamsOwner" runat="server"  Caption="Teams Contact" Width="400px" Height="290px" AutoRepaint="true"
	CaptionVisible="true" Key="TeamsOwnerCard" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" 
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
	<px:PXFormView ID="formTeamsOwner" runat="server" DataSourceID="ds" DataMember="TeamsOwnerCard" AllowCollapse="false" Width="100%" >
		<ContentStyle BackColor="Transparent" BorderStyle="None" />
		<Template>
			<px:PXImageView ID="edTeamsMemberPhoto" runat="server" DataField="PhotoFileName" Width="64" Height="64" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" />
			<px:PXTextEdit ID="edDisplayName" runat="server" DataField="DisplayName" SuppressLabel="True" Width="190px" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="XXS" ControlSize="XXS" />
			<px:PXButton ID="edStatusIconOwnerOffline" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerOffline" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@offline" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconOwnerAvailable" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerAvailable" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@available" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconOwnerBusy" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerBusy" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@busy" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconOwnerAway" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerAway" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@away" />
			</px:PXButton>
			<px:PXTextEdit ID="edStatus" runat="server" DataField="TeamsStatus" ControlSize="M" CssClass="teamsStatusLabel" SuppressLabel="True" Width="132px" />
			<px:PXLayoutRule runat="server" Merge="False" StartRow="True" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edJobTitle" runat="server" DataField="JobTitle" Style="margin-top:20px" />
			<px:PXTextEdit ID="edCompanyName" runat="server" DataField="CompanyName" />
			<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" />
			<px:PXTextEdit ID="edMobilePhone" runat="server" DataField="MobilePhone" />
		</Template>
	</px:PXFormView>

	<div style="padding-left:74px; text-align: center;">
		<px:PXButton ID="edOwnerChat" runat="server" CommandSourceID="ds" CommandName="OwnerChat" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Chat">
			<Images Normal="svg:teams@teams_chat" />
		</px:PXButton>
		<px:PXButton ID="edOwnerCall" runat="server" CommandSourceID="ds" CommandName="OwnerCall" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Call">
			<Images Normal="svg:teams@teams_call" />
		</px:PXButton>
		<px:PXButton ID="edOwnerMeeting" runat="server" CommandSourceID="ds" CommandName="OwnerMeeting" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Meeting">
			<Images Normal="svg:teams@teams_event" />
		</px:PXButton>
	</div>
</px:PXSmartPanel>