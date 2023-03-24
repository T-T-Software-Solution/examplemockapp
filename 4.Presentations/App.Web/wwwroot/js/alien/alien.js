var alien_editMode = "CREATE";
var alien_API = "/api/v1/alien/";

//================= Search Customizaiton =========================================

function alien_GetSearchParameter() {
    var alienSearchObject = new Object();
    alienSearchObject.name = $("#s_alien_name").val();
    alienSearchObject.species = $("#s_alien_species").val();
    alienSearchObject.origin_planet = $("#s_alien_origin_planet").val();

    return alienSearchObject;
}

function alien_FeedDataToSearchForm(data) {
    $("#s_alien_name").val(data.name);
    $("#s_alien_species").val(data.species);
    $("#s_alien_origin_planet").val(data.origin_planet);

}

//================= Form Data Customizaiton =========================================

function alien_FeedDataToForm(data) {
    $("#alien_id").val(data.id);
    $("#alien_name").val(data.name);
    $("#alien_species").val(data.species);
    $("#alien_origin_planet").val(data.origin_planet);

}

function alien_GetFromForm() {
    var alienObject = new Object();
    alienObject.id = $("#alien_id").val();
    alienObject.name = $("#alien_name").val();
    alienObject.species = $("#alien_species").val();
    alienObject.origin_planet = $("#alien_origin_planet").val();


    return alienObject;
}

function alien_InitialForm(s) {
    var successFunc = function (result) {
        alien_FeedDataToForm(result);
        alien_FeedDataToSearchForm(result);
        if (s) {
            // Incase model popup
            $("#alienModel").modal("show");
        }
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + alien_API + "GetBlankItem", successFunc, AlertDanger);
}

//================= Form Mode Setup and Flow =========================================

function alien_GoCreate() {
    // Incase model popup
    alien_SetCreateForm(true);

    // Incase open new page
    //window_open(appsite + "/alienView/alien_d");
}

function alien_GoEdit(a) {
    // Incase model popup
    alien_SetEditForm(a);

    // Incase open new page
    //window_open(appsite + "/alienView/alien_d?id=" + a);
}

function alien_SetEditForm(a) {
    var successFunc = function (result) {
        alien_editMode = "UPDATE";
        alien_FeedDataToForm(result);
        $("#alienModel").modal("show");
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + alien_API + a, successFunc, AlertDanger);
}

function alien_SetCreateForm(s) {
    alien_editMode = "CREATE";
    alien_InitialForm(s);
}

function alien_RefreshTable() {
    // Incase model popup
    alien_DoSearch();

    // Incase open new page
    //window.parent.alien_DoSearch();
}

//================= Update and Delete =========================================

var alien_customValidation = function (group) {
    return "";
};

function alien_PutUpdate() {
    if (!ValidateForm('alien', alien_customValidation)) {
        return;
    }

    var data = alien_GetFromForm();

    //Update Mode
    if (alien_editMode === "UPDATE") {
        var successFunc1 = function (result) {
            $("#alienModel").modal("hide");
            AlertSuccess(result.code + " " + result.message);
            alien_RefreshTable();
            endLoad();
        };
        startLoad();
        AjaxPutRequest(apisite + alien_API + data.id, data, successFunc1, AlertDanger);
    }
    // Create mode
    else {
        var successFunc2 = function (result) {
            $("#alienModel").modal("hide");
            AlertSuccess(result.code + " " + result.message);
            alien_RefreshTable();
            endLoad();
        };
        startLoad();
        AjaxPostRequest(apisite + alien_API, data, successFunc2, AlertDanger);
    }
}

function alien_GoDelete(a) {
    if (confirm('คุณต้องการลบข้อมูล ใช่หรือไม่?')) {
        var successFunc = function (result) {
            $("#alienModel").modal("hide");
            AlertSuccess(result.code + " " + result.message);
            alien_RefreshTable();
            endLoad();
        };
        startLoad();
        AjaxDeleteRequest(apisite + alien_API + a, null, successFunc, AlertDanger);
    }
}

//================= Data Table =========================================

var alienTableV;

var alien_setupTable = function (result) {
    tmp = '"';
    alienTableV = $('#alienTable').DataTable({
        "processing": true,
        "serverSide": false,
        "data": result,
        //"select": {
        //    "style": 'multi'
        //},
        "columns": [
            //{ "data": "" },
            { "data": "id" },
            { "data": "name" },
            { "data": "species" },
            { "data": "origin_planet" },
        ],
        "columnDefs": [
            {
                "targets": 0, //1,
                "data": "id",
                "render": function (data, type, row, meta) {
                    return "<button type='button' class='btn btn-warning btn-sm' onclick='javascript:alien_GoEdit(" + tmp + data + tmp + ")'><i class='fa fa-pencil-alt'></i></button> <button type='button' class='btn btn-danger btn-sm' onclick='javascript:alien_GoDelete(" + tmp + data + tmp + ")'><i class='fa fa-trash-alt'></i></button> ";
                }
            },
            //{
            //    targets: 0,
            //    data: "",
            //    defaultContent: '',
            //    orderable: false,
            //    className: 'select-checkbox'
            //}
        ],
        "language": {
            "url": appsite + "/DataTables-1.10.16/thai.json"
        },
        "paging": true,
        "searching": false
    });
    endLoad();
};

function alien_InitiateDataTable() {
    startLoad();
    var p = $.param(alien_GetSearchParameter());
    AjaxGetRequest(apisite + "/api/v1/alien/GetListBySearch?" + p, alien_setupTable, AlertDanger);
}

function alien_DoSearch() {
    var p = $.param(alien_GetSearchParameter());
    var alien_reload = function (result) {
        alienTableV.destroy();
        alien_setupTable(result);
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + "/api/v1/alien/GetListBySearch?" + p, alien_reload, AlertDanger);
}

function alien_GetSelect(f) {
    var alien_selectitem = [];
    $.each(alienTableV.rows('.selected').data(), function (key, value) {
        alien_selectitem.push(value[f]);
    });
    alert(alien_selectitem);
}

//================= File Upload =========================================



//================= Multi-Selection Function =========================================



