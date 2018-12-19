<!--
    Tags: fsharp csharp functional-programming
-->

# Why you should learn F#

If you were thinking of learning a new programming language in 2019 then I would highly recommend to have a close look at F#. No matter if you are already a functional developer from a different community (Haskell, Clojure, Scala, etc.) or you are a complete newbie to functional programming (like I was 3 years ago) I think F# can equally impress you. F# is a [functional first language](https://dotnet.microsoft.com/languages/fsharp). This means it is not a pure functional language but it is heavily geared towards the [functional programming paradigm](https://en.wikipedia.org/wiki/Functional_programming). However, because F# is also part of the [.NET language family](https://dotnet.microsoft.com/languages) it is equally well equipped to write object oriented code too. Secondly F# is - contrary to common believe - an extremely well designed [general purpose language](https://fsharpforfunandprofit.com/why-use-fsharp/). This means that F# is not only good for all sorts of "mathematical" stuff, but also for so much more. Without doubt F# is, like most other functional (algebraic) languages, greatly suited for this kind of work, but it is certainly not at the forefront of the creators of F# and neither a very common use case by most people who I know work with F#. So what is F# really good for? Well, the honest answer is almost anything! F# is an extremely pragmatic, expressive, statically typed programming language. Whether you want to build a distributed real time application, a service oriented web backend, a fancy looking single page app, mobile games, a line of business application or the next big social internet flop, F# will satisfy most if not all of your needs. As a matter of fact F# is probably a much better language for these types of applications than let's say Python, Java or C#. If you don't believe me then please continue reading and hopefully I will have convinced you by the end of this post!

## Table of contents

