# CarStockAPI

## Overview

Car Stock API is a backend application based on C# and FastEndpoints framework, implemented using SQLite database.The API provides the following features:

1\. user login (generate JWT Token)

2\. add vehicles

3\. delete vehicles

4\. list vehicle inventory

5\. update vehicle inventory

6\. Search for vehicles by make and model

## Stack

\- Language: C#

\- Framework: FastEndpoints

\- Database: SQLite

\- Dependencies: Dapper (for database operations)

\- Dapper (for database operations)

\- Microsoft.Data.Sqlite (SQLite driver)

\- Microsoft.AspNetCore.Authentication.JwtBearer (for JWT authentication)

## How to Download Project

1. Clone the project from GitHub:

```bash

git clone https://github.com/your-repo-name/CarStockAPI.git

cd CarStockAPI

```

2. Download dotnet SDK

3. Download dotnet dependency

```bash

dotnet  restore

```

4.initialize database

first time run the project, the sample data will insert to database.

## How to Run the project

1. run backend server

```
dotnet run

```
2. test api at postman or swagger

## API DESIGN DOCS （I AM using POSTMAN to test all API）
1. Authentication API

**1\. Login API**

-  **URL**: POST /auth/login

-  **Method**: POST

-  **Description**: Authenticates the user and generates a JWT token.

-  **Request Body**:

```json

{
    "Username": "dealer1",
    "Password": "password123"
}

```

- **Response**:

200

```json

{
"message": "Login successful. Please copy this token to test the rest API.",
"token": "{JWTToken}"

}
```
400(bad request)

```json

{
    "error": "Username and password cannot be empty."
}
```
401(Unauthorized)

```json

{
    "error": "Invalid username or password."
}
```
500(server error)

```json
{
    "error": "An error occurred: {ErrorMessage}"
}
```
After receiving the token, copy it and use it in the Authorization header to test other APIs

2. Add Car API

**2\. Add CAR API**

-  **URL**: POST /cars/add

-  **Method**: POST

-  **Description**: Adds a new car to the dealer's inventory. Only authenticated dealers can use this endpoint
- **Headers**:

KEY: Authorization   VALUE: Bearer {JWTToken} (JWT token copy from the login API)
-  **Request Body**:

```json
{
    "make": "Audi",
    "model": "A4",
    "year": 2018,
    "stock": 10
}

```

- **Response**:

200

```json

{
    "message": "Car added successfully."
}
```
400(bad request)

```json
{
    "statusCode": 400,
    "message": "One or more errors occurred!",
    "errors": {
        "generalErrors": [
            "Invalid input data."
        ]
    }
}
```
401(Unauthorized)

```json
{
    "error": "Unauthorized access."
}
```
500(server error)

```json
{
    "error": "An error occurred: {ErrorMessage}"
}
```

3. Remove Car API

**3\. Remove CAR API**

-  **URL**: DELETE /cars/{id}

-  **Method**: DELETE

-  **Description**: Deletes a car from the dealer’s inventory. Only authenticated dealers can remove cars that belong to them

- **Headers**:
KEY: Authorization   VALUE: Bearer {JWTToken} (JWT token copy from the login API)

-  **Path Parameters**:

Id : The unique ID of the car to delete
eg:
http://localhost:5131/cars/2

- **Response**:

200

```json
{
    "message": "Car deleted successfully."
}

```
400(bad request)

```json
{
    "statusCode": 400,
    "message": "One or more errors occurred!",
    "errors": {
        "id": [
            "Value [a] is not valid for a [Int32] property!"
        ]
    }
}
```
401(Unauthorized)

```json
{
    "error": "Unauthorized access."
}
```
500(server error)

```json
{
    "error": "An error occurred: {ErrorMessage}"
}
```

4. List Car API

**4\. LIST CAR API**

-  **URL**: GET /cars/list

-  **Method**: GET

-  **Description**: Lists all cars in the dealer’s inventory. Allows filtering by make and model

- **Headers**:
KEY: Authorization   VALUE: Bearer {JWTToken} (JWT token copy from the login API)

-  **Parameters(this params is optional, if no params will present all cars belongs to this dealer )**:

make: (string) Filter results by car make (e.g., “Audi”).
model: (string) Filter results by car model (e.g., “A4”).
eg:
http://localhost:5131/cars/list
http://localhost:5131/cars/list?make=Audi
http://localhost:5131/cars/list?model=A4
http://localhost:5131/cars/list?make=Audi&model=A4

- **Response**:

200

```json
{
    {
        ....
    },
    {
        "id": 1,
        "make": "Audi",
        "model": "A4",
        "year": 2018,
        "stock": 10
    },
    {
        ....
    },
}

```
200(NO MATCH CAR)

```json
[]
```
401(Unauthorized)

```json
{
    "error": "Unauthorized access."
}

```
500(server error)

```json
{
    "error": "An error occurred: {ErrorMessage}"
}
```

**5\. Update CAR stock API**

-  **URL**: PUT /cars/update-stock

-  **Method**: PUT

-  **Description**: Updates the stock level of a car. Only authenticated dealers can update the stock of cars they own

- **Headers**:
KEY: Authorization   VALUE: Bearer {JWTToken} (JWT token copy from the login API)

-  **Request Body**:
```json
{
    "id": 1,
    "stock": 20
}
```
- **Response**:

200

```json
{
    "message": "Stock updated successfully."
}

```
400(bad request)

```json
{
    "statusCode": 400,
    "message": "One or more errors occurred!",
    "errors": {
        "id": [
            "'x' is an invalid start of a value. LineNumber: 1 | BytePositionInLine: 10."
        ]
    }
}
```
404(NOT FOUND)

```json
{
    "statusCode": 404,
    "message": "One or more errors occurred!",
    "errors": {
        "generalErrors": [
            "Unauthorized or car not found."
        ]
    }
}
```
500(server error)

```json
{
    "error": "An error occurred: {ErrorMessage}"
}
```
6. Search Car API

**6\. Search CAR API**

-  **URL**: GET /cars/search

-  **Method**: GET

-  **Description**: Searches for cars by make and/or model. At least one of the parameters (make or model) must be provided. 

- **Headers**:
KEY: Authorization   VALUE: Bearer {JWTToken} (JWT token copy from the login API)

-  **Parameters(this params is optional, if no params will present all cars belongs to this dealer )**:

make: (string) Filter results by car make (e.g., “Audi”).
model: (string) Filter results by car model (e.g., “A4”).
eg:
http://localhost:5131/cars/search?make=Audi&model=A4

- **Response**:

200

```json
{
    {
        ....
    },
    {
        "id": 1,
        "make": "Audi",
        "model": "A4",
        "year": 2018,
        "stock": 10
    },
    {
        ....
    },
}

```
200(NO MATCH CAR)

```json
[]
```
400(NO params)

```json
{
    "statusCode": 400,
    "message": "One or more errors occurred!",
    "errors": {
        "generalErrors": [
            "At least one parameter (make or model) must be provided."
        ]
    }
}
```
401(Unauthorized)

```json
{
    "error": "Unauthorized access."
}

```
500(server error)

```json
{
    "error": "An error occurred: {ErrorMessage}"
}
```