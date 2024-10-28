# NAM-API
.NET API serving Financial information built by fetching a Python API.
Below is a server written in python with an API that exposes some portfolio related data.
 
The task is to create an application in C# that uses the python API to expose another API that can provide the following List of portfolios that contain a given stock Cash as a fraction of full portfolio value for all portfolios as a single call
 
.NET API Server
Things to consider:
Execution-time
User-friendlyness of the API
Testing

The solution contains: 
.NET Solution that exposes an API to answer the requirements with a corresponding Test Project. 

 
Python API Server
In order to start the python api:
Make sure you have python installed
Pip install flask
Run the server by invoking
flask --app server run
 
End-points provided by the python API are:
/portfolios
Gives a list of all portfolios
/<portfolio>/holdings
Gives a list of all holdings for a given portfolio, replace <portfolio> with the name of a portfolio from the /portfolios call
/<portfolio>/cash
Gives the amount of cash for a given portfolio, replace <portfolio> with the name of a portfolio from the /portfolios call

Run it from: NAM-Python-API\server.py 

