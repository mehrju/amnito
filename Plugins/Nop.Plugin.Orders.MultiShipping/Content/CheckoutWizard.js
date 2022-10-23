var checkOutModelList = [];

var reader = new FileReader();
var reader2 = new FileReader();
var reader3 = new FileReader();


function validateCurrentSetp(isNext, currentStep) {
    var msg = '';
    if (isNext) {
        if (currentStep == 0) {
            //return true;
            if (!senderAddress) {
                msg += '* ' + 'اطلاعات آدرس فرستنده به درستی وارد نشده' + '\r\n'
            }
            else {
                if (!senderAddress.CountryId)
                    msg += '* ' + 'استان مبدا را مشخص نمایید' + '\r\n'
                if (!senderAddress.StateProvinceId)
                    msg += '* ' + 'شهر مبدا را مشخص نمایید' + '\r\n'
                if (!senderAddress.Lat || !senderAddress.Lon) {
                    msg += '* ' + 'مکان فرستنده را بر روی نقشه مشخص کنید' + ' \r\n ';
                }
            }
        }
        else if (currentStep == 1) {
            var _serviceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');
            //var codId = [667, 670];
            //if (codId.indexOf(parseInt(_serviceId)) >= 0) {
            //    $('#PrinterWarpper').hide();
            //    $('#needCatonWrapper').hide();
            //    $('#HasAccessToPrinter').val('true');
            //}
            //else {
            //    $('#PrinterWarpper').show();
            //    $('#needCatonWrapper').show();
            //    $('#HasAccessToPrinter').val('');
            //}
        }
        else if (currentStep == 2) {

        }
        else if (currentStep == 3) {
            if (checkOutModelList.length == 0) {
                alert('اطلاعاتی جهت ثبت موجود نمی باشد. مجداد اقدام به ثبت اطلاعات مرسوله کنید');
                return;
            }
            if (!checkOutModelList[0].UbbraTruckType) {
                if (!$('#HasAccessToPrinter').val()) {
                    msg += 'مشخص کنید که امکان پرینت فاکتور سفارش را دارید یا خیر' + ' \r\n ';
                }
                if ($('#HasAccessToPrinter').val() == 'true' && $('#needCaton').is(':checked')) {
                    msg += ` کاربر گرامی، شما در مرحله اول درخواست کارتن و لفاف
بندی کردید بدین خاطر چاپ و الصاق فاکتور بر روی
بسته شما باید در مرکز پستی و یا دفتر نمایندگی انجام
شود لطفا گزینه "فاکتور سفارش رو برام چاپ و الصاق کنید"
را در قسمت امکان چاپ فاکتور انتخاب کنید `+ ' \r\n ';
                }
            }
            if (msg) {
                alert(msg);
                return false;
            }
            RegisterOrder();
            return false;

        }

        if (msg != '' || currentStep == 3) {
            if (currentStep != 3)
                alert(msg);
            return false;
        }
    }
    if (isNext) {
        showStep(currentStep + 2);
    }
    else
        showStep(currentStep);
    return true;
}
function RegisterOrder() {
    debugger;
    if (checkOutModelList[0].IsForegin) {
        //if (loaddedFilesCounter < 3) {
        //    alert('لطفا تصاویر مرسوله خود را بارگذاری کنید');
        //    return;
        //}
        if (checkOutModelList[0].storeLink == "" || checkOutModelList[0].storeLink == null) {
            alert('لطفا لینک فروشگاه راوارد کنید');
            return;
        }
    }

    if ($('#discountCouponCode').val()) {
        checkOutModelList[0].discountCouponCode = $('#discountCouponCode').val();
    }
    else
        checkOutModelList[0].discountCouponCode = null;

    for (var i in checkOutModelList) {

        var item = checkOutModelList[i];
        item.RequestPrintAvatar = $('#RequestPrintAvatar').is(':checked');
        if (item.UbbraTruckType) {
            item.HasAccessToPrinter = true;
        }
        else
            item.HasAccessToPrinter = $('#HasAccessToPrinter').val();
        item.hasNotifRequest = $('#hasNotifRequest').is(':checked');
    }
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
                var ssf = $(`#safeBuy`).val() == 'true' ? true : false;
                if (!ssf)
                    alert('اطلاعات شما جهت بررسی و ثبت به سامانه ارسال شد. به صفحه مشاهده صورت حساب و پرداخت هدایت خواهید شد');

                window.location = '/order/Sh_billpayment?orderIds[0]=' + result.orderIds + '&safeOrder=' + ssf;
            }
            else {
                alert(result.message);
                //window.location = '/';
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('کاربر گرامی در زمان ثبت سفارش شما اشکالی به وجود آمده، لطفا ارتباط اینترنتی دستگاه خود را بررسی کنید');
        }
    });
}
function showStep(stepId) {

    for (var i = 1; i <= 4; i++) {
        var stepClass = '.steps' + i.toString();
        if (i == stepId)
            $(stepClass).show();
        else
            $(stepClass).hide();
    }
}
function showTooltipmessage(name) {
    if (name == 'postalBans') {
        $('#modal_postalBans').appendTo("body");
        $('#modal_postalBans').modal('show', { backdrop: 'static' });
    }
    else if (name == 'weightBans') {
        var msg = 'پست بار: 30 کیلو گرم در هر بسته';
        msg += '\r\n' + 'شرکت های پستی خصوصی: 100 کیلو گرم در هر بسته';
        msg += '\r\n' + 'پست خارجی: 30 کیلو گرم در هر بسته';
        msg += '\r\n' + 'سرویس درون شهری: 20 کیلو گرم در هر بسته';
        msg += '\r\n' + 'حمل و نقل سنگین:حداکثر 20 تن';
        alert(msg);
    }
    else if (name == 'dimensionsBans') {
        var msg = 'پست بار: 100 سانتیمتر*100 سانتیمتر*100 سانتیمتر';
        msg += '\r\n' + 'سرویس پست خصوصی: 300 سانتیمتر*300 سانتیمتر*300 سانتیمتر';
        msg += '\r\n' + 'سرویس درون شهری: 70 سانتیمتر*70 سانتیمتر*70 سانتیمتر';
        msg += '\r\n' + 'سرویس بین الملی: 100 سانتیمتر*100 سانتیمتر*100 سانتیمتر';
        alert(msg);
    }
}

