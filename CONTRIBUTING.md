# Contributing

## Pre-requisites
Please ensure that you have the following tools:
* Visual Studio 2022 or equivalent (Rider or VSCode setup)
* .NET 6
* Docker Desktop

Please add a sensible URI in the `"DmrServiceSettings:DmrApiUri"` property in `appsettings.json`.

## Development
For updates to the application's hosting environment, please make changes in `Dockerfile` file. \

The first-class launch settings option for this project is **Docker**.

Please add all unit tests into the `MockClassifier.UnitTests` project and ensure that you have good quality tests covering your changes.

// TODO: Add details of:
//  - How the CI pipeline will validate changes
//  - What to expect from Stryker (mutation test result)
//  - Anything relevant for engineers to get started on the project