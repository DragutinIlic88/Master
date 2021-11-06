Repository contains master thesis on subject "Application of reactive distributed programming on the example of online banking services"

Document which describe system and principles behind the system is Master.pdf

System is web based and consists from two part: server side and client side.

For running server side of system follow next steps:

1. Open BankUsersContext.cs inside of /server/BankUsersDBContextLib folder;
2. Find modelBuilder.Entity<User>.HasData method and add data for the user which you want to be registered in system;
   2.1) New user can be also added after creation of BankUsers database inside of MSSQL server by adding insert statement;
3. Perform migrations of each database context used in application:
   3.1) If you do not have installed dotnet-ef CLI tool install it by typing
   dotnet tool install dotnet-ef --global
   command inside (for example) of Package Manager Console
   3.2) Check if it is installed by typing
   dotnet tool list --global
   3.3) Add migrations for each context used in system by typing following commands:
   dotnet ef migrations add "NameOfMigaritons" --context OnlineBankingContext
   dotnet ef migrations add "NameOfMigaritons" --context OnlineBankingAccountContext
   dotnet ef migrations add "NameOfMigaritons" --context OnlineBankingTransactionContext
   dotnet ef migrations add "NameOfMigaritons" --context OnlineBankingCurrencyContext
   dotnet ef migrations add "NameOfMigaritons" --context OnlineBankingHelpContext
   dotnet ef migrations add "NameOfMigaritons" --context OnlineBankingNotificationContext
   dotnet ef migrations add "NameOfMigaritons" --context BankUsersContext
   Every context is configured to use sql server and connection string for each can be found inside of /server/OnlineBankingWebApi/appsettings.json
4. You need to apply migrations which are created for each database by typing following commands:
   dotnet ef database update --context OnlineBankingContext
   dotnet ef database update --context OnlineBankingAccountContext
   dotnet ef database update --context OnlineBankingTransactionContext
   dotnet ef database update --context OnlineBankingCurrencyContext
   dotnet ef database update --context OnlineBankingHelpContext
   dotnet ef database update --context OnlineBankingNotificationContext
   dotnet ef database update --context BankUsersContext
5. When databases are created server side application can be runned by performming build and than run it with some IDE or command line

For running client side of system follow next steps inside of client foler:

1. Run npm install command which will install all the dependencies from package.json in node_modules folder
2. Run npm run build command so that webpack can bundle and build code
3. Run npm start so that application can be started

Client side will be runned on port 3000 while for server side it is used port 8000 for web api and 5002 and 5002 for actor system replicas.

After user performs registration and when first time log in to the system it will have no account nor currencies.
Arbitary number of accounts and currencies can be added for new user in AccountDB and CurrencyDB databases.