function detrminBoxDimantion() {
    var value = $('#KartonLafaf').val();

    if (parseInt($('#Weight_g').val()) > 2000 && value.indexOf('پاکت') >= 0) {
        $('#KartonLafaf').val('0').trigger('change');
        alert('در مرسوله های با وزن بیشتر از 2000 گرم امکان انتخاب پاکت وجود ندارد و می بایستی از کارتن پستی مناسب استفاده شود');
        return;
    }

    if (value == '0')
        return;

    if (value.indexOf('A') >= 0)
        $('#boxType').val('پاکت');
    else
        $('#boxType').val('بسته');

    if (value.indexOf('نیاز') <= 0) {
        if (value.indexOf('سایر') >= 0) {
            $('#dimenationsBox').show();
            $('#length').val('');
            $('#width').val('')
            $('#height').val('');
        }
        else {
            $('#dimenationsBox').hide();
            var daimantion = value.split('(')[1].replace(')', '').split('*');
            $('#length').val(daimantion[0])
            $('#width').val(daimantion[1])
            if (daimantion.length > 2)
                $('#height').val(daimantion[2])
            else {
                $('#height').val("2");
            }
        }
    }
}

function getServices(isCod, options) {
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
        data: { "countryId": senderAddress.CountryId, "stateId": senderAddress.StateProvinceId },
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
    var model = {
        'senderCountry': senderAddress.CountryId
        , 'senderState': senderAddress.StateProvinceId
        , 'receiverCountry': receiverAddress.CountryId
        , 'receiverState': receiverAddress.StateProvinceId
        , 'weightItem': (options.IsHeavy ? (parseInt(($('#Weight_g').val()).replace(/,/g, '')) * 1000000) : ($('#Weight_g').val()).replace(/,/g, ''))
        , 'AproximateValue': ($('#ApproximateValue').val()).replace(/,/g, '')
        , 'height': parseInt(!$('#height').val() ? '0' : $('#height').val())
        , 'width': parseInt(!$('#width').val() ? '0' : $('#width').val())
        , 'length': parseInt(!$('#length').val() ? '0' : $('#length').val())
        , 'Content': $('#GoodsType').val()
        , 'IsCod': options.IsCod
        , 'ShowPrivatePost': (isInPrivatePostArea({ lat: senderAddress.Lat, lng: senderAddress.Lon }) && isInPrivatePostArea({ lat: receiverAddress.Lat, lng: receiverAddress.Lon }))
        , 'ShowDistributer': (isInCollectorArea({ lat: senderAddress.Lat, lng: senderAddress.Lon }) && isInCollectorArea({ lat: receiverAddress.Lat, lng: receiverAddress.Lon }))
        , 'IsFromAp': false
        , 'boxType': $('#boxType').val() == 'پاکت' ? 0 : 1
        , 'SenderAddress': senderAddress
        , 'ReciverAddress': receiverAddress
    };
    if (options.IsForegin) {
        model.receiver_ForeginCountry = receiverAddress.ForeginCountryId;
        model.receiver_ForeginCountryNameEn = receiverAddress.ForeginCountryNameEn;
    }
    if (options.IsHeavy) {
        model.dispatch_date = transformNumbers.toNormal($('#dispatch_date').val());
        model.UbbraTruckType = $('#UbbraTruckType').val();
        model.VechileOptions = $('#VechileOptions').val();
        model.UbbarPackingLoad = $('#UbbarPackingLoad').val();
    }
    model.NeedCollector = $('#NeedCollector').is(':checked');
    model.NeedDistributer = $('#NeedDistributer').is(':checked');
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

                //$('#servicePayType').change(function () {
                //    if ($(this).val() == "2") {
                //        if (!options.IsHeavy && !options.IsForegin)
                //            getServices(true, options);
                //    }
                //    else {
                //        if (!options.IsHeavy && !options.IsForegin)
                //            getServices(false, options);
                //    }
                //});
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
                else if ([733].indexOf(parseInt(item.ServiceId)) >= 0) {
                    imgName = "KBK";
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
            if (data[0].messageFroShow) {
                alert(data[0].messageFroShow);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $('#tblServiceInfo').find('tbody').html('');
            alert('به دلیل قطع ارتباط دستگاه شما با سامانه، لیست سرویس ها دریافت نشد. مجددا سعی کنید');
            $('#loader').hide();

        }

    });
    return canNext;
}

function isIsland(stateId) {
    var islandId = [308, 310, 314, 2259, 2272, 2274, 2275, 2319, 2278];
    return (islandId.indexOf(parseInt(stateId)) != -1);
}

