using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using SPlus.Model.Domain;
using System.Data.Entity;

namespace SPlus.DataAccess
{
    public class AttachmentDataAccess : Repository<Attachment>, IAttachmentRepository
    {
        public AttachmentDataAccess(EntityModel context)
        : base(context)
        {
        }
        public IEnumerable<Attachment> CreateList(List<Attachment> attachements)
        {
            return Context.Set<Attachment>().AddRange(attachements);
        }
    } 
}
