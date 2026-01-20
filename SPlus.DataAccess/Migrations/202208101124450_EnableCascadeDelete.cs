namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableCascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Workflow", "BaseWorkflowID", "dbo.BaseWorkflows");
            DropForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow");
            DropForeignKey("dbo.KPI", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.KPIComments", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPIMeasures", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure");
            DropForeignKey("dbo.KPI", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme");
            DropForeignKey("dbo.UsersGroups", "UserName", "dbo.Users");
            DropForeignKey("dbo.UsersGroups", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.KPIThreshold", "Code", "dbo.Status");
            DropForeignKey("dbo.SystemPerformanceThreshold", "Status_Code", "dbo.Status");
            DropForeignKey("dbo.NotificationParameters", "TemplateID", "dbo.NotificationConfiguration");
            DropForeignKey("dbo.NotificationReceivers", "TemplateID", "dbo.NotificationConfiguration");
            AddForeignKey("dbo.Workflow", "BaseWorkflowID", "dbo.BaseWorkflows", "BaseWorkflowID", cascadeDelete: true);
            AddForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType", "KPITypeID", cascadeDelete: true);
            AddForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow", "WorkflowID", cascadeDelete: true);
            AddForeignKey("dbo.KPI", "KPITypeID", "dbo.KPIType", "KPITypeID", cascadeDelete: true);
            AddForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType", "KPITypeID", cascadeDelete: true);
            AddForeignKey("dbo.KPIComments", "KPIID", "dbo.KPI", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPIMeasures", "KPIID", "dbo.KPI", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPI", "StrategicObjectiveID", "dbo.StrategicObjective", "ID", cascadeDelete: true);
            AddForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme", "ID", cascadeDelete: true);
            AddForeignKey("dbo.UsersGroups", "UserName", "dbo.Users", "UserName", cascadeDelete: true);
            AddForeignKey("dbo.UsersGroups", "GroupID", "dbo.Groups", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPIThreshold", "Code", "dbo.Status", "Code", cascadeDelete: true);
            AddForeignKey("dbo.SystemPerformanceThreshold", "Status_Code", "dbo.Status", "Code", cascadeDelete: true);
            AddForeignKey("dbo.NotificationParameters", "TemplateID", "dbo.NotificationConfiguration", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NotificationReceivers", "TemplateID", "dbo.NotificationConfiguration", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationReceivers", "TemplateID", "dbo.NotificationConfiguration");
            DropForeignKey("dbo.NotificationParameters", "TemplateID", "dbo.NotificationConfiguration");
            DropForeignKey("dbo.SystemPerformanceThreshold", "Status_Code", "dbo.Status");
            DropForeignKey("dbo.KPIThreshold", "Code", "dbo.Status");
            DropForeignKey("dbo.UsersGroups", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.UsersGroups", "UserName", "dbo.Users");
            DropForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme");
            DropForeignKey("dbo.KPI", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure");
            DropForeignKey("dbo.KPIMeasures", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPIComments", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.KPI", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow");
            DropForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.Workflow", "BaseWorkflowID", "dbo.BaseWorkflows");
            AddForeignKey("dbo.NotificationReceivers", "TemplateID", "dbo.NotificationConfiguration", "ID");
            AddForeignKey("dbo.NotificationParameters", "TemplateID", "dbo.NotificationConfiguration", "ID");
            AddForeignKey("dbo.SystemPerformanceThreshold", "Status_Code", "dbo.Status", "Code");
            AddForeignKey("dbo.KPIThreshold", "Code", "dbo.Status", "Code");
            AddForeignKey("dbo.UsersGroups", "GroupID", "dbo.Groups", "ID");
            AddForeignKey("dbo.UsersGroups", "UserName", "dbo.Users", "UserName");
            AddForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme", "ID");
            AddForeignKey("dbo.KPI", "StrategicObjectiveID", "dbo.StrategicObjective", "ID");
            AddForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure", "ID");
            AddForeignKey("dbo.KPIMeasures", "KPIID", "dbo.KPI", "ID");
            AddForeignKey("dbo.KPIComments", "KPIID", "dbo.KPI", "ID");
            AddForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType", "KPITypeID");
            AddForeignKey("dbo.KPI", "KPITypeID", "dbo.KPIType", "KPITypeID");
            AddForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow", "WorkflowID");
            AddForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType", "KPITypeID");
            AddForeignKey("dbo.Workflow", "BaseWorkflowID", "dbo.BaseWorkflows", "BaseWorkflowID");
        }
    }
}
