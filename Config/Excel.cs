using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesWeb.Models.CommonConfig
{
    public class Excel
    {
        public static (byte[], string, string) GenerateExcel<T>(string fileName, IEnumerable<T> models, string[] columnsName, string[] columsValue)
        {
            DataTable dataTable = new DataTable("Table");
            foreach (var column in columnsName)
            {
                dataTable.Columns.Add(new DataColumn(column));
            }

            foreach (var model in models)
            {
                int i = 0;
                var row = dataTable.NewRow();
                foreach (var column in columsValue)
                {
                    //GetProperty lấy thuộc tính có tên là  = column
                    row[columnsName[i++]] = typeof(T).GetProperty(column).GetValue(model, null);
                }
                dataTable.Rows.Add(row);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileContents = stream.ToArray();
                    return (fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

    }
}
