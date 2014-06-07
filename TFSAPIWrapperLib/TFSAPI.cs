using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Client;

//C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\ReferenceAssemblies\v2.0\
/*
  
1   "Microsoft.TeamFoundation.Client.dll" 
2   "Microsoft.TeamFoundation.Common.dll" 
3   "Microsoft.TeamFoundation.WorkItemTracking.Common.dll" 
4   "Microsoft.TeamFoundation.WorkItemTracking.Client.dll" 
    
*/

namespace TFSAPIWrapperLib
{
    /// <summary>
    /// TFS API Library Wrapper for basic operations on Work Items
    /// </summary>
    public class TFSAPI
    {
        WorkItemStore wiStore;
        TfsTeamProjectCollection teamProjectCollection;
        WorkItemTypeCollection workItemTypeCollection;

       /// <summary>
       /// Create a new instance of TFSAPI Class
       /// </summary>
       /// <param name="TfsCollectionURI">URI to access the TFS Server and project collection</param>
       /// <param name="ProjectName">Default project</param>
        public TFSAPI(Uri TfsCollectionURI, string ProjectName)
        {
            
            teamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TfsCollectionURI);
            teamProjectCollection.Connect(Microsoft.TeamFoundation.Framework.Common.ConnectOptions.IncludeServices);
            teamProjectCollection.EnsureAuthenticated();
            
            wiStore = new WorkItemStore(teamProjectCollection);


            workItemTypeCollection = wiStore.Projects[ProjectName].WorkItemTypes;
        }

        /// <summary>
        /// Query for a specific item
        /// </summary>
        /// <param name="id">ID of the item</param>
        /// <returns></returns>
        public WorkItem GetWorkItem(int id)
        {           
            return wiStore.GetWorkItem(id);
        }        

        /// <summary>
        /// Creates a new work item of a defined type
        /// </summary>
        /// <param name="workItemType">The type name</param>
        /// <param name="title">Default title</param>
        /// <param name="description">Default description</param>
        /// <param name="fieldsAndValues">List of extra propierties and values</param>
        /// <returns></returns>
        public WorkItem CreateWorkItem(string workItemType, string title, string description, Dictionary<string, object> fieldsAndValues)
        {
            WorkItemType wiType = workItemTypeCollection[workItemType];
            WorkItem wi = new WorkItem(wiType) { Title = title, Description = description };

            foreach (KeyValuePair<string, object> fieldAndValue in fieldsAndValues)
            {
                string fieldName = fieldAndValue.Key;
                object value = fieldAndValue.Value;

                if (wi.Fields.Contains(fieldName))
                    wi.Fields[fieldName].Value = value;
                else
                    throw new ApplicationException(string.Format("Field not found {0} in workItemType {1}, failed to save the item", fieldName, workItemType));
            }

            if (wi.IsValid())
            {
                wi.Save();                
            }
            else
            {
                ArrayList validationErrors = wi.Validate();
                string errMessage = "Work item cannot be saved...";
                foreach (Field field in validationErrors)
                    errMessage += "Field: " + field.Name + " has status: " + field.Status + "/n";

                throw new ApplicationException(errMessage);
            }

            return wi;
        }

        /// <summary>
        /// Delete an item by the ID
        /// </summary>
        /// <param name="id">ID of the item to be deleted</param>
        /// <returns></returns>
        public bool DeleteWorkItem(int id)
        {
            List<WorkItemOperationError> errors = DeleteWorkItems(new[] { id });
            return errors.Count == 0;
        }

        /// <summary>
        /// Delete a list of items
        /// </summary>
        /// <param name="ids">List of ID's to be deleted</param>
        /// <returns></returns>
        public List<WorkItemOperationError> DeleteWorkItems(int[] ids)
        {
            return new List<WorkItemOperationError>(wiStore.DestroyWorkItems(ids));
        }

        /// <summary>
        /// Update a work item with new property values
        /// </summary>
        /// <param name="id">ID to be updated</param>
        /// <param name="changes">List of properties to be updated</param>
        public void UpdateWorkItem(int id, Dictionary<string, object> changes)
        {
            WorkItem wi = GetWorkItem(id);
            if (wi == null)
                throw new ApplicationException(string.Format("No item found with ID {0} to be updated", id));

            wi.Open();

            foreach (KeyValuePair<string, object> change in changes)
            {
                string fieldName = change.Key.Replace('_', ' ');
                string fieldName2 = change.Key.Replace("_", "");

                object value = change.Value;

                if (wi.Fields.Contains(fieldName))
                    wi.Fields[fieldName].Value = value;

                else if (wi.Fields.Contains(fieldName2))
                    wi.Fields[fieldName2].Value = value;
            }

            if (wi.IsValid())
                wi.Save();
            else
            {
                ArrayList validationErrors = wi.Validate();
                string errMessage = "Work item cannot be saved...";
                foreach (Field field in validationErrors)
                    errMessage += "Field " + field.Name + " has status " + field.Status + "/n";

                throw new ApplicationException(errMessage);
            }
        }

        /// <summary>
        /// Run a WIQL and retorns a collection of items
        /// </summary>
        /// <param name="wiql">WIQL sentence to be executed</param>
        /// <returns></returns>
        public WorkItemCollection Query(string wiql)
        {
            return this.wiStore.Query(wiql);
        }

    }
}


