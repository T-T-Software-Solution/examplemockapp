using System;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using App.GenCode;
using System.Globalization;

TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

string enter = @"
";

string getNavigationEf(string tableName, Dictionary<string, string> fk)
{
    string result = @"
";

    foreach (var f in fk)
    {
        if (f.Key.EndsWith(tableName))
        {
            string toTableName = f.Key.Replace("->", ">").Split(">")[0];
            result += $@"        public List<{toTableName}Entity> {toTableName}s <1> get; <2> = new();" + enter;
        }
    }

    return result;
}

string fix_key_method(string template, string key_data_type)
{
    string int_key_method = @"
        public int GetNewPrimaryKey()
        <1>
            int? newkey = 0;

            var x = (from i in _dataContext.{19}s
                    orderby i.id descending
                    select i).Take(1).ToList();

            if(x.Count > 0)
            <1>
                newkey = x[0].id + 1;
            <2>

            return newkey.Value;
        <2>
";

    if (key_data_type.StartsWith("int"))
    {
        return template.Replace("<new_key_method>", int_key_method);
    }
    else
    {
        return template.Replace("<new_key_method>", "");
    }
}

string getLayout(string table_name, int maxrow, int maxcol,
            Dictionary<string, string> layout_ui,
            Dictionary<string, string> layout_desc,
            Dictionary<string, string> layout_length,
            Dictionary<string, string> layout_column_name,
            string template)
{
    string result = "";
    string total_hiddenfield = "";

    for (int i = 0; i <= maxrow; i++)
    {
        result += "<div class='row'>";
        for (int j = 0; j <= maxcol; j++)
        {
            if (layout_ui.ContainsKey(i.ToString() + "," + j.ToString()))
            {
                string length = layout_length[i.ToString() + "," + j.ToString()];
                string desc = layout_desc[i.ToString() + "," + j.ToString()];
                string ui = layout_ui[i.ToString() + "," + j.ToString()];
                string ColumnName = layout_column_name[i.ToString() + "," + j.ToString()];

                string result_ui = ui.Replace("{p}", "").Replace("{0}", table_name + "_" + ColumnName)
                    .Replace("{1}", table_name).Replace("{2}", ColumnName).Replace("{3}", desc);

                if (result_ui.Contains("hidden") && !result_ui.Contains("<p>"))
                {
                    total_hiddenfield += result_ui + enter;
                }
                else
                {
                    result += string.Format(template,
                    '"'.ToString(),
                    length,
                    desc,
                    result_ui,
                    table_name,
                    ColumnName
                    );
                }
            }
        }
        result += "</div>" + enter;
    }

    return total_hiddenfield + enter + result;
}

string getLayoutReact(string table_name, int maxrow, int maxcol,
            Dictionary<string, string> layout_ui,
            Dictionary<string, string> layout_desc,
            Dictionary<string, string> layout_length,
            Dictionary<string, string> layout_column_name,
            string template)
{
    string result = "";
    string total_hiddenfield = "";

    for (int i = 0; i <= maxrow; i++)
    {
        result += "";
        for (int j = 0; j <= maxcol; j++)
        {
            if (layout_ui.ContainsKey(i.ToString() + "," + j.ToString()))
            {
                string length = layout_length[i.ToString() + "," + j.ToString()];
                string desc = layout_desc[i.ToString() + "," + j.ToString()];
                string ui = layout_ui[i.ToString() + "," + j.ToString()];
                string ColumnName = layout_column_name[i.ToString() + "," + j.ToString()];

                string result_ui = ui.Replace("{p}", "").Replace("{0}", ColumnName)
                    .Replace("{1}", table_name).Replace("{2}", ColumnName).Replace("{3}", desc);

                result += string.Format(template,
                    '"'.ToString(),
                    length,
                    desc,
                    result_ui,
                    table_name,
                    ColumnName
                    );
            }
        }
        result += "" + enter;
    }

    return total_hiddenfield + enter + result;
}


Console.WriteLine($".net core 7 gen tool version {Assembly.GetExecutingAssembly().GetName().Version.ToString()} by Nakorn R.");

#region Read config and set variable
var configfile = Utils.GetDataTableFromExcel(Path.Combine("./", "config.xlsx"));
Dictionary<string, string> con = new Dictionary<string, string>();
foreach (DataRow i in configfile.Rows)
{
    con.Add(i["name"].ToString(), i["value"].ToString());
}

string template = con["template"];
string dir_path = Path.Combine(con["rootpath"]);
string target_path = Path.Combine(con["targetpath"]);
Utils.theconstring = @"Server=LAPTOP-KB8JC2K2\SQLEXPRESS;Integrated Security=true;Initial Catalog=BLL2Core2"; //con["ConnectionString"];
string textbox1 = con["projectcode"];

Console.WriteLine("root path = " + dir_path);
Console.WriteLine("target path = " + target_path);
Console.WriteLine("Generating source code...");
Console.WriteLine();

#endregion

string table_name = "";
string real_table_name = "";

#region declare Template

#region model

string B64Entity_Template = Utils.readTemplate("core", Path.Combine("model", "B64Entity_Template.txt"));
string Entity_template = Utils.readTemplate("core", Path.Combine("model", "Entity_template.txt"));
string ViewModel_template = Utils.readTemplate("core", Path.Combine("model", "ViewModel_template.txt"));
string InputModel_template = Utils.readTemplate("core", Path.Combine("model", "InputModel_template.txt"));
string SearchModel_template = Utils.readTemplate("core", Path.Combine("model", "SearchModel_template.txt"));
string ReportRequestModel_template = Utils.readTemplate("core", Path.Combine("model", "ReportRequestModel_template.txt"));
string MappingProfile_template = Utils.readTemplate("core", Path.Combine("model", "MappingProfile_template.txt"));

#endregion

#region service

string FK_Service_Template = Utils.readTemplate("core", Path.Combine("service", "FK_Service_Template.txt"));
string iservice_template = Utils.readTemplate("core", Path.Combine("service", "iservice_template.txt"));
string service_template = Utils.readTemplate("core", Path.Combine("service", "service_template.txt"));

#endregion

#region upload service

