namespace My.Feature.ManageChildItems.Commands
{
    using System;

    using My.Feature.ManageChildItems.Dialogs;
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.Dialogs.SortContent;
    using Sitecore.Shell.Applications.WebEdit.Commands;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Opens manage child items dialog.
    /// </summary>
    [Serializable]
    public class ManageContent : SortContent
    {
        /// <inheritdoc/>
        protected override SortContentOptions GetOptions(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Item obj = Client.ContentDatabase.GetItem(args.Parameters["itemid"], WebEditUtil.GetClientContentLanguage() ?? Context.Language);
            Assert.IsNotNull(obj, "item");
            return new ManageContentOptions(obj);
        }
    }
}
