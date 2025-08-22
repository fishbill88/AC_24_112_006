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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PM;
using System;

namespace PX.Objects.PR.Standalone
{
	/// <summary>
	/// Standalone DAC related to PR.Objects.PR.PREmployeeClass />
	/// </summary>
	[PXCacheName("Payroll Employee Class")]
	[Serializable]
	public class PREmployeeClass : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PREmployeeClass>.By<employeeClassID>
		{
			public static PREmployeeClass Find(PXGraph graph, string employeeClassID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, employeeClassID, options);
		}

		public new static class FK
		{
			public class WorkCode : PMWorkCode.PK.ForeignKeyOf<PREmployeeClass>.By<workCodeID> { }
		}
		#endregion Keys

		#region EmployeeClassID
		public abstract class employeeClassID : PX.Data.BQL.BqlString.Field<employeeClassID> { }
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		public string EmployeeClassID { get; set; }
		#endregion
		#region WorkCodeID
		public abstract class workCodeID : BqlString.Field<workCodeID> { }
		[PXDBString(PMWorkCode.workCodeID.Length)]
		public string WorkCodeID { get; set; }
		#endregion
	}
}