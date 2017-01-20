using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamSkunk.ViewModels
{
    /// <summary>Selection VM for Dropdown Lists.</summary>
	public class SelectionVM : IComparable
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public SelectionVM()
        {
            this.Text = "";
            this.Value = 0;
        }

        public SelectionVM(string Text, int Value)
        {
            this.Text = Text;
            this.Value = Value;
        }

        public SelectionVM(SelectionVM Copy)
        {
            this.Text = Copy.Text;
            this.Value = Copy.Value;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            SelectionVM toCompare = obj as SelectionVM;
            return this.Text.CompareTo(toCompare.Text);
        }
    }

    /// <summary>Selection VM for Dropdown Lists with a string for the Value.</summary>
    public class SelectionStringVM
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
