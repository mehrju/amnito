using Newtonsoft.Json;

namespace FrotelServiceLibrary.Input
{
    public class NewShopInput
    {
        [JsonProperty("site")]
        public string Site { get; set; }

        /// <summary>
        /// مدیرمسئول	
        /// </summary>
        [JsonProperty("Aname")]
        public string ManagerName { get; set; }

        /// <summary>
        /// نام فارسی
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("tel")]
        public string Tel { get; set; }

        [JsonProperty("pcode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// استان
        /// </summary>
        [JsonProperty("State")]
        public int State { get; set; }

        /// <summary>
        /// استان
        /// </summary>
        [JsonProperty("city")]
        public int City { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// کد ملی/ش.ثبت	
        /// </summary>
        [JsonProperty("code")]
        public string NationalCode { get; set; }

        /// <summary>
        /// نام کاربری
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }


        /// <summary>
        /// تاریخ فیش واریزی	
        /// </summary>
        [JsonProperty("fyear")]
        public string FishDateYear { get; set; }

        /// <summary>
        /// تاریخ فیش واریزی	
        /// </summary>
        [JsonProperty("fmonth")]
        public string FishDateMonth { get; set; }

        /// <summary>
        /// تاریخ فیش واریزی	
        /// </summary>
        [JsonProperty("fday")]
        public string FishDateDay { get; set; }

       //B2.x:951
       //B2.y:220

    }
}
