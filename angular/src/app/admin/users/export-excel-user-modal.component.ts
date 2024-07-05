import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
    selector: 'exportExcelUserModal',
    templateUrl: './export-excel-user-modal.component.html',
    styleUrl: './export-excel-user-modal.component.less',
})
export class ExportExcelUserModalComponent extends AppComponentBase implements OnInit{

    @ViewChild('exportExcelModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    form: FormGroup;
    toggleAllBtnText: string;
    selectedColumnsList: string[];

    constructor(
        injector: Injector,
        private fb: FormBuilder,
        private _userServiceProxy: UserServiceProxy,
        private _fileDownloadService: FileDownloadService,
    ) {
        super(injector);
    }
    ngOnInit(): void {
        this.form = this.fb.group({
            selectedColumns: this.fb.array([])
        });
    }

    show(): void {
        this.active = true;
        this.toggleAllBtnText = this.l('SelectAll');

        this._userServiceProxy.getUserExcelColumnsToExcel().subscribe((result) => {
            this.selectedColumnsList = result;

            this.form = this.fb.group({
                selectedColumns: this.fb.array(Array(result.length).fill(false)),
            });

            this.modal.show();
        });
    }

    toggleClick(): void {
        const formArray = this.form.controls.selectedColumns as FormArray;
        const allSelected = formArray.controls.every((control) => control.value);

        if (allSelected) {
            formArray.controls.forEach((control) => {
                control.setValue(false);
                this.toggleAllBtnText = this.l('SelectAll');
            });
        } else {
            formArray.controls.forEach((control) => {
                control.setValue(true);
                this.toggleAllBtnText = this.l('UnselectAll');
            });
        }
    }

    selectedColumnChange(): void {
        const formArray = this.form.controls.selectedColumns as FormArray;
        const allSelected = formArray.controls.every((control) => control.value);

        if (allSelected) {
            formArray.controls.forEach(() => {
                this.toggleAllBtnText = this.l('UnselectAll');
            });
        } else {
            formArray.controls.forEach(() => {
                this.toggleAllBtnText = this.l('SelectAll');
            });
        }
    }

    selectColumns(): void {
        this.saving = true;

        const formArray = this.form.controls.selectedColumns as FormArray;
        const selectedColumns = formArray.controls
            .map((control, index) => ({ index, value: control.value }))
            .filter((control) => control.value)
            .map((control) => this.selectedColumnsList[control.index]);

        this.modalSave.emit(selectedColumns)
        this.saving = false;
        this.close();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
