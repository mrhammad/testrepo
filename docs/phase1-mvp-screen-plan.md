# OrderWise ERP WPF - Phase 1 MVP Screen Plan

## Goals

- Launch a usable desktop workflow for low-volume yearly transactions (5-6 invoices/year).
- Keep all transactions project-wise (order-wise) to support profitability and settlement.
- Capture tax-sensitive purchase/sales lines (VAT, import duty, income tax fields).
- Prepare reporting structure without implementing full GL engine in phase 1.

## Phase 1 Scope (build first)

### 1) Shell and navigation

1. Login (placeholder in MVP shell)
2. Dashboard
3. Module navigation

### 2) Master data

1. Projects (order-wise)
2. Contacts (customer / supplier / both)
3. Products
4. Accounts (cash / bank)

### 3) Transactions

1. Purchases (header + items)
2. Sales (header + items)
3. Payments (header + allocations)
4. Expenses
5. Stock adjustments
6. Stock transfers

### 4) Reports (MVP placeholders + filters)

1. Cash flow
2. Project-wise profit and loss
3. Expense report
4. Sales and purchase report
5. Input/output VAT report

## Screen inventory for MVP

### Dashboard module

- Dashboard view (KPIs and quick links)

### Projects module

- Project list
- Project add/edit
- Project transaction ledger

### Contacts module

- Contact list
- Contact add/edit

### Products module

- Product list
- Product add/edit

### Purchases module

- Purchase list
- Purchase add/edit
- Purchase print/preview

### Sales module

- Sales list
- Sales add/edit
- Sales print/preview

### Payments module

- Payments list
- Payment add/edit
- Allocation screen

### Expenses module

- Expenses list
- Expense add/edit

### Stock module

- Stock adjustment list
- Stock adjustment add/edit
- Stock transfer list
- Stock transfer add/edit

### Reports module

- Cash flow
- P&L (project/all)
- Expense (project/all)
- Sales & purchase (project/all)
- VAT (input/output)

## Out-of-scope in phase 1 (phase 2+)

- Trial balance and balance sheet with complete double-entry ledger posting
- Yearly income tax return automation
- Multi-location advanced stock costing
- Role-based permissions and approval workflows
- Full document print designer

## Notes for implementation

- `timestamp` fields in SQL Server should be replaced with `datetime2` if used as created date.
- Keep `contact_type` as enum-like UI values: Customer, Supplier, Both.
- Keep project mandatory on purchases, sales, and expenses to preserve order-wise accounting.
- Allow stock adjustments for zero or negative stock as required by workflow.
