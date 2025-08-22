<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GL301000.aspx.cs" Inherits="Page_GL301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" EnableAttributes="true" UDFTypeField="Module" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.GL.JournalEntry" PrimaryView="BatchModule" HeaderDescriptionField="Description">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="CurrencyView" Visible="False" />
			<px:PXDSCallbackCommand Visible="false" Name="BatchRegisterDetails" />
			<px:PXDSCallbackCommand Visible="false" Name="GLEditDetails" />
            <px:PXDSCallbackCommand Visible="false" Name="GLReversingBatches" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewVoucherBatch" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewWorkBook" />
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewDocument" Visible="false" />
            <px:PXDSCallbackCommand DependOnGrid="grid" Name="ReclassificationHistory" Visible="false"/>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Width="100%" DataMember="BatchModule" Caption="Batch Summary" NoteIndicator="True" FilesIndicator="True" ActivityIndicator="True" ActivityField="NoteActivity" LinkIndicator="True"
		BPEventsIndicator="True" DefaultControlID="edModule" DataSourceID="ds" TabIndex="18100" >
		<Parameters>
			<px:PXQueryStringParam Name="Batch.module" QueryStringField="Module" Type="String" OnLoadOnly="True" />
		</Parameters>
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
			<px:PXDropDown ID="edModule" runat="server" DataField="Module"/>
			<px:PXSelector ID="edBatchNbr" runat="server" DataField="BatchNbr" AutoRefresh="True" DataSourceID="ds" />
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" />
			<px:PXDateTimeEdit ID="edDateEntered" runat="server" DataField="DateEntered" OnValueChange="Commit" />
			<px:PXSelector ID="edFinPeriodID" runat="server" DataField="FinPeriodID" OnValueChange="Commit" DataSourceID="ds" AutoRefresh="True"/>
			<px:PXLabel runat="server"/> 
		    <px:PXLayoutRule runat="server" ColumnSpan="3"/>
			<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
			

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXSegmentMask ID="edBranchID" runat="server" DataField="BranchID" OnValueChange="Commit" DataSourceID="ds" AutoRefresh="True"/>
            <px:PXSelector ID="edLedgerID" runat="server" DataField="LedgerID" OnValueChange="Commit" DataSourceID="ds" AutoRefresh="True"/>
            <pxa:PXCurrencyRate ID="edCury" runat="server" DataMember="_Currency_" 
                DataField="CuryID" RateTypeView="_Batch_CurrencyInfo_" DataSourceID="ds"></pxa:PXCurrencyRate>
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="S"  Merge="True"/>
            <px:PXCheckBox ID="chkAutoReverse" runat="server" DataField="AutoReverse" OnValueChange="Commit" />
			<px:PXCheckBox ID="AutoReverseCopy" runat="server" DataField="AutoReverseCopy"  OnValueChange="Commit"/>
			<px:PXLayoutRule runat="server" />
            <px:PXCheckBox ID="chkCreateTaxTrans" runat="server" CommitChanges="True" DataField="CreateTaxTrans"/>			
            <px:PXCheckBox ID="chkSkipTaxValidation" runat="server" CommitChanges="True" DataField="SkipTaxValidation"/>
             

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
            <px:PXDropDown ID="edBatchType" runat="server" DataField="BatchType" Enabled="False"/>
			<px:PXSelector ID="edOrigBatchNbr" runat="server" DataField="OrigBatchNbr" Enabled="False" AllowEdit="True" DataSourceID="ds" />
            <px:PXNumberEdit ID="edReverseCount" runat="server" DataField="ReverseCount" Enabled="False">
                <LinkCommand Target="ds" Command="GLReversingBatches"></LinkCommand>
            </px:PXNumberEdit>
			<px:PXNumberEdit ID="edCuryDebitTotal" runat="server" DataField="CuryDebitTotal" Enabled="False" Size="SM" />
			<px:PXNumberEdit ID="edCuryCreditTotal" runat="server" DataField="CuryCreditTotal" Enabled="False" Size="SM" />
			<px:PXNumberEdit ID="edCuryControlTotal" runat="server" DataField="CuryControlTotal" Size="SM" OnValueChange="Commit"/>
            
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server"  Style="z-index: 100;" Width="100%" TabIndex="200">
		<Items>
			<px:PXTabItem Text="Details">
				<Template>
                    	<px:PXGrid ID="grid" runat="server" Width="100%" Caption="Transaction Details" SkinID="Details" SyncPosition="True" TabIndex="200">
						<Levels>
							<px:PXGridLevel DataMember="GLTranModuleBatNbr">
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
									<px:PXSegmentMask ID="edBranchID" runat="server" DataField="BranchID" OnValueChange="Commit" AutoRefresh ="true"/>
									<px:PXSegmentMask ID="edAccountID" runat="server" DataField="AccountID" AutoRefresh="True" OnValueChange="Commit" />
									<px:PXSegmentMask ID="edSubID" runat="server" DataField="SubID" AutoRefresh="True" OnValueChange="Commit" />
									<px:PXTextEdit ID="edRefNbr" runat="server" DataField="RefNbr" Size="S" />
									<px:PXDateTimeEdit ID="edTranDate" runat="server" DataField="TranDate" Enabled="False" Size="S" />
									<px:PXSelector ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True" OnValueChange="Commit" Size="S" />
									<px:PXNumberEdit ID="edQty" runat="server" DataField="Qty" Size="S" />
									<px:PXNumberEdit ID="edCuryDebitAmt" runat="server" DataField="CuryDebitAmt" Size="S" />
									<px:PXNumberEdit ID="edCuryCreditAmt" runat="server" DataField="CuryCreditAmt" Size="S" />
									<px:PXLayoutRule runat="server" ColumnSpan="2" />
									<px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" />
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
									<px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" CommitChanges="True" />
									<px:PXSegmentMask ID="edTaskID" runat="server" DataField="TaskID" CommitChanges="True" AutoRefresh="True" />
									<px:PXSegmentMask ID="edCostCode" runat="server" DataField="CostCodeID" CommitChanges="True" AutoRefresh="True" />
									<px:PXSelector ID="edReferenceID" runat="server" DataField="ReferenceID" Enabled="False"/>
									<px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" Enabled="False" />
									<px:PXSegmentMask ID="edLedger" runat="server" DataField="LedgerID" AllowEdit="True" Enabled="False"/>
									<px:PXSelector ID="edTaxID" runat="server" DataField="TaxID" AutoRefresh="True" OnValueChange="Commit" Size="S" />                    
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="LineNbr" TextAlign="Right" />
									<px:PXGridColumn DataField="BranchID" AutoCallBack="True" />
									<px:PXGridColumn DataField="AccountID" AutoCallBack="True" />
									<px:PXGridColumn DataField="AccountID_Account_description" />
									<px:PXGridColumn DataField="SubID" AutoCallBack="True"/>
									<px:PXGridColumn DataField="FinPeriodID"/>
									<px:PXGridColumn DataField="TranPeriodID"/>
									<px:PXGridColumn DataField="ProjectID" AutoCallBack="true" />
									<px:PXGridColumn DataField="TaskID" Label="Task" AutoCallBack="True" />
									<px:PXGridColumn DataField="CostCodeID" Label="Task" AutoCallBack="True" />
									<px:PXGridColumn DataField="PMTranID" LinkCommand="ViewPMTran" />
									<px:PXGridColumn DataField="RefNbr" />
									<px:PXGridColumn DataField="TranDate" DisplayFormat="d" />
									<px:PXGridColumn DataField="Qty" TextAlign="Right" />
									<px:PXGridColumn DataField="UOM" AutoCallBack="True" />
									<px:PXGridColumn DataField="CuryDebitAmt" TextAlign="Right" />
									<px:PXGridColumn DataField="CuryCreditAmt" TextAlign="Right" />
									<px:PXGridColumn DataField="TranDesc" />
									<px:PXGridColumn DataField="InventoryID" />
									<px:PXGridColumn DataField="LedgerID" AllowShowHide="True" Visible="False" SyncVisible="False" />
									<px:PXGridColumn DataField="ReferenceID" />
									<px:PXGridColumn DataField="TaxID" AutoCallBack="True" AllowShowHide="Server"/>
									<px:PXGridColumn DataField="TaxCategoryID" AutoCallBack="True" AllowShowHide="Server"/>
									<px:PXGridColumn DataField="NonBillable" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="CuryReclassRemainingAmt" TextAlign="Right" AllowShowHide="Server" />
									<px:PXGridColumn DataField="ReclassBatchNbr" TextAlign="Right" LinkCommand="ViewReclassBatch" AllowShowHide="Server" SyncVisibility="false" />
                     
									<px:PXGridColumn DataField="OrigModule"  AllowShowHide="Server"/>
									<px:PXGridColumn DataField="OrigBatchNbr" LinkCommand="viewOrigBatch" AllowShowHide="Server"/>
									<px:PXGridColumn DataField="OrigLineNbr" TextAlign="Right" AllowShowHide="Server" />
                    
									<px:PXGridColumn DataField="IncludedInReclassHistory" AllowShowHide="False" Visible="false" SyncVisible="false" />
								</Columns>
								<Layout FormViewHeight=""></Layout>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" />
						<Mode InitNewRow="True" AllowFormEdit="True" AllowUpload="True" />
						<Layout FormViewHeight="400px" />
						<LevelStyles>
							<RowForm Height="159px" />
						</LevelStyles>
						<ActionBar PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Text="View Source Document">
									<AutoCallBack Command="ViewDocument" Target="ds" />
								</px:PXToolBarButton>
								 <px:PXToolBarButton Text="Reclassification History" StateColumn="IncludedInReclassHistory">
									<AutoCallBack Command="ReclassificationHistory" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Approvals" BindingContext="form" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True" Style="left: 0; top: 0;">
						<AutoSize Container="Window" Enabled="True" MinHeight="150" />
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						<Levels>
							<px:PXGridLevel DataMember="Approval">
								<Columns>
									<px:PXGridColumn DataField="ApproverEmployee__AcctCD" />
									<px:PXGridColumn DataField="ApproverEmployee__AcctName" />
									<px:PXGridColumn DataField="WorkgroupID" />
									<px:PXGridColumn DataField="ApprovedByEmployee__AcctCD" />
									<px:PXGridColumn DataField="ApprovedByEmployee__AcctName" />
									<px:PXGridColumn DataField="OrigOwnerID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="ApproveDate" />
									<px:PXGridColumn DataField="Status" AllowNull="False" AllowUpdate="False" RenderEditorText="True" />
									<px:PXGridColumn DataField="Reason" AllowUpdate="False" />
									<px:PXGridColumn DataField="AssignmentMapID"  Visible="false" SyncVisible="false"/>
									<px:PXGridColumn DataField="RuleID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="StepID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
	</px:PXTab>
	<!--#include file="~\Pages\Includes\CRApprovalReasonPanel.inc"-->
	<!--#include file="~\Pages\Includes\EPReassignApproval.inc"-->
</asp:Content>
