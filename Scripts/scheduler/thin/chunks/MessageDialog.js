/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Base, DomHelper, GlobalEvents, Rectangle, Delayable, EventHelper, Events, ObjectHelper, BrowserHelper, Widget, Point, Mask, Toast, StringHelper, LocaleManagerSingleton, DateHelper, DayTime, Panel, Tooltip, TimeZoneHelper, ArrayHelper, Field, TextField, Combo, PickerField, Container, Duration, VersionHelper, DomClassList, Popup } from './Editor.js';

/**
 * @module Core/helper/mixin/DragHelperContainer
 */
/**
 * Mixin for DragHelper that handles dragging elements between containers (or rearranging within)
 *
 * @mixin
 * @private
 */
var DragHelperContainer = (Target => class DragHelperContainer extends (Target || Base) {
  static get $name() {
    return 'DragHelperContainer';
  }
  //region Init
  /**
   * Initialize container drag mode.
   * @private
   */
  initContainerDrag() {
    const me = this;
    //use container drag as default mode
    if (!me.mode) {
      me.mode = 'container';
    }
    if (me.mode === 'container' && !me.containers) {
      throw new Error('Container drag mode must specify containers');
    }
  }
  //endregion
  //region Grab, update, finish
  /**
   * Grab an element which can be dragged between containers.
   * @private
   * @param event
   * @returns {Boolean}
   */
  grabContainerDrag(event) {
    const me = this;
    // allow specified selectors to prevent drag
    if (!me.ignoreSelector || !event.target.closest(me.ignoreSelector)) {
      // go up from "handle" to draggable element
      const element = DomHelper.getAncestor(event.target, me.containers, me.outerElement);
      if (element) {
        const box = element.getBoundingClientRect();
        me.context = {
          element,
          valid: true,
          action: 'container',
          offsetX: event.pageX - box.left,
          offsetY: event.pageY - box.top,
          originalPosition: {
            parent: element.parentElement,
            prev: element.previousElementSibling,
            next: element.nextElementSibling
          }
        };
      }
      return true;
    }
    return false;
  }
  /**
   * Starts dragging, called when mouse moves first time after grabbing
   * @private
   * @param event
   */
  startContainerDrag(event) {
    var _outerWidgetEl$parent;
    const me = this,
      {
        context,
        floatRootOwner
      } = me,
      {
        element: dragElement
      } = context,
      clonedNode = dragElement.cloneNode(true),
      box = dragElement.getBoundingClientRect(),
      outerWidgetEl = floatRootOwner === null || floatRootOwner === void 0 ? void 0 : floatRootOwner.element.closest('.b-outer');
    // init drag proxy
    clonedNode.classList.add(me.dragProxyCls);
    clonedNode.classList.add(me.draggingCls);
    // Append drag proxy to float root, fall back to root context node
    ((floatRootOwner === null || floatRootOwner === void 0 ? void 0 : floatRootOwner.floatRoot) || DomHelper.getRootElement(dragElement)).appendChild(clonedNode);
    context.dragProxy = clonedNode;
    // Always set the proxy element width manually, drag target could be sized with flex or % width
    clonedNode.style.width = box.width + 'px';
    clonedNode.style.height = box.height + 'px';
    DomHelper.setTranslateXY(context.dragProxy, box.left, box.top);
    // style dragged element
    context.dragging = dragElement;
    dragElement.classList.add(me.dropPlaceholderCls);
    // If element being dragged is also a child of the float root, add +1 to the cloned node z-index
    if (outerWidgetEl !== null && outerWidgetEl !== void 0 && (_outerWidgetEl$parent = outerWidgetEl.parentElement) !== null && _outerWidgetEl$parent !== void 0 && _outerWidgetEl$parent.matches('.b-float-root')) {
      clonedNode.style.zIndex = floatRootOwner.floatRootMaxZIndex + 1;
    }
  }
  /**
   * Move the placeholder element into its new position on valid drag.
   * @private
   * @param event
   */
  updateContainerDrag(event) {
    var _context$dragging;
    const me = this,
      {
        context
      } = me;
    if (!context.started || !context.targetElement) {
      return;
    }
    const containerElement = DomHelper.getAncestor(context.targetElement, me.containers, 'b-gridbase'),
      willLoseFocus = (_context$dragging = context.dragging) === null || _context$dragging === void 0 ? void 0 : _context$dragging.contains(DomHelper.getActiveElement(context.dragging));
    if (containerElement && DomHelper.isDescendant(context.element, containerElement)) {
      // dragging over part of self, do nothing
      return;
    }
    // The dragging element contains focus, and moving it within the DOM
    // will cause focus loss which might affect an encapsulating autoClose Popup.
    // Prevent focus loss handling during the DOM move.
    if (willLoseFocus) {
      GlobalEvents.suspendFocusEvents();
    }
    if (containerElement && context.valid) {
      me.moveNextTo(containerElement, event);
    } else {
      // dragged outside of containers, revert position
      me.revertPosition();
    }
    if (willLoseFocus) {
      GlobalEvents.resumeFocusEvents();
    }
    event.preventDefault();
  }
  /**
   * Finalize drag, fire drop.
   * @private
   * @param event
   * @fires drop
   */
  finishContainerDrag(event) {
    const me = this,
      {
        context
      } = me,
      // extracting variables to make code more readable
      {
        dragging,
        dragProxy,
        valid,
        draggedTo,
        insertBefore,
        originalPosition
      } = context;
    if (dragging) {
      // needs to have a valid target
      context.valid = valid && draggedTo && (
      // no drop on self or parent
      dragging !== insertBefore || originalPosition.parent !== draggedTo);
      context.finalize = (valid = context.valid) => {
        // revert if invalid (and context still exists, might have been aborted from outside)
        if (!valid && me.context) {
          me.revertPosition();
        }
        dragging.classList.remove(me.dropPlaceholderCls);
        dragProxy.remove();
        me.reset();
      };
      // allow async finalization by setting async to true on context in drop handler,
      // requires implementer to call context.finalize later to finish the drop
      context.async = false;
      me.trigger('drop', {
        context,
        event
      });
      if (!context.async) {
        // finalize immediately
        context.finalize();
      }
    }
  }
  /**
   * Aborts a drag operation.
   * @private
   * @param {Boolean} [invalid]
   * @param {Object} [event]
   * @param {Boolean} [silent]
   */
  abortContainerDrag(invalid = false, event = null, silent = false) {
    const me = this,
      {
        context
      } = me;
    if (context.dragging) {
      context.dragging.classList.remove(me.dropPlaceholderCls);
      context.dragProxy.remove();
      me.revertPosition();
    }
    if (!silent) {
      me.trigger(invalid ? 'drop' : 'abort', {
        context,
        event
      });
    }
    me.reset();
  }
  //endregion
  //region Helpers
  /**
   * Updates the drag proxy position.
   * @private
   * @param event
   */
  updateContainerProxy(event) {
    const me = this,
      {
        context
      } = me,
      proxy = context.dragProxy;
    let newX = event.pageX - context.offsetX,
      newY = event.pageY - context.offsetY;
    if (typeof me.minX === 'number') {
      newX = Math.max(me.minX, newX);
    }
    if (typeof me.maxX === 'number') {
      newX = Math.min(me.maxX - proxy.offsetWidth, newX);
    }
    if (typeof me.minY === 'number') {
      newY = Math.max(me.minY, newY);
    }
    if (typeof me.maxY === 'number') {
      newY = Math.min(me.maxY - proxy.offsetHeight, newY);
    }
    if (me.lockX) {
      DomHelper.setTranslateY(proxy, newY);
    } else if (me.lockY) {
      DomHelper.setTranslateX(proxy, newX);
    } else {
      DomHelper.setTranslateXY(proxy, newX, newY);
    }
    let targetElement;
    if (event.type === 'touchmove') {
      const touch = event.changedTouches[0];
      targetElement = DomHelper.elementFromPoint(touch.clientX, touch.clientY);
    } else {
      targetElement = event.target;
    }
    context.targetElement = targetElement;
  }
  /**
   * Positions element being dragged in relation to targetElement.
   * @private
   * @param targetElement
   * @param event
   */
  moveNextTo(targetElement, event) {
    const {
        context
      } = this,
      dragElement = context.dragging,
      parent = targetElement.parentElement;
    if (targetElement !== dragElement) {
      // dragged over a container and not over self, calculate where to insert
      const centerX = Rectangle.from(targetElement).center.x;
      if (this.isRTL && event.pageX > centerX || !this.isRTL && event.pageX < centerX) {
        // dragged left of target center, insert before
        parent.insertBefore(dragElement, targetElement);
        context.insertBefore = targetElement;
      } else {
        // dragged right of target center, insert after
        if (targetElement.nextElementSibling) {
          // check that not dragged to the immediate left of self. in such case, position should not change
          if (targetElement.nextElementSibling !== dragElement) {
            context.insertBefore = targetElement.nextElementSibling;
            parent.insertBefore(dragElement, targetElement.nextElementSibling);
          } else if (!context.insertBefore && dragElement.parentElement.lastElementChild !== dragElement) {
            // dragged left initially, should stay in place (checked in finishContainerDrag)
            context.insertBefore = targetElement.nextElementSibling;
          }
        } else {
          parent.appendChild(dragElement);
          context.insertBefore = null;
        }
      }
      context.draggedTo = parent;
    }
  }
  /**
   * Moves element being dragged back to its original position.
   * @private
   */
  revertPosition() {
    const {
        context
      } = this,
      {
        dragging
      } = context,
      {
        parent,
        next
      } = context.originalPosition;
    // revert to correct location
    if (next) {
      const isNoop = next.previousSibling === dragging || !next && dragging === parent.lastChild;
      if (!isNoop) {
        parent.insertBefore(dragging, next);
      }
    } else {
      parent.appendChild(dragging);
    }
    // no target container
    context.draggedTo = null;
  }
  //endregion
});

/**
 * @module Core/helper/mixin/DragHelperTranslate
 */
const noScroll = {
  pageXOffset: 0,
  pageYOffset: 0
};
/**
 * Mixin for DragHelper that handles repositioning (translating) an element within its container
 *
 * @mixin
 * @private
 */
var DragHelperTranslate = (Target => class DragHelperTranslate extends Delayable(Target || Base) {
  static get $name() {
    return 'DragHelperTranslate';
  }
  static get configurable() {
    return {
      positioning: null,
      // Private config that disables updating elements position, for when data is live updated during drag,
      // leading to element being redrawn
      skipUpdatingElement: null
    };
  }
  //region Init
  /**
   * Initialize translation drag mode.
   * @private
   */
  initTranslateDrag() {
    const me = this;
    if (!me.isElementDraggable && me.targetSelector) {
      me.isElementDraggable = element => element.closest(me.targetSelector);
    }
  }
  //endregion
  //region Grab, update, finish
  /**
   * Grab an element which can be moved using translation.
   * @private
   * @param event
   * @returns {Boolean}
   */
  grabTranslateDrag(event) {
    const element = this.getTarget(event);
    if (element) {
      this.context = {
        valid: true,
        element,
        startPageX: event.pageX,
        startPageY: event.pageY,
        startClientX: event.clientX,
        startClientY: event.clientY
      };
      return true;
    }
    return false;
  }
  getTarget(event) {
    return event.target.closest(this.targetSelector);
  }
  getX(element) {
    if (this.positioning === 'absolute') {
      // Read left style rather than offsetLeft as it is impacted by margin-left style (Gantt milestones)
      return parseFloat(element.style.left, 10);
    } else {
      return DomHelper.getTranslateX(element);
    }
  }
  getY(element) {
    if (this.positioning === 'absolute') {
      return parseFloat(element.style.top, 10);
    } else {
      return DomHelper.getTranslateY(element);
    }
  }
  getXY(element) {
    if (this.positioning === 'absolute') {
      return [element.offsetLeft, element.offsetTop];
    } else {
      return DomHelper.getTranslateXY(element);
    }
  }
  setXY(element, x, y) {
    if (this.skipUpdatingElement) {
      return;
    }
    if (this.positioning === 'absolute') {
      element.style.left = x + 'px';
      element.style.top = y + 'px';
    } else {
      DomHelper.setTranslateXY(element, x, y);
    }
  }
  /**
   * Start translating, called on first mouse move after dragging
   * @private
   * @param event
   */
  startTranslateDrag(event) {
    const me = this,
      {
        context,
        outerElement,
        proxySelector
      } = me,
      // When cloning an element to be dragged, we place it in BODY by default
      dragWithin = me.dragWithin = me.dragWithin || me.cloneTarget && document.body;
    let element = context.dragProxy || context.element;
    const grabbed = element,
      grabbedParent = element.parentElement;
    if (me.cloneTarget) {
      const elementToClone = proxySelector ? element.querySelector(proxySelector) : element,
        {
          width,
          height,
          x: proxyX,
          y: proxyY
        } = Rectangle.from(elementToClone, dragWithin);
      element = me.createProxy(element);
      let x = proxyX,
        y = proxyY;
      // Match the grabbed element's size and position.
      if (me.autoSizeClonedTarget) {
        element.style.width = `${width}px`;
        element.style.height = `${height}px`;
      }
      element.classList.add(me.dragProxyCls, me.draggingCls);
      // Remove some irrelevant CSS classes
      element.classList.remove('b-hover', 'b-selected', 'b-focused');
      dragWithin.appendChild(element);
      if (!me.autoSizeClonedTarget || proxySelector) {
        const
          // Center proxy at cursor position, we assume app is applying styles to the element (inline or CSS)
          proxyRect = element.getBoundingClientRect(),
          {
            x: dragWithinX,
            y: dragWithinY
          } = dragWithin.getBoundingClientRect(),
          localX = event.clientX - dragWithinX,
          // Body may have a non-zero top
          localY = event.clientY - dragWithinY + (dragWithin !== document.body ? document.body.getBoundingClientRect().y : 0);
        x = localX - proxyRect.width / 2;
        y = localY - proxyRect.height / 2;
        // When proxy is centered, update to not use original mousedown coordinates, but the first mouse move triggering the drag
        context.startPageX = event.pageX;
        context.startPageY = event.pageY;
      }
      me.setXY(element, x, y);
      grabbed.classList.add('b-drag-original');
      if (me.hideOriginalElement) {
        grabbed.classList.add('b-hidden');
      }
    }
    element.classList.add(me.draggingCls);
    Object.assign(context, {
      // The element which we're moving, could be a cloned version of grabbed, or the grabbed element itself
      element,
      // The original element upon which the mousedown event triggered a drag operation
      grabbed,
      // The parent of the original element where the pointerdown was detected - to be able to restore after an invalid drop
      grabbedParent,
      // The next sibling of the original element where the pointerdown was detected - to be able to restore after an invalid drop
      grabbedNextSibling: element.nextElementSibling,
      // elements position within parent element
      elementStartX: me.getX(element),
      elementStartY: me.getY(element),
      elementX: DomHelper.getOffsetX(element, dragWithin || outerElement),
      elementY: DomHelper.getOffsetY(element, dragWithin || outerElement),
      scrollX: 0,
      scrollY: 0,
      scrollManagerElementContainsDragProxy: !me.cloneTarget || dragWithin === outerElement
    });
    if (dragWithin) {
      context.parentElement = element.parentElement;
      if (dragWithin !== element.parentElement) {
        dragWithin.appendChild(element);
      }
      me.updateTranslateProxy(event);
    }
  }
  // When drag has started, create proxy versions (if applicable) and store original positions of all related elements
  // to be able to animate back to these positions in case of an aborted drag
  onDragStarted() {
    var _relatedElements;
    const me = this,
      {
        context
      } = me;
    let {
      relatedElements
    } = context;
    // For unified proxy mode - add a CSS class to the 'main' dragging element to be able to have it be on top of related elements with z-index
    if (me.unifiedProxy) {
      context.element.classList.add('b-drag-main', 'b-drag-unified-proxy');
    }
    if (((_relatedElements = relatedElements) === null || _relatedElements === void 0 ? void 0 : _relatedElements.length) > 0) {
      context.relatedElStartPos = [];
      context.relatedElDragFromPos = [];
      const {
        proxySelector
      } = me;
      let [elementStartX, elementStartY] = [context.elementStartX, context.elementStartY];
      // Store reference to original elements (may need cleanup to remove CSS classes after drop)
      context.originalRelatedElements = relatedElements;
      // Create clone proxy elements of all related elements
      relatedElements = context.relatedElements = relatedElements.map((relatedEl, i) => {
        const proxyTemplateElement = proxySelector ? relatedEl.querySelector(proxySelector) : relatedEl,
          {
            x,
            y,
            width,
            height
          } = Rectangle.from(proxyTemplateElement, me.dragWithin),
          relatedElementToDrag = me.cloneTarget ? me.createProxy(relatedEl) : relatedEl;
        relatedElementToDrag.classList.add(me.draggingCls);
        // Remove some irrelevant CSS classes
        relatedElementToDrag.classList.remove('b-hover', 'b-selected', 'b-focused');
        if (me.cloneTarget) {
          // Match the original related element's position.
          me.setXY(relatedElementToDrag, x, y);
          me.dragWithin.appendChild(relatedElementToDrag);
          relatedElementToDrag.classList.add(me.dragProxyCls);
          // Optionally also match the original related element's size
          if (me.autoSizeClonedTarget) {
            relatedElementToDrag.style.width = `${width}px`;
            relatedElementToDrag.style.height = `${height}px`;
          }
          if (me.hideOriginalElement) {
            relatedEl.classList.add('b-hidden');
          }
          relatedEl.classList.add('b-drag-original');
        }
        context.relatedElStartPos[i] = context.relatedElDragFromPos[i] = me.getXY(relatedElementToDrag);
        if (me.unifiedProxy) {
          relatedElementToDrag.classList.add('b-drag-unified-animation', 'b-drag-unified-proxy');
          // Move into cascade and cache the dragFrom pos
          elementStartX += me.unifiedOffset;
          elementStartY += me.unifiedOffset;
          me.setXY(relatedElementToDrag, elementStartX, elementStartY);
          context.relatedElDragFromPos[i] = [elementStartX, elementStartY];
          relatedElementToDrag.style.zIndex = 100 - i;
        }
        return relatedElementToDrag;
      });
      // Move the selected events into a unified cascade.
      if (me.unifiedProxy && relatedElements && relatedElements.length > 0) {
        // Animate related elements should into the position
        EventHelper.onTransitionEnd({
          element: relatedElements[0],
          property: 'transform',
          handler() {
            relatedElements.forEach(el => el.classList.remove('b-drag-unified-animation'));
          },
          thisObj: me,
          once: true
        });
      }
    }
  }
  /**
   * Limit translation to outer bounds and specified constraints
   * @private
   * @param element
   * @param x
   * @param y
   * @returns {{constrainedX: *, constrainedY: *}}
   */
  applyConstraints(element, x, y) {
    const me = this,
      {
        constrain,
        dragWithin
      } = me,
      {
        pageXOffset,
        pageYOffset
      } = dragWithin === document.body ? globalThis : noScroll;
    // limit to outer elements edges
    if (dragWithin && constrain) {
      if (x < 0) {
        x = 0;
      }
      if (x + element.offsetWidth > dragWithin.scrollWidth) {
        x = dragWithin.scrollWidth - element.offsetWidth;
      }
      if (y < 0) {
        y = 0;
      }
      if (y + element.offsetHeight > dragWithin.scrollHeight) {
        y = dragWithin.scrollHeight - element.offsetHeight;
      }
    }
    // limit horizontally
    if (typeof me.minX === 'number') {
      x = Math.max(me.minX + pageXOffset, x);
    }
    if (typeof me.maxX === 'number') {
      x = Math.min(me.maxX + pageXOffset, x);
    }
    // limit vertically
    if (typeof me.minY === 'number') {
      y = Math.max(me.minY + pageYOffset, y);
    }
    if (typeof me.maxY === 'number') {
      y = Math.min(me.maxY + pageYOffset, y);
    }
    return {
      constrainedX: x,
      constrainedY: y
    };
  }
  /**
   * Update elements translation on mouse move.
   * @private
   * @param {MouseEvent} event
   * @param {Object} scrollManagerConfig
   */
  updateTranslateProxy(event, scrollManagerConfig) {
    const me = this,
      {
        lockX,
        lockY,
        context
      } = me,
      element = context.dragProxy || context.element,
      {
        relatedElements,
        relatedElDragFromPos
      } = context;
    // If we are cloning the dragged element outside of the element(s) monitored by the ScrollManager, then no need
    // to take the scrollManager scroll values into account since it is only relevant when dragProxy is inside
    // the Grid (where scroll manager operates).
    if (context.scrollManagerElementContainsDragProxy && scrollManagerConfig) {
      context.scrollX = scrollManagerConfig.getRelativeLeftScroll(element);
      context.scrollY = scrollManagerConfig.getRelativeTopScroll(element);
    }
    context.pageX = event.pageX;
    context.pageY = event.pageY;
    context.clientX = event.clientX;
    context.clientY = event.clientY;
    let newX = context.elementStartX + event.pageX - context.startPageX + context.scrollX,
      newY = context.elementStartY + event.pageY - context.startPageY + context.scrollY;
    // First let outside world apply snapping
    if (me.snapCoordinates) {
      const snapped = me.snapCoordinates({
        element,
        newX,
        newY
      });
      newX = snapped.x;
      newY = snapped.y;
    }
    // Now constrain coordinates
    const {
      constrainedX,
      constrainedY
    } = me.applyConstraints(element, newX, newY);
    if (context.started || constrainedX !== newX || constrainedY !== newY) {
      me.setXY(element, lockX ? undefined : constrainedX, lockY ? undefined : constrainedY);
    }
    if (relatedElements) {
      const deltaX = lockX ? 0 : constrainedX - context.elementStartX,
        deltaY = lockY ? 0 : constrainedY - context.elementStartY;
      relatedElements.forEach((r, i) => {
        const [x, y] = relatedElDragFromPos[i];
        me.setXY(r, x + deltaX, y + deltaY);
      });
    }
    context.newX = constrainedX;
    context.newY = constrainedY;
  }
  /**
   * Finalize drag, fire drop.
   * @private
   * @param event
   * @fires drop
   */
  async finishTranslateDrag(event) {
    const me = this,
      context = me.context,
      {
        target
      } = event,
      xChanged = !me.lockX && Math.round(context.newX) !== Math.round(context.elementStartX),
      yChanged = !me.lockY && Math.round(context.newY) !== Math.round(context.elementStartY),
      element = context.dragProxy || context.element,
      {
        relatedElements
      } = context;
    if (!me.ignoreSamePositionDrop || xChanged || yChanged) {
      if (context.valid === false) {
        await me.abortTranslateDrag(true, event);
      } else {
        const targetRect = !me.allowDropOutside && Rectangle.from(me.dragWithin || me.outerElement);
        if (targetRect && (typeof me.minX !== 'number' && me.minX !== true && event.pageX < targetRect.left || typeof me.maxX !== 'number' && me.maxX !== true && event.pageX > targetRect.right || typeof me.minY !== 'number' && me.minY !== true && event.pageY < targetRect.top || typeof me.maxY !== 'number' && me.maxY !== true && event.pageY > targetRect.bottom)) {
          // revert location when dropped outside allowed element
          await me.abortTranslateDrag(true, event);
        } else {
          context.finalize = async (valid = context.valid) => {
            // In case someone tries to finalize twice
            if (context.finalized) {
              console.warn('DragHelper: Finalizing already finalized drag');
              return;
            }
            context.finalized = true;
            // abort if invalid (and context still exists, might have been aborted from outside)
            if (!valid && me.context) {
              // abort if flagged as invalid, without triggering abort or drop again
              await me.abortTranslateDrag(true, null, true);
            }
            if (!me.isDestroyed) {
              me.trigger('dropFinalized', {
                context,
                event,
                target
              });
              me.reset();
            }
            if (!me.cloneTarget && element.parentElement !== context.grabbedParent) {
              // If the dragged element was moved to another parent element, remove the transform style
              [element, ...(relatedElements || [])].forEach(el => el.style.transform = '');
            }
          };
          // allow async finalization by setting async to true on context in drop handler,
          // requires implementer to call context.finalize later to finish the drop
          context.async = false;
          await me.trigger('drop', {
            context,
            event,
            target
          });
          if (!context.async) {
            // finalize immediately
            await context.finalize();
          }
        }
      }
    } else {
      // no change, abort
      me.abortTranslateDrag(false, event);
    }
  }
  /**
   * Abort translation
   * @private
   * @param invalid
   * @fires abort
   */
  async abortTranslateDrag(invalid = false, event = null, silent = false) {
    var _me$scrollManager, _me$context;
    const me = this,
      {
        cloneTarget,
        context,
        proxySelector,
        dragWithin,
        draggingCls
      } = me,
      {
        relatedElements,
        relatedElStartPos,
        grabbed
      } = context,
      element = context.dragProxy || context.element;
    context.valid = false;
    (_me$scrollManager = me.scrollManager) === null || _me$scrollManager === void 0 ? void 0 : _me$scrollManager.stopMonitoring();
    if (context.aborted) {
      console.warn('DragHelper: Aborting already aborted drag');
      return;
    }
    let {
      elementStartX,
      elementStartY
    } = context;
    const proxyMoved = elementStartX !== me.getX(element) || elementStartY !== me.getY(element);
    if (element && context.started) {
      // Put the dragged element back where it was
      if (!cloneTarget && dragWithin && dragWithin !== context.grabbedParent) {
        context.grabbedParent.insertBefore(element, context.grabbedNextSibling);
      }
      // Align the now visible grabbed element with the clone, so that it looks like it's
      // sliding back into place when the clone is removed
      if (cloneTarget) {
        if (proxySelector) {
          const animateTo = grabbed.querySelector(proxySelector) || grabbed,
            {
              x,
              y
            } = Rectangle.from(animateTo);
          elementStartX = x;
          elementStartY = y;
        }
      }
      // animated restore of position.
      element.classList.add('b-aborting');
      // Move the main element back to its original position.
      me.setXY(element, elementStartX, elementStartY);
      // Move any related elements back to their original positions.
      relatedElements === null || relatedElements === void 0 ? void 0 : relatedElements.forEach((element, i) => {
        element.classList.remove(draggingCls);
        element.classList.add('b-aborting');
        me.setXY(element, relatedElStartPos[i][0], relatedElStartPos[i][1]);
      });
      if (!silent) {
        me.trigger(invalid ? 'drop' : 'abort', {
          context,
          event
        });
      }
      // Element may have been scrolled out of view, or otherwise removed while dragging
      if (element.isConnected && !me.isDestroying && proxyMoved) {
        await EventHelper.waitForTransitionEnd({
          element,
          property: DomHelper.getPropertyTransitionDuration(element, 'transform') ? 'transform' : 'all',
          thisObj: me,
          once: true,
          runOnDestroy: true
        });
      }
      if (!me.isDestroyed) {
        // Trigger event after transition has completed for UIs to redraw with stable DOM
        me.trigger('abortFinalized', {
          context,
          event
        });
      }
    }
    if ((_me$context = me.context) !== null && _me$context !== void 0 && _me$context.started) {
      me.reset();
    }
  }
  // Restore state of all mutated elements
  cleanUp() {
    const me = this,
      {
        context,
        cloneTarget,
        draggingCls,
        dragProxyCls
      } = me,
      element = context.dragProxy || context.element,
      {
        relatedElements,
        originalRelatedElements,
        grabbed
      } = context,
      removeClonedProxies = cloneTarget && (me.removeProxyAfterDrop || !context.valid),
      cssClassesToRemove = [draggingCls, 'b-aborting', dragProxyCls, 'b-drag-main', 'b-drag-unified-proxy'];
    element.classList.remove(...cssClassesToRemove);
    if (removeClonedProxies) {
      element.remove();
    }
    relatedElements === null || relatedElements === void 0 ? void 0 : relatedElements.forEach(element => {
      if (removeClonedProxies) {
        element.remove();
      } else {
        element.classList.remove(...cssClassesToRemove);
      }
    });
    // Restore originallly grabbed elements
    grabbed.classList.remove('b-drag-original', 'b-hidden');
    originalRelatedElements === null || originalRelatedElements === void 0 ? void 0 : originalRelatedElements.forEach(element => element.classList.remove('b-hidden', 'b-drag-original'));
  }
  //endregion
});

/**
 * @module Core/helper/DragHelper
 */
const rootElementListeners = {
  move: 'onMouseMove',
  up: 'onMouseUp',
  docclick: 'onDocumentClick',
  touchstart: 'onTouchStart',
  touchmove: 'onTouchMove',
  touchend: 'onTouchEnd',
  keydown: 'onKeyDown'
};
/**
 * ## Intro
 * A drag drop helper class which lets you move elements in page. It supports:
 *
 *  * Dragging the actual element
 *  * Dragging a cloned version of the element
 *  * Dragging extra `relatedElements` along with the main element
 *  * Firing useful events {@link #event-beforeDragStart}, {@link #event-dragStart}, {@link #event-drag}, {@link #event-drop}, {@link #event-abort}
 *  * Validation by setting a `valid` Boolean on the drag context object provided to event listeners
 *  * Aborting drag with ESCAPE key
 *  * Constraining drag to be only horizontal or vertical using {@link #config-lockX} and {@link #config-lockY}
 *  * Defining X / Y boundaries using {@link #config-minX}, {@link #config-maxX} and {@link #config-minY}, {@link #config-maxY}
 *  * Async finalization (e.g. to show confirmation prompts)
 *  * Animated final transition after mouse up of a valid drop (see {@link #function-animateProxyTo})
 *  * Animated abort transition after an invalid or aborted drop
 *
 * {@inlineexample Core/helper/DragHelper.js}
 *
 * ## Two modes
 *
 * DragHelper supports two {@link #config-mode modes}:
 *
 * * `container` - moving / rearranging elements within and between specified containers
 * * `translateXY` - freely repositioning an element, either using the element or a cloned version of it - a "drag proxy" (default mode)
 *
 * ## Container drag mode
 *
 * Container drag should be used when moving or rearranging child elements within and between specified containers
 *
 * Example:
 * ```javascript
 * // dragging element between containers
 * let dragHelper = new DragHelper({
 *   mode       : 'container',
 *   containers : [ container1, container2 ]
 * });
 *```
 *
 * ## Translate drag mode
 *
 * Use translate drag to reposition an element within its container using transform CSS.
 *
 * Example:
 * ```javascript
 * // dragging element within container
 * let dragHelper = new DragHelper({
 *   mode           : 'translateXY',
 *   targetSelector : 'div.movable'
 * });
 * ```
 *
 * ## Observable events
 * In the various events fired by the DragHelper, you will have access to the raw DOM event and some useful `context` about the drag operation:
 *
 * ```javascript
 *  myDrag.on({
 *      drag : ({event , context}) {
 *            // The element which we're moving, could be a cloned version of grabbed, or the grabbed element itself
 *           const element = context.element;
 *
 *           // The original mousedown element upon which triggered the drag operation
 *           const grabbed = context.grabbed;
 *
 *           // The target under the current mouse / pointer / touch position
 *           const target = context.target;
 *       }
 *  });
 * ```
 *
 * ## Simple drag helper subclass with a drop target specified:
 * ```javascript
 * export default class MyDrag extends DragHelper {
 *      static get defaultConfig() {
 *          return {
 *              // Don't drag the actual cell element, clone it
 *              cloneTarget        : true,
 *              mode               : 'translateXY',
 *              // Only allow drops on DOM elements with 'yourDropTarget' CSS class specified
 *              dropTargetSelector : '.yourDropTarget',
 *
 *              // Only allow dragging elements with the 'draggable' CSS class
 *              targetSelector : '.draggable'
 *          };
 *      }
 *
 *      construct(config) {
 *          const me = this;
 *
 *          super.construct(config);
 *
 *          me.on({
 *              dragstart : me.onDragStart
 *          });
 *      }
 *
 *      onDragStart({ event, context }) {
 *          const target = context.target;
 *
 *          // Here you identify what you are dragging (an image of a user, grid row in an order table etc) and map it to something in your
 *          // data model. You can store your data on the context object which is available to you in all drag-related events
 *          context.userId = target.dataset.userId;
 *      }
 *
 *      onEquipmentDrop({ context, event }) {
 *          const me = this;
 *
 *          if (context.valid) {
 *              const userId   = context.userId,
 *                    droppedOnTarget = context.target;
 *
 *              console.log(`You dropped user ${userStore.getById(userId).name} on ${droppedOnTarget}`, droppedOnTarget);
 *
 *              // Dropped on a scheduled event, display toast
 *              Toast.show(`You dropped user ${userStore.getById(userId).name} on ${droppedOnTarget}`);
 *          }
 *      }
 *  };
 * ```
 *
 * ## Dragging multiple elements
 *
 * You can tell the DragHelper to also move additional `relatedElements` when a drag operation is starting. Simply
 * provide an array of elements on the context object:
 *
 * ```javascript
 * new DragHelper ({
 *     callOnFunctions : true,
 *
 *     onDragStart({ context }) {
 *          // Let drag helper know about extra elements to drag
 *          context.relatedElements = Array.from(element.querySelectorAll('.b-resource-avatar'));
 *     }
 * });
 * ```
 *
 * ## Creating a custom drag proxy
 *
 * Using the {@link #function-createProxy} you can create any markup structure to use when dragging cloned targets.
 *
 * ```javascript
 * new DragHelper ({
 *    callOnFunctions      : true,
 *    // Don't drag the actual cell element, clone it
 *    cloneTarget          : true,
 *    // We size the cloned element using CSS
 *    autoSizeClonedTarget : false,
 *
 *    mode               : 'translateXY',
 *    // Only allow drops on certain DOM nodes
 *    dropTargetSelector : '.myDropTarget',
 *    // Only allow dragging cell elements in a Bryntum Grid
 *    targetSelector     : '.b-grid-row:not(.b-group-row) .b-grid-cell'
 *
 *    // Here we receive the element where the drag originated and we can choose to return just a child element of it
 *    // to use for the drag proxy (such as an icon)
 *    createProxy(element) {
 *        return element.querySelector('i').cloneNode();
 *    }
 * });
 * ```
 *
 * ## Animating a cloned drag proxy to a point before finalizing
 *
 * To provide users with the optimal user experience, you can set a `transitionTo` object (with `target` element and
 * `align` spec) on the DragHelper´s `context` object inside a {@link #event-drop} listener (only applies to translate
 * {@link #config-mode mode} operations). This will trigger a final animation of the drag proxy which should represent
 * the change of data state that will be triggered by the drop.
 *
 * You can see this in action in Gantt´s `drag-resource-from-grid` demo.
 *
 * ```javascript
 * new DragHelper ({
 *    callOnFunctions      : true,
 *    // Don't drag the actual cell element, clone it
 *    cloneTarget          : true,
 *    // We size the cloned element using CSS
 *    autoSizeClonedTarget : false,
 *
 *    mode               : 'translateXY',
 *    // Only allow drops on certain DOM nodes
 *    dropTargetSelector : '.myDropTarget',
 *    // Only allow dragging cell elements in a Bryntum Grid
 *    targetSelector     : '.b-grid-row:not(.b-group-row) .b-grid-cell'
 *
 *    // Here we receive the element where the drag originated and we can choose to return just a child element of it
 *    // to use for the drag proxy (such as an icon)
 *    createProxy(element) {
 *        return element.querySelector('i').cloneNode();
 *    },
 *
 *    async onDrop({ context, event }) {
 *       // If it's a valid drop, provide a point to animate the proxy to before finishing the operation
 *      if (context.valid) {
 *          await this.animateProxyTo(someElement, {
 *               // align left side of drag proxy to right side of the someElement
 *               align  : 'l0-r0'
 *          });
 *      }
 *      else {
 *          Toast.show(`You cannot drop here`);
 *      }
 *   }
 * });
 * ```
 *
 * @mixes Core/mixin/Events
 * @extends Core/Base
 */
class DragHelper extends Base.mixin(Events, DragHelperContainer, DragHelperTranslate) {
  //region Config
  static get defaultConfig() {
    return {
      /**
       * Drag proxy CSS class
       * @config {String}
       * @default
       * @private
       */
      dragProxyCls: 'b-drag-proxy',
      /**
       * CSS class added when drag is invalid
       * @config {String}
       * @default
       */
      invalidCls: 'b-drag-invalid',
      /**
       * CSS class added to the source element in Container drag
       * @config {String}
       * @default
       * @private
       */
      draggingCls: 'b-dragging',
      /**
       * CSS class added to the source element in Container drag
       * @config {String}
       * @default
       * @private
       */
      dropPlaceholderCls: 'b-drop-placeholder',
      /**
       * The amount of pixels to move mouse before it counts as a drag operation
       * @config {Number}
       * @default
       */
      dragThreshold: 5,
      /**
       * The outer element where the drag helper will operate (attach events to it and use as outer limit when looking for ancestors)
       * @config {HTMLElement}
       * @default
       */
      outerElement: document.body,
      /**
       * Outer element that limits where element can be dragged
       * @config {HTMLElement}
       */
      dragWithin: null,
      /**
       * Set to true to stack any related dragged elements below the main drag proxy element. Only applicable when
       * using translate {@link #config-mode} with {@link #config-cloneTarget}
       * @config {Boolean}
       */
      unifiedProxy: null,
      monitoringConfig: null,
      /**
       * Constrain translate drag to dragWithin elements bounds (set to false to allow it to "overlap" edges)
       * @config {Boolean}
       * @default
       */
      constrain: true,
      /**
       * Smallest allowed x when dragging horizontally.
       * @config {Number}
       */
      minX: null,
      /**
       * Largest allowed x when dragging horizontally.
       * @config {Number}
       */
      maxX: null,
      /**
       * Smallest allowed y when dragging horizontally.
       * @config {Number}
       */
      minY: null,
      /**
       * Largest allowed y when dragging horizontally.
       * @config {Number}
       */
      maxY: null,
      /**
       * Enabled dragging, specify mode:
       * <table>
       * <tr><td>container<td>Allows reordering elements within one and/or between multiple containers
       * <tr><td>translateXY<td>Allows dragging within a parent container
       * </table>
       * @config {'container'|'translateXY'}
       * @default
       */
      mode: 'translateXY',
      /**
       * A function that determines if dragging an element is allowed. Gets called with the element as argument,
       * return true to allow dragging or false to prevent.
       * @config {Function}
       */
      isElementDraggable: null,
      /**
       * A CSS selector used to determine if dragging an element is allowed.
       * @config {String}
       */
      targetSelector: null,
      /**
       * A CSS selector used to determine if a drop is allowed at the current position.
       * @config {String}
       */
      dropTargetSelector: null,
      /**
       * A CSS selector added to each drop target element while dragging.
       * @config {String}
       */
      dropTargetCls: null,
      /**
       * A CSS selector used to target a child element of the mouse down element, to use as the drag proxy element.
       * Applies to translate {@link #config-mode mode} when using {@link #config-cloneTarget}.
       * @config {String}
       */
      proxySelector: null,
      /**
       * Set to `true` to clone the dragged target, and not move the actual target DOM node.
       * @config {Boolean}
       * @default
       */
      cloneTarget: false,
      /**
       * Set to `false` to not apply width/height of cloned drag proxy elements.
       * @config {Boolean}
       * @default
       */
      autoSizeClonedTarget: true,
      /**
       * Set to true to hide the original element while dragging (applicable when `cloneTarget` is true).
       * @config {Boolean}
       * @default
       */
      hideOriginalElement: false,
      /**
       * Containers whose elements can be rearranged (and moved between the containers). Used when
       * mode is set to "container".
       * @config {HTMLElement[]}
       */
      containers: null,
      /**
       * A CSS selector used to exclude elements when using container mode
       * @config {String}
       */
      ignoreSelector: null,
      startEvent: null,
      /**
       * Configure as `true` to disallow dragging in the `X` axis. The dragged element will only move vertically.
       * @config {Boolean}
       * @default
       */
      lockX: false,
      /**
       * Configure as `true` to disallow dragging in the `Y` axis. The dragged element will only move horizontally.
       * @config {Boolean}
       * @default
       */
      lockY: false,
      /**
       * The amount of milliseconds to wait after a touchstart, before a drag gesture will be allowed to start.
       * @config {Number}
       * @default
       */
      touchStartDelay: 300,
      /**
       * Scroll manager of the target. If specified, scrolling while dragging is supported.
       * @config {Core.util.ScrollManager}
       */
      scrollManager: null,
      /**
       * A method provided to snap coordinates to fixed points as you drag
       * @config {Function}
       * @internal
       */
      snapCoordinates: null,
      /**
       * When using {@link #config-unifiedProxy}, use this amount of pixels to offset each extra element when dragging multiple items
       * @config {Number}
       * @default
       */
      unifiedOffset: 5,
      /**
       * Configure as `false` to take ownership of the proxy element after a valid drop (advanced usage).
       * @config {Boolean}
       * @default
       */
      removeProxyAfterDrop: true,
      clickSwallowDuration: 50,
      ignoreSamePositionDrop: true,
      // true to allow drops outside the dragWithin element
      allowDropOutside: null,
      // for container mode
      floatRootOwner: null,
      mouseMoveListenerElement: document,
      testConfig: {
        transitionDuration: 10,
        clickSwallowDuration: 50,
        touchStartDelay: 100
      },
      rtlSource: null,
      /**
       * Creates the proxy element to be dragged, when using {@link #config-cloneTarget}. Clones the original element by default.
       * Provide your custom {@link #function-createProxy} function to be used for creating drag proxy.
       * @param {HTMLElement} element The element from which the drag operation originated
       * @config {Function}
       * @typings {Function(HTMLElement): HTMLElement}
       */
      createProxy: null
    };
  }
  //endregion
  //region Events
  /**
   * Fired before dragging starts, return `false` to prevent the drag operation.
   * @preventable
   * @event beforeDragStart
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The original element upon which the mousedown event triggered a drag operation
   * @param {MouseEvent|TouchEvent} event
   */
  /**
   * Fired when dragging starts. The event includes a `context` object. If you want to drag additional elements you can
   * provide these as an array of elements assigned to the `relatedElements` property of the context object.
   * @event dragStart
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The element which we're moving, could be a cloned version of grabbed, or the grabbed element itself
   * @param {HTMLElement} context.grabbed The original element upon which the mousedown event triggered a drag operation
   * @param {HTMLElement[]} context.relatedElements Array of extra elements to include in the drag.
   * @param {MouseEvent|TouchEvent} event
   */
  /**
   * Fired while dragging, you can signal that the drop is valid or invalid by setting `context.valid = false;`
   * @event drag
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The element which we are moving, could be a cloned version of grabbed, or the grabbed element itself
   * @param {HTMLElement} context.target The target element below the cursor
   * @param {HTMLElement} context.grabbed The original element upon which the mousedown event triggered a drag operation
   * @param {HTMLElement[]} context.relatedElements An array of extra elements dragged with the main dragged element
   * @param {Boolean} context.valid Set this to true or false to indicate whether the drop position is valid.
   * @param {MouseEvent} event
   */
  /**
   * Fired after a drop at an invalid position
   * @event abort
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The element which we are moving, could be a cloned version of grabbed, or the grabbed element itself
   * @param {HTMLElement} context.target The target element below the cursor
   * @param {HTMLElement} context.grabbed The original element upon which the mousedown event triggered a drag operation
   * @param {HTMLElement[]} context.relatedElements An array of extra elements dragged with the main dragged element
   * @param {MouseEvent} event
   */
  /**
   * Fires after {@link #event-abort} and after drag proxy has animated back to its original position
   * @private
   * @event abortFinalized
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The element which we are moving, could be a cloned version of grabbed, or the grabbed element itself
   * @param {HTMLElement} context.target The target element below the cursor
   * @param {HTMLElement} context.grabbed The original element upon which the mousedown event triggered a drag operation
   * @param {MouseEvent} event
   */
  //endregion
  //region Init
  /**
   * Initializes a new DragHelper.
   * @param {DragHelperConfig} config Configuration object, accepts options specified under Configs above
   * @example
   * new DragHelper({
   *   containers: [div1, div2],
   *   isElementDraggable: element => element.className.contains('handle'),
   *   outerElement: topParent,
   *   listeners: {
   *     drop: onDrop,
   *     thisObj: this
   *   }
   * });
   * @function constructor
   */
  construct(config) {
    const me = this;
    super.construct(config);
    me.initListeners();
    if (me.isContainerDrag) {
      me.initContainerDrag();
    } else {
      me.initTranslateDrag();
    }
    me.onScrollManagerScrollCallback = me.onScrollManagerScrollCallback.bind(me);
  }
  doDestroy() {
    this.reset(true);
    super.doDestroy();
  }
  /**
   * Initialize listener
   * @private
   */
  initListeners() {
    const me = this,
      {
        outerElement
      } = me,
      dragStartListeners = {
        element: outerElement,
        pointerdown: 'onPointerDown',
        thisObj: me
      };
    me.mouseMoveListenerElement = me.getMouseMoveListenerTarget(outerElement);
    // These will be autoDetached upon destroy
    EventHelper.on(dragStartListeners);
  }
  // Salesforce hook: we override this method to move listener from the body (which is default root node) to element
  // inside of LWC
  getMouseMoveListenerTarget(element) {
    const root = element.getRootNode();
    let result = this.mouseMoveListenerElement;
    // If we are inside a closed shadow root and we are a child of a Widget, listen to mouse moves only inside outermost el
    if (root.nodeType === Node.DOCUMENT_FRAGMENT_NODE && root.mode === 'closed') {
      result = element.closest('.b-outer') || result;
    }
    return result;
  }
  get isRTL() {
    var _this$rtlSource;
    return Boolean((_this$rtlSource = this.rtlSource) === null || _this$rtlSource === void 0 ? void 0 : _this$rtlSource.rtl);
  }
  //endregion
  //region Events
  /**
   * Fires after drop. For valid drops, it exposes `context.async` which you can set to true to signal that additional
   * processing is needed before finalizing the drop (such as showing some dialog). When that operation is done, call
   * `context.finalize(true/false)` with a boolean that determines the outcome of the drop.
   *
   * You can signal that the drop is valid or invalid by setting `context.valid = false;`
   *
   * For translate type drags with {@link #config-cloneTarget}, you can also set `transitionTo` if you want to animate
   * the dragged proxy to a position before finalizing the operation. See class intro text for example usage.
   *
   * @event drop
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The element which we are moving, could be a cloned version of grabbed, or the grabbed element itself
   * @param {HTMLElement} context.target The target element below the cursor
   * @param {HTMLElement} context.grabbed The original element upon which the mousedown event triggered a drag operation
   * @param {HTMLElement[]} context.relatedElements An array of extra elements dragged with the main dragged element
   * @param {Boolean} context.valid true if the drop position is valid
   */
  /**
   * Fires after {@link #event-drop} and after drag proxy has animated to its final position (if setting `transitionTo`
   * on the drag context object).
   * @private
   * @event dropFinalized
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {HTMLElement} context.element The element which we are moving, could be a cloned version of grabbed, or the grabbed element itself
   * @param {HTMLElement} context.target The target element below the cursor
   * @param {HTMLElement} context.grabbed The original element upon which the mousedown event triggered a drag operation
   */
  onPointerDown(event) {
    const me = this;
    if (
    // Left button or touch allowed
    event.button !== 0 ||
    // Visibility check is needed because mousedown on a just-created event element
    // will cause the EventEdit to cancel the add, remove the record and hide the element
    // *but* the mousedown event still bubbles to here and that will throw an error.
    !DomHelper.isVisible(event.target) ||
    // If a drag is ongoing already, finalize it and don't proceed with new drag (happens if pointerup happened
    // when current window wasn't focused - tab switch or window switch). Also handles the edge case of trying to
    // start a new drag while previous is awaiting finalization, in which case it just bails out.
    me.context) {
      return;
    }
    // Check that element is draggable
    if (me.isElementDraggable && !me.isElementDraggable(event.target, event)) {
      return;
    }
    me.startEvent = event;
    const handled = me.isContainerDrag ? me.grabContainerDrag(event) : me.grabTranslateDrag(event);
    if (handled) {
      me.blurDetacher = EventHelper.on({
        element: globalThis,
        blur: me.onWindowBlur,
        thisObj: me
      });
      const dragListeners = {
        element: me.mouseMoveListenerElement,
        thisObj: me,
        capture: true,
        keydown: rootElementListeners.keydown
      };
      if (event.pointerType === 'touch') {
        me.touchStartTimer = me.setTimeout(() => me.touchStartTimer = null, me.touchStartDelay, 'touchStartDelay');
        dragListeners.touchmove = {
          handler: rootElementListeners.touchmove,
          passive: false // We need to be able to preventDefault on the touchmove
        };
        // Touch desktops don't fire touchend event when touch has ended, instead pointerup is fired
        // iOS do fire touchend
        dragListeners.touchend = dragListeners.pointerup = rootElementListeners.touchend;
      } else {
        dragListeners.pointermove = rootElementListeners.move;
        dragListeners.pointerup = rootElementListeners.up;
      }
      // A listener detacher is returned;
      me.dragListenersDetacher = EventHelper.on(dragListeners);
      if (me.dragWithin && me.dragWithin !== me.outerElement && me.outerElement.contains(me.dragWithin)) {
        const box = Rectangle.from(me.dragWithin, me.outerElement);
        me.minY = box.top;
        me.maxY = box.bottom;
        me.minX = box.left;
        me.maxX = box.right;
      }
    }
  }
  internalMove(event) {
    var _event$target;
    // Ignore events used to mimic pointer movement on scroll, those should not affect dragging
    if (event.scrollInitiated) {
      return;
    }
    const me = this,
      {
        context
      } = me,
      distance = EventHelper.getDistanceBetween(me.startEvent, event),
      abortTouchDrag = me.touchStartTimer && distance > me.dragThreshold;
    if (abortTouchDrag) {
      me.abort(true);
      return;
    }
    if (!me.touchStartTimer && context !== null && context !== void 0 && context.element && (context.started || distance >= me.dragThreshold) &&
    // Ignore text nodes
    ((_event$target = event.target) === null || _event$target === void 0 ? void 0 : _event$target.nodeType) === Node.ELEMENT_NODE) {
      if (!context.started) {
        var _me$scrollManager, _me$onDragStarted;
        if (me.trigger('beforeDragStart', {
          context,
          event
        }) === false) {
          return me.abort();
        }
        if (me.isContainerDrag) {
          me.startContainerDrag(event);
        } else {
          me.startTranslateDrag(event);
        }
        context.started = true;
        // Now that the drag drop is confirmed to be starting, activate the configured scrollManager if present
        (_me$scrollManager = me.scrollManager) === null || _me$scrollManager === void 0 ? void 0 : _me$scrollManager.startMonitoring(ObjectHelper.merge({
          scrollables: [{
            element: me.dragWithin || me.outerElement
          }],
          callback: me.onScrollManagerScrollCallback
        }, me.monitoringConfig));
        // Global informational class for when DragHelper is dragging
        context.outermostEl = DomHelper.getOutermostElement(event.target);
        context.outermostEl.classList.add('b-draghelper-active');
        if (me.dropTargetSelector && me.dropTargetCls) {
          DomHelper.getRootElement(me.outerElement).querySelectorAll(me.dropTargetSelector).forEach(el => el.classList.add(me.dropTargetCls));
        }
        // This event signals that the drag is started, observers could then provide relatedElements that should
        // be dragged along with the mousedowned element
        me.trigger('dragStart', {
          context,
          event
        });
        (_me$onDragStarted = me.onDragStarted) === null || _me$onDragStarted === void 0 ? void 0 : _me$onDragStarted.call(me);
        // This event is used to set visibility of the original events in case drag is started
        // in copy mode
        me.trigger('afterDragStart', {
          context,
          event
        });
      }
      me.update(event);
      // to prevent view drag (scroll) on ipad
      if (event.type === 'touchmove') {
        event.preventDefault();
        event.stopImmediatePropagation();
      }
    }
  }
  onScrollManagerScrollCallback(config) {
    var _this$context;
    const {
      lastMouseMoveEvent
    } = this;
    if ((_this$context = this.context) !== null && _this$context !== void 0 && _this$context.element && lastMouseMoveEvent) {
      // Indicate that this is a 'fake' mousemove event as a result of the scrolling
      lastMouseMoveEvent.isScroll = true;
      this.update(lastMouseMoveEvent, config);
    }
  }
  onTouchMove(event) {
    this.internalMove(event);
  }
  /**
   * Move drag element with mouse.
   * @param event
   * @fires beforeDragStart
   * @fires dragStart
   * @private
   */
  onMouseMove(event) {
    this.internalMove(event);
  }
  /**
   * Updates drag, called when an element is grabbed and mouse moves
   * @private
   * @fires drag
   */
  update(event, scrollManagerConfig) {
    const me = this,
      {
        context
      } = me,
      scrollingPageElement = document.scrollingElement || document.body; // two different modes used
    // In case of scrolling need to update target element based on [X, Y] of the mouse event
    // Salesforce workaround: we're listening on the document body and salesforce won't report correct target, so
    // we try to get it from composed path
    let target = me.getMouseMoveEventTarget(event);
    // "pointer-events:none" touchmove has no effect for the touchmove event target, meaning we cannot know
    // what's under the cursor as easily in touch devices
    if (event.type === 'touchmove') {
      const touch = event.changedTouches[0];
      target = DomHelper.elementFromPoint(touch.clientX + scrollingPageElement.scrollLeft, touch.clientY + scrollingPageElement.scrollTop);
    }
    context.target = target;
    let internallyValid = me.allowDropOutside || !me.dragWithin || me.dragWithin.contains(event.target);
    if (internallyValid && me.dropTargetSelector) {
      var _target;
      internallyValid = internallyValid && Boolean((_target = target) === null || _target === void 0 ? void 0 : _target.closest(me.dropTargetSelector));
    }
    // Move the drag proxy or dragged element before triggering the drag event
    if (me.isContainerDrag) {
      me.updateContainerProxy(event, scrollManagerConfig);
    } else {
      // Note, if you drag an element from Container A to Container B which is scrollable (handled by ScrollManager),
      // and you notice that the proxy element follows the scroll and goes away from the cursor,
      // make sure you set `outerElement` to the container of the source element (Container A)
      // and set `constrain` to `false`.
      me.updateTranslateProxy(event, scrollManagerConfig);
    }
    context.valid = internallyValid;
    // Allow external code to validate the context before updating a container drag
    me.trigger('drag', {
      context,
      event
    });
    // Move the placeholder element into its new place.
    // This will see the new state of context if mutated by a drag listener.
    if (me.isContainerDrag) {
      me.updateContainerDrag(event, scrollManagerConfig);
    }
    context.valid = context.valid && internallyValid;
    for (const element of me.draggedElements) {
      element.classList.toggle(me.invalidCls, !context.valid);
    }
    if (event) {
      me.lastMouseMoveEvent = event;
    }
  }
  get draggedElements() {
    const {
      context
    } = this;
    return [context.dragProxy || context.element, ...(context.relatedElements ?? [])];
  }
  /**
   * Abort dragging
   * @fires abort
   */
  async abort(silent = false) {
    var _me$scrollManager2, _me$scrollManager2$st;
    const me = this,
      {
        context
      } = me;
    (_me$scrollManager2 = me.scrollManager) === null || _me$scrollManager2 === void 0 ? void 0 : (_me$scrollManager2$st = _me$scrollManager2.stopMonitoring) === null || _me$scrollManager2$st === void 0 ? void 0 : _me$scrollManager2$st.call(_me$scrollManager2);
    me.removeListeners();
    if (context !== null && context !== void 0 && context.started && !context.aborted) {
      // Force a synchronous layout so that transitions from this point will work.
      context.element.getBoundingClientRect();
      // Aborted drag not considered valid
      context.valid = false;
      if (me.isContainerDrag) {
        me.abortContainerDrag(undefined, undefined, silent);
      } else {
        me.abortTranslateDrag(undefined, undefined, silent);
      }
      context.aborted = true;
    } else {
      me.reset(true);
    }
  }
  // Empty class implementation. If listeners *are* added, the detacher is added
  // as an instance property. So this is always callable.
  removeListeners() {
    var _this$dragListenersDe, _this$blurDetacher;
    (_this$dragListenersDe = this.dragListenersDetacher) === null || _this$dragListenersDe === void 0 ? void 0 : _this$dragListenersDe.call(this);
    (_this$blurDetacher = this.blurDetacher) === null || _this$blurDetacher === void 0 ? void 0 : _this$blurDetacher.call(this);
  }
  // Called when a drag operation is completed, or aborted
  // Removes DOM listeners and resets context
  reset(silent) {
    const me = this,
      {
        context
      } = me;
    if (context !== null && context !== void 0 && context.started) {
      for (const element of me.draggedElements) {
        element.classList.remove(me.invalidCls);
      }
      context.outermostEl.classList.remove('b-draghelper-active');
      if (me.isContainerDrag) {
        context.dragProxy.remove();
      } else {
        me.cleanUp();
      }
      if (me.dropTargetSelector && me.dropTargetCls) {
        DomHelper.getRootElement(me.outerElement).querySelectorAll(me.dropTargetSelector).forEach(el => el.classList.remove(me.dropTargetCls));
      }
    }
    me.removeListeners();
    /**
     * Fired after a drag operation is completed or aborted
     * @event reset
     * @private
     * @param {Core.helper.DragHelper} dragHelper
     */
    if (!silent) {
      me.trigger('reset');
    }
    me.context = me.lastMouseMoveEvent = null;
  }
  onTouchEnd(event) {
    this.onMouseUp(event);
  }
  /**
   * This is a capture listener, only added during drag, which prevents a click gesture
   * propagating from the terminating mouseup gesture
   * @param {MouseEvent} event
   * @private
   */
  onDocumentClick(event) {
    event.stopPropagation();
  }
  /**
   * Drop on mouse up (if dropped on valid target).
   * @param event
   * @private
   */
  onMouseUp(event) {
    const me = this,
      {
        context
      } = me;
    me.removeListeners();
    if (context) {
      var _me$scrollManager3;
      (_me$scrollManager3 = me.scrollManager) === null || _me$scrollManager3 === void 0 ? void 0 : _me$scrollManager3.stopMonitoring();
      if (context.started) {
        // Nobody else must get to process the pointerup event of a drag.
        // We are using capture : true, so we see it first
        event.stopPropagation();
        context.finalizing = true;
        if (me.isContainerDrag) {
          me.finishContainerDrag(event);
        } else {
          me.finishTranslateDrag(event);
        }
        // Prevent the impending document click from the mouseup event from propagating
        // into a click on our element.
        EventHelper.on({
          element: document,
          thisObj: me,
          click: rootElementListeners.docclick,
          capture: true,
          expires: me.clickSwallowDuration,
          // In case a click did not ensue, remove the listener
          once: true
        });
      } else {
        me.reset(true);
      }
    }
  }
  /**
   * Cancel on ESC key
   * @param event
   * @private
   */
  onKeyDown(event) {
    var _this$context2;
    if ((_this$context2 = this.context) !== null && _this$context2 !== void 0 && _this$context2.started && event.key === 'Escape') {
      // Nobody else must get to process the ESCAPE key event of a drag.
      // We are using capture : true, so we see it first
      event.stopImmediatePropagation();
      this.abort();
    }
  }
  onWindowBlur() {
    // If window blur occurs while we are dragging (tab is switched, another window steals focus from browser)
    // pointer might be released and current window will not know about that. Thus allowing to pointerdown again
    // when focus comes back. In this case we want to let drag helper know that next pointerdown should be ignored.
    if (this.context && !this.context.finalizing) {
      this.abort();
    }
  }
  /**
   * Creates the proxy element to be dragged, when using {@link #config-cloneTarget}. Clones the original element by default.
   * Override it to provide your own custom HTML element structure to be used as the drag proxy.
   * @param {HTMLElement} element The element from which the drag operation originated
   * @returns {HTMLElement}
   */
  createProxy(element) {
    if (this.proxySelector) {
      element = element.querySelector(this.proxySelector) || element;
    }
    const proxy = element.cloneNode(true);
    proxy.removeAttribute('id');
    return proxy;
  }
  //endregion
  get isContainerDrag() {
    return this.mode === 'container';
  }
  /**
   * Animated the proxy element to be aligned with the passed element. Returns a Promise which resolves after the
   * DOM transition completes. Only applies to 'translateXY' mode.
   * @param {HTMLElement|Core.helper.util.Rectangle} element The target element or a Rectangle
   * @param {Object} [alignSpec] An object describing how to the align drag proxy to the target element
   * @param {String} [alignSpec.align] The alignment specification string, `[trbl]n-[trbl]n`.
   * @param {Number|Number[]} [alignSpec.offset] The 'x' and 'y' offset values to create an extra margin round the target
   * to offset the aligned widget further from the target. May be configured as -ve to move the aligned widget
   * towards the target - for example producing the effect of the anchor pointer piercing the target.
   */
  async animateProxyTo(targetElement, alignSpec = {
    align: 'c-c'
  }) {
    const {
        context,
        draggedElements
      } = this,
      {
        element
      } = context,
      targetRect = targetElement.isRectangle ? targetElement : Rectangle.from(targetElement);
    draggedElements.forEach(el => {
      el.classList.add('b-drag-final-transition');
      DomHelper.alignTo(el, targetRect, alignSpec);
    });
    await EventHelper.waitForTransitionEnd({
      element,
      property: 'all',
      thisObj: this,
      once: true
    });
    draggedElements.forEach(el => el.classList.remove('b-drag-final-transition'));
  }
  /**
   * Returns true if a drag operation is active
   * @property {Boolean}
   * @readonly
   */
  get isDragging() {
    var _this$context3;
    return Boolean((_this$context3 = this.context) === null || _this$context3 === void 0 ? void 0 : _this$context3.started);
  }
  //#region Salesforce hooks
  getMouseMoveEventTarget(event) {
    return !event.isScroll ? event.target : DomHelper.elementFromPoint(event.clientX, event.clientY);
  }
  //#endregion
}

DragHelper._$name = 'DragHelper';

const documentListeners = {
  down: 'onMouseDown',
  move: 'onMouseMove',
  up: 'onMouseUp',
  docclick: 'onDocumentClick',
  touchstart: {
    handler: 'onTouchStart',
    // We preventDefault touchstart so as not to scroll. Must not be passive.
    // https://developers.google.com/web/updates/2017/01/scrolling-intervention
    passive: false
  },
  touchmove: 'onTouchMove',
  touchend: 'onTouchEnd',
  keydown: 'onKeyDown'
};
/**
 * @module Core/helper/ResizeHelper
 */
/**
 * Contextual information available during a resize.
 *
 * @typedef {Object} ResizeContext
 * @property {HTMLElement} element Element being resized
 * @property {'top'|'right'|'bottom'|'left'} edge Edge being dragged
 * @property {Boolean} valid `true` if the resize is valid, `false` if it is not
 * @property {Boolean} async Set to `true` in a `resize` listener to allow asynchronous finalization of the resize
 * @property {Function} finalize Call this function to finalize the resize, required if it was flagged as `async` in a
 * `resize` listener
 * @property {Number} newWidth New width of the element
 * @property {Number} newHeight New height of the element
 * @property {Number} newX New x coordinate of the element, when dragging left edge
 * @property {Number} newY New y coordinate of the element, when dragging top edge
 */
/**
 * Handles resizing of elements using handles. Handles can be actual elements or virtual handles specified as a border
 * area on elements left and right edges.
 *
 * ```javascript
 * // enable resizing all elements with class 'resizable'
 * let resizer = new ResizeHelper({
 *   targetSelector: '.resizable'
 * });
 * ```
 *
 * @mixes Core/mixin/Events
 * @internal
 */
class ResizeHelper extends Events(Base) {
  //region Config
  static get defaultConfig() {
    return {
      /**
       * CSS class added when resizing
       * @config {String}
       * @default
       */
      resizingCls: 'b-resizing',
      /**
       * The amount of pixels to move mouse before it counts as a drag operation
       * @config {Number}
       * @default
       */
      dragThreshold: 5,
      /**
       * Resizing handle size
       * @config {Number}
       * @default
       */
      handleSize: 10,
      /**
       * Automatically shrink virtual handles when available space < handleSize. The virtual handles will
       * decrease towards width/height 1, reserving space between opposite handles to for example leave room for
       * dragging. To configure reserved space, see {@link #config-reservedSpace}.
       * @config {Boolean}
       * @default false
       */
      dynamicHandleSize: null,
      //
      /**
       * Room in px to leave unoccupied by handles when shrinking them dynamically (see
       * {@link #config-dynamicHandleSize}).
       * @config {Number}
       * @default
       */
      reservedSpace: 10,
      /**
       * Resizing handle size on touch devices
       * @config {Number}
       * @default
       */
      touchHandleSize: 30,
      /**
       * Minimum width when resizing
       * @config {Number}
       * @default
       */
      minWidth: 1,
      /**
       * Max width when resizing.
       * @config {Number}
       * @default
       */
      maxWidth: 0,
      /**
       * Minimum height when resizing
       * @config {Number}
       * @default
       */
      minHeight: 1,
      /**
       * Max height when resizing
       * @config {Number}
       * @default
       */
      maxHeight: 0,
      // outerElement, attach events to it and use as outer limit when looking for ancestors
      outerElement: document.body,
      /**
       * Optional scroller used to read scroll position. If unspecified, the outer element will be used.
       * @config {Core.helper.util.Scroller}
       */
      scroller: null,
      /**
       * Assign a function to determine if a hovered element can be resized or not
       * @config {Function}
       * @default
       */
      allowResize: null,
      /**
       * Outer element that limits where element can be dragged
       * @config {HTMLElement}
       * @default
       */
      dragWithin: null,
      /**
       * A function that determines if dragging an element is allowed. Gets called with the element as argument,
       * return true to allow dragging or false to prevent.
       * @config {Function}
       * @default
       */
      isElementResizable: null,
      /**
       * A CSS selector used to determine if resizing an element is allowed.
       * @config {String}
       * @default
       */
      targetSelector: null,
      /**
       * Use left handle when resizing. Only applies when `direction` is 'horizontal'
       * @config {Boolean}
       * @default
       */
      leftHandle: true,
      /**
       * Use right handle when resizing. Only applies when `direction` is 'horizontal'
       * @config {Boolean}
       * @default
       */
      rightHandle: true,
      /**
       * Use top handle when resizing. Only applies when `direction` is 'vertical'
       * @config {Boolean}
       * @default
       */
      topHandle: true,
      /**
       * Use bottom handle when resizing. Only applies when `direction` is 'vertical'
       * @config {Boolean}
       * @default
       */
      bottomHandle: true,
      /**
       * A CSS selector used to determine where handles should be "displayed" when resizing. Defaults to
       * targetSelector if unspecified
       * @config {String}
       */
      handleSelector: null,
      /**
       * A CSS selector used to determine which inner element contains handles.
       * @config {String}
       */
      handleContainerSelector: null,
      startEvent: null,
      /*
       * Optional config object, used by EventResize feature: it appends proxy and has to start resizing immediately
       * @config {Object}
       * @private
       */
      grab: null,
      /**
       * CSS class added when the resize state is invalid
       * @config {String}
       * @default
       */
      invalidCls: 'b-resize-invalid',
      // A number that controls whether or not the element is wide enough for it to make sense to show resize handles
      // e.g. handle width is 10px, so doesn't make sense to show them unless handles on both sides fit
      handleVisibilityThreshold: null,
      // Private config that disables translation when resizing left edge. Useful for example in cases when element
      // being resized is part of a flex layout
      skipTranslate: false,
      /**
       * Direction to resize in, either 'horizontal' or 'vertical'
       * @config {'horizontal'|'vertical'}
       * @default
       */
      direction: 'horizontal',
      clickSwallowDuration: 50,
      rtlSource: null
    };
  }
  static configurable = {
    // Private config that disables updating elements width and position, for when data is live updated during
    // resize, leading to element being redrawn
    skipUpdatingElement: null
  };
  //endregion
  //region Events
  /**
   * Fired while dragging
   * @event resizing
   * @param {Core.helper.ResizeHelper} source
   * @param {ResizeContext} context Resize context
   * @param {MouseEvent} event Browser event
   */
  /**
   * Fired when dragging starts.
   * @event resizeStart
   * @param {Core.helper.ResizeHelper} source
   * @param {ResizeContext} context Resize context
   * @param {MouseEvent|TouchEvent} event Browser event
   */
  /**
   * Fires after resize, and allows for asynchronous finalization by setting 'async' to `true` on the context object.
   * @event resize
   * @param {Core.helper.ResizeHelper} source
   * @param {ResizeContext} context Context about the resize operation. Set 'async' to `true` to indicate asynchronous
   * validation of the resize flow (for showing a confirmation dialog etc)
   */
  /**
   * Fires when a resize is canceled (width is unchanged)
   * @event cancel
   * @param {Core.helper.ResizeHelper} source
   * @param {ResizeContext} context Resize context
   * @param {MouseEvent|TouchEvent} event Browser event
   */
  //endregion
  //region Init
  construct(config) {
    const me = this;
    super.construct(config);
    // Larger draggable zones on pure touch devices with no mouse
    if (!me.handleSelector && !BrowserHelper.isHoverableDevice) {
      me.handleSize = me.touchHandleSize;
    }
    me.handleVisibilityThreshold = me.handleVisibilityThreshold || 2 * me.handleSize;
    me.initListeners();
    me.initResize();
  }
  doDestroy() {
    this.abort(true);
    super.doDestroy();
  }
  updateSkipUpdatingElement(skip) {
    if (skip) {
      this.skipTranslate = true;
    }
  }
  /**
   * Initializes resizing
   * @private
   */
  initResize() {
    const me = this;
    if (!me.isElementResizable && me.targetSelector) {
      me.isElementResizable = element => element.closest(me.targetSelector);
    }
    if (me.grab) {
      const {
        edge,
        element,
        event
      } = me.grab;
      me.startEvent = event;
      const cursorOffset = me.getCursorOffsetToElementEdge(event, element, edge);
      // emulates mousedown & grabResize
      me.context = {
        element,
        edge,
        valid: true,
        async: false,
        elementStartX: DomHelper.getTranslateX(element) || element.offsetLeft,
        // extract x from translate
        elementStartY: DomHelper.getTranslateY(element) || element.offsetTop,
        // extract x from translate
        newX: DomHelper.getTranslateX(element) || element.offsetLeft,
        // No change yet on start, but info must be present
        newY: DomHelper.getTranslateY(element) || element.offsetTop,
        // No change yet on start, but info must be present
        elementWidth: element.offsetWidth,
        elementHeight: element.offsetHeight,
        cursorOffset,
        startX: event.clientX + cursorOffset.x + me.scrollLeft,
        startY: event.clientY + cursorOffset.y + me.scrollTop,
        finalize: () => {
          var _me$reset;
          return (_me$reset = me.reset) === null || _me$reset === void 0 ? void 0 : _me$reset.call(me);
        }
      };
      element.classList.add(me.resizingCls);
      me.internalStartResize(me.isTouch);
    }
  }
  /**
   * Initialize listeners
   * @private
   */
  initListeners() {
    const me = this,
      dragStartListeners = {
        element: me.outerElement,
        mousedown: documentListeners.down,
        touchstart: documentListeners.touchstart,
        thisObj: me
      };
    if (!me.handleSelector && BrowserHelper.isHoverableDevice) {
      dragStartListeners.mousemove = {
        handler: documentListeners.move,
        // Filter events for checkResizeHandles so we only get called if the mouse
        // is over one of our targets.
        delegate: me.targetSelector
      };
      // We need to clean up when we exit one of our targets
      dragStartListeners.mouseleave = {
        handler: 'onMouseLeaveTarget',
        delegate: me.targetSelector,
        capture: true
      };
    }
    // These will be autoDetached upon destroy
    EventHelper.on(dragStartListeners);
  }
  get isRTL() {
    var _this$rtlSource;
    return Boolean((_this$rtlSource = this.rtlSource) === null || _this$rtlSource === void 0 ? void 0 : _this$rtlSource.rtl);
  }
  //endregion
  //region Scroll helpers
  get scrollLeft() {
    if (this.scroller) {
      return this.scroller.x;
    }
    return this.outerElement.scrollLeft;
  }
  get scrollTop() {
    if (this.scroller) {
      return this.scroller.y;
    }
    return this.outerElement.scrollTop;
  }
  //endregion
  //region Events
  internalStartResize(isTouch) {
    const dragListeners = {
      element: document,
      keydown: documentListeners.keydown,
      thisObj: this
    };
    if (isTouch) {
      dragListeners.touchmove = documentListeners.touchmove;
      // Touch desktops don't fire touchend event when touch has ended, instead pointerup is fired
      // iOS do fire touchend
      dragListeners.touchend = dragListeners.pointerup = documentListeners.touchend;
    } else {
      dragListeners.mousemove = documentListeners.move;
      dragListeners.mouseup = documentListeners.up;
    }
    // A listener detacher is returned
    this.removeDragListeners = EventHelper.on(dragListeners);
  }
  // Empty class implementation. If listeners *are* added, the detacher is added
  // as an instance property. So this is always callable.
  removeDragListeners() {}
  reset() {
    var _this$removeDragListe;
    (_this$removeDragListe = this.removeDragListeners) === null || _this$removeDragListe === void 0 ? void 0 : _this$removeDragListe.call(this);
    this.context = null;
    this.trigger('reset');
  }
  canResize(element, event) {
    return !this.isElementResizable || this.isElementResizable(element, event);
  }
  onPointerDown(isTouch, event) {
    const me = this;
    me.startEvent = event;
    if (me.canResize(event.target, event) && me.grabResizeHandle(isTouch, event)) {
      // Stop event if resize handle was grabbed (resize started)
      event.stopImmediatePropagation();
      if (event.type === 'touchstart') {
        event.preventDefault();
      }
      me.internalStartResize(isTouch);
    }
  }
  onTouchStart(event) {
    // only allowing one finger for now...
    if (event.touches.length > 1) {
      return;
    }
    this.onPointerDown(true, event);
  }
  /**
   * Grab draggable element on mouse down.
   * @private
   * @param {MouseEvent|PointerEvent} event
   */
  onMouseDown(event) {
    // only dragging with left mouse button
    if (event.button !== 0) {
      return;
    }
    this.onPointerDown(false, event);
  }
  internalMove(isTouch, event) {
    const me = this,
      {
        context,
        direction
      } = me;
    if (context !== null && context !== void 0 && context.element && (context.started || EventHelper.getDistanceBetween(me.startEvent, event) >= me.dragThreshold)) {
      if (!context.started) {
        var _me$scrollManager;
        (_me$scrollManager = me.scrollManager) === null || _me$scrollManager === void 0 ? void 0 : _me$scrollManager.startMonitoring(ObjectHelper.merge({
          scrollables: [{
            element: me.dragWithin || me.outerElement,
            direction
          }],
          callback: config => {
            var _me$context;
            return ((_me$context = me.context) === null || _me$context === void 0 ? void 0 : _me$context.element) && me.lastMouseMoveEvent && me.update(me.lastMouseMoveEvent, config);
          }
        }, me.monitoringConfig));
        me.trigger('resizeStart', {
          context,
          event
        });
        context.started = true;
      }
      me.update(event);
    }
    // If a mousemove, and we are using zones, and not handles, we have to
    // programmatically check whether we are over a handle, and add/remove
    // classes to change the mouse cursor to resize.
    // If we are using handles, their CSS will set the mouse cursor.
    else if (!isTouch && !me.handleSelector) {
      me.checkResizeHandles(event);
    }
  }
  onTouchMove(event) {
    this.internalMove(true, event);
  }
  /**
   * Move grabbed element with mouse.
   * @param {MouseEvent|PointerEvent} event
   * @fires resizestart
   * @private
   */
  onMouseMove(event) {
    this.internalMove(false, event);
  }
  onPointerUp(isTouch, event) {
    var _me$removeDragListene;
    const me = this,
      context = me.context;
    (_me$removeDragListene = me.removeDragListeners) === null || _me$removeDragListene === void 0 ? void 0 : _me$removeDragListene.call(me);
    if (context) {
      var _me$scrollManager2;
      (_me$scrollManager2 = me.scrollManager) === null || _me$scrollManager2 === void 0 ? void 0 : _me$scrollManager2.stopMonitoring();
      if (context.started) {
        // Prevent the impending document click from the mouseup event from propagating
        // into a click on our element.
        EventHelper.on({
          element: document,
          thisObj: me,
          click: documentListeners.docclick,
          expires: me.clickSwallowDuration,
          // In case a click did not ensue, remove the listener
          capture: true,
          once: true
        });
      }
      me.finishResize(event);
    } else {
      var _me$reset2;
      (_me$reset2 = me.reset) === null || _me$reset2 === void 0 ? void 0 : _me$reset2.call(me);
    }
  }
  onTouchEnd(event) {
    this.onPointerUp(true, event);
  }
  /**
   * Drop on mouse up (if dropped on valid target).
   * @param {MouseEvent|PointerEvent} event
   * @private
   */
  onMouseUp(event) {
    this.onPointerUp(false, event);
  }
  /**
   * This is a capture listener, only added during drag, which prevents a click gesture
   * propagating from the terminating mouseup gesture
   * @param {MouseEvent} event
   * @private
   */
  onDocumentClick(event) {
    event.stopPropagation();
  }
  /**
   * Cancel on ESC key
   * @param {KeyboardEvent} event
   * @private
   */
  onKeyDown(event) {
    if (event.key === 'Escape') {
      this.abort();
    }
  }
  //endregion
  //region Grab, update, finish
  /**
   * Updates resize, called when an element is grabbed and mouse moves
   * @private
   * @fires resizing
   */
  update(event) {
    const me = this,
      context = me.context,
      parentRectangle = Rectangle.from(me.outerElement);
    // Calculate the current pointer X. Do not allow overflowing either edge
    context.currentX = Math.max(Math.min(event.clientX + context.cursorOffset.x, parentRectangle.right), parentRectangle.x) + me.scrollLeft;
    context.currentY = Math.max(Math.min(event.clientY + context.cursorOffset.y, parentRectangle.bottom), parentRectangle.y) + me.scrollTop;
    if (event) {
      if (me.updateResize(event)) {
        me.trigger('resizing', {
          context,
          event
        });
        context.element.classList.toggle(me.invalidCls, context.valid === false);
      }
      me.lastMouseMoveEvent = event;
    }
  }
  /**
   * Abort dragging
   */
  abort(silent = false) {
    var _me$scrollManager3, _me$scrollManager3$st;
    const me = this;
    (_me$scrollManager3 = me.scrollManager) === null || _me$scrollManager3 === void 0 ? void 0 : (_me$scrollManager3$st = _me$scrollManager3.stopMonitoring) === null || _me$scrollManager3$st === void 0 ? void 0 : _me$scrollManager3$st.call(_me$scrollManager3);
    if (me.context) {
      me.abortResize(null, silent);
    } else if (!me.isDestroyed) {
      me.reset();
    }
  }
  /**
   * Starts resizing, updates ResizeHelper#context with relevant info.
   * @private
   * @param {Boolean} isTouch
   * @param {MouseEvent} event
   * @returns {Boolean} True if handled, false if not
   */
  grabResizeHandle(isTouch, event) {
    const me = this;
    if (me.allowResize && !me.allowResize(event.target, event)) {
      return false;
    }
    const handleSelector = me.handleSelector,
      coordsFrom = event.type === 'touchstart' ? event.changedTouches[0] : event,
      clientX = coordsFrom.clientX,
      clientY = coordsFrom.clientY,
      // go up from "handle" to resizable element
      element = me.targetSelector ? event.target.closest(me.targetSelector) : event.target;
    if (element) {
      let edge;
      // Calculate which edge to resize
      // If there's a handle selector, see if it's anchored on the left or the right
      if (handleSelector) {
        if (event.target.matches(handleSelector)) {
          if (me.direction === 'horizontal') {
            if (event.pageX < DomHelper.getPageX(element) + element.offsetWidth / 2) {
              edge = me.isRTL ? 'right' : 'left';
            } else {
              edge = me.isRTL ? 'left' : 'right';
            }
          } else {
            if (event.pageY < DomHelper.getPageY(element) + element.offsetHeight / 2) {
              edge = 'top';
            } else {
              edge = 'bottom';
            }
          }
        } else {
          return false;
        }
      }
      // If we're not using handles, but just active zones
      // then test whether the event position is in an active resize zone.
      else {
        if (me.direction === 'horizontal') {
          if (me.overLeftHandle(event, element)) {
            edge = me.isRTL ? 'right' : 'left';
          } else if (me.overRightHandle(event, element)) {
            edge = me.isRTL ? 'left' : 'right';
          }
        } else {
          if (me.overTopHandle(event, element)) {
            edge = 'top';
          } else if (me.overBottomHandle(event, element)) {
            edge = 'bottom';
          }
        }
        if (!edge) {
          me.context = null;
          // not over an edge, abort
          return false;
        }
      }
      // If resizing is initiated by a touch, we must preventDefault on the touchstart
      // so that scrolling is not invoked when dragging. This is in lieu of a functioning
      // touch-action style on iOS Safari. When that's fixed, this will not be needed.
      if (event.type === 'touchstart') {
        event.preventDefault();
      }
      const cursorOffset = me.getCursorOffsetToElementEdge(coordsFrom, element, edge);
      if (me.trigger('beforeResizeStart', {
        element,
        event
      }) !== false) {
        // store initial size
        me.context = {
          element,
          edge,
          isTouch,
          valid: true,
          async: false,
          direction: me.direction,
          elementStartX: DomHelper.getTranslateX(element) || element.offsetLeft,
          // extract x from translate
          elementStartY: DomHelper.getTranslateY(element) || element.offsetTop,
          // extract y from translate
          newX: DomHelper.getTranslateX(element) || element.offsetLeft,
          // No change yet on start, but info must be present
          newY: DomHelper.getTranslateY(element) || element.offsetTop,
          // No change yet on start, but info must be present
          elementWidth: element.offsetWidth,
          elementHeight: element.offsetHeight,
          cursorOffset,
          startX: clientX + cursorOffset.x + me.scrollLeft,
          startY: clientY + cursorOffset.y + me.scrollTop,
          finalize: () => {
            var _me$reset3;
            return (_me$reset3 = me.reset) === null || _me$reset3 === void 0 ? void 0 : _me$reset3.call(me);
          }
        };
        element.classList.add(me.resizingCls);
        return true;
      }
    }
    return false;
  }
  getCursorOffsetToElementEdge(event, element, edge) {
    const rectEl = Rectangle.from(element);
    let x = 0,
      y = 0;
    switch (edge) {
      case 'left':
        x = rectEl.x - (this.isRTL ? rectEl.width : 0) - event.clientX; // negative
        break;
      case 'right':
        x = rectEl.x + (this.isRTL ? 0 : rectEl.width) - event.clientX; // positive
        break;
      case 'top':
        y = rectEl.y - event.clientY; // negative
        break;
      case 'bottom':
        y = rectEl.y + rectEl.height - event.clientY; // positive
        break;
    }
    return {
      x,
      y
    };
  }
  /**
   * Check if mouse is over a resize handle (virtual). If so, highlight.
   * @private
   * @param {MouseEvent} event
   */
  checkResizeHandles(event) {
    const me = this,
      target = me.targetSelector ? event.target.closest(me.targetSelector) : event.target;
    // mouse over a target element and allowed to resize?
    if (target && (!me.allowResize || me.allowResize(event.target, event))) {
      me.currentElement = me.handleContainerSelector ? event.target.closest(me.handleContainerSelector) : event.target;
      if (me.currentElement) {
        let over;
        if (me.direction === 'horizontal') {
          over = me.overLeftHandle(event, target) || me.overRightHandle(event, target);
        } else {
          over = me.overTopHandle(event, target) || me.overBottomHandle(event, target);
        }
        if (over) {
          me.highlightHandle(); // over handle
        } else {
          me.unHighlightHandle(); // not over handle
        }
      }
    } else if (me.currentElement) {
      me.unHighlightHandle(); // outside element
    }
  }

  onMouseLeaveTarget(event) {
    const me = this;
    me.currentElement = me.handleContainerSelector ? event.target.closest(me.handleContainerSelector) : event.target;
    if (me.currentElement) {
      me.unHighlightHandle();
    }
  }
  /**
   * Updates size of target (on mouse move).
   * @private
   * @param {MouseEvent|PointerEvent} event
   */
  updateResize(event) {
    const me = this,
      {
        context,
        allowEdgeSwitch,
        skipTranslate,
        skipUpdatingElement
      } = me;
    let updated;
    // flip which edge is being dragged depending on whether we're to the right or left of the mousedown
    if (allowEdgeSwitch) {
      if (me.direction === 'horizontal') {
        context.edge = context.currentX > context.startX ? 'right' : 'left';
      } else {
        context.edge = context.currentY > context.startY ? 'bottom' : 'top';
      }
    }
    const {
        element,
        elementStartX,
        elementStartY,
        elementWidth,
        elementHeight,
        edge
      } = context,
      {
        style
      } = element,
      // limit to outerElement if set
      deltaX = context.currentX - context.startX,
      deltaY = context.currentY - context.startY,
      minWidth = DomHelper.getExtremalSizePX(element, 'minWidth') || me.minWidth,
      maxWidth = DomHelper.getExtremalSizePX(element, 'maxWidth') || me.maxWidth,
      minHeight = DomHelper.getExtremalSizePX(element, 'minHeight') || me.minHeight,
      maxHeight = DomHelper.getExtremalSizePX(element, 'maxHeight') || me.maxHeight,
      // dragging the right edge right increases the width, dragging left edge right decreases width
      sign = edge === 'right' && !me.isRTL || edge === 'bottom' ? 1 : -1,
      // new width, not allowed to go below minWidth
      newWidth = elementWidth + deltaX * sign,
      newHeight = elementHeight + deltaY * sign;
    let width = Math.max(minWidth, newWidth),
      height = Math.max(minHeight, newHeight);
    if (maxWidth > 0) {
      width = Math.min(width, maxWidth);
    }
    if (maxHeight > 0) {
      height = Math.min(height, maxHeight);
    }
    // remove flex when resizing
    if (style.flex) {
      style.flex = '';
    }
    if (me.direction === 'horizontal' && elementWidth !== width) {
      if (!skipUpdatingElement) {
        style.width = Math.abs(width) + 'px';
      }
      context.newWidth = width;
      // when dragging left edge, also update position (so that right edge remains in place)
      if (edge === 'left' || width < 0) {
        const newX = Math.max(Math.min(elementStartX + elementWidth - me.minWidth, elementStartX + deltaX), 0);
        if (!skipTranslate) {
          DomHelper.setTranslateX(element, Math.round(newX));
        }
        context.newX = newX;
      }
      // When dragging the right edge and we're allowed to flip the drag from left to right
      // through the start point (eg drag event creation) the element must be at its initial X position
      else if (edge === 'right' && allowEdgeSwitch && !skipTranslate) {
        DomHelper.setTranslateX(element, elementStartX);
      }
      updated = true;
    } else if (me.direction === 'vertical' && elementHeight !== newHeight) {
      if (!skipUpdatingElement) {
        style.height = Math.abs(height) + 'px';
      }
      context.newHeight = height;
      // when dragging top edge, also update position (so that bottom edge remains in place)
      if (edge === 'top' || height < 0) {
        context.newY = Math.max(Math.min(elementStartY + elementHeight - me.minHeight, elementStartY + deltaY), 0);
        if (!skipTranslate) {
          DomHelper.setTranslateY(element, context.newY);
        }
      }
      // When dragging the bottom edge and we're allowed to flip the drag from top to bottom
      // through the start point (eg drag event creation) the element must be at its initial Y position
      else if (edge === 'bottom' && allowEdgeSwitch && !skipTranslate) {
        DomHelper.setTranslateY(element, elementStartY);
      }
      updated = true;
    }
    return updated;
  }
  /**
   * Finalizes resize, fires drop.
   * @private
   * @param {MouseEvent|PointerEvent} event
   * @fires resize
   * @fires cancel
   */
  finishResize(event) {
    const me = this,
      context = me.context,
      eventObject = {
        context,
        event
      };
    context.element.classList.remove(me.resizingCls);
    if (context.started) {
      let changed = false;
      if (me.direction === 'horizontal') {
        changed = context.newWidth && context.newWidth !== context.elementWidth;
      } else {
        changed = context.newHeight && context.newHeight !== context.elementHeight;
      }
      me.trigger(changed ? 'resize' : 'cancel', eventObject);
      if (!context.async) {
        context.finalize();
      }
    } else {
      var _me$reset4;
      (_me$reset4 = me.reset) === null || _me$reset4 === void 0 ? void 0 : _me$reset4.call(me);
    }
  }
  /**
   * Abort resizing
   * @private
   * @fires cancel
   */
  abortResize(event = null, silent = false) {
    const me = this,
      context = me.context;
    context.element.classList.remove(me.resizingCls);
    if (me.direction === 'horizontal') {
      // With these statements, no x pos is changed when resizing. Should therefor not reset it when cancelling
      if (context.edge === 'left' || context.allowEdgeSwitch && !context.skipTranslate) {
        DomHelper.setTranslateX(context.element, context.elementStartX);
      }
      context.element.style.width = context.elementWidth + 'px';
    } else {
      DomHelper.setTranslateY(context.element, context.elementStartY);
      context.element.style.height = context.elementHeight + 'px';
    }
    !silent && me.trigger('cancel', {
      context,
      event
    });
    if (!me.isDestroyed) {
      me.reset();
    }
  }
  //endregion
  //region Handles
  // /**
  //  * Constrain resize to outerElements bounds
  //  * @private
  //  * @param x
  //  * @returns {*}
  //  */
  // constrainResize(x) {
  //     const me = this;
  //
  //     if (me.outerElement) {
  //         const box = me.outerElement.getBoundingClientRect();
  //         if (x < box.left) x = box.left;
  //         if (x > box.right) x = box.right;
  //     }
  //
  //     return x;
  // }
  /**
   * Highlights handles (applies css that changes cursor).
   * @private
   */
  highlightHandle() {
    const me = this,
      target = me.targetSelector ? me.currentElement.closest(me.targetSelector) : me.currentElement;
    // over a handle, add cls to change cursor
    me.currentElement.classList.add('b-resize-handle');
    target.classList.add('b-over-resize-handle');
  }
  /**
   * Unhighlight handles (removes css).
   * @private
   */
  unHighlightHandle() {
    const me = this,
      target = me.targetSelector ? me.currentElement.closest(me.targetSelector) : me.currentElement;
    target && target.classList.remove('b-over-resize-handle');
    me.currentElement.classList.remove('b-resize-handle');
    me.currentElement = null;
  }
  overAnyHandle(event, target) {
    return this.overStartHandle(event, target) || this.overEndHandle(event, target);
  }
  overStartHandle(event, target) {
    return this.direction === 'horizontal' ? this.overLeftHandle(event, target) : this.overTopHandle(event, target);
  }
  overEndHandle(event, target) {
    return this.direction === 'horizontal' ? this.overRightHandle(event, target) : this.overBottomHandle(event, target);
  }
  getDynamicHandleSize(opposite, offsetWidth) {
    const handleCount = opposite ? 2 : 1,
      {
        handleSize
      } = this;
    // Shrink handle size when configured to do so, preserving reserved space between handles
    if (this.dynamicHandleSize && handleSize * handleCount > offsetWidth - this.reservedSpace) {
      return Math.max(Math.floor((offsetWidth - this.reservedSpace) / handleCount), 0);
    }
    return handleSize;
  }
  /**
   * Check if over left handle (virtual).
   * @private
   * @param {MouseEvent} event MouseEvent
   * @param {HTMLElement} target The current target element
   * @returns {Boolean} Returns true if mouse is over left handle, otherwise false
   */
  overLeftHandle(event, target) {
    const me = this,
      {
        offsetWidth
      } = target;
    if (me.leftHandle && me.canResize(target, event) && (offsetWidth >= me.handleVisibilityThreshold || me.dynamicHandleSize)) {
      const leftHandle = Rectangle.from(target);
      leftHandle.width = me.getDynamicHandleSize(me.rightHandle, offsetWidth);
      return leftHandle.width > 0 && leftHandle.contains(EventHelper.getPagePoint(event));
    }
    return false;
  }
  /**
   * Check if over right handle (virtual).
   * @private
   * @param {MouseEvent} event MouseEvent
   * @param {HTMLElement} target The current target element
   * @returns {Boolean} Returns true if mouse is over left handle, otherwise false
   */
  overRightHandle(event, target) {
    const me = this,
      {
        offsetWidth
      } = target;
    if (me.rightHandle && me.canResize(target, event) && (offsetWidth >= me.handleVisibilityThreshold || me.dynamicHandleSize)) {
      const rightHandle = Rectangle.from(target);
      rightHandle.x = rightHandle.right - me.getDynamicHandleSize(me.leftHandle, offsetWidth);
      return rightHandle.width > 0 && rightHandle.contains(EventHelper.getPagePoint(event));
    }
    return false;
  }
  /**
   * Check if over top handle (virtual).
   * @private
   * @param {MouseEvent} event MouseEvent
   * @param {HTMLElement} target The current target element
   * @returns {Boolean} Returns true if mouse is over top handle, otherwise false
   */
  overTopHandle(event, target) {
    const me = this,
      {
        offsetHeight
      } = target;
    if (me.topHandle && me.canResize(target, event) && (offsetHeight >= me.handleVisibilityThreshold || me.dynamicHandleSize)) {
      const topHandle = Rectangle.from(target);
      topHandle.height = me.getDynamicHandleSize(me.bottomHandle, offsetHeight);
      return topHandle.height > 0 && topHandle.contains(EventHelper.getPagePoint(event));
    }
    return false;
  }
  /**
   * Check if over bottom handle (virtual).
   * @private
   * @param {MouseEvent} event MouseEvent
   * @param {HTMLElement} target The current target element
   * @returns {Boolean} Returns true if mouse is over bottom handle, otherwise false
   */
  overBottomHandle(event, target) {
    const me = this,
      {
        offsetHeight
      } = target;
    if (me.bottomHandle && me.canResize(target, event) && (offsetHeight >= me.handleVisibilityThreshold || me.dynamicHandleSize)) {
      const bottomHandle = Rectangle.from(target);
      bottomHandle.y = bottomHandle.bottom - me.getDynamicHandleSize(me.bottomHandle, offsetHeight);
      return bottomHandle.height > 0 && bottomHandle.contains(EventHelper.getPagePoint(event));
    }
    return false;
  }
  //endregion
}

ResizeHelper._$name = 'ResizeHelper';

/**
 * @module Core/helper/WidgetHelper
 */
/**
 * Helper for creating widgets.
 */
class WidgetHelper {
  //region Querying
  /**
   * Returns the widget with the specified id.
   * @param {String} Id of widget to find
   * @returns {Core.widget.Widget} The widget if any
   * @category Querying
   */
  static getById(id) {
    return Widget.getById(id);
  }
  /**
   * Returns the Widget which owns the passed element (or event).
   * @param {HTMLElement|Event} element The element or event to start from
   * @param {String|Function} [type] The type of Widget to scan upwards for. The lowercase
   * class name. Or a filter function which returns `true` for the required Widget.
   * @param {HTMLElement|Number} [limit] The number of components to traverse upwards to find a
   * match of the type parameter, or the element to stop at.
   * @returns {Core.widget.Widget} The found Widget or null.
   * @category Querying
   */
  static fromElement(element, type, limit) {
    return Widget.fromElement(element, type, limit);
  }
  //endregion
  //region Widgets
  /**
   * Create a widget.
   * @example
   * WidgetHelper.createWidget({
   *   type: 'button',
   *   icon: 'user',
   *   text: 'Edit user'
   * });
   * @param {ContainerItemConfig} config Widget config
   * @returns {Core.widget.Widget} The widget
   * @category Widgets
   */
  static createWidget(config = {}) {
    return config.isWidget ? config : Widget.create(config);
  }
  /**
   * Appends a widget (array of widgets) to the DOM tree. If config is empty, widgets are appended to the DOM. To
   * append widget to certain position you can pass HTMLElement or its id as config, or as a config, that will be
   * applied to all passed widgets.
   *
   * Usage:
   *
   * ```javascript
   * // Will append button as last item to element with id 'container'
   * let [button] = WidgetHelper.append({ type : 'button' }, 'container');
   *
   * // Same as above, but will add two buttons
   * let [button1, button2] = WidgetHelper.append([
   *     { type : 'button' },
   *     { type : 'button' }
   *     ], { appendTo : 'container' });
   *
   * // Will append two buttons before element with id 'someElement'. Order will be preserved and all widgets will have
   * // additional class 'my-cls'
   * let [button1, button2] = WidgetHelper.append([
   *     { type : 'button' },
   *     { type : 'button' }
   *     ], {
   *         insertBefore : 'someElement',
   *         cls          : 'my-cls'
   *     });
   * ```
   *
   * @param {ContainerItemConfig|ContainerItemConfig[]} widget Widget config or array of such configs
   * @param {HTMLElement|String|Object} [config] Element (or element id) to which to append the widget or config to
   * apply to all passed widgets
   * @returns {Core.widget.Widget[]} Array or widgets
   * @category Widgets
   */
  static append(widget, config) {
    widget = Array.isArray(widget) && widget || [widget];
    if (config instanceof HTMLElement || typeof config === 'string') {
      config = {
        appendTo: config
      };
    }
    // We want to fix position to insert into to keep order of passed widgets
    if (config.insertFirst) {
      const target = typeof config.insertFirst === 'string' ? document.getElementById(config.insertFirst) : config.insertFirst;
      if (target.firstChild) {
        config.insertBefore = target.firstChild;
      } else {
        config.appendTo = target;
      }
    }
    return widget.map(item => Widget.create(ObjectHelper.assign({}, config || {}, item)));
  }
  //endregion
  //region Popups
  /**
   * Shows a popup (~tooltip) containing widgets connected to specified element.
   * @example
   * WidgetHelper.openPopup(element, {
   *   position: 'bottom center',
   *   items: [
   *      { widgetConfig }
   *   ]
   * });
   * @param {HTMLElement} element Element to connect popup to
   * @param {PopupConfig} config Config object, or string to use as html in popup
   * @returns {*|{close, widgets}}
   * @category Popups
   */
  static openPopup(element, config) {
    return Widget.create(ObjectHelper.assign({
      forElement: element
    }, typeof config === 'string' ? {
      html: config
    } : config), 'popup');
  }
  /**
   * Shows a context menu connected to the specified element.
   * @example
   * WidgetHelper.showContextMenu(element, {
   *   items: [
   *      { id: 'addItem', icon: 'add', text: 'Add' },
   *      ...
   *   ],
   *   onItem: item => alert('Clicked ' + item.text)
   * });
   * @param {HTMLElement|Number[]} element Element (or a coordinate) to show the context menu for
   * @param {MenuItemConfig} config Context menu config, see example
   * @returns {Core.widget.Menu}
   * @category Popups
   */
  static showContextMenu(element, config) {
    const me = this;
    if (me.currentContextMenu) {
      me.currentContextMenu.destroy();
    }
    if (element instanceof HTMLElement) {
      config.forElement = element;
    } else {
      config.forElement = document.body;
      if (Array.isArray(element)) {
        element = new Point(...element);
      }
      if (element instanceof Point) {
        config.align = {
          position: element
        };
      }
    }
    config.internalListeners = {
      destroy: me.currentContextMenu = null
    };
    return me.currentContextMenu = Widget.create(config, 'menu');
  }
  /**
   * Attached a tooltip to the specified element.
   * @example
   * WidgetHelper.attachTooltip(element, {
   *   text: 'Useful information goes here'
   * });
   * @param {HTMLElement} element Element to attach tooltip for
   * @param {String|TooltipConfig} configOrText Tooltip config or tooltip string, see example and source
   * @returns {HTMLElement} The passed element
   * @category Popups
   */
  static attachTooltip(element, configOrText) {
    return Widget.attachTooltip(element, configOrText);
  }
  /**
   * Checks if element has tooltip attached
   *
   * @param {HTMLElement} element Element to check
   * @returns {Boolean}
   * @category Popups
   */
  static hasTooltipAttached(element) {
    return Widget.resolveType('tooltip').hasTooltipAttached(element);
  }
  /**
   * Destroys any tooltip attached to an element, removes it from the DOM and unregisters any tip related listeners
   * on the element.
   *
   * @param {HTMLElement} element Element to remove tooltip from
   * @category Popups
   */
  static destroyTooltipAttached(element) {
    return Widget.resolveType('tooltip').destroyTooltipAttached(element);
  }
  //endregion
  //region Mask
  /**
   * Masks the specified element, showing a message in the mask.
   * @param {HTMLElement} element Element to mask
   * @param {String} msg Message to show in the mask
   * @category Mask
   */
  static mask(element, msg = 'Loading') {
    if (element) {
      // Config object normalization
      if (element instanceof HTMLElement) {
        element = {
          target: element,
          text: msg
        };
      }
      return Mask.mask(element, element.target);
    }
  }
  /**
   * Unmask the specified element.
   * @param {HTMLElement} element
   * @category Mask
   */
  static unmask(element, close = true) {
    if (element.mask) {
      if (close) {
        element.mask.close();
      } else {
        element.mask.hide();
      }
    }
  }
  //endregion
  //region Toast
  /**
   * Show a toast
   * @param {String} msg message to show in the toast
   * @category Mask
   */
  static toast(msg) {
    return Toast.show(msg);
  }
  //endregion
}

WidgetHelper._$name = 'WidgetHelper';

const hasOwnProperty = Object.prototype.hasOwnProperty;
let cacheKey = null;
function setParser(me, parser) {
  Object.defineProperty(me, 'parser', {
    value: parser
  });
  return parser;
}
class Default {
  constructor(formatter) {
    this.formatter = formatter;
  }
  format(value) {
    return this.formatter.defaultFormat(value);
  }
  parse(value, strict) {
    return this.formatter.defaultParse(value, strict);
  }
  resolvedOptions() {
    return null;
  }
}
// This class does not extend Core.Base because instances are not reconfigurable (making
// setConfig harmful) nor destroyable. Instead, they get frozen and cached according to
// their "config" definition.
/**
 * Abstract base class for formatters.
 * @private
 */
class Formatter {
  static get(format) {
    if (format == null) {
      return this.NULL;
    }
    if (format instanceof this) {
      return format;
    }
    const key = typeof format === 'string' ? format : JSON.stringify(format),
      cache = this.cache;
    let fmt = cache.get(key);
    if (!fmt) {
      cacheKey = key; // this is grabbed by our constructor below...
      fmt = new this(format);
      cache.set(key, fmt);
    }
    return fmt;
  }
  static get cache() {
    return hasOwnProperty.call(this, '_cache') && this._cache || (this._cache = new Map());
  }
  static get NULL() {
    return hasOwnProperty.call(this, '_null') ? this._null : this._null = new this(null);
  }
  constructor(config) {
    const me = this;
    // This is done in a funny way so as not to complicate the derived constructor's
    // desire to maintain a single argument signature, as well as it's calling of
    // Object.freeze() to ensure immutability in dev mode.
    me.cacheKey = cacheKey;
    cacheKey = null;
    me.initialize();
    if (config === null) {
      me.formatter = new Default(me);
    } else {
      me.configure(config);
      // Bring locale and other defaulted options back onto this object:
      for (const [key, value] of Object.entries(me.resolvedOptions())) {
        // For some reason (locale-specific perhaps), resolvedOptions returns
        // with 'undefined' in some keys (e.g., min/maximumFractionDigits) when
        // we specified 0.
        //
        // The second check is to only bring back values that we understand.
        if (value != null && key in me.defaults) {
          me[key] = value;
        }
      }
    }
  }
  get parser() {
    // Replace this property w/the actual instance:
    return setParser(this, new this.constructor.Parser(this));
  }
  defaultFormat(value) {
    return value == null ? value : String(value);
  }
  defaultParse(value) {
    return value;
  }
  format(value) {
    return value == null ? value : this.formatter.format(value);
  }
  parse(value, strict) {
    return value == null ? value : this.parser.parse(value, strict);
  }
  parseStrict(value) {
    return this.parse(value, true);
  }
  resolvedOptions() {
    return this.formatter.resolvedOptions();
  }
}
Formatter._$name = 'Formatter';

/**
 * @module Core/helper/util/NumberFormat
 */
const escapeRegExp = StringHelper.escapeRegExp,
  digitsRe = /[\d+-]/g,
  // We cannot pass locale=null:
  newFormatter = (locale, config) => new Intl.NumberFormat(locale || undefined, config),
  numFormatRe = /^(?:([$])\s*)?(?:(\d+)>)?\d+(,\d+)?(?:\.((\d*)(?:#*)|[*]))?(?:\s*([%])?)?$/,
  unicodeMinus = '\u2212';
class NumberParser {
  constructor(formatter) {
    const me = this,
      locale = formatter.locale,
      // We need a formatter for this locale that has decimals and grouping:
      numFmt = newFormatter(locale, {
        maximumFractionDigits: 3
      }),
      currency = formatter.is.currency ? me._decodeStyle(locale, {
        style: 'currency',
        currency: formatter.currency,
        currencyDisplay: formatter.currencyDisplay
      }) : null,
      percent = formatter.is.percent ? me._decodeStyle(locale, {
        style: 'percent'
      }) : null,
      decimal = numFmt.format(1.2).replace(digitsRe, '')[0],
      grouper = numFmt.format(1e9).replace(digitsRe, '')[0] || '';
    Object.assign(me, {
      currency,
      decimal,
      formatter,
      grouper,
      percent
    });
    me.decimal = decimal;
    me.decimalRe = escapeRegExp(decimal, 'g');
    me.grouper = grouper;
    // The stripRe removes whitespace, currency text, percent text and grouping chars:
    me.stripRe = new RegExp(`(?:\\s+|${escapeRegExp(grouper)})` + (currency ? `|(?:${escapeRegExp(currency.text)})` : '') + (percent ? `|(?:${escapeRegExp(percent.text)})` : ''), 'g');
  }
  decimalPlaces(value) {
    value = value.replace(this.stripRe, '');
    const dot = value.indexOf(this.decimal) + 1;
    return dot && value.length - dot;
  }
  parse(value, strict) {
    if (typeof value === 'string') {
      value = value.replace(this.stripRe, '').replace(this.decimalRe, '.').replace(unicodeMinus, '-');
      value = strict ? Number(value) : parseFloat(value);
      if (this.formatter.is.percent) {
        value /= 100;
      }
    }
    // else, a number is already parsed but could be null...
    return value;
  }
  _decodeStyle(locale, fmtDef) {
    const fmt = newFormatter(locale, fmtDef),
      decFmt = newFormatter(locale, Object.assign(fmt.resolvedOptions(), {
        style: 'decimal'
      })),
      zero = fmt.format(0),
      // = '0%' or '$0.00' in en-US
      zeroDec = decFmt.format(0); // = '0' or '0.00' in en-US
    return {
      suffix: zero.startsWith(zeroDec),
      text: zero.replace(zeroDec, '').trim()
    };
  }
}
/**
 * This class is an enhancement to `Intl.NumberFormat` that has a more flexible
 * constructor as well as other features such as `parse()`.
 *
 * All constructor forms take a single argument. The most common is to pass a format
 * {@link #config-template} string:
 *```
 *  const formatter = new NumberFormat('9,999.99##');
 *```
 * The above is equivalent to:
 *```
 *  const formatter = new Intl.NumberFormat({
 *      useGrouping           : true,
 *      minimumFractionDigits : 2,
 *      maximumFractionDigits : 4
 *  });
 *```
 * The `formatter` created above is used as follows (in the `en-US` locale):
 *```
 *  console.log(formatter.format(12345.54321));
 *  console.log(formatter.format(42));
 *
 *  // 12,345.5432
 *  // 42.00
 *```
 * When a format template is insufficient, a config object can be provided, similar to
 * `Intl.NumberFormat`'s `options` parameter. While all options from `Intl.NumberFormat`
 * are valid properties for this class's config object, additional properties are
 * supported.
 *
 * For example:
 *```
 *  new NumberFormat({
 *      locale      : 'en-US',
 *      template    : '$9,999',
 *      currency    : 'USD',
 *      significant : 5
 *  });
 *```
 * The `locale` option takes the place of the first positional parameter to the
 * `Intl.NumberFormat` constructor. The `template` config is the same string that can be
 * passed by itself.
 *
 * The shorthand properties `fraction`, `integer`, and `significant` set the standard
 * options `minimumFractionDigits`, `maximumFractionDigits`, `minimumIntegerDigits`,
 * `minimumSignificantDigits`, and `maximumSignificantDigits`.
 *
 * NOTE: Instances of `NumberFormat` are immutable after construction.
 *
 * For details about `Intl.NumberFormat` see [MDN](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/NumberFormat).
 */
class NumberFormat extends Formatter {
  static get $name() {
    return 'NumberFormat';
  }
  initialize() {
    this._as = {
      // cacheKey : cachedInstance
    };
    this.is = {
      decimal: false,
      currency: false,
      percent: false,
      null: true,
      from: null
    };
  }
  get truncator() {
    const scale = this.maximumFractionDigits;
    return scale == null ? null : this.as({
      style: 'decimal',
      maximumFractionDigits: Math.min(20, scale + 1)
    }, 'truncator');
  }
  configure(options) {
    const me = this;
    if (typeof options !== 'string') {
      Object.assign(me, options);
    } else {
      me.template = options;
    }
    const config = {},
      loc = me.locale ? LocaleManagerSingleton.locales[me.locale] : LocaleManagerSingleton.locale,
      defaults = loc && loc.NumberFormat,
      template = me.template;
    if (defaults) {
      for (const key in defaults) {
        if (me[key] == null && typeof defaults[key] !== 'function') {
          me[key] = defaults[key];
        }
      }
    }
    if (template) {
      const match = numFormatRe.exec(template),
        m2 = match[2],
        m4 = match[4];
      me.useGrouping = !!match[3];
      me.style = match[1] ? 'currency' : match[6] ? 'percent' : 'decimal';
      if (m2) {
        me.integer = +m2;
      }
      if (m4 === '*') {
        me.fraction = [0, 20];
      } else if (m4 != null) {
        me.fraction = [match[5].length, m4.length];
      }
    }
    me._minMax('fraction', true, true);
    me._minMax('integer', true, false);
    me._minMax('significant', false, true);
    for (const key in me.defaults) {
      if (me[key] != null) {
        config[key] = me[key];
      }
    }
    me.is.from = me.from && me.from.is;
    me.is[me.style] = !(me.is.null = false);
    me.formatter = newFormatter(me.locale, config);
  }
  /**
   * Creates a derived `NumberFormat` from this instance, with a different `style`. This is useful for processing
   * currency and percentage styles without the symbols being injected in the formatting.
   *
   * @param {String|Object} change The new style (if a string) or a set of properties to update.
   * @param {String} [cacheAs] A key by which to cache this derived formatter.
   * @returns {Core.helper.util.NumberFormat}
   */
  as(change, cacheAs = null) {
    const config = this.resolvedOptions() || {
        template: '9.*'
      },
      cache = this._as;
    let ret = cacheAs && cache[cacheAs];
    if (!ret) {
      if (typeof change === 'string') {
        config.style = change;
      } else {
        Object.assign(config, change);
      }
      config.from = this;
      ret = new NumberFormat(config);
    }
    if (cacheAs) {
      cache[cacheAs] = ret;
    }
    return ret;
  }
  defaultParse(value, strict) {
    return value == null ? value : strict ? Number(value) : parseFloat(value);
  }
  /**
   * Returns the given `value` formatted in accordance with the specified locale and
   * formatting options.
   *
   * @param {Number} value
   * @returns {String}
   */
  format(value) {
    if (typeof value === 'string') {
      const v = Number(value);
      value = isNaN(v) ? this.parse(value) : v;
    }
    return super.format(value);
  }
  // The parse() method is inherited but the base class implementation
  // cannot properly document the parameter and return types:
  /**
   * Returns a `Number` parsed from the given, formatted `value`, in accordance with the
   * specified locale and formatting options.
   *
   * If the `value` cannot be parsed, `NaN` is returned.
   *
   * Pass `strict` as `true` to require all text to convert. In essence, the default is
   * in line with JavaScript's `parseFloat` while `strict=true` behaves like the `Number`
   * constructor:
   *```
   *  parseFloat('1.2xx');  // = 1.2
   *  Number('1.2xx')       // = NaN
   *```
   * @method parse
   * @param {String} value
   * @param {Boolean} [strict=false]
   * @returns {Number}
   */
  /**
   * Returns a `Number` parsed from the given, formatted `value`, in accordance with the
   * specified locale and formatting options.
   *
   * If the `value` cannot be parsed, `NaN` is returned.
   *
   * This method simply passes the `value` to `parse()` and passes `true` for the second
   * argument.
   *
   * @method parseStrict
   * @param {String} value
   * @returns {Number}
   */
  /**
   * Returns the given `Number` rounded in accordance with the specified locale and
   * formatting options.
   *
   * @param {Number|String} value
   * @returns {Number}
   */
  round(value) {
    return this.parse(this.format(value));
  }
  /**
   * Returns the given `Number` truncated to the `maximumFractionDigits` in accordance
   * with the specified locale and formatting options.
   *
   * @param {Number|String} value
   * @returns {Number}
   */
  truncate(value) {
    const me = this,
      scale = me.maximumFractionDigits,
      {
        truncator
      } = me;
    let v = me.parse(value),
      dot;
    if (truncator) {
      v = truncator.format(v);
      dot = v.indexOf(truncator.parser.decimal);
      if (dot > -1 && v.length - dot - 1 > scale) {
        v = v.substr(0, dot + scale + 1);
      }
      v = truncator.parse(v);
    }
    return v;
  }
  resolvedOptions() {
    const options = super.resolvedOptions();
    for (const key in options) {
      // For some reason, on TeamCity, tests get undefined for some properties...
      // maybe a locale issue?
      if (options[key] === undefined) {
        options[key] = this[key];
      }
    }
    return options;
  }
  /**
   * Expands the provided shorthand into the "minimum*Digits" and "maximum*Digits".
   * @param {String} name
   * @param {Boolean} setMin
   * @param {Boolean} setMax
   * @private
   */
  _minMax(name, setMin, setMax) {
    const me = this,
      value = me[name];
    if (value != null) {
      const capName = StringHelper.capitalize(name),
        max = `maximum${capName}Digits`,
        min = `minimum${capName}Digits`;
      if (typeof value === 'number') {
        if (setMin) {
          me[min] = value;
        }
        if (setMax) {
          me[max] = value;
        }
      } else {
        me[min] = value[0];
        me[max] = value[1];
      }
    }
  }
}
NumberFormat.Parser = NumberParser;
Object.assign(NumberFormat.prototype, {
  // This object holds only those properties that Intl.NumberFormat accepts in its
  // "options" parameter. Only these options will be copied from the NumberFormat
  // and passed to the Intl.NumberFormat constructor and only these will be copied
  // back from its resolvedOptions:
  defaults: {
    /**
     * The formatting style.
     *
     * Valid values are: `'decimal'` (the default), `'currency'`, and `'percent'`.
     * @config {'decimal'|'currency'|'percent'}
     * @default
     */
    style: 'decimal',
    /**
     * The currency to use when using `style: 'currency'`. For example, `'USD'` (US dollar)
     * or `'EUR'` for the euro.
     *
     * If not provided, the {@link Core.localization.LocaleManager} default will be used.
     * @config {Boolean}
     */
    currency: null,
    /**
     * The format in which to display the currency value when using `style: 'currency'`.
     *
     * Valid values are: `'symbol'` (the default), `'code'`, and `'name'`.
     * @config {'symbol'|'code'|'name'}
     * @default
     */
    currencyDisplay: 'symbol',
    /**
     * The name of the locale. For example, `'en-US'`. This config is the same as the
     * first argument to the `Intl.NumberFormat` constructor.
     *
     * Defaults to the browser's default locale.
     * @config {String}
     */
    locale: null,
    /**
     * The maximum number of digits following the decimal.
     *
     * This is more convenient to specify using the {@link #config-fraction} config.
     * @config {Number}
     */
    maximumFractionDigits: null,
    /**
     * The minimum number of digits following the decimal.
     *
     * This is more convenient to specify using the {@link #config-fraction} config.
     * @config {Number}
     */
    minimumFractionDigits: null,
    /**
     * The minimum number of digits preceding the decimal.
     *
     * This is more convenient to specify using the {@link #config-integer} config.
     * @config {Number}
     */
    minimumIntegerDigits: null,
    /**
     * The maximum number of significant digits.
     *
     * This is more convenient to specify using the {@link #config-significant} config.
     * @config {Number}
     */
    maximumSignificantDigits: null,
    /**
     * The minimum number of significant digits.
     *
     * This is more convenient to specify using the {@link #config-significant} config.
     * @config {Number}
     */
    minimumSignificantDigits: null,
    /**
     * Specify `false` to disable thousands separators.
     * @config {Boolean}
     * @default
     */
    useGrouping: true
  },
  /**
   * Specifies the `minimumFractionDigits` and `minimumFractionDigits` in a compact
   * way. If this value is a `Number`, it sets both the minimum and maximum to that
   * value. If this value is an array, `[0]` sets the minimum and `[1]` sets the
   * maximum.
   * @config {Number|Number[]}
   */
  fraction: null,
  from: null,
  /**
   * An alias for `minimumIntegerDigits`.
   * @config {Number}
   */
  integer: null,
  /**
   * Specifies the `minimumSignificantDigits` and `minimumSignificantDigits` in a compact
   * format. If this value is a `Number`, it sets only the maximum to that value. If this
   * value is an array, `[0]` sets the minimum and `[1]` sets the maximum.
   *
   * If this value (or `minimumSignificantDigits` or `minimumSignificantDigits`) is set,
   * `integer` (and `minimumIntegerDigits`) and `fraction` (and `minimumFractionDigits`
   * and `minimumFractionDigits`) are ignored.
   *
   * @config {Number|Number[]}
   */
  significant: null,
  /**
   * A format template consisting of the following parts:
   *```
   *  [$] [\d+:] \d+ [,\d+] [.\d* [#*] | *] [%]
   *```
   * If the template begins with a `'$'`, the formatter's `style` option is set to
   * `'currency'`. If the template ends with `'%'`, `style` is set to `'percent'`.
   * It is invalid to include both characters. When using `'$'`, the `currency` symbol
   * defaults to what is provided by the {@link Core.localization.LocaleManager}.
   *
   * To set the `minimumIntegerDigits`, the desired minimum comes before the first
   * digits in the template and is followed by a `'>'` (greater-than). For example:
   *```
   *  5>9,999.00
   *```
   * The above sets `minimumIntegerDigits` to 5.
   *
   * The `useGrouping` option is enabled if there is a `','` (comma) present and is
   * disabled otherwise.
   *
   * If there is a `'.'` (decimal) present, it may be followed by either of:
   *
   *  - Zero or more digits which may then be followed by zero or more `'#'` characters.
   *    The number of digits determines the `minimumFractionDigits`, while the total
   *    number of digits and `'#'`s determines the `maximumFractionDigits`.
   *  - A single `'*'` (asterisk) indicating any number of fractional digits (no minimum
   *    or maximum).
   *
   * @config {String}
   */
  template: null
});
Object.assign(NumberFormat.prototype, NumberFormat.prototype.defaults);
Formatter.number = (format, value) => NumberFormat.format(format, value);
NumberFormat._$name = 'NumberFormat';

/**
 * @module Core/util/Month
 */
/**
 * A class which encapsulates a calendar view of a month, and offers information about
 * the weeks and days within that calendar view.
 *
 * ```javascript
 *   // December 2018 using Monday as week start
 *   const m = new Month({
 *       date         : '2018-12-01',
 *       weekStartDay : 1
 *   });
 *
 *   m.eachWeek((week, dates) => console.log(dates.map(d => d.getDate())))
 * ```
 */
class Month extends Events(Base) {
  static $name = 'Month';
  static get configurable() {
    return {
      /**
       * The date which the month should encapsulate. May be a `Date` object, or a
       * `YYYY-MM-DD` format string.
       *
       * Mutating a passed `Date` after initializing a `Month` object has no effect on
       * the `Month` object.
       * @config {Date|String}
       */
      date: {
        $config: {
          equal: 'date'
        },
        value: DateHelper.clearTime(new Date())
      },
      month: null,
      year: null,
      /**
       * The week start day, 0 meaning Sunday, 6 meaning Saturday.
       * Defaults to {@link Core.helper.DateHelper#property-weekStartDay-static}.
       * @config {Number}
       */
      weekStartDay: null,
      /**
       * Configure as `true` to have the visibleDayColumnIndex and visibleColumnCount properties
       * respect the configured {@link #config-nonWorkingDays}.
       * @config {Boolean}
       */
      hideNonWorkingDays: null,
      /**
       * Non-working days as an object where keys are day indices, 0-6 (Sunday-Saturday), and the value is `true`.
       * Defaults to {@link Core.helper.DateHelper#property-nonWorkingDays-static}.
       * @config {Object<String,Boolean>}
       */
      nonWorkingDays: null,
      /**
       * Configure as `true` to always have the month encapsulate six weeks.
       * This is useful for UIs which must be a fixed height.
       * @prp {Boolean}
       */
      sixWeeks: null
    };
  }
  //region events
  /**
   * Fired when setting the {@link #config-date} property causes the encapsulated date to change
   * in **any** way, date, week, month or year.
   * @event dateChange
   * @param {Core.util.Month} source The Month which triggered the event.
   * @param {Date} newDate The new encapsulated date value.
   * @param {Date} oldDate The previous encapsulated date value.
   * @param {Number} changes An object which contains properties which indicate what part of the date changed.
   * @param {Boolean} changes.d True if the date changed in any way.
   * @param {Boolean} changes.w True if the week changed (including same week in a different year).
   * @param {Boolean} changes.m True if the month changed (including same month in a different year).
   * @param {Boolean} changes.y True if the year changed.
   * @param {Boolean} changes.r True if the row count (with respect to the {@link #config-sixWeeks} setting) changed.
   */
  /**
   * Fired when setting the {@link #config-date} property causes a change of week. Note that
   * weeks are calculated in the ISO standard form such that if there are fewer than four
   * days in the first week of a year, then that week is owned by the previous year.
   *
   * The {@link #config-weekStartDay} is honoured when making this calculation and this is a
   * locale-specific value which defaults to the ISO standard of 1 (Monday) in provided European
   * locales and 0 (Sunday) in the provided US English locale.
   * @event weekChange
   * @param {Core.util.Month} source The Month which triggered the event.
   * @param {Date} newDate The new encapsulated date value.
   * @param {Date} oldDate The previous encapsulated date value.
   * @param {Number} changes An object which contains properties which indicate what part of the date changed.
   * @param {Boolean} changes.d True if the date changed in any way.
   * @param {Boolean} changes.w True if the week changed (including same week in a different year).
   * @param {Boolean} changes.m True if the month changed (including same month in a different year).
   * @param {Boolean} changes.y True if the year changed.
   * @param {Boolean} changes.r True if the row count (with respect to the {@link #config-sixWeeks} setting) changed.
   */
  /**
   * Fired when setting the {@link #config-date} property causes a change of month. This
   * will fire when changing to the same month in a different year.
   * @event monthChange
   * @param {Core.util.Month} source The Month which triggered the event.
   * @param {Date} newDate The new encapsulated date value.
   * @param {Date} oldDate The previous encapsulated date value.
   * @param {Number} changes An object which contains properties which indicate what part of the date changed.
   * @param {Boolean} changes.d True if the date changed in any way.
   * @param {Boolean} changes.w True if the week changed (including same week in a different year).
   * @param {Boolean} changes.m True if the month changed (including same month in a different year).
   * @param {Boolean} changes.y True if the year changed.
   * @param {Boolean} changes.r True if the row count (with respect to the {@link #config-sixWeeks} setting) changed.
   */
  /**
   * Fired when setting the {@link #config-date} property causes a change of year.
   * @event yearChange
   * @param {Core.util.Month} source The Month which triggered the event.
   * @param {Date} newDate The new encapsulated date value.
   * @param {Date} oldDate The previous encapsulated date value.
   * @param {Number} changes An object which contains properties which indicate what part of the date changed.
   * @param {Boolean} changes.d True if the date changed in any way.
   * @param {Boolean} changes.w True if the week changed (including same week in a different year).
   * @param {Boolean} changes.m True if the month changed (including same month in a different year).
   * @param {Boolean} changes.y True if the year changed.
   * @param {Boolean} changes.r True if the row count (with respect to the {@link #config-sixWeeks} setting) changed.
   */
  //endRegion
  /**
   * For use when this Month's `weekStartDay` is non-zero.
   *
   * An array to map the days of the week 0-6 of this Calendar to the canonical day numbers
   * used by the Javascript [Date](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date) object.
   * @member {Number[]} canonicalDayNumbers
   * @readonly
   */
  /**
   * An array to map a canonical day number to a *visible* column index.
   * For example, if we have `weekStartDay` as Monday which is 1, and non working days as
   * Wednesday, and `hideNonWorkingDays : true`, then the calendar would look like
   *
   *```
   * ┌────┬────┬────┬────┬────┬────┐
   * | Mo | Tu | Th | Fr | Sa | Su |
   * └────┴────┴────┴────┴────┴────┘
   *```
   *
   * So we'd need this array: `[ 5, 0, 1, undefined, 2, 3, 4]`
   * @member {Number[]} visibleDayColumnIndex
   * @readonly
   */
  /**
   * An array to map a canonical day number to a 0-6 column index.
   * For example, if we have `weekStartDay` as Monday which is 1, then the calendar would look like
   *
   *```
   * ┌────┬────┬────┬────┬────┬────┬────┐
   * | Mo | Tu | We | Th | Fr | Sa | Su |
   * └────┴────┴────┴────┴────┴────┴────┘
   *```
   *
   * So we'd need this array: `[ 6, 0, 1, 2, 3, 4, 5]`
   * @member {Number[]} dayColumnIndex
   * @readonly
   */
  /**
   * The number of visible days in the week as defined by the `nonWorkingDays` and
   * `hideNonWorkingDays` options.
   * @member {Number} weekLength
   * @readonly
   */
  configure(config) {
    super.configure(config);
    this.updateDayNumbers();
    // The set is rejected during configuration because everything else has to be set up.
    if (config.date) {
      this.date = config.date;
    }
    this.generation = 0;
  }
  changeDate(date) {
    // Date has to be set after we know everything else
    if (this.isConfiguring) {
      return;
    }
    date = typeof date === 'string' ? DateHelper.parse(date, 'YYYY-MM-DD') : new Date(date);
    if (isNaN(date)) {
      throw new Error('Month date ingestion must be passed a Date, or a valid YYYY-MM-DD date string');
    }
    return date;
  }
  updateDate(newDate, oldDate) {
    const me = this,
      {
        dayColumnIndex,
        weekCount
      } = me,
      monthStart = DateHelper.getFirstDateOfMonth(newDate),
      monthEnd = DateHelper.getLastDateOfMonth(monthStart),
      startWeekDay = dayColumnIndex[monthStart.getDay()],
      endWeekDay = dayColumnIndex[monthEnd.getDay()],
      yearChanged = !oldDate || newDate.getFullYear() !== oldDate.getFullYear(),
      monthChanged = !oldDate || newDate.getMonth() !== oldDate.getMonth(),
      // Collect changes as bitwise flags if we have any listeners:
      // 0001 = date has changed.
      // 0010 = week has changed.
      // 0100 = month has changed.
      // 1000 = year has changed.
      // We need this because 10/1/2010 -> 10/1/2011 must fire a dateChange
      // and a monthChange in addition to the yearChange.
      // And 10/1/2010 -> 10/2/2010 must fire a dateChange in addition to the monthChange.
      changes = me.eventListeners && (oldDate ? newDate.getDate() !== oldDate.getDate() | (me.getWeekId(newDate) !== me.getWeekId(oldDate)) << 1 | monthChanged << 2 | yearChanged << 3 : 15);
    // Keep our properties in sync with reality.
    // Access the private properties directly to avoid recursion.
    me._year = newDate.getFullYear();
    me._month = newDate.getMonth();
    // These comments assume ISO standard of Monday as week start day.
    //
    // This is the date of month that is the beginning of the first week row.
    // So this may be -ve. Eg: for Dec 2018, Monday 26th Nov is the first
    // cell on the calendar which is the -4th of December. Note that the 0th
    // of December was 31st of November, so needs -4 to get back to the 26th.
    me.startDayOfMonth = 1 - startWeekDay;
    // This is the date of month that is the end of the last week row.
    // So this may be > month end. Eg: for Dec 2018, Sunday 6th Jan is the last
    // cell on the calendar which is the 37th of December.
    me.endDayOfMonth = monthEnd.getDate() + (6 - endWeekDay);
    if (me.sixWeeks) {
      me.endDayOfMonth += (6 - me.weekCount) * 7;
    }
    // Calculate the start point of where we calculate weeks from if we need to.
    // It's either the first "weekStartDay" in this year if this year's
    // first week is last year's, and so should work out as zero,
    // or the "weekStartDay" of the week before, so that dates in the first week
    // the Math.floor(DH.diff(weekBase, date, 'day') / 7) calculates as 1.
    if (!me.weekBase || yearChanged) {
      me.calculateWeekBase();
    }
    // Allow calling code to detect whether a set date operation resulted in a change
    // of month.
    if (monthChanged || yearChanged) {
      me.generation++;
    }
    if (changes) {
      const event = {
        newDate,
        oldDate,
        changes: {
          d: true,
          w: Boolean(changes & 2),
          m: Boolean(changes & 12),
          y: Boolean(changes & 8),
          r: me.weekCount !== weekCount
        }
      };
      // If either date, month or year changes, we fire a dateChange
      me.trigger('dateChange', event);
      // If the week has changed, fire a weekChange
      if (changes & 2) {
        me.trigger('weekChange', event);
      }
      // If month or year changed, we fire a monthChange
      if (changes & 12) {
        me.trigger('monthChange', event);
      }
      // If the year has changed, fire a yearChange
      if (changes & 8) {
        me.trigger('yearChange', event);
      }
    }
  }
  calculateWeekBase() {
    const me = this,
      {
        dayColumnIndex
      } = me,
      jan1 = new Date(me.year, 0, 1),
      dec31 = new Date(me.year, 11, 31),
      january = me.month ? me.getOtherMonth(jan1) : me;
    // First 7 days are in last week of previous year if the year
    // starts after our 4th day of week.
    if (me.dayColumnIndex[jan1.getDay()] > 3) {
      // Week base is calculated from the year start
      me.weekBase = january.startDate;
    }
    // First 7 days are in week 1 of this year
    else {
      // Week base is the start of week before
      me.weekBase = new Date(me.year, 0, january.startDayOfMonth - 7);
    }
    const dec31Week = Math.floor(DateHelper.diff(me.weekBase, dec31, 'day') / 7);
    // Our year only has a 53rd week if 53rd week ends after our week's 3rd day
    me.has53weeks = dec31Week === 53 && dayColumnIndex[dec31.getDay()] > 2;
  }
  /**
   * Returns the week start date, based on the configured {@link #config-weekStartDay} of the
   * passed week.
   * @param {Number| Number[]} week The week number in the current year, or an array containing
   * `[year, weekOfYear]` for any year.
   *
   * Week numbers greater than the number of weeks in the year just wrap into the following year.
   */
  getWeekStart(week) {
    // Week number n of current year
    if (typeof week === 'number') {
      return DateHelper.add(this.weekBase, Math.max(week, 1) * 7, 'day');
    }
    // Week n of year nnnn
    const me = this,
      [year, weekOfYear] = week;
    // nnnn is our year, so we know it
    if (year === me.year) {
      return me.getWeekStart(weekOfYear);
    }
    return me.getOtherMonth(new Date(year, 0, 1)).getWeekStart(weekOfYear);
  }
  getOtherMonth(date) {
    const me = this,
      result = me === otherMonth ? new Month(null) : otherMonth;
    result.configure({
      weekBase: null,
      weekStartDay: me.weekStartDay,
      nonWorkingDays: me.nonWorkingDays,
      hideNonWorkingDays: me.hideNonWorkingDays,
      sixWeeks: me.sixWeeks,
      date: new Date(date.getFullYear(), 0, 1) // Make it easy to calculate its own weekBase
    });

    result.date = date;
    // in this case, the date config ignores changes w/=== getTime so we have to force the update because we
    // also cleared weekBase above
    result.updateDate(result.date, result.date);
    return result;
  }
  changeYear(year) {
    const newDate = new Date(this.date);
    newDate.setFullYear(year);
    // changeDate rejects non-changes, otherwise a change event will be emitted
    this.date = newDate;
  }
  changeMonth(month) {
    const newDate = new Date(this.date);
    newDate.setMonth(month);
    // changeDate rejects non-changes, otherwise a change event will be emitted
    this.date = newDate;
  }
  get weekStartDay() {
    // This trick allows our weekStartDay to float w/the locale even if the locale changes
    return typeof this._weekStartDay === 'number' ? this._weekStartDay : DateHelper.weekStartDay;
  }
  updateWeekStartDay() {
    const me = this;
    me.updateDayNumbers();
    if (!me.isConfiguring && me.date) {
      me.weekBase = null; // force a calculateWeekBase
      me.updateDate(me.date, me.date);
    }
    // else date will be set soon and weekBase is null so calculateWeekBase will be called by updateDate
  }

  get nonWorkingDays() {
    return this._nonWorkingDays || DateHelper.nonWorkingDays;
  }
  changeNonWorkingDays(nonWorkingDays) {
    return ObjectHelper.assign({}, nonWorkingDays);
  }
  updateNonWorkingDays() {
    this.updateDayNumbers();
  }
  updateHideNonWorkingDays() {
    this.updateDayNumbers();
  }
  updateSixWeeks() {
    if (!this.isConfiguring) {
      this.updateDate(this.date, this.date);
    }
  }
  /**
   * The number of days in the calendar for this month. This will always be
   * a multiple of 7, because this represents the number of calendar cells
   * occupied by this month.
   * @property {Number}
   * @readonly
   */
  get dayCount() {
    // So for the example month, Dec 2018 has 42 days, from Mon 26th Nov (-4th Dec) 2018
    // to Sun 6th Jan (37th Dec) 2019
    return this.endDayOfMonth + 1 - this.startDayOfMonth;
  }
  /**
   * The number of weeks in the calendar for this month.
   * @property {Number}
   * @readonly
   */
  get weekCount() {
    return this.dayCount / 7;
  }
  /**
   * The date of the first cell in the calendar view of this month.
   * @property {Date}
   * @readonly
   */
  get startDate() {
    const me = this;
    if (me.year != null && me.month != null && me.startDayOfMonth != null) {
      return new Date(me.year, me.month, me.startDayOfMonth);
    }
  }
  /**
   * The date of the last cell in the calendar view of this month.
   * @property {Date}
   * @readonly
   */
  get endDate() {
    const me = this;
    if (me.year != null && me.month != null && me.startDayOfMonth != null) {
      return new Date(me.year, me.month, me.endDayOfMonth);
    }
  }
  /**
   * Iterates through all calendar cells in this month, calling the passed function for each date.
   * @param {Function} fn The function to call.
   * @param {Date} fn.date The date for the cell.
   */
  eachDay(fn) {
    for (let dayOfMonth = this.startDayOfMonth; dayOfMonth <= this.endDayOfMonth; dayOfMonth++) {
      fn(new Date(this.year, this.month, dayOfMonth));
    }
  }
  /**
   * Iterates through all weeks in this month, calling the passed function
   * for each week.
   * @param {Function} fn The function to call.
   * @param {Number[]} fn.week An array containing `[year, weekNumber]`
   * @param {Date[]} fn.dates The dates for the week.
   */
  eachWeek(fn) {
    const me = this,
      {
        weekCount
      } = me;
    for (let dayOfMonth = me.startDayOfMonth, week = 0; week < weekCount; week++) {
      const weekDates = [],
        weekOfYear = me.getWeekNumber(new Date(me.year, me.month, dayOfMonth));
      for (let day = 0; day < 7; day++, dayOfMonth++) {
        weekDates.push(new Date(me.year, me.month, dayOfMonth));
      }
      fn(weekOfYear, weekDates);
    }
  }
  /**
   * Returns the week of the year for the passed date. This returns an array containing *two* values,
   * the year **and** the week number are returned.
   *
   * The week number is calculated according to ISO rules, meaning that if the first week of the year
   * contains less than four days, it is considered to be the last week of the preceding year.
   *
   * The configured {@link #config-weekStartDay} is honoured in this calculation. So if the weekStartDay
   * is **NOT** the ISO standard of `1`, (Monday), then the weeks do not coincide with ISO weeks.
   * @param {Date} date The date to calculate the week for.
   * @returns {Number[]} A numeric array: `[year, week]`
   */
  getWeekNumber(date) {
    const me = this;
    date = DateHelper.clearTime(date);
    // If it's a date in another year, use our otherMonth to find the answer.
    if (date.getFullYear() !== me.year) {
      return me.getOtherMonth(new Date(date.getFullYear(), 0, 1)).getWeekNumber(date);
    }
    let weekNo = Math.floor(DateHelper.diff(me.weekBase, date, 'day') / 7),
      year = date.getFullYear();
    // No week 0. It's the last week of last year
    if (!weekNo) {
      // Week is the week of last year's 31st Dec
      return me.getOtherMonth(new Date(me.year - 1, 0, 1)).getWeekNumber(new Date(me.year, 0, 0));
    }
    // Only week 53 if year ends before our week's 5th day
    else if (weekNo === 53 && !me.has53weeks) {
      weekNo = 1;
      year++;
    }
    // 54 wraps round to 2 of next year
    else if (weekNo > 53) {
      weekNo = weekNo % 52;
    }
    // Return array of year and week number
    return [year, weekNo];
  }
  getWeekId(date) {
    const week = this.getWeekNumber(date);
    return week[0] * 100 + week[1];
  }
  getCellData(date, ownerMonth, dayTime = DayTime.MIDNIGHT) {
    const me = this,
      day = date.getDay(),
      visibleColumnIndex = me.visibleDayColumnIndex[day],
      isNonWorking = me.nonWorkingDays[day],
      isHiddenDay = me.hideNonWorkingDays && isNonWorking;
    // Automatically move to required month
    if (date < me.startDate || date > me.endDate) {
      me.month = date.getMonth();
    }
    return {
      day,
      dayTime,
      visibleColumnIndex,
      isNonWorking,
      week: me.getOtherMonth(date).getWeekNumber(date),
      key: DateHelper.format(date, 'YYYY-MM-DD'),
      columnIndex: me.dayColumnIndex[day],
      date: new Date(date),
      dayEnd: dayTime.duration('s'),
      tomorrow: dayTime.dayOfDate(DateHelper.add(date, 1, 'day')),
      // These two properties are only significant when used by a CalendarPanel which encapsulates
      // a single month.
      isOtherMonth: Math.sign(date.getMonth() + date.getFullYear() * 12 - (ownerMonth.month + ownerMonth.year * 12)),
      visible: !isHiddenDay && date >= ownerMonth.startDate && date < DateHelper.add(ownerMonth.endDate, 1, 'day'),
      isRowStart: visibleColumnIndex === 0,
      isRowEnd: visibleColumnIndex === me.visibleColumnCount - 1
    };
  }
  updateDayNumbers() {
    const me = this,
      {
        weekStartDay,
        nonWorkingDays,
        hideNonWorkingDays
      } = me,
      dayColumnIndex = me.dayColumnIndex = [],
      canonicalDayNumbers = me.canonicalDayNumbers = [],
      visibleDayColumnIndex = me.visibleDayColumnIndex = [];
    // So, if they set weekStartDay to 1 meaning Monday which is ISO standard, we will
    // have mapping of internal day number to canonical day number (as used by Date class)
    // and to abbreviated day name like this:
    // canonicalDayNumbers = [1, 2, 3, 4, 5, 6, 0] // Use for translation from our day number to Date class's day number
    //
    // Also, we need a map from canonical day number to *visible* cell index.
    // for example, if we have weekStartDay as Monday which is 1, and non working days as
    // Wednesday, and hideNonWorkingDays:true, then the calendar would look like
    // +----+----+----+----+----+----+
    // | Mo | Tu | Th | Fr | Sa | Su |
    // +----+----+----+----+----+----+
    //
    // So we'd need this array
    // [ 5, 0, 1, undefined, 2, 3, 4]
    // Or think of it as this map:
    // {
    //      1 : 0,
    //      2 : 1,
    //      4 : 2,
    //      5 : 3,
    //      6 : 4,
    //      0 : 5
    // }
    // To be able to ascertain the cell index directly from the canonical day number.
    //
    // We also need a logical column map which would be
    // +----+----+----+----+----+----+----+
    // | Mo | Tu | We | Th | Fr | Sa | Su |
    // +----+----+----+----+----+----+----+
    //
    // So we'd need this array
    // [ 6, 0, 1, 2, 3, 4, 5]
    // Or think of it as this map:
    // {
    //      1 : 0,
    //      2 : 1,
    //      3 : 2
    //      4 : 3,
    //      5 : 4,
    //      6 : 5,
    //      0 : 6
    // }
    // We use this to cache the number of visible columns so that cell renderers can tell whether
    // they are on the last visible column.
    let visibleColumnIndex = 0;
    for (let columnIndex = 0; columnIndex < 7; columnIndex++) {
      const canonicalDay = (weekStartDay + columnIndex) % 7;
      canonicalDayNumbers[columnIndex] = canonicalDay;
      dayColumnIndex[canonicalDay] = columnIndex;
      // If this day is going to have visible representation, we need to
      // map it to a columnIndex;
      if (!hideNonWorkingDays || !nonWorkingDays[canonicalDay]) {
        visibleDayColumnIndex[canonicalDay] = visibleColumnIndex++;
      }
    }
    me.visibleColumnCount = visibleColumnIndex;
    me.weekLength = hideNonWorkingDays ? 7 - ObjectHelper.keys(nonWorkingDays).length : 7;
  }
}
// Instance needed for internal tasks
const otherMonth = new Month(null);
Month._$name = 'Month';

/**
 * @module Core/widget/CalendarPanel
 */
/**
 * A Panel which displays a month of date cells.
 *
 * This is a base class for UI widgets like {@link Core.widget.DatePicker} which need to display a calendar layout
 * and should not be used directly.
 * @extends Core/widget/Panel
 */
class CalendarPanel extends Panel {
  static get $name() {
    return 'CalendarPanel';
  }
  // Factoryable type name
  static get type() {
    return 'calendarpanel';
  }
  static get configurable() {
    return {
      textContent: false,
      /**
       * Gets or sets the date that orientates the panel to display a particular month.
       * Changing this causes the content to be refreshed.
       * @member {Date} date
       */
      /**
       * The date which this CalendarPanel encapsulates.
       * @config {Date|String}
       */
      date: {
        $config: {
          equal: 'date'
        },
        value: null
      },
      /**
       * A {@link Core.util.Month} Month utility object which encapsulates this Panel's month
       * and provides contextual information and navigation services.
       * @config {Core.util.Month|MonthConfig}
       */
      month: {},
      year: null,
      /**
       * The week start day, 0 meaning Sunday, 6 meaning Saturday.
       * Defaults to {@link Core.helper.DateHelper#property-weekStartDay-static}.
       * @config {Number}
       */
      weekStartDay: null,
      /**
       * Configure as `true` to always show a six week calendar.
       * @config {Boolean}
       * @default
       */
      sixWeeks: true,
      /**
       * Configure as `true` to show a week number column at the start of the calendar block.
       * @deprecated Since 4.0.0. Use {@link #config-showWeekColumn} instead.
       * @config {Boolean}
       */
      showWeekNumber: null,
      /**
       * Configure as `true` to show a week number column at the start of the calendar block.
       * @config {Boolean}
       */
      showWeekColumn: null,
      /**
       * Either an array of `Date` objects which are to be disabled, or
       * a function (or the name of a function), which, when passed a `Date` returns `true` if the
       * date is disabled.
       * @config {Function|Date[]|String}
       */
      disabledDates: null,
      /**
       * A function (or the name of a function) which creates content in, and may mutate a day header element.
       * The following parameters are passed:
       *  - cell [HTMLElement](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement) The header element.
       *  - day [Number](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The day number conforming to the specified {@link #config-weekStartDay}. Will be in the range 0 to 6.
       *  - weekDay [Number](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The canonical day number where Monday is 0 and Sunday is.
       * @config {Function|String}
       */
      headerRenderer: null,
      /**
       * A function (or the name of a function) which creates content in, and may mutate the week cell element at the start of a week row.
       * The following parameters are passed:
       *  - cell [HTMLElement](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement) The header element.
       *  - week [Number[]](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) An array containing `[year, weekNumber]`.
       * @config {Function|String}
       */
      weekRenderer: null,
      /**
       * A function (or the name of a function) which creates content in, and may mutate a day cell element.
       * The following parameters are passed:
       *  - cell [HTMLElement](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement) The header element.
       *  - date [Date](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date) The date for the cell.
       *  - day [Number](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The day for the cell (0 to 6 for Sunday to Saturday).
       *  - rowIndex [Number[]](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The row index, 0 to month row count (6 if {@link #config-sixWeeks} is `true`).
       *  _ row [HTMLElement](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement) The row element encapsulating the week which the cell is a part of.
       *  - cellIndex [Number[]](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The cell index in the whole panel. May be from 0 to up to 42.
       *  - columnIndex [Number[]](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The column index, 0 to 6.
       *  - visibleColumnIndex [Number[]](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number) The visible column index taking hidden non working days into account.
       * @config {Function|String}
       */
      cellRenderer: null,
      /**
       * Configure as `true` to render weekends as {@link #config-disabledDates}.
       * @config {Boolean}
       */
      disableWeekends: null,
      hideNonWorkingDays: null,
      hideNonWorkingDaysCls: 'b-hide-nonworking-days',
      /**
       * Non-working days as an object where keys are day indices, 0-6 (Sunday-Saturday), and the value is `true`.
       * Defaults to {@link Core.helper.DateHelper#property-nonWorkingDays-static}.
       * @config {Object<Number,Boolean>}
       */
      nonWorkingDays: null,
      /**
       * A config object to create a tooltip which will show on hover of a date cell including disabled, weekend,
       * and "other month" cells.
       *
       * It is the developer's responsibility to hook the `beforeshow` event to either veto the show by returning
       * `false` or provide contextual content for the date.
       *
       * The tip instance will be primed with a `date` property.
       * @config {TooltipConfig}
       */
      tip: null,
      dayCellCls: 'b-calendar-cell',
      dayHeaderCls: 'b-calendar-day-header',
      /**
       * The class name to add to disabled calendar cells.
       * @config {String}
       * @private
       */
      disabledCls: 'b-disabled-date',
      /**
       * The class name to add to calendar cells which are in the previous or next month.
       * @config {String}
       * @private
       */
      otherMonthCls: 'b-other-month',
      /**
       * The class name to add to calendar cells which are weekend dates.
       * @config {String}
       * @private
       */
      weekendCls: 'b-weekend',
      /**
       * The class name to add to the calendar cell which contains today's date.
       * @config {String}
       * @private
       */
      todayCls: 'b-today',
      /**
       * The class name to add to calendar cells which are {@link #config-nonWorkingDays}.
       * @config {String}
       * @private
       */
      nonWorkingDayCls: 'b-nonworking-day',
      /**
       * The {@link Core.helper.DateHelper} format string to format the day names
       * in the header row above the calendar cells.
       * @config {String}
       * @default
       */
      dayNameFormat: 'ddd',
      /**
       * By default, week rows flex to share available Panel height equally.
       *
       * Set this config if the available height is too small, and the cell height needs
       * to be larger to show events.
       *
       * Setting this config causes the month grid to become scrollable in the `Y` axis.
       *
       * May be specified as a number in which case it will be taken to mean pixels,
       * or a length in standard CSS units.
       * @config {Number|String}
       */
      minRowHeight: {
        $config: ['lazy'],
        value: null
      },
      /**
       * By default, day cells flex to share available Panel width equally.
       *
       * Set this config if the available width is too small, and the cell width needs
       * to be larger to show events.
       *
       * Setting this config causes the month grid to become scrollable in the `X` axis.
       * @config {Number}
       */
      minColumnWidth: {
        $config: ['lazy'],
        value: null
      },
      /**
       * Configure this as true to disable pointer interaction with cells which are outside the
       * range of the current month.
       * @config {Boolean}
       */
      disableOtherMonthCells: null,
      disableOtherMonthCellsCls: 'b-disable-othermonth-cells',
      /**
       * Configure this as `true` to hide cells which are outside the range of the current month.
       * @config {Boolean}
       */
      hideOtherMonthCells: null,
      hideOtherMonthCellsCls: 'b-hide-othermonth-cells',
      /**
       * By default, when navigating through time, the next time
       * block will be animated in from the appropriate direction.
       *
       * Configure this as `false` to disable this.
       * @prp {Boolean} animateTimeShift
       * @default
       */
      animateTimeShift: true
    };
  }
  construct(config) {
    super.construct(config);
    if (!this.refreshCount) {
      this.refresh();
    }
  }
  onPaint({
    firstPaint
  }) {
    var _super$onPaint;
    (_super$onPaint = super.onPaint) === null || _super$onPaint === void 0 ? void 0 : _super$onPaint.call(this, ...arguments);
    // Invoke the lazy configs when we first hit the visible DOM
    if (firstPaint) {
      // The cell structure must exist for the configs to apply to.
      if (!this.refreshCount) {
        this.refresh();
      }
      this.getConfig('minColumnWidth');
      this.getConfig('minRowHeight');
    }
  }
  get overflowElement() {
    return this.weeksElement;
  }
  doDestroy() {
    var _this$tip;
    (_this$tip = this.tip) === null || _this$tip === void 0 ? void 0 : _this$tip.destroy();
    super.doDestroy();
  }
  changeMinRowHeight(minRowHeight) {
    // Fall back to 75 on platforms that do not support CSS vars
    const minValue = parseInt(DomHelper.getStyleValue(this.element, '--min-row-height'), 10) || 75;
    return isNaN(minRowHeight) ? minRowHeight : Math.max(parseInt(minRowHeight) || 0, minValue);
  }
  updateMinRowHeight(minRowHeight) {
    this.weekElements.forEach(w => DomHelper.setLength(w, 'minHeight', minRowHeight));
    this.scrollable = {
      overflowY: minRowHeight ? 'auto' : false
    };
  }
  changeMinColumnWidth(minColumnWidth) {
    // Fall back to 75 on platforms that do not support CSS vars
    const minValue = parseInt(DomHelper.getStyleValue(this.element, '--min-column-width'), 10) || 75;
    return minColumnWidth == null ? minColumnWidth : Math.max(parseInt(minColumnWidth) || 0, minValue);
  }
  updateMinColumnWidth(minColumnWidth) {
    const me = this;
    me.weekdayCells.forEach(c => DomHelper.setLength(c, 'minWidth', minColumnWidth));
    me.cellElements.forEach(c => c.matches(`.${me.dayCellCls}`) && DomHelper.setLength(c, 'minWidth', minColumnWidth));
    me.scrollable = {
      overflowX: minColumnWidth ? 'auto' : false
    };
    me.overflowElement.classList[minColumnWidth ? 'add' : 'remove']('b-min-columnwidth');
  }
  getDateFromDomEvent(domEvent) {
    const element = (domEvent.nodeType === Element.ELEMENT_NODE ? domEvent : domEvent.target).closest(`#${this.id} [data-date]`);
    if (element) {
      return DateHelper.parseKey(element.dataset.date);
    }
  }
  changeTip(tip, existingTip) {
    const me = this;
    return Tooltip.reconfigure(existingTip, tip, {
      owner: me,
      defaults: {
        type: 'tooltip',
        owner: me,
        id: `${me.id}-cell-tip`,
        forElement: me.bodyElement,
        forSelector: `.${me.dayCellCls}`
      }
    });
  }
  updateTip(tip) {
    this.detachListeners('tip');
    tip === null || tip === void 0 ? void 0 : tip.ion({
      pointerOver: 'onTipOverCell',
      name: 'tip',
      thisObj: this
    });
  }
  updateElement(element, was) {
    const me = this;
    super.updateElement(element, was);
    me.updateHideNonWorkingDays(me.hideNonWorkingDays);
    me.weekdayCells = Array.from(element.querySelectorAll('.b-calendar-day-header'));
    me.weekElements = Array.from(element.querySelectorAll('.b-calendar-week'));
    me.weekDayElements = Array.from(element.querySelectorAll('.b-calendar-days'));
    me.cellElements = [];
    for (let i = 0, {
        length
      } = me.weekDayElements; i < length; i++) {
      me.cellElements.push(me.weekDayElements[i].previousSibling, ...me.weekDayElements[i].children);
    }
  }
  changeDate(date) {
    date = typeof date === 'string' ? DateHelper.parse(date) : new Date(date);
    if (isNaN(date)) {
      throw new Error('CalendarPanel date ingestion must be passed a Date, or a YYYY-MM-DD date string');
    }
    return DateHelper.clearTime(date);
  }
  /**
   * The date which this CalendarPanel encapsulates. Setting this causes the
   * content to be refreshed.
   * @property {Date}
   */
  updateDate(value) {
    // We respond to Month change events to update the UI
    this.month.date = value;
  }
  updateDayNameFormat() {
    // 4th June 2000 was a Sunday
    const d = new Date('2000-06-04T12:00:00');
    this.shortDayNames = [];
    // Collect local shortDayNames in default order.
    for (let date = 4; date < 11; date++) {
      d.setDate(date);
      this.shortDayNames.push(DateHelper.format(d, this.dayNameFormat));
    }
  }
  get weekStartDay() {
    // This trick allows our weekStartDay to float w/the locale even if the locale changes
    return typeof this._weekStartDay === 'number' ? this._weekStartDay : DateHelper.weekStartDay;
  }
  /**
   * Set to 0 for Sunday (the default), 1 for Monday etc.
   *
   * Set to `null` to use the default value from {@link Core/helper/DateHelper}.
   */
  updateWeekStartDay(weekStartDay) {
    const me = this;
    if (me._month) {
      me.month.weekStartDay = weekStartDay;
      me.dayNames = [];
      // So, if they set weekStartDay to 1 meaning Monday which is ISO standard, we will
      // dayNames = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
      for (let i = 0; i < 7; i++) {
        me.dayNames[i] = me.shortDayNames[me.canonicalDayNumbers[i]];
      }
      if (me.refreshCount) {
        me.refresh();
      }
    }
  }
  updateHideNonWorkingDays(hideNonWorkingDays) {
    var _this$scrollable;
    // Undefined must be cast to Boolean, otherwise it will toggle the class *on*.
    this.contentElement.classList.toggle(this.hideNonWorkingDaysCls, Boolean(hideNonWorkingDays));
    (_this$scrollable = this.scrollable) === null || _this$scrollable === void 0 ? void 0 : _this$scrollable.syncOverflowState();
    if (this._month) {
      this.month.hideNonWorkingDays = hideNonWorkingDays;
    }
    // First/last visible cell might change
    if (!this.isConfiguring) {
      this.refresh();
    }
  }
  updateHideOtherMonthCells(hideOtherMonthCells) {
    var _this$scrollable2;
    // Undefined must be cast to Boolean, otherwise it will toggle the class *on*.
    this.element.classList.toggle(this.hideOtherMonthCellsCls, Boolean(hideOtherMonthCells));
    (_this$scrollable2 = this.scrollable) === null || _this$scrollable2 === void 0 ? void 0 : _this$scrollable2.syncOverflowState();
  }
  updateDisableOtherMonthCells(disableOtherMonthCells) {
    var _this$scrollable3;
    // Undefined must be cast to Boolean, otherwise it will toggle the class *on*.
    this.element.classList.toggle(this.disableOtherMonthCellsCls, Boolean(disableOtherMonthCells));
    (_this$scrollable3 = this.scrollable) === null || _this$scrollable3 === void 0 ? void 0 : _this$scrollable3.syncOverflowState();
  }
  get nonWorkingDays() {
    // If we were not configured with non working days, ask the locale for them. Once.
    // The cached value is cleared on locale change.
    return this._nonWorkingDays || this._localeNonWorkingDays || (this._localeNonWorkingDays = DateHelper.nonWorkingDays);
  }
  get weekends() {
    // Ask the DateHelper which days are weekend days only once.
    // The cached value is cleared on locale change.
    return this._localeWeekends || (this._localeWeekends = DateHelper.weekends);
  }
  changeNonWorkingDays(nonWorkingDays) {
    return ObjectHelper.assign({}, nonWorkingDays);
  }
  updateNonWorkingDays(nonWorkingDays) {
    if (this._month) {
      var _this$scrollable4;
      this.month.nonWorkingDays = nonWorkingDays;
      this.refresh();
      (_this$scrollable4 = this.scrollable) === null || _this$scrollable4 === void 0 ? void 0 : _this$scrollable4.syncOverflowState();
    }
  }
  get visibleDayColumnIndex() {
    return this.month.visibleDayColumnIndex;
  }
  get dayColumnIndex() {
    return this.month.dayColumnIndex;
  }
  get canonicalDayNumbers() {
    return this.month.canonicalDayNumbers;
  }
  get visibleColumnCount() {
    return this.month.visibleColumnCount;
  }
  get weekLength() {
    return this.month.weekLength;
  }
  /**
   * The date of the first day cell in this panel.
   * Note that this may *not* be the first of this panel's current month.
   * @property {Date}
   * @readonly
   */
  get startDate() {
    return this.month.startDate;
  }
  get duration() {
    // The endDate is "exclusive" because it means 00:00:00 of that day.
    return DateHelper.diff(this.month.startDate, this.month.endDate, 'day') + 1;
  }
  /**
   * The end date of this view. Note that in terms of full days, this is exclusive,
   * ie: 2020-01-012 to 2020-01-08 is *seven* days. The end is 00:00:00 on the 8th.
   *
   * Note that this may *not* be the last date of this panel's current month.
   * @property {Date}
   * @readonly
   */
  get endDate() {
    const {
      endDate
    } = this.month;
    if (endDate) {
      return DateHelper.add(endDate, 1, 'day');
    }
  }
  changeMonth(month, currentMonth) {
    const me = this;
    if (!(month instanceof Month)) {
      // Setting month to a number when we already have a Month means
      // just updating the month number of our Month
      if (typeof month === 'number') {
        if (currentMonth) {
          currentMonth.month = month;
          return;
        }
        const date = me.date || DateHelper.clearTime(new Date());
        date.setMonth(month);
        month = {
          date
        };
      }
      month = Month.new({
        weekStartDay: me.weekStartDay,
        nonWorkingDays: me.nonWorkingDays,
        hideNonWorkingDays: me.hideNonWorkingDays,
        sixWeeks: me.sixWeeks
      }, month);
    }
    month.ion({
      dateChange: 'onMonthDateChange',
      thisObj: me
    });
    return month;
  }
  onMonthDateChange({
    source: month,
    newDate,
    oldDate,
    changes
  }) {
    const me = this;
    // Ensure we're always in sync with the data our Month holds
    me.year = month.year;
    if (!me.isConfiguring) {
      // Only refresh if we don't contain a cell for the new date
      // or if, internally, the Month we are mapping to the UI is different.
      if (!me.getCell(newDate) || changes.m || changes.y) {
        me.refresh();
        if (me.animateTimeShift && me.isVisible) {
          DomHelper.slideIn(me.contentElement, newDate > oldDate ? 1 : -1);
        }
      }
      /**
       * Fires when the date of this CalendarPanel is set.
       * @event dateChange
       * @param {Date} value The new date.
       * @param {Date} oldValue The old date.
       * @param {Object} changes An object which contains properties which indicate what part of the date changed.
       * @param {Boolean} changes.d True if the date changed in any way.
       * @param {Boolean} changes.w True if the week changed (including same week in a different year).
       * @param {Boolean} changes.m True if the month changed (including same month in a different year).
       * @param {Boolean} changes.y True if the year changed.
       */
      me.trigger('dateChange', {
        changes,
        value: newDate,
        oldValue: oldDate
      });
    }
  }
  updateYear(year) {
    this.month.year = year;
  }
  updateShowWeekNumber(showWeekNumber) {
    this.updateShowWeekColumn(showWeekNumber);
  }
  updateShowWeekColumn(showWeekColumn) {
    const me = this;
    me.element.classList[showWeekColumn ? 'add' : 'remove']('b-show-week-column');
    if (me.floating) {
      // Must realign because content change might change dimensions
      if (!me.isAligning) {
        me.realign();
      }
    }
  }
  updateSixWeeks(sixWeeks) {
    if (this.month) {
      this.month.sixWeeks = sixWeeks;
      this.refresh();
    }
  }
  /**
   * Refreshes the UI after changing a config that would affect the UI.
   */
  refresh() {
    // This method may be overridden by subclasses to add things like refresh scheduling.
    this.doRefresh();
  }
  /**
   * Implementation of the UI refresh.
   * @private
   */
  doRefresh() {
    var _me$project;
    // Ensure sub elements are all present
    this.getConfig('element');
    const me = this,
      timeZone = me.timeZone != null ? me.timeZone : (_me$project = me.project) === null || _me$project === void 0 ? void 0 : _me$project.timeZone,
      today = timeZone != null ? TimeZoneHelper.toTimeZone(new Date(), timeZone) : new Date(),
      {
        weekElements,
        weekDayElements,
        date,
        month,
        dayCellCls,
        dayHeaderCls,
        disabledCls,
        otherMonthCls,
        weekendCls,
        todayCls,
        nonWorkingDayCls,
        nonWorkingDays,
        canonicalDayNumbers,
        sixWeeks
      } = me;
    today.setHours(0, 0, 0, 0);
    // If we have not been initialized with a current date, use today
    if (!date) {
      me.date = today;
      return;
    }
    /**
     * Fires before this CalendarPanel refreshes in response to changes in its month.
     * @event beforeRefresh
     * @param {Core.widget.DatePicker} source This DatePicker.
     */
    me.trigger('beforeRefresh');
    // Make sure we've calculated our shortDayNames
    me.getConfig('dayNameFormat');
    for (let columnIndex = 0; columnIndex < 7; columnIndex++) {
      const cell = me.weekdayCells[columnIndex],
        cellDay = me.canonicalDayNumbers[columnIndex],
        cellClassList = {
          [dayHeaderCls]: 1,
          [weekendCls]: DateHelper.weekends[cellDay],
          [nonWorkingDayCls]: nonWorkingDays[cellDay]
        };
      if (me.headerRenderer) {
        cell.innerHTML = '';
        me.callback(me.headerRenderer, me, [cell, columnIndex, cellDay]);
      } else {
        DomHelper.setInnerText(cell, me.shortDayNames[cellDay]);
      }
      // Sync day name cell classes with its calculated status
      DomHelper.syncClassList(cell, cellClassList);
      cell.dataset.columnIndex = columnIndex;
      cell.dataset.cellDay = cellDay;
    }
    // Create cell content
    let rowIndex = 0,
      cellIndex = 0,
      lastWorkingColumn = 6;
    // Which column is the last working day so it can be tagged with an identifying class
    for (let columnIndex = 6; columnIndex >= 0; columnIndex--) {
      if (!nonWorkingDays[canonicalDayNumbers[columnIndex]]) {
        lastWorkingColumn = columnIndex;
        break;
      }
    }
    // Hide or show the "other month" week row depending on our sixWeeks setting
    weekElements[4].classList.toggle('b-hide-display', month.weekCount < 5 && !sixWeeks);
    weekElements[5].classList.toggle('b-hide-display', month.weekCount < 6 && !sixWeeks);
    month.eachWeek((week, dates) => {
      const weekDayElement = weekDayElements[rowIndex],
        weekCells = [weekDayElement.previousSibling, ...weekDayElement.children];
      // Stamp week into week row's dataset
      weekElements[rowIndex].dataset.week = `${week[0]},${week[1]}`;
      if (me.weekRenderer) {
        me.callback(me.weekRenderer, me, [weekCells[0], week]);
      } else {
        weekCells[0].innerText = week[1];
      }
      for (let columnIndex = 0; columnIndex < 7; columnIndex++) {
        const date = dates[columnIndex],
          day = date.getDay(),
          key = DateHelper.makeKey(date),
          isNonWorking = nonWorkingDays[day],
          cell = weekCells[columnIndex + 1],
          cellClassList = {
            [dayCellCls]: 1,
            [disabledCls]: me.isDisabledDate(date),
            [otherMonthCls]: date.getMonth() !== month.month,
            [weekendCls]: DateHelper.weekends[day],
            [todayCls]: date.getTime() === today.getTime(),
            [nonWorkingDayCls]: isNonWorking,
            'b-last-working-day': columnIndex === lastWorkingColumn,
            'b-first-visible-cell': !(date - (me.firstVisibleDate || -1)),
            'b-last-visible-cell': !(date - (me.lastVisibleDate || -1)),
            [`b-day-of-week-${day}`]: 1
          };
        // Sync day cell classes with its calculated status
        DomHelper.syncClassList(cell, cellClassList);
        cell.dataset.date = key;
        cell.dataset.cellIndex = cellIndex;
        cell.dataset.columnIndex = columnIndex;
        // Since we manipulate the classList/Name directly, we need to trick DomSync's config comparison logic or it
        // may think the class has not changed.
        if (cell.lastDomConfig) {
          delete cell.lastDomConfig.class;
          delete cell.lastDomConfig.className;
        }
        if (me.cellRenderer) {
          me.callback(me.cellRenderer, me, [{
            cell,
            date,
            day,
            row: weekElements[rowIndex],
            rowIndex,
            cellIndex,
            columnIndex,
            visibleColumnIndex: me.visibleDayColumnIndex[day],
            week,
            key
          }]);
        } else {
          cell.innerHTML = date.getDate();
        }
        cellIndex++;
      }
      rowIndex++;
    });
    /**
     * The number of rows displayed in this month. If {@link #config-sixWeeks} is not set,
     * this may be from 4 to 6.
     * @member {Number} visibleWeekCount
     * @readonly
     */
    me.visibleWeekCount = rowIndex;
    if (me.floating) {
      // Must realign because content change might change dimensions
      if (!me.isAligning) {
        me.realign();
      }
    }
    me.refreshCount = (me.refreshCount || 0) + 1;
    /**
     * Fires when this CalendarPanel refreshes.
     * @event refresh
     */
    me.trigger('refresh');
  }
  isDisabledDate(date) {
    const day = date.getDay(),
      {
        disabledDates,
        nonWorkingDays
      } = this;
    if (this.disableWeekends && nonWorkingDays[day]) {
      return true;
    }
    if (disabledDates) {
      if (Array.isArray(disabledDates)) {
        date = DateHelper.clearTime(date, true);
        return disabledDates.some(d => !(DateHelper.clearTime(d, true) - date));
      } else {
        return this.callback(this.disabledDates, this, [date]);
      }
    }
  }
  get bodyConfig() {
    const result = super.bodyConfig,
      weeksContainerChildren = [];
    result.children = [{
      tag: 'div',
      className: 'b-calendar-row b-calendar-weekdays',
      reference: 'weekdaysHeader',
      children: [{
        class: 'b-week-number-cell'
      }, ...ArrayHelper.fill(7, {
        class: this.dayHeaderCls
      }), DomHelper.scrollBarPadElement]
    }, {
      // `notranslate` prevents google translate messing up the DOM, https://github.com/facebook/react/issues/11538
      className: 'b-weeks-container notranslate',
      reference: 'weeksElement',
      children: weeksContainerChildren
    }];
    for (let i = 0; i < 6; i++) {
      const weekRow = {
        className: 'b-calendar-row b-calendar-week',
        dataset: {
          rowIndex: i
        },
        children: [{
          className: 'b-week-number-cell'
        }, {
          className: 'b-calendar-days',
          children: [{}, {}, {}, {}, {}, {}, {}],
          syncOptions: {
            ignoreRefs: true,
            strict: false // allow complete replacement of classes w/o matching lastDomConfig
          }
        }]
      };

      weeksContainerChildren.push(weekRow);
    }
    return result;
  }
  get firstVisibleDate() {
    if (this.hideOtherMonthCells) {
      const {
        year,
        month
      } = this.month;
      return new Date(year, month, 1);
    }
    for (const me = this, date = me.month.startDate;; date.setDate(date.getDate() + 1)) {
      if (!me.hideNonWorkingDays || !me.nonWorkingDays[date.getDay()]) {
        return date;
      }
    }
  }
  get lastVisibleDate() {
    const lastDate = DateHelper.add(this.endDate, -1, 'd');
    if (this.hideOtherMonthCells) {
      return lastDate;
    }
    for (const me = this, date = lastDate;; date.setDate(date.getDate() - 1)) {
      if (!me.hideNonWorkingDays || !me.nonWorkingDays[date.getDay()]) {
        return date;
      }
    }
  }
  /**
   * Returns the cell associated with the passed date.
   *
   * To exclude dates which are outside of the panel's current month, pass the `strict` parameter as `true`
   * @param {Date|String} date The date to find the element for or a key in the format `YYYY-MM-DD`
   * @param {Boolean} strict Only return the element if this view *owns* the date.
   * @returns {HTMLElement} The cell for the passed date if it exists
   */
  getCell(date, strict) {
    if (!(typeof date === 'string')) {
      date = DateHelper.makeKey(date);
    }
    const cell = this.weeksElement.querySelector(`[data-date="${date}"]`);
    if (cell && (!strict || !cell.classList.contains(this.otherMonthCls))) {
      return cell;
    }
  }
  onTipOverCell({
    source: tip,
    target
  }) {
    tip.date = DateHelper.parseKey(target.dataset.date);
  }
  updateLocalization() {
    // Uncache the cached locale data
    this._localeNonWorkingDays = this._localeWeekends = null;
    this.updateDayNameFormat();
    this.updateWeekStartDay(this.weekStartDay);
    super.updateLocalization();
  }
}
// Register this widget type with its Factory
CalendarPanel.initClass();
CalendarPanel._$name = 'CalendarPanel';

/**
 * @module Core/widget/Checkbox
 */
const whenNotChecked = field => !field.value;
/**
 * Checkbox field, wraps <code>&lt;input type="checkbox"&gt;</code>.
 * Color can be specified and you can optionally configure {@link #config-text}
 * to display in a label to the right of the checkbox in addition to a standard
 * field {@link #config-label}.
 *
 * {@inlineexample Core/widget/Checkbox.js vertical}
 *
 * This field can be used as an {@link Grid.column.Column#config-editor} for the {@link Grid.column.Column}.
 *
 * ## Nested Items
 * A checkbox can also have a {@link #config-container} of additional {@link Core.widget.Container#config-items}. These
 * items can be displayed immediately following the field's label (which is the default when there is only one item) or
 * below the checkbox. This can be controlled using the {@link #config-inline} config.
 *
 * In the demo below notice how additional fields are displayed when the checkboxes are checked:
 *
 * {@inlineexample Core/widget/Checkbox-items.js vertical}
 *
 * @extends Core/widget/Field
 * @classType checkbox
 * @inputfield
 */
class Checkbox extends Field {
  //region Config
  static get $name() {
    return 'Checkbox';
  }
  // Factoryable type name
  static get type() {
    return 'checkbox';
  }
  // Factoryable type alias
  static get alias() {
    return 'check';
  }
  static get configurable() {
    return {
      inputType: 'checkbox',
      /**
       * Specify `true` to automatically {@link Core.widget.FieldContainer#config-collapsed collapse} the field's
       * {@link #config-container} when the field is not {@link #property-checked}.
       *
       * Alternatively, this can be a function that returns the desired `collapse` state when passed the field
       * instance as its one parameter.
       *
       * @config {Boolean|Function}
       * @default false
       */
      autoCollapse: null,
      containerDefaults: {
        syncableConfigs: {
          disabled: field => field.disabled || !field.value
        },
        syncConfigTriggers: {
          autoCollapse: 1,
          value: 1
        }
      },
      /**
       * Get/set label
       * @member {String} name
       */
      /**
       * Text to display on checkbox label
       * @config {String}
       */
      text: '',
      /**
       * The value to provide for this widget in {@link Core.widget.Container#property-values} when it is
       * {@link #property-checked}.
       * A value of `undefined` will cause this widget not to include its value when checked.
       * @config {*}
       * @default
       */
      checkedValue: true,
      /**
       * The value to provide for this widget in {@link Core.widget.Container#property-values} when it is not
       * {@link #property-checked}.
       *
       * A value of `undefined` will cause this widget to not include its value when it is unchecked.
       * @config {*}
       * @default
       */
      uncheckedValue: false,
      /**
       * The checked state. The same as `value`.
       * @config {Boolean} checked
       */
      /**
       * Checkbox color, must have match in CSS
       * @config {String}
       */
      color: null,
      /**
       * Get/set value
       * @member {String} value
       */
      /**
       * Sets input fields value attribute
       * @config {String}
       */
      value: '',
      toggleGroup: null,
      localizableProperties: ['label', 'text']
    };
  }
  //endregion
  //region Init
  construct(config) {
    // Convert checked to value so that initializing getter can read it if requested prior to trying to set it.
    if ('checked' in config) {
      config = ObjectHelper.assign({}, config); // copy inherited properties unlike Object.assign()
      config.value = config.checked;
      delete config.checked;
    }
    super.construct(config);
    this.syncHasText();
  }
  get textLabelCls() {
    return 'b-checkbox-label';
  }
  // Implementation needed at this level because it has two inner elements in its inputWrap
  get innerElements() {
    return [this.inputElement, {
      tag: 'label',
      class: this.textLabelCls,
      for: `${this.id}-input`,
      reference: 'textLabel',
      html: this.text || ''
    }];
  }
  get inputElement() {
    const config = super.inputElement;
    if (this.toggleGroup) {
      config.dataset = {
        group: this.toggleGroup
      };
    }
    config.listeners = {
      click: 'internalOnClick',
      change: 'internalOnChange',
      input: 'internalOnInput'
    };
    return config;
  }
  //endregion
  //region Toggle
  /**
   * Get/set checked state. Equivalent to `value` config.
   * @property {Boolean}
   */
  get checked() {
    return this.value;
  }
  set checked(value) {
    this.value = value;
  }
  syncHasText() {
    this.element.classList[this.text ? 'add' : 'remove']('b-text');
  }
  updateText(value) {
    if (this.textLabel) {
      this.syncHasText();
      this.textLabel.innerHTML = value;
    }
  }
  afterSyncChildConfigs(container) {
    super.afterSyncChildConfigs(container);
    let {
      autoCollapse
    } = this;
    if (autoCollapse) {
      autoCollapse = autoCollapse === true ? whenNotChecked : autoCollapse;
      container.collapsed = autoCollapse(this);
    }
  }
  assignFieldValue(values, key, value) {
    this.value = value === this.checkedValue || (value === this.uncheckedValue ? false : null);
  }
  fetchInputValue() {
    if (!this.readOnly) {
      this.value = this.input.checked;
    }
  }
  gatherValue(values) {
    var _values$valueName;
    const me = this,
      value = me.value ? me.checkedValue : me.uncheckedValue,
      storedValue = value !== undefined,
      {
        valueName
      } = me;
    if (storedValue) {
      values[valueName] = value;
    }
    me.gatherValues(values, storedValue);
    if (value === true && ((_values$valueName = values[valueName]) === null || _values$valueName === void 0 ? void 0 : _values$valueName.value) === value) {
      delete values[valueName].value;
    }
  }
  changeValue(value) {
    return value === 'false' ? false : Boolean(value);
  }
  updateValue(value) {
    var _me$container;
    const me = this,
      changed = me.input.checked !== value;
    me.input.checked = value;
    (_me$container = me.container) === null || _me$container === void 0 ? void 0 : _me$container.syncChildConfigs();
    if (changed && !me.inputting && !me.isConfiguring) {
      me.uncheckToggleGroupMembers();
      // The change event does not fire on programmatic change of input.
      me.triggerChange(false);
    }
  }
  get inputValueAttr() {
    return 'checked';
  }
  updateColor(value, was) {
    const classes = this.element.classList;
    if (was) {
      classes.remove(was);
    }
    if (value) {
      classes.add(value);
    }
  }
  getToggleGroupMembers() {
    const me = this,
      {
        checked,
        toggleGroup,
        input: checkedElement,
        type
      } = me,
      result = [];
    if (checked && toggleGroup) {
      DomHelper.forEachSelector(me.rootElement, `input[type=${type}][data-group=${toggleGroup}]`, inputEl => {
        if (inputEl !== checkedElement) {
          const partnerCheckbox = Widget.fromElement(inputEl);
          partnerCheckbox && result.push(partnerCheckbox);
        }
      });
    }
    return result;
  }
  uncheckToggleGroupMembers() {
    if (this.checked && this.toggleGroup) {
      this.getToggleGroupMembers().forEach(widget => widget.checked = false);
    }
  }
  /**
   * Check the box
   */
  check() {
    this.checked = true;
  }
  /**
   * Uncheck the box
   */
  uncheck() {
    this.checked = false;
  }
  /**
   * Toggle checked state. If you want to force a certain state, assign to {@link #property-checked} instead.
   */
  toggle() {
    this.checked = !this.checked;
  }
  //endregion
  //region Events
  internalOnClick(event) {
    // Native checkboxes has no readonly attribute, we prevent toggling it here instead
    if (this.readOnly) {
      event.preventDefault();
      return false;
    }
    /**
     * Fires when the checkbox is clicked
     * @event click
     * @param {Core.widget.Checkbox} source The checkbox
     * @param {Event} event DOM event
     */
    return this.trigger('click', {
      event
    });
  }
  /**
   * Triggers events when user toggles the checkbox
   * @fires beforeChange
   * @fires change
   * @fires action
   * @private
   */
  internalOnChange(event) {
    const me = this;
    // Chrome somehow sets checked state when re-enabling, have to reset that
    if (me.readOnly && me.value !== me.input.checked) {
      me.input.checked = me.value;
      return;
    }
    me.value = me.input.checked;
    if (!me.inputting) {
      me.inputting = true;
      me.triggerChange(true);
      me.inputting = false;
    }
  }
  // Need to catch changes even if readOnly, because of chrome behaviour when re-enabling
  updateInputReadOnly(readOnly) {}
  /**
   * Triggers events when checked state is changed
   * @fires beforeChange
   * @fires change
   * @fires action
   * @private
   */
  triggerChange(userAction) {
    const me = this,
      {
        checked
      } = me.input;
    /**
     * Fired before checkbox is toggled. Returning false from a listener prevents the checkbox from being toggled.
     * @event beforeChange
     * @preventable
     * @param {Core.widget.Checkbox} source Checkbox
     * @param {Boolean} checked Checked or not
     */
    /**
     * Fired when checkbox is toggled
     * @event change
     * @param {Core.widget.Checkbox} source Checkbox
     * @param {Boolean} checked Checked or not
     */
    // Prevent uncheck if this checkbox is part of a toggleGroup (radio-button mode) ..also ensure the group has
    // visible active members
    const eventObject = {
        checked,
        value: checked,
        userAction,
        valid: true
      },
      prevented = !checked && userAction && me.toggleGroup && me.getToggleGroupMembers().filter(widget => widget.isVisible && !widget.disabled).length ||
      // Since Widget has Events mixed in configured with 'callOnFunctions' this will also call onBeforeChange,
      // onChange and onAction
      me.trigger('beforeChange', eventObject) === false;
    // If prevented need to rollback the checkbox input
    if (prevented) {
      // Input change is not preventable, so need to revert the changes
      // The change event does not fire on programmatic change of input, so no need to suspend
      me.input.checked = me._value = !checked;
    } else {
      me.triggerFieldChange(eventObject, false);
      if (userAction) {
        me.uncheckToggleGroupMembers();
      }
      /**
       * User performed the default action (toggled the checkbox)
       * @event action
       * @param {Core.widget.Checkbox} source Checkbox
       * @param {Boolean} checked Checked or not
       */
      me.trigger('action', eventObject);
      me.trigger('change', eventObject);
      return true;
    }
  }
  //endregion
}
// Register this widget type with its Factory
Checkbox.initClass();
Checkbox._$name = 'Checkbox';

/**
 * @module Core/widget/YearPicker
 */
/**
 * A Panel subclass which allows a year to be selected from a range of 12 displayed years.
 *
 * The panel can be configured with {@link #config-startYear} to specify the first year in the
 * displayed range.
 *
 * The {@link #property-year} indicates and sets the currently selected year.
 *
 * The {@link #event-select} event is fired when a new year is selected.
 *
 * {@inlineexample Core/widget/YearPicker.js}
 *
 * @extends Core/widget/Panel
 *
 * @classType yearpicker
 * @widget
 */
class YearPicker extends Panel {
  static $name = 'YearPicker';
  // Factoryable type name
  static type = 'yearpicker';
  static configurable = {
    textContent: false,
    /**
     * The definition of the top toolbar which displays the title and "previous" and
     * "next" buttons.
     *
     * This contains the following predefined `items` which may be reconfigured by
     * application code:
     *
     * - `title` A widget which displays the visible year range. Weight 100.
     * - `previous` A button which navigates to the previous block. Weight 200.
     * - `next` A button which navigates to the next block. Weight 300.
     *
     * These may be reordered:
     *
     * ```javascript
     * new YearPicker({
     *     appendTo : targetElement,
     *     tbar     : {
     *         items : {
     *             // Move title to centre
     *             title : {
     *                 weight : 250
     *             }
     *         }
     *     },
     *     width    : '24em'
     * });
     * ```
     * @config {ToolbarConfig}
     */
    tbar: {
      overflow: null,
      items: {
        previous: {
          type: 'tool',
          cls: 'b-icon b-icon-previous',
          onAction: 'up.previous',
          weight: 100
        },
        title: {
          type: 'button',
          cls: 'b-yearpicker-title',
          weight: 200,
          onAction: 'up.handleTitleClick'
        },
        next: {
          type: 'tool',
          cls: 'b-icon b-icon-next',
          onAction: 'up.next',
          weight: 300
        }
      }
    },
    itemCls: 'b-year-container',
    /**
     * The number of clickable year buttons to display in the widget.
     *
     * It may be useful to change this if a non-standard shape or size is used.
     * @config {Number}
     * @default
     */
    yearButtonCount: 12,
    /**
     * The currently selected year.
     * @member {Number} year
     */
    /**
     * The year to use as the selected year. Defaults to the current year.
     * @config {Number}
     */
    year: null,
    /**
     * The lowest year to allow.
     * @config {Number}
     */
    minYear: null,
    /**
     * The highest year to allow.
     * @config {Number}
     */
    maxYear: null,
    /**
     * The starting year displayed in the widget.
     * @member {Number} startYear
     */
    /**
     * The year to show at the start of the widget
     * @config {Number}
     */
    startYear: null
  };
  construct(config) {
    super.construct({
      year: new Date().getFullYear(),
      ...config
    });
    EventHelper.on({
      element: this.contentElement,
      click: 'onYearClick',
      delegate: '.b-yearpicker-year',
      thisObj: this
    });
  }
  get focusElement() {
    return this.getYearButton(this.year) || this.getYearButton(this.startYear);
  }
  getYearButton(y) {
    return this.contentElement.querySelector(`.b-yearpicker-year[data-year="${y}"]`);
  }
  /**
   * The currently selected year.
   * @member {Number} value
   */
  get value() {
    return this.year;
  }
  set value(year) {
    this.year = year;
  }
  onYearClick({
    target
  }) {
    const clickedYear = Math.min(Math.max(parseInt(target.innerText), this.minYear || 1), this.maxYear || 9999);
    // The updater won't run, so fire the select event here.
    if (this.year === clickedYear) {
      this.trigger('select', {
        oldValue: clickedYear,
        value: clickedYear
      });
    } else {
      this.year = clickedYear;
    }
  }
  handleTitleClick(e) {
    this.trigger('titleClick', e);
  }
  previous() {
    this.startYear = this.startYear - this.yearButtonCount;
  }
  next() {
    this.startYear = this.endYear + 1;
  }
  ingestYear(year) {
    if (!isNaN(year)) {
      return ObjectHelper.isDate(year) ? year.getFullYear() : year;
    }
  }
  changeYear(year) {
    // ingestYear returns undefined if invalid input
    if (year = this.ingestYear(year)) {
      return Math.min(Math.max(year, this.minYear || 1), this.maxYear || 9999);
    }
  }
  updateYear(year, oldValue) {
    const me = this;
    if (!me.startYear || year > me.endYear) {
      me.startYear = year;
    } else if (year < me.startYear) {
      me.startYear = year - (me.yearButtonCount - 1);
    }
    if (!me.isConfiguring) {
      /**
       * Fired when a year is selected.
       * @event select
       * @param {Number} value The previously selected year.
       * @param {Core.widget.YearPicker} source This YearPicker
       */
      me.trigger('select', {
        oldValue,
        value: year
      });
    }
  }
  /**
   * The ending year displayed in the widget.
   * @member {Number} endYear
   * @readonly
   */
  get endYear() {
    return this.startYear + this.yearButtonCount - 1;
  }
  changeStartYear(startYear) {
    // ingestYear returns undefined if invalid input
    if (startYear = this.ingestYear(startYear)) {
      startYear = this.minYear ? Math.max(startYear, this.minYear) : startYear;
      return this.maxYear ? Math.min(startYear, this.maxYear - (this.yearButtonCount - 1)) : startYear;
    }
  }
  async updateStartYear(startYear, oldStartYear) {
    if (this.isVisible) {
      DomHelper.slideIn(this.contentElement, Math.sign(startYear - oldStartYear));
    }
  }
  composeBody() {
    // Must be ingested before first compose.
    this.getConfig('year');
    const {
        startYear
      } = this,
      result = super.composeBody(),
      children = result.children[this.tbar ? 1 : 0].children = [];
    this.widgetMap.title.text = `${`000${startYear}`.slice(-4)} - ${`000${this.endYear}`.slice(-4)}`;
    for (let i = 0, y = startYear; i < this.yearButtonCount; i++, y++) {
      children.push({
        tag: 'button',
        dataset: {
          year: y
        },
        class: {
          'b-yearpicker-year': 1,
          'b-selected': y === this.year
        },
        text: `000${y}`.slice(-4)
      });
    }
    return result;
  }
}
// Register this widget type with its Factory
YearPicker.initClass();
YearPicker._$name = 'YearPicker';

/**
 * @module Core/widget/DisplayField
 */
/**
 * A widget used to show a read only value. Can also use a {@link #config-template template string} to show custom
 * markup inside a Container.
 *
 * @extends Core/widget/Field
 *
 * @example
 * let displayField = new DisplayField({
 *   appendTo : document.body,
 *   label: 'name',
 *   value : 'John Doe',
 *   // or use a template
 *   // template : name => `${name} is the name`
 * });
 *
 * @classType displayField
 * @inlineexample Core/widget/DisplayField.js
 * @widget
 * @inputfield
 */
class DisplayField extends TextField {
  static get $name() {
    return 'DisplayField';
  }
  // Factoryable type name
  static get type() {
    return 'displayfield';
  }
  // Factoryable type alias
  static get alias() {
    return 'display';
  }
  static get configurable() {
    return {
      readOnly: true,
      editable: false,
      cls: 'b-display-field',
      /**
       * A template string used to render the value of this field. Please note you are responsible for encoding
       * any strings protecting against XSS.
       *
       * ```javascript
       * new DisplayField({
       *     appendTo : document.body,
       *     name     : 'age',
       *     label    : 'Age',
       *     template : data => `${data.value} years old`
       * })
       * ```
       * @config {Function}
       */
      template: null,
      ariaElement: 'displayElement'
    };
  }
  get focusElement() {
    // we're not focusable.
  }
  changeReadOnly() {
    return true;
  }
  changeEditable() {
    return false;
  }
  get inputElement() {
    return {
      tag: 'span',
      id: `${this.id}-input`,
      reference: 'displayElement',
      html: this.template ? this.template(this.value) : StringHelper.encodeHtml(this.value)
    };
  }
}
// Register this widget type with its Factory
DisplayField.initClass();
DisplayField._$name = 'DisplayField';

const generateMonthNames = () => DateHelper.getMonthNames().map((m, i) => [i, m]),
  dateSort = (lhs, rhs) => lhs.valueOf() - rhs.valueOf(),
  emptyArray = Object.freeze([]);
class ReadOnlyCombo extends Combo {
  static get $name() {
    return 'ReadOnlyCombo';
  }
  static get type() {
    return 'readonlycombo';
  }
  static get configurable() {
    return {
      editable: false,
      inputAttributes: {
        tag: 'div',
        tabIndex: -1
      },
      inputValueAttr: 'innerHTML',
      highlightExternalChange: false,
      monitorResize: false,
      triggers: {
        expand: false
      },
      picker: {
        align: {
          align: 't-b',
          axisLock: true,
          matchSize: false
        },
        cls: 'b-readonly-combo-list',
        scrollable: {
          overflowX: false
        }
      }
    };
  }
}
ReadOnlyCombo.initClass();
/**
 * @module Core/widget/DatePicker
 */
/**
 * A Panel which can display a month of date cells, which navigates between the cells, fires events upon user selection
 * actions, optionally navigates to other months in response to UI gestures, and optionally displays information about
 * each date cell.
 *
 * A date is selected (meaning a single value is selected if {@link #config-multiSelect} is not set, or
 * added to the {@link #property-selection} if {@link #config-multiSelect if set}) by clicking a cell
 * or by pressing `ENTER` when focused on a cell.
 *
 * The `SHIFT` and `CTRL` keys modify selection behaviour depending on the value of {@link #config-multiSelect}.
 *
 * This class is used as a {@link Core.widget.DateField#config-picker} by the {@link Core.widget.DateField} class.
 *
 * {@inlineexample Core/widget/DatePicker.js}
 *
 * ## Custom cell rendering
 * You can easily control the content of each date cell using the {@link #config-cellRenderer}. The example below shows
 * a view typically seen when booking hotel rooms or apartments.
 *
 * {@inlineexample Core/widget/DatePickerCellRenderer.js}
 *
 * ## Multi selection
 * You can select multiple date ranges or a single date range using the {@link #config-multiSelect} config.
 *
 * {@inlineexample Core/widget/DatePickerMulti.js}
 *
 * @classtype datepicker
 * @extends Core/widget/CalendarPanel
 * @widget
 */
class DatePicker extends CalendarPanel {
  static get $name() {
    return 'DatePicker';
  }
  // Factoryable type name
  static get type() {
    return 'datepicker';
  }
  static get delayable() {
    return {
      refresh: 'raf'
    };
  }
  static get configurable() {
    return {
      /**
       * The date that the user has navigated to using the UI *prior* to setting the widget's
       * value by selecting. The initial default is today's date.
       *
       * This may be changed using keyboard navigation. The {@link Core.widget.CalendarPanel#property-date} is set
       * by pressing `ENTER` when the desired date is reached.
       *
       * Programmatically setting the {@link Core.widget.CalendarPanel#config-date}, or using the UI to select the date
       * by clicking it also sets the `activeDate`
       * @config {Date}
       */
      activeDate: {
        value: new Date(),
        $config: {
          equal: 'date'
        }
      },
      focusable: true,
      textContent: false,
      tbar: {
        overflow: null,
        items: {
          prevYear: {
            cls: 'b-icon b-icon-first',
            onAction: 'up.gotoPrevYear',
            tooltip: 'L{DatePicker.gotoPrevYear}'
          },
          prevMonth: {
            cls: 'b-icon b-icon-previous',
            onAction: 'up.gotoPrevMonth',
            tooltip: 'L{DatePicker.gotoPrevMonth}'
          },
          fields: {
            type: 'container',
            cls: 'b-datepicker-title',
            items: {
              monthField: {
                type: 'readonlycombo',
                cls: 'b-datepicker-monthfield',
                items: generateMonthNames(),
                internalListeners: {
                  select: 'up.onMonthPicked'
                }
              },
              yearButton: {
                type: 'button',
                cls: 'b-datepicker-yearbutton',
                internalListeners: {
                  click: 'up.onYearPickerRequested'
                }
              }
            }
          },
          nextMonth: {
            cls: 'b-icon b-icon-next',
            onAction: 'up.gotoNextMonth',
            tooltip: 'L{DatePicker.gotoNextMonth}'
          },
          nextYear: {
            cls: 'b-icon b-icon-last',
            onAction: 'up.gotoNextYear',
            tooltip: 'L{DatePicker.gotoNextYear}'
          }
        }
      },
      yearPicker: {
        value: {
          type: 'YearPicker',
          yearButtonCount: 16,
          trapFocus: true,
          positioned: true,
          hidden: true,
          internalListeners: {
            titleClick: 'up.onYearPickerTitleClick',
            select: 'up.onYearPicked'
          }
        },
        $config: 'lazy'
      },
      /**
       * The initially selected date.
       * @config {Date}
       */
      date: null,
      /**
       * The minimum selectable date. Selection of and navigation to dates prior
       * to this date will not be possible.
       * @config {Date}
       */
      minDate: {
        value: null,
        $config: {
          equal: 'date'
        }
      },
      /**
       * The maximum selectable date. Selection of and navigation to dates after
       * this date will not be possible.
       * @config {Date}
       */
      maxDate: {
        value: null,
        $config: {
          equal: 'date'
        }
      },
      /**
       * By default, disabled dates cannot be navigated to, and they are skipped over
       * during keyboard navigation. Configure this as `true` to enable navigation to
       * disabled dates.
       * @config {Boolean}
       * @default
       */
      focusDisabledDates: null,
      /**
       * Configure as `true` to enable selecting multiple discontiguous date ranges using
       * click and Shift+click to create ranges and Ctrl+click to select/deselect individual dates.
       *
       * Configure as `'range'` to enable selecting a single date range by selecting a
       * start and end date. Hold "SHIFT" button to select date range. Ctrl+click may add
       * or remove dates to/from either end of the range.
       * @config {Boolean|'range'}
       * @default
       */
      multiSelect: false,
      /**
       * If {@link #config-multiSelect} is configured as `true`, this is an array of dates
       * which are selected. There may be multiple, discontiguous date ranges.
       *
       * If {@link #config-multiSelect} is configured as `'range'`, this is a two element array
       * specifying the first and last selected dates in a range.
       * @config {Date[]}
       */
      selection: {
        $config: {
          equal: (v1, v2) => v1 && v1.equals(v2)
        },
        value: null
      },
      /**
       * By default, the month and year are editable. Configure this as `false` to prevent that.
       * @config {Boolean}
       * @default
       */
      editMonth: true,
      /**
       * The {@link Core.helper.DateHelper} format string to format the day names.
       * @config {String}
       * @default
       */
      dayNameFormat: 'dd',
      trapFocus: true,
      role: 'grid',
      focusDescendant: true,
      /**
       * By default, when the {@link #property-date} changes, the UI will only refresh
       * if it doesn't contain a cell for that date, so as to keep a stable UI when
       * navigating.
       *
       * Configure this as `true` to refresh the UI whenever the month changes, even if
       * the UI already shows that date.
       * @config {Boolean}
       * @internal
       */
      alwaysRefreshOnMonthChange: null
    };
  }
  static get prototypeProperties() {
    return {
      /**
       * The class name to add to the calendar cell whose date which is outside of the
       * {@link #config-minDate}/{@link #config-maxDate} range.
       * @config {String}
       * @private
       */
      outOfRangeCls: 'b-out-of-range',
      /**
       * The class name to add to the currently focused calendar cell.
       * @config {String}
       * @private
       */
      activeCls: 'b-active-date',
      /**
       * The class name to add to selected calendar cells.
       * @config {String}
       * @private
       */
      selectedCls: 'b-selected-date'
    };
  }
  // region Init
  construct(config) {
    const me = this;
    super.construct(config);
    me.externalCellRenderer = me.cellRenderer;
    me.cellRenderer = me.internalCellRenderer;
    me.element.setAttribute('aria-activedescendant', `${me.id}-active-day`);
    me.weeksElement.setAttribute('role', 'grid');
    me.weekElements.forEach(w => w.setAttribute('role', 'row'));
    me.element.setAttribute('ariaLabelledBy', me.widgetMap.fields.id);
    EventHelper.on({
      element: me.weeksElement,
      click: {
        handler: 'onCellClick',
        delegate: `.${me.dayCellCls}:not(.${me.disabledCls}):not(.${me.outOfRangeCls})`
      },
      mousedown: {
        handler: 'onCellMousedown',
        delegate: `.${me.dayCellCls}`
      },
      thisObj: me
    });
    me.widgetMap.monthField.readOnly = me.widgetMap.yearButton.disabled = !me.editMonth;
    // Ensure the DatePicker is immediately ready for use.
    me.refresh.flush();
  }
  afterHide() {
    var _this$_yearPicker;
    (_this$_yearPicker = this._yearPicker) === null || _this$_yearPicker === void 0 ? void 0 : _this$_yearPicker.hide();
    super.afterHide(...arguments);
  }
  doDestroy() {
    var _this$yearButton, _this$monthField;
    (_this$yearButton = this.yearButton) === null || _this$yearButton === void 0 ? void 0 : _this$yearButton.destroy();
    (_this$monthField = this.monthField) === null || _this$monthField === void 0 ? void 0 : _this$monthField.destroy();
    super.doDestroy();
  }
  // endregion
  get focusElement() {
    return this.weeksElement.querySelector(`.${this.dayCellCls}[tabIndex="0"]`);
  }
  doRefresh() {
    const me = this,
      oldActiveCell = me.focusElement,
      // Coerce the active date to be in the visible range.
      // Do not use the setter, the sync is done below
      activeDate = DateHelper.betweenLesser(me.activeDate, me.month.startDate, me.month.endDate) ? me.activeDate : me._activeDate = me.date;
    super.doRefresh(...arguments);
    // The focused cell will have been repurposed for a new date
    const dateOfOldActiveCell = DateHelper.parseKey(oldActiveCell === null || oldActiveCell === void 0 ? void 0 : oldActiveCell.dataset.date);
    // The position of the cell may have changed, so the "from" cell must
    // be identified by the date that is stamped into it *after* the refresh..
    if (activeDate - dateOfOldActiveCell) {
      me.syncActiveDate(activeDate, dateOfOldActiveCell);
    }
  }
  internalCellRenderer({
    cell,
    date
  }) {
    const me = this,
      {
        activeCls,
        selectedCls,
        externalCellRenderer
      } = me,
      isSelected = me.isSelectedDate(date),
      cellClassList = {
        [activeCls]: activeCls && me.isActiveDate(date),
        [selectedCls]: isSelected,
        [me.outOfRangeCls]: me.minDate && date < me.minDate || me.maxDate && date > me.maxDate
      };
    if (isSelected) {
      // Fix up start/inner/end range classes
      if (me.multiSelect) {
        const isStart = !me.isSelectedDate(DateHelper.add(date, -1, 'd')),
          isEnd = !me.isSelectedDate(DateHelper.add(date, 1, 'd'));
        cellClassList['b-range-start'] = isStart;
        cellClassList['b-range-end'] = isEnd;
        cellClassList['b-in-range'] = !isStart && !isEnd;
      }
    }
    DomHelper.updateClassList(cell, cellClassList);
    // Must replace entire content in case we have an externalCellRenderer
    cell.innerHTML = `<div class="b-datepicker-cell-inner">${date.getDate()}</div>`;
    cell.setAttribute('role', 'gridcell');
    cell.setAttribute('aria-label', DateHelper.format(date, 'MMMM D, YYYY'));
    if (me.isActiveDate(date)) {
      cell.id = `${me.id}-active-day`;
    } else {
      cell.removeAttribute('id');
    }
    if (externalCellRenderer) {
      arguments[0].cell = cell.firstChild;
      me.callback(externalCellRenderer, this, arguments);
    }
  }
  onCellMousedown(event) {
    const cell = event.target.closest('[data-date]');
    event.preventDefault();
    cell.focus();
    this.activeDate = DateHelper.parseKey(cell.dataset.date);
  }
  onCellClick(event) {
    const cell = event.target.closest('[data-date]');
    this.onUIDateSelect(DateHelper.parseKey(cell.dataset.date), event);
  }
  onMonthDateChange({
    newDate,
    changes
  }) {
    // toolbar widgets must have been instantiated.
    this.getConfig('tbar');
    super.onMonthDateChange(...arguments);
    // Keep header widgets synced with our month
    if (changes.m || changes.y) {
      this.widgetMap.monthField.value = newDate.getMonth();
      this.widgetMap.yearButton.text = newDate.getFullYear();
    }
  }
  /**
   * Called when the user uses the UI to select the current activeDate. So ENTER when focused
   * or clicking a date cell.
   * @param {Date} date The active date to select
   * @param {Event} event the instigating event, either a `click` event or a `keydown` event.
   * @internal
   */
  onUIDateSelect(date, event) {
    const me = this,
      {
        lastClickedDate,
        multiSelect
      } = me;
    me.lastClickedDate = date;
    if (!me.isDisabledDate(date)) {
      me.activatingEvent = event;
      // Handle multi selecting.
      // * single contiguous date range, eg: an event start and end
      // * multiple discontiguous ranges
      if (multiSelect) {
        me.handleMultiSelect(lastClickedDate, date, event);
      } else {
        me.selection = date;
        if (me.floating) {
          me.hide();
        }
      }
      me.activatingEvent = null;
    }
  }
  // Calls updateSelection if the selection is mutated
  handleMultiSelect(lastClickedDate, date, event) {
    const me = this,
      {
        multiSelect
      } = me,
      _selection = me._selection || (me._selection = new DateSet()),
      selection = _selection.dates,
      singleRange = multiSelect === 'range',
      {
        size,
        generation
      } = _selection,
      rangeEnds = size && {
        [DateHelper.makeKey(DateHelper.add(selection[0], -1, 'd'))]: 1,
        [DateHelper.makeKey(selection[0])]: 1,
        [DateHelper.makeKey(selection[selection.length - 1])]: 1,
        [DateHelper.makeKey(DateHelper.add(selection[selection.length - 1], 1, 'd'))]: 1
      },
      isSelected = _selection.has(date),
      toggleFn = isSelected ? 'delete' : 'add';
    // If we're allowed to create one range and they clicked on a togglable date of a range
    const clickedRangeEnd = singleRange && (rangeEnds === null || rangeEnds === void 0 ? void 0 : rangeEnds[DateHelper.makeKey(date)]);
    // Ctrl+click means toggle the date, leaving remaining selection unchanged
    if (event.ctrlKey) {
      // Allow individual date toggling if we are allowing multi ranges
      // or there's no current selection, or they are on, or adjacent to the range end
      if (multiSelect === true || !size || clickedRangeEnd) {
        _selection[toggleFn](date);
        // Check that the start hasn't been deselected
        if (singleRange && !_selection.has(me.rangeStartDate)) {
          me.rangeStartDate.setDate(me.rangeStartDate.getDate() + (date < selection[1] ? 1 : -1));
        }
      }
    }
    // Shift+click means add a range
    else if (event.shiftKey && size) {
      const [start, end] = [new Date(singleRange ? me.rangeStartDate || (me.rangeStartDate = selection[0]) : lastClickedDate), date].sort(dateSort);
      // If we can only have one range
      if (singleRange) {
        _selection.clear();
      }
      // Add all dates in the range
      for (const d = start; d <= end; d.setDate(d.getDate() + 1)) {
        _selection.add(d);
      }
    }
    // Make the clicked date the only selected date.
    // Avoid a no-op which would still cause a generation change
    else if (!(_selection.has(date) && _selection.size === 1)) {
      _selection.clear();
      _selection.add(date);
    }
    const newSize = _selection.size;
    // Keep track of the range start date. The first selected date is the start and the end then
    // can move to either side of that.
    if (newSize === 1) {
      me.rangeStartDate = date;
    } else if (!newSize) {
      me.rangeStartDate = null;
    }
    // Process selection change if we changed the selection.
    if (_selection.generation !== generation) {
      me.updateSelection(_selection);
    }
  }
  changeSelection(selection) {
    // We always need a _selection property to be a DateSet.
    // Falsy selection value means empty DateSet.
    const me = this;
    let result, rangeStartDate;
    if (selection) {
      // Convert single Date into Array
      if (!selection.forEach) {
        selection = [selection];
      }
      // Clamp selection into range. May duplicate, but the Set will dedupe.
      selection.forEach((d, i) => selection[i] = me.changeDate(d));
      // Cache the first date, regardless of sort order for use as the "clicked date"
      // around which the range revolves when shift+click is used.
      rangeStartDate = selection[0];
      selection.sort(dateSort);
      // A two element array means a start and end
      if (me.multiSelect === 'range' && selection.length === 2) {
        result = new DateSet();
        for (const d = new Date(selection[0]); d <= selection[1]; d.setDate(d.getDate() + 1)) {
          result.add(d);
        }
      } else {
        // Multi dates may be in any order, so use the temporally first date as range start
        rangeStartDate = selection[0];
        result = new DateSet(selection);
      }
    } else {
      result = new DateSet();
    }
    if (rangeStartDate) {
      me.activeDate = me.rangeStartDate = DateHelper.clearTime(rangeStartDate);
    }
    return result;
  }
  updateMultiSelect(multiSelect) {
    this.element.classList.toggle('b-multiselect', Boolean(multiSelect));
    if (!multiSelect) {
      this.selection = [...this.selection][0];
    }
  }
  updateSelection(dateSet) {
    const me = this,
      {
        dates
      } = dateSet,
      selection = me.multiSelect === 'range' ? [dates[0], dates[dates.length - 1]] : dates;
    // "date" property must be seen to be the selected date.
    dates.length && (me.date = dates[0]);
    if (!me.isConfiguring) {
      // We're going to announce the change. UI must be up to date
      me.refresh.now();
      /**
       * Fires when a date or date range is selected. If {@link #config-multiSelect} is specified,
       * this will fire upon deselection and selection of dates.
       * @event selectionChange
       * @param {Date[]} selection The selected date. If {@link #config-multiSelect} is specified
       * this may be a two element array specifying start and end dates.
       * @param {Boolean} userAction This will be `true` if the change was caused by user interaction
       * as opposed to programmatic setting.
       */
      me.trigger('selectionChange', {
        selection,
        userAction: Boolean(me.activatingEvent)
      });
    }
  }
  /**
   * The selected Date(s).
   *
   * When {@link #config-multiSelect} is `'range'`, then this yields a two element array
   * representing the start and end of the selected range.
   *
   * When {@link #config-multiSelect} is `true`, this yields an array containing every selected
   * Date.
   * @member {Date[]} selection
   */
  get selection() {
    const {
        _selection
      } = this,
      dates = _selection ? _selection.dates : emptyArray;
    return this.multiSelect === 'range' && dates.length ? [dates[0], dates[dates.length - 1]] : dates;
  }
  onInternalKeyDown(keyEvent) {
    const me = this,
      keyName = keyEvent.key.trim() || keyEvent.code,
      activeDate = me.activeDate;
    let newDate = new Date(activeDate);
    if (keyName === 'Escape' && me.floating) {
      return me.hide();
    }
    // Only navigate if not focused on one of our child widgets.
    // We have a prevMonth and nextMonth tool and possibly month and year pickers.
    if (activeDate && me.weeksElement.contains(keyEvent.target)) {
      do {
        switch (keyName) {
          case 'ArrowLeft':
            // Disable browser use of this key.
            // Ctrl+ArrowLeft navigates back.
            // ArrowLeft scrolls if there is horizontal scroll.
            keyEvent.preventDefault();
            if (keyEvent.ctrlKey) {
              newDate = me.gotoPrevMonth();
            } else {
              newDate.setDate(newDate.getDate() - 1);
            }
            break;
          case 'ArrowUp':
            // Disable browser use of this key.
            // ArrowUp scrolls if there is vertical scroll.
            keyEvent.preventDefault();
            newDate.setDate(newDate.getDate() - 7);
            break;
          case 'ArrowRight':
            // Disable browser use of this key.
            // Ctrl+ArrowRight navigates forwards.
            // ArrowRight scrolls if there is horizontal scroll.
            keyEvent.preventDefault();
            if (keyEvent.ctrlKey) {
              newDate = me.gotoNextMonth();
            } else {
              newDate.setDate(newDate.getDate() + 1);
            }
            break;
          case 'ArrowDown':
            // Disable browser use of this key.
            // ArrowDown scrolls if there is vertical scroll.
            keyEvent.preventDefault();
            newDate.setDate(newDate.getDate() + 7);
            break;
          case 'Enter':
            return me.onUIDateSelect(activeDate, keyEvent);
        }
      } while (me.isDisabledDate(newDate) && !me.focusDisabledDates);
      // Don't allow navigation to outside of date bounds.
      if (me.minDate && newDate < me.minDate) {
        return;
      }
      if (me.maxDate && newDate > me.maxDate) {
        return;
      }
      me.activeDate = newDate;
    }
  }
  changeMinDate(minDate) {
    // Avoid changeDate which clamps incoming value into current allowable range
    return minDate && CalendarPanel.prototype.changeDate.apply(this, arguments);
  }
  updateMinDate(minDate) {
    this._yearpicker && (this._yearpicker.minYear = minDate === null || minDate === void 0 ? void 0 : minDate.getFullYear());
    this.refresh();
  }
  changeMaxDate(minDate) {
    // Avoid changeDate which clamps incoming value into current allowable range
    return minDate && CalendarPanel.prototype.changeDate.apply(this, arguments);
  }
  updateMaxDate(maxDate) {
    this._yearpicker && (this._yearpicker.maxYear = maxDate === null || maxDate === void 0 ? void 0 : maxDate.getFullYear());
    this.refresh();
  }
  changeDate(date) {
    return DateHelper.clamp(super.changeDate(date), this.minDate, this.maxDate);
  }
  updateDate(date) {
    const me = this;
    // Directly configuring a date creates the selection
    me.isConfiguring && !me.initializingActiveDate && (me.selection = date);
    // Only change the month's date if it is within our current month
    // or we have to because we don't have a cell for it.
    // If it's a date in the "otherMonth" part of the grid, do not update.
    if (!me.month.date || date.getMonth() === me.month.month || !me.getCell(date) || me.alwaysRefreshOnMonthChange || me.isNavigating) {
      super.updateDate(date);
    }
  }
  changeActiveDate(activeDate, oldActiveDate) {
    if (this.trigger('beforeActiveDateChange', {
      activeDate,
      oldActiveDate
    }) === false) {
      return;
    }
    activeDate = activeDate ? this.changeDate(activeDate) : this.date || (this.date = DateHelper.clearTime(new Date()));
    if (isNaN(activeDate)) {
      throw new Error('DatePicker date ingestion must be passed a Date, or a YYYY-MM-DD date string');
    }
    return DateHelper.clamp(activeDate, this.minDate, this.maxDate);
  }
  updateActiveDate(activeDate, wasActiveDate) {
    const me = this,
      {
        isConfiguring
      } = me;
    if (isConfiguring || !me.getCell(activeDate)) {
      me.initializingActiveDate = isConfiguring;
      me.date = activeDate;
      me.initializingActiveDate = false;
    }
    if (!isConfiguring && !me.refresh.isPending) {
      me.syncActiveDate(activeDate, wasActiveDate);
    }
  }
  syncActiveDate(activeDate, wasActiveDate) {
    const me = this,
      {
        activeCls
      } = me,
      activeCell = me.getCell(activeDate),
      wasActiveCell = wasActiveDate && me.getCell(wasActiveDate),
      activeElement = DomHelper.getActiveElement(me.element);
    activeCell.setAttribute('tabIndex', 0);
    activeCls && activeCell.classList.add(activeCls);
    activeCell.id = `${me.id}-active-day`;
    if (me.weeksElement.contains(activeElement) /*|| me.owner?.element.contains(activeElement)*/) {
      activeCell.focus();
    }
    if (wasActiveCell && wasActiveCell !== activeCell) {
      wasActiveCell.removeAttribute('tabIndex');
      activeCls && wasActiveCell.classList.remove(activeCls);
      wasActiveCell.removeAttribute('id');
    }
  }
  set value(value) {
    const me = this,
      {
        selection,
        duration
      } = me;
    if (value) {
      value = me.changeDate(value, me.value);
      // If we're maintaining a single date range, move the range
      if (me.multiSelect === 'range' && (selection === null || selection === void 0 ? void 0 : selection.length) === 2) {
        if (!DateHelper.betweenLesserEqual(value, ...selection)) {
          // Move range back to encapsulate date
          if (value < selection[0]) {
            me.selection = [value, DateHelper.add(value, duration - 1, 'd')];
          }
          // Move range forwards to encapsulate date
          else {
            me.selection = [DateHelper.add(value, -(duration - 1), 'd'), value];
          }
        }
        me.date = me.activeDate = value;
        return;
      }
      // Undefined return value means no change
      if (value !== undefined) {
        me.selection = value;
      }
    } else {
      // Clearing the value - go to today's calendar
      me.date = new Date();
      me.selection = null;
    }
  }
  get value() {
    return this.selection[this.selection.length - 1];
  }
  get duration() {
    return this.multiSelect === 'range' ? DateHelper.diff(...this.selection, 'd') + 1 : 1;
  }
  gotoPrevYear() {
    return this.goto(-1, 'year');
  }
  gotoPrevMonth() {
    return this.goto(-1, 'month');
  }
  gotoNextMonth() {
    return this.goto(1, 'month');
  }
  gotoNextYear() {
    return this.goto(1, 'year');
  }
  goto(direction, unit) {
    const me = this,
      {
        activeDate
      } = me,
      activeCell = activeDate && me.getCell(activeDate);
    let newDate;
    // If active date is already in the month we're going to, use it
    if (unit === 'month' && activeCell && (activeDate === null || activeDate === void 0 ? void 0 : activeDate.getMonth()) === me.month.month + direction) {
      newDate = activeDate;
    }
    // Move the date by the requested unit
    else {
      newDate = DateHelper.add(activeCell ? activeDate : me.date, direction, unit);
    }
    const firstDateOfNewMonth = new Date(newDate);
    firstDateOfNewMonth.setDate(1);
    const lastDateOfNewMonth = DateHelper.add(DateHelper.add(firstDateOfNewMonth, 1, 'month'), -1, 'day');
    // Don't navigate if month is outside bounds
    if (me.minDate && direction < 0 && lastDateOfNewMonth < me.minDate || me.maxDate && direction > 0 && firstDateOfNewMonth > me.maxDate) {
      return;
    }
    // We need to force a UI change even if the UI contains the target date.
    // updateDate always calls super and CalendarPanel will refresh
    me.isNavigating = true;
    const result = me.date = newDate;
    if (activeCell) {
      me.activeDate = newDate;
    }
    me.isNavigating = false;
    return result;
  }
  isActiveDate(date) {
    return !(date - this.activeDate);
  }
  isSelectedDate(date) {
    var _this$_selection;
    return (_this$_selection = this._selection) === null || _this$_selection === void 0 ? void 0 : _this$_selection.has(date);
  }
  onMonthPicked({
    record,
    userAction
  }) {
    if (userAction) {
      var _this$focusElement;
      this.activeDate = DateHelper.add(this.activeDate, record.value - this.activeDate.getMonth(), 'month');
      (_this$focusElement = this.focusElement) === null || _this$focusElement === void 0 ? void 0 : _this$focusElement.focus();
    }
  }
  onYearPickerRequested() {
    const {
      yearPicker
    } = this;
    if (yearPicker.isVisible) {
      yearPicker.hide();
    } else {
      yearPicker.year = yearPicker.startYear = this.activeDate.getFullYear();
      yearPicker.show();
      yearPicker.focus();
    }
  }
  onYearPickerTitleClick() {
    this.yearPicker.hide();
  }
  onYearPicked({
    value,
    source
  }) {
    const newDate = new Date(this.activeDate);
    newDate.setFullYear(value);
    this.activeDate = newDate;
    // Move focus without scroll *before* focus reversion from the hide.
    // Browser behaviour of scrolling to focused element would break animation.
    this.focusElement && DomHelper.focusWithoutScrolling(this.focusElement);
    source.hide();
  }
  changeYearPicker(yearPicker, oldYearPicker) {
    var _this$minDate, _this$maxDate;
    return YearPicker.reconfigure(oldYearPicker, yearPicker ? YearPicker.mergeConfigs({
      owner: this,
      appendTo: this.element,
      minYear: (_this$minDate = this.minDate) === null || _this$minDate === void 0 ? void 0 : _this$minDate.getFullYear(),
      maxYear: (_this$maxDate = this.maxDate) === null || _this$maxDate === void 0 ? void 0 : _this$maxDate.getFullYear()
    }, yearPicker) : null, this);
  }
  get childItems() {
    const {
        _yearPicker
      } = this,
      result = super.childItems;
    if (_yearPicker) {
      result.push(_yearPicker);
    }
    return result;
  }
  updateLocalization() {
    const {
        monthField
      } = this.widgetMap,
      newData = generateMonthNames();
    if (!this.isConfiguring && !newData.every((d, i) => d[1] === monthField.store.getAt(i).text)) {
      newData[monthField.value].selected = true;
      monthField.items = newData;
    }
    super.updateLocalization();
  }
}
// Dates are never equal, so raw Set won't work.
class DateSet extends Set {
  add(d) {
    d = DateHelper.makeKey(d);
    if (!this.has(d)) {
      this.generation = (this.generation || 0) + 1;
    }
    return super.add(d);
  }
  delete(d) {
    d = DateHelper.makeKey(d);
    if (this.has(d)) {
      this.generation++;
    }
    return super.delete(d);
  }
  has(d) {
    return super.has(DateHelper.makeKey(d));
  }
  clear() {
    if (this.size) {
      this.generation++;
    }
    return super.clear();
  }
  equals(other) {
    Array.isArray(other) && (other = new DateSet(other));
    return other.size === this.size && [...this].every(s => other.has(s));
  }
  get dates() {
    return [...this].sort().map(k => DateHelper.parseKey(k));
  }
}
// Register this widget type with its Factory
DatePicker.initClass();
DatePicker._$name = 'DatePicker';

/**
 * @module Core/widget/DateField
 */
/**
 * Date field widget (text field + date picker).
 *
 * This field can be used as an {@link Grid.column.Column#config-editor editor} for the {@link Grid.column.Column Column}.
 * It is used as the default editor for the {@link Grid.column.DateColumn DateColumn}.
 *
 * This widget may be operated using the keyboard. `ArrowDown` opens the date picker, which itself
 * is keyboard navigable. `Shift+ArrowDown` activates the {@link #config-step} back trigger.
 * `Shift+ArrowUp` activates the {@link #config-step} forwards trigger.
 *
 * @extends Core/widget/PickerField
 *
 * @example
 * // minimal DateField config with date format specified
 * let dateField = new DateField({
 *   format: 'YYMMDD'
 * });
 *
 * @classType datefield
 * @inlineexample Core/widget/DateField.js
 * @inputfield
 */
class DateField extends PickerField {
  //region Config
  static get $name() {
    return 'DateField';
  }
  // Factoryable type name
  static get type() {
    return 'datefield';
  }
  // Factoryable type alias
  static get alias() {
    return 'date';
  }
  static get configurable() {
    return {
      /**
       * Get / set format for date displayed in field (see {@link Core.helper.DateHelper#function-format-static}
       * for formatting options).
       * @member {String} format
       */
      /**
       * Format for date displayed in field. Defaults to using long date format, as defined by current locale (`L`)
       * @config {String}
       * @default
       */
      format: 'L',
      /**
       * A flag which indicates whether the date parsing should be strict - meaning if the date
       * is missing a year/month/day part - parsing fails.
       *
       * Turned off by default, meaning default values are substituted for missing parts.
       *
       * @config {Boolean}
       * @default
       */
      strictParsing: false,
      // same for all languages
      fallbackFormat: 'YYYY-MM-DD',
      timeFormat: 'HH:mm:ss:SSS',
      /**
       * A flag which indicates what time should be used for selected date.
       * `false` by default which means time is reset to midnight.
       *
       * Possible options are:
       * - `false` to reset time to midnight
       * - `true` to keep original time value
       * - `'17:00'` a string which is parsed automatically
       * - `new Date(2020, 0, 1, 17)` a date object to copy time from
       * - `'entered'` to keep time value entered by user (in case {@link #config-format} includes time info)
       *
       * @config {Boolean|Date|String}
       * @default
       */
      keepTime: false,
      /**
       * Format for date in the {@link #config-picker}. Uses localized format per default
       * @config {String}
       */
      pickerFormat: null,
      /**
       * Set to true to first clear time of the field's value before comparing it to the max value
       * @internal
       * @config {Boolean}
       */
      validateDateOnly: null,
      triggers: {
        expand: {
          cls: 'b-icon-calendar',
          handler: 'onTriggerClick',
          weight: 200
        },
        back: {
          cls: 'b-icon b-icon-angle-left b-step-trigger',
          key: 'Shift+ArrowDown',
          handler: 'onBackClick',
          align: 'start',
          weight: 100
        },
        forward: {
          cls: 'b-icon b-icon-angle-right b-step-trigger',
          key: 'Shift+ArrowUp',
          handler: 'onForwardClick',
          align: 'end',
          weight: 100
        }
      },
      // An optional extra CSS class to add to the picker container element
      calendarContainerCls: '',
      /**
       * Get/set min value, which can be a Date or a string. If a string is specified, it will be converted using
       * the specified {@link #config-format}.
       * @member {Date} min
       * @accepts {String|Date}
       */
      /**
       * Min value
       * @config {String|Date}
       */
      min: null,
      /**
       * Get/set max value, which can be a Date or a string. If a string is specified, it will be converted using
       * the specified {@link #config-format}.
       * @member {Date} max
       * @accepts {String|Date}
       */
      /**
       * Max value
       * @config {String|Date}
       */
      max: null,
      /**
       * The `step` property may be set in object form specifying two properties, `magnitude`, a Number, and
       * `unit`, a String.
       *
       * If a Number is passed, the step's current unit is used (or `day` if no current step set) and just the
       * magnitude is changed.
       *
       * If a String is passed, it is parsed by {@link Core.helper.DateHelper#function-parseDuration-static}, for
       * example `'1d'`, `'1 d'`, `'1 day'`, or `'1 day'`.
       *
       * Upon read, the value is always returned in object form containing `magnitude` and `unit`.
       * @member {DurationConfig} step
       * @accepts {String|Number|DurationConfig}
       */
      /**
       * Time increment duration value. If specified, `forward` and `back` triggers are displayed.
       * The value is taken to be a string consisting of the numeric magnitude and the units.
       * The units may be a recognised unit abbreviation of this locale or the full local unit name.
       * For example `'1d'` or `'1w'` or `'1 week'`. This may be specified as an object containing
       * two properties: `magnitude`, a Number, and `unit`, a String
       * @config {String|Number|DurationConfig}
       */
      step: false,
      stepTriggers: null,
      /**
       * The week start day in the {@link #config-picker}, 0 meaning Sunday, 6 meaning Saturday.
       * Uses localized value per default.
       * @config {Number}
       */
      weekStartDay: null,
      /**
       * A config object used to configure the {@link Core.widget.DatePicker datePicker}.
       * ```javascript
       * dateField = new DateField({
       *      picker    : {
       *          multiSelect : true
       *      }
       *  });
       * ```
       * @config {DatePickerConfig}
       */
      picker: {
        type: 'datepicker',
        role: 'dialog',
        floating: true,
        scrollAction: 'realign',
        align: {
          align: 't0-b0',
          axisLock: true
        }
      },
      /**
       * Get/set value, which can be set as a Date or a string but always returns a Date. If a string is
       * specified, it will be converted using the specified {@link #config-format}
       * @member {Date} value
       * @accepts {String|Date}
       */
      /**
       * Value, which can be a Date or a string. If a string is specified, it will be converted using the
       * specified {@link #config-format}
       * @config {String|Date}
       */
      value: null
    };
  }
  //endregion
  //region Init & destroy
  /**
   * Creates default picker widget
   *
   * @internal
   */
  changePicker(picker, oldPicker) {
    const me = this,
      defaults = {
        owner: me,
        forElement: me[me.pickerAlignElement],
        minDate: me.min,
        maxDate: me.max,
        weekStartDay: me._weekStartDay,
        // need to pass the raw value to let the component to use its default value
        align: {
          anchor: me.overlayAnchor,
          target: me[me.pickerAlignElement]
        },
        onSelectionChange: ({
          selection,
          source: picker
        }) => {
          // We only care about what DatePicker does if it has been opened
          if (picker.isVisible) {
            me._isUserAction = me._isPickerInput = true;
            me.value = selection[0];
            me._isPickerInput = me._isUserAction = false;
            picker.hide();
          }
        }
      };
    if (me.calendarContainerCls) {
      defaults.cls = me.calendarContainerCls;
    }
    // If we are in cleared state, the picker will also have no value.
    // But have it focused on today as a default.
    if (me.value) {
      defaults.value = me.value;
    } else {
      defaults.activeDate = new Date();
    }
    const result = DatePicker.reconfigure(oldPicker, picker, {
      owner: me,
      defaults
    });
    // Cells must exist early
    result === null || result === void 0 ? void 0 : result.refresh.flush();
    return result;
  }
  //endregion
  //region Click listeners
  get backShiftDate() {
    return DateHelper.add(this.value, -1 * this._step.magnitude, this._step.unit);
  }
  onBackClick() {
    const me = this,
      {
        min
      } = me;
    if (!me.readOnly && me.value) {
      const newValue = me.backShiftDate;
      if (!min || min.getTime() <= newValue) {
        me._isUserAction = true;
        me.value = newValue;
        me._isUserAction = false;
      }
    }
  }
  get forwardShiftDate() {
    return DateHelper.add(this.value, this._step.magnitude, this._step.unit);
  }
  onForwardClick() {
    const me = this,
      {
        max
      } = me;
    if (!me.readOnly && me.value) {
      const newValue = me.forwardShiftDate;
      if (!max || max.getTime() >= newValue) {
        me._isUserAction = true;
        me.value = newValue;
        me._isUserAction = false;
      }
    }
  }
  //endregion
  //region Toggle picker
  showPicker(focusPicker) {
    if (this.readOnly) {
      return;
    }
    const me = this,
      {
        _picker
      } = me;
    // If it's already instanced, navigate it to our date, or default of today.
    // It will be initialized correctly if not.
    if (_picker) {
      // In case a subclass uses a min getter (which does not update our min value) - ensure picker is correctly configured
      const pickerConfig = {
        minDate: me.min,
        maxDate: me.max
      };
      if (me.value) {
        pickerConfig.value = me.value;
      }
      // If the field is cleared, show today's Calendar in the picker
      // unless it's already been navigated elsewhere.
      else if (!_picker.activeDate) {
        pickerConfig.activeDate = new Date();
      }
      // Config dependencies are correctly processed by setConfig
      _picker.setConfig(pickerConfig);
    }
    super.showPicker(focusPicker);
  }
  focusPicker() {
    this.picker.focus();
  }
  //endregion
  // region Validation
  get isValid() {
    const me = this;
    me.clearError('L{Field.minimumValueViolation}', true);
    me.clearError('L{Field.maximumValueViolation}', true);
    let value = me.value;
    if (value) {
      const {
        min,
        max,
        validateDateOnly
      } = me;
      // Validation of the date should only care about the date part
      if (validateDateOnly) {
        value = DateHelper.clearTime(value, false);
      }
      if (min && value < min) {
        me.setError('L{Field.minimumValueViolation}', true);
        return false;
      }
      if (max && value > max) {
        me.setError('L{Field.maximumValueViolation}', true);
        return false;
      }
    }
    return super.isValid;
  }
  //endregion
  //region Getters/setters
  transformDateValue(value) {
    const me = this;
    if (value != null) {
      if (!DateHelper.isDate(value)) {
        if (typeof value === 'string') {
          // If date cannot be parsed with set format, try fallback - the more general one
          value = DateHelper.parse(value, me.format, me.strictParsing) || DateHelper.parse(value, me.fallbackFormat, me.strictParsing);
        } else {
          value = new Date(value);
        }
      }
      // We insist on a *valid* Date as the value
      if (DateHelper.isValidDate(value)) {
        if ((!me.min || value - me.min > -DateHelper.MS_PER_DAY) && (!me.max || value <= me.max)) {
          return me.transformTimeValue(value);
        }
        return value;
      }
    }
    return null;
  }
  transformTimeValue(value) {
    const me = this,
      {
        keepTime
      } = me;
    value = DateHelper.clone(value);
    if (!keepTime) {
      DateHelper.clearTime(value, false);
    }
    // change time if keepTime !== 'entered'
    else if (keepTime !== 'entered') {
      const timeValue = DateHelper.parse(keepTime, me.timeFormat);
      // if this.keepTime is a valid date or a string describing valid time copy from it
      if (DateHelper.isValidDate(timeValue)) {
        DateHelper.copyTimeValues(value, timeValue);
      }
      // otherwise try to copy from the current value
      else if (DateHelper.isValidDate(me.value)) {
        DateHelper.copyTimeValues(value, me.value);
      }
    }
    // if keepTime === 'entered' and picker is used apply current value time
    else if (me._isPickerInput && DateHelper.isValidDate(me.value)) {
      DateHelper.copyTimeValues(value, me.value);
    }
    // else don't change time
    return value;
  }
  changeMin(value) {
    return this.transformDateValue(value);
  }
  updateMin(min) {
    const {
      input,
      _picker
    } = this;
    if (input) {
      if (min == null) {
        input.removeAttribute('min');
      } else {
        input.min = min;
      }
    }
    // See if our lazy config has been realized...
    if (_picker) {
      _picker.minDate = min;
    }
    this.syncInvalid();
  }
  changeMax(value) {
    return this.transformDateValue(value);
  }
  updateMax(max) {
    const {
      input,
      _picker
    } = this;
    if (input) {
      if (max == null) {
        input.removeAttribute('max');
      } else {
        input.max = max;
      }
    }
    if (_picker) {
      _picker.maxDate = max;
    }
    this.syncInvalid();
  }
  get weekStartDay() {
    // This trick allows our weekStartDay to float w/the locale even if the locale changes
    return typeof this._weekStartDay === 'number' ? this._weekStartDay : DateHelper.weekStartDay;
  }
  updateWeekStartDay(weekStartDay) {
    if (this._picker) {
      this._picker.weekStartDay = weekStartDay;
    }
  }
  changeValue(value, oldValue) {
    const me = this,
      newValue = me.transformDateValue(value);
    // A value we could not parse
    if (value && !newValue) {
      // setError uses localization
      me.setError('L{invalidDate}');
      return;
    }
    me.clearError('L{invalidDate}');
    // Reject non-change
    if (me.hasChanged(oldValue, newValue)) {
      return super.changeValue(newValue, oldValue);
    }
    // But we must fix up the display in case it was an unparseable string
    // and the value therefore did not change.
    if (!me.inputting) {
      me.syncInputFieldValue();
    }
  }
  updateValue(value, oldValue) {
    const picker = this._picker;
    if (picker && !this.inputting) {
      picker.value = picker.activeDate = value;
    }
    super.updateValue(value, oldValue);
  }
  changeStep(value, was) {
    const type = typeof value;
    if (!value) {
      return null;
    }
    if (type === 'number') {
      value = {
        magnitude: Math.abs(value),
        unit: was ? was.unit : 'day'
      };
    } else if (type === 'string') {
      value = DateHelper.parseDuration(value);
    }
    if (value && value.unit && value.magnitude) {
      if (value.magnitude < 0) {
        value = {
          magnitude: -value.magnitude,
          // Math.abs
          unit: value.unit
        };
      }
      return value;
    }
  }
  updateStep(value) {
    // If a step is configured, show the steppers
    this.element.classList[value ? 'remove' : 'add']('b-no-steppers');
    this.syncInvalid();
  }
  hasChanged(oldValue, newValue) {
    // if both dates are provided and the field does not has time info in its format
    if (oldValue !== null && oldValue !== void 0 && oldValue.getTime && newValue !== null && newValue !== void 0 && newValue.getTime && this.keepTime !== 'entered') {
      // Only compare date part
      return !DateHelper.isEqual(DateHelper.clearTime(oldValue), DateHelper.clearTime(newValue));
    }
    return super.hasChanged(oldValue && oldValue.getTime(), newValue && newValue.getTime());
  }
  get inputValue() {
    // Do not use the _value property. If called during configuration, this
    // will import the configured value from the config object.
    const date = this.value;
    return date ? DateHelper.format(date, this.format) : '';
  }
  updateFormat() {
    if (!this.isConfiguring) {
      this.syncInputFieldValue(true);
    }
  }
  //endregion
  //region Localization
  updateLocalization() {
    super.updateLocalization();
    this.syncInputFieldValue(true);
  }
  //endregion
  //region Other
  internalOnKeyEvent(event) {
    super.internalOnKeyEvent(event);
    if (event.key === 'Enter' && this.isValid) {
      this.picker.hide();
    }
  }
  //endregion
}
// Register this widget type with its Factory
DateField.initClass();
DateField._$name = 'DateField';

/**
 * @module Core/widget/NumberField
 */
/**
 * Number field widget. Similar to native `<input type="number">`, but implemented as `<input type="text">` to support
 * formatting.
 *
 * This field can be used as an {@link Grid/column/Column#config-editor} for the {@link Grid/column/Column}.
 * It is used as the default editor for the {@link Grid/column/NumberColumn},
 * {@link Grid/column/PercentColumn}, {@link Grid/column/AggregateColumn}.
 *
 * ```javascript
 * const number = new NumberField({
 *     min   : 1,
 *     max   : 5,
 *     value : 3
 * });
 * ```
 *
 * Provide a {@link Core/helper/util/NumberFormat} config as {@link #config-format} to be able to show currency. For
 * example:
 *
 * ```javascript
 * new NumberField({
 *   format : {
 *      style    : 'currency',
 *      currency : 'USD'
 *   }
 * });
 * ```
 *
 * @extends Core/widget/Field
 * @classType numberfield
 * @inlineexample Core/widget/NumberField.js
 * @inputfield
 */
class NumberField extends Field {
  //region Config
  static get $name() {
    return 'NumberField';
  }
  // Factoryable type name
  static get type() {
    return 'numberfield';
  }
  // Factoryable type alias
  static get alias() {
    return 'number';
  }
  static get configurable() {
    return {
      /**
       * Min value
       * @config {Number}
       */
      min: null,
      /**
       * Max value
       * @config {Number}
       */
      max: null,
      /**
       * Step size for spin button clicks.
       * @member {Number} step
       */
      /**
       * Step size for spin button clicks. Also used when pressing up/down keys in the field.
       * @config {Number}
       * @default
       */
      step: 1,
      /**
       * Large step size, defaults to 10 * `step`. Applied when pressing SHIFT and stepping either by click or
       * using keyboard.
       * @config {Number}
       * @default 10
       */
      largeStep: 0,
      /**
       * Initial value
       * @config {Number}
       */
      value: null,
      /**
       * The format to use for rendering numbers.
       *
       * For example:
       * ```
       *  format: '9,999.00##'
       * ```
       * The above enables digit grouping and will display at least 2 (but no more than 4) fractional digits.
       * @config {String|NumberFormatConfig}
       * @default
       */
      format: '',
      /**
       * The number of decimal places to allow. Defaults to no constraint.
       *
       * This config has been replaced by {@link #config-format}. Instead of this:
       *```
       *  decimalPrecision : 3
       *```
       * Use `format`:
       *```
       *  format : '9.###'
       *```
       * To set both `decimalPrecision` and `leadingZeroes` (say to `3`), do this:
       *```
       *  format : '3>9.###'
       *```
       * @config {Number}
       * @default
       * @deprecated Since 3.1. Use {@link #config-format} instead.
       */
      decimalPrecision: null,
      /**
       * The maximum number of leading zeroes to show. Defaults to no constraint.
       *
       * This config has been replaced by {@link #config-format}. Instead of this:
       *```
       *  leadingZeros : 3
       *```
       * Use `format`:
       *```
       *  format : '3>9'
       *```
       * To set both `leadingZeroes` and `decimalPrecision` (say to `2`), do this:
       *```
       *  format : '3>9.##'
       *```
       * @config {Number}
       * @default
       * @deprecated Since 3.1. Use {@link #config-format} instead.
       */
      leadingZeroes: null,
      triggers: {
        spin: {
          type: 'spintrigger'
        }
      },
      /**
       * Controls how change events are triggered when stepping the value up or down using either spinners or
       * arrow keys.
       *
       * Configure with:
       * * `true` to trigger a change event per step
       * * `false` to not trigger change while stepping. Will trigger on blur/Enter
       * * A number of milliseconds to buffer the change event, triggering when no steps are performed during that
       *   period of time.
       *
       * @config {Boolean|Number}
       * @default
       */
      changeOnSpin: true,
      // NOTE: using type="number" has several trade-offs:
      //
      // Negatives:
      //   - No access to caretPos/textSelection. This causes anomalies when replacing
      //     the input value with a formatted version of that value (the caret moves to
      //     the end of the input el on each character typed).
      //   - The above also prevents Siesta/synthetic events from mimicking typing.
      //   - Thousand separators cannot be displayed (input.value = '1,000' throws an
      //     exception).
      // Positives:
      //   - On mobile, the virtual keyboard only shows digits et al.
      //   - validity property on DOM node that handles min/max checks.
      //
      // The above may not be exhaustive, but there is not a compelling reason to
      // use type="number" except on mobile.
      /**
       * This can be set to `'number'` to enable the numeric virtual keyboard on
       * mobile devices. Doing so limits this component's ability to handle keystrokes
       * and format properly as the user types, so this is not recommended for
       * desktop applications. This will also limit similar features of automated
       * testing tools that mimic user input.
       * @config {String}
       * @default text
       */
      inputType: null
    };
  }
  //endregion
  //region Init
  construct(config) {
    super.construct(config);
    const me = this;
    // Support for selecting all by double click in empty input area
    // Browsers work differently at this case
    me.input.addEventListener('dblclick', () => {
      me.select();
    });
    if (typeof me.changeOnSpin === 'number') {
      me.bufferedSpinChange = me.buffer(me.triggerChange, me.changeOnSpin);
    }
  }
  //endregion
  //region Internal functions
  acceptValue(value, rawValue) {
    let accept = !isNaN(value);
    // https://github.com/bryntum/support/issues/1349
    // Pass through if there is a text selection in the field. This fixes the case when
    // valid value is typed already and we are replacing it by selecting complete string and typing over it.
    // Cannot be tested in siesta, see ticket for more info.
    if (accept && !this.hasTextSelection) {
      accept = false;
      const raw = this.input.value,
        current = parseFloat(raw);
      if (raw !== rawValue) {
        // The new value is out of range, but we accept it if the current value
        // is also problematic. Consider the case where the input is empty and the
        // minimum value is 100. The user must first type "1" and we must accept it
        // if they are to get the opportunity to type the "0"s.
        accept = !this.acceptValue(current, raw);
        // Also, if we are checking the current value, be sure not to infinitely
        // recurse here.
      }
    }

    return accept;
  }
  okMax(value) {
    return isNaN(this.max) || value <= this.max;
  }
  okMin(value) {
    return isNaN(this.min) || value >= this.min;
  }
  internalOnKeyEvent(e) {
    if (e.type === 'keydown') {
      const me = this,
        key = e.key;
      let block;
      // Native arrow key spin behaviour differs between browsers, so we replace
      // the native spinners w/our own triggers and handle arrows keys as well.
      if (key === 'ArrowUp') {
        me.doSpinUp(e.shiftKey);
        block = true;
      } else if (key === 'ArrowDown') {
        me.doSpinDown(e.shiftKey);
        block = true;
      } else if (!e.altKey && !e.ctrlKey && key && key.length === 1) {
        // The key property contains the character or key name... so ignore
        // keys that aren't a single character.
        const after = me.getAfterValue(key),
          afterValue = me.formatter.parseStrict(after),
          // no need to check if typing same value or - if negative numbers are allowed
          accepted = afterValue === me.value || after === '-' && (isNaN(me.min) || me.min < 0);
        block = !accepted && !me.acceptValue(afterValue, after);
      }
      if (key === 'Enter' && me._changedBySilentSpin) {
        me.triggerChange(e, true);
        // reset the flag
        me._changedBySilentSpin = false;
      }
      if (block) {
        e.preventDefault();
      }
    }
    super.internalOnKeyEvent(e);
  }
  doSpinUp(largeStep = false) {
    const me = this;
    if (me.readOnly) {
      return;
    }
    let newValue = (me.value || 0) + (largeStep ? me.largeStep : me.step);
    if (!me.okMin(newValue)) {
      newValue = me.min;
    }
    if (me.okMax(newValue)) {
      me.applySpinChange(newValue);
    }
  }
  doSpinDown(largeStep = false) {
    const me = this;
    if (me.readOnly) {
      return;
    }
    let newValue = (me.value || 0) - (largeStep ? me.largeStep : me.step);
    if (!me.okMax(newValue)) {
      newValue = me.max;
    }
    if (me.okMin(newValue)) {
      me.applySpinChange(newValue);
    }
  }
  applySpinChange(newValue) {
    const me = this;
    me._isUserAction = true;
    // Should not trigger change immediately?
    if (me.changeOnSpin !== true) {
      me._changedBySilentSpin = true;
      // Silence the change
      me.silenceChange = true;
      // Optionally buffer the change
      me.bufferedSpinChange && me.bufferedSpinChange(null, true);
    }
    me.value = newValue;
    me._isUserAction = false;
    me.silenceChange = false;
  }
  triggerChange() {
    if (!this.silenceChange) {
      super.triggerChange(...arguments);
    }
  }
  onFocusOut(e) {
    var _me$triggers, _me$triggers$spin, _me$triggers$spin$cli;
    super.onFocusOut(...arguments);
    const me = this,
      {
        input
      } = me,
      raw = input.value,
      value = me.formatter.truncate(raw),
      formatted = isNaN(value) ? raw : me.formatValue(value);
    // Triggers may have been reconfigured
    (_me$triggers = me.triggers) === null || _me$triggers === void 0 ? void 0 : (_me$triggers$spin = _me$triggers.spin) === null || _me$triggers$spin === void 0 ? void 0 : (_me$triggers$spin$cli = _me$triggers$spin.clickRepeater) === null || _me$triggers$spin$cli === void 0 ? void 0 : _me$triggers$spin$cli.cancel();
    if (raw !== formatted) {
      input.value = formatted;
    }
    if (me._changedBySilentSpin) {
      me.triggerChange(e, true);
      // reset the flag
      me._changedBySilentSpin = false;
    }
  }
  internalOnInput(event) {
    const me = this,
      {
        formatter,
        input
      } = me,
      {
        parser
      } = formatter,
      raw = input.value,
      decimals = parser.decimalPlaces(raw);
    if (formatter.truncator && decimals) {
      let value = raw;
      const trunc = formatter.truncate(raw);
      if (!isNaN(trunc)) {
        value = me.formatValue(trunc);
        if (parser.decimalPlaces(value) < decimals) {
          // If typing has caused truncation or rounding, reset. To best preserve
          // the caret pos (which is reset by assigning input.value), we grab and
          // restore the distance from the end. This allows special things to format
          // into the string (such as thousands separators) since they always go to
          // the front of the input.
          const pos = raw.length - me.caretPos;
          input.value = value;
          me.caretPos = value.length - pos;
        }
      }
    }
    super.internalOnInput(event);
  }
  formatValue(value) {
    return this.formatter.format(value);
  }
  changeFormat(format) {
    const me = this;
    if (format === '') {
      const {
        leadingZeroes,
        decimalPrecision
      } = me;
      format = leadingZeroes ? `${leadingZeroes}>9` : null;
      if (decimalPrecision != null) {
        format = `${format || ''}9.${'#'.repeat(decimalPrecision)}`;
      } else if (format) {
        // When we only have leadingZeroes, we'll have a format like "4>9", but
        // that will default to 3 decimal digits. Prior behavior implied no limit
        // on decimal digits unless decimalPrecision was specified.
        format += '.*';
      }
    }
    return format;
  }
  get formatter() {
    const me = this,
      format = me.format;
    let formatter = me._formatter;
    if (!formatter || me._lastFormat !== format) {
      formatter = NumberFormat.get(me._lastFormat = format);
      me._formatter = formatter;
    }
    return formatter;
  }
  //endregion
  //region Getters/Setters
  updateStep(step) {
    this.element.classList[step ? 'remove' : 'add']('b-hide-spinner');
    this._step = step;
  }
  changeLargeStep(largeStep) {
    return largeStep || this.step * 10;
  }
  get validity() {
    const value = this.value,
      validity = {};
    // Assert range for non-empty fields, empty fields will turn invalid if `required: true`
    if (value != null) {
      validity.rangeUnderflow = !this.okMin(value);
      validity.rangeOverflow = !this.okMax(value);
    }
    validity.valid = !validity.rangeUnderflow && !validity.rangeOverflow;
    return validity;
  }
  /**
   * Get/set the NumberField's value, or `undefined` if the input field is empty
   * @property {Number}
   */
  changeValue(value, was) {
    const me = this;
    if (value || value === 0) {
      let valueIsNaN;
      // We insist on a number as the value
      if (typeof value !== 'number') {
        value = typeof value === 'string' ? me.formatter.parse(value) : Number(value);
        valueIsNaN = isNaN(value);
        if (valueIsNaN) {
          value = '';
        }
      }
      if (!valueIsNaN && me.format) {
        value = me.formatter.round(value);
      }
    } else {
      value = undefined;
    }
    return super.changeValue(value, was);
  }
  get inputValue() {
    let value = this.value;
    if (value != null && this.format) {
      value = this.formatValue(value);
    }
    return value;
  }
  //endregion
}
// Register this widget type with its Factory
NumberField.initClass();
NumberField._$name = 'NumberField';

/**
 * @module Core/widget/TimePicker
 */
/**
 * A Container which displays hour and minute number fields and AM/PM switcher buttons for 12 hour time format.
 *
 * ```javascript
 * new TimeField({
 *     label     : 'Time field',
 *     appendTo  : document.body,
 *     // Configure the time picker
 *     picker    : {
 *         items : {
 *             minute : {
 *                 step : 5
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ## Contained widgets
 *
 * The default widgets contained in this picker are:
 *
 * | Widget ref | Type                                        | Description      |
 * |------------|---------------------------------------------|------------------|
 * | `hour`     | {@link Core.widget.NumberField NumberField} | The hour field   |
 * | `minute`   | {@link Core.widget.NumberField NumberField} | The minute field |
 * | `second`   | {@link Core.widget.NumberField NumberField} | The second field |
 * | `amButton` | {@link Core.widget.Button Button}           | The am button    |
 * | `pmButton` | {@link Core.widget.Button Button}           | The pm button    |
 *
 * This class is not intended for use in applications. It is used internally by the {@link Core.widget.TimeField} class.
 *
 * @classType timepicker
 * @extends Core/widget/Container
 * @widget
 */
class TimePicker extends Container {
  //region Config
  static get $name() {
    return 'TimePicker';
  }
  // Factoryable type name
  static get type() {
    return 'timepicker';
  }
  static configurable = {
    floating: true,
    items: {
      hour: {
        label: 'L{TimePicker.hour}',
        type: 'number',
        min: 0,
        max: 23,
        highlightExternalChange: false,
        format: '2>9'
      },
      minute: {
        label: 'L{TimePicker.minute}',
        type: 'number',
        min: 0,
        max: 59,
        highlightExternalChange: false,
        format: '2>9'
      },
      second: {
        hidden: true,
        label: 'L{TimePicker.second}',
        type: 'number',
        min: 0,
        max: 59,
        highlightExternalChange: false,
        format: '2>9'
      },
      amButton: {
        type: 'button',
        text: 'AM',
        toggleGroup: 'am-pm',
        cls: 'b-blue',
        onClick: 'up.onAmPmButtonClick'
      },
      pmButton: {
        type: 'button',
        text: 'PM',
        toggleGroup: 'am-pm',
        cls: 'b-blue',
        onClick: 'up.onAmPmButtonClick'
      }
    },
    autoShow: false,
    trapFocus: true,
    /**
     * By default the seconds field is not displayed. If you require seconds to be visible,
     * configure this as `true`
     * @config {Boolean}
     * @default false
     */
    seconds: null,
    /**
     * Time value, which can be a Date or a string. If a string is specified, it will be converted using the
     * specified {@link #config-format}
     * @prp {Date}
     * @accepts {Date|String}
     */
    value: {
      $config: {
        equal: 'date'
      },
      value: null
    },
    /**
     * Time format. Used to set appropriate 12/24 hour format to display.
     * See {@link Core.helper.DateHelper#function-format-static DateHelper} for formatting options.
     * @prp {String}
     */
    format: null,
    /**
     * Max value, which can be a Date or a string. If a string is specified, it will be converted using the
     * specified {@link #config-format}
     * @prp {Date}
     * @accepts {Date|String}
     */
    max: null,
    /**
     * Min value, which can be a Date or a string. If a string is specified, it will be converted using the
     * specified {@link #config-format}
     * @prp {Date}
     * @accepts {Date|String}
     */
    min: null,
    /**
     * Initial value, which can be a Date or a string. If a string is specified, it will be converted using the
     * specified {@link #config-format}. Initial value is restored on Escape click
     * @member {Date} initialValue
     * @accepts {Date|String}
     */
    initialValue: null // Not documented as config on purpose, API was that way
  };
  //endregion
  //region Init
  construct(config) {
    super.construct(config);
    this.refresh();
  }
  updateSeconds(seconds) {
    this.widgetMap.second[seconds ? 'show' : 'hide']();
  }
  //endregion
  //region Event listeners
  // Automatically called by Widget's triggerFieldChange which announces changes to all ancestors
  onFieldChange() {
    if (!this.isConfiguring && !this.isRefreshing) {
      this.value = this.pickerToTime();
    }
  }
  onAmPmButtonClick({
    source
  }) {
    this._pm = source.ref === 'pmButton';
    if (this._value) {
      this.value = this.pickerToTime();
    }
  }
  onInternalKeyDown(keyEvent) {
    var _super$onInternalKeyD;
    const me = this;
    switch (keyEvent.key) {
      case 'Escape':
        // Support for undefined initial time
        me.triggerTimeChange(me._initialValue);
        me.hide();
        keyEvent.preventDefault();
        return;
      case 'Enter':
        me.value = me.pickerToTime();
        me.hide();
        keyEvent.preventDefault();
        return;
    }
    (_super$onInternalKeyD = super.onInternalKeyDown) === null || _super$onInternalKeyD === void 0 ? void 0 : _super$onInternalKeyD.call(this, keyEvent);
  }
  //endregion
  //region Internal functions
  pickerToTime() {
    const me = this,
      pm = me._pm,
      {
        hour,
        minute,
        second
      } = me.widgetMap;
    hour.format = me._is24Hour ? '2>9' : null;
    let hours = hour.value,
      newValue = new Date(me.value);
    if (!me._is24Hour) {
      if (pm && hours < 12) hours = hours + 12;
      if (!pm && hours === 12) hours = 0;
    }
    newValue.setHours(hours);
    newValue.setMinutes(minute.value);
    if (me.seconds) {
      newValue.setSeconds(second.value);
    }
    if (me._min) {
      newValue = DateHelper.max(me._min, newValue);
    }
    if (me._max) {
      newValue = DateHelper.min(me._max, newValue);
    }
    return newValue;
  }
  triggerTimeChange(time) {
    /**
     * Fires when a time is changed.
     * @event timeChange
     * @param {Date} time The selected time.
     */
    this.trigger('timeChange', {
      time
    });
  }
  //endregion
  //region Getters / Setters
  updateInitialValue(initialValue) {
    this.value = initialValue;
  }
  changeValue(value) {
    if (value) {
      value = typeof value === 'string' ? DateHelper.parse(value, this.format) : value;
    }
    if (!this.isVisible) {
      this._initialValue = value;
    }
    return value ?? DateHelper.getTime(0);
  }
  updateValue(value) {
    if (this.isVisible) {
      this.triggerTimeChange(value);
    }
    this.refresh();
  }
  updateFormat(format) {
    this._is24Hour = DateHelper.is24HourFormat(format);
    this.refresh();
  }
  changeMin(min) {
    return typeof min === 'string' ? DateHelper.parse(min, this.format) : min;
  }
  changeMax(max) {
    return typeof max === 'string' ? DateHelper.parse(max, this.format) : max;
  }
  //endregion
  //region Display
  refresh() {
    const me = this;
    if (!me.isConfiguring && me.value) {
      me.isRefreshing = true;
      const {
          hour,
          minute,
          second,
          amButton,
          pmButton
        } = me.widgetMap,
        time = me.value,
        is24 = me._is24Hour,
        hours = time.getHours(),
        pm = me._pm = hours >= 12;
      me.element.classList[is24 ? 'add' : 'remove']('b-24h');
      hour.min = is24 ? 0 : 1;
      hour.max = is24 ? 23 : 12;
      hour.value = is24 ? hours : hours % 12 || 12;
      minute.value = time.getMinutes();
      second.value = time.getSeconds();
      amButton.pressed = !pm;
      pmButton.pressed = pm;
      amButton.hidden = pmButton.hidden = is24;
      me.isRefreshing = false;
    }
  }
  //endregion
}
// Register this widget type with its Factory
TimePicker.initClass();
TimePicker._$name = 'TimePicker';

/**
 * @module Core/widget/TimeField
 */
/**
 * The time field widget is a text input field with a time picker drop down. It shows left/right arrows to increase or
 * decrease time by the {@link #config-step step value}.
 *
 * This field can be used as an {@link Grid.column.Column#config-editor editor} for the {@link Grid.column.Column Column}.
 * It is used as the default editor for the {@link Grid.column.TimeColumn TimeColumn}.
 *
 * ## Configuring the picker hour / minute fields
 *
 * You can easily configure the fields in the drop-down picker, to control the increment of the up/down step arrows:
 *
 * ```javascript
 * new TimeField({
 *     label     : 'Time field',
 *     appendTo  : document.body,
 *     picker    : {
 *         items : {
 *             minute : {
 *                 step : 5
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * This widget may be operated using the keyboard. `ArrowDown` opens the time picker, which itself
 * is keyboard navigable. `Shift+ArrowDown` activates the {@link #config-step} back trigger.
 * `Shift+ArrowUp` activates the {@link #config-step} forwards trigger.
 *
 * @extends Core/widget/PickerField
 *
 * @example
 * let field = new TimeField({
 *   format: 'HH'
 * });
 *
 * @classType timefield
 * @inlineexample Core/widget/TimeField.js
 * @inputfield
 */
class TimeField extends PickerField {
  //region Config
  static get $name() {
    return 'TimeField';
  }
  // Factoryable type name
  static get type() {
    return 'timefield';
  }
  // Factoryable type alias
  static get alias() {
    return 'time';
  }
  static get configurable() {
    return {
      picker: {
        type: 'timepicker',
        align: {
          align: 't0-b0',
          axisLock: true
        }
      },
      /**
       * Get/Set format for time displayed in field (see {@link Core.helper.DateHelper#function-format-static}
       * for formatting options).
       * @member {String} format
       */
      /**
       * Format for date displayed in field (see Core.helper.DateHelper#function-format-static for formatting
       * options).
       * @config {String}
       * @default
       */
      format: 'LT',
      triggers: {
        expand: {
          align: 'end',
          handler: 'onTriggerClick',
          compose: () => ({
            children: [{
              class: {
                'b-icon-clock-live': 1
              }
            }]
          })
        },
        back: {
          align: 'start',
          cls: 'b-icon b-icon-angle-left b-step-trigger',
          key: 'Shift+ArrowDown',
          handler: 'onBackClick'
        },
        forward: {
          align: 'end',
          cls: 'b-icon b-icon-angle-right b-step-trigger',
          key: 'Shift+ArrowUp',
          handler: 'onForwardClick'
        }
      },
      /**
       * Get/set min value, which can be a Date or a string. If a string is specified, it will be converted using
       * the specified {@link #config-format}.
       * @member {Date} min
       * @accepts {String|Date}
       */
      /**
       * Min time value
       * @config {String|Date}
       */
      min: null,
      /**
       * Get/set max value, which can be a Date or a string. If a string is specified, it will be converted using
       * the specified {@link #config-format}.
       * @member {Date} max
       * @accepts {String|Date}
       */
      /**
       * Max time value
       * @config {String|Date}
       */
      max: null,
      /**
       * The `step` property may be set in Object form specifying two properties, `magnitude`, a Number, and
       * `unit`, a String.
       *
       * If a Number is passed, the steps's current unit is used and just the magnitude is changed.
       *
       * If a String is passed, it is parsed by {@link Core.helper.DateHelper#function-parseDuration-static}, for
       * example `'5m'`, `'5 m'`, `'5 min'`, `'5 minutes'`.
       *
       * Upon read, the value is always returned in object form containing `magnitude` and `unit`.
       * @member {DurationConfig} step
       * @accepts {String|Number|DurationConfig}
       */
      /**
       * Time increment duration value. Defaults to 5 minutes.
       * The value is taken to be a string consisting of the numeric magnitude and the units.
       * The units may be a recognised unit abbreviation of this locale or the full local unit name.
       * For example `"10m"` or `"5min"` or `"2 hours"`
       * @config {String}
       */
      step: '5m',
      stepTriggers: null,
      /**
       * Get/set value, which can be a Date or a string. If a string is specified, it will be converted using the
       * specified {@link #config-format}.
       * @member {Date} value
       * @accepts {String|Date}
       */
      /**
       * Value, which can be a Date or a string. If a string is specified, it will be converted using the
       * specified {@link #config-format}
       * @config {String|Date}
       */
      value: null,
      /**
       * Set to true to not clean up the date part of the passed value. Set to false to reset the date part to
       * January 1st
       * @prp {Boolean}
       * @default
       */
      keepDate: false
    };
  }
  //endregion
  //region Init & destroy
  changePicker(picker, oldPicker) {
    const me = this;
    return TimePicker.reconfigure(oldPicker, picker, {
      owner: me,
      defaults: {
        value: me.value,
        forElement: me[me.pickerAlignElement],
        owner: me,
        align: {
          anchor: me.overlayAnchor,
          target: me[me.pickerAlignElement]
        },
        onTimeChange({
          time
        }) {
          me._isUserAction = true;
          me.value = time;
          me._isUserAction = false;
        }
      }
    });
  }
  //endregion
  //region Click listeners
  onBackClick() {
    const me = this,
      {
        min
      } = me;
    if (!me.readOnly && me.value) {
      const newValue = DateHelper.add(me.value, -1 * me.step.magnitude, me.step.unit);
      if (!min || min.getTime() <= newValue) {
        me._isUserAction = true;
        me.value = newValue;
        me._isUserAction = false;
      }
    }
  }
  onForwardClick() {
    const me = this,
      {
        max
      } = me;
    if (!me.readOnly && me.value) {
      const newValue = DateHelper.add(me.value, me.step.magnitude, me.step.unit);
      if (!max || max.getTime() >= newValue) {
        me._isUserAction = true;
        me.value = newValue;
        me._isUserAction = false;
      }
    }
  }
  //endregion
  // region Validation
  get isValid() {
    const me = this;
    me.clearError('L{Field.minimumValueViolation}', true);
    me.clearError('L{Field.maximumValueViolation}', true);
    let value = me.value;
    if (value) {
      value = value.getTime();
      if (me._min && me._min.getTime() > value) {
        me.setError('L{Field.minimumValueViolation}', true);
        return false;
      }
      if (me._max && me._max.getTime() < value) {
        me.setError('L{Field.maximumValueViolation}', true);
        return false;
      }
    }
    return super.isValid;
  }
  hasChanged(oldValue, newValue) {
    if (oldValue !== null && oldValue !== void 0 && oldValue.getTime && newValue !== null && newValue !== void 0 && newValue.getTime) {
      // Only care about the time part
      return oldValue.getHours() !== newValue.getHours() || oldValue.getMinutes() !== newValue.getMinutes() || oldValue.getSeconds() !== newValue.getSeconds() || oldValue.getMilliseconds() !== newValue.getMilliseconds();
    }
    return super.hasChanged(oldValue, newValue);
  }
  //endregion
  //region Toggle picker
  /**
   * Show picker
   */
  showPicker() {
    const me = this,
      {
        picker,
        value
      } = me;
    if (me.readOnly) {
      return;
    }
    picker.value = value;
    picker.format = me.format;
    picker.maxTime = me.max;
    picker.minTime = me.min;
    // If we had no value initially.
    if (!value) {
      me.value = picker.value;
    }
    super.showPicker(true);
  }
  onPickerShow() {
    var _this$pickerKeyDownRe;
    super.onPickerShow();
    // Remove PickerField key listener
    this.pickerKeyDownRemover = (_this$pickerKeyDownRe = this.pickerKeyDownRemover) === null || _this$pickerKeyDownRe === void 0 ? void 0 : _this$pickerKeyDownRe.call(this);
  }
  /**
   * Focus time picker
   */
  focusPicker() {
    this.picker.focus();
  }
  //endregion
  //region Getters/setters
  transformTimeValue(value) {
    if (value != null) {
      if (typeof value === 'string') {
        value = DateHelper.parse(value, this.format);
        if (this.keepDate) {
          value = DateHelper.copyTimeValues(new Date(this.value), value);
        }
      } else {
        value = new Date(value);
      }
      // We insist on a *valid* Time as the value
      if (DateHelper.isValidDate(value)) {
        if (!this.keepDate) {
          // Clear date part back to zero so that all we have is the time part of the epoch.
          return DateHelper.getTime(value);
        } else {
          return value;
        }
      }
    }
    return null;
  }
  changeMin(value) {
    return this.transformTimeValue(value);
  }
  updateMin(value) {
    const {
      input
    } = this;
    if (input) {
      if (value == null) {
        input.removeAttribute('min');
      } else {
        input.min = value;
      }
    }
    this.syncInvalid();
  }
  changeMax(value) {
    return this.transformTimeValue(value);
  }
  updateMax(value) {
    const {
      input
    } = this;
    if (input) {
      if (value == null) {
        input.removeAttribute('max');
      } else {
        input.max = value;
      }
    }
    this.syncInvalid();
  }
  changeValue(value, was) {
    const me = this,
      newValue = me.transformTimeValue(value);
    // A value we could not parse
    if (value && !newValue || me.isRequired && value === '') {
      // setError uses localization
      me.setError('L{invalidTime}');
      return;
    }
    me.clearError('L{invalidTime}');
    // Reject non-change
    if (me.hasChanged(was, newValue)) {
      return super.changeValue(newValue, was);
    }
    // But we must fix up the display in case it was an unparseable string
    // and the value therefore did not change.
    if (!me.inputting) {
      me.syncInputFieldValue(true);
    }
  }
  updateValue(value, was) {
    const {
      expand
    } = this.triggers;
    // This makes to clock icon show correct time
    if (expand && value) {
      expand.element.firstElementChild.style.animationDelay = -((value.getHours() * 60 + value.getMinutes()) / 10) + 's';
    }
    super.updateValue(value, was);
  }
  changeStep(value, was) {
    var _value, _value2;
    const type = typeof value;
    if (!value) {
      return null;
    }
    if (type === 'number') {
      value = {
        magnitude: Math.abs(value),
        unit: was ? was.unit : 'hour'
      };
    } else if (type === 'string') {
      value = DateHelper.parseDuration(value);
    }
    if ((_value = value) !== null && _value !== void 0 && _value.unit && (_value2 = value) !== null && _value2 !== void 0 && _value2.magnitude) {
      if (value.magnitude < 0) {
        value = {
          magnitude: -value.magnitude,
          // Math.abs
          unit: value.unit
        };
      }
      return value;
    }
  }
  updateStep(value) {
    // If a step is configured, show the steppers
    this.element.classList[value ? 'remove' : 'add']('b-no-steppers');
    this.syncInvalid();
  }
  updateFormat() {
    this.syncInputFieldValue(true);
  }
  get inputValue() {
    return DateHelper.format(this.value, this.format);
  }
  //endregion
  //region Localization
  updateLocalization() {
    super.updateLocalization();
    this.syncInputFieldValue(true);
  }
  //endregion
}
// Register this widget type with its Factory
TimeField.initClass();
TimeField._$name = 'TimeField';

/**
 * @module Core/widget/DurationField
 */
/**
 * A specialized field allowing a user to also specify duration unit when editing the duration value.
 *
 * This field can be used as an {@link Grid.column.Column#config-editor editor} for the {@link Grid.column.Column Column}.
 * It is used as the default editor for the `DurationColumn`.
 *
 * @extends Core/widget/TextField
 * @classType durationfield
 * @inlineexample Core/widget/DurationField.js
 * @inputfield
 */
class DurationField extends TextField {
  static get $name() {
    return 'DurationField';
  }
  // Factoryable type name
  static get type() {
    return 'durationfield';
  }
  // Factoryable type alias
  static get alias() {
    return 'duration';
  }
  static get defaultConfig() {
    return {
      /**
       * The `value` config may be set in Object form specifying two properties,
       * `magnitude`, a Number, and `unit`, a String.
       *
       * If a String is passed, it is parsed in accordance with current locale rules.
       * The string is taken to be the numeric magnitude, followed by whitespace, then an abbreviation, or name of
       * the unit.
       * @config {DurationConfig|String}
       * @category Common
       */
      value: null,
      /**
       * Step size for spin button clicks.
       * @config {Number}
       * @default
       * @category Common
       */
      step: 1,
      /**
       * The duration unit to use with the current magnitude value.
       * @config {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
       * @category Common
       */
      unit: null,
      defaultUnit: 'day',
      /**
       * The duration magnitude to use with the current unit value. Can be either an integer or a float value.
       * Both "," and "." are valid decimal separators.
       * @config {Number}
       * @category Common
       */
      magnitude: null,
      /**
       * When set to `true` the field will use short names of unit durations
       * (as returned by {@link Core.helper.DateHelper#function-getShortNameOfUnit-static}) when creating the
       * input field's display value.
       * @config {Boolean}
       * @category Common
       */
      useAbbreviation: false,
      /**
       * Set to `true` to allow negative duration
       * @config {Boolean}
       * @category Common
       */
      allowNegative: false,
      /**
       * The number of decimal places to allow. Defaults to no constraint.
       * @config {Number}
       * @default
       * @category Common
       */
      decimalPrecision: null,
      triggers: {
        spin: {
          type: 'spintrigger'
        }
      },
      nullValue: null
    };
  }
  /**
   * Fired when this field's value changes.
   * @event change
   * @param {Core.data.Duration} value - This field's value
   * @param {Core.data.Duration} oldValue - This field's previous value
   * @param {Boolean} valid - True if this field is in a valid state.
   * @param {Event} [event] - The triggering DOM event if any.
   * @param {Boolean} userAction - Triggered by user taking an action (`true`) or by setting a value (`false`)
   * @param {Core.widget.DurationField} source - This field
   */
  /**
   * User performed default action (typed into this field or hit the triggers).
   * @event action
   * @param {Core.data.Duration} value - This field's value
   * @param {Core.data.Duration} oldValue - This field's previous value
   * @param {Boolean} valid - True if this field is in a valid state.
   * @param {Event} [event] - The triggering DOM event if any.
   * @param {Boolean} userAction - Triggered by user taking an action (`true`) or by setting a value (`false`)
   * @param {Core.widget.DurationField} source - This field
   */
  static get configurable() {
    return {
      /**
       * Get/set the min value (e.g. 1d)
       * @member {String} min
       * @category Common
       */
      /**
       * Minimum duration value (e.g. 1d)
       * @config {String}
       * @category Common
       */
      min: null,
      /**
       * Get/set the max value
       * @member {String} max (e.g. 10d)
       * @category Common
       */
      /**
       * Max duration value (e.g. 10d)
       * @config {String}
       * @category Common
       */
      max: null,
      /**
       * Get/set the allowed units, e.g. "day,hour,year".
       * @member {String} allowedUnits
       * @category Common
       */
      /**
       * Comma-separated list of units to allow in this field, e.g. "day,hour,year". Leave blank to allow all
       * valid units (the default)
       * @config {String}
       * @category Common
       */
      allowedUnits: null
    };
  }
  changeMin(value) {
    return typeof value === 'string' ? new Duration(value) : value;
  }
  changeMax(value) {
    return typeof value === 'string' ? new Duration(value) : value;
  }
  changeAllowedUnits(units) {
    if (typeof units === 'string') {
      units = units.split(',');
    }
    return units;
  }
  updateAllowedUnits(units) {
    this.allowedUnitsRe = new RegExp(`(${units.join('|')})`, 'i');
  }
  get inputValue() {
    // Do not use the _value property. If called during configuration, this
    // will import the configured value from the config object.
    return this.value == null ? '' : this.calcValue(true).toString(this.useAbbreviation);
  }
  /**
   * Get/Set duration unit to use with the current magnitude value.
   * Valid values are:
   * - "millisecond" - Milliseconds
   * - "second" - Seconds
   * - "minute" - Minutes
   * - "hour" - Hours
   * - "day" - Days
   * - "week" - Weeks
   * - "month" - Months
   * - "quarter" - Quarters
   * - "year"- Years
   *
   * @property {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
   * @category Common
   */
  set unit(unit) {
    this._unit = unit;
    this.value = this.calcValue();
  }
  get unit() {
    return this._unit;
  }
  get unitWithDefault() {
    return this._unit || DurationField.defaultConfig.defaultUnit;
  }
  /**
   * Get/Set numeric magnitude `value` to use with the current unit value.
   * @property {Number}
   * @category Common
   */
  set magnitude(magnitude) {
    this.clearError('L{invalidUnit}');
    this._magnitude = magnitude;
    super.value = this.calcValue();
  }
  get magnitude() {
    return this._magnitude;
  }
  roundMagnitude(value) {
    return value && this.decimalPrecision != null ? ObjectHelper.round(value, this.decimalPrecision) : value;
  }
  get allowDecimals() {
    return this.decimalPrecision !== 0;
  }
  get isValid() {
    const me = this,
      isEmpty = me.value == null || me.value && me.value.magnitude == null;
    return super.isValid && (isEmpty && !me.required || !isEmpty && (me.allowNegative || me.value.magnitude >= 0));
  }
  internalOnChange(event) {
    const me = this,
      value = me.value,
      oldVal = me._lastValue;
    if (me.hasChanged(oldVal, value)) {
      me._lastValue = value;
      me.triggerFieldChange({
        value,
        event,
        userAction: true,
        valid: me.isValid
      });
    }
  }
  onFocusOut(e) {
    var _this$triggers, _this$triggers$spin, _this$triggers$spin$c;
    this.syncInputFieldValue(true);
    (_this$triggers = this.triggers) === null || _this$triggers === void 0 ? void 0 : (_this$triggers$spin = _this$triggers.spin) === null || _this$triggers$spin === void 0 ? void 0 : (_this$triggers$spin$c = _this$triggers$spin.clickRepeater) === null || _this$triggers$spin$c === void 0 ? void 0 : _this$triggers$spin$c.cancel();
    return super.onFocusOut(e);
  }
  /**
   * The `value` property may be set in Object form specifying two properties, `magnitude`, a Number, and `unit`, a
   * String.
   *
   * If a Number is passed, the field's current unit is used and just the magnitude is changed.
   *
   * If a String is passed, it is parsed in accordance with current locale rules. The string is taken to be the
   * numeric magnitude, followed by whitespace, then an abbreviation, or name of the unit.
   *
   * Upon read, the value is always a {@link Core.data.Duration} object containing `magnitude` and `unit`.
   *
   * @property {Core.data.Duration}
   * @accepts {String|Number|DurationConfig|Core.data.Duration}
   * @category Common
   */
  set value(value) {
    const me = this;
    let newMagnitude, newUnit;
    this.clearError('L{invalidUnit}');
    if (typeof value === 'number') {
      // A number means preserving existing unit value
      newMagnitude = value;
      newUnit = me._unit;
    } else if (typeof value === 'string') {
      if (/^\s*$/.test(value)) {
        // special case of "empty" (in user meaning) string - set to `null` to allow unscheduling of tasks
        newMagnitude = null;
      } else {
        // Parse as a string
        const parsedDuration = DateHelper.parseDuration(value, me.allowDecimals, me.unitWithDefault);
        if (parsedDuration) {
          if (!me.allowedUnitsRe || me.allowedUnitsRe.test(parsedDuration.unit)) {
            newUnit = parsedDuration.unit;
            newMagnitude = parsedDuration.magnitude;
          } else {
            me.setError('L{invalidUnit}');
          }
        }
      }
    } else {
      // Using value object with unit and magnitude
      if (value && 'unit' in value && 'magnitude' in value) {
        newUnit = value.unit;
        newMagnitude = value.magnitude;
      } else {
        newUnit = null;
        newMagnitude = null;
      }
    }
    if (me._magnitude !== newMagnitude || me._unit != newUnit) {
      me._magnitude = newMagnitude;
      // Once we have unit, do not clear it if setting clearing value
      if (newUnit) {
        me._unit = newUnit;
      }
      super.value = me.calcValue();
    }
  }
  okMax(value) {
    if (typeof value === 'number') {
      value = new Duration({
        unit: this.unitWithDefault,
        magnitude: value
      });
    }
    return this.max == null || value <= this.max;
  }
  okMin(value) {
    if (typeof value === 'number') {
      value = new Duration({
        unit: this.unitWithDefault,
        magnitude: value
      });
    }
    return this.min == null || value >= this.min;
  }
  get validity() {
    const value = this.value,
      validity = {};
    // Assert range for non-empty fields, empty fields will turn invalid if `required: true`
    if (value != null) {
      validity.rangeUnderflow = !this.okMin(value);
      validity.rangeOverflow = !this.okMax(value);
    }
    validity.valid = !validity.rangeUnderflow && !validity.rangeOverflow;
    return validity;
  }
  get value() {
    return super.value;
  }
  calcValue(round = false) {
    const me = this;
    if ((!me._unit || me._magnitude == null) && me.clearable) {
      return null;
    } else {
      return new Duration(round ? this.roundMagnitude(me._magnitude) : this._magnitude, me.unitWithDefault);
    }
  }
  hasChanged(oldValue, newValue) {
    return newValue && !oldValue || !newValue && oldValue || newValue && oldValue && !oldValue.isEqual(newValue);
  }
  /**
   * The `milliseconds` property is a read only property which returns the number of milliseconds in this field's
   * value
   * @member {Number} milliseconds
   * @readonly
   */
  get milliseconds() {
    // For reasons unknown documenting as @property did not work
    return this.value ? this.value.milliseconds : 0;
  }
  onInternalKeyDown(keyEvent) {
    if (keyEvent.key === 'ArrowUp') {
      this.doSpinUp();
    } else if (keyEvent.key === 'ArrowDown') {
      this.doSpinDown();
    }
  }
  doSpinUp() {
    const me = this;
    if (me.readOnly) {
      return;
    }
    let newValue = (me.magnitude || 0) + me.step;
    me._isUserAction = true;
    if (!me.okMin(newValue)) {
      newValue = me.min;
    }
    if (me.okMax(newValue)) {
      me.value = newValue;
    }
    me._isUserAction = false;
  }
  doSpinDown() {
    const me = this;
    if (me.readOnly) {
      return;
    }
    let newValue = (me.magnitude || 0) - me.step;
    if (!me.okMax(newValue)) {
      newValue = me.max;
    }
    if (me.okMin(newValue) && (me.allowNegative || (me.magnitude || 0) > 0)) {
      me._isUserAction = true;
      me.value = newValue;
      me._isUserAction = false;
    }
  }
}
// Register this widget type with its Factory
DurationField.initClass();
DurationField._$name = 'DurationField';

/**
 * @module Core/widget/FieldFilterPicker
 */
const SUPPORTED_FIELD_DATA_TYPES = ['number', 'boolean', 'string', 'date', 'duration'];
const isSupportedDurationField = field => {
  var _field$column;
  return ((_field$column = field.column) === null || _field$column === void 0 ? void 0 : _field$column.type) === 'duration';
};
const EMPTY_STRING = '';
/**
 * A field that is available for selection when defining filters.
 *
 * @typedef {Object} FieldOption
 * @property {'string'|'number'|'date'|'boolean'} type The data type of the values in this field in the data source
 * @property {String} title The human-friendly display name for the field, as might be displayed in a data column header
 */
/**
 * A dictionary of value field placeholder strings, keyed by data type.
 *
 * @typedef {Object} ValueFieldPlaceholders
 * @property {String} string Placeholder text for text value fields
 * @property {String} number Placeholder text for number value fields
 * @property {String} date Placeholder text for date value fields
 * @property {String} list Placeholder text for multi-select list values, e.g. for the 'is one of' operator
 */
/**
 * Widget for defining a {@link Core.util.CollectionFilter} for use in filtering a {@link Core.data.Store}.
 * Filters consist of `property` (the name of the data field whose values are checked), `operator`
 * (the type of comparison to use), and `value` (the pre-defined value against which to compare the
 * data field value during filtering).
 *
 * {@inlineexample Core/widget/FieldFilterPicker.js}
 *
 * For example:
 *
 * ```javascript
 * new FieldFilterPicker({
 *     appendTo : domElement,
 *
 *     fields: {
 *         // Allow filters to be defined against the 'age' and 'role' fields in our data
 *         age       : { title: 'Age', type: 'number' },
 *         startDate : { title: 'Start Date', type: 'date' }
 *     },
 *
 *     filter : {
 *         property : 'startDate',
 *         operator : '<',
 *         value    : new Date()
 *     }
 * });
 * ```
 *
 * @extends Core/widget/Container
 * @demo Grid/fieldfilters
 * @classtype fieldfilterpicker
 * @widget
 */
class FieldFilterPicker extends Container {
  //region Config
  static get $name() {
    return 'FieldFilterPicker';
  }
  // Factoryable type name
  static get type() {
    return 'fieldfilterpicker';
  }
  static operators = {
    empty: {
      value: 'empty',
      text: 'L{isEmpty}',
      argCount: 0
    },
    notEmpty: {
      value: 'notEmpty',
      text: 'L{isNotEmpty}',
      argCount: 0
    },
    '=': {
      value: '=',
      text: 'L{equals}'
    },
    '!=': {
      value: '!=',
      text: 'L{doesNotEqual}'
    },
    '>': {
      value: '>',
      text: 'L{isGreaterThan}'
    },
    '<': {
      value: '<',
      text: 'L{isLessThan}'
    },
    '>=': {
      value: '>=',
      text: 'L{isGreaterThanOrEqualTo}'
    },
    '<=': {
      value: '<=',
      text: 'L{isLessThanOrEqualTo}'
    },
    between: {
      value: 'between',
      text: 'L{isBetween}',
      argCount: 2
    },
    notBetween: {
      value: 'notBetween',
      text: 'L{isNotBetween}',
      argCount: 2
    },
    isIncludedIn: {
      value: 'isIncludedIn',
      text: 'L{isOneOf}'
    },
    isNotIncludedIn: {
      value: 'isNotIncludedIn',
      text: 'L{isNotOneOf}'
    }
  };
  static defaultOperators = {
    string: [
    // In display order
    this.operators.empty, this.operators.notEmpty, this.operators['='], this.operators['!='], {
      value: 'includes',
      text: 'L{contains}'
    }, {
      value: 'doesNotInclude',
      text: 'L{doesNotContain}'
    }, {
      value: 'startsWith',
      text: 'L{startsWith}'
    }, {
      value: 'endsWith',
      text: 'L{endsWith}'
    }, this.operators.isIncludedIn, this.operators.isNotIncludedIn],
    number: [this.operators.empty, this.operators.notEmpty, this.operators['='], this.operators['!='], this.operators['>'], this.operators['<'], this.operators['>='], this.operators['<='], this.operators.between, this.operators.notBetween, this.operators.isIncludedIn, this.operators.isNotIncludedIn],
    date: [this.operators.empty, this.operators.notEmpty, this.operators['='], this.operators['!='], {
      value: '<',
      text: 'L{isBefore}'
    }, {
      value: '>',
      text: 'L{isAfter}'
    }, this.operators.between, {
      value: 'isToday',
      text: 'L{isToday}',
      argCount: 0
    }, {
      value: 'isTomorrow',
      text: 'L{isTomorrow}',
      argCount: 0
    }, {
      value: 'isYesterday',
      text: 'L{isYesterday}',
      argCount: 0
    }, {
      value: 'isThisWeek',
      text: 'L{isThisWeek}',
      argCount: 0
    }, {
      value: 'isNextWeek',
      text: 'L{isNextWeek}',
      argCount: 0
    }, {
      value: 'isLastWeek',
      text: 'L{isLastWeek}',
      argCount: 0
    }, {
      value: 'isThisMonth',
      text: 'L{isThisMonth}',
      argCount: 0
    }, {
      value: 'isNextMonth',
      text: 'L{isNextMonth}',
      argCount: 0
    }, {
      value: 'isLastMonth',
      text: 'L{isLastMonth}',
      argCount: 0
    }, {
      value: 'isThisYear',
      text: 'L{isThisYear}',
      argCount: 0
    }, {
      value: 'isNextYear',
      text: 'L{isNextYear}',
      argCount: 0
    }, {
      value: 'isLastYear',
      text: 'L{isLastYear}',
      argCount: 0
    }, {
      value: 'isYearToDate',
      text: 'L{isYearToDate}',
      argCount: 0
    }, this.operators.isIncludedIn, this.operators.isNotIncludedIn],
    boolean: [{
      value: 'isTrue',
      text: 'L{isTrue}',
      argCount: 0
    }, {
      value: 'isFalse',
      text: 'L{isFalse}',
      argCount: 0
    }],
    duration: [this.operators.empty, this.operators.notEmpty, this.operators['='], this.operators['!='], this.operators['>'], this.operators['<'], this.operators['>='], this.operators['<='], this.operators.between, this.operators.notBetween, this.operators.isIncludedIn, this.operators.isNotIncludedIn],
    relation: [this.operators.isIncludedIn, this.operators.isNotIncludedIn]
  };
  static get defaultValueFieldPlaceholders() {
    return {
      string: 'L{enterAValue}',
      number: 'L{enterANumber}',
      date: 'L{selectADate}',
      list: 'L{selectOneOrMoreValues}',
      duration: 'L{enterAValue}'
    };
  }
  static configurable = {
    /**
     * Dictionary of {@link #typedef-FieldOption} representing the fields against which filters can be defined,
     * keyed by field name.
     *
     * <div class="note">5.3.0 Syntax accepting FieldOption[] was deprecated in favor of dictionary and will be removed in 6.0</div>
     *
     * If filtering a {@link Grid.view.Grid}, consider using {@link Grid.widget.GridFieldFilterPicker}, which can be configured
     * with an existing {@link Grid.view.Grid} instead of, or in combination with, defining fields manually.
     *
     * Example:
     * ```javascript
     * fields: {
     *     // Allow filters to be defined against the 'age' and 'role' fields in our data
     *     age  : { title: 'Age', type: 'number' },
     *     role : { title: 'Role', type: 'string' }
     * }
     * ```
     *
     * @config {Object<String,FieldOption>}
     */
    fields: null,
    /**
     * Make the entire picker disabled.
     *
     * @config {Boolean}
     * @default
     */
    disabled: false,
    /**
     * Make the entire picker read-only.
     *
     * @config {Boolean}
     * @default
     */
    readOnly: false,
    layout: 'vbox',
    /**
     * Make only the property selector readOnly.
     * @private
     *
     * @config {Boolean}
     * @default
     */
    propertyLocked: false,
    /**
     * Make only the operator selector readOnly.
     * @private
     *
     * @config {Boolean}
     * @default
     */
    operatorLocked: false,
    /**
     * Configuration object for the {@link Core.util.CollectionFilter} displayed
     * and editable in this picker.
     *
     * Example:
     *
     * ```javascript
     * {
     *     property: 'age',
     *     operator: '=',
     *     value: 25
     * }
     * ```
     *
     * @config {CollectionFilterConfig}
     */
    filter: null,
    /**
     * Optional configuration for the property selector {@link Core.widget.Combo}.
     *
     * @config {ComboConfig}
     */
    propertyFieldConfig: null,
    /**
     * Optional configuration for the operator selector {@link Core.widget.Combo}.
     *
     * @config {ComboConfig}
     * @private
     */
    operatorFieldConfig: null,
    /**
     * Optional CSS class to apply to the value field(s).
     *
     * @config {String}
     * @private
     */
    valueFieldCls: null,
    /**
     * @private
     */
    items: {
      propertyAndOperator: {
        type: 'container',
        layout: 'box',
        items: {
          propertyPicker: {
            type: 'combo',
            items: {},
            cls: `b-${FieldFilterPicker.type}-property`,
            placeholder: 'L{FieldFilterPicker.selectAProperty}'
          },
          operatorPicker: {
            type: 'combo',
            items: {},
            cls: `b-${FieldFilterPicker.type}-operator`,
            placeholder: 'L{FieldFilterPicker.selectAnOperator}'
          }
        }
      },
      valueFieldRow: {
        type: 'container',
        cls: `b-${FieldFilterPicker.type}-values`,
        items: {}
      },
      caseSensitive: {
        type: 'checkbox',
        text: 'L{FieldFilterPicker.caseSensitive}',
        cls: `b-${FieldFilterPicker.type}-case-sensitive`
      }
    },
    /**
     * Overrides the built-in list of operators that are available for selection. Specify operators as
     * an object with data types as keys and lists of operators as values, like this:
     *
     * ```javascript
     * operators : {
     *     string : [
     *         { value : 'empty', text : 'is empty', argCount : 0 },
     *         { value : 'notEmpty', text : 'is not empty', argCount : 0 }
     *     ],
     *     number : [
     *         { value : '=', text : 'equals' },
     *         { value : '!=', text : 'does not equal' }
     *     ],
     *     date : [
     *         { value : '<', text : 'is before' }
     *     ]
     * }
     * ```
     *
     * Here `value` is what will be stored in the `operator` field in the filter when selected, `text` is the text
     * displayed in the Combo for selection, and `argCount` is the number of arguments (comparison values) the
     * operator requires. The default argCount if not specified is 1.
     *
     * @config {Object}
     */
    operators: FieldFilterPicker.defaultOperators,
    /**
     * The date format string used to display dates when using the 'is one of' / 'is not one of' operators with a date
     * field. Defaults to the current locale's `FieldFilterPicker.dateFormat` value.
     *
     * @config {String}
     * @default
     */
    dateFormat: 'L{FieldFilterPicker.dateFormat}',
    /**
     * Optional {Core.data.Store} against which filters are being defined. This is used to supply options to filter against
     * when using the 'is one of' and 'is not one of' operators.
     *
     * @config {Core.data.Store}
     */
    store: null,
    /**
     * Optional {@link ValueFieldPlaceholders} object specifying custom placeholder text for value input fields.
     *
     * @config {ValueFieldPlaceholders}
     */
    valueFieldPlaceholders: null,
    /**
     * Optional function that modifies the configuration of value fields shown for a filter. The default configuration
     * is received as an argument and the returned value will be used as the final configuration. For example:
     *
     * ```javascript
     * getValueFieldConfig : (filter, fieldConfig) => {
     *     return {
     *         ...fieldConfig,
     *         title : fieldName    // Override the `title` config for the field
     *     };
     * }
     * ```
     *
     * The supplied function should accept the following arguments:
     *
     * @param {Core.util.CollectionFilter} filter The filter being displayed
     * @param {ContainerItemConfig} fieldConfig Configuration object for the value field
     *
     * @config {Function}
     */
    getValueFieldConfig: null
  };
  //endregion
  // Make lookup of operator arity (arg count) by [fieldType][operator]
  static buildOperatorArgCountLookup = operators => ArrayHelper.keyBy(Object.entries(operators), ([fieldType]) => fieldType, ([, operators]) => ArrayHelper.keyBy(operators, ({
    value
  }) => value, ({
    argCount
  }) => argCount === undefined ? 1 : argCount));
  afterConstruct() {
    const me = this;
    if (!me._fields) {
      throw new Error(`${FieldFilterPicker.name} requires 'fields' to be configured.`);
    }
    if (!me._filter) {
      throw new Error(`${FieldFilterPicker.name} requires 'filter' to be configured.`);
    }
    super.afterConstruct();
    const {
      widgetMap: {
        propertyPicker,
        operatorPicker,
        caseSensitive
      }
    } = me;
    propertyPicker.ion({
      select: 'onPropertySelect',
      thisObj: me
    });
    operatorPicker.ion({
      select: 'onOperatorSelect',
      thisObj: me
    });
    caseSensitive.ion({
      change: 'onCaseSensitiveChange',
      thisObj: me
    });
    me.propertyFieldConfig && propertyPicker.setConfig(me.propertyFieldConfig);
    me.operatorFieldConfig && operatorPicker.setConfig(me.operatorFieldConfig);
    propertyPicker.cls = me.allPropertyPickerClasses;
    operatorPicker.cls = me.allOperatorPickerClasses;
    me.populateUIFromFilter();
  }
  changeDateFormat(dateFormat) {
    return this.L(dateFormat);
  }
  get allChildInputs() {
    const {
      propertyPicker,
      operatorPicker,
      caseSensitive
    } = this.widgetMap;
    return [propertyPicker, operatorPicker, ...this.valueFields, caseSensitive];
  }
  updateDisabled(newDisabled) {
    this.allChildInputs.forEach(field => field.disabled = newDisabled);
  }
  updateReadOnly(newReadOnly) {
    const {
      propertyPicker,
      operatorPicker
    } = this.widgetMap;
    this.allChildInputs.forEach(field => field.readOnly = newReadOnly);
    // Respect these individual configs when un-setting global readOnly
    propertyPicker.readOnly = propertyPicker.readOnly || newReadOnly;
    operatorPicker.readOnly = operatorPicker.readOnly || newReadOnly;
  }
  updatePropertyLocked(newPropertyLocked) {
    this.widgetMap.propertyPicker.readOnly = newPropertyLocked || this.readOnly;
    this.widgetMap.propertyPicker.cls = this.allPropertyPickerClasses;
  }
  updateOperatorLocked(newOperatorLocked) {
    this.widgetMap.operatorPicker.readOnly = newOperatorLocked || this.readOnly;
    this.widgetMap.operatorPicker.cls = this.allOperatorPickerClasses;
  }
  changeOperators(newOperators) {
    const operators = newOperators ?? FieldFilterPicker.defaultOperators;
    return Object.keys(operators).reduce((outOperators, dataType) => ({
      ...outOperators,
      [dataType]: operators[dataType].map(op => ({
        ...op,
        text: this.L(op.text)
      }))
    }), {});
  }
  changeFields(newFields) {
    let fields = newFields;
    if (Array.isArray(newFields)) {
      VersionHelper.deprecate('Core', '6.0.0', 'FieldOption[] deprecated, use Object<String, FieldOption> keyed by field name instead');
      // Support old array syntax for `fields` during deprecation
      fields = ArrayHelper.keyBy(fields, 'name');
    }
    return fields;
  }
  get isMultiSelectValueField() {
    var _this$_filter;
    return ['isIncludedIn', 'isNotIncludedIn'].includes((_this$_filter = this._filter) === null || _this$_filter === void 0 ? void 0 : _this$_filter.operator);
  }
  get allPropertyPickerClasses() {
    var _this$propertyFieldCo;
    return new DomClassList(`b-${FieldFilterPicker.type}-property`, (_this$propertyFieldCo = this.propertyFieldConfig) === null || _this$propertyFieldCo === void 0 ? void 0 : _this$propertyFieldCo.cls, {
      [`b-${FieldFilterPicker.type}-combo-locked`]: this.propertyLocked
    });
  }
  get allOperatorPickerClasses() {
    var _this$operatorFieldCo;
    return new DomClassList(`b-${FieldFilterPicker.type}-operator`, (_this$operatorFieldCo = this.operatorFieldConfig) === null || _this$operatorFieldCo === void 0 ? void 0 : _this$operatorFieldCo.cls, {
      [`b-${FieldFilterPicker.type}-combo-locked`]: this.operatorLocked
    });
  }
  getValueFieldConfigs() {
    const me = this,
      {
        valueFieldCls,
        fieldType,
        _filter: {
          operator
        },
        onValueChange,
        filterValues,
        isMultiSelectValueField,
        operatorArgCount,
        getValueFieldConfig
      } = me,
      valueFieldPlaceholders = ObjectHelper.merge({}, FieldFilterPicker.defaultValueFieldPlaceholders, me.valueFieldPlaceholders);
    if (!fieldType || !operator || operatorArgCount === 0) {
      return [];
    }
    let valueFieldCfg = {
      type: 'textfield',
      // replaced as needed below
      internalListeners: {
        change: onValueChange,
        input: onValueChange,
        thisObj: me
      },
      cls: valueFieldCls,
      dataset: {
        type: fieldType
      },
      placeholder: me.L(valueFieldPlaceholders[isMultiSelectValueField ? 'list' : fieldType])
    };
    if (isMultiSelectValueField) {
      valueFieldCfg = {
        ...valueFieldCfg,
        type: 'combo',
        multiSelect: true,
        createOnUnmatched: true,
        items: this.getUniqueDataValues(filterValues),
        value: filterValues ?? []
      };
    } else if (['number', 'date', 'boolean'].includes(fieldType)) {
      valueFieldCfg.type = `${fieldType}field`;
    } else if (fieldType === 'duration') {
      valueFieldCfg.type = 'durationfield';
    }
    // Allow client to modify value field config
    if (getValueFieldConfig) {
      valueFieldCfg = me.callback(getValueFieldConfig, me, [me.filter, valueFieldCfg]);
    }
    if (isMultiSelectValueField) {
      // We only support a single multi-select Combo for now
      return [valueFieldCfg];
    }
    return ArrayHelper.populate(operatorArgCount, index => [{
      type: 'widget',
      tag: 'div',
      cls: `b-${FieldFilterPicker.type}-value-separator`,
      content: me.L('L{FieldFilterPicker.and}')
    }, {
      ...valueFieldCfg,
      value: filterValues[index]
    }]).flat().slice(1);
  }
  /**
   * Return an array of unique values in the data store for the currently selected field. If no store is
   * configured or no field is selected, returns an empty array.
   */
  getUniqueDataValues(extraValuesToInclude = []) {
    var _me$_filter;
    const me = this,
      {
        fieldType
      } = me;
    if (!me.store || !((_me$_filter = me._filter) !== null && _me$_filter !== void 0 && _me$_filter.property)) {
      return [];
    }
    const {
      relatedDisplayField
    } = me.selectedField;
    let values, sortedValues;
    if (me.fieldIsRelation) {
      const {
        foreignStore
      } = me.currentPropertyRelationConfig;
      if (relatedDisplayField) {
        // Display field specified -- sort by text label;
        // this bypasses the type-specific sorting for raw data values below
        values = foreignStore.allRecords.reduce((options, record) => {
          if (record.id != null) {
            options.push({
              text: record[relatedDisplayField],
              value: record.id
            });
          }
          return options;
        }, []);
        // Currently only support getting text from remote field and sorting as text
        sortedValues = values.sort((a, b) => me.sortStrings(a.text, b.text));
      } else {
        // If no display field, fall back to only data values
        values = foreignStore.allRecords.map(record => record.id);
      }
    } else {
      // Local data field
      values = me.store.allRecords.map(record => record[me._filter.property]);
    }
    if (!sortedValues) {
      values.push(...extraValuesToInclude);
      const uniqueValues = ArrayHelper.unique(values.reduce((primitiveValues, value) => {
        if (value != null && String(value).trim() !== '') {
          // Get primitive values from complex types, for deduping
          if (fieldType === 'date') {
            primitiveValues.push(value.valueOf());
          } else if (fieldType === 'duration') {
            primitiveValues.push(value.toString());
          } else {
            primitiveValues.push(value);
          }
        }
        return primitiveValues;
      }, []));
      // Sort
      if (fieldType === 'string') {
        sortedValues = uniqueValues.sort(me.sortStrings);
      } else if (fieldType === 'duration') {
        sortedValues = uniqueValues.map(durationStr => new Duration(durationStr)).filter(duration => duration.isValid).sort(me.sortDurations);
      } else {
        sortedValues = uniqueValues.sort(me.sortNumerics);
      }
      // Provide labels for complex value types
      if (fieldType === 'date') {
        sortedValues = sortedValues.map(timestamp => {
          const date = new Date(timestamp);
          return {
            text: DateHelper.format(date, me.dateFormat),
            value: timestamp
          };
        });
      } else if (fieldType === 'duration') {
        sortedValues = sortedValues.map(duration => duration.toString());
      }
    }
    return sortedValues;
  }
  sortStrings(a, b) {
    return (a ?? EMPTY_STRING).localeCompare(b ?? EMPTY_STRING);
  }
  sortNumerics(a, b) {
    return a - b;
  }
  sortDurations(a, b) {
    return a.valueOf() - b.valueOf();
  }
  get fieldType() {
    var _this$selectedField;
    return (_this$selectedField = this.selectedField) === null || _this$selectedField === void 0 ? void 0 : _this$selectedField.type;
  }
  get selectedField() {
    var _this$fields, _this$_filter2;
    return (_this$fields = this.fields) === null || _this$fields === void 0 ? void 0 : _this$fields[(_this$_filter2 = this._filter) === null || _this$_filter2 === void 0 ? void 0 : _this$_filter2.property];
  }
  get propertyOptions() {
    return Object.entries(this.fields ?? {}).filter(([, fieldDef]) => SUPPORTED_FIELD_DATA_TYPES.includes(fieldDef.type) || isSupportedDurationField(fieldDef)).map(([fieldName, {
      title
    }]) => ({
      value: fieldName,
      text: title ?? fieldName
    })).sort((a, b) => a.text.localeCompare(b.text));
  }
  get operatorOptions() {
    return this.operators[this.fieldIsRelation ? 'relation' : this.fieldType];
  }
  get fieldIsRelation() {
    return Boolean(this.currentPropertyRelationConfig);
  }
  get currentPropertyRelationConfig() {
    var _this$store, _this$store$modelRela;
    return (_this$store = this.store) === null || _this$store === void 0 ? void 0 : (_this$store$modelRela = _this$store.modelRelations) === null || _this$store$modelRela === void 0 ? void 0 : _this$store$modelRela.find(({
      foreignKey
    }) => {
      var _this$_filter3;
      return foreignKey === ((_this$_filter3 = this._filter) === null || _this$_filter3 === void 0 ? void 0 : _this$_filter3.property);
    });
  }
  updateOperators() {
    delete this._operatorArgCountLookup;
  }
  /**
   * @internal
   */
  get operatorArgCountLookup() {
    return this._operatorArgCountLookup || (this._operatorArgCountLookup = FieldFilterPicker.buildOperatorArgCountLookup(this.operators));
  }
  updateFilter() {
    if (this._filter) {
      this.onFilterChange();
    }
  }
  updateStore(newStore) {
    var _this$_store;
    (_this$_store = this._store) === null || _this$_store === void 0 ? void 0 : _this$_store.un(this);
    newStore === null || newStore === void 0 ? void 0 : newStore.ion({
      refresh: 'onStoreRefresh',
      thisObj: this
    });
  }
  onStoreRefresh({
    action
  }) {
    if (this.isMultiSelectValueField && ['dataset', 'create', 'update', 'delete'].includes(action)) {
      this.valueFields[0].items = this.getUniqueDataValues(this.filterValues);
    }
  }
  refreshValueFields() {
    const me = this,
      {
        valueFieldRow
      } = me.widgetMap;
    valueFieldRow.removeAll();
    valueFieldRow.add(me.getValueFieldConfigs());
    delete me._valueFields;
    me.refreshCaseSensitive();
  }
  refreshCaseSensitive() {
    var _me$_filter2, _me$_filter3;
    const me = this,
      {
        fieldType,
        operatorArgCount,
        isMultiSelectValueField
      } = me,
      operator = (_me$_filter2 = me._filter) === null || _me$_filter2 === void 0 ? void 0 : _me$_filter2.operator,
      {
        caseSensitive
      } = me.widgetMap;
    caseSensitive.hidden = fieldType !== 'string' || !operator || isMultiSelectValueField || operatorArgCount === 0;
    caseSensitive.checked = ((_me$_filter3 = me._filter) === null || _me$_filter3 === void 0 ? void 0 : _me$_filter3.caseSensitive) !== false;
  }
  onPropertySelect(event) {
    var _event$record;
    const me = this,
      {
        _filter
      } = me;
    _filter.property = ((_event$record = event.record) === null || _event$record === void 0 ? void 0 : _event$record.data.value) || null;
    if (me.fieldType !== me._fieldType) {
      _filter.operator = null;
      _filter.value = null;
    }
    me._fieldType = _filter.type = me.fieldType;
    me.refreshOperatorPicker();
    me.refreshValueFields();
    me.triggerChange();
  }
  onCaseSensitiveChange({
    checked
  }) {
    this._filter.caseSensitive = checked;
    this.triggerChange();
  }
  onOperatorSelect(event) {
    var _event$record2;
    const me = this,
      wasMultiSelectValueField = me.isMultiSelectValueField;
    const prevArgCount = this.operatorArgCount;
    me._filter.operator = ((_event$record2 = event.record) === null || _event$record2 === void 0 ? void 0 : _event$record2.data.value) || null;
    if (me.operatorArgCount !== prevArgCount) {
      me._filter.value = null;
    }
    if (me.isMultiSelectValueField && !wasMultiSelectValueField) {
      me._filter.value = [];
    }
    me.refreshValueFields();
    me.triggerChange();
  }
  triggerChange() {
    const {
      filter,
      isValid
    } = this;
    /**
     * Fires when the filter is modified.
     * @event change
     * @param {Core.widget.FieldFilterPicker} source The FieldFilterPicker instance that fired the event.
     * @param {Array} filter The {@link Core.util.CollectionFilter} configuration object for the filter represented by this
     *                       {@link Core.widget.FieldFilterPicker}.
     * @param {Boolean} isValid Whether the current configuration object represents a complete and valid filter
     */
    this.trigger('change', {
      filter,
      isValid
    });
  }
  onValueChange() {
    const me = this,
      {
        isMultiSelectValueField,
        fieldType,
        _filter
      } = me,
      values = this.valueFields.map(field => field.value);
    if (isMultiSelectValueField && fieldType === 'date') {
      _filter.value = values[0].map(timestamp => new Date(timestamp));
    } else if (isMultiSelectValueField && fieldType === 'duration') {
      _filter.value = values[0].map(durationStr => new Duration(durationStr));
    } else {
      // Treat end date as inclusive by setting time to end of day
      if (fieldType === 'date' && _filter.operator === 'between' && DateHelper.isValidDate(values[1])) {
        values[1].setHours(23, 59, 59, 999);
      }
      _filter.value = values.length === 1 ? values[0] : values;
    }
    me.triggerChange();
  }
  refreshOperatorPicker() {
    const {
        operatorPicker
      } = this.widgetMap,
      {
        _filter: {
          operator,
          property
        },
        operatorOptions
      } = this;
    operatorPicker.items = operatorOptions;
    operatorPicker.value = operator;
    operatorPicker.hidden = property === null;
  }
  populateUIFromFilter(forceRefreshValueFields = false) {
    const me = this,
      {
        filterValues,
        widgetMap: {
          propertyPicker,
          operatorPicker
        },
        _filter: {
          property,
          operator,
          disabled
        },
        propertyOptions,
        operatorOptions,
        isMultiSelectValueField
      } = me;
    propertyPicker.items = propertyOptions;
    operatorPicker.items = operatorOptions;
    operatorPicker.hidden = property === null;
    let refreshValueFields = forceRefreshValueFields;
    if (propertyPicker.value !== property) {
      propertyPicker.value = property;
      me.refreshOperatorPicker();
      refreshValueFields = true;
    }
    if (operatorPicker.value !== operator) {
      if (operator === null || !operatorPicker.items.find(({
        value
      }) => value === operator)) {
        operatorPicker.clear();
      } else {
        operatorPicker.value = operator;
      }
      refreshValueFields = true;
    }
    if (refreshValueFields) {
      me.refreshValueFields();
    }
    me.refreshCaseSensitive();
    me.valueFields.forEach((valueField, fieldIndex) => {
      if (isMultiSelectValueField && (valueField.value.length > 0 || filterValues.length > 0)) {
        if (me.fieldType === 'date') {
          valueField.value = filterValues.map(date => date === null || date === void 0 ? void 0 : date.valueOf());
        } else if (me.fieldType === 'duration') {
          valueField.value = filterValues.map(duration => duration === null || duration === void 0 ? void 0 : duration.toString());
        } else {
          valueField.value = filterValues;
        }
      } else if (fieldIndex >= filterValues.length) {
        valueField.clear();
      } else {
        valueField.value = filterValues[fieldIndex];
      }
    });
    // Grey out all inputs if filter is disabled
    me.allChildInputs.forEach(widget => widget.disabled = me.disabled || disabled);
  }
  get valueFields() {
    return this._valueFields || (this._valueFields = this.widgetMap.valueFieldRow.queryAll(w => ['textfield', 'datefield', 'numberfield', 'durationfield', 'combo'].includes(w.type)));
  }
  get filterValues() {
    var _this$_filter4;
    if (((_this$_filter4 = this._filter) === null || _this$_filter4 === void 0 ? void 0 : _this$_filter4.value) == null) {
      return [];
    }
    return ArrayHelper.asArray(this._filter.value);
  }
  // Must be called manually when filter modified externally
  onFilterChange() {
    const me = this,
      newFieldType = me.fieldType,
      forceRefreshValueFields = newFieldType !== me._fieldType;
    me._fieldType = me._filter.type = newFieldType;
    me.populateUIFromFilter(forceRefreshValueFields);
  }
  get operatorArgCount() {
    const {
      fieldType,
      filter: {
        operator
      },
      operatorArgCountLookup
    } = this;
    return fieldType && operator ? operatorArgCountLookup[fieldType][operator] : 1;
  }
  get isValid() {
    const me = this,
      {
        filter,
        fieldType,
        filterValues,
        isMultiSelectValueField,
        operatorArgCount
      } = me,
      {
        operator
      } = filter,
      missingValue = operatorArgCount > 0 && (filter === null || filter === void 0 ? void 0 : filter.value) == null;
    return (
      // fieldType here validates that we have a matching field
      fieldType && operator && !missingValue && (isMultiSelectValueField && filterValues.length > 0 || filterValues.length === operatorArgCount) && filterValues.every(value => value != null && (fieldType !== 'duration' || value.isValid))
    );
  }
}
FieldFilterPicker.initClass();
FieldFilterPicker._$name = 'FieldFilterPicker';

class Label extends Widget {
  static $name = 'Label';
  static type = 'label';
  static configurable = {
    text: null,
    localizableProperties: ['text']
  };
  compose() {
    const {
      text,
      html
    } = this;
    return {
      tag: 'label',
      text,
      html
    };
  }
}
Label.initClass();
Label._$name = 'Label';

/**
 * @module Core/widget/FieldFilterPickerGroup
 */
/**
 * A set of {@link Core.widget.FieldFilterPicker}s, representing an array
 * of {@link Core.util.CollectionFilter}s. The filters are provided to the
 * picker as an array of filter configuration objects.
 *
 * {@inlineexample Core/widget/FieldFilterPickerGroup.js}
 *
 * When {@link #config-store} is provided in the configuration, the picker group will
 * read filters from the store and monitor for filter changes, and reflect any changes. It is
 * possible to synchronize multiple {@link Core.widget.FieldFilterPickerGroup}s by configuring
 * them with the same store.
 *
 * Does not support modifying filters defined as custom functions.
 *
 * @classtype fieldfilterpickergroup
 * @extends Core/widget/Container
 * @widget
 */
class FieldFilterPickerGroup extends Container {
  //region Config
  static get $name() {
    return 'FieldFilterPickerGroup';
  }
  // Factoryable type name
  static get type() {
    return 'fieldfilterpickergroup';
  }
  /**
   * @private
   */
  static addFilterButtonDefaultText = 'L{FieldFilterPickerGroup.addFilter}';
  static configurable = {
    /**
     * Array of {@link Core.util.CollectionFilter} configuration objects. One
     * {@link Core.widget.FieldFilterPicker} will be created
     * for each object in the array.
     *
     * When {@link #config-store} is provided, any filters in the store will
     * be automatically added and do not need to be provided explicitly.
     *
     * Example:
     * ```javascript
     * filters: [{
     *     // Filter properties should exist among field names configured
     *     // via `fields` or `store`
     *     property: 'age',
     *     operator: '<',
     *     value: 30
     * },{
     *     property: 'title',
     *     operator: 'startsWith',
     *     value: 'Director'
     * }]
     * ```
     *
     * @config
     * @type {CollectionFilterConfig[]}
     */
    filters: [],
    /**
     * Dictionary of {@link Core.widget.FieldFilterPicker#typedef-FieldOption} representing the fields against which filters can be defined,
     * keyed by field name.
     *
     * If filtering a {@link Grid.view.Grid}, consider using {@link Grid.widget.GridFieldFilterPicker}, which can be configured
     * with an existing {@link Grid.view.Grid} instead of, or in combination with, defining fields manually.
     *
     * Example:
     * ```javascript
     * fields: {
     *     // Allow filters to be defined against the 'age' and 'role' fields in our data
     *     age  : { text: 'Age', type: 'number' },
     *     role : { text: 'Role', type: 'string' }
     * }
     * ```
     *
     * @config {Object<String,FieldOption>}
     * @deprecated 5.3.0 Syntax accepting FieldOptions[] was deprecated in favor of dictionary and will be removed in 6.0
     */
    fields: null,
    /**
     * Whether the picker group is disabled.
     *
     * @config {Boolean}
     * @default
     */
    disabled: false,
    /**
     * Whether the picker group is read-only.
     *
     * Example:
     * fields: [
     *    { name: 'age', type: 'number' },
     *    { name: 'title', type: 'string' }
     * ]
     *
     * @config {Boolean}
     * @default
     */
    readOnly: false,
    layout: 'vbox',
    /**
     * The {@link Core.data.Store} whose records will be filtered. The store's {@link Core.data.Store#property-modelClass}
     * will be used to determine field types.
     *
     * This store will be kept in sync with the filters defined in the picker group, and new filters added to the store
     * via other means will appear in this filter group when they are able to be modified by it. (Some types of filters,
     * like arbitrary filter functions, cannot be managed through this widget.)
     *
     * As a corollary, multiple `FieldFilterPickerGroup`s configured with the same store will stay in sync, showing the
     * same filters as the store's filters change.
     *
     * @config {Core.data.Store}
     */
    store: null,
    /**
     * When `limitToProperty` is set to the name of an available field (as specified either
     * explicitly in {@link #config-fields} or implicitly in the
     * {@link #config-store}'s model), it has the following effects:
     *
     * - the picker group will only show filters defined on the specified property
     * - it will automatically set the `property` to the specified property for all newly added
     *   filters where the property is not already set
     * - the property selector is made read-only
     *
     * @config {String}
     */
    limitToProperty: null,
    /**
     * Optional CSS class to apply to the value field(s).
     *
     * @config {String}
     * @private
     */
    valueFieldCls: null,
    /**
     * Show a button at the bottom of the group that adds a new, blank filter to the group.
     *
     * @config {Boolean}
     * @default
     */
    showAddFilterButton: true,
    /**
     * Optional predicate that returns whether a given filter can be deleted. When `canDeleteFilter` is provided,
     * it will be called for each filter and will not show the delete button for those for which the
     * function returns false.
     *
     * @config {Function}
     */
    canDeleteFilter: null,
    /**
     * Optional function that returns {@link Core.widget.FieldFilterPicker} configuration properties for
     * a given filter. When `getFieldFilterPickerConfig` is provided, it will be called for each filter and the returned
     * object will be merged with the configuration properties for the individual
     * {@link Core.widget.FieldFilterPicker} representing that filter.
     *
     * The supplied function should accept a single argument, the {@link Core.util.CollectionFilter} whose picker
     * is being created.
     *
     * @config {Function}
     */
    getFieldFilterPickerConfig: null,
    /**
     * Optional predicate that returns whether a given filter can be managed by this widget. When `canManageFilter`
     * is provided, it will be used to decide whether to display filters found in the configured
     * {@link #config-store}.
     *
     * @config {Function}
     */
    canManageFilter: null,
    /**
     * Sets the text displayed in the 'add filter' button if one is present.
     *
     * @config {String}
     */
    addFilterButtonText: null,
    /**
     * @private
     */
    items: {
      pickers: {
        type: 'container',
        layout: 'vbox',
        scrollable: true,
        items: {}
      },
      addFilterButton: {
        type: 'button',
        text: FieldFilterPickerGroup.addFilterButtonDefaultText,
        cls: `b-${FieldFilterPickerGroup.type}-add-button`,
        hidden: true
      }
    },
    /**
     * When specified, overrides the built-in list of available operators. See
     * {@link Core.widget.FieldFilterPicker#config-operators}.
     *
     * @config {Object}
     */
    operators: null,
    /**
     * The date format string used to display dates when using the 'is one of' / 'is not one of' operators with a date
     * field. Defaults to the current locale's `FieldFilterPicker.dateFormat` value.
     *
     * @config {String}
     * @default
     */
    dateFormat: 'L{FieldFilterPicker.dateFormat}'
  };
  // endregion
  static childPickerType = 'fieldfilterpicker';
  afterConstruct() {
    const me = this;
    me.validateConfig();
    const {
      addFilterButton
    } = me.widgetMap;
    addFilterButton.ion({
      click: 'addFilter',
      thisObj: me
    });
    addFilterButton.text = me.L(addFilterButton.text);
    me.store && me.updateStore(me.store);
    super.afterConstruct();
  }
  changeDateFormat(dateFormat) {
    return this.L(dateFormat);
  }
  validateConfig() {
    if (!this.fields && !this.store) {
      throw new Error(`FieldFilterPickerGroup requires either a 'fields' or 'store' config property.`);
    }
  }
  /**
   * @private
   */
  get someFiltersNotShown() {
    return this.store && this.store.filters.count > this.filters.length;
  }
  updateFields(newFields) {
    this.widgetMap.pickers.childItems.forEach(picker => picker.fields = newFields);
  }
  updateFilters(newFilters, oldFilters) {
    const me = this;
    if (oldFilters) {
      oldFilters.filter(filter => !newFilters.find(newFilter => newFilter.id === filter.id)).forEach(filter => {
        var _me$store;
        return (_me$store = me.store) === null || _me$store === void 0 ? void 0 : _me$store.removeFilter(filter.id);
      });
    }
    newFilters.forEach(filter => filter.id = filter.id || me.nextFilterId);
    me.widgetMap.pickers.items = (newFilters === null || newFilters === void 0 ? void 0 : newFilters.map(filter => me.getPickerRowConfig(filter))) || [];
  }
  changeFilters(newFilters) {
    const {
      canManageFilter
    } = this;
    return newFilters && canManageFilter ? newFilters.filter(filter => this.callback(canManageFilter, this, [filter])) : newFilters;
  }
  updateStore(newStore) {
    const me = this;
    me.detachListeners('store');
    if (newStore) {
      // Make sure the store has all of our configured filters
      me.widgetMap.pickers.childItems.forEach(({
        widgetMap: {
          filterPicker: {
            filter,
            isValid
          }
        }
      }) => {
        newStore.removeFilter(filter.id, true);
        if (isValid) {
          newStore.addFilter(filter, true);
        }
      });
      newStore.filter();
      me.appendFiltersFromStore();
      newStore.ion({
        name: 'store',
        filter: 'onStoreFilter',
        thisObj: me
      });
    }
    me.widgetMap.pickers.childItems.forEach(picker => picker.store = newStore);
  }
  updateShowAddFilterButton(newShow) {
    this.widgetMap.addFilterButton.hidden = !newShow;
  }
  updateAddFilterButtonText(newText) {
    this.widgetMap.addFilterButton.text = newText ?? FieldFilterPickerGroup.addFilterButtonDefaultText;
  }
  /**
   * Find any filters the store has that we don't know about yet, and add to our list
   * @private
   */
  appendFiltersFromStore() {
    const me = this;
    me.store.filters.forEach(filter => {
      var _me$filters;
      const canManage = me.canManage(filter),
        {
          property,
          operator,
          value,
          id,
          disabled = false,
          caseSensitive
        } = filter;
      if (canManage && property && operator && !((_me$filters = me.filters) !== null && _me$filters !== void 0 && _me$filters.find(filter => filter.id === id))) {
        me.appendFilter({
          id,
          property,
          operator,
          value,
          disabled,
          caseSensitive
        });
      }
    });
  }
  /**
   * @private
   */
  canManage(filter) {
    const me = this;
    return !me.canManageFilter || me.callback(me.canManageFilter, me, [filter]) === true;
  }
  /**
   * Get the configuration object for one child FieldFilterPicker.
   * @param {Core.util.CollectionFilter} filter The filter represented by the child FieldFilterPicker
   * @returns {Object} The FieldFilterPicker configuration
   */
  getFilterPickerConfig(filter) {
    const me = this,
      {
        fields,
        store,
        disabled,
        readOnly,
        valueFieldCls,
        operators,
        limitToProperty,
        dateFormat,
        getFieldFilterPickerConfig
      } = me;
    return {
      type: me.constructor.childPickerType,
      fields: fields ?? me.getFieldsFromStore(store),
      filter,
      store,
      disabled,
      readOnly,
      propertyLocked: Boolean(limitToProperty),
      valueFieldCls,
      operators,
      dateFormat,
      internalListeners: {
        change: 'onFilterPickerChange',
        thisObj: me
      },
      flex: 1,
      ...(getFieldFilterPickerConfig ? me.callback(getFieldFilterPickerConfig, me, [filter]) : undefined)
    };
  }
  /**
   * Get store fields as {@link Core.widget.FieldFilterPicker#typedef-FieldOption}s in a dictionary keyed by name.
   * @private
   */
  getFieldsFromStore(store) {
    var _store$fields;
    return Object.fromEntries(((_store$fields = store.fields) === null || _store$fields === void 0 ? void 0 : _store$fields.map(({
      name,
      type
    }) => [name, {
      type
    }])) ?? []);
  }
  getPickerRowConfig(filter) {
    const me = this,
      {
        disabled,
        readOnly,
        canDeleteFilter
      } = me;
    return {
      type: 'container',
      layout: 'box',
      layoutStyle: {
        alignItems: 'start'
      },
      cls: `b-${FieldFilterPickerGroup.type}-row`,
      dataset: {
        separatorText: me.L('L{FieldFilterPicker.and}')
      },
      items: {
        activeCheckbox: {
          type: 'checkbox',
          disabled,
          readOnly,
          checked: !Boolean(filter.disabled),
          internalListeners: {
            change: 'onFilterActiveChange',
            thisObj: me
          },
          cls: `b-${FieldFilterPickerGroup.type}-filter-active`
        },
        filterPicker: me.getFilterPickerConfig(filter),
        removeButton: {
          type: 'button',
          ref: 'removeButton',
          disabled,
          readOnly,
          hidden: canDeleteFilter && me.callback(canDeleteFilter, me, [filter]) === false,
          cls: `b-transparent b-${FieldFilterPickerGroup.type}-remove`,
          icon: 'b-fa-trash',
          internalListeners: {
            click: 'removeFilter',
            thisObj: me
          }
        }
      }
    };
  }
  get allInputs() {
    const childInputTypes = [this.constructor.childPickerType, 'button', 'checkbox'];
    return this.queryAll(w => childInputTypes.includes(w.type));
  }
  updateDisabled(newDisabled) {
    this.allInputs.forEach(input => input.disabled = newDisabled);
  }
  updateReadOnly(newReadOnly) {
    this.allInputs.forEach(input => input.readOnly = newReadOnly);
  }
  onFilterActiveChange({
    source,
    checked
  }) {
    const me = this,
      filterIndex = me.getFilterIndex(source),
      filter = me.filters[filterIndex],
      filterPicker = me.getFilterPicker(filterIndex);
    filter.disabled = !checked;
    filterPicker.onFilterChange();
    if (me.store && filterPicker.isValid) {
      me.store.addFilter(filter, true);
    }
    me.updateStoreFilter();
    me.triggerChange();
  }
  onFilterPickerChange({
    source,
    filter,
    isValid
  }) {
    const me = this,
      {
        store
      } = me,
      filterIndex = me.getFilterIndex(source);
    if (store) {
      store.removeFilter(filter.id, true);
      if (isValid) {
        store.addFilter(filter, true);
      }
      me.updateStoreFilter();
    }
    Object.assign(me.filters[filterIndex], filter);
    me.triggerChange();
  }
  getFilterIndex(eventSource) {
    return this.widgetMap.pickers.childItems.indexOf(eventSource.containingWidget);
  }
  getPickerRow(index) {
    return this.widgetMap.pickers.childItems[index];
  }
  /**
   * Return the {@link Core.widget.FieldFilterPicker} for the filter at the specified index.
   * @param {Number} filterIndex
   * @returns {Core.widget.FieldFilterPicker}
   */
  getFilterPicker(filterIndex) {
    return this.getPickerRow(filterIndex).widgetMap.filterPicker;
  }
  get nextFilterId() {
    this._nextId = (this._nextId || 0) + 1;
    return `${this.id}-filter-${this._nextId}`;
  }
  removeFilter({
    source
  }) {
    const me = this,
      filterIndex = me.getFilterIndex(source),
      filter = me.filters[filterIndex],
      pickerRow = me.getPickerRow(filterIndex),
      newFocusWidget = me.query(w => w.isFocusable && w.type !== 'container' && !pickerRow.contains(w));
    if (newFocusWidget) {
      newFocusWidget.focus();
    }
    me.removeFilterAt(filterIndex);
    if (me.store) {
      me.store.removeFilter(filter.id, true);
      me.updateStoreFilter();
    }
    me.trigger('remove', {
      filter
    });
    me.triggerChange();
  }
  /**
   * Appends a filter at the bottom of the list
   * @param {CollectionFilterConfig} [filter={}] Configuration object for the {@link Core.util.CollectionFilter} to
   * add. Defaults to an empty filter.
   */
  addFilter({
    property = null,
    operator = null,
    value = null
  } = {}) {
    const me = this,
      {
        filters
      } = me,
      newFilter = {
        property: me.limitToProperty || property,
        operator,
        value,
        disabled: false,
        id: me.nextFilterId,
        caseSensitive: false
      };
    me.appendFilter(newFilter);
    if (me.getFilterPicker(filters.length - 1).isValid) {
      var _me$store2;
      (_me$store2 = me.store) === null || _me$store2 === void 0 ? void 0 : _me$store2.addFilter(newFilter, true);
      me.store && me.updateStoreFilter();
    }
    me.trigger('add', {
      filter: newFilter
    });
    me.triggerChange();
  }
  /**
   * @private
   */
  appendFilter(filter) {
    const me = this;
    if (!me.limitToProperty || filter.property === me.limitToProperty) {
      me.filters.push(filter);
      me.widgetMap.pickers.add(me.getPickerRowConfig(filter, me.filters.length - 1));
    }
  }
  onStoreFilter(event) {
    const me = this;
    if (me._isUpdatingStore) {
      return;
    }
    const {
        filters
      } = event,
      storeFiltersById = filters.values.reduce((byId, filter) => ({
        ...byId,
        [filter.id]: filter
      }), {});
    for (let filterIndex = me.filters.length - 1; filterIndex >= 0; filterIndex--) {
      const filter = me.filters[filterIndex],
        storeFilter = storeFiltersById[filter.id],
        filterRow = me.getPickerRow(filterIndex);
      if (filterRow) {
        const {
          filterPicker,
          activeCheckbox
        } = filterRow.widgetMap;
        if (!storeFilter && filterPicker.isValid) {
          me.removeFilterAt(filterIndex);
        } else if (storeFilter !== undefined) {
          const {
            operator,
            value,
            property,
            disabled,
            caseSensitive
          } = storeFilter;
          if (filter !== storeFilter) {
            Object.assign(filter, {
              operator,
              value,
              property,
              disabled,
              caseSensitive
            });
          }
          filterPicker.filter = filter;
          filterPicker.onFilterChange();
          activeCheckbox.checked = !disabled;
        }
      }
    }
    me.appendFiltersFromStore();
    me.triggerChange();
  }
  /**
   * Remove the filter at the given index.
   * @param {Number} filterIndex The index of the filter to remove
   */
  removeFilterAt(filterIndex) {
    const {
      widgetMap: {
        pickers
      },
      filters
    } = this;
    pickers.remove(pickers.childItems[filterIndex]);
    filters.splice(filterIndex, 1);
    this.triggerChange();
  }
  /**
   * Trigger a store re-filter after filters have been silently modified.
   * @private
   */
  updateStoreFilter() {
    var _this$store;
    this._isUpdatingStore = true;
    (_this$store = this.store) === null || _this$store === void 0 ? void 0 : _this$store.filter();
    this._isUpdatingStore = false;
  }
  /**
   * Returns the array of filter configuration objects currently represented by this picker group.
   * @type {CollectionFilterConfig[]}
   */
  get value() {
    return this.filters;
  }
  triggerChange() {
    const {
        filters
      } = this,
      validFilters = filters.filter((f, index) => this.getPickerRow(index).widgetMap.filterPicker.isValid);
    /**
     * Fires when any filter in the group is added, removed, or modified.
     * @event change
     * @param {Core.widget.FieldFilterPickerGroup} source The FieldFilterPickerGroup instance that fired the event.
     * @param {CollectionFilterConfig[]} filters The array of {@link Core.util.CollectionFilter} configuration
     * objects currently represented by the FieldFilterPickerGroup. **IMPORTANT:** Note that this includes all filters
     * currently present in the UI, including partially completed ones that may not be ready to apply to a Store.
     * To retrieve only valid filters, use the `validFilters` parameter on this event, or filter out incomplete filters
     * in your own code.
     * @param {CollectionFilterConfig[]} validFilters The subset of {@link Core.util.CollectionFilter} configuration
     * objects in the `filters` parameter on this event that are complete and valid for application to a Store.
     */
    this.trigger('change', {
      filters,
      validFilters
    });
  }
  /**
   * Sets all current filters to enabled and checks their checkboxes.
   */
  activateAll() {
    this.setAllActiveStatus(true);
  }
  /**
   * Sets all current filters to disabled and clears their checkboxes.
   */
  deactivateAll() {
    this.setAllActiveStatus(false);
  }
  /**
   * @private
   */
  setAllActiveStatus(newActive) {
    const me = this,
      {
        _filters,
        store
      } = me;
    _filters.forEach((filter, filterIndex) => {
      // Only do anything if status is changing
      if (newActive === filter.disabled) {
        const {
          filterPicker,
          activeCheckbox
        } = me.getPickerRow(filterIndex).widgetMap;
        filter.disabled = !newActive;
        filterPicker.onFilterChange();
        activeCheckbox.checked = newActive;
        if (newActive && store && filterPicker.isValid) {
          store.addFilter(filter, true);
        }
      }
    });
    me.updateStoreFilter();
  }
}
FieldFilterPickerGroup.initClass();
FieldFilterPickerGroup._$name = 'FieldFilterPickerGroup';

/**
 * @module Core/widget/MessageDialog
 */
const items = [{
  ref: 'cancelButton',
  cls: 'b-messagedialog-cancelbutton b-gray',
  text: 'L{Object.Cancel}',
  onClick: 'up.onCancelClick'
}, {
  ref: 'okButton',
  cls: 'b-messagedialog-okbutton b-raised b-blue',
  text: 'L{Object.Ok}',
  onClick: 'up.onOkClick'
}];
// Windows has OK button to the left, Mac / Ubuntu to the right
if (BrowserHelper.isWindows) {
  items.reverse();
}
class MessageDialogConstructor extends Popup {
  static get $name() {
    return 'MessageDialog';
  }
  // Factoryable type name
  static get type() {
    return 'messagedialog';
  }
  static get configurable() {
    return {
      centered: true,
      modal: true,
      hidden: true,
      autoShow: false,
      closeAction: 'hide',
      title: '\xa0',
      lazyItems: {
        $config: ['lazy'],
        value: [{
          cls: 'b-messagedialog-message',
          ref: 'message'
        }, {
          type: 'textfield',
          cls: 'b-messagedialog-input',
          ref: 'input'
        }]
      },
      showClass: null,
      bbar: {
        overflow: null,
        items
      }
    };
  }
  construct() {
    /**
     * The enum value for the OK button
     * @member {Number} okButton
     * @readOnly
     */
    this.okButton = this.yesButton = 1;
    /**
     * The enum value for the Cancel button
     * @member {Number} cancelButton
     * @readOnly
     */
    this.cancelButton = 3;
    super.construct(...arguments);
  }
  // Protect from queryAll -> destroy
  destroy() {}
  /**
   * Shows a confirm dialog with "Ok" and "Cancel" buttons. The returned promise resolves passing the button identifier
   * of the button that was pressed ({@link #property-okButton} or {@link #property-cancelButton}).
   * @function confirm
   * @param {Object} options An options object for what to show.
   * @param {String} [options.title] The title to show in the dialog header.
   * @param {String} [options.message] The message to show in the dialog body.
   * @param {String} [options.rootElement] The root element of this widget, defaults to document.body. Use this
   * if you use the MessageDialog inside a web component ShadowRoot
   * @param {String|ButtonConfig} [options.cancelButton] A text or a config object to apply to the Cancel button.
   * @param {String|ButtonConfig} [options.okButton] A text or config object to apply to the OK button.
   * @returns {Promise} A promise which is resolved when the dialog is closed
   */
  async confirm() {
    return this.showDialog('confirm', ...arguments);
  }
  /**
   * Shows an alert popup with a message. The returned promise resolves when the button is clicked.
   * @function alert
   * @param {Object} options An options object for what to show.
   * @param {String} [options.title] The title to show in the dialog header.
   * @param {String} [options.message] The message to show in the dialog body.
   * @param {String} [options.rootElement] The root element of this widget, defaults to document.body. Use this
   * if you use the MessageDialog inside a web component ShadowRoot
   * @param {String|ButtonConfig} [options.okButton] A text or config object to apply to the OK button.
   * @returns {Promise} A promise which is resolved when the dialog is closed
   */
  async alert() {
    return this.showDialog('alert', ...arguments);
  }
  /**
   * Shows a popup with a basic {@link Core.widget.TextField} along with a message. The returned promise resolves when
   * the dialog is closed and yields an Object with a `button` ({@link #property-okButton} or {@link #property-cancelButton})
   * and a `text` property with the text the user provided
   * @function prompt
   * @param {Object} options An options object for what to show.
   * @param {String} [options.title] The title to show in the dialog header.
   * @param {String} [options.message] The message to show in the dialog body.
   * @param {String} [options.rootElement] The root element of this widget, defaults to document.body. Use this
   * if you use the MessageDialog inside a web component ShadowRoot
   * @param {TextFieldConfig} [options.textField] A config object to apply to the TextField.
   * @param {String|ButtonConfig} [options.cancelButton] A text or a config object to apply to the Cancel button.
   * @param {String|ButtonConfig} [options.okButton] A text or config object to apply to the OK button.
   * @returns {Promise} A promise which is resolved when the dialog is closed. The promise yields an Object with
   * a `button` ({@link #property-okButton} or {@link #property-cancelButton}) and a `text` property with the text the
   * user provided
   */
  async prompt({
    textField
  }) {
    const field = this.widgetMap.input;
    Widget.reconfigure(field, textField);
    field.value = '';
    return this.showDialog('prompt', ...arguments);
  }
  showDialog(mode, {
    message = '',
    title = '\xa0',
    cancelButton,
    okButton,
    rootElement = document.body
  }) {
    const me = this;
    me.rootElement = rootElement;
    // Ensure our child items are instanced
    me.getConfig('lazyItems');
    me.title = me.optionalL(title);
    me.widgetMap.message.html = me.optionalL(message);
    me.showClass = `b-messagedialog-${mode}`;
    // Normalize string input to config object
    if (okButton) {
      okButton = typeof okButton === 'string' ? {
        text: okButton
      } : okButton;
    }
    if (cancelButton) {
      cancelButton = typeof cancelButton === 'string' ? {
        text: cancelButton
      } : cancelButton;
    }
    // Ensure default configs are applied
    okButton = Object.assign({}, me.widgetMap.okButton.initialConfig, okButton);
    cancelButton = Object.assign({}, me.widgetMap.cancelButton.initialConfig, cancelButton);
    // Ensure strings are localized
    okButton.text = me.optionalL(okButton.text);
    cancelButton.text = me.optionalL(cancelButton.text);
    Widget.reconfigure(me.widgetMap.okButton, okButton);
    Widget.reconfigure(me.widgetMap.cancelButton, cancelButton);
    me.show();
    return me.promise = new Promise(resolve => {
      me.resolve = resolve;
    });
  }
  show() {
    const activeElement = DomHelper.getActiveElement(this.element);
    // So that when we focus, we don't close an autoClose popup, but temporarily become
    // part of its ownership tree.
    this.owner = this.element.contains(activeElement) ? null : MessageDialogConstructor.fromElement(document.activeElement);
    return super.show(...arguments);
  }
  updateShowClass(showClass, oldShowClass) {
    const {
      classList
    } = this.element;
    if (oldShowClass) {
      classList.remove(oldShowClass);
    }
    if (showClass) {
      classList.add(showClass);
    }
  }
  doResolve(value) {
    const me = this,
      {
        resolve
      } = me;
    if (resolve) {
      const isPrompt = me.showClass === 'b-messagedialog-prompt';
      if (isPrompt && value === me.okButton && !me.widgetMap.input.isValid) {
        return;
      }
      me.resolve = me.reject = me.promise = null;
      resolve(isPrompt ? {
        button: value,
        text: me.widgetMap.input.value
      } : value);
      me.hide();
    }
  }
  onInternalKeyDown(event) {
    // Cancel on escape key
    if (event.key === 'Escape') {
      event.stopImmediatePropagation();
      this.onCancelClick();
    }
    if (event.key === 'Enter') {
      event.stopImmediatePropagation();
      event.preventDefault(); // Needed to not spill over into next MessageDialog if closing this opens another
      this.onOkClick();
    }
    super.onInternalKeyDown(event);
  }
  onOkClick() {
    this.doResolve(MessageDialog.okButton);
  }
  onCancelClick() {
    this.doResolve(MessageDialog.cancelButton);
  }
}
// Register this widget type with its Factory
MessageDialogConstructor.initClass();
// Instantiate MessgeDialog Widget on first use.
const MessageDialog = new Proxy({}, {
  get(target, prop) {
    const instance = target.instance || (target.instance = new MessageDialogConstructor({
        rootElement: document.body
      })),
      result = instance[prop];
    return typeof result === 'function' ? result.bind(instance) : result;
  }
});

export { CalendarPanel, Checkbox, DateField, DatePicker, DisplayField, DragHelper, DurationField, FieldFilterPicker, FieldFilterPickerGroup, Formatter, Label, MessageDialog, Month, NumberField, NumberFormat, ResizeHelper, SUPPORTED_FIELD_DATA_TYPES, TimeField, TimePicker, WidgetHelper, YearPicker, isSupportedDurationField };
//# sourceMappingURL=MessageDialog.js.map
