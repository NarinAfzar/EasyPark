Engineering guidelines
======================

* [Source code management](#source-code-management)
* [Coding guidelines](#coding-guidelines)
* [Tips and tricks](#tips-and-tricks)

## Source code management

The *structure* of the code that we write and the *tools* that we use to write the code.

### Branch strategy

In general:

* `master` has the code for the latest release on NuGet.org (e.g. alpha, beta, RC, RTM)
* `dev` has the code that is being worked on but not yet released. This is the branch into which devs normally submit pull requests and merge changes into.
* `release` has the code that is being staged and stabilized for an upcoming release

Shortly before a release, the `release` branches are created from `dev`, and stabilization work happens there. Post-release work continus in the `dev` branch. Once the release takes place, the code is pushed from `release` to `master`.


### Solution and project folder structure and naming

Solution files go in the repo root.

Solution names match repo names (e.g. Mvc.sln in the Mvc repo).

Solutions need to contain solution folders that match the physical folders (`src`, `test`, etc.).

For example, in the `Fruit` repo with the `Banana` and `Lychee` projects you would have these files checked in:

```
/Fruit.sln
/src
/src/Microsoft.AspNet.Banana
/src/Microsoft.AspNet.Banana/project.json
/src/Microsoft.AspNet.Banana/Banana.kproj
/src/Microsoft.AspNet.Banana/Banana.cs
/src/Microsoft.AspNet.Banana/Util/BananaUtil.cs
/src/Microsoft.AspNet.Lychee
/src/Microsoft.AspNet.Lychee/project.json
/src/Microsoft.AspNet.Lychee/Lychee.kproj
/src/Microsoft.AspNet.Lychee/Lychee.cs
/src/Microsoft.AspNet.Lychee/Util/LycheeUtil.cs
/test
/test/Microsoft.AspNet.Banana.Tests
/test/Microsoft.AspNet.Banana.Tests/project.json
/test/Microsoft.AspNet.Banana.Tests/BananaTest.kproj
/test/Microsoft.AspNet.Banana.Tests/BananaTest.cs
/test/Microsoft.AspNet.Banana.Tests/Util/BananaUtilTest.cs
```

All these files are set to be ignored in the `.gitignore` file.

## Coding guidelines

The *content* of the code that we write.


### Coding style guidelines – general

The most general guideline is that we use all the VS default settings in terms of code formatting, except that we put `System` namespaces before other namespaces (this used to be the default in VS, but it changed in a more recent version of VS).

1. Use four spaces of indentation (no tabs)
2. Use `_camelCase` for private members
3. Avoid `this.` unless absolutely necessary
4. Always specify member visiblity, even if it's the default (i.e. `private string _foo;` not `string _foo;`)


### Usage of the var keyword

The `var` keyword is to be used as much as the compiler will allow. For example, these are correct:

```c#
var fruit = "Lychee";
var fruits = new List<Fruit>();
var flavor = fruit.GetFlavor();
string fruit = null; // can't use "var" because the type isn't known (though you could do (string)null, don't!)
const string expectedName = "name"; // can't use "var" with const
```

The following are incorrect:

```c#
string fruit = "Lychee";
List<Fruit> fruits = new List<Fruit>();
FruitFlavor flavor = fruit.GetFlavor();
```


### Use C# type keywords in favor of .NET type names

When using a type that has a C# keyword the keyword is used in favor of the .NET type name. For example, these are correct:

```c#
public string TrimString(string s) {
    return string.IsNullOrEmpty(s)
        ? null
        : s.Trim();
}

var intTypeName = nameof(Int32); // can't use C# type keywords with nameof
```

The following are incorrect:

```c#
public String TrimString(String s) {
    return String.IsNullOrEmpty(s)
        ? null
        : s.Trim();
}
```

### Cross-platform coding

Our frameworks should work on CoreCLR, which supports multiple operating systems. Don't assume we only run (and develop) on Windows. Code should be sensitive to the differences between OS's. Here are some specifics to consider.

#### Line breaks
Windows uses `\r\n`, OS X and Linux uses `\n`. When it is important, use `Environment.NewLine` instead of hard-coding the line break.

Note: this may not always be possible or necessary. 

Be aware that these line-endings may cause problems in code when using `@""` text blocks with line breaks.

#### Environment Variables

OS's use different variable names to represent similar settings. Code should consider these differences.

For example, when looking for the user's home directory, on Windows the variable is `USERPROFILE` but on most Linux systems it is `HOME`.

```c#
var homeDir = Environment.GetEnvironmentVariable("USERPROFILE") 
                  ?? Environment.GetEnvironmentVariable("HOME");
```

#### File path separators

Windows uses `\` and OS X and Linux use `/` to separate directories. Instead of hard-coding either type of slash, use [`Path.Combine()`](https://msdn.microsoft.com/en-us/library/system.io.path.combine(v=vs.110).aspx) or [`Path.DirectorySeparatorChar`](https://msdn.microsoft.com/en-us/library/system.io.path.directoryseparatorchar(v=vs.110).aspx).

If this is not possible (such as in scripting), use a forward slash. Windows is more forgiving than Linux in this regard.

### When to use internals vs. public and when to use InternalsVisibleTo

As a modern set of frameworks, usage of internal types and members is allowed, but discouraged.

`InternalsVisibleTo` is used only to allow a unit test to test internal types and members of its runtime assembly. We do not use `InternalsVisibleTo` between two runtime assemblies.

If two runtime assemblies need to share common helpers then we will use a "shared source" solution with build-time only packages.

If two runtime assemblies need to call each other's APIs, the APIs must be public. If we need it, it is likely that our customers need it.


### Async method patterns

By default all async methods must have the `Async` suffix. There are some exceptional circumstances where a method name from a previous framework will be grandfathered in.

Passing cancellation tokens is done with an optional parameter with a value of `default(CancellationToken)`, which is equivalent to `CancellationToken.None` (one of the few places that we use optional parameters). The main exception to this is in web scenarios where there is already an `HttpContext` being passed around, in which case the context has its own cancellation token that can be used when needed.

Sample async method:

```c#
public Task GetDataAsync(
    QueryParams query,
    int maxData,
    CancellationToken cancellationToken = default(CancellationToken))
{
    ...
}
```


### Extension method patterns

The general rule is: if a regular static method would suffice, avoid extension methods.

Extension methods are often useful to create chainable method calls, for example, when constructing complex objects, or creating queries.

Internal extension methods are allowed, but bear in mind the previous guideline: ask yourself if an extension method is truly the most appropriate pattern.

The namespace of the extension method class should generally be the namespace that represents the functionality of the extension method, as opposed to the namespace of the target type. One common exception to this is that the namespace for middleware extension methods is normally always the same is the namespace of `IAppBuilder`.

The class name of an extension method container (also known as a "sponsor type") should generally follow the pattern of `<Feature>Extensions`, `<Target><Feature>Extensions`, or `<Feature><Target>Extensions`. For example:

```c#
namespace Food {
    class Fruit { ... }
}
namespace Fruit.Eating {
    class FruitExtensions { public static void Eat(this Fruit fruit); }
  OR
    class FruitEatingExtensions { public static void Eat(this Fruit fruit); }
  OR
    class EatingFruitExtensions { public static void Eat(this Fruit fruit); }
}
```

When writing extension methods for an interface the sponsor type name must not start with an `I`.


### Doc comments

The person writing the code will write the doc comments. Public APIs only. No need for doc comments on non-public types.

Note: Public means callable by a customer, so it includes protected APIs. However, some public APIs might still be "for internal use only" but need to be public for technical reasons. We will still have doc comments for these APIs but they will be documented as appropriate.


### Assertions

Use `Debug.Assert()` to assert a condition in the code. Do not use Code Contracts (e.g. `Contract.Assert`).

Please note that assertions are only for our own internal debugging purposes. They do not end up in the released code, so to alert a developer of a condition use an exception.

## Tips and tricks

:grey_exclamation: The *structure* of the code that we write and the *tools* that we use to write the code.


### I've broken my build and I can't get up!

The build system is brand new, so problems can catch us by surprise. As such, you'll sometimes end up in a broken state and can't build. The following steps should fix most broken builds:

```
git clean -xdf (clean all non-source controlled files)
build (this will run the build and will pull in NuGet packages, etc.)
```
