/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Base, DynamicObject, ObjectHelper, StringHelper, TextField, Delayable, BrowserHelper, FunctionHelper } from './Editor.js';

/**
 * @module Core/mixin/Featureable
 */
/**
 * This mixin provides management of a set of features that can be manipulated via the `features` config.
 *
 * The first step in using `Featureable` is to define the family of features using `Factoryable` to declare a base
 * class for features to extend:
 *
 * ```javascript
 *  class SuperWidgetFeature extends InstancePlugin.mixin(Factoryable) {
 *      static get factoryable() {
 *          //
 *      }
 *  }
 * ```
 *
 * The various feature classes extend the `SuperWidgetFeature` base class and call `initClass()` to register themselves:
 *
 * ```javascript
 *  export default class AmazingSuperWidgetFeature extends SuperWidgetFeature {
 *      static get type() {
 *          return 'amazing';
 *      }
 *  }
 *
 *  AmazingSuperWidgetFeature.initClass();
 * ```
 *
 * A class that supports these features via `Featureable` is declared like so:
 *
 * ```javascript
 *  class SuperWidget extends Widget.mixin(Featureable) {
 *      static get featureable() {
 *          return {
 *              factory : SuperWidgetFeature
 *          };
 *      }
 *
 *      static get configurable() {
 *          return {
 *              // Declare the default features. These can be disabled by setting them to a falsy value. Using
 *              // configurable(), the value defined by this class is merged with values defined by derived classes
 *              // and ultimately the instance.
 *              features : {
 *                  amazing : {
 *                      ...
 *                  }
 *              }
 *          };
 *      }
 *  }
 *```
 * @mixin
 * @internal
 */
