/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
var g=Object.defineProperty;var d=(r,t,n)=>t in r?g(r,t,{enumerable:!0,configurable:!0,writable:!0,value:n}):r[t]=n;var o=(r,t,n)=>(d(r,typeof t!="symbol"?t+"":t,n),n);import{Base as h,ObjectHelper as l,StringHelper as m}from"./Editor.js";var p=r=>{var t;return t=class extends(r||h){changeCssVarPrefix(e){return l.assertString(e,"prefix"),e&&!e.endsWith("-")&&(e=e+"-"),e||""}changeCss(e){l.assertObject(e,"css");const s=this;if(!globalThis.Proxy)throw new Error("Proxy not supported");const c=new Proxy({},{get(u,i){var a;return(a=getComputedStyle(s.element||document.documentElement).getPropertyValue(`--${s.cssVarPrefix}${m.hyphenate(i)}`))===null||a===void 0?void 0:a.trim()},set(u,i,a){return(s.element||document.documentElement).style.setProperty(`--${s.cssVarPrefix}${m.hyphenate(i)}`,a),!0}});return e&&(s._element?l.assign(c,e):s.$initialCSS=e),c}updateElement(e,...s){super.updateElement(e,...s),this.$initialCSS&&l.assign(this.css,this.$initialCSS)}get widgetClass(){}},o(t,"$name","Styleable"),o(t,"configurable",{cssVarPrefix:"",css:{}}),t};export{p as Styleable};
//# sourceMappingURL=Styleable.js.map
