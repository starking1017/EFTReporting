using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EFTReporting.Model;

namespace EFTReporting.ActiveDocs
{
  public class PaymentTemplateXml
  {
    public string GetJobXmlFor(PaymentReport report, bool emailToAccounting)
    {
      try
      {
        System.IO.MemoryStream ioStream = new System.IO.MemoryStream();
        System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(ioStream, null);

        // Start TemplateSet
        xmlWriter.WriteStartElement("Job", "urn:ActiveDocs.Enterprise.JobV2");
        // Site is optional so don't create if one is not specified, ActiveDocs will  
        // use (default) site if subsite is not specified.,,This one use to display document under particular tab on activedocs
        //if ("D" != "")
        //{
        xmlWriter.WriteAttributeString("Subsite", "BC");
        //}
        //setting evaluate=1 will use active docs template data view instead of adding all activefields through jobxml..
        //xmlWriter.WriteAttributeString("Evaluate", "1");
        xmlWriter.WriteAttributeString("Format", "19"); // 19 = Word2007 Docx, 1=WordML xml, 2=Word doc, 4=PDF


        xmlWriter.WriteStartElement("DeliveryQueues");
        xmlWriter.WriteStartElement("EmailQueue");

        xmlWriter.WriteAttributeString("Name", "Email Now");
        xmlWriter.WriteAttributeString("SenderEmail", report.companyInfo.CompanyAccountingEmail);

        //ICBC Plate Assignment – [number of files] files – [assigned bailiff name]
        string pSubject = "DRAFT - Direct Deposit Notification";
        xmlWriter.WriteAttributeString("MessageSubject", pSubject);

        string pReceiverEmail;//test = "Chris@phoenixbussolutions.com,James@phoenixbussolutions.com";
        if (emailToAccounting)
        {
          pReceiverEmail = report.companyInfo.CompanyAccountingEmail;
        }
        else
        {
          pReceiverEmail = report.VendorInfo.RemittanceEmail;
        }
        //xmlWriter.WriteAttributeString("Body", email_body);
        xmlWriter.WriteStartElement("Recipient");//
        xmlWriter.WriteAttributeString("RecipientType", "4"); //4 means To
        xmlWriter.WriteAttributeString("RecipientEmail", "" + pReceiverEmail + "");
        xmlWriter.WriteEndElement();// for Recipient

        if (emailToAccounting == false)
        {
          xmlWriter.WriteStartElement("Recipient");//
          xmlWriter.WriteAttributeString("RecipientType", "5"); //5 means CC
          xmlWriter.WriteAttributeString("RecipientEmail", "" + report.companyInfo.CompanyAccountingEmail + "");
          xmlWriter.WriteEndElement();// for Recipient
        }

        xmlWriter.WriteEndElement();// for EmailQueue

        xmlWriter.WriteEndElement();// for deliverqueue

        xmlWriter.WriteStartElement("JobItem");

        xmlWriter.WriteStartElement("TemplateSet");
        //xmlWriter.WriteStartElement("Template");
        //we have 2 package plate templateset name in database table which given an error so we get a particular templateset through it's id
        //xmlWriter.WriteValue("FC3E20A0FB8E4DC0AD7DA36F40907588");
        //Getting templateset name
        xmlWriter.WriteValue("EFT Report Template Set");
        //xmlWriter.WriteEndElement(); //End Template
        xmlWriter.WriteEndElement(); //End TemplateSet
        // End TemplateSet


        // Start JobItem
        xmlWriter.WriteStartElement("ActiveDocsAnswers");
        xmlWriter.WriteAttributeString("Version", "3.0");
        xmlWriter.WriteStartElement("Body");

        // Start Body
        xmlWriter.WriteStartElement("companyfullname");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyFullName);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companystreetaddress");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyStreetAddress);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companycity");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyCity);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companyprovince");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyProvince);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companypostalcode");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyPostalCode);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companyphone");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyPhone);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companyfax");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyFax);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("companyaccountingemail");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.companyInfo.CompanyAccountingEmail);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("eftdate");
        xmlWriter.WriteAttributeString("Type", "date");
        xmlWriter.WriteAttributeString("Value", report.EFTime.ToString());
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("remittanceemail");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.RemittanceEmail);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("totalamount");
        xmlWriter.WriteAttributeString("Type", "numeric");
        xmlWriter.WriteAttributeString("Value", report.totalChecked.ToString());
        xmlWriter.WriteEndElement();

        // vendorInfo
        xmlWriter.WriteStartElement("vendorname");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.VendorName);
        xmlWriter.WriteEndElement();

        // vendorInfo
        xmlWriter.WriteStartElement("vendoraddress1");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.VendorAddress1);
        xmlWriter.WriteEndElement();

        // vendorInfo
        xmlWriter.WriteStartElement("vendoraddress2");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.VendorAddress2);
        xmlWriter.WriteEndElement();

        // vendorInfo
        xmlWriter.WriteStartElement("vendorcity");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.VendorCity);
        xmlWriter.WriteEndElement();

        // vendorInfo
        xmlWriter.WriteStartElement("vendorprovince");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.VendorProvince);
        xmlWriter.WriteEndElement();

        // vendorInfo
        xmlWriter.WriteStartElement("vendorpostalcode");
        xmlWriter.WriteAttributeString("Type", "text");
        xmlWriter.WriteAttributeString("Value", report.VendorInfo.VendorPostal);
        xmlWriter.WriteEndElement();

        foreach (var payment in report.payments)
        {
          xmlWriter.WriteStartElement("paymentlist");
          xmlWriter.WriteAttributeString("Type", "group");
          xmlWriter.WriteAttributeString("Value", "");

          //xmlWriter.WriteStartElement("invoicedate"); //Start QuestionAnswersGroup
          //xmlWriter.WriteAttributeString("Type", "date");
          //xmlWriter.WriteAttributeString("Value", "2016-09-01");
          //xmlWriter.WriteEndElement();

          xmlWriter.WriteStartElement("invoicenumber"); //Start QuestionAnswersGroup
          xmlWriter.WriteAttributeString("Type", "text");
          xmlWriter.WriteAttributeString("Value", payment.Reference);
          xmlWriter.WriteEndElement();

          xmlWriter.WriteStartElement("payment"); //Start QuestionAnswersGroup
          xmlWriter.WriteAttributeString("Type", "numeric");
          xmlWriter.WriteAttributeString("Value", payment.Amount.ToString());
          xmlWriter.WriteEndElement();

          xmlWriter.WriteEndElement();//paymentlist
        }

        xmlWriter.WriteEndElement(); // End Body
        xmlWriter.WriteEndElement(); // End ActiveDocs Answers
        xmlWriter.WriteEndElement(); // End JobItem

        xmlWriter.WriteEndElement(); //end job

        xmlWriter.Flush();
        ioStream.Flush();
        ioStream.Position = 0;
        System.IO.StreamReader ioReader = new System.IO.StreamReader(ioStream);

        // Read out the JobXML stream and return.
        return ioReader.ReadToEnd().ToString();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
        return null;
      }
    }
  }
}
