/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Events, Base, Panel, Labelable, DomHelper, ObjectHelper, Widget, Button, FunctionHelper, DomClassList, Toolbar, ArrayHelper, GlobalEvents } from './Editor.js';
import { Checkbox } from './MessageDialog.js';
import './Card.js';

/**
 * @module Core/helper/WebSocketManager
 */
/**
 * This class allows to send and receive messages from websocket server passing responses via events. This helper is
 * meant to be used with a demo websocket server. It sends messages that are JSON strings including "command" key and
 * arbitrary data keys. For example:
 *
 * ```javascript
 * // request string to notify other clients that new client is connected
 * "{ \"command\": \"hello\", \"userName\": \"new user\" }"
 *
 * // response message from the websocket server with list of connected users
 * "{ \"command\": \"users\", \"users\": [\"new user\"] }"
 * ```
 *
 * Usage:
 * ```javascript
 * connector = new WebSocketManager({
 *     address     : 'ws://localhost:8080',
 *     userName    : 'Test client',
 *     autoConnect : false
 * });
 *
 * const opened = await connector.open();
 *
 * if (!opened) {
 *     console.log('Could not open connection');
 * }
 *
 * connector.on({
 *     message({ data }) {
 *         console.log(data);
 *     }
 * });
 *
 * // Sends "{ \"command\": \"hello\", \"userName\": \"mark\" }" string to the websocket server
 * // When response arrives helper will log following message: "{ command: 'users', users: ['mark'] }"
 * connector.send('hello', { userName : 'mark' });
 * ```
 *
 * @class
 * @extends Core/Base
 * @mixes Core/mixin/Events
 * @private
 */
class WebSocketManager extends Events(Base) {
  // This allows to hook into for testing purposes
  static webSocketImplementation = typeof WebSocket === 'undefined' ? null : WebSocket;
  static configurable = {
    /**
     * WebSocket server address
     * @config {String}
     */
    address: '',
    /**
     * User name allowing to identify client
     * @config {String}
     */
    userName: 'User',
    /**
     * Connect to websocket server immediately after instantiation
     * @config {Boolean}
     */
    autoConnect: true
  };
  construct(config = {}) {
    const me = this;
    super.construct(config);
    me.onWsOpen = me.onWsOpen.bind(me);
    me.onWsClose = me.onWsClose.bind(me);
    me.onWsMessage = me.onWsMessage.bind(me);
    me.onWsError = me.onWsError.bind(me);
    if (me.autoConnect && me.address) {
      me.open();
    }
  }
  doDestroy() {
    const me = this;
    if (me.connector) {
      me.detachSocketListeners(me.connector);
      me.connector.close();
      me.connector = null;
    }
    super.doDestroy();
  }
  //#region Websocket state
  get isConnecting() {
    var _this$connector;
    return ((_this$connector = this.connector) === null || _this$connector === void 0 ? void 0 : _this$connector.readyState) === this.constructor.webSocketImplementation.CONNECTING;
  }
  get isOpened() {
    var _this$connector2;
    return ((_this$connector2 = this.connector) === null || _this$connector2 === void 0 ? void 0 : _this$connector2.readyState) === this.constructor.webSocketImplementation.OPEN;
  }
  get isClosing() {
    var _this$connector3;
    return ((_this$connector3 = this.connector) === null || _this$connector3 === void 0 ? void 0 : _this$connector3.readyState) === this.constructor.webSocketImplementation.CLOSING;
  }
  get isClosed() {
    var _this$connector4;
    return ((_this$connector4 = this.connector) === null || _this$connector4 === void 0 ? void 0 : _this$connector4.readyState) === this.constructor.webSocketImplementation.CLOSED;
  }
  //#endregion
  //#region Websocket init
  createWebSocketConnector() {
    const connector = this.connector = new this.constructor.webSocketImplementation(this.address);
    this.attachSocketListeners(connector);
  }
  destroyWebSocketConnector() {
    this.detachSocketListeners(this.connector);
    this.connector.close();
    this.connector = null;
  }
  attachSocketListeners(connector) {
    const me = this;
    connector.addEventListener('open', me.onWsOpen);
    connector.addEventListener('close', me.onWsClose);
    connector.addEventListener('message', me.onWsMessage);
    connector.addEventListener('error', me.onWsError);
  }
  detachSocketListeners(connector) {
    const me = this;
    connector.removeEventListener('open', me.onWsOpen);
    connector.removeEventListener('close', me.onWsClose);
    connector.removeEventListener('message', me.onWsMessage);
    connector.removeEventListener('error', me.onWsError);
  }
  //#endregion
  //#region Websocket methods
  /**
   * Connect to the server and start listening for messages
   * @returns {Promise} Returns true if connection was successful and false otherwise
   */
  async open() {
    const me = this;
    if (me._openPromise) {
      return me._openPromise;
    }
    if (!me.address) {
      console.warn('Server me.address cannot be empty');
      return;
    }
    if (me.isOpened) {
      return true;
    }
    me.createWebSocketConnector();
    let detacher;
    // Wait for `open` or `close` event
    me._openPromise = new Promise(resolve => {
      detacher = me.ion({
        open() {
          resolve(true);
        },
        error() {
          resolve(false);
        }
      });
    }).then(value => {
      // Detach listeners
      detacher();
      // Cleanup the promise
      me._openPromise = null;
      // If quit early with a timeout then remove reference to the WebSocket instance
      if (!value) {
        me.destroyWebSocketConnector();
      }
      return value;
    }).catch(() => {
      me._openPromise = null;
      me.destroyWebSocketConnector();
    });
    return me._openPromise;
  }
  /**
   * Close socket and disconnect from the server
   */
  close() {
    if (this.connector) {
      this.destroyWebSocketConnector();
      this.trigger('close');
    }
  }
  /**
   * Send data to the websocket server
   * @param {String} command
   * @param {*} data
   */
  send(command, data = {}) {
    var _this$connector5;
    (_this$connector5 = this.connector) === null || _this$connector5 === void 0 ? void 0 : _this$connector5.send(JSON.stringify({
      command,
      ...data
    }));
  }
  //#endregion
  //#region websocket event listeners
  onWsOpen(event) {
    this.trigger('open', {
      event
    });
  }
  onWsClose(event) {
    this.trigger('close', {
      event
    });
  }
  onWsMessage(message) {
    try {
      const data = JSON.parse(message.data);
      this.trigger('message', {
        data
      });
    } catch (error) {
      this.trigger('error', {
        error
      });
    }
  }
  onWsError(error) {
    this.trigger('error', {
      error
    });
  }
  //#endregion
}

