$(document).ready(function () {
    $(document).on('focus', ':input', function () {
        $(this).attr('autocomplete', 'nakorn2');
    });

    $('*[data-provide="datepicker"]').wrap('<div class="input-group"></div>');

    $('*[data-provide="datepicker"]').calendarsPicker(
        {
            calendar: $.calendars.instance('thai', 'th'),
            yearRange: 'c-200:c+10',
            showOnFocus: false,
            showTrigger: '<div class="input-group-append"><span class="trigger input-group-text"><i class="fa fa-calendar"></i></span></div>'
        }
    );
    //$('*[data-provide="datepicker"]').attr("placeholder", "วว/ดด/ปปปป");
    $('*[data-provide="datepicker"]').mask("99/99/9999", { placeholder: 'วว/ดด/ปปปป' });

});

//=======================================================================================================================

function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}


function AjaxGetFileRequest(path, successFunc, failFunc) {

    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: GetUnix(path),
            type: 'GET',
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: GetUnix(path),
            type: 'GET',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            success: successFunc,
            error: failFunc
        });
    }
}

function AjaxGetRequest(path, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: GetUnix(path),
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: GetUnix(path),
            type: 'GET',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: successFunc,
            error: failFunc
        });
    }
}
function AjaxGetRequestWithData(path, jData, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: GetUnix(path),
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: GetUnix(path),
            type: 'GET',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    }
}
function AjaxPostRequest(path, jData, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: path,
            type: 'POST',
            //headers: { 'Authorization': "Bearer " + Cookies.get('ap_access_token') },
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: path,
            type: 'POST',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    }

}
function AjaxPutRequest(path, jData, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: path,
            type: 'PUT',
            //headers: { 'Authorization': "Bearer " + Cookies.get('ap_access_token') },
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: path,
            type: 'PUT',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    }
}

function AjaxPutRequestFile(path, jData, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: path,
            type: 'PUT',
            //headers: { 'Authorization': "Bearer " + Cookies.get('ap_access_token') },
            contentType: 'application/json',
            dataType: 'binary',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: path,
            type: 'PUT',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            contentType: 'application/json',
            dataType: 'binary',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    }
}

function AjaxDeleteRequest(path, jData, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: path,
            type: 'DELETE',
            //headers: { 'Authorization': "Bearer " + Cookies.get('ap_access_token') },
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: path,
            type: 'DELETE',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(jData),
            success: successFunc,
            error: failFunc
        });
    }
}
function AjaxUploadRequest(path, formData, successFunc, failFunc) {
    if (getCookie("ap_access_token") === null || getCookie("ap_access_token") === "") {
        $.ajax({
            url: path,
            type: 'POST',
            processData: false, // important
            contentType: false, // important
            dataType: 'json',
            data: formData,
            success: successFunc,
            error: failFunc
        });
    } else {
        $.ajax({
            url: path,
            type: 'POST',
            headers: { 'Authorization': "Bearer " + getCookie('ap_access_token') },
            processData: false, // important
            contentType: false, // important
            dataType: 'json',
            data: formData,
            success: successFunc,
            error: failFunc
        });
    }
}

function DropDownClearFormAndFeedWithData(d, result, x, y, z, i) {
    $(d).html('');
    $(d).append($("<option></option>")
        .attr("value", "")
        .text("ไม่ระบุ"));
    if (z !== "") {
        $.each(result[z], function (key, value) {
            $(d).append($("<option></option>")
                .attr("value", value[x])
                .text(value[y]));
        });
    } else {
        $.each(result, function (key, value) {
            $(d).append($("<option></option>")
                .attr("value", value[x])
                .text(value[y]));
        });
    }
    $(d).val(i);
}

