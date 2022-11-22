using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strategies
{
    public class SearchComponentsWindow : OdinEditorWindow
    {
        [Searchable]
        [ListDrawerSettings(Expanded = true, ShowPaging = true)]
        public List<DrawChoice> drawChoices = new List<DrawChoice>(256);

        public void Init(DropdownField dropdownField)
        {
            foreach (var c in dropdownField.choices)
            {
                drawChoices.Add(new DrawChoice(c, this, dropdownField));
            }
        }
    }

    [Serializable]
    public class DrawChoice
    {
        [ReadOnly, HideIf("@true")]
        public string Name;

        private SearchComponentsWindow searchComponentsWindow;
        private DropdownField dropdownField;

        public DrawChoice(string choice, SearchComponentsWindow searchComponentsWindow, DropdownField dropdownField)
        {
            Name = choice;
            this.searchComponentsWindow = searchComponentsWindow;
            this.dropdownField = dropdownField;
        }

        [Button("@Name")]
        public void SetComponent()
        {
            dropdownField.value = Name;
            searchComponentsWindow.Close();
        }
    }
}