string upload_add = Utils.readTemplate("core", Path.Combine("uploadservice", "upload_add.txt"));
string upload_update = Utils.readTemplate("core", Path.Combine("uploadservice", "upload_update.txt"));
string upload_db_add = Utils.readTemplate("core", Path.Combine("uploadservice", "upload_db_add.txt"));
string upload_db_update = Utils.readTemplate("core", Path.Combine("uploadservice", "upload_db_update.txt"));

#endregion

#region controller
string controller_template = Utils.readTemplate("core", Path.Combine("controller", "controller_template.txt"));
string view_controller_template = Utils.readTemplate("core", Path.Combine("controller", "view_controller_template.txt"));
#endregion

#region view

string CheckBoxListTemplate = Utils.readTemplate("core", Path.Combine("viewjs", "CheckBoxListTemplate.txt"));
string view_upload_js_template = Utils.readTemplate("core", Path.Combine("viewjs", "view_upload_js_template.txt"));
string view_g_template = Utils.readTemplate(template, Path.Combine("view", "view_g_template.txt"));
string view_grid_template = Utils.readTemplate(template, Path.Combine("view", "view_grid_template.txt"));
string view_d_template = Utils.readTemplate(template, Path.Combine("view", "view_d_template.txt"));
string view_js_template = Utils.readTemplate(template, Path.Combine("viewjs", "view_js_template.txt"));
string view_d_js_template = Utils.readTemplate("core", Path.Combine("viewjs", "view_d_js_template.txt"));
string view_js_report_template = Utils.readTemplate("core", Path.Combine("viewjs", "view_js_report_template.txt"));
string view_report_template = Utils.readTemplate(template, Path.Combine("view", "view_report_template.txt"));

#endregion

#region inline edit

string inline_html = Utils.readTemplate(template, Path.Combine("inlineedit", "inline_html.txt"));
string inline_js = Utils.readTemplate(template, Path.Combine("inlinejs", "inline_js.txt"));

#endregion

#region React

string react_index_template = Utils.readTemplate("core", Path.Combine("react", "index.txt"));
string react_form_template = Utils.readTemplate("core", Path.Combine("react", "form.txt"));
string react_datatable_template = Utils.readTemplate("core", Path.Combine("react", "datatable.txt"));
string react_searchform_template = Utils.readTemplate("core", Path.Combine("react", "searchform.txt"));

#endregion

#endregion

#region declare variable

Dictionary<string, string> key = new Dictionary<string, string>();
Dictionary<string, string> key_data_type = new Dictionary<string, string>();
Dictionary<string, string> nname = new Dictionary<string, string>();
Dictionary<string, string> nname_data_type = new Dictionary<string, string>();
Dictionary<string, string> fk = new Dictionary<string, string>();
Dictionary<string, int> layout_maxrow = new Dictionary<string, int>();
Dictionary<string, int> layout_maxcol = new Dictionary<string, int>();

if (Directory.Exists(Path.Combine(target_path, "Target")))
{
    Directory.Delete(Path.Combine(target_path, "Target"), true);
}
Directory.CreateDirectory(Path.Combine(target_path, "Target"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "Models"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "CoreInterface"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "InfraRepositories"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "ApiControllers"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "ViewControllers"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "Javascripts"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "Views"));
Directory.CreateDirectory(Path.Combine(target_path, "Target", "React"));

string entity_model = "";
string view_model = "";
string input_model = "";
string search_model = "";

// Model2
string ViewModelVar = "";
string JoinData = "";
string JoinResultMain = "";
string JoinResultFK = "";
string SelectionView = "";
string SelectionItemList = "";

string ServiceUploadAdd = "";
string ServiceUploadUpdate = "";
string FK_IService = "";
string FK_Service = "";
string xml_print = "";
string xml_map = "";
string ServiceUploadUpdateMultiple = "";
string whereSearch = "";
string LookupForLog = "";

string FeedDataToForm = "";
string GetFromForm = "";
string ClearForm = "";
string InitialForm = "";
string JS_Column_Data = "";
string FileUpload = "";
string MultiSelectionTXT = "";
string AfterInsertUpdate = "";
string AfterDelete = "";
string GetSearchParameter = "";
string FeedDataToSearchForm = "";
string search_ui = "";
string FormItem = "";
string TableHeader = "";
string Controller_SetValue = "";
string FormItemReact = "";
string ReactTableColumn = "";
string ReactSearchColumn = "";

// Inline Edit
string ClearForm_Inline = "";
string FeedDataToForm_Inline = "";
string GetFromForm_Inline = "";
string FormUI_Inline = "";
string Header_Inline = "";
string Get_Inline = "";
string Post_Inline = "";
string HiddenField_UI = "";

// Startup
string AutoMapper = "";
string DIService = "";
string DataContext = "";

#endregion

Dictionary<string, string> object_name = new Dictionary<string, string>();
Dictionary<string, string> object_type = new Dictionary<string, string>();