WebSocketManager._$name = 'WebSocketManager';

/**
 * @module Core/widget/FieldSet
 */
/**
 * The `FieldSet` widget wraps an <code>&lt;fieldset&gt;</code> element. A fieldset is a specially styled
 * {@link Core.widget.Panel} intended to hold form fields.
 *
 * @extends Core/widget/Panel
 * @mixes Core/widget/mixin/Labelable
 * @classType fieldset
 * @widget
 */
class FieldSet extends Panel.mixin(Labelable) {
  //region Config
  static get $name() {
    return 'FieldSet';
  }
  // Factoryable type name
  static get type() {
    return 'fieldset';
  }
  static get configurable() {
    return {
      bodyTag: 'fieldset',
      focusable: false,
      /**
       * Setting this config to `true` assigns a horizontal box layout (`flex-flow: row`) to the items in this
       * container, while `false` assigns a vertical box layout (`flex-flow: column`).
       *
       * By default, this value is automatically determined based on the {@link #config-label} and
       * {@link #config-labelPosition} configs.
       * @config {Boolean}
       */
      inline: null,
      inlineInternal: null,
      layout: {
        type: 'box',
        horizontal: false
      }
    };
  }
  static get prototypeProperties() {
    return {
      flexRowCls: 'b-hbox',
      flexColCls: 'b-vbox'
    };
  }
  //endregion
  //region Composition
  get bodyConfig() {
    const result = super.bodyConfig,
      {
        className
      } = result,
      {
        inlineInternal: inline,
        hasLabel,
        title
      } = this;
    delete result.html;
    className['b-inline'] = inline;
    className['b-fieldset-has-label'] = hasLabel;
    if (title) {
      result.children = {
        // We render the <legend> element for a11y (not 100% sure it is needed)
        legendElement: {
          tag: 'legend',
          text: title,
          class: {
            'b-fieldset-legend': 1
          }
        }
      };
    }
    return result;
  }
  compose() {
    const {
      inlineInternal: inline,
      label,
      labelCls,
      labelWidth
    } = this;
    return {
      class: {
        'b-field': label,
        'b-vbox': !inline // override panel
      },

      children: {
        'labelElement > headerElement': (label || null) && {
          tag: 'label',
          html: label,
          class: {
            'b-label': 1,
            'b-align-start': 1,
            [labelCls]: labelCls
          },
          style: {
            width: DomHelper.unitize('width', labelWidth)[1]
          }
        }
      }
    };
  }
  //endregion
  syncInlineInternal() {
    this.inlineInternal = this.inline ?? (this.label != null && this.labelPosition === 'before');
  }
  updateDisabled(value, was) {
    super.updateDisabled(value, was);
    // Needs {}'s to avoid a "return false" that ends iteration
    this.eachWidget(item => {
      item.disabled = value;
    }, /* deep = */false);
  }
  updateInline() {
    this.syncInlineInternal();
  }
  updateInlineInternal(inline) {
    this.layout.horizontal = inline;
  }
  updateLabel() {
    this.syncInlineInternal();
  }
  updateLabelPosition() {
    this.syncInlineInternal();
  }
}
// Register this widget type with its Factory
FieldSet.initClass();
FieldSet._$name = 'FieldSet';

/**
 * @module Core/widget/Radio
 */
/**
 * The `Radio` widget wraps an <code>&lt;input type="radio"&gt;</code> element.
 *
 * Color can be specified and you can optionally configure {@link #config-text} to display in a label to the right of
 * the radio button instead of, or in addition to, a standard field {@link #config-label}.
 *
 * {@inlineexample Core/widget/Radio.js vertical}
 *
 * ## Nested Items
 * A radio button can also have a {@link #config-container} of additional {@link Core.widget.Container#config-items}.
 * These items can be displayed immediately following the field's label (which is the default when there is only one
 * item) or below the radio button. This can be controlled using the {@link #config-inline} config.
 *
 * In the demo below notice how additional fields are displayed for the checked radio button:
 *
 * {@inlineexample Core/widget/Radio-items.js vertical}
 *
 * For a simpler way to create a set of radio buttons, see the {@link Core.widget.RadioGroup} widget.
 *
 * @extends Core/widget/Checkbox
 * @classType radio
 * @widget
 */
class Radio extends Checkbox {
  //region Config
  static get $name() {
    return 'Radio';
  }
  // Factoryable type name
  static get type() {
    return 'radio';
  }
  // Factoryable type alias
  static get alias() {
    return 'radiobutton';
  }
  static get configurable() {
    return {
      inputType: 'radio',
      /**
       * Set this to `true` so that clicking a checked radio button will clear its checked state.
       * @config {Boolean}
       * @default false
       */
      clearable: null,
      uncheckedValue: undefined // won't store to Container#values when unchecked
    };
  }
  //endregion
  //region Init
  get textLabelCls() {
    return super.textLabelCls + ' b-radio-label';
  }
  //endregion
  internalOnClick(info) {
    if (super.internalOnClick(info) !== false) {
      if (this.checked && this.clearable) {
        this.checked = false;
      }
    }
  }
  updateName(name) {
    this.toggleGroup = name;
  }
  // Empty override to get rid of clear trigger
  updateClearable() {}
}
// Register this widget type with its Factory
Radio.initClass();
Radio._$name = 'Radio';

