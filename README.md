[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=DaySmart_scheduling-reservation&token=827b5060f4b9c9c483542852cd20a1b7dad520ae)](https://sonarcloud.io/summary/new_code?id=DaySmart_scheduling-reservation)

## Scheduling API

DaySmart Scheduling API providing functionality to other services.

### Information

- Runtime & SDK: [dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Language: [C# 10](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10)
- Framework: [aspnetcore-6.0](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-6.0?view=aspnetcore-6.0)
- Database: [AWS DocDB 4](https://aws.amazon.com/documentdb/) (partially compatible [mongodb](https://www.mongodb.com/) derivative)
  - Using the [dotnet/C# MongoDB driver](https://www.mongodb.com/docs/drivers/csharp/)

### Repo Documentation

- [Developer Setup](./docs/developer-setup.md)
- [Infrastructure](./docs/infrastructure.md)
- [Configuration](./docs/configuration.md)
- [Code Versioning](./docs/versioning.md)
- [Mongo Schema Versioning](./docs/schema-versioning.md)

---

## Usage

### Run the application

You can run the application locally, self-contained by using the provided container image definition and compose file that is included in the repo root directory.

```bash
docker-compose up
```

> **NOTE** The included compose file loads two container images, the built project image and a supporting mongo database.

> **NOTE** Initial run of the compose file will build the container and the project. If you wish to rebuild the project you will have to execute a `docker-compose build` command.

### Local development run

```bash
# Reservation development
dotnet run --project ./src/Scheduling.Reservation.API/Scheduling.Reservation.API.csproj
```

```bash
# Availability development
dotnet run --project ./src/Scheduling.Availability.API/Scheduling.Availability.API.csproj
```

### Testing

```bash
# unit tests
dotnet test -c Release --verbosity normal --filter "Category!=LongRunning"
```

[//]: # "----"
[//]: # "## Conventions"
[//]: #
[//]: # "### Commits"
[//]: #
[//]: # "We follow the [angular conventions](https://github.com/angular/angular/blob/main/CONTRIBUTING.md#-commit-message-format)"
[//]: # "for commits to help support the [semantic versioning](https://semver.org/) used on this repository. Please familiarize"
[//]: # "yourself with these conventions before making commits. This functionality is governed by the"
[//]: # "[semantic-release](https://github.com/semantic-release/semantic-release) github action consumed by this repository."
[//]: #
[//]: # "Example commit message:"
[//]: # "- `fix(pencil): stop graphite breaking when too much pressure applied`"
[//]: # "- `feat(pencil): add 'graphiteWidth' option`"
[//]: # "- `perf(pencil): remove graphiteWidth option`"
[//]: # "- [more info](https://github.com/semantic-release/semantic-release#commit-message-format)"
