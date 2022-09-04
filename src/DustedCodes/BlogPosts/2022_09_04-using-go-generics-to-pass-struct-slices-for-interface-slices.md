<!--
    Tags: golang
-->

# Using Go generics to pass struct slices for interface slices

Have you ever tried to pass a struct slice into a function which accepts a slice of interfaces? In Go this won't work.

Let's have a quick look at an example. Let's assume we have an interface called `Human` and a function called `GreetHumans` which accepts a slice of humans and prints their names:

```
package main

import "fmt"

type Human interface {
	Name() string
}

func GreetHumans(humans []Human) {
	for _, h := range humans {
		fmt.Println("Hello " + h.Name())
	}
}
```

Then we have a separate struct which implements the `Human` interface:

```
type Hero struct {
	FirstName string
	LastName  string
}

func (h Hero) Name() string {
	return h.FirstName + " " + h.LastName
}
```

Nothing unusual so far.

Now one can create an object of type `Hero` and pass it into any function that requires an object of `Human`. That is expected.

However, the issue occurs when one deals with a slice of `Hero` and a function accepts a slice of `Human`.

This code won't compile:

```
func main() {
	heroes := []Hero{
		{FirstName: "Peter", LastName: "Parker"},
		{FirstName: "Bruce", LastName: "Wayne"},
	}

	GreetHumans(heroes) // <-- Compilation error here
}
```

![Go incompatible assignment error](https://cdn.dusted.codes/images/blog-posts/2022-09-04/go-incompatible-assignment-error.jpg)

Even though all heroes are humans the compiler won't accept this assignment.

You wonder why? Simply because Go doesn't want to hide expensive operations behind convenient syntax:

![Go type cast explanation](https://cdn.dusted.codes/images/blog-posts/2022-09-04/go-type-cast-explanation.jpg)

One has to iterate through a slice of `Hero` themselves and convert each object of `Hero` explicitly to a `Human` in order to cast the entire slice before passing it into the `GreetHumans` function. The cost of this conversion becomes immediately visible to the programmer.

What did Go programmers do up until recently?

Well there were mainly three options:

1. Create a conversion function for each individual type which implements the `Human` interface
2. Create a "generic" conversion function using `interface{}`
3. Create a "generic" conversion function using reflection

Option 1 is extremely tedious and option 2 and 3 provide very weak (or basically no) type safety (because checks would be only performed at runtime).

Since Go 1.18 one can use [Generics](https://go.dev/blog/intro-generics) to tackle the issue.

First we can modify the `GreetHumans` function to use Generics and therefore not require any casting at all:

```
func GreetHumans[T Human](humans []T) {
	for _, h := range humans {
		fmt.Println("Hello " + h.Name())
	}
}
```

This makes it possible to pass the `heroes` slice into the `GreetHumans` functions now.

However, sometimes it's not possible to make a function generic. Methods (functions on a type) require all type parameters to be on the type. [Parameterized methods are not allowed](https://go.googlesource.com/proposal/+/refs/heads/master/design/43651-type-parameters.md#No-parameterized-methods).

The good news is that even in this case Generics can help to provide a much better conversion function then the previously listed options:

```
func CastToHumans[T Human](humans []T) []Human {
	result := []Human{}
	for _, h := range humans {
		result = append(result, h)
	}
	return result
}
```

Here the generic `CastToHumans` function provides type safety at the time of compilation. It still remains an expensive operation but at least it cannot be used in an improper way any longer.

I wasn't sure if this is going to work and I was positively surprised to find out that it does indeed.

It's another neat use case of Generics in Go!