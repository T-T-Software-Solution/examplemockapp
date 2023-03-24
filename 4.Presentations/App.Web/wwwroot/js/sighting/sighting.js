var sighting_editMode = "CREATE";
var sighting_API = "/api/v1/sighting/";

//================= Search Customizaiton =========================================

function sighting_GetSearchParameter() {
    var sightingSearchObject = new Object();
    sightingSearchObject.alien_id = $("#s_sighting_alien_id").val();
    sightingSearchObject.found_date = formatDateForGetParameter(getDate($("#s_sighting_found_date").val()));
    sightingSearchObject.location = $("#s_sighting_location").val();
    sightingSearchObject.witness = $("#s_sighting_witness").val();

    return sightingSearchObject;
}

function sighting_FeedDataToSearchForm(data) {
    DropDownClearFormAndFeedWithData($("#s_sighting_alien_id"), data, "id", "species", "item_alien_id", data.alien_id);
    $("#s_sighting_found_date").val(formatDate(data.found_date));
    $("#s_sighting_location").val(data.location);
    $("#s_sighting_witness").val(data.witness);

}

//================= Form Data Customizaiton =========================================

function sighting_FeedDataToForm(data) {
    $("#sighting_id").val(data.id);
    DropDownClearFormAndFeedWithData($("#sighting_alien_id"), data, "id", "species", "item_alien_id", data.alien_id);
    $("#sighting_found_date").val(formatDate(data.found_date));
    $("#sighting_location").val(data.location);
    $("#sighting_witness").val(data.witness);

}

function sighting_GetFromForm() {
    var sightingObject = new Object();
    sightingObject.id = $("#sighting_id").val();
    sightingObject.alien_id = $("#sighting_alien_id").val();
    sightingObject.found_date = getDate($("#sighting_found_date").val());
    sightingObject.location = $("#sighting_location").val();
    sightingObject.witness = $("#sighting_witness").val();


    return sightingObject;
}

function sighting_InitialForm(s) {
    var successFunc = function (result) {
        sighting_FeedDataToForm(result);
        sighting_FeedDataToSearchForm(result);
        if (s) {
            // Incase model popup
            $("#sightingModel").modal("show");
        }
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + sighting_API + "GetBlankItem", successFunc, AlertDanger);
}

//================= Form Mode Setup and Flow =========================================

function sighting_GoCreate() {
    // Incase model popup
    sighting_SetCreateForm(true);

    // Incase open new page
    //window_open(appsite + "/sightingView/sighting_d");
}

function sighting_GoEdit(a) {
    // Incase model popup
    sighting_SetEditForm(a);

    // Incase open new page
    //window_open(appsite + "/sightingView/sighting_d?id=" + a);
}

function sighting_SetEditForm(a) {
    var successFunc = function (result) {
        sighting_editMode = "UPDATE";
        sighting_FeedDataToForm(result);
        $("#sightingModel").modal("show");
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + sighting_API + a, successFunc, AlertDanger);
}

function sighting_SetCreateForm(s) {
    sighting_editMode = "CREATE";
    sighting_InitialForm(s);
}

function sighting_RefreshTable() {
    // Incase model popup
    sighting_DoSearch();

    // Incase open new page
    //window.parent.sighting_DoSearch();
}

//================= Update and Delete =========================================

var sighting_customValidation = function (group) {
    return "";
};

function sighting_PutUpdate() {
    if (!ValidateForm('sighting', sighting_customValidation)) {
        return;
    }

    var data = sighting_GetFromForm();

    //Update Mode
    if (sighting_editMode === "UPDATE") {
        var successFunc1 = function (result) {
            $("#sightingModel").modal("hide");
            AlertSuccess(result.code + " " + result.message);
            sighting_RefreshTable();
            endLoad();
        };
        startLoad();
        AjaxPutRequest(apisite + sighting_API + data.id, data, successFunc1, AlertDanger);
    }
    // Create mode
    else {
        var successFunc2 = function (result) {
            $("#sightingModel").modal("hide");
            AlertSuccess(result.code + " " + result.message);
            sighting_RefreshTable();
            endLoad();
        };
        startLoad();
        AjaxPostRequest(apisite + sighting_API, data, successFunc2, AlertDanger);
    }
}

function sighting_GoDelete(a) {
    if (confirm('คุณต้องการลบข้อมูล ใช่หรือไม่?')) {
        var successFunc = function (result) {
            $("#sightingModel").modal("hide");
            AlertSuccess(result.code + " " + result.message);
            sighting_RefreshTable();
            endLoad();
        };
        startLoad();
        AjaxDeleteRequest(apisite + sighting_API + a, null, successFunc, AlertDanger);
    }
}

//================= Data Table =========================================

var sightingTableV;

var sighting_setupTable = function (result) {
    tmp = '"';
    sightingTableV = $('#sightingTable').DataTable({
        "processing": true,
        "serverSide": false,
        "data": result,
        //"select": {
        //    "style": 'multi'
        //},
        "columns": [
            //{ "data": "" },
            { "data": "id" },
            { "data": "alien_id_alien_species" },
            { "data": "txt_found_date" },
            { "data": "location" },
            { "data": "witness" },
        ],
        "columnDefs": [
            {
                "targets": 0, //1,
                "data": "id",
                "render": function (data, type, row, meta) {
                    return "<button type='button' class='btn btn-warning btn-sm' onclick='javascript:sighting_GoEdit(" + tmp + data + tmp + ")'><i class='fa fa-pencil-alt'></i></button> <button type='button' class='btn btn-danger btn-sm' onclick='javascript:sighting_GoDelete(" + tmp + data + tmp + ")'><i class='fa fa-trash-alt'></i></button> ";
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

function sighting_InitiateDataTable() {
    startLoad();
    var p = $.param(sighting_GetSearchParameter());
    AjaxGetRequest(apisite + "/api/v1/sighting/GetListBySearch?" + p, sighting_setupTable, AlertDanger);
}

function sighting_DoSearch() {
    var p = $.param(sighting_GetSearchParameter());
    var sighting_reload = function (result) {
        sightingTableV.destroy();
        sighting_setupTable(result);
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + "/api/v1/sighting/GetListBySearch?" + p, sighting_reload, AlertDanger);
}

function sighting_GetSelect(f) {
    var sighting_selectitem = [];
    $.each(sightingTableV.rows('.selected').data(), function (key, value) {
        sighting_selectitem.push(value[f]);
    });
    alert(sighting_selectitem);
}

//================= File Upload =========================================



//================= Multi-Selection Function =========================================