function validateParcelinfo(options) {
    var msg = '';
    if (!$("#acceptRole").is(':checked'))
        msg += '* ' + 'لطفا قبل از ادامه قوانین را بخوانید و تایید بفرمایید' + '\r\n'
    if ($('#GoodsType').val() == '')
        msg += '* ' + 'محتویات مرسوله را مشخص نمایید' + '\r\n'
    if ($('#Weight_g').val() == '' || parseInt(($('#Weight_g').val()).replace(/,/g, '')) == 0)
        msg += '* ' + 'وزن مرسوله را مشخص نمایید' + '\r\n'
    if ($('#ApproximateValue').val() == '' || parseInt(($('#ApproximateValue').val()).replace(/,/g, '')) == 0)
        msg += '* ' + 'ارزش ریالی مرسوله را مشخص نمایید' + '\r\n'
    if(!options.IsForegin && options.IsHeavy && parseInt(($('#ApproximateValue').val()).replace(/,/g, '')) > 200000000)
        msg += '* ' + 'امکان ارسال مرسوله با ارزش بیشتر 20 میلیون تومان وچود ندارد' + '\r\n'
    if (!$('#Count').val() || parseInt($('#Count').val()) <= 0)
        msg += '* ' + 'تعداد را به درستی مشخص نمایید نمایید' + '\r\n'
    if ($('#KartonLafaf').val() == 'کارتن نیاز ندارم.') {
        msg += '* ' + 'لطفا حدود ابعاد مرسوله خود را انتخاب کنید' + '\r\n'
    }
    if (!options.IsHeavy && $('#KartonLafaf').val().indexOf('سایر') >= 0) {
        if ((!$('#height').val() || parseInt($('#height').val()) <= 0)
            || (!$('#length').val() || parseInt($('#length').val()) <= 0)
            || (!$('#width').val() || parseInt($('#width').val()) <= 0)) {
            msg += '* ' + 'وارد کردن ابعاد برای سایز "سایر" الزامی می باشد' + '\r\n'
        }
        else if ((parseInt($('#height').val().replace(/,/g, '')) * parseInt($('#width').val().replace(/,/g, '')) * parseInt($('#length').val().replace(/,/g, ''))) < 85750) {
            msg += '* ' + 'حداقل سایز مجاز سایز"سایر": 70*35*35 یا 35*70*35 یا 35*35*70 می باشد' + '\r\n'
        }
    }
    if ($('#boxType').val() == '-1')
        msg += '* ' + 'نوع مرسوله را مشخص نمایید' + '\r\n'

    if (!options.IsForegin) {
        if (!receiverAddress.CountryId)
            msg += '* ' + 'استان مقصد را مشخص نمایید' + '\r\n'
        if (!receiverAddress.StateProvinceId)
            msg += '* ' + 'شهرستان مقصد را مشخص نمایید' + '\r\n'
    }
    else {
        if (!receiverAddress.ForeginCountryId) {
            msg += '* ' + 'کشور مقصد را مشخص نمایید' + '\r\n'
        }
        if (!receiverAddress.ForeginCityName) {
            msg += '* ' + 'نام شهر در کشور مقصد را مشخص نمایید' + '\r\n'
        }
        if ((!$('#height').val() || parseInt($('#height').val()) < 0)
            || (!$('#length').val() || parseInt($('#length').val()) <= 0)
            || (!$('#width').val() || parseInt($('#width').val()) <= 0)
        ) {
            msg += '* ' + 'وارد کردن ابعاد الزامی می باشد' + '\r\n'
        }

        if ($('#fileImg1').val() == '' || $('#fileImg2').val() == '' || $('#fileImg3').val() == '')
            msg += '* ' + 'لطفا تصاویر مرسوله خود را بارگزاری کنید' + '\r\n';
    }

    if (!options.IsHeavy) {
        if ((isIsland(senderAddress.StateProvinceId) || isIsland(receiverAddress.StateProvinceId)) && (parseInt(($('#Weight_g').val()).replace(/,/g, ''))) > 500) {
            msg += '* ' + 'امکان ارسال و دریافت بار به جزایر ایران با وزن بیشتر از 500 گرم وجود ندارد' + '\r\n'
        }
    }
    //if (!senderAddress.Lat || !senderAddress.Lon) {
    //    msg += '* ' + 'مکان فرستنده را بر روی نقشه مشخص کنید' + ' \r\n ';
    //}
    if ((!receiverAddress.Lat || !receiverAddress.Lon) && !options.IsForegin) {

        msg += '* ' + 'مکان گیرنده را بر روی نقشه مشخص کنید' + ' \r\n ';
    }
    if (options.IsHeavy) {
        if ($('#UbbraTruckType').val() == '0') {
            msg += '* ' + 'نوع خودرو را انتخاب نمایید' + '\r\n'
        }
        if ($('#VechileOptions').val() == '0') {
            msg += '* ' + 'قابلیت خودرو را انتخاب نمایید' + '\r\n'
        }
        if ($('#UbbarPackingLoad').val() == '0') {
            msg += '* ' + 'نوع بسته بندی بار خود را مشخص نمایید' + '\r\n'
        }
        if ($('#dispatch_date').val() == '')
            msg += '* ' + 'لطفا تاریخ و ساعت بارگیری را مشخص نمایید' + '\r\n'
    }
    return msg;
}

function DelOrderId(orderItemId) {

    //asanPardakht.application.showConfirmBox('حذف', 'آیا از حذف مرسوله اطمینان دارید؟', 'بله', 'خیر', function () {
    checkOutModelList.splice(checkOutModelList.findIndex(v => v.Id == orderItemId), 1);
    ClearOrderDataInput();
    CreateorderItemList();
    //}, function () { });

}

function ClearOrderDataInput() {
    $('#ServiceId').val('-1');
    $('#GoodsType').val('');
    $('#Weight_g').val('');
    $('#ApproximateValue').val('');
    $('#Count').val('1');
    $('#KartonLafaf').val('کارتن نیاز ندارم.').trigger('change');
    $('#Insurance').val('* انتخاب بیمه ضروری است *').trigger('change');
    $('#ReceiverStateTown').html('');
    $('#ReciverLat').val('');
    $('#ReciverLon').val();
    $('#height').val('');
    $('#length').val('');
    $('#width').val('');
    $('#dimenationsBox').hide();
    $('#needCaton').removeAttr('checked');
    ClearAddress('Reciver');
    $('#tblServiceInfo').find('tbody').html('');
};

function EditOrderId(orderItemId) {
    $('#checkOutModelItemId').val(orderItemId);
    ClearOrderDataInput();
    if (fillOrder()) {
        showStep(2);
        setActiveStep(1);
        setActivePanel(1);
        $('#sendconfirmService').show();
    }
}

