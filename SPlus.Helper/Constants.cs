using System;
using System.Configuration;

namespace SPlus.Helper
{

    public class Constants
    {
        #region Groups
        public static string _AdminGroup = ConfigurationManager.AppSettings["AdminGroup"];
        public static string _ChampionGroup = ConfigurationManager.AppSettings["ChampionGroup"];
        public static int _SLAForRejectionKPI = Convert.ToInt32(ConfigurationManager.AppSettings["SLAForRejectionKPI"]);
        public static string _SMOGroup =ConfigurationManager.AppSettings["SMOGroup"];
        public static string _CPMDirectorGroup = ConfigurationManager.AppSettings["CPMDirector"];
        public static string _CPMSupervisorGroup = ConfigurationManager.AppSettings["CPMSupervisor"];
        public static string _EPMSpecialistGroup = ConfigurationManager.AppSettings["EPMSpecialist"];

        public static string AddMitigationActionGroup = ConfigurationManager.AppSettings["AddMitigationActionGroup"];
        #endregion

        #region ConnectionStrings
        public static string DBConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        public static string PPlusDBConnectionString = ConfigurationManager.AppSettings["PPlusConnectionString"];
        #endregion

        #region URL
        public static string PPlusURL = ConfigurationManager.AppSettings["PPlusURL"];

        #endregion

        #region Tokens
        public static string PPlusToken = ConfigurationManager.AppSettings["PPlusToken"];
        public static string PPlusBasicToken = ConfigurationManager.AppSettings["PPlusBasicToken"];


        #endregion

        #region ConnectionStrings
        public static string CreateStrategicInitiativeEmailSubject = "Strategic Initiative Creation Failed";
        public static string UpdateStrategicInitiativeEmailSubject = "Strategic Initiative Update Failed";
        public static string DeleteStrategicInitiativeEmailSubject = "Strategic Initiative Delete Failed";
        #endregion

        public static int CorporateSectortID = Convert.ToInt32(ConfigurationManager.AppSettings["CorporateSectortID"]);
        public static int CorporateDepartmentID = Convert.ToInt32(ConfigurationManager.AppSettings["CorporateDepartmentID"]);

        public static string _DomainName = ConfigurationManager.AppSettings["DomainName"];
        public static string _LogPath = ConfigurationManager.AppSettings["LogPath"];
        public static bool _Error = ConfigurationManager.AppSettings["Error"] == "false" || ConfigurationManager.AppSettings["Error"] == "False" ? false : true;
        public static string _ThemeList = ConfigurationManager.AppSettings["ThemeList"];
        public static string _PillarList = ConfigurationManager.AppSettings["PillarList"];
        public static string _MainStrategicObjectiveList = ConfigurationManager.AppSettings["MainStrategicObjectiveList"];
        public static string _StrategicObjectiveList = ConfigurationManager.AppSettings["StrategicObjectiveList"];
        public static string _BenefitList = ConfigurationManager.AppSettings["BenefitList"];
        public static string _PortfolioList = ConfigurationManager.AppSettings["PortfolioList"];
        public static string _KPIList = ConfigurationManager.AppSettings["KPIList"];
        public static string _KPIMeasures = ConfigurationManager.AppSettings["KPIMeasuresList"];
        public static string _Attachements = ConfigurationManager.AppSettings["AttachementsList"];
        public static string _ExceptionsList = ConfigurationManager.AppSettings["ExceptionsList"];
        public static string _StrategicObjectiveRelationList = ConfigurationManager.AppSettings["StrategicObjectiveRelationList"]; 
        public static string _PillarRelation = ConfigurationManager.AppSettings["PillarRelationList"];
        public static string _BenefitRealizationList = ConfigurationManager.AppSettings["BenefitRealizationList"];
        public static string _StrategicObjectiveAndKPIRelationList = ConfigurationManager.AppSettings["StrategicObjectiveAndKPIRelationList"];
        public static string _FunctionL1List = ConfigurationManager.AppSettings["FunctionL1List"];
        public static string _FunctionL2List = ConfigurationManager.AppSettings["FunctionL2List"];
        public static string _FunctionL3List = ConfigurationManager.AppSettings["FunctionL3List"];
        public static string _ProgramList = ConfigurationManager.AppSettings["ProgramList"];
        public static string _InitiativeList = ConfigurationManager.AppSettings["InitiativeList"];
        public static string _ProgramRelationList = ConfigurationManager.AppSettings["ProgramRelationList"];

