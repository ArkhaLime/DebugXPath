# DebugXPath

DebugXPath is a little tool that allow user to test XPath expression on a xml file.

## Prerequisites

Before you begin, ensure you have met the following requirements:
 * You have a Windows machine, only Windows is supported
 * You have .Net Framework 4.6.2 or above on your machine

## Installing DebugXPath

To install DebugXPath, download a zip file from the latest release.
Unpack the file in a directory.

## Using DebugXPath

You can launch the application from several ways:
 * From the command line: `DebugXPath`, maybe add the application's executable path to your `PATH`.
 * From the command line with a file path: `DebugXPath <file path>`.
 * Click on the application executable.

### File mode
When launched from the command line without parameter or when clicking on the executable, you enter "file mode" (prompt is `File path > `).
In "file mode" you can only enter a file path or specific commands.

Available commands:
 * `help` : to show availables commands.
 * `exit` : to exit the application.
 * `/namespaces` or `/ns` : enter in "namespace mode".
 * `<file path>` : enter in "XPath mode".

### Namespace mode
In "namespace mode", you can manage your custom namespaces and their prefixes. Prompt is `Namespaces > `. An asterisk (*) is added when you add/delete a namespace. The asterisk is removed when you save.

Availables commands:
 * `help` : to show availables commands.
 * `exit` : to return to "file mode".
 * `display` : to show all loaded custom namespaces.
 * `add <prefix> <uri>` : add a new custom namespace.
 * `delete <prefix>` : remove a custom namespace.
 * `save` : save all custom namespaces.

### XPath mode
When the provided path exists (or when launched with a parameter), you enter "xpath mode" (prompt is `XPath > `).  

Available commands:
 * `help` : to show availables commands
 * `exit` keyword to quit "xpath mode" or "selection mode".
 * `/nodes` command to display child nodes of the current work node (default is the entire xml document).
 * `/select <xpath>` command to select a node. The xpath parameter should target only one node. That single node will be used as the current work node. Enter "selection mode".
 * `<xpath>` to display nodes corresponding to the provided xpath expression.

### Selection mode
When using `/select <xpath>` command and when only one node is targeted, you enter in "selection mode" (prompt is `XPath (selection) > `).
You can use the same commands as the "xpath mode".

### Custom prefix for namespaces
After the loading of a xml document, the application will try to display the default namespace and the corresponding prefix.
When no prefix is found, a message will be displayed.

You should add that namespace in the `Namespaces.txt` file near the executable. You can also add a prefix when in "namespace mode".
The format used in the file is `<prefix><semi-colon><namespace uri>`. For example: `env;http://schemas.xmlsoap.org/soap/envelope/`.
The namespaces file is only loaded at application launch.