using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Enums
{
    public enum OrderShipmentStatusEnum
    {
        [Display(Name = "ثبت اولیه و ارجاع به نماینده")]
        Registration = 100,
        [Display(Name = "آدرس فرستنده شناسایی نشد")]
        SenderAddressNoApproved = 101,
        [Display(Name = "فرستنده مرسوله را تحویل نداد")]
        SenderDidntAccepted = 102,
        [Display(Name = "مرسوله در انبار امنیتو - شهر مبدا")]
        IsInSenderCityStore = 103,
        [Display(Name = "مرسوله در ترمینال - شهر مبدا")]
        IsInSenderCityTerminal = 104,
        [Display(Name = "مرسوله در ترمینال - شهر مقصد یا میانی")]
        IsInReceiverCityTerminal = 105,
        [Display(Name = "مرسوله در انبار امنیتو - شهر مقصد یا میانی")]
        IsInReceiverCityStore = 106,
        [Display(Name = "مرسوله در اختیار پیک")]
        BikeDelivery = 107,
        [Display(Name = "مرسوله در ایستگاه قطار")]
        InTrainStation = 108,
        [Display(Name = "مرسوله در فرودگاه")]
        InAirPort = 109,
        [Display(Name = "آدرس گیرنده یافت نشد")]
        ReceiverAddressNoApproved = 110,
        [Display(Name = "گیرنده در محل حضور نداشت")]
        ReceiverWerentHome = 111,
        [Display(Name = "گیرنده مرسوله را تحویل نگرفت")]
        ReceiverDidntAccept = 112,

        [Display(Name = "جمع آوری نشده")]
        DidntCollect = 1,
        [Display(Name = "ارسال نشده")]
        DidntSend = 2,
        [Display(Name = "تحویل نشده")]
        DidntDeliver = 3,
        [Display(Name = "کلی")]
        All = 0,
        [Display(Name = "نامشخص")]
        NotDetermined = 113,

        [Display(Name = "هدیه شما بسته بندی شد")]
        GiftWrapped = 200,
        [Display(Name = "درخواست خرید گل شما انجام شد")]
        FlowerBought = 201,
        [Display(Name ="درخواست خرید کیک شما انجام شد")]
        CakeBought = 202
    }
}