        public static string _WeightFieldName = ConfigurationManager.AppSettings["WeightFieldName"];
        public static string _EnglishName = ConfigurationManager.AppSettings["EnglishName"];
        public static string _ArabicName = ConfigurationManager.AppSettings["ArabicName"];
        public static string _SiteURL = ConfigurationManager.AppSettings["SiteURL"];
        public static string _TitleFieldName = ConfigurationManager.AppSettings["TitleFieldName"];
        public static string _ThemeFieldName = ConfigurationManager.AppSettings["ThemeFieldName"];
        public static string _ThemeTypeFieldName = ConfigurationManager.AppSettings["ThemeTypeFieldName"];
        public static string _PillarFieldName = ConfigurationManager.AppSettings["PillarFieldName"];
        public static string _StrategicObjectiveFieldName = ConfigurationManager.AppSettings["MainStrategicObjectiveFieldName"];
        public static string _MainStrategicObjectiveFieldName = ConfigurationManager.AppSettings["TargetFieldName"];
        public static string _BaselineFieldName = ConfigurationManager.AppSettings["BaselineFieldName"];
        public static string _UnitOfMeasureFieldName = ConfigurationManager.AppSettings["UnitOfMeasureFieldName"];
        public static string _EnglishDescription = ConfigurationManager.AppSettings["EnglishDescriptionFieldName"];
        public static string _ArabicDescription = ConfigurationManager.AppSettings["ArabicDescriptionFieldName"];
        public static string _EnglishEquation = ConfigurationManager.AppSettings["EnglishEquationFieldName"];
        public static string _ArabicEquation = ConfigurationManager.AppSettings["ArabicEquationFieldName"];
        public static string _FrequencyFieldName = ConfigurationManager.AppSettings["FrequencyFieldName"];
        public static string _MeasureTargetFieldName = ConfigurationManager.AppSettings["TargetFieldName"];
        public static string _MeasureValueFieldName = ConfigurationManager.AppSettings["ValueFieldName"];
        public static string _MeasureDueDateFieldName = ConfigurationManager.AppSettings["DueDateFieldName"];
        public static string _KPIFieldName = ConfigurationManager.AppSettings["KPIFieldName"];
        public static string _KPIOwner1FieldName = ConfigurationManager.AppSettings["KPIOwner1FieldName"];
        public static string _KPIOwner2FieldName = ConfigurationManager.AppSettings["KPIOwner2FieldName"];
        public static string _KPITypeFieldName = ConfigurationManager.AppSettings["KPITypeFieldName"];
        public static string _KPIDirectionFieldName = ConfigurationManager.AppSettings["KPIDirectionFieldName"];
        public static string _KPIStatusFieldName = ConfigurationManager.AppSettings["KPIStatusFieldName"];
        public static string _YearsFieldName = ConfigurationManager.AppSettings["YearsFieldName"];
        public static string _BaselineYearFieldName = ConfigurationManager.AppSettings["BaselineYearFieldName"];        
        public static string _FunctionL1FieldName = ConfigurationManager.AppSettings["FunctionL1FieldName"];
        public static string _FunctionL2FieldName = ConfigurationManager.AppSettings["FunctionL2FieldName"];
        public static string _FunctionL3FieldName = ConfigurationManager.AppSettings["FunctionL3FieldName"];
        public static string _PolarityFieldName = ConfigurationManager.AppSettings["PolarityFieldName"];
        public static string IsBoardFieldName = ConfigurationManager.AppSettings["IsBoardFieldName"];
        public static string _DataSourceFieldName = ConfigurationManager.AppSettings["DataSourceFieldName"];
        public static string _StrategicObjectiveIDFieldName = ConfigurationManager.AppSettings["StrategicObjectiveIDFieldName"];
        public static string _BenefitIDFieldName = ConfigurationManager.AppSettings["BenefitIDFieldName"];
        public static string _UserPhotoList = ConfigurationManager.AppSettings["UserPhotoList"];
        public static string _ProgressFieldName = ConfigurationManager.AppSettings["ProgressFieldName"];
        public static string _StatusFieldName = ConfigurationManager.AppSettings["StatusFieldName"];        
        public static string _MessageFieldName = ConfigurationManager.AppSettings["MessageFieldName"];
        public static string _ControllerFieldName = ConfigurationManager.AppSettings["ControllerFieldName"];
        public static string _FunctionFieldName = ConfigurationManager.AppSettings["FunctionFieldName"];
        public static string _siteProtocol = ConfigurationManager.AppSettings["SiteProtocol"];
        public static string AdminGroupID = ConfigurationManager.AppSettings["AdminGroupID"];
        public static string PertmittiveForAllUsersFieldName = ConfigurationManager.AppSettings["PertmittiveForAllUsersFieldName"];
        public static string PermittedUsersFieldName = ConfigurationManager.AppSettings["PermittedUsersFieldName"];
        public static string _KPIIDFieldName = ConfigurationManager.AppSettings["KPIIDFieldName"];
        public static string _RelatedItemID = ConfigurationManager.AppSettings["RelatedItemID"];
        public static string _ItemType = ConfigurationManager.AppSettings["ItemType"];
        public static string _KPIID = ConfigurationManager.AppSettings["KPIID"];

        public static bool _OpenKPI_DueDate_EndOfMonth = Convert.ToBoolean(ConfigurationManager.AppSettings["OpenKPI_DueDate_EndOfMonth"] ?? "false");

