SETTING UP FRONEND (React client)
1.	Open . \Frontend\merchant-react in Visual Studio Code
2.	In console execute:
•	npm install
•	npm run

SETTING UP BACKEND (API server)
1.	Open Merchant.sln in Visual Studio
2.	Open appsettings.json and edit 'DefaultConnection' => to point your new empty DB
3.	Open Package Manager console: 
•	 in the default project combo, the 'Mechant.API' project should be selected
•	 using cd/ cd.. command navigate to Merchan.API folder
•	 run command: dotnet ef database update
•	 run command: dotnet ef database update --context OrderContext
4.	 Run solution

Running app
a).	 Although swagger has been configured for solution, you may face some issues with using it to call endpoint 
 https://github.com/swagger-api/swagger-js/issues/1163 (swagger and supporting cookie authentication)
b).	 For testing/running -> please use Postman
c).	 Both front and backend are configured to use https (some installed certs are required for front app to run it correctly)
d).	 After migrating DB, you will have 4 accounts available for testing:
•	Administrator (login - password)
	admin		1234
•	Customers (login - password)
	customer1	1234
	customer2	1234
	customer3	1234
