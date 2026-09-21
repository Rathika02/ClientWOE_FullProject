# Interview Notes

## 1. Project overview

This is a Client Work Order Entry system for creating laboratory test orders.

The user selects a client, enters patient details, selects laboratory tests with quantities, sees the total amount and submits the order.

The ASP.NET Core Web API handles business logic and EF Core stores the data in SQL Server.

## 2. Why Web API?

I used ASP.NET Core Web API because the frontend and backend are separated. The frontend communicates with the backend using HTTP and JSON.

## 3. Why EF Core?

EF Core is an ORM. It lets the application work with SQL Server using C# classes and LINQ instead of writing SQL for every operation.

## 4. Why DTOs?

DTOs define exactly what data the API accepts and returns. They avoid directly exposing database entities.

## 5. Database relationships

Client has many Patients.
Client has many WorkOrders.
Patient has many WorkOrders.
WorkOrder has many WorkOrderTestDetails.
TestMaster has many WorkOrderTestDetails.

## 6. Why store Rate in WorkOrderTestDetail?

The test master rate can change later. The order should keep the rate that was used when the order was created.

## 7. How is total calculated?

The API receives test IDs and quantities. It gets the current rate for each test from the database and calculates:

Amount = Rate × Quantity

Then it adds all line amounts to get TotalAmount.

## 8. Why transaction?

Creating a work order involves multiple database operations: patient, work order and test detail rows. A transaction makes sure they are saved together. If one operation fails, the changes can be rolled back.

## 9. HTTP status codes

200 - successful GET
201 - successfully created
400 - invalid request
404 - requested record not found
409 - duplicate client code
500 - unexpected server error

## 10. How WOE number works

The format is:

WOE-yyyyMMdd-####

Example:

WOE-20260921-0001

The date is included and the sequence increments for that day.

## 11. Frontend

The frontend is plain HTML, CSS and JavaScript. JavaScript uses fetch() to call the Web API.

## 12. Swagger

Swagger provides an interactive page for testing API endpoints without needing another frontend.

## 13. Common interview question

Why did you not use authentication?

This is a scoped assignment and authentication was not required, so I kept the implementation focused on the work-order flow.

## 14. What I would add in production

Authentication and authorization, audit logging, pagination, better concurrency handling for number generation, centralized structured logging, automated tests and more detailed error handling.
