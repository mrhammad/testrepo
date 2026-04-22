using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using OrderWiseErp.App.Infrastructure;

namespace OrderWiseErp.App.Services;

public enum AppLanguage
{
    English,
    Arabic
}

public sealed class LocalizationService : ObservableObject
{
    public const string EnglishLanguageCode = "English";
    public const string ArabicLanguageCode = "Arabic";

    private static readonly IReadOnlyDictionary<string, string> English = new Dictionary<string, string>
    {
        ["App.WindowTitle"] = "OrderWise ERP - Phase 1 MVP",
        ["App.BrandName"] = "OrderWise ERP",
        ["App.BrandTagline"] = "Phase 1 Data Entry MVP",
        ["Common.LastDataChange"] = "Last data change:",
        ["Common.New"] = "New",
        ["Common.Save"] = "Save",
        ["Common.Delete"] = "Delete",
        ["Common.Add"] = "Add",
        ["Common.Update"] = "Update",
        ["Common.RemoveItem"] = "Remove Item",
        ["Common.Date"] = "Date",
        ["Common.Total"] = "Total",
        ["Common.Balance"] = "Balance",
        ["Common.Project"] = "Project",
        ["Common.Product"] = "Product",
        ["Common.Supplier"] = "Supplier",
        ["Common.Customer"] = "Customer",
        ["Common.InvoiceNo"] = "Invoice #",
        ["Common.TermDays"] = "Term Days",
        ["Common.DueDate"] = "Due Date",
        ["Common.PoSoc"] = "PO / SOC",
        ["Common.Type"] = "Type",
        ["Common.Name"] = "Name",
        ["Common.Description"] = "Description",
        ["Common.Unit"] = "Unit",
        ["Common.Qty"] = "Qty",
        ["Common.Rate"] = "Rate",
        ["Common.Amount"] = "Amount",
        ["Common.Discount"] = "Discount",
        ["Common.DiscountPercent"] = "Disc %",
        ["Common.Vat"] = "VAT",
        ["Common.VatPercent"] = "VAT %",
        ["Common.Tax"] = "Tax",
        ["Common.TaxPercent"] = "Tax %",
        ["Common.Net"] = "Net",
        ["Common.Email"] = "Email",
        ["Common.Mobile"] = "Mobile",
        ["Common.Phone"] = "Phone",
        ["Common.Category"] = "Category",
        ["Common.Notes"] = "Notes",
        ["Common.Language"] = "Language",
        ["Common.English"] = "English",
        ["Common.Arabic"] = "Arabic",
        ["nav.dashboard"] = "Dashboard",
        ["nav.dashboard.subtitle"] = "Overview and quick metrics",
        ["nav.projects"] = "Projects",
        ["nav.projects.subtitle"] = "Create and manage order-wise projects",
        ["nav.contacts"] = "Contacts",
        ["nav.contacts.subtitle"] = "Customers, suppliers, and both",
        ["nav.products"] = "Products",
        ["nav.products.subtitle"] = "Products with pricing and tax fields",
        ["nav.purchases"] = "Purchases",
        ["nav.purchases.subtitle"] = "Purchase invoice entry with item lines",
        ["nav.sales"] = "Sales",
        ["nav.sales.subtitle"] = "Sales invoice entry with item lines",
        ["nav.payments"] = "Payments",
        ["nav.payments.subtitle"] = "Phase 2: Payment and allocation",
        ["nav.expenses"] = "Expenses",
        ["nav.expenses.subtitle"] = "Phase 2: Expense vouchers",
        ["nav.stock"] = "Stock",
        ["nav.stock.subtitle"] = "Phase 2: Stock transfers and adjustments",
        ["nav.reports"] = "Reports",
        ["nav.reports.subtitle"] = "Phase 2: Financial and tax reports",
        ["nav.settings"] = "Settings",
        ["nav.settings.subtitle"] = "Application preferences and language",
        ["placeholder.next-iteration"] = "Planned in next iteration",
        ["placeholder.last-change"] = "Last data change:",
        ["app.title"] = "OrderWise ERP",
        ["app.subtitle"] = "Phase 1 Data Entry MVP",
        ["Dashboard.Description"] = "Core phase-1 counters and quick health checks for your order-wise workflow.",
        ["Dashboard.Projects"] = "Projects",
        ["Dashboard.Contacts"] = "Contacts",
        ["Dashboard.Products"] = "Products",
        ["Projects.ProjectNo"] = "Project #",
        ["Projects.StartDate"] = "Start Date",
        ["Projects.EndDate"] = "End Date",
        ["Contacts.BusinessName"] = "Business Name",
        ["Contacts.FirstName"] = "First Name",
        ["Contacts.LastName"] = "Last Name",
        ["Contacts.Opening"] = "Opening",
        ["Contacts.TitleLabel"] = "Title",
        ["Contacts.AccountNo"] = "Account No.",
        ["Contacts.Website"] = "Website",
        ["Contacts.BillingAddress"] = "Billing Address",
        ["Contacts.City"] = "City",
        ["Contacts.Province"] = "Province",
        ["Contacts.PostalCode"] = "Postal Code",
        ["Contacts.Country"] = "Country",
        ["Contacts.Bataka"] = "Bataka #",
        ["Contacts.BusinessLicense"] = "Business License #",
        ["Contacts.VatNumber"] = "VAT #",
        ["Contacts.CreditLimit"] = "Credit Limit",
        ["Contacts.OpeningBalance"] = "Opening Balance",
        ["Products.ListHeader"] = "Products",
        ["Products.FormHeader"] = "Product Form",
        ["Products.Sku"] = "SKU",
        ["Products.StockAssetAccount"] = "Stock Asset Acct.",
        ["Products.SalePrice"] = "Sale Price",
        ["Products.Cost"] = "Cost",
        ["Products.SaleDiscount"] = "Sale Disc. (%)",
        ["Products.PurchaseDiscount"] = "Purchase Disc. (%)",
        ["Products.Weight"] = "Weight",
        ["Products.VatRate"] = "VAT Rate (%)",
        ["Products.AdtRate"] = "ADT Rate (%)",
        ["Products.LowStock"] = "Low Stock",
        ["Products.BinLocation"] = "Bin Location",
        ["Products.SalesInfoNote"] = "Sales Info / Note",
        ["Purchases.ListHeader"] = "Purchases",
        ["Purchases.LineItem"] = "Line Item",
        ["Purchases.Duty"] = "Duty",
        ["Purchases.DutyPercent"] = "Duty %",
        ["Sales.ListHeader"] = "Sales Invoices",
        ["Sales.AddLineItem"] = "Add Line Item",
        ["Sales.NetTotal"] = "Net Total",
        ["Sales.Subtotal"] = "Subtotal",
        ["Stock.TabAdjustments"] = "Stock Adjustments",
        ["Stock.TabTransfers"] = "Stock Transfers",
        ["Stock.Filters"] = "Filters",
        ["Stock.FilterFrom"] = "From",
        ["Stock.FilterTo"] = "To",
        ["Stock.FilterParty"] = "Customer / Supplier",
        ["Stock.FilterLocation"] = "Location",
        ["Stock.ApplyFilters"] = "Apply Filters",
        ["Stock.AdjustmentsList"] = "Adjustments List",
        ["Stock.TransfersList"] = "Transfers List",
        ["Stock.AdjustmentForm"] = "Add / Edit Stock Adjustment",
        ["Stock.TransferForm"] = "Add / Edit Stock Transfer",
        ["Stock.ReferenceNo"] = "Reference #",
        ["Stock.AdjustmentType"] = "Adjustment Type",
        ["Stock.TotalRecovered"] = "Total Recovered",
        ["Stock.Reason"] = "Reason",
        ["Stock.AddedBy"] = "Added By",
        ["Stock.LineItems"] = "Add SKU Line Items",
        ["Stock.UnitPrice"] = "Unit Price",
        ["Stock.SubTotal"] = "Sub Total",
        ["Stock.LocationFrom"] = "Location From",
        ["Stock.LocationTo"] = "Location To",
        ["Stock.ShippingCharges"] = "Shipping Charges",
        ["Stock.AdditionalNote"] = "Additional Note",
        ["Stock.Status"] = "Status",
        ["Stock.StatusPending"] = "Pending",
        ["Stock.StatusInTransit"] = "In-Transit",
        ["Stock.StatusCompleted"] = "Completed",
        ["Stock.SavePrint"] = "Save & Print",
        ["Settings.FormHeader"] = "Application Settings",
        ["Settings.Language"] = "Language",
        ["Settings.Help"] = "Language changes apply immediately to titles and labels."
    };

