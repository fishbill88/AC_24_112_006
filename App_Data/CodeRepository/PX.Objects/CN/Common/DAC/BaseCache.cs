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

namespace PX.Objects.CN.Common.DAC
{
	[Serializable]
	public class BaseCache : PXBqlTable, INotable
	{
		[PXDBTimestamp]
		public virtual byte[] Tstamp
		{
			get;
			set;
		}

		[PXDBCreatedByID]
		public virtual Guid? CreatedById
		{
			get;
			set;
		}

		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenId
		{
			get;
			set;
		}

		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}

		[PXDBLastModifiedByID(Visibility = PXUIVisibility.Invisible)]
		public virtual Guid? LastModifiedById
		{
			get;
			set;
		}

		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenId
		{
			get;
			set;
		}

		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}

		[PXNote]
		public virtual Guid? NoteID
		{
			get;
			set;
		}
	}
}
