/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Delayable, Events, Localizable, Base, DomHelper, BrowserHelper, TemplateHelper, IdHelper, DomSync } from './Editor.js';
import { Row } from './GridBase.js';

const RowsRange = {
  all: 'all',
  visible: 'visible'
};
const PaperFormat = {
  A6: {
    width: 4.11,
    height: 5.81
  },
  A5: {
    width: 5.81,
    height: 8.25
  },
  A4: {
    width: 8.25,
    height: 11.69
  },
  A3: {
    width: 11.69,
    height: 16.49
  },
  Legal: {
    width: 8.5,
    height: 14
  },
  Letter: {
    width: 8.5,
    height: 11
  }
};
const Orientation = {
  portrait: 'portrait',
  landscape: 'landscape'
};
const FileFormat = {
  pdf: 'pdf',
  png: 'png'
};
const FileMIMEType = {
  pdf: 'application/pdf',
  png: 'image/png'
};

/**
 * @module Grid/feature/export/exporter/Exporter
 */
/**
 * Base class for all exporters
 * @mixes Core/localization/Localizable
 * @mixes Core/mixin/Events
 */
class Exporter extends Delayable(Events(Localizable(Base))) {
  static get defaultConfig() {
    return {
      /**
       * `True` to replace all linked CSS files URLs to absolute before passing HTML to the server.
       * When passing a string the current origin of the CSS files URLS will be replaced by the passed origin.
       *
       * For example: css files pointing to /app.css will be translated from current origin to {translateURLsToAbsolute}/app.css
       * @config {Boolean|String}
       * @default
       */
      translateURLsToAbsolute: true,
      /**
       * When true links are converted to absolute by combining current window location (with replaced origin) with
       * resource link.
       * When false links are converted by combining new origin with resource link (for angular)
       * @config {Boolean}
       * @default
       */
      keepPathName: true,
      /**
       * This method accepts all stylesheets (link and style tags) which are supposed to be put on the page. Use this
       * hook method to filter or modify them.
       *
       * ```javascript
       * new Grid({
       *     features: {
       *         pdfExport: {
       *             // filter out inline styles and bootstrap.css
       *             filterStyles: styles => styles.filter(item => !/(link|bootstrap.css)/.test(item))
       *         }
       *     }
       * });
       * ```
       * @param {String[]} styles
       * @returns {String[]} List of stylesheets to put on the exported page
       */
      filterStyles: styles => styles
    };
  }
  /**
   * Template of an extracted page.
   * @param {Object} data Data for the page template
   * @returns {String}
   */
  pageTpl(data) {
    const {
      title,
      header,
      footer,
      styles,
      htmlClasses,
      bodyStyle,
      bodyClasses = [],
      paperHeight,
      paperWidth,
      html
    } = data;
    bodyClasses.push(`b-${this.constructor.type}`);
    // Other classes needed on outer element.
    if (DomHelper.scrollBarWidth) {
      bodyClasses.push('b-visible-scrollbar');
    } else {
      bodyClasses.push('b-overlay-scrollbar');
    }
    if (BrowserHelper.isChrome) {
      bodyClasses.push('b-chrome');
    } else if (BrowserHelper.isSafari) {
      bodyClasses.push('b-safari');
    } else if (BrowserHelper.isFirefox) {
      bodyClasses.push('b-firefox');
    }
    return TemplateHelper.tpl`
            <!DOCTYPE html>
            <html class="${htmlClasses}" style="width: ${paperWidth}in; height: ${paperHeight}in;">
                <head>
                    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
                    <title>${title}</title>
                    ${styles.join('')}
                </head>
                <body class="b-export ${bodyClasses.join(' ')}" style="width: ${paperWidth}in; height: ${paperHeight}in; ${bodyStyle}">
                    <div class="b-export-content">
                        ${header && `<div class="b-export-header" style="width: 100%">${header}</div>`}
                        <div class="b-export-body"><div class="b-export-viewport">${html}</div></div>
                        ${footer && `<div class="b-export-footer" style="width: 100%">${footer}</div>`}
                    </div>
                </body>
            </html>`;
  }
  /**
   * Returns all style-related tags: `<style>` and `<link rel="stylesheet">`
   * @property {String[]}
   * @readonly
   */
  get stylesheets() {
    const me = this;
    if (me._stylesheets) {
      return me._stylesheets;
    }
    const translate = me.translateURLsToAbsolute,
      origin = globalThis.origin,
      styleSheetNodes = Array.from(document.querySelectorAll('link[rel="stylesheet"], style')),
      styles = [];
    styleSheetNodes.forEach(node => {
      node = node.cloneNode(true);
      // put absolute URL to node `href` attribute
      if (translate && node.href) {
        let result;
        if (translate === true) {
          result = node.href;
        }
        // translate is truthy if we are here, which means it is string now
        else if (this.keepPathName) {
          result = node.href.replace(origin, translate);
        } else {
          result = new URL(node.getAttribute('href'), translate);
        }
        node.setAttribute('href', result);
      }
      let styleText = node.outerHTML;
      if (translate && /style/i.test(node.tagName)) {
        const converter = me.getStyleTagURLConverter(translate);
        styleText = styleText.replace(/url\(['"]?(.+?)['"]?\)/g, converter);
      }
      styles.push(styleText);
    });
    styles.push(`<style>
                body, html {
                    overflow: auto;
                }
                body {
                    position: relative;
                    margin: 0;
                }
            </style>`);
    return me._stylesheets = me.filterStyles(styles);
  }
  set stylesheets(value) {
    this._stylesheets = value ? this.filterStyles(value) : value;
  }
  getStyleTagURLConverter(translate) {
    return function (match, url) {
      let result;
      try {
        let base;
        // Filter out local references, e.g. to svg marker
        if (/^#/.test(url)) {
          result = match;
        } else {
          if (translate === true) {
            base = globalThis.location.href;
          }
          // translate is truthy if we are here, which means it is string now
          else if (this.keepPathName) {
            base = globalThis.location.href.replace(globalThis.location.origin, translate);
          } else {
            base = translate;
          }
          result = `url('${new URL(url, base).href}')`;
        }
      } catch (e) {
        result = match;
      }
      return result;
    }.bind(this);
  }
  saveState({
    client
  }) {
    this.state = client.state;
  }
  async restoreState({
    client
  }) {
    const promises = [],
      detachers = new Set();
    detachers.add(client.scrollable.ion({
      scroll() {
        promises.push(client.scrollable.await('scrollEnd'));
      }
    }));
    client.eachSubGrid(({
      header,
      scrollable
    }) => {
      detachers.add(scrollable.ion({
        scroll() {
          promises.push(scrollable.await('scrollEnd'));
        }
      }));
      detachers.add(header.scrollable.ion({
        scroll() {
          promises.push(header.scrollable.await('scrollEnd'));
        }
      }));
    });
    client.state = this.state;
    await Promise.all(promises);
    detachers.forEach(fn => fn());
  }
  beforeExport() {
    // Into this element we will put HTML for export
    this.element = document.createElement('div');
  }
  //region DOM helpers
  cloneElement(element, target = this.element, clear = true) {
    if (clear) {
      target.innerHTML = '';
    }
    target.appendChild(element.cloneNode(true));
    DomHelper.removeEachSelector(target, '.b-grid-row,.b-grid-merged-cells-container');
    const maskEl = target.querySelector('.b-gridbase > .b-mask');
    if (maskEl) {
      maskEl.remove();
    }
  }
  createPlaceholder(el, clear = true, config = {}) {
    if (clear) {
      el.innerHTML = '';
    }
    return DomHelper.createElement(Object.assign({
      parent: el,
      id: IdHelper.generateId('export')
    }, config));
  }
  prepareElement({
    client
  }) {
    const {
      tbar,
      bbar
    } = client;
    if (tbar) {
      this.element.querySelector(`#${tbar.id}`).remove();
    }
    if (bbar) {
      this.element.querySelector(`#${bbar.id}`).remove();
    }
  }
  /**
   * Appends generated header/footer element to the document body to measure their height
   * @param html
   * @returns {Number}
   * @private
   */
  measureElement(html = '') {
    if (html instanceof HTMLElement) {
      html = html.outerHTML;
    }
    const target = DomHelper.createElement({
      parent: document.body,
      style: {
        visibility: 'hidden',
        position: 'absolute'
      },
      // Add html to measure to a div between two other divs to take margin into account
      html: `<div style="height: 1px"></div>${html}<div style="height: 1px"></div>`
    });
    const result = target.offsetHeight - 2;
    target.remove();
    return result;
  }
  // Converts local urls to absolute
  prepareHTML(html) {
    if (html instanceof HTMLElement) {
      html = html.outerHTML;
    }
    const target = DomHelper.createElement({
      parent: document.body,
      style: {
        visibility: 'hidden',
        position: 'absolute'
      },
      html
    });
    const elements = target.querySelectorAll('img');
    for (let i = 0, l = elements.length; i < l; i++) {
      elements[i].setAttribute('src', elements[i].src);
    }
    const result = target.innerHTML;
    target.remove();
    return result;
  }
  getVirtualScrollerHeight(client) {
    let result = 0;
    // If overlay scroll is enabled, this will return 0
    // when disabled, it will report proper virtualScrollers element height
    client.eachSubGrid(subGrid => {
      if (subGrid.overflowingHorizontally) {
        result = DomHelper.scrollBarWidth;
      }
    });
    // If there's a visible scrollbar, need to also take border height into account
    return result === 0 ? result : result + 1;
  }
  //endregion
  inchToPx(value) {
    // 1in = 96px for screens
    // https://developer.mozilla.org/en-US/docs/Web/CSS/length#Absolute_length_units
    return value * 96;
  }
  getScaleValue(base, value) {
    return Math.floor(base * 10000 / value) / 10000;
  }
  getVisibleRowsCount(client) {
    const rowManager = client.rowManager,
      // We cannot rely on `visibleRowCount` property, it is a mere estimate
      firstVisibleIndex = rowManager.rows.indexOf(rowManager.firstVisibleRow),
      lastVisibleIndex = rowManager.rows.indexOf(rowManager.lastVisibleRow);
    let result;
    // first index could be negative only if there are no rendered rows, in which case we return store count
    if (firstVisibleIndex !== -1) {
      // last visible index could be negative if last actually visible row doesn't reach view bottom, i.e.
      // view is not completely filled with rows
      if (lastVisibleIndex === -1) {
        result = client.store.count - firstVisibleIndex;
      } else {
        // for a zero-based index we need to add 1 to the result
        result = lastVisibleIndex - firstVisibleIndex + 1;
      }
    } else {
      result = client.store.count;
    }
    return result;
  }
  async export(config) {
    const me = this;
    let pages;
    me.beforeExport();
    me.saveState(config);
    await me.prepareComponent(config);
    try {
      pages = await me.getPages(config);
    } finally {
      await me.restoreComponent(config);
      me.stylesheets = null;
      // https://app.assembla.com/spaces/bryntum/tickets/9400-scrollrowintoview-promise-is-not-reliable/details
      // Restoring scroll might trigger rows repaint on next animation frame. We are
      // waiting for next animation frame in such case. Covered in SinglePage.t.js
      await new Promise(resolve => me.requestAnimationFrame(resolve));
      await me.restoreState(config);
    }
    return pages;
  }
  async getPages(config) {
    const generator = this.pagesExtractor(config),
      pages = [];
    let step;
    while ((step = await generator.next()) && !step.done) {
      pages.push(step.value);
    }
    return pages;
  }
  // Row buffer has to be adjusted to render complete row set per exported page. See virtual scrolling section in README
  // for more details
  adjustRowBuffer(client) {
    const {
        contentHeight
      } = this.exportMeta,
      {
        rowManager
      } = client;
    this.oldRowManagerConfig = {
      prependRowBuffer: rowManager.prependRowBuffer,
      appendRowBuffer: rowManager.appendRowBuffer
    };
    // render 3 times more rows to get enough to fill exported page
    const adjustedRowBuffer = Math.ceil(contentHeight / rowManager.rowOffsetHeight);
    rowManager.prependRowBuffer = adjustedRowBuffer;
    rowManager.appendRowBuffer = adjustedRowBuffer;
    client.renderRows();
    // In case we are at the bottom of the view and we are increasing buffer, rowmanager might return to the top row
    // In this case we need to force it to render rows at the current scroll position
    client.rowManager.jumpToPosition(client.scrollable.y);
  }
  restoreRowBuffer(client) {
    client.rowManager.prependRowBuffer = this.oldRowManagerConfig.prependRowBuffer;
    client.rowManager.appendRowBuffer = this.oldRowManagerConfig.appendRowBuffer;
    // There is no need to force rendering rows here, those would be updated during applyState call on a grid.
    // More precisely - during setRowHeight method.
  }

  async prepareComponent(config) {
    const me = this,
      {
        client,
        columns,
        rowsRange,
        keepRegionSizes,
        enableDirectRendering
      } = config,
      {
        rowManager
      } = client,
      exportMeta = me.exportMeta = {
        enableDirectRendering,
        totalWidth: 0,
        totalHeight: 0 - (enableDirectRendering ? 0 : me.getVirtualScrollerHeight(client)),
        subGrids: {}
      };
    client.columns.forEach(column => {
      if (columns.includes(column.id)) {
        column.show();
      } else {
        column.hide();
      }
    });
    // Wait for animation frame to catch optional resize event which may occur after hiding columns. Scroll
    // stretcher should be synced before we proceed.
    // Covered by export/Columns.t
    await new Promise(resolve => client.requestAnimationFrame(resolve));
    if (client.rowManager.rowCount > 0) {
      if (rowsRange === RowsRange.all) {
        exportMeta.firstVisibleDataIndex = rowManager.rows[0].dataIndex;
      } else {
        exportMeta.firstVisibleDataIndex = rowManager.firstVisibleRow.dataIndex;
        config.alignRows = true;
      }
      if (!enableDirectRendering) {
        await client.scrollRowIntoView(client.store.getAt(exportMeta.firstVisibleDataIndex), {
          block: 'start'
        });
      }
    }
    const {
      element
    } = me;
    // clone whole grid element to the detached container
    me.cloneElement(client.element);
    me.prepareElement(config);
    let fakeRow;
    // Create fake row, we will use it to render stuff
    if (enableDirectRendering) {
      exportMeta.fakeRow = fakeRow = new Row({
        cls: client.rowCls,
        rowManager: client.rowManager,
        grid: client,
        // use fake indices, they aren't really required
        index: -10,
        dataIndex: -10
      });
    }
    client.eachSubGrid(subGrid => {
      var _subGrid$splitterElem, _subGrid$header, _subGrid$footer;
      // Call private listener to create actual DOM nodes
      if (fakeRow) {
        subGrid.onAddRow({
          rows: [fakeRow],
          isExport: true
        });
        // Trick inline widgets to think they're rendered into component (in fact they are)
        // This is required to remove `Widget.outerCls` classes from the widget
        fakeRow.element.dataset.ownerCmp = client.id;
      }
      const placeHolder = me.createPlaceholder(element.querySelector(`[id="${subGrid.id}"]`), false);
      let width;
      if (keepRegionSizes !== null && keepRegionSizes !== void 0 && keepRegionSizes[subGrid.region]) {
        width = subGrid.element.offsetWidth;
      } else {
        width = subGrid.columns.visibleColumns.reduce((result, column) => {
          if (typeof column.width === 'number') {
            result += column.width;
          } else {
            result += (client.hideHeaders ? client.rowManager.rows[0].getCell(column.id) : column.element).offsetWidth;
          }
          return result;
        }, 0);
      }
      exportMeta.totalWidth += width;
      const splitterWidth = ((_subGrid$splitterElem = subGrid.splitterElement) === null || _subGrid$splitterElem === void 0 ? void 0 : _subGrid$splitterElem.offsetWidth) || 0;
      exportMeta.totalWidth += splitterWidth;
      exportMeta.subGrids[subGrid.region] = {
        id: subGrid.id,
        headerId: ((_subGrid$header = subGrid.header) === null || _subGrid$header === void 0 ? void 0 : _subGrid$header.id) || null,
        footerId: ((_subGrid$footer = subGrid.footer) === null || _subGrid$footer === void 0 ? void 0 : _subGrid$footer.id) || null,
        rows: [],
        splitterWidth,
        placeHolder,
        width
      };
    });
  }
  prepareExportElement() {
    const me = this,
      {
        element,
        exportMeta
      } = me;
    // Exporters may change subGrid width, e.g. when specific date range is exported
    Object.values(exportMeta.subGrids).forEach(({
      width,
      id,
      headerId,
      footerId
    }) => {
      [id, headerId, footerId].forEach(id => {
        if (id) {
          const childElement = element.querySelector(`[id="${id}"]`);
          // SubGrid now always have header & footer, but not always in DOM
          if (childElement) {
            childElement.style.width = `${width}px`;
            childElement.style.flex = '';
          }
        }
      });
    });
    return element.innerHTML;
  }
  async restoreComponent(config) {
    if (this.exportMeta.fakeRow) {
      this.exportMeta.fakeRow.destroy();
      delete this.exportMeta.fakeRow;
    }
  }
  async scrollRowIntoView(client, index) {
    await client.scrollRowIntoView(client.store.getAt(index), {
      block: 'start'
    });
    // #9400 - scrollRowIntoView promise is not reliable
    await new Promise(resolve => this.requestAnimationFrame(resolve));
  }
  collectRow(row) {
    const subGrids = this.exportMeta.subGrids,
      re = /data-owner-cmp=".+?"/;
    Object.entries(row.elements).forEach(([key, value]) => {
      subGrids[key].rows.push([value.outerHTML.replace(re, ''), row.top, row.offsetHeight, new Map() // This one is used to collect events (or in general additional row content)
      ]);
    });
  }

  renderMergedCells(config, fromIndex, toIndex, rows) {
    const me = this,
      {
        client
      } = config,
      {
        subGrids
      } = me.exportMeta,
      // Generate config for merged cells
      domConfigMap = client.features.mergeCells.buildMergedCellsConfig(fromIndex, toIndex, rows);
    for (const subGridName in subGrids) {
      const subGrid = subGrids[subGridName],
        target = document.createElement('div');
      // Render config into temporary element
      DomSync.sync({
        targetElement: target,
        domConfig: {
          children: domConfigMap[subGridName].children
        }
      });
      if (target.childNodes.length) {
        // mergeCells feature uses dependency injection, so in order to get rendered cell content we
        // need to get the cell element matching the range
        target.childNodes.forEach(child => {
          const {
              syncId
            } = child.dataset,
            range = client.features.mergeCells.mergedRanges.find(range => {
              var _range$cellElement;
              return ((_range$cellElement = range.cellElement) === null || _range$cellElement === void 0 ? void 0 : _range$cellElement.parentNode.dataset.syncId) === syncId;
            });
          if (range) {
            // Replace cell content if range is found (it might not be found if there's only one cell)
            child.innerHTML = range.cellElement.outerHTML;
          }
        });
        subGrid.mergedCellsHtml = [target.innerHTML];
      }
    }
  }
}
// HACK: terser/obfuscator doesn't yet support async generators, when processing code it converts async generator to regular async
// function.
/**
 * Pages generator. Value should be string with exported HTML
 * @param {Object} config
 * @returns {AsyncIterableIterator<{value: String, done: Boolean}>}
 * @private
 */
Exporter.prototype.pagesExtractor = async function* pagesExtractor() {
  throw new Error('Implement this method in a subclass');
};
Exporter._$name = 'Exporter';

export { Exporter, FileFormat, FileMIMEType, Orientation, PaperFormat, RowsRange };
//# sourceMappingURL=Exporter.js.map
