using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SPlus.Helper;
using System.Configuration;

namespace SPlus.Model
{

    public enum EnumWFStatuses
    {   
        New = 0,
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Completed = 4,
        NA = 5,
        ApprovedWithDocument = 6,
        Return = 7,


        //for history

        SaveAsDraft = 8,
        Submitted = 9,
        Closed=10
    }

    public enum EnumWFForms
    {
        UpdateKPIVital = 1,
        UpdateKPIBoard = 2,
        UPdateKPIStrategic = 3,
    }

    public enum EnumWFBaseWorkflows
    {
        Update = 1,
        KPIChangeRequest = 2,
        MitigationAction=6,
        CreateKPI=7,
        StrategicKPI = 8,
        DepartmentalKPI = 9,
        ReUpdate = 10,
        AutoApprove =11,    
    }
    public enum EnumRequestAction
    {
        Submit = 1,
        Save = 2,
    }

    public enum EnumKPIChangeType
    {
        KPIInfo = 1,
        KPITarget = 2,
        Both = 3,

    }

   // public enum EnumWorkflows
  //  {
       

        //StrategyKPIChangeRequest_Info = 7,//4,
        //DivisionalKPIChangeRequest_Info = 8,//5,
        //StrategyKPIChangeRequest_Target=9,//6,
        //    DivisionalKPIChangeRequest_Target=10,//7,
        //DivisionalDeleteKPI=11//8,
    
    //}

    //public enum EnumActivityWorkflows
    //{
        
    //    StrategyActivityChangeRequest_Info =5, //2,
    //    DivisionalActivityChangeRequest_Info =6, //3,
    //    StrategyActivityChangeRequest_Target = 7,//4,
    //    DivisionalActivityChangeRequest_Target =8,// 5,
    //    DivisionalDeleteActivity = 9//6
    //}
    public enum TaskCentreWFTypeEnum
    {
        KPI,
        All,
        WorkProcedure,
        MitigationActions,
        ReUpdate = 8
    }

}
