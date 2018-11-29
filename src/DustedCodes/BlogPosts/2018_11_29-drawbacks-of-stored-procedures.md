<!--
    Tags: stored-procedures database architecture
-->

# Drawbacks of Stored Procedures

A few weeks ago I had a conversion with someone about the pros and cons of stored procedures. Personally I don't like them and try to avoid stored procedures as much as possible. I know there are some good reasons for using stored procedures (sometimes), but I'm also very well aware of the downsides which stored procedures bring with them.

This was not the first time that I had such a conversation and therefore I thought that I would quickly summarise all the reasons (and problems) which I had encountered with stored procedures in the past and put them into one concise blog post for future reference.

## Testability

First and foremost business logic which is encapsulated in stored procedures becomes very difficult to test (if tested at all).

Some developers prefer to write a thin data access layer on top of stored procedures to workaround this issue, but even in this instance the extent of testing is mostly limited to a few integration tests only. Writing unit tests for any business logic inside a stored procedure is not possible, because there is no way to clearly separate the domain logic from the actual data. Mocking, faking or stubbing won't be possible either.

## Debugging

Depending on the database technology debugging stored procedures will either not be possible at all or extremely clunky. Some relational databases, such as SQL Server, have some debugging capabilities and others none. There's nothing worse than having to use a database profiler to track down an application issue or to debug your database via print statements.

## Versioning

Versioning is another crucial feature which stored procedures don't support out of the box. Putting stored procedure changes into re-runnable scripts and placing them into a version control system is certainly advisable, but it doesn't solve the problem that there is nothing inside a stored procedure which tells us which version a stored procedure is on and if there wasn't any other change being made after the latest script had been applied.

## History

Similar to versioning, there's no history attached to stored procedures. Specifically if business logic spans across multiple stored procedures then it can be very difficult to establish the exact combination of different versions of different stored procedures at a given point in time.

## Branching

Branching is a wonderful feature which enables the isolation of related software changes until a certain piece of work has been completed. This also allows development teams to work on multiple changes simultaneously without breaking each others' code.

As soon as a stored procedure requires to change then a development team will either face the maintenance of multiple database instances for their affected branches or have to coordinate the deployment of different stored procedures throughout the entire development life cycle.

## Runtime Validation

Errors in stored procedures cannot be caught as part of a compilation or build step in a CI/CD pipeline. The same is true if a stored procedure went missing or another database error has crept into the application during the development process (e.g. missing permission to execute a stored procedure). In such a scenario a development team will often not know about the error until they execute the application. Catching fundamental mistakes like this can be very disruptive if it happens so late in the process.

## Maintainability

Stored procedures introduce a cliff (or disconnect) between coherent functionality, because the domain logic gets split between the application- and the database layer. It's rarely clear where the line is drawn (e.g. which part of a query should go into the application layer and which part into the database layer?). Code which is divided between two disconnected systems makes it harder to read, comprehend and therefore reason about.

## Fear of change

One of the biggest drawbacks of stored procedures is that it is extremely difficult to tell which parts of a system use them and which not. Especially if software is broken down into multiple applications then it's often not possible to find all references in one go (or at all if a developer doesn't have read access to all projects) and therefore it might be difficult to confidently establish how a certain change will affect the overall system. As a result stored procedures pose a huge risk of introducing breaking changes and development teams often shy away from making any changes at all. Sometimes this can lead to crippling new technological innovations.

## Logging and Error handling

Robust software often relies on very sophisticated logging frameworks and/or error handling modules. An exception can get logged in several places, events can be raised based on different severity levels and custom notifications can be sent out to selective members of the team. However, business logic which is encapsulated inside a stored procedure cannot directly benefit from the same tools without having to either duplicate some of the code  or introduce additional layers and workarounds.

## Deployments

Not entirely impossible, but if a stored procedure has to change as part of a new application version then a zero downtime deployment will become a lot more difficult than without it. It's much easier to deploy and run two different versions of a web services than having to run two different versions of a set of stored procedures.

## Conclusion

These are some of the issues which I had personally experienced when dealing with complex stored procedures in the past. There are obviously many good reasons for using stored procedures too, but overall I feel that the majority of these drawbacks are pretty big trade offs to swallow for very few benefits which can also be achieved without having to use a stored procedure.

If you think I've missed something or misstated one of the downsides of stored procedures then please let me know in the comments below.