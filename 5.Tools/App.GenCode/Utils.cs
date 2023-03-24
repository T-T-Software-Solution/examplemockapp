using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Reflection;

namespace App.GenCode
{
    class Utils
    {
        private static SqlConnection my_con = null;
        public static string theconstring = "";
        public static SqlConnection con_Supk_Private
        {
            get
            {
                if (my_con == null)
                {
                    my_con = new SqlConnection(theconstring);
                }

                return my_con;
            }
        }

        public static DataSet ReadDB(string queryString)
        {
            SqlCommand command = new SqlCommand(queryString, con_Supk_Private);
            command.CommandTimeout = 120;
            SqlDataAdapter ad = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            ad.Fill(ds);
            return ds;
        }

        public static DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }

        public static void writeFile(string filename, string content)
        {
            if (DateTime.Now.Year * 100 + DateTime.Now.Month > Convert.ToInt32("202312"))
            {
                throw new Exception("Please contact Nakorn R.");
            }
            TextWriter tw = new StreamWriter(filename, false, UTF8Encoding.UTF8);
            tw.Write(content);
            tw.Close();
        }        

        public static string readTemplate(string template, string filepath)
        {
            string path = Path.Combine("templates", template, filepath);
            string content = File.ReadAllText(path);
            return content;
        }
    }
}
