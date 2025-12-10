# Address Distance Calculator

C# ASP.NET Core Web API for creating, reading, updating, and deleting addresses, and calculating the distance in kilometers between **Dutch addresses** using the Bing Maps REST API and `GeoCoordinatePortable`.

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

- Stores **addresses** in a SQLite database  
- Uses the **Bing Maps REST Locations API** to geocode Dutch zip codes  
- Calculates **air distance (straight‑line distance in kilometers)** between two stored addresses based on their coordinates  

**Main use case:**
1. Create one or more Dutch addresses via the `/api/Adresses` endpoint  
2. Ask the API for the distance between two addresses by their IDs using `/api/location/{id1}/{id2}`  

---

## Features

- **CRUD for addresses**  
  Create, read, update, and delete address records  

- **Filtering & paging**  
  Filter by `city`, `streetname`, `zipcode`, `housenumber`, `id`  
  Simple paging via `page` and `pageSize` query parameters  

- **Distance calculation**  
  Uses `GeoCoordinatePortable` to compute air distance between coordinates  
  Returns distance in **kilometers**  

- **External geocoding**  
  Uses **Bing Maps REST Locations API** to validate and geocode **Dutch** zip codes (NL)  

---

## Architecture & Tech Stack

**Backend**
- C# / .NET 5 (`net5.0`)  
- ASP.NET Core Web API  
- Entity Framework Core  
- SQLite (`sqlite.db`)  

**Key Libraries**
- `GeoCoordinatePortable` – distance calculations between coordinates  
- `Newtonsoft.Json` – JSON parsing of Bing Maps responses  
- `Microsoft.EntityFrameworkCore` – ORM used via `SocialBrothersContext`  

**Data Layer**
```csharp
// Data/SocialBrothersContext.cs
public class SocialBrothersContext : DbContext
{
    public SocialBrothersContext(DbContextOptions options) : base(options) { }
    public DbSet<Adress> Adresses { get; set; }
}

// Models/Adress.cs
public class Adress
{
    public int ID { get; set; }
    public string StreetName { get; set; }
    public string HouseNumber { get; set; }
    public string Zipcode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
```

---

## Prerequisites

- .NET 5 SDK  
- A Bing Maps API key (Bing Maps REST services)  

Optional but recommended:
- Postman or any HTTP client for testing the API  
- Visual Studio / Rider / VS Code with C# support  

---

## Getting Started

### Clone & Restore
```bash
git clone https://github.com/ferrannl/Address-Distance-Calculator.git
cd Address-Distance-Calculator
dotnet restore
```

### Configure Database
The project uses a local SQLite database file named `sqlite.db` by default:

```json
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
```

You can:
- Use the existing `sqlite.db` file in the root, or  
- Point `cs` to another SQLite file/location  

### Configure Bing Maps API Key
The geocoding logic lives in:

- `Controllers/LocationController.cs`  
- `Controllers/AdressesController.cs`  

Inside both controllers, replace the placeholder key:
```csharp
string bingkey = "YOUR_REAL_BING_MAPS_KEY";
```

> Tip: In a real project, move this into configuration (e.g., `appsettings.json` or environment variables).

### Run the API
From the project root:
```bash
dotnet run
```

By default ASP.NET Core will host the API on:
- https://localhost:5001  
- http://localhost:5000  

---

## API Reference

### Addresses (`/api/Adresses`)

**Base URL:** `https://localhost:[port]/api/Adresses`

- **GET /api/Adresses**  
  Returns a paged list of addresses. Supports filtering via query parameters.  

  Example:  
  ```
  GET https://localhost:[port]/api/Adresses?city=Houten&page=1&pageSize=10
  ```

- **GET /api/Adresses/{id}**  
  Returns a single address by ID.  

- **POST /api/Adresses**  
  Creates a new address. Validates zipcode using Bing Maps API.  

- **PUT /api/Adresses/{id}**  
  Updates an existing address.  

- **DELETE /api/Adresses/{id}**  
  Deletes an address by ID.  

---

### Distance (`/api/location`)

**Base URL:** `https://localhost:[port]/api/location`

- **GET /api/location/{id1}/{id2}**  
  Calculates the air distance in kilometers between two stored addresses.  

Return values:
- `"Same addresses / distance = 0"` if identical  
- `"Distance in km: {value}"` otherwise  

---

## Examples

### Create Address (POST)
```http
POST https://localhost:5001/api/Adresses
Content-Type: application/json

{
  "streetName": "Marskramersgilde",
  "houseNumber": "1",
  "zipcode": "3994AB",
  "city": "Houten",
  "country": "Netherlands"
}
```

### List Addresses with Filters & Paging (GET)
```http
GET https://localhost:5001/api/Adresses?city=Houten&zipcode=3994AB&page=1&pageSize=5
```

### Calculate Distance Between Two Addresses (GET)
```http
GET https://localhost:5001/api/location/1/2
```
Example response:
```
Distance in km: 12.3456789
```

---

## Notes & Limitations

- **Dutch addresses only** – Bing Maps API calls use `"NL"` hard‑coded in the URL  
- **Straight‑line distance** – not route/road distance  
- **External dependency** – requires Bing Maps API key, subject to Microsoft’s terms and rate limits  
- **Demo project** – intended as a learning case, not production‑ready  

---

## License

This repository is published for educational/demo purposes.  
If you add an explicit license later (e.g., MIT), reference it here.  

Otherwise, all rights remain with the author.
```
