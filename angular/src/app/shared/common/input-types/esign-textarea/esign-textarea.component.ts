import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import {
    Component,
    Input,
    Output,
    EventEmitter,
    forwardRef,
    ViewChild,
    ElementRef,
} from '@angular/core';

@Component({
    selector: 'esign-textarea',
    templateUrl: './esign-textarea.component.html',
    styleUrls: ['./esign-textarea.component.less'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => EsignTextareaComponent),
            multi: true,
        }
    ],
})
export class EsignTextareaComponent implements ControlValueAccessor {
    @ViewChild('input') input!: ElementRef;
    @Input() className: string = '';
    @Input() addOnMinWidth: string = '';
    @Input() text: string = '';
    @Input() isRequired: boolean = false;
    @Input() isValidate: boolean = false;
    @Input() placeholder: string = '';
    @Input() disable: boolean = false;
    @Input() value: any;
    @Input() rows: any;
    @Input() hideLeftText: boolean = false;
    @Input() height: string = '';
    @Input() setColor: boolean = false;

    style: any;

    // tslint:disable-next-line:no-output-on-prefix
    @Output() onChoose = new EventEmitter();
    // tslint:disable-next-line:ban-types
    private onChange: Function = new Function();
    @Input() isDisabled: boolean = false;

    constructor() { }

    // tslint:disable-next-line: use-lifecycle-interface
    ngOnInit(): void {
        //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
        //Add 'implements OnInit' to the class.
        if (this.height) {
            this.setHeight(this.height);
        }
    }

    writeValue(obj: any): void {
        this.value = obj || '';
    }
    registerOnChange(fn: any): void {
        this.onChange = fn;
    }
    registerOnTouched(fn: any): void { }
    setDisabledState?(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
    }

    onValueChange(event: any) {
        this.value = event ? (event.target as HTMLInputElement).value : '';
        if (typeof this.onChange === 'function') {
            this.onChange(event.target.value);
        }
    }

    setHeight(height: string) {
        this.style = Object.assign({}, { height });
    }
}
