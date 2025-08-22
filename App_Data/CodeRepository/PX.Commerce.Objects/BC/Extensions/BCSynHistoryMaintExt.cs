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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Commerce.Core;
using PX.Data;
using PX.Data.BQL.Fluent;

namespace PX.Commerce.Objects
{
	public class BCSynHistoryMaintExt : PXGraphExtension<BCSyncHistoryMaint>
	{
		public static bool IsActive() => CommerceFeaturesHelper.CommerceEdition;

		public SelectFrom<BCMatrixOptionsMapping>
		  .InnerJoin<BCSyncStatus>.On<BCSyncStatus.syncID.IsEqual<BCMatrixOptionsMapping.syncID>>
		  .Where<BCSyncStatus.status.IsNotEqual<BCSyncStatusAttribute.synchronized>>
		  .OrderBy<BCSyncStatus.externDescription.Asc, BCMatrixOptionsMapping.syncID.Asc, BCMatrixOptionsMapping.externalOptionName.Asc, BCMatrixOptionsMapping.externalOptionValue.Asc>
		  .View OptionMappings;
	}
}