/**
 * @module Core/widget/RadioGroup
 */
/**
 * The `RadioGroup` widget contains a set of related `{@link Core/widget/Radio}` button widgets.
 *
 * For example, to present three choices and have the user select one of them:
 *
 * ```javascript
 *  {
 *      type    : 'radiogroup',
 *      title   : 'Resolve Conflict',
 *      name    : 'resolution',
 *      value   : 'A',  // the default choice
 *      options : {
 *          A : 'Keep the original version',
 *          B : 'Use the new version',
 *          C : 'Reconcile individual conflicts'
 *      }
 *  }
 * ```
 *
 * {@inlineexample Core/widget/RadioGroup.js}
 *
 * The {@link #config-name} config is required for this widget and it will be assigned to all radio buttons created by
 * processing the {@link #config-options} config.
 *
 * ## Nested Items
 * Radio buttons can also have a {@link Core.widget.Radio#config-container} of additional
 * {@link Core.widget.Container#config-items}. These items can be displayed immediately following the field's label
 * (which is the default when there is only one item) or below the radio button. This can be controlled using the
 * {@link Core.widget.Radio#config-inline} config.
 *
 * In the demo below notice how additional fields are displayed for the checked radio button:
 *
 * {@inlineexample Core/widget/RadioGroupNested.js vertical}
 *
 * @extends Core/widget/FieldSet
 * @classType radiogroup
 * @widget
 */
class RadioGroup extends FieldSet {
  //region Config
  static get $name() {
    return 'RadioGroup';
  }
  // Factoryable type name
  static get type() {
    return 'radiogroup';
  }
  static get configurable() {
    return {
      defaultType: 'radio',
      /**
       * Set this to `true` so that clicking the currently checked radio button will clear the check from all
       * radio buttons in the group.
       * @config {Boolean}
       * @default false
       */
      clearable: null,
      /**
       * The name by which this widget's {@link #property-value} is accessed using the parent container's
       * {@link Core.widget.Container#property-values}.
       *
       * The config must be provided as it is used to set the {@link Core.widget.Radio#config-name} of the
       * child {@link Core.widget.Radio radio buttons}.
       * @config {String}
       */
      name: null,
      /**
       * The set of radio button options for this radio button group. This is a shorthand for defining these in
       * the {@link Core.widget.Container#config-items}. The keys of this object hold the radio button's
       * {@link Core.widget.Radio#config-checkedValue} while the object values are a string for the radio button's
       * {@link Core.widget.Radio#config-text} or a config object for that radio button.
       *
       * The {@link #property-value} of this radio button group will be one of the keys in this object or `null`
       * if no radio button is checked.
       *
       * For example, consider the following configuration:
       * ```javascript
       *  {
       *      type    : 'radiogroup',
       *      name    : 'resolution',
       *      value   : 'A',
       *      options : {
       *          A : 'Keep the original version',
       *          B : 'Use the new version',
       *          C : 'Reconcile individual conflicts'
       *      }
       *  }
       * ```
       *
       * The above is equivalent to this configuration below using {@link #config-items}:
       * ```javascript
       *  {
       *      type  : 'radiogroup',
       *      items : [{
       *          text         : 'Keep the original version',
       *          name         : 'resolution',
       *          ref          : 'resolution_A',
       *          checked      : true,
       *          checkedValue : 'A'
       *      }, {
       *          text         : 'Use the new version',
       *          name         : 'resolution',
       *          ref          : 'resolution_B',
       *          checkedValue : 'B'
       *      }, {
       *          text         : 'Reconcile individual conflicts',
       *          name         : 'resolution',
       *          ref          : 'resolution_C',
       *          checkedValue : 'C'
       *      }]
       *  }
       * ```
       * @config {Object<String,String|RadioConfig>} options
       */
      options: {
        value: null,
        $config: {
          merge: 'items'
        }
      },
      defaultBindProperty: 'value'
    };
  }
  get existingOptions() {
    const {
      name
    } = this;
    return this.ensureItems().filter(c => c.name === name);
  }
  get refPrefix() {
    return `${this.name || this.ref || this.id}_`;
  }
  get selected() {
    return this.existingOptions.filter(c => c.input.checked)[0] || null;
  }
  /**
   * This property corresponds to the {@link Core.widget.Radio#config-checkedValue} of the currently
   * {@link Core.widget.Radio#property-checked} radio button.
   * @property {String}
   */
  get value() {
    const {
      selected
    } = this;
    return selected ? selected.checkedValue : null;
  }
  set value(v) {
    this.existingOptions.forEach(c => {
      c.isConfiguring = this.isConfiguring;
      c.checked = c.checkedValue === v;
      c.isConfiguring = false;
    });
  }
  ensureItems() {
    this.getConfig('options');
    return super.ensureItems();
  }
  changeOptions(options, was) {
    if (!(options && was && ObjectHelper.isDeeplyEqual(was, options))) {
      return options;
    }
  }
  convertOption(key, option, existing) {
    const me = this,
      {
        name
      } = me,
      ret = {
        name,
        type: 'radio',
        value: key === me.value,
        ref: `${me.refPrefix}${key}`,
        checkedValue: key
      };
    if (typeof option === 'string') {
      ret.text = option;
    } else {
      ObjectHelper.assign(ret, option);
    }
    return existing ? Widget.reconfigure(existing, ret) : ret;
  }
  isOurRadio(item) {
    // Radio groups could be nested using field containers, so we need isRadio and name equality check:
    return item.isRadio && item.name === this.name;
  }
  isolateFieldChange(field) {
    // if this is one of our radio buttons, swallow the field change:
    return this.isOurRadio(field);
  }
  onChildAdd(item) {
    super.onChildAdd(item);
    if (this.isOurRadio(item)) {
      item.ion({
        name: item.id,
        beforeChange: 'onRadioItemBeforeChange',
        change: 'onRadioItemChange',
        click: 'onRadioClick',
        thisObj: this
      });
    }
  }
  onChildRemove(item) {
    if (this.isOurRadio(item)) {
      this.detachListeners(item.id);
    }
    super.onChildRemove(item);
  }
  onRadioClick(ev) {
    const {
      source
    } = ev;
    if (source.checked && this.clearable && source.clearable == null) {
      source.checked = false;
    }
  }
  onRadioItemBeforeChange(ev) {
    if (ev.checked) {
      const me = this,
        {
          lastValue
        } = me;
      if (!me.reverting && me.trigger('beforeChange', me.wrapRadioEvent(ev)) === false) {
        if (lastValue != null && lastValue !== me.value) {
          me.reverting = true;
          ev.source.uncheckToggleGroupMembers();
          me.value = lastValue;
          me.lastValue = lastValue;
          me.reverting = false;
          return false;
        }
      }
    }
  }
  onRadioItemChange(ev) {
    const me = this;
    if (ev.checked && !me.reverting) {
      me.triggerFieldChange(me.wrapRadioEvent(ev));
      me.lastValue = me.value;
    }
  }
  wrapRadioEvent(ev) {
    return {
      from: ev,
      item: ev.source,
      userAction: ev.userAction,
      lastValue: this.lastValue,
      value: this.value
    };
  }
  updateOptions() {
    const me = this,
      {
        options,
        refPrefix
      } = me,
      existingOptions = me.existingOptions.reduce((m, c) => {
        m[c.ref.substring(refPrefix.length)] = c;
        return m;
      }, {});
    let index = 0,
      key,
      option;
    if (options) {
      for (key in options) {
        option = me.convertOption(key, options[key], existingOptions[key]);
        delete existingOptions[key];
        me.insert(option, index++);
      }
    }
    const existing = Object.values(existingOptions);
    if (existing !== null && existing !== void 0 && existing.length) {
      me.remove(existing);
      existing.forEach(c => c.destroy());
    }
  }
  //endregion
}
// Register this widget type with its Factory
RadioGroup.initClass();
RadioGroup._$name = 'RadioGroup';

