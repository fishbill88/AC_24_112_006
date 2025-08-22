/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import{Grid as i}from"./Grid.js";import"./Tree.js";class r extends i{static get $name(){return"TreeGrid"}static get type(){return"treegrid"}static get configurable(){return{store:{tree:!0}}}updateStore(e,t){if(e&&!e.tree)throw new Error("TreeGrid requires a Store configured with tree : true");super.updateStore(e,t)}}r.initClass(),r._$name="TreeGrid";export{r as TreeGrid};
//# sourceMappingURL=TreeGrid.js.map
