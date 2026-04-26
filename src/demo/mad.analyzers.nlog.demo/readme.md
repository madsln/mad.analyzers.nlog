# Readme

# Template Test Lists

## MAD2017

## Tests per Method

```cs
var logger = new Logger();

var intArg = 42;
var boolArg = true;
var stringArg = "string";
var doubleArg = 3.14;
var objectArg = new object();

// no placeholder with 2 args
logger.Info("Message without placeholder", intArg, boolArg);
// no placeholder with 5 params
logger.Info("Message without placeholder", intArg, boolArg, stringArg, doubleArg, objectArg);
// 1 placeholder without args
logger.Info("Message with {Placeholder1}");
// 1 placeholder with 2 args
logger.Info("Message with {Placeholder1}", intArg, boolArg);
// 1 placeholder with 5 params
logger.Info("Message with {Placeholder1}", intArg, boolArg, stringArg, doubleArg, objectArg);
// 2 placeholder2 with 3 args
logger.Info("Message with {Placeholder1}{Placeholder2}", intArg, boolArg, stringArg);
// 3 placeholders with 2 args
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg);
// 3 placeholders with 4 args
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg, stringArg, doubleArg);
// 3 placeholders with 5 params
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg, stringArg, doubleArg, objectArg);
// 6 placeholders with no args
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}");
// 6 placeholders with 3 args
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg);
// 6 placeholders with 5 params
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg, doubleArg, objectArg);
// 6 placeholders with 7 params
logger.Info("Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg, doubleArg, objectArg, 420, 69);
// exception with no placeholder with 2 args
logger.Info(exception, "Message without placeholder", intArg, boolArg);
// exception with no placeholder with 5 params
logger.Info(exception, "Message without placeholder", intArg, boolArg, stringArg, doubleArg, objectArg);
// exception with 1 placeholder without args
logger.Info(exception, "Message with {Placeholder1}");
// exception with 1 placeholder with 2 args
logger.Info(exception, "Message with {Placeholder1}", intArg, boolArg);
// exception with 1 placeholder with 5 params
logger.Info(exception, "Message with {Placeholder1}", intArg, boolArg, stringArg, doubleArg, objectArg);
// exception with 2 placeholder2 with 3 args
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}", intArg, boolArg, stringArg);
// exception with 3 placeholders with 2 args
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg);
// exception with 3 placeholders with 4 args
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg, stringArg, doubleArg);
// exception with 3 placeholders with 5 params
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}", intArg, boolArg, stringArg, doubleArg, objectArg);
// exception with 6 placeholders with no args
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}");
// exception with 6 placeholders with 3 args
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg);
// exception with 6 placeholders with 5 params
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg, doubleArg, objectArg);
// exception with 6 placeholders with 7 params
logger.Info(exception, "Message with {Placeholder1}{Placeholder2}{Placeholder3}{Placeholder4}{Placeholder5}{Placeholder6}", intArg, boolArg, stringArg, doubleArg, objectArg, 420, 69);

// now do the same thing again and pass enumerables directly
// typed enumerables should be recognized as a single arg
// whereas object enumerables should be recognized as params
var typedArrayOf1 = new int[] { 1 };
var typedArrayOf2 = new int[] { 1, 2 };
var typedArrayOf3 = new int[] { 1, 2, 3 };
var typedArrayOf4 = new int[] { 1, 2, 3, 4 };
var typedArrayOf5 = new int[] { 1, 2, 3, 4, 5 };

var objectArrayOf1 = new object[] { 1 };
var objectArrayOf2 = new object[] { 1, 2 };
var objectArrayOf3 = new object[] { 1, 2, 3 };
var objectArrayOf4 = new object[] { 1, 2, 3, 4 };
var objectArrayOf5 = new object[] { 1, 2, 3, 4, 5 };

var typedSpanOf1 = new ReadOnlySpan<int>() { 1 };
var typedSpanOf2 = new ReadOnlySpan<int>() { 1, 2 };
var typedSpanOf3 = new ReadOnlySpan<int>() { 1, 2, 3 };
var typedSpanOf4 = new ReadOnlySpan<int>() { 1, 2, 3, 4 };
var typedSpanOf5 = new ReadOnlySpan<int>() { 1, 2, 3, 4, 5 };

var objectSpanOf1 = new ReadOnlySpan<object>() { 1 };
var objectSpanOf2 = new ReadOnlySpan<object>() { 1, 2 };
var objectSpanOf3 = new ReadOnlySpan<object>() { 1, 2, 3 };
var objectSpanOf4 = new ReadOnlySpan<object>() { 1, 2, 3, 4 };
var objectSpanOf5 = new ReadOnlySpan<object>() { 1, 2, 3, 4, 5 };

```

