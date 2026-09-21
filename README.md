# Client Work Order Entry (WOE)

A simple full-stack interview assignment developed using:

* ASP.NET Core 8 Web API
* Entity Framework Core 8
* SQL Server LocalDB
* HTML, CSS, JavaScript & Bootstrap

## Features

* Select client and enter patient details
* Search and add laboratory tests
* Set test quantities and calculate total
* Create and save Work Orders
* Generate unique WOE number
* Store data in SQL Server

## Project Structure

* `ClientWOE.API` – ASP.NET Core Web API
* `frontend` – HTML/CSS/JavaScript frontend
* `database` – SQL database script

## Run

1. Open `ClientWOE.sln` in Visual Studio.
2. Run the API.
3. Open `frontend/index.html`.
4. Make sure the API is running before using the frontend.

Swagger:

`http://localhost:5080/swagger`

## Database

The project uses SQL Server LocalDB with Entity Framework Core migrations.

```powershell
Add-Migration InitialCreate
Update-Database
```
