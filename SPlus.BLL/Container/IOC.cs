using SPlus.DataAccess;
using StructureMap;
namespace SPlus.BLL
{
    public class IOC
    {
        public static Container InitializeContainer()
        {
            var _Container = new Container();
         
            _Container = new Container(x =>
            {
                x.For<IUnitOfWorkFactory>().Use<UnitOfWorkFactory>();
            });
            return _Container;
        }
    }
}