    private static readonly IReadOnlyDictionary<string, string> Arabic = new Dictionary<string, string>
    {
        ["App.WindowTitle"] = "اوردر وايز ERP - المرحلة الأولى",
        ["App.BrandName"] = "اوردر وايز ERP",
        ["App.BrandTagline"] = "المرحلة الأولى لإدخال البيانات",
        ["Common.LastDataChange"] = "آخر تغيير للبيانات:",
        ["Common.New"] = "جديد",
        ["Common.Save"] = "حفظ",
        ["Common.Delete"] = "حذف",
        ["Common.Add"] = "إضافة",
        ["Common.Update"] = "تحديث",
        ["Common.RemoveItem"] = "إزالة البند",
        ["Common.Date"] = "التاريخ",
        ["Common.Total"] = "الإجمالي",
        ["Common.Balance"] = "الرصيد",
        ["Common.Project"] = "المشروع",
        ["Common.Product"] = "المنتج",
        ["Common.Supplier"] = "المورد",
        ["Common.Customer"] = "العميل",
        ["Common.InvoiceNo"] = "رقم الفاتورة",
        ["Common.TermDays"] = "أيام الأجل",
        ["Common.DueDate"] = "تاريخ الاستحقاق",
        ["Common.PoSoc"] = "رقم أمر الشراء / SOC",
        ["Common.Type"] = "النوع",
        ["Common.Name"] = "الاسم",
        ["Common.Description"] = "الوصف",
        ["Common.Unit"] = "الوحدة",
        ["Common.Qty"] = "الكمية",
        ["Common.Rate"] = "السعر",
        ["Common.Amount"] = "المبلغ",
        ["Common.Discount"] = "الخصم",
        ["Common.DiscountPercent"] = "نسبة الخصم",
        ["Common.Vat"] = "ضريبة القيمة المضافة",
        ["Common.VatPercent"] = "نسبة ضريبة القيمة المضافة",
        ["Common.Tax"] = "الضريبة",
        ["Common.TaxPercent"] = "نسبة الضريبة",
        ["Common.Net"] = "الصافي",
        ["Common.Email"] = "البريد الإلكتروني",
        ["Common.Mobile"] = "الجوال",
        ["Common.Phone"] = "الهاتف",
        ["Common.Category"] = "الفئة",
        ["Common.Notes"] = "ملاحظات",
        ["Common.Language"] = "اللغة",
        ["Common.English"] = "الإنجليزية",
        ["Common.Arabic"] = "العربية",
        ["nav.dashboard"] = "لوحة التحكم",
        ["nav.dashboard.subtitle"] = "نظرة عامة ومؤشرات سريعة",
        ["nav.projects"] = "المشاريع",
        ["nav.projects.subtitle"] = "إنشاء وإدارة المشاريع حسب الطلب",
        ["nav.contacts"] = "جهات الاتصال",
        ["nav.contacts.subtitle"] = "العملاء والموردون وكلاهما",
        ["nav.products"] = "المنتجات",
        ["nav.products.subtitle"] = "المنتجات مع الأسعار وحقول الضرائب",
        ["nav.purchases"] = "المشتريات",
        ["nav.purchases.subtitle"] = "إدخال فواتير الشراء مع البنود",
        ["nav.sales"] = "المبيعات",
        ["nav.sales.subtitle"] = "إدخال فواتير البيع مع البنود",
        ["nav.payments"] = "المدفوعات",
        ["nav.payments.subtitle"] = "المرحلة الثانية: المدفوعات والتوزيع",
        ["nav.expenses"] = "المصروفات",
        ["nav.expenses.subtitle"] = "المرحلة الثانية: سندات المصروف",
        ["nav.stock"] = "المخزون",
        ["nav.stock.subtitle"] = "المرحلة الثانية: تحويلات وتسويات المخزون",
        ["nav.reports"] = "التقارير",
        ["nav.reports.subtitle"] = "المرحلة الثانية: التقارير المالية والضريبية",
        ["nav.settings"] = "الإعدادات",
        ["nav.settings.subtitle"] = "تفضيلات التطبيق واللغة",
        ["placeholder.next-iteration"] = "مخطط له في التحديث القادم",
        ["placeholder.last-change"] = "آخر تغيير للبيانات:",
        ["app.title"] = "اوردر وايز ERP",
        ["app.subtitle"] = "المرحلة الأولى لإدخال البيانات",
        ["Dashboard.Description"] = "عدادات المرحلة الأولى وفحوصات سريعة لصحة سير العمل حسب المشروع.",
        ["Dashboard.Projects"] = "المشاريع",
        ["Dashboard.Contacts"] = "جهات الاتصال",
        ["Dashboard.Products"] = "المنتجات",
        ["Projects.ProjectNo"] = "رقم المشروع",
        ["Projects.StartDate"] = "تاريخ البداية",
        ["Projects.EndDate"] = "تاريخ النهاية",
        ["Contacts.BusinessName"] = "اسم النشاط",
        ["Contacts.FirstName"] = "الاسم الأول",
        ["Contacts.LastName"] = "اسم العائلة",
        ["Contacts.Opening"] = "الرصيد الافتتاحي",
        ["Contacts.TitleLabel"] = "اللقب",
        ["Contacts.AccountNo"] = "رقم الحساب",
        ["Contacts.Website"] = "الموقع الإلكتروني",
        ["Contacts.BillingAddress"] = "عنوان الفوترة",
        ["Contacts.City"] = "المدينة",
        ["Contacts.Province"] = "المنطقة",
        ["Contacts.PostalCode"] = "الرمز البريدي",
        ["Contacts.Country"] = "الدولة",
        ["Contacts.Bataka"] = "رقم البطاقة",
        ["Contacts.BusinessLicense"] = "رقم الرخصة التجارية",
        ["Contacts.VatNumber"] = "رقم ضريبة القيمة المضافة",
        ["Contacts.CreditLimit"] = "الحد الائتماني",
        ["Contacts.OpeningBalance"] = "الرصيد الافتتاحي",
        ["Products.ListHeader"] = "المنتجات",
        ["Products.FormHeader"] = "نموذج المنتج",
        ["Products.Sku"] = "رمز المنتج",
        ["Products.StockAssetAccount"] = "حساب أصل المخزون",
        ["Products.SalePrice"] = "سعر البيع",
        ["Products.Cost"] = "التكلفة",
        ["Products.SaleDiscount"] = "خصم البيع (%)",
        ["Products.PurchaseDiscount"] = "خصم الشراء (%)",
        ["Products.Weight"] = "الوزن",
        ["Products.VatRate"] = "نسبة ضريبة القيمة المضافة (%)",
        ["Products.AdtRate"] = "نسبة الرسوم الإضافية (%)",
        ["Products.LowStock"] = "حد المخزون المنخفض",
        ["Products.BinLocation"] = "موقع التخزين",
        ["Products.SalesInfoNote"] = "معلومة / ملاحظة البيع",
        ["Purchases.ListHeader"] = "المشتريات",
        ["Purchases.LineItem"] = "بند الفاتورة",
        ["Purchases.Duty"] = "الرسوم",
        ["Purchases.DutyPercent"] = "نسبة الرسوم",
        ["Sales.ListHeader"] = "فواتير المبيعات",
        ["Sales.AddLineItem"] = "إضافة بند",
        ["Sales.NetTotal"] = "الإجمالي الصافي",
        ["Sales.Subtotal"] = "الإجمالي الفرعي",
        ["Stock.TabAdjustments"] = "تسويات المخزون",
        ["Stock.TabTransfers"] = "تحويلات المخزون",
        ["Stock.Filters"] = "الفلاتر",
        ["Stock.FilterFrom"] = "من",
        ["Stock.FilterTo"] = "إلى",
        ["Stock.FilterParty"] = "العميل / المورد",
        ["Stock.FilterLocation"] = "الموقع",
        ["Stock.ApplyFilters"] = "تطبيق الفلاتر",
        ["Stock.AdjustmentsList"] = "قائمة تسويات المخزون",
        ["Stock.TransfersList"] = "قائمة تحويلات المخزون",
        ["Stock.AdjustmentForm"] = "إضافة / تعديل تسوية مخزون",
        ["Stock.TransferForm"] = "إضافة / تعديل تحويل مخزون",
        ["Stock.ReferenceNo"] = "رقم المرجع",
        ["Stock.AdjustmentType"] = "نوع التسوية",
        ["Stock.TotalRecovered"] = "إجمالي المبلغ المسترد",
        ["Stock.Reason"] = "السبب",
        ["Stock.AddedBy"] = "أضيف بواسطة",
        ["Stock.LineItems"] = "إضافة بنود SKU",
        ["Stock.UnitPrice"] = "سعر الوحدة",
        ["Stock.SubTotal"] = "المجموع الفرعي",
        ["Stock.LocationFrom"] = "الموقع من",
        ["Stock.LocationTo"] = "الموقع إلى",
        ["Stock.ShippingCharges"] = "مصاريف الشحن",
        ["Stock.AdditionalNote"] = "ملاحظة إضافية",
        ["Stock.Status"] = "الحالة",
        ["Stock.StatusPending"] = "قيد الانتظار",
        ["Stock.StatusInTransit"] = "قيد النقل",
        ["Stock.StatusCompleted"] = "مكتمل",
        ["Stock.SavePrint"] = "حفظ وطباعة",
        ["Settings.FormHeader"] = "إعدادات التطبيق",
        ["Settings.Language"] = "اللغة",
        ["Settings.Help"] = "تغيير اللغة يطبق فوراً على العناوين والتسميات."
    };

