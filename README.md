# Wow-Loremaster
ASP.NET MVC web app for viewing your account-wide progress towards The Loremaster achievement in World of Warcraft.

# Wow-Loremaster
ASP.NET MVC web app for viewing your account-wide progress towards The Loremaster achievement in World of Warcraft.


# Setup
In order to connect to the battle.net APIs for user authentication and account/character data, you will first need to create an API key with Blizzard at https://develop.battle.net/.  Once you have created your API key, copy/paste the API key and API Secret to the web.config appSettings AuthApiKey and AuthApiSecret value fields, respectively.

Additionally, the application is configured to use ELMAH for error logging to a SQL database.  You can either add in a connection string to your ELMAH databse or reconfigure ELMAH for local logging.  If you choose to remove the dependancy on ELMAH entirely you will need to make a few code chages.
