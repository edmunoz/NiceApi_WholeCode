using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public class UpdateInfo 
    {
        public string Title;
        public string[] Info;
        public Data_AppUserFile.eUserStatus Type;
        public UpdateInfoSection Number;
        public UpdateInfoSection Message;
        public UpdateInfoSection Month;
        public UpdateInfoSection OneTimeSetup;
        public UpdateInfoSection FullPayment;

        public class UpdateInfoOption
        {
            public int Factor;
            public string Text;

            public UpdateInfoOption(int factor, string text)
            {
                Factor = factor;
                Text = text;
            }
        }

        public class UpdateInfoSection
        {
            public Decimal CostPerUnit;
            public UpdateInfoOption[] Options;
            public string PromptText;
            public bool Active { get { return Options != null; } }

            public UpdateInfoSection(string promptText, Decimal costPerUnit, UpdateInfoOption[] options)
            {
                CostPerUnit = costPerUnit;
                Options = options;
                PromptText = promptText;
            }
        }

        public UpdateInfo(
            string title, Data_AppUserFile.eUserStatus type,
            string[] info,
            UpdateInfoSection number,
            UpdateInfoSection message,
            UpdateInfoSection month,
            UpdateInfoSection oneTimeSetup,
            UpdateInfoSection fullPayment)
        {
            Title = title;
            Type = type;
            Info = info;
            Number = number;
            Message = message;
            Month = month;
            OneTimeSetup = oneTimeSetup;
            FullPayment = fullPayment;

            Number = Number != null ? Number : new UpdateInfoSection("", 0M, null);
            Message = Message != null ? Message : new UpdateInfoSection("", 0M, null);
            Month = Month != null ? Month : new UpdateInfoSection("", 0M, null);
            OneTimeSetup = OneTimeSetup != null ? OneTimeSetup : new UpdateInfoSection("", 0M, null);
            FullPayment = FullPayment != null ? FullPayment : new UpdateInfoSection("", 0M, null);
        }

        public static UpdateInfo GetInfo(Data_AppUserFile.eUserStatus type)
        {
            switch (type)
            {
                case Data_AppUserFile.eUserStatus.free_account:
                    return UpdatePriceText.s_FreeAccountInfo;
                case Data_AppUserFile.eUserStatus.commercial_payassent:
                    return UpdatePriceText.s_PayAsYouSend;
                case Data_AppUserFile.eUserStatus.commercial_monthly:
                    return UpdatePriceText.s_PayMonthly;
                case Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice:
                    return UpdatePriceText.s_PayMonthlyDifPrice;
                case Data_AppUserFile.eUserStatus.commercial_systemDuplication:
                    return UpdatePriceText.s_SystemDuplication;
            }
            return null;
        }
        public static Data_AppUserFile.eUserStatus ParseType(string type)
        {
            switch (type)
            {
                case "PayAsYouSend":
                    return Data_AppUserFile.eUserStatus.commercial_payassent;
                case "PayMonthly":
                    return Data_AppUserFile.eUserStatus.commercial_monthly;
                case "Pay Monthly":
                    return Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice;
                case "SystemDuplication":
                    return Data_AppUserFile.eUserStatus.commercial_systemDuplication;
                case "FreeAccount":
                    return Data_AppUserFile.eUserStatus.free_account;
            }
            return Data_AppUserFile.eUserStatus.blocked;
        }
    }
    public static class UpdatePriceText
    {
        public static UpdateInfo s_FreeAccountInfo = new UpdateInfo(
        "Free Account",
        Data_AppUserFile.eUserStatus.free_account,
        new string[] {
"There are no fees at all.",
"You can send Whatsapp messages for one of our Whatsapp accounts to your registered numbers.",
"You can register up to 5 numbers on which you will receive the Whatsapp message.",
"After registration you can not change your registered numbers, however, send us an email with the numbers you want to add/change/remove and we will do it for you.",
"You can send one message each 60 seconds.",
"You can send up to 200 messages in total.",
"Delivery takes some 15 seconds.",
"The received message contains our NiceApi.net footer."},
        null, null, null, null, null);


        public static UpdateInfo s_PayAsYouSend = new UpdateInfo(
        "PayAsYouSend",
        Data_AppUserFile.eUserStatus.commercial_payassent,
        new string[] {
"No monthly fee.",
"You can send Whatsapp messages for one of our Whatsapp accounts to your registered numbers.",
new PromotionText("We will provide you with an API to add new numbers to your account. Each number you add costs ", "$ 1.00", " $ 0.50 ", ". You can add unlimited numbers.").ToString(),
"We will provide you with an API to check your current balance.",
new PromotionText("Each message you send costs ", "$ 0.25", " $ 0.12 ", ".").ToString(),
"No time limitation between messages.",
"The receiving message contains exactly what you sent (no NiceApi.net footer).",
"Delivery takes some 15 seconds with priority over free messages.",
"You can add funds to your account using PayPal, Bitcoin or Bitcoin Cash."},
        new UpdateInfo.UpdateInfoSection(
            "How many numbers would you like to add to your account?",
            0.50M, 
            new UpdateInfo.UpdateInfoOption[] {
                new UpdateInfo.UpdateInfoOption(  10, "   10 numbers"),
                new UpdateInfo.UpdateInfoOption(  20, "   20 numbers"),
                new UpdateInfo.UpdateInfoOption(  50, "   50 numbers"),
                new UpdateInfo.UpdateInfoOption( 100, "  100 numbers"),
                new UpdateInfo.UpdateInfoOption(1000, " 1000 numbers"),
                new UpdateInfo.UpdateInfoOption(5000, " 5000 numbers") }),
        new UpdateInfo.UpdateInfoSection(
            "How many Whatsapp messages would you like to send?",
            0.12M, new UpdateInfo.UpdateInfoOption[] {
                new UpdateInfo.UpdateInfoOption(  10, "   10 messages"),
                new UpdateInfo.UpdateInfoOption(  50, "   50 messages"),
                new UpdateInfo.UpdateInfoOption( 100, "  100 messages"),
                new UpdateInfo.UpdateInfoOption(1000, " 1000 messages") }),
        null,
        null,
        null);

        public static UpdateInfo s_PayMonthly = new UpdateInfo(
        "PayMonthly",
        Data_AppUserFile.eUserStatus.commercial_monthly,
        new string[] {
new PromotionText("Monthly fee of ", "$ 19.99", " $ 9.99 ", "").ToString(),
"You can send Whatsapp messages for one of our Whatsapp accounts to your registered numbers.",
new PromotionText("We will provide you with an API to add new numbers to your account. Each number you add costs ", "$ 1.00", " $ 0.50 ", ". You can add unlimited numbers.").ToString(),
"We will provide you with an API to check your current balance.",
"Send as many messages as you wish without additional charges.",
"You can send one message each 10 seconds.",
"Delivery takes some 15 seconds with priority over free messages.",
"You can add funds to your account using PayPal, Bitcoin or Bitcoin Cash."},
        new UpdateInfo.UpdateInfoSection(
            "How many numbers would you like to add to your account?",
            0.50M,
            new UpdateInfo.UpdateInfoOption[] {
                new UpdateInfo.UpdateInfoOption(  10, "   10 numbers"),
                new UpdateInfo.UpdateInfoOption(  20, "   20 numbers"),
                new UpdateInfo.UpdateInfoOption(  50, "   50 numbers"),
                new UpdateInfo.UpdateInfoOption( 100, "  100 numbers"),
                new UpdateInfo.UpdateInfoOption(1000, " 1000 numbers"),
                new UpdateInfo.UpdateInfoOption(5000, " 5000 numbers") }),
        null,
        new UpdateInfo.UpdateInfoSection(
            "For how long would you like to upgraded your account?",
            9.99M, 
            new UpdateInfo.UpdateInfoOption[] {
                new UpdateInfo.UpdateInfoOption(  1, " 1 month"),
                new UpdateInfo.UpdateInfoOption(  2, " 2 months"),
                new UpdateInfo.UpdateInfoOption(  6, " 6 months"),
                new UpdateInfo.UpdateInfoOption( 12, " 12 months"),
                new UpdateInfo.UpdateInfoOption( 24, " 24 months") }),
        null, null);


        public static UpdateInfo s_PayMonthlyDifPrice = new UpdateInfo(
        "Pay Monthly",
        Data_AppUserFile.eUserStatus.commercial_monthlyDifPrice,
        new string[] {
new PromotionText("Monthly base fee of ", "$ 19.99", " $ 9.99 ", " for witch you can send up to 200 messages per month.").ToString(),
"Should you send more than 200 messages, there is an additional charge of 5.00 (in total 14.99) for witch you can send uo to 500 messages per month.",
"Should you send more than 500 messages, there is an additional charge of 5.00 (in total 19.99) for witch you can send uo to 1000 messages per month.",
"Should you send more than 1000 messages, there is an additional charge of 5.00 (in total 24.99) for witch you can send uo to 2000 messages per month.",
"Should you send more than 2000 messages, there is an additional charge of 5.00 (in total 29.99) for witch you can send uo to 5000 messages per month.",
"Should you send more than 5000 messages, there is an additional charge of 5.00 (in total 34.99) for witch you can send uo to 10000 messages per month.",
"You can send Whatsapp messages for one of our Whatsapp accounts to your registered numbers.",
new PromotionText("We will provide you with an API to add new numbers to your account. Each number you add costs ", "$ 1.00", " $ 0.50 ", ". You can add unlimited numbers.").ToString(),
"We will provide you with an API to check your current balance.",
"No time limitation between messages.",
"Delivery takes some 15 seconds with priority over free messages.",
"You can add funds to your account using PayPal, Bitcoin or Bitcoin Cash."},
        null, 
        null, 
        null,
        null,
        new UpdateInfo.UpdateInfoSection(
                "Please choose the amount you would like to put on your account.",
                1M,
                new UpdateInfo.UpdateInfoOption[] {
                    new UpdateInfo.UpdateInfoOption(  10, " 10 USD"),
                    new UpdateInfo.UpdateInfoOption(  20, " 20 USD"),
                    new UpdateInfo.UpdateInfoOption(  50, " 50 USD"),
                    new UpdateInfo.UpdateInfoOption(  100, " 100 USD"),
                    new UpdateInfo.UpdateInfoOption(  200, " 200 USD"),
                    new UpdateInfo.UpdateInfoOption(  500, " 500 USD") 
                }
            ));


        public static UpdateInfo s_SystemDuplication = new UpdateInfo(
        "SystemDuplication",
        Data_AppUserFile.eUserStatus.commercial_systemDuplication,
        new string[] {
new PromotionText("One time setup fee of ", "$ 150.0", " $ 0.00 ", " .").ToString(),
new PromotionText("Plus monthly fee of ", "$ 200.0", " $ 100.00 ", " .").ToString(),
"You can send Whatsapp messages from your own number to any of your registered receiving numbers.",
"You can choose the icon of the sending Whatsapp account.",
//"You need to send us a SIM card with you sending number. ",
"We will provide you with an API to add new numbers to your account. No additional costs to add receiving numbers. You can add unlimited numbers.",
"We will provide you with an API to check your current balance.",
"Send as many messages as you wish without additional charges.",
"No time limitation between messages.",
"Delivery takes some 15 seconds, you are the only one using the hardware.",
"You can add funds to your account using PayPal, Bitcoin or Bitcoin Cash.",
"We issue an invoice should you need one."},
        null,
        null,
        new UpdateInfo.UpdateInfoSection(
            "For how long would you like to upgraded your account?",
            100.00M,
            new UpdateInfo.UpdateInfoOption[] {
                new UpdateInfo.UpdateInfoOption(  6, " 6 months"),
                new UpdateInfo.UpdateInfoOption( 12, " 1 year"),
                new UpdateInfo.UpdateInfoOption( 24, " 2 years") }),
        null, null);
    }

    class PromotionText
    {
        public StringBuilder sb = new StringBuilder();
        public PromotionText(string normal1, string old, string promo, string normal2)
        {
            AddNormalText(normal1);
            AddOld(old);
            AddPromo(promo);
            AddNormalText(normal2);

        }

        private void AddNormalText(string text)
        {
            sb.Append(text);
        }
        private void AddOld(string old)
        {
            sb.Append("<s>" + old + "</s>");
        }
        private void AddPromo(string promo)
        {
            sb.Append("<div style=\"display: inline\" class=\"promotion\">" + promo + "</div>");
        }
        public override string ToString()
        {
            return this.sb.ToString();
        }
    }

    public class UpgradeTextList
    {
        public string[] Vals;

        public UpgradeTextList(string[] list)
        {
            Vals = list;
        }

        public string GetAsHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string text in this.Vals)
                {
                    sb.AppendLine("&nbsp;-&nbsp;" + text + "\n");
                }
                return sb.ToString();
            }
        }

        public string GetAsEmialText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string line1 in this.Vals)
                {
                    string line = line1;
                    while (true)
                    {
                        int s1 = line.IndexOf('<');
                        if (s1 == -1)
                        {
                            break;
                        }
                        int e1 = line.IndexOf('>', s1) + 1;
                        int s2 = line.IndexOf('<', e1);
                        int e2 = line.IndexOf('>', s2) + 1;

                        string pre = line.Substring(0, s1);
                        string x1 = line.Substring(s1, e1 - s1);
                        string mid = line.Substring(e1, s2 - e1);
                        string x2 = line.Substring(s2, e2 - s2);
                        string rest = line.Substring(e2);

                        if (x1 == "<s>")
                        {
                            // filter it out
                            line = pre + rest;
                        }
                        else if (x1.Contains("class=\"promotion\""))
                        {
                            line = pre + mid + rest;
                        }
                        else
                        {
                            throw new NotImplementedException("hidePromotion");
                        }
                    }
                    sb.AppendLine("&nbsp;-&nbsp;" + line + "<br>\n");
                }
                return sb.ToString();
            }
        }
    }
}
