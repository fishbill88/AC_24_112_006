/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using CRLocation = PX.Objects.CR.Standalone.Location;

namespace PX.Objects.GraphExtensions.ExtendBAccount
{
	public abstract class ExtendToVendorGraph<TGraph, TPrimary> : PXGraphExtension<TGraph>
		where TGraph : PXGraph
		where TPrimary : class, IBqlTable, new()
	{
		[PXHidden]
		public class SourceAccount : SourceAccount<ExtendToVendorGraph<TGraph, TPrimary>>
		{
		}
		/// <summary>A class that defines the default mapping of the <see cref="SourceAccount" /> mapped cache extension to a DAC.</summary>
		protected class SourceAccountMapping : IBqlMapping
		{
			/// <exclude />
			protected Type _extension = typeof(SourceAccount);
			/// <exclude />
			public Type Extension => _extension;

			/// <exclude />
			protected Type _table;
			/// <exclude />
			public Type Table => _table;

			/// <summary>Creates the default mapping of the <see cref="SourceAccount" /> mapped cache extension to the specified table.</summary>
			/// <param name="table">A DAC.</param>
			public SourceAccountMapping(Type table)
			{
				_table = table;
			}
			/// <exclude />
			public Type AcctCD = typeof(SourceAccount.acctCD);
			/// <exclude />
			public Type Type = typeof(SourceAccount.type);
			/// <exclude />
			public Type LocaleName = typeof(SourceAccount.localeName);

		}
		/// <summary>Returns the mapping of the <see cref="SourceAccount" /> mapped cache extension to a DAC. This method must be overridden in the implementation class of the base graph.</summary>
		/// <remarks>In the implementation graph for a particular graph, you can either return the default mapping or override the default mapping in this method.</remarks>
		/// <example><para>The following code shows the method that overrides the GetSourceAccountMapping() method in the implementation class. The method returns the default mapping of the SourceAccount mapped cache extension to the CROpportunity DAC.</para>
		///   <code title="Example" lang="CS">
		/// protected override SourceAccountMapping GetSourceAccountMapping()
		/// {
		///       return new SourceAccountMapping(typeof(CROpportunity));
		/// }</code>
		/// </example>
		protected abstract SourceAccountMapping GetSourceAccountMapping();
		/// <summary>A mapping-based view of the <see cref="SourceAccount" /> data.</summary>
		public PXSelectExtension<SourceAccount> Accounts;

		public PXAction<TPrimary> extendToVendor;
		[PXUIField(DisplayName = PX.Objects.AR.Messages.ExtendToVendor, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ExtendToVendor(PXAdapter adapter)
		{
			if (Extend() is { } graph)
			{
				if (!Base.IsContractBasedAPI)
					throw new PXRedirectRequiredException(graph, "Edit Vendor");

				graph.Save.Press();

				Base.Actions.PressCancel();
			}
			return adapter.Get();
		}

		public virtual VendorMaint Extend()
		{
			SourceAccount source = this.Accounts.Current;
			BAccount bacct = (BAccount)this.Accounts.Cache.GetMain(source);			
			if (source != null &&
			    source.Type != BAccountType.VendorType &&
			    source.Type != BAccountType.CombinedType)
				
			{
				Base.Actions["Save"].Press();
				AP.VendorMaint graph = PXGraph.CreateInstance<AP.VendorMaint>();

				// Need to take TimeStamp from this because Current may have outdated tstamp
				// after PXUpdates in this.Persist().
				graph.TimeStamp = Base.TimeStamp;

				if (bacct.Type != BAccountType.CustomerType || bacct.COrgBAccountID == VisibilityRestriction.EmptyBAccountID)
				{
					bacct.BaseCuryID = null;
				}

				AP.VendorR vendor = (AP.VendorR)graph.BAccount.Cache.Extend<BAccount>(bacct);
				graph.BAccount.Cache.SetDefaultExt<AP.VendorR.vOrgBAccountID>(vendor);
				graph.BAccount.Cache.SetDefaultExt<AP.VendorR.curyID>(vendor);
				graph.BAccount.Cache.SetDefaultExt<AP.VendorR.allowOverrideCury>(vendor);
				graph.BAccount.Cache.SetDefaultExt<AP.VendorR.curyRateTypeID>(vendor);
				graph.BAccount.Cache.SetDefaultExt<AP.VendorR.allowOverrideRate>(vendor);
				graph.BAccount.Current = vendor;
				vendor.NoteID = source.NoteID;
				vendor.CreatedByID = bacct.CreatedByID;
				vendor.BAccountClassID = bacct.ClassID;
				vendor.Type = source.Type == BAccountType.CustomerType
					? BAccountType.CombinedType
					: BAccountType.VendorType;
				vendor.LocaleName = source.LocaleName;
				string locationType = source.Type == BAccountType.CustomerType
					? LocTypeList.CombinedLoc
					: LocTypeList.VendorLoc;

				var defLocationExt = graph.GetExtension<AP.VendorMaint.DefLocationExt>();
				CRLocation defLocation = defLocationExt.DefLocation.Select();
				defLocationExt.DefLocation.Cache.RaiseRowSelected(defLocation);
				if (defLocation.VTaxZoneID == null || bacct.IsBranch == true)
				{
					defLocationExt.DefLocation.Cache.SetDefaultExt<CRLocation.vTaxZoneID>(defLocation);
				}
				if (bacct.IsBranch != true
					&& defLocation != null
					&& defLocation.CTaxZoneID != null)
				{
					defLocationExt.DefLocation.Cache.SetValueExt<Location.vTaxZoneID>(defLocation, defLocation.CTaxZoneID);
				}

				defLocationExt.InitLocation(defLocation, locationType, bacct.IsBranch == true);
				if (bacct.IsBranch == true && defLocation != null)
				{
					defLocationExt.DefLocation.Cache.SetDefaultExt<CRLocation.vBranchID>(defLocation);
				}
				defLocation = defLocationExt.DefLocation.Update(defLocation);

				var locationDetails = graph.GetExtension<AP.VendorMaint.LocationDetailsExt>();
				foreach (CRLocation iLoc in locationDetails.Locations.Select())
				{
					if (iLoc.LocationID != defLocation.LocationID)
					{
						defLocationExt.InitLocation(iLoc, locationType, bacct.IsBranch == true);
						locationDetails.Locations.Update(iLoc);
					}
				}

				UDFHelper.CopyAttributes(this.Accounts.Cache, source, graph.BAccount.Cache, graph.BAccount.Current, vendor.ClassID);
				graph.Caches[typeof(CSAnswers)].Clear();
				graph.Caches[typeof(CSAnswers)].ClearQueryCache();

				// change Note type
				object currentNoteText = PXNoteAttribute.GetNote(graph.BAccount.Cache, vendor);
				graph.BAccount.Cache.RaiseFieldUpdating("NoteText", vendor, ref currentNoteText);

				return graph;
			}
			return null;
		}
		public PXDBAction<TPrimary> viewVendor;
		[PXUIField(DisplayName = CR.Messages.ViewVendor, Enabled = false, Visible = true, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewVendor(PXAdapter adapter)
		{
			SourceAccount bacct = this.Accounts.Current;
			if (bacct != null && (bacct.Type == BAccountType.VendorType || bacct.Type == BAccountType.CombinedType))
			{
				AP.VendorMaint editingBO = PXGraph.CreateInstance<AP.VendorMaint>();
				editingBO.BAccount.Current = editingBO.BAccount.Search<AP.VendorR.acctCD>(bacct.AcctCD);
				throw new PXRedirectRequiredException(editingBO, CR.Messages.EditVendor);
			}
			return adapter.Get();
		}

		protected virtual void _(Events.RowSelected<TPrimary> e)
		{
			PXEntryStatus baccountStatus = e.Cache.GetStatus(e.Row);
			SourceAccount source = Accounts.Current;
			if (source == null) return;

			BAccount baccount = (BAccount)Accounts.Cache.GetMain(source);

			bool isCustomerPersisted = true;
			if(baccountStatus != PXEntryStatus.Inserted && baccount?.Type == BAccountType.CustomerType)
			{
				CA.Light.Customer customer = SelectFrom<CA.Light.Customer>
					.Where<CA.Light.Customer.bAccountID.IsEqual<@P.AsInt>>
					.View
					.ReadOnly
					.SelectSingleBound(Base, null, baccount.BAccountID);
				isCustomerPersisted = customer != null;
			}

			extendToVendor.SetEnabled(
				!(source.Type == BAccountType.OrganizationType 
					|| source.Type == BAccountType.VendorType 
					|| source.Type == BAccountType.CombinedType) 
				&& baccountStatus != PXEntryStatus.Inserted
				&& isCustomerPersisted);
			viewVendor.SetEnabled((source.Type == BAccountType.VendorType || source.Type == BAccountType.CombinedType) && baccountStatus != PXEntryStatus.Inserted);
		}
	}

	public abstract class OrganizationUnitExtendToVendor<TGraph, TPrimary> : ExtendToVendorGraph<TGraph, TPrimary>
		where TGraph : PXGraph, IActionsMenuGraph
		where TPrimary : class, IBqlTable, new()
	{
		public override void Initialize()
		{
			base.Initialize();
			Base.ActionsMenuItem.AddMenuAction(viewVendor, nameof(BranchMaint.ChangeID), false);
			Base.ActionsMenuItem.AddMenuAction(extendToVendor, nameof(BranchMaint.ChangeID), false);
		}
	}

}
