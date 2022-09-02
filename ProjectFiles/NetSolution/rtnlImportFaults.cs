#region Using directives
using System;
using System.IO;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.SQLiteStore;
using FTOptix.CoreBase;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Retentivity;
using FTOptix.Core;
#endregion

public class rtnlImportFaults : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
    [ExportMethod]
    public void ImportFaults()
    {
        // Read the fault information from the csv file and write the records into the database

        // Get the csv file
        var csvPath = GetCSVFilePath();

        if (string.IsNullOrEmpty(csvPath))
        {
            Log.Error("ImportFaults", "No CSV file chosen, please select a file for the FaultDataFile variable");
            return;
        }

        // Check to see that the file exists
        if (!File.Exists(csvPath))
        {
            Log.Error("ImportFaults", "The file " + csvPath + " does not exist.");
            return;
        }

        // Create the iterator
        var i = 0;

        try
        {
            // Get the database
            var faultDb = Project.Current.Get<Store>("DataStores/dbFaults");

            // Get the table
            var tableFaults = faultDb.Tables.Get<Table>("Faults");

            // Create the column name string
            string[] columns = { "No", "Message", "Cause", "Remedy", "SparePart" };

            // Get the lines
            string[] faultLines = System.IO.File.ReadAllLines(csvPath);

            // Create the object to hold the values
            var newValues = new object[faultLines.Length, 5];

            foreach (string faultLine in faultLines)
            {
                string[] columnValue = faultLine.Split(',');

                newValues[i, 0] = columnValue[0].ToString();
                newValues[i, 1] = columnValue[1].ToString();
                newValues[i, 2] = columnValue[2].ToString();
                newValues[i, 3] = columnValue[3].ToString();
                if (columnValue.Length == 5)
                {
                    newValues[i, 4] = columnValue[4].ToString();
                } 
                else
                    newValues[i, 4] = "";

                i++;
            }

            var importLabel = Project.Current.Get<Label>("UI/Panels/Reporting/FaultLookup/lblImport");
            importLabel.Text = "Imported " + i.ToString() + " fault defintions.";

            tableFaults.Insert(columns, newValues);

            Log.Info("ImportFaults", "Faults successfully imported");

        }
        catch (Exception ex)
        {
            Log.Error("ImportFaults", "Unable to import Faults: " + ex.ToString());
        }

    }
    private string GetCSVFilePath()
    {
        var csvPathVariable = LogicObject.GetVariable("FaultCSVFile");
        if (csvPathVariable == null)
        {
            Log.Error("ImportExportAlarms", "CSVPath variable not found");
            return "";
        }

        return new ResourceUri(csvPathVariable.Value).Uri;
    }
}
