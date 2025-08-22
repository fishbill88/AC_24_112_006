using FreightCustomization;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Objects;
using PX.Objects.AP;
using PX.Objects.AP.MigrationMode;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CN.Compliance.PO.CacheExtensions;
using PX.Objects.Common.DAC;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using PX.Objects.SO;
using PX.TM;
using System;
using System.Collections;
using System.Collections.Generic;
using CRLocation = PX.Objects.CR.Standalone.Location;
using SOLine5 = PX.Objects.PO.POOrderEntry.SOLine5;
using SOLineSplit3 = PX.Objects.PO.POOrderEntry.SOLineSplit3;

namespace PX.Objects.PO
{
    public sealed class STPOCreate_Extension : PXGraphExtension<PX.Objects.PO.POCreate>
    {
        public static bool IsActive() => true;

        #region Event Handlers

        public delegate void LinkPOLineToSOLineSplitDelegate(POOrderEntry docgraph, SOLineSplit3 soline, POLine line);
        [PXOverride]
        public void LinkPOLineToSOLineSplit(POOrderEntry docgraph, SOLineSplit3 soline, POLine line, LinkPOLineToSOLineSplitDelegate baseMethod)
        {
            baseMethod(docgraph, soline, line);
            docgraph.Save.Press();
            SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
                  And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
                                  .Select(Base, soline.OrderType, soline.OrderNbr, soline.LineNbr);
            if (soLine != null)
            {
                SOOrder soOrder = SOOrder.PK.Find(Base, soline.OrderType, soline.OrderNbr);
                SOOrderEntry sograph = PXGraph.CreateInstance<SOOrderEntry>();
                sograph.Clear();
                sograph.Document.Current = soOrder;
                SOSetup sosetup = PXSelect<SOSetup>.Select(Base);
                SOSetupExt soExt = sosetup.GetExtension<SOSetupExt>();
                CopyNotesAndAttachmentsToPO<SOOrder, SOLine>(docgraph,
                                                             sograph,
                                                             soOrder,
                                                             new List<SOLine> { soLine },
                                                             docgraph.CurrentDocument.Current,
                                                             new List<POLine> { line },
                                                             (soExt.UsrCopyHeaderNotesToPO ?? false),
                                                             (soExt.UsrCopyHeaderAttachmentsToPO ?? false),
                                                             (soExt.UsrCopyLineNotesToPO ?? false),
                                                             (soExt.UsrCopyLineAttachmentsToPO ?? false));
              
            }
        }

        public delegate String LinkPOLineToBlanketDelegate(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText);
        [PXOverride]
        public String LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText, LinkPOLineToBlanketDelegate baseMethod)
        {
            var result = baseMethod(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);
            POOrderExt poLineExt = docgraph.Document.Current?.GetExtension<POOrderExt>();
            //var docGraphExt = docgraph?.GetExtension<POOrderEntry_Extension>();
            SOOrder soOrder = PXSelect<SOOrder,
                Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
                    And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
                .Select(Base, soline.OrderType, soline.OrderNbr);
            SOOrderExt orderExt = soOrder.GetExtension<SOOrderExt>();

            docgraph?.Document.Cache.SetValueExt<POOrderExt.usrShippingInstructions>(line, orderExt.UsrShippingInstructions);

            return result;
        }

