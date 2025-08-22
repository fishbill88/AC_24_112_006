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
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Data.BQL;

namespace PX.Objects.Localizations.CA.IN.DAC
{
	/// <summary>
	/// An extension for <see cref="InventoryItem"/> that is used in Canadian Localization.
	/// </summary>
	[Serializable]
	public sealed class InventoryItemExt : PXCacheExtension<InventoryItem>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion

		#region T5018Service
		public abstract class t5018Service : BqlBool.Field<t5018Service> { }
		protected bool? _T5018Service;

		/// <summary>
		/// Flag the indicates that this Item should be considered a construction service for the purpose of
		/// T5018 report generation.
		/// </summary>
		[PXDBBool()]
		[PXUIField(DisplayName = "T5018 Service", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(false, SourceField = typeof(INItemClassExt.t5018Service), CacheGlobal = true, PersistingCheck = PXPersistingCheck.Nothing)]
		public bool? T5018Service
		{
			get
			{
				return this._T5018Service;
			}
			set
			{
				this._T5018Service = value;
			}
		}
		#endregion
	}
}