- logger.Info(msg)
- logger.Info(msg, arg)
- logger.Info(msg, arg1, arg2)
- logger.Info(msg, arg1, arg2, arg3)
- logger.Info(msg, arg1, arg2, arg3, arg4)
- logger.Info(msg, arg1, arg2, arg3, arg4, arg5)
- logger.Info(msg, arg1, arg2, arg3, arg4, arg5, params object)
- logger.Info(msg, arg1, arg2, arg3, arg4, arg5, params span)
- logger.Info(exception, msg)
- logger.Info(exception, msg, arg)
- logger.Info(exception, msg, arg1, arg2)
- logger.Info(exception, msg, arg1, arg2, arg3)
- logger.Info(exception, msg, arg1, arg2, arg3, arg4)
- logger.Info(exception, msg, arg1, arg2, arg3, arg4, arg5)
- logger.Info(exception, msg, arg1, arg2, arg3, arg4, arg5, params object)
- logger.Info(exception, msg, arg1, arg2, arg3, arg4, arg5, params span)

- ilogger.Info(msg)
- ilogger.Info(msg, arg)
- ilogger.Info(msg, arg1, arg2)
- ilogger.Info(msg, arg1, arg2, arg3)
- ilogger.Info(msg, arg1, arg2, arg3, arg4)
- ilogger.Info(msg, arg1, arg2, arg3, arg4, arg5)
- ilogger.Info(msg, arg1, arg2, arg3, arg4, arg5, params object)
- ilogger.Info(msg, arg1, arg2, arg3, arg4, arg5, params span)
- ilogger.Info(exception, msg)
- ilogger.Info(exception, msg, arg)
- ilogger.Info(exception, msg, arg1, arg2)
- ilogger.Info(exception, msg, arg1, arg2, arg3)
- ilogger.Info(exception, msg, arg1, arg2, arg3, arg4)
- ilogger.Info(exception, msg, arg1, arg2, arg3, arg4, arg5)
- ilogger.Info(exception, msg, arg1, arg2, arg3, arg4, arg5, params object)
- ilogger.Info(exception, msg, arg1, arg2, arg3, arg4, arg5, params span)

- loggerext.Info(CultureInfo, msg)
- loggerext.Info(CultureInfo, msg, arg)
- loggerext.Info(CultureInfo, msg, arg1, arg2)
- loggerext.Info(CultureInfo, msg, arg1, arg2, arg3)
- loggerext.Info(CultureInfo, msg, arg1, arg2, arg3, arg4)
- loggerext.Info(CultureInfo, msg, arg1, arg2, arg3, arg4, arg5)
- loggerext.Info(CultureInfo, msg, arg1, arg2, arg3, arg4, arg5, params object)
- loggerext.Info(CultureInfo, msg, arg1, arg2, arg3, arg4, arg5, params span)
- loggerext.Info(exception, CultureInfo, msg)
- loggerext.Info(exception, CultureInfo, msg, arg)
- loggerext.Info(exception, CultureInfo, msg, arg1, arg2)
- loggerext.Info(exception, CultureInfo, msg, arg1, arg2, arg3)
- loggerext.Info(exception, CultureInfo, msg, arg1, arg2, arg3, arg4)
- loggerext.Info(exception, CultureInfo, msg, arg1, arg2, arg3, arg4, arg5)
- loggerext.Info(exception, CultureInfo, msg, arg1, arg2, arg3, arg4, arg5, params object)
- loggerext.Info(exception, CultureInfo, msg, arg1, arg2, arg3, arg4, arg5, params span)