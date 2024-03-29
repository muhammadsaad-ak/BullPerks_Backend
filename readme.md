# Project Name

BullPerks Backend

## Table of Contents

- [Introduction](#introduction)
- [Technologies](#technologies)
- [Setup](#setup)
  - [Setting up the Database](#setting-up-the-database)
  - [Running the Application](#running-the-application)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## Introduction

This is the backend of the project which gets the Total Supply of BLP token on BNB chain and calculates its Circulating Supply on different wallet addresses.

## Technologies

List the technologies used in your project. For example:
- ASP.NET Core
- Entity Framework Core
- MySQL
- Swagger

## Setup

### Setting up the Database

1. Install PHPMyAdmin on your local machine if not already installed.
2. Access PHPMyAdmin in your browser.
3. Create a new database named `your_database_name`.

### Running the Application

1. Clone this repository to your local machine.
2. Navigate to the project directory.
3. Install the necessary dependencies using `dotnet restore`.
4. Update the database connection string in `appsettings.json` file with your MySQL database credentials.
5. Run the application using `dotnet run`.

## Usage

This project is using two APIs from bsc scan. 

1. `https://api.bscscan.com/api?module=stats&action=tokensupply&contractaddress=0xe9e7cea3dedca5984780bafc599bd69add087d56&apikey=YourApiKeyToken` ("For getting the total supply of the BLP token")

2. `https://api.bscscan.com/api?module=account&action=tokenbalance&contractaddress=0xe9e7cea3dedca5984780bafc599bd69add087d56&address=0x89e73303049ee32919903c09e8de5629b84f59eb&tag=latest&apikey=YourApiKeyToken` ("For getting the balances of the BLP token on different wallet addresses")

## API Endpoints

Provide a list of API endpoints along with brief descriptions. For example:

- `POST /api/auth/login`: Logs in the user and returns a JWT token.
- `POST /api/token/calculate-supply`: Calculates and stores token supply.
- `GET /api/token/data`: Retrieves stored token data.
