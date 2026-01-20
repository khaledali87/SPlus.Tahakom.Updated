using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model.Domain;

namespace SPlus.DataAccess
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        IEnumerable<Attachment> CreateList(List<Attachment> attachements);
      
    }
}
 