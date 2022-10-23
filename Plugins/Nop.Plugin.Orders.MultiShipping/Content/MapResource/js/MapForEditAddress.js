$(function () {

});

currentMap = new createPostMap(false);

var currentMarker,
    normalSenderMarker,
    movesenderMarker,
    map;
var changeCity = true;


function createPostMap(isForgein, isFromAp) {
    normalSenderMarker = L.icon({
        iconUrl: '/Plugins/Orders.MultiShipping/Content/MapResource/css/images/senderNormal.png',
        iconSize: [50, 107], // size of the icon
        //shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        //shadowAnchor: [4, 62],  // the same for the shadow
        //popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });
    movesenderMarker = L.icon({
        iconUrl: '/Plugins/Orders.MultiShipping/Content/MapResource/css/images/senderMove.png',
        iconSize: [50, 107], // size of the icon
        // shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        // shadowAnchor: [4, 62],  // the same for the shadow
        // popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    map = null;
    map = new L.Map('mapBox', {
        key: 'web.opKiXFO6bIKUdikqL3nlTDxID4p4cKSAaDT7iHeu',
        maptype: 'dreamy-gold',
        poi: true,
        traffic: true,
        zoom: 14,
    });

    //map.on('move', function (e) {
    //    if (currentMarker)
    //        currentMarker.setLatLng(map.getCenter());
    //}).on('movestart', function () {
    //    if (currentMarker) {
    //        var _addressType = $('#addressType').val();
    //        currentMarker.setIcon(movesenderMarker);
    //    }
    //}).on('moveend', function () {
    //    if (currentMarker) {
    //        var _addressType = $('#addressType').val();
    //        currentMarker.setIcon(normalSenderMarker);
    //        SetLocationData();
    //    }
    //});
    //api key :service.V2F9Dx5uv23EkUsc6ceFjueZogiOusCpviy9PEDl
    //web key : web.opKiXFO6bIKUdikqL3nlTDxID4p4cKSAaDT7iHeu

    map.setMapType("dreamy-gold");
    setmapView(null, null, map, 11);

    var searchboxControl = createSearchboxControl();
    var control = new searchboxControl({
        sidebarTitleText: 'Header'
    });
    control._searchfunctionCallBack = function (searchkeywords) {
        var latlng = map.getCenter();

        var requestUrl = 'https://api.neshan.org/v1/search?term=' + searchkeywords + '&lat=' + latlng.lat + '&lng=' + latlng.lng
        $.ajax({
            beforeSend: function (request) {
                request.setRequestHeader("Api-Key", 'service.V2F9Dx5uv23EkUsc6ceFjueZogiOusCpviy9PEDl');
                $('#searchbox-searchbutton').hide();
                $('#Place_spinner').show();
            },
            complete: function () {
                $('#searchbox-searchbutton').show();
                $('#Place_spinner').hide();
            },
            type: "GET",
            url: requestUrl,
            success: function (data) {
                console.log(data);
                showSearchResult(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });

    }
    map.addControl(control);
}

function showSearchResult(result, isForegin) {
    $('.searchResult').each(function () {
        $(this).remove();
    });
    var country = $('#Country option:selected').text();
    var State = $('#CityId option:selected').text();

    if (result && result.count > 0) {
        for (var i in result.items) {
            if (i == 5)
                break;
            var current_Item = result.items[i];
            var addressTxt = (current_Item.region ? current_Item.region : '')
                + (current_Item.neighbourhood ? ',' + current_Item.neighbourhood : '')
                + (current_Item.address ? ',' + current_Item.address : '');

            var location = (current_Item.location ? current_Item.location.y + `,` + current_Item.location.x : '');
            var item = `<div id="boxcontainer" onClick="searchResultClick(this)" style="margin:3px;padding-right: 10px;" class="searchbox searchbox-shadow searchResult" data-val="` + location + `"><i class="fa fa-map-pin" aria-hidden="true"></i>`
                + `<span style="margin-right:10px">` + addressTxt + `</div>`;
            if ((country && addressTxt.includes(country)) || (State && addressTxt.includes(State))) {
                $('#controlbox').append(item);
            }
        }
    }

}


function searchResultClick(item) {
    var _lat = parseFloat($(item).attr('data-val').split(',')[0]);
    var _lon = parseFloat($(item).attr('data-val').split(',')[1]);
    $('.searchResult').each(function () {
        $(this).remove();
    });
    map.setView([_lat, _lon], 14);
};

function setmapView(_lat, _lon, currentMap, zoom) {

    var map = currentMap;/// (postMap ? postMap : (PrivatePostMap ? PrivatePostMap : CollectorAndDistributermap));
    if (!map)
        return;
    if (!_lat || !_lon) {
        _lat = 35.7248;
        _lon = 51.3817;
    }
    map.setView([_lat, _lon], (zoom ? zoom : 11));
    currentLocation = { lat: parseFloat(_lat), lng: parseFloat(_lon) };
    var _addressType = $('#addressType').val();
    if (!currentMarker) {
        currentMarker = L.marker(currentLocation, { normalSenderMarker });
        currentMarker.off('click');
        currentMarker.on('click', function (e) {

            e.originalEvent.stopPropagation();
            SetLocationData();
            currentMarker.setIcon(normalSenderMarker);
        }).addTo(map);
        currentMarker.setIcon(normalSenderMarker);
    }
    else {
        currentMarker.setLatLng(currentLocation);
        currentMarker.setIcon(normalSenderMarker);
    }
}

function SetLocationData() {
    var latlng = currentMarker.getLatLng();
    $('#Latitude').val(latlng.lat);
    $('#Longitude').val(latlng.lng);
}

function loadCity() {
    var countryId = $(`#Country`).val();
    $.ajax({
        cache: true,
        type: "GET",
        url: '/ShipitoCheckout/GetStatesByCountryId?countryId=' + countryId,
        data: {},
        success: function (data) {
            $(`#CityId`).html('');
            $(`#CityId`).append(new Option('انتخاب کنید....', '0', true, true));
            $.each(data, function (id, item) {
                $(`#CityId`).append(new Option(item.Text, item.Value, false, false));
            });
            $(`#CityId`).select2();
            if (changeCity)
                $('#CityId').val('@Model.CityId').trigger('change');
            changeCity = false;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log('Failed to retrieve ');
            $(`#CityId`).css('border', '1px solid red');
        }
    });
}
$(document).on("click", "#grid tbody tr", function (e) {
    var element = e.target || e.srcElement;
    if ($(element).hasClass('.BtnEdit')) {
        var data = $("#grid").data("kendoGrid").dataItem($(element).closest("tr"));

    }
});