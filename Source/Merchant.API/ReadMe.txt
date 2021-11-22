For Creating DB objects
1. Open appsettings.json and edit 'DefaultConnection' => to point your new empty DB
2. Open Package Manager console: 
 => int the default project combo, the 'Mechant.API' project should be selected
 => using cd/ cd.. command navigate to Merchan.API folder
 => run command: dotnet ef database update
 => run command: dotnet ef database update --context OrderContext
 3. Enjoy initialized DB


 Running app
 1. Although swagger has been configured for solution, you may face some issues with using it to call endpoint 
 https://github.com/swagger-api/swagger-js/issues/1163 (swagger and supporting cookie authentication)
 2. For testing/running -> please use Postman
 3. Both front and backend are configured to use https (some installed certs are required for front app to run it correctly)
 4. After migrating DB, you will have 4 accounts available for testing:
 a) Administrator (login - password)
	admin		1234
 b) Customers (login - password)
	customer1	1234
	customer2	1234
	customer3	1234
