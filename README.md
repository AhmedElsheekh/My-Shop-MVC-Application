# My-Shop-MVC-Application

## Overview

My Shop is an e-commerce web application built using ASP.NET Core MVC. It allows users to browse products, add items to their shopping cart, and proceed to checkout. The application includes features such as product listing, category filtering, shopping cart management, and user authentication.

## Features

- **Product Catalog**: Browse a list of products with details like name, price, description, and category.
- **Category Filtering**: Filter products by category for easier navigation.
- **Shopping Cart**: Add, remove, and update items in the shopping cart.
- **User Authentication**: Register, log in, and manage user accounts.
- **Checkout Process**: Proceed through checkout with cart review, address, and payment information.
- **Pagination**: Paginated product listing for improved user experience.
- **Admin Panel**: Manage products, categories, and orders through a dedicated admin interface.

## Technologies Used

- **.NET 6**: Backend framework.
- **ASP.NET Core MVC**: Web framework used for building the application.
- **Entity Framework Core**: ORM for database interactions.
- **SQL Server**: Database management system.
- **Bootstrap**: CSS framework for responsive design.
- **X.Web.PagedList**: Library used for pagination.

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/AhmedElsheekh/My-Shop-MVC-Application.git
   cd my-shop-mvc
   ```

2. Set up the database:

   - Open **`appsettings.json`** and update the connection string with your SQL Server credentials.
   - Run the following commands to apply migrations and seed the database:

     ```bash
     dotnet ef database update
     ```

3. Run the application:

   ```bash
   dotnet run
   ```

   Alternatively, you can open the project in Visual Studio and press `F5` to start debugging.

### Areas Structure

The application uses ASP.NET Areas for better organization and separation of concerns. The following areas are included:

- **Admin**: Contains controllers and views for managing products, categories, users and orders.
- **Customer**: Contains controllers and views for user-facing features like product browsing and checkout.

### Pagination Setup

The application uses the `X.Web.PagedList` library to implement pagination. The `PagedListPager` helper is configured to work seamlessly with ASP.NET Areas.

### Session Management

The shopping cart functionality is managed using `HttpContext.Session`. Alternatively, a `ViewComponent` can be used to fetch the number of items in the cart dynamically.

## Usage

- **Admin Panel**: Accessible at `/Admin/Dashboard/Index` after logging in with an admin account.
- **User Registration/Login**: Use the links in the navigation bar to sign up or log in.
- **Product Browsing**: Browse products from the homepage or by name.
- **Shopping Cart**: Add products to your cart and proceed to checkout.

## Contributing

Contributions are welcome! Please submit a pull request or open an issue to discuss any changes or features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any questions or feedback, feel free to contact [https://github.com/AhmedElsheekh](mailto:your-email@example.com).

---
