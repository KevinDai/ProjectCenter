using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using ProjectCenter.Models;
using ProjectCenter.Services.Models;

namespace ProjectCenter.Web.Exports
{
    public class ProjectExprot
    {
        private static readonly int ProjectSheetDataRowStartIndex = 2;

        private static readonly int FinanceSheetDataRowStartIndex = 3;
        private static readonly int FinanceExpenditureColumnStartIndex = 4;
        private static readonly int FinanceExpenditureColumnCount = 10;

        private static readonly string ProjectSheetTitle = "咨询策划中心工作任务清单";
        private static readonly string FinanceSheetTitle = "咨询策划中心项目经费清单";

        private static readonly ExportColumn[] ProjectSheetColumns = new ExportColumn[] {
            new ExportColumn(){Name="序号",Width=5},
            new ExportColumn(){Name="类别",Width=10},
            new ExportColumn(){Name="任务编码",Width=15},
            new ExportColumn(){Name="任务内容",Width=25},
            new ExportColumn(){Name="委托单位",Width=25},
            new ExportColumn(){Name="工作内容",Width=25},
            new ExportColumn(){Name="具体进展",Width=25},
            new ExportColumn(){Name="责任人",Width=25},
            new ExportColumn(){Name="参与人",Width=25},
            new ExportColumn(){Name="完成时间",Width=15},
            new ExportColumn(){Name="需要的支持",Width=25},
            new ExportColumn(){Name="推进计划",Width=25},
            new ExportColumn(){Name="状态",Width=10},
            new ExportColumn(){Name="开始年份",Width=10},
            new ExportColumn(){Name="截止年份",Width=10}
        };

        private static readonly ExportColumn[] FinanceSheetColumns = new ExportColumn[] {
            new ExportColumn(){Name="序号",Width=5},
            new ExportColumn(){Name="任务内容",Width=25},
            new ExportColumn(){Name="财务编号",Width=25},
            new ExportColumn(){Name="项目金额",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="工资（奖金）",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="劳务费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="差旅费（交通费）",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="会议费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="印刷费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="设备购置费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="委托业务费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="办公费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="管理费",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="其他",Width=20,DataFormat="#,##0.00"},
            new ExportColumn(){Name="付款方式",Width=25},
            new ExportColumn(){Name="到款情况",Width=25},
            new ExportColumn(){Name="开票情况",Width=25}
        };

        protected HSSFWorkbook HSSFWorkbook
        {
            get;
            private set;
        }

        protected ISheet ProjectSheet
        {
            get;
            private set;
        }

        protected ISheet FinanceSheet
        {
            get;
            private set;
        }

        protected ICellStyle TitleCellDefaultStyle
        {
            get;
            private set;
        }

        protected ICellStyle HeaderCellDefaultStyle
        {
            get;
            private set;
        }

        protected ICellStyle ColumnDefaultStyle
        {
            get;
            private set;
        }

        protected int ProjectSheetDataRowIndex
        {
            get;
            private set;
        }

        protected int FinanceSheetDataRowIndex
        {
            get;
            private set;
        }

        public bool WithFinance
        {
            get;
            private set;
        }

        public ProjectExprot(bool withFinance)
        {
            WithFinance = withFinance;

            HSSFWorkbook = new HSSFWorkbook();

            ColumnDefaultStyle = CreateColumnDefaultStyle();
            TitleCellDefaultStyle = CreateTitleCellStyle();
            HeaderCellDefaultStyle = CreateHeaderCellDefaultStyle();

            ProjectSheet = HSSFWorkbook.CreateSheet("任务信息");
            ProjectSheet.DisplayGridlines = true;
            SetColumnStyle(ProjectSheet, ProjectSheetColumns);
            WriteTitle(ProjectSheet, ProjectSheetColumns.Length, ProjectSheetTitle);
            WriteProjectSheetHeader(ProjectSheet);

            if (WithFinance)
            {
                FinanceSheet = HSSFWorkbook.CreateSheet("财务信息");
                FinanceSheet.DisplayGridlines = true;
                SetColumnStyle(FinanceSheet, FinanceSheetColumns);
                WriteTitle(FinanceSheet, FinanceSheetColumns.Length, FinanceSheetTitle);
                WriteFinanceSheetHeader(FinanceSheet);
            }

            ProjectSheetDataRowIndex = ProjectSheetDataRowStartIndex;
            FinanceSheetDataRowIndex = FinanceSheetDataRowStartIndex;
        }

