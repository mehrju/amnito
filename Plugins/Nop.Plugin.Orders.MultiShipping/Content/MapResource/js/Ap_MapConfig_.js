postMap = null;
var PrivatePostMap;
var CollectorAndDistributermap;
var clickMarker = null;
var reciverMarker = null;
var senderMarker = null;
var currentLocation = null;
var currentMarker = null;
var SetLocationData = null;
var bounds_group = null;
var CollectorAndDistributertLayer = null;
var privatePostLayer = null;
var postLayer = null;
var cityAreaLayer = null;
var tehranCityAreaLayer = null;
var tabrizCityAreaLayer = null;
var ahvazCityAreaLayer = null;

var tehranPostAreas = [
    {
        Id: 4,
        Name: "منطقه جنوب شرق ( 11 پستی )"
    },
    {
        Id: 579,
        Name: "منطقه جنوب شرق ( 17 پستی )"
    },
    {
        Id: 580,
        Name: "منطقه جنوب غرب ( 13 پستی )"
    },
    {
        Id: 581,
        Name: "منطقه شمال غرب ( 14 پستی )"
    },
    {
        Id: 582,
        Name: "منطقه شمال ( 15 پستی )"
    },
    {
        Id: 583,
        Name: "منطقه شمال ( 19 پستی )"
    },
    {
        Id: 584,
        Name: "منطقه شمال شرق ( 16 پستی )"
    },
    {
        Id: 585,
        Name: "منطقه جنوب ( 18 پستی )"
    }
]
function searchResultClick(item) {
    var _lat = parseFloat($(item).attr('data-val').split(',')[0]);
    var _lon = parseFloat($(item).attr('data-val').split(',')[1]);
    $('.searchResult').each(function () {
        $(this).remove();
    });
    postMap.setView([_lat, _lon], 14);
};

function distroyMap() {
    var map = (postMap ? postMap : (PrivatePostMap ? PrivatePostMap : CollectorAndDistributermap));
    if (map)
        map.off();
    if (map && map.remove)
        map.remove();
    postMap = PrivatePostMap = CollectorAndDistributermap = null;
}

function setmapView(_lat, _lon, currentMap, zoom) {
    
    var map = currentMap;/// (postMap ? postMap : (PrivatePostMap ? PrivatePostMap : CollectorAndDistributermap));
    if (!map)
        return;
    if (!_lat || !_lon) {
        _lat = 35.7248;
        _lon = 51.3817;
    }
    map.setView([_lat, _lon], (zoom ?zoom:11));
    currentLocation = { lat: parseFloat(_lat), lng: parseFloat(_lon) };
    var _addressType = $('#addressType').val();
    if (!currentMarker) {
        currentMarker = L.marker(currentLocation, { icon: _addressType == 'Sender' ? normalSenderMarker : normalReciverMarker });
        currentMarker.off('click');
        currentMarker.on('click', function (e) {
            
            e.originalEvent.stopPropagation();
            currentMarker.setIcon(_addressType == 'Sender' ? normalSenderMarker : normalReciverMarker);
            
        }).addTo(map);
    }
    else {
        currentMarker.setIcon(_addressType == 'Sender' ? normalSenderMarker : normalReciverMarker);
    }
}

function isInArea(layerArray, latlng) {
    var insideLayer = false;
    for (var i in layerArray) {
        var inLayer = leafletPip.pointInLayer(latlng, layerArray[i], true);
        if (inLayer.length > 0) {
            insideLayer = true;
            break;
        }
    }
    return insideLayer;
}

isInPrivatePostArea = function (latlng) {
    var insideLayer = false;
    for (var i in privatePostLayer) {
        var inLayer = leafletPip.pointInLayer(latlng, privatePostLayer[i], true);
        if (inLayer.length > 0) {
            insideLayer = true;
            break;
        }
    }
    return insideLayer;
}
isInCollectorArea = function (latlng) {
    var insideLayer = false;
    for (var i in CollectorAndDistributertLayer) {
        var inLayer = leafletPip.pointInLayer(latlng, CollectorAndDistributertLayer[i], true);
        if (inLayer.length > 0) {
            insideLayer = true;
            break;
        }
    }
    return insideLayer;
}

