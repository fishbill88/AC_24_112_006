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
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.Localizations.CA.IN.DAC;


namespace PX.Objects.Localizations.CA.IN
{
	public class INItemClassMaintCAExt : PXGraphExtension<INItemClassMaint>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		protected virtual void _(Events.RowSelected<INItemClass> e)
		{
			if (e.Row == null)
			{
				return;
			}

			PXUIFieldAttribute.SetVisible<INItemClassExt.t5018Service>(Base.itemclass.Cache, null, (bool)!e.Row.StkItem);
			PXUIFieldAttribute.SetEnabled<INItemClassExt.t5018Service>(Base.itemclass.Cache, null, (bool)!e.Row.StkItem);
		}

		protected virtual void _(Events.FieldUpdated<INItemClass, INItemClass.stkItem> e)
		{
			if (e.Row == null || e.NewValue == null) return;

			e.Cache.SetValueExt<INItemClassExt.t5018Service>(e.Row, false);
		}
	}
}
