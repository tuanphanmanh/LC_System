import { Component, OnInit, Input, Output, EventEmitter, AfterViewInit, OnChanges, ViewChild, ElementRef, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'esign-input',
    templateUrl: './esign-search-input.component.html',
    styleUrls: ['./esign-search-input.component.less'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => EsignSearchInputComponent),
            multi: true
        }]
})
export class EsignSearchInputComponent implements ControlValueAccessor, OnInit, AfterViewInit, OnChanges {

    @Input() className: string = '';
    @Input() addOnMinWidth: string = '';
    @Input() text: string = '';
    @Input() ignoreEnterEvent: boolean = false;
    @Input() isRequired: boolean = false;
    @Input() isValidate: boolean = false;
    @Input() placement: string = 'top';
    @Input() value: any;
    @Input() isReadonly: boolean = false;
    @Input() isDisabled: boolean = false;
    @Input() maxLength: number = 0;
    @Input() isKeepFocus: boolean = false;
    @Input() alwaysEnableBtn: boolean = false;
    @Input() placeholder: string = '';
    @Input() showTrash: boolean = false;
    @Input() showModal: boolean = false;
    @Input() showSearch: boolean = false;
    @Input() type: string = 'text';
    private onChange: Function = new Function();
    @Output() onFocus = new EventEmitter();
    @Input() focus: any;
    @ViewChild('input', { static: false }) input!: ElementRef;
    @Output() onSearch = new EventEmitter();
    @Output() onRefresh = new EventEmitter();
    @Output() keydown = new EventEmitter();
    @Output() onClickInput = new EventEmitter();
    @Output() onChangeValue = new EventEmitter();

    constructor() { }
    writeValue(val: any): void {
        this.value = val ?? '';
    }
    registerOnChange(fn: any): void {
        this.onChange = fn;
    }
    registerOnTouched(fn: any): void {

    }
    setDisabledState?(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }

    ngAfterViewInit() {
        this.setFocus();
    }
    ngOnChanges() {
        this.setFocus();
    }
    paste(event: ClipboardEvent) {
        const value = event.clipboardData.getData('text');
        if (typeof this.onChange === 'function') {
            this.onChange(value);
        }
    }

    setFocus() {
        setTimeout(() => {
            if (!!this.focus && this.input) {
                this.input.nativeElement.focus();
                this.onFocus.emit();
                this.focus = false;
            }
        }, 100);
    }

    ngOnInit() {
    }

    btnSearch() {
        this.onSearch.emit(this.value);
    }

    changeValue(e: any) {
        if (!this.ignoreEnterEvent && e.key === 'Enter') {
            this.blurInput();
        }

        this.value = e.target.value;
        if (typeof this.onChange === 'function') {
            this.onChange(this.value);
        }

        this.onChangeValue.emit(this.value);
    }

    onClickInputValue(val: any) {
        this.onClickInput.emit(val);
    }

    blurInput() {
        if (!this.alwaysEnableBtn && this.isDisabled) {
            return;
        }
        this.onSearch.emit(this.value);
        this.input.nativeElement.blur();

        if (this.isKeepFocus) {
            this.input.nativeElement.focus();
        }
    }

    refresh() {
        this.onRefresh.emit();
    }

    onKeyDown(event: any) {
        this.keydown.emit(event);
    }
}
