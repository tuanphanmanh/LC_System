import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, Input, forwardRef, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'esign-checkbox',
    templateUrl: './esign-checkbox.component.html',
    styleUrls: ['./esign-checkbox.component.less'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => EsignCheckboxComponent),
            multi: true,
        }],
})
export class EsignCheckboxComponent extends AppComponentBase implements ControlValueAccessor {

    @Input() text: string = '';
    @Input() className: string = '';
    @Input() name: string = '';
    @Input() isReadonly: string = '';
    @Input() show: boolean = true;
    @ViewChild('iCheckBox', { static: false }) iCheckBox!: ElementRef;
    @Input() styleClass = '';
    // tslint:disable-next-line:ban-types
    private onChange: Function = new Function();
    @Input() selectedValue: any;
    @Input() isDisabled: boolean = false;
    @Input() rightCheckbox: boolean = true;
    @Input() labelWidth: number = 0;
    @Input() checked: boolean = false;
    @Input() value: any;

    @Input() id: number = 0;
    @Output() changeValueCheckBox = new EventEmitter();

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    writeValue(val: any) {
        this.selectedValue = val || false;
    }

    registerOnChange(fn: Function) {
        this.onChange = fn;
    }

    registerOnTouched(fn: Function) {
    }

    setDisabledState(isDisabled: boolean) {
        this.isDisabled = isDisabled;
    }

    onChangeValue(val: any) {
        if (this.isDisabled || this.isReadonly) {
            return;
        }

        if (typeof this.onChange === 'function') {
            this.onChange(val);
            this.selectedValue = val;
        }
        this.changeValueCheckBox.emit(val);
    }
}
