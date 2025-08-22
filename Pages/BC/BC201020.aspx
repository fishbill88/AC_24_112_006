<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="BC201020.aspx.cs" Inherits="Page_BC201020" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:pxdatasource pageloadbehavior="GoFirstRecord" id="ds" runat="server" visible="True" width="100%" primaryview="Bindings" typename="PX.Commerce.Amazon.BCAmazonStoreMaint">

		<callbackcommands>
			<px:pxdscallbackcommand name="Reload" postdata="Self" visible="False"></px:pxdscallbackcommand>
			<px:pxdscallbackcommand name="navigate" dependongrid="CstPXGrid60"></px:pxdscallbackcommand>
		</callbackcommands>
	</px:pxdatasource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:pxformview id="form" runat="server" datasourceid="ds" style="z-index: 100" width="100%" datamember="Bindings" tabindex="6500">
		<template>
			<px:pxlayoutrule runat="server" startrow="True" controlsize="L" labelswidth="SM"></px:pxlayoutrule>
			<px:pxdropdown allowedit="False" commitchanges="True" runat="server" id="CstPXDropDown16" datafield="ConnectorType"></px:pxdropdown>
			<px:pxselector autorefresh="True" allowedit="False" commitchanges="True" runat="server" id="CstPXSelector17" datafield="BindingName"></px:pxselector>
			<px:pxlayoutrule runat="server" id="CstPXLayoutRule69" startcolumn="True" />
			<px:pxcheckbox commitchanges="True" alignleft="True" runat="server" id="CstPXCheckBox67" datafield="IsActive"></px:pxcheckbox>
			<px:pxcheckbox alignleft="True" runat="server" id="CstPXCheckBox68" datafield="IsDefault"></px:pxcheckbox>
		</template>
	</px:pxformview>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:pxtab id="tab" runat="server" width="100%" height="150px" datasourceid="ds" datamember="CurrentStore">
		<items>
			<px:pxtabitem text="Connection Settings">
				<template>
					<px:pxlayoutrule runat="server" id="CstPXLayoutRule118" startcolumn="True"></px:pxlayoutrule>
					<px:pxlayoutrule controlsize="XL" labelswidth="SM" runat="server" id="CstPXLayoutRule70" startgroup="True"></px:pxlayoutrule>
					<px:pxformview renderstyle="Simple" datamember="CurrentBindingAmazon" runat="server" id="frmCurrentBindingAmazon">
						<template>
							<px:PXLayoutRule ControlSize="XL" LabelsWidth="SM" GroupCaption="Settings" runat="server" ID="CstPXLayoutRule17" StartGroup="True" ></px:PXLayoutRule>
								<px:pxdropdown id="edStoreBaseURL" runat="server" alreadylocalized="False" datafield="Region" commitchanges="True"></px:pxdropdown>
								<px:pxdropdown id="Pxdropdown1" runat="server" alreadylocalized="False" datafield="Marketplace" commitchanges="True"></px:pxdropdown>
                        </template>
					</px:pxformview>
					<px:pxformview renderstyle="Simple" datamember="CurrentBinding" runat="server" id="PXFormView1">
						<template>
							<px:pxlayoutrule runat="server" id="CstPXLayoutRule15" startgroup="True" groupcaption="System Settings" labelswidth="SM" controlsize="XL"></px:pxlayoutrule>
							<px:pxselector runat="server" id="CstPXSelectorLocaleName" datafield="LocaleName"></px:pxselector>
						</template>
					</px:pxformview>
					<px:pxformview renderstyle="Simple" datamember="CurrentBindingAmazon" runat="server" id="frmCurrentBindingAmazon2">
						<template>
							<px:PXLayoutRule ControlSize="XL" LabelsWidth="SM" GroupCaption="Store Properties" runat="server" ID="CstPXLayoutRule18" StartGroup="True" ></px:PXLayoutRule>
							<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit23" DataField="SellerPartnerId" ></px:PXTextEdit>
				        </template>
			        </px:pxformview>
					<px:PXLayoutRule runat="server" ID="PXLayoutRule2" StartColumn="True"></px:PXLayoutRule>
					<px:PXLayoutRule runat="server" ControlSize="L" LabelsWidth="SM" StartGroup="True" GroupCaption="Store Administrator Details"></px:PXLayoutRule>
					<px:PXFormView RenderStyle="Simple" DataMember="CurrentBinding" runat="server" ID="frmOther">
						<Template>
							<px:PXLayoutRule runat="server" ID="PXLayoutRule12" LabelsWidth="SM" ControlSize="L"></px:PXLayoutRule>
							<px:PXSelector runat="server" ID="edBindingAdministrator" DataField="BindingAdministrator"></px:PXSelector>
							<px:PXLayoutRule runat="server" ControlSize="L" LabelsWidth="SM" StartGroup="True" GroupCaption="License Restrictions"></px:PXLayoutRule>
							<px:PXNumberEdit runat="server" ID="edAllowedStores" DataField="AllowedStores"></px:PXNumberEdit>
						</Template>
					</px:PXFormView>
				</template>
			</px:pxtabitem>
			<px:pxtabitem text="Entity Settings">
				<template>
					<px:pxgrid AutoAdjustColumns="True" matrixmode="True" runat="server" skinid="Details" width="100%" id="CstPXGrid60">
						<autosize enabled="True" container="Window"></autosize>
						<actionbar defaultaction="navigate">
							<actions>
								<addnew toolbarvisible="False"></addnew>
								<delete toolbarvisible="False"></delete>
								<exportexcel toolbarvisible="False"></exportexcel>
							</actions>
						</actionbar>
						<levels>
							<px:pxgridlevel datamember="Entities">
								<columns>
									<px:pxgridcolumn type="CheckBox" textalign="Center" datafield="IsActive" width="60" commitchanges="True"></px:pxgridcolumn>
									<px:pxgridcolumn linkcommand="Navigate" datafield="EntityType" width="70"></px:pxgridcolumn>
									<px:pxgridcolumn commitchanges="True" datafield="Direction" width="70"></px:pxgridcolumn>
									<px:pxgridcolumn commitchanges="True" datafield="PrimarySystem" width="120"></px:pxgridcolumn>
									<px:pxgridcolumn datafield="MaxAttemptCount" width="120"></px:pxgridcolumn></columns>
								<rowtemplate>
									<px:pxnumberedit runat="server" id="CstPXNumberEdit70" datafield="MaxAttemptCount"></px:pxnumberedit>
								</rowtemplate>
							</px:pxgridlevel>
						</levels>
						<mode allowaddnew="False" allowdelete="False" ></mode>
					</px:pxgrid>
				</template>
			</px:pxtabitem>
			<px:PXTabItem Text="Inventory Settings">
				<Template>
					<px:PXLayoutRule ControlSize="L" LabelsWidth="M" runat="server" ID="CstPXLayoutRule56" StartGroup="True" GroupCaption="Inventory Settings"></px:PXLayoutRule>
							
					<px:PXDropDown CommitChanges="True" runat="server" DataField="AvailabilityCalcRule" ID="CstPXDropDown45"></px:PXDropDown>
					<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown71" DataField="WarehouseMode"></px:PXDropDown>
					
					<px:PXLayoutRule ControlSize="L" LabelsWidth="M" GroupCaption="Warehouse Mapping for Inventory Export" runat="server" ID="CstPXLayoutRule761" StartGroup="True"></px:PXLayoutRule>
					<px:PXGrid Height="200px" Width="510px" SyncPosition="True" AllowPaging="False" SkinID="Inquire" AutoAdjustColumns="True" MatrixMode="True" runat="server" ID="gridExportLocations">
						<Levels>
							<px:PXGridLevel DataMember="ExportLocations">
								<RowTemplate>
									<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" DataField="LocationID" ID="pxsLocationId"></px:PXSelector>
									<px:PXTextEdit runat="server" DataField="Description" AlreadyLocalized="False" ID="pxtLocationDescription"></px:PXTextEdit>
									<px:PXSelector runat="server" ID="pxsSiteId" DataField="SiteID" DisplayMode="Hint" CommitChanges="True"></px:PXSelector>
								</RowTemplate>
								<Columns>
									<px:PXGridColumn CommitChanges="True" DataField="SiteID" Width="140px"></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="LocationID" Width="140px"></px:PXGridColumn>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="True" />
						<ActionBar>
							<Actions>
								<AddNew Enabled="True" /></Actions></ActionBar>
						<ActionBar>
							<Actions>
								<Delete Enabled="True" /></Actions></ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Order Settings">
				<Template>

					<px:PXLayoutRule LabelsWidth="M" ControlSize="M" runat="server" ID="CstPXLayoutRule112" StartColumn="True"></px:PXLayoutRule>
					<px:PXLayoutRule LabelsWidth="M" ControlSize="M" runat="server" ID="CstPXLayoutRule115" StartGroup="True" GroupCaption="General"></px:PXLayoutRule>
					<px:PXFormView RenderStyle="Simple" DataMember="CurrentBinding" runat="server" ID="frmCurrentBinding1">
						<Template>
							<px:PXLayoutRule LabelsWidth="M" ControlSize="M" runat="server" ID="CstPXLayoutRule753"></px:PXLayoutRule>
							<px:PXSelector runat="server" CommitChanges="true" ID="edBranchID" AutoRefresh="true" DataField="BranchID" AllowEdit="True"></px:PXSelector>
						</Template>
					</px:PXFormView>

					<px:PXLayoutRule LabelsWidth="M" ControlSize="M" GroupCaption="Order" runat="server" ID="CstPXLayoutRule75" StartGroup="True"></px:PXLayoutRule>

					<px:PXFormView RenderStyle="Simple" DataMember="CurrentBindingAmazon" runat="server" ID="CurrentBindingAmazon4">
						<Template>
							<px:PXLayoutRule LabelsWidth="M" ControlSize="M" runat="server" ID="CstPXLayoutRule751"></px:PXLayoutRule>
							<px:PXSelector CommitChanges="True" AllowEdit="True" runat="server" ID="edOrderTpe" DataField="SellerFulfilledOrderType"></px:PXSelector>
							<px:PXSelector CommitChanges="True" AllowEdit="True" runat="server" ID="edAmazonFulfilledOrderType" DataField="AmazonFulfilledOrderType"></px:PXSelector>
						</Template>
					</px:PXFormView>

					<px:PXDropDown ID="edTimeZone" runat="server" DataField="OrderTimeZone" ></px:PXDropDown>
					<px:PXDropDown runat="server" ID="CstPXDropDown80" CommitChanges="True" DataField="PostDiscounts"></px:PXDropDown>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeSyncFrom" DataField="SyncOrdersFrom" ></px:PXDateTimeEdit>
					<px:PXSelector runat="server" ID="CstPXSelector39" DataField="GiftWrappingItemID" ></px:PXSelector>
					<px:PXFormView RenderStyle="Simple" DataMember="CurrentBindingAmazon" runat="server" ID="CurrentBindingAmazon5">
						<Template>
							<px:PXLayoutRule LabelsWidth="M"  ControlSize="M" runat="server" ID="PXLayoutRule159"></px:PXLayoutRule>
							<px:PXSelector runat="server" ID="CstPXSelector40" DataField="ShippingPriceItem" ></px:PXSelector>
						</Template>
					</px:PXFormView>
					
					<px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="CstPXLayoutRule25" StartGroup="True" GroupCaption="Customer" ></px:PXLayoutRule>
					<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask49" DataField="GuestCustomerID"></px:PXSegmentMask>

                    <px:PXFormView RenderStyle="Simple" DataMember="CurrentBindingAmazon" runat="server" ID="CurrentBindingAmazon1">
	                    <Template>
		                    <px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="CstPXLayoutRule26" StartGroup="True" GroupCaption="Marketplace-Fulfilled Order" ></px:PXLayoutRule>
							<px:PXCheckBox runat="server" ID="CstPXCheckBox4999" DataField="ReleaseInvoices"></px:PXCheckBox>
		                    <px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector32" DataField="CurrentBindingAmazon.Warehouse" CommitChanges = "True"></px:PXSelector>
		                    <px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector33" DataField="CurrentBindingAmazon.LocationID" CommitChanges = "True"></px:PXSelector>
		                    <px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector37" DataField="CurrentBindingAmazon.ShippingAccount" ></px:PXSelector>
		                    <px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector38" DataField="CurrentBindingAmazon.ShippingSubAccount" ></px:PXSelector>
	                    </Template>
                    </px:PXFormView>

					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule24" StartColumn="True" ></px:PXLayoutRule>
					<px:PXLayoutRule LabelsWidth="M" ControlSize="M" runat="server" ID="CstPXLayoutRule117" StartGroup="True" GroupCaption="Taxes"></px:PXLayoutRule>
					<px:PXCheckBox runat="server" ID="CstPXCheckBox2" DataField="TaxSynchronization" CommitChanges="True"></px:PXCheckBox>
					<px:PXSelector CommitChanges="True" AutoRefresh="True" runat="server" ID="CstPXSelector118" DataField="DefaultTaxZoneID"></px:PXSelector>

					<px:PXFormView RenderStyle="Simple" DataMember="CurrentBindingAmazon" runat="server" ID="CurrentBindingAmazon3">
						<Template>
							<px:PXLayoutRule LabelsWidth="M"  ControlSize="M" runat="server" ID="PXLayoutRule158"></px:PXLayoutRule>
							<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector35" DataField="DefaultTaxID" ></px:PXSelector>
						</Template>
					</px:PXFormView>

                    <px:PXFormView RenderStyle="Simple" DataMember="CurrentBindingAmazon" runat="server" ID="CurrentBindingAmazon2">
	                    <Template>
							<px:PXLayoutRule LabelsWidth="M"  ControlSize="M" runat="server" ID="PXLayoutRule118" StartGroup="True" GroupCaption="Substitution Lists"></px:PXLayoutRule>
							<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector31" DataField="ShipViaCodesToCarriers" ></px:PXSelector>
							<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector311" DataField="ShipViaCodesToCarrierServices" ></px:PXSelector>
	                    </Template>
                    </px:PXFormView>

                </Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Payment Settings" Visible="True">
				<Template>
					<px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="300" SkinID="Horizontal" Height="500px">
						<AutoSize Enabled="True" />                        
						<Template1>
							<px:PXGrid AutoAdjustColumns="True" SyncPosition="True" runat="server" ID="PaymentsMethods"
								MatrixMode="True"  SkinID="DetailsInTab" Width="100%" Height="180px">
							<AutoSize Container="Parent" Enabled="True"></AutoSize>
							<Levels>
								<px:PXGridLevel DataMember="PaymentMethods">
								<Columns>
									<px:PXGridColumn CommitChanges="True" TextAlign="Center" Type="CheckBox" DataField="Active" Width="30"></px:PXGridColumn>
									<px:PXGridColumn DataField="StorePaymentMethod" Width="200"></px:PXGridColumn>
									<px:PXGridColumn  DataField="StoreCurrency"  CommitChanges="True" Width="140"></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="PaymentMethodID" Width="140"></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="CashAccountID" Width="140"></px:PXGridColumn>
									<px:PXGridColumn TextAlign="Center" Type="CheckBox" DataField="ReleasePayments" Width="80"></px:PXGridColumn>
								</Columns>
								<RowTemplate>
									<px:PXSelector AutoRefresh="True" runat="server" ID="edPaymentMethodID" DataField="PaymentMethodID" AllowEdit="True"></px:PXSelector>
									<px:PXSelector AutoRefresh="True" runat="server" ID="edCashAccountID" DataField="CashAccountID" AllowEdit="True"></px:PXSelector>
									<px:PXSelector AutoRefresh="True" runat="server" ID="edProcessingCenterID" DataField="ProcessingCenterID" AllowEdit="True"></px:PXSelector>
								</RowTemplate>
								</px:PXGridLevel>
							</Levels>
							<Mode AllowDelete="True" AllowAddNew="True"></Mode>
							<ActionBar ActionsVisible="True" DefaultAction="">
								<Actions>
								<AddNew Enabled="True" ToolBarVisible="Top" MenuVisible="True"></AddNew>
								<Delete Enabled="True" ToolBarVisible="Top"></Delete>
								</Actions>
							</ActionBar>
							<AutoCallBack Target="FeeMappings" Command="Refresh">
								<Behavior CommitChanges="False" RepaintControlsIDs="FeeMappings"></Behavior>
							</AutoCallBack>
							</px:PXGrid>
						</Template1>
						<Template2>
							<px:PXGrid CaptionVisible="True" AutoAdjustColumns="True" MatrixMode="" SyncPosition="True" SkinID="Details"
								Width="100%" runat="server" ID="FeeMappings" Caption="Amazon Fees">
								<Levels>
									<px:PXGridLevel DataMember="FeeMappings" >
									<Columns>
										<px:PXGridColumn CommitChanges="True" TextAlign="Center" Type="CheckBox" DataField="Active" Width="30" />
										<px:PXGridColumn CommitChanges="True" DisplayMode="Text" DataField="FeeType" Width="80" />
										<px:PXGridColumn CommitChanges="True" DisplayMode="Text" DataField="FeeDescription" Width="220" />
										<px:PXGridColumn CommitChanges="True" DisplayMode="Value" DataField="EntryTypeID" Width="120" />
										<px:PXGridColumn DataField="EntryDescription" Width="220" />
										<px:PXGridColumn DataField="TransactionType" Width="70" />
									</Columns>
									<RowTemplate>
										<px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector113" DataField="EntryTypeID"></px:PXSelector>
									</RowTemplate>
									</px:PXGridLevel>
								</Levels>
								<ActionBar ActionsVisible="True">
									<Actions>
									<AddNew MenuVisible="True" ToolBarVisible="Top" Enabled="True" ></AddNew>
									<Delete ToolBarVisible="Top" ></Delete>
									</Actions>
								</ActionBar>
								<Mode AllowAddNew="True" AllowDelete="True" ></Mode>
								<AutoSize Container="Parent" Enabled="True"></AutoSize>
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Shipping Settings">
				<Template>
					<px:PXGrid Width="100%" SyncPosition="True" AllowPaging="False" SkinID="Inquire" AutoAdjustColumns="True" MatrixMode="True" runat="server" ID="PXGrid1">
						<Levels>
							<px:PXGridLevel DataMember="ShippingMappings">
								<Columns>
									<px:PXGridColumn CommitChanges="True" TextAlign="Center" Type="CheckBox" DataField="Active" Width="60"></px:PXGridColumn>
									<px:PXGridColumn DataField="ShippingZone" Width="150"></px:PXGridColumn>
									<px:PXGridColumn DataField="ShippingMethod" Width="180"></px:PXGridColumn>
									<px:PXGridColumn DataField="CarrierID" Width="140"></px:PXGridColumn>
									<px:PXGridColumn DataField="ZoneID" Width="120"></px:PXGridColumn>
									<px:PXGridColumn DataField="ShipTermsID" Width="120"></px:PXGridColumn>
								</Columns>
								<RowTemplate>
									<px:PXSelector runat="server" ID="edCarrierID" DataField="CarrierID" AllowEdit="True"></px:PXSelector>
									<px:PXSelector runat="server" ID="edZoneID" DataField="ZoneID" AllowEdit="True"></px:PXSelector>
									<px:PXSelector runat="server" ID="edShipTermsID" DataField="ShipTermsID" AllowEdit="True"></px:PXSelector>
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<ActionBar>
							<Actions>
								<AddNew Enabled="True" />
								<Delete Enabled="True" />
							</Actions>
						</ActionBar>
						<AutoSize Enabled="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</items>
		<autosize container="Window" enabled="True" minheight="150"></autosize>
	</px:pxtab>
</asp:Content>