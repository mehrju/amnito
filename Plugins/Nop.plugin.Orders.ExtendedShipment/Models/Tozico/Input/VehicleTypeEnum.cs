using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input
{
    public enum VehicleTypeEnum
    {
        [Display(Name = "موتور")]
        bike = 0,
        [Display(Name = "خودرو شخصی")]
        sedan = 1,
        [Display(Name = "وانت")]
        pickup = 2,
        [Display(Name = "کامیون")]
        truck = 3
    }
}
