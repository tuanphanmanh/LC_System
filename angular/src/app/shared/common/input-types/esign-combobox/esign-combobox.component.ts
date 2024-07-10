import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Component, Input, forwardRef, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'esign-combobox',
  templateUrl: './esign-combobox.component.html',
  styleUrls: ['./esign-combobox.component.less'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EsignComboboxComponent),
      multi: true,
    },
  ],
})
export class EsignComboboxComponent implements ControlValueAccessor {
  @Input() className: string = '';
  @Input() value: any;
  @Input() items: any[] = [];
  @Input() text: string = '';
  @Input() isRequired: boolean = false;
  @Input() isValidate: boolean = false;
  @Input() isDisabled: boolean = false;
  @Input() selectedItem: any;
  @Input() label: string = 'label';
  @Input() key: string = 'value';
  @Input() type: string = 'text';

  @Output() onChangeValue = new EventEmitter();
  @ViewChild('input', {static: false}) input!: ElementRef;

  private onChange: Function = new Function();

  constructor(
  ) {
  }
  writeValue(val: any): void {
    this.value = val ?? '';
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void { }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  changeValue(e: any) {
    if (!isNaN(e.target.value) && this.type === 'number') {
      this.value = Number(e.target.value);
      if (typeof this.onChange === 'function') {
        this.onChange(Number(e.target.value));
      }
      this.onChangeValue.emit(Number(e.target.value));
    } else {
      this.value = e.target.value;
      if (typeof this.onChange === 'function') {
        this.onChange(e.target.value);
      }
      this.onChangeValue.emit(e.target.value);
    }
  }
}
