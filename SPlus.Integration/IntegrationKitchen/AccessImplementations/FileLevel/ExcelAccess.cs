using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;
using SPlus.Model;

namespace SPlus.Integration.IntegrationKitchen.AccessImplementations.FileLevel
{
    public class ExcelAccess : IAccess
    {
        FileInfo file;
        string filePath = "test.xls";
        HSSFWorkbook wbXls;
        HSSFSheet shXls;

        XSSFWorkbook wbXlsx;
        XSSFSheet shXlsx;

        ExcelFile excelFile;
        //Dictionary<string, double> values = new Dictionary<string, double>();

       List<Parameters> values = new List<Parameters>();

        public ExcelAccess()
        {
        }

        #region Xls

        private void ReadXlsExcelFile()
        {
            ReadXlsSheets();
            ReadXlsSheetColumns();
        }

        private void ReadXlsSheets()
        {
            // get sheets list from xls
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                wbXls = new HSSFWorkbook(fs);

                for (int i = 0; i < wbXls.Count; i++)
                {
                    excelFile.ExcelFileSheets.Add(new ExcelFileSheet
                    {
                        ExcelFileSheetId = excelFile.ExcelFileId + "." + wbXls.GetSheetAt(i).SheetName,
                        ExcelFileSheetName = wbXls.GetSheetAt(i).SheetName,
                        ExcelFileSheetColumn = new List<ExcelFileSheetColumn>()
                    });
                }
            }
        }

        private void ReadXlsSheetColumns()
        {
            for (int s = 0; s < excelFile.ExcelFileSheets.Count; s++)
            {
                shXls = (HSSFSheet)wbXls.GetSheet(excelFile.ExcelFileSheets[s].ExcelFileSheetName);

                if (shXls.GetRow(0) == null)
                {
                    throw new Exception("Your data source's structure is not correct");
                }

                excelFile.ExcelFileSheets[s].ExcelFileSheetColumn = new List<ExcelFileSheetColumn>();
                // write row value
                for (int j = 0; j < shXls.GetRow(0).Cells.Count; j++)
                {
                    var cell = shXls.GetRow(0).GetCell(j);

                    if (cell != null)
                    {
                        excelFile.ExcelFileSheets[s].ExcelFileSheetColumn.Add(new ExcelFileSheetColumn()
                        {
                            ExcelFileSheetColumnId = excelFile.ExcelFileSheets[s].ExcelFileSheetId + "." + shXls.GetRow(0).GetCell(j).StringCellValue,
                            ExcelFileSheetColumnName = shXls.GetRow(0).GetCell(j).StringCellValue
                        });
                    }
                }
            }
        }

        #endregion Xls

        #region Xls

        private void ReadXlsxExcelFile()
        {
            ReadXlsxSheets();
            ReadXlsxSheetColumns();
        }