var Featureable = (Target => class Featureable extends (Target || Base) {
  static get $name() {
    return 'Featureable';
  }
  static get configurable() {
    return {
      /**
       * Specifies the features to create and associate with the instance. The keys of this object are the names
       * of features. The values are config objects for those feature instances.
       *
       * After construction, this property can be used to access the feature instances and even reconfigure them.
       *
       * For example:
       * ```
       *  instance.features.amazing = {
       *      // reconfigure this feature
       *  }
       * ```
       * This can also be done in bulk:
       * ```
       *  instance.features = {
       *      amazing : {
       *          // reconfigure this feature
       *      },
       *      // reconfigure other features
       *  }
       * ```
       * @config {Object}
       */
      features: null
    };
  }
  static get declarable() {
    return [
    /**
     * This property getter returns options that control feature management for the derived class. This
     * property getter must be defined by the class that mixes in `Featureable` in order to initialize the
     * class properly.
     * ```
     *  class SuperWidget extends Widget.mixin(Featureable) {
     *      static get featureable() {
     *          return {
     *              factory : SuperWidgetFeature
     *          };
     *      }
     *      ...
     *  }
     * ```
     * @static
     * @member {Object} featureable
     * @property {Core.mixin.Factoryable} featureable.factory The factoryable class (not one of its instances)
     * that will be used to create feature instances.
     * @property {String} [featureable.ownerName='client'] The config or property to assign on each feature as
     * a reference to its creator, the `Featureable` instance.
     * @internal
     */
    'featureable'];
  }
  static setupFeatureable(cls) {
    const featureable = {
      ownerName: 'client',
      ...cls.featureable
    };
    featureable.factory.initClass();
    // Replace the class/static getter with a new one that returns the complete featureable object:
    Reflect.defineProperty(cls, 'featureable', {
      get() {
        return featureable;
      }
    });
  }
  doDestroy() {
    const features = this.features;
    super.doDestroy();
    for (const name in features) {
      var _feature$destroy;
      const feature = features[name];
      // Feature might be false or destroyed already by Grid (EventList mixes in CalendarMixin which has this mixin)
      (_feature$destroy = feature.destroy) === null || _feature$destroy === void 0 ? void 0 : _feature$destroy.call(feature);
    }
  }
  /**
   * Returns `true` if the specified feature is active for this instance and `false` otherwise.
   * @param {String} name The feature name
   * @returns {Boolean}
   */
  hasFeature(name) {
    var _this$features;
    return Boolean((_this$features = this.features) === null || _this$features === void 0 ? void 0 : _this$features[name]);
  }
  changeFeatures(features, was) {
    if (this.isDestroying) {
      return;
    }
    const me = this,
      {
        featureable
      } = me.constructor,
      manager = me.$features || (me.$features = new DynamicObject({
        configName: 'features',
        factory: featureable.factory,
        owner: me,
        ownerName: featureable.ownerName
      }));
    manager.update(features);
    if (!was) {
      // Only return the target once. Further calls are processed above so we need to return undefined to ensure
      // onConfigChange is called. By returning the same target on 2nd+ call, it passes the === test and won't
      // trigger onConfigChange.
      return manager.target;
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
  getCurrentConfig(options) {
    const result = super.getCurrentConfig(options),
      {
        features
      } = result;
    if (features) {
      // Replace empty configs with `true`
      for (const featureName in features) {
        if (Object.keys(features[featureName]).length === 0) {
          features[featureName] = true;
        }
      }
    }
    return result;
  }
});

/**
 * @module Core/mixin/Fencible
 */
const {
    defineProperty
  } = Object,
  {
    hasOwn
  } = ObjectHelper,
  fencibleSymbol = Symbol('fencible'),
  NONE = [],
  distinct = array => Array.from(new Set(array)),
  parseNames = names => names ? distinct(StringHelper.split(names)) : NONE,
  fenceMethod = (target, name, options) => {
    if (options === true) {
      options = name;
    }
    if (!ObjectHelper.isObject(options)) {
      options = {
        all: options
      };
    }
    let any = parseNames(options.any);
    const all = parseNames(options.all),
      lock = options.lock ? parseNames(options.lock) : distinct(all.concat(any)),
      implName = name + 'Impl',
      fence = function (...params) {
        // cannot use => since we need to receive "this" from the caller
        const me = this,
          // For static methods we have to be careful to use hasOwn to check the "point of entry" (i.e., the
          // class reference used to call the method) since "." will climb the constructor's __proto__ chain
          // to find properties from a super class. This does not happen to instances since we never put our
          // fences object on the prototype chain.
          fences = hasOwn(me, fencibleSymbol) ? me[fencibleSymbol] : me[fencibleSymbol] = {},
          isFree = key => !fences[key];
        if (all.every(isFree) && (!any || any.some(isFree))) {
          try {
            lock.forEach(key => fences[key] = (fences[key] || 0) + 1);
            return me[implName](...params);
          } finally {
            lock.forEach(key => --fences[key]);
          }
        }
      };
    any = any.length ? any : null; // [].some(f) is always false, but [].every(f) is always true
    !target[implName] && defineProperty(target, implName, {
      configurable: true,
      value: target[name]
    });
    defineProperty(target, name, {
      configurable: true,
      value: fence
    });
  };
/**
 * A description of how to protect a method from reentry.
 *
 * A value of `true` is transformed using the key as the `all` value. For example, this:
 *
 * ```javascript
 *  class Foo extends Base.mixin(Fencible) {
 *      static fenced = {
 *          foo : true
 *      };
 * ```
 *
 * Is equivalent to this:
 *
 * ```javascript
 *  class Foo extends Base.mixin(Fencible) {
 *      static fenced = {
 *          foo : {
 *              all : ['foo']
 *          }
 *      };
 * ```
 *
 * Strings are split on spaces to produce the `all` array. For example, this:
 *
 * ```javascript
 *  class Foo extends Base.mixin(Fencible) {
 *      static fenced = {
 *          foo : 'foo bar'
 *      };
 * ```
 *
 * Is equivalent to this:
 *
 * ```javascript
 *  class Foo extends Base.mixin(Fencible) {
 *      static fenced = {
 *          foo : {
 *              all : ['foo', 'bar']
 *          }
 *      };
 * ```
 *
 * This indicates that `foo()` cannot be reentered if `foo()` or `bar()` are already executing. On entry to `foo()`,
 * both `foo()` and `bar()` will be fenced (prevented from entering).
 *
 * @typedef {Object} MethodFence
 * @property {String|String[]} [all] One or more keys that must all be currently unlocked to allow entry to the fenced
 * method. String values are converted to an array by splitting on spaces.
 * @property {String|String[]} [any] One or more keys of which at least one must be currently unlocked to allow entry
 * to the fenced method. String values are converted to an array by splitting on spaces.
 * @property {String|String[]} [lock] One or more keys that will be locked on entry to the fenced method and released
 * on exit. String values are converted to an array by splitting on spaces. By default, this array includes all keys
 * in `all` and `any`.
 */
/**
 * This mixin is used to apply reentrancy barriers to methods. For details, see
 * {@link Core.mixin.Fencible#property-fenced-static}.
 * @mixin
 * @internal
 */
var Fencible = (Target => class Fencible extends (Target || Base) {
  static $name = 'Fencible';
  static declarable = [
  /**
   * This class property returns an object that specifies methods to be wrapped to prevent reentrancy.
   *
   * It is used like so:
   * ```javascript
   *  class Foo extends Base.mixin(Fencible) {
   *      static fenced = {
   *          reentrantMethod : true
   *      };
   *
   *      reentrantMethod() {
   *          // things() may cause reentrantMethod() to be called...
   *          // but we won't be allowed to reenter this method since we are already inside it
   *          this.things();
   *      }
   *  }
   * ```
   *
   * This can also be used to protect mutually reentrant method groups:
   *
   * ```javascript
   *  class Foo extends Base.mixin(Fencible) {
   *      static fenced = {
   *          foo : 'foobar'
   *          bar : 'foobar'
   *      };
   *
   *      foo() {
   *          console.log('foo');
   *          this.bar();
   *      }
   *
   *      bar() {
   *          console.log('bar');
   *          this.foo();
   *      }
   *  }
   *
   *  instance = new Foo();
   *  instance.foo();
   *  >> foo
   *  instance.bar();
   *  >> bar
   * ```
   *
   * The value for a fenced method value can be `true`, a string, an array of strings, or a
   * {@link #typedef-MethodFence} options object.
   *
   * Internally these methods are protected by assigning a wrapper function in their place. The original function
   * is moved to a new named property by appending 'Impl' to the original name. For example, in the above code,
   * `foo` and `bar` are wrapper functions that apply reentrancy protection and call `fooImpl` and `barImpl`,
   * respectively. This is important for inheritance and `super` calling because the new name must be used in
   * order to retain the guard function implementations.
   *
   * @static
   * @member {Object} fenced
   * @internal
   */
  'fenced'];
  static setupFenced(cls) {
    let {
      fenced
    } = cls;
    const statics = fenced.static,
      pairs = [];
    if (statics) {
      fenced = {
        ...fenced
      };
      delete fenced.static;
      pairs.push([statics, cls]);
    }
    pairs.push([fenced, cls.prototype]);
    for (const [methods, target] of pairs) {
      for (const methodName in methods) {
        fenceMethod(target, methodName, methods[methodName]);
      }
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Core/widget/FilterField
 */
/**
 * A simple text field for filtering a store.
 *
 * Allows filtering by {@link #config-field field}:
 *
 * ```javascript
 * const filterField = new FilterField({
 *    store : eventStore,
 *    field : 'name'
 * });
 * ```
 *
 * Or by using a {@link #config-filterFunction filter function} for greater control/custom logic:
 *
 * ```javascript
 * const filterField = new FilterField({
 *    store          : eventStore,
 *    filterFunction : (record, value) => record.name.includes(value)
 * });
 * ```
 *
 * @extends Core/widget/TextField
 * @classType filterfield
 * @widget
 */
class FilterField extends TextField {
  static get $name() {
    return 'FilterField';
  }
  // Factoryable type name
  static get type() {
    return 'filterfield';
  }
  static get configurable() {
    return {
      /**
       * The model field name to filter by. Can optionally be replaced by {@link #config-filterFunction}
       * @config {String}
       * @category Filtering
       */
      field: null,
      /**
       * The store to filter.
       * @config {Core.data.Store}
       * @category Filtering
       */
      store: null,
      /**
       * Optional filter function to be called with record and value as parameters for store filtering.
       * ```javascript
       * {
       *     type           : 'filterfield',
       *     store          : myStore,
       *     filterFunction : (record, value)  => {
       *        return record.text.includes(value);
       *     }
       * }
       * ```
       * @param {Core.data.Model} record Record for comparison
       * @param {String} value Value to compare with
       * @returns {Boolean} Return true if record matches comparison requirements
       * @config {Function}
       * @category Filtering
       */
      filterFunction: null,
      clearable: true,
      revertOnEscape: true,
      keyStrokeChangeDelay: 100,
      onChange({
        value
      }) {
        const {
          store,
          field,
          filterFunction
        } = this;
        if (store) {
          const filterId = `${field || this.id}-Filter`;
          if (value.length === 0) {
            store.removeFilter(filterId);
          } else {
            let filterBy;
            if (filterFunction) {
              filterBy = record => filterFunction(record, value);
            } else {
              // We filter using a RegExp, so quote significant characters
              value = value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
              filterBy = record => {
                var _record$get;
                return (_record$get = record.get(field)) === null || _record$get === void 0 ? void 0 : _record$get.match(new RegExp(value, 'i'));
              };
            }
            // A filter with an id replaces any previous filter with that id.
            // Leave any other filters which may be in use in place.
            store.filter({
              id: filterId,
              filterBy
            });
          }
        }
      }
    };
  }
  updateValue(value, old) {
    super.updateValue(value, old);
    // Initial value, apply it
    if (value && this.isConfiguring) {
      this.onChange({
        value
      });
    }
  }
}
FilterField.initClass();
FilterField._$name = 'FilterField';

/**
 * @module Core/widget/mixin/Responsive
 */
const EMPTY = [],
  isStateName = name => name[0] !== '*',
  pop = (object, key) => {
    const ret = object[key] || null;
    delete object[key];
    return ret;
  },
  responsiveRootFn = widget => widget.responsiveRoot,
  scoring = {
    number: threshold => ({
      width
    }) => width <= threshold && threshold
  },
  splitConfigs = configs => {
    delete configs.once;
    return {
      callback: pop(configs, 'callback'),
      configs,
      when: pop(configs, 'when')
    };
  },
  // We cheat a bit and leave "once", "when" and "callback" in the config object for the merge.
  splitMergedConfigs = (cls, ...parts) => {
    const once = parts.flatMap(p => (p === null || p === void 0 ? void 0 : p.once) || EMPTY),
      // onces are not arrays, so are unchanged; ==> filter().map()
      configs = cls.mergeConfigs(...parts),
      ret = splitConfigs(configs);
    ret.once = once.length ? splitConfigs(cls.mergeConfigs(...once)) : null;
    return ret;
  },
  // Allow responsiveTarget to be a DOM element? (see git history for wrapElement impl)
  wrapWidget = (widget, handler) => {
    let triggers,
      desc = Proxy.revocable(widget, {
        get(o, name) {
          if (triggers) {
            triggers[name] = true;
          }
          return widget[name];
        }
      }),
      detacher = FunctionHelper.after(widget, 'onConfigChange', (ignore, {
        name
      }) => {
        var _triggers;
        if ((_triggers = triggers) !== null && _triggers !== void 0 && _triggers[name]) {
          handler();
        }
      }),
      resizer = widget.ion({
        resize: () => {
          handler();
        }
      });
    widget.monitorResize = true;
    return {
      widget,
      get object() {
        var _desc;
        return (_desc = desc) === null || _desc === void 0 ? void 0 : _desc.proxy;
      },
      destroy() {
        if (desc) {
          desc.revoke();
          detacher();
          resizer();
          desc = detacher = resizer = null;
        }
      },
      reset() {
        triggers = Object.create(null);
      }
    };
  };
/**
 * A state definition object used by the {@link Core.widget.mixin.Responsive#config-responsive} config property.
 *
 * ```javascript
 *  {
 *      responsive : {
 *          small : {
 *              // a ResponsiveState object
 *              when : 400,
 *
 *              callback() {
 *                  console.log('Applied small not first time');
 *              },
 *
 *              once : {
 *                  mode : 'full',
 *
 *                  callback() {
 *                      console.log('Applied small first time');
 *                  }
 *              }
 *
 *              // All other properties are configs to apply when
 *              // the state is active
 *              text  : null,
 *              color : 'b-blue'
 *          }
 *      }
 *  }
 * ```
 *
 * See {@link Core.widget.mixin.Responsive} for more details.
 *
 * @typedef ResponsiveState
 * @property {ResponsiveState} once A `ResponsiveState` object applied only one time when a state is first activated. It
 * is not valid to specify a `when` or `once` property on these `ResponsiveState` objects. It is valid to supply a
 * `callback`, and if done, this callback will be called instead of the `callback` on the parent `ResponsiveState`
 * object on first activation.
 * @property {Function|Number} when A two argument function to return the score for the state, or a number for both the
 * width threshold and score. The arguments passed are as follows:
 *  - `widget` The {@link Core.widget.mixin.Responsive#config-responsiveTarget widget} whose properties should determine
 *  the state's score
 *  - `browserHelper` The {@link Core.helper.BrowserHelper} singleton object
 * @property {Function} [callback] An optional callback, called when the state is activated. This function receives an
 * object with the following properties:
 *  - `source` The instance whose state is being activated (typically a {@link Core.widget.Widget})
 *  - `target` The {@link Core.widget.Widget} identified as the {@link Core.widget.mixin.Responsive#config-responsiveTarget}
 *  - `state` The name of the newly active responsive state
 *  - `oldState` The name of the previously active responsive state
 */
/**
 * A breakpoint definition. Used when defining breakpoints, see {@link #config-breakpoints}.
 *
 * ```javascript
 * {
 *     name    : 'Small',
 *     configs : {
 *         text  : null,
 *         color : 'b-blue'
 *     },
 *     callback() {
 *         console.log('Applied small');
 *     }
 * }
 * ```
 *
 * @typedef Breakpoint
 * @property {String} name Name of the breakpoint
 * @property {Object} [configs] An optional configuration object to apply to the widget when the breakpoint is activated
 * @property {Function} [callback] An optional callback, called when the breakpoint is activated
 * @deprecated 5.0 Breakpoints have been replaced by {@link Core.widget.mixin.Responsive#config-responsive}.
 */
/**
 * This mixin provides management of a named set of {@link #typedef-ResponsiveState} objects that are conditionally
 * applied in response to the widget's size or other platform details. The names of the {@link #typedef-ResponsiveState}
 * objects are the keys of the {@link #config-responsive} config object. For example:
 *
 * ```javascript
 *  class ResponsiveButton extends Button.mixin(Responsive) {
 *      static configurable = {
 *          responsive : {
 *              small : {
 *                  // this is a ResponsiveState object named "small"
 *                  text : 'S'
 *              },
 *              medium : {
 *                  text : 'M'
 *              }
 *              large : {
 *                  text : 'L'
 *              }
 *          }
 *      };
 *  }
 * ```
 *
 * When the conditions are right for the button to be in the `'small'` responsive state, the `text` config will be set
 * to `'S'`.
 *
 * Any desired configs can be present in a {@link #typedef-ResponsiveState} object, however, the `when` and `callback`
 * properties have special meaning to this mixin and are reserved.
 *
 * ## Selecting the Responsive State
 *
 * To determine the current responsive state, the `when` property is consulted for each candidate state.
 *
 * If `when` is a number, it is understood to be a width threshold and, if the widget's `width` is equal or less than
 * that value, the score is that value. For example, a value of 400 would produce a score of 400 if the widget's width
 * were less than or equal to 400. If the widget's width is greater than 400, the state would be skipped.
 *
 * If `when` is a function, it is called with two parameters: a readonly reference to the widget and the
 * {@link Core.helper.BrowserHelper} singleton object. The function should return the numeric score if the state is
 * applicable, or `null` or `false` if the state should be skipped.
 *
 * The state that has the minimum score is selected as the responsive state for the widget.
 *
 * Consider the default responsive states and their `when` values:
 *
 * ```javascript
 *  responsive : {
 *      small : {
 *          when : 400
 *      },
 *
 *      medium : {
 *          when : 800
 *      },
 *
 *      large : {
 *          when : () => Infinity
 *      },
 *
 *      '*' : {}
 *  },
 * ```
 *
 * For example, if the width of the widget is 300: the score for the `small` responsive state is 400, the score for
 * the `medium` responsive state is 800, and the score for `large` is infinity. In effect, the `large` state is always
 * a candidate, but will also always lose to other candidate states. In this case, the `small` state has the minimum
 * score and is selected as the responsive state.
 *
 * If the width of the widget is 600: the `small` state would be skipped, while the `medium` and `large` states would
 * produce the same scores resulting in `medium` being the responsive state.
 *
 * The `when` functions have access to any properties of the widget instance in the first argument, but are also passed
 * the {@link Core.helper.BrowserHelper} singleton as a second argument. This can be used as shown in the following,
 * over-simplified example:
 *
 * ```javascript
 *  class ResponsiveWidget extends Widget.mixin(Responsive) {
 *      static configurable = {
 *          responsive : {
 *              small : {
 *                  when : ({ width }, { isMobileSafari }) => isMobileSafari && width <= 600 && 10
 *                  text : 'iPhone'
 *              },
 *              medium : {
 *                  when : ({ width }, { isMobileSafari }) => isMobileSafari && width <= 1024 && 20
 *                  text : 'iPad'
 *              }
 *              large : {
 *                  text : 'Desktop'
 *              }
 *          }
 *      };
 *  }
 * ```
 *
 * It is best to avoid mixing `when` threshold values and `when` functions as the resulting scores can be confusing.
 * @mixin
 */
var Responsive = (Target => class Responsive extends (Target || Base).mixin(Delayable, Fencible) {
  static $name = 'Responsive';
  static configurable = {
    /**
     * Specifies the various responsive state objects keyed by their name. Each key (except `'*'`, see below) in
     * this object is a state name (see {@link #config-responsiveState}) and its corresponding value is the
     * associated {@link #typedef-ResponsiveState} object.
     *
     * Some properties of a `ResponsiveState` object are special, for example `when` and `callback`. All other
     * properties of the state object are config properties to apply when that state is active.
     *
     * The `when` property can be a function that computes the score for the state. The state whose `when` function
     * returns the lowest score is selected and its non-special properties will be assigned to the instance. If
     * `when` is a number, it will be converted into a scoring function (see below).
     *
     * A `when` function accepts two readonly parameters and returns either a numeric score if the state should be
     * considered, or `false` or `null` if the state should be ignored (i.e., it does match with the current state).
     *
     * The first parameter is a readonly proxy for the {@link #config-responsiveTarget widget} whose size and other
     * properties determine the state's score. The proxy tracks property access to that widget in order to update
     * the responsive state should any of those properties change.
     *
     * The second argument to a `when` function is the {@link Core.helper.BrowserHelper} singleton. This allows
     * a `when` function to conveniently test platform and browser information.
     *
     * The state whose `when` function returns the lowest score is selected as the new
     * {@link #config-responsiveState} and its config object (minus the `when` function and other special
     * properties) is applied to the instance.
     *
     * If `when` is a number, it is converted to function. The following two snippets produce the same `when`
     * scoring:
     *
     * ```javascript
     *      small : {
     *          when : 400,
     *          ...
     *      }
     * ```
     *
     * The above converted to:
     *
     * ```javascript
     *      small : {
     *          when : ({ width }) => width <= 400 && 400,
     *          ...
     *      }
     * ```
     * Selecting the lowest score as the winner allows for the simple conversion of width threshold to score value,
     * such that the state with the smallest matching width is selected.
     *
     * If the `responsive` config object has an asterisk key (`'*'`), its value is used as the default set of config
     * properties to apply all other states. This will be the only config properties to apply if no `when` function
     * returns a score. In this way, this special state object acts as a default state as well as a set of
     * default values for other states to share. This state object has no `when` function.
     *
     * The default for this config is:
     * ```javascript
     *  {
     *      small : {
     *          when : 400
     *      },
     *
     *      medium : {
     *          when : 800
     *      },
     *
     *      large : {
     *          when : () => Infinity
     *      },
     *
     *      '*' : {}
     *  }
     * ```
     *
     * A derived class (or instance) can use these states by populating other config properties, define
     * additional states, and/or adjust the `when` properties to use different size thresholds.
     *
     * @config {Object}
     */
    responsive: {
      $config: {
        lazy: 'paint'
      },
      value: null
    },
    /**
     * The defaults for the {@link #config-responsive} config. These are separated so that the act of setting the
     * {@link #config-responsive} config is what triggers additional processing.
     * @config {Object}
     * @internal
     * @default
     */
    responsiveDefaults: {
      small: {
        when: 400
      },
      medium: {
        when: 800
      },
      large: {
        when: () => Infinity
      },
      '*': {}
    },
    /**
     * Set to `true` to mark this instance as the default {@link #config-responsiveTarget} for descendants that do
     * not specify an explicit {@link #config-responsiveTarget} of their own.
     * @config {Boolean}
     * @default false
     */
    responsiveRoot: null,
    /**
     * The name of the active state of the {@link #config-responsive} config. This is assigned internally
     * and should not be assigned directly.
     *
     * @config {String}
     * @readonly
     */
    responsiveState: null,
    /**
     * The widget whose size and other properties drive this object's responsive behavior. If this config is not
     * specified, the closest ancestor that specified {@link #config-responsiveRoot responsiveRoot=true} will be
     * used. If there is no such ancestor, then the instance using this mixin is used.
     *
     * If this value is set to `'@'`, then this instance is used even if there is a {@link #config-responsiveRoot}
     * ancestor.
     *
     * If this config is a string that starts with `'@'`, the text following the first character is the name of the
     * property on this instance that holds the target to use. For example, `'@owner'` to use the value of the
     * `owner` property as the responsive target.
     *
     * If this config is a string that does not start with `'@'`, that string is passed to
     * {@link Core.widget.Widget#function-up} to find the closest matching ancestor.
     *
     * If another widget is used as the `responsiveTarget` and if this instance does not specify any explicit `when`
     * properties in its {@link #config-responsive} config, then the `when` definitions of the `responsiveTarget`
     * will be used for this instance.
     * @config {String|Core.widget.Widget}
     */
    responsiveTarget: {
      value: null,
      $config: {
        lazy: 'paint'
      }
    },
    responsiveWidget: {
      value: null,
      $config: {
        nullify: true
      }
    },
    /**
     * Defines responsive breakpoints, based on max-width or max-height.
     *
     * When the widget is resized, the defined breakpoints are queried to find the closest larger or equal
     * breakpoint for both width and height. If the found breakpoint differs from the currently applied, it is
     * applied.
     *
     * Applying a breakpoint triggers an event that applications can catch to react to the change. It also
     * optionally applies a set of configs and calls a configured callback.
     *
     * ```javascript
     * breakpoints : {
     *     width : {
     *         50 : { name : 'small', configs : { text : 'Small', ... } }
     *         100 : { name : 'medium', configs : { text : 'Medium', ... } },
     *         '*' : { name : 'large', configs : { text : 'Large', ... } }
     *     }
     * }
     * ```
     *
     * @config {Object}
     * @param {Object} width Max-width breakpoints, with keys as numerical widths (or '*' for larger widths than the
     * largest defined one) and the value as a {@link #typedef-Breakpoint breakpoint definition}
     * @param {Object} height Max-height breakpoints, with keys as numerical heights (or '*' for larger widths than
     * the largest defined one) and the value as a {@link #typedef-Breakpoint breakpoint definition}
     * @deprecated 5.0 Use {@link #config-responsive} instead.
     */
    breakpoints: null
  };
  static delayable = {
    responsiveUpdate: 'raf'
  };
  static fenced = {
    syncResponsiveWidget: true
  };
  static prototypeProperties = {
    responsiveStateChanges: 0,
    responsiveUpdateCount: 0
  };
  get isResponsivePending() {
    return this.responsiveUpdateCount === 0 && this.hasConfig('responsive');
  }
  get isResponsiveUpdating() {
    var _this$responsiveWidge;
    return this._responsiveUpdating || ((_this$responsiveWidge = this.responsiveWidget) === null || _this$responsiveWidge === void 0 ? void 0 : _this$responsiveWidge._responsiveUpdating);
  }
  // responsive
  updateResponsive(responsive) {
    const me = this,
      cls = me.constructor,
      {
        responsiveDefaults
      } = me,
      stateNames = Array.from(new Set(ObjectHelper.keys(responsive).concat(ObjectHelper.keys(responsiveDefaults)))).filter(isStateName);
    let states = null,
      hasWhen,
      name,
      state,
      when;
    if (responsive) {
      states = {
        '*': splitMergedConfigs(cls, responsiveDefaults['*'], responsive['*'])
      };
      for (name of stateNames) {
        state = responsive[name];
        if (state !== null && state !== false) {
          var _scoring$when;
          // Track whether any state has an explicit "when" property
          hasWhen = hasWhen || state && 'when' in state;
          states[name] = splitMergedConfigs(cls, responsiveDefaults['*'], responsiveDefaults[name], responsive['*'], state);
          when = states[name].when;
          states[name].when = ((_scoring$when = scoring[typeof when]) === null || _scoring$when === void 0 ? void 0 : _scoring$when.call(scoring, when)) || when; // convert numbers to fns based on width
        }
      }
    }

    me.$responsiveStates = states;
    me.$responsiveWhen = hasWhen;
    me.syncResponsiveWidget();
  }
  // responsiveState
  updateResponsiveState(state, oldState) {
    var _me$element;
    const me = this,
      {
        $responsiveStates: states
      } = me,
      initial = ++me.responsiveStateChanges === 1,
      classList = (_me$element = me.element) === null || _me$element === void 0 ? void 0 : _me$element.classList,
      defaults = states['*'],
      def = states[state] || defaults,
      once = initial && (def.once || defaults.once),
      isStateful = initial && me.isStateful,
      target = me.responsiveWidget;
    let config = def.configs,
      otherConfigs = once === null || once === void 0 ? void 0 : once.configs;
    if (otherConfigs) {
      // overlay ":once" configs on normal configs (the mergeConfigs method clones the first parameter before
      // merging it with other values)
      config = config ? me.constructor.mergeConfigs(config, otherConfigs) : otherConfigs;
    }
    oldState && (classList === null || classList === void 0 ? void 0 : classList.remove(`b-responsive-${oldState.toLowerCase()}`));
    state && (classList === null || classList === void 0 ? void 0 : classList.add(`b-responsive-${state.toLowerCase()}`));
    if (isStateful) {
      // our responsiveState is munged with the stateId to retrieve state info for this responsiveState (to track
      // state by small/medium/large/etc). If we don't load the state on each call here we would end up smashing
      // the values saved in state with those defined by the developer in the responsive config.
      otherConfigs = me.loadStatefulData();
      if (otherConfigs) {
        // if there is stateful data for this responsiveState, it takes priority over our config object
        config = config ? me.constructor.mergeConfigs(config, otherConfigs) : otherConfigs;
      }
      // We don't want responsive changes to configs to trigger state save:
      me.suspendStateful();
    }
    me._responsiveUpdating = true;
    try {
      var _me$trigger, _def$callback, _once$callback, _me$trigger2;
      /**
       * Triggered before a new {@link #config-responsiveState} is applied.
       * @event beforeResponsiveStateChange
       * @param {Core.widget.Widget} source The widget whose `responsiveState` is to be changed
       * @param {String} state The new value for the widget's `responsiveState`
       * @param {String} oldState The previous value for the widget's `responsiveState`
       */
      (_me$trigger = me.trigger) === null || _me$trigger === void 0 ? void 0 : _me$trigger.call(me, 'beforeResponsiveStateChange', {
        state,
        oldState,
        target
      });
      config && me.setConfig(config);
      (_def$callback = def.callback) === null || _def$callback === void 0 ? void 0 : _def$callback.call(def, {
        source: me,
        state,
        oldState,
        target,
        initial
      });
      once === null || once === void 0 ? void 0 : (_once$callback = once.callback) === null || _once$callback === void 0 ? void 0 : _once$callback.call(once, {
        source: me,
        state,
        oldState,
        target,
        initial
      });
      /**
       * Triggered when a new {@link #config-responsiveState} is applied.
       * @event responsiveStateChange
       * @param {Core.widget.Widget} source The widget whose `responsiveState` has changed
       * @param {String} state The new value for the widget's `responsiveState`
       * @param {String} oldState The previous value for the widget's `responsiveState`
       */
      (_me$trigger2 = me.trigger) === null || _me$trigger2 === void 0 ? void 0 : _me$trigger2.call(me, 'responsiveStateChange', {
        state,
        oldState,
        target
      });
      // we normally would check for !me.isConstructing or !me.isConfiguring but this event needs to be fired
      // during that time to allow the app to receive the initial responsive state since it is dynamic (i.e.,
      // not something the app has configured into the widget)
    } finally {
      // Be sure to reset these even if an exception occurs
      me._responsiveUpdating = false;
      isStateful && me.resumeStateful();
    }
  }
  // responsiveTarget
  get responsiveTarget() {
    return this.responsiveWidget || this._responsiveTarget;
  }
  updateResponsiveTarget() {
    this.syncResponsiveWidget();
  }
  // responsiveWidget
  updateResponsiveWidget(target) {
    var _me$$responsiveWrappe;
    const me = this,
      // being a delayable raf method effectively auto-bind's our this pointer
      responsiveUpdate = target && me.responsiveUpdate;
    (_me$$responsiveWrappe = me.$responsiveWrapper) === null || _me$$responsiveWrappe === void 0 ? void 0 : _me$$responsiveWrappe.destroy();
    me.$responsiveWrapper = target && wrapWidget(target, responsiveUpdate);
    responsiveUpdate === null || responsiveUpdate === void 0 ? void 0 : responsiveUpdate.now();
  }
  // Support methods
  responsiveUpdate() {
    const me = this,
      {
        $responsiveStates: states,
        $responsiveWrapper: wrapper
      } = me,
      responsiveTarget = wrapper === null || wrapper === void 0 ? void 0 : wrapper.widget;
    if (states && wrapper) {
      let best = null,
        bestScore = 0,
        // 0 doesn't get used (since !best) but data flow warnings arise w/o assignment
        fromWhen = states,
        score,
        state;
      // If this instance has a responsiveWidget (via responsiveTarget being set to a widget), and that widget
      // is not this instance, and if this instance did not specify any explicit "when" properties in its own
      // "responsive" config, use those of the target
      if (responsiveTarget && responsiveTarget !== me && !me.$responsiveWhen) {
        responsiveTarget.getConfig('responsive'); // make sure the config has been evaluated
        fromWhen = responsiveTarget.$responsiveStates || fromWhen;
      }
      wrapper.reset();
      for (state in states) {
        if (state !== '*') {
          score = fromWhen[state].when(wrapper.object, BrowserHelper);
          if (score != null && score !== false && (!best || score < bestScore)) {
            best = state;
            bestScore = score;
          }
        }
      }
      ++me.responsiveUpdateCount; // this unlocks statefulId() getter in State mixin
      me.responsiveState = best;
    }
  }
  syncResponsiveWidget() {
    const me = this;
    let widget = null,
      responsiveTarget;
    if (!me.isDestroying && me.responsive) {
      responsiveTarget = me.responsiveTarget;
      if (!(widget = responsiveTarget)) {
        var _me$up;
        widget = !me.responsiveRoot && ((_me$up = me.up) === null || _me$up === void 0 ? void 0 : _me$up.call(me, responsiveRootFn)) || me;
      } else if (typeof responsiveTarget === 'string') {
        widget = responsiveTarget === '@' ? me : responsiveTarget[0] === '@' ? me[responsiveTarget.substring(1)] : me.up(responsiveTarget);
        if (!widget) {
          throw new Error(`No match for responsiveTarget="${responsiveTarget}"`);
        }
      }
      if (!widget.isWidget) {
        throw new Error(`${widget.constructor.$$name} is not a widget and cannot be a responsiveTarget`);
      }
    }
    me.responsiveWidget = widget;
    return widget;
  }
  changeBreakpoints(breakpoints) {
    ObjectHelper.assertObject(breakpoints, 'breakpoints');
    // Normalize breakpoints
    if (breakpoints !== null && breakpoints !== void 0 && breakpoints.width) {
      Object.keys(breakpoints.width).forEach(key => {
        breakpoints.width[key].maxWidth = key;
      });
    }
    if (breakpoints !== null && breakpoints !== void 0 && breakpoints.height) {
      Object.keys(breakpoints.height).forEach(key => {
        breakpoints.height[key].maxHeight = key;
      });
    }
    return breakpoints;
  }
  updateBreakpoints(breakpoints) {
    if (breakpoints) {
      this.monitorResize = true;
    }
  }
  // Get a width/height breakpoint for the supplied dimension
  getBreakpoint(levels, dimension) {
    const
      // Breakpoints as reverse sorted array of numerical widths [NaN for *, 50, 100]
      ascendingLevels = Object.keys(levels).map(l => parseInt(l)).sort(),
      // Find first one larger than current width
      breakpoint = ascendingLevels.find(bp => dimension <= bp);
    // Return matched breakpoint or * if available and none matched
    return levels[breakpoint ?? (levels['*'] && '*')];
  }
  // Apply a breakpoints configs, trigger event and call any callback
  activateBreakpoint(orientation, breakpoint) {
    const me = this,
      prevBreakpoint = me[`current${orientation}Breakpoint`];
    if (breakpoint !== prevBreakpoint) {
      var _breakpoint$callback, _me$recompose;
      me[`current${orientation}Breakpoint`] = breakpoint;
      me.setConfig(breakpoint.configs);
      prevBreakpoint && me.element.classList.remove(`b-breakpoint-${prevBreakpoint.name.toLowerCase()}`);
      me.element.classList.add(`b-breakpoint-${breakpoint.name.toLowerCase()}`);
      /**
       * Triggered when a new max-width based breakpoint is applied.
       * @event responsiveWidthChange
       * @param {Core.widget.Widget} source The widget
       * @param {Breakpoint} breakpoint The applied breakpoint
       * @param {Breakpoint} prevBreakpoint The previously applied breakpoint
       * @deprecated 5.0 This event is associated with {@link #config-breakpoints} which is deprecated in favor of
       * {@link #config-responsive}.
       */
      /**
       * Triggered when a new max-height based breakpoint is applied.
       * @event responsiveHeightChange
       * @param {Core.widget.Widget} source The widget
       * @param {Breakpoint} breakpoint The applied breakpoint
       * @param {Breakpoint} prevBreakpoint The previously applied breakpoint
       * @deprecated 5.0 This event is associated with {@link #config-breakpoints} which is deprecated in favor of
       * {@link #config-responsive}.
       */
      me.trigger(`responsive${orientation}Change`, {
        breakpoint,
        prevBreakpoint
      });
      (_breakpoint$callback = breakpoint.callback) === null || _breakpoint$callback === void 0 ? void 0 : _breakpoint$callback.call(breakpoint, {
        source: me,
        breakpoint,
        prevBreakpoint
      });
      (_me$recompose = me.recompose) === null || _me$recompose === void 0 ? void 0 : _me$recompose.call(me);
    }
  }
  // Called on resize to pick and apply a breakpoint, if size changed enough
  applyResponsiveBreakpoints(width, height) {
    const me = this,
      {
        width: widths,
        height: heights
      } = me.breakpoints ?? {};
    if (widths) {
      const breakpoint = me.getBreakpoint(widths, width);
      me.activateBreakpoint('Width', breakpoint);
    }
    if (heights) {
      const breakpoint = me.getBreakpoint(heights, height);
      me.activateBreakpoint('Height', breakpoint);
    }
  }
  onInternalResize(element, width, height, oldWidth, oldHeight) {
    super.onInternalResize(element, width, height, oldWidth, oldHeight);
    this.applyResponsiveBreakpoints(width, height);
  }
});

export { Featureable, Fencible, FilterField, Responsive };
//# sourceMappingURL=Responsive.js.map
