using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using OrderWiseErp.App.Infrastructure;
using OrderWiseErp.App.Models;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.ViewModels;

public sealed class ContactsViewModel : ObservableObject
{
    private readonly IAppDataService _dataService;
    private Contact? _selectedContact;
    private string _selectedTypeFilter = "All";
    private string _searchText = string.Empty;
    private string _statusMessage = "Ready";
    private string _editContactType = "Customer";
    private string _editContactCategory = "Business";
    private string _editBusinessName = string.Empty;
    private string _editTitle = string.Empty;
    private string _editFirstName = string.Empty;
    private string _editLastName = string.Empty;
    private string _editEmail = string.Empty;
    private string _editMobile = string.Empty;
    private string _editPhone = string.Empty;
    private string _editAccountNo = string.Empty;
    private string _editWebsite = string.Empty;
    private string _editBillingAddress = string.Empty;
    private string _editCity = string.Empty;
    private string _editProvince = string.Empty;
    private string _editPostalCode = string.Empty;
    private string _editCountry = string.Empty;
    private string _editBatakaNumber = string.Empty;
    private string _editBusinessLicenseNumber = string.Empty;
    private string _editVatNumber = string.Empty;
    private string _editPaymentTermDays = "0";
    private string _editCreditLimit = "0";
    private string _editOpeningBalance = "0";
    private string _editNotes = string.Empty;

    public ContactsViewModel(IAppDataService dataService)
    {
        _dataService = dataService;

        ContactsView = CollectionViewSource.GetDefaultView(_dataService.Contacts);
        ContactsView.Filter = FilterContacts;

        ContactTypeFilters = new ObservableCollection<string>
        {
            "All",
            "Customer",
            "Supplier",
            "Both"
        };

        AddCommand = new RelayCommand(AddContact);
        UpdateCommand = new RelayCommand(UpdateContact, () => SelectedContact is not null);
        DeleteCommand = new RelayCommand(DeleteContact, () => SelectedContact is not null);
        ClearFormCommand = new RelayCommand(ClearForm);
    }

    public ICollectionView ContactsView { get; }

    public ObservableCollection<string> ContactTypeFilters { get; }

    public ObservableCollection<string> ContactTypes { get; } =
    [
        "Customer",
        "Supplier",
        "Both"
    ];

    public ObservableCollection<string> ContactCategories { get; } =
    [
        "Business",
        "Individual"
    ];

    public RelayCommand AddCommand { get; }

    public RelayCommand UpdateCommand { get; }

    public RelayCommand DeleteCommand { get; }

    public RelayCommand ClearFormCommand { get; }

    public event Action? DataChanged;

    public Contact? SelectedContact
    {
        get => _selectedContact;
        set
        {
            if (!SetProperty(ref _selectedContact, value))
            {
                return;
            }

            if (value is not null)
            {
                EditContactType = value.ContactType;
                EditContactCategory = value.ContactCategory;
                EditBusinessName = value.BusinessName;
                EditTitle = value.Title;
                EditFirstName = value.FirstName;
                EditLastName = value.LastName;
                EditEmail = value.Email;
                EditMobile = value.Mobile;
                EditPhone = value.Phone;
                EditAccountNo = value.AccountNo;
                EditWebsite = value.Website;
                EditBillingAddress = value.Address;
                EditCity = value.City;
                EditProvince = value.Province;
                EditPostalCode = value.PostalCode;
                EditCountry = value.Country;
                EditBatakaNumber = value.BatakaNumber;
                EditBusinessLicenseNumber = value.BusinessLicenseNumber;
                EditVatNumber = value.VatNumber;
                EditPaymentTermDays = value.PaymentTermDays.ToString();
                EditCreditLimit = value.CreditLimit.ToString("0.##");
                EditOpeningBalance = value.OpeningBalance.ToString("0.##");
                EditNotes = value.Notes;
            }

            UpdateCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }
    }

