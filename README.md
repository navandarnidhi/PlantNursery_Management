# Plant Nursery E-Commerce Platform

## Project Overview
A modern e-commerce web application for a plant nursery, supporting user registration, shopping cart functionality, order processing, and admin management. Similar to [CTN Petals](https://ctnpetals.com/), this platform provides a seamless online shopping experience for plant enthusiasts.

---

## 1. Database & Tables

**Database Name:** `MyNurseryDB`

### Tables
- **Admins**: AdminId (PK), Name, Email, PasswordHash
- **Users**: UserId (PK), Name, Email, PasswordHash
- **Plants**: PlantId (PK), Name, Description, Price, ImageUrl, QuantityAvailable
- **CartItems**: CartItemId (PK), UserId (FK), PlantId (FK), Quantity, AddedDate
- **Orders**: OrderId (PK), UserId (FK), OrderDate, TotalAmount, Status, PaymentStatus, ShippingAddress, PhoneNumber
- **OrderItems**: OrderItemId (PK), OrderId (FK), PlantId (FK), Quantity, UnitPrice, TotalPrice

---

## 2. Setup Instructions

1. **Clone the repository**
2. **Create the database and tables** in SQL Server using the following SQL:

```sql
CREATE DATABASE MyNurseryDB;
GO
USE MyNurseryDB;
GO

CREATE TABLE Admins (
    AdminId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL
);

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL
);

CREATE TABLE Plants (
    PlantId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl NVARCHAR(255),
    QuantityAvailable INT NOT NULL
);

CREATE TABLE CartItems (
    CartItemId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PlantId INT NOT NULL,
    Quantity INT NOT NULL,
    AddedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (PlantId) REFERENCES Plants(PlantId) ON DELETE CASCADE
);

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    ShippingAddress NVARCHAR(500) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    PlantId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    FOREIGN KEY (PlantId) REFERENCES Plants(PlantId) ON DELETE CASCADE
);
```

3. **Run the additional SQL script** (`SQL_Update.txt`) to add sample data
4. **Update the connection string** in `appsettings.json`:
```json
"ConnectionStrings": {
  "MyNurseryDBContext": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyNurseryDB;Integrated Security=True;MultipleActiveResultSets=true"
}
```

5. **Restore NuGet packages** and build the project.
6. **Run the application**.

---

## 3. Main Features

### **Public Features**
- Browse plants without registration
- View plant details and pricing
- Responsive e-commerce design

### **User Features**
- Register/Login
- Browse and search plants
- Add plants to shopping cart
- Manage cart (update quantities, remove items)
- Checkout with shipping information
- Multiple payment options (COD, Online, UPI)
- View order history and status
- Track order timeline

### **Admin Features**
- Register/Login
- Manage Plants (Add, Edit, Delete, List)
- View All Users
- View All Orders
- Update order status
- Manage inventory

---

## 4. Main API/Route List

| Route                        | Method | Description                        | Access   |
|------------------------------|--------|------------------------------------|----------|
| `/`                          | GET    | Home page with e-commerce design  | Public   |
| `/Plants/Shop`               | GET    | Public plant browsing              | Public   |
| `/Account/Register`          | GET/POST | Register as user or admin         | Public   |
| `/Account/Login`             | GET/POST | Login as user or admin            | Public   |
| `/Account/UserDashboard`     | GET    | User dashboard                     | User     |
| `/Account/AdminDashboard`    | GET    | Admin dashboard                    | Admin    |
| `/Plants/UserList`           | GET    | List all plants for users          | User     |
| `/Plants/Index`              | GET    | List all plants for admin          | Admin    |
| `/Plants/Create`             | GET/POST | Add new plant (admin)             | Admin    |
| `/Plants/Edit/{id}`          | GET/POST | Edit plant (admin)                | Admin    |
| `/Plants/Delete/{id}`        | GET/POST | Delete plant (admin)              | Admin    |
| `/Cart/Index`                | GET    | View shopping cart                 | User     |
| `/Cart/AddToCart`            | POST   | Add item to cart                   | User     |
| `/Cart/UpdateQuantity`       | POST   | Update cart item quantity          | User     |
| `/Cart/RemoveFromCart`       | POST   | Remove item from cart              | User     |
| `/Cart/ClearCart`            | POST   | Clear entire cart                  | User     |
| `/Order/Checkout`            | GET    | Checkout page                      | User     |
| `/Order/ProcessOrder`        | POST   | Process order and payment          | User     |
| `/Order/OrderConfirmation`   | GET    | Order confirmation page             | User     |
| `/Order/MyOrders`            | GET    | View user's order history          | User     |
| `/Order/OrderDetails`        | GET    | View specific order details        | User     |
| `/Admin/Users`               | GET    | View all users (admin)             | Admin    |

---

## 5. E-Commerce Features

### **Shopping Cart**
- Add multiple plants to cart
- Update quantities
- Remove items
- Clear entire cart
- Real-time price calculation

### **Checkout Process**
- Shipping address collection
- Phone number for delivery
- Multiple payment options:
  - Cash on Delivery (COD)
  - Online Payment (Credit/Debit Card)
  - UPI Payment
- Order summary with totals

### **Order Management**
- Order confirmation emails
- Order status tracking
- Order timeline visualization
- Order history for users

### **Payment Processing**
- Simulated payment processing
- Payment status tracking
- Order status updates

---

## 6. Design Features

### **Modern UI/UX**
- Responsive Bootstrap design
- Card-based product layout
- Hover effects and animations
- Professional color scheme
- Mobile-friendly interface

### **User Experience**
- Intuitive navigation
- Clear call-to-action buttons
- Progress indicators
- Success/error messaging
- Loading states

---

## 7. Sample Data

The application comes with sample plants and admin credentials:

**Admin Login:**
- Email: `admin@plantnursery.com`
- Password: `admin`

**Sample Plants:**
- Monstera Deliciosa - ₹1,200
- Snake Plant - ₹800
- Peace Lily - ₹950
- Fiddle Leaf Fig - ₹1,800
- Pothos Golden - ₹650
- ZZ Plant - ₹750
- Aloe Vera - ₹450
- Spider Plant - ₹550

---

## 8. Notes
- Passwords are stored as SHA256 hashes
- Session-based authentication is used
- Cart items are user-specific
- Orders are processed immediately
- Free shipping on orders above ₹999
- Responsive design works on all devices

---

## 9. Technologies Used
- **Backend**: ASP.NET Core MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Frontend**: Bootstrap 5, jQuery
- **Icons**: Bootstrap Icons
- **Styling**: CSS3 with animations

---

## 10. Future Enhancements
- Product search and filtering
- Product categories
- Wishlist functionality
- Product reviews and ratings
- Email notifications
- Real payment gateway integration
- Inventory management
- Discount codes and promotions