getTehranPostAreaFromLayer = function (latlng) {
    var _stateName = '';
    for (var i in postLayer) {
        var _currentLayer = postLayer[i];
        var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
        if (inLayer.length > 0) {
            //var AreaId = _currentLayer.options.layerName.split('_')[1];
            //_stateName = tehranPostAreas.find(x => x.Name.includes(AreaId)).Name;
            _stateName = 'شهر تهران';// tehranPostAreas.find(x => x.Id == 579).Name;
            break;
        }
    }
    return _stateName;
}
getTehranAreaFromLayer = function (latlng, ) {
    debugger;
    var AreaId;
    for (var i in tehranCityAreaLayer) {
        var _currentLayer = tehranCityAreaLayer[i];
        var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
        if (inLayer.length > 0) {
            var AreaId = _currentLayer.options.layerName;
            break;
        }
    }
    if (!AreaId) {
        for (var i in ahvazCityAreaLayer) {
            var _currentLayer = ahvazCityAreaLayer[i];
            var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
            if (inLayer.length > 0) {
                var AreaId = _currentLayer.options.layerName;
                break;
            }
        }
    }
    if (!AreaId) {
        for (var i in tabrizCityAreaLayer) {
            var _currentLayer = tabrizCityAreaLayer[i];
            var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
            if (inLayer.length > 0) {
                var AreaId = _currentLayer.options.layerName;
                break;
            }
        }
    }
    return AreaId;
}
isInCityAreaFromLayer = function (latlng) {
    debugger;
    for (var i in cityAreaLayer) {
        var _currentLayer = cityAreaLayer[i];
        var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
        if (inLayer.length > 0) {
            return true;
        }
    }

        for (var i in ahvazCityAreaLayer) {
            var _currentLayer = ahvazCityAreaLayer[i];
            var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
            if (inLayer.length > 0) {
                return true;
            }
        }
    
   
        for (var i in tabrizCityAreaLayer) {
            var _currentLayer = tabrizCityAreaLayer[i];
            var inLayer = leafletPip.pointInLayer(latlng, _currentLayer, true);
            if (inLayer.length > 0) {
                return true;
            }
        }
    
    return false;
}

