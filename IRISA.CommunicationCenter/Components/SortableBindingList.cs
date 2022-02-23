using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IRISA.CommunicationCenter.Components
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;
        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }
        protected override bool IsSortedCore
        {
            get
            {
                return isSorted;
            }
        }
        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return propertyDescriptor;
            }
        }
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return listSortDirection;
            }
        }
        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }
        public SortableBindingList() : base(new List<T>())
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }
        public SortableBindingList(IEnumerable<T> enumeration) : base(new List<T>(enumeration))
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> list = (List<T>)Items;
            Type propertyType = property.PropertyType;
            if (!comparers.TryGetValue(propertyType, out PropertyComparer<T> propertyComparer))
            {
                propertyComparer = new PropertyComparer<T>(property, direction);
                comparers.Add(propertyType, propertyComparer);
            }
            propertyComparer.SetPropertyAndDirection(property, direction);
            list.Sort(propertyComparer);
            propertyDescriptor = property;
            listSortDirection = direction;
            isSorted = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        protected override void RemoveSortCore()
        {
            isSorted = false;
            propertyDescriptor = base.SortPropertyCore;
            listSortDirection = base.SortDirectionCore;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        protected override int FindCore(PropertyDescriptor property, object key)
        {
            int count = Count;
            int result;
            for (int i = 0; i < count; i++)
            {
                T t = base[i];
                if (property.GetValue(t).Equals(key))
                {
                    result = i;
                    return result;
                }
            }
            result = -1;
            return result;
        }
    }
}
