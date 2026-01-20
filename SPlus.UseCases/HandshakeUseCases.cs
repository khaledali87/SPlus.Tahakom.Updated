using SPlus.BLL;
using SPlus.DTO;
using StructureMap;
using System.Collections.Generic;
namespace SPlus.UseCases
{
    public class HandshakeUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly HandshakeBLL HandshakeBLL;
        public HandshakeUseCases()
        {
            HandshakeBLL = _Container.GetInstance<HandshakeBLL>();
        }
        #region Read
        public List<LookupDTO> Read()
        {
            return AutoMapper.Mapper.Map<List<LookupDTO>>(HandshakeBLL.Read());
        }
        #endregion


    }
}
