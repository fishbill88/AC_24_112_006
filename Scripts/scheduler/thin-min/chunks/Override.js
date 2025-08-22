/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import{VersionHelper as a}from"./Editor.js";const c={constructor:1,prototype:1,name:1,length:1,arguments:1,caller:1,callee:1,__proto__:1};class l{static apply(r){if(!r.target)throw new Error("Override must specify what it overrides, using static getter target");if(!r.target.class)throw new Error("Override must specify which class it overrides, using target.class");if(!this.shouldApplyOverride(r))return!1;const e=Object.getOwnPropertyNames(r),t=Object.getOwnPropertyNames(r.prototype);return e.splice(e.indexOf("target"),1),this.internalOverrideAll(r.target.class,e,r),this.internalOverrideAll(r.target.class.prototype,t,r.prototype),!0}static internalOverrideAll(r,e,t){Reflect.ownKeys(t).forEach(s=>{if(e.includes(s)&&!c[s]){const o=Object.getOwnPropertyDescriptor(t,s);let i=r,n=null;for(;!n&&i;)n=Object.getOwnPropertyDescriptor(i,s),n||(i=Object.getPrototypeOf(i));n&&this.internalOverride(i,s,o,n)}})}static internalOverride(r,e,t,s){const o=r._overridden=r._overridden||{};o[e]=r[e],s.get?Object.defineProperty(r,e,{enumerable:!1,configurable:!0,get:t.get}):r[e]=t.value}static shouldApplyOverride(r){const e=r.target;if(!e.maxVersion&&!e.minVersion)return!0;if(!e.product)throw new Error("Override must specify product when using versioning");return!(e.maxVersion&&a[e.product].isNewerThan(e.maxVersion)||e.minVersion&&a[e.product].isOlderThan(e.minVersion))}}l._$name="Override";export{l as Override};
//# sourceMappingURL=Override.js.map
