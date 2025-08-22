<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AU205200.aspx.cs" Inherits="Page_AU205200"
	 %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXLabel runat="server" Text="Access Rights" CssClass="projectLink transparent border-box" />

	<px:PXDataSource ID="ds" runat="server" Width="100%" TypeName="PX.SM.ProjectRoleMaintenance" PrimaryView="Items" Visible="true">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">

	<px:PXGrid ID="grid" runat="server" Width="100%"
		SkinID="Primary" AutoAdjustColumns="True" SyncPosition="True" FilesIndicator="False" NoteIndicator="False">
		<AutoSize Enabled="true" Container="Window" />
		<Mode AllowAddNew="False" />
		<ActionBar Position="Top" ActionsVisible="false">
			<Actions>
				<NoteShow MenuVisible="False" ToolBarVisible="False" />
				<AddNew MenuVisible="False" ToolBarVisible="False" />
				<ExportExcel MenuVisible="False" ToolBarVisible="False" />
				<AdjustColumns ToolBarVisible="False"/>
			</Actions>
		</ActionBar>
		<Levels>
			<px:PXGridLevel DataMember="Items">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXTextEdit SuppressLabel="True" Height="100%" runat="server" ID="edSource" TextMode="MultiLine"
						DataField="Content" Font-Size="10pt" Font-Names="Courier New" Wrap="False" SelectOnFocus="False">
						<AutoSize Enabled="True" />
					</px:PXTextEdit>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="Name" Width="108px"/>
					<px:PXGridColumn DataField="Description" Width="108px" />
                    <px:PXGridColumn DataField="AccessRightsMergeRule" Width="50px" Type="DropDownList"/>
					<px:PXGridColumn AllowUpdate="False" DataField="LastModifiedByID_Modifier_Username" Width="108px" />
					<px:PXGridColumn AllowUpdate="False" DataField="LastModifiedDateTime" Width="90px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>
	
	
	<px:PXSmartPanel runat="server" ID="ViewBaseMethod" Width="500px" Height="270px" CaptionVisible="True"
                     Caption="Add Access Rights for Screen" ShowMaximizeButton="True" Overflow="Hidden" Key="ScreensWithRoles" 
                     AutoRepaint="True" AllowResize="false" AcceptButtonID="btnAdd" CallBackMode-CommitChanges="True" 
                     CallBackMode-PostData="Page" CommandSourceID="ds">
        <px:PXFormView ID="formAddAccessRights" runat="server" DataSourceID="ds" DataMember="ScreensWithRoles" AllowCollapse="False" 
                       SkinID="Transparent" Style="z-index: 100" Width="100%">
            <Template>
                <px:PXSelector ID="edScreenID" runat="server" DataField="ScreenID" DataSourceID="ds" DisplayMode="Value"/>
                <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Merge Rule"/>
                <px:PXGroupBox ID="gbMergeRule" runat="server" CommitChanges="True" DataField="AccessRightsMergeRule" RenderSimple="False" 
                               RenderStyle="Simple" >
                    <Template>
                        <px:PXRadioButton ID="rbGrantAll" runat="server" GroupName="gbMergeRule" Value="GrantAll"/>
                        <px:PXRadioButton ID="rbRevokeAll" runat="server" GroupName="gbMergeRule" Value="RevokeAll"/>
                        <px:PXRadioButton ID="rbApplyReset" runat="server" GroupName="gbMergeRule" Value="ApplyAndReset"/>
                        <px:PXRadioButton ID="rbApplyKeep" runat="server" GroupName="gbMergeRule" Value="ApplyAndKeep" Checked="True"/>
                    </Template>
                    <ContentLayout LabelsWidth="S" Layout="Stack" SpacingSize="Medium"/>
                </px:PXGroupBox>	
	        
                 <px:PXPanel ID="pnlAddAccessRightsButtons" runat="server" SkinID="Buttons">
                     <px:PXButton ID="btnAdd" runat="server" DialogResult="OK" Text="Add" />
                     <px:PXButton ID="btnCancel" runat="server" DialogResult="No" Text="Cancel" CausesValidation="False" />
                </px:PXPanel>
		    </Template>
	    </px:PXFormView>
	</px:PXSmartPanel>
</asp:Content>