    private string? _settingsPath;
    private AppLanguage _currentLanguage = AppLanguage.English;

    private LocalizationService()
    {
    }

    public static LocalizationService Instance { get; } = new();

    public AppLanguage CurrentLanguage
    {
        get => _currentLanguage;
        private set
        {
            if (!SetProperty(ref _currentLanguage, value))
            {
                return;
            }

            OnPropertyChanged(nameof(CurrentLanguage));
            OnPropertyChanged(nameof(CurrentLanguageCode));
            OnPropertyChanged(nameof(MainWindowTitle));
            OnPropertyChanged(nameof(SidebarTitle));
            OnPropertyChanged(nameof(SidebarSubtitle));
            OnPropertyChanged(nameof(CurrentFlowDirection));
            OnPropertyChanged("Item[]");
            LanguageChanged?.Invoke();
            SaveSettings();
        }
    }

    public event Action? LanguageChanged;

    public string CurrentLanguageCode
    {
        get => CurrentLanguage == AppLanguage.Arabic ? ArabicLanguageCode : EnglishLanguageCode;
        set => SetLanguage(value);
    }

    public string MainWindowTitle => this["App.WindowTitle"];

    public string SidebarTitle => this["App.BrandName"];

    public string SidebarSubtitle => this["App.BrandTagline"];