function DropDownClearFormAndFeedWithDataAndText(d, result, x, y, z, i, text) {
    $(d).html('');
    $(d).append($("<option></option>")
        .attr("value", "")
        .text(text));
    if (z !== "") {
        $.each(result[z], function (key, value) {
            $(d).append($("<option></option>")
                .attr("value", value[x])
                .text(value[y]));
        });
    } else {
        $.each(result, function (key, value) {
            $(d).append($("<option></option>")
                .attr("value", value[x])
                .text(value[y]));
        });
    }
    $(d).val(i);
}

function DropDownInitialFormWithData(d, result, x, y, z) {
    $(d).html('');
    $(d).append($("<option></option>")
        .attr("value", "")
        .text("ไม่ระบุ"));
    if (z !== "") {
        $.each(result[z], function (key, value) {
            $(d).append($("<option></option>")
                .attr("value", value[x])
                .text(value[y]));
        });
    } else {
        $.each(result, function (key, value) {
            $(d).append($("<option></option>")
                .attr("value", value[x])
                .text(value[y]));
        });
    }
}

////////////////////////// Alert Functions //////////////////////////////////
function AlertSuccess(msg) {
    $(".alert_confirm").hide();
    $(".alert_basic").show();
    myalert01(msg);
}

function AlertDanger(xhr, status, error) {
    //console.log(xhr);
    var errorMessage = xhr.responseText;
    endLoad();

    if (xhr.status === 401) {
        myalert01('ไม่อนุญาตให้เข้าใช้งาน');
        window.location.href = "/home/logout";
    } else {
        $(".alert_confirm").hide();
        $(".alert_basic").show();
        myalert01('มีข้อผิดพลาดเกิดขึ้น ดังนี้ : ' + errorMessage);
    }
}

function AlertDanger2(xhr, status, error) {
    console.log(xhr);
    var errorMessage = xhr.responseText;
    endLoad();
    alert(errorMessage);
}

function AlertDanger02(msg) {
    $(".alert_confirm").hide();
    $(".alert_basic").show();
    myalert01(msg);
}

function AlertConfirm(msg, yesCallback) {
    $(".alert_confirm").show();
    $(".alert_basic").hide();
    myalert01(msg);

    $('#myconfirm_yes').click(function () {
        $("#alertModel").modal("hide");
        myalert01("");
        yesCallback();
    });
    $('#myconfirm_no').click(function () {
        $("#alertModel").modal("hide");
        myalert01("");
        endLoad();
    });
}

function GetUnix(path) {
    if (path !== "") {
        if (path.indexOf("?") > -1)
            path += "&";
        else
            path += "?";
        path += "_=" + moment().unix();
    }
    return path;
}

function startLoad() {
    $.LoadingOverlay("show");
}
function endLoad() {
    $.LoadingOverlay("hide");
}

////////////////////////// Service Frame //////////////////////////////////

function inIframe() {
    try {
        return window.self !== window.top;
    } catch (e) {
        return true;
    }
}

function window_open_from_root(frameSrc) {
    if (inIframe()) {
        window.parent.window_open_from_root(frameSrc);
    } else {
        window.location = frameSrc;
    }
}

function window_open(frameSrc) {
    $(this).parents('body').addClass('overflow-fix');
    $('#myframe').addClass('iframe-visible');
    $('#myframe').attr('src', frameSrc);
    //return false;
}

function window_close() {
    $(window.frameElement).removeClass('iframe-visible');
    $(window.frameElement).parents('body').removeClass('overflow-fix');
    $(window.frameElement).attr('src', '');
    endLoad();
    //return false;
}

function refresh_frame() {
    //&#xf021;
    console.log("refresh_btn");
    var ref = $('#refresh_icon');
    //if (window.parent == window.top) {
    ref.addClass("fa fa-refresh fa-spin");
    ref.html("");
    window.location.reload(true);
    //}
}



////////////////////////// Control Service //////////////////////////////////

function CheckBoxGetFromForm(d) {
    if ($(d).is(':checked')) {
        return true;
    } else {
        return false;
    }
}