    public string SelectedTypeFilter
    {
        get => _selectedTypeFilter;
        set
        {
            if (!SetProperty(ref _selectedTypeFilter, value))
            {
                return;
            }

            ContactsView.Refresh();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!SetProperty(ref _searchText, value))
            {
                return;
            }

            ContactsView.Refresh();
        }
    }

    public string EditContactType
    {
        get => _editContactType;
        set => SetProperty(ref _editContactType, value);
    }

    public string EditBusinessName
    {
        get => _editBusinessName;
        set => SetProperty(ref _editBusinessName, value);
    }

    public string EditContactCategory
    {
        get => _editContactCategory;
        set => SetProperty(ref _editContactCategory, value);
    }

    public string EditTitle
    {
        get => _editTitle;
        set => SetProperty(ref _editTitle, value);
    }

    public string EditFirstName
    {
        get => _editFirstName;
        set => SetProperty(ref _editFirstName, value);
    }

    public string EditLastName
    {
        get => _editLastName;
        set => SetProperty(ref _editLastName, value);
    }

    public string EditEmail
    {
        get => _editEmail;
        set => SetProperty(ref _editEmail, value);
    }

    public string EditMobile
    {
        get => _editMobile;
        set => SetProperty(ref _editMobile, value);
    }

    public string EditPhone
    {
        get => _editPhone;
        set => SetProperty(ref _editPhone, value);
    }

    public string EditAccountNo
    {
        get => _editAccountNo;
        set => SetProperty(ref _editAccountNo, value);
    }

    public string EditWebsite
    {
        get => _editWebsite;
        set => SetProperty(ref _editWebsite, value);
    }

    public string EditBillingAddress
    {
        get => _editBillingAddress;
        set => SetProperty(ref _editBillingAddress, value);
    }

    public string EditCity
    {
        get => _editCity;
        set => SetProperty(ref _editCity, value);
    }

    public string EditProvince
    {
        get => _editProvince;
        set => SetProperty(ref _editProvince, value);
    }

    public string EditPostalCode
    {
        get => _editPostalCode;
        set => SetProperty(ref _editPostalCode, value);
    }

    public string EditCountry
    {
        get => _editCountry;
        set => SetProperty(ref _editCountry, value);
    }

    public string EditBatakaNumber
    {
        get => _editBatakaNumber;
        set => SetProperty(ref _editBatakaNumber, value);
    }

    public string EditBusinessLicenseNumber
    {
        get => _editBusinessLicenseNumber;
        set => SetProperty(ref _editBusinessLicenseNumber, value);
    }

    public string EditVatNumber
    {
        get => _editVatNumber;
        set => SetProperty(ref _editVatNumber, value);
    }

    public string EditPaymentTermDays
    {
        get => _editPaymentTermDays;
        set => SetProperty(ref _editPaymentTermDays, value);
    }

    public string EditCreditLimit
    {
        get => _editCreditLimit;
        set => SetProperty(ref _editCreditLimit, value);
    }

    public string EditOpeningBalance
    {
        get => _editOpeningBalance;
        set => SetProperty(ref _editOpeningBalance, value);
    }

    public string EditNotes
    {
        get => _editNotes;
        set => SetProperty(ref _editNotes, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public void Refresh()
    {
        ContactsView.Refresh();
    }

    private bool FilterContacts(object obj)
    {
        if (obj is not Contact contact)
        {
            return false;
        }

        if (SelectedTypeFilter != "All" &&
            !string.Equals(contact.ContactType, SelectedTypeFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return true;
        }

        return contact.BusinessName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
               || contact.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
               || contact.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
               || contact.Mobile.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
               || contact.AccountNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
               || contact.VatNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private void AddContact()
    {
        if (!TryBuildContactModel(0, out var model))
        {
            return;
        }

        _dataService.AddContact(model);

        Refresh();
        DataChanged?.Invoke();
        ClearForm();
        StatusMessage = "Contact added.";
    }

    private void UpdateContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        if (!TryBuildContactModel(SelectedContact.Id, out var model))
        {
            return;
        }

        _dataService.UpdateContact(model);
        Refresh();
        DataChanged?.Invoke();
        StatusMessage = "Contact updated.";
    }

    private void DeleteContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        _dataService.DeleteContact(SelectedContact.Id);
        SelectedContact = null;
        Refresh();
        DataChanged?.Invoke();
        ClearForm();
        StatusMessage = "Contact deleted.";
    }

    private void ClearForm()
    {
        SelectedContact = null;
        EditContactType = "Customer";
        EditContactCategory = "Business";
        EditBusinessName = string.Empty;
        EditTitle = string.Empty;
        EditFirstName = string.Empty;
        EditLastName = string.Empty;
        EditEmail = string.Empty;
        EditMobile = string.Empty;
        EditPhone = string.Empty;
        EditAccountNo = string.Empty;
        EditWebsite = string.Empty;
        EditBillingAddress = string.Empty;
        EditCity = string.Empty;
        EditProvince = string.Empty;
        EditPostalCode = string.Empty;
        EditCountry = string.Empty;
        EditBatakaNumber = string.Empty;
        EditBusinessLicenseNumber = string.Empty;
        EditVatNumber = string.Empty;
        EditPaymentTermDays = "0";
        EditCreditLimit = "0";
        EditOpeningBalance = "0";
        EditNotes = string.Empty;
        UpdateCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }

    private bool TryBuildContactModel(int id, out Contact model)
    {
        model = new Contact();

        if (string.IsNullOrWhiteSpace(EditBusinessName) &&
            string.IsNullOrWhiteSpace(EditFirstName) &&
            string.IsNullOrWhiteSpace(EditLastName))
        {
            StatusMessage = "Provide business name or contact person name.";
            return false;
        }

        if (!TryParseDecimal(EditOpeningBalance, out var openingBalance))
        {
            StatusMessage = "Opening balance must be a valid number.";
            return false;
        }

        if (!TryParseInt(EditPaymentTermDays, out var paymentTermDays))
        {
            StatusMessage = "Payment term days must be a valid integer.";
            return false;
        }

        if (!TryParseDecimal(EditCreditLimit, out var creditLimit))
        {
            StatusMessage = "Credit limit must be a valid number.";
            return false;
        }

        model = new Contact
        {
            Id = id,
            ContactType = string.IsNullOrWhiteSpace(EditContactType) ? "Customer" : EditContactType.Trim(),
            ContactCategory = string.IsNullOrWhiteSpace(EditContactCategory) ? "Business" : EditContactCategory.Trim(),
            BusinessName = EditBusinessName.Trim(),
            Title = EditTitle.Trim(),
            FirstName = EditFirstName.Trim(),
            LastName = EditLastName.Trim(),
            Email = EditEmail.Trim(),
            Mobile = EditMobile.Trim(),
            Phone = EditPhone.Trim(),
            AccountNo = EditAccountNo.Trim(),
            Website = EditWebsite.Trim(),
            Address = EditBillingAddress.Trim(),
            City = EditCity.Trim(),
            Province = EditProvince.Trim(),
            PostalCode = EditPostalCode.Trim(),
            Country = EditCountry.Trim(),
            BatakaNumber = EditBatakaNumber.Trim(),
            BusinessLicenseNumber = EditBusinessLicenseNumber.Trim(),
            VatNumber = EditVatNumber.Trim(),
            PaymentTermDays = paymentTermDays,
            CreditLimit = creditLimit,
            OpeningBalance = openingBalance,
            Notes = EditNotes.Trim()
        };
        return true;
    }

    private static bool TryParseDecimal(string value, out decimal result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0m;
            return true;
        }

        return decimal.TryParse(value, out result);
    }

    private static bool TryParseInt(string value, out int result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return true;
        }

        return int.TryParse(value, out result);
    }
}
