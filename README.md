# Shopify- MVC E-Commerce Application

A comprehensive ASP.NET Core MVC e-commerce platform with complete product catalog, shopping cart, checkout, order management, wishlist, and admin panel functionality.

## 📋 Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Features](#features)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)
- [Known Issues & Solutions](#known-issues--solutions)
- [API Reference](#api-reference)

---

## 🎯 Overview

E-Shop is a modern, full-featured e-commerce application built with ASP.NET Core 10.0 following clean architecture principles. The application separates concerns into four distinct layers (Domain, Application, Infrastructure, UI) and implements the Repository and Unit of Work patterns for data access.

### Key Highlights

- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **Secure Authentication**: ASP.NET Identity with role-based authorization
- **Responsive Design**: Bootstrap 5 with custom CSS design system
- **Transaction Management**: Atomic operations for critical flows (checkout, orders)
- **Image Management**: Multi-image product support with file upload service
- **Real-time Cart Updates**: Dynamic cart badge with count display
- **Advanced Product Filtering**: Category, search, sort, and pagination

---

## 🛠️ Tech Stack

### Backend

- **.NET 10.0** - Target framework
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core 10.0.3** - ORM
- **SQL Server** - Database
- **ASP.NET Identity** - Authentication & authorization
- **Mapster 7.4.0** - Object mapping
- **FluentValidation 11.3.1** - DTO validation

### Frontend

- **Razor Views** - Server-side rendering
- **Bootstrap 5** - UI framework
- **Font Awesome 6.5.1** - Icons
- **Google Fonts (Inter)** - Typography
- **Custom CSS Design System** - Brand styling with CSS variables

### Patterns & Practices

- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Generic Repository** - Reusable CRUD operations
- **DTO Pattern** - Data transfer objects
- **Dependency Injection** - IoC container
- **Area Routing** - Admin panel separation

---

## 🏗️ Architecture

### Layer Overview

```
E-Shop/
├── E-Shop.Domain/          # Domain entities, enums, repository contracts
├── E-Shop.Application/     # Business logic, services, DTOs, validators
├── E-Shop.Infrastructure/  # Data access, EF Core, repositories
└── E-Shop.UI/              # MVC controllers, views, wwwroot
```

### Domain Layer

- **Entities**: Product, Category, Order, OrderItem, ProductImage, Wishlist, Address
- **Identity**: ApplicationUser, ApplicationRole (with Guid keys)
- **Enums**: OrderStatus, UserOption
- **Contracts**: IGenericRepository, IUnitOfWork, IUserContext

### Application Layer

- **Services**: ProductService, CategoryService, OrderService, WishlistService, ShoppingCartService, FileService
- **DTOs**: Request/Response objects for all entities
- **Validators**: FluentValidation rules (LoginDto, RegisterDto)
- **Interfaces**: Service contracts for dependency injection

### Infrastructure Layer

- **DbContext**: EShopDbContext with entity configurations
- **Repositories**: GenericRepository implementation, UnitOfWork
- **Migrations**: Database schema versioning
- **Configurations**: Fluent API entity configurations

### UI Layer

- **Areas**: Admin panel with separate layout and routing
- **Controllers**: Account, Home, Catalog, Cart, Checkout, Orders, Wishlist
- **Admin Controllers**: Dashboard, Categories, Products, Orders management
- **Views**: Razor views with Bootstrap 5 styling
- **Static Files**: CSS, JS, uploaded images

---

## ✨ Features

### 🛍️ Public Features

#### Product Catalog

- Browse products by category
- Search products by name
- Sort by: Price (asc/desc), Name (asc/desc), Newest
- Pagination (12 items per page)
- Product detail page with image gallery
- Related products recommendations
- Stock availability indicator

#### Shopping Cart

- Add products to cart (memory-cached)
- Update quantities with +/- controls
- Remove items from cart
- Real-time cart badge count
- Order summary with totals
- Persistent cart during session

#### Checkout Process

- Customer information form
- Select existing shipping address
- Add new shipping address
- Order review before placement
- **Atomic transaction**: Stock validation → Order creation → Stock reduction
- Automatic cart clearing after successful order

#### Order Management

- View order history with pagination
- Filter orders by status (All, Pending, Processing, Shipped, Completed)
- Order detail view with tracking progress
- Visual progress bar for order status
- Access control (users can only see their own orders)

#### Wishlist

- Add products to wishlist from catalog or product details
- View wishlist with product cards
- Quick "Add to Cart" from wishlist items
- Remove items with heart overlay button
- Pagination support
- Duplicate prevention

#### Authentication

- User registration with validation
- Login/logout functionality
- Cookie-based authentication
- Role-based authorization (USER, ADMIN)
- Styled auth pages

### 🔐 Admin Features

#### Dashboard

- Overview of key metrics
- Quick navigation to management areas

#### Category Management (CRUD)

- Create categories with image upload
- Edit category details
- Delete categories
- Hierarchical category support (parent/subcategories)
- Search and pagination

#### Product Management (CRUD)

- Create products with multiple images
- Edit product details (name, SKU, price, stock, category)
- Replace product images during edit
- Delete products (images auto-deleted)
- SKU uniqueness validation
- Active/Inactive status toggle
- Search by name or SKU
- Image thumbnails in list view

#### Order Management

- View all customer orders
- Filter by order status
- View order details
- Update order status
- Customer information display
- Order items breakdown

---

## 📁 Project Structure

```
E-Shop/
│
├── E-Shop.Domain/
│   ├── Enums/
│   │   ├── OrderStatus.cs
│   │   └── UserOption.cs
│   ├── Models/
│   │   ├── Product.cs
│   │   ├── Category.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── ProductImage.cs
│   │   ├── Wishlist.cs
│   │   ├── Address.cs
│   │   └── Identity/
│   │       ├── ApplicationUser.cs
│   │       └── ApplicationRole.cs
│   └── RepositoryContract/
│       ├── IGenericRepository.cs
│       ├── IUnitOfWork.cs
│       └── IUserContext.cs
│
├── E-Shop.Application/
│   ├── Dtos/
│   │   ├── AccountDtos/ (LoginDto, RegisterDto, UserDto)
│   │   ├── ProductDtos/ (ProductAddRequest, ProductUpdateRequest, ProductResponse)
│   │   ├── CategoryDtos/
│   │   ├── OrderDtos/
│   │   └── WishlistDtos/
│   ├── Services/
│   │   ├── ProductService.cs
│   │   ├── CategoryService.cs
│   │   ├── OrderService.cs
│   │   ├── WishlistService.cs
│   │   ├── ShoppingCartService.cs
│   │   ├── FileService.cs
│   │   └── ProductImageService.cs
│   ├── ServicesContract/ (Interfaces)
│   ├── Validators/
│   │   └── AccountValidator/
│   ├── Exceptions/
│   └── DependencyInjection.cs
│
├── E-Shop.Infrastructure/
│   ├── Data/
│   │   ├── EShopDbContext.cs
│   │   └── Configuration/ (Entity configurations)
│   ├── Migrations/
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── UnitOfWork.cs
│   │   └── UserContext.cs
│   └── DependencyInjection.cs
│
└── E-Shop.UI/
    ├── Areas/
    │   └── Admin/
    │       ├── Controllers/ (Dashboard, Categories, Products, Orders)
    │       └── Views/
    ├── Controllers/
    │   ├── AccountController.cs
    │   ├── HomeController.cs
    │   ├── CatalogController.cs
    │   ├── CartController.cs
    │   ├── CheckoutController.cs
    │   ├── OrdersController.cs
    │   └── WishlistController.cs
    ├── Views/
    │   ├── Account/ (Login, Register)
    │   ├── Home/ (Index)
    │   ├── Catalog/ (Index, Details)
    │   ├── Cart/ (Index)
    │   ├── Checkout/ (Index)
    │   ├── Orders/ (Index, Details)
    │   ├── Wishlist/ (Index)
    │   └── Shared/ (_Layout, _AdminLayout)
    ├── wwwroot/
    │   ├── css/
    │   │   ├── site.css (~1740+ lines)
    │   │   └── admin.css
    │   ├── js/
    │   ├── lib/ (Bootstrap, Font Awesome)
    │   └── Upload/ (Product images directory)
    ├── Program.cs
    └── appsettings.json
```

---

## 🗄️ Database Schema

### Key Entities

**Products**

- Id (PK, Guid)
- Name (nvarchar(100), required)
- SKU (nvarchar(50), unique, required)
- Price (decimal(18,2))
- StockQuantity (int)
- IsActive (bit)
- CreatedAt (datetime2, default GETDATE())
- CategoryId (FK to Categories)

**Categories**

- Id (PK, Guid)
- Name (nvarchar(100), required)
- ImageUrl (nvarchar(max))
- ParentCategoryId (FK self-reference, nullable)

**Orders**

- Id (PK, Guid)
- Status (int enum: Pending=0, Processing=1, Shipped=2, Completed=3)
- OrderDate (datetime2)
- TotalAmount (decimal(18,2))
- UserId (FK to AspNetUsers)
- ShippingAddressId (FK to Addresses)

**OrderItems**

- Id (PK, Guid)
- Quantity (int)
- UnitPrice (decimal(18,2))
- LineTotal (computed: Quantity \* UnitPrice)
- OrderId (FK to Orders)
- ProductId (FK to Products)

**ProductImages**

- Id (PK, Guid)
- ImageUrl (nvarchar(max))
- ProductId (FK to Products)

**Wishlists**

- Id (PK, Guid)
- AddedAt (datetime2)
- UserId (FK to AspNetUsers)
- ProductId (FK to Products)

**Addresses**

- Id (PK, Guid)
- Country, City, Street, ZipCode
- IsDefault (bit)
- UserId (FK to AspNetUsers)

**Cart** (Memory-cached, not in DB)

- CartItems list
- Stored in IMemoryCache with key `"ShoppingCart_{userId}"`

---

## 🚀 Setup Instructions

### Prerequisites

- .NET 10.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 / VS Code / Rider

### Installation Steps

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd E-Shop
   ```

2. **Configure database connection**
   - Open `E-Shop.UI/appsettings.json`
   - Update the connection string:
     ```json
     "ConnectionStrings": {
       "EshopConnection": "Server=.;Database=EshopDb;TrustServerCertificate=true;Integrated Security=SSPI;"
     }
     ```

3. **Restore packages**

   ```bash
   dotnet restore
   ```

4. **Apply migrations**

   ```bash
   dotnet ef database update --project E-Shop.Infrastructure --startup-project E-Shop.UI
   ```

5. **Run the application**

   ```bash
   cd E-Shop.UI
   dotnet run
   ```

6. **Access the application**
   - Navigate to: `https://localhost:5001` (or the port shown in console)
   - Register a new account
   - For admin access, manually update the database:
     ```sql
     UPDATE AspNetUsers SET UserName = 'admin@example.com' WHERE Email = 'admin@example.com';
     -- Add admin role claim or update user role
     ```

---

## ⚙️ Configuration

### Connection String

Located in `E-Shop.UI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "EshopConnection": "Server=.;Database=EshopDb;TrustServerCertificate=true;Integrated Security=SSPI;"
  }
}
```

### Authentication

- **Cookie Authentication**: Configured in `Program.cs`
- **Login Path**: `/Account/Login`
- **Access Denied Path**: `/Account/AccessDenied`
- **Cookie Expiration**: Session-based

### File Upload

- **Upload Directory**: `wwwroot/Upload/`
- **Supported Formats**: Image files (jpg, png, gif, etc.)
- **File Naming**: GUID-based to prevent conflicts
- **Base URL**: Generated dynamically using `IHttpContextAccessor`

### Mapster Configuration

Critical mappings in `E-Shop.Application/DependencyInjection.cs`:

```csharp
// Product → ProductResponse (explicit SKU mapping required for Mapster 7.4.0 bug)
TypeAdapterConfig<Product, ProductResponse>.NewConfig()
    .Map(dest => dest.SKU, src => src.SKU)
    .Map(dest => dest.ImageUrls, src => src.Images.Select(i => i.ImageUrl));

// Ignore Images property when mapping DTOs to entities
TypeAdapterConfig<ProductAddRequest, Product>.NewConfig()
    .Ignore(dest => dest.Images);

TypeAdapterConfig<ProductUpdateRequest, Product>.NewConfig()
    .Ignore(dest => dest.Images);
```

---

## 🐛 Known Issues & Solutions

### Issue 1: Images Not Appearing After Product Creation

**Cause**: Mapster auto-mapped `IFormFile` collection to `ProductImage` collection, creating garbage records with empty URLs alongside real images from `SaveImagesAsync`.

**Solution**: Added `.Ignore(dest => dest.Images)` to Mapster configs for `ProductAddRequest` and `ProductUpdateRequest` to prevent auto-mapping. Images are now handled exclusively by `ProductImageService`.

### Issue 2: SKU Field Not Saved or Displayed

**Cause**: Mapster 7.4.0 bug where `.NewConfig()` fails to auto-map all-uppercase property names (like `SKU`) to record constructor parameters.

**Solution**: Added explicit mapping `.Map(dest => dest.SKU, src => src.SKU)` in `Product → ProductResponse` config.

### Issue 3: Product Updates Not Saving When No Images Uploaded

**Cause**: In `UpdateProductAsync`, the entire update logic (`request.Adapt(product)` + `UpdateAsync`) was inside `if(request.Images != null && request.Images.Any())`, so properties were never updated if images weren't changed.

**Solution**: Moved update logic outside the images check. Images are now handled conditionally inside the transaction, but product properties always update.

### Issue 4: Build Errors (MSB3027) When App Running

**Cause**: Running application locks DLL files, preventing build from copying updated assemblies.

**Solution**: Stop the application before building, or build individual projects that aren't locked.

---

## 📚 API Reference

### Public Controllers

#### CatalogController

- `GET /Catalog` - Product catalog with filtering
  - Query params: `categoryId`, `q` (search), `sort`, `page`
- `GET /Catalog/Details/{id}` - Product details

#### CartController `[Authorize]`

- `GET /Cart` - View cart
- `POST /Cart/Add` - Add item to cart (productId, quantity)
- `POST /Cart/Update` - Update item quantity (productId, quantity)
- `POST /Cart/Remove` - Remove item (productId)
- `GET /Cart/GetCartCount` - JSON cart count for badge

#### CheckoutController `[Authorize]`

- `GET /Checkout` - Checkout page
- `POST /Checkout/PlaceOrder` - Submit order
  - Params: addressId OR (country, city, street, zipCode)

#### OrdersController `[Authorize]`

- `GET /Orders` - User's order history
  - Query params: `status`, `page`
- `GET /Orders/Details/{id}` - Order details (access-controlled)

#### WishlistController `[Authorize]`

- `GET /Wishlist` - User's wishlist
  - Query params: `page`
- `POST /Wishlist/Add` - Add to wishlist (productId, returnUrl)
- `POST /Wishlist/Remove` - Remove from wishlist (id)

### Admin Controllers `[Area("Admin"), Authorize(Roles = "ADMIN")]`

#### ProductsController

- `GET /Admin/Products` - Product list (q, page)
- `GET /Admin/Products/Create` - Create form
- `POST /Admin/Products/Create` - Submit new product
- `GET /Admin/Products/Edit/{id}` - Edit form
- `POST /Admin/Products/Edit` - Update product
- `POST /Admin/Products/Delete/{id}` - Delete product

#### CategoriesController

- `GET /Admin/Categories` - Category list
- `GET /Admin/Categories/Create` - Create form
- `POST /Admin/Categories/Create` - Submit new category
- `GET /Admin/Categories/Edit/{id}` - Edit form
- `POST /Admin/Categories/Edit` - Update category
- `POST /Admin/Categories/Delete/{id}` - Delete category

#### OrdersController (Admin)

- `GET /Admin/Orders` - All orders (status, page)
- `GET /Admin/Orders/Details/{id}` - Order details
- `POST /Admin/Orders/UpdateStatus` - Change order status

---

## 📝 Design Patterns Used

1. **Repository Pattern**: Abstracts data access with `IGenericRepository<T>`
2. **Unit of Work Pattern**: Manages transactions with `IUnitOfWork`
3. **DTO Pattern**: Separates domain models from API contracts
4. **Service Layer Pattern**: Business logic in service classes
5. **Dependency Injection**: IoC container for loose coupling
6. **Factory Pattern**: Generic repository creation in UnitOfWork
7. **Template Method Pattern**: `ExecuteWithTransactionAsync` in services
8. **Specification Pattern**: LINQ expressions for filtering

---

## 🎨 UI/UX Features

- **Responsive Design**: Mobile-first Bootstrap 5 layout
- **Custom Design System**: CSS variables for consistent theming
- **Interactive Elements**:
  - Cart badge with real-time updates
  - Product image galleries
  - Quantity spinners
  - Status badges
  - Progress bars for order tracking
- **Form Validation**: Client-side + server-side validation
- **Toast Notifications**: TempData success/error messages
- **Loading States**: Visual feedback for async operations
- **Accessibility**: Semantic HTML, ARIA labels

---

## 🔐 Security Features

- **Authentication**: ASP.NET Identity with secure password hashing
- **Authorization**: Role-based access control (ADMIN, USER)
- **CSRF Protection**: Anti-forgery tokens on all POST forms
- **SQL Injection Prevention**: EF Core parameterized queries
- **XSS Prevention**: Razor automatic HTML encoding
- **Secure Cookies**: HttpOnly, Secure flags enabled

---

## 🚧 Future Enhancements

- Payment gateway integration (Stripe, PayPal)
- Product reviews and ratings
- Email notifications (order confirmations, status updates)
- Advanced search with Elasticsearch
- Product recommendations engine
- Customer profile management
- Discount codes and promotions
- Inventory alerts for low stock
- Multi-language support
- Dark mode theme
- Export orders to PDF/Excel

---

**Built with  using ASP.NET Core 10.0**