        private void ReadXlsxSheets()
        {
            // get sheets list from xls
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                wbXlsx = new XSSFWorkbook(fs);

                for (int i = 0; i < wbXlsx.Count; i++)
                {
                    excelFile.ExcelFileSheets.Add(new ExcelFileSheet
                    {
                        ExcelFileSheetId = excelFile.ExcelFileId + "." + wbXlsx.GetSheetAt(i).SheetName,
                        ExcelFileSheetName = wbXlsx.GetSheetAt(i).SheetName,
                        ExcelFileSheetColumn = new List<ExcelFileSheetColumn>()
                    });
                }
            }
        }

        private void ReadXlsxSheetColumns()
        {
            for (int s = 0; s < excelFile.ExcelFileSheets.Count; s++)
            {
                shXlsx = (XSSFSheet)wbXlsx.GetSheet(excelFile.ExcelFileSheets[s].ExcelFileSheetName);

                if (shXlsx.GetRow(0) == null)
                {
                    throw new Exception("Your data source's structure is not correct");
                }

                excelFile.ExcelFileSheets[s].ExcelFileSheetColumn = new List<ExcelFileSheetColumn>();
                // write row value
                for (int j = 0; j < shXlsx.GetRow(0).Cells.Count; j++)
                {
                    var cell = shXlsx.GetRow(0).GetCell(j);

                    if (cell != null)
                    {
                        excelFile.ExcelFileSheets[s].ExcelFileSheetColumn.Add(new ExcelFileSheetColumn()
                        {
                            ExcelFileSheetColumnId = excelFile.ExcelFileSheets[s].ExcelFileSheetId + "." + shXlsx.GetRow(0).GetCell(j).StringCellValue,
                            ExcelFileSheetColumnName = shXlsx.GetRow(0).GetCell(j).StringCellValue
                        });
                    }
                }
            }
        }

        #endregion Xls

        public async Task<string> GetDatabaseDefintions()
        {
            //validate file existance
            if (!File.Exists(filePath))
            {
                throw new Exception("File not exist!");
            }
            file = new FileInfo(filePath);

            excelFile = new ExcelFile()
            {
                ExcelFileId = file.Name,
                ExcelFileName = file.Name,
                ExcelFileSheets = new List<ExcelFileSheet>()
            };

            if (file.Extension == "xls")
            {
                ReadXlsExcelFile();
            }
            else if (file.Extension == "xlsx")
            {
                ReadXlsxExcelFile();
            }
            else
            {
                throw new Exception("File type is not supported!");
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(excelFile);
            return jsonString.ToString();
        }


        public async Task<List<Parameters>> GetSelectedData(List<Field> Fields)
        {
            List<string> keys = new List<string>(); IntegrationDataFilter integrationDataFilter = IntegrationDataFilter.Sum;
            if (!File.Exists(filePath))
            {
                throw new Exception("File not exist!");
            }
            file = new FileInfo(filePath);

            if (file.Extension == "xls")
            {
                GetSelectedDataXls(Fields, integrationDataFilter);
            }
            else if (file.Extension == "xlsx")
            {
                GetSelectedDataXlsx(Fields, integrationDataFilter);
            }
            else
            {
                throw new Exception("File type is not supported!");
            }

            return values;
        }

        private void GetSelectedDataXls(List<Field> Fields, Enums.IntegrationDataFilter integrationDataFilter)
        {
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet;
            double outvalue;

            for (int i = 0; i < Fields.Count; i++)
            {
                var info = Fields[i].FielId.Split('.');
                sheet = hssfwb.GetSheet(info[1]);

                int valueCellIndex = 0;
                for (int cellIndex = 0; cellIndex < shXlsx.GetRow(0).Cells.Count; cellIndex++)
                {
                    if (shXlsx.GetRow(0).GetCell(cellIndex).ToString() == info[2])
                    {
                        valueCellIndex = cellIndex;
                    }
                }

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        if (sheet.GetRow(row).GetCell(valueCellIndex).CellType == CellType.Numeric)
                        {
                            if (double.TryParse(sheet.GetRow(row).GetCell(valueCellIndex).NumericCellValue.ToString(), out outvalue))
                            {
                                //values.Add(Fields[i].FielId, outvalue);
                                values.Add(new Parameters
                                {

                                    FieldId = Fields[i].FielId,
                                    Value = Convert.ToString(outvalue)

                                });
                            }
                            else
                            {
                                throw new Exception("Data format is not valid!");
                            }
                        }
                        else if (sheet.GetRow(row).GetCell(valueCellIndex).CellType == CellType.String)
                        {
                            if (double.TryParse(sheet.GetRow(row).GetCell(valueCellIndex).StringCellValue.ToString(), out outvalue))
                            {
                                // values.Add(Fields[i].FielId, outvalue);
                                values.Add(new Parameters
                                {

                                    FieldId = Fields[i].FielId,
                                    Value = Convert.ToString(outvalue)

                                });
                            }
                            else
                            {
                                throw new Exception("Data format is not valid!");
                            }
                        }
                        else
                        {
                            throw new Exception("Data format is not valid!");
                        }
                    }
                }
            }
        }

        private void GetSelectedDataXlsx(List<Field> Fields, Enums.IntegrationDataFilter integrationDataFilter)
        {
            XSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfwb = new XSSFWorkbook(file);
            }

            ISheet sheet;
            double outvalue;

            for (int i = 0; i < Fields.Count; i++)
            {
                var info = Fields[i].FielId.Split('.');
                sheet = hssfwb.GetSheet(info[1]);

                int valueCellIndex = 0;
                for (int cellIndex = 0; cellIndex < shXlsx.GetRow(0).Cells.Count; cellIndex++)
                {
                    if (shXlsx.GetRow(0).GetCell(cellIndex).ToString() == info[2])
                    {
                        valueCellIndex = cellIndex;
                    }
                }

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        if (sheet.GetRow(row).GetCell(valueCellIndex).CellType == CellType.Numeric)
                        {
                            if (double.TryParse(sheet.GetRow(row).GetCell(valueCellIndex).NumericCellValue.ToString(), out outvalue))
                            {
                                //values.Add(Fields[i].FielId, outvalue);
                                values.Add(new Parameters
                                {

                                    FieldId = Fields[i].FielId,
                                    Value = Convert.ToString(outvalue)

                                });
                            }
                            else
                            {
                                throw new Exception("Data format is not valid!");
                            }
                        }
                        else if (sheet.GetRow(row).GetCell(valueCellIndex).CellType == CellType.String)
                        {
                            if (double.TryParse(sheet.GetRow(row).GetCell(valueCellIndex).StringCellValue.ToString(), out outvalue))
                            {
                                //values.Add(Fields[i].FielId, outvalue);
                                values.Add(new Parameters
                                {

                                    FieldId = Fields[i].FielId,
                                    Value = Convert.ToString(outvalue)

                                });
                            }
                            else
                            {
                                throw new Exception("Data format is not valid!");
                            }
                        }
                        else
                        {
                            throw new Exception("Data format is not valid!");
                        }
                    }
                }
            }
        }
    }
}
