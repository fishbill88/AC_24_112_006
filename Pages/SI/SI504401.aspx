<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SI504401.aspx.cs" Inherits="Page_SI504401"
	Title="Opportunity Activity Process" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%"
		PrimaryView="Filter" TypeName="PX.Objects.CR.SIOpportunityActivityProcess" PageLoadBehavior="PopulateSavedValues">
		<CallbackCommands>
			<px:PXDSCallbackCommand DependOnGrid="grdItems" CommitChanges="True" Name="Items_ViewDetails" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
		DataMember="Filter" Caption="Selection" DefaultControlID="edClassID">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXMultiSelector ID="edMultedClassID" DataField="ClassID" DataSourceID="ds" runat="server" ValuesSeparator="," AllowCustomItems="True"  AutoRefresh="True" CommitChanges="True" />    
			<px:PXMultiSelector ID="edMultiStatus" DataField="Status" DataSourceID="ds" runat="server" ValuesSeparator="," AllowCustomItems="True" AutoRefresh="True" CommitChanges="True" />
		</Template>
	</px:PXFormView>
	<px:PXGrid ID="grdItems" runat="server" DataSourceID="ds" Height="150px" Width="100%"
		ActionsPosition="Top" Caption="Opportunities" AllowPaging="True" AdjustPageSize="auto" 
		SkinID="PrimaryInquire" FastFilterFields="OpportunityID,Subject" 
		AutoGenerateColumns="AppendDynamic" RestrictFields="True">
		<Levels>
			<px:PXGridLevel DataMember="Items">
				<Columns>
					<px:PXGridColumn AllowCheckAll="True" AllowShowHide="False" DataField="Selected"
						TextAlign="Center" Type="CheckBox" Width="40px" AutoCallBack="True" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OpportunityID" LinkCommand="Items_ViewDetails" Width="120px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Subject" Width="200px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="ContactID" Width="150px" DisplayMode="Text" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Status" Width="90px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="StageID" Width="120px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CuryID" Width="70px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CloseDate" Width="90px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CuryAmount" Width="100px" TextAlign="Right" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrServicesEstimate" Width="120px" TextAlign="Right" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OwnerID" Width="120px" DisplayMode="Text" ></px:PXGridColumn>
					<px:PXGridColumn DataField="ClassID" Width="100px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Source" Width="120px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CuryWgtAmount" Width="100px" TextAlign="Right" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BAccount__AcctName" Width="200px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Contact__DisplayName" Width="150px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CreatedDateTime" Width="120px" DisplayFormat="g" TimeMode="True" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CampaignSourceID_Description" Width="200px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="ClosingDate" Width="90px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Resolution" Width="120px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrReferralSource" Width="200px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Campaign__AttributeCAMPGNSRC" Width="150px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrActivityNote" Width="300px" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<ActionBar PagerVisible="False"/>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="True" />
	</px:PXGrid>
</asp:Content>
