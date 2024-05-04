# blog API

This project is a playground with .NET for me. My plan is to make Blog API from it. Current code uses JWT (JSON Web Tokens) for authentication and refresh tokens for maintaining user sessions.

## Features

- User registration
- User login
- Token refresh
- Token revocation

## Technologies

- C#
- .NET Core
- Entity Framework Core
- JWT

## Future Plans

In the future, this project will be extended to include a blog API. This will allow users to create, read, update, and delete blog posts.

## API Documentation
### Work In Progress - NOT READY

API documentation will be provided through Swagger. To view the API documentation, run the application and navigate to `/swagger` in your web browser.

## Running the Project

To run the project, follow these steps:

1. Clone the repository.
2. Navigate to the project directory.
3. Run the command `dotnet run`.

This will start the application and it will be accessible at `http://localhost:5000`.

## Endpoints

The following endpoints are currently available:

- `POST /Users/register`: Register a new user.
- `POST /Users/login`: Login a user and get an access token and refresh token.
- `POST /Users/refresh-token`: Refresh the access token using a refresh token.
- `POST /Users/revoke-token`: Revoke a refresh token.

In the future, additional endpoints for the blog API will be added.

## License

This project is licensed under the MIT License.