#region First Loop
foreach (string file3 in Directory.GetFiles(dir_path, "*.xlsx"))
{

    #region prepare
    var excel = Utils.GetDataTableFromExcel(file3);

    table_name = "";
    real_table_name = "";

    if (file3.Contains("@"))
    {
        var temp = file3.Split('@');
        table_name = temp[1].Replace(".xlsx", "");
        real_table_name = temp[0].Replace(".xlsx", "");
        table_name = table_name.Replace(dir_path, "");
        table_name = table_name.Replace(@"\", "").Replace(@"/", "");
        real_table_name = real_table_name.Replace(dir_path, "");
        real_table_name = real_table_name.Replace(@"\", "").Replace(@"/", "");
    }
    else
    {
        table_name = file3.Replace(".xlsx", "");
        table_name = table_name.Replace(dir_path, "");
        table_name = table_name.Replace(@"\", "").Replace(@"/", "");
        real_table_name = table_name;
    }

    if (table_name.StartsWith("_")) continue;

    #endregion

    string[] tmp = table_name.Split('@');
    real_table_name = tmp[0];
    table_name = tmp[0];
    if (tmp.Length == 2) table_name = tmp[1];

    layout_maxrow[table_name] = 0;
    layout_maxcol[table_name] = 0;

    // Foreach Row in Excel
    for (int j = 0; j < excel.Rows.Count; j++)
    {
        string ColumnName = excel.Rows[j]["Column Name"].ToString();
        string DataType = excel.Rows[j]["Data Type"].ToString();
        string PrimaryKey = excel.Rows[j]["Primary Key"].ToString();
        string FK = excel.Rows[j]["FK"].ToString();
        string UI = excel.Rows[j]["UI"].ToString();
        string row = excel.Rows[j]["row"].ToString();
        string column = excel.Rows[j]["column"].ToString();
        string length = excel.Rows[j]["length"].ToString();
        string on_grid = "yes";
        if (excel.Columns.Contains("on grid"))
        {
            on_grid = excel.Rows[j]["on grid"].ToString();
        }

        if (Convert.ToInt32(row) > layout_maxrow[table_name])
            layout_maxrow[table_name] = Convert.ToInt32(row);
        if (Convert.ToInt32(column) > layout_maxcol[table_name])
            layout_maxcol[table_name] = Convert.ToInt32(column);

        if (PrimaryKey == "yes") { key[table_name] = ColumnName; key_data_type[table_name] = DataType; };
        if (PrimaryKey == "name") { nname[table_name] = ColumnName; nname_data_type[table_name] = DataType; };
        if (FK != "") { fk[table_name + "->" + FK] = ColumnName; };
    }

    //Path.Combine(target_path,"Target", "Models", table_name, "I" + table_name + "Service.cs"

    if (!Directory.Exists(Path.Combine(target_path, "Target", "Models", table_name)))
    {
        Directory.CreateDirectory(Path.Combine(target_path, "Target", "Models", table_name));
    }
    //if (!Directory.Exists(Path.Combine(target_path, "Target", "CoreInterface", table_name)))
    //{
    //    Directory.CreateDirectory(Path.Combine(target_path, "Target", "CoreInterface", table_name));
    //}
    if (!Directory.Exists(Path.Combine(target_path, "Target", "InfraRepositories", table_name)))
    {
        Directory.CreateDirectory(Path.Combine(target_path, "Target", "InfraRepositories", table_name));
    }
    if (!Directory.Exists(Path.Combine(target_path, "Target", "Javascripts", table_name)))
    {
        Directory.CreateDirectory(Path.Combine(target_path, "Target", "Javascripts", table_name));
    }
    if (!Directory.Exists(Path.Combine(target_path, "Target", "Views", table_name + "View")))
    {
        Directory.CreateDirectory(Path.Combine(target_path, "Target", "Views", table_name + "View"));
    }
    if (!Directory.Exists(Path.Combine(target_path, "Target", "React", table_name)))
    {
        Directory.CreateDirectory(Path.Combine(target_path, "Target", "React", table_name));
    }
}

#endregion

#region Second Loop
foreach (string file3 in Directory.GetFiles(dir_path, "*.xlsx"))
{

    #region prepare
    var excel = Utils.GetDataTableFromExcel(file3);

    table_name = "";
    real_table_name = "";

    if (file3.Contains("@"))
    {
        var temp = file3.Split('@');
        table_name = temp[1].Replace(".xlsx", "");
        real_table_name = temp[0].Replace(".xlsx", "");
        table_name = table_name.Replace(dir_path, "");
        table_name = table_name.Replace(@"\", "").Replace(@"/", "");
        real_table_name = real_table_name.Replace(dir_path, "");
        real_table_name = real_table_name.Replace(@"\", "").Replace(@"/", "");
    }
    else
    {
        table_name = file3.Replace(".xlsx", "");
        table_name = table_name.Replace(dir_path, "");
        table_name = table_name.Replace(@"\", "").Replace(@"/", "");
        real_table_name = table_name;
    }

    if (table_name.StartsWith("_")) continue;

    entity_model = "";
    view_model = "";
    input_model = "";
    search_model = "";

    ViewModelVar = "";
    JoinData = "";
    JoinResultMain = "";
    JoinResultFK = "";
    SelectionView = "";
    SelectionItemList = "";

    ServiceUploadAdd = "";
    ServiceUploadUpdate = "";
    FK_IService = "";
    FK_Service = "";
    xml_print = "";
    xml_map = "";
    ServiceUploadUpdateMultiple = "";
    whereSearch = "";
    LookupForLog = "";

    FeedDataToForm = "";
    GetFromForm = "";
    ClearForm = "";
    InitialForm = "";
    JS_Column_Data = "";
    FileUpload = "";
    MultiSelectionTXT = "";
    AfterInsertUpdate = "";
    AfterDelete = "";
    GetSearchParameter = "";
    FeedDataToSearchForm = "";
    search_ui = "";
    FormItem = "";
    TableHeader = "";
    FormItemReact = "";
    ReactTableColumn = "";
    ReactSearchColumn = "";

    ClearForm_Inline = "";
    FeedDataToForm_Inline = "";
    GetFromForm_Inline = "";
    FormUI_Inline = "";
    Header_Inline = "";
    Get_Inline = "";
    Post_Inline = "";
    HiddenField_UI = "";
    Controller_SetValue = "";

    #endregion


    Dictionary<string, string> layout_ui = new Dictionary<string, string>();
    Dictionary<string, string> layout_desc = new Dictionary<string, string>();
    Dictionary<string, string> layout_length = new Dictionary<string, string>();
    Dictionary<string, string> layout_column_name = new Dictionary<string, string>();

    Dictionary<string, string> layout_ui_react = new Dictionary<string, string>();

    // Foreach Row in Excel
    for (int j = 0; j < excel.Rows.Count; j++)
    {
        string ColumnName = excel.Rows[j]["Column Name"].ToString();
        string DataType = excel.Rows[j]["Data Type"].ToString();
        string Size = excel.Rows[j]["Size"].ToString();
        string PrimaryKey = excel.Rows[j]["Primary Key"].ToString();
        string FK = excel.Rows[j]["FK"].ToString();
        string UI = excel.Rows[j]["UI"].ToString();
        string Desc = excel.Rows[j]["Desc"].ToString();
        string DescEN = excel.Rows[j]["Desc EN"].ToString();
        string row = excel.Rows[j]["row"].ToString();
        string column = excel.Rows[j]["column"].ToString();
        string length = excel.Rows[j]["length"].ToString();
        string MultiSelectionTable = excel.Rows[j]["MultiSelectionTable"].ToString();
        string on_grid = "yes";
        if (excel.Columns.Contains("on grid"))
        {
            on_grid = excel.Rows[j]["on grid"].ToString();
        }

        string HTML_tag = "";
        string JSHL_FeedDataToForm = "";
        string JSHL_GetFromForm = "";
        string JSHL_ClearForm = "";
        string JSHL_InitialForm = "";
        string React_tag = "";

        string HTML_tag_Inline = "";
        string JSHL_FeedDataToForm_Inline = "";
        string JSHL_GetFromForm_Inline = "";
        string JSHL_ClearForm_Inline = "";

        if (!string.IsNullOrEmpty(UI))
        {
            DataTable dt = Utils.ReadDB("select * from UI2 where UI2.UI='" + UI + "'").Tables[0];
            HTML_tag = dt.Rows[0]["HTML Tag"].ToString();
            JSHL_FeedDataToForm = dt.Rows[0]["FeedDataToForm"].ToString();
            JSHL_GetFromForm = dt.Rows[0]["GetFromForm"].ToString();
            JSHL_ClearForm = dt.Rows[0]["ClearForm"].ToString();
            JSHL_InitialForm = dt.Rows[0]["InitialForm"].ToString();
            React_tag = dt.Rows[0]["React Tag"].ToString();

            HTML_tag_Inline = dt.Rows[0]["HTML Tag_Inline"].ToString();
            if (UI == "HiddenField")
            {
                HTML_tag_Inline = "<input class={0}form-control{0} type={0}hidden{0} id={0}{1}_{2}_' + (i + 1)+'{0} />";
            }

            JSHL_FeedDataToForm_Inline = dt.Rows[0]["FeedDataToForm_Inline"].ToString();
            JSHL_GetFromForm_Inline = dt.Rows[0]["GetFromForm_Inline"].ToString();
            JSHL_ClearForm_Inline = dt.Rows[0]["ClearForm_Inline"].ToString();

            layout_ui[row + "," + column] = HTML_tag;
            layout_desc[row + "," + column] = Desc;
            layout_length[row + "," + column] = length;
            layout_column_name[row + "," + column] = ColumnName;
            layout_ui_react[row + "," + column] = React_tag;
        }

        #region model management

        //===========================================================
        if (UI == "CheckBoxList")
        //===========================================================
        {
            // ไม่ต้องสร้าง
        }
        //===========================================================
        else if (UI == "FileUpload" || UI == "ImageUpload")
        //===========================================================
        {
            if (DataType.StartsWith("string") && !string.IsNullOrEmpty(Size))
            {
                entity_model += string.Format(@"
        [MaxLength({0}), Column(Order = {1}), Comment(^{2}^)]".Replace('^','"'), Size, j+1, Desc);
            }
            else
            {
                entity_model += string.Format(@"
        [Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }
            entity_model += string.Format(@"
        public {1} {0} <1> get; set; <2>
", ColumnName, DataType, FK);
            entity_model += string.Format(@"
        [NotMapped]
        public string {1}Display
        <1>
            get
            <1>
                return (string.IsNullOrEmpty({1}) ? {0}{0} :
                    FileUtil.GetFileInfo(TTSW.Constant.FilePathConstant.DirType.FilesTestUpload, id, {1}).RelativePath).Replace(@{0}\{0}, {0}/{0});
            <2>
        <2>
", '"'.ToString(), ColumnName, DataType);


            view_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
        public {2} {1}Display <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);


            input_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
        }
        //===========================================================
        else if (UI == "FileUploadDB")
        //===========================================================
        {
            if (DataType.StartsWith("string") && !string.IsNullOrEmpty(Size))
            {
                entity_model += string.Format(@"
        [MaxLength({0}), Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }
            else
            {
                entity_model += string.Format(@"
        [Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }
            entity_model += string.Format(@"
        public {1} {0} <1> get; set; <2>
", ColumnName, DataType, FK);
            entity_model += string.Format(@"
        public {1} {0}Base64FileName <1> get; set; <2>
", ColumnName, DataType, FK);
            entity_model += string.Format(@"
        public {1} {0}Base64Content <1> get; set; <2>
", ColumnName, DataType, FK);
            entity_model += string.Format(@"
        public {1} {0}Base64MimeType <1> get; set; <2>
", ColumnName, DataType, FK);
            entity_model += string.Format(@"
        [NotMapped]
        public string {1}Display
        <1>
            get
            <1>
                return (string.IsNullOrEmpty({1}) ? {0}{0} :
                    FileUtil.GetFileInfo(TTSW.Constant.FilePathConstant.DirType.FilesTestUpload, id, {1}).RelativePath).Replace(@{0}\{0}, {0}/{0});
            <2>
        <2>
", '"'.ToString(), ColumnName, DataType);
            entity_model += string.Format(@"
        [NotMapped]
        public string {1}Base64DataURL
        <1>
            get
            <1>
                return (string.IsNullOrEmpty({1}Base64Content) || string.IsNullOrEmpty({1}Base64MimeType) ? {0}{0} :
                    FileUtil.GetBase64Info({1}Base64Content, {1}Base64MimeType).DataURL);
            <2>
        <2>
", '"'.ToString(), ColumnName, DataType);

            view_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
        public {2} {1}Base64DataURL <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);

            input_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
        public {2} {1}PathForBase64 <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
        }
        //===========================================================
        else if (!string.IsNullOrEmpty(FK))
        //===========================================================
        {
            var temp = "";
            if (DataType.StartsWith("string") && !string.IsNullOrEmpty(Size))
            {
                temp = string.Format(@"
        [MaxLength({0}), Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }
            else
            {
                temp = string.Format(@"
        [Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }

            if (FK == "external_linkage")
            {
                entity_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
                view_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);

            }
            else
            {
                entity_model += string.Format(@"
        [ForeignKey({0}{1}{0})]
        public {3}Entity? {3}_{1} <1> get; set; <2>
{4}
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK, temp);
                view_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);

            }

            input_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
        }
        //===========================================================
        else if (PrimaryKey != "yes" && !string.IsNullOrEmpty(UI))
        //===========================================================
        {
            if (DataType.StartsWith("string") && !string.IsNullOrEmpty(Size))
            {
                entity_model += string.Format(@"
        [MaxLength({0}), Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }
            else
            {
                entity_model += string.Format(@"
        [Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }

            entity_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
            view_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);

            input_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
        }
        //===========================================================
        else if (PrimaryKey != "yes")
        //===========================================================
        {
            if (DataType.StartsWith("string") && !string.IsNullOrEmpty(Size))
            {
                entity_model += string.Format(@"
        [MaxLength({0}), Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }
            else
            {
                entity_model += string.Format(@"
        [Column(Order = {1}), Comment(^{2}^)]".Replace('^', '"'), Size, j + 1, Desc);
            }

            entity_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
            view_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);


            input_model += "";
        }
        //===========================================================
        else if (PrimaryKey == "yes")
        //===========================================================
        {
            input_model += string.Format(@"
        public {2}? {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
        }

        if (!string.IsNullOrEmpty(FK))
        {
            ViewModelVar += string.Format("        public {3} {2}_{0}_{1} <1> get; set; <2>" + enter, FK, nname[FK], ColumnName, nname_data_type[FK]);
            if (FK == "external_linkage" && DataType.StartsWith("string"))
            {
                JoinData += string.Format(@"
                join fk_{0}{4} in ext.GetDemoItem() on m_{1}.{2} equals fk_{0}{4}.external_code
                into {0}Result{4}
                from fk_{0}Result{4} in {0}Result{4}.DefaultIfEmpty()
", FK.ToLower(), table_name.ToLower(), ColumnName, key[FK], j.ToString());
                JoinResultFK += $"                    {ColumnName}_{FK}_{nname[FK]} = fk_{FK.ToLower()}Result{j.ToString()}.{nname[FK]}," + enter;
            }
            else if (FK == "external_linkage" && DataType.StartsWith("int"))
            {
                JoinData += string.Format(@"
                join fk_{0}{4} in ext.GetDemoItem() on m_{1}.{2} equals fk_{0}{4}.{3}
                into {0}Result{4}
                from fk_{0}Result{4} in {0}Result{4}.DefaultIfEmpty()
", FK.ToLower(), table_name.ToLower(), ColumnName, key[FK], j.ToString());
                JoinResultFK += $"                    {ColumnName}_{FK}_{nname[FK]} = fk_{FK.ToLower()}Result{j.ToString()}.{nname[FK]}," + enter;
            }
            else
            {
                JoinData += string.Format(@"
                join fk_{0}{4} in _dataContext.{5}s on m_{1}.{2} equals fk_{0}{4}.{3}
                into {0}Result{4}
                from fk_{0}Result{4} in {0}Result{4}.DefaultIfEmpty()
", FK.ToLower(), table_name.ToLower(), ColumnName, key[FK], j.ToString(), FK);
                JoinResultFK += $"                    {ColumnName}_{FK}_{nname[FK]} = fk_{FK.ToLower()}Result{j.ToString()}.{nname[FK]}," + enter;
            }
        }

        if (UI == "CheckBoxList" && !string.IsNullOrEmpty(MultiSelectionTable))
        {
            SelectionView += $"        public List<{MultiSelectionTable}ViewModel>? item_{ColumnName} <1> get; set; <2>" + enter;
            SelectionView += $"        public List<{key_data_type[MultiSelectionTable]}>? selected_{ColumnName} <1> get; set; <2>" + enter;
            input_model += string.Format(@"
        public List<{2}> selected_{1} <1> get; set; <2>
", '"'.ToString(), ColumnName, key_data_type[MultiSelectionTable]);
        }
        if (UI == "DropDownList" && !string.IsNullOrEmpty(FK))
        {
            if (FK == "external_linkage")
            {
                SelectionView += $"        public List<{FK}ViewModel>? item_{ColumnName} <1> get; set; <2>" + enter;
            }
            else
            {
                SelectionView += $"        public List<{FK}ViewModel>? item_{ColumnName} <1> get; set; <2>" + enter;
            }
        }

        if (UI != "CheckBoxList")
        {
            JoinResultMain += string.Format("                    {0} = m_{1}.{0}," + enter, ColumnName, table_name.ToLower());
        }
        if (UI == "FileUpload" || UI == "ImageUpload")
        {
            JoinResultMain += string.Format("                    {0}Display = m_{1}.{0}Display," + enter, ColumnName, table_name.ToLower());
        }
        if (UI == "FileUploadDB")
        {
            JoinResultMain += string.Format("                    {0}Base64DataURL = m_{1}.{0}Base64DataURL," + enter, ColumnName, table_name.ToLower());
        }

        if (UI == "TextBoxDate")
        {
            view_model += string.Format(@"
        public string txt_{0} <1> get <1> return Common.GetDateStringForReport(this.{0}); <2> <2>
", ColumnName);

        }
        if (UI == "FileUpload" || UI == "ImageUpload")
        {
            view_model += string.Format(@"
        public string txt_{0}
        <1>
            get
            <1>
                return (string.IsNullOrEmpty({0}) ? {1}{1} :
                    ${1}<a href='../<1>{0}Display<2>' target='_blank'><1>{0}<2></a>{1});
            <2>
        <2>		
", ColumnName, '"'.ToString());
        }
        if (UI == "FileUploadDB")
        {
            view_model += string.Format(@"
        public string txt_{0}
        <1>
            get
            <1>
                return (string.IsNullOrEmpty({0}) ? {1}{1} :
                    ${1}<a href='<1>{0}Base64DataURL<2>' target='_blank'><1>{0}<2></a>{1});
            <2>
        <2>		
", ColumnName, '"'.ToString());
        }

        if (PrimaryKey == "name" || PrimaryKey == "search" || PrimaryKey == "yes")
        {
            search_model += string.Format(@"
        public {2} {1} <1> get; set; <2>
", '"'.ToString(), ColumnName, DataType, FK);
        }

        #endregion

        #region view management

        if (!(ColumnName.StartsWith("mig_")))
        {
            if (UI == "FileUpload" || UI == "FileUploadDB")
            {
                FileUpload += string.Format(view_upload_js_template, table_name, ColumnName);
            }

            if (UI == "ImageUpload")
            {
                string tempUploadImage = @"
$('#{0}_{1}_file').change(function () <1>
    UploadImagePhoto($('#{0}_{1}_file'), '{0}_{1}');
<2>);
";

                FileUpload += string.Format(tempUploadImage, table_name, ColumnName);
            }

            GetFromForm += (JSHL_GetFromForm + enter)
                .Replace("{0}", table_name)
                .Replace("{1}", ColumnName)
                .Replace("{2}", table_name);
            if (UI != "CheckBoxList")
            {
                if (on_grid == "yes")
                {
                    TableHeader += string.Format("            <th><label id='h_{1}_{2}'>{0}</label></th>", Desc, table_name, ColumnName) + enter;
                    if (JS_Column_Data != "") JS_Column_Data += "," + enter;
                    if (FK != "")
                    {
                        JS_Column_Data += string.Format("                <1> {0}data{0}: {0}{1}{0} <2>", '"'.ToString(), ColumnName + "_" + FK + "_" + nname[FK]);
                    }
                    else if (UI == "TextBoxDate" || UI == "FileUpload" || UI == "FileUploadDB" || UI == "ImageUpload")
                    {
                        JS_Column_Data += string.Format("                <1> {0}data{0}: {0}txt_{1}{0} <2>", '"'.ToString(), ColumnName);
                    }
                    else
                    {
                        JS_Column_Data += string.Format("                <1> {0}data{0}: {0}{1}{0} <2>", '"'.ToString(), ColumnName);
                    }

                    ReactTableColumn += string.Format(@"
        <1>
            title: '{0}',
            dataIndex: '{2}',
            sorter: <1>
                compare: (a, b) => <1>
                    a = a.username.toLowerCase();
                    b = b.username.toLowerCase();
                    return a > b ? -1 : b > a ? 1 : 0;
                <2>,
            <2>,
        <2>,", Desc, table_name, ColumnName);
                }
            }

            Controller_SetValue += string.Format("            {1}.{0} = item.{0};" + enter, ColumnName, table_name);
            if (!string.IsNullOrEmpty(FK))
            {
                ClearForm += string.Format(JSHL_ClearForm + enter, table_name, ColumnName, "/api/" + FK, key[FK], nname[FK]);
                InitialForm += string.Format(JSHL_InitialForm + enter, table_name, ColumnName, "/api/" + FK, key[FK], nname[FK]);
                FeedDataToForm += (JSHL_FeedDataToForm + enter)
                .Replace("{0}", table_name)
                .Replace("{1}", ColumnName)
                .Replace("{2}", key[FK])
                .Replace("{3}", nname[FK]);
            }
            else
            {
                ClearForm += string.Format(JSHL_ClearForm + enter, table_name, ColumnName, "", "", "");
                InitialForm += string.Format(JSHL_InitialForm + enter, table_name, ColumnName, "", "", "");
                FeedDataToForm += (JSHL_FeedDataToForm + enter)
                .Replace("{0}", table_name)
                .Replace("{1}", ColumnName)
                .Replace("{2}", "")
                .Replace("{3}", "");
            }
        }

        if (PrimaryKey == "name" || PrimaryKey == "search")
        {
            string lab = "<label id='lab_s_{0}_{1}' for='s_{0}_{1}'>{2}</label>";

            ReactSearchColumn += @"            <Form.Item
                label={0}<1>{0}
                name={0}<2>{0}
            >
                <Input />
            </Form.Item>
".Replace("{0}", '"'.ToString()).Replace("<1>", Desc).Replace("<2>", ColumnName);

            search_ui += @"
                        <div class={0}form-group col-md-3{0}>
                            {yyy}
                            {xxx}
                        </div>
".Replace("{0}", '"'.ToString()).Replace("{xxx}", string.Format(HTML_tag.Replace("{p}", $"title='{Desc}' placeholder='{Desc}'"), "s_" + table_name + "_" + ColumnName, "s_" + table_name, "", Desc))
.Replace("{yyy}", string.Format(lab, table_name, ColumnName, Desc));

            if (UI == "TextBoxDate")
            {
                GetSearchParameter += (JSHL_GetFromForm.Replace("getDate(", "formatDateForGetParameter(getDate(").Replace(";", ");") + enter)
            .Replace("{0}", "s_" + table_name)
            .Replace("{1}", ColumnName)
            .Replace("{2}", table_name + "Search");
            }
            else
            {
                GetSearchParameter += (JSHL_GetFromForm + enter)
            .Replace("{0}", "s_" + table_name)
            .Replace("{1}", ColumnName)
            .Replace("{2}", table_name + "Search");
            }

            if (!string.IsNullOrEmpty(FK))
            {
                FeedDataToSearchForm += (JSHL_FeedDataToForm + enter)
            .Replace("{0}", "s_" + table_name)
            .Replace("{1}", ColumnName)
            .Replace("{2}", key[FK])
            .Replace("{3}", nname[FK]);
            }
            else
            {
                FeedDataToSearchForm += (JSHL_FeedDataToForm + enter)
            .Replace("{0}", "s_" + table_name)
            .Replace("{1}", ColumnName)
            .Replace("{2}", "")
            .Replace("{3}", "");
            }
        }

        #endregion

        #region Service

        if (!(ColumnName.StartsWith("mig_") || ColumnName.StartsWith("rep_")))
        {

            if (UI == "FileUpload" || UI == "ImageUpload")
            {
                ServiceUploadAdd += string.Format(upload_add, ColumnName);
                ServiceUploadUpdate += string.Format(upload_update, ColumnName);
                ServiceUploadUpdateMultiple += string.Format(upload_update.Replace("model.", "i."), ColumnName);
            }
            else if (UI == "FileUploadDB")
            {
                ServiceUploadAdd += string.Format(upload_db_add, ColumnName);
                ServiceUploadUpdate += string.Format(upload_db_update, ColumnName);
                ServiceUploadUpdateMultiple += string.Format(upload_db_update.Replace("model.", "i."), ColumnName);
            }
            else if (UI == "CheckBoxList")
            {
                // ไม่ต้องสร้างตัวแปร
            }
            else if (PrimaryKey != "yes")
            {
                string temp101 = @"                existingEntity.{0} = model.{0};
";
                ServiceUploadUpdate += string.Format(temp101, ColumnName);
                ServiceUploadUpdateMultiple += string.Format(temp101.Replace("model.", "i."), ColumnName);
                string temp102 = @"            xmlContent = xmlContent.Replace({0}<1>txt_{1}<2>{0}, HttpUtility.HtmlEncode(entity.{1} ?? {0}{0}));
";
                xml_print += string.Format(temp102, '"'.ToString(), ColumnName).Replace("<1>", "{").Replace("<2>", "}");
                string temp103 = @"                {1} = c.{1},
";
                xml_map += string.Format(temp103, '"'.ToString(), ColumnName).Replace("<1>", "{").Replace("<2>", "}");
            }

            //===========================================================
            if (FK != "")
            //===========================================================
            {
                FK_IService += string.Format(@"        //void InsertListBy{1}({2} {1}, List<{3}> items);
        List<{0}ViewModel> GetListBy{1}({4} {1});", table_name, ColumnName, key_data_type[table_name], key_data_type[FK], DataType) + enter;
                FK_Service += string.Format(FK_Service_Template, table_name, ColumnName) + enter;
            }

            if (UI == "CheckBoxList" && !string.IsNullOrEmpty(MultiSelectionTable))
            {
                SelectionItemList += $"            i.item_{ColumnName} = await _dataContext.{MultiSelectionTable}s.Select(x => _mapper.Map<{MultiSelectionTable}ViewModel>(x)).ToListAsync();" + enter;
                SelectionItemList += $"            //i.selected_{ColumnName} = await _dataContext.???s.Where(x => x.??? == {ColumnName}).Select(x => _mapper.Map<{FK}ViewModel>(x)).ToListAsync();" + enter;
            }
            if (UI == "DropDownList" && !string.IsNullOrEmpty(FK))
            {
                SelectionItemList += $"            i.item_{ColumnName} = await _dataContext.{FK}s.Select(x => _mapper.Map<{FK}ViewModel>(x)).ToListAsync();" + enter;
            }
        }

        if (PrimaryKey == "name" || PrimaryKey == "search")
        {
            if (whereSearch != "")
            {
                whereSearch += "                && ";
            }
            else
            {
                whereSearch += "                1 == 1 " + enter;
                whereSearch += "                && ";
            }

            if (DataType.StartsWith("string"))
            {
                whereSearch += $"(string.IsNullOrEmpty(model.{ColumnName}) || m_{table_name.ToLower()}.{ColumnName}.Contains(model.{ColumnName}))" + enter;
            }
            else if (DataType.EndsWith("?"))
            {
                whereSearch += $"(!model.{ColumnName}.HasValue || m_{table_name.ToLower()}.{ColumnName} == model.{ColumnName})" + enter;
            }
            else
            {
                whereSearch += $"(m_{table_name.ToLower()}.{ColumnName} == model.{ColumnName})" + enter;
            }

        }

        #endregion

        #region Inline JS

        ////===========================================================
        //if (PrimaryKey != "yes")
        ////===========================================================
        //{
        //$("#{0}_{1}_" + i).prop('checked', false);

        if (!(ColumnName.StartsWith("mig_") || ColumnName.StartsWith("rep_")))
        {

            if (!string.IsNullOrEmpty(FK))
            {
                ClearForm_Inline += string.Format(JSHL_ClearForm_Inline, table_name, ColumnName, "/api/" + FK, key[FK], nname[FK]) + enter;
                FeedDataToForm_Inline += string.Format(JSHL_FeedDataToForm_Inline, table_name, ColumnName, "/api/" + FK, key[FK], nname[FK]) + enter;
            }
            else
            {
                ClearForm_Inline += string.Format(JSHL_ClearForm_Inline + enter, table_name, ColumnName, "", "", "");
                FeedDataToForm_Inline += string.Format(JSHL_FeedDataToForm_Inline, table_name, ColumnName, "", "", "") + enter;
            }

            //{2}Object.{1} = obj.find("#{0}_{1}_" + i).val();
            GetFromForm_Inline += string.Format(JSHL_GetFromForm_Inline, table_name, ColumnName, table_name) + enter;

            // tag += '<td><input class={0}form-control{0} type={0}date{0} id={0}{1}_{2}_' + (i + 1)+'{0} /></td>';
            if (UI == "HiddenField")
            {
                HiddenField_UI += string.Format(HTML_tag_Inline, '"'.ToString(), table_name, ColumnName);
            }
            else
            {
                FormUI_Inline += string.Format(HTML_tag_Inline, '"'.ToString(), table_name, ColumnName) + enter;
                Header_Inline += string.Format("<th><label id='h_{1}_{2}'>{0}</label></th>", Desc, table_name, ColumnName) + enter;
            }
        }

        //}

        //===========================================================
        if (FK != "")
        //===========================================================
        {
            Get_Inline += string.Format(@"		//AjaxGetRequest(apisite + '/api/{0}/GetListBy{1}/' + a, successFunc, AlertDanger);", table_name, ColumnName) + enter;
            Post_Inline += string.Format(@"    //AjaxPostRequest(apisite + '/api/{0}/InsertListBy{1}/' + id, {0}, successFunc, AlertDanger);", table_name, ColumnName) + enter;
        }

        #endregion
    }

    string main_layout = @"
    <div class={0}form-group col-md-{1}{0}>
	    <label id={0}lab_{4}_{5}{0} for={0}{4}_{5}{0}>{2}</label>
	    {3}
    </div>
";
    FormItem = getLayout(table_name, layout_maxrow[table_name], layout_maxcol[table_name],
        layout_ui, layout_desc, layout_length, layout_column_name, main_layout);

    string react_layout = @"
{3}
";
    FormItemReact = getLayoutReact(table_name, layout_maxrow[table_name], layout_maxcol[table_name],
        layout_ui_react, layout_desc, layout_length, layout_column_name, react_layout);

    #region StartUp

    AutoMapper += string.Format(@"                builder.Services.AddAutoMapper(typeof({0}MappingProfile));" + enter + enter, table_name, real_table_name);
    if (real_table_name == table_name)
    {
        DIService += string.Format(@"            builder.Services.AddScoped<IBaseRepository<{0}Entity, {1}>, BaseRepository<{0}Entity, {1}>>();
            builder.Services.AddScoped<I{0}Service, {0}Service>();" + enter + enter, table_name, key_data_type[table_name]);
    }
    else
    {
        DIService += string.Format(@"            builder.Services.AddScoped<I{0}Service, {0}Service>();" + enter + enter, table_name);
    }

    if (table_name == real_table_name)
    {
        DataContext += string.Format(@"        public DbSet<{0}Entity> {1}s <1> get; set; <2>" + enter, table_name, table_name);
    }
    else
    {
        DataContext += string.Format(@"        //public DbSet<{0}Entity> {1}s <1> get; set; <2>" + enter, table_name, table_name);
    }

    #endregion

    #region write file

    Utils.writeFile(Path.Combine(target_path, "Target", "ViewControllers", table_name + "ViewControllers.cs"), string.Format(view_controller_template,
                            '"'.ToString(), table_name, textbox1
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "ApiControllers", table_name + "Controllers.cs"), string.Format(controller_template,
                            '"'.ToString(), table_name, textbox1, nname[table_name], "", "", key_data_type[table_name], key[table_name], //7
                            nname_data_type[table_name] //8
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    if (real_table_name == table_name)
    {
        Utils.writeFile(Path.Combine(target_path, "Target", "Models", table_name, table_name + "Entity.cs"), string.Format(Entity_template,
                textbox1, table_name, entity_model, key_data_type[table_name], key[table_name], getNavigationEf(table_name, fk)
                ).Replace("<1>", "{").Replace("<2>", "}").Replace("<3>", '"'.ToString()));
    }

    Utils.writeFile(Path.Combine(target_path, "Target", "Models", table_name, table_name + "WithSelectionViewModel.cs"), string.Format(B64Entity_Template,
                textbox1, table_name, SelectionView
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Models", table_name, table_name + "InputModel.cs"), string.Format(InputModel_template,
                textbox1, table_name, input_model
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Models", table_name, table_name + "SearchModel.cs"), string.Format(SearchModel_template,
                textbox1, table_name, search_model
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Models", table_name, table_name + "ReportRequestModel.cs"), string.Format(ReportRequestModel_template,
                textbox1, table_name
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Models", table_name, table_name + "ViewModel.cs"), string.Format(ViewModel_template,
                textbox1, table_name, view_model, ViewModelVar, key_data_type[table_name], key[table_name]
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "InfraRepositories", table_name, table_name + "MappingProfile.cs"), string.Format(MappingProfile_template,
                textbox1, table_name
                ).Replace("<1>", "{").Replace("<2>", "}"));

    //MappingProfile_template

    string new_key_method = "Guid.NewGuid()";
    if (key_data_type[table_name] != "Guid") new_key_method = "GetNewPrimaryKey()";

    Utils.writeFile(Path.Combine(target_path, "Target", "InfraRepositories", table_name, table_name + "Service.cs"), string.Format(
                    fix_key_method(service_template, key_data_type[table_name]),
                            '"'.ToString(), textbox1, table_name, nname[table_name], //3
                            "", ServiceUploadAdd, ServiceUploadUpdate, FK_Service, xml_print, xml_map, //9
                            JoinData, JoinResultMain, JoinResultFK, table_name.ToLower(), ServiceUploadUpdateMultiple, //14
                            SelectionItemList, key_data_type[table_name], key[table_name], // 17
                            whereSearch, real_table_name, nname_data_type[table_name], new_key_method, // 20
                            LookupForLog
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "CoreInterface", "I" + table_name + "Service.cs"), string.Format(iservice_template,
                '"'.ToString(), textbox1, table_name, nname[table_name], "", FK_IService, //5
                key_data_type[table_name], key[table_name], real_table_name, nname_data_type[table_name] //9
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Javascripts", table_name, table_name + ".js"), string.Format(view_js_template,
                           '"'.ToString(), table_name, FeedDataToForm, GetFromForm, ClearForm, InitialForm, JS_Column_Data, nname[table_name], FileUpload, // 8
                           MultiSelectionTXT, AfterInsertUpdate, AfterDelete, GetSearchParameter, FeedDataToSearchForm, key[table_name]
                           ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Javascripts", table_name, table_name + "_report.js"), string.Format(view_js_report_template,
                '"'.ToString(), table_name, GetSearchParameter, FeedDataToSearchForm
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Views", table_name + "View", table_name + "_report.cshtml"), string.Format(view_report_template,
                '"'.ToString(), table_name, search_ui,
                object_name.ContainsKey(table_name) ? object_name[table_name] : table_name
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Javascripts", table_name, table_name + "_d.js"), string.Format(view_d_js_template,
                '"'.ToString(), table_name, FeedDataToForm, GetFromForm, ClearForm, InitialForm, JS_Column_Data,  //6
                nname[table_name], FileUpload, MultiSelectionTXT, AfterInsertUpdate, AfterDelete //11
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Views", table_name + "View", table_name + ".cshtml"), string.Format(view_grid_template,
                '"'.ToString(), table_name, FormItem, TableHeader, key[table_name], search_ui,
                object_name.ContainsKey(table_name) ? object_name[table_name] : table_name

                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Views", table_name + "View", table_name + "_d.cshtml"), string.Format(view_d_template,
                '"'.ToString(), table_name, FormItem,
                object_name.ContainsKey(table_name) ? object_name[table_name] : table_name
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Javascripts", table_name, table_name + "_inline.js"), string.Format(inline_js,
                '"'.ToString(), table_name, ClearForm_Inline, FeedDataToForm_Inline,
                GetFromForm_Inline, FormUI_Inline, FormUI_Inline, Get_Inline, Post_Inline, HiddenField_UI
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "Views", table_name + "View", table_name + "_inline.cshtml"), string.Format(inline_html,
                '"'.ToString(), table_name, Header_Inline,
                object_name.ContainsKey(table_name) ? object_name[table_name] : table_name
                ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "React", table_name, "index.js"), string.Format(react_index_template,
                            '"'.ToString(), table_name, ti.ToTitleCase(table_name)
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "React", table_name, ti.ToTitleCase(table_name) + "Form.js"), string.Format(react_form_template,
                            '"'.ToString(), table_name, ti.ToTitleCase(table_name), FormItemReact
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "React", table_name, ti.ToTitleCase(table_name) + "Datatable.js"), string.Format(react_datatable_template,
                            '"'.ToString(), table_name, ti.ToTitleCase(table_name), ReactTableColumn
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    Utils.writeFile(Path.Combine(target_path, "Target", "React", table_name, ti.ToTitleCase(table_name) + "Searchform.js"), string.Format(react_searchform_template,
                            '"'.ToString(), table_name, ti.ToTitleCase(table_name), ReactSearchColumn
                            ).Replace("<1>", "{").Replace("<2>", "}"));

    #endregion
}

Utils.writeFile(Path.Combine(target_path, "Target", "StartUp.cs"),
    (DIService + enter + AutoMapper + enter + DataContext).Replace("<1>", "{").Replace("<2>", "}"));

#endregion



