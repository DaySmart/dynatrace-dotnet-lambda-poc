# Application Configuration

This application depends on certain configuration values being supplied.
The application will use dotnet mechanism acquire configuration values in the following order.

1. Application settings file
   2. appsettings.json
   3. appsettings.{environment}.json
4. Environment variables

Order of precedence:
```
environment variables > 
    environment-specific settings file > 
        generic settings file
```

Defined values will be replaced based on order of precedence.

Custom Configuration Sections

- `ReservationDatabase`
  - `ConnectionString`
  - `DatabaseName`
  - `CollectionName`
