<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AM208100.aspx.cs"
    Inherits="Page_AM208100" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <script language="javascript" type="text/javascript">
        function showTreeChanged(owner, args)
    	{
            refreshTree();
        }

        function refreshTree()
        {
            var bomTree = px_alls["treeBomTree"];
            bomTree.element.au.controller.viewModel.config.refresh = true;
        }

        function commandResult(ds, context)
        {
            if (context.command == "Save" || context.command == "Delete" || context.command == "Insert" ||
                context.command == "First" || context.command == "Previous" ||
                context.command == "Next" || context.command == "Last")
            {
                refreshTree();
            }
        }
    </script>
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Documents" TypeName="PX.Objects.AM.EngineeringWorkbenchMaint">
        <ClientEvents CommandPerformed="commandResult" />
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXSplitContainer runat="server" CssClass="manufacturing-tree-split-container" ID="sp1" SplitterPosition="830">
    <AutoSize Container="Window" Enabled="True" MinHeight="300" />
        <Template1>
            <px:PXFormView ID="form" runat="server" DataSourceID="ds" DefaultControlID="edBOMID" Width="100%" DataMember="Documents">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule15" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                    <px:PXSelector ID="edBOMID" runat="server" DataField="BOMID" CommitChanges="True" FilterByAllFields="True" AllowEdit="True" >
                        <ClientEvents ValueChanged="showTreeChanged" />
                    </px:PXSelector>
                    <px:PXSelector ID="edRevisionID" runat="server" DataField="RevisionID" CommitChanges="True" AutoRefresh="True" >
                        <ClientEvents ValueChanged="showTreeChanged" />
                    </px:PXSelector>
                    <px:PXCheckBox ID="chkHold" runat="server" DataField="Hold" Width="50px" CommitChanges="True" />
                    <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" AllowNull="False" DataField="Status" />
                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                    <px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" CommitChanges="True" />
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="true" LabelsWidth="S" ControlSize="L"/>
                    <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" DisplayMode="Hint" AllowEdit="True" CommitChanges="True" />
                    <px:PXSegmentMask ID="edSubItemID" runat="server" DataField="SubItemID" CommitChanges="True" />
                    <px:PXSelector ID="edSiteID" runat="server" DataField="SiteID" DisplayMode="Hint" AllowEdit="True" CommitChanges="True" />
                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" Merge="true" LabelsWidth="S" ControlSize="SM"/>
                    <px:PXDateTimeEdit ID="edEffStartDate" runat="server" DataField="EffStartDate" />
                    <px:PXDateTimeEdit ID="edEffEndDate" runat="server" DataField="EffEndDate" />
                </Template>
            </px:PXFormView>
            <px:PXFormView ID="formTree" CssClass="manufacturing-tree-form-view" runat="server" DataSourceID="ds" Width="100%" Caption="Tree"
                NoteIndicator="False" FilesIndicator="False" LinkIndicator="False" BPEventsIndicator="False" TabIndex="100">
                <Template>
                    <px:PXTree runat="server" ID="treeBomTree" CallbackUpdatable="True" DataMember="BomTree" DefaultDrag="Cut" Width="100%" Height="100%" 
                        ParentIDName="IDParent" IDName="IDName" Description="Description" ExtraColumnField="ExtraColumns" ActionField="Actions" IconField="Icon" IconColorField="Iconcolor" IconSize="22"
                        AddSiblingNode="NewSiblingNodeDefault" AddChildNode="NewChildNodeDefault"
                        OnAdd="AddNode" OnDelete="DeleteNode" OnChange="UpdateNode" OnSelect="SelectNode" CheckDropCommand="CheckDropAction" Modifiable="True" Mode="single" SingleClickSelect="True" >
						<ExtraColumns>
							<px:ExtraColumn Title="Qty" TagName="qp-text-editor" Width="115" TextAlign="3"></px:ExtraColumn>
							<px:ExtraColumn Title="Uom" TagName="qp-text-editor" Width="85" TextAlign="1"></px:ExtraColumn>
						</ExtraColumns>
					</px:PXTree>
                </Template>
            </px:PXFormView>
        </Template1>
        <Template2>
            <px:PXFormView ID="SelectedBOMForm" runat="server" DataSourceID="ds" Width="100%" DataMember="SelectedBomItem2" Caption="Selected BOM"
                NoteIndicator="True" FilesIndicator="True" LinkIndicator="True" BPEventsIndicator="True" TabIndex="100">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule25" runat="server" GroupCaption="Selected BOM" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                    <px:PXSelector ID="edBOMID2" runat="server" AutoRefresh="True" DataField="BOMID" DataSourceID="ds" CommitChanges="true" FilterByAllFields="True" AllowEdit="True" />
                    <px:PXSelector ID="edRevisionID2" runat="server" DataField="RevisionID" CommitChanges="True" AutoRefresh="True" DataSourceID="ds" />
                    <px:PXCheckBox ID="chkHold2" runat="server" DataField="Hold" Width="50px" CommitChanges="True" />
                    <px:PXDropDown CommitChanges="True" ID="edStatus2" runat="server" AllowNull="False" DataField="Status" />
                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                    <px:PXTextEdit ID="edDescr2" runat="server" DataField="Descr" CommitChanges="True" />
                    <px:PXLayoutRule ID="PXLayoutRule32" runat="server" StartColumn="true" LabelsWidth="M" ControlSize="L"/>
                    <px:PXSegmentMask ID="edInventoryID2" runat="server" DataField="InventoryID" DisplayMode="Hint" AllowEdit="True" CommitChanges="True" />
                    <px:PXSegmentMask ID="edSubItemID2" runat="server" DataField="SubItemID" CommitChanges="True" AutoRefresh="True" />
                    <px:PXSelector ID="edSiteID2" runat="server" DataField="SiteID" DisplayMode="Hint" AllowEdit="True" CommitChanges="True" />
                    <px:PXLayoutRule ID="PXLayoutRule12" runat="server" Merge="true" LabelsWidth="M" ControlSize="L"/>
                    <px:PXDateTimeEdit ID="edEffStartDate2" runat="server" DataField="EffStartDate"/>
                    <px:PXDateTimeEdit ID="edEffEndDate2" runat="server" DataField="EffEndDate"/>
                </Template>
            </px:PXFormView>
            <px:PXFormView ID="TreeNodeSelectedForm" runat="server" DataSourceID="ds" Width="100%" DataMember="SelectedTreeNode">
                <Template>
                    <px:PXLayoutRule ID="TreeNodeSelectedFormLayoutRule01" runat="server" GroupCaption="Selected Operation" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                    <px:PXCheckBox ID="chkIsOperation" runat="server" DataField="IsOperation" Visible="False" />
                    <px:PXCheckBox ID="chkIsSubassembly" runat="server" DataField="IsSubassembly" Visible="False" />
                    <px:PXTextEdit ID="edSelectedNodeOperationCD" runat="server" DataField="OperationCD" />
                    <px:PXLayoutRule ID="TreeNodeSelectedFormLayoutRule02" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                    <px:PXSelector ID="edSelectedNodeWcID" runat="server" DataField="WcID" AllowEdit="True" />
                    <px:PXTextEdit ID="edSelectedNodeOperationDescription" runat="server" DataField="OperationDescription" />
                </Template>
            </px:PXFormView>
            <px:PXTab ID="tab" runat="server" Width="100%" Height="100%">
            <Items>
                <px:PXTabItem Text="Material" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == False">
                    <Template>
                        <px:PXFormView ID="selectedMaterial" runat="server" CaptionVisible="False" DataMember="SelectedBomMatl" DataSourceID="ds" 
                            Width="100%" Height="100%" SyncPosition="True" RenderStyle="Simple">
                            <Template>
                                <px:PXLayoutRule ID="selectedMaterialLayoutRule01" runat="server" LabelsWidth="M" ControlSize="L" />
                                <px:PXSegmentMask ID="edSelectedMatlInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" CommitChanges="True" />
                                <px:PXSegmentMask ID="edSelectedMatlSubItemID" runat="server" DataField="SubItemID" />
                                <px:PXTextEdit ID="edSelectedMatlDescr" runat="server" DataField="Descr" />
                                <px:PXNumberEdit ID="edSelectedMatlQtyReq" runat="server" DataField="QtyReq" />
                                <px:PXSelector ID="edSelectedMatlUOM" runat="server" DataField="UOM" AutoRefresh="True" />
                                <px:PXCheckBox ID="edSelectedMatlBFlush" runat="server" DataField="BFlush" />
                                <px:PXNumberEdit ID="edSelectedMatlBatchSize" runat="server" DataField="BatchSize" />
                                <px:PXNumberEdit ID="edSelectedMatlScrapFactor" runat="server" DataField="ScrapFactor" />
                                <px:PXNumberEdit ID="edSelectedMatlUnitCost" runat="server" DataField="UnitCost" />
                                <px:PXNumberEdit ID="edSelectedMatlMatlPlanCost" runat="server" DataField="PlanCost"/>
                                <px:PXLayoutRule ID="selectedMaterialLayoutRule02" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                <px:PXDropDown ID="edSelectedMatlMaterialType" runat="server" DataField="MaterialType" CommitChanges="true" />
                                <px:PXDropDown ID="edSelectedMatlSubcontractSource" runat="server" DataField="SubcontractSource" CommitChanges="True" />
                                <px:PXDropDown ID="edSelectedMatlPhtmRtngIorE" runat="server" AllowNull="False" DataField="PhantomRouting" />
                                <px:PXSegmentMask ID="edSelectedMatlSiteID" runat="server" DataField="SiteID" CommitChanges="true" />
                                <px:PXSegmentMask ID="edSelectedMatlLocationID" runat="server" DataField="LocationID" />
                                <px:PXSelector ID="edSelectedMatlCompBOMID" runat="server" DataField="CompBOMID" AutoRefresh="True" />
                                <px:PXSelector ID="edSelectedMatlCompBOMRevisionID" runat="server" DataField="CompBOMRevisionID" AllowEdit="True" AutoRefresh="True" />
                                <px:PXTextEdit ID="edSelectedMatlBubbleNbr" runat="server" DataField="BubbleNbr" />
                                <px:PXDateTimeEdit ID="edSelectedMatlEffDate" runat="server" DataField="EffDate" />
                                <px:PXDateTimeEdit ID="edSelectedMatlExpDate" runat="server" DataField="ExpDate" />
                            </Template>
                        </px:PXFormView>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Subassembly" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsSubassembly&quot;].Value == True">
                    <Template>
                        <px:PXFormView ID="selectedSubassembly" runat="server" CaptionVisible="False" DataMember="SubassemblyBomItem3" DataSourceID="ds" 
                            Width="100%" Height="100%" SyncPosition="True" RenderStyle="Simple">
                            <Template>
                                <px:PXLayoutRule ID="selectedSubassemblyLayoutRule1" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                <px:PXSelector ID="selectedSubassemblyBOMID" runat="server" AutoRefresh="True" DataField="BOMID" DataSourceID="ds" CommitChanges="true" FilterByAllFields="True" AllowEdit="True" />
                                <px:PXSelector ID="selectedSubassemblyRevisionID" runat="server" DataField="RevisionID" CommitChanges="True" AutoRefresh="True" DataSourceID="ds" />
                                <px:PXCheckBox ID="selectedSubassemblyHold" runat="server" DataField="Hold" Width="50px" CommitChanges="True" />
                                <px:PXDropDown ID="selectedSubassemblyStatus" runat="server" DataField="Status" />
                                <px:PXLayoutRule ID="selectedSubassemblyLayoutRule2" runat="server" ColumnSpan="2" />
                                <px:PXTextEdit ID="selectedSubassemblyDescr" runat="server" DataField="Descr" CommitChanges="True" />
                                <px:PXLayoutRule ID="selectedSubassemblyLayoutRule3" runat="server" StartColumn="true" LabelsWidth="M" ControlSize="L"/>
                                <px:PXSegmentMask ID="selectedSubassemblyInventoryID" runat="server" DataField="InventoryID" DisplayMode="Hint" AllowEdit="True" CommitChanges="True" />
                                <px:PXSegmentMask ID="selectedSubassemblySubItemID" runat="server" DataField="SubItemID" CommitChanges="True" AutoRefresh="True" />
                                <px:PXSelector ID="selectedSubassemblySiteID" runat="server" DataField="SiteID" DisplayMode="Hint" AllowEdit="True" CommitChanges="True" />
                                <px:PXLayoutRule ID="selectedSubassemblyLayoutRule4" runat="server" Merge="true" LabelsWidth="M" ControlSize="L"/>
                                <px:PXDateTimeEdit ID="selectedSubassemblyEffStartDate" runat="server" DataField="EffStartDate"/>
                                <px:PXDateTimeEdit ID="selectedSubassemblyEffEndDate" runat="server" DataField="EffEndDate"/>
                            </Template>
                        </px:PXFormView>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Operation" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == True">
                    <Template>
                        <px:PXFormView ID="selectedOperation" runat="server" CaptionVisible="False" DataMember="SelectedBomOper" DataSourceID="ds" 
                            Width="100%" SyncPosition="True" RenderStyle="Simple">
                            <Template>
                                <px:PXLayoutRule ID="selectedOperationLayoutRule1" runat="server" GroupCaption="General Settings" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                <px:PXMaskEdit ID="edOperationCD" runat="server" DataField="OperationCD" />
                                <px:PXLayoutRule runat="server" />
                                <px:PXSelector ID="edWcID" runat="server" DataField="WcID" CommitChanges="True" AllowEdit="True" />
                                <px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
                                <px:PXMaskEdit ID="edSetupTime" runat="server" DataField="SetupTime" />
                                <px:PXNumberEdit ID="edRunUnits" runat="server" DataField="RunUnits" />
                                <px:PXMaskEdit ID="edRunUnitTime" runat="server" DataField="RunUnitTime" />
                                <px:PXNumberEdit ID="edMachineUnits" runat="server" DataField="MachineUnits" />
                                <px:PXMaskEdit ID="edMachineUnitTime" runat="server" DataField="MachineUnitTime" />
                                <px:PXCheckBox ID="edBFlushLabor" runat="server" DataField="BFlush" />
                                <px:PXCheckBox ID="edControlPoint" runat="server" DataField="ControlPoint" />
                                <px:PXMaskEdit ID="edQueueTime" runat="server" DataField="QueueTime" />
                                <px:PXMaskEdit ID="edFinishTime" runat="server" DataField="FinishTime" />
                                <px:PXMaskEdit ID="edMoveTime" runat="server" DataField="MoveTime" />
                                <px:PXDropDown ID="edScrapAction" runat="server" DataField="ScrapAction" />
                            </Template>
                        </px:PXFormView>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Materials" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == True">
                    <Template>
                        <px:PXGrid ID="gridMatl" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" SyncPosition="True">
                            <Levels>
                                <px:PXGridLevel DataMember="BomMatlRecords">
                                    <RowTemplate>
                                        <px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                        <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" />
                                        <px:PXSegmentMask ID="edSubItemID" runat="server" DataField="SubItemID" />
                                        <px:PXTextEdit ID="edDescrMat" runat="server" DataField="Descr" MaxLength="60" />
                                        <px:PXNumberEdit ID="edQtyReq" runat="server" DataField="QtyReq" />
                                        <px:PXNumberEdit ID="edBatchSize" runat="server" DataField="BatchSize" />
                                        <px:PXSelector ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True" />
                                        <px:PXNumberEdit ID="edUnitCost" runat="server" DataField="UnitCost" />
                                        <px:PXNumberEdit ID="edMatlPlanCost" runat="server" DataField="PlanCost"/>
                                        <px:PXDropDown ID="edMaterialType" runat="server" AllowNull="False" DataField="MaterialType" CommitChanges="true" />
                                        <px:PXDropDown ID="edPhtmRtngIorE" runat="server" AllowNull="False" DataField="PhantomRouting" />
                                        <px:PXCheckBox ID="chkBFlush1" runat="server" DataField="BFlush" />
                                        <px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                        <px:PXSegmentMask ID="edSiteID" runat="server" DataField="SiteID" CommitChanges="true" />
                                        <px:PXSelector ID="edCompBOMID" runat="server" DataField="CompBOMID" AutoRefresh="True" />
                                        <px:PXSelector ID="edCompBOMRevisionID" runat="server" DataField="CompBOMRevisionID" AllowEdit="True" AutoRefresh="True" />
                                        <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" />
                                        <px:PXNumberEdit ID="edScrapFactor" runat="server" DataField="ScrapFactor" />
                                        <px:PXTextEdit ID="edBubbleNbr" runat="server" DataField="BubbleNbr" />
                                        <px:PXDateTimeEdit ID="edEffDate" runat="server" DataField="EffDate" />
                                        <px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                        <px:PXDateTimeEdit ID="edExpDate" runat="server" DataField="ExpDate" />
                                        <px:PXMaskEdit ID="edBOMIDmatl" runat="server" DataField="BOMID" />
                                        <px:PXTextEdit ID="edOperationIDmatl" runat="server" DataField="OperationID" />
                                        <px:PXNumberEdit ID="edLineIDmatl" runat="server" DataField="LineID" />
                                        <px:PXDropDown ID="edSubcontractSource" runat="server" DataField="SubcontractSource" CommitChanges="True" />
                                    </RowTemplate>
                                    <Columns>
                                        <px:PXGridColumn DataField="LineID" TextAlign="Right" Width="54px" />
                                        <px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="54px" />
                                        <px:PXGridColumn DataField="InventoryID" Width="130px" CommitChanges="True" AllowDragDrop="True" />
                                        <px:PXGridColumn DataField="SubItemID" Width="81px" />
                                        <px:PXGridColumn DataField="Descr" MaxLength="255" Width="200px" />
                                        <px:PXGridColumn DataField="QtyReq" TextAlign="Right" Width="108px" CommitChanges="True" AllowDragDrop="True" />                            
                                        <px:PXGridColumn DataField="BatchSize" TextAlign="Right" Width="108px" />
                                        <px:PXGridColumn DataField="UOM" Width="81px" CommitChanges="True" />
                                        <px:PXGridColumn DataField="UnitCost" TextAlign="Right" Width="108px" />
                                        <px:PXGridColumn DataField="PlanCost" TextAlign="Right" Width="100px" /> 
                                        <px:PXGridColumn DataField="MaterialType" TextAlign="Left" Width="100px" CommitChanges="True" />
                                        <px:PXGridColumn DataField="PhantomRouting" TextAlign="Left" Width="110px" />
                                        <px:PXGridColumn DataField="BFlush" TextAlign="Center" Type="CheckBox" Width="85px" />
                                        <px:PXGridColumn DataField="SiteID" TextAlign="Left" Width="130px" CommitChanges="true" />
                                        <px:PXGridColumn DataField="CompBOMID" LinkCommand="ViewCompBomID" CommitChanges="true" />
                                        <px:PXGridColumn DataField="CompBOMRevisionID" Width="85px" />
                                        <px:PXGridColumn DataField="LocationID" TextAlign="Right" Width="130px" />
                                        <px:PXGridColumn DataField="ScrapFactor" TextAlign="Right" Width="108px" />
                                        <px:PXGridColumn DataField="BubbleNbr" Width="90px" />
                                        <px:PXGridColumn DataField="EffDate" Width="85px" />
                                        <px:PXGridColumn DataField="ExpDate" Width="85px" />
                                        <px:PXGridColumn DataField="SubcontractSource" Width="95px" CommitChanges="True" />
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <Mode AllowUpload="True" AllowDragRows="true"/>
                            <AutoSize Enabled="True" />
                            <ActionBar ActionsText="False">
                                <CustomItems>
                                    <px:PXToolBarButton Text="Reference Designators" PopupPanel="PanelRef" Enabled="true">
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton DependOnGrid="gridMatl" Key="cmdResetOrder">
                                        <AutoCallBack Command="ResetOrder" Target="ds" />
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton Text="Insert Row" SyncText="false" ImageSet="main" ImageKey="AddNew">
                                        <AutoCallBack Target="gridMatl" Command="AddNew" Argument="1"></AutoCallBack>
                                        <ActionBar ToolBarVisible="External" MenuVisible="true" />
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton Text="Cut Row" SyncText="false" ImageSet="main" ImageKey="Copy">
                                        <AutoCallBack Target="gridMatl" Command="Copy"></AutoCallBack>
                                        <ActionBar ToolBarVisible="External" MenuVisible="true" />
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton Text="Insert Cut Row" SyncText="false" ImageSet="main" ImageKey="Paste">
                                        <AutoCallBack Target="gridMatl" Command="Paste"></AutoCallBack>
                                        <ActionBar ToolBarVisible="External" MenuVisible="true" />
                                    </px:PXToolBarButton>
                                </CustomItems>
                            </ActionBar>
                            <AutoCallBack Enabled="True" Command="Refresh" Target="gridref">
                            </AutoCallBack>
                            <ActionBar ActionsText="False"></ActionBar>
                            <CallbackCommands PasteCommand="PasteLine">
                                <Save PostData="Container" />
                            </CallbackCommands>
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Steps" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == True">
                    <Template>
                        <px:PXGrid ID="gridStep" runat="server" DataSourceID="ds" Height="150px" Width="100%" SkinID="DetailsInTab">
                            <Levels>
                                <px:PXGridLevel DataMember="BomStepRecords" >
                                    <RowTemplate>
                                        <px:PXLayoutRule ID="PXLayoutRule8" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" Merge="true" />
                                        <px:PXTextEdit Size="xl" ID="edStepDescr" runat="server" AllowNull="False" DataField="Descr" />
                                        <px:PXNumberEdit ID="edStepSortOrder" runat="server" DataField="SortOrder" />
                                    </RowTemplate>
                                    <Columns>
                                        <px:PXGridColumn AllowNull="False" DataField="Descr" Width="351px" />
                                        <px:PXGridColumn DataField="LineID" TextAlign="Right" Visible="False" />
                                        <px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="85px" />
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <Mode InitNewRow="True" AllowUpload="True" />
                            <AutoSize Enabled="True" />
                            <ActionBar ActionsText="False">
                            </ActionBar>
                            <AutoCallBack>
                            </AutoCallBack>
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Tools" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == True">
                    <Template>
                        <px:PXGrid ID="gridTool" runat="server" DataSourceID="ds" Height="150px" Width="100%" SkinID="DetailsInTab">
                            <Levels>
                                <px:PXGridLevel DataMember="BomToolRecords" >
                                    <RowTemplate>
                                        <px:PXLayoutRule ID="PXLayoutRule9" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                        <px:PXSelector ID="edToolID" runat="server" DataField="ToolID" AllowEdit="True" />
                                        <px:PXLayoutRule ID="PXLayoutRule10" runat="server" Merge="True" />
                                        <px:PXTextEdit ID="edToolDescr" runat="server" DataField="AMToolMst__Descr" MaxLength="60" />
                                        <px:PXLayoutRule ID="PXLayoutRule11" runat="server" Merge="True" />
                                        <px:PXLabel Size="xs" ID="lblToolQtyReq" runat="server" Encode="True">Qty Required:</px:PXLabel>
                                        <px:PXNumberEdit Size="s" ID="edToolQtyReq" runat="server" DataField="QtyReq" />
                                        <px:PXLayoutRule ID="PXLayoutRule12" runat="server" Merge="True" />
                                        <px:PXLabel Size="xs" ID="lblToolUnitCost" runat="server" Encode="True">Unit Cost:</px:PXLabel>
                                        <px:PXNumberEdit Size="s" ID="edToolUnitCost" runat="server" DataField="UnitCost" />
                                    </RowTemplate>
                                    <Columns>
                                        <px:PXGridColumn DataField="LineID" TextAlign="Right" Visible="False" />
                                        <px:PXGridColumn AllowNull="False" DataField="ToolID" MaxLength="30" Width="180px" AutoCallBack="True" />
                                        <px:PXGridColumn AllowNull="False" DataField="Descr" MaxLength="60" Width="351px" />
                                        <px:PXGridColumn AllowNull="False" DataField="QtyReq" TextAlign="Right" Width="117px" />
                                        <px:PXGridColumn AllowNull="False" DataField="UnitCost" TextAlign="Right" Width="117px" />
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <Mode InitNewRow="True" AllowUpload="True" />
                            <AutoSize Enabled="True" />
                            <ActionBar ActionsText="False">
                            </ActionBar>
                            <AutoCallBack>
                            </AutoCallBack>
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Overhead" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == True">
                    <Template>
                        <px:PXGrid ID="gridOvhd" runat="server" DataSourceID="ds" Height="150px" Width="100%" SkinID="DetailsInTab">
                            <Levels>
                                <px:PXGridLevel DataMember="BomOvhdRecords" >
                                    <RowTemplate>
                                        <px:PXLayoutRule ID="PXLayoutRule13" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                        <px:PXSelector ID="edOvhdID" runat="server" AllowNull="False" DataField="OvhdID" DataKeyNames="OvhdID" AllowEdit="True" />
                                        <px:PXTextEdit ID="edAMOverhead__Descr" runat="server" AllowNull="False" DataField="AMOverhead__Descr" MaxLength="60" />
                                        <px:PXDropDown ID="edAMOverhead__OvhdType" runat="server" AllowNull="False" DataField="AMOverhead__OvhdType" />
                                        <px:PXNumberEdit ID="edOFactor" runat="server" DataField="OFactor" />
                                    </RowTemplate>
                                    <Columns>
                                        <px:PXGridColumn DataField="LineID" TextAlign="Right" Visible="False" />
                                        <px:PXGridColumn AllowNull="False" DataField="OvhdID" MaxLength="10" Width="81px" AutoCallBack="True" DisplayFormat="&gt;AAAAAAAAAA" />
                                        <px:PXGridColumn DataField="AMOverhead__Descr" AllowNull="False" MaxLength="60" Width="351px" />
                                        <px:PXGridColumn AllowNull="False" DataField="AMOverhead__OvhdType" Width="198px" MaxLength="1" RenderEditorText="True" />
                                        <px:PXGridColumn DataField="OFactor" AllowNull="False" TextAlign="Right" Width="117px" />
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <Mode AllowUpload="True"/>
                            <AutoSize Enabled="True" />
                            <ActionBar ActionsText="False">
                            </ActionBar>
                            <AutoCallBack>
                            </AutoCallBack>
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Outside Process" LoadOnDemand="True" RepaintOnDemand="True" BindingContext="TreeNodeSelectedForm" VisibleExp="DataControls[&quot;chkIsOperation&quot;].Value == True">
                    <Template >
                        <px:PXFormView ID="outsideProcessingform" runat="server" CaptionVisible="False" DataMember="OutsideProcessingOperationSelected" DataSourceID="ds" 
                            Width="100%" SyncPosition="True" SkinID="Transparent">
                            <Template>
                                <px:PXLayoutRule ID="PXLayoutRule25" runat="server" GroupCaption="General Settings" StartColumn="True" LabelsWidth="M" ControlSize="L" />
                                <px:PXCheckBox ID="edOutsideProcess" runat="server" DataField="OutsideProcess" CommitChanges="True" />
                                <px:PXCheckBox ID="edDropShippedToVendor" runat="server" DataField="DropShippedToVendor" />
                                <px:PXFormView ID="curySettingsForm" runat="server" SkinID="Inside" RenderStyle="simple" DataSourceID="ds" DataMember="CurySettings_AMBomOper" CaptionVisible="false">
                                    <Template>
                                        <px:PXLayoutRule runat="server" LabelsWidth="125px" />
                                        <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" AutoRefresh="True" AllowAddNew="True" AllowEdit="True" />
                                        <px:PXSegmentMask CommitChanges="True" ID="edVendorLocationID" runat="server" AutoRefresh="True" DataField="VendorLocationID" />
                                    </Template>
                                </px:PXFormView>
                            </Template>
                        </px:PXFormView>
                    </Template>
                </px:PXTabItem>
            </Items>
            <AutoSize Enabled="True" />
            </px:PXTab>

        </Template2>
    </px:PXSplitContainer>
    <!--#include file="~\Pages\AM\Includes\BOMReferenceDesignators.inc"-->
    <!--#include file="~\Pages\AM\Includes\CopyBom.inc"-->
    <!--#include file="~\Pages\AM\Includes\BOMMakeDefault.inc"-->
    <!--#include file="~\Pages\AM\Includes\BOMMakePlanning.inc"-->
    <!--#include file="~\Pages\AM\Includes\BOMCostSummary.inc"-->
    <!--#include file="~\Pages\AM\Includes\BOMCostSummarySettings.inc"-->
</asp:Content>