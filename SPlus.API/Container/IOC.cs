using StructureMap;
using SPlus.UseCases;
namespace SPlus.API
{
    public class IOC
    {
        public static Container InitializeContainer()
        {
            var _Container = new Container();
         
            _Container = new Container(x =>
            {
                x.ForConcreteType<AttachmentUseCases>();
                x.ForConcreteType<DelegationUseCases>();
                x.ForConcreteType<HandshakeUseCases>();
                x.ForConcreteType<KPITypeUseCases>();
                x.ForConcreteType<KPIUseCases>();
                x.ForConcreteType<LoggingUseCases>();
                x.ForConcreteType<LoginUseCases>();
                x.ForConcreteType<NotificationConfigurationUseCases>();
                x.ForConcreteType<NotificationUseCases>();
                x.ForConcreteType<OrgStructureUseCases>();
                x.ForConcreteType<ParameterUseCases>();
                x.ForConcreteType<RequestUseCases>();
                x.ForConcreteType<StrategicObjectiveUseCases>();
                x.ForConcreteType<TaskCenterUseCases>();
                x.ForConcreteType<ThemeUseCases>();
                x.ForConcreteType<UserUseCases>();
                x.ForConcreteType<WeightUseCases>();
                x.ForConcreteType<SystemPerformanceThresholdUseCases>();

            });
            return _Container;
        }
    }
}