- [Domain Driven Development](#domain-driven-development)
- [Immutability and lack of Nulls](#immutability-and-lack-of-nulls)
- [SOLID made easy in F#](#solid-made-easy-in-fsharp)
- [Simplicity](#simplicity)
- [Asynchronous programming](#asynchronous-programming)
- [.NET Core](#net-core)
- [Open Source](#open-source)
- [Tooling](#tooling)
- [F# conquering the web](#fsharp-conquering-the-web)
- [F# Everywhere](#fsharp-everywhere)
- [Final Words](#final-words)
- [Useful Resources](#useful-resources)

<h2 id="domain-driven-development">Domain Driven Development</h2>

Before I started to write this article I asked myself why do I like F# so much? There are many reasons which came to my mind, but the one which really stood out to me was the fact that F# has some great capabilities of modelling a domain. After all, the majority of work which we do as software developers is to model real world processes into a digital abstraction of them. A language which makes this kind of work almost feel natural is immensely valuable and should not be missed.

Let's look at some code examples to demonstrate what I mean. For this task and for the rest of this blog post I'll be comparing F# with C# in order to show some of the benefits. I've chosen C# because many developers consider it as one of the best object oriented languages and mainly because C# is the language which I am the most proficient at myself.

### Identifying bad design

A common use case in a modern application is to read a customer object from a database. In C# this would look something like this:

```
public Customer GetCustomerById(string customerId)
{
    // do stuff...
}
```

I have purposefully omitted the internals of this method, because from a caller's point of view the signature of a method is often all we know. Even though this operation is so simple (and very familiar) there are still a lot of unknowns around it:

- Which values are accepted for the `customerId`? Can it have an empty string? Probably not, but will it instantly throw an `ArgumentException` or still try to fetch some user data?
- Does the ID follow a specific format? What if the `customerId` has the correct format but is all upper case? Is it case sensitive or will the method normalise the string anyway?
- What happens if a given `customerId` doesn't exist? Will it return `null` or throw an `Exception`? There's no way to find out without checking the internal implementation of this method (docs, decompilation, GitHub, etc.) or by testing against all sorts of input.
- What happens if the database connection is down? Will it return the same result as if the customer didn't exist or will it throw a different type of exception?
- How many different exception types will this code throw anyway?

The interface/signature of this method is not very clear in answering any of these questions. This is pretty poor given that the signature or interface of a method has the only purpose of defining a clear contract between the caller and the method itself. Of course there are many conventions which make C# developers feel safe, mostly by making broad assumptions about the underlying code, but at the end of the day these are only assumptions which can (and will eventually) lead to severe errors. If a library only slightly differs from an established convention then there is a high chance of introducing a bug which will catch them later.

If anything, conventions are rather weak workarounds for missing language features. Just like C# is perhaps seen as a better language than JavaScript, because of its statically typed feature, many functional programming languages are seen superior to C#, Java, and others, because of their domain modelling features.

There are ways of improving this code in C#, but none of those options are very straightforward (or often very cumbersome), which is why there is still plenty of code written like the one above.

### F# makes correct code easy

F# on the other hand has a rich type system which allows developers to express the true state of a function. If a function might or might not return a `Customer` object then the function can return an object of type `Option<'T>`.

The `Option<'T>` type defines a return value which can either be something or nothing:

```
let getCustomerById customerId =
    match db.TryFindCustomerById customerId with
    | true, customer -> Some customer
    | false, _       -> None
```

It is important to understand that `None` is not another way of saying `null`, because `null` is truly nothing (there is nothing allocated in memory), whereas `None` is an actual object/case of type `Option<'T>`.

In this example the `TryFindCustomerId` method is a typical .NET member which has an `out` parameter defined like this:

```
bool TryFindCustomerById(string customerId, out Customer customer)
```

In F# you can use simple pattern matching to extract the `out` parameter on success:

```
match db.TryFindCustomerById customerId with
| true, customer -> Some customer
| false, _       -> None
```

The benefit of the `Option<'T>` type is not only that it is more expressive (and therefore more honest about the true state of the function), but also that it forces the calling code to implement the case of `None`, which means that a developer has to think of this edge case straight from the beginning:

```
let someOtherFunction customerId =
    match getCustomerById customerId with
    | Some customer -> // Do something when customer exist
    | None          -> // Do something when customer doesn't exist
```

Another extremely useful type which comes with F# is the `Result<'T,'TError>` type:

```
let validateCustomerId customerId =
    match customerId with
    | null -> Error "Customer ID cannot be null."
    | ""   -> Error "Customer ID cannot be empty."
    | id when id.Length <> 10 -> Error "Invalid Customer ID."
    | _ -> Ok (customerId.ToLower())
```

The `validateCustomerId` function will either return `Ok` with a normalised `customerId` or an `Error` object which contains a relevant error message. In this example `'T` and `'TError` are both of type `string`, but it doesn't have to be the same type and you can even wrap multiple types into a much richer return value such as `Result<Option<Customer>, string list>`.

The type system in F# allows for even more flexibility. One can easily create a new type which truly represents all possible outcomes of a function like `getCustomerById`:

```
type DataResult<'T> =
    | Success         of 'T option
    | ValidationError of string
    | DataError       of Exception

let getCustomerById customerId =
    try
        match validateCustomerId customerId with
        | Error msg -> ValidationError msg
        | Ok    id  ->
            match db.TryFindCustomerById id with
            | true, customer -> Some customer
            | false, _       -> None
            |> Success
    with ex -> DataError ex
```

The custom defined `DataResult<'T>` type declares three distinctive cases which the calling code might want to treat differently. By explicitly declaring a type which represents all these possibilities we can model the `getCustomerById` function in such a way that it removes all ambiguity about error- and edge case handling as well as preventing unexpected behaviour and forcing calling code to handle these cases.

### F# makes invalid state impossible

So far we have always assumed that the `customerId` is a value of type `string`, but as we've seen this creates a lot of ambiguity around the allowed values for it and also forces a developer to write a lot of guard clauses to protect themselves from errors:

```
public Customer GetCustomerById(string customerId)
{
    if (customerId == null)
        throw new ArgumentNullException(nameof(customerId));

    if (customerId == "")
        throw new ArgumentException(
            "Customer ID cannot be empty.", nameof(customerId));

    if (customerId.Length != 10 || customerId.ToLower().StartsWith("c"))
        throw new ArgumentException(
            "Invalid customer ID", nameof(customerId));

    // do stuff...
}
```

The correct way of avoiding this anti-pattern is to model the concept of a `CustomerId` into its own type. In C# you can either create a `class` or `struct` to do so, but either way you'll end up writing a lot of boilerplate code to get the type to behave the way it should (eg. GetHashCode, Equality, ToString, etc.):

```
public class CustomerId
{
    public string Value { get; }

    public CustomerId(string customerId)
    {
        if (customerId == null)
            throw new ArgumentNullException(nameof(customerId));

        if (customerId == "")
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));

        var value = customerId.ToLower();

        if (value.Length != 10 || value.StartsWith("c"))
            throw new ArgumentException(
                "Invalid customer ID",
                nameof(customerId));

        Value = value;
    }

    // Lots of overrides to make a
    // CustomerId behave the correct way
}
```

Needless to say that this is extremely annoying and the exact reason why it is so rarely seen in C#. Also a class is less favourable, because code which will accept a `CustomerId` will still have to deal with the possibility of `null`, which is not really a thing. A `CustomerId` should never be `null`, just like an `int`, a `Guid` or a `DateTime` can never be `null`. Once you've finished implementing a correct `CustomerId` type in C# you'll end up with 200 lines of code which itself open up a lot of room for further errors.

In F# we can define a new type as easily as this:

```
type CustomerId = private CustomerId of string
```

This version of `CustomerId` is basically a wrapper of `string`, but provides additional type safety, because one couldn't accidentally assign a `string` to a parameter of type `CustomerId` or vice versa.

The private access modifier prevents code from different modules or namespaces to create an object of type `CustomerId`. This is intentional, because now we can force the creation via a specific function like this:

```
module CustomerId =
    let create (customerId : string) =
        match customerId with
        | null -> Error "Customer ID cannot be null."
        | ""   -> Error "Customer ID cannot be empty."
        | id when id.Length <> 10 -> Error "Invalid Customer ID."
        | _ -> Ok (CustomerId (customerId.ToLower()))
```

The above implementation is extremely efficient and almost free of any noise. As a developer I didn't have to write a lot of boilerplate code and was able to focus on the actual domain which is what I really want:

- The system has a type called `CustomerId`, which wraps a `string`.
- The only way to create a `CustomerId` is via the `CustomerId.create` function, which does all of the relevant checks before emitting an object of `CustomerId`.
- If a `string` violates the `CustomerId` requirements then a meaningful `Error` is returned and the calling code is forced to deal with this scenario.
- The `CustomerId` object is immutable and non-nullable. Once successfully created all subsequent code can confidently rely on correct state.
- The `CustomerId` type automatically inherits all other behaviour from a `string`, which means I didn't have to write a `GetHashCode` implementation, equality overrides, operator overloads and all of the other nonsense which I would have to do in C#.

This is a perfect example where F# can provide a lot of value with very few lines of code. Also because there is not much code to begin with there is very little room for making a mistake. The only real mistake I could have made is in the actual implementation of the `CustomerId` validation, which is more of a domain responsibility rather than a shortcoming of the language itself.

C# developers are not very used to model real world concepts like a `CustomerId`, an `OrderId` or an `EmailAddress` into their own types, because the language doesn't make it easy. These objects are often represented by very primitive types such as `string` or `int` and are being handled very loosely by the domain.

If you would like to learn more about [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) in F# then I would highly recommend to watch [Scott Wlaschin's Domain Modeling Made Functional](https://vimeo.com/223983252) presentation from NDC London. This is a fantastic talk with lots of food for thought and also the source of some of the ideas which I have introduced in this article:

<iframe src="https://www.youtube.com/embed/Up7LcbGZFuo" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

<h2 id="immutability-and-lack-of-nulls">Immutability and lack of Nulls</h2>

One of the greatest features of F# is that **objects in F# are immutable by default and cannot be null**. This makes it a lot easier to reason about code and also implement bug free applications. Not having to think twice if an object has changed state after passing it into a function or having to check for nulls has a huge impact on how easily someone can write reliable applications.

### Saying goodbye to nulls

[Tony Hoare](https://en.wikipedia.org/wiki/Tony_Hoare), who invented (amongst many other great things) the null reference [called it his billion dollar mistake](https://en.wikipedia.org/wiki/Null_pointer#History). He even apologised for the creation of `null` during QCon London in 2009.

**The problem with `null` is that it doesn't reflect any  real state and yet has too many meanings at the same time.** It's never clear if `null` means "unknown", "empty", "does not exist", "not initialised", "invalid", "some other error" or perhaps "end of line/file/stream/etc."? Today's scholars agree that the existence of `null` is certainly a mistake and hence why languages like [C# try to slowly move away from it in their upcoming versions](https://msdn.microsoft.com/en-us/magazine/mt829270.aspx).

Fortunately F# never had nulls to begin with. The only way to force a `null` into F# is by interoperating with C# and not properly fencing it off.

### Immutability > Mutability

Mutability is another topic where functional programming really shines. The problem is not mutability per se, but **whether objects are mutable by default or not** in a given language. It can only be one of the two and each programming language has to pick which one it wants it to be.

Immutability has the benefit of making code a lot easier to understand. It also prevents a lot of errors, because no class, method or function can change the state of an object after it had been created. This is particularly useful when objects get passed around between many different methods where the internal implementation is not always known (third party code, etc.).

On the other hand mutability doesn't have many benefits at all. It makes code arguably harder to follow, introduces a lot more ways for classes and methods to overstep their responsibility and lets poorly written libraries introduce unexpected behaviour. The small benefit of being able to directly mutate an object comes at a rather high cost.

Now the question is which one is easier, change an object in an immutable-by-default language, or introduce immutability in a mutable-by-default one?

I can quickly answer the first one by looking at F# and how it deals with the desire of changing an object's state. In F# mutations are performed by creating a new object with the modified values applied:

```
let c  = { Name = "Susan Doe"; Address = "1 Street, London, UK" }
let c' = { c with Address = "3 Avenue, Oxford, UK" }
```

This is a very elegant solution which produces almost the same outcome as if mutability was allowed (but without the cost).

Introducing immutability in C# is a little bit more awkward.

C# has no language construct which allows one to create an immutable object out of the box. First I have to create a new type, but I cannot use a `class`, because a `class` is a reference type which could be `null`. If `null` can be assigned to an object after it had been created then it is not immutable:

```
public class Customer
{
    public string Name { get; set; }
    public string Address { get; set; }
}

// Somewhere later in the program:

var c = new Customer();
c = null;
```

This leads me to using a `struct`:

```
public struct Customer
{
    public string Name { get; set; }
    public string Address { get; set; }
}
```

Now `null` is not an issue anymore, but the properties still are:

```
var c = new Customer();
c.Name = "Haha gotcha!";
```

Let's make the setters private then:

```
public struct Customer
{
    public string Name { get; private set; }
    public string Address { get; private set; }
}
```

Better, but not immutable yet. One could still do something like this:

```
public struct Customer
{
    public string Name { get; private set; }
    public string Address { get; private set; }

    public void ChangeName(string name)
    {
        Name = name;
    }
}

var c = new Customer();
c.ChangeName("Haha gotcha!");
```

The problem is not that `ChangeName` is public, but the fact that there is still a method which can alter the object's state after it was created.

Let's introduce two private backing fields for the properties and remove the setters altogether:

```
public struct Customer
{
    private string _name;
    private string _address;

    public string Name { get { return _name; } }
    public string Address { get { return _address; } }

    public Customer(string name, string address)
    {
        _name = name;
        _address = address;
    }
}
```

This looks perhaps better, but it's not (yet). A class member can still change the `_name` and `_address` fields from inside.

We can fix this by making the fields `readonly`:

```
public struct Customer
{
    private readonly string _name;
    private readonly string _address;

    public string Name { get { return _name; } }
    public string Address { get { return _address; } }

    public Customer(string name, string address)
    {
        _name = name;
        _address = address;
    }
}
```

Now this is immutable (at least for now), but a bit verbose. At this point we might as well collapse the properties into `public readonly` fields:

```
public struct Customer
{
    public readonly string Name;
    public readonly string Address;

    public Customer(string name, string address)
    {
        Name = name;
        Address = address;
    }
}
```

Alternatively with C# 6 (or later) we could also create readonly properties like this:

```
public struct Customer
{
    public string Name { get; }
    public string Address { get; }

    public Customer(string name, string address)
    {
        Name = name;
        Address = address;
    }
}
```

So far so good, but unless someone knows C# very well one could have easily gotten this wrong.

Unfortunately real world applications are never this simple though.

What if the `Customer` type would look more like this?

```
public class Address
{
    public string Street { get; set; }
}

public struct Customer
{
    public readonly string Name;
    public readonly Address Address;

    public Customer(string name, Address address)
    {
        Name = name;
        Address = address;
    }
}

var address = new Address { Street = "Springfield Road" };
var c = new Customer("Susan", address);
address.Street = "Gotcha";
```

At this point it should be evident that introducing immutability in C# is not as straightforward as someone might have thought.

This is another great example where the stark contrast between F# and C# really stands out. Writing correct code shouldn't be that hard and the language of choice can really make a difference.

<h2 id="solid-made-easy-in-fsharp">SOLID made easy in F#</h2>

Object oriented programming is all about producing [SOLID](https://en.wikipedia.org/wiki/SOLID) code. In order to understand and write decent C# one has to read at least five different books, [study 20+ design patterns](https://en.wikipedia.org/wiki/Software_design_pattern), follow [composition over inheritance](https://en.wikipedia.org/wiki/Composition_over_inheritance), practise [TDD](https://en.wikipedia.org/wiki/Test-driven_development) and [BDD](https://en.wikipedia.org/wiki/Behavior-driven_development), apply the [onion architecture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/), layer everything into tiers, [MVP](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93presenter), [MVVM](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel) and most importantly [single responsibility](https://en.wikipedia.org/wiki/Single_responsibility_principle) all the things.

We all know the importance of these principles, because they are vital in keeping object oriented code in maintainable shape. Object oriented developers are so used to practise these patterns that it is unimaginable to them that someone could possibly produce SOLID code without injecting everything through a constructor. I've been there myself. The first time I saw functional code it looked plain wrong to me. I think most C# developers are put off by F# when they look at functional code for the very first time and don't see anything which looks familiar to them. There is no classes, no constructors and most importantly no IoC containers.

**Functional code is often mistaken for procedural code to the inexperienced eye**.

In functional programming everything is a function. The only design pattern which someone has to know is that a function is a first class citizen. Functions can be composed, instantiated, partially applied, passed around and executed.

There is this [famous slide](//www.slideshare.net/ScottWlaschin/fp-patterns-ndc-london2014) by [Scott Wlaschin](https://twitter.com/ScottWlaschin) which nicely sums it up:

<iframe src="//www.slideshare.net/slideshow/embed_code/key/oCM5TxRgKh1vme?startSlide=15" width="595" height="485" frameborder="0" marginwidth="0" marginheight="0" scrolling="no" style="border:1px solid #CCC; border-width:1px; margin-bottom:5px; max-width: 100%;" allowfullscreen> </iframe>

This slide deck is from Scott Wlaschin's [Functional programming design patterns](https://vimeo.com/113588389) talk which can be viewed on Vimeo.

### To hell with interfaces

In C# everything requires an interface. For example if a method requires to support multiple sort algorithms then the [strategy pattern](https://en.wikipedia.org/wiki/Strategy_pattern) can help with this:

```
public interface ISortAlgorithm
{
    List<int> Sort(List<int> values);
}

public class QuickSort : ISortAlgorithm
{
    public List<int> Sort(List<int> values)
    {
        // Do QuickSort
        return values;
    }
}

public class MergeSort : ISortAlgorithm
{
    public List<int> Sort(List<int> values)
    {
        // Do MergeSort
        return values;
    }
}

public void DoSomething(ISortAlgorithm sortAlgorithm, List<int> values)
{
    var sorted = sortAlgorithm.Sort(values);
}

public void Main()
{
    var values = new List<int> { 9, 1, 5, 7 };
    DoSomething(new QuickSort(), values);
}
```

In F# the same can be done with simple functions:

```
let quickSort values =
    values // Do QuickSort

let mergeSort values =
    values // Do MergeSort

let doSomething sortAlgorithm values =
    let sorted = sortAlgorithm values
    ()

let main =
    let values = [| 9; 1; 5; 7 |]
    doSomething quickSort values
```

Functions in F#, which have the same signature, are interchangeable and don't require an explicit interface declaration. Both sort functions are therefore of type `int list -> int list` and can be alternatively passed into the `doSomething` function.

This is literally all it requires to implement the strategy pattern in in F#!

Let's look at a slightly more complex example to really demonstrate the strengths of F#.

### Everything is a function

One of the most useful patterns in C# and one of my personal favourites is the [decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern). It allows adding additional functionality to an existing class without violating the [open-closed principle](https://en.wikipedia.org/wiki/Open%E2%80%93closed_principle) of the SOLID guidelines. A password policy is a perfect example for this:

```
public interface IPasswordPolicy
{
    bool IsValid(string password);
}

public class BasePolicy : IPasswordPolicy
{
    public bool IsValid(string password)
    {
        return true;
    }
}

public class MinimumLengthPolicy : IPasswordPolicy
{
    private readonly int _minLength;
    private readonly IPasswordPolicy _nextPolicy;

    public MinimumLengthPolicy(int minLength, IPasswordPolicy nextPolicy)
    {
        _minLength = minLength;
        _nextPolicy = nextPolicy;
    }

    public bool IsValid(string password)
    {
        return
            password != null
            && password.Length >= _minLength
            && _nextPolicy.IsValid(password);
    }
}

public class MustHaveDigitsPolicy : IPasswordPolicy
{
    private readonly IPasswordPolicy _nextPolicy;

    public MustHaveDigitsPolicy(IPasswordPolicy nextPolicy)
    {
        _nextPolicy = nextPolicy;
    }

    public bool IsValid(string password)
    {
        if (password == null) return false;

        return password.ToCharArray().Any(c => char.IsDigit(c))
            && _nextPolicy.IsValid(password);
    }
}

public class MustHaveUppercasePolicy : IPasswordPolicy
{
    private readonly IPasswordPolicy _nextPolicy;

    public MustHaveUppercasePolicy(IPasswordPolicy nextPolicy)
    {
        _nextPolicy = nextPolicy;
    }

    public bool IsValid(string password)
    {
        if (password == null) return false;

        return password.ToCharArray().Any(c => char.IsUpper(c))
            && _nextPolicy.IsValid(password);
    }
}

public class Programm
{
    public void Main()
    {
        var passwordPolicy =
            new MustHaveDigitsPolicy(
                new MustHaveUppercasePolicy(
                    new MinimumLengthPolicy(
                        8, new BasePolicy())));

        var result = passwordPolicy.IsValid("Password1");
    }
}
```

During the instantiation of the `passwordPolicy` object one can decide which policies to use. A different password policy can be created without having to modify a single class. While this works really well in C#, it is also extremely verbose. There is a lot of code which had to be written for arguably little functionality at this point. I also had to use an additional interface and constructor injection to glue policies together. The `passwordPolicy` variable is of type `IPasswordPolicy` and can be injected anywhere a password policy is required. This is as good as it gets in C#.

The only thing which I could have possibly improved (by writing a lot more boilerplate code) would have been to add additional syntactic sugar to compose a policy like this:

```
var passwordPolicy =
    Policy.Create()
        .MustHaveMinimumLength(8)
        .MustHaveDigits()
        .MustHaveUppercase();
```

In F# the equivalent implementation is "just" functions again:

```
let mustHaveUppercase (password : string) =
    password.ToCharArray()
    |> Array.exists Char.IsUpper

let mustHaveDigits (password : string) =
    password.ToCharArray()
    |> Array.exists Char.IsDigit

let mustHaveMinimumLength length (password : string) =
    password.Length >= length

let isValidPassword (password : string) =
    mustHaveMinimumLength 8 password
    && mustHaveDigits password
    && mustHaveUppercase password
```

Just like in C# the `passwordPolicy` object implemented the `IPasswordPolicy` interface, the `isValidPassword` function implements the `string -> bool` signature which therefore can be interchanged with any other function which also implements `string -> bool`.

The F# solution is almost embarrassingly easy when compared to the overly complex one in C#. Yet I didn't have to compromise on any of the SOLID principles. Each function validates a single requirement (single responsibility) and can be tested in isolation. They can be swapped or mocked for any other function which also implements `string -> bool` and I can create multiple new policies without having to modify existing code (open closed principle):

```
let isValidPassword2 (password : string) =
    mustHaveMinimumLength 12 password
    && mustHaveUppercase password
```

### Inversion of Control made functional

The only pattern which a functional developer has to understand is functions. To prove my point one last time I'll explore the [Inversion of Control principle](https://en.wikipedia.org/wiki/Inversion_of_control) next.

First let's be clear what the Inversion of Control principle is, because many developers wrongly confuse it with the dependency injection pattern. The Inversion of Control principle states that a class shall never instantiate its own dependencies itself. [Martin Fowler](https://martinfowler.com/) uses the term [Hollywood Principle](https://martinfowler.com/bliki/InversionOfControl.html) as in *"Don't call us, we'll call you"*.

There are three distinctive design patterns which follow the IoC principle:

- Dependency Injection
- Factory
- Service Locator

The [Service Locator is considered an anti pattern](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/) so I won't go any further here.

The Factory pattern consists of two further sub-patterns:

- [Abstract Factory](https://en.wikipedia.org/wiki/Abstract_factory_pattern)
- [Factory Method](https://en.wikipedia.org/wiki/Factory_method_pattern)

The [Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection) pattern breaks down into three more sub-patterns:

- Constructor Injection
- Method Injection
- Property Injection

Despite [Constructor Injection](https://martinfowler.com/articles/injection.html#ConstructorInjectionWithPicocontainer) being the most popular IoC pattern in object oriented programming, it is only one of many other patterns which follow the Dependency Inversion Principle. Each of these patterns is extremely useful and satisfies a specific use case which Constructor Injection couldn't do on its own.

This has nothing to do with F# directly, but I wanted to underline how the sheer number of different design patterns can sometimes be very confusing. It may take years for an OO software engineer to fully grasp the vast amount of concepts and understand how and when they play an important role.

Now that I got this out of the way let's take a look at how C# handles Dependency Injection via Constructor Injection:

```
public interface INotificationService
{
    void SendMessage(Customer customer, string message);
}

public class OrderService
{
    private readonly INotificationService _notificationService;

    public OrderService(INotificationService notificationService)
    {
        _notificationService =
            notificationService
            ?? throw new ArgumentNullException(
                nameof(notificationService));
    }

    public void CompleteOrder(Customer customer, ShoppingBasket basket)
    {
        // Do stuff

        _notificationService.SendMessage(customer, "Your order has been received.");
    }
}
```

Nothing should be surprising here. The `OrderService` has a dependency on an object of type `INotificationService` which is responsible for sending order updates to a customer.

There could be multiple implementations of the `INotificationService`, such as an `SmsNotificationService` or an `EmailNotificationService`:

```
public class EmailNotificationService : INotificationService
{
    private readonly EmailSettings _settings;

    public EmailNotificationService(EmailSettings settings)
    {
        _settings = settings;
    }

    public void SendMessage(Customer customer, string message)
    {
        // Do stuff
    }
}
```

Typically in C# these dependencies would get registered in an IoC container. I've skipped this part in order to keep the C# implementation small as it's already becoming large.

Now let's take a look at how dependency injection can be done in F#:

```
let sendEmailNotification emailSettings customer message =
    ()

let sendSmsNotification smsService apiKey customer message =
    ()

let completeOrder notify customer shoppingBasket =
    notify customer "Your order has been received."
    ()
```

That's it - Dependency Injection in functional programming is achieved by simply passing one function into another (basically what I've already been doing in the examples before)!

The only difference here is that the `sendEmailNotification` and `sendSmsNotification` functions do not share the same signature at the moment. Not only is `emailSettings` of a different type than `smsService`, but both functions also differ in the number of parameters they need. The `sendEmailNotification` function requires three parameters in total and the `sendSmsNotification` requires four. Furthermore the `notify` parameter of the `completeOrder` function doesn't know which concrete function will be injected and therefore doesn't care about anything except the `Customer` object and the `string` message. So how does it work?

The answer is **partial application**. In functional programming one can partially apply parameters of one function in order to generate a new one:

```
let sendEmailFromHotmailAccount =
    // Here I only apply the `emailSettings` parameter:
    sendEmailNotification hotmailSettings

let sendSmsWithTwillio =
    // Here I only apply the `smsService` and `apiKey` parameters:
    sendSmsNotification twilioService twilioApiKey
```

After partially applying both functions the newly created `sendEmailFromHotmailAccount` and `sendSmsWithTwillio` functions share the same signature again:

```
Customer -> string -> unit
```

Now both functions can be passed into the `completeOrder` function.

There is no need for an IoC container either. If one doesn't want to repeatedly pass all dependencies into the `completeOrder` function then partial application can be utilised once again:

```
let completeOrderAndNotify =
    emailSettings
    |> sendEmailNotification
    |> completeOrder

// Later in the program one would use:

completeOrderAndNotify customer shoppingBasket
```

If we compare this solution to the one from C# then there isn't much of a difference (except for simplicity). Classes mainly require their dependencies to be injected through their constructor and functions take other functions as a dependency. In C# all dependencies get registered only once at IoC container level. In F# all dependencies get "registered" only once through partial application. In both cases one can create mocks, stubs and fakes for their dependencies and unit test each class or function in isolation.

There is a few advantages with the functional approach though:

- Dependencies can get "registered" (partially applied) closer to the functions where they belong.
- Simpler by having a lot less code.
- No additional (third party) IoC container required.
- Dependency Injection is a pattern which has to be taught in OO programming whereas passing a function into another function is the most fundamental/normal thing one could do in functional programming.

[Mark Seeman](http://blog.ploeh.dk/), author of [Dependency Injection in .NET](https://www.manning.com/books/dependency-injection-in-dot-net), did a fantastic talk on more advanced dependency patterns in F#. Watch his talk "[From dependency injection to dependency rejection](https://www.youtube.com/watch?v=xG5qP5AWQws)" on YouTube:

<iframe src="https://www.youtube.com/embed/xG5qP5AWQws" frameborder="0" allow="accelerometer; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

<h2 id="simplicity">Simplicity</h2>

If there is one theme which has been consistent throughout this blog post then it must be the remarkable simplicity of F#. No matter if it is creating a new immutable type, expressing the true state of a function, modelling a domain or applying advanced programming patterns, F# always seems to have a slight edge over C#.

The abstinence of classes, complex design patterns, IoC containers, mutability, inheritance, overrides and interfaces has a few more benefits which come extremely handy at work.

First there is a lot less code to write. This makes applications smaller, faster to comprehend and much easier to maintain.

Secondly it allows for blazingly fast prototyping. In F# one can very quickly hack one function after another until a desired prototype has been reached. Furthermore, the additional work to transition from prototype to production is almost nothing. Since everything is a function and gets naturally compartmentalised into smaller functions the difference between a prototype- and a production-ready function is often very little.

<h2 id="asynchronous-programming">Asynchronous programming</h2>

Speaking of simplicity, F# makes asynchronous programming strikingly easy:

```
let readFileAsync fileName =
    async {
        use stream = File.OpenRead(fileName)
        let! content = stream.AsyncRead(int stream.Length)
        return content
    }
```

There is a lot of great content available which explains the differences and benefits of F#'s asynchronous programming model so that I won't rehash everything again, but I would highly recommend to read [Thomans Petricek](http://tomasp.net/)'s article on [Async in C# and F#: Asynchronous gotchas in C#](http://tomasp.net/blog/csharp-async-gotchas.aspx/), followed by his blog series on [Asynchronous C# and F#](http://tomasp.net/blog/csharp-fsharp-async-intro.aspx/), including [How do they differ?](http://tomasp.net/blog/async-csharp-differences.aspx/) and [How does it work?](http://tomasp.net/blog/async-compilation-internals.aspx/).

<h2 id="net-core">.NET Core</h2>

So far I've talked mostly about generic concepts of the functional programming paradigm, but there is a wealth of benefits which come specifically with F#. The obvious one is [.NET Core](https://github.com/dotnet/core). As we all know Microsoft is putting a lot of work into their new open source, cross platform, multi language runtime.

F# is part of .NET and therefore runs on all .NET runtimes, which include [.NET Core](https://en.wikipedia.org/wiki/.NET_Core), [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework) and [Xamarin](https://en.wikipedia.org/wiki/Xamarin) (Mono). This means that anyone can develop F# on either Windows, Linux or macOS. It also means that F# developers have access to a large eco system of extremely mature and high quality libraries. Because F# is a multi paradigm language (yes you can write object oriented code too if you want) it can reference and call into any third party package no matter if it was written in F#, C# or VB.NET.

<h2 id="open-source">Open Source</h2>

Long time before Microsoft embraced the OSS community they debuted with F# as their first language which was born out of [Microsoft Research](https://www.microsoft.com/en-us/research/) as an open source project from the get go. The open source community behind F# is very strong, with many contributions coming from outside Microsoft and driving the general direction of the language.

You can find all [F# source code](https://github.com/fsharp) hosted on GitHub and start contributing by submitting an [F# language suggestion](https://github.com/fsharp/fslang-suggestions) first. When a suggestion gets approved then an [RFC](https://github.com/fsharp/fslang-design/tree/master/RFCs) gets created with a corresponding [discussion thread](https://github.com/fsharp/fslang-design/issues).

The F# language is under the direction of the [F# Foundation](https://fsharp.org/) with strong backing by Microsoft who is still the main driver of development.

<h2 id="tooling">Tooling</h2>

There is no match when it comes to tooling. Microsoft's .NET languages have always benefited from excellent tooling. [Visual Studio](https://visualstudio.microsoft.com/vs/) was the uncontested leader for a long time, but in recent years the competition has racked up. [JetBrains](https://www.jetbrains.com/), the company who invented [ReSharper](https://www.jetbrains.com/resharper/), has released a new IntelliJ driven cross platform IDE called [Rider](https://www.jetbrains.com/rider/). Meanwhile Microsoft developed a new open source editor called [Code](https://github.com/Microsoft/vscode). [Visual Studio Code](https://code.visualstudio.com/) has quickly emerged as the [most popular development environment](https://insights.stackoverflow.com/survey/2018/#technology-most-popular-development-environments) amongst programmers and boasts a huge marketplace of useful plugins. Thanks to [Krzysztof Cieślak](https://twitter.com/k_cieslak) there is a superb extension called [Ionide](http://ionide.io/) for F#.

Visual Studio, JetBrains Rider and Visual Studio Code with Ionide are three of the world's best programming IDEs which are cross platform compatible, run on all major operating systems and support F#.

<h2 id="fsharp-conquering-the-web">F# conquering the web</h2>

As I mentioned at the very beginning F# is not just a language for algebraic stuff. Functional programming in general is a perfect fit for anything web related. A web application is basically a large function with a single parameter input (HTTP request) and a single parameter output (HTTP response).

### F# on the Backend

F# has an abundance of diverse and feature rich web frameworks. My personal favourite is a library called [Giraffe](https://github.com/giraffe-fsharp/Giraffe) (disclaimer: I am the core maintainer of this project). [Giraffe](https://github.com/giraffe-fsharp/Giraffe) sits on top of [ASP.NET Core](https://www.asp.net/core), which means that it mostly piggybacks off the entire ASP.NET Core environment, its performance attributes and community contributions. In Giraffe a web application is composed through a combination of many smaller functions which get glued together via the Kleisli operator:

```
let webApp =
    choose [
        GET >=>
            choose [
                route "/ping" >=> text "pong"
                route "/"     >=> htmlFile "/pages/index.html"
            ]
        POST >=> route "/submit" >=> text "Successful" ]
```

[Giraffe](https://github.com/giraffe-fsharp/Giraffe) has also recently joined the [TechEmpower Web Framework Benchmarks](https://www.techempower.com/benchmarks/#section=data-r17&hw=ph&test=plaintext) and ranks with a total of **1,649,957 req/sec** as one of the fastest functional web frameworks available.

However, if Giraffe is not to your taste then there are many other great F# web libraries available:

- [Saturn](https://saturnframework.org/) (F# MVC framework built on top of Giraffe)
- [Suave](https://suave.io/) (An entire web server written in F#)
- [Freya](https://freya.io/)
- [WebSharper](https://websharper.com/)

ASP.NET Core and ASP.NET Core MVC are also perfectly compatible with F#.

### F# on the Frontend

After F# set its mark on the server side of things it has also seen a lot of innovation on the frontend of the web.

[Fable](https://fable.io/) is an F# to JavaScript transpiler which is built on top of [Babel](https://babeljs.io/), which itself is an extremely advanced JavaScript compiler. Babel, which is hugely popular and [backed by large organisations](https://opencollective.com/babel#contributors) such as Google, AirBnb, Adobe, Facebook, trivago and many more, is doing the heavy lifting of the compilation, whereas Fable is transpiling from F# to Babel's own abstract syntax tree. In simple terms you get the power of F# combined with the maturity and stability of Babel which allows you to write rich frontends in F#. [Alfonso Garcia-Caro](https://twitter.com/alfonsogcnunez) has done a magnificent job in merging the F# and JavaScript communities and recently [released Fable 2](https://fable.io/blog/Introducing-2-0-beta.html) which comes with a two-fold speed boost as well as a 30%-40% reduced bundle size.

[Fable](https://github.com/fable-compiler/Fable) and [Babel](https://github.com/babel/babel) are also open source and have a thriving community behind them.

On a complete different front Microsoft has worked on a new project called [Blazor](https://github.com/aspnet/Blazor). Blazor is a single-page web application framework built on .NET that runs in the browser with WebAssembly. It supports all major .NET languages including F# and is [currently in beta](https://blogs.msdn.microsoft.com/webdev/2018/11/15/blazor-0-7-0-experimental-release-now-available/).

With the availability of Fable and Blazor there is a huge potential of what an F# developer can do on the web today.

<h2 id="fsharp-everywhere">F# Everywhere</h2>

F# is one of very few languages which can truly run anywhere! Thanks to [.NET Core](https://dotnet.microsoft.com/) one can develop F# on any OS and run on any system. It can run natively on Windows, Linux and macOS or via a [Docker](https://www.docker.com/) container in a [Kubernetes](https://kubernetes.io/) cluster. You can also run F# serverless functions in [AWS Lambda](https://aws.amazon.com/lambda/) or [Azure Functions](https://azure.microsoft.com/en-gb/services/functions/). [Xamarin App Development](https://visualstudio.microsoft.com/xamarin/) brings F# to Android, iOS and Windows apps, and [Fable](https://fable.io/) and [Blazor](https://blazor.net/) into the browser. Since [.NET Core 2.1](https://blogs.msdn.microsoft.com/dotnet/2018/05/30/announcing-net-core-2-1/) one can even run F# on [Alpine Linux](https://alpinelinux.org/) and [ARM](https://www.arm.com/)! Machine learning, IoT and games are yet other areas where F# can be used today.

The list of supported platforms and architectures has been growing every year and I'm sure it will expand even further in the future!

<h2 id="final-words">Final Words</h2>

I've meant to write this blog post for a long time but never found the time to do it up until recently. My background is mainly C#, which is what I have been programming for more than ten years now and what I am still doing today. In the last three years I have taught myself F# and fallen madly in love with it. As a convert I get often asked what I like about F# and therefore I decided to put everything into writing. The list is obviously not complete and only a recollection of my own take on the main benefits of F#. If you think that I have missed something then please do not hesitate and let me know in the comments below. I see this blog post as an ever evolving resource where I hope I can point people to who have an interest in F#.

This blog post is also part of the [F# Advent Calendar 2018](https://sergeytihon.com/2018/10/22/f-advent-calendar-in-english-2018/) which has been kindly organised by [Sergey Tihon](https://sergeytihon.com/) again. Sergey does not only organise the [yearly F# Advent Calendar](https://sergeytihon.com/2017/10/22/f-advent-calendar-in-english-2017/), but also runs a [weekly F#](https://sergeytihon.com/category/f-weekly/) newsletter. [Subscribe to his newsletter](https://sergeytihon.com/) or [follow him on Twitter](https://twitter.com/sergey_tihon) and stay up to date with the latest developments on F#!

<h2 id="useful-resources">Useful Resources</h2>

### Blog and Websites

- [F# Foundation](https://fsharp.org/)
- [F# Guide](https://docs.microsoft.com/en-gb/dotnet/fsharp/)
- [F# for fun and profit](https://fsharpforfunandprofit.com/)
- [ploeh blog](http://blog.ploeh.dk/)
- [Tomas Petricek](http://tomasp.net/)
- [Sergey Tihon Weekly F#](https://sergeytihon.com/)
- [F# all the things](https://atlemann.github.io/)
- [SAFE Stack](https://safe-stack.github.io/)

### Videos

- [Domain Modeling Made Functional](https://www.youtube.com/watch?v=Up7LcbGZFuo&t=36s)
- [F# for C# programmers](https://www.youtube.com/watch?v=KPa8Yw_Navk)
- [Functional Design Patterns](https://www.youtube.com/watch?v=srQt1NAHYC0)
- [A gentle introduction to F#](https://www.youtube.com/watch?v=Fssvnaf8bMo)

### Books

- [Get Programming with F#](https://www.manning.com/books/get-programming-with-f-sharp) by [Isaac Abraham](https://cockneycoder.wordpress.com/)
- [Domain Modeling Made Functional](https://fsharpforfunandprofit.com/books/) by [Scott Wlaschin](https://fsharpforfunandprofit.com/)
- [Stylish F#](https://www.amazon.com/Stylish-Writing-More-Productive-Elegant/dp/1484239997) by [Kit Eason](http://www.kiteason.com)

### Conferences

- [F# Exchange](https://skillsmatter.com/conferences/10869-f-sharp-exchange-2019)
- [Open FSharp](https://www.openfsharp.org/)
- [FableConf](https://fable.io/fableconf/)
- [Lambda Days](http://www.lambdadays.org/lambdadays2019)