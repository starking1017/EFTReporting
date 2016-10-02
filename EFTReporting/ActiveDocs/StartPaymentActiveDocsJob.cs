using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Channels;
using System.Windows;
using System.Xml;
using EFTReporting.Model;

namespace EFTReporting.ActiveDocs
{
  public static class StartPaymentActiveDocsJob
  {
    private static adeActiveDocs.adeDocumentAssembly _wsadeActiveDocs = null;

    public static byte[] StartJob(string XMLContent)
    {
      string sJobId = string.Empty;
      string xmlDocumentID = string.Empty;
      string sDocID = string.Empty;

      _wsadeActiveDocs = new adeActiveDocs.adeDocumentAssembly();
      //_wsadeActiveDocs.UseDefaultCredentials = true;
      _wsadeActiveDocs.Credentials = new NetworkCredential("activedocsadmin@agent-tech.ca", "G04S07L1953", "agent");

      _wsadeActiveDocs.Url = "http://cgybackup/activedocsdev/" + "adeDocumentAssembly.asmx";

      //RepeatingTemplateXML pGetXML = new RepeatingTemplateXML();
      //string sADEJobXML = pGetXML.GetJobXMLFor();
      string sADEJobXML = XMLContent;
      try
      {
        sJobId = _wsadeActiveDocs.StartJob(sADEJobXML, "");
      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString());
        DoWait(1);
        sJobId = _wsadeActiveDocs.StartJob(sADEJobXML, "");
      }

      if (!String.IsNullOrWhiteSpace(sJobId))
      {
        int iCtr = 0;

        while (_wsadeActiveDocs.GetCompileStatus(sJobId, "") != 2)
        {
          System.Threading.Thread.Sleep(2000); //Sleep for 2 seconds before calling ActiveDocs WS again

          short iDocumentStatus = _wsadeActiveDocs.GetCompileStatus(sJobId, "");
          switch (iDocumentStatus)
          {
            case 2: //Complete
              break;

            case 3: //Error
                    //TODO Report Error to user and redirect to LOB Application - no document was created, Replace this with your own user message page 
              string sErrorText = _wsadeActiveDocs.GetErrorDetailsXML(sJobId, "");
              // (see ActiveDocs.DocumentErrors.xsd)                            
              throw new ArgumentNullException("Message=" + "ERROR: " + sErrorText);
              break;
          }

          iCtr += 1;
          if (iCtr > 60)
          {
            //Time-out, looping for over 2 minutes
            //TODO Report Error to user and redirect to LOB Application - no document was created, Replace this with your own user message page
            throw new ArgumentNullException("Message=" +
                                              "ERROR: Document Failed to created within the allowed time.");
            break;
          }
        }


        System.Threading.Thread.Sleep(1000);

        //If we get here then Document was created sucessfully (see ActiveDocs.JobDocuments.xsd)


        xmlDocumentID = _wsadeActiveDocs.GetDocumentIDs(sJobId, "");

        if (string.IsNullOrWhiteSpace(xmlDocumentID))
          throw new ArgumentNullException("No Document was found for Job '" + sJobId + "'");

        XmlDocument xd = new XmlDocument();

        xd.LoadXml(xmlDocumentID);

        XmlNode xnDoc = xd.SelectSingleNode("//DocumentID");

        if (xnDoc != null)
          sDocID = xnDoc.FirstChild.InnerText;
      }

      //if (IsEmailAccounting)
      //{
      //  Email(sDocID,true,report);
      //}
      //else
      //{
      //  Email(sDocID,false,report);
      //}

      var documentBytes = _wsadeActiveDocs.GetDocumentBytes(sDocID, "");

      return documentBytes;
    }

    private static void Email(string DocId, bool emailToAccounting, PaymentReport report)
    {
      // The delivery Overrides parameter accepts the ActiveDocs answers XML structure
      // Only the override fields you require need to be added to the override fields
      string pSenderEmail = report.companyInfo.CompanyAccountingEmail;
      string pReceiverEmail = string.Empty;
      pReceiverEmail = "Chris@phoenixbussolutions.com,Ivan@PhoenixBusSolutions.com";

      //if (emailToAccounting)
      //{
      //  pReceiverEmail = report.companyInfo.CompanyAccountingEmail;
      //}
      //else
      //{
      //  pReceiverEmail = report.VendorInfo.RemittanceEmail;
      //}
      string pAttachment = "1";
      string pSubject = "Direct Deposit Notification";

      string sDeliveryOverrides = @"<ActiveDocsAnswers Version='3.0'>";
      sDeliveryOverrides += @"   <Properties />";
      sDeliveryOverrides += @"   <Body>";
      sDeliveryOverrides += @"        <ade_SenderEmail Type='text' Value='" + pSenderEmail + "' />";
      sDeliveryOverrides += @"        <ade_RecipientEmail Type='text' Value='" + pReceiverEmail + "' />";
      sDeliveryOverrides += @"        <ade_MessageAttachmentType Type='numeric' Value='" + pAttachment + "' />";
      sDeliveryOverrides += @"        <ade_MessageSubject Type='text' Value='" + pSubject + "' />";
      sDeliveryOverrides += @"   </Body>";
      sDeliveryOverrides += @"</ActiveDocsAnswers>";

      _wsadeActiveDocs.DeliverDocument(DocId, "Email Now", sDeliveryOverrides, "");
    }

    //private static void Print(string DocId)
    //{
    //  // The delivery Overrides parameter accepts the ActiveDocs answers XML structure
    //  // Only the override fields you require need to be added to the override fields
    //  string sDeliveryOverrides = @"<ActiveDocsAnswers Version='3.0'>";
    //  sDeliveryOverrides += @"   <Properties />";
    //  sDeliveryOverrides += @"   <Body>";
    //  sDeliveryOverrides += @"        <ade_printcopies Type='numeric' Value='" + 1 + "' />";
    //  // Three copies of this document will be delivered to the printer
    //  sDeliveryOverrides += @"   </Body>";
    //  sDeliveryOverrides += @"</ActiveDocsAnswers>";

    //  _wsadeActiveDocs.DeliverDocument(DocId, "Print to P4014", sDeliveryOverrides, "");
    //}
    private static void DoWait(long lNumberOfSeconds)
    {
      for (int iCtr = 1; (iCtr <= lNumberOfSeconds); iCtr++)
      {
        System.Threading.Thread.Sleep(1000);
        //Application.DoEvents();
      }
    }
  }
}
