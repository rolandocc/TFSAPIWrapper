using System;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.Client; //add a reference to Microsoft.TeamFoundation.WorkItemTracking.Client.dll

namespace TFSAPIWrapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string tfsURI = "https://rcapps.visualstudio.com/DefaultCollection";
            string tfsProjectName = "STUFFS";

            TFSAPIWrapperLib.TFSAPI tfsApi = new TFSAPIWrapperLib.TFSAPI(new Uri(tfsURI), tfsProjectName);

            Dictionary<string, object> itemValues = new Dictionary<string, object>();
            itemValues.Add("Repro Steps", "Step 1: Do Login");

            var newItem = tfsApi.CreateWorkItem("BUG", "New bug for product X", "New bug detected at...", itemValues);
            Console.WriteLine("Created new item with ID {0}", newItem.Id);

            var itemJustCreated = tfsApi.GetWorkItem(newItem.Id);

            Console.WriteLine("Item created by {0} on {1} ", itemJustCreated.CreatedBy, itemJustCreated.CreatedDate);

            var wasDeleted = tfsApi.DeleteWorkItem(itemJustCreated.Id);

            Console.WriteLine("Item {0} was deleted?  {1}", itemJustCreated.Id, wasDeleted);

            string wiql = "Select [ID]" +
                       " From WorkItems " +
                       " WHERE [Work Item Type] = 'Product Backlog Item'";

            foreach (WorkItem item in tfsApi.Query(wiql))
            {
                Console.WriteLine("Item ID: {0} with title: {1}", item.Id, item.Title);
            }
        }
    }
}
