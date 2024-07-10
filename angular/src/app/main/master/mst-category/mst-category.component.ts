import {  Injector, NgModule, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { Component } from '@angular/core';
import { CustomColumn, PaginationParamCustom } from '@app/shared/common/models/base.model';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MstLCCategoryServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { finalize } from 'rxjs';
import {   CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { AppComponent } from '@app/app.component'; 
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { PrimengTableHelper } from '@shared/helpers/PrimengTableHelper';

@Component({
  selector: 'app-mst-category',
  templateUrl: './mst-category.component.html',
  styleUrl: './mst-category.component.less'
})

// @NgModule({
//   declarations: [
//    ],
//   imports: [
//    ],
//   providers: [
//    ],
//   bootstrap: [AppComponent],
//   schemas: [CUSTOM_ELEMENTS_SCHEMA]
// })

export class MstCategoryComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTableAuditLogs', { static: true }) dataTableAuditLogs: Table;
  @ViewChild('dataTableEntityChanges', { static: true }) dataTableEntityChanges: Table;
  @ViewChild('paginatorAuditLogs', { static: true }) paginatorAuditLogs: Paginator;
  @ViewChild('paginatorEntityChanges', { static: true }) paginatorEntityChanges: Paginator;

  primengTableHelperCategory = new PrimengTableHelper();

  data: MstCategoryComponent[];
  [x: string]: any;
  code: string;
  name: string;
  selectionSettings: { type: 'Single' };
  filterSettings: { type: 'FilterBar', mode: 'Immediate', immediateModeDelay: 150 };
  rowData: any[] = [];
  pagination: PaginationParamCustom = { totalCount: 0, totalPage: 0, pageNum: 1, pageSize: 20 };

  columnDef: CustomColumn[] =
    [
      {
        header: this.l('Code'),
        field: 'id',
        width: 60,
        textAlign: 'Left',
        isPrimaryKey: true,
      },
      {
        header: this.l('Name'),
        field: 'nam',
        width: 50,
        textAlign: 'Left',
      },
    ]

  constructor(
    injector: Injector,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector)
  }
  ngOnInit() {
    this.searchDatas();
  }

  searchDatas() {
    this._service.getAllData(
      this.code,
      this.name,
      this.pagination?.pageSize,
      this.pagination?.pageSize * (this.pagination?.pageNum - 1)
    )
      .pipe(finalize(() => {
        this.spinnerService.hide();
      }))
      .subscribe(res => {
        this.pagination.totalCount = res.totalCount;
        this.rowData = res.items;
      });
  }

  clearTextSearch() {
    this.code = '';
    this.name = '';
    this.searchDatas();
  }

  // changeSelection(e: any) {
  //   this.selectionRow = new createOrEdit({
  //       id: e.data.id,
  //       name: e.data.name,
  //   });
  // }

  changePager(event: any) {
    this.pagination = event;
    this.searchDatas();
  }

  deleteRow(system: MstLCCategoryServiceProxy) {
    this.message.confirm(this.l('AreYouSure'), 'Delete Row', (isConfirmed) => {
      if (isConfirmed) {
        this.spinnerService.show();
        this._serviceProxy.delete(this.selectedRow.id)
          .pipe(finalize(() => {
            this.searchData()
            this.spinnerService.hide();
          }))
          .subscribe(res => {
            this.notify.success("Deleted Successfully")
          });
      }
      else {
        this.searchData()
        this.spinnerService.hide();
      }
    });

  }

}



