 

import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EditService, FilterService, SelectionService, PageService, SortService, GridComponent } from '@syncfusion/ej2-angular-grids';
import { Paginator } from 'primeng/paginator';
import { CustomColumn, PaginationParamCustom } from '../models/base.model';
import { ToolbarService } from '@syncfusion/ej2-angular-pdfviewer';
@Component({
    selector: 'app-esign-grid',
    templateUrl: './esign-grid.component.html',
    styleUrls: ['./esign-grid.component.less'],
    providers: [FilterService, SelectionService, PageService, SortService, ToolbarService, EditService]
})
export class EsignGridComponent extends AppComponentBase implements OnInit {
    @ViewChild('overviewgrid', { static: true }) overviewgrid: GridComponent;
    @ViewChild('paginator') paginator: Paginator;
    loadingIndicator = { indicatorType: 'Spinner' };
    @Input() allowFiltering: boolean = false;
    @Input() filterSettings: any = { type: "FilterBar" };
    @Input() selectionSettings: any = { type: "Multiple" };
    @Input() pagination: PaginationParamCustom = { pageSize: 10, pageNum: 1, totalCount: 0, totalPage: 0};
    @Input() columnDefs: CustomColumn[];e
    @Input() rowData: any;
    @Input() height: string = '400px';
    @Input() showIndex: boolean = false;
    @Input() allowPager: boolean = false;
    @Input() showCheckBox: boolean = false;
    @Input() allowSorting: boolean = false;
    @Input() allowReordering: boolean = false;
    @Input() allowResizing: boolean = true;
    @Input() allowRowDragAndDrop: boolean = false;
    @Input() width: string = '100%';
    @Input() sttWidth: number = 30;
    @Input() editSettings: any = { allowEditing: false, allowAdding: false, allowDeleting: false, mode: 'Normal' };
    @Output() onChangeSelection: EventEmitter<any> = new EventEmitter<any>();
    @Output() onChangePager: EventEmitter<any> = new EventEmitter<any>();
    resizeSettings: any = { mode: 'Auto' };
    totalPages: number;
    arrayPage: any[] = [];
    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit() {
        this.calculateTotalPages();
    }

    onChangeSelectionRow(event: any) {
        this.onChangeSelection.emit(event);
    }

    onChangePage(event: any) {
        this.pagination.pageNum = event;
        this.onChangePager.emit(this.pagination);
    }

    onChangePageSize(event: any) {
        this.pagination.pageNum = 1;
        this.pagination.pageSize = event;
        this.calculateTotalPages();
        this.onChangePager.emit(this.pagination);
    }

    onChangePageCbx(event: any) {
        this.paginator.changePage(event.value);
        this.onChangePager.emit(this.paginator);
    }

    calculateTotalPages(){
        this.totalPages = Math.ceil(this.pagination.totalCount / this.pagination.pageSize) || 1;
        this.pagination.totalPage = this.totalPages;
        this.arrayPage = this.range(1, this.totalPages, 1);
    }

    range(start, end, step) {
        const len = Math.floor((end - start) / step) + 1;
        return Array(len).fill(0).map((_, idx) => start + (idx * step));
    }

    onChangeDataState() {
        this.calculateTotalPages();
    }
}

