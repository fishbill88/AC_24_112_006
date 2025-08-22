<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM404000.aspx.cs" Inherits="Page_SM404000" Title="Teams" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.MSTeams.Graph.SM.SMImportSharedFilesInq" PrimaryView="SharedFiles">
        <CallbackCommands>
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
        <px:PXFormView ID="formFilter" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" >
            <Template>
                <px:PXLayoutRule runat="server" LabelsWidth="XS" ControlSize="XXL" />
				<px:PXTextEdit runat="server" ID="edFileName" DataField="FileName" CommitChanges="True" />
            </Template>
        </px:PXFormView>
		<px:PXGrid ID="gridFiles" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="100%" SkinID="Inquire" 
				   NotesIndicator="False" FastFilterID="edFileName" FastFilterFields="FileName" FilesIndicator="False" SyncPosition="true"
				   AllowPaging="True" AllowSearch="true" AdjustPageSize="Auto">
		<ActionBar>
			<Actions>
				<AdjustColumns MenuVisible="False" ToolBarVisible="false" />
				<ExportExcel MenuVisible="False" ToolBarVisible="false" />
				<Refresh MenuVisible="False" ToolBarVisible="False" />
			</Actions>
		</ActionBar>
		<AutoSize Container="Window" Enabled="True"  />
		<Levels>
			<px:PXGridLevel DataMember="SharedFiles">
				<Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="80px" CommitChanges="False" />
					<px:PXGridColumn DataField="FileName" Width="280px" />
					<px:PXGridColumn DataField="LastModifiedDateTime" Width="100px" />
					<px:PXGridColumn DataField="TeamPhoto" AllowFocus="False" DisplayMode="Value" TextAlign="Center" Type="Icon" Width="64px" />
					<px:PXGridColumn DataField="SharedByName" Width="200px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>
</asp:Content>