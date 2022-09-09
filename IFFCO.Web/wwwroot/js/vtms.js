

function BindDropDownWithSelectAsNull(data, id) {
    var districtHtml = "<option value=''>---Select---</option>";
    $.each(data, function (index, District) { districtHtml += "<option value=" + District.value + ">" + District.text + "</option>"; });
    $(id).html(districtHtml);
}

