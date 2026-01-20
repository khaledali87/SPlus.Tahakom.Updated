using SPlus.BLL;
using StructureMap;
namespace SPlus.UseCases
{
    public class IOC
    {
        public static Container InitializeContainer()
        {
            var _Container = new Container();

            _Container = new Container(x =>
            {
                x.ForConcreteType<AttachmentBLL>();
                x.ForConcreteType<DelegationBLL>();
                x.ForConcreteType<HandshakeBLL>();
                x.ForConcreteType<KPIBLL>();
                x.ForConcreteType<KPITypeBLL>();
                x.ForConcreteType<LoggingBLL>();
                x.ForConcreteType<LoginBLL>();
                x.ForConcreteType<NotificationBLL>();
                x.ForConcreteType<NotificationConfigurationBLL>();
                x.ForConcreteType<OrgStructureBLL>();
                x.ForConcreteType<ParameterBLL>();
                x.ForConcreteType<RequestBLL>();
                x.ForConcreteType<StrategicObjectiveBLL>();
                x.ForConcreteType<ThemeBLL>();
                x.ForConcreteType<UserBLL>();
                x.ForConcreteType<SystemPerformanceThresholdBLL>();
                x.ForConcreteType<DivisionalObjectiveBLL>();
            });
            return _Container;
        }
    }
}
