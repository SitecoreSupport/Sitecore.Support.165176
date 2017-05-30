using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.ListManagement.ContentSearch.Model;

namespace Sitecore.Support.ListManagement.ContentSearch
{
    public class ContactListQueryProvider : Sitecore.ListManagement.ContentSearch.ContactListQueryProvider
    {
        bool IncludeSubFolders;
        public ContactListQueryProvider(Sitecore.ContentSearch.ISearchIndex index, string repositoryPath, ID parentId, IEnumerable<ID> templateIds, bool includeSubFolders) : base(index, repositoryPath, parentId, templateIds, includeSubFolders)
        {
            this.IncludeSubFolders = includeSubFolders;
        }

        protected override object Execute(IProviderSearchContext searchContext, Expression expression)
        {
            IQueryable<ContactList> queryable = from cl in searchContext.GetQueryable<ContactList>()
                                                where this.TemplateIds.Contains(cl.TemplateId) && cl.Path.StartsWith(this.RepositoryPath) && cl.IsLatestVersion
                                                select cl;
            if (!object.ReferenceEquals(this.ParentId, null) && !this.IncludeSubFolders)
            {
                queryable = from contactList in queryable
                            where contactList.ParentId == this.ParentId
                            select contactList;
            }
            expression = base.BuildContentSearchExpression(expression, null, queryable.Expression);
            return this.EnsureResultIsNotQueryable(Expression.Lambda(expression, null).Compile().DynamicInvoke(new object[0]));
        }
    }
}