/**
 * @module Core/widget/Tab
 */
/**
 * This widget class is used to present items in a {@link Core.widget.TabPanel} on its {@link Core.widget.TabBar tabBar}.
 * A reference to this widget is stored via the {@link Core.widget.Widget#config-tab} config on the tab panel's items.
 *
 * ```javascript
 * let tabPanel = new TabPanel({
 *  items: [
 *      {
 *          title: 'Settings',
 *          // Tab configs
 *          tab : {
 *              // Show an icon in the tab
 *              icon : 'b-fa b-fa-cog'
 *          },
 *          items: [
 *              ...
 *          ]
 *      }
 *  ]
 * });
 * ```
 *
 * @extends Core/widget/Button
 * @classType tab
 */
class Tab extends Button {
  //region Config
  static get $name() {
    return 'Tab';
  }
  // Factoryable type name
  static get type() {
    return 'tab';
  }
  static get configurable() {
    return {
      /**
       * This config is set to `true` when this tab represents the `activeTab` of a {@link Core.widget.TabPanel}. It
       * is managed by the tab panel is not set directly.
       * @config {Boolean} active
       * @default false
       */
      active: null,
      /**
       * This config is set to the ordinal position of this tab in the {@link Core.widget.TabPanel}. It is managed
       * by the tab panel is not set directly.
       * @config {Number} index
       */
      index: null,
      /**
       * This config is set to `true` when this tab represents the first tab of a {@link Core.widget.TabPanel}. It
       * is managed by the tab panel is not set directly.
       * @config {Boolean} isFirst
       */
      isFirst: null,
      /**
       * This config is set to `true` when this tab represents the last tab of a {@link Core.widget.TabPanel}. It
       * is managed by the tab panel is not set directly.
       * @config {Boolean} isLast
       */
      isLast: null,
      /**
       * The {@link Core.widget.Widget} in the {@link Core.widget.TabPanel} corresponding to this tab. This is
       * managed by the tab panel is not set directly.
       * @config {Core.widget.Widget} item
       */
      item: {
        value: null,
        $config: 'nullify'
      },
      itemCls: null,
      /**
       * The tab panel that owns this tab.
       * @config {Core.widget.TabPanel} tabPanel
       */
      tabPanel: null,
      /**
       * The config property on this tab that will be set to the value of the {@link #config-titleSource} property
       * of this tab's {@link #config-item}.
       *
       * By default, the {@link #config-text} property of the tab is set to the {@link Core.widget.Widget#config-title}
       * property of its {@link #config-item}.
       * @config {String} titleProperty
       * @default
       */
      titleProperty: 'text',
      /**
       * The config property on this tab's {@link #config-item} that is used to set the value of the
       * {@link #config-titleProperty} of this tab.
       *
       * By default, the {@link #config-text} property of the tab is set to the {@link Core.widget.Widget#config-title}
       * property of its {@link #config-item}.
       * @config {String} titleSource
       * @default
       */
      titleSource: 'title',
      role: 'tab'
    };
  }
  compose() {
    const {
        active,
        cls,
        index,
        isFirst,
        isLast
      } = this,
      setSize = this.owner.visibleChildCount;
    return {
      tabindex: 0,
      'aria-selected': active,
      'aria-setsize': setSize,
      'aria-posinset': index + 1,
      class: {
        'b-tabpanel-tab': 1,
        'b-active': active,
        'b-tab-first': isFirst,
        'b-tab-last': isLast,
        ...cls // cls is a DomClassList
      },

      dataset: {
        index
      }
    };
  }
  //endregion
  updateIndex(index) {
    this.isFirst = !index;
  }
  updateItem(item, was) {
    var _me$itemChangeDetache, _me$itemHideDetacher;
    const me = this;
    if ((was === null || was === void 0 ? void 0 : was.tab) === me) {
      was.tab = null;
    }
    if (item) {
      item.tab = me;
      me[me.titleProperty] = item[me.titleSource];
      me.itemCls = item.cls;
      me.ariaElement.setAttribute('aria-controls', item.id);
      item.role = 'tabpanel';
    }
    (_me$itemChangeDetache = me.itemChangeDetacher) === null || _me$itemChangeDetache === void 0 ? void 0 : _me$itemChangeDetache.call(me);
    me.itemChangeDetacher = item && FunctionHelper.after(item, 'onConfigChange', 'onItemConfigChange', me, {
      return: false
    });
    (_me$itemHideDetacher = me.itemHideDetacher) === null || _me$itemHideDetacher === void 0 ? void 0 : _me$itemHideDetacher.call(me);
    me.itemHideDetacher = item === null || item === void 0 ? void 0 : item.ion({
      beforeChangeHidden: 'onItemBeforeChangeHidden',
      beforeHide: 'onItemBeforeHide',
      beforeUpdateDisabled: 'onItemBeforeUpdateDisabled',
      thisObj: me,
      prio: 1000 // We must know before the layout intercepts and activates a sibling
    });

    me.syncMinMax();
  }
  updateItemCls(cls, was) {
    const {
        element
      } = this,
      classList = element && DomClassList.from(element === null || element === void 0 ? void 0 : element.classList, /* returnEmpty= */true);
    if (element) {
      classList.remove(was).add(cls);
      element.className = classList.value;
    }
  }
  updateRotate(rotate, was) {
    if (!rotate !== !was) {
      this.syncMinMax();
    }
  }
  syncMinMax() {
    const me = this,
      {
        rotate,
        tabPanel
      } = me;
    // We have to read the configs directly since there are getters that read the DOM styles:
    let {
      _minWidth: minWidth,
      _minHeight: minHeight,
      _maxWidth: maxWidth,
      _maxHeight: maxHeight
    } = me;
    // When a tab rotation changes, we need to pivot the min/max width values with the height values
    if (tabPanel) {
      const {
        tabMinWidth,
        tabMaxWidth
      } = tabPanel;
      if (tabMinWidth != null) {
        if (rotate) {
          // if we were previously not rotated, the tabMinWidth may be effecting our minWidth:
          if (minWidth === tabMinWidth) {
            minWidth = null;
          }
          // noinspection JSSuspiciousNameCombination
          minHeight = tabMinWidth;
        } else {
          // if we were previously rotated, the tabMinWidth may be effecting our minHeight:
          if (minHeight === tabMinWidth) {
            minHeight = null;
          }
          minWidth = tabMinWidth;
        }
      }
      if (tabMaxWidth != null) {
        if (rotate) {
          if (maxWidth === tabMaxWidth) {
            maxWidth = null;
          }
          // noinspection JSSuspiciousNameCombination
          maxHeight = tabMaxWidth;
        } else {
          if (maxHeight === tabMaxWidth) {
            maxHeight = null;
          }
          maxWidth = tabMaxWidth;
        }
      }
      me.minWidth = minWidth;
      me.minHeight = minHeight;
      me.maxWidth = maxWidth;
      me.maxHeight = maxHeight;
    }
  }
  onItemBeforeChangeHidden({
    source: hidingChild,
    hidden
  }) {
    // If it's a hide/show that is not part of the layout's deactivating/activating, we must hide/show the tab
    if (!hidingChild.$isDeactivating && !hidingChild.$isActivating) {
      const {
        tabPanel
      } = this;
      this.hidden = hidden;
      // if tab to hide is active, let's active previous visible and enabled tab
      if (hidden && hidingChild === tabPanel.activeItem) {
        tabPanel.activateAvailableTab(hidingChild);
      }
    }
  }
  onItemBeforeHide() {
    // If it's a hide that is not part of the layout's deactivating, we hide the tab
    if (!this.item.$isDeactivating) {
      this.hide();
    }
  }
  onItemBeforeUpdateDisabled({
    source: disablingChild,
    disabled
  }) {
    const {
      tabPanel
    } = this;
    this.disabled = disabled;
    // if tab to disable is active, let's active previous visible and enabled tab
    if (disablingChild === tabPanel.activeItem) {
      tabPanel.activateAvailableTab(disablingChild);
    }
  }
  onItemConfigChange({
    name,
    value
  }) {
    if (name === this.titleSource) {
      this[this.titleProperty] = value;
    }
  }
}
// Register this widget type with its Factory
Tab.initClass();
Tab._$name = 'Tab';

