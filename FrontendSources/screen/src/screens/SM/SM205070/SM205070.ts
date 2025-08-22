import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection } from 'client-controls';
import { SMPerformanceFilterRow,
	SMPerformanceInfo,
	SMPerformanceInfoSQLSummary,
	SMPerformanceInfoExceptionSummary,
	SMPerformanceInfoTraceEvents,
	SMPerformanceInfoSQL,
	SMPerformanceInfoTraceEvents2,
	SMPerformanceInfoTraceEvents3,
	SMPerformanceInfoSQLSummary2,
	SMPerformanceInfoSQL2,
	SMPerformanceInfoSQL3,
	SMPerformanceInfoTraceEvents4,
	SMPerformanceInfoTraceEvents5 } from './views';

@graphInfo({graphType: 'PX.SM.PerformanceMonitorMaint', primaryView: 'Filter', })
export class SM205070 extends PXScreen {
	Filter = createSingle(SMPerformanceFilterRow);
	Samples = createCollection(SMPerformanceInfo);
	SqlSummary = createCollection(SMPerformanceInfoSQLSummary);
	TraceExceptionsSummary = createCollection(SMPerformanceInfoExceptionSummary);
	TraceEventsLog = createCollection(SMPerformanceInfoTraceEvents);

	Sql = createCollection(SMPerformanceInfoSQL);
	TraceExceptions = createCollection(SMPerformanceInfoTraceEvents2);
	TraceEvents = createCollection(SMPerformanceInfoTraceEvents3);
	SqlSummaryFilter = createSingle(SMPerformanceInfoSQLSummary2);
	SqlSummaryRows = createCollection(SMPerformanceInfoSQL2);
	SqlSummaryRowsPreview = createSingle(SMPerformanceInfoSQL3);
	TraceEventsLogDetails = createSingle(SMPerformanceInfoTraceEvents4);
	TraceExceptionDetails = createCollection(SMPerformanceInfoTraceEvents5);
}