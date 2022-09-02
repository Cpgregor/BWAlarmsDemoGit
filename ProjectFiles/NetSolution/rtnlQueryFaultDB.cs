#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.UI;
using FTOptix.EventLogger;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.EthernetIP;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.NetLogic;
using FTOptix.Core;
#endregion

public class rtnlQueryFaultDB : BaseNetLogic
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
    public void GetFaultFields()
    {
        // Get the current project
        var myProject = Project.Current;

        // Get the fault number
        var faultNumber = myProject.GetVariable("Model/Faults/varSelected").Value;

        if (faultNumber == 0)
        {
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblMessage").Text = "Fault number " + faultNumber + " is not defined";
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblCause").Text = "";
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblRemedy").Text = "";
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblSparePart").Text = "";
        }

        // Get the database
        var faultStore = myProject.Get<Store>("DataStores/dbFaults");

        try
        {
            // Query the database
            object[,] resultSet;
            string[] header;
            faultStore.Query("SELECT * FROM Faults WHERE No = " + faultNumber, out header, out resultSet);

            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblMessage").Text = resultSet[0, 1].ToString();
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblCause").Text = resultSet[0, 2].ToString();
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblRemedy").Text = resultSet[0, 3].ToString();
            if (resultSet[0, 4].ToString() == "")
            {
                myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblSparePart").Text = "N/A";
            }
            else
                myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblSparePart").Text = resultSet[0, 4].ToString();
        }
        catch (Exception ex)
        {
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblMessage").Text = "Fault number " + faultNumber + " is not defined";
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblCause").Text = "";
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblRemedy").Text = "";
            myProject.Get<Label>("UI/Panels/Reporting/FaultLookup/lblSparePart").Text = "";
        }
    }
}

