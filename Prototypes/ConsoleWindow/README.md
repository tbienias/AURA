# Prototype: ConsoleWindow
## Serves as a prototype for showing console output and log massages in the augmented reality.
This protyp gives the feature to show the console output and log messages in the augmented reality by
adding a "billboard" to your scene. The following functionality will be delivered with this prototype:
	- Console prefab
	- Hide/Show functionality (at the moment only by function calls)
	- Debugging color in relation to the message severity
	- Stack tracing

For usage you can either:
	- Make a call to Debug.Log(string msg) which is wired to the console
	- Directly call ConsoleWindow.HandleLog(string msg, string trace, LogType type) 
