# Matbaa

![](./printer.svg)

Matbaa is a mini API that provides an easy-to-use interface over Puppeteer to generate PDFs from HTML.

## Usage

You need a browser that exposes its remote debugging interface. You can launch a local Chrome instance or
run [Browserless](https://github.com/browserless/chrome) on Docker:

### Remote browser

Launch a `browserless/chrome` container with:

```powershell
docker run -p 12345:3000 browserless/chrome
```

Then use the following URL:

```
ws://localhost:12345/webdriver
```

### Local browser

Open up a shell and run:

```powershell
chrome.exe --headless --remote-debugging-port=9222 --no-first-run --no-default-browser-check  --user-data-dir=.chrome
```

This will launch a hidden Chrome window.

Then fetch the debugger URL using

```powershell
(curl --silent http://127.0.0.1: 9222/json/version | ConvertFrom-Json).webSocketDebuggerUrl
```

## Configuration

Once you have a running browser instance, set `Puppeteer:BrowserWsEndpoint` in `appsettings.json`.

Or via an environment variable:

```
export Puppeteer__BrowserWsEndpoint=ws://url.to.browser 
dotnet run
```

## API

Go to `/api` to view Swagger documentation.

## TODO:

- Add UI

