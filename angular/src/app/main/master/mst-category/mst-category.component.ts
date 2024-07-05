import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ceil } from 'lodash-es';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-mst-category',
  standalone: true,
  imports: [],
  templateUrl: './mst-category.component.html',
  styleUrl: './mst-category.component.css'
})
export class MstCategoryComponent {
//   currencyValue = undefined;
//     currencyList: any[] = [];
//     tabKey: number = 1;
//     paginationParams: PaginationParamsModel = { pageNum: 1, pageSize: 20, totalCount: 0, totalPage: 0, sorting: '', skipCount: 0 };
//     gridParams: GridParams | undefined;
//     gridDetailsParams: GridParams | undefined;
//     selectedRow: SearchCatagoryOutputDto = new SearchCatagoryOutputDto();
//     gridColDef: any[];
//     gridDetailsColDef: any[];

//     requestTypeList: any[] = [];
//     filterText = "";

//     fromDate;
//     toDate;

//     rowData: any[] = [];
//     rowDataDetails: any[] = [];
//     constructor(injector: Injector, private dataFormatService: DataFormatService, private _serviceProxy: MstCategỏyServiceProxy, private _curency: MstCurrencyServiceProxy, private inventoryGroup: MstInventoryGroupServiceProxy) {
//         super(injector);
//         this.gridColDef = [
//             {
//                 // STT
//                 headerName: this.l('STT'),
//                 headerTooltip: this.l('STT'),
//                 cellRenderer: (params: ICellRendererParams) => (params.rowIndex + 1).toString(),
//                 cellClass: ['text-center'],
//                 flex: 0.2,
//             },
//             {
//                 headerName: this.l('Name'), 
//                 field: 'name',
//                 flex: 1,
//             },
//         ]
//     }

//     inventoryGroupList: any[] = [];

//     ngOnInit() {
//         this.searchData();
//         this.inventoryGroupList = [];
//         this.spinnerService.show();
//         this.inventoryGroup.getAllInventoryGroup()
//             .pipe(finalize(() => {
//                 this.spinnerService.hide();
//             }))
//             .subscribe(res => {
//                 res.forEach(e => {
//                     this.inventoryGroupList.push({
//                         label: e.productGroupName,
//                         value: e.id
//                     })
//                 })
//             })

//     }

//     searchData() {
//         this.spinnerService.show();
//         this._serviceProxy.getAllData(
//             this.filterText,
//             (this.paginationParams ? this.paginationParams.sorting : ''),
//             (this.paginationParams ? this.paginationParams.pageSize : 20),
//             (this.paginationParams ? this.paginationParams.skipCount : 1))
//             .pipe(finalize(() => {
//                 this.spinnerService.hide();
//             }))
//             .subscribe((result) => {
//                 this.rowData = result.items;
//                 this.gridParams.api.setRowData(this.rowData);
//                 this.paginationParams.totalCount = result.totalCount;
//                 this.paginationParams.totalPage = ceil(result.totalCount / this.paginationParams.pageSize);
//                 this.gridParams.api.sizeColumnsToFit();
//             });
//     }

//     onChangeSelection(params) {
//         this.selectedRow =
//             params.api.getSelectedRows()[0] ?? new SearchCategoryOutputDto();
//         this.selectedRow = Object.assign({}, this.selectedRow);
//         //
//         this.rowDataDetails = [];
//         this.gridDetailsParams.api.setRowData(this.rowDataDetails);
//         //
//         this.spinnerService.show();
//         this._serviceProxy.searchContractTerm(this.selectedRow.id)
//             .pipe(finalize(() => {
//                 this.spinnerService.hide();
//             }))
//             .subscribe((result) => {
//                 this.rowDataDetails = result;
//                 this.gridDetailsParams.api.setRowData(this.rowDataDetails);
//                 this.gridDetailsParams.api.sizeColumnsToFit();
//             });
//     }

//     callBackGrid(params: GridParams) {
//         this.gridParams = params;
//         params.api.setRowData([]);
//     }

//     callBackGridDetails(params: GridParams) {
//         this.gridDetailsParams = params;
//         params.api.setRowData([]);
//     }

//     deleteRow() {
//         this.spinnerService.show();
//         this._serviceProxy.delete(this.selectedRow.id)
//             .pipe(finalize(() => {
//                 this.searchData()
//                 this.spinnerService.hide();
//             }))
//             .subscribe(res => {
//                 this.notify.success("Deleted Successfully")
//             });
//     }

//     changePaginationParams(paginationParams: PaginationParamsModel) {
//         if (!this.rowData) {
//             return;
//         }
//         this.paginationParams = paginationParams;
//         this.paginationParams.skipCount = (paginationParams.pageNum - 1) * paginationParams.pageSize;
//         this.paginationParams.pageSize = paginationParams.pageSize;
//         this.searchData();
//     }
}