    public FlowDirection CurrentFlowDirection =>
        CurrentLanguage == AppLanguage.Arabic
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

    public string this[string key]
    {
        get
        {
            var dictionary = CurrentLanguage == AppLanguage.Arabic ? Arabic : English;
            if (dictionary.TryGetValue(key, out var translated))
            {
                return translated;
            }

            if (English.TryGetValue(key, out var fallback))
            {
                return fallback;
            }

            return key;
        }
    }

    public void SetLanguage(string languageCode)
    {
        CurrentLanguage = string.Equals(languageCode, ArabicLanguageCode, StringComparison.OrdinalIgnoreCase)
            ? AppLanguage.Arabic
            : AppLanguage.English;
    }

    public void Initialize(string settingsPath)
    {
        _settingsPath = settingsPath;
        LoadSettings();
        OnPropertyChanged(nameof(MainWindowTitle));
        OnPropertyChanged(nameof(SidebarTitle));
        OnPropertyChanged(nameof(SidebarSubtitle));
        OnPropertyChanged(nameof(CurrentLanguageCode));
        OnPropertyChanged(nameof(CurrentFlowDirection));
        OnPropertyChanged("Item[]");
    }

    private void LoadSettings()
    {
        if (string.IsNullOrWhiteSpace(_settingsPath))
        {
            return;
        }

        try
        {
            if (!File.Exists(_settingsPath))
            {
                return;
            }

            var content = File.ReadAllText(_settingsPath);
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var settings = JsonSerializer.Deserialize<LocalizationSettings>(content);
            if (settings?.LanguageCode is null)
            {
                return;
            }

            _currentLanguage = string.Equals(settings.LanguageCode, ArabicLanguageCode, StringComparison.OrdinalIgnoreCase)
                ? AppLanguage.Arabic
                : AppLanguage.English;
        }
        catch
        {
            // Ignore invalid settings and continue with default language.
        }
    }

    private void SaveSettings()
    {
        if (string.IsNullOrWhiteSpace(_settingsPath))
        {
            return;
        }

        try
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var settings = new LocalizationSettings
            {
                LanguageCode = CurrentLanguageCode
            };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Ignore save failures to avoid blocking normal app flow.
        }
    }

    private sealed class LocalizationSettings
    {
        public string LanguageCode { get; set; } = EnglishLanguageCode;
    }
}
