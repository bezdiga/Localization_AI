using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HatchStudios.Editor.Utils
{
    public class GenericDropDown : AdvancedDropdown
    {
        private readonly IEnumerable<ItemPair> modules;
        public Action<Type> OnItemPressed;

        private string m_title;
        private class ItemElement : AdvancedDropdownItem
        {
            public Type moduleType;

            public ItemElement(string displayName, Type moduleType) : base(displayName)
            {
                this.moduleType = moduleType;
            }
        }
        
        public GenericDropDown(AdvancedDropdownState state, IEnumerable<ItemPair> states,string title = "state") : base(state)
        {
            m_title = title;
            this.modules = states;
            minimumSize = new Vector2(minimumSize.x, 270f);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(m_title);
            var groupMap = new Dictionary<string, AdvancedDropdownItem>();
            foreach (var module in modules)
            {
                Type type = module.itemType;
                string name = module.itemName;
                
                // Split the name into groups
                string[] groups = name.Split('/');
                
                // Create or find the groups
                AdvancedDropdownItem parent = root;
                for (int i = 0; i < groups.Length - 1; i++)
                {
                    string groupPath = string.Join("/", groups.Take(i + 1));
                    if (!groupMap.ContainsKey(groupPath))
                    {
                        var newGroup = new AdvancedDropdownItem(groups[i]);
                        parent.AddChild(newGroup);
                        groupMap[groupPath] = newGroup;
                    }
                    parent = groupMap[groupPath];
                }
                
                // Create the item and add it to the last group
                ItemElement item = new ItemElement(groups.Last(), type);

                //item.icon = MotionIcon;
                parent.AddChild(item);
            }
            return root;
        }
        
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            ItemElement element = (ItemElement)item;
            OnItemPressed?.Invoke(element.moduleType);
        }
    }

    public struct ItemPair
    {
        public Type itemType;
        public string itemName;
    }
}