# Plant Nursery Management

## Project Overview
A web application for managing a plant nursery, supporting admin and user roles, plant inventory, and a booking system.

---

## 1. Database & Tables

**Database Name:** `MyNurseryDB`

### Tables
- **Admins**: AdminId (PK), Name, Email, PasswordHash
- **Users**: UserId (PK), Name, Email, PasswordHash
- **Plants**: PlantId (PK), Name, Description, Price, QuantityAvailable
- **Bookings**: BookingId (PK), UserId (FK), PlantId (FK), Quantity, BookingDate, Status (Pending/Approved/Cancelled), AdminId (FK, nullable)

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
    QuantityAvailable INT NOT NULL
);

CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PlantId INT NOT NULL,
    Quantity INT NOT NULL,
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL,
    AdminId INT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PlantId) REFERENCES Plants(PlantId),
    FOREIGN KEY (AdminId) REFERENCES Admins(AdminId)
);
```

3. **Update the connection string** in `appsettings.json`:
```json
"ConnectionStrings": {
  "MyNurseryDBContext": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyNurseryDB;Integrated Security=True;MultipleActiveResultSets=true"
}
```

4. **Restore NuGet packages** and build the project.
5. **Run the application**.

---

## 3. Main Features

- **Admin**
  - Register/Login
  - Manage Plants (Add, Edit, Delete, List)
  - View All Users
  - View All Bookings (Approve/Cancel)
- **User**
  - Register/Login
  - View Plants
  - Book Plants
  - View/Edit/Delete Own Bookings
  - See Booking Status (Pending/Approved/Cancelled)

---

## 4. Main API/Route List

| Route                        | Method | Description                        | Access   |
|------------------------------|--------|------------------------------------|----------|
| `/Account/Register`          | GET/POST | Register as user or admin         | Public   |
| `/Account/Login`             | GET/POST | Login as user or admin            | Public   |
| `/Account/UserDashboard`     | GET    | User dashboard                     | User     |
| `/Account/AdminDashboard`    | GET    | Admin dashboard                    | Admin    |
| `/Plants/UserList`           | GET    | List all plants for users          | User     |
| `/Plants/Index`              | GET    | List all plants for admin          | Admin    |
| `/Plants/Create`             | GET/POST | Add new plant (admin)             | Admin    |
| `/Plants/Edit/{id}`          | GET/POST | Edit plant (admin)                | Admin    |
| `/Plants/Delete/{id}`        | GET/POST | Delete plant (admin)              | Admin    |
| `/Bookings/Create`           | GET/POST | Book a plant (user)               | User     |
| `/Bookings/MyBookings`       | GET    | View user's bookings               | User     |
| `/Bookings/Edit/{id}`        | GET/POST | Edit user's booking               | User     |
| `/Bookings/Delete/{id}`      | GET/POST | Delete user's booking             | User     |
| `/Bookings/AdminList`        | GET    | View all bookings (admin)          | Admin    |
| `/Bookings/Approve/{id}`     | POST   | Approve booking (admin)            | Admin    |
| `/Bookings/Cancel/{id}`      | POST   | Cancel booking (admin)             | Admin    |
| `/Admin/Users`               | GET    | View all users (admin)             | Admin    |

---

## 5. Notes
- Passwords are stored as SHA256 hashes.
- Session-based authentication is used.
- Only admins can manage plants, users, and approve/cancel bookings.
- Users can only manage their own bookings.

---

