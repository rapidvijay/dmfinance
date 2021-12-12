$("#btnCreateCommunityPlace").on("click", function () {
    var url = $(this).data("url");
    var hiddenAreaId = $("#hiddenAreaId").val();
    var hiddencommunitiesPlacesIds = $("#communitiesPlacesIds").val();

    url = url + '?areaId=' + hiddenAreaId + '&communityPlaceIds=' + hiddencommunitiesPlacesIds;

    $.get(url, function (data) {
        $('#createCommunityPlacesContainer').html(data);

        $('#CreateCommunityPlaceModal').modal('show');
    });

});


//########################################################################
var communityPlaceListVM;
communityPlaceListVM = {
    dt: null,

    init: function () {
        dt = $('#communityPlace-data-table').DataTable({
            "serverSide": true,
            "processing": true,
            "ajax": {
                "url": '/Areas/Get/' + $("#hiddenAreaId").val(),
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
                        return '<a  href="#" style="' + $("#displayId").val() + '"  onclick="deleteAreaCommunityPlacesById(' + data + ')" class="deleteAreaCommunityPlacesById">Delete</a>';
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


//$(function () {
//    var hiddenAreaId = $("#hiddenAreaId").val();
    

//    // Advanced Search Modal Search button click handler
//    //$('#btnPerformAdvancedSearch').on("click", communityPlaceListVM.refresh);
//    if (hiddenAreaId != null && hiddenAreaId != "") {
//        // initialize the datatables
//        communityPlaceListVM.init();
//    }

//});



function saveCommunityPlace(communityPlaceId) {
    var hiddenAreaId = $("#hiddenAreaId").val();

    $.ajax({
        url: '/Areas/saveCommunityPlace?communityPlaceId=' + communityPlaceId + '&areaId=' + hiddenAreaId,
        data: {
            // format: 'json'
        },
        error: function () {
            $('#info').html('<p>An error has occurred</p>');
        },
        // dataType: 'jsonp',
        success: function (data) {
            console.log("data= " + data);
            if (data == "success") {
                $('#CreateCommunityPlaceModal').modal('hide');
                $('#createCommunityPlacesContainer').html("");
                communityPlaceListVM.refresh();
                var communitiesPlacesIds = $("#communitiesPlacesIds").val();
                if (communitiesPlacesIds != null && communitiesPlacesIds != '') {
                    communitiesPlacesIds = communitiesPlacesIds + ',' + communityPlaceId;
                } else {
                    communitiesPlacesIds = communityPlaceId;
                }

                $("#communitiesPlacesIds").val(communitiesPlacesIds);
            }
        },
        type: 'POST'
    });

    return false;
}


function deleteAreaCommunityPlacesById(areaCommunitiesPlaceId) {
    var hiddenAreaId = $("#hiddenAreaId").val();

    bootbox.confirm("Are you sure want to delete Community Place?", function (result) {
        if (result == true) {
            deleteAreaCommunityPlacesByIdAjax(areaCommunitiesPlaceId, hiddenAreaId);
        }
    });


    return false;
}


function deleteAreaCommunityPlacesByIdAjax(areaCommunitiesPlaceId, hiddenAreaId) {
    $.ajax({
        url: '/Areas/deleteAreaCommunityPlacesById?areaCommunitiesPlaceId=' + areaCommunitiesPlaceId + '&areaId=' + hiddenAreaId,
        data: {
            // format: 'json'
        },
        error: function () {
            $('#info').html('<p>An error has occurred</p>');
        },
        // dataType: 'jsonp',
        success: function (data) {
            console.log("data= " + data);
            var communityPlaceId = data;
            $('#createCommunityPlacesContainer').html("");
            communityPlaceListVM.refresh();
            var communitiesPlacesIds = $("#communitiesPlacesIds").val();
            var communitiesPlacesIds = removeCommunityPlaceValue(communitiesPlacesIds, communityPlaceId, ",");
            $("#communitiesPlacesIds").val(communitiesPlacesIds);

        },
        type: 'POST'
    });
}

var removeCommunityPlaceValue = function (list, value, separator) {
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