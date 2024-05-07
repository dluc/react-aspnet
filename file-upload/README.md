# File Upload template

A template combining ASP.NET and React, using ASP.NET static files feature
to serve the React build, and ASP.NET minimal API to provide a web service.

This template shows how to upload files from React to ASP.NET supporting
large files (up to ~3Gb, configurable).

# UI changes, React development

To create and edit your React app, work from the [react](react) folder, edit the
React code as usual, like any other TypeScript app.

## Web service

If your React application requires your web service to be running, run the
.NET app (see below), and point React to https://127.0.0.1:9001/api/yourendpoints.
In this case TCP ports will differ, needing CORS.

The web service already includes a `POST /api/upload` endpoint accepting file uploads.

## Build

Once the React application code is ready, run `yarn build`. This publishes
the optimized React build under [react/build](react/build), needed later (see below).
As usual the React build merges and compresses all stylesheets and javascript
files.

# Web API changes, ASP.NET development

Open [app.sln](app.sln) in VS/VSCode/Rider and edit the .NET code as usual, like any
other ASP.NET app.

The code uses ASP.NET minimal API syntax, making it easy to add new endpoints.

The web service already includes a `POST /api/upload` endpoint accepting file uploads.

## Build

The ASP.NET build process copies files from [react/build](react/build)
to [aspnet/wwwroot](aspnet/wwwroot).
The logic used to copy these files is defined in [aspnet.csproj](aspnet/aspnet.csproj).

## Run frontend + backend

1. Build the React app (see above).
2. Build the ASP.NET app. The build process copies files from [react/build](react/build)
   to [aspnet/wwwroot](aspnet/wwwroot).
3. Start the ASP.NET app. The .NET app runtime provides both a Web API and
   a Web app, on the same port.
4. Open https://127.0.0.1:9001 (or http://127.0.0.1:9000) in your browser,
   enjoy.
