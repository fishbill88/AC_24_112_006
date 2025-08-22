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

#nullable enable
using System;

using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CA;
using PX.SM;

namespace PX.Objects.CC
{
	/// <summary>
	/// Represents default POS Terminal of the User in the Branch
	/// </summary>
	[PXCacheName(Messages.DefaultTerminal)]
	[Serializable]
	public class DefaultTerminal : PXBqlTable, IBqlTable
	{
		public class PK : PrimaryKeyOf<DefaultTerminal>.By<userID, branchID, processingCenterID>
		{
			public static DefaultTerminal Find(PXGraph graph, Guid? userID, int? branchID, string processingCenterID) => FindBy(graph, userID, branchID, processingCenterID);
		}

		#region UserID
		public abstract class userID : PX.Data.BQL.BqlGuid.Field<userID> { }
		/// <summary>
		/// The identifier of the <see cref="PX.SM.Users">Users</see> to be used for the employee to sign into the system.
		/// </summary>
		/// <value>
		/// Corresponds to the value of the <see cref="PX.SM.Users.PKID">Users.PKID</see> field.
		/// </value>
		[PXDBGuid(IsKey = true)]
		[PXDefault(typeof(Search<Users.pKID, Where<Users.pKID, Equal<Current<AccessInfo.userID>>>>))]
		public virtual Guid? UserID { get; set; }
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
		/// <summary>Branch ID</summary>
		[PXDBInt(IsKey = true)]
		public virtual int? BranchID
		{
			get;
			set;
		}
		#endregion
		#region ProcessingCenterID
		public abstract class processingCenterID : PX.Data.BQL.BqlString.Field<processingCenterID> { }

		/// <summary>Processing Center ID</summary>
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(CCProcessingCenter.processingCenterID))]
		[PXUIField(DisplayName = "Processing Center ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string? ProcessingCenterID
		{
			get;
			set;
		}
		#endregion
		#region TerminalID
		public abstract class terminalID : PX.Data.BQL.BqlString.Field<terminalID> { }

		/// <summary>POS Terminal ID</summary>
		[PXDBString(36, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Terminal ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string? TerminalID
		{
			get;
			set;
		}
		#endregion
	}
}
