using SPlus.BLL;
using StructureMap;
using System.Collections.Generic;
using System.Threading.Tasks;
using model = SPlus.Model;
namespace SPlus.UseCases
{
    public class LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly LoggingBLL LoggingBLL;
        public LoggingUseCases()
        {
            LoggingBLL = _Container.GetInstance<LoggingBLL>();
        }

        public async Task<int> Log(IEnumerable<string> Token, object item, model.AuditTrailActionENums action, int id = 0)
        {
            return await LoggingBLL.Log(Token, item, action, id);
        }

        public void CreateException(string Title, string ControllerName, string Message, string MethodName)
        {
            LoggingBLL.CreateException(Title, ControllerName, Message, MethodName);
        }
    }
}
