using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    static class Common
    {
        public static string GetContentType(string fileType)
        {
            //One of the following formats: pdf, html, xls, xlsx, rtf, csv, xml, docx, odt, ods.
            if (fileType == "pdf")
            {
                return "application/pdf";
            }
            if (fileType == "xls")
            {
                return "application/vnd.ms-excel";
            }
            if (fileType == "xlsx")
            {
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            if (fileType == "rtf")
            {
                return "application/rtf";
            }
            if (fileType == "csv")
            {
                return "text/csv";
            }
            if (fileType == "xml")
            {
                return "text/xml";
            }
            if (fileType == "docx")
            {
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            if (fileType == "odt")
            {
                return "application/vnd.oasis.opendocument.text";
            }
            if (fileType == "ods")
            {
                return "application/vnd.oasis.opendocument.spreadsheet";
            }

            return "";
        }

        public static string GetDateStringForReport(DateTime? date)
        {
            if (date.HasValue)
            {
                DateTime d = date.Value;

                string year = "";

                var thaimonth = new string[12] { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };

                int y = d.Year;
                if (y <= 2500) y += 543;
                year = y.ToString();

                return d.Day.ToString() + " " + thaimonth[d.Month - 1] + " " + year;
            }
            else
            {
                return "";
            }
        }

        public static string GetDateStringForReportEN(DateTime? date)
        {
            if (date.HasValue)
            {
                DateTime d = date.Value;

                string year = "";

                var thaimonth = new string[12] { "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December" };

                int y = d.Year;
                if (y > 2500) y -= 543;
                year = y.ToString();

                return d.Day.ToString() + " " + thaimonth[d.Month - 1] + " " + year;
            }
            else
            {
                return "";
            }
        }
    }
}
