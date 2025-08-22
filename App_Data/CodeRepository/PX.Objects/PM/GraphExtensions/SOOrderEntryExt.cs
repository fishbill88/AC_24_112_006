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
using PX.Objects.GL;
using PX.Objects.SO;
using System;
using System.Collections.Generic;

namespace PX.Objects.PM
{
    public class SOOrderEntryExt : ProjectCostCenterBase<SOOrderEntry>, IN.ICostCenterSupport<SOLine>
	{
		public int SortOrder => 200;


		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.projectAccounting>();
		}

		public virtual IEnumerable<Type> GetFieldsDependOn()
		{
			yield return typeof(SOLine.isSpecialOrder);
			yield return typeof(SOLine.siteID);
			yield return typeof(SOLine.projectID);
			yield return typeof(SOLine.taskID);
		}

		public bool IsSpecificCostCenter(SOLine line) => line.IsSpecialOrder != true && IsSpecificCostCenter(line.SiteID, line.ProjectID, line.TaskID);

		public virtual int GetCostCenterID(SOLine tran)
		{
			var projectId = tran.ProjectID ?? Base.Document.Current?.ProjectID;
			return (int)FindOrCreateCostCenter(tran.SiteID, projectId, tran.TaskID);
		}

		protected virtual void _(Events.RowUpdated<SOOrder> e)
		{
			if (e.Row == null) return;

			if (!e.Cache.ObjectsEqual<SOOrder.projectID>(e.Row, e.OldRow))
			{
				foreach (SOLine tran in Base.Transactions.Select())
				{
					tran.ProjectID = e.Row.ProjectID;
					Base.Transactions.Update(tran);
				}
			}
		}

		protected virtual void _(Events.FieldVerifying<SOLine, SOLine.salesAcctID> e)
		{
			if (e.Row == null || e.Row.ProjectID == null || e.Row.TaskID == null) return;
			if (ProjectDefaultAttribute.IsNonProject(e.Row.ProjectID)) return;

			var account = Account.PK.Find(Base, e.NewValue as int?);
			if (account != null && account.AccountGroupID == null)
			{
				var exception = new PXSetPropertyException(Messages.NoAccountGroup, PXErrorLevel.Warning, account.AccountCD);
				e.Cache.RaiseExceptionHandling<SOLine.salesAcctID>(e.Row, e.NewValue, exception);
			}
		}

		protected virtual void _(Events.FieldUpdated<SOLine, SOLine.salesAcctID> e)
		{
			if (e.Row == null || e.Row.TaskID != null) return;
			if (Base.IsCopyPasteContext) return;

			e.Cache.SetDefaultExt<SOLine.taskID>(e.Row);
		}
	}
}
