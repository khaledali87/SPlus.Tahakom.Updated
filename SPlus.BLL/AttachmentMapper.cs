using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DataAccess;
using SPlus.Model.Domain;
namespace SPlus.BLL
{
    public static class AttachmentMapper
    {
        #region KPI
        public static IEnumerable<KPI> MapAttachment(this IEnumerable<KPI> kpis, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            List<Attachment> Attachments = new List<Attachment>();
            foreach (var kpi in kpis)
            {
                Attachments = SourceAttachments.Where(w => w.RelatedItemID == kpi.ID && w.Type == typeof(KPI).Name).ToList();
                kpi.Attachments.AddRange(Attachments);
            }
            return kpis;
        }
        public static KPI MapAttachment(this KPI kpi, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            List<Attachment> Attachments = new List<Attachment>();
            Attachments = SourceAttachments.Where(w => w.RelatedItemID == kpi.ID && w.Type == typeof(KPI).Name).ToList();
            kpi.Attachments.AddRange(Attachments);
            return kpi;
        }
        #endregion

        #region KPI Type
        public static IEnumerable<KPIType> MapAttachment(this IEnumerable<KPIType> KPITypes, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            foreach (var type in KPITypes)
            {
                Attachment = SourceAttachments.Where(w => w.RelatedItemID == type.KPITypeID && w.Type == typeof(KPIType).Name).FirstOrDefault();
                type.Attachment = Attachment;
            }
            return KPITypes;
        }
        public static KPIType MapAttachment(this KPIType type, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            Attachment = SourceAttachments.Where(w => w.RelatedItemID == type.KPITypeID && w.Type == typeof(KPIType).Name).FirstOrDefault();
            type.Attachment = Attachment;
            return type;
        }
        #endregion

        #region Theme
        public static IEnumerable<Theme> MapAttachment(this IEnumerable<Theme> Themes, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            foreach (var Theme in Themes)
            {
                Attachment = SourceAttachments.Where(w => w.RelatedItemID == Theme.ID && w.Type == typeof(Theme).Name).FirstOrDefault();
                Theme.Attachment = Attachment;
            }
            return Themes;
        }
        public static Theme MapAttachment(this Theme Theme, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            Attachment = SourceAttachments.Where(w => w.RelatedItemID == Theme.ID && w.Type == typeof(Theme).Name).FirstOrDefault();
            Theme.Attachment = Attachment;
            return Theme;
        }

        #endregion        

        #region Perspective
        public static IEnumerable<Perspective> MapAttachment(this IEnumerable<Perspective> Perspectives, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            foreach (var Perspective in Perspectives)
            {
                Attachment = SourceAttachments.Where(w => w.RelatedItemID == Perspective.ID && w.Type == typeof(Perspective).Name).FirstOrDefault();
                Perspective.Attachment = Attachment;
            }
            return Perspectives;
        }
        public static Perspective MapAttachment(this Perspective Perspective, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            Attachment = SourceAttachments.Where(w => w.RelatedItemID == Perspective.ID && w.Type == typeof(Perspective).Name).FirstOrDefault();
            Perspective.Attachment = Attachment;
            return Perspective;
        }

        #endregion        

        #region Request
        public static IEnumerable<Request> MapAttachment(this IEnumerable<Request> requests, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            if (requests != null)
            {
                foreach (var request in requests)
                {
                    Attachment = SourceAttachments.Where(w => w.RelatedItemID == request.ID && w.Type == typeof(Request).Name).FirstOrDefault();
                }
            }
            return requests;
        }
        public static Request MapAttachment(this Request request, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            if (request != null)
            {
                Attachment = SourceAttachments.Where(w => w.RelatedItemID == request.ID && w.Type == typeof(Request).Name).FirstOrDefault();
                request.Attachment = Attachment;
            }
            return request;
        }
        public static IEnumerable<Request> MapAttachments(this IEnumerable<Request> requests, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            List<Attachment> Attachments = new List<Attachment>();
            foreach (var request in requests)
            {
                Attachments = SourceAttachments.Where(w => w.RelatedItemID == request.ID && w.Type == typeof(Request).Name).ToList();
                request.Attachments = new List<Attachment>();
                request.Attachments.AddRange(Attachments);
            }
            return requests;
        }
        public static Request MapAttachments(this Request request, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            List<Attachment> Attachments = new List<Attachment>();
            Attachments = SourceAttachments.Where(w => w.RelatedItemID == request.ID && w.Type == typeof(Request).Name).ToList();
            request.Attachments = new List<Attachment>();
            request.Attachments.AddRange(Attachments);
            return request;
        }

        #endregion                      

        #region Strategy
        public static IEnumerable<Strategy> MapAttachment(this IEnumerable<Strategy> strategies, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            foreach (var strategy in strategies)
            {
                Attachment = SourceAttachments.Where(w => w.RelatedItemID == strategy.ID && w.Type == typeof(Strategy).Name).FirstOrDefault();
                strategy.Attachment = Attachment;
            }
            return strategies;
        }
        public static Strategy MapAttachment(this Strategy Strategy, IUnitOfWork dataAccess)
        {
            IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
            Attachment Attachment = new Attachment();
            Attachment = SourceAttachments.Where(w => w.RelatedItemID == Strategy.ID && w.Type == typeof(Strategy).Name).FirstOrDefault();
            Strategy.Attachment = Attachment;
            return Strategy;
        }

        #endregion        
    }
}
