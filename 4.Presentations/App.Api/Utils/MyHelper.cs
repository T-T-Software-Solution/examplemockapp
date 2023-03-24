using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Net;
using TodoAPI2.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TTSW.Utils;

/// <summary>
/// Summary description for MyHelper
/// </summary>
public static class MyHelper
{

    public static T ToObject<T>(this DataRow dataRow) where T : new()
    {
        T item = new T();

        foreach (DataColumn column in dataRow.Table.Columns)
        {
            PropertyInfo property = GetProperty(typeof(T), column.ColumnName);

            if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL")
            {
                property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
            }
        }

        return item;
    }

    private static PropertyInfo GetProperty(Type type, string attributeName)
    {
        PropertyInfo property = type.GetProperty(attributeName);

        if (property != null)
        {
            return property;
        }

        return type.GetProperties()
             .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
             .FirstOrDefault();
    }

    public static object ChangeType(object value, Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
        }

        return Convert.ChangeType(value, type);
    }

    public static string GetDummyText()
    {
        return Guid.NewGuid().ToString();
    }

    public static string GetDecimalStringForReport(decimal? d)
    {
        string result = "";

        if (d.HasValue)
        {
            result = d.ToString();
        }

        return result;
    }

    public static DateTime? RemoveTimeFromDate(DateTime? d)
    {
        if (d.HasValue)
        {
            return new DateTime(d.Value.Year, d.Value.Month, d.Value.Day);
        }
        return null;
    }

    public static string GetDateStringForReport(DateTime? date)
    {
        if (date.HasValue)
        {
            DateTime d = date.Value;

            string year = "";

            var thaimonth = new string[12]{ "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" };

            int y = d.Year;
            if (y <= 2500) y += 543;
            year = y.ToString();

            return d.Day.ToString() + " " + thaimonth[d.Month-1] + " " + year;
        }
        else
        {
            return "";
        }
    }

    public static string GetStringFromDate(DateTime? date)
    {
        if (date.HasValue)
        {
            DateTime d = date.Value;

            string day = "";
            string month = "";
            string year = "";

            int y = d.Year;
            if (y <= 2500) y += 543;

            if (d.Day < 10) day = "0" + d.Day.ToString(); else day = d.Day.ToString();
            if (d.Month < 10) month = "0" + d.Month.ToString(); else month = d.Month.ToString();
            year = y.ToString();

            return day + "/" + month + "/" + year;
        }
        else
        {
            return "";
        }        
    }

    public static string GetTimeStringFromDate(DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToShortTimeString() + "น.";
        }
        else
        {
            return "";
        }
    }

    public static DateTime? GetDateFromString(string date)
    {
        if (string.IsNullOrEmpty(date)) return null;
        if (date.Split('/').Length != 3) return null;

        string[] s = date.Split('/');

        int day = 0;
        int month = 0;
        int year = 0;

        day = Convert.ToInt16(s[0]);
        month = Convert.ToInt16(s[1]);
        year = Convert.ToInt16(s[2]);

        if (year >= 2500) year -= 543;

        DateTime d = new DateTime(year, month, day);
        return d;
    }

    public static string GetPriceThaiBath(decimal? a)
    {
        string result = "";

        if (a.HasValue)
        {
            result = GetPricePrint((double)a.Value);
        }

        return result;
    }

    public static string GetPriceText(double a)
    {
        Dictionary<int, string> index = new Dictionary<int, string>();
        index[1] = "หนึ่ง";
        index[2] = "สอง";
        index[3] = "สาม";
        index[4] = "สี่";
        index[5] = "ห้า";
        index[6] = "หก";
        index[7] = "เจ็ด";
        index[8] = "แปด";
        index[9] = "เก้า";
        index[0] = "ศูนย์";

        Dictionary<int, string> b = new Dictionary<int, string>();
        b[1] = "";
        b[10] = "สิบ";
        b[100] = "ร้อย";
        b[1000] = "พัน";
        b[10000] = "หมื่น";
        b[100000] = "แสน";
        b[1000000] = "ล้าน";

        string x = "";
        int ix = 0;
        double iy = a;

        int i = 1000000;
        while (i >= 1)
        {
            ix = (int)Math.Floor(Convert.ToDecimal(iy) / Convert.ToDecimal(i));
            iy = iy - ix * i;
            if (i == 10 & ix == 2)
            {
                x += "ยี่" + b[i];
            }
            else if (i == 1 & ix == 1)
            {
                x += "เอ็ด";
            }
            else if (i == 10 & ix == 1)
            {
                x += "สิบ";
            }
            else if (ix > 0 & ix < 10)
            {
                x += index[ix] + b[i];
            }

            i = i / 10;
        }

        //x += "บาท";


        return x;
    }

    public static string GetPricePrint(double a)
    {
        string x1 = GetPriceText(a) + "บาท ";
        string x2 = GetPriceText(Convert.ToDouble(a.ToString("N2").Split('.')[1]));
        return x1 + x2 + (x2 == "" ? "ถ้วน" : "สตางค์");
    }

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

    public static string GetParameterForJasperReport(object obj)
    {
        string parameter = "";
        foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
        {
            if (propertyInfo.GetValue(obj, null) != null)
            {
                if(propertyInfo.GetValue(obj, null).ToString() != "")
                {
                    if (parameter != "") parameter += "&";
                    var value = propertyInfo.GetValue(obj, null);
                    if (propertyInfo.PropertyType.ToString().Contains("Date"))
                    {
                        var d = (DateTime)propertyInfo.GetValue(obj, null);

                        value = (d.Year > 2400 ? d.Year - 543 : d.Year).ToString() + "-"
                            + (d.Month < 10 ? "0" + d.Month.ToString() : d.Month.ToString()) + "-"
                            + (d.Day < 10 ? "0" + d.Day.ToString() : d.Day.ToString());
                    }
                    if (propertyInfo.PropertyType.ToString().Contains("Decimal")
                        || propertyInfo.PropertyType.ToString().Contains("Double")
                        || propertyInfo.PropertyType.ToString().Contains("Float"))
                    {
                        if(value != null)
                        {
                            value = ((decimal)value).ToString("0.#####");
                        }                        
                    }
                    parameter += propertyInfo.Name + "=" + value;
                }                
            }
        }
        return parameter;
    }

    public static WebClient getHttpClient(IConfiguration Configuration)
    {
        string mainurl = MyHelper.GetConfig(Configuration, "JasperReportServer:MainURL");
        string loginurl = MyHelper.GetConfig(Configuration, "JasperReportServer:LoginURL");
        string username = MyHelper.GetConfig(Configuration, "JasperReportServer:username");
        string password = MyHelper.GetConfig(Configuration, "JasperReportServer:password");

        WebClient httpclient = new WebClient();
        string login_url = $"{loginurl}?j_username={username}&j_password={password}";
        var result = httpclient.DownloadString(loginurl);
        string session = httpclient.ResponseHeaders.Get("Set-Cookie");
        httpclient.Headers.Add("Cookie", session);
        return httpclient;
    }

    

    public static bool checkAuth(IConfiguration Configuration, Microsoft.AspNetCore.Http.HttpContext context)
    {
        //if (!string.IsNullOrEmpty(context.Request.Cookies["user_id"]))
        //{
        //    return true;
        //}

        //return false;

        return true;

    }

    public static string GetConfig(IConfiguration Configuration, string variable)
    {
        var env_var = Environment.GetEnvironmentVariable(variable);
        if (string.IsNullOrEmpty(env_var))
        {
            env_var = Environment.GetEnvironmentVariable(variable.Replace(":", "_"));
        }
        string var = !string.IsNullOrEmpty(env_var) ? env_var : Configuration[variable];
        return var;
    }

    public static decimal RoundOff(decimal i, decimal round_number)
    {
        string temp = i.ToString().Split(".")[0];

        return Convert.ToDecimal(temp.Substring(0, temp.Length - 1)) * 10 + 10;
    }
}
