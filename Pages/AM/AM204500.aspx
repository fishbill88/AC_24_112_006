<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AM204500.aspx.cs" Inherits="Page_AM204500" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.AM.MachMaint" PrimaryView="Machines">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Machines" DataKeyNames="MachID" DefaultControlID="edMachID">
		<Template>
			<px:PXLayoutRule ID="LR1" runat="server" LabelsWidth="S" ControlSize="XM" Merge="true"/>
				<px:PXSelector Size="xm" ID="edMachID" runat="server" DataField="MachID" DataMember="_AMMach_" DataSourceID="ds" MaxLength="30" DataKeyNames="MachID" InputMask=">AAAAAAAAAA" />
				<px:PXCheckBox ID="chkActiveFlg" runat="server" DataField="ActiveFlg" />
			<px:PXLayoutRule ID="LR2" runat="server" Merge="true" />
				<px:PXTextEdit Size="xl" ID="edDescr" runat="server" AllowNull="False" DataField="Descr" MaxLength="60"  />
				<px:PXCheckBox ID="chkDownFlg" runat="server" DataField="DownFlg" />
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General" />
				<px:PXTextEdit ID="edAssetID" runat="server" DataField="AssetID"  />
				<px:PXFormView ID="curySettingsForm" runat="server" SkinID="Inside" RenderStyle="simple" DataSourceID="ds" DataMember="CurySettings_AMMach" CaptionVisible="false">
					<Template>
						<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="XM" />
						<px:PXNumberEdit ID="edStdCost" runat="server" DataField="StdCost"  />
					</Template>
				</px:PXFormView>
				<px:PXSelector ID="edCalendarID" runat="server" DataField="CalendarID" AllowEdit="True"/>
				<px:PXNumberEdit ID="edMachEff" runat="server" DataField="MachEff"  />
				<px:PXSegmentMask ID="edMachAcctID" runat="server" DataField="MachAcctID"/>
				<px:PXSegmentMask ID="edMachSubID" runat="server" CommitChanges="True" DataField="MachSubID" SelectMode="Segment"/>
			</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>