function createPostMap(isForgein,isFromAp) {
    normalSenderMarker = L.icon({
        iconUrl: '../Plugins/Orders.MultiShipping/Content/MapResource/css/images/senderNormal.png',
        iconSize: [50, 107], // size of the icon
        //shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        //shadowAnchor: [4, 62],  // the same for the shadow
        //popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });
    movesenderMarker = L.icon({
        iconUrl: '../Plugins/Orders.MultiShipping/Content/MapResource/css/images/senderMove.png',
        iconSize: [50, 107], // size of the icon
        // shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        // shadowAnchor: [4, 62],  // the same for the shadow
        // popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });
    normalReciverMarker = L.icon({
        iconUrl: '../Plugins/Orders.MultiShipping/Content/MapResource/css/images/reciverNormal.png',
        iconSize: [50, 107], // size of the icon
        //shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        //shadowAnchor: [4, 62],  // the same for the shadow
        //popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });
    moveReciverMarker = L.icon({
        iconUrl: '../Plugins/Orders.MultiShipping/Content/MapResource/css/images/reciverMove.png',
        iconSize: [54, 107], // size of the icon
        // shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        // shadowAnchor: [4, 62],  // the same for the shadow
        // popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });
    var map = null;
    if (!isForgein) {
        map = new L.Map('mapBox', {
            key: 'web.opKiXFO6bIKUdikqL3nlTDxID4p4cKSAaDT7iHeu',
            maptype: 'dreamy-gold',
            poi: true,
            traffic: false,
            zoom: 14,
        });

        map.setMapType("dreamy-gold");
    }
    else {
        map = L.map('mapBox', { zoomControl: true, maxZoom: 28, minZoom: 1 });
        var openStreetMap_layer = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            opacity: 1.0,
            attribution: '',
            minZoom: 1,
            maxZoom: 28,
            minNativeZoom: 0,
            maxNativeZoom: 19
        });
        map.addLayer(openStreetMap_layer);
    }
    postMap = map;

    bounds_group = new L.featureGroup([]);
    function setBounds() {
    }
   
    var searchboxControl = createSearchboxControl();
    var control = new searchboxControl({
        sidebarTitleText: 'Header',
    });
    control._searchfunctionCallBack = function (searchkeywords) {
        var latlng = map.getCenter();
        if ($('#isForginRequest').val()=='true' && $('#addressType').val() != 'Sender') {
            var foreginurl = 'https://nominatim.openstreetmap.org/?addressdetails=1&q=' + searchkeywords + '&format=json&limit=1';
            $.ajax({
                beforeSend: function (request) {
                    $('#searchbox-searchbutton').hide();
                    $('#Place_spinner').show();
                },
                complete: function () {
                    $('#searchbox-searchbutton').show();
                    $('#Place_spinner').hide();
                },
                type: "GET",
                url: foreginurl,
                success: function (data) {
                    showSearchResult(data, true);
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }
        else {
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
                    showSearchResult(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }
    }
    map.addControl(control);
   
    function showSearchResult(result, isForegin) {
        $('.searchResult').each(function () {
            $(this).remove();
        });
        if (!isForegin) {
            if (result && result.count > 0) {
                for (var i in result.items) {
                    if (i == 5)
                        return;
                    var current_Item = result.items[i];
                    var addressTxt = (current_Item.region ? current_Item.region : '')
                        + (current_Item.neighbourhood ? ',' + current_Item.neighbourhood : '')
                        + (current_Item.address ? ',' + current_Item.address : '');
                    var location = (current_Item.location ? current_Item.location.y + `,` + current_Item.location.x : '');
                    var item = `<div id="boxcontainer" onClick="searchResultClick(this)" style="margin:3px;padding-right: 10px;" class="searchbox searchbox-shadow searchResult" data-val="` + location + `"><i class="fa fa-map-pin" aria-hidden="true"></i>`
                        + `<span style="margin-right:10px">` + addressTxt + `</div>`;
                    $('#controlbox').append(item);
                }
            }
        }
        else if (result) {
            $(result).each(function () {
                var location = (this.lat + `,` + this.lon);
                var item = `<div id="boxcontainer" onClick="searchResultClick(this)" style="margin:3px;padding-right: 10px;" class="searchbox searchbox-shadow searchResult" data-val="` + location + `"><i class="fa fa-map-pin" aria-hidden="true"></i>`
                    + `<span style="margin-right:10px">` + this.display_name + `</div>`;
                $('#controlbox').append(item);
            });
        }
    }

    map.on('click', function () {
        $('.searchResult').each(function () {
            $(this).remove();
        });
    });
    map.on('move', function (e) {
        if (currentMarker)
            currentMarker.setLatLng(map.getCenter());
    }).on('movestart', function () {
        if (currentMarker) {
            var _addressType = $('#addressType').val();
            currentMarker.setIcon(_addressType == 'Sender' ? movesenderMarker : moveReciverMarker);
        }
    }).on('moveend', function () {
        if (currentMarker) {
            var _addressType = $('#addressType').val();
            currentMarker.setIcon(_addressType == 'Sender' ? normalSenderMarker : normalReciverMarker);
        }
    });
    function reversByNominatim(IsForegin, latlng) {
        var forginReversurl = 'https://nominatim.openstreetmap.org/reverse?format=json&lat=' + latlng.lat + '&lon=' + latlng.lng + '&zoom=18&addressdetails=18';
        $.ajax({
            beforeSend: function (request) {
                request.setRequestHeader("accept-language", 'fa');
                $('.ajax-loading-block-window').show();
            },
            complete: function () {
                $('.ajax-loading-block-window').hide();
            },
            async: IsForegin,
            type: "GET",
            url: forginReversurl,
            success: function (data) {
                if (data.address) {
                    SwitchToAddressContent();
                    var ForeginCountry = data.address.country;
                    var Country = data.address.state;
                    if (Country)
                        Country = Country.replace('استان', '').trim();
                    var city = data.address.city;
                    var county = data.address.county;
                    var address = data.display_name;
                    if (IsForegin) {
                        if (ForeginCountry)
                            $('#receiver_ForeginCountry').val($('#receiver_ForeginCountry').find("option:contains('" + ForeginCountry + "')").val()).trigger('change');
                        if (city)
                            $('#receiver_ForeginCountryCity').val(city);
                        var canFillAddress = true;
                        if ($('#Address1').val() != '') {
                           
                                asanPardakht.application.showConfirmBox('آدرس گیرنده', 'فیلد آدرس گیرنده شما دارای مقدار می باشد.آیا آدرس پستی روی نقشه با آدرس شما جایگزین شود؟', 'بله', 'خیر', function () {
                                    canFillAddress = true;
                                }, function () { canFillAddress = false; });
                        }
                        if (canFillAddress) {
                            if (address) {
                                address = address.replace(ForeginCountry + ',', '').replace(city + ',', '').trim();
                                $('#Address1').val(address);
                            }
                        }
                    } else {
                        if (county)
                            county = county.replace('شهرستان', '').trim();
                        var _state = ((city ? city : '') + ',' + (county ? county : ''));
                        if (!$('#Country option:selected').text().includes(Country))
                            $('#tempStateName').val(_state);
                        else {
                            var stateItem = _state.split(',');
                            if ($('#State').find("option:contains('" + stateItem[0] + "')").length > 0) {
                                $('#State').val($('#State').find("option:contains('" + stateItem[0] + "')").val()).trigger('change');
                            }
                            else
                                $('#State').val($('#State').find("option:contains('" + stateItem[1] + "')").val()).trigger('change');
                        }

                       
                        console.log($('#tempStateName').val());
                    }

                }
                $('#mapBox').hide(250);
                $('#AddressContent').show(250);
                $('#confirmAddress').show();
                $('#ContinueAddress').hide();
                $('#_NewAddress').show(250);
                $('#oldAddressbox').hide(250);
                if ($('#addressType').val() != 'Sender') {
                    $('#_ForeginCountryCityDiv').show();
                    $('#_foreignCountryDiv').show();
                    $('#_CountryDiv').hide();
                    $('#_StateDiv').hide();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    }
    SetLocationData = function () {
        var latlng = currentMarker.getLatLng();

        var isInPostLayer = isInArea(postLayer, latlng);

        var isInCollectorAndDistributertLayer = isInArea(CollectorAndDistributertLayer, latlng);

        var isInPrivatePostLayer = isInArea(privatePostLayer, latlng);
        
        if ($('#addressType').val() == 'Sender') {
            $('#SenderLat').val(latlng.lat);
            $('#SenderLon').val(latlng.lng);
        }
        else {
            $('#ReciverLat').val(latlng.lat);
            $('#ReciverLon').val(latlng.lng);
        }

        if ($('#isForginRequest').val()=='true' && $('#addressType').val() != 'Sender') {
            reversByNominatim(true, latlng);
        }
        else {
            var requestUrl = 'https://api.neshan.org/v2/reverse?lat=' + latlng.lat + '&lng=' + latlng.lng;
            $.ajax({
                beforeSend: function (request) {
                    request.setRequestHeader("Api-Key", 'service.V2F9Dx5uv23EkUsc6ceFjueZogiOusCpviy9PEDl');
                    $('.ajax-loading-block-window').show();
                },
                complete: function () {
                    $('.ajax-loading-block-window').hide();
                },
                type: "GET",
                async: false,
                url: requestUrl,
                success: function (data) {
                    
                    if (data.status == 'OK' && data.state) {
                        SwitchToAddressContent();
                        var Country = data.state.replace('استان', '').trim();
                        var State = '';
                        if (data.city)
                            State = data.city.trim();
                        var address = '';
                        if (data.formatted_address)
                            address = data.formatted_address;

                        if (State) {
                            if (State == 'تهران') {
                                var _tehranState = getTehranPostAreaFromLayer(latlng);
                                if (!_tehranState) {
                                    reversByNominatim(false, latlng);
                                }
                                else {
                                    if (!$('#Country option:selected').text().includes(Country))
                                        $('#tempStateName').val(_tehranState);
                                    else
                                        $('#State').val($('#State').find("option:contains('" + _tehranState + "')").val()).trigger('change');
                                }
                            }
                            else {
                                if (!$('#Country option:selected').text().includes(Country))
                                    $('#tempStateName').val(State);
                                else
                                    $('#State').val($('#State').find("option:contains('" + State + "')").val()).trigger('change');
                            }
                        }
                        else if (Country == 'تهران') {
                            reversByNominatim(false, latlng);
                        }
                        if (Country) {
                            if (!$('#Country option:selected').text().includes(Country))
                                $('#Country').val($('#Country').find("option:contains('" + Country + "')").val()).trigger('change');
                        }
                        if (address) {
                            if ($('#Address1').val() == '') {
                                address = address.replace(data.state + '،', '').trim();
                                if (data.city)
                                    address = address.replace(data.city + '،', '').trim();
                                $('#Address1').val(address);
                            }
                        }
                        $('#mapBox').hide(250);
                        $('#confirmAddress').show();
                        $('#ContinueAddress').hide();
                        $('#AddressContent').show(250);
                        $('#_NewAddress').show(250);
                        $('#oldAddressbox').hide(250);
                        $('#_ForeginCountryCityDiv').hide();
                        $('#_foreignCountryDiv').hide();
                        $('#_CountryDiv').show();
                        $('#_StateDiv').show();
                        //initCountryDropdown();
                        //initStateDropdown();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }
    }
    postLayer = AddPostLayer(map);
    CollectorAndDistributertLayer = createCollectorAndDistributertLayer(map);
    privatePostLayer = createPrivatePostLayer(map);
    map.on('locationerror', function () {
        setmapView(null, null, currentMap, 11);
        $('path').each(function () { $(this).hide() });
    });
    map.on('locationfound', function () {
        $('path').each(function () { $(this).hide() });
    });
    return map;
}
function AddPostLayer(map) {

    var postLayerArray = [];
    map.createPane('pane_11_1');
    map.getPane('pane_11_1').style.zIndex = 401;
    map.getPane('pane_11_1').style['mix-blend-mode'] = 'normal';
    var layer_11_1 = new L.geoJson(json_11_1, {
        attribution: '',
        interactive: true,
        dataVar: 'json_11_1',
        layerName: 'layer_11_1',
        pane: 'pane_11_1'
    });
    bounds_group.addLayer(layer_11_1);
    map.addLayer(layer_11_1);
    postLayerArray.push(layer_11_1);

    map.createPane('pane_13_2');
    map.getPane('pane_13_2').style.zIndex = 402;
    map.getPane('pane_13_2').style['mix-blend-mode'] = 'normal';
    var layer_13_2 = new L.geoJson(json_13_2, {
        attribution: '',
        interactive: true,
        dataVar: 'json_13_2',
        layerName: 'layer_13_2',
        pane: 'pane_13_2'
    });
    bounds_group.addLayer(layer_13_2);
    map.addLayer(layer_13_2);
    postLayerArray.push(layer_13_2);

    map.createPane('pane_18_3');
    map.getPane('pane_18_3').style.zIndex = 403;
    map.getPane('pane_18_3').style['mix-blend-mode'] = 'normal';
    var layer_18_3 = new L.geoJson(json_18_3, {
        attribution: '',
        interactive: true,
        dataVar: 'json_18_3',
        layerName: 'layer_18_3',
        pane: 'pane_18_3'
    });
    bounds_group.addLayer(layer_18_3);
    map.addLayer(layer_18_3);
    postLayerArray.push(layer_18_3);

    map.createPane('pane_14_4');
    map.getPane('pane_14_4').style.zIndex = 404;
    map.getPane('pane_14_4').style['mix-blend-mode'] = 'normal';
    var layer_14_4 = new L.geoJson(json_14_4, {
        attribution: '',
        interactive: true,
        dataVar: 'json_14_4',
        layerName: 'layer_14_4',
        pane: 'pane_14_4'
    });
    bounds_group.addLayer(layer_14_4);
    map.addLayer(layer_14_4);
    postLayerArray.push(layer_14_4);

    map.createPane('pane_19_5');
    map.getPane('pane_19_5').style.zIndex = 405;
    map.getPane('pane_19_5').style['mix-blend-mode'] = 'normal';
    var layer_19_5 = new L.geoJson(json_19_5, {
        attribution: '',
        interactive: true,
        dataVar: 'json_19_5',
        layerName: 'layer_19_5',
        pane: 'pane_19_5'
    });
    bounds_group.addLayer(layer_19_5);
    map.addLayer(layer_19_5);
    postLayerArray.push(layer_19_5);

    map.createPane('pane_15_6');
    map.getPane('pane_15_6').style.zIndex = 406;
    map.getPane('pane_15_6').style['mix-blend-mode'] = 'normal';
    var layer_15_6 = new L.geoJson(json_15_6, {
        attribution: '',
        interactive: true,
        dataVar: 'json_15_6',
        layerName: 'layer_15_6',
        pane: 'pane_15_6'
    });
    bounds_group.addLayer(layer_15_6);
    map.addLayer(layer_15_6);
    postLayerArray.push(layer_15_6);

    map.createPane('pane_16_7');
    map.getPane('pane_16_7').style.zIndex = 407;
    map.getPane('pane_16_7').style['mix-blend-mode'] = 'normal';
    var layer_16_7 = new L.geoJson(json_16_7, {
        attribution: '',
        interactive: true,
        dataVar: 'json_16_7',
        layerName: 'layer_16_7',
        pane: 'pane_16_7'
    });
    bounds_group.addLayer(layer_16_7);
    map.addLayer(layer_16_7);
    postLayerArray.push(layer_16_7);

    map.createPane('pane_17_8');
    map.getPane('pane_17_8').style.zIndex = 408;
    map.getPane('pane_17_8').style['mix-blend-mode'] = 'normal';
    var layer_17_8 = new L.geoJson(json_17_8, {
        attribution: '',
        interactive: true,
        dataVar: 'json_17_8',
        layerName: 'layer_17_8',
        pane: 'pane_17_8'
    });
    bounds_group.addLayer(layer_17_8);
    map.addLayer(layer_17_8);
    postLayerArray.push(layer_17_8);

    //setBounds();
    return postLayerArray;
}
function createCollectorAndDistributertLayer(map) {

    var layerArray = [];
    tehranCityAreaLayer = [];
    ahvazCityAreaLayer = [];
    tabrizCityAreaLayer = [];
    //tehran
    for (var i = 1; i <= 22; i++) {


        map.createPane('pane_tca' + i.toString());
        map.getPane('pane_tca' + i.toString()).style.zIndex = 401;
        map.getPane('pane_tca' + i.toString()).style['mix-blend-mode'] = 'normal';
        var layer_tca = new L.geoJson(eval('json_tca' + i.toString()), {
            attribution: '',
            interactive: true,
            dataVar: 'json_tca' + i.toString(),
            layerName: i.toString(),
            pane: 'pane_tca' + i.toString()
        });
        bounds_group.addLayer(layer_tca);
        layerArray.push(layer_tca);
        tehranCityAreaLayer.push(layer_tca);
        map.addLayer(layer_tca);
    }
    //ahvaz
    for (var i = 1; i <= 2; i++) {


        map.createPane('pane_Ahvaz_' + i.toString());
        map.getPane('pane_Ahvaz_' + i.toString()).style.zIndex = 401;
        map.getPane('pane_Ahvaz_' + i.toString()).style['mix-blend-mode'] = 'normal';
        var layer_Ahv = new L.geoJson(eval('json_Ahvaz_' + i.toString()), {
            attribution: '',
            interactive: true,
            dataVar: 'json_Ahvaz_' + i.toString(),
            layerName: i.toString(),
            pane: 'pane_Ahvaz_' + i.toString()
        });
        bounds_group.addLayer(layer_Ahv);
        layerArray.push(layer_Ahv);
        ahvazCityAreaLayer.push(layer_Ahv);
        map.addLayer(layer_Ahv);
    }
    //tabriz
    for (var i = 1; i <= 2; i++) {


        map.createPane('pane_Tabriz_' + i.toString());
        map.getPane('pane_Tabriz_' + i.toString()).style.zIndex = 401;
        map.getPane('pane_Tabriz_' + i.toString()).style['mix-blend-mode'] = 'normal';
        var layer_tab = new L.geoJson(eval('json_Tabriz_' + i.toString()), {
            attribution: '',
            interactive: true,
            dataVar: 'json_Tabriz_' + i.toString(),
            layerName: i.toString(),
            pane: 'pane_Tabriz_' + i.toString()
        });
        bounds_group.addLayer(layer_tab);
        layerArray.push(layer_tab);
        tabrizCityAreaLayer.push(layer_tab);
        map.addLayer(layer_tab);
    }

    map.createPane('pane_Isfahan_2');
    map.getPane('pane_Isfahan_2').style.zIndex = 410;
    map.getPane('pane_Isfahan_2').style['mix-blend-mode'] = 'normal';
    var layer_Isfahan_2 = new L.geoJson(json_Isfahan_2, {
        attribution: '',
        interactive: true,
        dataVar: 'json_Isfahan_2',
        layerName: 'layer_Isfahan_2',
        pane: 'pane_Isfahan_2'
    });
    bounds_group.addLayer(layer_Isfahan_2);
    layerArray.push(layer_Isfahan_2);
    map.addLayer(layer_Isfahan_2);

   

    map.createPane('pane_get_geojson_4');
    map.getPane('pane_get_geojson_4').style.zIndex = 412;
    map.getPane('pane_get_geojson_4').style['mix-blend-mode'] = 'normal';
    var layer_get_geojson_4 = new L.geoJson(json_get_geojson_4, {
        attribution: '',
        interactive: true,
        dataVar: 'json_get_geojson_4',
        layerName: 'layer_get_geojson_4',
        pane: 'pane_get_geojson_4'
    });
    bounds_group.addLayer(layer_get_geojson_4);
    layerArray.push(layer_get_geojson_4);
    map.addLayer(layer_get_geojson_4);

    map.createPane('pane_Alborz_5');
    map.getPane('pane_Alborz_5').style.zIndex = 413;
    map.getPane('pane_Alborz_5').style['mix-blend-mode'] = 'normal';
    var layer_Alborz_5 = new L.geoJson(json_Alborz_5, {
        attribution: '',
        interactive: true,
        dataVar: 'json_Alborz_5',
        layerName: 'layer_Alborz_5',
        pane: 'pane_Alborz_5'
    });
    bounds_group.addLayer(layer_Alborz_5);
    layerArray.push(layer_Alborz_5);
    map.addLayer(layer_Alborz_5);

    map.createPane('pane_Shiraz_6');
    map.getPane('pane_Shiraz_6').style.zIndex = 414;
    map.getPane('pane_Shiraz_6').style['mix-blend-mode'] = 'normal';
    var layer_Shiraz_6 = new L.geoJson(json_Shiraz_6, {
        attribution: '',
        interactive: true,
        dataVar: 'json_Shiraz_6',
        layerName: 'layer_Shiraz_6',
        pane: 'pane_Shiraz_6'
    });
    bounds_group.addLayer(layer_Shiraz_6);
    layerArray.push(layer_Shiraz_6);
    map.addLayer(layer_Shiraz_6);

    map.createPane('pane_Mashad_7');
    map.getPane('pane_Mashad_7').style.zIndex = 415;
    map.getPane('pane_Mashad_7').style['mix-blend-mode'] = 'normal';
    var layer_Mashad_7 = new L.geoJson(json_Mashad_7, {
        attribution: '',
        interactive: true,
        dataVar: 'json_Mashad_7',
        layerName: 'layer_Mashad_7',
        pane: 'pane_Mashad_7'
    });
    bounds_group.addLayer(layer_Mashad_7);
    layerArray.push(layer_Mashad_7);
    map.addLayer(layer_Mashad_7);

    //setBounds();
    return layerArray;
}
function createPrivatePostLayer(map) {

    var layerArray = [];

    map.createPane('pane__1');
    map.getPane('pane__1').style.zIndex = 416;
    map.getPane('pane__1').style['mix-blend-mode'] = 'normal';
    var layer__1 = new L.geoJson(json__1, {
        attribution: '',
        interactive: true,
        dataVar: 'json__1',
        layerName: 'layer__1',
        pane: 'pane__1'
    });
    bounds_group.addLayer(layer__1);
    layerArray.push(layer__1);
    map.addLayer(layer__1);



    map.createPane('pane__2');
    map.getPane('pane__2').style.zIndex = 417;
    map.getPane('pane__2').style['mix-blend-mode'] = 'normal';
    var layer__2 = new L.geoJson(json__2, {
        attribution: '',
        interactive: true,
        dataVar: 'json__2',
        layerName: 'layer__2',
        pane: 'pane__2'
    });
    bounds_group.addLayer(layer__2);
    map.addLayer(layer__2);
    layerArray.push(layer__2);


    map.createPane('pane__3');
    map.getPane('pane__3').style.zIndex = 418;
    map.getPane('pane__3').style['mix-blend-mode'] = 'normal';
    var layer__3 = new L.geoJson(json__3, {
        attribution: '',
        interactive: true,
        dataVar: 'json__3',
        layerName: 'layer__3',
        pane: 'pane__3',
    });
    bounds_group.addLayer(layer__3);
    map.addLayer(layer__3);
    layerArray.push(layer__3);


    map.createPane('pane__4');
    map.getPane('pane__4').style.zIndex = 419;
    map.getPane('pane__4').style['mix-blend-mode'] = 'normal';
    var layer__4 = new L.geoJson(json__4, {
        attribution: '',
        interactive: true,
        dataVar: 'json__4',
        layerName: 'layer__4',
        pane: 'pane__4',
    });
    bounds_group.addLayer(layer__4);
    map.addLayer(layer__4);
    layerArray.push(layer__4);

    map.createPane('pane__5');
    map.getPane('pane__5').style.zIndex = 420;
    map.getPane('pane__5').style['mix-blend-mode'] = 'normal';
    var layer__5 = new L.geoJson(json__5, {
        attribution: '',
        interactive: true,
        dataVar: 'json__5',
        layerName: 'layer__5',
        pane: 'pane__5',
    });
    bounds_group.addLayer(layer__5);
    map.addLayer(layer__5);
    layerArray.push(layer__5);


    map.createPane('pane__6');
    map.getPane('pane__6').style.zIndex = 421;
    map.getPane('pane__6').style['mix-blend-mode'] = 'normal';
    var layer__6 = new L.geoJson(json__6, {
        attribution: '',
        interactive: true,
        dataVar: 'json__6',
        layerName: 'layer__6',
        pane: 'pane__6',
    });
    bounds_group.addLayer(layer__6);
    map.addLayer(layer__6);
    layerArray.push(layer__6);
    //setBounds();
    return layerArray;
}
