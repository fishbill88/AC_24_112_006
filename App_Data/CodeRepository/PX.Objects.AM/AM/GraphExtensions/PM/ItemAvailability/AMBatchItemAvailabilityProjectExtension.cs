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

using PX.Data;
using PX.Objects.AM;

namespace PX.Objects.PM.MaterialManagement.GraphExtensions.ItemAvailability
{
	// TODO: ensure this class is even needed - could project availability be used in AMBatchEntryBase?
	// if yes, then the GetStatusProject's meaningful implementation is missing, otherwise this class should be removed
	public abstract class AMBatchItemAvailabilityProjectExtension<TBatchGraph, TBatchItemAvailExt> : ItemAvailabilityProjectExtension<TBatchGraph, TBatchItemAvailExt, AMMTran, AMMTranSplit>
		where TBatchGraph : AMBatchEntryBase
		where TBatchItemAvailExt : AMBatchItemAvailabilityExtension<TBatchGraph>
	{
		protected override string GetStatusProject(AMMTran line) => null;
	}

	// TODO: ensure this class is even needed - could project availability be used in MaterialEntry?
	[PXProtectedAccess(typeof(MaterialEntry.ItemAvailabilityExtension))]
	public abstract class MaterialEntry_ItemAvailabilityProjectExtension
		: AMBatchItemAvailabilityProjectExtension<MaterialEntry, MaterialEntry.ItemAvailabilityExtension>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<CS.FeaturesSet.materialManagement>();
	}

	// TODO: ensure this class is even needed - could project availability be used in MoveEntry?
	[PXProtectedAccess(typeof(MoveEntry.ItemAvailabilityExtension))]
	public abstract class MoveEntry_ItemAvailabilityProjectExtension
		: AMBatchItemAvailabilityProjectExtension<MoveEntry, MoveEntry.ItemAvailabilityExtension>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<CS.FeaturesSet.materialManagement>();
	}

	// TODO: ensure this class is even needed - could project availability be used in LaborEntry?
	[PXProtectedAccess(typeof(LaborEntry.ItemAvailabilityExtension))]
	public abstract class LaborEntry_ItemAvailabilityProjectExtension
		: AMBatchItemAvailabilityProjectExtension<LaborEntry, LaborEntry.ItemAvailabilityExtension>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<CS.FeaturesSet.materialManagement>();
	}
}
