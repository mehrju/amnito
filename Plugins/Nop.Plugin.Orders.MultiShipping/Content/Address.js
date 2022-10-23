jQuery.expr[':'].contains = function (a, i, m) {
    return jQuery(a).text().replace(/\s+/g, '')
        .indexOf(m[3].replace(/\s+/g, '')) >= 0;
};
senderAddress = {};
receiverAddress = {};
_senderAddressArr = [];
_reciverAddressArr = [];
cancelAddressItem = function () {
    $('#mapModal').modal('hide');
}
SwitchToAddressContent = function () {
    if ($('#newOldAddress').val() == 'newAddress') {
        $('#mapBox').hide(250);
        $('#AddressContent').show(250);
    }
}
function ClearAddress() {
    $('#FirstName').val('');
    $('#LastName').val('');
    $('#Company').val('');
    $('#Email').val('');
    $('#PhoneNumber').val('');
    $('#ZipPostalCode').val('');
    $('#Address1').val('');
    //$('#SenderLat').val('');
    //$('#SenderLon').val('');
    //$('#ReciverLat').val('');
    //$('#ReciverLon').val('');
    $('#addressType').val('');
    $('#tehranAreaId').val('');
    $('#isInCityArea').val('');
    $('#CollectorAreaId').val('');
    $('#trafficArea').val('');
}
function ClearAddress(type) {
    if (!type)
        return;
    $('#FirstName').val('');
    $('#LastName').val('');
    $('#Company').val('');
    $('#Email').val('');
    $('#PhoneNumber').val('');
    $('#ZipPostalCode').val('');
    $('#Address1').val('');
    if (type == 'Sender') {
        $('#SenderLat').val('');
        $('#SenderLon').val('');
        $('#tehranAreaId').val('');
        $('#isInCityArea').val('');
    }
    else {
        $('#ReciverLat').val('');
        $('#ReciverLon').val('');
    }
    $('#addressType').val('');
    $('#Country').val(0).trigger('change');
    $('#receiver_ForeginCountry').val(0).trigger('change');
    $('#receiver_ForeginCountryCity').val('');
}
var Address = function (options) {
    currentMap = null;
    var defulte = {
        document: null,
        wizard: "",
        WeightItem: "",
        InsuranceItem: "",
        CountryItem: "",
        StateItem: "",
        kartonSizeItem: "",
        IsAgent: false,
        IsInCodRole: false,
        IsForegin: false,
        IsHeavy: false,
        IsCod: false
    }
    this.construct = function (options) {
        $.extend({}, defulte, options);


        function fillAddress(selectitem, addressType) {
            ClearAddress();

            if (selectitem) {
                $('#FirstName').val(selectitem.FirstName);
                $('#LastName').val(selectitem.LastName);
                $('#Company').val(selectitem.Company);
                $('#Email').val(selectitem.Email);
                $('#PhoneNumber').val(selectitem.PhoneNumber);
                $('#ZipPostalCode').val(selectitem.ZipPostalCode);
                $('#Address1').val(selectitem.Address1);
                $('#addressType').val(addressType);
                $('#tempStateName').val(selectitem.StateProvinceName);
                $('#Country').val(selectitem.CountryId).trigger('change');
                if ($('#addressType').val() != 'Sender' && options.IsForegin) {
                    $('#_CountryDiv').hide();
                    $('#_StateDiv').hide();
                    $('#_foreignCountryDiv').show();
                    $('#_ForeginCountryCityDiv').show();
                    $('#receiver_ForeginCountryCity').val(selectitem.ForeginCityName);
                    $('#receiver_ForeginCountry').val(selectitem.ForeginCountryId).trigger('change');
                    $(options.CountryItem).val('0').trigger('change');
                    $(options.StateItem).val('0').trigger('change');
                } else {
                    if ($('#newOldAddress').val() != 'newAddress') {

                        $('#_CountryDiv').show();
                        $('#_StateDiv').show();
                    } else {
                        $('#_CountryDiv').hide();
                        $('#_StateDiv').hide();
                    }
                    $('#_foreignCountryDiv').hide();
                    $('#_ForeginCountryCityDiv').hide();
                    //$(options.CountryItem).val(selectitem.CountryId).trigger('change');
                    //$(options.StateItem).val(selectitem.StateProvinceId).trigger('change');
                    $('#receiver_ForeginCountryCity').val('');
                    $('#receiver_ForeginCountry').val('0').trigger('change');
                }

                if (selectitem.Lat && selectitem.Lon) {
                    if ($('#addressType').val() == 'Sender') {
                        $('#SenderLat').val(selectitem.Lat);
                        $('#SenderLon').val(selectitem.Lon);
                        $('#tehranAreaId').val(selectitem.tehranCityArea);
                        $('#CollectorAreaId').val(selectitem.collectorArea),
                            $('#isInCityArea').val(selectitem.isInCityArea);
                        $('#trafficArea').val(selectitem.trafficArea);
                        selectitem.SenderLat = selectitem.Lat;
                        selectitem.SenderLon = selectitem.Lon;
                    } else {
                        $('#ReciverLat').val(selectitem.Lat);
                        $('#ReciverLon').val(selectitem.Lon);
                        selectitem.ReciverLat = selectitem.Lat;
                        selectitem.ReciverLon = selectitem.Lon;
                    }
                }
                if ($('#addressType').val() == 'Sender' && selectitem.SenderLat) {
                    $('#SenderLat').val(selectitem.SenderLat);
                    $('#SenderLon').val(selectitem.SenderLon);
                } else if (selectitem.ReciverLat) {
                    $('#ReciverLat').val(selectitem.ReciverLat);
                    $('#ReciverLon').val(selectitem.ReciverLon);
                }
            } else {
                if ($('#addressType').val() != 'Sender' && options.IsForegin) {

                    $('#_CountryDiv').hide();
                    $('#_StateDiv').hide();
                    $('#_foreignCountryDiv').show();
                    $('#_ForeginCountryCityDiv').show();
                } else {
                    if ($('#newOldAddress').val() != 'newAddress') {

                        $('#_CountryDiv').show();
                        $('#_StateDiv').show();
                    } else {

                        $('#_CountryDiv').hide();
                        $('#_StateDiv').hide();
                    }
                    $('#_foreignCountryDiv').hide();
                    $('#_ForeginCountryCityDiv').hide();
                }
            }
        }


        $('#confirmAddress').click(function () {

            var $firstName = $('#FirstName');
            var $lastName = $('#LastName');
            var $company = $('#Company');
            var $email = $('#Email');
            var $phoneNumber = $('#PhoneNumber');
            var $zipPostalCode = $('#ZipPostalCode');
            var $address1 = $('#Address1');
            var $SenderLat = $('#SenderLat');
            var $SenderLon = $('#SenderLon');

            var $ReciverLat = $('#ReciverLat');
            var $ReciverLon = $('#ReciverLon');

            var msg = '';

            if ($('#addressType').val() == 'Sender') {
                if (!$(options.CountryItem).val() ||
                    $(options.CountryItem).val() == '0' ||
                    $(options.CountryItem).val() == '' ||
                    $(options.CountryItem).val() == '-1') {
                    msg = 'استان فرستنده را مشخص نمایید' + '\r\n';
                }
                if (!$(options.StateItem).val() ||
                    $(options.StateItem).val() == '0' ||
                    $(options.StateItem).val() == '' ||
                    $(options.StateItem).val() == '-1') {
                    msg = 'شهر فرستنده را مشخص نمایید' + '\r\n';
                }
                if ($SenderLat.val() == '' || $SenderLon.val() == '') {
                    msg = 'مکان فرستنده را بر روی نقشه مشخص نمایید' + '\r\n';

                    $('#AddressContent').hide(250);
                    $('#oldAddressbox').hide();
                    showMap();
                    $('path:not(.notMap)').each(function () { $(this).hide() });
                    $('.notMap').each(function () { $(this).show() });
                    loadlocation($(options.countryId).val(), $(options.StateItem).val(), $('#addressType').val());
                    $('#ContinueAddress').show();
                    $('#confirmAddress').hide();
                }

            } else {
                if (options.IsForegin == true) {
                    if ($('#receiver_ForeginCountry').val() == '0' || $('#receiver_ForeginCountry').val() == '') {
                        msg = 'کشور گیرنده را مشخص نمایید' + '\r\n';
                    }
                    if ($('#receiver_ForeginCountryCity').val() == '') {
                        msg = 'نام شهر در کشور گیرنده را مشخص نمایید' + '\r\n';
                    }
                } else {
                    if ($(options.CountryItem).val() == '0' ||
                        $(options.CountryItem).val() == '' ||
                        $(options.CountryItem).val() == '-1') {
                        msg = 'استان گیرنده را مشخص نمایید' + '\r\n';
                    }
                    if ($(options.StateItem).val() == '0' ||
                        $(options.StateItem).val() == '' ||
                        $(options.StateItem).val() == '-1') {
                        msg = 'شهر گیرنده را مشخص نمایید' + '\r\n';
                    }
                }
                if (options.IsForegin != true) {
                    if ($ReciverLat.val() == '' || $ReciverLon.val() == '') {
                        msg = 'مکان گیرنده را بر روی نقشه مشخص نمایید' + '\r\n';
                        $('#AddressContent').hide(250);
                        $('#oldAddressbox').hide();
                        showMap();
                        $('path:not(.notMap)').each(function () { $(this).hide() });
                        $('.notMap').each(function () { $(this).show() });
                        loadlocation($(options.countryId).val(), $(options.StateItem).val(), $('#addressType').val());
                    }
                }
            }
            if ($('#addressType').val() == 'Sender' && !options.IsForegin) {

                $.ajax({
                    beforeSend: function () {
                        $('.ajax-loading-block-window').show();
                    },
                    complete: function () {
                        $('.ajax-loading-block-window').hide();
                    },
                    type: "GET",
                    async: false,
                    url: "/ShipitoCheckout/IsInInvalidService",
                    data: { "countryId": $(options.CountryItem).val(), "stateId": $(options.StateItem).val() },
                    success: function (data) {
                        if (data.isInvalid == true) {
                            msg += 'در حال حاضر امکان ثبت سفارش از مبدا مورد نظر وجود ندارد';
                            return;
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                    }
                });
            }
            if ($firstName.val() == '')
                msg += 'وارد کردن نام الزامی می باشد' + '\r\n';
            if ($lastName.val() == '')
                msg += 'وارد کردن نام خانوادگی الزامی می باشد' + '\r\n';
            if ($firstName.val().length + $lastName.val().length > 50) {
                msg += 'نام و نام خانوادگی مجموعا نمی تواند بیشترا از 50 کاراکتر باشد' + '\r\n';
            }
            if ($phoneNumber.val() == '')
                msg += 'وارد کردن شماره تماس الزامی می باشد' + '\r\n';
            if ($phoneNumber.val().length < 11)
                msg += 'شماره تماس وارد شده نامعتبر می باشد' + '\r\n';
            if ($address1.val() == '')
                msg += 'وارد کردن آدرس الزامی می باشد' + '\r\n';

            if (msg != '') {
                alert(msg);
                //openSidePanel();
                return;
            }

            if ($('#addressType').val() == 'Sender') {

                $('#tehranAreaId')
                    .val(getTehranAreaFromLayer(
                        { lat: parseFloat($SenderLat.val()), lng: parseFloat($SenderLon.val()) }));
                $('#CollectorAreaId')
                    .val(getCollectorAreaId({ lat: parseFloat($SenderLat.val()), lng: parseFloat($SenderLon.val()) }));
                $('#isInCityArea')
                    .val(isInCityAreaFromLayer({
                        lat: parseFloat($SenderLat.val()),
                        lng: parseFloat($SenderLon.val())
                    }));
                if ($('#newOldAddress').val() != 'newAddress') {
                    $('#trafficArea')
                        .val(isIntarfficZone({ lat: parseFloat($SenderLat.val()), lng: parseFloat($SenderLon.val()) }));
                }
                senderAddress = {
                    FirstName: $firstName.val(),
                    LastName: $lastName.val(),
                    Company: $company.val(),
                    Email: $email.val(),
                    PhoneNumber: $phoneNumber.val(),
                    ZipPostalCode: $zipPostalCode.val(),
                    Address1: $address1.val(),
                    Lat: $SenderLat.val(),
                    Lon: $SenderLon.val(),
                    CountryId: $(options.CountryItem).val(),
                    StateProvinceId: $(options.StateItem).val(),
                    CountryName: $(options.CountryItem).find('option:selected').text(),
                    StateProvinceName: $(options.StateItem).find('option:selected').text(),
                    tehranCityArea: $('#tehranAreaId').val(),
                    collectorArea: $('#CollectorAreaId').val(),
                    isInCityArea: $('#isInCityArea').val(),
                    trafficArea: $('#trafficArea').val()
                };
                senderAddress.text = (senderAddress.CountryName +
                    '-' +
                    senderAddress.StateProvinceName +
                    '-' +
                    senderAddress.Address1 +
                    //'-' +
                    //senderAddress.ZipPostalCode +

                    (senderAddress.Company ? '-' + senderAddress.Company : '') +
                    '-' +
                    senderAddress.FirstName +
                    ' ' +
                    senderAddress.LastName +
                    '-' +
                    (senderAddress.PhoneNumber));
                $('#SenderStateTown').html(senderAddress.text);
                //if (!$('#oldAddress').val()) {
                $('#AddressBox').append(new Option(senderAddress.text, '-2', true, true));
                //}
            } else {

                receiverAddress = {
                    FirstName: $firstName.val(),
                    LastName: $lastName.val(),
                    Company: $company.val(),
                    Email: $email.val(),
                    PhoneNumber: $phoneNumber.val(),
                    ZipPostalCode: $zipPostalCode.val(),
                    Address1: $address1.val(),
                    Lat: $ReciverLat.val(),
                    Lon: $ReciverLon.val(),
                    CountryId: $(options.CountryItem).val(),
                    StateProvinceId: $(options.StateItem).val(),
                    ForeginCountryId: ($('#receiver_ForeginCountry').val()
                        ? $('#receiver_ForeginCountry').val().split('|')[0]
                        : null),
                    ForeginCountryNameEn: ($('#receiver_ForeginCountry').val()
                        ? $('#receiver_ForeginCountry').val().split('|')[1]
                        : null),
                    ForeginCountryName: $('#receiver_ForeginCountry option:selected').text(),
                    ForeginCityName: $('#receiver_ForeginCountryCity').val(),
                    CountryName: $(options.CountryItem).find('option:selected').text(),
                    StateProvinceName: $(options.StateItem).find('option:selected').text(),
                    //tehranCityArea = '',
                    //isInCityArea = ''
                };
                if (!options.IsForegin) {
                    receiverAddress.text = (receiverAddress.CountryName +
                        ' ' +
                        receiverAddress.StateProvinceName +
                        ' ' +
                        receiverAddress.Address1 +
                        ' ' +
                        receiverAddress.ZipPostalCode +
                        ' ' +
                        (receiverAddress.Company) +
                        ' ' +
                        receiverAddress.FirstName +
                        ' ' +
                        receiverAddress.LastName +
                        ' ' +
                        (receiverAddress.PhoneNumber))
                } else {
                    receiverAddress.text = (receiverAddress.ForeginCountryName +
                        ' ' +
                        receiverAddress.ForeginCityName +
                        ' ' +
                        receiverAddress.Address1 +
                        ' ' +
                        receiverAddress.ZipPostalCode +
                        ' ' +
                        (receiverAddress.Company) +
                        ' ' +
                        receiverAddress.FirstName +
                        ' ' +
                        receiverAddress.LastName +
                        ' ' +
                        (receiverAddress.PhoneNumber))
                }
                $('#ReceiverStateTown').html(receiverAddress.text);
                //if (!$('#AddressBox').val()) {
                $('#AddressBox').append(new Option(receiverAddress.text, '-2', true, true));
                //}
            }
            onSave();
            $('#mapModal').modal('hide');
        });

        //====================================Addresses=====================================================================

        $('.get-btn-map, .get-btn-map_i').click(function () {
            if ($(this).attr('Id') == 'get-btn-map_i' && options.IsHeavy) {
                alert('لطفا از ثبت آدرس جدید استفاده کنید');
                return false;
            }
            $('#addressType').val($(this).attr('data-val'));
            $('#newOldAddress').val($(this).attr('data-type'));
            SwitchToMapBox(options.IsForegin);
            if ($(this).attr('data-val') == 'Sender') {
                $('#MapAddressModalLabel').html('آدرس فرستنده');
            } else {
                $('#MapAddressModalLabel').html('آدرس گیرنده');
            }
            $('#mapModal').appendTo("body");
            $('#mapModal').modal('show', { backdrop: 'static' });
            $('#addressType').val($(this).attr('data-val'));
            $('[data-toggle="tooltip"], .tooltip').tooltip("hide");
            if ($('#newOldAddress').val() == 'newAddress') {
                if (!options.IsForegin || $(this).attr('data-val') == 'Sender') {
                    $('#Country').prop("disabled", true);
                    $('#State').prop("disabled", true);
                    if (!options.IsHeavy)
                        $('#mapBox').show();
                    else
                        $('#mapBox').hide();    
                    $('#EditAddress').hide(250);
                    $('#confirmAddress').hide(250);
                    $('#PreNewAddress').show(250);
                    $('#ContinueAddress').show(250);
                } else {
                    $('#mapBox').hide(250);
                    $('#EditAddress').hide(250);
                    $('#confirmAddress').show(250);
                    $('#PreNewAddress').hide(250);
                    $('#ContinueAddress').hide(250);
                }
            } else {
                $('#Country').prop("disabled", false);
                $('#State').prop("disabled", false);
                $('#mapBox').hide(250);
                $('#EditAddress').hide(250);
                $('#confirmAddress').show(250);
                $('#PreNewAddress').hide(250);
                $('#ContinueAddress').hide(250);
            }
        });
        $('#mapModal').on('shown.bs.modal',
            function () {
                if ($('div.leaflet-control-container').next().length > 0) {
                    $('div.leaflet-control-container').next().remove();
                    $('.leaflet-control-attribution').remove();
                }

                if (!$(options.CountryItem).hasClass("select2-hidden-accessible")) {
                    $(".select2-container:not(#AddressBox)").tooltip({
                        title: function () {
                            return $(this).prev().attr("title");
                        },
                        placement: "top",
                        html: true
                    });
                    $("#AddressBox").tooltip({
                        title: function () {
                            return $(this).prev().attr("title");
                        },
                        placement: "auto",
                        html: true
                    });
                    $(".myTooltip").tooltip({
                        placement: "top",
                        html: true
                    });

                    initCountryDropdown();
                    initStateDropdown();

                    $('#receiver_ForeginCountry').select2({
                        dropdownParent: $('#mapModal'),
                        allowClear: true,
                        placeholder: "انتخاب کشور گیرنده",
                    });

                    $('#AddressBox').select2({
                        placeholder: "قسمتی از  مشخصات یا موبایل یا آدرس قبلی را وارد کنید",
                        minimumInputLength: 4,
                        dropdownParent: $('#mapModal'),
                        ajax:
                        {
                            url: "/ShipitoCheckout/FetchAddress",
                            dataType: 'json',
                            quietMillis: 250,
                            data: function (term, page) {

                                return { 'searchtext': term.term };
                            },
                            processResults: function (data) {
                                _reciverAddressArr = data.results;
                                return {
                                    results: $.map(data.results, function (e) { return { id: e.id, text: e.text } })
                                };
                            },
                            cache: false
                        }
                    });


                    $('#AddressBox').on('change',
                        function (e) {

                            var id = $(this).val();
                            if (id) {
                                var selectedAddressitem = _reciverAddressArr.filter(x => x.id === id);
                                var isSender = $('#addressType').val() == 'Sender';
                                if (isSender) {
                                    senderAddress = {};
                                    $.extend(senderAddress, selectedAddressitem[0]);
                                    fillAddress(selectedAddressitem[0], 'Sender');
                                } else {
                                    receiverAddress = {};
                                    $.extend(receiverAddress, selectedAddressitem[0]);
                                    fillAddress(selectedAddressitem[0], 'Reciver');
                                }
                                $('#mapBox').hide(250);
                                $('#AddressContent').show(250);
                                $('#_NewAddress').show(250);
                                $('#oldAddressbox').hide(250);
                            }
                        });
                }
                if ($('#newOldAddress').val() == 'newAddress')
                    showAndFillAddress();
                $('path:not(.notMap)').each(function () { $(this).hide() });
                $('.notMap').each(function () { $(this).show() });
            });
        $('#searchOldAddress').click(function () {
            $('#mapBox').hide(250);
            $('#AddressContent').show(250);
            if (($('#addressType').val() == 'Sender' && senderAddress) ||
                (($('#addressType').val() != 'Sender' && receiverAddress))) {
                $('#AddressBox').empty().trigger('change');
                if ($('#addressType').val() == 'Sender' && senderAddress)
                    $('#AddressBox').append(new Option(senderAddress.text, '-2', true, true));
                else if ($('#addressType').val() != 'Sender' && receiverAddress)
                    $('#AddressBox').append(new Option(receiverAddress.text, '-2', true, true));

                $('#_NewAddress').show(250);
            } else {
                $('#_NewAddress').hide(250);
            }

            $('#oldAddressbox').show(250);
            if (!$(options.CountryItem).hasClass("select2-hidden-accessible")) {
                initCountryDropdown();
                initStateDropdown();
            }
        });
        $('#searchOnMap').click(function () {
            showMap();
            $('path:not(.notMap)').each(function () { $(this).hide() });
            $('.notMap').each(function () { $(this).show() });
            var lat, lon;
            lat = lon = null;
            if ($('#addressType').val() != 'Sender') {
                var lat = $('#ReciverLat').val();
                var lon = $('#ReciverLon').val();
            } else {
                var lat = $('#SenderLat').val();
                var lon = $('#SenderLon').val();
            }
            if (lat && lon)
                setmapView(lat, lon, currentMap, 14, options.IsCod);

            $('#AddressContent').hide(250);
            $('#_NewAddress').hide(250);
            $('#oldAddressbox').hide(250);
        });
        $('#ContinueAddress').click(function () {
            debugger;
            SetLocationData(options.IsCod, options.IsHeavy);
        });
        $('#EditAddress').click(function () {
            $('#_NewAddress').hide(250);
            $('#oldAddressbox').hide(250);
            $('#AddressContent').hide(250);
            $('#EditAddress').hide(250);
            $('#confirmAddress').hide(250)
            if(!options.IsHeavy)
                $('#mapBox').show(250);
            else
                $('#mapBox').hide();
            $('#PreNewAddress').show(250);
            $('#ContinueAddress').show(250);
        });
        $('#cleanAddressItem').click(function () {
            cancelAddressItem();
        });

        function showAndFillAddress() {

            if ($('#addressType').val() != 'Sender' && !$('#addressType').attr('showAlert')) {
                $('#addressType').attr('showAlert', 'true');
            }
            var _addressType = $('#addressType').val();

            var _senderLattxt = $('#SenderLat').val();
            var _senderLontxt = $('#SenderLon').val();
            var _ReciverLattxt = $('#ReciverLat').val();
            var _ReciverLontxt = $('#ReciverLon').val();

            var _address = (_addressType == 'Sender' ? senderAddress : receiverAddress);

            var lat = (_address
                ? (_address.Lat ? _address.Lat : (_addressType == 'Sender' ? _senderLattxt : _ReciverLattxt))
                : (_addressType == 'Sender' ? _senderLattxt : _ReciverLattxt));

            var lon = (_address
                ? (_address.Lon ? _address.Lon : (_addressType == 'Sender' ? _senderLontxt : _ReciverLontxt))
                : (_addressType == 'Sender' ? _senderLontxt : _ReciverLontxt));
            $.fn.modal.Constructor.prototype.enforceFocus = function () { };
            fillAddress(_address, _addressType);
            if (navigator.geolocation && (!lat || !lon)) {
                currentMap.locate({ setView: false, maxZoom: 14 });
            } else
                setmapView(lat, lon, currentMap, 14, options.IsCod);
        }
        function loadlocation(_countryId, _StateId, AddressType) {
            if (AddressType == 'Sender') {
                if ($('#SenderLat').val() && $('#SenderLon').val())
                    return;
            }
            else if ($('#ReciverLat').val() && $('#ReciverLon').val()) {
                return;
            }
            var postData = {
                CountryId: _countryId,
                StatePrivenceId: _StateId
            };
            $.ajax({
                cache: false,
                url: '/NewCheckout/GetLatLong',
                data: postData,
                type: "POST",
                success: function (data) {
                    if (data.Lat && data.Lon) {
                        setmapView(data.Lat, data.Lon, currentMap, 11, options.IsCod);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('خطا در زمان دریافت اطلاعات مختصات');
                }
            });
        }
        function loadlocationByStateChange(_countryId) {
            var postData = {
                CountryId: _countryId
            };
            $.ajax({
                cache: false,
                url: '/NewCheckout/GetLatLong',
                data: postData,
                type: "POST",
                success: function (data) {
                    if (data.Lat && data.Lon) {
                        //if (AddressType == 'Sender') {
                        //    $('#SenderLat').val(data.Lat);
                        //    $('#SenderLon').val(data.Lon);
                        //    senderAddress.Lat = data.Lat;
                        //    senderAddress.Lon = data.Lon;
                        //}
                        //else {
                        //    $('#ReciverLat').val(data.Lat);
                        //    $('#ReciverLon').val(data.Lon);
                        //    receiverAddress.Lat = data.Lat;
                        //    receiverAddress.Lon = data.Lon;
                        //}
                        setmapView(data.Lat, data.Lon, currentMap, 11, options.IsCod);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('خطا در زمان دریافت اطلاعات مختصات');
                }
            });
        }
        function getLocationByStateName(searchkeywords) {
            var latlng = currentMarker.getLatLng();
            var requestUrl = 'https://api.neshan.org/v1/search?term=' + searchkeywords + '&lat=' + latlng.lat + '&lng=' + latlng.lng;
            var latLng;
            $.ajax({
                beforeSend: function (request) {
                    request.setRequestHeader("Api-Key", 'service.V2F9Dx5uv23EkUsc6ceFjueZogiOusCpviy9PEDl');
                },
                complete: function () {
                },
                type: "GET",
                async: false,
                url: requestUrl,
                success: function (result) {

                    if (result && result.count > 0) {
                        if (result.items) {
                            var validLocation = result.items.filter(function (obj) {
                                return obj.category == 'region' && (obj.region.includes($('#PreCountry option:selected').text()) || obj.region.includes($('#PreState option:selected').text));
                            });
                            if (validLocation) {
                                var current_Item = validLocation[0];
                                latLng = (current_Item.location ? { lat: current_Item.location.y, lng: current_Item.location.x } : {});
                            }
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
            return latLng;
        }
        $.ajax({
            cache: true,
            type: "GET",
            url: "/ShipitoCheckout/getCountryList",
            data: {},
            success: function (data) {
                var $Country = $(options.CountryItem);
                var $Country1 = $('#PreCountry');

                $Country.append(new Option('استان را انتخاب کنید', '0', true, true));
                $Country1.append(new Option('استان را انتخاب کنید', '0', true, true));
                $.each(data, function (id, item) {
                    $Country.append(new Option(item.Text, item.Value, false, false));
                    $Country1.append(new Option(item.Text, item.Value, false, false));
                });
                $(PreCountry).select2({
                    placeholder: "انتخاب استان",
                    allowClear: true,
                    dropdownParent: $('#mapModal')
                });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log('Failed to retrieve Countries.');
            }
        });
        function onCountryChange() {
            var selectedItem = $(this).val();
            var ddlStates = $(options.StateItem);
            ddlStates.html('');

            ddlStates.append(new Option('درحال بارگذاری....', '-1', true, true));
            $.ajax({
                beforeSend: function () {
                    $('.ajax-loading-block-window').show();
                },
                complete: function () {
                    $('.ajax-loading-block-window').hide();
                },
                cache: false,
                type: "GET",
                url: !options.IsHeavy ? "/ShipitoCheckout/GetStatesByCountryId" : "/ShipitoCheckout/GetUbbarStatesByCountryId",
                data: { "countryId": selectedItem, "isCod": options.IsCod },
                success: function (reuslt) {
                    ddlStates.html('');
                    ddlStates.append(new Option('انتخاب کنید....', '0', true, true));
                    $.each(reuslt, function (id, item) {
                        ddlStates.append(new Option(item.Text, item.Value, false, false));
                    });
                    //$(options.StateItem).select2({
                    //    placeholder: "انتخاب شهر",
                    //    allowClear: true,
                    //    dropdownParent: $('#mapModal')
                    //});

                    if ($('#tempStateName').val() != '') {
                        var stateItem = $('#tempStateName').val().split(',');
                        if ($('#State').find("option:contains('" + stateItem[0] + "')").length > 0) {
                            $('#State').val($('#State').find("option:contains('" + stateItem[0] + "')").val()).trigger('change');
                            $('#PreState').val($('#PreState').find("option:contains('" + stateItem[0] + "')").val()).trigger('change');
                        }
                        else {
                            $('#State').val($('#State').find("option:contains('" + stateItem[1] + "')").val()).trigger('change');
                            $('#PreState').val($('#PreState').find("option:contains('" + stateItem[1] + "')").val()).trigger('change');
                            //alert('در حال حاضر امکان ثبت سفارش در شهر انتخابی شما وجود ندارد');
                        }

                        $('#tempStateName').val('');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve states.');
                }
            });
        }

        function onCountryChangePre() {
            var selectedItem = $(this).val();
            var ddlStates = $('#PreState');
            ddlStates.html('');
            $(options.CountryItem).val(selectedItem).trigger('change');
            loadlocationByStateChange(selectedItem);
            if (selectedItem == '1' && options.IsCod == true) {
                alert('کاربر گرامی در صورتی که ساکن شهر تهران هستید و نام منطقه پستی خود را نمی دانید از نقشه استفاده کنید');
            }
            ddlStates.append(new Option('درحال بارگذاری....', '-1', true, true));
            $.ajax({
                cache: false,
                type: "GET",
                url: !options.IsHeavy ? "/ShipitoCheckout/GetStatesByCountryId" : "/ShipitoCheckout/GetUbbarStatesByCountryId",
                data: { "countryId": selectedItem, "isCod": options.IsCod },
                success: function (reuslt) {
                    ddlStates.html('');
                    ddlStates.append(new Option('شهر مورد نظر را انتخاب کنید', '0', true, true));
                    $.each(reuslt, function (id, item) {
                        ddlStates.append(new Option(item.Text, item.Value, false, false));
                    });
                    $(ddlStates).select2({
                        placeholder: "انتخاب شهر",
                        allowClear: true,
                        dropdownParent: $('#mapModal')
                    });
                    //if ($('#tempStateName').val() != '') {
                    //    var stateItem = $('#tempStateName').val().split(',');
                    //    if ($('#State').find("option:contains('" + stateItem[0] + "')").length > 0) {
                    //        $('#State').val($('#State').find("option:contains('" + stateItem[0] + "')").val()).trigger('change');
                    //    }
                    //    else {
                    //        $('#State').val($('#State').find("option:contains('" + stateItem[1] + "')").val()).trigger('change');
                    //        //alert('در حال حاضر امکان ثبت سفارش در شهر انتخابی شما وجود ندارد');
                    //    }

                    //    $('#tempStateName').val('');
                    //}

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve states.');
                }
            });
            $('#PreState').off('changes');
            $('#PreState').on('change', function () {
                if ($(this).val()) {
                    $('#tempStateName').val($('#PreState option:selected').text());
                    $(options.StateItem).val($(this).val()).trigger('change');

                    var latLng = getLocationByStateName($('#PreState option:selected').text());
                    //var latLng = $(this).val().split(',');
                    if (latLng)
                        setmapView(latLng.lat, latLng.lng, currentMap, 15);
                    //else
                    //    alert('محل را برروی نقشه مشخص نمایید');
                }
            });
        }

        function initCountryDropdown() {
            if ($('#Country').hasClass("select2-hidden-accessible"))
                $('#Country').select2("destroy");
            $('#Country').select2({
                placeholder: "انتخاب استان",
                allowClear: true,
                dropdownParent: $('#mapModal')
            });
            $('#Country').off('changes');
            $('#Country').on('change', onCountryChange);
            $('#PreCountry').off('changes');
            $('#PreCountry').on('change', onCountryChangePre);

            if ($('#addressType').val() == 'Sender' && senderAddress.CountryId) {
                $('#Country').val(senderAddress.CountryId).trigger('change');
            } else if ($('#addressType').val() != 'Sender' && receiverAddress.CountryId) {
                $('#Country').val(receiverAddress.CountryId).trigger('change');
            }
        }

        function initStateDropdown() {
            $('#State').select2({
                placeholder: "انتخاب شهر",
                allowClear: true,
                dropdownParent: $('#mapModal')
            });
            $('#State').change(function () {
                var stateId = $(this).val();
                var CountryId = $('#Country').val();
                //loadlocation(CountryId, stateId, 'Sender');
            });
            if ($('#addressType').val() == 'Sender' && senderAddress.StateProvinceId) {
                $('#State').val(senderAddress.CountryId).trigger('change');
            } else if ($('#addressType').val() != 'Sender' && receiverAddress.StateProvinceId) {
                $('#State').val(receiverAddress.CountryId).trigger('change');
            }
        }
        function SwitchToMapBox(isForegin) {
            if (!isForegin) {
                if ($('#newOldAddress').val() == 'newAddress') {
                    $('#AddressContent').hide(250);
                    //showMap();
                    $('#PreNewAddress').show();
                } else {
                    $('#mapBox').hide(250);
                    $('#AddressContent').show(250);
                    $('#oldAddressbox').show(250);
                    if ($('#AddressBox').val()) {
                        $("#AddressBox").empty().trigger('change');
                        if ($('#addressType').val() == 'Sender' && senderAddress.text) {
                            $('#AddressBox').append(new Option(senderAddress.text, '-2', true, true));
                            $('#_NewAddress').show(250);
                            return;
                        }
                        if ($('#addressType').val() != 'Sender' && receiverAddress.text) {
                            $('#AddressBox').append(new Option(receiverAddress.text, '-2', true, true));
                            $('#_NewAddress').show(250);
                            return;
                        }
                        $('#_NewAddress').hide(250);
                    } else {
                        $('#_NewAddress').hide(250);
                    }
                }
            } else {
                if ($('#addressType').val() != 'Sender') {
                    if ($('#newOldAddress').val() == 'newAddress') {
                        $('#mapBox').hide(250);
                        $('#AddressContent').show(250);
                        $('#_NewAddress').show(250);
                        $('#oldAddressbox').hide(250);
                        $('#_foreignCountryDiv').show(250);
                        $('#_ForeginCountryCityDiv').show(250);
                    } else {
                        $('#mapBox').hide(250);
                        $('#AddressContent').show(250);
                        $('#_NewAddress').hide(250);
                        $('#oldAddressbox').show(250);
                        $('#_foreignCountryDiv').hide(250);
                        $('#_ForeginCountryCityDiv').hide(250);
                    }
                } else {
                    if ($('#newOldAddress').val() == 'newAddress') {
                        $('#AddressContent').hide(250);
                        //showMap();
                        $('#PreNewAddress').show();
                    } else {
                        $('#mapBox').hide(250);
                        $('#AddressContent').show(250);
                        $('#oldAddressbox').show(250);
                        if ($('#AddressBox').val()) {
                            $("#AddressBox").empty().trigger('change');
                            if ($('#addressType').val() == 'Sender' && senderAddress.text) {
                                $('#AddressBox').append(new Option(senderAddress.text, '-2', true, true));
                                $('#_NewAddress').show(250);
                                return;
                            }
                            if ($('#addressType').val() != 'Sender' && receiverAddress.text) {
                                $('#AddressBox').append(new Option(receiverAddress.text, '-2', true, true));
                                $('#_NewAddress').show(250);
                                return;
                            }
                            $('#_NewAddress').hide(250);
                        } else {
                            $('#_NewAddress').hide(250);
                        }
                    }
                }
            }
        }

        function showMap() {
            setTimeout(function () {
                $('path:not(.notMap)').each(function () { $(this).hide() });
                $('.notMap').each(function () { $(this).show() });
                if(!options.IsHeavy)
                    $('#mapBox').show();
                else
                    $('#mapBox').hide();
                postMap.invalidateSize();

            },
                251);
        }
        currentMap = new createPostMap(false, false, options.IsCod);
    }
    this.construct(options);
    $('body').on('show.bs.modal', '.modal', function (e) {
        // fix the problem of ios modal form with input field
        var $this = $(this);
        if (navigator.userAgent.match(/Android/i) && $(window).width() < 767) {
            $this.find('input').blur(function () {
                $('.modal-open').removeClass('fix-modal')
            })
                .focus(function () {
                    $('.modal-open').addClass('fix-modal')
                });
        }
    });
}
function onSave() {

}