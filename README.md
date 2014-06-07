TFSAPIWrapper
=============
Simple to use TFS API Wrapper Class written in c# (VS 2013) for basic query and update operations against a TFS Server.
##Dependencies
You need the following libraries installed by default with Visual Studio on:
C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\ReferenceAssemblies\v2.0\

- Microsoft.TeamFoundation.Client.dll
- Microsoft.TeamFoundation.Common.dll
- Microsoft.TeamFoundation.WorkItemTracking.Common.dll
- Microsoft.TeamFoundation.WorkItemTracking.Client.dll

###Create an Object and Open a Connection
Just specify the URI and default project name.
```c#
string tfsURI = "https://{HERE YOUR ACCOUNT}.visualstudio.com/DefaultCollection";
string tfsProjectName = "STUFFS";
TFSAPIWrapperLib.TFSAPI tfsApi = new TFSAPIWrapperLib.TFSAPI(new Uri(tfsURI), tfsProjectName);
```

###Create a BUG Work Item
Create a new BUG Work Item and specify the a value for “Repro Steps” field.
```c#
Dictionary<string, object> itemValues = new Dictionary<string, object>();
            itemValues.Add("Repro Steps", "Step 1: Do Login");

            var newItem = tfsApi.CreateWorkItem("BUG", "New bug for product X", "New bug detected at...", itemValues);
```
###Delete a Work Item
Delete by ID and get a Boolean.
```c#
var wasDeleted = tfsApi.DeleteWorkItem(itemJustCreated.Id);
```
###Get a Work Item by ID
Get a work item by using the ID.
```c#
var item = tfsApi.GetWorkItem(newItem.Id);
```
###Get a Work Item Collection using WIQL
Get a Work Item Collection.
```c#
  string wiql = "Select [ID]" +
                       " From WorkItems " +
                       " WHERE [Work Item Type] = 'Product Backlog Item'";
Var collection = tfsApi.Query(wiql));
```
