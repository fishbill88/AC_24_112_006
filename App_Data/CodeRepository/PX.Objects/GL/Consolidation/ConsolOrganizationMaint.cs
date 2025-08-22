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
using PX.Objects.GL.DAC;

namespace PX.Objects.GL.Consolidation
{
	public class ConsolOrganizationMaint : PXGraph<ConsolOrganizationMaint>
	{
		public PXSelectJoin<Organization,
						InnerJoin<OrganizationLedgerLink,
							On<Organization.organizationID, Equal<OrganizationLedgerLink.organizationID>>,
						InnerJoin<Ledger,
							On<Ledger.ledgerID, Equal<OrganizationLedgerLink.ledgerID>,
							And<Where<Ledger.consolAllowed, Equal<True>>>>>>> OrganizationRecords;

		public ConsolOrganizationMaint()
		{
		}
	}
}