/**
 * @module Core/widget/TabBar
 */
const isTab = t => t.isTab;
/**
 * A special toolbar used by {@link Core.widget.TabPanel} to present {@link Core.widget.Tab tabs} for the container's
 * items.
 *
 * The {@link Core.widget.Container#config-items} of a tab bar are typically managed by the tab panel, however,
 * items can be added that do not correspond to items in the tab panel. The {@link Core.widget.Widget#config-weight}
 * config of each tab defaults to 0 or the weight of its corresponding item.
 *
 * @extends Core/widget/Toolbar
 * @classType tabbar
 */
class TabBar extends Toolbar {
  static get $name() {
    return 'TabBar';
  }
  // Factoryable type name
  static get type() {
    return 'tabbar';
  }
  static get configurable() {
    return {
      defaultType: 'tab',
      overflow: 'scroll',
      role: 'tablist',
      ignoreParentReadOnly: true
    };
  }
  get firstTab() {
    return this.tabAt(0);
  }
  get lastTab() {
    return this.tabAt(-1);
  }
  get tabCount() {
    return this._items.countOf(isTab);
  }
  get tabs() {
    return ArrayHelper.from(this._items, isTab);
  }
  compose() {
    return {
      children: {
        toolbarContent: {
          class: {
            'b-tabpanel-tabs': 1
          }
        }
      }
    };
  }
  indexOfTab(tab) {
    return this._items.indexOf(tab, isTab);
  }
  onChildAdd(child) {
    super.onChildAdd(child);
    if (child.index == null) {
      this.syncTabs();
    }
  }
  onChildRemove(child) {
    super.onChildRemove(child);
    this.syncTabs();
  }
  onFocusIn() {
    const {
      activeIndex
    } = this.owner;
    // It must have a numeric active index set up
    if (!isNaN(activeIndex)) {
      this.tabs[activeIndex].focus();
    }
  }
  syncTabs() {
    const {
      tabs
    } = this;
    for (let i = 0, n = tabs.length; i < n; ++i) {
      tabs[i].index = i;
      tabs[i].isFirst = !i;
      tabs[i].isLast = i === n - 1;
    }
  }
  tabAt(index) {
    return this._items.find(isTab, index) || null;
  }
}
// Register this widget type with its Factory
TabBar.initClass();
TabBar._$name = 'TabBar';

