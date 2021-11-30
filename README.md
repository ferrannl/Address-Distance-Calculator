C# ASP.NET Web API 
Create, Read, Update and Delete addresses.
Calculate distance between Dutch addresses in kilometers.

Make sure to enter your own Bing API KEY in Controllers/LocationController.cs 
Line 55: string bingkey = "BING_KEY";


CRUD Addresses:

-GET
https://localhost:[port]/api/Adresses/{id}
Body - > none

-DELETE
https://localhost:[port]/api/Adresses/{id}
Body - > none

-POST
https://localhost:[port]/api/Adresses

body -> raw
    {
        "id":
        "Zipcode":"1234AB"
    }


Calculate distance between addresses:

GET: api/location/{id_location1}/{id_location2}
Body - > none