        private ICellStyle CreateTitleCellStyle()
        {
            ICellStyle style = HSSFWorkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.CENTER;
            //create a font style
            IFont font = HSSFWorkbook.CreateFont();
            //font.Boldweight
            font.FontHeightInPoints = 16;
            //font.FontHeight = 16 * 16;
            font.FontName = "宋体";
            font.Boldweight = (short)FontBoldWeight.BOLD; //加粗
            style.SetFont(font);
            return style;
        }

        /// <summary>
        /// 创建默认的数据列Excel样式
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private ICellStyle CreateHeaderCellDefaultStyle()
        {
            ICellStyle defaultDataStyle = HSSFWorkbook.CreateCellStyle();
            defaultDataStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            defaultDataStyle.Alignment = HorizontalAlignment.CENTER;
            defaultDataStyle.VerticalAlignment = VerticalAlignment.CENTER;
            defaultDataStyle.BorderBottom = BorderStyle.THIN;
            defaultDataStyle.BorderLeft = BorderStyle.THIN;
            defaultDataStyle.BorderRight = BorderStyle.THIN;
            defaultDataStyle.BorderTop = BorderStyle.THIN;
            defaultDataStyle.WrapText = true;

            IFont font = HSSFWorkbook.CreateFont();
            //font.FontHeight = 10 * 10;
            font.FontHeightInPoints = 9;
            font.FontName = "宋体";
            font.Boldweight = (short)FontBoldWeight.BOLD; //加粗
            defaultDataStyle.SetFont(font);

            return defaultDataStyle;
        }

        /// <summary>
        /// 创建表头单元格默认样式
        /// </summary>
        /// <returns></returns>
        private ICellStyle CreateColumnDefaultStyle(string dataFormat = null)
        {
            ICellStyle headerStyle = HSSFWorkbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.CENTER;
            headerStyle.VerticalAlignment = VerticalAlignment.CENTER;
            //create a font style
            IFont font = HSSFWorkbook.CreateFont();
            font.FontHeightInPoints = 9;
            //font.FontHeight = 10 * 10;
            font.FontName = "宋体";
            headerStyle.SetFont(font);
            headerStyle.BorderBottom = BorderStyle.THIN;
            headerStyle.BorderLeft = BorderStyle.THIN;
            headerStyle.BorderRight = BorderStyle.THIN;
            headerStyle.BorderTop = BorderStyle.THIN;
            headerStyle.WrapText = true;

            if (!string.IsNullOrEmpty(dataFormat))
            {
                var format = HSSFDataFormat.GetBuiltinFormat(dataFormat);
                if (format == -1)
                {
                    format = HSSFWorkbook.CreateDataFormat().GetFormat(dataFormat);
                }
                headerStyle.DataFormat = format;
            }
            return headerStyle;
        }

        private void SetColumnStyle(ISheet sheet, ExportColumn[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                sheet.SetDefaultColumnStyle(i, CreateColumnDefaultStyle(columns[i].DataFormat));
            }
        }

