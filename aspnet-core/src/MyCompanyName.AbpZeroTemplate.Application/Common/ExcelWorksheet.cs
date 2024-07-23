using GemBox.Spreadsheet.ConditionalFormatting;
using GemBox.Spreadsheet.PivotTables;
using GemBox.Spreadsheet.Tables;
using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemBox.Spreadsheet.Charts;
using System.Data.Common;

namespace MyCompanyName.AbpZeroTemplate.Common
{
    public sealed class ExcelWorksheet
    {
        public ExcelPictureFormat BackgroundPictureFormat { get; }
        public bool Protected { get; set; }
        public IgnoredErrorCollection IgnoredErrors { get; }
        public ConditionalFormatRuleCollection ConditionalFormatting { get; }
        public AutoFilter Filter { get; set; }
        public SheetType Type { get; }
        public int Index { get; }
        public ExcelPictureCollection Pictures { get; }
        public ExcelChartCollection Charts { get; }
        public WorksheetProtection ProtectionSettings { get; }
        public ExcelShapeCollection Shapes { get; }
        public MemoryStream BackgroundPictureStream { get; }
        public DataValidationCollection DataValidations { get; }
        public NamedRangeCollection NamedRanges { get; }
        public SpreadsheetHyperlinkCollection Hyperlinks { get; }
        public ExcelCommentCollection Comments { get; }
        public string Name { get; set; }
        public ExcelRowCollection Rows { get; }
        public ExcelColumnCollection Columns { get; }
        public ExcelEmbeddedObjectCollection EmbeddedObjects { get; }
        public CellRange Cells { get; }
        public ProtectedRangeCollection ProtectedRanges { get; }
        public int DefaultColumnWidth { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use 'GemBox.Spreadsheet.WorksheetProtection.PasswordHash' property instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetPasswordHash.")]
        public int PasswordHash { get; set; }
        public bool FilterMode { get; }
        public TableCollection Tables { get; }
        public PivotTableCollection PivotTables { get; }
        public SpreadsheetColor TabColor { get; set; }
        public CellRangeCollection SelectedRanges { get; }
        public SheetVisibility Visibility { get; set; }
        public WorksheetPanes Panes { get; set; }
        public bool HasSplitOrFreezePanes { get; }
        public SheetHeaderFooter HeadersFooters { get; set; }
        public ExcelViewOptions ViewOptions { get; }
        public ExcelPrintOptions PrintOptions { get; }
        public ExcelFile Parent { get; }
        public VerticalPageBreakCollection VerticalPageBreaks { get; }
        public HorizontalPageBreakCollection HorizontalPageBreaks { get; }
        public int DefaultRowHeight { get; set; }
        public bool HasHeadersFooters { get; }
        public SortState Sort { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use 'GemBox.Spreadsheet.ExtractToDataTableOptions.ExcelCellToDataTableCellConverting' event instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetExtractDataEvent.")]
        public event ExtractDataEventHandler ExtractDataEvent;

        //public void Calculate();
        //public int CalculateMaxUsedColumns();
        //public void Clear();
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetCreateDataTable3.")]
        //public DataTable CreateDataTable(ColumnTypeResolution resolution, ExcelRow startRow, int numberOfRows, ExtractDataOptions options, params ExcelColumn[] columns);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetCreateDataTable.")]
        //public DataTable CreateDataTable(CellRange range, ColumnTypeResolution resolution, ExtractDataOptions options, bool useFirstRowForColumnNames);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetCreateDataTable2.")]
        //public DataTable CreateDataTable(ColumnTypeResolution resolution);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetCreateDataTable4.")]
        //public DataTable CreateDataTable(ColumnTypeResolution resolution, ExcelRow startRow, int numberOfRows, ExtractDataOptions options, bool useFirstRowForColumnNames, params ExcelColumn[] columns);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetCreateDataTable5.")]
        //public DataTable CreateDataTable(ColumnTypeResolution resolution, ExcelRow startRow, int numberOfRows, ExtractDataOptions options, bool useFirstRowForColumnNames, IEnumerable<ExcelColumn> columns);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetCreateDataTable6.")]
        //public DataTable CreateDataTable(ColumnTypeResolution resolution, ExcelRow startRow, int numberOfRows, ExtractDataOptions options, IEnumerable<ExcelColumn> columns);
        //public DataTable CreateDataTable(CreateDataTableOptions options);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheetCollection.Remove(System.Int32)' method with 'GemBox.Spreadsheet.ExcelWorksheet.Index' as parameter instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetDelete.")]
        //public void Delete();
        //public void DeleteBackgroundPicture();
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.ExtractToDataTable(System.Data.DataTable, GemBox.Spreadsheet.ExtractToDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetExtractToDataTable2.")]
        //public void ExtractToDataTable(DataTable dataTable, int numberOfRows, ExtractDataOptions options, DataColumnMappingCollection dataColumnMappingCollection, ExcelRow startRow);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.ExtractToDataTable(System.Data.DataTable, GemBox.Spreadsheet.ExtractToDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetExtractToDataTable.")]
        //public void ExtractToDataTable(DataTable dataTable, int numberOfRows, ExtractDataOptions options, ExcelRow startRow, ExcelColumn startColumn);
        //public void ExtractToDataTable(DataTable dataTable, ExtractToDataTableOptions options);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.CreateDataTable(GemBox.Spreadsheet.CreateDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetExtractUsedRangeToDataTable.")]
        //public DataTable ExtractUsedRangeToDataTable(ExtractDataOptions options);
        //public CellRange GetUsedCellRange(bool ignoreCellsWithoutValue);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.GetUsedCellRange(System.Boolean)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetGetUsedCellRange.")]
        //public CellRange GetUsedCellRange();
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheetCollection.InsertCopy(System.Int32, System.String, GemBox.Spreadsheet.ExcelWorksheet)' method with 'GemBox.Spreadsheet.ExcelWorksheet.Index' as first parameter instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetInsertCopy.")]
        //public ExcelWorksheet InsertCopy(string destinationWorksheetName, ExcelWorksheet sourceWorksheet);
        //public int InsertDataTable(DataTable dataTable);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.InsertDataTable(System.Data.DataTable, GemBox.Spreadsheet.InsertDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetInsertDataTable.")]
        //public int InsertDataTable(DataTable dataTable, int startRow, int startColumn, bool columnHeaders);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheet.InsertDataTable(System.Data.DataTable, GemBox.Spreadsheet.InsertDataTableOptions)' method instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetInsertDataTable2.")]
        //public int InsertDataTable(DataTable dataTable, string startCell, bool columnHeaders);
        //public int InsertDataTable(DataTable dataTable, InsertDataTableOptions options);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.ExcelWorksheetCollection.InsertEmpty(System.Int32, System.String)' method with 'GemBox.Spreadsheet.ExcelWorksheet.Index' as first parameter instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetInsertEmpty.")]
        //public ExcelWorksheet InsertEmpty(string worksheetName);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("Use 'GemBox.Spreadsheet.AbstractRange.Style' property instead. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExcelWorksheetResolveCellStyle.", true)]
        //public CellStyle ResolveCellStyle(int row, int column);
        //public void SetBackgroundPicture(string picturePath);
        //public void SetBackgroundPicture(MemoryStream pictureStream, ExcelPictureFormat pictureFormat);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Used in obsolete 'GemBox.Spreadsheet.ExcelWorksheet.ExtractDataEvent' event. This delegate is obsolete and will be removed in a future version. For more info, see https://www.gemboxsoftware.com/spreadsheet/help/html/Obsolete_Members.htm#ExtractDataEventHandler.")]
        public delegate void ExtractDataEventHandler(object sender, ExtractDataDelegateEventArgs e);
    }
}
