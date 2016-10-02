using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using EFTReporting.ActiveDocs;
using EFTReporting.Model;
using EFTReporting.Properties;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace EFTReporting.ViewModels
{
  public class PaymentViewModel : ViewModelBase
  {
    public PaymentViewModel()
    {
      PclawDatabase.Add(Resources.DB1);
      PclawDatabase.Add(Resources.DB2);
      PclawDatabase.Add(Resources.DB3);

      _paymentList = new ObservableCollection<Payment>();
      GetSDSVendorList();
    }

    private void GetSDSVendorList()
    {
      PclawVendorNo.Clear();
      using (var dbCtx = new SDSEntities())
      {
        // get data from SDS first

        var pData = dbCtx.EFTAdmins.Where(d => d.PclawDb == SelectedPclawDatabase).Select(u =>
              new
              {
                id = u.PclawVendorNo,
                name = u.PclawVendorName
              }).ToList();

        foreach (var datas in pData.OrderBy(p => p.name.ToString()))
        {
          VendorNameNo data = new VendorNameNo();
          {
            data.Id = datas.id;
            data.Name = datas.name + " - " + datas.id;
          };
          PclawVendorNo.Add(data);
        }
      }
    }

    #region property
    private ObservableCollection<string> _pclawDatabase = new ObservableCollection<string>();
    public ObservableCollection<string> PclawDatabase
    {
      get { return _pclawDatabase; }
      set
      {
        if (_pclawDatabase == value)
          return;

        _pclawDatabase = value;

        this.RaisePropertyChanged(() => this.PclawDatabase);
      }
    }

    private ObservableCollection<VendorNameNo> _pclawVendorNo = new ObservableCollection<VendorNameNo>();
    public ObservableCollection<VendorNameNo> PclawVendorNo
    {
      get { return _pclawVendorNo; }
      set
      {
        if (_pclawVendorNo == value)
          return;

        _pclawVendorNo = value;

        this.RaisePropertyChanged(() => this.PclawVendorNo);
      }
    }

    private ObservableCollection<Payment> _paymentList;
    public ObservableCollection<Payment> PaymentLists
    {
      get { return _paymentList; }
      set
      {
        if (_paymentList == value)
          return;

        _paymentList = value;

        this.SendEmailCommand.RaiseCanExecuteChanged();
        this.EmailAccountingCommaod.RaiseCanExecuteChanged();
        this.RaisePropertyChanged(() => this.PaymentLists);
      }
    }

    string _selectPclawDatabase = String.Empty;
    public string SelectedPclawDatabase
    {
      get { return _selectPclawDatabase; }
      set
      {
        if (_selectPclawDatabase == value)
          return;

        _selectPclawDatabase = value;

        ResetAllfield();
        GetSDSVendorList();

        this.RaisePropertyChanged(() => this.SelectedPclawDatabase);
        this.LoadDataCommand.RaiseCanExecuteChanged();
      }
    }

    private VendorNameNo _selectedVendor = new VendorNameNo();

    public VendorNameNo SelectedPclawSelectedVendorNo
    {
      get { return _selectedVendor; }
      set
      {
        if (value == null)
          return;
        if (_selectedVendor.Id == value.Id)
          return;

        _selectedVendor.Id = value.Id;
        this.RaisePropertyChanged(() => this.SelectedPclawSelectedVendorNo);

        this.LoadDataCommand.RaiseCanExecuteChanged();
      }
    }

    private bool _isAllSelected = false;
    public bool IsAllSelected
    {
      get { return _isAllSelected; }
      set
      {
        if (IsAllSelected != value)
        {
          _isAllSelected = value;
        }
        this.RaisePropertyChanged(() => this.IsAllSelected);
      }
    }

    private Payment _selectPayment;
    public Payment SelectPayment
    {
      get { return _selectPayment; }
      set
      {
        _selectPayment = value;
        this.RaisePropertyChanged(() => this.SelectPayment);
      }
    }

    private double _totalChecked;
    public double TotalChecked
    {
      get { return _totalChecked; }
      set
      {
        _totalChecked = value;
        this.RaisePropertyChanged(() => this.TotalChecked);
      }
    }

    private DateTime _payDate;
    public DateTime PayDate
    {
      get { return _payDate; }
      set
      {
        if (_payDate == value)
          return;
        _payDate = value;

        this.RaisePropertyChanged(() => this.PayDate);
        this.LoadDataCommand.RaiseCanExecuteChanged();
      }
    }

    private DateTime _startEFTDate;
    public DateTime EFTDate
    {
      get { return _startEFTDate; }
      set
      {
        if (_startEFTDate == value)
          return;
        _startEFTDate = value;

        this.RaisePropertyChanged(() => this.EFTDate);
      }
    }

    private EFTAdmin _eFTAdminData = new EFTAdmin();
    public EFTAdmin EFTAdminDataSet
    {
      get { return _eFTAdminData; }
      set
      {
        if (_eFTAdminData == value)
          return;

        _eFTAdminData = value;

        this.RaisePropertyChanged(() => this.EFTAdminDataSet);
      }
    }
    #endregion

    #region LoadDataCommand
    RelayCommand _LoadDataCommand = null;
    public RelayCommand LoadDataCommand
    {
      get
      {
        if (_LoadDataCommand == null)
          _LoadDataCommand = new RelayCommand(LoadData, () => !string.IsNullOrEmpty(this.SelectedPclawDatabase)
          && !string.IsNullOrEmpty(this.SelectedPclawSelectedVendorNo.Id)
          && !string.IsNullOrEmpty(PayDate.ToString()));

        return _LoadDataCommand;
      }
    }


    private void LoadData()
    {
      //if (IsError == true)
      //{
      //    MessageBox.Show(Properties.Resources.errOnSomeFields);
      //    return;
      //}
      try
      {
        // get vendor info from SDS
        using (var dbCtx = new SDSEntities())
        {
          if (SelectedPclawSelectedVendorNo.Id != null)
            dbCtx.EFTAdmins.Where(
                k =>
                    k.PclawVendorNo == SelectedPclawSelectedVendorNo.Id &&
                    k.PclawDb == SelectedPclawDatabase).Load();

          if (dbCtx.EFTAdmins.Local.Count > 0)
          {
            EFTAdmin tempSet = new EFTAdmin();
            tempSet = dbCtx.EFTAdmins.Local[0];
            EFTAdminDataSet = tempSet;
          }
        }

        // load payment info from specific database
        int nVendorId = 0;
        int.TryParse(SelectedPclawSelectedVendorNo.Id, out nVendorId);
        int nPayDate = int.Parse(PayDate.ToString("yyyyMMdd"));
        var ctxPaymentEntities =
            new PCLAW_FRSReportsEntities(
                ConfigurationManager.ConnectionStrings[$"{SelectedPclawDatabase}_PCLAW_ReportsEntities"]
                    .ConnectionString);

        var items = ctxPaymentEntities.EFT_Reporting_AccountPaymentDetail(nVendorId, nPayDate);
        PaymentLists = new ObservableCollection<Payment>(items.Select(item => new Payment()
        {
          IsPaymentSelected = true,
          Date = item.APInvoiceEntryDate,
          Reference = item.APInvoiceInvNumr,
          Amount = item.APInvoiceTotPaid,
          CheckNo = item.GBankCommInfCheck
        }).ToList());
        IsAllSelected = true;
        SelectAll();

      }
      catch (Exception e)
      {
        MessageBox.Show(e.ToString());
      }
    }
    #endregion

    #region SelectCommand

    RelayCommand _SelectCommand = null;
    public RelayCommand SelectCommand
    {
      get
      {
        if (_SelectCommand == null)
          _SelectCommand = new RelayCommand(OnSelectionChanged);

        return _SelectCommand;
      }
    }


    private void OnSelectionChanged()
    {
      TotalChecked = 0;
      if (PaymentLists.Any(pp => pp.IsPaymentSelected == false))
        IsAllSelected = false;

      if (PaymentLists.All(pp => pp.IsPaymentSelected))
        IsAllSelected = true;

      foreach (var pp in PaymentLists.Where(pp => pp.IsPaymentSelected))
      {
        if (pp.Amount.HasValue)
          TotalChecked += pp.Amount.Value;
      }
    }

    #endregion

    #region SelectAllCommand
    RelayCommand _SelectAllCommand = null;
    public RelayCommand SelectAllCommand
    {
      get
      {
        if (_SelectAllCommand == null)
          _SelectAllCommand = new RelayCommand(SelectAll);

        return _SelectAllCommand;
      }
    }

    private void SelectAll()
    {
      PaymentLists.ToList().ForEach(pItem => pItem.IsPaymentSelected = IsAllSelected);
      OnSelectionChanged();
    }
    #endregion

    #region EmailAccountingCommaod
    RelayCommand _EmailAccountingCommaod = null;
    public RelayCommand EmailAccountingCommaod
    {
      get
      {
        if (_EmailAccountingCommaod == null)
          _EmailAccountingCommaod = new RelayCommand(EmailAccountingDocuments, () => PaymentLists.Count != 0);

        return _EmailAccountingCommaod;
      }
    }

    private void EmailAccountingDocuments()
    {
      MessageBoxResult dialogResult = MessageBox.Show("Do you want send email to accounting?", "Confirm", MessageBoxButton.YesNo);
      if (dialogResult == MessageBoxResult.Yes)
      {
        var report = CollectPaymentData();

        PaymentTemplateXml pGetXML = new PaymentTemplateXml();
        var sADEJobXML = pGetXML.GetJobXmlFor(report, true);

        if (StartPaymentActiveDocsJob.StartJob(sADEJobXML) == null)
        {
          MessageBox.Show("Document Generate Fail!");
        }
        else
        {
          MessageBox.Show("Done!");
        }
      }
      else if (dialogResult == MessageBoxResult.No)
      {
        //do something else
      }
    }

    private PaymentReport CollectPaymentData()
    {
      PaymentReport report = new PaymentReport();
      report.EFTime = EFTDate;
      report.VendorInfo = EFTAdminDataSet;
      report.totalChecked = TotalChecked;

      ObservableCollection<Payment> payments = new ObservableCollection<Payment>();
      foreach (var list in PaymentLists.Where(pp => pp.IsPaymentSelected))
      {
        payments.Add(list);
      }
      report.payments = payments;

      using (var dbCtx = new SDSEntities())
      {
        dbCtx.ContactInformations.Where(d => d.CompanyCode == SelectedPclawDatabase).Load();

        if (dbCtx.ContactInformations.Local.Count > 0)
        {
          ContactInformation info = new ContactInformation();
          info = dbCtx.ContactInformations.Local[0];
          report.companyInfo = info;
        }
      }
      return report;
    }

    #endregion

    #region SendEmailCommand
    RelayCommand _SendEmailCommand = null;
    public RelayCommand SendEmailCommand
    {
      get
      {
        if (_SendEmailCommand == null)
          _SendEmailCommand = new RelayCommand(SendEmail, () => PaymentLists.Count != 0);

        return _SendEmailCommand;
      }
    }

    private void SendEmail()
    {
      MessageBoxResult dialogResult = MessageBox.Show("Do you want send email?", "Confirm", MessageBoxButton.YesNo);
      if (dialogResult == MessageBoxResult.Yes)
      {
        var report = CollectPaymentData();
        PaymentTemplateXml pGetXML = new PaymentTemplateXml();
        string sADEJobXML = pGetXML.GetJobXmlFor(report, false);
        if (StartPaymentActiveDocsJob.StartJob(sADEJobXML) == null)
        {
          MessageBox.Show("Document Generate Fail!");
        }
        else
        {
          MessageBox.Show("Done!");
        }
      }
      else if (dialogResult == MessageBoxResult.No)
      {
        //do something else
      }
    }
    #endregion

    //public void send_email(string msg, string mysubject, string address)
    //{
    //  //MailMessage message = new MailMessage("abc@yahoo.com.tw", address);
    //  //message.IsBodyHtml = true;
    //  //message.BodyEncoding = System.Text.Encoding.UTF8;
    //  //message.Subject = mysubject;
    //  //message.Body = msg;

    //  //SmtpClient smtpClient = new SmtpClient("127.0.0.1", 25);


    //  //string sSenderEmail = string.Empty;
    //  //string sDefaultEmail = "jc@ccebailiff.ca";

    //  //try
    //  //{
    //  //    string sSMTPServer = "cgymail1.agent-tech.ca";
    //  //    var oSmtpClient = new SmtpClient(sSMTPServer);
    //  //    oSmtpClient.UseDefaultCredentials = true;

    //  //    sSenderEmail = string.IsNullOrWhiteSpace(pEmail) ? sDefaultEmail : pEmail;

    //  //    MailAddress fromAddress = new MailAddress(sSenderEmail);
    //  //    MailMessage oMailMsg = new MailMessage();

    //  //    oMailMsg.From = fromAddress;

    //  //    oMailMsg.To.Add(sDefaultEmail);

    //  //    oMailMsg.Subject = "Maestro Error #" + ' ' + pErrorID;
    //  //    oMailMsg.SubjectEncoding = System.Text.Encoding.UTF8;
    //  //    message.BodyEncoding = System.Text.Encoding.UTF8;
    //  //    oMailMsg.IsBodyHtml = true;
    //  //    oMailMsg.Body = pEmailBody;

    //  //    // Set High Priority
    //  //    oMailMsg.Priority = MailPriority.High;

    //  //    // Set Read Reciept
    //  //    oMailMsg.Headers.Add("Disposition-Notification-To", "jc@ccebailiff.ca");

    //  //    //method to identify this send operation.
    //  //    try
    //  //    {
    //  //        oSmtpClient.SendMailAsync(oMailMsg);
    //  //        EmailDelivered = true;
    //  //    }
    //  //    catch (Exception ex)
    //  //    {
    //  //        EmailDelivered = false;
    //  //        //throw;
    //  //        //string msg = ex.Message + "Email Failed to send";
    //  //    }
    //  //}
    //  //catch (SmtpException ex)
    //  //{
    //  //    //throw;              

    //  //    EmailDelivered = false;
    //  //}
    //  //catch (Exception ex)
    //  //{
    //  //    //throw;
    //  //    EmailDelivered = false;
    //  //}

    //}

    private void ResetAllfield()
    {
      SelectedPclawSelectedVendorNo.Id = null;
      PayDate = DateTime.Parse("2016-08-02");
      EFTDate = DateTime.Now;

      //    this.EFTAdminDataSet = new EFTAdmin() { EFTStartupDate = DateTime.Now };
      //    EFTAdminDataSet.EFTStartupDate = DateTime.Now;
      //    if (this.PclawDatabase != null)
      //        SelectedPclawDatabase = this.PclawDatabase[0];
      //    SelectedPclawSelectedVendorNo.Id = null;
      //    GetSDSVendorList();
      //    this.IsAllowSave = false;
      //    IsAllowChange = true;
      //    TypeInPclawVendorVendorNo = string.Empty;
    }
  }
}
