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

namespace PX.Objects.PM
{
    [Serializable]
	public class RateDefinitionMaint : PXGraph<RateDefinitionMaint>
	{
		
		#region Views/Selects

	    public PXSetup<PMSetup> Setup; 
		public PXFilter<PMRateDefinitionFilter> Filter;
		public PXSelect<PMRateDefinition, 
			Where<PMRateDefinition.rateTypeID, Equal<Current<PMRateDefinitionFilter.rateTypeID>>,
			And<PMRateDefinition.rateTableID, Equal<Current<PMRateDefinitionFilter.rateTableID>>>>,
			OrderBy<Asc<PMRateDefinition.sequence>>> RateDefinitions;
		
		#endregion

		#region Actions/Buttons
        public PXSave<PMRateDefinitionFilter> Save;
        public PXCancel<PMRateDefinitionFilter> Cancel;
		
		public PXAction<PMRateDefinitionFilter> viewRate;
		[PXUIField(DisplayName = Messages.ViewRates, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public void ViewRate()
		{
			if (RateDefinitions.Current != null)
			{
				this.Save.Press();

				RateMaint graph = PXGraph.CreateInstance<RateMaint>();
				graph.RateSequence.Current = PXSelect
					<PMRateSequence, Where<PMRateSequence.rateTableID, Equal<Current<PMRateDefinition.rateTableID>>,
						And<PMRateSequence.rateTypeID, Equal<Current<PMRateDefinition.rateTypeID>>,
							And<PMRateSequence.sequence, Equal<Current<PMRateDefinition.sequence>>>>>>.Select(this);
				if (graph.RateSequence.Current == null)
				{
					var row = graph.RateSequence.Insert();
					graph.RateSequence.SetValueExt<PMRateSequence.rateTableID>(row, RateDefinitions.Current.RateTableID);
					graph.RateSequence.SetValueExt<PMRateSequence.rateTypeID>(row, RateDefinitions.Current.RateTypeID);
					graph.RateSequence.SetValueExt<PMRateSequence.sequence>(row, RateDefinitions.Current.Sequence.ToString());
				}
				
				throw new PXRedirectRequiredException(graph, false, Messages.RateMaint + " - " + Messages.ViewRates);
			}
		}

		#endregion	
		
		#region Event Handlers

	    
		protected virtual void PMRateDefinition_RateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PMRateDefinition row = e.Row as PMRateDefinition;
			if (row != null)
			{
				e.NewValue = Filter.Current.RateTypeID;
			}
		}

		protected virtual void PMRateDefinition_RateTableID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PMRateDefinition row = e.Row as PMRateDefinition;
			if (row != null)
			{
				e.NewValue = Filter.Current.RateTableID;
			}
		}

		protected virtual void PMRateDefinition_Sequence_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PMRateDefinition row = e.Row as PMRateDefinition;
			if (row != null)
			{
				e.NewValue = GetNextSequence();
			}
		}

		#endregion

		private short GetNextSequence()
		{
			PXSelectBase<PMRateDefinition> select = new PXSelect<PMRateDefinition,
				Where<PMRateDefinition.rateTypeID, Equal<Current<PMRateDefinitionFilter.rateTypeID>>,
				And<PMRateDefinition.rateTableID, Equal<Current<PMRateDefinitionFilter.rateTableID>>>>,
				OrderBy<Asc<PMRateDefinition.sequence>>>(this);

			short cx = 1;
			short max = 0;
			foreach (PMRateDefinition rd in select.Select())
			{
				cx++;
				max = Math.Max(rd.Sequence.GetValueOrDefault(), max);
			}

			max++;
			return Math.Max(max, cx);
		}

		#region Local Types
		[PXHidden]
        [Serializable]
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public partial class PMRateDefinitionFilter : PXBqlTable, IBqlTable
		{
            #region RateTableID
            public abstract class rateTableID : PX.Data.BQL.BqlString.Field<rateTableID> { }
            protected String _RateTableID;
            [PXDBString(PMRateTable.rateTableID.Length, IsUnicode = true)]
            [PXUIField(DisplayName = "Rate Table")]
            [PXSelector(typeof(PMRateTable.rateTableID), DescriptionField = typeof(PMRateTable.description))]
            public virtual String RateTableID
            {
                get
                {
                    return this._RateTableID;
                }
                set
                {
                    this._RateTableID = value;
                }
            }
            #endregion

			#region RateTypeID
			public abstract class rateTypeID : PX.Data.BQL.BqlString.Field<rateTypeID> { }
			protected String _RateTypeID;
			[PXDBString(PMRateType.rateTypeID.Length, IsUnicode = true)]
			[PXSelector(typeof(PMRateType.rateTypeID), DescriptionField = typeof(PMRateType.description))]
			[PXUIField(DisplayName = "Rate Type")]
			public virtual String RateTypeID
			{
				get
				{
					return this._RateTypeID;
				}
				set
				{
					this._RateTypeID = value;
				}
			}
			#endregion
		} 
		#endregion
	}
}
