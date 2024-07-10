import { FormStoringService } from './form-storing.service';
import { NotifyService } from 'abp-ng2-module';
import { Injectable } from '@angular/core';
import { GridApi, RowNode } from '@ag-grid-enterprise/all-modules';

@Injectable({
  providedIn: 'root'
})
export class GridTableService {

  constructor(private notify: NotifyService,
    private formStoringService: FormStoringService) { }

  // gridApi = params.api
  selectFirstRow(gridApi?: GridApi) {
    setTimeout(() => {
      gridApi?.forEachLeafNode((node: RowNode) => {
          if (node.childIndex === 0) node.setSelected(true);
          gridApi.setFocusedCell(0, gridApi.getColumnDefs()[0]['field']);
      });
    }, 40);
  }

  getAllData(params) {
    const dataArr = [];
    params?.api?.forEachLeafNode(node => {
      dataArr.push(node.data);
    });
    return dataArr;
  }

  // gridApi = params.api
  setFocusCell(gridApi: GridApi, colName?: string, displayedData?: any[], rowIndex?: number, editing?: boolean) {

    let rowToFocus = rowIndex != null ? rowIndex : (displayedData ? displayedData.length : 0);
    gridApi.clearRangeSelection();
    gridApi?.forEachNode(node => {
      if (node.rowIndex === rowToFocus) {
        node.setSelected(true);
        if (editing) {
          setTimeout(() => {
            gridApi.setFocusedCell(rowToFocus, colName)
            gridApi.startEditingCell({
              rowIndex: rowToFocus,
              colKey: colName
            });
          });
          return;
        } else  setTimeout(() => {
          gridApi.setFocusedCell(rowToFocus, colName);
        });
      }
    });
  }

  // Check nếu dòng trước đó dữ liệu không hợp lệ thì không cho thêm dòng nữa
  validateRow(gridApi, field, displayedData, headerName?, checkRow?) {
    const rowToCheck = checkRow ?? (displayedData ? displayedData.length : 0);
    let validateCheck = true;

    if (checkRow) {
    }
    gridApi.forEachNode(node => {
      if (node.childIndex === rowToCheck) {
        if (!node.data[field] || node.data[field] === '' || node.data[field] === 0) {
          this.notify.warn(`${headerName} dòng thứ ${node.childIndex + 1} không được phép bỏ trống`);
          validateCheck = false;
        }
      }
    });
    return validateCheck;
  }

  // EXPORT
  generateExportFileName(startingName: string, storageKey: string) {
    const year = (new Date().getFullYear()).toString().slice(2, 4);
    const month = new Date().getMonth() + 1;
    const date = new Date().getDate();
    return `${startingName}${year}${month < 10 ? `0${month}` : month}${date < 10 ? `0${date}` : date}${this.storeAndGetExportTime(storageKey)}`;
  }
  // Storage Export Time
  storeAndGetExportTime(storageKey: any) {
    const storage = this.formStoringService.get(storageKey);
    const year = new Date().getFullYear();
    const month = new Date().getMonth();
    const date = new Date().getDate();
    if (storage) {
      if (storage.setTime < new Date(year, month, date, 0, 0, 0)) {
        this.formStoringService.clear(storageKey);
        this.formStoringService.set(storageKey, {value: 1, setTime: new Date().getTime()});
        return '001';
      }
      this.formStoringService.clear(storageKey);
      this.formStoringService.set(storageKey, {value: storage.value + 1, setTime: new Date().getTime()});
      return storage.value + 1 < 10 ? `00${storage.value + 1}`
        : storage.value + 1 < 100 ? `0${storage.value + 1}` : storage.value + 1;
    } else {
      this.formStoringService.set(storageKey, {value: 1, setTime: new Date().getTime()});
      return '001';
    }
  }
}
