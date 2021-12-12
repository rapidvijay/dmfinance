$("#btnCreateArea").on("click", function () {
    var url = $(this).data("url");
    var hiddenPropertyId = $("#hiddenPropertyId").val();
    var hiddenAreaIds = $("#areaIds").val();

    url = url + '?propertyId=' + hiddenPropertyId + '&areaIds=' + hiddenAreaIds;

    $.get(url, function (data) {
        $('#createAreaContainer').html(data);

        $('#CreateAreaModal').modal('show');
    });

});

//########################################################################
var areaListVM;
areaListVM = {
    dt: null,

    init: function () {
        dt = $('#areas-data-table').DataTable({
            "serverSide": true,
            "processing": true,
            "ajax": {
                "url": '/PropertyTypes/Get/' + $("#hiddenPropertyId").val(),
                "data": function (data) {
                    // data.FacilitySite = $("#FacilitySite").val();

                }
            },
            "columns": [
                { "title": "Ref No", "data": "Id", "searchable": true },
                { "title": "English Name", "data": "areaEnglishName", "searchable": true },
                { "title": "Arabic Name", "data": "areaArabicName", "searchable": true },

                {
                    "title": "Actions",
                    "data": "Id",
                    "searchable": false,
                    "sortable": false,
                    "render": function (data, type, full, meta) {
                        return '<a  href="#" style="' + $("#displayId").val() + '"  onclick="deletePropertyAreaById(' + data + ')" class="deletePropertyAreaById">Delete</a>';
                    }
                }
            ],
            "lengthMenu": [[10, 25, 50, 100], [10, 25, 50, 100]],
        });
    },

    refresh: function () {
        dt.ajax.reload();
    }
}
//########################################################################





function saveNewArea(areaId) {
    var hiddenPropertyId = $("#hiddenPropertyId").val();

    $.ajax({
        url: '/PropertyTypes/SaveNewArea?areaId=' + areaId + '&propertyId=' + hiddenPropertyId,
        data: {
           // format: 'json'
        },
        error: function () {
            $('#info').html('<p>An error has occurred</p>');
        },
       // dataType: 'jsonp',
        success: function (data) {

            try {
                console.log("data= " + data);

                $('#CreateAreaModal').modal('hide');
                $('#createAreaContainer').html("");
                areaListVM.refresh();
                var hiddenAreaIds = $("#areaIds").val();
                if (hiddenAreaIds != null && hiddenAreaIds != '') {
                    hiddenAreaIds = hiddenAreaIds + ',' + areaId;
                } else {
                    hiddenAreaIds = areaId;
                }
                $("#areaIds").val(hiddenAreaIds);
                
            } catch (error) {
                alert(error.message);
            }

        },
        type: 'POST'
    });

    return false;
}


function deletePropertyAreaById(propertyAreaId) {
    var hiddenPropertyId = $("#hiddenPropertyId").val();

    bootbox.confirm("Are you sure want to delete Area?", function (result) {
        if (result == true) {
            deletePropertyAreaByIdAjax(propertyAreaId , hiddenPropertyId);
        }
    });


    return false;
}


function deletePropertyAreaByIdAjax(propertyAreaId , hiddenPropertyId) {
    $.ajax({
        url: '/PropertyTypes/deletePropertyAreaById?propertyAreaId=' + propertyAreaId + '&propertyId=' + hiddenPropertyId,
        data: {
            // format: 'json'
        },
        error: function () {
            $('#info').html('<p>An error has occurred</p>');
        },
        // dataType: 'jsonp',
        success: function (data) {
            try {
                console.log("data= " + data);
                var areaId = data;
                $('#createAreaContainer').html("");
                areaListVM.refresh();
                var hiddenAreaIds = $("#areaIds").val();
                var hiddenAreaIds = removeAreaValue(hiddenAreaIds, areaId, ",");
                $("#areaIds").val(hiddenAreaIds);
            } catch (error) {
                alert(error.message);
            }

        },
        type: 'POST'
    });
}

var removeAreaValue = function (list, value, separator) {
    separator = separator || ",";
    var values = list.split(separator);
    for (var i = 0; i < values.length; i++) {
        if (values[i] == value) {
            values.splice(i, 1);
            return values.join(separator);
        }
    }
    return list;
}