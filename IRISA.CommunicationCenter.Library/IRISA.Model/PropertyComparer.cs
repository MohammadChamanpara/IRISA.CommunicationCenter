using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace IRISA.Model
{
	public class PropertyComparer<T> : IComparer<T>
	{
		private readonly IComparer comparer;
		private PropertyDescriptor propertyDescriptor;
		private int reverse;
		public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
		{
			this.propertyDescriptor = property;
			Type type = typeof(Comparer<>).MakeGenericType(new Type[]
			{
				property.PropertyType
			});
			this.comparer = (IComparer)type.InvokeMember("Default", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null);
			this.SetListSortDirection(direction);
		}
		public int Compare(T x, T y)
		{
			return this.reverse * this.comparer.Compare(this.propertyDescriptor.GetValue(x), this.propertyDescriptor.GetValue(y));
		}
		private void SetPropertyDescriptor(PropertyDescriptor descriptor)
		{
			this.propertyDescriptor = descriptor;
		}
		private void SetListSortDirection(ListSortDirection direction)
		{
			this.reverse = ((direction == ListSortDirection.Ascending) ? 1 : -1);
		}
		public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
		{
			this.SetPropertyDescriptor(descriptor);
			this.SetListSortDirection(direction);
		}
	}
}
