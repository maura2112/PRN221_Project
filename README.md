# PRN221_Project
### Ứng dụng "Quản lý kho của cửa hàng nước hoa Maura" *(WPF – MVVM)
# Đặc tả
### Object (Đối tượng)
- Id
- DisplayName
- IdUnit
- IdSuplier
- QRCode
- BarCode
### Unit (Đơn vị tính)
- Id
- DisplayName
### Suplier (Nhà cung cấp)
- Id
- DisplayName
- Address
- Phone
- Email
- MoreInfo
- ContractDate
### Customer (Khách hàng)
- Id
- DisplayName
- Address
- Phone
- Email
- MoreInfo
- ContractDate
### Input (Phiếu nhập)
- Id
- DateInput
### InputInfo (Thông tin phiếu nhập)
- Id
- IdObject
- IdInput
- Count
- InputPrice
- OutputPrice
- Status
### Output (Phiếu xuất)
- Id
- DateOutput
### OutputInfo (Thông tin phiếu xuất)
- Id
- IdObject
- IdInputInfo
- Count
- IdCustomer
- DateOutput
- Status
### Report view update every time table changed

### Update
- Model tạo thêm chỉ dùng để thống kê, không có trong database
### Statistic (Thống kê)
- Object
- OrdinalNumbers
- Input
- Output
- Inventory
- InputPrice
- OuputPrice
- InventoryPrice
- RevenuePrice
### Inventory (Tồn kho)
- Object
- OrdinalNumber
- Count
- CountInput
- CountOutput
- CountInventory
- MoneyInventory
- MoneyOutput
- MoneyInput
- MoneyEarn



