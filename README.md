# DebugXPath

DebugXPath is a little tool that allow user to test XPath statement on a xml file.

## Prerequisites

Before you begin, ensure you have met the following requirements:
 * You have a Windows machine, only Windows is supported
 * You have .Net Framework 4.6.2 or above on your machine

## Installing DebugXPath

To install DebugXPath, download a zip file from the latest release.
Unpack the file in a directory.

## Using DebugXPath

You can launch the application from several ways:
 * From the command line: `DebugXPath`, maybe add the application path to your `PATH`
 * From the command line with a file path: `DebugXPath <file path>`
 * Click on the application executable

### File mode
When launched from the command line without parameter or when clicking on the executable, you enter the "file mode" (prompt is `File path > `).
In "file mode" you can only enter a file path or the `exit` keyword to quit the application.

### XPath mode
When the provided path exists (and when launched with a parameter), you enter "xpath mode" (prompt is `XPath > `).
Available commands are:
 * `exit` keyword to quit "xpath mode"
 * `/nodes` command to display child nodes of the current work node
 * `/select <xpath>` command to select a node. The xpath parameter should target only one node
 * `<xpath>` to display nodes corresponding to the provided xpath

### Selection mode
When using `/select <xpath>` command and when only one node is targeted, you enter in "selection mode" (prompt is `XPath (selection) > `).
You can use the same commands as the "xpath mode".

### Custom prefix for namespaces
After the loading of a xml document, the application will try to display the default namespace and the corresponding prefix.
When no prefix is found, a message will be displayed.

You should add that namespace without prefix in the `Namespaces.txt` file near to then executable.
The format used in the file is `<prefix><semi-colon><namespace uri>`. For example: `env;http://schemas.xmlsoap.org/soap/envelope/`.
The namespaces file is only loaded at application launch.