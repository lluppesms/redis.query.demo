# Azure Redis Graph Query Demo

[![Open in vscode.dev](https://img.shields.io/badge/Open%20in-vscode.dev-blue)][1]

[1]: https://vscode.dev/github/lluppesms/redis.query.demo/

![azd Compatible](./Docs/images/AZD_Compatible.png)

[![deploy.infra.and.function](https://github.com/lluppesms/redis.query.demo/actions/workflows/deploy-infra-function.yml/badge.svg)](https://github.com/lluppesms/redis.query.demo/actions/workflows/deploy-infra-function.yml)

---

## Overview

This repo was created to show an example of how to use and Azure Function with Azure Redis Cache to store values, and to do a union query of those values.

The Azure Functions are defined in the [Http_Trigger_RedisQuery.cs](./src/Graph.Query/Http_Trigger_RedisQuery.cs) file and that class is basically just a skeleton.

All of the Redis logic and functions are defined in the [RedisHelper.cs](./src/Graph.Query/DataRepository/RedisHelper.cs) class.

This example also includes .http test files in the [src/Graph.Query/TestHarness](./src/Graph.Query/TestHarness) folder, and includes a Swagger UI for testing the API.

---

## Azure Deployment Options

This project can be deployed to Azure via the AZD command, an Azure DevOps Pipeline, or a GitHub Action.  All resources are defined in the [main.bicep](./infra/Bicep/main.bicep) file and automatically deployed.

1. [Deploy using AZD Command Line Tool](./Docs/AzdDeploy.md)

2. [Deploy using Azure DevOps](./Docs/AzureDevOps.md)

3. [Deploy using GitHub Actions](./Docs/GitHubActions.md)

<!-- ---

## Running the Application

[How to run the application](./Docs/RunApplication.md) -->
