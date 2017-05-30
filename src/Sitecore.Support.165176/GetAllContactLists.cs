using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.ListManagement.Configuration;
using Sitecore.ListManagement.ContentSearch;
using Sitecore.ListManagement.ContentSearch.Model;
using Sitecore.ListManagement.ContentSearch.Pipelines;
using System.Linq;


namespace Sitecore.Support.ListManagement.ContentSearch.Pipelines.GetAllLists
{
    public class GetAllContactLists: Sitecore.ListManagement.ContentSearch.Pipelines.ListProcessor
    {
        private readonly ISearchIndex index;

        public GetAllContactLists()
        {
            this.index = ContentSearchManager.GetIndex(ListManagementSettings.ContactListIndexName);
        }

        public virtual void Process(ListsArgs args)
        {
            Diagnostics.Assert.ArgumentNotNull(args, "args");
            ID itemId = (!args.RootId.IsNull) ? args.RootId : base.RootId;
            Sitecore.Data.Items.Item item = base.Database.GetItem(itemId);
            if (item == null)
            {
                args.ResultLists = Enumerable.Empty<ContactList>().AsQueryable<ContactList>();
                return;
            }
            Sitecore.Data.ID[] templateIds = new Sitecore.Data.ID[]
            {
        base.TemplateId,
        base.SegmentedListTemplateId
            };
            args.ResultLists = new QueryableProxy<ContactList>(new ContactListQueryProvider(this.index, item.Paths.FullPath, item.ID, templateIds, args.IncludeSubFolders));
        }
    }
}