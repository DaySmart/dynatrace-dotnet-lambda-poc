# Developer Local Environment Setup

This project utilizes the following technologies and as a prerequisite for development you must have your environment properly configured.

- [dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html) (optional)

# Installation Instructions

> **NOTE** Depending on your operating system these commands may vary, reference the source material for appropriate steps for your operating system.

### .NET 6.0

```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update; sudo apt-get install -y apt-transport-https &&   sudo apt-get update && sudo apt-get install -y dotnet-sdk-6.0

which dotnet
dotnet --version
```

## Optional Setup

> **NOTE** This setup is optional for development but these tools are required if you intend to use the `amazon.lambda.testtool-6.0` product to simulate execution in AWS Lambda.

### AWS CLI
```bash
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip
sudo ./aws/install
rm -rf awscliv2.zip ./aws

which aws
aws --version
```

### AWS .NET Templates

```bash
dotnet tool install -g Amazon.Lambda.Tools
dotnet new -i Amazon.Lambda.Templates
dotnet tool install -g amazon.lambda.testtool-6.0
```

# Debugging

## Debugging Individual Functions
The project must be built before the `amazon.lambda.testtool-6.0` package can be executed locally as it requires the projects `deps.json` file.

Within the project folder execute the following command
- `dotnet-lambda-test-tool-6.0`

This will launch the amazon testing tool acting as a host to exercise the lambda functions.
This tool has an "Execute Assembly" function that can be used to provide input to the function you want to exercise.

You will need to specify the the environment variable of `AWS_LAMBDA_RUNTIME_API=localhost:5050` in the launch configuration of your chosen debugging tool. Then simply setup breakpoints and launch the debugger.

## Debugging AspNet APIs

Simply launch project for debugging in favored Editor/IDE as you would any standard AspNet project.

## Seeding Sample Database

Option 1 - Create a local MongoDB instance and seed the database with the following command:
```
docker run --name mongo -v <local-path>:/data/db -p 27017:27017 -d mongo
```
    
Then run the following command to seed the database with sample data.

```
dotnet run --project src/Reservation.Database.Seeder/Reservation.Database.Seeder.csproj
```

# References

- [Serverless Office Hours | Using the new .NET 6.0 runtime in AWS Lambda](https://www.youtube.com/watch?v=l4_WNjMHDx8)
    - `https://www.youtube.com/watch?v=l4_WNjMHDx8`
- https://aws.amazon.com/blogs/compute/introducing-the-net-6-runtime-for-aws-lambda/
- https://github.com/aws/aws-lambda-dotnet/issues/979



# Notes


## Lambda - Cold Start

### JSON Code Generation

Lambda can take advantage of Code Generation at compile time for json serialization to help reduce the impact of a cold-start. This technique is outlined in the following article as well as in the linked video.
- https://docs.aws.amazon.com/lambda/latest/dg/csharp-handler.html#json-source-generation
- [Serverless Office Hours | Using the new .NET 6.0 runtime in AWS Lambda](https://www.youtube.com/watch?v=l4_WNjMHDx8)
    - uri: `https://www.youtube.com/watch?v=l4_WNjMHDx8`

----

### PublishReadyToRun

This functionality in .NET is toggled with `<PublishReadyToRun>true</PublishReadyToRun>` in the project file. Enabling this flag will cause architecture-specific publications to take advantage of the compiler generating machine-specific code instead of IL. In .NET6 this can be done from any OS. This is an example of publishing a release for 64bit linux.
- `dotnet publish -c Release -r linux-x64 --no-self-contained`
