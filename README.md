# Local Deployment Architecture Demo

This project is a full-stack proof-of-concept designed to demonstrate a local deployment architecture using **React**, **.NET 8**, **MySQL**, and **Redis**.

It showcases the **Cache-Aside Pattern**, demonstrating how a backend manages data retrieval between a persistent database (MySQL) and a high-performance cache (Redis) to optimize responses.

## 🏗 Architecture

The application consists of three main parts:

1.  **Frontend**: A React (Vite) dashboard to trigger API calls.
2.  **Backend**: A .NET 8 Web API that handles logic and data aggregation.
3.  **Data Layer**:
    * **MySQL**: Persistent storage for user data.
    * **Redis**: In-memory cache to store frequently accessed data.

## 🚀 Prerequisites

To run this project locally, ensure you have the following installed:

* **Node.js** (v18+)
* **.NET 8 SDK**
* **MySQL Server** (Running on port 3306)
* **Redis Server** (Running on port 6379)

## ⚙️ Configuration

### 1. Database Setup
The backend expects a specific database and user configuration. Run the following SQL commands in your MySQL instance to set up the environment:

```sql
-- Create Database
CREATE DATABASE appdb;

-- Create User (Matches Program.cs connection string)
CREATE USER 'appuser'@'localhost' IDENTIFIED BY 'MySuperSecretPass123!';
GRANT ALL PRIVILEGES ON appdb.* TO 'appuser'@'localhost';
FLUSH PRIVILEGES;

-- Create Table and Dummy Data
USE appdb;
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

INSERT INTO Users (Name) VALUES ('Alice'), ('Bob'), ('Charlie'), ('Deployment Dave');

```
2. Backend Configuration
The connection strings are currently hardcoded in BackenApi/Program.cs for simplicity:

```
MySQL: Server=localhost;Database=appdb;User=appuser;Password=MySuperSecretPass123!;
```

Redis: localhost

🏃‍♂️ How to Run
Step 1: Start the Backend
Navigate to the backend directory and start the API:

```
Bash

cd BackenApi
dotnet restore
dotnet run
```
The backend will typically start on http://localhost:5138.

Step 2: Start the Frontend
Open a new terminal, navigate to the frontend directory, and start the Vite server:

```
Bash

cd frontend
npm install
npm run dev
```
Step 3: Deployment / Proxy Note
The frontend makes requests to /api/users. Since the frontend and backend run on different ports locally (e.g., 5173 vs 5138), you must ensure one of the following:

Use a Reverse Proxy (Recommended): Set up Nginx to serve the frontend and proxy /api requests to the .NET backend.

Vite Proxy: Configure vite.config.js to proxy requests to localhost:5138.

🧠 How It Works (The Logic)
Health Check: Click "Check API Status" to verify the backend is reachable.

Data Retrieval: Click "Load Users".

First Click: The backend checks Redis. It's empty. It fetches from MySQL, saves to Redis, and returns the list. You will see (Served from MySQL Database) in the UI.

Second Click: The backend checks Redis. Data exists! It returns the data immediately without hitting the database. You will see (Served from Redis Cache) in the UI.

After 60 Seconds: The cache expires, and the cycle repeats.

📂 Project Structure
/BackenApi: .NET 8 Web API project.

/frontend: React + Vite project.
