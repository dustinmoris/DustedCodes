<!--
    Tags: golang security
-->

# Building a secure note sharing service in Go

Welcome to my first [Go](https://go.dev) related article which I am releasing on my blog.

In this blog post I’ll be using Go to develop a super small web service which can be used by people or organisations to share secrets in a slightly more private way. We all have the occasional need to share a secret with a co-worker or another person. It may be an API key, a password or even some confidential data from a customer. When we share secrets via channels such as Slack, Teams or Email then we essentially send the secret to the servers of a complete stranger. We have no oversight over how the data is being handled, how long it will persist on third party servers and who the people are who have access to it. Sending secrets directly via Slack or Teams can also pose other unwanted side effects. For instance new employees who get added to an existing channel could discover previously shared confidential data via a channel's chat history. That could be a breach of security in itself if those employees didn't have the necessary clearance beforehand. Overall secrets and/or confidential data should never be shared directly via (untrusted) third party channels.

I thought writing a small data sharing app could be a good way of learning Go. The goal is to create a small web service which can be run as a single binary or from a Docker container inside a company's own infrastructure. Why rely on an (untrusted) third party service ([noterip](https://www.noterip.com), [safenote](https://safenote.co/private-message), [onetimesecret](https://onetimesecret.com), [circumvent](https://password.link) or [privnote](https://privnote.com)) if one could run their own?

## The Foundation

This is going to be an MVP so we’ll be making some fast gains by keeping the service extremely simple and making use of Redis as the main persistence layer. Redis seems to be a good fit for an MVP as it can be easily hosted in a container and can be used as a distributed data store that can serve multiple instances of our app. We can also make use of the TTL (time to live) feature which gives us a quick and dirty implementation of short lived, self destructing links.

Our web service will be a simple Go executable which can also run in a container and which will implement basic functionality to persist and retrieve a secret.

The entire solution will be [open source with an OSS friendly Apache 2.0 license](https://github.com/dustinmoris/self-destruct-notes) so that people can fork it and make their own modifications to it.

I call this project `self-destruct-notes`:

![self-destruct-notes GitHub repository](https://cdn.dusted.codes/images/blog-posts/2022-07-26/self-destruct-in-golang-1.png)

For the purpose of this blog post I'll keep the service very rudimental and use as few third party dependencies as possible. I'm actually coding this project as I'm writing this blog post so one can follow the evolution of this app through this article or the associated [commit history in Git](https://github.com/dustinmoris/self-destruct-notes/commits/main).

## Creating a new Go project

First let’s create a simple Go project to kick things off.

I'll start with a basic `main.go` file which yields a typical "hello world" message and then I run a couple `go mod` commands to initiliase the project:

#### main.go:

```
package main

import "fmt"

func main() {
    fmt.Println("hello world")
}
```

#### Terminal:

```
go mod init github.com/dustinmoris/self-destruct-notes
go mod tidy
```

Running `go run .` now will return `hello world`.

For the people who are new to Go (my main readership comes from .NET), the `go mod` commands are Go’s way of managing third party packages. They generate a `go.mod` and `go.sum` file when the service takes on any external dependencies. For now the `go.mod` file remains mostly empty:

#### go.mod:

```
module github.com/dustinmoris/self-destruct-notes

go 1.17
```

## Creating a simple hello world web server

Next I’m going to change the `main` function from a `hello world` console application to a `hello world` web server. We’re going to read the `PORT` environment variable to establish the port which our HTTP server should be listening to. If that variable is not set then we’ll default to port `3000`.

The “web server” itself will be a basic HTTP handler of the form `func (http.ResponseWriter, *http.Request)`:

```
package main

import (
    "fmt"
    "net/http"
    "os"
)

func main() {
    port := os.Getenv("PORT")
    if len(port) == 0 {
        port = "3000"
    }
    addr := ":" + port

    fmt.Printf("Starting web server, listening on %s\n", addr)

    err := http.ListenAndServe(addr, http.HandlerFunc(webServer))
    if err != nil {
        panic(err)
    }
}

func webServer(w http.ResponseWriter, r *http.Request) {
    w.WriteHeader(200)
    w.Write([]byte("hello world"))
}
```

If I hit `go run .` now then our web server will start and I'll be able to `curl localhost:3000` to get a `hello world` response:

![cURLing Go web server](https://cdn.dusted.codes/images/blog-posts/2022-07-26/self-destruct-in-golang-2.png)

## Adding basic routing

Now that we have a super tiny web server running we can add basic routing for the three main endpoints which I'd like to support:

- `Get /` (start page with a form to create a new note)
- `POST /` (handles the form post to persist a new note)
- `GET /{noteID}` (returns a previously saved note)

So far our web server is just a singe function and we had to use the `http.HandlerFunc` wrapper in order to implement the `http.Handler` interface on the function. Another way of defining the server would have been to create a struct type which implements the `http.Handler` interface by exposing a `ServeHTTP` function. Personally I prefer this option because I know that we will require some dependencies for Redis and other functionality later on and management of those dependencies will be easier that way.

Below I have refactored the `webServer` function into a `Server` struct and added some basic routing to it:

```
type Server struct{}

func (s *Server) ServeHTTP(
    w http.ResponseWriter,
    r *http.Request,
) {
    if r.Method == "GET" || r.Method == "HEAD" {
        noteID := strings.TrimPrefix(r.URL.Path, "/")
        w.WriteHeader(http.StatusOK)
        w.Write([]byte(
            fmt.Sprintf(
                "You requested the note with the ID '%s'.",
                noteID)))
        return
    }

    if r.Method == "POST" && r.URL.Path == "/" {
        w.WriteHeader(http.StatusOK)
        w.Write([]byte("You posted to /."))
        return
    }

    w.WriteHeader(http.StatusNotFound)
    w.Write([]byte("Not Found"))
}
```

As you can see I didn’t use any third party packages to add more complex routing capabilities. Our server only needs basic functionality which can be easily satisfied by the standard library. Although this is slightly more verbose than in other languages it keeps the Go code very simple and easy to understand.

Additionally I have also changed the `http.ListenAndServe` function call to accept a new `Server` object:

```
http.ListenAndServe(addr, &Server{})
```

Next I'm splitting the web server code into smaller functions. I am creating a new handler for posting notes, another handler for getting notes and a helper function to return a `404 Not Found` response.

After this refactoring the `ServeHTTP` function serves as the main routing handler and nothing else:

```
func (s *Server) ServeHTTP(w http.ResponseWriter, r *http.Request) {
    if r.Method == "GET" || r.Method == "HEAD" {
        s.handleGET(w, r)
        return
    }
    if r.Method == "POST" && r.URL.Path == "/" {
        s.handlePOST(w, r)
        return
    }
    s.notFound(w, r)
}

func (s *Server) notFound(w http.ResponseWriter, r *http.Request) {
    w.WriteHeader(http.StatusNotFound)
    w.Write([]byte("Not Found"))
}

func (s *Server) handlePOST(w http.ResponseWriter, r *http.Request) {
    w.WriteHeader(http.StatusOK)
    w.Write([]byte("You posted to /."))
}

func (s *Server) handleGET(w http.ResponseWriter, r *http.Request) {
    noteID := strings.TrimPrefix(r.URL.Path, "/")
    w.WriteHeader(http.StatusOK)
    w.Write([]byte(fmt.Sprintf("You requested the note with the ID '%s'.", noteID)))
}
```

That's a very quick and neat way to keep the Go web server code well structured and yet super simple at the same time.

##  Adding HTML views

The next step is to add some basic HTML to the project. We need a HTML template which we can return on the `/` index page when someone visits the service.

It's up to an individual to decide how they like to structure their code, but I generally place all web content which needs to get shipped alongside the Go executable into a `/dist` folder. With this in mind I've added two initial HTML templates to the project:

- `/dist/layout.html`
- `/dist/index.html`

The `layout.html` is the main layout page which can be re-used by other templates. Other programming languages might call this a "master page" (e.g. MVC). The `index.html` is the template which mainly includes the HTML form for creating a new note.

For simplicity I've kept them extremely small and I've added only a tiny bit of CSS to make the UI passable to the eye. If you're not familiar with Go's templating language then I'd recommend to have a quick look at the [templates in my Git repository](https://github.com/dustinmoris/self-destruct-notes/tree/main/dist) itself.

In order to respond with a HTML template on the index (`/`) route I added one helper function to our `Server` struct and covered the `/` route in the `handleGET` handler:

#### Helper function:

```
func (s *Server) renderTemplate(
	w http.ResponseWriter,
	r *http.Request,
	data interface{},
	name string,
	files ...string,
) {
	t := template.Must(template.ParseFiles(files...))
	err := t.ExecuteTemplate(w, name, data)
	if err != nil {
		panic(err)
	}
}
```

The `renderTemplate` function is just a small convenience method to parse all requested template files and initialise a `*template.Template` object and then write the result combined with the `data` model to the HTTP response body.

This method could be optimised by computing all known templates in advance but for brevity I'll keep it this way for now.

If you're new to Go and wonder what `files ...string` means in the function declaration then just know that this is a special form of specifying a string array where the function can accept a dynamic amount of string parameters at the end of a function call. It's similar to what `params string[]` does in C#.

Using the `renderTemplate` function from within the `handleGET` method looks like this now:

#### handleGET:

```
func (s *Server) handleGET(
	w http.ResponseWriter,
	r *http.Request,
) {
	path := r.URL.Path
	if path == "/" {
		s.renderTemplate(
            w, r, nil,
            "layout",
            "dist/layout.html",
            "dist/index.html")
		return
	}

	noteID := strings.TrimPrefix(path, "/")
	w.WriteHeader(http.StatusOK)
	w.Write([]byte(fmt.Sprintf("You requested the note with the ID '%s'.", noteID)))
}
```

In the `handleGET` function I check for the root path of the application and call the newly created `renderTemplate` function to complete the request. The [index.html](https://github.com/dustinmoris/self-destruct-notes/blob/main/dist/index.html) template doesn't require any model at this point and therefore I kept the `data` argument `nil`. The `name` argument is set to `layout` because this is the [name I chose](https://github.com/dustinmoris/self-destruct-notes/blob/main/dist/layout.html#L1) for the `layout.html` template at the top of the file.

Once everything is put together one should see the following UI when visiting `http://localhost:3000`:

![Self Destruct Notes Form UI](https://cdn.dusted.codes/images/blog-posts/2022-07-26/self-destruct-in-golang-3.png)

## Adding a Redis dependency

Now that the service can display a simple HTML page to create a note we need to implement the logic to actually handle the HTTP POST request and save the note on the backend. As mentioned before I'll use Redis for the purpose of this MVP.

First I'll add the required dependencies to the `go.mod` file and also reference them in the `main.go` file as part of the `import` declaration:

#### go.mod

```
module github.com/dustinmoris/self-destruct-notes

go 1.17

require (
	github.com/go-redis/cache/v8 v8.4.3
	github.com/go-redis/redis/v8 v8.11.4
)
```

#### main.go

```
import (
	"fmt"
	"net/http"
	"os"
	"strings"

	"github.com/go-redis/cache/v8"
	"github.com/go-redis/redis/v8"
)
```

In order to access the Redis cache from our `Server` struct we also need to add a reference in the struct declaration itself:

```
type Server struct {
	RedisCache *cache.Cache
}
```

Finally we can initialise a Redis cache object from within the application's `main` function and subsequently pass it into the `server` object before launching the service:

```
redisURL := os.Getenv("REDIS_URL")
if len(redisURL) == 0 {
    redisURL = "redis://:@localhost:6379/1"
}

redisOptions, err := redis.ParseURL(redisURL)
if err != nil {
    panic(err)
}
redisClient := redis.NewClient(redisOptions)
defer redisClient.Close()
redisCache := cache.New(&cache.Options{
    Redis: redisClient,
})
server := &Server{
    RedisCache: redisCache,
}
```

The code is mostly self explanatory but I'll do a quick run through anyway. Similar to the port we also read the Redis URL (connection string) from an environment variable which I chose to call `REDIS_URL`. If none was provided then we assume a default Redis instance to run behind the default port `6370`. Using the `redis.ParseURL` function we can convert the "magic" string variable into a strongly typed object which encapsulates all the Redis options. Using the `redisOptions` object we can then initialise a Redis client. Go has a neat way of deferring the closure of the connection to the end of the function using the `defer` keyword:

```
defer redisClient.Close()
```

In the case of the `main` function that would be the case when the application shuts down. This is not quite the same but very similar to how one would write code using C#'s `using` statement.

Finally a cache object is being initialised using the `redisClient` and then passed into our `Server` struct.

## Saving notes

Now that the `Server` struct has everything it needs we can implement the actual logic to persist a message. For that purpose we'll extend the `handlePOST` method, which is the handler which we call when someone sends a HTTP POST to the root (`/`) of the application.

For good measure let's run some basic validation first:

```
func (s *Server) handlePOST(
	w http.ResponseWriter,
	r *http.Request,
) {
	mediaType := r.Header.Get("Content-Type")
	if mediaType != "application/x-www-form-urlencoded" {
		s.badRequest(
			w, r,
			http.StatusUnsupportedMediaType,
			"Invalid media type posted.")
		return
	}

	err := r.ParseForm()
	if err != nil {
		s.badRequest(
			w, r,
			http.StatusBadRequest,
			"Invalid form data posted.")
		return
	}
	form := r.PostForm

    //...
```

If the HTTP request didn't include the `application/x-www-form-urlencoded` HTTP header then we return a `400 Bad Request` response. The same is true if the form data cannot be successfully parsed by the server (because it didn't adhere to the url encoded format).

If everything was okay then we can access the posted form data via the `r.PostForm` property and assign it to a `form` variable.

Next I'll attempt to read the `message` and `ttl` (time to live) fields from the form and initialise matching variables to hold their values:

```
message := form.Get("message")
destruct := false
ttl := time.Hour * 24
if form.Get("ttl") == "untilRead" {
    destruct = true
    ttl = ttl * 365
}
```

A note will either persist for 24 hours or until read (but no longer than a maximum of 365 days). The `destruct` variable indicates whether a note should get destroyed immediately after it was opened or if it should remain until the end of the 24 hour period.

Using the initialised data we can create a `note` object of type `Note`:

```
type Note struct {
	Data     []byte
	Destruct bool
}
```

... then inside `handlePOST` prepare a `note` which will be subsequently stored in a db:

```
note := &Note{
    Data:     []byte(message),
    Destruct: destruct,
}
```

The last step before completing the request is to actually write the note to Redis:

```
key := uuid.NewString()
err = s.RedisCache.Set(
    &cache.Item{
        Ctx:            r.Context(),
        Key:            key,
        Value:          note,
        TTL:            ttl,
        SkipLocalCache: true,
    })
if err != nil {
    fmt.Println(err)
    s.serverError(w, r)
    return
}
```

The `RedisCache` object which our `Server` is holding makes the storing of the note super easy.

By the way, the `s.badRequest(...)` and `s.serverError(...)` functions are further two small convenience methods which I've added to the `Server` struct to make error responses slightly easier:

```
func (s *Server) badRequest(
	w http.ResponseWriter,
	r *http.Request,
	statusCode int,
	message string,
) {
	w.WriteHeader(statusCode)
	w.Write([]byte(message))
}

func (s *Server) serverError(
	w http.ResponseWriter,
	r *http.Request,
) {
	w.WriteHeader(http.StatusInternalServerError)
	w.Write([]byte("Ops something went wrong. Please check the server logs."))
}
```

Anyhow, coming back to `handlePOST` the last remaining job is to print a friendly message with a link to the newly created note at the end of a successful POST:

```
noteURL := fmt.Sprintf("%s/%s", s.BaseURL, key)
w.WriteHeader(http.StatusOK)
s.renderMessage(
    w, r,
    "Note was successfully created",
    template.HTML(
        fmt.Sprintf("<a href='%s'>%s</a>", noteURL, noteURL)))
```

There's a couple new things in this code. First there is the `BaseURL` variable on the `Server` struct. This is something that I've added when instantiating the server object in the `main` function. It makes the `Server` struct aware of the public URL which should get displayed to the user after a successful POST (after all we won't show them a localhost URL in production).

The other thing is the `renderMessage` method, which is another helper function which outputs a new HTML page called `message.html` with an anonymous model:

#### renderMessage

```
func (s *Server) renderMessage(
	w http.ResponseWriter,
	r *http.Request,
	title string,
	paragraphs ...interface{},
) {
	s.renderTemplate(
		w, r,
		struct {
			Title      string
			Paragraphs []interface{}
		}{
			Title:      title,
			Paragraphs: paragraphs,
		},
		"layout",
		"dist/layout.html",
		"dist/message.html",
	)
}
```

#### message.html

```
{{ define "header" }}
<style>
    h3 {
        margin: 2rem 0 1rem 0;
    }
</style>
{{ end }}

{{ define "content" }}

<h3>{{ .Title }}</h3>
{{ range $i, $p := .Paragraphs }}
<p>{{ $p }}</p>
{{ end }}

<p><a href="/">Back to home</a></p>

{{ end }}
```

Thanks to Docker we can easily spin up a new instance of Redis and try out the app so far:

```
docker run -p 6379:6379 redis:latest
```

If everything went according to plan then creating a note should return a response like this now:

![Self Destruct Notes Success Message](https://cdn.dusted.codes/images/blog-posts/2022-07-26/self-destruct-in-golang-4.png)

## Retrieving notes

At last we need to implement the logic to retrieve a previously saved note. This can be done inside the `handleGET` method. I would like to point out that `handlePOST` and `handleGET` are obviously not brilliant function names for a larger web application, but for this small app where our service has only ever 3 endpoints to deal with those names are appropriate enough. It keeps a nice balance between structure and simplicity for an app that won't need more than 250 lines of code when finished.

Reading a note from Redis is very easy using its key. In order to get the key we assume that everything that follows the forward slash in the URL will be part of the note ID:

```
noteID := strings.TrimPrefix(path, "/")
```

Then we can try to find the note inside Redis using its ID:

```
ctx := r.Context()
note := &Note{}
err := s.RedisCache.GetSkippingLocalCache(
    ctx,
    noteID,
    note)
if err != nil {
    s.badRequest(
        w, r,
        http.StatusNotFound,
        fmt.Sprintf("Note with ID %s does not exist.", noteID))
    return
}
```

If the user desired the note to be destroyed after it was opened then we should honour their request as well:

```
if note.Destruct {
    err := s.RedisCache.Delete(ctx, noteID)
    if err != nil {
        fmt.Println(err)
        s.serverError(w, r)
        return
    }
}
```

If everything was fine so far then we finish the request by outputting the contents of the note itself:

```
w.WriteHeader(http.StatusOK)
w.Write(note.Data)
```

Voila, this completes the `handleGET` method and the application itself:

```
func (s *Server) handleGET(
	w http.ResponseWriter,
	r *http.Request,
) {
	path := r.URL.Path
	if path == "/" {
		s.renderTemplate(
			w, r, nil,
			"layout",
			"dist/layout.html",
			"dist/index.html")
		return
	}

	noteID := strings.TrimPrefix(path, "/")

	ctx := r.Context()
	note := &Note{}
	err := s.RedisCache.GetSkippingLocalCache(
		ctx,
		noteID,
		note)
	if err != nil {
		s.badRequest(
			w, r,
			http.StatusNotFound,
			fmt.Sprintf("Note with ID %s does not exist.", noteID))
		return
	}

	if note.Destruct {
		err := s.RedisCache.Delete(ctx, noteID)
		if err != nil {
			fmt.Println(err)
			s.serverError(w, r)
			return
		}
	}

	w.WriteHeader(http.StatusOK)
	w.Write(note.Data)
}
```

## Links

The full [source code is available on GitHub](https://github.com/dustinmoris/self-destruct-notes) and licensed under a permissive Apache-2.0 license.

I have also added a [GitHub Action to build and publish](https://github.com/dustinmoris/self-destruct-notes/blob/main/.github/workflows/build.yml) a [public Docker image on Docker Hub](https://hub.docker.com/repository/docker/dustinmoris/self-destruct-notes).

## Next steps

This web service was just a little toy project to dip my toes into Go. I am not planning on making it much better beyond this point, although if someone would send me a PR I would probably accept it. If I did want to improve it I'd probably swap Redis for a different db and also encrypt the note before persisting it for additional security against database attacks.

## How to run

The easiest way to start the public Docker image is by running this docker-compose:

#### docker-compose.yml

```
version: "3.9"

services:
  db:
    image: redis:latest
    ports:
      - "6379:6379"
  web:
    image: dustinmoris/self-destruct-notes:1.0.0
    environment:
      - PORT=3000
      - REDIS_URL=redis://:@db:6379/1
    ports:
      - "3000:3000"
    depends_on:
      - db
```

#### Terminal

```
docker compose up
```

## Closing words

I love how quickly I was able to build a web service using Go. The standard library and the `net/http` package in particular are super powerful and include all the functionality which one would expect to build rich web applications with ease. I also admire how there was no unnecessary boilerplate to begin with. The web project feels very light-weight and close to the metal. There is no layers of bloatware, no hard requirement or pull towards a certain paradigm and no unnecessary additional packages for simple use cases like mine. At the moment the entire web application is essentially a single `main.go` file and less than a handful of HTML pages. However, it doesn't mean that the project is unstructured or "dirty" in any way. It's production ready code as far as I am concerned. It was very easy for me to expand on structure as the application grew, keeping things easy to understand and easy to maintain. Every single line of code has a purpose and adds functionality which makes sense.

If the application continued to grow then I could apply the same principles as I already did before and further break down the `handleGET` and `handlePOST` methods into smaller parts. I would continue to keep the main routing logic inside the `ServeHTTP` method of the `Server` struct, which is the main entry point of the (web) app.

I really appreciate the simplicity of Go and how it allowed me to add **just enough complexity** when I built out the app. For me Go enables "just-in-time-complexity" coding which feels very refreshing coming from .NET. I can start a project without any at all and only add more as and when I need.