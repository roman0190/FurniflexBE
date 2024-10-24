# FurniFlex Backend

This is the backend for the FurniFlex project, an e-commerce platform for browsing, ordering, and managing furniture purchases. It is built with ASP.NET Framework (4.7.2), using Web API, Entity Framework, and Identity Framework with JWT authentication. This backend serves as the API for the FurniFlex Next.js 14 frontend.

## Frontend
The frontend of this project is built with **Next.js 14**. You can find the frontend repository [(https://github.com/zinx110/FurniFlex-FE)](https://github.com/roman0190/FurniFlex-FE).

## Demo Video of frontend

[![FurniFlex Demo](https://github.com/roman0190/FurniFlex-FE/raw/main/FurniFlex_Demo_HomePage.png)](https://github.com/roman0190/FurniFlex-FE/raw/main/FurniFlex_Demo.mp4)




## Features

- **User Authentication**: Implemented with JWT-based authentication using ASP.NET Identity.
- **Entity Framework Integration**: For database access and operations.
- **Product Management**: Admins can add, edit, and delete products.
- **Order Management**: Users can place orders and track them, and admins can update order statuses.
- **User & Sales Summary**: Admins can view sales and user summaries in charts.
- **JWT-based Authorization**: Ensures secure API access for users and admins.
  
## Setup Instructions

1. Clone the repository:
2. install the necessary dependencies:
    Ensure you have Visual Studio and .NET Framework installed.
    Open the project in Visual Studio and restore the NuGet packages.
3. Configure the database connection:
    Update the connection string in the appsettings.json file.
4. Run the migrations to set up the database:
    ```
    Update-Database
    ```
5. Start the API:
    Build and run the project in Visual Studio.

## Technologies Used
- ASP.NET Framework Web API (4.7.2)
- Entity Framework
- Identity Framework
- JWT Authentication
- MSSQL Server
