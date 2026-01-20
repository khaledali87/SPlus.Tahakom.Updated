namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachements",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AttachementID = c.String(nullable: false, maxLength: 255),
                        AttachementName = c.String(maxLength: 255),
                        AttachementSize = c.String(maxLength: 255),
                        AttachementType = c.String(maxLength: 255),
                        AttachementURL = c.String(maxLength: 255),
                        RelatedItemID = c.String(maxLength: 255),
                        KPIID = c.String(maxLength: 255),
                        AttachementDate = c.String(maxLength: 255),
                        ItemType = c.String(maxLength: 255),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Configurations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Value = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Delegation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FromUser = c.String(maxLength: 255),
                        ToUser = c.String(maxLength: 255),
                        FromDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 255),
                        ModifiedBy = c.String(maxLength: 255),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.EmailConfiguration",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        SmtpServer = c.String(maxLength: 255),
                        SmtpPort = c.String(maxLength: 255),
                        SenderEmail = c.String(maxLength: 255),
                        SenderPassword = c.String(maxLength: 255),
                        EnableSsl = c.Boolean(),
                        UsingSharepointSMTP = c.Boolean(),
                        SiteURL = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.EmailTemplate",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Body = c.String(),
                        Subject = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Exceptions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Message = c.String(),
                        Controller = c.String(maxLength: 255),
                        Function = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                        IsDeletable = c.Boolean(),
                        IsEditable = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UsersGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 255),
                        GroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserName)
                .ForeignKey("dbo.Groups", t => t.GroupID)
                .Index(t => t.UserName)
                .Index(t => t.GroupID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 255),
                        ID = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(nullable: false, maxLength: 255),
                        Department = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 255),
                        UserProfilePicture = c.String(maxLength: 255),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserName);
            
            CreateTable(
                "dbo.KPI",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false, maxLength: 255),
                        ArabicName = c.String(nullable: false, maxLength: 255),
                        ReferenceNo = c.String(nullable: false, maxLength: 255),
                        EnglishDescription = c.String(nullable: false),
                        ArabicDescription = c.String(nullable: false),
                        Formula = c.String(maxLength: 255),
                        Champion = c.String(nullable: false, maxLength: 255),
                        Owner = c.String(nullable: false, maxLength: 255),
                        Sponser = c.String(nullable: false, maxLength: 255),
                        DataSource = c.String(nullable: false, maxLength: 255),
                        KPIType = c.String(maxLength: 255),
                        UnitOfMeasure = c.String(nullable: false, maxLength: 255),
                        Polarity = c.String(nullable: false, maxLength: 255),
                        Frequency = c.String(nullable: false, maxLength: 255),
                        StartDate = c.String(nullable: false, maxLength: 255),
                        Baseline = c.Decimal(nullable: false, precision: 18, scale: 0),
                        BaselineDate = c.DateTime(nullable: false),
                        Years = c.String(nullable: false, maxLength: 255),
                        Direction = c.String(maxLength: 255),
                        EnglishUnitDetails = c.String(maxLength: 255),
                        ArabicUnitDetails = c.String(maxLength: 255),
                        EnglishEquation = c.String(),
                        ArabicEquation = c.String(),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        StrategicObjectiveID = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        UnlockDate = c.DateTime(storeType: "date"),
                        OrgStructureID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OrgStructure", t => t.OrgStructureID)
                .ForeignKey("dbo.StrategicObjective", t => t.StrategicObjectiveID)
                .ForeignKey("dbo.Users", t => t.Champion)
                .ForeignKey("dbo.Users", t => t.Owner)
                .ForeignKey("dbo.Users", t => t.Sponser)
                .Index(t => t.Champion)
                .Index(t => t.Owner)
                .Index(t => t.Sponser)
                .Index(t => t.StrategicObjectiveID)
                .Index(t => t.OrgStructureID);
            
            CreateTable(
                "dbo.KPIComments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KPIID = c.Int(nullable: false),
                        Comment = c.String(nullable: false, maxLength: 255),
                        CreatedBy = c.String(nullable: false, maxLength: 255),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KPI", t => t.KPIID)
                .Index(t => t.KPIID);
            
            CreateTable(
                "dbo.KPIMeasures",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Value = c.Decimal(precision: 18, scale: 0),
                        DueDate = c.DateTime(nullable: false),
                        Status = c.String(nullable: false, maxLength: 255),
                        UpdateDate = c.DateTime(),
                        KPIID = c.Int(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        OutOfTarget = c.Decimal(precision: 18, scale: 0),
                        AllowUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KPI", t => t.KPIID)
                .Index(t => t.KPIID);
            
            CreateTable(
                "dbo.OrgStructure",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        ParentID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StrategicObjective",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false, maxLength: 255),
                        ArabicName = c.String(nullable: false, maxLength: 255),
                        PerspectiveID = c.Int(),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        ThemeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Perspective", t => t.PerspectiveID)
                .ForeignKey("dbo.Theme", t => t.ThemeID)
                .Index(t => t.PerspectiveID)
                .Index(t => t.ThemeID);
            
            CreateTable(
                "dbo.Perspective",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false, maxLength: 255),
                        ArabicName = c.String(nullable: false, maxLength: 255),
                        Icon = c.String(maxLength: 255),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Theme",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(maxLength: 255),
                        ArabicName = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.KPIThreshold",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KPITypeID = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 3),
                        Min = c.Int(),
                        Max = c.Int(),
                        MinOperator = c.String(maxLength: 10),
                        Operator = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KPIType", t => t.KPITypeID)
                .ForeignKey("dbo.Status", t => t.Code)
                .Index(t => t.KPITypeID)
                .Index(t => t.Code);
            
            CreateTable(
                "dbo.KPIType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Icon = c.String(nullable: false),
                        GracePeriod = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Workflows", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Workflows",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false, storeType: "date"),
                        Modified = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WorkflowSteps",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Approver = c.String(nullable: false),
                        WorkflowID = c.Int(nullable: false),
                        IsGroup = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false, storeType: "date"),
                        Modified = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Workflows", t => t.WorkflowID)
                .Index(t => t.WorkflowID);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 3),
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Color = c.String(nullable: false),
                        Order = c.Int(),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.SystemPerformanceThreshold",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Min = c.Int(),
                        Max = c.Int(),
                        MinOperator = c.String(maxLength: 10),
                        MaxOperator = c.String(maxLength: 10),
                        Color = c.String(maxLength: 255),
                        Status_Code = c.String(nullable: false, maxLength: 3),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Status", t => t.Status_Code)
                .Index(t => t.Status_Code);
            
            CreateTable(
                "dbo.Lookup",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LookupValue",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        English = c.String(maxLength: 255),
                        Arabic = c.String(maxLength: 255),
                        Lookup = c.Int(),
                        Order0 = c.String(maxLength: 255),
                        Color = c.String(maxLength: 255),
                        Value = c.String(maxLength: 255),
                        Others = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                        LookupValue2_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.LookupValue", t => t.LookupValue2_ID)
                .Index(t => t.LookupValue2_ID);
            
            CreateTable(
                "dbo.Matrix",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GroupID = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.NotificationConfiguration",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        NotificationEnglish = c.String(),
                        NotificationArabic = c.String(),
                        EmailSubject = c.String(),
                        EmailBody = c.String(),
                        ActionType = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.NotificationParameters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        IsUser = c.Boolean(nullable: false),
                        Value = c.String(nullable: false),
                        TemplateID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NotificationConfiguration", t => t.TemplateID)
                .Index(t => t.TemplateID);
            
            CreateTable(
                "dbo.NotificationReceivers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TemplateID = c.Int(nullable: false),
                        Receiver = c.String(maxLength: 255),
                        IsCC = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NotificationConfiguration", t => t.TemplateID)
                .Index(t => t.TemplateID);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(maxLength: 255),
                        ArabicName = c.String(maxLength: 255),
                        AssignedTo = c.String(maxLength: 255),
                        Status = c.String(maxLength: 255),
                        KPIID = c.Decimal(precision: 18, scale: 0),
                        DelegationID = c.Decimal(precision: 18, scale: 0),
                        KPIName = c.String(maxLength: 255),
                        NotificationType = c.String(maxLength: 255),
                        TaskID = c.Decimal(precision: 18, scale: 0),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ReminderConfiguration",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TypeID = c.Int(nullable: false),
                        BeforeReminder = c.Int(nullable: false),
                        FirstReminder = c.Int(nullable: false),
                        SecondReminder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        IsKPI = c.Boolean(nullable: false),
                        TypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Role = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SYNC_Job_Logging",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        SyncStartAt = c.String(maxLength: 255),
                        SyncEndAt = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WF_ChangeRequest",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RequestID = c.Int(),
                        KPIID = c.Int(),
                        MeasureID = c.Int(),
                        NewTarget = c.Decimal(precision: 18, scale: 2),
                        OldTarget = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WFReminderRegistry",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RelatedItemID = c.Int(nullable: false),
                        Type = c.String(nullable: false),
                        ReminderDate = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WFRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        WorkflowID = c.Int(nullable: false),
                        RelatedItemID = c.Int(nullable: false),
                        RequestedBy = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false, storeType: "date"),
                        Modified = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WFRequestSteps",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RequestID = c.Int(nullable: false),
                        Approver = c.String(nullable: false),
                        WorkflowStepID = c.Int(nullable: false),
                        ActionBy = c.String(),
                        Comments = c.String(),
                        IsGroup = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        WFRequest_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.WFRequests", t => t.WFRequest_ID)
                .Index(t => t.WFRequest_ID);
            
            CreateTable(
                "dbo.WorkflowsSnapshot",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        Created = c.DateTime(nullable: false, storeType: "date"),
                        Modified = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WorkflowStepsSnapshot",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Approver = c.String(nullable: false),
                        WorkflowID = c.Int(nullable: false),
                        IsGroup = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false, storeType: "date"),
                        Modified = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WorkflowTasks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        InstanceID0 = c.String(maxLength: 255),
                        Status = c.String(maxLength: 255),
                        AssignedTo = c.String(maxLength: 255),
                        StepID = c.String(maxLength: 255),
                        EscalateLevel = c.String(maxLength: 255),
                        Comments = c.String(),
                        LastRemiderDate = c.DateTime(),
                        LastEscalationDate = c.DateTime(),
                        TaskCreatedDate = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WFRequestSteps", "WFRequest_ID", "dbo.WFRequests");
            DropForeignKey("dbo.NotificationReceivers", "TemplateID", "dbo.NotificationConfiguration");
            DropForeignKey("dbo.NotificationParameters", "TemplateID", "dbo.NotificationConfiguration");
            DropForeignKey("dbo.LookupValue", "LookupValue2_ID", "dbo.LookupValue");
            DropForeignKey("dbo.SystemPerformanceThreshold", "Status_Code", "dbo.Status");
            DropForeignKey("dbo.KPIThreshold", "Code", "dbo.Status");
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflows");
            DropForeignKey("dbo.KPIType", "ID", "dbo.Workflows");
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.UsersGroups", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.UsersGroups", "UserName", "dbo.Users");
            DropForeignKey("dbo.KPI", "Sponser", "dbo.Users");
            DropForeignKey("dbo.KPI", "Owner", "dbo.Users");
            DropForeignKey("dbo.KPI", "Champion", "dbo.Users");
            DropForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme");
            DropForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective");
            DropForeignKey("dbo.KPI", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure");
            DropForeignKey("dbo.KPIMeasures", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPIComments", "KPIID", "dbo.KPI");
            DropIndex("dbo.WFRequestSteps", new[] { "WFRequest_ID" });
            DropIndex("dbo.NotificationReceivers", new[] { "TemplateID" });
            DropIndex("dbo.NotificationParameters", new[] { "TemplateID" });
            DropIndex("dbo.LookupValue", new[] { "LookupValue2_ID" });
            DropIndex("dbo.SystemPerformanceThreshold", new[] { "Status_Code" });
            DropIndex("dbo.WorkflowSteps", new[] { "WorkflowID" });
            DropIndex("dbo.KPIType", new[] { "ID" });
            DropIndex("dbo.KPIThreshold", new[] { "Code" });
            DropIndex("dbo.KPIThreshold", new[] { "KPITypeID" });
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            DropIndex("dbo.StrategicObjective", new[] { "PerspectiveID" });
            DropIndex("dbo.KPIMeasures", new[] { "KPIID" });
            DropIndex("dbo.KPIComments", new[] { "KPIID" });
            DropIndex("dbo.KPI", new[] { "OrgStructureID" });
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            DropIndex("dbo.KPI", new[] { "Sponser" });
            DropIndex("dbo.KPI", new[] { "Owner" });
            DropIndex("dbo.KPI", new[] { "Champion" });
            DropIndex("dbo.UsersGroups", new[] { "GroupID" });
            DropIndex("dbo.UsersGroups", new[] { "UserName" });
            DropTable("dbo.WorkflowTasks");
            DropTable("dbo.WorkflowStepsSnapshot");
            DropTable("dbo.WorkflowsSnapshot");
            DropTable("dbo.WFRequestSteps");
            DropTable("dbo.WFRequests");
            DropTable("dbo.WFReminderRegistry");
            DropTable("dbo.WF_ChangeRequest");
            DropTable("dbo.SYNC_Job_Logging");
            DropTable("dbo.Roles");
            DropTable("dbo.Resources");
            DropTable("dbo.ReminderConfiguration");
            DropTable("dbo.Notifications");
            DropTable("dbo.NotificationReceivers");
            DropTable("dbo.NotificationParameters");
            DropTable("dbo.NotificationConfiguration");
            DropTable("dbo.Matrix");
            DropTable("dbo.LookupValue");
            DropTable("dbo.Lookup");
            DropTable("dbo.SystemPerformanceThreshold");
            DropTable("dbo.Status");
            DropTable("dbo.WorkflowSteps");
            DropTable("dbo.Workflows");
            DropTable("dbo.KPIType");
            DropTable("dbo.KPIThreshold");
            DropTable("dbo.Theme");
            DropTable("dbo.Perspective");
            DropTable("dbo.StrategicObjective");
            DropTable("dbo.OrgStructure");
            DropTable("dbo.KPIMeasures");
            DropTable("dbo.KPIComments");
            DropTable("dbo.KPI");
            DropTable("dbo.Users");
            DropTable("dbo.UsersGroups");
            DropTable("dbo.Groups");
            DropTable("dbo.Exceptions");
            DropTable("dbo.EmailTemplate");
            DropTable("dbo.EmailConfiguration");
            DropTable("dbo.Delegation");
            DropTable("dbo.Configurations");
            DropTable("dbo.Attachements");
        }
    }
}
