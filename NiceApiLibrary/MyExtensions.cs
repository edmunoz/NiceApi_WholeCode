using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using NiceApiLibrary_low;

namespace NiceApiLibrary
{
    public static class MyExtensions_high
    {
        public static DateTime PaidUntil(this Data_AppUserFile.monthlyDifPriceAccount difPrice)
        {
            DateTime ret;
            if (difPrice.monthlyDifPrice_PeriodeDurationInDays == 30)
            {
                DateTime st = new DateTime(difPrice.monthlDifPricey_PeriodeStart, DateTimeKind.Utc);
                ret = st.AddMonths(1);
            }
            else
            {
                DateTime st = new DateTime(difPrice.monthlDifPricey_PeriodeStart, DateTimeKind.Utc);
                ret = st.AddDays(difPrice.monthlyDifPrice_PeriodeDurationInDays);
            }
            return ret;
        }

        public static TimeSpan TimeLeft(this Data_AppUserFile.monthlyDifPriceAccount difPrice)
        {
            TimeSpan ts = difPrice.PaidUntil() - DateTime.UtcNow;
            return ts;
        }

        public static TimeSpan ExpiredSince(this Data_AppUserFile.monthlyDifPriceAccount difPrice)
        {
            TimeSpan ts = DateTime.UtcNow - difPrice.PaidUntil();
            return ts;
        }

        public static bool HasExpired(this Data_AppUserFile.monthlyDifPriceAccount difPrice)
        {
            if (difPrice.PaidUntil() < DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        public static bool DeductIfEnoughFund(this Data_AppUserFile.niceMoney wallet, Data_AppUserFile.niceMoney deduct)
        {
            if (wallet.ValueInUsCent >= deduct.ValueInUsCent)
            {
                wallet.ValueInUsCent -= deduct.ValueInUsCent;
                return true;
            }
            return false;
        }

        public static System.Data.DataTable Exceptio2Table(this Exception ex)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            System.Data.DataRow dr = null;
            //Create the Columns Definition
            dt.Columns.Add(new System.Data.DataColumn("Info", typeof(string)));

            //Add the first Row to each columns defined
            dr = dt.NewRow();
            dr["Info"] = ex.ToString();
            dt.Rows.Add(dr);

            return dt;
        }

        public static System.Data.DataTable SQLiteDataReader2Table(this System.Data.SQLite.SQLiteDataReader d)
        {
            //Create a new DataTable.
            System.Data.DataTable dt = new System.Data.DataTable("Result");

            //Load DataReader into the DataTable.
            dt.Load(d);
            return dt;
        }

        public static WebControlsTableResult DataTable2WebControlsTable(this System.Data.DataTable dt, int maxText)
        {
            string unitString = dt.Columns.Count == 0 ? "" : (100 / dt.Columns.Count).ToString() + "%";

            WebControlsTableResult ret = new WebControlsTableResult();
            ret.Table = new System.Web.UI.WebControls.Table();
            ret.Table.BorderWidth = 1;
            ret.Table.GridLines = System.Web.UI.WebControls.GridLines.Both;
            System.Web.UI.WebControls.TableRow row = null;

            //Add the Headers
            row = new System.Web.UI.WebControls.TableRow();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                System.Web.UI.WebControls.TableHeaderCell headerCell = new System.Web.UI.WebControls.TableHeaderCell();
                headerCell.Text = dt.Columns[j].ColumnName;
                row.Cells.Add(headerCell);
            }
            ret.Table.Rows.Add(row);

            //Add the Column values
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                row = new System.Web.UI.WebControls.TableRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    System.Web.UI.WebControls.TableCell cell = new System.Web.UI.WebControls.TableCell();
                    cell.Text = dt.Rows[i][j].ToString().LimitText(maxText);
                    cell.Width = new System.Web.UI.WebControls.Unit(unitString);
                    row.Cells.Add(cell);
                }
                // Add the TableRow to the Table
                ret.Table.Rows.Add(row);
            }

