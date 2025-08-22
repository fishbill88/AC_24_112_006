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

using PX.Commerce.Core;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CA;
using PX.Objects.CS;
using System.Linq;

namespace PX.Commerce.Amazon.Amazon.Extension
{
	/// <summary>
	/// The graph extension of CATranEntry to handle editability of the <see cref="CAAdj.extRefNbr"/>.
	/// </summary>
	public sealed class CATranEntryExt : PXGraphExtension<CATranEntry>
	{
		/// <summary>
		/// Defines if the extension is active.
		/// </summary>
		/// <returns>True if the extension is active; otherwise - false.</returns>
		public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.amazonIntegration>(); }

		protected void CAAdj_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected base_Handler)
		{
			base_Handler?.Invoke(sender, e);

			if (!(e.Row is CAAdj cashTransaction))
				return;

			var syncDetails = SelectFrom<BCSyncDetail>.Where<BCSyncDetail.localID.IsEqual<@P.AsGuid>>.View
				.Select(this.Base, cashTransaction.NoteID)
				.FirstOrDefault();

			PXUIFieldAttribute.SetEnabled<CAAdj.extRefNbr>(sender, cashTransaction, syncDetails is null);
		}
	}
}
