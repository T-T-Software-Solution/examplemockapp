var sighting_API = "/api/v1/sighting/";

//================= Search Customizaiton =========================================

function sighting_GetSearchParameter(fileType) {
    var sightingSearchObject = new Object();
sightingSearchObject.alien_id = $("#s_sighting_alien_id").val();
sightingSearchObject.found_date = formatDateForGetParameter(getDate($("#s_sighting_found_date").val()));
sightingSearchObject.location = $("#s_sighting_location").val();
sightingSearchObject.witness = $("#s_sighting_witness").val();


    sightingSearchObject.fileType = fileType;

    console.log(sightingSearchObject);

    return sightingSearchObject;
}

function sighting_FeedDataToSearchForm(data) {
DropDownClearFormAndFeedWithData($("#s_sighting_alien_id"), data, "id", "species", "item_alien_id", data.alien_id);
$("#s_sighting_found_date").val(formatDate(data.found_date));
$("#s_sighting_location").val(data.location);
$("#s_sighting_witness").val(data.witness);

}

//================= Form Data Customizaiton =========================================

function sighting_InitialForm(s) {
    var successFunc = function (result) {
        sighting_FeedDataToSearchForm(result);
        endLoad();
    };
    startLoad();
    AjaxGetRequest(apisite + sighting_API + "GetBlankItem", successFunc, AlertDanger);
}

//================= Data Table =========================================

var s_sighting_customValidation = function (group) {
    return "";
};


function sighting_DoSearch(fileType) {
    if (!ValidateForm('s_sighting', s_sighting_customValidation)) {
        return;
    }

    var p = $.param(sighting_GetSearchParameter(fileType));

    var report_url = apisite + "/api/v1/sighting/sighting_report?" + p;

    if (fileType === "pdf") {        
        $("#report_result").attr("src", report_url);
        $("#report_result").show();
        //window.open(report_url);
    } else {
        $("#report_result").hide();
        window.open(report_url);
    }
}

