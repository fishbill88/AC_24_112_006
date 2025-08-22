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
using PX.Objects.CR;
using PX.Objects.GL.DAC;

namespace PX.Objects.CS.DAC
{
	[PXCacheName(Messages.Company)]
	[Serializable]
	[PXPrimaryGraph(typeof(OrganizationMaint))]
	[PXProjection(typeof(Select2<BAccount,
		InnerJoin<Organization, On<Organization.bAccountID, Equal<BAccount.bAccountID>>>, Where<True, Equal<True>>>), new Type[] { typeof(BAccount) })]
	public partial class OrganizationBAccount : BAccount
	{
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
		public new abstract class defContactID : PX.Data.BQL.BqlInt.Field<defContactID> { }
		public new abstract class defAddressID : PX.Data.BQL.BqlInt.Field<defAddressID> { }
		public new abstract class defLocationID : PX.Data.BQL.BqlInt.Field<defLocationID> { }

		#region OrganizationType
		public new abstract class organizationType : PX.Data.BQL.BqlString.Field<organizationType> { }
		[PXDBString(30, BqlField = typeof(Organization.organizationType))]
		public virtual String OrganizationType { get; set; }
		#endregion
		#region AcctCD
		public new abstract class acctCD : PX.Data.BQL.BqlString.Field<acctCD> { }

		[PXDimensionSelector("COMPANY", 
			typeof(Search2<BAccount.acctCD, 
				InnerJoin<Organization, 
					On<Organization.bAccountID, Equal<BAccount.bAccountID>>>, 
				Where<Match<Organization, Current<AccessInfo.userName>>
					.And<Organization.organizationType.IsNotEqual<OrganizationTypes.group>>>>), 
			typeof(BAccount.acctCD),
			typeof(BAccount.acctCD), typeof(BAccount.acctName))]
		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Company ID", Visibility = PXUIVisibility.SelectorVisible)]
		public override String AcctCD
		{
			get
			{
				return base._AcctCD;
			}
			set
			{
				base._AcctCD = value;
			}
		}
		#endregion
		#region AcctName
		public new abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }

		[PXDBString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Company Name", Visibility = PXUIVisibility.SelectorVisible)]
		public override String AcctName
		{
			get
			{
				return this._AcctName;
			}
			set
			{
				this._AcctName = value;
			}
		}
		#endregion
		#region OrganizationID
		public abstract class organizationID : PX.Data.BQL.BqlInt.Field<organizationID> { }

		[PXDBInt(BqlField = typeof(Organization.organizationID))]
		public virtual int? OrganizationID { get; set; }
		#endregion
	}
}
