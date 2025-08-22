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

using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.GL;

namespace PX.Objects.PM.GraphExtensions
{
	public class JournalEntryTranRef : PXGraph<JournalEntryTranRef>
	{
		public virtual string GetDocType(APInvoice apDoc, ARInvoice arDoc, GLTran glTran)
		{
			if (apDoc != null)
			{
				switch (apDoc.DocType)
				{
					case APDocType.Invoice:
						return PMOrigDocType.APBill;
					case APDocType.CreditAdj:
						return PMOrigDocType.CreditAdjustment;
					case APDocType.DebitAdj:
						return PMOrigDocType.DebitAdjustment;
					default: return null;
				}
			}
			else if (arDoc != null)
			{
				switch (arDoc.DocType)
				{
					case ARDocType.Invoice:
						return PMOrigDocType.ARInvoice;
					case ARDocType.CreditMemo:
						return PMOrigDocType.CreditMemo;
					case ARDocType.DebitMemo:
						return PMOrigDocType.DebitMemo;
					default: return null;
				}
			}
			else
				return null;
		}

		public virtual Guid? GetNoteID(APInvoice apDoc, ARInvoice arDoc, GLTran glTran)
		{

			if (apDoc != null)
			{
				return apDoc.NoteID;
			}
			else if (arDoc != null)
			{
				return arDoc.NoteID;
			}
			else
				return null;
		}

		public virtual void AssignCustomerVendorEmployee(GLTran glTran, PMTran pmTran)
		{
			pmTran.BAccountID = glTran.ReferenceID;
		}
		
		public virtual void AssignAdditionalFields(GLTran glTran, PMTran pmTran) { }

		public virtual List<TranWithInfo> GetAdditionalProjectTrans(string module, string tranType, string refNbr)
		{
			return new List<TranWithInfo>();
		}
	}

	public class TranWithInfo
	{
		public PMTran Tran { get; private set; }
		public Account Account { get; private set; }
		public PMAccountGroup AccountGroup { get; private set; }
		public PMProject Project { get; private set; }
		public PMTask Task { get; private set; }

		public TranWithInfo(PMTran tran, Account account, PMAccountGroup accountGroup, PMProject project, PMTask task)
		{
			Tran = tran;
			Account = account;
			AccountGroup = accountGroup;
			Project = project;
			Task = task;
		}
	}
}
