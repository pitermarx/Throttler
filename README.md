### [_Throttler_](http://pitermarx.github.io/Throttler)

_A simple Throttle class that can throttle a method with the signature Action&lt;List&lt;T&gt;&gt;_

#### Project Setup

_Includes a solution with a test project and the Throttle class_ 

#### License

The content of this project itself is licensed under the
[Creative Commons Attribution 3.0 license](http://creativecommons.org/licenses/by/3.0/us/deed.en_US),
and the underlying source code used to format and display that content
is licensed under the [MIT license](http://opensource.org/licenses/mit-license.php).

#### Example
```cs
Action<List<int>> actionToBeThrottled = () => /* Do awesome stuff */;
var maximumElementsBeforeFlush = 100;
var automaticFlushTimeout = TimeSpan.FromMinutes(5);
var throttledAction = new Throttler<int>(actionToBeThrottled, maximumElementsBeforeFlush, automaticFlushTimeout);

// now, call the throttledAction
throttledAction.Call(new int());
throttledAction.Call(new List<int>(5));

// you can flush manually to...
throttledAction.Flush();
```

[![Build status](https://ci.appveyor.com/api/projects/status/ue5h2bp6sgtm23bx)](https://ci.appveyor.com/project/pitermarx/throttler)
