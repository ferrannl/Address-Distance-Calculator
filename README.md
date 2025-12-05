# Address Distance Calculator

C# ASP.NET Core Web API for creating, reading, updating and deleting addresses, and calculating the distance in kilometers between **Dutch addresses** using the Bing Maps REST API and `GeoCoordinatePortable`.

> .NET 5 · ASP.NET Core Web API · Entity Framework Core · SQLite · Bing Maps REST Locations API

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture & Tech Stack](#architecture--tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Clone & Restore](#clone--restore)
  - [Configure Database](#configure-database)
  - [Configure Bing Maps API Key](#configure-bing-maps-api-key)
  - [Run the API](#run-the-api)
- [API Reference](#api-reference)
  - [Addresses (`/api/Adresses`)](#addresses-apiadresses)
  - [Distance (`/api/location`)](#distance-apilocation)
- [Examples](#examples)
  - [Create Address (POST)](#create-address-post)
  - [List Addresses with Filters & Paging (GET)](#list-addresses-with-filters--paging-get)
  - [Calculate Distance Between Two Addresses (GET)](#calculate-distance-between-two-addresses-get)
- [Notes & Limitations](#notes--limitations)
- [License](#license)

---

## Overview

This project is a small .NET 5 Web API case study that:

- Stores **addresses** in a SQLite database.
- Uses the **Bing Maps REST Locations API** to geocode Dutch zip codes.
- Calculates **air distance (straight-line distance in kilometers)** between two stored addresses based on their coordinates.

The main use case is:  
1. Create one or more Dutch addresses via the `/api/Adresses` endpoint.  
2. Ask the API for the distance between two addresses by their IDs using `/api/location/{id1}/{id2}`.

---

## Features

- **CRUD for addresses**
  - Create, read, update and delete address records.
- **Filtering & paging**
  - Filter by `city`, `streetname`, `zipcode`, `housenumber`, `id`.
  - Simple paging via `page` and `pageSize` query parameters.
- **Distance calculation**
  - Uses `GeoCoordinatePortable` to compute air distance between coordinates.
  - Returns distance in **kilometers**.
- **External geocoding**
  - Uses **Bing Maps REST Locations API** to validate and geocode **Dutch** zip codes (NL).

---

## Architecture & Tech Stack

**Backend**

- C# / .NET 5 (`net5.0`)
- ASP.NET Core Web API
- Entity Framework Core
- SQLite (`sqlite.db`)

**Key Libraries**

- `GeoCoordinatePortable` – distance calculations between coordinates.
- `Newtonsoft.Json` – JSON parsing of Bing Maps responses.
- `Microsoft.EntityFrameworkCore` – ORM used via `SocialBrothersContext`.

**Data Layer**

- `Data/SocialBrothersContext.cs` defines `DbSet<Adress>`:
  ```csharp
  public class SocialBrothersContext : DbContext
  {
      public SocialBrothersContext(DbContextOptions options) : base(options) { }

      public DbSet<Adress> Adresses { get; set; }
  }

Models/Adress.cs defines the address model:

public class Adress
{
    public int ID { get; set; }
    public string StreetName { get; set; }
    public string HouseNumber { get; set; }
    public string Zipcode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}



---

Prerequisites

.NET 5 SDK

A Bing Maps API key (Bing Maps REST services)


Optional but recommended:

Postman or any HTTP client for testing the API.

Visual Studio / Rider / VS Code with C# support.



---

Getting Started

Clone & Restore

git clone https://github.com/ferrannl/Address-Distance-Calculator.git
cd Address-Distance-Calculator
dotnet restore

Configure Database

The project uses a local SQLite database file named sqlite.db by default:

// appsettings.json
{
  "ConnectionStrings": {
    "cs": "Data Source=sqlite.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}

You can:

Use the existing sqlite.db file in the root, or

Point cs to another SQLite file/location.


Configure Bing Maps API Key

The geocoding logic lives in:

Controllers/LocationController.cs

Controllers/AdressesController.cs


Inside both controllers, there is a hard-coded placeholder key:

// LocationController.cs
string bingkey = "BING_KEY";

and:

// AdressesController.cs
string bingkey = "YOUR_BING_KEY_HERE";

Replace these values with your actual Bing Maps key, for example:

string bingkey = "YOUR_REAL_BING_MAPS_KEY";

> Tip: In a real project you would move this into configuration (e.g. appsettings.json or environment variables).



Run the API

From the project root:

dotnet run

By default ASP.NET Core will host the API on something like:

https://localhost:5001 and/or

http://localhost:5000


Check the console output to see the exact URLs.


---

API Reference

Addresses (/api/Adresses)

Base URL

https://localhost:[port]/api/Adresses

GET /api/Adresses

Returns a paged list of addresses. Supports filtering via query parameters.

Query parameters:

city – filter by city (optional)

streetname – filter by street name (optional)

zipcode – filter by Dutch zipcode (e.g. 1234AB) (optional)

housenumber – filter by house number (optional)

id – filter by exact address ID (optional)

page – page number, default = 1

pageSize – page size, default = 3


Example:

GET https://localhost:[port]/api/Adresses?city=Houten&page=1&pageSize=10

GET /api/Adresses/{id}

Returns a single address by ID.

GET https://localhost:[port]/api/Adresses/5

200 OK – returns the Adress entity.

404 Not Found – if the address does not exist.


POST /api/Adresses

Creates a new address.

POST https://localhost:[port]/api/Adresses
Content-Type: application/json

Body: see Create Address (POST).

The controller:

Validates the zipcode using the Bing Maps API.

If the zipcode is valid and geocodable:

Saves the address to the database.

Returns 201 Created and text "Address created".


If invalid or empty:

Returns a message:
"Sorry, your zipcode is empty or invalid. Make sure you use a Dutch zipcode [1234AB]."



PUT /api/Adresses/{id}

Updates an existing address.

PUT https://localhost:[port]/api/Adresses/5
Content-Type: application/json

The id in route must match adress.ID in the body.

Returns:

204 No Content on success.

400 Bad Request if IDs do not match.

404 Not Found if the address does not exist.



DELETE /api/Adresses/{id}

Deletes an address by ID.

DELETE https://localhost:[port]/api/Adresses/5

204 No Content on success.

404 Not Found if the address does not exist.



---

Distance (/api/location)

Base URL

https://localhost:[port]/api/location

GET /api/location/{id1}/{id2}

Calculates the air distance in kilometers between two stored addresses.

GET https://localhost:[port]/api/location/{id1}/{id2}

Looks up both addresses in the database by id1 and id2.

Geocodes their Zipcode fields using the Bing Maps REST Locations API (for NL).

Uses GeoCoordinate to calculate distance (in meters) and converts to kilometers.


Return values:

"Same addresses / distance = 0"

When both coordinates are identical.


"Distance in km: {value}"

When the coordinates differ.



> Note: This is straight-line distance, not route/road distance.




---

Examples

Create Address (POST)

POST https://localhost:5001/api/Adresses
Content-Type: application/json

{
  "streetName": "Marskramersgilde",
  "houseNumber": "1",
  "zipcode": "3994AB",
  "city": "Houten",
  "country": "Netherlands"
}

Possible responses:

201 Created with body:

Address created

If zipcode is invalid/empty or not a Dutch zipcode:

Sorry, your zipcode is empty or invalid. Make sure you use a Dutch zipcode [1234AB].



---

List Addresses with Filters & Paging (GET)

GET https://localhost:5001/api/Adresses?city=Houten&zipcode=3994AB&page=1&pageSize=5

Returns a JSON array of Adress objects that match the filters.


---

Calculate Distance Between Two Addresses (GET)

Assume you have two addresses with IDs 1 and 2.

GET https://localhost:5001/api/location/1/2

Example response:

Distance in km: 12.3456789

If both addresses geocode to the same coordinates:

Same addresses / distance = 0


---

Notes & Limitations

Dutch addresses only
Both controllers call the Bing Maps Locations API with "NL" hard-coded in the URL:

"REST/v1/Locations/NL/" + _addressLine + "/?o=json&key=" + bingkey

If you need other countries, you’ll need to refactor this part.

Straight-line distance
The distance is based on coordinates and does not reflect road/route distance.

External dependency
The project depends on Bing Maps REST Locations API:

Requires a valid API key.

Subject to Microsoft’s terms of service and rate limits.


Demo / learning project
As noted in the controllers, this project is meant as a small case to “prove a theory”.
It is not optimized or hardened for production environments.



---

License

This repository is published for educational/demo purposes.
If you add an explicit license later (e.g. MIT), you can reference it here:

MIT License (see LICENSE file for details)

Otherwise, all rights remain with the author.
