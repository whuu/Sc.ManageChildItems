namespace My.Feature.ManageChildItems.Dialogs
{
    using Sitecore.Data.Items;
    using Sitecore.Shell.Applications.Dialogs.SortContent;

    /// <summary>
    /// Manage Content Dialog Options.
    /// </summary>
    public class ManageContentOptions : SortContentOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageContentOptions"/> class.
        /// </summary>
        /// <param name="item">Item.</param>
        public ManageContentOptions(Item item)
            : base(item)
        {
        }

        /// <inheritdoc/>
        protected override string GetXmlControl()
        {
            return "Sitecore.Shell.Applications.Dialogs.Manage";
        }
    }
}
