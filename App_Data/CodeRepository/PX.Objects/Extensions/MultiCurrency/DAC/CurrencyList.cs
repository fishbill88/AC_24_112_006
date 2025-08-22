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

namespace PX.Objects.CM.Extensions
{
	/// <summary>
	/// Contains general properties of the currencies that are stored in the system and are used during registration of documents across all modules.
	/// The DAC provides such information as code, description, and precision, which are required for amounts given in a particular currency.
	/// Financial settings associated with the currency (such as various accounts and subaccounts) are stored separately
	/// in the records of the <see cref="Currency"/> type.
	/// Records of this type are edited on the Currencies (CM202000) form (which corresponds to the <see cref="CurrencyMaint"/> graph).
	/// </summary>
	[PXPrimaryGraph(
		new Type[] { typeof(CurrencyMaint) },
		new Type[] { typeof(Select<CurrencyList, 
			Where<CurrencyList.curyID, Equal<Current<CurrencyList.curyID>>>>)
		})]
	[PXCacheName(Messages.Currency)]
	[Serializable]
	public partial class CurrencyList : PXBqlTable, PX.Data.IBqlTable
	{
		#region CuryID
		public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
		[PXDBString(5, IsUnicode = true, IsKey = true, InputMask = ">LLLLL")]
		[PXDefault()]
		[PXUIField(DisplayName = "Currency ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CurrencyList.curyID>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String CuryID
		{
			get;
			set;
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get;
			set;
		}
		#endregion
		#region CurySymbol
		public abstract class curySymbol : PX.Data.BQL.BqlString.Field<curySymbol> { }
		[PXDBString(10, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Currency Symbol")]
		public virtual String CurySymbol
		{
			get;
			set;
		}
		#endregion
		#region CuryCaption
		public abstract class curyCaption : PX.Data.BQL.BqlString.Field<curyCaption> { }
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Currency Caption")]
		public virtual String CuryCaption
		{
			get;
			set;
		}
		#endregion
		#region DecimalPlaces
		public abstract class decimalPlaces : PX.Data.BQL.BqlShort.Field<decimalPlaces> { }
		[PXDBShort(MinValue = 0, MaxValue = 4)]
		[PXDefault((short)2)]
		[PXUIField(DisplayName = "Decimal Precision")]
		public virtual short? DecimalPlaces
		{
			get;
			set;
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get;
			set;
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region isActive
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
		[PXDBBool]
		[PXDefault(true)]
		[PXUIEnabled(typeof(Where<isFinancial, NotEqual<True>>))]
		[PXUIField(DisplayName = "Active", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsActive
		{
			get;
			set;
		}

		#endregion
		#region isFinancial
		public abstract class isFinancial : PX.Data.BQL.BqlBool.Field<isFinancial> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIEnabled(typeof(isActive))]
		[PXUIField(DisplayName = "Use for Accounting", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsFinancial
		{
			get;
			set;
		}
		#endregion
	}
}