        public static string _PillarIDFieldName = ConfigurationManager.AppSettings["PillarIDFieldName"];
        public static string _UserImageExtenstion = ConfigurationManager.AppSettings["UserImageExtenstion"];
        public static DateTime DateTimeServiceMinDate = DateTime.MinValue.AddDays(1);
        public static string _StrategicObjectiveIDRelationFieldName = ConfigurationManager.AppSettings["StrategicObjectiveIDRelationFieldName"];
        public static string _InitiativeIDFieldName = ConfigurationManager.AppSettings["InitiativeIDFieldName"];
        public static string _PredectionServerURL = ConfigurationManager.AppSettings["PredectionServerURL"];
        public static string _PerformanceFieldName = ConfigurationManager.AppSettings["PerformanceFieldName"];

        //AS
        public static string _ParametersList = ConfigurationManager.AppSettings["ParametersList"];
        public static string _ParameterValues = ConfigurationManager.AppSettings["ParametersValuesList"];
        public static string _ParameterIdFieldName = ConfigurationManager.AppSettings["ParameterIdFieldName"];
        public static string _ParameterNameFieldName = ConfigurationManager.AppSettings["ParameterNameFieldName"];
        public static string _ParameterValueFieldName = ConfigurationManager.AppSettings["ParameterValueFieldName"];
        public static string _ParameterUpdateMethodFieldName = ConfigurationManager.AppSettings["ParameterUpdateMethodFieldName"];
        public static string _ParameterKPIIdFieldName = ConfigurationManager.AppSettings["ParameterKPIIdFieldName"];
        public static string _ParameterDescriptionFieldName = ConfigurationManager.AppSettings["ParameterDescriptionFieldName"];
        public static string _KPIFormulaFieldName = ConfigurationManager.AppSettings["KPIFormulaFieldName"];
        public static string _KPITaskIdFieldName = ConfigurationManager.AppSettings["KPITaskIdFieldName"];

        public static string _ParameterFieldIdFieldName = ConfigurationManager.AppSettings["ParameterFieldIdFieldName"];
        public static string _ParameterAggretationTypeFieldName = ConfigurationManager.AppSettings["ParameterAggretationTypeFieldName"];

        //users list info 
        public static string _UserInfolist = ConfigurationManager.AppSettings["UserInfolist"];
        public static string _UserNameFieldName = ConfigurationManager.AppSettings["UserNameFieldName"];
        public static string _DisplayNameFieldName = ConfigurationManager.AppSettings["DisplayNameFieldName"];
        public static string _UserDepartmentFieldName = ConfigurationManager.AppSettings["UserDepartmentFieldName"];
        public static string _UserEmailFieldName = ConfigurationManager.AppSettings["UserEmailFieldName"];
        public static string _UserProfilePictureFieldName = ConfigurationManager.AppSettings["UserProfilePictureFieldName"];
        public static string _UserPhoneNumberFieldName = ConfigurationManager.AppSettings["UserPhoneNumberFieldName"];

        public static string _UseRoleFieldName = ConfigurationManager.AppSettings["UseRoleFieldName"];
        public static string _UserFunctionL1FieldName = ConfigurationManager.AppSettings["UserFunctionL1FieldName"];
        public static string _UserFunctionL2FieldName = ConfigurationManager.AppSettings["UserFunctionL2FieldName"];
        public static string CSOMUserName = ConfigurationManager.AppSettings["CSOMUserName"];
        public static string CSOMPassword = ConfigurationManager.AppSettings["CSOMPassword"];

        public static string StrategicObjectiveFieldName = ConfigurationManager.AppSettings["StrategicObjectiveFieldName"];

        public static string _StrategicObjectiveAndBenefitRelationList = ConfigurationManager.AppSettings["StrategicObjectiveAndBenefitRelationList"];
        public static string _BenefitsPerformanceFieldName = ConfigurationManager.AppSettings["BenefitsPerformanceFieldName"];
        public static bool _CSOM = Convert.ToBoolean(ConfigurationManager.AppSettings["CSOM"]);

        public static int sessionTimeOutMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["sessionTimeOutMinutes"]);

        #region Configurations Constants
        public const string cfg_HandShakeLookups = "HandShake:Lookups";
        public const string cfg_HandShakeConfigs = "HandShake:Configs";
        #endregion
        #region Headers
        public const string hdr_SSOToken = "Ssotoken";
        public const string hdr_Token = "Token";
        public const string hdr_Language = "Lang";
        public const string hdr_WWWAuthenticate = "WWW-Authenticate";
        #endregion
        #region Sessions
        public const string ssn_UserInfo = "UserInfo";
        #endregion



        public static string constEnableTwoFactorAuth = ConfigurationManager.AppSettings["EnableTwoFactorAuth"];
        public static string constTwoFactorAuthBlockTime = ConfigurationManager.AppSettings["TwoFactorAuthBlockTime"];
        public static string constTwoFactorAuthAttemptsCount = ConfigurationManager.AppSettings["TwoFactorAuthAttemptsCount"];
        public static string constTwoFactorAuthSMSExpiryTime = ConfigurationManager.AppSettings["TwoFactorAuthSMSExpiryTime"];
      

    }
}
