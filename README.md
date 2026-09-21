# Client Work Order Entry (WOE)

A simple full-stack interview assignment using:

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server LocalDB
- Swagger
- HTML/CSS/JavaScript
- Bootstrap

## Main flow

1. Select a client.
2. Enter patient details.
3. Search laboratory tests.
4. Add multiple tests.
5. Set quantities.
6. Review running total.
7. Save the work order.
8. API generates a unique WOE number.
9. Order and test details are stored in SQL Server.
10. Saved WOE details are displayed.

## Database

Tables:

- Client
- Patient
- TestMaster
- WorkOrder
- WorkOrderTestDetail

## Run backend

Open `ClientWOE.sln` in Visual Studio.

Open Package Manager Console and select `ClientWOE.API`.

Run:

```powershell
Add-Migration InitialCreate
Update-Database
```

Then run the API.

Swagger:

`http://localhost:5080/swagger`

## Run frontend

Make sure the API is running.

Open:

`frontend/index.html`

The JavaScript uses:

`http://localhost:5080/api`

## Important

Use either EF Core migrations OR `database/schema.sql` to create the database. Do not use both on the same empty database.

## Interview explanation

This project follows a simple API + database + frontend structure.

The API contains controllers for clients, patients, tests and work orders. EF Core maps C# models to SQL Server tables. DTOs define the request and response structure.

The WorkOrdersController validates the client and tests, creates a patient, calculates the order amount using the database test rates, generates the WOE number, saves the order and line items inside an EF Core transaction, and returns the saved order.

The frontend uses JavaScript Fetch API to communicate with the REST API.
