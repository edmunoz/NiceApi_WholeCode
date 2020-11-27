using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NiceBackgroundApp
{
    class MyContactsHelper
    {
        private Context Context;

        private void Test()
        {
            Android.Net.Uri uri = ContactsContract.Contacts.ContentUri;
            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
            };
            Android.Database.ICursor cursor = Application.Context.ContentResolver.Query(uri, projection, null, null, null);
            List<string> contactList = new List<string>();
            if (cursor.MoveToFirst())
            {
                do
                {
                    contactList.Add(cursor.GetString(cursor.GetColumnIndex(projection[1])));
                } while (cursor.MoveToNext());
            }
        }

        internal static string ToCleanZipString(string strIn)
        {
            // strIn could be "zapi_41235689 zapi_41235689"
            string o = strIn.Replace(" ", "");
            if (o.Length > 5)
            {
                int secondZap = o.IndexOf("zapi", 5);
                if (secondZap != -1)
                {
                    o = o.Substring(0, secondZap);
                }
            }
            return o;
        }

        /*
         * Java:
    public static String GetAllContacts(int maxAmount, PrintWriter outWriter, Context context) {
        try {
            Uri contacts = ContactsContract.Contacts.CONTENT_URI;
            CursorLoader loader = new CursorLoader(context, contacts, null, null, null, ContactsContract.Contacts.DISPLAY_NAME + " asc");
            Cursor cur = loader.loadInBackground();
            String s = String.valueOf(cur.getCount());
            PrintStructuredString(outWriter, s);
            PrintStructuredString(outWriter, ":");
            if (cur.getCount() > 0) {
                while (cur.moveToNext()) {
                    if (maxAmount == 0) {
                        break;
                    } else if (maxAmount > 0) {
                        maxAmount--;
                    }
                    String name = cur.getString(cur
                            .getColumnIndex(ContactsContract.Contacts.DISPLAY_NAME));
                    PrintStructuredString(outWriter, name);
                }
            }
        }catch(Exception ex){
            return ex.getMessage();
        }
        return "";
    }
        */


        public static void GetAllContacts(int maxAmount, StreamWriter outWriter)
        {
            try
            {
                Android.Net.Uri uri = ContactsContract.Contacts.ContentUri;
                string[] projection = {
                    ContactsContract.Contacts.InterfaceConsts.Id,
                    ContactsContract.Contacts.InterfaceConsts.DisplayName
                };
                Android.Database.ICursor cursor = Application.Context.ContentResolver.Query(uri, projection, null, null, null);
                int count = cursor.Count;
                PrintStructuredString(outWriter, count.ToString());
                PrintStructuredString(outWriter, ":");
                if (cursor.MoveToFirst())
                {
                    while (true)
                    {
                        if ((maxAmount != -1) && (maxAmount-- == 0))
                        {
                            break;
                        }
                        string tel = ToCleanZipString(cursor.GetString(cursor.GetColumnIndex(projection[1])));
                        PrintStructuredString(outWriter, tel);
                        if (!cursor.MoveToNext())
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outWriter.Write(ex.ToString());
            }
        }

        /*
         Java:
    public static String InsertZap(String data, Context context) {
        try {
            String surename = "zapi_" + data;
            String tel = "+" + data;
            InsertNoTry(surename, tel, context);
        }catch(Exception ex){
            return ex.getMessage();
        }
        return "";
    }
    private static void InsertNoTry(String nameSurname, String telephone, Context context) throws Exception {
        ArrayList<ContentProviderOperation> ops = new ArrayList<ContentProviderOperation>();
        int rawContactInsertIndex = ops.size();

        ops.add(ContentProviderOperation.newInsert(ContactsContract.RawContacts.CONTENT_URI)
                .withValue(ContactsContract.RawContacts.ACCOUNT_TYPE, null)
                .withValue(ContactsContract.RawContacts.ACCOUNT_NAME, null).build());
        ops.add(ContentProviderOperation
                .newInsert(ContactsContract.Data.CONTENT_URI)
                .withValueBackReference(
                        ContactsContract.Data.RAW_CONTACT_ID,
                        rawContactInsertIndex)
                .withValue(ContactsContract.Contacts.Data.MIMETYPE, ContactsContract.CommonDataKinds.Phone.CONTENT_ITEM_TYPE)
                .withValue(ContactsContract.CommonDataKinds.Phone.NUMBER, telephone).build());
        ops.add(ContentProviderOperation
                .newInsert(ContactsContract.Data.CONTENT_URI)
                .withValueBackReference(ContactsContract.Data.RAW_CONTACT_ID,
                        rawContactInsertIndex)
                .withValue(ContactsContract.Data.MIMETYPE, ContactsContract.CommonDataKinds.StructuredName.CONTENT_ITEM_TYPE)
                .withValue(ContactsContract.CommonDataKinds.StructuredName.DISPLAY_NAME, nameSurname)
                .build());

        ContentProviderResult[] res = context.getContentResolver()
                .applyBatch(ContactsContract.AUTHORITY, ops);
    }

        */

        public static string InsertZap(String data)
        {
            //https://forums.xamarin.com/discussion/158608/how-to-get-the-value-of-contactscontract-rawcontacts-aggregation-mode-disabled-in-xamarin

            try
            {
                string lastName = "zapi_" + data;
                //string firstName = lastName;
                string phone = "+" + data;

                List<ContentProviderOperation> ops = new List<ContentProviderOperation>();

                // (AccountName = null)
                ContentProviderOperation.Builder builder =
                    ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
                builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
                builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
                builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AggregationMode,
                AggregationMode.Disabled.GetHashCode());
                ops.Add(builder.Build());

                //Name (FamilyName)
                builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                                  ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
                builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, lastName);
                //builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, firstName);
                ops.Add(builder.Build());

                //Number
                builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                                  ContactsContract.CommonDataKinds.Phone.ContentItemType);
                builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, phone);
                builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
                                  ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
                builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Work");
                ops.Add(builder.Build());

                //Email
                //builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                //builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                //builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                //                  ContactsContract.CommonDataKinds.Email.ContentItemType);
                //builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data, email);
                //builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type,
                //                  ContactsContract.CommonDataKinds.Email.InterfaceConsts.TypeCustom);
                //builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Label, "Work");
                //ops.Add(builder.Build());

                //Company
                //builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                //builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                //builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                //                  ContactsContract.CommonDataKinds.Organization.ContentItemType);
                //builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Data, company);
                //builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Type,
                //                  ContactsContract.CommonDataKinds.Organization.InterfaceConsts.TypeCustom);
                //builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Label, "Work");
                //ops.Add(builder.Build());

                //Add the new contact
                ContentProviderResult[] res;
                //res = this.               ContentResolver.ApplyBatch(ContactsContract.Authority, ops);
                res = Application.Context.ContentResolver.ApplyBatch(ContactsContract.Authority, ops);

                return null; // null => ok
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static string InsertZap_(String data)
        {
            try
            {
                String surename = "zapi_" + data;
                String tel = "+" + data;

                List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
                {
                    //// Name
                    //ops.add(
                    //    ContentProviderOperation
                    //        .newInsert(ContactsContract.RawContacts.CONTENT_URI)
                    //        .withValue(ContactsContract.RawContacts.ACCOUNT_TYPE, null)
                    //        .withValue(ContactsContract.RawContacts.ACCOUNT_NAME, null)
                    //        .build());

                    // Name
                    var builder = ContentProviderOperation
                        .NewInsert(ContactsContract.RawContacts.ContentUri)
                        .WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null)
                        .WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
                    ops.Add(builder.Build());
                }

                {
                    //// NUMBER
                    //ops.add(
                    //    ContentProviderOperation
                    //        .newInsert(ContactsContract.Data.CONTENT_URI)
                    //        .withValueBackReference(
                    //                ContactsContract.Data.RAW_CONTACT_ID,
                    //                rawContactInsertIndex)
                    //        .withValue(
                    //                ContactsContract.Contacts.Data.MIMETYPE,
                    //                ContactsContract.CommonDataKinds.Phone.CONTENT_ITEM_TYPE)
                    //        .withValue(
                    //                ContactsContract.CommonDataKinds.Phone.NUMBER, telephone)
                    //        .build());


                    // Number
                    var builder = ContentProviderOperation
                        .NewInsert(ContactsContract.Data.ContentUri)
                        .WithValueBackReference(
                            ContactsContract.RawContacts.InterfaceConsts.ContactId, 0)
                        .WithValue(
                            ContactsContract.Data.InterfaceConsts.Mimetype,
                            ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
                        .WithValue(
                            ContactsContract.CommonDataKinds.Phone.Number,
                            tel);
                    ops.Add(builder.Build());
                }

                {
                    //// DISPLAY_NAME
                    //ops.add(
                    //    ContentProviderOperation
                    //        .newInsert(ContactsContract.Data.CONTENT_URI)
                    //        .withValueBackReference(
                    //                ContactsContract.Data.RAW_CONTACT_ID,
                    //                rawContactInsertIndex)
                    //        .withValue(
                    //                ContactsContract.Data.MIMETYPE,
                    //                ContactsContract.CommonDataKinds.StructuredName.CONTENT_ITEM_TYPE)
                    //        .withValue(
                    //                ContactsContract.CommonDataKinds.StructuredName.DISPLAY_NAME,
                    //                nameSurname)
                    //        .build());

                    // display Name
                    var builder = ContentProviderOperation
                        .NewInsert(ContactsContract.Data.ContentUri)
                        .WithValueBackReference(
                            ContactsContract.Data.InterfaceConsts.RawContactId, 0)
                        .WithValue(
                            ContactsContract.Data.InterfaceConsts.Mimetype,
                            ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
                        .WithValue(
                            ContactsContract.CommonDataKinds.StructuredName.DisplayName,
                            surename);
                    ops.Add(builder.Build());
                }

                //Add the new contact
                ContentProviderResult[] res;
                res = Application.Context.ContentResolver.ApplyBatch(ContactsContract.Authority, ops);
                return null; // null => ok
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public static bool InsertZap_original(String data)
        //{
        //    try
        //    {
        //        String surename = "zapi_" + data;
        //        String tel = "+" + data;

        //        List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
        //        //Name
        //        //var nameBuilder = ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri)
        //        //    .WithValue(ContactsContract.RawContacts.aACCOUNT_TYPE, null)
        //        //    .WithValue(ContactsContract.RawContacts.ACCOUNT_NAME, null);

        //        //.withValue(ContactsContract.RawContacts.ACCOUNT_TYPE, null)
        //        //.withValue(ContactsContract.RawContacts.ACCOUNT_NAME, null).build());

        //        {
        //            // Name
        //            var builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
        //            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
        //            builder.WithValue(ContactsContract.RawContacts.a.Data.InterfaceConsts.Mimetype,
        //                              ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
        //            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, surename);
        //            ops.Add(builder.Build());
        //        }

        //        {
        //            // Number
        //            var builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
        //            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
        //            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
        //                              ContactsContract.CommonDataKinds.Phone.ContentItemType);
        //            builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, tel);
        //            builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
        //                              ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
        //            builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Work");
        //            ops.Add(builder.Build());
        //        }

        //        //Add the new contact
        //        ContentProviderResult[] res;
        //        res = Application.Context.ContentResolver.ApplyBatch(ContactsContract.Authority, ops);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return false;
        //}


        private static void PrintStructuredString(StreamWriter outWriter, string text)
        {
            String textLen = text.Length.ToString();
            String textLenLen = textLen.Length.ToString();
            outWriter.Write(textLenLen);
            outWriter.Write(textLen);
            outWriter.Write(text);
            outWriter.Flush();
            // java
            //String textLen = Integer.toString(text.length());
            //String textLenLen = Integer.toString(textLen.length());
            //outWriter.print(textLenLen);
            //outWriter.print(textLen);
            //outWriter.print(text);
            //outWriter.flush();
        }
    }
}
