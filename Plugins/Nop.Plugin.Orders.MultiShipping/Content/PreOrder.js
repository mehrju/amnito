var checkOutModelList = [];
var senderAddress = {};
var receiverAddress = {};

var PreOrder = function (Model) {

    function getServices(Model) {
        debugger;
        var canNext = false;
        var isInvalidSender = false;
        $.ajax({
            beforeSend: function () {
                $('#loader').show();
            },
            complete: function () {
                $('#loader').hide();
            },
            type: "GET",
            async: false,
            url: "/ShipitoCheckout/IsInInvalidService",
            data: { "countryId": Model.SenderAddress.StateId, "stateId": Model.SenderAddress.CityId },
            success: function (data) {
                if (data.isInvalid == true) {
                    alert('در حال حاضر امکان ثبت سفارش از مبدا مورد نظر وجود ندارد');
                    isInvalidSender = true;
                    return;
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
        if (isInvalidSender) {
            return;
        }
        var SenderAddressModel = {
            'Lat': Model.SenderAddress.Latitude,
            'Lon': Model.SenderAddress.Longitude,
            'text': '@Html.Raw(Model.SenderAddress.Address)'
        }
        var ReciverAddressModel = {
            'Lat': Model.ParcellList[0].ReceiverAddress.Latitude,
            'Lon': Model.ParcellList[0].ReceiverAddress.Longitude,
            'text': '@Html.Raw(Model.ParcellList[0].ReceiverAddress.Address)'
        }
        var model = {
            'senderCountry': Model.SenderAddress.StateId
            , 'senderState': Model.SenderAddress.CityId
            , 'receiverCountry': Model.ParcellList[0].ReceiverAddress.StateId
            , 'receiverState': Model.ParcellList[0].ReceiverAddress.CityId
            , 'weightItem': Model.ParcellList[0].ReceiverAddress.StateId
            , 'AproximateValue': Model.ParcellList[0].ContentValuePrice
            , 'height': Model.ParcellList[0].Height
            , 'width': Model.ParcellList[0].Width
            , 'length': Model.ParcellList[0].Lenght
            , 'Content': '@Html.Raw(Model.ParcellList[0].Content)'
            , 'SenderAddress': SenderAddressModel
            , 'ReciverAddress': ReciverAddressModel,
            'ShowPrivatePost': false,
            'ShowDistributer': false,
            'IsFromAp': false,
            'boxType': 0,
            'IsCod': false
        };
        _model = JSON.stringify(model);
        $.ajax({
            beforeSend: function () {
                $('#loader').show();
            },
            complete: function () {
                $('#loader').hide();
            },
            type: "POST",
            //async: false,
            url: "/ShipitoCheckout/getServicesInfo",
            data: { "_model": _model },
            success: function (data) {
                var tbody = $('#tblServiceInfo').find('tbody');

                if ($(data).length == 0) {
                    $(tbody).html('');
                    alert('با توجه به مقادیر ورودی شما در قسمت وزن،استان و شهرستان فرستنده و گیرنده سرویسی یافت نشد');
                    $('#loader').hide();
                    canNext = false;
                    return false;
                }

                if ($('#serviceSort').length == 0) {
                    tbody.html('');
                    tbody.append(`<tr class="trHeader">
                                        <td colspan="4" class="selectServiceLeft">
                                            <select class="form-control" id="serviceSort" style="font-size: 9pt !important;" title="مرتب سازی سرویس ها">
                                                <option value="1" selected="true">ارزان ترین </option>
                                                <option value="2">سریع ترین </option>
                                            </select></td>
                                    </tr>`);

                    $('#serviceSort').change(function () {
                        var table = $('#tblServiceInfo');
                        var colIndex = 1;
                        var rows = [];
                        var tbody = table.find('tbody');
                        var header = null;
                        if (tbody) {

                            var i = 0
                            $(tbody).find('tr').each(function () {
                                if (i != 0) {
                                    rows.push($(this).clone());
                                    $(this).remove();
                                }
                                i++;
                            });

                            if ($(this).val() == '1') {
                                rows.sort(function (row1, row2) {

                                    return parseInt($(row1).find('input[type="radio"]').attr('data-Price')) - parseInt($(row2).find('input[type="radio"]').attr('data-Price'));
                                });
                            }
                            else {
                                rows.sort(function (row1, row2) {

                                    return parseInt($(row1).find('input[type="radio"]').attr('data-SLA')) - parseInt($(row2).find('input[type="radio"]').attr('data-SLA'));
                                });
                            }
                            if (rows) {
                                $(rows).each(function () {
                                    $(tbody).append($(this));
                                });

                            }
                        }
                    });
                }
                else {
                    tbody.find('tr.data_item').each(function () { $(this).remove(); })
                }
                $.each(data, function (id, item) {
                    var imgName = '';
                    if ([654, 655, 667, 670, 662, 661, 660, 698, 697, 696, 695, 694, 693, 691, 690, 722, 723].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "POSTBAR";
                    }

                    else if ([725, 726, 727].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "shipito";
                    }
                    else if ([703, 699, 705, 706].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "DTS";
                    }
                    else if ([702].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "YARBOX";
                    }
                    else if ([701].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "UBAAR";
                    }
                    else if ([707, 708].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "PDE";
                    }
                    else if ([709, 710, 711].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "TPG";
                    }
                    else if ([712, 713, 714, 715].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "CHAPAR";
                    }
                    else if ([717].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "SNAPBOX";
                    }
                    else if ([718].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "POSTEXPLUS";
                    }
                    else if ([719].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "BLUESKY";
                    }
                    else if ([730, 731].indexOf(parseInt(item.ServiceId)) >= 0) {
                        imgName = "Mahex";
                    }
                    if (item.CanSelect) {
                        tbody.append(`<tr class="data_item">
                                        <td class="tdMinHeader">
                                            <input type="radio"  data-val="${item.ServiceId}" data-IsCod="${item.IsCod}" data-price="${item.Price}" data-SLA="${item.SLA.split('|')[1]}" name="radioGroup">
                                            <span>${item.ServiceName}</span>
                                            <img src="../ImageServiceProviderDashboard/${imgName}.png"/>
                                        </td>
    
                                        <td class="tdMaxHeader">
                                            ${item.ServiceName}
                                        </td>
                                        <td class="selectService selectServiceRight">${item.SLA.split('|')[0]}</td>
                                        <td class="selectService selectServiceLeft ${item.message ? `serviceDiscount` : ``}">${item.message ? `<del><span class="amount">${item.message}</span></del>` : ``}  ${item._Price} ريال</td>
                                    </tr>`);
                    }
                    else {
                        tbody.append(`<tr class="data_item">
                                        <td style="vertical-align: middle;"><input type="radio" style="display:none" data-val="${item.ServiceId}" data-IsCod="${item.IsCod}" data-price="${10000000000}" data-SLA="${50}" name="radioGroup">
                                            <img src="../ImageServiceProviderDashboard/${imgName}.png"/>
                                        </td>
                                        <td style="vertical-align: middle;">
                                            ${item.ServiceName}
                                        </td>
                                        <td style="vertical-align: middle;">-</td>
                                        <td style="vertical-align: middle;">${item._Price}</td>
                                    </tr>`);
                    }

                });
                $('#tblServiceInfo').parent().css('height', $('#tblServiceInfo').css('width') + 75);
                $('#servicesModal').appendTo("body");
                $('#servicesModal').modal('show', { backdrop: 'static' });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('به دلیل قطع ارتباط دستگاه شما با سامانه، لیست سرویس ها دریافت نشد. مجددا سعی کنید');
                $('#loader').hide();

            }

        });
        return canNext;
    }

    function validate_SaveLocal(checkOutModelItemId, Model) {
        var checkOutModelItem = {};
        var msg = '';
        if (!senderAddress) {
            msg = 'آدرس فرستنده به درستی وارد نشده' + ' \r\n ';
        }
        if (!receiverAddress) {
            msg = 'آدرس گیرنده به درستی وارد نشده' + ' \r\n ';
        }
        if (msg) {
            alert(msg);
            return;
        }
        checkOutModelItem = {};
        checkOutModelItem.Id = $('#checkOutModelItemId').val();
        //
        checkOutModelItem.ServiceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');

        var ServiceType = $('#tblServiceInfo').find('input[type=radio]:checked').parent().next()
        var slaName = ServiceType.next();
        var servicePrice = slaName.next();
        var pircetext = servicePrice.clone().find('del').remove().end().text();
        checkOutModelItem._ServiceId = ServiceType.text().trim() + ' ، ' + slaName.text().trim() + ' , ' + pircetext;

        checkOutModelItem.Weight = Model.ParcellList[0].Weight;
        checkOutModelItem._Weight = Model.ParcellList[0].Weight;
        checkOutModelItem.Count = 1;
        checkOutModelItem._Count = 1;
        checkOutModelItem.InsuranceName = Model.ParcellList[0].InsuranceTitle;
        checkOutModelItem._InsuranceName = Model.ParcellList[0].InsuranceTitle;
        //checkOutModelItem.CartonSizeName = ($('#needCaton').is(':checked') ? $('#KartonLafaf').val() : $($('#KartonLafaf option')[0]).val());
        //checkOutModelItem._CartonSizeName = $('#KartonLafaf option:selected').text();
        checkOutModelItem.ApproximateValue = Model.ParcellList[0].ContentValuePrice;
        checkOutModelItem._ApproximateValue = Model.ParcellList[0].ContentValuePrice + " ريال ";

        checkOutModelItem.SenderLat = senderAddress.Lat;
        checkOutModelItem.SenderLon = senderAddress.Lon;
        checkOutModelItem.billingAddressModel = senderAddress;
        checkOutModelItem._billingAddressModel = senderAddress.text;

        checkOutModelItem.ReciverLat = receiverAddress.Lat;
        checkOutModelItem.ReciverLon = receiverAddress.Lon;
        checkOutModelItem.shippingAddressModel = receiverAddress;
        checkOutModelItem._shippingAddressModel = receiverAddress.text;

        checkOutModelItem.HasAccessToPrinter = Model.ParcellList[0].HasAccessToPrinter;
        checkOutModelItem._HasAccessToPrinter = Model.ParcellList[0].HasAccessToPrinter;
        checkOutModelItem.hasNotifRequest = Model.ParcellList[0].SMSNotification;
        checkOutModelItem._hasNotifRequest = Model.ParcellList[0].SMSNotification;
        checkOutModelItem.length = Model.ParcellList[0].Lenght;
        checkOutModelItem.width = Model.ParcellList[0].Width;
        checkOutModelItem.height = Model.ParcellList[0].Height;
        checkOutModelItem._dimensions = checkOutModelItem.length + '*' + checkOutModelItem.width + '*' + checkOutModelItem.height;
        checkOutModelItem.boxType = 0;
        checkOutModelItem._boxType = 0;
        checkOutModelItem.RequestPrintAvatar = Model.ParcellList[0].PrintCommercialLogo;
        checkOutModelItem._RequestPrintAvatar = Model.ParcellList[0].PrintCommercialLogo;

        checkOutModelList.push(checkOutModelItem);
        return checkOutModelItem;
    }

    function CreateorderItemList() {
        var $OrderList = $('#OrdersList');
        $OrderList.html('');
        if (!checkOutModelList || !checkOutModelList.length || checkOutModelList.length == 0) {
            return false;
        }
        var accordion = $(` <div class="col-md-12" id="OrderList_accordion" style="margin-top: 10px;padding: 7px;"></div>`);
        for (var i in checkOutModelList) {
            var item = checkOutModelList[i];
            var card = $(`<div class="card myCard">
                                    <div class="card-header">
                                        مرسوله شماره ${(parseInt(i) + 1)}
                                        <a class="orderListItem-headerBtn" data-toggle="collapse" data-parent="#OrderList_accordion" href="#collapse${(parseInt(i) + 1)}" style="color:#707070;float: left;">
                                            <i class="fas fa-chevron-right" aria-hidden="true" style="top:2px;float:left"></i>
                                        </a>
                                        <a class="orderListItem-headerBtn" class="ActBtn" data-val="2" tabindex="-1" style="float: left;color:#F2432E;"><i class="fa fa-trash" onClick="DelOrderId('${item.Id}')"></i></a>
                                        <a class="orderListItem-headerBtn" class="ActBtn" data-val="2" tabindex="-1" style="float: left;color:#0F9400;"><i class="fa fa-edit faa-flash" onClick="EditOrderId('${item.Id}')"></i></a>
                                    </div>
                                    <div id="collapse${(parseInt(i) + 1)}" class="collapse">
                                        <div class="card-body pad-0">
                                            <div class="container pad-0">
                                                <div class="row">
                                                    <div class="box" style="margin-bottom:0px !important">
                                                        <div class="body_res">
                                                            <div class="row">
                                                                <div class="col-md-3 col-xl-3 col-lg-3 col-sm-12">
                                                                    <label >نوع کالا: </label><span class="reViewData" id="_GoodsType">${item._GoodsType}</span>
                                                                </div>
                                                                <div class="col-md-3 col-xl-3 col-lg-3 col-sm-12">
                                                                    <label>قیمت حدودی کالا: </label><span class="reViewData" id="_ApproximateValue">${item._ApproximateValue}</span>
                                                                </div>
                                                                <div class="col-md-3 col-xl-3 col-lg-3 col-sm-12">
                                                                    <label>وزن : </label><span class="reViewData" id="_Weight">${item._Weight}</span>
                                                                </div>
                                                                <div class="col-md-3 col-xl-3 col-lg-3 col-sm-12">
                                                                    <label>تعداد : </label><span class="reViewData" id="_Count">${item._Count}</span>
                                                                </div>
                                                                    <div class="col-md-3 col-xl-3 col-lg-3 col-sm-12">
                                                                        <label>ابعاد: </label><span class="reViewData" id="_dimensions">${item._dimensions}</span>
                                                                    </div>
                                                                    <div class="col-md-3 col-xl-3 col-lg-3 col-sm-12">
                                                                        <label>نوع بسته بندی: </label><span class="reViewData" id="_boxType">${item._boxType}</span>
                                                                    </div>
                                                            </div>
                                                                <div class="row">
                                                                    <div class="col-md-9 col-xl-9 col-lg-9 col-sm-12">
                                                                        <div class="address-Sender">
                                                                            <label>آدرس فرستنده:</label><span class="reViewData" id="_billingAddressModel">${item._billingAddressModel}</span>
                                                                        </div>
                                                                        <div class="addres_Receiver">
                                                                            <label>آدرس گیرنده:</label><span class="reViewData" id="_shippingAddressModel">${item._shippingAddressModel}</span>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-md-9 col-xl-9 col-lg-9 col-sm-12">
                                                                        <label>نوع سرویس:</label><span class="reViewData" id="_ServiceId">${item._ServiceId}</span>
                                                                    </div>
                                                                </div>
                                                                    <div class="row">
                                                                        <div class="col-md-6 col-xl-6 col-lg-6 col-sm-12">
                                                                            <label>بیمه :</label><span class="reViewData" id="_InsuranceName">${item._InsuranceName}</span>
                                                                        </div>
                                                                        <div class="col-md-6 col-xl-6 col-lg-6 col-sm-12">
                                                                            <label>جعبه و بسته بندی :</label><span class="reViewData" id="_CartonSizeName">${item._CartonSizeName}</span>
                                                                        </div>
                                                                    </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`);
            accordion.append(card);
        }
        $OrderList.append(accordion);
        return true;
    }
    function RegisterOrder() {
        checkOutModelList[0].discountCouponCode = null;
        checkOutModelList[0].IsFromFava = true;
        var sendData = JSON.stringify(checkOutModelList);
        $.ajax({
            beforeSend: function () {
                $('#loader').show();
            },
            complete: function () {
                $('#loader').hide();
            },
            type: "POST",
            url: "/ShipitoCheckout/SaveNewCheckOutOrder",
            data: { "JsonCheckoutModel": sendData },
            success: function (result) {
                debugger;
                if (result.success == true) {
                    var ssf = false;
                    if (!ssf)
                        alert('اطلاعات شما جهت بررسی و ثبت به سامانه ارسال شد. به صفحه مشاهده صورت حساب و پرداخت هدایت خواهید شد');

                    window.location = '/order/Sh_billpayment?orderIds[0]=' + result.orderIds + '&safeOrder=' + ssf;
                }
                else {
                    alert(result.message);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('کاربر گرامی در زمان ثبت سفارش شما اشکالی به وجود آمده، لطفا ارتباط اینترنتی دستگاه خود را بررسی کنید');
            }
        });
    }

    this.construct = function (Model) {
        $.extend({}, Model);
        senderAddress = {
            FirstName: Model.SenderAddress.FirstName,
            LastName: Model.SenderAddress.LastName,
            PhoneNumber: Model.SenderAddress.PhoneNumber,
            Address1: Model.SenderAddress.Address,
            Lat: Model.SenderAddress.Latitude,
            Lon: Model.SenderAddress.Longitude,
            CountryId: Model.SenderAddress.StateId,
            StateProvinceId: Model.SenderAddress.CityId
        };
        receiverAddress = {
            FirstName: Model.ParcellList[0].ReceiverAddress.FirstName,
            LastName: Model.ParcellList[0].ReceiverAddress.LastName,
            PhoneNumber: Model.ParcellList[0].ReceiverAddress.PhoneNumber,
            Address1: Model.ParcellList[0].ReceiverAddress.Address,
            Lat: Model.ParcellList[0].ReceiverAddress.Latitude,
            Lon: Model.ParcellList[0].ReceiverAddress.Longitude,
            CountryId: Model.ParcellList[0].ReceiverAddress.StateId,
            StateProvinceId: Model.ParcellList[0].ReceiverAddress.CityId
        };
        //=========================================================================================================================================
        $('#checkOutModelItemId').val(Math.floor((Math.random() * 500) + 1));

        $('#chooseService').click(function () {
            getServices(Model);
        });

        $('#cancelService').click(function () {
            $('#servicesModal').modal('hide');
        });
        $('#confirmService').click(function () {
            var _serviceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');
            if (!_serviceId) {
                alert('لطفا نوع سرویس مورد نظر را مشخص کنید');
                return false;
            }
            validate_SaveLocal($('#checkOutModelItemId').val(), Model);
            if (CreateorderItemList()) {
                RegisterOrder();
            }
            $('#servicesModal').modal('hide');
        });
    }

    this.construct(Model);
}
