using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Npgsql;
using OfficeOpenXml;

namespace App.Helper
{
    public class ExcelHelper
    {
        private readonly ConfigHelper configHelper;
        private readonly CommonHelper commonHelper;
        private readonly FileHelper fileHelper;
        private readonly IHostingEnvironment hostingEnvironment;
        public ExcelHelper(ConfigHelper configHelper, CommonHelper commonHelper, FileHelper fileHelper, IHostingEnvironment hostingEnvironment)
        {
            this.configHelper = configHelper;
            this.commonHelper = commonHelper;
            this.fileHelper = fileHelper;
            this.hostingEnvironment = hostingEnvironment;
        }

        public string Export(string connString, string command, Dictionary<string, int> widths = null, string fileName = "default")
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = command;

                    using (var reader = cmd.ExecuteReader())
                    {
                        var sWebRootFolder = hostingEnvironment.WebRootPath;
                        var sFileName = $@"{fileName}.xlsx";
                        var file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                        if (file.Exists)
                        {
                            file.Delete();
                            file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                        }

                        using (var package = new ExcelPackage(file))
                        {
                            var columns = reader.GetColumnSchema();

                            var worksheet = package.Workbook.Worksheets.Add("sheet1");
                            var columnIndex = 1;
                            var rowIndex = 1;
                            var totalColumn = columns.Count;

                            foreach (var column in columns)
                            {
                                worksheet.Cells[rowIndex, columnIndex].Value = column.ColumnName;
                                if (widths != null)
                                {
                                    var width = 50;
                                    widths.TryGetValue(column.ColumnName, out width);
                                    worksheet.Column(columnIndex).Width = width;
                                }
                                columnIndex++;
                            }

                            rowIndex++;

                            while (reader.Read())
                            {
                                for (var i = 0; i < totalColumn; i++)
                                {
                                    columnIndex = i + 1;
                                    if (reader.IsDBNull(i))
                                    {
                                        worksheet.Cells[rowIndex, columnIndex].Value = "";
                                    }
                                    else
                                    {
                                        try
                                        {
                                            switch (reader.GetFieldType(i).FullName)
                                            {
                                                case "System.Int16":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetInt16(i);
                                                    break;
                                                case "System.Int32":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetInt32(i);
                                                    break;
                                                case "System.Int64":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetInt64(i);
                                                    break;
                                                case "System.Double":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetDouble(i);
                                                    break;
                                                case "System.Decimal":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetDecimal(i);
                                                    break;
                                                case "System.Float":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetFloat(i);
                                                    break;
                                                case "System.Boolean":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetBoolean(i);
                                                    break;
                                                case "System.DateTime":
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        commonHelper
                                                            .ToLongString(reader.GetDateTime(i));
                                                    break;
                                                default:
                                                    worksheet.Cells[rowIndex, columnIndex].Value =
                                                        reader.GetString(i);
                                                    break;
                                            }
                                        }
                                        catch (System.Exception)
                                        {
                                            worksheet.Cells[rowIndex, columnIndex].Value =
                                                "Failed to parsing data";
                                        }
                                    }
                                }

                                rowIndex++;
                            }
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            package.Save();
                        }
                    }
                }
            }
            return "";
        }

        public ExcelPackage OpenExcel(string source)
        {
            source = source.StartsWith("/") ? source.Substring(1) : source;
            var sWebRootFolder = hostingEnvironment.WebRootPath;
            var file = new FileInfo(Path.Combine(sWebRootFolder, source));
            
            var  package = new ExcelPackage(file);
            return package;
        }

        public string TruncateString(string val)
        {
            return val.Trim().ToLower();
        }
    }
}
