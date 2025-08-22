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

using System.Collections.Generic;

namespace PX.Objects.PR.AUF
{
	public class EffRecord : AufRecord
	{
		public EffRecord(int ffdID, string fieldValue) : base(AufRecordType.Eff)
		{
			FfdID = ffdID;

			if (bool.TryParse(fieldValue, out bool boolValue))
			{
				FieldValue = boolValue ? AufConstants.SelectedBox : string.Empty;
			}
			else
			{
				FieldValue = fieldValue;
			}
		}

		public override string ToString()
		{
			object[] lineData =
			{
				FfdID,
				FieldValue
			};

			return FormatLine(lineData);
		}

		public virtual int FfdID { get; set; }
		public virtual string FieldValue { get; set; }
	}

	public class EffIDComparer : IEqualityComparer<EffRecord>
	{
		public bool Equals(EffRecord x, EffRecord y)
		{
			return x.FfdID == y.FfdID;
		}

		public int GetHashCode(EffRecord obj)
		{
			return obj.FfdID.GetHashCode();
		}
	}
}