function CheckBoxFeedDataToForm(d, v) {
    if (v === true) {
        $(d).prop('checked', true);
    } else {
        $(d).prop('checked', false);
    }
}

function formatDate(date) {
    let value = '';
    if (date === null) {
        value = '';
    } else {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + (d.getDate()),
            year = '' + (d.getFullYear() + 543);

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        value = [day, month, year].join('/');
    }

    return value;
}

function getDate(dateText) {
    //console.log(dateText);
    if (dateText !== "" && dateText !== undefined) {
        var parts = dateText.split('/');
        var day = parseInt(parts[0]);  // + 1;
        var month = parseInt(parts[1]) - 1;
        var year = parseInt(parts[2]) - 543;
        //console.log(parts);
        //console.log(year);
        //console.log(month);
        //console.log(day);

        //var d = new Date(year, month, day);
        var d = new Date(Date.UTC(year, month, day));
        return d;
    } else {
        return null;
    }
}

function formatDateForGetParameter(date) {
    let value = '';
    if (date === null) {
        value = '';
    } else {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            //day = '' + (d.getDate() - 1),
            day = '' + (d.getDate()),
            year = '' + (d.getFullYear());

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        value = [year, month, day].join('-');
    }

    return value;
}

// ================= File Upload ========================

function UploadImage(file, code) {
    var file_data = file.prop("files")[0];
    var myFormData = new FormData();
    myFormData.append('files', file_data);
    var successFunc = function (response) {
        $("#" + code + "_linkurl").show();
        $("#" + code + "_linkurl").text("ดูหรือดาวโหลดไฟล์ของคุณ");
        $("#" + code + "_linkurl").attr('href', apisite + "/" + response.filesUploaded[0]);
        $("#" + code + "_linkurl").attr('target', '_blank');
        $("#" + code + "_remove").show();
        $("#" + code + "_hidURL").val(response.filesUploaded[0]);
        endLoad();
    };
    startLoad();
    AjaxUploadRequest(apisite + "/api/Attachment/UploadMultipleFiles", myFormData, successFunc, AlertDanger);
}

function feedFileToControl(thefile, thefileDisplay, code, mode) {
    if (!thefile) {
        $("#" + code + "_linkurl").hide();
        $("#" + code + "_linkurl").text("ยังไม่มีการแนบไฟล์");
        $("#" + code + "_linkurl").attr('href', '#');
        $("#" + code + "_linkurl").attr('target', '');
        $("#" + code + "_remove").hide();
        $("#" + code + "_hidURL").val("");
        $("#" + code + "_file").val("");
    } else {
        $("#" + code + "_linkurl").show();
        $("#" + code + "_linkurl").text("ดูหรือดาวโหลดไฟล์ของคุณ");
        if (mode === "file") {
            $("#" + code + "_linkurl").attr('href', appsite + "/" + thefileDisplay);
        } else if (mode === "db") {
            $("#" + code + "_linkurl").attr('href', thefileDisplay);
        }
        $("#" + code + "_linkurl").attr('target', '_blank');
        $("#" + code + "_remove").show();
        $("#" + code + "_hidURL").val(thefile);
        $("#" + code + "_file").val("");
    }
}

function removeFile(code) {
    if (confirm('คุณต้องการลบไฟล์ ใช่หรือไม่?')) {
        $("#" + code + "_linkurl").hide();
        $("#" + code + "_linkurl").text("ยังไม่มีการแนบไฟล์");
        $("#" + code + "_linkurl").attr('href', '#');
        $("#" + code + "_linkurl").attr('target', '');
        $("#" + code + "_remove").hide();
        $("#" + code + "_hidURL").val("");
        $("#" + code + "_file").val("");
    }
}

// ================= Image Upload ========================