/**
 * @module Core/widget/TabPanel
 */
/**
 * A tab panel widget which displays a collection of tabs, each of which can contain other widgets (or simple HTML). This
 * widget has a {@link Core.widget.TabBar tab bar} on top of its contents, and each {@link Core.widget.Tab tab} can be
 * customized using the {@link Core.widget.Tab#config-tab} config.
 *
 * @extends Core/widget/Container
 * @example
 * let tabPanel = new TabPanel({
 *  items: [
 *      {
 *          title: 'First',
 *          items: [
 *              { type: 'textfield', label: 'Name' },
 *              ...
 *          ]
 *      }, {
 *          title: 'Settings',
 *          tab : {
 *              // Show an icon in the tab
 *              icon : 'b-fa b-fa-cog'
 *          },
 *          items: [
 *              ...
 *          ]
 *      }
 *  ]
 * });
 *
 * The tab selector buttons are focusable elememts. `Enter` or `Space` activates a tab, and moves
 * focus into the newly visible tab item.
 *
 * @classType tabpanel
 * @inlineexample Core/widget/TabPanel.js
 * @widget
 */
class TabPanel extends Panel {
  //region Config
  static get $name() {
    return 'TabPanel';
  }
  // Factoryable type name
  static get type() {
    return 'tabpanel';
  }
  // Factoryable type alias
  static get alias() {
    return 'tabs';
  }
  static get configurable() {
    return {
      /**
       * The index of the initially active tab.
       * @member {Number} activeTab
       */
      /**
       * The index of the initially active tab.
       * @config {Number}
       * @default
       */
      activeTab: 0,
      /**
       * Specifies whether to slide tabs in and out of visibility.
       * @config {Boolean}
       * @default
       */
      animateTabChange: true,
      /**
       * Set the height of all tabs to match the tab with the highest content.
       * @config {Boolean}
       * @default
       */
      autoHeight: false,
      defaultType: 'container',
      focusable: false,
      itemCls: 'b-tabpanel-item',
      layout: {
        type: 'card'
      },
      // Prevent child panels from displaying a header unless explicitly configured with one
      suppressChildHeaders: true,
      tabBar: {
        type: 'tabbar',
        weight: -2000
      },
      /**
       * Min width of a tab title. 0 means no minimum width. This is default.
       * @config {Number}
       * @default
       */
      tabMinWidth: null,
      /**
       * Max width of a tab title. 0 means no maximum width. This is default.
       * @config {Number}
       * @default
       */
      tabMaxWidth: null
    };
  }
  //endregion
  //region Init
  /**
   * The active tab index. Setting must be done through {@link #property-activeTab}
   * @property {Number}
   * @readonly
   */
  get activeIndex() {
    return this.layout.activeIndex;
  }
  /**
   * The active child widget. Setting must be done through {@link #property-activeTab}
   * @property {Core.widget.Widget}
   * @readonly
   */
  get activeItem() {
    return this.layout.activeItem;
  }
  get activeTabItemIndex() {
    var _tabBar$tabs$activeTa;
    const {
      activeTab,
      items,
      tabBar
    } = this;
    return items.indexOf((_tabBar$tabs$activeTa = tabBar.tabs[activeTab]) === null || _tabBar$tabs$activeTa === void 0 ? void 0 : _tabBar$tabs$activeTa.item);
  }
  get bodyConfig() {
    return ObjectHelper.merge({
      className: {
        'b-tabpanel-body': 1
      }
    }, super.bodyConfig);
  }
  get focusElement() {
    var _activeTab$tab;
    const activeTab = this.items[this.activeTab || 0];
    return (activeTab === null || activeTab === void 0 ? void 0 : activeTab.focusElement) || (activeTab === null || activeTab === void 0 ? void 0 : (_activeTab$tab = activeTab.tab) === null || _activeTab$tab === void 0 ? void 0 : _activeTab$tab.focusElement);
  }
  get tabPanelBody() {
    return this.bodyElement;
  }
  finalizeInit() {
    super.finalizeInit();
    const me = this,
      {
        activeTab,
        layout
      } = me,
      {
        activeIndex
      } = layout,
      {
        tabs
      } = me.tabBar,
      activeTabItemIndex = activeTab >= 0 && activeTab < tabs.length && me.items.indexOf(tabs[activeTab].item);
    if (tabs.length > 0 && (activeTabItemIndex === false || activeTabItemIndex < 0)) {
      throw new Error(`Invalid activeTab ${activeTab} (${tabs.length} tabs)`);
    }
    if (activeTabItemIndex !== activeIndex) {
      // Since we are responding to configuration, we need to sync activeIndex to activeTab as if it were the
      // initial value of activeIndex. This cannot be done (reasonably) during initialization of the card layout
      // because of the possibility of tabless items, so we wait until the dust settles on the items, the tabBar
      // and all other configs, but we must do the tab change silently (since the initial active item is set
      // without such ceremony) and without animation (to avoid the appearance of the initial tab animating in)
      layout.setActiveItem(activeTabItemIndex, activeIndex, {
        animation: false,
        silent: true
      });
    }
    layout.animateCardChange = me.animateTabChange;
  }
  onChildAdd(child) {
    // The layout will hide inactive new items.
    // And we must add our beforeHide listener *after* call super.
    super.onChildAdd(child);
    if (!this.initialItems) {
      const me = this,
        {
          tabBar
        } = me,
        config = me.makeTabConfig(child),
        // if child.tab === false, config will be null... no tab for this one
        firstTab = config && (tabBar === null || tabBar === void 0 ? void 0 : tabBar.firstTab),
        // if there are no tabs yet, this will be the first so we can skip all the indexing...
        tabBarItems = firstTab && tabBar._items,
        // not all items have tabs but the new child won't have one yet:
        tabItems = firstTab && ArrayHelper.from(me._items, it => it.tab || it === child),
        // non-tabs could be in the tabBar, but the tabs must be contiguous:
        index = firstTab ? tabItems.indexOf(child) + tabBarItems.indexOf(firstTab) : 0;
      if (config && tabBar) {
        if (firstTab && child.weight == null && index < tabBarItems.count - 1) {
          tabBar.insert(config, index);
        } else {
          tabBar.add(config);
        }
      }
    }
  }
  onChildRemove(child) {
    const {
        tab
      } = child,
      {
        items
      } = this;
    if (tab) {
      this.tabBar.remove(tab);
      tab.destroy();
    }
    // Removing the active item, then show a sibling if any are left
    if (child === this.activeItem) {
      this._activeTab = null;
      if (items.length) {
        this.activeTab = items[Math.min(this.activeIndex, items.length - 1)];
      }
    }
    super.onChildRemove(child);
  }
  //endregion
  //region Tabs
  isDisabledOrHiddenTab(tabIndex) {
    const {
        tabs
      } = this.tabBar,
      tab = tabs === null || tabs === void 0 ? void 0 : tabs[tabIndex];
    return tab && (tab.disabled || tab.hidden);
  }
  findAvailableTab(item, delta = 1) {
    const {
        tabs
      } = this.tabBar,
      tabCount = tabs.length,
      itemIndex = Math.max(0, tabs.indexOf(item.tab));
    if (itemIndex) {
      delta = -delta;
    }
    let activeTab;
    for (let n = 1; n <= tabCount; ++n) {
      //  itemIndex=2, tabCount=5:
      //               n : 1, 2, 3, 4, 5
      //      delta =  1 : 3, 4, 0, 1, 2
      //      delta = -1 : 1, 0, 4, 3, 2
      activeTab = (itemIndex + (delta < 0 ? tabCount : 0) + n * delta) % tabCount;
      if (!this.isDisabledOrHiddenTab(activeTab)) {
        break;
      }
    }
    return activeTab;
  }
  activateAvailableTab(item, delta = 1) {
    this.activeTab = this.findAvailableTab(item, delta);
  }
  changeActiveTab(activeTab, oldActiveTab) {
    const me = this,
      {
        tabBar,
        layout
      } = me,
      {
        tabCount
      } = tabBar;
    if (activeTab.isWidget || ObjectHelper.isObject(activeTab)) {
      // Must be a child widget, so add if it's not already in our items.
      if (me.items.indexOf(activeTab) === -1) {
        activeTab = me.add(activeTab);
      }
      activeTab = tabBar.indexOfTab(activeTab.tab);
    } else {
      activeTab = parseInt(activeTab, 10);
    }
    if (!me.initialItems && tabCount > 0 && (activeTab < -1 || activeTab >= tabCount)) {
      throw new Error(`Invalid activeTab ${activeTab} (${tabCount} tabs)`);
    }
    if (me.isDisabledOrHiddenTab(activeTab)) {
      activeTab = me.findAvailableTab(activeTab);
    }
    // If we are animating, we must wait until any animation is finished
    // before we can go ahead and apply the change.
    if (layout.animateCardChange && layout.cardChangeAnimation) {
      layout.cardChangeAnimation.then(cardChange => {
        // If the animation resulted in not where we want, update the activeTab
        if ((cardChange === null || cardChange === void 0 ? void 0 : cardChange.activeIndex) !== activeTab) {
          me._activeTab = activeTab;
          me.updateActiveTab(activeTab, oldActiveTab);
        }
      });
    } else {
      return activeTab;
    }
  }
  async updateActiveTab(activeTab, was) {
    if (!this.initialItems) {
      const {
        activeTabItemIndex,
        layout
      } = this;
      if (activeTabItemIndex > -1) {
        const oldActiveItem = this.items[was],
          newActiveItem = this.items[activeTabItemIndex];
        // Avoid no-change
        if (layout.activeItem !== newActiveItem) {
          var _layout$setActiveItem;
          if (layout.animateCardChange) {
            await this.tabSelectionPromise;
          }
          // Focus the active tab's button in TabPanel first so that focus doesn't leave
          // the TabPanel when ths active tab hides.
          if (oldActiveItem !== null && oldActiveItem !== void 0 && oldActiveItem.containsFocus) {
            oldActiveItem.tab.focus();
          }
          this.tabSelectionPromise = (_layout$setActiveItem = layout.setActiveItem(newActiveItem)) === null || _layout$setActiveItem === void 0 ? void 0 : _layout$setActiveItem.promise;
        }
      }
    }
  }
  changeTabBar(bar) {
    this.getConfig('strips');
    this.strips = {
      tabBar: bar
    };
    return this.strips.tabBar;
  }
  makeTabConfig(item) {
    const {
        tab
      } = item,
      config = {
        item,
        type: 'tab',
        tabPanel: this,
        disabled: Boolean(item.disabled),
        hidden: item.initialConfig.hidden,
        weight: item.weight || 0,
        internalListeners: {
          click: 'onTabClick',
          thisObj: this
        },
        localizableProperties: {
          // our tabs copy their text from the item's title and so are not directly localized
          text: false
        }
      };
    if (tab === false) {
      return null;
    }
    return ObjectHelper.isObject(tab) ? Tab.mergeConfigs(config, tab) : config;
  }
  updateItems(items, was) {
    const me = this,
      {
        activeTab,
        initialItems
      } = me;
    let index = 0,
      tabs;
    super.updateItems(items, was);
    if (initialItems) {
      tabs = Array.from(items, it => me.makeTabConfig(it)).filter(it => {
        if (it) {
          it.index = index++;
          return true;
        }
      });
      if (index) {
        tabs[0].isFirst = true;
        tabs[index - 1].isLast = true;
        tabs[activeTab].active = true;
        me.tabBar.add(tabs);
        me.activeTab = activeTab; // now we can validate the activeTab value
      }
    }
  }

