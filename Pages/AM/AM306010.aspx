<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AM306010.aspx.cs" Inherits="Page_AM306010" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.Objects.AM.ConfigurationEntryForAPI" PrimaryView="Results" SuspendUnloading="False">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="CurrencyView" Visible="False" />
            <px:PXDSCallbackCommand Name="ShowAll" Visible="false" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Results">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="S" StartColumn="True" />
            <px:PXTextEdit runat="server" ID="edConfigResultsID" DataField="ConfigResultsID" CommitChanges="true"/>
            <px:PXTextEdit ID="edInventoryID" runat="server" DataField="InventoryID" CommitChanges="true"/>
            <px:PXTextEdit runat="server" ID="edConfigurationID" DataField="ConfigurationID" CommitChanges="true" />
            <px:PXTextEdit runat="server" ID="edRevision" DataField="Revision" />
            <px:PXTextEdit ID="edSiteID" runat="server" DataField="SiteID" AllowEdit="True" CommitChanges="True" />
            <px:PXCheckBox ID="edCompleted" runat="server" DataField="Completed"/>
            <px:PXCheckBox ID="edIsConfigurationTesting" runat="server" DataField="IsConfigurationTesting"/>
            <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="S" StartColumn="True" />
            <px:PXTextEdit ID="edOrdTypeRef" runat="server" DataField="OrdTypeRef"/>
            <px:PXTextEdit ID="edOrdNbrRef" runat="server" DataField="OrdNbrRef"/>
            <px:PXTextEdit ID="edOrdLineRef" runat="server" DataField="OrdLineRef"/>
            <px:PXTextEdit ID="edOpportunityQuoteID" runat="server" DataField="OpportunityQuoteID"/>
            <px:PXTextEdit ID="edOpportunityLineNbr" runat="server" DataField="OpportunityLineNbr"/>
            <px:PXTextEdit ID="edProdOrderType" runat="server" DataField="ProdOrderType"/>
            <px:PXTextEdit ID="edProdOrderNbr" runat="server" DataField="ProdOrderNbr"/>
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="XM" StartColumn="True"/>
            <pxa:PXCurrencyRate DataField="CuryID" ID="edCury" runat="server" DataSourceID="ds" RateTypeView="_AMConfigurationResults_CurrencyInfo_"
                DataMember="_Currency_" />
            <px:PXNumberEdit ID="edDisplayPrice" runat="server" DataField="DisplayPrice" />
            <px:PXTextEdit CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowAddNew="True" AllowEdit="True" />
            <px:PXTextEdit CommitChanges="True" ID="edCustomerLocation" runat="server" DataField="CustomerLocationID" AllowEdit="True" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="300" SkinID="Horizontal" Height="600px" Panel1MinSize="100" Panel2MinSize="100">
        <AutoSize Container="Window" Enabled="True" MinHeight="300" />
        <Template1>
             <px:PXTab ID="tab" runat="server" Width="100%" DataSourceID="ds" DataMember="CurrentResults">
                <Items>
                    <px:PXTabItem Text="Features" LoadOnDemand="True" RepaintOnDemand="True">
                        <Template>
                            <px:PXGrid ID="FeaturesGrid" runat="server" DataSourceID="ds" SkinID="DetailsInTab" Width="100%" Height="100%" AutoAdjustColumns="True" SyncPosition="True"
                                MatrixMode="True" TabIndex="2000">
                                <Levels>
                                    <px:PXGridLevel DataKeyNames="ConfigResultsID,FeatureLineNbr" DataMember="CurrentFeatures">
                                        <Columns>
                                            <px:PXGridColumn DataField="AMConfigurationFeature__Label"/>
                                            <px:PXGridColumn DataField="MinSelection"/>
                                            <px:PXGridColumn DataField="MaxSelection"/>
                                            <px:PXGridColumn DataField="MinQty"/>
                                            <px:PXGridColumn DataField="MaxQty"/>
                                            <px:PXGridColumn DataField="LotQty"/>
                                            <px:PXGridColumn DataField="TotalQty"/>
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <AutoSize Enabled="true" />
                                <ActionBar ActionsText="False">
                                    <Actions>
                                        <AddNew ToolBarVisible="false" MenuVisible="false" />
                                        <Delete Enabled="false" />
                                    </Actions>
                                </ActionBar>
                                <AutoCallBack Command="Refresh" Target="SelectedOptionsGrid" ActiveBehavior="true" >
                                    <Behavior RepaintControlsIDs="SelectedOptionsGrid"  />
                                </AutoCallBack>
                            </px:PXGrid>
                        </Template>
                    </px:PXTabItem>
                    <px:PXTabItem Text="Attributes" LoadOnDemand="True" RepaintOnDemand="True">
                        <Template>
                            <px:PXGrid ID="AttributesGrid" runat="server" DataSourceID="ds" SkinID="DetailsInTab" Width="100%" Height="100%" AutoAdjustColumns="True" SyncPosition="True"
                                MatrixMode="True" TabIndex="2000">
                                <Levels>
                                    <px:PXGridLevel DataKeyNames="ConfigResultsID,AttributeLineNbr" DataMember="Attributes">
                                        <Columns>
                                            <px:PXGridColumn DataField="ConfigResultsID" TextAlign="Right"/>
                                            <px:PXGridColumn DataField="ConfigurationID"/>
                                            <px:PXGridColumn DataField="Revision"/>
                                            <px:PXGridColumn DataField="AttributeLineNbr" TextAlign="Right"/>
                                            <px:PXGridColumn DataField="AMConfigurationAttribute__Label" Width="120px"/>
                                            <px:PXGridColumn DataField="AMConfigurationAttribute__Descr" Width="200px"/>
                                            <px:PXGridColumn DataField="Required" TextAlign="Center" Type="CheckBox" Width="60px"/>
                                            <px:PXGridColumn DataField="Value" Width="200px" CommitChanges="true" MatrixMode="true"/>
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <AutoSize Enabled="True" />
                                <ActionBar ActionsText="False">
                                    <Actions>
                                        <AddNew ToolBarVisible="false" MenuVisible="false" />
                                    </Actions>
                                </ActionBar>
                            </px:PXGrid>
                        </Template>
                    </px:PXTabItem>
                </Items>
            </px:PXTab>
        </Template1>
        <Template2>
