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
using PX.Objects.IN;
using System;

namespace PX.Objects.AM.CacheExtensions
{
	/// <summary>
	/// Base Manufacturing Cache extension for <see cref="InventoryItem"/>
	/// </summary>
	public sealed class InventoryItemAMBaseExt : PXCacheExtension<InventoryItem>
    {
        public static bool IsActive() => PXAccess.FeatureInstalled<CS.FeaturesSet.manufacturingBase>();

		#region IsProcessMFG
		/// <inheritdoc cref="IsProcessMFG"/>
		public abstract class isProcessMFG : PX.Data.BQL.BqlBool.Field<isProcessMFG> { }
 
		/// <summary>
		/// Is the document linked to the Process Manufacturing feature.
		/// </summary>
		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Process Manufacturing", Visible = false, Enabled = false)]
		public Boolean? IsProcessMFG { get; set; }
		#endregion
    }
}
