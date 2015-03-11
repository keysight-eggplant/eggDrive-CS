
eggDriver for C#
================

eggDriver is a library used to communicate with eggPlant Functional in "drive" mode (called eggDrive). It is intended to be used as-is, but you may modify it as you see fit.


## Sessions ##

You must start an eggDrive session before performing additional actions, and only one session may be active at a time. The `OverridePreviousSession` property (default=`true`) will control whether or not the previous session (if it exists) should be overridden.

```cs
using TestPlant.EggDriver;

// Default eggDrive URL: http://localhost:5400
var eggDriver = new EggDriver();

Console.WriteLine ("Starting first session...");
eggDriver.StartSession ();

try {
    Console.WriteLine ("Starting another session without overriding...");
    eggDriver.OverridePreviousSession = false;
    eggDriver.StartSession ();
} catch (eggDriveException e) {
    Console.WriteLine ("Failed to start another session, as expected");
}

Console.WriteLine ("Starting another session with overriding...");
eggDriver.OverridePreviousSession = true;
eggDriver.StartSession ();

Console.WriteLine ("Ending session...");
eggDriver.EndSession ();
```


## Performing Built-In Actions ##

eggDriver supports a number of built-in actions.

```cs
using TestPlant.EggDriver;
using PropertyList = System.Collections.Generic.Dictionary<string, object>;

// Simple SenseTalk expressions
object EvaluateExpression (object expression);

// Logging
public void Log (string s);
public void LogExpression (object expression);
public void LogError (string s);
public void LogErrorExpression (object expression);


// SUT connections
public void ConnectToSUT (
	string serverID,
	int? portNum = null,
	EggplantConnectionType? type = null,
	string username = null,
	string password = null,
	string sshHost = null,
	string sshUser = null,
	string sshPassword = null,
	bool? visible = null,
	int? colorDepth = null);
public PropertyList ConnectionInfo (string connectionName = null);

// Remote screen dimensions
public Size RemoteScreenSize ();
public Point RemoteScreenCenter ();

// Pointer events
public void Click (Point point);
public void Click (string imageName, double? searchTimeoutSeconds = null);
public void ClickText (string text, PropertyList options = null);
public void DoubleClick (Point point);
public void DoubleClick (string imageName, double? searchTimeoutSeconds = null);
public void DoubleClickText (string text, PropertyList options = null);
public void RightClick (Point point);
public void RightClick (string imageName, double? searchTimeoutSeconds = null);
public void RightClickText (string text, PropertyList options = null);
public void MoveTo (Point point);
public void MoveTo (string imageName, double? searchTimeoutSeconds = null);
public void MoveToText (string text, PropertyList options = null);

// Gestures
public void Tap (Point point);
public void Tap (string imageName, double? searchTimeoutSeconds = null);
public void TapText (string text, PropertyList options = null);
public void SwipeLeft ();
public void SwipeLeft (Point point);
public void SwipeLeft (string imageName);
public void SwipeLeftText (string imageName, PropertyList options = null);
public void SwipeRight ();
public void SwipeRight (Point point);
public void SwipeRight (string imageName);
public void SwipeRightText (string imageName, PropertyList options = null);
public void SwipeDown ();
public void SwipeDown (Point point);
public void SwipeDown (string imageName);
public void SwipeDownText (string imageName, PropertyList options = null);
public void SwipeUp ();
public void SwipeUp (Point point);
public void SwipeUp (string imageName);
public void SwipeUpText (string imageName, PropertyList options = null);
public void PinchOut (
	double? durationSeconds = null, 
	int? distance = null);
public void PinchOut (
	Point atPoint, 
	double? durationSeconds = null,
	int? distance = null);
public void PinchOut (
	string atImageName, 
	double? durationSeconds = null,
	int? distance = null);
public void PinchOut (
	Point atPoint,
	Point toPoint,
	double? durationSeconds = null);
public void PinchOut (
	string atImageName,
	string toImageName,
	double? durationSeconds = null);
public void PinchIn (
	double? durationSeconds = null, 
	int? distance = null);
public void PinchIn (
	Point atPoint, 
	double? durationSeconds = null,
	int? distance = null);
public void PinchIn (
	string atImageName, 
	double? durationSeconds = null,
	int? distance = null);
public void PinchIn (
	Point atPoint,
	Point fromPoint,
	double? durationSeconds = null);
public void PinchIn (
	string atImageName,
	string fromImageName,
	double? durationSeconds = null);

// Image searching
public PropertyList ImageInfo (string imageName);
public PropertyList[] ImageInfo (string[] imageNames);
public void WaitFor (string imageName, double? searchTimeoutSeconds);
public void WaitFor (string[] imageNames, double? searchTimeoutSeconds);
public bool ImageFound (object imageNameOrNames, double? searchTimeoutSeconds = null);
public Point[] EveryImageLocation (string[] imageNames);

// SUT Text
public void TypeText (string s);
public void TypeExpression (object expression);
public string ReadText(Point point, PropertyList options = null);
public string ReadText(Rectangle rectangle, PropertyList options = null);
public string ReadText(string imageName, PropertyList options = null);
public string ReadText(string[] imageNames, PropertyList options = null);
public string RemoteClipboard(double? waitTimeoutSeconds = null);


// Mobile device control
public void LaunchApp (string appName);
public void LaunchApp (string deviceName, string appName);
```


## Performing Custom Actions ##

If one of the built-in actions does not suffice, custom actions may be performed using eggPlant SenseTalk syntax. `EggDriveXmlRpcConnector` provides the basic execution functionality and `EggDriver` is a subclass of `EggDriveXmlRpcConnector`, so either one can be used to perform custom actions.

```cs
var rpcConnector = new EggDriveXmlRpcConnector();
EggDriveResponse response = rpcConnector.Execute ("put \"Hello from Eggplant!!!\"");
Console.WriteLine ("Output: " + response.Output);
```

While SenseTalk statements are strings that can be built manually, `SenseTalkStatementBuilder` can be used to generate statements more abstractly. You can also use its FormatObject static method to format standard objects and containers as well as System.Drawing.Point, System.Drawing.Rectangle, and System.Drawing.Size objects.

```cs
var statementBuilder = new SenseTalkStatementBuilder (
    "SuiteInfo",
    SenseTalkStatementType.Function);

EggDriveResponse response = rpcConnector.Execute (statementBuilder.ToString ());

var propertyList = response.ReturnValueAsPropertyList ();
var senseTalkPropertyList = SenseTalkStatementBuilder.FormatObject (propertyList);
Console.WriteLine ("Return: " + senseTalkPropertyList);
```


## Debugging eggDrive Calls ##

Setting the `DebugEnabled` property to `true` for `EggDriveXmlRpcConnector` (or `EggDriver`) will cause all eggDrive XML-RPC calls to be logged to the Console.

```cs
rpcConnector.DebugEnabled = true;
```
