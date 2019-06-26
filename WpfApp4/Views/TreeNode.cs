using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Views
{
    public class TreeViewItem<T> : IEnumerable<TreeViewItem<T>>
    {
       

        public T Data { get; set; }
        public TreeViewItem<T> Parent { get; set; }
        public ICollection<TreeViewItem<T>> Children { get; set; }

        public Boolean IsRoot
        {
            get { return Parent == null; }
        }

        public Boolean IsLeaf
        {
            get { return Children.Count == 0; }
        }

        public int Level
        {
            get
            {
                if (this.IsRoot)
                    return 0;
                return Parent.Level + 1;
            }
        }


        public TreeViewItem(T data)
        {
            this.Data = data;
            this.Children = new LinkedList<TreeViewItem<T>>();

            this.ElementsIndex = new LinkedList<TreeViewItem<T>>();
            this.ElementsIndex.Add(this);
        }

        public TreeViewItem<T> AddChild(T child)
        {
            TreeViewItem<T> childNode = new TreeViewItem<T>(child) { Parent = this };
            this.Children.Add(childNode);

            this.RegisterChildForSearch(childNode);

            return childNode;
        }

        public override string ToString()
        {
            return Data != null ? Data.ToString() : "[data null]";
        }


        #region searching

        private ICollection<TreeViewItem<T>> ElementsIndex { get; set; }

        private void RegisterChildForSearch(TreeViewItem<T> node)
        {
            ElementsIndex.Add(node);
            if (Parent != null)
                Parent.RegisterChildForSearch(node);
        }

        public TreeViewItem<T> FindTreeNode(Func<TreeViewItem<T>, bool> predicate)
        {
            return this.ElementsIndex.FirstOrDefault(predicate);
        }

        #endregion


        #region iterating

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TreeViewItem<T>> GetEnumerator()
        {
            yield return this;
            foreach (var directChild in this.Children)
            {
                foreach (var anyChild in directChild)
                    yield return anyChild;
            }
        }

        #endregion
    }
}
