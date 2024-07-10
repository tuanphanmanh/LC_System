// import { ColDef, Column, ColumnApi, GridApi, ICellEditorParams, IsColumnFunc } from '@ag-grid-enterprise/all-modules';

export interface PaginationParamsModel {
    totalCount?: number | undefined;
    totalPage?: number | undefined;
    sorting?: string | undefined;
    skipCount?: number | undefined;
    pageSize?: number | undefined;
    pageNum?: number | undefined;
  }
  
  // export interface GridParams {
  //   api: GridApi,
  //   columnApi: ColumnApi,
  //   editingStartedValue: string,
  //   invalidRcCode?: boolean,
  //   invalidPartCode?: boolean,
  // }
  
  // export interface RowSelectionParams {
  //     api: GridApi,
  //     columnApi: ColumnApi
  // }
  
  // export interface CustomColDef extends ColDef {
  //     buttonDef?: {
  //         text?: string | Function;
  //         useRowData?: boolean;
  //         disabled?: boolean | Function;
  //         function?: (params: any) => void;
  //         iconName?: string;
  //         className?: string | Function;
  //         message?: string;
  //     },
  //     buttonDefTwo?: {
  //         text?: string | Function;
  //         useRowData?: boolean;
  //         disabled?: boolean | Function;
  //         function?: (params: any) => void;
  //         iconName?: string;
  //         className?: string | Function;
  //         message?: string;
  //     },
  //     disableSelect?: boolean | IsColumnFunc,
  //     list?: {
  //         key?: string | number,
  //         value?: string | number,
  //     }[] | Function,
  //     disabled?: boolean | IsColumnFunc,
  //     disableCheckbox?: boolean | IsColumnFunc,
  //     data?: string[] | boolean[] | number[],
  //     children?: CustomColDef[],
  //     validators?: string[],
  //     textFormatter?: IsColumnFunc | Function | any,
  //     property?: { key: string, value: string },
  //     listName?: string,
  //     api?: any,
  //     cellClass?: any,
  //     maxLength?: number,
  // }
  
  // export interface AgCellEditorParams extends ICellEditorParams {
  //     column: AgColumn,
  //     oldValue: string | number | undefined,
  //     newValue: string | number | undefined
  //     key?: string | number,
  //     event?: KeyboardEvent,
  //     colDef: CustomColDef,
  //     api: GridApi,
  //     columnApi: ColumnApi,
  //     data: any
  // }
  
  // export interface AgColumn extends Column {
  //     editingStartedValue: string | number | undefined
  // }
  
  // export interface AgCellPositionParams {
  //     column?: Column,
  //     rowIndex?: number,
  //     rowPinned: string | undefined,
  //     floating: string | undefined
  // }
  
  export interface PaginationParamCustom {
      totalCount?: number | undefined;
      totalPage?: number | undefined;
      pageSize?: number | undefined;
      pageNum?: number | undefined;
  }
  
  export interface CustomColumn {
      header?: string,
      field?: string,
      width?: number,
      textAlign?: string,
      editable?: boolean,
      editType?: string,
      format?: string | Object,
      type?: string,
      isPrimaryKey?: boolean,
      visible?: boolean,
      template?: string,
      disabled?: boolean,
      headerTooltip?: string,
      cellClass?: any
  }
  