<%--            <px:PXPanel runat="server">
                <px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="SM" StartColumn="True" />
                <px:PXFormView ID="FeatureForm" runat="server" DataSourceID="ds" DataMember="CurrentFeature" Caption="Feature" SyncPosition="true" RenderStyle="Fieldset" TabIndex="7100">
                    <Template>
                        <px:PXTextEdit ID="edMinMaxSelection" runat="server" DataField="MinMaxSelection">
                        </px:PXTextEdit>
                        <px:PXTextEdit ID="edMinLotMaxQty" runat="server" DataField="MinLotMaxQty">
                        </px:PXTextEdit>
                        <px:PXTextEdit ID="edTotalQty" runat="server" DataField="TotalQty">
                        </px:PXTextEdit>
                    </Template>
                </px:PXFormView>
                <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" ControlSize="M" LabelsWidth="SM" />                            
                <px:PXFormView ID="SelectedOptionForm" runat="server" DataSourceID="ds" DataMember="CurrentOption" Caption="Selected Option" RenderStyle="Fieldset" TabIndex="7150">
                    <Template>
                        <px:PXTextEdit ID="edOptionMinLotMaxQty" runat="server" DataField="MinLotMaxQty">
                        </px:PXTextEdit>
                    </Template>
                </px:PXFormView>
            </px:PXPanel>--%>
            <px:PXGrid ID="SelectedOptionsGrid" runat="server" DataSourceID="ds" SkinID="DetailsInTab" Width="100%"
                AutoAdjustColumns="True" SyncPosition="True">
                <Levels>
                    <px:PXGridLevel DataKeyNames="ConfigResultsID,FeatureLineNbr,OptionLineNbr" DataMember="Options">
                        <Columns>
                            <px:PXGridColumn DataField="ConfigResultsID"/>
                            <px:PXGridColumn DataField="FeatureLineNbr"/>
                            <px:PXGridColumn DataField="OptionLineNbr"/>
                            <px:PXGridColumn DataField="IsRemovable" Width="1px" RenderEditorText="True"/>
                            <px:PXGridColumn DataField="Included" Type="CheckBox" TextAlign="Center" CommitChanges="True"/>
                            <px:PXGridColumn DataField="AMConfigurationOption__Label" Width="150px"/>
                            <px:PXGridColumn DataField="AMConfigurationOption__Descr" Width="200px"/>
                            <px:PXGridColumn DataField="Qty" TextAlign="Right" Width="100px" CommitChanges="True"/>
                            <px:PXGridColumn DataField="UOM" Width="60px"/>
                            <px:PXGridColumn DataField="InventoryID" Width="100px"/>
                            <px:PXGridColumn DataField="SubItemID" Width="100px"/>
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
                <AutoSize Enabled="True" />
<%--                <AutoCallBack Command="Refresh" Target="SelectedOptionForm" ActiveBehavior="true">
                    <Behavior RepaintControlsIDs="SelectedOptionForm" />
                </AutoCallBack>--%>
                <ActionBar ActionsText="False">
                    <Actions>
                        <AddNew ToolBarVisible="false" MenuVisible="false" />
                        <Delete Enabled="false" />
                    </Actions>
                    <CustomItems>
                        <px:PXToolBarButton>
                            <AutoCallBack Target="ds" Command="ShowAll"/>
                        </px:PXToolBarButton>
                    </CustomItems>
                </ActionBar>
            </px:PXGrid>
        </Template2>
    </px:PXSplitContainer>
</asp:Content>