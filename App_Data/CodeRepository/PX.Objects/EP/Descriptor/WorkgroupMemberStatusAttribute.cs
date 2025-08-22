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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.TM;

namespace PX.Objects.EP
{
	public class WorkgroupMemberStatusAttribute : PXStringListAttribute, IPXFieldDefaultingSubscriber
	{
		public WorkgroupMemberStatusAttribute()
			: base(new (string, string)[] {
				(PermanentActive, Messages.PermanentActive),
				(PermanentInactive, Messages.PermanentInactive),
				(TemporaryActive, Messages.TemporaryActive),
				(TemporaryInactive, Messages.TemporaryInactive),
				(AdHoc, Messages.Adhoc) })
		{
		}

		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			base.FieldSelecting(sender, e);
			var row = (EPTimeActivitiesSummary)e.Row;
			if(row == null)
			{
				return;
			}

			if (row.IsMemberActive != null && row.Status != null)
			{
				e.ReturnValue = GetStatus(row.IsMemberActive, row.Status);
			}

			if (e.ReturnValue == null)
			{
				e.ReturnValue = AdHoc;
			}
		}

		public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{

			var row = (EPTimeActivitiesSummary)e.Row;
			if (row != null)
			{
				var member = SelectFrom<EPCompanyTreeMember>
							.Where<EPCompanyTreeMember.workGroupID.IsEqual<P.AsInt>
							.And<EPCompanyTreeMember.contactID.IsEqual<P.AsInt>>>.View.Select(sender.Graph, row.WorkgroupID, row.ContactID).TopFirst;
				e.NewValue = GetStatus(member?.Active, member?.MembershipType);
			}
		}

		private static string GetStatus(bool? isActive, string membershipType)
		{
			string status = AdHoc;
			if (membershipType == PermanentActive || isActive == true && membershipType == MembershipTypeListAttribute.Permanent)
			{
				status = PermanentActive;
			}
			else if (membershipType == PermanentInactive || isActive == false && membershipType == MembershipTypeListAttribute.Permanent)
			{
				status = PermanentInactive;
			}
			else if (membershipType == TemporaryActive || isActive == true && membershipType == MembershipTypeListAttribute.Temporary)
			{
				status = TemporaryActive;
			}
			else if (membershipType == TemporaryInactive || isActive == false && membershipType == MembershipTypeListAttribute.Temporary)
			{
				status = TemporaryInactive;
			}

			return status;
		}

		public const string PermanentActive = "PERMA";
		public class permanentActive : PX.Data.BQL.BqlString.Constant<permanentActive>
		{
			public permanentActive() : base(PermanentActive) { }
		}

		public const string PermanentInactive = "PERMI";
		public class permanentInactive : PX.Data.BQL.BqlString.Constant<permanentInactive>
		{
			public permanentInactive() : base(PermanentInactive) { }
		}

		public const string TemporaryActive = "TEMPA";
		public class temporaryActive : PX.Data.BQL.BqlString.Constant<temporaryActive>
		{
			public temporaryActive() : base(TemporaryActive) { }
		}

		public const string TemporaryInactive = "TEMPI";
		public class temporaryInactive : PX.Data.BQL.BqlString.Constant<temporaryInactive>
		{
			public temporaryInactive() : base(TemporaryInactive) { }
		}

		public const string AdHoc = "ADHOC";
		public class adHoc : PX.Data.BQL.BqlString.Constant<adHoc>
		{
			public adHoc() : base(AdHoc) { }
		}
	}
}
