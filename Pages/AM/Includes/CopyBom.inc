	<px:PXSmartPanel ID="PanelCopyBom" runat="server" Caption="Copy BOM" CaptionVisible="True" LoadOnDemand="true" Key="copybomfilter" 
	    AutoCallBack-Enabled="True" AutoCallBack-Target="formCopyBom" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page" >
		<px:PXFormView ID="formCopyBom" runat="server" DataSourceID="ds" CaptionVisible="False"
			DataMember="copybomfilter" SkinID="Transparent" Width="100%">
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule16" runat="server" StartGroup="True" GroupCaption="Copy From" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXMaskEdit ID="edFromBOMID" runat="server" DataField="FromBOMID" />
                <px:PXTextEdit ID="edFromRevisionID" runat="server" DataField="FromRevisionID" />
                <px:PXSegmentMask ID="edFromInvtID" runat="server" DataField="FromInventoryID" />
                <px:PXSegmentMask ID="edFromSubItemID" runat="server" DataField="FromSubItemID" />
                <px:PXSelector ID="edFromSiteID" runat="server" DataField="FromSiteID" />
				<px:PXLayoutRule ID="PXLayoutRule18" runat="server" StartGroup="True" GroupCaption="Copy To" />
				<px:PXMaskEdit ID="edToBOMID" runat="server" DataField="ToBOMID" CommitChanges="True" />
                <px:PXMaskEdit ID="edToRevisionID" runat="server" DataField="ToRevisionID" />
				<px:PXSegmentMask ID="edToInvtID" runat="server" DataField="ToInventoryID" CommitChanges="True" />
                <px:PXSegmentMask ID="edToSubItemCD" runat="server" DataField="ToSubItemCD" CommitChanges="True" />
				<px:PXSelector ID="edToSiteID" runat="server" DataField="ToSiteID" CommitChanges="True" />
                <px:PXCheckBox ID="edUpdateMaterialWarehouse" runat="server" DataField="UpdateMaterialWarehouse" />
			    <px:PXLayoutRule ID="PXLayoutRuleCopyNotes" runat="server" StartGroup="True" GroupCaption="Copy Notes" />
			    <px:PXGroupBox ID="copyNotesGroupBox" runat="server" RenderStyle="Simple" RenderSimple="True">
			        <Template>
			            <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True"/>
			            <px:PXCheckBox ID="PXCopyNotesItem" runat="server" DataField="CopyNotesItem" AlignLeft="True"/>
			            <px:PXCheckBox ID="PXCopyNotesOper" runat="server" DataField="CopyNotesOper" AlignLeft="True"/>
			            <px:PXLayoutRule runat="server" StartColumn="True"/>
			            <px:PXCheckBox ID="PXCopyNotesMatl" runat="server" DataField="CopyNotesMatl" AlignLeft="True"/>
			            <px:PXCheckBox ID="PXCopyNotesStep" runat="server" DataField="CopyNotesStep" AlignLeft="True"/>
			            <px:PXLayoutRule runat="server" StartColumn="True"/>
			            <px:PXCheckBox ID="PXCopyNotesTool" runat="server" DataField="CopyNotesTool" AlignLeft="True"/>
			            <px:PXCheckBox ID="PXCopyNotesOvhd" runat="server" DataField="CopyNotesOvhd" AlignLeft="True"/>
			        </Template>
			    </px:PXGroupBox>
			</Template>
		</px:PXFormView>
        <px:PXPanel ID="PXPanelCopyBomBtn" runat="server" SkinID="Buttons" Width="90%">
			<px:PXButton ID="PXButton3" runat="server" DialogResult="OK" Text="Copy"></px:PXButton>
			<px:PXButton ID="PXButton4" runat="server" DialogResult="Cancel" Text="Cancel"></px:PXButton>
		</px:PXPanel>
	</px:PXSmartPanel>