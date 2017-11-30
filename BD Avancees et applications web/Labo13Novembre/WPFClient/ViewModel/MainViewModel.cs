using System;
using GalaSoft.MvvmLight;
using WPFClient.Model;
using WPFClient.Services;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System.Linq;
namespace WPFClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage == value)
                    return;
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                    return;
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        private double _amountToAddToBalance = 0;

        public double AmountToAddToBalance
        {
            get { return _amountToAddToBalance; }
            set
            {
                if (value == _amountToAddToBalance)
                    return;
                _amountToAddToBalance = value;
                RaisePropertyChanged(() => AmountToAddToBalance);
                this.SaveChangesCommand.RaiseCanExecuteChanged();
            }
        }


        private IDataservice _dataService;
        private Customer _customer;

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                if (value == Customer)
                    return;
                _customer = value;
                RaisePropertyChanged(() => Customer);
            }
        }



        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataservice dataService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            _dataService = dataService;
            LoadCustomerDataAsync();
        }

        private async Task LoadCustomerDataAsync()
        {
            IsBusy = true;
            this.Customer = (await _dataService.GetAllCustomersAsync()).First();
            IsBusy = false;
        }

        private RelayCommand _saveChangesCommand;
        public RelayCommand SaveChangesCommand { get => _saveChangesCommand ?? (_saveChangesCommand = new RelayCommand(async () => await SaveAsync(), () => AmountToAddToBalance > 0)); }


        private RelayCommand _refreshCustomerCommand;
        public RelayCommand RefreshCustomerCommand { get => _refreshCustomerCommand ?? (_refreshCustomerCommand = new RelayCommand(async () => await LoadCustomerDataAsync())); }

        private async Task SaveAsync()
        {
            IsBusy = true;
            ErrorMessage = null;
            this._customer.AccountBalance += this._amountToAddToBalance;
            // après la MAJ, on recharge le détail du client pour avoir des infos fraîches à jour!!!
            try
            {
                await _dataService.SaveCustomerChangesAsync(this._customer);
                this.Customer = await _dataService.GetCustomerInfoAsync(this._customer.Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            IsBusy = false;
        }
    }


}