# OrderWise ERP MVP Scaffold

This repository now includes a Windows WPF MVVM scaffold for your order-wise ERP workflow.

## Structure

- `docs/phase1-mvp-screen-plan.md` - MVP modules and screen plan
- `docs/windows-installer.md` - publish + installer packaging guide
- `src/OrderWiseErp.App` - WPF app scaffold (`net8.0-windows`)

## Current MVP modules in the app shell

1. Dashboard
2. Projects
3. Contacts
4. Products
5. Purchases
6. Sales
7. Payments
8. Expenses
9. Stock
10. Reports

## Implemented data-entry modules (current iteration)

- Dashboard counters (bound to persisted local data)
- Projects CRUD (list, create, edit, delete)
- Contacts CRUD with filtering and search
- Products CRUD with pricing/tax inputs
- Purchases CRUD (invoice header + multi-line items with VAT, duty, income tax totals)
- Sales CRUD (invoice header + multi-line items with discount and VAT totals)

## Persistence

- App data is stored in a local JSON file:
  - `%LOCALAPPDATA%/OrderWiseErp/phase1-data.json` on Windows
- Current storage includes:
  - Projects
  - Contacts
  - Products
  - Purchases (with items)
  - Sales (with items)

## Notes

- This cloud runner does not have the .NET SDK installed, so build verification could not run here.
- Open this project on a Windows machine with Visual Studio 2022+ and .NET 8 SDK to build and run.
- The scaffold is intentionally UI-first with placeholder content to speed up iteration.

## Windows installer packaging

Installer files are set up in-repo:

- `installer/OrderWiseErp.iss` (Inno Setup script)
- `scripts/build-installer.ps1` (publish + compile installer)
- `scripts/build-installer.bat` (helper wrapper)

See `docs/windows-installer.md` for exact prerequisites and commands.