        private void WriteTitle(ISheet sheet, int columnsLenght, string title)
        {
            IRow row = sheet.CreateRow(0);
            var cell = row.CreateCell(0);
            cell.CellStyle = TitleCellDefaultStyle;
            cell.SetCellValue(title);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, columnsLenght - 1));
        }

        private void WriteProjectSheetHeader(ISheet sheet)
        {
            IRow row = sheet.CreateRow(1);
            for (int i = 0; i < ProjectSheetColumns.Length; i++)
            {
                WriteHeaderColumn(sheet, row, i, ProjectSheetColumns[i]);
            }
        }

        private void WriteFinanceSheetHeader(ISheet sheet)
        {
            IRow row1 = sheet.CreateRow(1);
            IRow row2 = sheet.CreateRow(2);
            for (int i = 0; i < FinanceSheetColumns.Length; i++)
            {
                WriteHeaderColumn(sheet, row1, i, FinanceSheetColumns[i]);

                if (i == FinanceExpenditureColumnStartIndex)
                {
                    WriteHeaderColumn(sheet, row1, i, new ExportColumn() { Name = "支出明细" });

                    var endIndex = FinanceExpenditureColumnStartIndex + FinanceExpenditureColumnCount - 1;
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, i, endIndex));

                    for (int j = i; j <= endIndex; j++)
                    {
                        WriteHeaderColumn(sheet, row2, j, FinanceSheetColumns[j]);
                    }
                    i = endIndex;
                }
                else
                {
                    sheet.AddMergedRegion(new CellRangeAddress(1, 2, i, i));
                }
            }
        }

        private void WriteHeaderColumn(ISheet sheet, IRow row, int i, ExportColumn column)
        {
            var cell = row.CreateCell(i);
            cell.CellStyle = HeaderCellDefaultStyle;
            cell.SetCellValue(column.Name);
            sheet.SetColumnWidth(i, column.Width * 256);
        }

        //public void WriteProjects(IEnumerable<Project> projects)
        //{
        //    foreach (var project in projects)
        //    {
        //        WriteProject(project);
        //    }
        //}

        public void WriteProject(Project project, IEnumerable<ExpenditureStatisticItem> items)
        {
            var row = ProjectSheet.CreateRow(ProjectSheetDataRowIndex);


            row.CreateCell(0).SetCellValue(ProjectSheetDataRowIndex - ProjectSheetDataRowStartIndex + 1);
            row.CreateCell(1).SetCellValue(project.TypeString);
            row.CreateCell(2).SetCellValue(project.Code);
            row.CreateCell(3).SetCellValue(project.Name);
            row.CreateCell(4).SetCellValue(project.Consignor);
            row.CreateCell(5).SetCellValue(project.NeedFinish);
            row.CreateCell(6).SetCellValue(project.CurrentProgress);
            row.CreateCell(7).SetCellValue(project.ManagerNames);
            row.CreateCell(8).SetCellValue(project.ParticipantNames);
            row.CreateCell(9).SetCellValue(project.Deadline.ToShortDateString());
            row.CreateCell(10).SetCellValue(project.NeedSupport);
            row.CreateCell(11).SetCellValue(project.AdvancePlan);
            row.CreateCell(12).SetCellValue(project.StatusString);
            row.CreateCell(13).SetCellValue(project.StartTime.Year.ToString());
            row.CreateCell(14).SetCellValue(project.Deadline.Year.ToString());

            ProjectSheetDataRowIndex++;
            if (WithFinance)
            {
                WirteFinance(project, items);
            }
        }

        protected void WirteFinance(Project project, IEnumerable<ExpenditureStatisticItem> items)
        {
            var row = FinanceSheet.CreateRow(FinanceSheetDataRowIndex);

            row.CreateCell(0).SetCellValue(FinanceSheetDataRowIndex - FinanceSheetDataRowStartIndex + 1);
            row.CreateCell(1).SetCellValue(project.Name);
            row.CreateCell(2).SetCellValue(project.FinanceCode);
            row.CreateCell(3).SetCellValue(project.Amount);
            row.CreateCell(4).SetCellValue(0);
            row.CreateCell(5).SetCellValue(0);
            row.CreateCell(6).SetCellValue(0);
            row.CreateCell(7).SetCellValue(0);
            row.CreateCell(8).SetCellValue(0);
            row.CreateCell(9).SetCellValue(0);
            row.CreateCell(10).SetCellValue(0);
            row.CreateCell(11).SetCellValue(0);
            row.CreateCell(12).SetCellValue(0);
            row.CreateCell(13).SetCellValue(0);
            row.CreateCell(14).SetCellValue(project.TypeOfPayment);
            row.CreateCell(15).SetCellValue(project.AmountReceivedStatus == (int)AmountReceivedStatus.Part
                ? project.AmountReceivedStatusString + project.AmountReceived
                : project.AmountReceivedStatusString);
            row.CreateCell(16).SetCellValue(project.InvoiceStatusString);

            if (items != null)
            {
                foreach (var item in items)
                {
                    row.GetCell(item.BudgetCategory + FinanceSheetDataRowStartIndex).SetCellValue(item.Total);
                }
            }

            FinanceSheetDataRowIndex++;
        }

        public void GroupProjectTypeColumn()
        {
            if (ProjectSheetDataRowIndex > ProjectSheetDataRowStartIndex)
            {
                var cellIndex = 1;
                var tempIndex = ProjectSheetDataRowStartIndex;
                var tempValue = ProjectSheet.GetRow(ProjectSheetDataRowStartIndex).GetCell(cellIndex).StringCellValue;
                var endIndex = ProjectSheetDataRowIndex - 1;
                for (int i = ProjectSheetDataRowStartIndex + 1; i <= endIndex; i++)
                {
                    var cellValue = ProjectSheet.GetRow(i).GetCell(cellIndex).StringCellValue;
                    if (cellValue != tempValue)
                    {
                        if (i - tempIndex > 1)
                        {
                            ProjectSheet.AddMergedRegion(new CellRangeAddress(tempIndex, i - 1, cellIndex, cellIndex));
                        }
                        tempValue = cellValue;
                        tempIndex = i;
                    }
                    else if (i == endIndex)
                    {
                        ProjectSheet.AddMergedRegion(new CellRangeAddress(tempIndex, i, cellIndex, cellIndex));
                    }
                }
            }
        }

        public void SaveToFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                HSSFWorkbook.Write(fs);
            }
        }
    }
}