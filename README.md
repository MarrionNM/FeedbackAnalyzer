# Feedback AI Platform

This repository contains the full-stack implementation of the **Feedback AI Analysis Platform**, consisting of:

- **Frontend:** Angular 18 (`Analyzer_FE`)
- **Backend:** .NET 8 Web API + EF Core (`Analyzer_BE`)
- **AI Integration:** OpenAI GPT-4o-mini

The system allows users to:

- Submit feedback
- Automatically analyze feedback using AI
- Generate system-level tags
- Assign priority scoring
- View feedback lists, filters, and full detail pages

---

## 🚀 Prerequisites

Make sure the following tools are installed on your machine:

- [Node.js](https://nodejs.org/) (v18+)
- [Angular CLI](https://angular.io/cli) (v18+)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PgAdmin 4](https://www.pgadmin.org/download/)
- [Visual Studio Code](https://code.visualstudio.com/) or any IDE
- [Postman](https://www.postman.com/) (optional)
- [OpenAI Platform Account](https://platform.openai.com/) (for API key)

> **If you have any issues setting up the project, feel free to contact me — I can walk you through the entire setup.**

---

# 🛠️ Getting Started

All commands assume you are in the **root project folder (`Analyzer`)**.

---

# Frontend Setup (Angular)

### 1. Navigate to the frontend project

```bash
cd Analyzer_FE
```

### 2. Install dependencies

```bash
npm install
```

### 3. Configure the environment file

Open:

src/environments/environment.ts

Replace the content with:

```json
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5034'
};
```

### 4. Run the Angular application

```bash
ng serve
```

Your frontend will be available at:

👉 http://localhost:4200

# Backend Setup (.NET 8 + PostgreSQL)

### 1. Navigate to the backend root

```bash
cd Analyzer_BE/FeedbackAnalyzer
```

cd Analyzer_BE/FeedbackAnalyzer

### 2. Clean, restore, and build the solution

```bash
dotnet clean
dotnet restore
dotnet build
```

### 3. Configure application settings

Open:

FeedbackAnalyzer/appsettings.json

Update the following fields:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=Analyzer;Username={userName};Password={Password}"
  },
  "OpenAI": {
    "ApiKey": "API_KEY",
    "Model": "gpt-4o-mini",
    "BaseUrl": "https://api.openai.com/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Replace:

{userName} → your PostgreSQL username

{Password} → your PostgreSQL password

"API_KEY" → your OpenAI API key

### 4. Apply database migrations

```bash
dotnet ef database update
```

### 5. Run the API

```bash
dotnet run
```

Your backend will be available at:

👉 https://localhost:5034

# Project Structure

```bash
Analyzer/
│
├── Analyzer_FE/        # Angular frontend
│
├── Analyzer_BE/        # .NET backend API + EF Core
│   └── FeedbackAnalyzer/
│
├── README.md
└── SOLUTION.md         # (included for assessment documentation)
```
