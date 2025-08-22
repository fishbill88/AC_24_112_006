<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM200400.aspx.cs" Inherits="Page_SM200400" Title="Archival Policy" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Width="100%" PrimaryView="Setup" TypeName="PX.Data.Archiving.ArchivalPolicyMaint" Visible="True"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Setup">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XM" ControlSize="XXS" />
			<px:PXNumberEdit ID="edDuration" runat="server" DataField="ArchivingProcessDurationLimitInHours" CommitChanges="True"/>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" AllowPaging="True" AllowSearch="True" DataSourceID="ds" SkinID="Details">
		<Levels>
			<px:PXGridLevel DataMember="Policies">
				<Columns>
					<px:PXGridColumn DataField="TableName" CommitChanges="true" />
					<px:PXGridColumn DataField="RetentionPeriodInMonths" CommitChanges="true"/>
				</Columns>
				<RowTemplate>
					<px:PXNumberEdit runat="server" ID="edRetentionPeriodInMonths" DataField="RetentionPeriodInMonths" CommitChanges="true"/>
				</RowTemplate>
			</px:PXGridLevel>
		</Levels>
		<ActionBar>
			<Actions>
				<NoteShow Enabled="False" />
				<Upload Enabled="false" />
			</Actions>
		</ActionBar>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
		<Mode AllowUpload="True" />
	</px:PXGrid>
</asp:Content>
