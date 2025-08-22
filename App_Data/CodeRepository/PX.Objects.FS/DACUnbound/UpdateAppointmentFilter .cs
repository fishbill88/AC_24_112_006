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
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.PXGenericInqGrph;


namespace PX.Objects.FS
{
	[System.SerializableAttribute]
	[PXHidden]
	public class UpdateAppointmentFilter : PXBqlTable, IBqlTable
	{
		#region AppointmentID
		public abstract class appointmentID : PX.Data.BQL.BqlInt.Field<appointmentID> { }

		[PXInt]
		public virtual int? AppointmentID { get; set; }
		#endregion

		#region NewResourceID
		public abstract class newResourceID : PX.Data.BQL.BqlInt.Field<newResourceID> { }

		[PXInt]
		public virtual int? NewResourceID { get; set; }
		#endregion

		#region OldResourceID
		public abstract class oldResourceID : PX.Data.BQL.BqlInt.Field<oldResourceID> { }

		[PXInt]
		public virtual int? OldResourceID { get; set; }
		#endregion

		#region NewBegin
		public abstract class newBegin : PX.Data.BQL.BqlDateTime.Field<newBegin> { }

		[PXDateAndTime(UseTimeZone = true)]
		public virtual DateTime? NewBegin { get; set; }
		#endregion

		#region NewEnd
		public abstract class newEnd : PX.Data.BQL.BqlDateTime.Field<newEnd> { }

		[PXDateAndTime(UseTimeZone = true)]
		public virtual DateTime? NewEnd { get; set; }
		#endregion

		#region Confirmed
		public abstract class confirmed : PX.Data.BQL.BqlBool.Field<confirmed> { }

		[PXBool]
		public virtual bool? Confirmed { get; set; }
		#endregion

		#region ValidatedByDispatcher
		public abstract class validatedByDispatcher : PX.Data.BQL.BqlBool.Field<validatedByDispatcher> { }

		[PXBool]
		public virtual bool? ValidatedByDispatcher { get; set; }
		#endregion
	}

}

