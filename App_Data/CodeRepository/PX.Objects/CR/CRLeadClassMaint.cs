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

namespace PX.Objects.CR
{
	/// <exclude/>
	public class CRLeadClassMaint : PXGraph<CRLeadClassMaint, CRLeadClass>
	{
		[PXViewName(Messages.LeadClass)]
		public PXSelect<CRLeadClass> 
			Class;

		[PXHidden]
		public PXSelect<CRLeadClass, 
			Where<CRLeadClass.classID, Equal<Current<CRLeadClass.classID>>>>
			ClassCurrent;

        [PXViewName(Messages.Attributes)]
        public CSAttributeGroupList<CRLeadClass, CRLead> 
			Mapping;

        [PXHidden]
		public PXSelect<CRSetup> 
			Setup;

		protected virtual void _(Events.RowDeleted<CRLeadClass> e)
		{
			var row = e.Row;
			if (row == null) return;
			
            CRSetup s = Setup.Select();

			if (s != null && (s.DefaultLeadClassID == row.ClassID))
			{
				s.DefaultLeadClassID = null;
				Setup.Update(s);
			}
		}

		protected virtual void _(Events.FieldVerifying<CRLeadClass, CRLeadClass.defaultOwner> e)
		{
			var row = e.Row;
			if (row == null) return;

			if (e.NewValue == null)
				e.NewValue = row.DefaultOwner ?? CRDefaultOwnerAttribute.DoNotChange;
		}

		protected virtual void _(Events.FieldUpdated<CRLeadClass, CRLeadClass.defaultOwner> e)
		{
			var row = e.Row;
			if (row == null || e.NewValue == e.OldValue)
				return;

			row.DefaultAssignmentMapID = null;
		}

		protected virtual void _(Events.RowSelected<CRLeadClass> e)
		{
			var row = e.Row;
			if (row == null) return;

			Delete.SetEnabled(CanDelete(row));
		}

		protected virtual void _(Events.RowDeleting<CRLeadClass> e)
		{
			var row = e.Row;
			if (row == null) return;

			if (!CanDelete(row))
			{
				throw new PXException(Messages.RecordIsReferenced);
			}
		}

		private bool CanDelete(CRLeadClass row)
		{
			if (row != null)
			{
				CRLead lead = PXSelect<CRLead,
					Where<CRLead.classID, Equal<Required<CRLeadClass.classID>>>>.
					SelectWindowed(this, 0, 1, row.ClassID);
				if (lead != null)
				{
					return false;
				}
			}
			return true;
		}
	}
}
