namespace My.Feature.ManageChildItems.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Comparers;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Diagnostics;
    using Sitecore.SecurityModel;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Shell.Applications.Dialogs.Sort;
    using Sitecore.Shell.Applications.Dialogs.SortContent;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Child items sort and delete form.
    /// </summary>
    public class ManageForm : SortForm
    {
        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
            {
                return;
            }

            var sortContentOptions = SortContentOptions.Parse();
            var contentToSortQuery = sortContentOptions.ContentToSortQuery;
            Assert.IsNotNullOrEmpty(contentToSortQuery, "query");

            // use reflection to call private method, we don't want to copy&paste it from Sitecore with bunch of other needed private methods
            var getItemsMethod = typeof(SortForm).GetMethod("GetItemsToSort", BindingFlags.NonPublic | BindingFlags.Instance);
            Item[] itemsToSort = (Item[])getItemsMethod.Invoke(this, new object[] { sortContentOptions.Item, contentToSortQuery });

            Array.Sort(itemsToSort, new DefaultComparer());

            // we need to change behaviour to check if there's at least 1 child item, istead of 2
            if (itemsToSort.Length < 1)
            {
                this.OK.Disabled = true;
            }
            else
            {
                this.OK.Disabled = false;
                this.MainContainer.Controls.Clear();

                // use reflection to call private method, we don't want to copy&paste it from Sitecore  with bunch of other needed private methods
                var renderMethod = typeof(SortForm).GetMethod("Render", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IEnumerable<Item>) }, null);
                this.MainContainer.InnerHtml = (string)renderMethod.Invoke(this, new object[] { itemsToSort });
            }
        }

        /// <inheritdoc/>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            var idsToSort = new ListString(WebUtil.GetFormValue("sortorder"));
            var idsToDelete = new ListString(WebUtil.GetFormValue("deleteItem"));
            if (idsToSort.Count == 0 && idsToDelete.Count == 0)
            {
                base.OnOK(sender, args);
            }
            else
            {
                if (idsToDelete.Count > 0)
                {
                    this.DeleteItems(from i in idsToDelete select ShortID.DecodeID(i));
                    SheerResponse.SetDialogValue("1");
                    base.OnOK(sender, args);
                }

                if (idsToSort.Count > 0)
                {
                    base.OnOK(sender, args);
                }
            }
        }

        /// <summary>
        /// Edit item.
        /// </summary>
        protected virtual void OnEdit()
        {
            var itemId = WebUtil.GetFormValue("editItem");
            this.OpenEditDialog(Client.ContentDatabase.GetItem(new ID(itemId)));

            // force to reload page after closing the dialog, not too optimal, but we don't have feedback from dialog
            SheerResponse.SetDialogValue("1");
        }

        private void DeleteItems(IEnumerable<ID> toDeleteList)
        {
            Assert.ArgumentNotNull(toDeleteList, "toDeleteList");
            foreach (ID id in toDeleteList)
            {
                ID idToFind = id;
                var itemToDelete = Client.ContentDatabase.GetItem(ID.Parse(idToFind));
                if (itemToDelete != null)
                {
                    using (new SecurityDisabler())
                    {
                        itemToDelete.Delete();
                    }
                }
            }
        }

        private void OpenEditDialog(Item item)
        {
            if (item == null)
            {
                return;
            }

            var fieldList = this.CreateFieldDescriptors(item);
            var fieldEditorOptions = new FieldEditorOptions(fieldList)
            {
                SaveItem = true,
            };
            var url = fieldEditorOptions.ToUrlString().ToString();
            SheerResponse.ShowModalDialog(new ModalDialogOptions(url)
            {
                Width = "800",
                Height = "600",
                Response = false,
                ForceDialogSize = false,
            });
        }

        private IEnumerable<FieldDescriptor> CreateFieldDescriptors(Item item)
        {
            var fieldDescriptors = new List<FieldDescriptor>();

            var template = TemplateManager.GetTemplate(item.TemplateID, Client.ContentDatabase);
            var allFields = template.GetFields(true);
            foreach (var field in allFields)
            {
                if (field.Name.StartsWith("__"))
                {
                    continue;
                }

                fieldDescriptors.Add(new FieldDescriptor(item, field.Name));
            }

            return fieldDescriptors;
        }
    }
}
