using System.Collections.Generic;

namespace Known.Web.Data
{
    public class MenuItem
    {
        private readonly List<MenuItem> _items = new List<MenuItem>();
        private MenuItem Parent { get; set; }

        public IEnumerable<MenuItem> Items => _items;
        public string Text { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        public void AddItem(MenuItem item)
        {
            item.Parent = this;
            _items.Add(item);
        }
    }
}
