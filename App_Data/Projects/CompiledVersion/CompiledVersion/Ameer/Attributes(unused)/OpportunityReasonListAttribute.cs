//using System;
//using System.Collections.Generic;
//using PX.Data;
//using PX.Data.BQL;
//using PX.Objects.CR;
//using CompiledVersion.DAC;

//namespace CompiledVersion.Attributes
//{
//    // Dynamic reason list provider for CROpportunity.Resolution and other2-char fields
//    // It builds the string list from CROpportunityClassStageReason by ClassID + StageCode.
//    // Usage:
//    // [OpportunityReasonByClassStage(typeof(CROpportunity.classID), typeof(CROpportunity.stageID))]
//    // public string Resolution { get; set; }
//    public class OpportunityReasonByClassStageAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
//    {
//        private readonly Type _classIDField;
//        private readonly Type _stageIDField;

//        public OpportunityReasonByClassStageAttribute(Type classIDField, Type stageIDField)
//        : base()
//        {
//            if (classIDField == null || !typeof(IBqlField).IsAssignableFrom(classIDField))
//                throw new PXArgumentException(nameof(classIDField));
//            if (stageIDField == null || !typeof(IBqlField).IsAssignableFrom(stageIDField))
//                throw new PXArgumentException(nameof(stageIDField));

//            _classIDField = classIDField;
//            _stageIDField = stageIDField;
//        }

//        public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
//        {
//            object row = e.Row; if (row == null) return;

//            string classID = sender.GetValue(row, _classIDField.Name) as string;
//            string stageID = sender.GetValue(row, _stageIDField.Name) as string;

//            var pairs = BuildPairs(sender.Graph, classID, stageID);
//            if (pairs != null && pairs.Length > 0)
//            {
//                PXStringListAttribute.SetList(sender, row, _FieldName, pairs);
//            }
//        }

//        private Tuple<string, string>[] BuildPairs(PXGraph graph, string classID, string stageID)
//        {
//            if (string.IsNullOrEmpty(classID) || string.IsNullOrEmpty(stageID))
//                return Array.Empty<Tuple<string, string>>();

//            var values = new List<Tuple<string, string>>();
//            foreach (CROpportunityClassStageReason r in PXSelect<CROpportunityClassStageReason,
//            Where<CROpportunityClassStageReason.classID, Equal<Required<CROpportunityClassStageReason.classID>>,
//            And<CROpportunityClassStageReason.stageCode, Equal<Required<CROpportunityClassStageReason.stageCode>>>>>.
//            Select(graph, classID, stageID))
//            {
//                string code = (values.Count + 1).ToString("00");
//                values.Add(Pair(code, r.Reason));
//            }
//            return values.ToArray();
//        }
//    }
//}
