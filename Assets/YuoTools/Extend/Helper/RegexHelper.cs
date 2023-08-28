namespace YuoTools.Extend.Helper
{
    public class RegexHelper
    {
        public const string Email = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string Mobile = @"^1[3|4|5|7|8]\d{9}$";
        public const string Phone = @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,8}$";
        public const string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
        public const string IdCard = @"^(\d{15}|\d{18})$";
        public const string ZipCode = @"^\d{6}$";
        public const string Number = @"^\d+$";
        public const string Integer = @"^[-\+]?\d+$";
        public const string Double = @"^[-\+]?\d+(\.[0-9]+)?$";
        public const string English = @"^[A-Za-z]+$";
        public const string Chinese = @"^[\u4e00-\u9fa5]+$";
        public const string ChineseAndEnglish = @"^[\u4e00-\u9fa5A-Za-z]+$";


        public const string 身份证号码 = @"^([0-9]){7,18}(x|X)?$ 或 ^\d{8,18}|[0-9x]{8,18}|[0-9X]{8,18}?$";

        public const string 账号 = "^[a-zA-Z][a-zA-Z0-9_]{4,15}$";

        public const string 密码 = @"^[a-zA-Z]\w{5,17}$";

        public const string 强密码 = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,10}$";

        public const string 日期格式 = @"^\d{4}-\d{1,2}-\d{1,2}";

        public const string 腾讯qq号 = "[1-9][0-9]{4,} (腾讯QQ号从10000开始)";

        public const string 中国邮政编码 = @"[1-9]\d{5}(?!\d)";

        public const string Ip地址 = @"\d+\.\d+\.\d+\.\d+";

        public const string Ipv4地址 =
            "\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b";

        public const string 子网掩码 = "((?:(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d)\\.){3}(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d))";

        public const string 抽取注释 = "<!--(.*?)-->";

        public const string 判断ie版本 = "^.*MSIE [5-8](?:\\.[0-9]+)?(?!.*Trident\\/[5-9]\\.0).*$";
    }
}