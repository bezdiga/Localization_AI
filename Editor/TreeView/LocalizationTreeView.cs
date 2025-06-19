using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace HatchStudio.Editor.Localization
{
    public class LocalizationTreeView : TreeView
    {
        private string selectedItem = "Nimic selectat";

        public LocalizationTreeView(TreeViewState state) : base(state)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            // Rădăcina (invizibilă)
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();
            
            int id = 1;

            // Categorie: Fructe
            var fructe = new TreeViewItem { id = id++, depth = 0, displayName = "Fructe" };
            allItems.Add(fructe);
            allItems.Add(new TreeViewItem { id = id++, depth = 1, displayName = "Mere" });
            allItems.Add(new TreeViewItem { id = id++, depth = 1, displayName = "Pere" });

            // Categorie: Legume
            var legume = new TreeViewItem { id = id++, depth = 0, displayName = "Legume" };
            allItems.Add(legume);
            allItems.Add(new TreeViewItem { id = id++, depth = 1, displayName = "Morcovi" });
            allItems.Add(new TreeViewItem { id = id++, depth = 1, displayName = "Cartofi" });

            // Construim ierarhia
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            // Când se schimbă selecția
            if (selectedIds.Count > 0)
            {
                var selected = FindItem(selectedIds[0], rootItem);
                selectedItem = selected.displayName;
            }
            else
            {
                selectedItem = "Nimic selectat";
            }
        }

        public string GetSelectedItem() => selectedItem;
    }
}