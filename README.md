In Order to make the application runnable follow the steps.

1- clone the repository

2- create appsettings.json file and add the following data.
  
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Error"
      }      
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "FitnessSystemConnection": "server=;database=;Trusted_connection=true;encrypt=false;"
    },
    "SMTPConfig": {
      "SenderAddress": "",
      "SenderDisplayName": "",
      "Username": "",
      "Password": "",
      "host": "smtp.gmail.com",
      "Port": 587,
      "EnableSSL": true,
      "UseDefaulCredentials": false,
      "IsBodyHTML": true
    }

3- schemaa.backup database in order to haves sample data

4- when run
use the admin credentials for signing in.
admin@gmail.com 
admin12345
