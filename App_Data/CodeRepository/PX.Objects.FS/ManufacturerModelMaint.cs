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

namespace PX.Objects.FS
{
    public class ManufacturerModelMaint : PXGraph<ManufacturerModelMaint, FSManufacturerModel>
    {
        #region Selects
        public 
            PXSelect<FSManufacturerModel,
            Where<
                FSManufacturerModel.manufacturerID, Equal<Optional<FSManufacturerModel.manufacturerID>>>>
            ManufacturerModelRecords;

        public 
            PXSelect<FSManufacturerModel,
            Where<
                FSManufacturerModel.manufacturerModelID, Equal<Current<FSManufacturerModel.manufacturerModelID>>>>
            ManufacturerModelSelected;
		#endregion

		#region CacheAttached
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXSelector(typeof(Search<FSManufacturerModel.manufacturerModelCD,
							Where<
								FSManufacturerModel.manufacturerID, Equal<Current<FSManufacturerModel.manufacturerID>>>>),
						   SubstituteKey = typeof(FSManufacturerModel.manufacturerModelCD))]
		protected virtual void _(Events.CacheAttached<FSManufacturerModel.manufacturerModelCD> e)
		{
		}
		#endregion
	}
}
