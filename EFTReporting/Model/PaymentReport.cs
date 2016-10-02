using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace EFTReporting.Model
{
  public class PaymentReport
  {
    public ObservableCollection<Payment> payments { get; set; }
    public double totalChecked { get; set; }
    public ContactInformation companyInfo { get; set; }
    public DateTime EFTime{ get; set; }
    public EFTAdmin VendorInfo { get; set; }
  }
  
}
