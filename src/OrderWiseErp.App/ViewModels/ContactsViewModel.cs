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
    private string _editBusinessName = string.Empty;
    private string _editFirstName = string.Empty;
    private string _editLastName = string.Empty;
    private string _editEmail = string.Empty;
    private string _editMobile = string.Empty;
    private decimal _editOpeningBalance;

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
                EditBusinessName = value.BusinessName;
                EditFirstName = value.FirstName;
                EditLastName = value.LastName;
                EditEmail = value.Email;
                EditMobile = value.Mobile;
                EditOpeningBalance = value.OpeningBalance;
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

    public decimal EditOpeningBalance
    {
        get => _editOpeningBalance;
        set => SetProperty(ref _editOpeningBalance, value);
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
               || contact.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }

    private void AddContact()
    {
        if (string.IsNullOrWhiteSpace(EditBusinessName))
        {
            StatusMessage = "Business/organization name is required.";
            return;
        }

        _dataService.AddContact(
            new Contact
            {
                ContactType = EditContactType,
                BusinessName = EditBusinessName.Trim(),
                FirstName = EditFirstName.Trim(),
                LastName = EditLastName.Trim(),
                Email = EditEmail.Trim(),
                Mobile = EditMobile.Trim(),
                OpeningBalance = EditOpeningBalance
            });

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

        if (string.IsNullOrWhiteSpace(EditBusinessName))
        {
            StatusMessage = "Business/organization name is required.";
            return;
        }

        SelectedContact.ContactType = EditContactType;
        SelectedContact.BusinessName = EditBusinessName.Trim();
        SelectedContact.FirstName = EditFirstName.Trim();
        SelectedContact.LastName = EditLastName.Trim();
        SelectedContact.Email = EditEmail.Trim();
        SelectedContact.Mobile = EditMobile.Trim();
        SelectedContact.OpeningBalance = EditOpeningBalance;
        _dataService.UpdateContact(SelectedContact);
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
        EditContactType = "Customer";
        EditBusinessName = string.Empty;
        EditFirstName = string.Empty;
        EditLastName = string.Empty;
        EditEmail = string.Empty;
        EditMobile = string.Empty;
        EditOpeningBalance = 0m;
        UpdateCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}
