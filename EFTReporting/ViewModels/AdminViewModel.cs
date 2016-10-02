using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EFTReporting.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EFTReporting.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        public AdminViewModel()
        {
            PclawDatabase.Add(Properties.Resources.DB1);
            PclawDatabase.Add(Properties.Resources.DB2);
            PclawDatabase.Add(Properties.Resources.DB3);
            EFTAdminDataSet.EFTStartupDate = DateTime.Now;

            GetSDSVendorList();
            ResetAllfield();
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

        #region Property
        public string SelectedPclawDatabase
        {
            get { return EFTAdminDataSet.PclawDb; }
            set
            {
                if (EFTAdminDataSet.PclawDb == value)
                    return;

                EFTAdminDataSet.PclawDb = value;
                GetSDSVendorList();
                this.RaisePropertyChanged(() => this.SelectedPclawDatabase);
                this.SaveDataCommand.RaiseCanExecuteChanged();
                this.CancelCommand.RaiseCanExecuteChanged();
                this.LoadDataCommand.RaiseCanExecuteChanged();
            }
        }

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

        private VendorNameNo _selectedVendor = new VendorNameNo();

        public VendorNameNo SelectedPclawSelectedVendorNo
        {
            get { return _selectedVendor; }
            set
            {
                if (value == null)
                    return;
                if (EFTAdminDataSet.PclawVendorNo == value.Id)
                    return;

                EFTAdminDataSet.PclawVendorNo = value.Id;
                _selectedVendor.Id = value.Id;
                this.RaisePropertyChanged(() => this.SelectedPclawSelectedVendorNo);

                this.SaveDataCommand.RaiseCanExecuteChanged();
                this.CancelCommand.RaiseCanExecuteChanged();
                this.LoadDataCommand.RaiseCanExecuteChanged();
            }
        }

        string _userTypeinVendorNo = String.Empty;
        public string TypeInPclawVendorVendorNo
        {
            get { return _userTypeinVendorNo; }
            set
            {
                if (value == null)
                    return;
                if (_userTypeinVendorNo == value)
                    return;

                _userTypeinVendorNo = value;
                EFTAdminDataSet.PclawVendorNo = value;
                this.RaisePropertyChanged(() => this.TypeInPclawVendorVendorNo);

                this.SaveDataCommand.RaiseCanExecuteChanged();
                this.CancelCommand.RaiseCanExecuteChanged();
                this.LoadDataCommand.RaiseCanExecuteChanged();
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

        private bool _isAllowSave = false;
        public bool IsAllowSave
        {
            get { return _isAllowSave; }
            set
            {
                if (_isAllowSave == value)
                    return;

                _isAllowSave = value;

                this.RaisePropertyChanged(() => this.IsAllowSave);
                this.SaveDataCommand.RaiseCanExecuteChanged();
                this.CancelCommand.RaiseCanExecuteChanged();
                this.LoadDataCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isAllowChange = true;
        public bool IsAllowChange
        {
            get { return _isAllowChange; }
            set
            {
                if (_isAllowChange == value)
                    return;

                _isAllowChange = value;

                this.RaisePropertyChanged(() => this.IsAllowChange);
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
                    _LoadDataCommand = new RelayCommand(LoadData, () => (this.SelectedPclawDatabase != "") &&
                    (!string.IsNullOrEmpty(this.SelectedPclawSelectedVendorNo.Id) || !string.IsNullOrEmpty(TypeInPclawVendorVendorNo)) && !IsAllowSave);

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
                using (var dbCtx = new SDSEntities())
                {
                    bool isByTypein = false;
                    // get data from SDS first
                    if (SelectedPclawSelectedVendorNo.Id != null)
                        dbCtx.EFTAdmins.Where(k => k.PclawVendorNo == SelectedPclawSelectedVendorNo.Id && k.PclawDb == SelectedPclawDatabase).Load();
                    else
                    {
                        isByTypein = true;
                        dbCtx.EFTAdmins.Where(k => k.PclawVendorNo == TypeInPclawVendorVendorNo && k.PclawDb == SelectedPclawDatabase).Load();
                    }
                    if (dbCtx.EFTAdmins.Local.Count > 0)
                    {
                        EFTAdmin tempSet = new EFTAdmin();
                        tempSet = dbCtx.EFTAdmins.Local[0];
                        EFTAdminDataSet = tempSet;
                    }
                    else
                    {
                        //get vendor name from pclaw db
                        string VendorId = string.Empty;
                        if (isByTypein)
                        {
                            VendorId = TypeInPclawVendorVendorNo;
                        }
                        else
                        {
                            VendorId = SelectedPclawSelectedVendorNo.Id;
                        }
                        if (!GetVendorNameFromPclaw(VendorId))
                        {
                            MessageBox.Show("Can't find data from PCLAW.");

                        }
                    }
                    IsAllowChange = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            IsAllowSave = true;
        }

        private bool GetVendorNameFromPclaw(string VendorId)
        {
            var ctxPaymentEntities =
                    new PCLAW_FRSReportsEntities(
                        ConfigurationManager.ConnectionStrings[$"{SelectedPclawDatabase}_PCLAW_ReportsEntities"]
                            .ConnectionString);
            var dbCtx1 = ctxPaymentEntities.EFT_Reporting_VendorList_vw.FirstOrDefault(k => k.APVendorListID.ToString() == VendorId);

            if (dbCtx1 != null)
            {
                EFTAdminDataSet.PclawVendorName = dbCtx1.APVendorListSortName;
                EFTAdminDataSet.LegalCompanyName = dbCtx1.APVendorListSortName;
                EFTAdminDataSet.VendorAddress1 = dbCtx1.AddressInfoAddrLine1;
                EFTAdminDataSet.VendorAddress2 = dbCtx1.AddressInfoAddrLine2;
                EFTAdminDataSet.VendorCity = dbCtx1.AddressInfoCity;
                EFTAdminDataSet.VendorProvince = dbCtx1.AddressInfoProv;
                EFTAdminDataSet.VendorPostal = dbCtx1.AddressInfoCode;
                this.RaisePropertyChanged(() => this.EFTAdminDataSet);
                return true;
            }
            return false;

        }

        #endregion
        #region SaveDataCommand
        RelayCommand _SaveDataCommand = null;
        public RelayCommand SaveDataCommand
        {
            get
            {
                if (_SaveDataCommand == null)
                    _SaveDataCommand = new RelayCommand(SaveData, () => IsAllowSave);

                return _SaveDataCommand;
            }
        }

        private void SaveData()
        {
            //if (IsError == true)
            //{
            //    MessageBox.Show(Properties.Resources.errOnSomeFields);
            //    return;
            //}
            try
            {
                using (var dbCtx = new SDSEntities())
                {
                    // get data from SDS first
                    if (SelectedPclawSelectedVendorNo.Id != null)
                        dbCtx.EFTAdmins.Where(k => k.PclawVendorNo == SelectedPclawSelectedVendorNo.Id && k.PclawDb == SelectedPclawDatabase).Load();
                    else
                    {
                        dbCtx.EFTAdmins.Where(k => k.PclawVendorNo == TypeInPclawVendorVendorNo && k.PclawDb == SelectedPclawDatabase).Load();
                    }
                    if (dbCtx.EFTAdmins.Local.Count > 0)
                    {
                        dbCtx.EFTAdmins.Local[0] = EFTAdminDataSet;
                        dbCtx.SaveChanges();
                        MessageBox.Show("Data updated!");
                    }
                    else
                    {
                        //Add Student object into Students DBset
                        dbCtx.EFTAdmins.Add(EFTAdminDataSet);
                        // call SaveChanges method to save student into database
                        dbCtx.SaveChanges();
                        MessageBox.Show("Data added!");
                    }
                }
                ResetAllfield();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        #endregion
        #region CancelCommand
        RelayCommand _CancelCommand = null;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                    _CancelCommand = new RelayCommand(Cancel, () => IsAllowSave);

                return _CancelCommand;
            }
        }

        private void Cancel()
        {
            ResetAllfield();
        }
        #endregion
        #region NewDataCommand
        RelayCommand _NewDataCommand = null;
        public RelayCommand NewDataCommand
        {
            get
            {
                if (_NewDataCommand == null)
                    _NewDataCommand = new RelayCommand(NewData);

                return _NewDataCommand;
            }
        }

        private void NewData()
        {
            ResetAllfield();
            PclawVendorNo.Clear();
        }
        #endregion

        private void ResetAllfield()
        {
            this.EFTAdminDataSet = new EFTAdmin() { EFTStartupDate = DateTime.Now };
            EFTAdminDataSet.EFTStartupDate = DateTime.Now;
            if (this.PclawDatabase != null)
                SelectedPclawDatabase = this.PclawDatabase[0];
            SelectedPclawSelectedVendorNo.Id = null;
            GetSDSVendorList();
            this.IsAllowSave = false;
            IsAllowChange = true;
            TypeInPclawVendorVendorNo = string.Empty;
        }
    }


    public class VendorNameNo
    {
        public string Name { get; set; }

        public string Id { get; set; }
    }
}