function UploadImagePhoto(file, code) {
    var file_data = file.prop("files")[0];
    var myFormData = new FormData();
    myFormData.append('files', file_data);
    var successFunc = function (response) {
        $("#" + code + "_img").show();
        $("#" + code + "_img").attr("src", apisite + "/" + response.filesUploaded[0]);
        $("#" + code + "_remove").show();
        $("#" + code + "_hidURL").val(response.filesUploaded[0]);
        endLoad();
    };
    startLoad();
    AjaxUploadRequest(apisite + "/api/Attachment/UploadMultipleFiles", myFormData, successFunc, AlertDanger);
}

function feedFileToControlImagePhoto(thefile, thefileDisplay, code, mode) {
    if (!thefile) {
        $("#" + code + "_img").hide();
        $("#" + code + "_remove").hide();
        $("#" + code + "_hidURL").val("");
        $("#" + code + "_file").val("");
    } else {
        $("#" + code + "_img").show();
        if (thefileDisplay.startsWith("http") === true) {
            $("#" + code + "_img").attr("src", thefileDisplay);
        } else {
            $("#" + code + "_img").attr("src", appsite + "/" + thefileDisplay);
        }
        $("#" + code + "_remove").show();
        $("#" + code + "_hidURL").val(thefile);
        $("#" + code + "_file").val("");
    }
}

function removeImagePhoto(code) {
    if (confirm('คุณต้องการลบรูป ใช่หรือไม่?')) {
        $("#" + code + "_img").hide();
        $("#" + code + "_img").attr("src", "");
        $("#" + code + "_remove").hide();
        $("#" + code + "_hidURL").val("");
        $("#" + code + "_file").val("");
    }
}

// ================= Other Upload ========================

function setupUploadEvent() {

}

function hasExtension(inputID, exts) {
    var fileName = document.getElementById(inputID).value;
    return (new RegExp('(' + exts.join('|').replace(/\./g, '\\.') + ')$')).test(fileName);
}

// ================= Validation ========================

function TestDate(d, m, y) {

    var dd = parseInt(d);
    var mm = parseInt(m);
    var yy = parseInt(y);

    if (yy > 2100) {
        yy -= 543;
    }

    if (mm > 12) return false;
    if (dd > 31) return false;
    if (yy <= 0) return false;

    // Create list of days of a month [assume there is no leap year by default]  
    var ListofDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    //ตรวจสอบวันที่ว่าควรเป็น 30 หรือ 31 วัน
    if (mm == 1 || mm > 2) {
        if (dd > ListofDays[mm - 1]) {
            //Invalid date format!'
            return false;
        }
    }

    //ตรวจสอบว่าเดือนกุมภาพันธ์ ว่าครวมี 28 หรือ 29 วัน
    if (mm == 2) {
        var lyear = false;
        if ((!(yy % 4) && yy % 100) || !(yy % 400)) {
            lyear = true;
        }
        if ((lyear == false) && (dd >= 29)) {
            //'Invalid date format!'  
            return false;
        }
        if ((lyear == true) && (dd > 29)) {
            //'Invalid date format!
            return false;
        }
    }

    return true;

}

function SetupValidationRemark(group) {
    $("[iGroup='" + group + "'][iRequire='true']").each(function (index) {
        var k = "lab_" + $(this).attr("id");
        $("<font color='red'> *</font>").insertAfter("#" + k);
    });
    $("[iGroup='" + group + "'][iShowmark='true']").each(function (index) {
        var k = "lab_" + $(this).attr("id");
        $("<font color='red'> *</font>").insertAfter("#" + k);
    });
    // in case of file upload
    $("input[iGroup='" + group + "'][iRequire='true'][type='hidden']").each(function (index) {
        var k = "lab_" + $(this).attr("id");
        $("<font color='red'> *</font>").insertAfter("#" + k.replace('_hidURL', ''));
    });
}

