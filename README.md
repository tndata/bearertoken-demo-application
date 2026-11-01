
# ASP.NET Core BearerToken demo application

A small ASP.NET Core application that shows the **BearerToken authentication handler**, introduced in .NET 8. Instead of a session cookie, the handler issues an **access token and a refresh token** when a user signs in. The tokens are meant for use between a client and the application, not for calling external APIs, and they are not JWTs.

This is the sample code for my blog post [BearerToken: The new Authentication handler in ASP.NET Core 8](https://nestenius.se/net/bearertoken-the-new-authentication-handler-in-net-8/), where I explain how the handler works, what is inside its tokens, and how it compares to the Cookie and JwtBearer handlers.

## What the demo does
- `/login` signs in a hard-coded user, with a few standard and custom claims, and shows the access token and refresh token as JSON.
- `/` is the home page. When you send a request with a valid access token, it lists the claims taken from the token.
- `/logout` is included for completeness. The BearerToken handler does not handle sign-out, so it does nothing.

## Run it
1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and open the solution in the `BearerToken - demo project` folder.
2. Start the application and open its `/login` page in a browser. You get the access and refresh tokens as JSON.
3. In a client like Postman, make a `GET` request to the home page, choose **Auth > Bearer Token**, paste the access token and press **Send**.
4. You get an HTML page back that shows the claims extracted from the token.

You cannot just browse to the home page, because the handler expects the client to send the token itself, which a browser does not do.

## Good to know
- The demo uses a transparent data protector (`MyDataProtector`) that does **not** encrypt the tokens, so you can look inside them. Never use it in production.
- You can switch to the Cookie handler by uncommenting the cookie code in `Program.cs`. The application then works as an ordinary web application with a session cookie.

## Frequently asked questions

The answers come from my blog post, which was written for ASP.NET Core 8 in 2023. Check the current documentation if you use a newer version.

### What is the BearerToken authentication handler?
It is an authentication handler that Microsoft added in ASP.NET Core 8, as part of its work to improve authentication and identity. It can be seen as an alternative to the Cookie handler: instead of issuing a session cookie when a user signs in, it issues an access token and a refresh token, and it authenticates later requests from the token sent by the client.

### Are the BearerToken tokens JWTs?
No. The tokens are not JWTs and are not meant for accessing external APIs. They are meant to be used between a client and your web application. By default they are encrypted with the Data Protection API.

### How is it different from the Cookie and JwtBearer handlers?
All three authenticate user requests. The Cookie handler and the BearerToken handler also handle sign-in, and the difference is how the user's identity travels: in a session cookie, or in an access token that the client sends in the `Authorization` header. The JwtBearer handler validates JWT access tokens that were issued elsewhere, for example by an OpenID Connect provider.

### Can I look inside the tokens?
Yes, if you replace the default data protector with a transparent one, as this demo does. The tokens then contain the user's claims and an expiry time, base64 encoded, so any base64 decoder shows what is inside. Do this only for learning.

### What is the refresh token for?
The handler issues refresh tokens but contains no code to handle them. They are meant to be consumed by another library, such as ASP.NET Core Identity. The refresh token holds the same user information as the access token, with a different expiry time.

### Why can't I run it in the browser?
The handler is meant for clients that sign in programmatically, so the client must put the token in the `Authorization` header. A browser does not do that for you, which is why the demo uses a tool like Postman.

### Is the BearerToken handler a good fit for single-page apps?
I have mixed feelings about issuing and storing tokens in a public client such as a browser or mobile app, since such clients cannot keep secrets. I would probably put a BFF-style proxy between the client and the server. Read more about [the BFF pattern](https://nestenius.se/bff/).

### What are its limitations?
Two things I noticed: the tokens can grow large when they hold many claims, and there is no session store option like the Cookie handler has (`ITicketStore`). You also cannot customise what goes into the refresh token.

## Learn more
- Blog post: [BearerToken: The new Authentication handler in ASP.NET Core 8](https://nestenius.se/net/bearertoken-the-new-authentication-handler-in-net-8/)
- More posts: [Debugging cookie problems in ASP.NET Core](https://nestenius.se/net/debugging-cookie-problems/), [Troubleshooting JwtBearer authentication problems](https://nestenius.se/net/troubleshooting-jwtbearer-authentication-problems-in-asp-net-core/), [What's new in ASP.NET Core authentication in .NET 11](https://nestenius.se/net/whats-new-in-asp-net-core-authentication-in-net-11/)
- Workshop: [Authentication and Authorization in ASP.NET Core](https://tn-data.se/courses/authentication-and-authorization-in-aspnet-core/)
- Video courses on [Dometrain](https://dometrain.com/author/tore-nestenius/):
  - [Getting Started: Authentication and Authorization in .NET](https://dometrain.com/course/getting-started-authentication-and-authorization-in-dotnet/)
  - [Getting Started: OpenID Connect](https://dometrain.com/course/getting-started-openid-connect/)

## About the author
This code was written by [Tore Nestenius](https://nestenius.se/), a Microsoft .NET MVP and independent consultant and trainer from Göteborg, Sweden, specializing in authentication, identity and security in .NET.

* [LinkedIn](https://www.linkedin.com/in/torenestenius/)
* [Blog](https://nestenius.se/)
* [Company](https://tn-data.se/)
* [Dometrain Author](https://dometrain.com/author/tore-nestenius/)
