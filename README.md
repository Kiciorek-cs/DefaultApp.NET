# Monolithic Application in .NET 9

This repository contains a **monolithic application** built with **.NET 9**. It consolidates a variety of essential features and functionalities into a single cohesive application.

## Key Features

The application includes a wide range of functionalities that make it a complete solution for various business needs. Some of the key features include:

- background tasks
    - background task management (enpoint)
    - simple queue 
    - saving background tasks on the database
    - automatic start after system startup
    - action block with mediatR publish
- custom authorization and authentication (account with confirmation, reset password e.t.c)
- logging information from the entire system (Serilog, file, request tracking)
- two databases
- generic paging (country/pagination) e.g. ./api/v1/County/pagination?WhereColumns[0].Column=englishName&WhereColumns[0].Value=Kosovo&WhereColumns[0].Expression=10&PageSize=15&Page=1
- sending emails
- sending http requests
- entityFramework + dapper
- FluentValidation
- excel generator
- e.t.c

## Technology Stack

- **Platform**: .NET 9
- **Architecture**: Monolithic
- **Database**: Two MsSQL databases (Demo, Permission)
- **Other Tools & Libraries**: 
        -Microsoft.EntityFrameworkCore
        -Dapper
        -AutoMapper
        -ClosedXML
        -FluentValidation
        -MediatR
        -Microsoft.AspNetCore.Authentication.JwtBearer
        -Microsoft.Graph 
        -Serilog
        -e.t.c

## Getting Started

To get started with the application, follow these steps:

1. Clone this repository to your local machine
2. Add some credentials for microsoft graph
3. Run migration (File "Migration")
4. Run SQL file on Demo Database
5. Using enpoint add some Resources and Roles (first role will be default)

## Test
Tests will be added in the next iteration using NUnit