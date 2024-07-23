using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using GemBox.Spreadsheet;
using FastMember;
using System.Linq;
using System.Drawing;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml.Drawing;
using OfficeOpenXml;

namespace tmss.Common
{
    public class Commons
    {
        public static string GetMessage(string code)
        {
            try
            {
                var sb = new StringBuilder();
                string path = path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Common/Message.xml");

                if (!(File.Exists(path)))
                {
                    return "";
                }

                var ds = new DataSet();
                ds.ReadXml(path);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        for (int j = 0; j <= ds.Tables[0].Columns.Count - 1; j++)
                        {
                            if (code.Equals(ds.Tables[0].Columns[j].ColumnName))
                            {
                                return ds.Tables[0].Rows[i][j].ToString();
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void FillExcel<T>(List<T> data, ExcelWorksheet v_worksheet, int startRow, int startCol, string[] properties, string[] p_header)
        {
            IEnumerable<T> dataE = data.AsEnumerable();
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(dataE, properties))
            {
                table.Load(reader);
            }

            if (table.Rows.Count > 0)
            {
                v_worksheet.Cells.GetSubrange(v_worksheet.Cells[startRow, startCol].Name, v_worksheet.Cells[startRow + table.Rows.Count - 1, startCol + table.Columns.Count - 1].Name).Style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), GemBox.Spreadsheet.LineStyle.Thin);

                InsertDataTableOptions ins = new InsertDataTableOptions(startRow, startCol);
                v_worksheet.InsertDataTable(table, ins);
            }

            for (int i = 0; i < p_header.Length; i++)
            {
                v_worksheet.Cells[0, i].Value = p_header[i];
            }

            SetBorder(v_worksheet, 0, 0, data.Count(), p_header.Count() - 1);
            SetColorHeader(v_worksheet, 0, 0, 0, p_header.Count() - 1);
        }

        public static void ExcelFormatDate(ExcelWorksheet v_worksheet, int columns)
        {
            v_worksheet.Columns[columns].Style.NumberFormat = "dd/MM/yyyy";
        }

        public static void ExcelFormatDateTime(ExcelWorksheet v_worksheet, int columns)
        {
            v_worksheet.Columns[columns].Style.NumberFormat = "dd/MM/yyyy hh:mm:ss";
        }

        public static void SetBorder(ExcelWorksheet v_worksheet, int p_start_rows, int p_start_columns, int p_end_rows, int p_end_columns)
        {
            v_worksheet.Cells.GetSubrange(v_worksheet.Cells[p_start_rows, p_start_columns].Name, v_worksheet.Cells[p_end_rows, p_end_columns].Name).Style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), GemBox.Spreadsheet.LineStyle.Thin);

        }

        public static void SetColorHeader(ExcelWorksheet v_worksheet, int p_start_rows, int p_start_columns, int p_end_rows, int p_end_columns)
        {
            v_worksheet.Cells.GetSubrange(v_worksheet.Cells[p_start_rows, p_start_columns].Name, v_worksheet.Cells[p_end_rows, p_end_columns].Name).Style.FillPattern.SetPattern(FillPatternStyle.Solid, SpreadsheetColor.FromName(ColorName.LightBlue), SpreadsheetColor.FromName(ColorName.LightBlue));
        }

        public static void Merged(ExcelWorksheet v_worksheet, int p_start_rows, int p_start_columns, int p_end_rows, int p_end_columns)
        {
            v_worksheet.Cells.GetSubrange(v_worksheet.Cells[p_start_rows, p_start_columns].Name, v_worksheet.Cells[p_end_rows, p_end_columns].Name).Merged = true;
        }
        public static string SetAutoFit(string p_path, int p_num_column)
        {
            XSSFWorkbook xSSFWorkbook = null;
            using (FileStream file = new FileStream(p_path, FileMode.Open, FileAccess.ReadWrite))
            {
                xSSFWorkbook = new XSSFWorkbook(file);
                ISheet sheet = xSSFWorkbook.GetSheetAt(0);
                for (int i = 0; i < p_num_column; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            }
            var tempfile2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");
            using (FileStream file = new FileStream(tempfile2, FileMode.Create, FileAccess.ReadWrite))
            {
                xSSFWorkbook.Write(file);
                file.Close();
            }

            return tempfile2;
        }

        public static string GetFinPeriod(DateTime date)
        {
            int y = date.Year;
            int m = date.Month;
            if (m > 3)
            {
                return (y + 1).ToString();
            }
            else
            {
                return y.ToString();
            }

        }

        //public static SessionOptions SftpConnect()
        //{
        //    SessionOptions sessionOptions = new SessionOptions
        //    {
        //        Protocol = Protocol.Sftp,
        //        HostName = "192.168.2.162",
        //        UserName = "Sysadmin",
        //        Password = "Nov@2022@6869",
        //        SshHostKeyFingerprint = "ssh-ed25519 255 Ecl1tDChtWi8VIBE0UjNxiLlnXOargOHpIBYBK1urUc"
        //    };

        //    return sessionOptions;
        //}

        //public static void GetFile(SessionOptions sessionOptions, string p_from_path, string p_to_path)
        //{
        //    p_to_path = @"Test";
        //    using (Session session = new Session())
        //    {
        //        TransferOptions transferOptions = new TransferOptions
        //        {
        //            TransferMode = TransferMode.Binary,
        //            OverwriteMode = OverwriteMode.Overwrite
        //        };
        //        session.Open(sessionOptions);
        //        session.PutFileToDirectory(@"D:\CheckWMI.xlsx",p_to_path,  false, transferOptions);
        //    }
        //}
    }
}
