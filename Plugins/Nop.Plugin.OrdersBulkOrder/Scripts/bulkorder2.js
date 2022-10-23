var trCounter = 1;
var RowGotFocused = null;
var RowLostFocused = null;

var currentRow;
var JsonEmptyMap;
var SenderAddressJson = null;

var Item1Tocopy;
var Item2Tocopy;
var Item3Tocopy;
var Item4Tocopy;

var Item1TocopyType;
var Item2TocopyType;
var Item3TocopyType;
var Item4TocopyType;

var isMouseDown = false;
var IsAgent1 = true;
var sizesShowing = false;

$(document).ready(function () {
    $.ajax({
        cache: false,
        type: "POST",
        url: "/BulkOrder/getServiceByFileType",
        data: { "FileType": "4", "ispeyk": $('#isPeyk').val() },
        success: function (data) {
            $('#ServiceId').append(new Option('انتخاب کنید....', '0', true, true));
            $.each(data, function (id, item) {
                $('#ServiceId').append(new Option(item.Text, item.Value, false, false));
            });
            $('#ServiceId').trigger('databound');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            debugger;
            alert('خطا در زمان دریافت لیست سرویس ها');
        }
    });
    $('#ServiceId').change(function () {
        if ($(this).val() > 0) {
            $.ajax({
                cache: false,
                type: "POST",
                url: "/BulkOrder/getServiceInfo",
                data: { "ServiceId": $(this).val() },
                success: function (data) {
                    if (data) {
                        debugger;
                        $('#IsCod').val(data.IsCod);
                        if (data.IsCod) {
                            $('._isCod').show();
                        }
                        else {
                            $('._isCod').hide();
                            $('input._isCod').each(function () {
                                    $(this).prop('checked', false);
                                    $('input[name="IsCod"]').val('');
                            });
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $('#IsCod').val(0);
                }
            });
        }
    });
    $(`#inpXls`).change(function () {
        debugger;

        if ($(`#inpXls`).prop('files')[0].type == 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') // xlsx
        {
            readXlsxFile($(`#inpXls`).prop('files')[0], { dateFormat: 'MM/DD/YY' }).then(function (data) {
                // `data` is an array of rows
                // each row being an array of cells.
                //document.getElementById('result').innerText = JSON.stringify(data, null, 2)

                // Applying `innerHTML` hangs the browser when there're a lot of rows/columns.
                // For example, for a file having 2000 rows and 20 columns on a modern
                // mid-tier CPU it parses the file (using a "schema") for 3 seconds
                // (blocking) with 100% single CPU core usage.
                // Then applying `innerHTML` hangs the browser.

                $('#tbody tr').each(function () {
                    $(this).remove();
                });
                var skipHeader = true;
                data.map(function (row) {
                    debugger;
                    if (skipHeader) { skipHeader = false; return; }
                    fnAddRow();
                    var lRow = $('#tbody').children().last();

                    $($($($(lRow).children()[2])).children()[0]).val(row[3]);
                    $($($($(lRow).children()[3])).children()[0]).val(row[0]);

                    var daimantion = row[0].split('(')[1].replace(')', '').split('*');
                    var KartonLafaflength = daimantion[0];
                    var KartonLafafwidth = daimantion[1];
                    var KartonLafafheight = "2";
                    if (daimantion.length > 2)
                        KartonLafafheight = daimantion[2];

                    $($($($(lRow).children()[4])).children()[0]).val(KartonLafaflength);
                    $($($($(lRow).children()[5])).children()[0]).val(KartonLafafwidth);
                    $($($($(lRow).children()[6])).children()[0]).val(KartonLafafheight);

                    if (row[2] == 'true')
                        $($($($(lRow).children()[7])).children()[0]).prop('checked', true);
                    else
                        $($($($(lRow).children()[7])).children()[0]).prop('checked', false);

                    $($($($(lRow).children()[8])).children()[0]).val(row[4]);
                    $($($($(lRow).children()[9])).children()[0]).val(row[1]);
                    $($($($(lRow).children()[10])).children()[0]).val(row[5]);
                    $($($($(lRow).children()[13])).children()[0]).val(row[25]);

                    var AddressInfo = {
                        FirstName: row[6],
                        LastName: row[7],
                        PhoneNumber: row[8],
                        Address1: row[14],
                        Country: row[9],
                        State: row[10]
                    };

                    $($($($(lRow).children()[11])).children()[0]).val(JSON.stringify(AddressInfo));
                    $($($($(lRow).children()[12])).children()[0]).val(row[14]);
                    //for (var i = 2; i < row.length; i++) {
                    //    debugger;
                    //    if (row[i] == 'true')
                    //        $($($($(lRow).children()[i])).children()[0]).prop('checked', true);
                    //    else
                    //        $($($($(lRow).children()[i])).children()[0]).val(row[i]);
                    //}
                });


            }, function (error) {
                console.error(error);
                alert("خطا در زمان خواندن اکسل ،لطفا نمونه فایل اکسل را  از سایت دانلود کرده و پر نمایید");
            });
        }
        else
            if ($(`#inpXls`).prop('files')[0].type == 'application/vnd.ms-excel') {
                $('#tbody tr').each(function () {
                    $(this).remove();
                });
                var skipHeader = true;
                var fr = new FileReader();
                fr.onload = function () {
                    $(`#divTblImportxls`).html(fr.result);
                    $(`#divTblImportxls`).find(`tbody tr`).each(function () {
                        debugger;
                        if (skipHeader) { skipHeader = false; return; }
                        fnAddRow();
                        var lRow = $('#tbody').children().last();
                        var rowItems = $(this).children();
                        for (var i = 2; i < rowItems.length; i++) {
                            debugger;
                            var CellVal = $(rowItems[i]).text();
                            if (CellVal == 'true')
                                $($($(lRow).children()[i]).children()[0]).prop('checked', true);
                            else
                                $($($(lRow).children()[i]).children()[0]).val(CellVal);
                        }
                    });
                }
                fr.readAsText(this.files[0]);
            }
        $(`#inpXls`).val("");
    });



    $("input").addClass("form-control");
    $("select").addClass("form-control");

    var options = {
        document: document,
        wizard: '#wizard',
        WeightItem: '#Weight_g',
        //InsuranceItem: '#Insurance',
        CountryItem: '#Country',
        StateItem: '#State',
        //kartonSizeItem: '#KartonLafaf',
        IsAgent: false,
        IsInCodRole: false,
        IsForegin: false,
        IsHeavy: false
    };
    var form = new newCheckout(options);

    $.ajax({
        cache: true,
        type: "GET",
        url: "/ShipitoCheckout/getInsuranceItems",
        data: {},
        success: function (data) {
            $.each(data, function (id, item) {
                $("#Insurance2").append(new Option(item.Text, item.Value, false, false));
            });
            if ($('#KartonLafaf2').html() != '')
                fnAddRow();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });
    $.ajax({
        cache: true,
        type: "GET",
        url: "/ShipitoCheckout/getKartonItems",
        data: {},
        success: function (data) {
            $.each(data, function (id, item) {
                if (item.Value != 'کارتن نیاز ندارم.')
                    $("#KartonLafaf2").append(new Option(item.Text, item.Value, false, false));
            });

            if ($('#Insurance2').html() != '')
                fnAddRow();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(thrownError);
        }
    });


    JsonEmptyMap = toJSONString();


});
function showBoxTooltipmessage(bb) {
    if (bb) {
        alert("مشتری محترم کارتن و لفاف بندی مرسوله شما در محل دفتر نمایندگی انجام می پذیرد ");
    }
    else {
        alert("مشتری محترم جعبه استاندارد پستی میبایستی کارتنی ۵ لایه باشد و از قرار دادن مرسوله در داخل کیسه پلاستیکی یا گونی یا هر بسته بندی غیر از کارتن در بسته و چسب کاری شده پرهیز نمایید");
    }
}
function fnAddRow() {
    //if ($("#SenderStateTown").text() == '') {
    //    alert("لطفا ابتدا اطلاعات فرستنده را وارد کنید");
    //    return;
    //}
    var InsuranceItems = $('#Insurance2').html();
    var KartonLafafItems = $('#KartonLafaf2').html();

    if (InsuranceItems == '' || KartonLafafItems == '') {
        alert('لطفا چند ثانیه صبر کنید تا اطلاعات بارگزاری شود');
        return;
    }
    //<td><div name='ValidationMessages'></div></td>
    var lastRow = `<tr id='tr${trCounter}'>
        <td name='RowNumber' class="RowNUmber"></td>
        <td style='width:auto'>
            <button name='btnDelete'  onclick='fnDelete(this)' title='حذف' type='button' class='btn btn-danger' style='width: 28px;padding: 0px;height: 36px;'>
                <i class='fa fa-times' aria-hidden="true" />
            </button>
            <button name='btneditRow' onclick='fnEdit(this)' title='ویرایش' type='button' class='btn btn-primary' style='display:none; width: 28px;padding: 0px;height: 36px;'>
                <i class='fa fa-pencil' aria-hidden="true" />
            </button>
        </td>
        <td>
            <input name='GoodsType' type='text' />
            <div class="clspin"></div>
        </td>
        <td><select name='KartonLafaf' onchange='fnKartonLafafChange(this)'>${KartonLafafItems}</select><div class="clspin"></div></td>
        <td name='thLength'><input name='thLength' type='text' /><div class="clspin"></div></td>
        <td name='thWidth'><input name='thWidth' type='text' /><div class="clspin"></div></td>
        <td name='thHeight'><input name='thHeight' type='text' /><div class="clspin"></div></td>
        <td><input name='needCaton' type='checkbox' /><div class="clspin"></div></td>
        <td><input name='ApproximateValue' onkeyup='separate(this)' /><div class="clspin"></div></td>
        <td class="_isCod" style="display:none"><input name='IsCod'  type='checkbox'  /><div class="clspin"></div></td>
        <td><input name='Weight_g' onkeyup='separate(this)' type='text' /><div class="clspin"></div></td>
        <td><select name='Insurance' class='rowInsurance'>${InsuranceItems}</select><div class="clspin"></div></td>
        <td name='ReceiverAddress'>
            <input name='connectionInfo' hidden  type='text' />
            <button type='button' class='btn btn-primary' onclick='fnShowReciverAddress(this)'>تخصیص آدرس</button>
            <div class="clspin"></div>
        </td>
        <td>
            <input name='connectionInfoDetail' readonly disabled='disabled' style='width:100% !important;color: black !important; border:none !important;background-color:transparent;' />
        </td>
        <td name='thValueAdded'>
            <input name='ValueAdded' type='text' number onkeyup='separate(this)' />
            <div class="clspin"></div>
        </td>
    </tr>
    `;
    $("#tbody").append(lastRow);
    $("input").addClass("form-control");
    $("select").addClass("form-control");

    InstallAutofill();
    AssignRowNumbers();

    //$(lastRow).find("input[name='connectionInfo']").bind('DOMSubtreeModified', function (e, a) {

    //});


    //**********************************************
    $(`#tr${trCounter}`).focusin(function () {
        RowGotFocused = $(this).attr('id');
        if (RowGotFocused == RowLostFocused) return;
        isValidRow(`${RowLostFocused}`);
    }
    );

    $(`#tr${trCounter}`).focusout(function () {
        RowLostFocused = $(this).attr('id');
    });

    //$('td').bind('keypress', function (event) {
    //    if (event.which === 13) {
    //        debugger;
    //        $(this).next().find('input,select').focus();
    //    }
    //});

    if (trCounter == 1 || !sizesShowing) {
        $(`[name="thLength"]`).hide();
        $(`[name="thWidth"]`).hide();
        $(`[name="thHeight"]`).hide();
    }
    if (sizesShowing) {
        $(`[name="thLength"]`).show();
        $(`[name="thWidth"]`).show();
        $(`[name="thHeight"]`).show();
    }

    trCounter++;

    var lRow = $('#tbody').children().last();

    $(lRow).find(`input[name="thLength"]`).val('22.5');
    $(lRow).find(`input[name="thWidth"]`).val('11.5');
    $(lRow).find(`input[name="thHeight"]`).val('2');

    $(lRow).find(`input[name="thLength"]`).prop("disabled", true);
    $(lRow).find(`input[name="thWidth"]`).prop("disabled", true);
    $(lRow).find(`input[name="thHeight"]`).prop("disabled", true);

    $(lRow).find(`input[name="ValueAdded"]`).val('0');

    if (!IsAgent1)
        $(`[name="thValueAdded"]`).hide();

    if (trCounter > 2 && $('[name="connectionInfoDetail"]').offset().left > 0)
        $('.pt-4').animate({ scrollLeft: $('[name="connectionInfoDetail"]').offset().left + 500 }, 'slow');
    $('.rowInsurance').off('change');
    $('.rowInsurance').change(function () {
        if ($(this).val() && $('[name="connectionInfoDetail"]').offset().left < 0)
            $('.pt-4').animate({ scrollLeft: $('[name="connectionInfoDetail"]').offset().left - 300 }, 'slow');
    });
    $('input[name="IsCod"]').change(function() {
        if(!this.checked) {
            $(this).parent().prev().find('input[name="IsCod"]').val('');
        }
    });
    debugger;
    var _isCOD = $('#IsCod').val();
    if (_isCOD) {
        $('._isCod').show();
    }
    else {
        $('._isCod').hide();
        $('input._isCod').each(function () {
                $(this).prop('checked', false);
                $('input[name="IsCod"]').val('');
        });
    }
}


function isValidRow(rowElemId) {
    var validRow = true;
    $(`[name="thLength"]`).show();
    $(`[name="thWidth"]`).show();
    $(`[name="thHeight"]`).show();
    sizesShowing = true;

    for (var i = 0; i == 0; i++) {
        var GoodsType = $(`#${rowElemId}`).find("input[name='GoodsType']");
        if ($(GoodsType).val() == "") { validRow = false; break; }
        var needCaton = $(`#${rowElemId}`).find("input[name='needCaton']");
        var KartonLafaf = $(`#${rowElemId}`).find("select[name='KartonLafaf']");
        debugger;
        var thLength = $(`#${rowElemId}`).find("input[name='thLength']");
        var thWidth = $(`#${rowElemId}`).find("input[name='thWidth']");
        var thHeight = $(`#${rowElemId}`).find("input[name='thHeight']");
        if (IsNullOrEmpty(thLength.val()) || IsNullOrEmpty(thWidth.val()) || IsNullOrEmpty(thHeight.val())) {
            validRow = false;
            break;
        }

        var ApproximateValue = $(`#${rowElemId}`).find("input[name='ApproximateValue']");
        if ($(ApproximateValue).val() == "") { validRow = false; break; }
        var Weight_g = $(`#${rowElemId}`).find("input[name='Weight_g']");
        if ($(Weight_g).val() == "") { validRow = false; break; }
        var Insurance = $(`#${rowElemId}`).find("select[name='Insurance']");
        if ($(Insurance).find(":selected").index() == 0 && !IsAgent1) { validRow = false; break; }
        var ValueAdded = $(`#${rowElemId}`).find("input[name='ValueAdded']");
        if ($(ValueAdded).val() == "") { validRow = false; break; }
        var ReceiverAddress = $(`#${rowElemId}`).find("label[name='connectionInfoDetail']");
        if ($(ReceiverAddress).val() == "") { validRow = false; break; }
        var ConnectionInfo = $(`#${rowElemId}`).find("input[name='connectionInfo']");
        if ($(ConnectionInfo).val() == "") { validRow = false; break; }
    }
    if (!validRow) {
        $(`#${RowLostFocused}`).children().each(function (e) {
            $(this).children().each(function (e2) {
                $(this).addClass('invalid');
            });
        });
        $(`#${RowLostFocused}`).find("button[name='btneditRow']").hide();
    }
    else {
        $(`#${RowLostFocused}`).children().each(function (e) {
            $(this).children().each(function (e2) {
                $(this).prop("disabled", true);
                $(this).removeClass('invalid');
            });
        });

        $(`#${RowLostFocused}`).find("button[name='btneditRow']").show();
        $(`#${RowLostFocused}`).find("button[name='btneditRow']").prop("disabled", false);
        $(`#${RowLostFocused}`).find("button[name='btnDelete']").prop("disabled", false);
    }
    return validRow;
}

function fnShowReciverAddress(e) {
    currentRow = $(e).parent();
    $("#mapreciver").find('#get-btn-map_i').click();
    $("#_NewAddress").show(250);
    $("#_CountryDiv").show(250);
    $("#_StateDiv").show(250);

    LoadMapInfo(currentRow);
}

function InstallAutofill() {

    $(".clspin").mousedown(function () {
        try {
            Item1TocopyType = $(this).parent().children()[0].type;
            if (Item1TocopyType == "checkbox")
                Item1Tocopy = $($(this).parent().children()[0]).prop('checked');
            else Item1Tocopy = $($(this).parent().children()[0]).val();

            Item2TocopyType = $(this).parent().children()[1].type;
            if (Item2TocopyType == "checkbox")
                Item2Tocopy = $($(this).parent().children()[1]).prop('checked');
            else Item2Tocopy = $($(this).parent().children()[1]).val();

            Item3TocopyType = $(this).parent().children()[2].type;
            if (Item3TocopyType == "checkbox")
                Item3Tocopy = $($(this).parent().children()[2]).prop('checked');
            else Item3Tocopy = $($(this).parent().children()[2]).val();

            Item4TocopyType = $(this).parent().children()[3].type;
            if (Item4TocopyType == "checkbox")
                Item4Tocopy = $($(this).parent().children()[3]).prop('checked');
            else Item4Tocopy = $($(this).parent().children()[3]).val();

        } catch { }
        isMouseDown = true;

        $('#tbody').css('cursor', 'cell');
        $(this).addClass("highlighted");
        //return true; // prevent text selection
    });
    var CurrentElemEntered;
    var lastElemEntered;

    //$("#tbody td").mouseout(function () {
    //    if (isMouseDown) {
    //        //if (lastElemEntered == $(this).attr("id"))
    //        {
    //            $(this).addClass("highlighted");
    //        }
    //    }
    //});

    $("#tbody td").mouseover(function () {
        if (isMouseDown) {
            //if ($(this).hasClass("highlighted")) {
            //    lastElemEntered = CurrentElemEntered;
            //    CurrentElemEntered = $(this).attr("id");
            //}
            $(this).addClass("highlighted");
        }
    });

    //$("#tbody td").mouseover(function () {
    //        if (isMouseDown) {
    //            $(this).toggleClass("highlighted");
    //        }
    //    });

    $("#tbody tr td")
        .mouseup(function () {
            var ask = false;
            $('#tbody tr td').each(function () {
                if ($(this).hasClass("highlighted")) {
                    ask = true;
                    return;
                }
            });

            if (!ask) return;

            swal({
                title: "",
                text: "مقادیر در سلول های انتخاب شده کپی شوند؟",
                icon: "warning",
                showCancelButton: true,
                closeOnConfirm: true,
                confirmButtonText: 'بله',
                cancelButtonText: "خیر"
            },
                function (isConfirm) {
                    if (isConfirm) {
                        $('#tbody tr td').each(function () {
                            if ($(this).hasClass("highlighted"))
                                try {
                                    if (Item1TocopyType == "checkbox")
                                        $($(this).children()[0]).prop("checked", Item1Tocopy);
                                    else $($(this).children()[0]).val(Item1Tocopy);

                                    if (Item2TocopyType == "checkbox")
                                        $($(this).children()[1]).prop("checked", Item2Tocopy);
                                    else $($(this).children()[1]).val(Item2Tocopy);

                                    if (Item3TocopyType == "checkbox")
                                        $($(this).children()[2]).prop("checked", Item3Tocopy);
                                    else $($(this).children()[2]).val(Item3Tocopy);

                                    if (Item4TocopyType == "checkbox")
                                        $($(this).children()[3]).prop("checked", Item4Tocopy);
                                    else $($(this).children()[3]).val(Item4Tocopy);

                                }
                                catch { debugger; }
                        });
                    }
                    $('td').removeClass("highlighted");
                    $('#tbody').css('cursor', 'default');
                    isMouseDown = false;
                    window.onkeydown = null;
                    window.onfocus = null;
                }
            );
            window.onkeydown = null;
            window.onfocus = null;
        });
}

function AssignRowNumbers() {
    window.onkeydown = null;
    window.onkeyup = null;
    window.onkeypress = null;

    var rowCounter = 1;

    $('#tbody tr').each(function () {
        $(this).find("td[name='RowNumber']").html(rowCounter++);

        var colCounter = 1;
        $(this).children().each(function () {
            $(this).attr("id", "td" + rowCounter.toString() + colCounter.toString());
            colCounter++;
        });
    });
}

function separate(inp) {
    Number = inp.value;
    Number += '';
    Number = Number.replaceAll(',', '');

    var matches = Number.match(/^\d+$/);

    if (matches == null) { inp.value = ""; return; }

    x = Number.split('.');
    y = x[0];
    z = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(y))
        y = y.replace(rgx, '$1' + ',' + '$2');
    inp.value = y + z;
}

function LoadMapInfo(RowInfo) {
    $("#divloaddingMap").show();
    //$("#AddressBox").val('');
    //$("#AddressBox").trigger("change");

    var info = $(RowInfo).find('input[name="connectionInfo"]').val();

    if (info == undefined || info == null || info.length == 0)
        info = JsonEmptyMap;

    var myObject = JSON.parse(info);

    var keys = Object.keys(myObject);


    var timer3 = setInterval(function () {
        $(`#Country`).val(myObject["Country"]);
        $("#Country").trigger("change");
        var timer = setInterval(function () {
            clearInterval(timer);
            var timer2 = setInterval(function () {
                $(`#State`).val(myObject["State"]);
                $("#State").trigger("change");

                for (i = 0; i < keys.length; i++) {

                    if (keys[i] == "State" || keys[i] == "Country") {
                        //$(`#${keys[i]}`).trigger("change");
                    }
                    else
                        $(`#${keys[i]}`).val(myObject[keys[i]]);
                }
                $("#divloaddingMap").hide();

                $("#searchOldAddress").click();
                clearInterval(timer2);
            }, 1000);
        }, 1500);
        clearInterval(timer3);
    }, 500);

}


//overriden
function onSave() {
    debugger;
    if ($('#addressType').val() == "Sender") { SenderAddressJson = toJSONString(); return; }
    if ($('#ReceiverStateTown').text() == "") return;
    $(currentRow).parent().find('input[name="connectionInfoDetail"]').val($('#ReceiverStateTown').text());
    var a = toJSONString();
    $(currentRow).find('input[name="connectionInfo"]').val(a);
}

function toJSONString() {
    var obj = {};

    AppendElement(addressType, obj);
    AppendElement(newOldAddress, obj);
    AppendElement(tehranAreaId, obj);
    AppendElement(trafficArea, obj);
    AppendElement(CollectorAreaId, obj);
    AppendElement(isInCityArea, obj);
    AppendElement(Company, obj);
    AppendElement(Email, obj);
    AppendElement(Address1, obj);
    AppendElement(ZipPostalCode, obj);
    AppendElement(PhoneNumber, obj);
    AppendElement(LastName, obj);
    AppendElement(FirstName, obj);
    AppendElement(receiver_ForeginCountryCity, obj);

    AppendElement(receiver_ForeginCountry, obj);
    AppendElement(tempStateName, obj);
    AppendElement(State, obj);
    AppendElement(Country, obj);

    //AppendElement(AddressBox, obj);
    AppendElement(ReceiverStateTown, obj);

    if (ReciverLat.value == "") ReciverLat.value = "0";
    AppendElement(ReciverLat, obj);
    if (ReciverLon.value == "") ReciverLon.value = "0";
    AppendElement(ReciverLon, obj);

    return JSON.stringify(obj);
}

function AppendElement(elem, obj) {
    var name = elem.name;
    var value = elem.value;

    if (elem.type == 'checkbox')
        value = $(elem).prop('checked');

    if (name) {
        obj[name] = value;
    }
}

function IsNullOrEmpty(value) {
    return (!value || value == "");
}

//******************************* UI Handler Events
function fnSubmit() {

    try {
        var lstRes = [];
        var validaData = false;
        $('#tbody tr').each(function () {
            validaData = isValidRow($(this).attr("id"));
            if (!validaData) return false;
        });
        debugger;
        if (!validaData) {
            alert("لطفا اطلاعات را کامل کنید");
            return;
        }
        if (IsNullOrEmpty($('#SenderLat').val()) || IsNullOrEmpty($('#SenderLon').val()) || IsNullOrEmpty($('#FirstName').val())) {
            alert("لطفا آدرس فرستنده را وارد کنید");
            return false;
        }
        if ($('#ServiceId').val() == "0") {
            alert("لطفا نوع سرویس را وارد کنید");
            return false;
        }
        $(`#divSubmitLoading`).show();
        $("#BtnSubmit").prop("disabled", true);
        var billingAddressModel1 = JSON.parse(SenderAddressJson);
        billingAddressModel1['StateProvinceId'] = billingAddressModel1.State;
        billingAddressModel1['CountryId'] = billingAddressModel1.Country;
        billingAddressModel1['Country'] = null;

        $('#tbody tr').each(function () {
            var boxtype1 = "کارتن";
            if ($(this).find("input[name='thHeight']").val() == '0') boxtype1 = "بسته";
            var CartonSizeName1 = 'کارتن نیاز ندارم.';
            if ($(this).find("input[name='needCaton']").prop('checked') == true)
                CartonSizeName1 = $(this).find("select[name='KartonLafaf']").val();

            var shippingAddressModel1 = JSON.parse($(this).find("input[name='connectionInfo']").val());
            shippingAddressModel1['StateProvinceId'] = shippingAddressModel1.State;
            shippingAddressModel1['CountryId'] = shippingAddressModel1.Country;
            shippingAddressModel1['Country'] = null;

            var obj1 = {
                //compatible object with single order
                ServiceId: $('#ServiceId').val(),
                GoodsType: $(this).find("input[name='GoodsType']").val(),
                ApproximateValue: $(this).find("input[name='ApproximateValue']").val().replaceAll(',', ''),
                CodGoodsPrice: ($(this).find('input[name="IsCod"]').is(':checked') ? ($(this).find("input[name='ApproximateValue']").val().replaceAll(',', '')) : 0),
                Weight: $(this).find("input[name='Weight_g']").val().replaceAll(',', ''),
                InsuranceName: $(this).find("select[name='Insurance']").val(),
                CartonSizeName: CartonSizeName1,
                billingAddressModel: billingAddressModel1,
                shippingAddressModel: shippingAddressModel1,
                IsCOD: $(this).find('input[name="IsCod"]').is(':checked'),
                HasAccessToPrinter: $(`#HasAccessToPrinter`).prop('checked'),
                hasNotifRequest: $(`#SendSms`).prop('checked'),//?
                getItNow: false,//?
                AgentSaleAmount: $(this).find("input[name='ValueAdded']").val().replaceAll(',', ''),//?
                discountCouponCode: $(`#discountCouponCode`).val(),
                Count: 1,//?
                length: $(this).find("input[name='thLength']").val(),
                width: $(this).find("input[name='thWidth']").val(),
                height: $(this).find("input[name='thHeight']").val(),
                boxType: boxtype1,
                receiver_ForeginCountry: 0,
                receiver_ForeginCountryName: "",
                receiver_ForeginCountryNameEn: "",
                receiver_ForeginCityName: "",
                UbbraTruckType: "",
                VechileOptions: "",
                UbbarPackingLoad: "",
                dispatch_date: "",
                _dispatch_date: "",
                RequestPrINTAvatar: false,
                SenderLat: "0",//ok
                SenderLon: "0",//ok
                ReciverLat: "0",//ok
                ReciverLon: "0",//ok
                IsFromAp: false,
                tehranCityArea: "",//ok
                collectorArea: "",//ok
                trafficArea: false,//ok
                isInCityArea: false//ok
            };
            lstRes.push(obj1);
        });
        var dataToSend = {
            JsonCheckoutModel: JSON.stringify(lstRes),
            phoneOrderId: $('#phoneOrderId').val(),
            SenderPhoneorderCustomer: billingAddressModel1.PhoneNumber
        }
        addAntiForgeryToken(dataToSend);
        debugger;
        $.ajax({
            cache: false,
            type: "POST",
            url: "/ShipitoCheckout/SaveNewCheckOutBulkOrder",
            data: dataToSend,
            success: function (result) {
                if (result.success == true) {

                    alert('اطلاعات شما جهت بررسی و ثبت به سامانه ارسال شد. به صفحه مشاهده صورت حساب و پرداخت هدایت خواهید شد');
                    window.location = '/order/Sh_billpayment?orderIds[0]=' + result.orderIds;
                }
                else {
                    alert(result.message);
                }
                $(`#divSubmitLoading`).hide();
                $("#BtnSubmit").prop("disabled", false);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('کاربر گرامی در زمان ثبت سفارش شما اشکالی به وجود آمده، لطفا ارتباط اینترنتی دستگاه خود را بررسی کنید');
                $(`#divSubmitLoading`).hide();
                $("#BtnSubmit").prop("disabled", false);
            }
        });
    } catch (ex) {
        console.dir(ex);
        $(`#divSubmitLoading`).hide();
        $("#BtnSubmit").prop("disabled", false);
    }
}

function fnDeleteAll() {
    swal({
        title: "",
        text: "آیا مطمعن هستید؟",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: 'بله',
        cancelButtonText: "خیر"
    },
        function (isConfirm) {
            if (isConfirm == true) {
                $('#tbody tr').each(function () {
                    $(this).remove();
                });
            }
        }
    );
}


function fnEdit(e) {
    var row = $($(e).parent()).parent();
    $(row).children().each(function (e) {
        $(this).children().each(function (e2) {
            $(this).prop("disabled", false);
        })
    });
}

function fnDelete(e) {
    swal({
        title: "",
        text: "آیا مطمعن هستید؟",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: 'بله',
        cancelButtonText: "خیر"
    },
        function (isConfirm) {
            if (isConfirm == true) {
                $($(e).parent()).parent().remove();
                AssignRowNumbers();
            }
        }
    );
}

function btnAddRow() {
    var len = $("#txtAddCount").val();
    if (len > 0)
        for (var i = 0; i < len; i++) {
            fnAddRow();
        }
}


function fnKartonLafafChange(own) {
    debugger;

    var KartonLafafvalue = $(own).val();

    if (KartonLafafvalue == 'سایر(بزرگتر از سایز 9)') {

        $(`[name="thLength"]`).show();
        $(`[name="thWidth"]`).show();
        $(`[name="thHeight"]`).show();
        sizesShowing = true;

        $(own).parent().parent().find(`[name="thLength"]`).prop("disabled", false);
        $(own).parent().parent().find(`[name="thWidth"]`).prop("disabled", false);
        $(own).parent().parent().find(`[name="thHeight"]`).prop("disabled", false);
    }
    else {
        $(`[name="thLength"]`).hide();
        $(`[name="thWidth"]`).hide();
        $(`[name="thHeight"]`).hide();
        sizesShowing = false;
        $(own).parent().parent().find(`[name="thLength"]`).prop("disabled", true);
        $(own).parent().parent().find(`[name="thWidth"]`).prop("disabled", true);
        $(own).parent().parent().find(`[name="thHeight"]`).prop("disabled", true);

        var KartonLafaflength = 0;
        var KartonLafafwidth = 0;
        var KartonLafafheight = 0;

        var daimantion = KartonLafafvalue.split('(')[1].replace(')', '').split('*');
        KartonLafaflength = daimantion[0];
        KartonLafafwidth = daimantion[1];
        KartonLafafheight = "2";
        if (daimantion.length > 2)
            KartonLafafheight = daimantion[2];

        $(own).parent().parent().find(`[name="thLength"]`).val(KartonLafaflength);
        $(own).parent().parent().find(`[name="thWidth"]`).val(KartonLafafwidth);
        $(own).parent().parent().find(`[name="thHeight"]`).val(KartonLafafheight);

    }
}