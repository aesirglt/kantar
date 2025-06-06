# 📌 Author's Note

Due to time constraints, I opted not to include more advanced architectural tools in this project, such as FluentValidation, AutoMapper and MediatR. However, if you're interested in seeing a more "real-world" implementation of my work, I invite you to explore one of my personal projects:

🔗 **Ragstore Backend**  
[https://github.com/aleffmoura/Ragstore/tree/master/Backend](https://github.com/aleffmoura/Ragstore/tree/master/Backend)  
> While the README is not thoroughly documented, the codebase itself is well-structured and showcases my development approach. As it is a personal project, I prioritized functionality over documentation. The project incorporates several advanced concepts, including multi-tenancy.

Additionally, I have published a NuGet package that I use professionally to facilitate working with `Result` and `Option` types in C#, applying some functional programming concepts:

🔗 **Functional Concepts for C#**  
[https://github.com/aleffmoura/FunctionalConcepts](https://github.com/aleffmoura/FunctionalConcepts)

# Kantar.TechnicalAssessment Web API

Kantar.TechnicalAssessment is a simple basket shopping API built with C# minimal api. It allows basic operations for Baskets.
that application includes swagger documentation, in-memory database, and basic error handling.

## 📝 API Usage Instructions
### ➕ Create an Basket
- **Explanation:** 
- This endpoint allows you to create a new basket item for a user. When a basket doesnt exists, will create.
- **Endpoint:** `POST /baskets/{id}/items`
- **Body:**
  ```json
{
  "items": [
    {
      "itemId": "d1f8b2c4-3e5a-4f0b-9c6e-7f8b2c4f0b9c",
      "quantity": 0
    }
  ]
}
  ```
## 🚀 Getting Started

To run the project:

1. Clone or download the solution.
2. Open the solution in Visual Studio or your preferred IDE.
3. Run the project. No additional setup is required.

## 🧪 In-Memory Database

The project uses an **in-memory database** via Entity Framework Core. This means:

- All data is stored in memory and lost when the application stops.
- Al **default items** is seeded at startup for convenience.
- the items ids are:
  ```
	BreadId  => "8f97bcbe-83d6-43f6-acf9-6abf9fbc272b"
	AppleId  => "d1f8b2c4-3e5a-4f0b-9c6e-7f8b2c4f0b9c"
	SoupId	 => "d1d8bfbe-5df6-4c0e-b448-0b43c5dbe485"
	Milk	 => "d297e3ba-8dcd-41f7-bfd1-beb8bdeb9efa"
	Pears	 => "3d36eebf-0e7e-4753-83ea-5766b4c8b65f"
	Eggplant => "cb525f53-cf68-400c-b8f4-ffa24286a9f5"
	Cucumber => "6222470e-077c-4cd1-ad8a-47ef7dfbe2c0"
  ```