        //public delegate String LinkPOLineToBlanketDelegate(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText);
        //[PXOverride]
        //public String LinkPOLineToBlanket(POLine line, POOrderEntry docgraph, POFixedDemand demand, SOLineSplit3 soline, ref PXErrorLevel ErrorLevel, ref String ErrorText, LinkPOLineToBlanketDelegate baseMethod)
        //{
        //    SOLine soLine = PXSelect<SOLine, Where<SOLine.orderType, Equal<Required<SOLine.orderType>>,
        //          And<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>, And<SOLine.lineNbr, Equal<Required<SOLine.lineNbr>>>>>>
        //                          .Select(Base, soline.OrderType, soline.OrderNbr, soline.LineNbr);
        //    if (soLine != null)
        //    {
        //        SOOrder soOrder = SOOrder.PK.Find(Base, soline.OrderType, soline.OrderNbr);
        //        SOSetup sosetup = PXSelect<SOSetup>.Select(Base);
        //        SOSetupExt soExt = sosetup.GetExtension<SOSetupExt>();
        //        CopyNotesAndAttachmentsToPO<SOOrder, SOLine>(Base, 
        //                                                     soOrder, 
        //                                                     new List<SOLine> { soLine }, 
        //                                                     docgraph.CurrentDocument.Current, 
        //                                                     new List<POLine> { line },
        //                                                     (soExt.UsrCopyHeaderNotesToPO ?? false), 
        //                                                     (soExt.UsrCopyHeaderAttachmentsToPO ?? false),
        //                                                     (soExt.UsrCopyLineNotesToPO ?? false),
        //                                                     (soExt.UsrCopyLineAttachmentsToPO ?? false));
        //    }

        //    return baseMethod(line, docgraph, demand, soline, ref ErrorLevel, ref ErrorText);
        //}

        /// <summary>
        /// Copies header and line notes and attachments from a source document to the POOrder and its lines, based on checkboxes.
        /// </summary>
        /// <typeparam name="TSourceOrder">Type of the source order (e.g., SOOrder)</typeparam>
        /// <typeparam name="TSourceLine">Type of the source line (e.g., SOLine)</typeparam>
        /// <param name="graph">PXGraph context</param>
        /// <param name="sourceOrder">Source order object</param>
        /// <param name="sourceLines">Source lines (IEnumerable)</param>
        /// <param name="poOrder">Target POOrder</param>
        /// <param name="poLines">Target POLine collection (IEnumerable)</param>
        /// <param name="copyHeaderNotes">Copy header notes</param>
        /// <param name="copyHeaderFiles">Copy header attachments</param>
        /// <param name="copyLineNotes">Copy line notes</param>
        /// <param name="copyLineFiles">Copy line attachments</param>
        public static void CopyNotesAndAttachmentsToPO<TSourceOrder, TSourceLine>(
            PXGraph pograph,
            PXGraph sograph,
            TSourceOrder sourceOrder,
            System.Collections.Generic.IEnumerable<TSourceLine> sourceLines,
            POOrder poOrder,
            System.Collections.Generic.IEnumerable<POLine> poLines,
            bool copyHeaderNotes,
            bool copyHeaderFiles,
            bool copyLineNotes,
            bool copyLineFiles)
            where TSourceOrder : class, IBqlTable, new()
            where TSourceLine : class, IBqlTable, new()
        {
            // Copy header notes and attachments if enabled
            if (copyHeaderNotes || copyHeaderFiles)
            {
                PXNoteAttribute.CopyNoteAndFiles(
                    sograph.Caches[typeof(TSourceOrder)], sourceOrder,
                    pograph.Caches[typeof(POOrder)], poOrder,
                    copyNotes: copyHeaderNotes, copyFiles: copyHeaderFiles);
            }

            // Copy line notes and attachments if enabled
            if (copyLineNotes || copyLineFiles)
            {
                var sourceLineList = new System.Collections.Generic.List<TSourceLine>(sourceLines);
                var poLineList = new System.Collections.Generic.List<POLine>(poLines);
                for (int i = 0; i < sourceLineList.Count && i < poLineList.Count; i++)
                {
                    PXNoteAttribute.CopyNoteAndFiles(
                        pograph.Caches[typeof(TSourceLine)], sourceLineList[i],
                        pograph.Caches[typeof(POLine)], poLineList[i],
                        copyNotes: copyLineNotes, copyFiles: copyLineFiles);
                }
            }
        }


        #endregion
    }
}