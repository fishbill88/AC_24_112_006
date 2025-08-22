<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL406000.aspx.cs" Inherits="Page_GL406000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.GLAnomalyDetection.AnomalyTransactionEnq"
		PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="previousperiod" CommitChanges="True" HideText="True"/>
			<px:PXDSCallbackCommand Name="nextperiod" CommitChanges="True" HideText="True"/>
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="Reclassify" CommitChanges="True"/>
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="ReclassifyAll"/>
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="ReclassificationHistory" StateColumn="IncludedInReclassHistory"/>
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="MarkAsCorrect" CommitChanges="True"/>
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="UnMarkCorrected" CommitChanges="True"/>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Width="100%"
		Caption="Selection" DataMember="Filter" DefaultControlID="edLedgerID" DataSourceID="ds" TabIndex="100" MarkRequired="Dynamic">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXBranchSelector DataField="OrgBAccountID" ID="edOrgBAccountID" runat="server" CommitChanges="True"/>
			<px:PXSelector DataField="LedgerID" ID="edLedgerID" runat="server" CommitChanges="True" Autorefresh="true"/>
			<px:PXSelector DataField="PeriodID" ID="edPeriodID" runat="server" CommitChanges="True" AutoRefresh="True"/>
			<px:PXSegmentMask DataField="AccountID" ID="edAccountID" runat="server" CommitChanges="True"/>
			<px:PXSegmentMask DataField="SubID" ID="edSubID" runat="server" CommitChanges="True" SelectMode="Segment"/>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXNumberEdit ID="edScoreTreshold" runat="server" DataField="ScoreTreshold" AutoRefresh="True" CommitChanges="True"/>
			<px:PXNumberEdit ID="edAmountTreshold" runat="server" DataField="AmountTreshold" AutoRefresh="True" CommitChanges="True"/>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server"  Height="150px"
		Width="100%" AllowPaging="True" AdjustPageSize="Auto" Caption="Summary By Period" SyncPosition ="True" FastFilterFields="TranDesc,RefNbr,"
		AllowSearch="True" SkinID="PrimaryInquire" DataSourceID="ds" TabIndex="100" PreserveSortsAndFilters="False"
		OnRowDataBound="Tran_RowDataBound">
		<CallbackCommands>
			<Refresh RepaintControlsIDs="form"/>
		</CallbackCommands>
		<AutoSize Container="Window" Enabled="True" />
		<Mode AllowAddNew="False" AllowDelete="False"/>
		<Levels>
			<px:PXGridLevel DataMember="Transactions">
				<Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowShowHide="Server" AutoCallBack="True"/>
					<px:PXGridColumn DataField="MLStatusUI" />
					<px:PXGridColumn DataField="MLScore" LinkCommand="ViewSubspace" />
					<px:PXGridColumn DataField="Module" />
					<px:PXGridColumn DataField="BatchNbr" LinkCommand="ViewBatch" />
					<px:PXGridColumn DataField="TranDate" />
					<px:PXGridColumn DataField="TranDesc" />
					<px:PXGridColumn DataField="RefNbr" LinkCommand="ViewDocument" />
					<px:PXGridColumn DataField="SubID" />
					<px:PXGridColumn DataField="DebitAmt" TextAlign="Right" />
					<px:PXGridColumn DataField="CreditAmt" TextAlign="Right" />
					<px:PXGridColumn DataField="ReferenceID" />
					<px:PXGridColumn DataField="LineNbr" TextAlign="Right" />
					<px:PXGridColumn DataField="BranchID" />
					<px:PXGridColumn DataField="FinPeriodID" />
					<px:PXGridColumn DataField="TranPeriodID" />
					<px:PXGridColumn DataField="InventoryID" />
					<px:PXGridColumn DataField="ReclassBatchNbr" TextAlign="Right" AllowShowHide="Server" LinkCommand="ViewReclassBatch" />
					<px:PXGridColumn DataField="IncludedInReclassHistory" AllowShowHide="False" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="PMTranID" LinkCommand="ViewPMTran" />
				</Columns>
				<RowTemplate>
					<px:PXNumberEdit ID="edMLScore" runat="server" DataField="MLScore" AutoRefresh="True"/>
					<px:PXDateTimeEdit ID="edDRTermEndDate" runat="server" DataField="TranDate" CommitChanges="true" />
				</RowTemplate>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="400" />
		<ActionBar DefaultAction="DoubleClick" />
	</px:PXGrid>
	<px:PXSmartPanel ID="Subspaces" runat="server" Caption="Anomaly Scores" CaptionVisible="true" AcceptButtonID="Ok" AutoRepaint="true" AutoReload="true" Key="subspaces"
		Height="415px" Style="z-index: 108; position: absolute; left: 660px; top: 99px" Width="960px">
		<px:PXGrid ID="gridSubspaces" runat="server" Height="240px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px" AutoAdjustColumns="true" PageSize="200" SkinID="Inquire"
			StatusField="TextForSubspacesGrid">
			<Levels>
				<px:PXGridLevel DataMember="Subspaces">
					<Columns>
						<px:PXGridColumn DataField="MLScore" TextAlign="Right" />
						<px:PXGridColumn DataField="SubspaceDescr" />
						<px:PXGridColumn DataField="AnomalyType" />
						<px:PXGridColumn DataField="Reason" />
					</Columns>
					<Mode AllowAddNew="false" AllowUpdate="false" AllowDelete="false" />
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="true" />
		</px:PXGrid>
	</px:PXSmartPanel>
</asp:Content>