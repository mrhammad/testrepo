# OrderWise ERP MVP Scaffold

This repository now includes a Windows WPF MVVM scaffold for your order-wise ERP workflow.

## Structure

- `docs/phase1-mvp-screen-plan.md` - MVP modules and screen plan
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

- Dashboard counters (bound to live in-memory collections)
- Projects CRUD (list, create, edit, delete)
- Contacts CRUD with filtering and search
- Products CRUD with pricing/tax inputs

Other modules are currently represented as planned placeholders in navigation.

## Notes

- This cloud runner does not have the .NET SDK installed, so build verification could not run here.
- Open this project on a Windows machine with Visual Studio 2022+ and .NET 8 SDK to build and run.
- The scaffold is intentionally UI-first with placeholder content to speed up iteration.
