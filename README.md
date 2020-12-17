# BrowserStack-C-Sharp-Percy
Percy visual testing for C# Selenium.

## Setup
> If your project does not already have a package.json file in the root directory, run npm init and follow the prompts. Without a package.json present in the project, the next install step will fail.

```
$ npm install --save-dev @percy/agent
```
This will install the percy agent executable in a node_modules/ folder inside the current working directory. You can access the agent's executable at ./node_modules/.bin/percy.

In order to start creating snapshots from your C# Selenium scripts or tests, you'll need to import the BrowserStackPercy class. You will need to do this in each file from which you want to take snapshots:
```
using BrowserStackPercy;
```
After you instantiate a Selenium WebDriver, create a new Percy instance:
```
Percy percy = new Percy(driver);
```
Use percy.snapshot(...) to generate a snapshot:
```
percy.snapshot("Snapshot Name");
```

Before you can successfully run Percy, the PERCY_TOKEN environment variable must be set:
```
# Windows 
$ set PERCY_TOKEN=<your token here>

# Windows PowerShell
$env:PERCY_TOKEN = "<your token here>"

# Unix 
$ export PERCY_TOKEN=<your token here>
```

Finally, wrap your program or test runner command in the percy exec command. This will start a Percy agent to receive snapshots from your tests and render screenshots for you to review in your Percy dashboard. For example, if you are using Maven to run your tests, your new test command becomes:

```
npx percy exec -- mvn test
```
**Note:** the double dash, --, between percy exec and your test run command.

That's it! Now, whenever CI runs, a snapshot of the app in that state will be uploaded to Percy for visual regression testing!
