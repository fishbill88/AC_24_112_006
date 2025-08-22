/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import{Combo as r}from"./Editor.js";class o extends r{static get $name(){return"ProjectCombo"}static get type(){return"projectcombo"}static get configurable(){return{project:null,displayField:"title",valueField:"url",highlightExternalChange:!1,editable:!1}}updateProject(e){var t;(t=e.transport.load)!==null&&t!==void 0&&t.url&&(this.value=e.transport.load.url)}onChange({value:e,userAction:t}){t&&this.project&&(this.project.transport.load.url=e,this.project.load())}}o.initClass(),o._$name="ProjectCombo";export{o as ProjectCombo};
//# sourceMappingURL=ProjectCombo.js.map
