from flask import *
import time

portfolios = [

    { 'name': 'PORTFOLIO_A', 'is_disabled': False },
    { 'name': 'PORTFOLIO_B', 'is_disabled': False },
    { 'name': 'PORTFOLIO_C', 'is_disabled': True  },
    { 'name': 'PORTFOLIO_D', 'is_disabled': False },
    { 'name': 'PORTFOLIO_E', 'is_disabled': False },

]

 

holdings = {
    'PORTFOLIO_A': [ { 'stock_id': 'AMZN', 'value': 1000 }, { 'stock_id': 'GOOGL', 'value': 1000 }, { 'stock_id': 'APPL',  'value': 1000 } ],
    'PORTFOLIO_B': [ { 'stock_id': 'AMZN', 'value': 2000 }, { 'stock_id': 'MSFT',  'value': 2000 }, { 'stock_id': 'GOOGL', 'value': 3000 } ],
    'PORTFOLIO_C': [ { 'stock_id': 'AMZN', 'value': 3000 }, { 'stock_id': 'GOOGL', 'value': 4400 }, { 'stock_id': 'APPL',  'value': 4000 } ],
    'PORTFOLIO_D': [ { 'stock_id': 'AMZN', 'value': 2000 }, { 'stock_id': 'MSFT',  'value': 2000 }, { 'stock_id': 'APPL',  'value': 5000 } ],
    'PORTFOLIO_E': [ { 'stock_id': 'AMZN', 'value': 6400 }, { 'stock_id': 'GOOGL', 'value': 6000 }, { 'stock_id': 'APPL',  'value': 6000 } ],
    'PORTFOLIO_F': [ { 'stock_id': 'AMZN', 'value': 2100 }, { 'stock_id': 'GOOGL', 'value': 7400 }, { 'stock_id': 'APPL',  'value': 7000 } ]
} 

cash = {
    'PORTFOLIO_A': { 'value': 100 },
    'PORTFOLIO_B': { 'value': 200 },
    'PORTFOLIO_C': { 'value': None },
    'PORTFOLIO_D': { 'value': 300 },
    'PORTFOLIO_E': { 'value': 400 },
    'PORTFOLIO_F': { 'value':   0 },
}

 
app = Flask(__name__)

@app.route("/")
def index():
    return "The server is running"
    
@app.route("/portfolios")
def get_portfolios():
    return portfolios 

@app.route("/<portfolio>/holdings")
def get_holdings(portfolio):
    try:
        time.sleep(2)
        return holdings[portfolio]    
    except:
        abort(404)
   
@app.route("/<portfolio>/cash")  
def get_cash(portfolio):
    try:
        time.sleep(1)
        return cash[portfolio]
    except:
        abort(404)