  updateTabMinWidth(tabMinWidth) {
    var _this$tabBar;
    (_this$tabBar = this.tabBar) === null || _this$tabBar === void 0 ? void 0 : _this$tabBar.items.forEach(tab => {
      if (tab.isTab) {
        tab.minWidth = tabMinWidth;
      }
    });
  }
  updateTabMaxWidth(tabMaxWidth) {
    var _this$tabBar2;
    (_this$tabBar2 = this.tabBar) === null || _this$tabBar2 === void 0 ? void 0 : _this$tabBar2.items.forEach(tab => {
      if (tab.isTab) {
        tab.maxWidth = tabMaxWidth;
      }
    });
  }
  //endregion
  //region Auto height
  updateAutoHeight(autoHeight) {
    this.detachListeners('themeAutoHeight');
    autoHeight && GlobalEvents.ion({
      name: 'themeAutoHeight',
      theme: 'internalOnThemeChange',
      thisObj: this
    });
    this.$measureHeight = autoHeight;
  }
  applyAutoHeight() {
    const me = this,
      {
        layout,
        activeTab,
        element
      } = me,
      {
        animateCardChange
      } = layout;
    // stop animate to change tabs on back stage.
    layout.animateCardChange = false;
    // override any previously applied height when measuring
    me.height = null;
    // get the max height comparing all tabs and apply to the tab
    me.height = Math.max(...me.items.map(tab => {
      me.activeTab = tab;
      return element.clientHeight;
    })) + 1;
    // Go back to initial configs
    me.activeTab = activeTab;
    layout.animateCardChange = animateCardChange;
    me.$measureHeight = false;
  }
  internalOnThemeChange() {
    if (this.isVisible) {
      this.applyAutoHeight();
    } else {
      this.$measureHeight = true;
    }
  }
  //endregion
  //region Events
  // Called after beforeActiveItemChange has fired and not been vetoed before animation and activeItemChange
  onBeginActiveItemChange(activeItemChangeEvent) {
    const tabs = this.tabBar.tabs,
      {
        activeItem,
        prevActiveItem
      } = activeItemChangeEvent;
    // Our UI changes immediately, our state must be accurate
    this.activeTab = tabs.indexOf(activeItem === null || activeItem === void 0 ? void 0 : activeItem.tab);
    // Deactivate previous active tab
    if (prevActiveItem !== null && prevActiveItem !== void 0 && prevActiveItem.tab) {
      prevActiveItem.tab.active = false;
    }
    if (activeItem !== null && activeItem !== void 0 && activeItem.tab) {
      activeItem.tab.active = true;
      activeItem.tab.show();
    }
  }
  // Auto called because Card layout triggers the beforeActiveItemChange on its owner
  onBeforeActiveItemChange(activeItemChangeEvent) {
    /**
     * The active tab is about to be changed. Return `false` to prevent this.
     * @event beforeTabChange
     * @preventable
     * @param {Number} activeIndex - The new active index.
     * @param {Core.widget.Widget} activeItem - The new active child widget.
     * @param {Number} prevActiveIndex - The previous active index.
     * @param {Core.widget.Widget} prevActiveItem - The previous active child widget.
     */
    return this.trigger('beforeTabChange', activeItemChangeEvent);
  }
  // Auto called because Card layout triggers the activeItemChange on its owner
  onActiveItemChange(activeItemChangeEvent) {
    /**
     * The active tab has changed.
     * @event tabChange
     * @param {Number} activeIndex - The new active index.
     * @param {Core.widget.Widget} activeItem - The new active child widget.
     * @param {Number} prevActiveIndex - The previous active index.
     * @param {Core.widget.Widget} prevActiveItem - The previous active child widget.
     */
    this.trigger('tabChange', activeItemChangeEvent);
  }
  onTabClick(event) {
    this.activeTab = event.source.item;
  }
  onPaint() {
    super.onPaint(...arguments);
    // Measure tabs on first paint if configured to do so
    if (this.$measureHeight) {
      this.applyAutoHeight();
    }
  }
  //endregion
}
// Register this widget type with its Factory
TabPanel.initClass();
TabPanel._$name = 'TabPanel';

export { FieldSet, Radio, RadioGroup, Tab, TabBar, TabPanel, WebSocketManager };
//# sourceMappingURL=TabPanel.js.map
