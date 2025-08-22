/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Field } from './Editor.js';

/**
 * @module Core/widget/TextAreaField
 */
/**
 * TextAreaField widget for multiline text input. Wraps a native &lt;textarea&gt; HTML element.
 *
 * ```javascript
 * const textAreaField = new TextAreaField({
 *   placeholder: 'Enter some text'
 * });
 *```
 *
 * @extends Core/widget/Field
 * @classType textareafield
 * @inlineexample Core/widget/TextAreaField.js
 * @inputfield
 */
class TextAreaField extends Field {
  static get $name() {
    return 'TextAreaField';
  }
  // Factoryable type name
  static get type() {
    return 'textareafield';
  }
  // Factoryable type alias
  static get alias() {
    return 'textarea';
  }
  static get configurable() {
    return {
      /**
       * The resize style to apply to the `<textarea>` element.
       * @config {'none'|'both'|'horizontal'|'vertical'}
       * @default
       */
      resize: 'none',
      inputAttributes: {
        tag: 'textarea'
      }
    };
  }
  updateResize(resize) {
    this.input.style.resize = resize;
  }
}
// Register this widget type with its Factory
TextAreaField.initClass();
TextAreaField._$name = 'TextAreaField';

export { TextAreaField };
//# sourceMappingURL=TextAreaField.js.map
