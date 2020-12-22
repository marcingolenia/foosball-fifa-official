# Context:
* Many of us are avid foosball players, but running around the office to check what is the status of the game is both tiring and wastes lots of our precious time. Therefore, we would appreciate a bit of help.

# Task:
* Implement a simple microservice for tracking status of foosball games.

# Our Foosball Rules:
* Each game consist from sets
* We play in BO3 system, meaning that the first team that wins 2 sets, wins the game
* Each set consist from goals
* The first team to score 10 goals wins a set
* Goals, sets and games can only be incremented (no "minus" goals, sets or games are allowed)

# Business Requirements:
* As an API user, I'd like to create and update status of a foosball game, so a progress can be tracked
* As an API user, I'd like to list all games sorted by start date (descending), so I could check the details of the one that is interesting to me
* As an API user, I'd like to see the details of a particular game, so I could check if it was one-sided

# Technical Requirements:
* .NET Core
* C# or F#
* RESTful API
* Some kind of data storage, i.e. a file, SQL or document database
* (optional, if you plan to write tests) Some kind of tests framework, i.e. xUnit, nUnit, Unquote

# Additional Notes:
* Try to not spend more than 4 hours on this task. If you don’t manage to implement everything, no worries - we’d be very pleasantly surprised if you did
* Use common sense if something wasn't specified
* Try to deliver a working solution
* If possible keep your code on GitHub or at least use git to track changes