            ret.Label = new System.Web.UI.WebControls.Label();
            ret.Label.Text = "C:" + dt.Columns.Count.ToString() + " R:" + dt.Rows.Count.ToString();
            return ret;
        }

        public static int SmallRest(this DateTime dt)
        {
            DateTime dt2 = new DateTime(
                dt.Year, dt.Month, dt.Day,
                dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
            return (int)(dt.Ticks - dt2.Ticks);
        }

        public static PriceAndText CalucateCost(this Data_AppUserWallet appUserWallet)
        {
            PriceAndText ret = new PriceAndText();

            List<string> left = new List<string>();
            List<string> rigt = new List<string>();

            if ((appUserWallet.Numbers != null) && (appUserWallet.Numbers.Amount != 0))
            {
                left.Add(String.Format("{0:####} Registered Numbers * $ {1:0.00}",
                    appUserWallet.Numbers.Amount, appUserWallet.Numbers.Price));
                rigt.Add((appUserWallet.Numbers.Amount * appUserWallet.Numbers.Price).rigtStr());
                ret.FinalPrice += appUserWallet.Numbers.Amount * appUserWallet.Numbers.Price;
            }

            if ((appUserWallet.Messages != null) && (appUserWallet.Messages.Amount != 0))
            {
                left.Add(String.Format("{0:####} Messages * $ {1:0.00}",
                    appUserWallet.Messages.Amount, appUserWallet.Messages.Price));
                rigt.Add((appUserWallet.Messages.Amount * appUserWallet.Messages.Price).rigtStr());
                ret.FinalPrice += appUserWallet.Messages.Amount * appUserWallet.Messages.Price;
            }

            if ((appUserWallet.Month != null) && (appUserWallet.Month.Amount != 0))
            {
                left.Add(String.Format("{0:##} Month * $ {1:0.00}", appUserWallet.Month.Amount, appUserWallet.Month.Price));
                rigt.Add((appUserWallet.Month.Amount * appUserWallet.Month.Price).rigtStr());
                ret.FinalPrice += appUserWallet.Month.Amount * appUserWallet.Month.Price;
            }

            if ((appUserWallet.Setup != null) && (appUserWallet.Setup.Price != 0))
            {
                left.Add(String.Format("$ {0:0.00} Setup Fee", appUserWallet.Setup.Price));
                rigt.Add((appUserWallet.Setup.Price).rigtStr());
                ret.FinalPrice += appUserWallet.Setup.Price;
            }

            if ((appUserWallet.FullPayment != null) && ((appUserWallet.FullPayment.Amount * appUserWallet.FullPayment.Price) != 0))
            {
                left.Add(String.Format("$ {0:0.00} Payment", appUserWallet.FullPayment.Amount * appUserWallet.FullPayment.Price));
                rigt.Add((appUserWallet.FullPayment.Amount * appUserWallet.FullPayment.Price).rigtStr());
                ret.FinalPrice += appUserWallet.FullPayment.Amount * appUserWallet.FullPayment.Price;
            }

            // make left same length
            int maxLen = 0;
            foreach (string l1 in left)
            {
                if (l1.Length > maxLen)
                {
                    maxLen = l1.Length;
                }
            }
            for (int i = 0; i < left.Count; i++)
            {
                ret.Explained += left[i].PadRight(maxLen + 1) + rigt[i] + "\n";
            }
            ret.Explained += "".PadRight(maxLen + 1 + rigt[0].Length, '-') + "\n";
            ret.Explained += "TOTAL".PadRight(maxLen + 1);
            ret.Explained += ret.FinalPrice.rigtStr();
            return ret;
        }

        public static bool HasUpgradeRequest(this Data_AppUserWallet appUserWallet)
        {
            return true;
        }

        public static string GetEmailBody(this Data_AppUserWallet appUserWallet, string userName, string userEmail)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<p>Hi {0}</p>\n", userName);
            sb.AppendLine("You have committed yourself to the following account upgrade:<br>");
            sb.AppendFormat("Please visit <a href=\"{0}\">{0}</a> for payment instructions.<br>\n", "https://niceapi.net/Upgrade");
            sb.AppendFormat("Your email is {0}\n", userEmail);
            sb.AppendLine("<br>");
            sb.AppendLine(new UpgradeTextList(appUserWallet.DisplayLines).GetAsEmialText);
            sb.AppendLine("<br>\n");
            sb.AppendLine("<pre>" + appUserWallet.CalucateCost().Explained + "</pre>");
            sb.AppendLine("<br>\n");
            sb.AppendLine("<p>Your NiceApi.net team.</p>");
            return sb.ToString();
        }

        public static string DisplayLinesAsHTML(this Data_AppUserWallet appUserWallet)
        {
            UpgradeTextList t = new UpgradeTextList(appUserWallet.DisplayLines);
            return t.GetAsHTML;
        }

        public static NiceSystemInfo GetSystemInfoFromAPIId(this string apiId)
        {
            // use Default if not there
            NiceSystemInfo ret = DSSwitch.full().GetSystems(false).FirstOrDefault(_ => _.APIId == apiId);
            if (ret == null)
            {
                return NiceSystemInfo.DEFAULT;
            }
            return ret;
        }

        public static NiceSystemInfo GetSystemInfoFromTrayType(this string trayType)
        {
            // use null if not there
            NiceSystemInfo ret = DSSwitch.full().GetSystems(false).FirstOrDefault(_ => _.Name == trayType);
            return ret;
        }
    }

    public static class MailMessageExtensions
    {
        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly Type MailWriter = typeof(SmtpClient).Assembly.GetType("System.Net.Mail.MailWriter");
        private static readonly ConstructorInfo MailWriterConstructor = MailWriter.GetConstructor(Flags, null, new[] { typeof(Stream) }, null);
        private static readonly MethodInfo CloseMethod = MailWriter.GetMethod("Close", Flags);
        private static readonly MethodInfo SendMethod = typeof(MailMessage).GetMethod("Send", Flags);

        /// <summary>
        /// A little hack to determine the number of parameters that we
        /// need to pass to the SaveMethod.
        /// </summary>
        private static readonly bool IsRunningInDotNetFourPointFive = SendMethod.GetParameters().Length == 3;

        /// <summary>
        /// The raw contents of this MailMessage as a MemoryStream.
        /// </summary>
        /// <param name="self">The caller.</param>
        /// <returns>A MemoryStream with the raw contents of this MailMessage.</returns>
        public static MemoryStream RawMessage(this MailMessage self)
        {
            var result = new MemoryStream();
            var mailWriter = MailWriterConstructor.Invoke(new object[] { result });
            SendMethod.Invoke(self, Flags, null, IsRunningInDotNetFourPointFive ? new[] { mailWriter, true, true } : new[] { mailWriter, true }, null);
            result = new MemoryStream(result.ToArray());
            CloseMethod.Invoke(mailWriter, Flags, null, new object[] { }, null);
            return result;
        }

        public static string ToNiceString(this Data_AppUserFile appUserFile)
        {
            Data_AppUserFile_CheckerBase checker = appUserFile.GetCheckerBase(true);
            if (checker == null)
            {
                return "???";
            }
            return checker.Info(appUserFile);
        }
    }

}