function fillOrder() {
    debugger;
    var checkOutModelItem = checkOutModelList.find(x => x.Id == $('#checkOutModelItemId').val());
    if (checkOutModelItem) {

        $('#ServiceId').val(checkOutModelItem.ServiceId).trigger('change');
        $('#GoodsType').val(checkOutModelItem.GoodsType).trigger('change');
        $('#GoodsType').val(checkOutModelItem._GoodsType);
        $('#ApproximateValue').val(checkOutModelItem.ApproximateValue);
        $('#Weight_g').val(checkOutModelItem.Weight);
        $('#Count').val(checkOutModelItem.Count);
        $('#Insurance').val(checkOutModelItem.InsuranceName).trigger('change');
        $('#KartonLafaf').val(checkOutModelItem.CartonSizeName).trigger('change');


        var _dimensions = '';
        if (checkOutModelItem.height != '2')
            _dimensions = checkOutModelItem.length + '*' + checkOutModelItem.width + '*' + checkOutModelItem.height;
        else
            _dimensions = checkOutModelItem.length + '*' + checkOutModelItem.width;
        $('#KartonLafaf').val($('#KartonLafaf').find("option:contains('" + _dimensions + "')").val()).trigger('change');
        if (checkOutModelItem.CartonSizeName && checkOutModelItem.CartonSizeName != '' && checkOutModelItem.CartonSizeName != 'کارتن نیاز ندارم.') {
            $('#needCaton').attr('checked', 'checked');
        }
        $('#length').val(checkOutModelItem.length);
        $('#width').val(checkOutModelItem.width);
        $('#height').val(checkOutModelItem.height)
        $('#boxType').val(checkOutModelItem.boxType).trigger('change');
        if (checkOutModelItem.CartonSizeName.indexOf('سایر') >= 0) {
            $('#dimenationsBox').show();
        }
        else {
            $('#dimenationsBox').hide();
        }
        receiverAddress = checkOutModelItem.shippingAddressModel;
        $('#ReceiverStateTown').html(checkOutModelItem._shippingAddressModel);
        $('#SenderLat').val(senderAddress.Lat);
        $('#SenderLon').val(senderAddress.Lon);
        $('#ReciverLat').val(receiverAddress.Lat);
        $('#ReciverLon').val(receiverAddress.Lon);
        return true;
    }
    return false;
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

persian = { 0: '۰', 1: '۱', 2: '۲', 3: '۳', 4: '۴', 5: '۵', 6: '۶', 7: '۷', 8: '۸', 9: '۹' };
Arabic = { 0: '٠', 1: '١', 2: '٢', 3: '٣', 4: '٤', 5: '٥', 6: '٦', 7: '٧', 8: '٨', 9: '٩' };
function traverse(el) {
    if (el.nodeType == 3) {
        var list = el.data.match(/[0-9]/g);
        if (list != null && list.length != 0) {
            for (var i = 0; i < list.length; i++) {
                el.data = el.data.replace(list[i], persian[list[i]]);
            }
        }
    }
    for (var i = 0; i < el.childNodes.length; i++) {
        traverse(el.childNodes[i]);
    }
}

function addNeg(value, name, options) {
    if ((value.replace(/,/g, '')).length >= 9) {
        value = value.replace(/,/g, '').substring(0, 9);
    }
    var result = ToLocalInt(value);
    if (name == 'Weight_g') {
        if (!options.IsHeavy && parseInt(result.replace(/,/g, '')) > 1000) {
            //$('#boxType').val('بسته');
            //if (!IsForegin)
            //    $('#height').parent().parent().parent().show();
        }
        else if (!options.IsHeavy && parseInt(result.replace(/,/g, '')) <= 1000) {
            //$('#boxType').val('پاکت');
            //if (!IsForegin) {
            //    $('#height').parent().parent().parent().hide();
            //    //$('#height').val('');
            //    //$('#length').val('');
            //    //$('#width').val('');
            //}
        }
    }
    if (options.IsForegin) {
        if (name == 'Weight_g' && parseFloat(result.replace(/,/g, '')) > 30000) {
            alert('حداکثر وزن مجاز 30000 گرم(30 کیلو گرم) در هر بسته می باشد');
            result = 30000;
        }
        else if ((name == 'height' || name == 'width' || name == 'length') && parseFloat(result.replace(/,/g, '')) > 100) {
            alert('حداکثر ابعاد مجاز برای سرویس بین الملی: 100 سانتیمتر*100 سانتیمتر*100 سانتیمتر می باشد');
            result = 100;
        }
    }
    else if (options.IsHeavy) {
        if (name == 'Weight_g' && parseFloat(result.replace(/,/g, '')) > 20) {
            alert('حداکثر وزن مجاز برای هر خودرو حداکثر 20 تن می با شد');
            result = 20;
        }
    }
    else {

        if (name == 'Weight_g' && parseInt(result.replace(/,/g, '')) > 100000) {
            alert('حداکثر وزن مجاز 100000 گرم(100 کیلو گرم) در هر بسته می باشد');
            result = 50000;
        }
        else if ((name == 'height' || name == 'width' || name == 'length') && parseFloat(result.replace(/,/g, '')) > 150) {
            alert('حداکثر ابعاد مجاز برای سرویس پست داخلی: 150 سانتیمتر*150 سانتیمتر*150 سانتیمتر می باشد');
            result = 0;
        }
        //else if ((name == 'height' || name == 'width' || name == 'length') && $('#KartonLafaf').val().indexOf('سایر') >= 0) {
        //    if ((parseInt($('#height').replace(/,/g, '')) * parseInt($('#width').replace(/,/g, '')) * parseInt($('#length').replace(/,/g, ''))) < 85750)
        //        alert('حداقل سایز مجاز سایز"سایر": 70*35*35 یا 35*70*35 یا 35*35*70 می باشد');
        //    return '';//  result = '';
        //}
    }
    return result.toLocaleString('en-US');
}

function validate_SaveLocal(checkOutModelItemId, options) {
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

    var checkOutModelItem = checkOutModelList.find(x => x.Id == checkOutModelItemId);
    if (checkOutModelItem) {
        $.each(checkOutModelList, function () {

            if (this.Id == checkOutModelItemId) {

                this.Id = checkOutModelItemId;
                if ($('#tblServiceInfo').find('input[type=radio]:checked')) {
                    this.ServiceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');

                    var SetviceType = $('#tblServiceInfo').find('input[type=radio]:checked').parent().next()
                    var slaName = SetviceType.next();
                    var servicePrice = slaName.next();
                    this._ServiceId = SetviceType.text().trim() + ' ، ' + slaName.text().trim() + ' , ' + servicePrice.text().trim();
                }
                this.IsCod = options.IsCod
                this.GoodsType = $('#GoodsType').val();
                this._GoodsType = $('#GoodsType').val();
                this.ApproximateValue = ($('#ApproximateValue').val()).replace(/,/g, '');
                this._ApproximateValue = $('#ApproximateValue').val() + " ريال "
                this.CodGoodsPrice = ($('#CodGoodsPrice').val()).replace(/,/g, '');
                this.CodGoodsPrice = (this.CodGoodsPrice == '' ? null : this.CodGoodsPrice);
                if ($('#AgentSaleAmount').length > 0) {
                    this.AgentSaleAmount = $('#AgentSaleAmount').val().replace(/,/g, '');
                    this.AgentSaleAmount = (this.AgentSaleAmount == '' ? 0 : this.AgentSaleAmount);
                }
                this.Weight = (options.IsHeavy ? (parseInt(($('#Weight_g').val()).replace(/,/g, '')) * 1000000) : ($('#Weight_g').val()).replace(/,/g, ''));
                this._Weight = $('#Weight_g').val() + (options.IsHeavy ? " تن " : " گرم ");;
                this.Count = $('#Count').val();
                this._Count = $('#Count').val();
                this.InsuranceName = $('#Insurance').val();
                this._InsuranceName = $('#Insurance option:selected').text();
                this.CartonSizeName = ($('#needCaton').is(':checked') ? $('#KartonLafaf').val() : $($('#KartonLafaf option').first()).val());
                this._CartonSizeName = $('#KartonLafaf option:selected').text();

                //senderAddress.CountryId = $('#Sender_Country').val();
                //senderAddress.StateProvinceId = $('#Sender_State').val();
                this.SenderLat = senderAddress.Lat;
                this.SenderLon = senderAddress.Lon;
                this.billingAddressModel = senderAddress;
                this.isInCityArea = senderAddress.isInCityArea;
                this.trafficArea = (senderAddress.trafficArea ? senderAddress.trafficArea : false);
                this.tehranCityArea = senderAddress.tehranCityArea;
                this.collectorArea = senderAddress.collectorArea;
                this._billingAddressModel = senderAddress.text;

                //receiverAddress.CountryId = $('#receiver_Country').val();
                //receiverAddress.StateProvinceId = $('#receiver_State').val();
                this.ReciverLat = receiverAddress.Lat;
                this.ReciverLon = receiverAddress.Lon;
                this.shippingAddressModel = receiverAddress;
                this._shippingAddressModel = receiverAddress.text;


                this.HasAccessToPrinter = $('#HasAccessToPrinter').val();
                this._HasAccessToPrinter = $('#HasAccessToPrinter').val();
                this.hasNotifRequest = $('#hasNotifRequest').is(':checked');
                this._hasNotifRequest = $('#hasNotifRequest').is(':checked');
                this.getItNow = $('#getItNow').is(':checked');
                this._getItNow = $('#getItNow').is(':checked');
                this.length = parseFloat($('#length').val() == '' ? '0' : $('#length').val());
                this.width = parseFloat($('#width').val() == '' ? '0' : $('#width').val());
                this.height = parseFloat($('#height').val() == '' ? '0' : $('#height').val());
                this._dimensions = this.length + '*' + this.width + '*' + this.height;
                this.boxType = $('#boxType').val();
                this._boxType = $('#boxType option:selected').text();
                this.RequestPrintAvatar = $('#RequestPrintAvatar').is(':checked');
                this._RequestPrintAvatar = $('#RequestPrintAvatar').is(':checked');

                if (options.IsForegin) {
                    this.receiver_ForeginCountry = receiverAddress.ForeginCountryId;// شناسه کشور درپست خارجی
                    this.receiver_ForeginCountryName = receiverAddress.ForeginCountryName;// نام کشور در پست خارجی
                    this.receiver_ForeginCityName = receiverAddress.ForeginCityName;// نام کشور در پست خارجی
                    this.receiver_ForeginCountryNameEn = receiverAddress.ForeginCountryNameEn;

                    this.imgFile1 = "";
                    this.imgFile2 = "";
                    this.imgFile3 = "";
                    reader.readAsDataURL(fileImg1.files[0]);
                    reader.onloadend = () => {
                        var img = new Image();
                        img.src = reader.result;
                        img.onload = () => {
                            var elem = document.getElementById('canvas1');//create a canvas
                            var scaleFactor = 1024 / img.width;
                            elem.width = 1024;
                            elem.height = img.height * scaleFactor;
                            var ctx = elem.getContext('2d');
                            ctx.drawImage(img, 0, 0, elem.width, elem.height);
                            var srcEncoded = ctx.canvas.toDataURL(img, 'image/jpeg', 0);
                            this.imgFile1 = srcEncoded.replace(/^data:.+;base64,/, '');
                        }
                    };
                    reader2.readAsDataURL(fileImg2.files[0]);
                    reader2.onloadend = () => {
                        var img2 = new Image();
                        img2.src = reader2.result;
                        img2.onload = () => {
                            var elem = document.getElementById('canvas2');//create a canvas
                            var scaleFactor = 1024 / img2.width;
                            elem.width = 1024;
                            elem.height = img2.height * scaleFactor;
                            var ctx = elem.getContext('2d');
                            ctx.drawImage(img2, 0, 0, elem.width, elem.height);
                            var srcEncoded = ctx.canvas.toDataURL(img2, 'image/jpeg', 0);
                            this.imgFile2 = srcEncoded.replace(/^data:.+;base64,/, '');
                        }
                    };
                    reader3.readAsDataURL(fileImg3.files[0]);
                    reader3.onloadend = () => {
                        var img3 = new Image();
                        img3.src = reader3.result;
                        img3.onload = () => {
                            var elem = document.getElementById('canvas3');//create a canvas
                            var scaleFactor = 1024 / img3.width;
                            elem.width = 1024;
                            elem.height = img3.height * scaleFactor;
                            var ctx = elem.getContext('2d');
                            ctx.drawImage(img3, 0, 0, elem.width, elem.height);
                            var srcEncoded = ctx.canvas.toDataURL(img3, 'image/jpeg', 0);
                            this.imgFile3 = srcEncoded.replace(/^data:.+;base64,/, '');
                        }
                    };

                    this.storeLink = $('#storeLink').val();
                    this.isSecondhand = $('#isSecondhand').is(':checked');
                }
                this.IsSafeBuy = $(`#safeBuy`).val() == 'true' ? true : false;

                if (options.IsHeavy) {
                    this.UbbarPackingLoad = $('#UbbarPackingLoad').val();// نوع بسته بندی بار سنگین
                    this._UbbarPackingLoad = $('#UbbarPackingLoad option:selected').text();// نوع بسته بندی بار سنگین

                    this.UbbraTruckType = $('#UbbraTruckType').val();// نوع خودرو بار سنگین
                    this._UbbraTruckType = $('#UbbraTruckType option:selected').text();// نوع خودرو بار سنگین

                    this.VechileOptions = $('#VechileOptions').val();// قابلیت خودرو بار سنگین
                    this._VechileOptions = $('#VechileOptions option:selected').text();// قابلیت خودرو بار سنگین
                    this.dispatch_date = transformNumbers.toNormal($('#dispatch_date').val());
                }
                //this.AgentSaleAmount = $('#AgentSaleAmount').val();
                checkOutModelItem = this;
                return;
            }
        });
        return checkOutModelItem;
    }
    else {
        checkOutModelItem = {};
        checkOutModelItem.Id = $('#checkOutModelItemId').val();
        debugger;
        checkOutModelItem.ServiceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');

        var SetviceType = $('#tblServiceInfo').find('input[type=radio]:checked').parent().next()
        var slaName = SetviceType.next();
        var servicePrice = slaName.next();
        var pircetext = servicePrice.clone().find('del').remove().end().text();
        checkOutModelItem._ServiceId = SetviceType.text().trim() + ' ، ' + slaName.text().trim() + ' , ' + pircetext;

        checkOutModelItem.IsCod = options.IsCod
        checkOutModelItem.GoodsType = $('#GoodsType').val();
        checkOutModelItem._GoodsType = $('#GoodsType').val();
        checkOutModelItem.ApproximateValue = ($('#ApproximateValue').val()).replace(/,/g, '');
        checkOutModelItem._ApproximateValue = $('#ApproximateValue').val() + " ريال "

        checkOutModelItem.CodGoodsPrice = ($('#CodGoodsPrice').val()).replace(/,/g, '');
        checkOutModelItem.CodGoodsPrice = (checkOutModelItem.CodGoodsPrice == '' ? null : checkOutModelItem.CodGoodsPrice);
        if ($('#AgentSaleAmount').length > 0) {
            checkOutModelItem.AgentSaleAmount = $('#AgentSaleAmount').val().replace(/,/g, '');
            checkOutModelItem.AgentSaleAmount = (checkOutModelItem.AgentSaleAmount == '' ? 0 : checkOutModelItem.AgentSaleAmount);
        }
        checkOutModelItem.Weight = (options.IsHeavy ? (parseInt(($('#Weight_g').val()).replace(/,/g, '')) * 1000000) : ($('#Weight_g').val()).replace(/,/g, ''));
        checkOutModelItem._Weight = $('#Weight_g').val() + (options.IsHeavy ? " تن " : " گرم ");
        checkOutModelItem.Count = $('#Count').val();
        checkOutModelItem._Count = $('#Count').val();
        checkOutModelItem.InsuranceName = $('#Insurance').val();
        checkOutModelItem._InsuranceName = $('#Insurance option:selected').text();
        checkOutModelItem.CartonSizeName = ($('#needCaton').is(':checked') ? $('#KartonLafaf').val() : $($('#KartonLafaf option').first()).val());
        checkOutModelItem._CartonSizeName = $('#KartonLafaf option:selected').text();

        //senderAddress.CountryId = $('#Sender_Country').val();
        //senderAddress.StateProvinceId = $('#Sender_State').val();
        checkOutModelItem.billingAddressModel = senderAddress;
        checkOutModelItem.SenderLat = senderAddress.Lat;
        checkOutModelItem.SenderLon = senderAddress.Lon;
        checkOutModelItem._billingAddressModel = senderAddress.text;
        checkOutModelItem.isInCityArea = senderAddress.isInCityArea;
        checkOutModelItem.trafficArea = (senderAddress.trafficArea ? senderAddress.trafficArea : false);;
        checkOutModelItem.tehranCityArea = senderAddress.tehranCityArea;
        checkOutModelItem.collectorArea = senderAddress.collectorArea;

        //receiverAddress.CountryId = $('#receiver_Country').val();
        //receiverAddress.StateProvinceId = $('#receiver_State').val();
        checkOutModelItem.ReciverLat = receiverAddress.Lat;
        checkOutModelItem.ReciverLon = receiverAddress.Lon;
        checkOutModelItem.shippingAddressModel = receiverAddress;
        checkOutModelItem._shippingAddressModel = receiverAddress.text;

        checkOutModelItem.HasAccessToPrinter = $('#HasAccessToPrinter').val();
        checkOutModelItem._HasAccessToPrinter = $('#HasAccessToPrinter').val();
        checkOutModelItem.hasNotifRequest = $('#hasNotifRequest').is(':checked');
        checkOutModelItem._hasNotifRequest = $('#hasNotifRequest').is(':checked');
        checkOutModelItem.getItNow = $('#getItNow').is(':checked');
        checkOutModelItem._getItNow = $('#getItNow').is(':checked');
        checkOutModelItem.length = parseFloat($('#length').val());
        checkOutModelItem.width = parseFloat($('#width').val());
        checkOutModelItem.height = parseFloat($('#height').val());
        checkOutModelItem._dimensions = checkOutModelItem.length + '*' + checkOutModelItem.width + '*' + checkOutModelItem.height;
        checkOutModelItem.boxType = $('#boxType').val();
        checkOutModelItem._boxType = $('#boxType option:selected').text();
        checkOutModelItem.RequestPrintAvatar = $('#RequestPrintAvatar').is(':checked');
        checkOutModelItem._RequestPrintAvatar = $('#RequestPrintAvatar').is(':checked');
        if (options.IsForegin) {
            checkOutModelItem.receiver_ForeginCountry = receiverAddress.ForeginCountryId;// شناسه کشور درپست خارجی
            checkOutModelItem.receiver_ForeginCountryName = receiverAddress.ForeginCountryName;// نام کشور در پست خارجی
            checkOutModelItem.receiver_ForeginCityName = receiverAddress.ForeginCityName;// نام کشور در پست خارجی
            checkOutModelItem.receiver_ForeginCountryNameEn = receiverAddress.ForeginCountryNameEn;

            reader.readAsDataURL(fileImg1.files[0]);
            reader.onloadend = () => {
                var img4 = new Image();
                img4.src = reader.result;
                img4.onload = () => {
                    debugger;
                    var elem = document.getElementById('canvas4');//create a canvas
                    var scaleFactor = 1024 / img4.width;
                    elem.width = 1024;
                    elem.height = img4.height * scaleFactor;
                    var ctx = elem.getContext('2d');
                    ctx.drawImage(img4, 0, 0, elem.width, elem.height);
                    var srcEncoded = ctx.canvas.toDataURL(img4, 'image/jpeg', 0);
                    checkOutModelItem.imgFile1 = srcEncoded.replace(/^data:.+;base64,/, '');
                }
            };
            reader2.readAsDataURL(fileImg2.files[0]);
            reader2.onloadend = () => {
                var img5 = new Image();
                img5.src = reader2.result;
                img5.onload = () => {
                    var elem = document.getElementById('canvas5');//create a canvas
                    var scaleFactor = 1024 / img5.width;
                    elem.width = 1024;
                    elem.height = img5.height * scaleFactor;
                    var ctx = elem.getContext('2d');
                    ctx.drawImage(img5, 0, 0, elem.width, elem.height);
                    var srcEncoded = ctx.canvas.toDataURL(img5, 'image/jpeg', 0);
                    checkOutModelItem.imgFile2 = srcEncoded.replace(/^data:.+;base64,/, '');
                }
            };
            reader3.readAsDataURL(fileImg3.files[0]);
            reader3.onloadend = () => {
                var img6 = new Image();
                img6.src = reader3.result;
                img6.onload = () => {
                    var elem = document.getElementById('canvas6');//create a canvas
                    var scaleFactor = 1024 / img6.width;
                    elem.width = 1024;
                    elem.height = img6.height * scaleFactor;
                    var ctx = elem.getContext('2d');
                    ctx.drawImage(img6, 0, 0, elem.width, elem.height);
                    var srcEncoded = ctx.canvas.toDataURL(img6, 'image/jpeg', 0);
                    checkOutModelItem.imgFile3 = srcEncoded.replace(/^data:.+;base64,/, '');
                }
            };

            checkOutModelItem.storeLink = $('#storeLink').val();
            checkOutModelItem.isSecondhand = $('#isSecondhand').is(':checked');
        }

        checkOutModelItem.IsSafeBuy = $(`#safeBuy`).val() == 'true' ? true : false;

        if ($('#AgentSaleAmount').length > 0) {
            checkOutModelItem.AgentSaleAmount = $('#AgentSaleAmount').val().replace(/,/g, '');
            checkOutModelItem.AgentSaleAmount = (checkOutModelItem.AgentSaleAmount == '' ? 0 : checkOutModelItem.AgentSaleAmount);
        }
        if (options.IsHeavy) {
            checkOutModelItem.UbbarPackingLoad = $('#UbbarPackingLoad').val();// نوع بسته بندی بار سنگین
            checkOutModelItem._UbbarPackingLoad = $('#UbbarPackingLoad option:selected').text();// نوع بسته بندی بار سنگین

            checkOutModelItem.UbbraTruckType = $('#UbbraTruckType').val();// نوع خودرو بار سنگین
            checkOutModelItem._UbbraTruckType = $('#UbbraTruckType option:selected').text();// نوع خودرو بار سنگین

            checkOutModelItem.VechileOptions = $('#VechileOptions').val();// قابلیت خودرو بار سنگین
            checkOutModelItem._VechileOptions = $('#VechileOptions option:selected').text();// قابلیت خودرو بار سنگین
            checkOutModelItem.dispatch_date = transformNumbers.toNormal($('#dispatch_date').val());
        }

        checkOutModelList.push(checkOutModelItem);
        return checkOutModelItem;
    }
}
function isValidService(serviceid) {
    _canCheckoutAsCod = false;
    $.ajax({
        beforeSend: function () {
            // $('.ajax-loading-block-window').show();
        },
        complete: function () {
            // $('.ajax-loading-block-window').hide();
        },
        type: "POST",
        async: false,
        url: "/ShipitoCheckout/IsvalidService",
        data: { 'serviceId': serviceid },
        success: function (data) {
            _canCheckoutAsCod = data.success;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('به دلیل قطع ارتباط دستگاه شما با سامانه،اعتبار سنجی امکان پذیر نیست مجددا سعی کنید');
            _canCheckoutAsCod = false;
        }
    });
    return _canCheckoutAsCod;
}
var CheckoutWizard = function (options) {

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
        //=========================================================================================================================================
        $("#acceptRole").click(function () {
            if ($(this).is(':checked')) {
                $('#_RuleModal').modal('show');
            }
        });
        $("#needCaton").click(function () {
            if ($(this).is(':checked')) {
                alert("مشتری محترم کارتن و لفاف بندی مرسوله شما در محل دفتر نمایندگی انجام می پذیرد ");
            }
            else {
                alert("مشتری محترم جعبه استاندارد پستی میبایستی کارتنی ۵ لایه باشد و از قرار دادن مرسوله در داخل کیسه پلاستیکی یا گونی یا هر بسته بندی غیر از کارتن در بسته و چسب کاری شده پرهیز نمایید");
            }
        });
        $("#HasAccessToPrinter").change(function () {

            var str = $('#HasAccessToPrinter').val();
            if (str == "true") {
                alert("مشتری محترم چنانچه به پرینتر دسترسی دارید میبایستی در انتهای سفارش فاکتور - بارکد تولید شده را دانلود و سپس پرینت و به درستی روی جعبه به نحوی بچسبانید که انتهای فرایند از روی جعبه جدا نشود");
            }
            if (str == "false") {
                alert("مشتری محترم چنانچه به پرینتر دسترسی ندارید میبایستی آدرس فرستنده و گیرنده و شماره سفارش را بروی جعبه پستی خود مطابق با آدرسهای مندرج در سفارش درج نمایید.");
            }

        });
        $('#checkOutModelItemId').val(Math.floor((Math.random() * 500) + 1));

        document.getElementById('ApproximateValue').addEventListener('input', event =>
            event.target.value = addNeg(event.target.value, 'ApproximateValue', options)
        );
        document.getElementById('Weight_g').addEventListener('input', function (event) {
            event.target.value = addNeg(event.target.value, 'Weight_g', options);
            if (options.IsHeavy == true) {
                var weight = parseInt(event.target.value);
                $('#UbbraTruckType').find('option').each(function () {
                    if ($(this).val() == 0)
                        return;
                    if (parseInt($(this).attr('data-MaxWeight')) < weight) {
                        $(this).attr('disabled', 'disabled');
                    }
                    else
                        $(this).removeAttr('disabled');
                });
                //$('#UbbraTruckType').select2('destroy');
                //$('#UbbraTruckType').select2();
            }
        });
        document.getElementById('ZipPostalCode').addEventListener('input', event =>
            event.target.value = ToJsutNumber(event.target.value)
        );
        document.getElementById('PhoneNumber').addEventListener('input', event =>
            event.target.value = ToJsutNumber(event.target.value)
        );
        if (!options.IsHeavy) {
            document.getElementById('height').addEventListener('input', event =>
                event.target.value = addNeg(event.target.value, 'height', options)
            );

            document.getElementById('width').addEventListener('input', event =>
                event.target.value = addNeg(event.target.value, 'width', options)
            );

            document.getElementById('length').addEventListener('input', event =>
                event.target.value = addNeg(event.target.value, 'length', options)
            );

        }
        if ($($('#Insurance').find('option')).length < 2) {
            $.ajax({
                cache: true,
                type: "GET",
                url: "/ShipitoCheckout/getInsuranceItems",
                data: {},
                success: function (data) {
                    $('#Insurance').html('');
                    $.each(data, function (id, item) {
                        $('#Insurance').append(new Option(item.Text, item.Value, false, false));
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve InsuranceItems.');
                }
            });
        }
        if ($($('#KartonLafaf').find('option')).length < 2) {
            $.ajax({
                cache: true,
                type: "GET",
                url: "/ShipitoCheckout/getKartonItems",
                data: {},
                success: function (data) {
                    $('#KartonLafaf').html('');
                    $.each(data, function (id, item) {
                        item.Text = item.Text.indexOf('نیاز') > 0 ? 'انتخاب کنید' : item.Text;
                        $('#KartonLafaf').append(new Option(item.Text, item.Value, false, false));
                    });
                    $('#KartonLafaf').change(function () {
                        detrminBoxDimantion();
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('خطا در زمان بارگذاری ابعاد بسته ها.');
                }
            });
        }
        $('#chooseService').click(function () {
            debugger;
            var msg = validateParcelinfo(options);
            if (msg != '') {
                alert(msg);
                return false;
            }

            if (options.IsHeavy || options.IsForegin)
                getServices(null, options);
            else
                getServices(false, options);
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

            $('#sendconfirmService').click();
        });
        $('#sendconfirmService').click(function () {
            var msg = validateParcelinfo(options);
            if (msg != '') {
                alert(msg);
                return false;
            }
            var gatwayId = [667, 670, 722, 723];

            var _serviceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');
            if (_serviceId) {
                if (checkOutModelList.length > 0 && checkOutModelList.findIndex(v => v.ServiceId != _serviceId) >= 0) {
                    alert('در حال حاضر در یک سفارش فقط می توانید از یک نوع سرویس پستی استفاده کنید');
                    return false;
                }
            }
            //if ($('#needCaton').is(':checked') && gatwayId.indexOf(parseInt(_serviceId)) >= 0) {
            //    msg += '* ' + 'در سرویس انتخابی نمی توانید درخواست کارتن و لفاف داشته باشید' + '\r\n';
            //}
            if (!isValidService($('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val'))) {
                msg += '* ' + 'امکان ثبت سفارش در سرویس انتخابی برای شما فعال نیست، جهت استفاده از این سرویس با پشتیبانی سامانه {09422020249} تماس بگیرید' + '\r\n';
            }
            if (msg != '') {
                alert(msg);
                return false;
            }
            //if (gatwayId.indexOf(parseInt(_serviceId)) >= 0) {
            //    $('#PrinterWarpper').hide();
            //    //$('#needCatonWrapper').hide();
            //    //$('#BoxNo').removeAttr('checked');
            //    //$('#BoxYes').attr('checked', true);
            //    $('#HasAccessToPrinter').val('true');
            //}
            //else {
            //    $('#PrinterWarpper').show();
            //    //$('#needCatonWrapper').show();
            //    //$('#BoxNo').attr('checked', true);
            //    //$('#BoxYes').removeAttr('checked');
            //    $('#HasAccessToPrinter').val('');
            //}
            if ($('#tblServiceInfo').find('input[type=radio]:checked').attr('data-IsCod') == 'true') {
                if (confirm('آیا مبلغ کالای شما باید از گیرنده گرفته شود؟')) {
                    $('#CodGoodsPrice').val($('#ApproximateValue').val());

                }
                else {
                    $('#CodGoodsPrice').val('');
                }
            }
            validate_SaveLocal($('#checkOutModelItemId').val(), options);
            if (CreateorderItemList()) {
                $('#sendconfirmService').hide();
            }
            $('#servicesModal').modal('hide');
        });
        $('#AddNewParcell').click(function () {
            $('#checkOutModelItemId').val(Math.floor((Math.random() * 500) + 1));
            ClearOrderDataInput();
            showStep(2)
            setActiveStep(1);
            setActivePanel(1);
        });

        //====================================Ubbar truckType & Options==========================================================
        if (options.IsHeavy) {
            $.ajax({
                cache: true,
                type: "GET",
                url: "/ShipitoCheckout/getUbbarTruckType",
                data: {},
                success: function (data) {
                    $('#UbbraTruckType').append(new Option('انتخاب کنید....', '0', true, true));
                    $.each(data, function (id, item) {
                        var option = new Option(item.Text, item.Value.split('|')[0], false, false);
                        $(option).attr('data-MaxWeight', item.Value.split('|')[1]);
                        $('#UbbraTruckType').append(option);
                    });

                    // $('#UbbraTruckType').select2();
                    $('#UbbraTruckType').change(function () {

                        var selectedItem = $(this).val();
                        var vechileOptions = $('#VechileOptions');
                        vechileOptions.html('');
                        vechileOptions.append(new Option('درحال بارگذاری....', '-1', true, true));
                        $.ajax({
                            cache: false,
                            type: "GET",
                            url: "/ShipitoCheckout/getUbbarVechileOption",
                            data: { "TruckType": selectedItem },
                            success: function (reuslt) {
                                vechileOptions.html('');

                                vechileOptions.append(new Option('انتخاب کنید....', '0', true, true));
                                $.each(reuslt, function (id, item) {
                                    vechileOptions.append(new Option(item.Text, item.Value, false, false));
                                });
                                // vechileOptions.select2();
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                console.log('Failed to retrieve vechileOptions.');
                            }
                        });
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve Countries.');
                }
            });
            $('#enter_date').MdPersianDateTimePicker({
                targetTextSelector: '#dispatch_date',
                targetDateSelector: '#enter_date_2',
                fromDate: true,
                groupId: 'enter_date',
                modalMode: true,
                disableBeforeToday: true,
                enableTimePicker: true,
                textFormat: 'yyyy/MM/dd HH:mm',
            });
        }
        //====================================Ubbar truckType & Options==========================================================
        if (options.IsForegin) {
            $.ajax({
                cache: true,
                type: "GET",
                url: "/ShipitoCheckout/getForeginCOuntry",
                data: {},
                success: function (data) {
                    $('#receiver_ForeginCountry').append(new Option('انتخاب کنید....', '0', true, true));
                    $.each(data, function (id, item) {
                        $('#receiver_ForeginCountry').append(new Option(item.Text, item.Value, false, false));
                    });

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve ForeginCOuntry.');
                }
            });
        }
        $(".select2-container").tooltip({
            title: function () {
                return $(this).prev().attr("title");
            },
            placement: "top",
            html: true,
            trigger: 'focus'
        });
        $(".myTooltip").tooltip({
            placement: "top",
            html: true,
            trigger: 'focus'
        });
        $(".hoverTooltip").tooltip({
            placement: "top",
            html: true,
            trigger: 'hover'
        });
    }
    this.construct(options);
};