function ValidateForm(group, customValidate) {
    //console.log("ValidateForm "+group);
    var result = "";
    var obj = null;
    $("input[data-provide='datepicker'][iGroup='" + group + "']").each(function (index) {
        var inputText = $(this).val();
        var dateArray = inputText.split('/');
        if (!TestDate(dateArray[0], dateArray[1], dateArray[2])) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "ให้ถูกต้อง อาทิเช่น วันที่ 1 มกราคม 2563 ให้ระบุ 01/01/2563\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        }
    });
    $("input[iRequire='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null || !$(this).val().trim().length) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        } else {
            $(this).css('border-color', '');
        }
    });
    $("textarea[iRequire='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        } else {
            $(this).css('border-color', '');
        }
    });
    $("select[iRequire='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        } else {
            $(this).css('border-color', '');
        }
    });

    result += customValidate(group);
    if (result !== "") {
        //if(obj !== null) obj.focus();
        AlertDanger02(result);
        return false;
    } else {
        return true;
    }
}

function ValidateForm2(group, customValidate) {
    //console.log("ValidateForm "+group);
    var result = "";
    var obj = null;
    $("input[data-provide='datepicker'][iGroup='" + group + "']").each(function (index) {
        var inputText = $(this).val();
        var dateArray = inputText.split('/');
        if (!TestDate(dateArray[0], dateArray[1], dateArray[2])) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "ให้ถูกต้อง อาทิเช่น วันที่ 1 มกราคม 2563 ให้ระบุ 01/01/2563\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        }
    });
    $("input[iRequire='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        } else {
            $(this).css('border-color', '');
        }
    });
    $("textarea[iRequire='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        } else {
            $(this).css('border-color', '');
        }
    });
    $("select[iRequire='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + "\n";
            $(this).css('border-color', 'red');
            if (obj === null) obj = $(this);
        } else {
            $(this).css('border-color', '');
        }
    });

    result += customValidate(group);
    if (result !== "") {
        //if(obj !== null) obj.focus();
        alert(result);
        return false;
    } else {
        return true;
    }
}

function ValidateMinusFlag(group) {
    var result = "";
    $("input[iRequireMinusFlag='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + " เป็น - ในกรณีที่ไม่มีข้อมูล\n";
            $(this).css('border-color', 'red');
            $(this).focus();
        } else {
            $(this).css('border-color', '');
        }
    });
    $("textarea[iRequireMinusFlag='true'][iGroup='" + group + "']").each(function (index) {
        if ($(this).val() === "" || $(this).val() === null) {
            result += "- กรุณาระบุ " + $(this).attr("iLabel") + " เป็น - ในกรณีที่ไม่มีข้อมูล\n";
            $(this).css('border-color', 'red');
            $(this).focus();
        } else {
            $(this).css('border-color', '');
        }
    });

    return result;
}

//============== Set Disabled / Enabled Control Set ===================

function SetDisabled(group, value) {
    var result = "";
    var obj = null;
    $("input[iGroup='" + group + "']").each(function (index) {
        $(this).prop("disabled", value);
    });
    $("textarea[iGroup='" + group + "']").each(function (index) {
        $(this).prop("disabled", value);
    });
    $("select[iGroup='" + group + "']").each(function (index) {
        $(this).prop("disabled", value);
    });
    $("file[iGroup='" + group + "']").each(function (index) {
        $(this).prop("disabled", value);
    });
}

//============== Set Disabled / Enabled Control Set ===================

function setNextFocus(objId) {
    if (event.keyCode === 13) {
        var obj = document.getElementById(objId);
        if (obj) {
            obj.focus();
            obj.select();
        }
    }
}

//====================== ELSE ===================================

function checkIDCard(id) {
    if (id.length !== 13) return false;
    for (i = 0, sum = 0; i < 12; i++)
        sum += parseFloat(id.charAt(i)) * (13 - i); if ((11 - sum % 11) % 10 !== parseFloat(id.charAt(12)))
        return false;
    return true;
}

//====================== MY ALERT ===================================

function myalert01(message) {
    //$("#div_message").html("<p>"+message.replace(/\n/g, "<br />")+"</p>");
    //$("#alertModel").modal("show");
    alert(message);
}

