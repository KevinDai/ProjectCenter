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

namespace ProjectCenter.Web.Exports
{
    public class ProjectExprot
    {
        private static readonly int DataRowStartIndex = 2;
        private static readonly string Title = "咨询策划中心工作任务清单";
        private static readonly ExportColumn[] Columns = new ExportColumn[] {
            new ExportColumn(){Name="序号",Width=5},
            new ExportColumn(){Name="类别",Width=10},
            new ExportColumn(){Name="任务内容",Width=25},
            new ExportColumn(){Name="委托单位",Width=25},
            new ExportColumn(){Name="工作内容",Width=25},
            new ExportColumn(){Name="具体进展",Width=25},
            new ExportColumn(){Name="责任人",Width=25},
            new ExportColumn(){Name="参与人",Width=25},
            new ExportColumn(){Name="完成时间",Width=10},
            new ExportColumn(){Name="需要的支持",Width=25},
            new ExportColumn(){Name="推进计划",Width=25},
            new ExportColumn(){Name="状态",Width=10},
            new ExportColumn(){Name="开始年份",Width=10},
            new ExportColumn(){Name="截止年份",Width=10}
        };

        protected HSSFWorkbook HSSFWorkbook
        {
            get;
            private set;
        }

        protected ISheet Sheet
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

        protected int DataRowIndex
        {
            get;
            private set;
        }

        public ProjectExprot()
        {
            HSSFWorkbook = new HSSFWorkbook();

            ColumnDefaultStyle = CreateColumnDefaultStyle();
            TitleCellDefaultStyle = CreateTitleCellStyle();
            HeaderCellDefaultStyle = CreateHeaderCellDefaultStyle();

            Sheet = HSSFWorkbook.CreateSheet("sheet1");
            Sheet.DisplayGridlines = true;
            SetColumnDefaultStyle(Sheet);
            WriteTitle(Sheet);
            WriteHeader(Sheet);

            DataRowIndex = DataRowStartIndex;
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
        private ICellStyle CreateColumnDefaultStyle()
        {
            ICellStyle HeaderStyle = HSSFWorkbook.CreateCellStyle();
            HeaderStyle.Alignment = HorizontalAlignment.CENTER;
            HeaderStyle.VerticalAlignment = VerticalAlignment.CENTER;
            //create a font style
            IFont font = HSSFWorkbook.CreateFont();
            font.FontHeightInPoints = 9;
            //font.FontHeight = 10 * 10;
            font.FontName = "宋体";
            HeaderStyle.SetFont(font);
            HeaderStyle.BorderBottom = BorderStyle.THIN;
            HeaderStyle.BorderLeft = BorderStyle.THIN;
            HeaderStyle.BorderRight = BorderStyle.THIN;
            HeaderStyle.BorderTop = BorderStyle.THIN;
            HeaderStyle.WrapText = true;
            return HeaderStyle;
        }

        private void SetColumnDefaultStyle(ISheet sheet)
        {
            for (int i = 0; i < Columns.Length; i++)
            {
                sheet.SetDefaultColumnStyle(i, ColumnDefaultStyle);
            }
        }

        private void WriteTitle(ISheet sheet)
        {
            IRow row = sheet.CreateRow(0);
            var cell = row.CreateCell(0);
            cell.CellStyle = TitleCellDefaultStyle;
            cell.SetCellValue(Title);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, Columns.Length - 1));
        }

        private void WriteHeader(ISheet sheet)
        {
            IRow row = sheet.CreateRow(1);
            for (int i = 0; i < Columns.Length; i++)
            {
                var column = Columns[i];
                var cell = row.CreateCell(i);
                cell.CellStyle = HeaderCellDefaultStyle;
                cell.SetCellValue(column.Name);
                sheet.SetColumnWidth(i, column.Width * 256);
            }
        }

        //public void WriteProjects(IEnumerable<Project> projects)
        //{
        //    foreach (var project in projects)
        //    {
        //        WriteProject(project);
        //    }
        //}

        public void WriteProject(Project project)
        {
            var row = Sheet.CreateRow(DataRowIndex);
            row.CreateCell(0).SetCellValue(DataRowIndex - DataRowStartIndex + 1);
            row.CreateCell(1).SetCellValue(project.TypeString);
            row.CreateCell(2).SetCellValue(project.Name);
            row.CreateCell(3).SetCellValue(project.Consignor);
            row.CreateCell(4).SetCellValue(project.NeedFinish);
            row.CreateCell(5).SetCellValue(project.CurrentProgress);
            row.CreateCell(6).SetCellValue(project.ManagerNames);
            row.CreateCell(7).SetCellValue(project.ParticipantNames);
            row.CreateCell(8).SetCellValue(project.Deadline.ToShortDateString());
            row.CreateCell(9).SetCellValue(project.NeedSupport);
            row.CreateCell(10).SetCellValue(project.AdvancePlan);
            row.CreateCell(11).SetCellValue(project.StatusString);
            row.CreateCell(12).SetCellValue(project.StartTime.Year.ToString());
            row.CreateCell(13).SetCellValue(project.Deadline.Year.ToString());
            DataRowIndex++;
        }

        public void GroupProjectTypeColumn()
        {
            if (DataRowIndex > DataRowStartIndex)
            {
                var cellIndex = 1;
                var tempIndex = DataRowStartIndex;
                var tempValue = Sheet.GetRow(DataRowStartIndex).GetCell(cellIndex).StringCellValue;
                var endIndex = DataRowIndex - 1;
                for (int i = DataRowStartIndex + 1; i <= endIndex; i++)
                {
                    var cellValue = Sheet.GetRow(i).GetCell(cellIndex).StringCellValue;
                    if (cellValue != tempValue)
                    {
                        if (i - tempIndex > 1)
                        {
                            Sheet.AddMergedRegion(new CellRangeAddress(tempIndex, i - 1, cellIndex, cellIndex));
                        }
                        tempValue = cellValue;
                        tempIndex = i;
                    }
                    else if (i == endIndex)
                    {
                        Sheet.AddMergedRegion(new CellRangeAddress(tempIndex, i, cellIndex, cellIndex));
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