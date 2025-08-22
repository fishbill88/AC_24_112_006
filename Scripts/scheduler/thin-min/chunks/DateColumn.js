/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
var n=Object.defineProperty;var d=(a,t,e)=>t in a?n(a,t,{enumerable:!0,configurable:!0,writable:!0,value:e}):a[t]=e;var i=(a,t,e)=>(d(a,typeof t!="symbol"?t+"":t,e),e);import{ColumnStore as p,Column as l}from"./GridBase.js";import{DateHelper as o}from"./Editor.js";class r extends l{static get defaults(){return{format:"L",step:1,minWidth:85,filterType:"date"}}defaultRenderer({value:t}){return t?this.formatValue(t):""}groupRenderer({cellElement:t,groupRowFor:e}){t.innerHTML=this.formatValue(e)}formatValue(t){return typeof t=="string"&&(t=o.parse(t,this.format||void 0)),o.format(t,this.format||void 0)}set format(t){const{editor:e}=this.data;this.set("format",t),e&&(e.format=t)}get format(){return this.get("format")}get defaultEditor(){const t=this,{min:e,max:s,step:m,format:f}=t;return{name:t.field,type:"date",calendarContainerCls:"b-grid-cell-editor-related",weekStartDay:t.grid.weekStartDay,format:f,max:s,min:e,step:m}}}i(r,"$name","DateColumn"),i(r,"type","date"),i(r,"fieldType","date"),i(r,"fields",["format","pickerFormat","step","min","max"]),p.registerColumnType(r,!0),r.exposeProperties(),r._$name="DateColumn";export{r as DateColumn};
//# sourceMappingURL=DateColumn.js.map
