namespace UnitOfWork.Customer
{
    /// <summary>
    /// 联系人地址
    /// </summary>
    public class ContactAddress : Entity
    {
        public ContactAddress(string contactRealName, string contactPhone, string province, string city, string county,
            string street, string zip = "")
        {
            Province = province;
            City = city;
            County = county;
            Street = street;
            ContactRealName = contactRealName;
            ContactPhone = contactPhone;
            Zip = zip;
        }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string ContactRealName { get; private set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; private set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; private set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// 区县
        /// </summary>
        public string County { get; private set; }

        /// <summary>
        /// 街道
        /// </summary>
        public string Street { get; private set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string Zip { get; private set; }

        /// <summary>
        /// 省市区街道
        /// </summary>
        public string SimpleAddress => $"{Province} {City} {County} {Street}";

        /// <summary>
        /// 市区街道详情
        /// </summary>
        public string DetailAddress => $"{County}{Street}({Zip})";

        public bool IsDefault { get; set; }
    }
}