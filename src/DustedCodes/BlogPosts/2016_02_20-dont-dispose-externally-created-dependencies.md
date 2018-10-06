<!--
    Tags: ioc architecture dotnet disposable
-->

# Don't dispose externally created dependencies

Today I wanted to blog about a common mistake which I often see during code reviews or when browsing open source projects in relation to the `IDisposable` interface. Heck, even Microsoft got it wrong when they wrote on [how to implement the UnitOfWork pattern](http://www.asp.net/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) in an ASP.NET MVC application.

What I mean is this, in a very simplified way:

<pre><code>public interface IRepository : IDisposable
{
    // Member definition
}

public class MyClass : IDisposable
{
    private readonly IRepository _repository;

    public MyClass(IRepository repository)
    {
        _repository = repository;
    }

    public void Dispose()
    {
        _repository.Dispose();
    }
}</code></pre>

The issue with the above implementation is this little detail:

<pre><code>public void Dispose()
{
    _repository.Dispose();
}
</code></pre>

Disposing an externally created dependency is a bad idea and usually leads to severe bugs in an application. `MyClass` did not create the instance of `IRepository`, but decided to dispose it on behalf of the creator. However, `MyClass` has no information on who or what created the instance or for how long it is supposed to live. What if the object has been setup as a Singleton or an instance per HttpRequest? If that would be the case then any code which follows this and relies on an `IRepository` is broken now. `MyClass` has crossed its boundaries by interfering with the lifespan of an externally managed dependency.

It doesn't even matter what the current lifespan is. It could have been setup as a transient object, but the point is that a developer should be able to change the lifespan in the future without breaking the entire application. This does not only apply to the repository but to any dependency which is managed outside a class. The rule of thumb is that **you cannot dispose an object which you did not create** yourself.

The question is what shall we do with dependencies which implement `IDisposable`?

## How to manage a dependency which implements IDisposable?

The intentions were definitely good. The repository implements `IDisposable` obviously for a good reason. `IDisposable` is usually used [when a class has some expensive or unmanaged resources](https://msdn.microsoft.com/en-us/library/system.idisposable) allocated which need to be released after their usage. Not disposing an object can lead to memory leaks. In fact it is so important that the C# language offers the [using](https://msdn.microsoft.com/en-us/library/yh598w02.aspx?f=255&MSPPError=-2147217396) keyword to minimize risk of not cleaning up resources in certain edge cases such as the occurence of an exception. Generally it should be a red flag if an instance of `IDisposable` has not been wrapped in a `using` block, which is another good indicator that the code from above has a fundamental problem. I am actually surprised that no one at Microsoft picked this up before publishing the [article on the UnitOfWork](http://www.asp.net/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) pattern.

In most cases the creator of an object will be the IoC container. Therefore it should be the IoC container's responsibility to dispose an object after its usage, except that this is very difficult at this stage of an application. Either the object will live too long or too short which brings us back to the initial problem.

Typically if something implements `IDisposable` we want to dispose it as soon as possible. Often we even want to dispose it way sooner than the lifespan of the class where it has been used, which is yet another indicator that disposing it in the `Dispose()` method of `MyClass` is not a good place. This makes me doubt if injecting the repository through the constructor is a good idea at all.

## Taking control of creating and disposing an IDisposable

Constructor injection is only one of many IoC patterns which we have to our availability. Another approach would be to use a factory which will issue a new repository on every request:

<pre><code>public interface IRepository : IDisposable
{
    // Member definition
}

public interface IRepositoryFactory
{
    IRepository Create();
}

public class MyClass
{
    private readonly IRepositoryFactory _repositoryFactory;

    public MyClass(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public string GetSomething()
    {
        using (var repository = _repositoryFactory.Create())
        {
            return repository.GetSomething();
        }
    }
}</code></pre>

The constructor parameter has been changed from an `IRepository` to an `IRepositoryFactory`. The instantiation of the repository has been deferred to the the actual time of usage and the `Dispose()` method has been made redundant. Additionally I was able to benefit from the `using` keyword and overall reduce the total amount of code. This solution is much more elegant, more robust and free of the initial error.

The big difference is that `MyClass` takes care of creating a repository now. Per definition a factory always creates a new instance, so there is no ambiguity about the object's lifespan. In terms of testability and extensibility nothing has really changed. The factory gets injected through the constructor which makes it easily exchangable with another implementation or a mock in tests.

## Dependency injection is not always the best option

This is a great example where dependency injection is not always the best suited IoC pattern. [Dependency injection in all three forms](https://en.wikipedia.org/wiki/Dependency_injection#Three_types_of_dependency_injection) (constructor, property and method injection) is only one of many possible ways of the [dependency inversion principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle). Factories and other [creational design patterns](https://en.wikipedia.org/wiki/Creational_pattern) are still useful and have their place in modern software architecture.

Subtle differences like the one from above can determine wether you have a severe bug in your application or not. Sometimes it is not even a question of stylistic preference.

Another good read on the proper use of the `IDisposable` interface is [this amazing StackOverflow answer](http://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface#answer-538238) by [Ian Boyd](http://stackoverflow.com/users/12597/ian-boyd).