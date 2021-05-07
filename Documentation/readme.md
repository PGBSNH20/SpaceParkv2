# Documentation
This is an Web-Api project that is based on the Starwars Web API, https://swapi.dev/ and is used to for practicing Http-Requests, EF and ASP.NET web framework.

## [Project SpacePark-API](https://github.com/PGBSNH20/spaceparkv2-buddygroup6-renegades/tree/main/Source/SpacePark-API):
This is the main project and contains code regarding:
- Data Models
- DbContext
- Controllers
- Logic

## [Poject: SpaceParkTests](https://github.com/PGBSNH20/spaceparkv2-buddygroup6-renegades/tree/main/Source/SpaceParkTests):
This is an x-unit project that do all the testing of the application.
We use an in-memory-sql database (dependent on nu-get EntityFrameworkCore.InMemory) to run tests of the db-context and we also use an test-unique web-host for testing the http-requests. See [MockWebHostFactory.cs](https://github.com/PGBSNH20/spaceparkv2-buddygroup6-renegades/blob/main/Source/SpaceParkTests/MockWebHostFactory.cs)

## [Project: StarwarsConsoleClient](https://github.com/PGBSNH20/spaceparkv2-buddygroup6-renegades/tree/main/Source/StarwarsConsoleClient)

A console application partly migrated from an SpacePark version 1.
![image](https://user-images.githubusercontent.com/63591629/117511759-95e59500-af8e-11eb-83e1-c4a9da4c66c7.png)
