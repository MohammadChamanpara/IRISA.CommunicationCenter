using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IRISA.Model
{
	public class EntityBusiness<TContext, TEntity> : IDisposable where TContext : ObjectContext, new() where TEntity : EntityObject
	{
		private ObjectSet<TEntity> entities;
		public TContext Context
		{
			get;
			private set;
		}
		public ObjectSet<TEntity> Entities
		{
			get
			{
				if (this.entities == null)
				{
					TContext context = this.Context;
					this.entities = context.CreateObjectSet<TEntity>();
				}
				return this.entities;
			}
		}
		public string FullyQualifiedEntitySetName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
				{
					this.Entities.EntitySet.EntityContainer.Name,
					this.Entities.EntitySet.Name
				});
			}
		}
		private static void CallWithTimeout(Action action, int timeoutSeconds)
		{
			Thread threadToKill = null;
			Action action2 = delegate
			{
				threadToKill = Thread.CurrentThread;
				action();
			};
			IAsyncResult asyncResult = action2.BeginInvoke(null, null);
			if (asyncResult.AsyncWaitHandle.WaitOne(timeoutSeconds * 1000))
			{
				action2.EndInvoke(asyncResult);
				return;
			}
			threadToKill.Abort();
			throw new TimeoutException();
		}

		public bool Connected
		{
			get
			{
				bool result;
				try
				{
					TContext context = this.Context;
					if (context.Connection.State != ConnectionState.Open)
					{
						context = this.Context;
						Action arg_5B_0 = new Action(context.Connection.Open);
						context = this.Context;
						CallWithTimeout(arg_5B_0, context.Connection.ConnectionTimeout);
					}
					result = true;
				}
				catch
				{
					result = false;
				}
				return result;
			}
		}
		public EntityBusiness() : this(Activator.CreateInstance<TContext>())
		{
		}
		public EntityBusiness(TContext context)
		{
			this.Context = context;
			TContext context2 = this.Context;
			context2.MetadataWorkspace.LoadFromAssembly(typeof(TContext).Assembly);
		}
		private static List<PropertyInfo> GetKeyColumns(Type type)
		{
			List<PropertyInfo> list = new List<PropertyInfo>();
			PropertyInfo[] properties = type.GetProperties();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				IEnumerable<Attribute> source = propertyInfo.GetCustomAttributes(true).OfType<Attribute>().AsEnumerable<Attribute>();
				EdmScalarPropertyAttribute edmScalarPropertyAttribute = source.OfType<EdmScalarPropertyAttribute>().FirstOrDefault<EdmScalarPropertyAttribute>();
				if (edmScalarPropertyAttribute != null)
				{
					if (edmScalarPropertyAttribute.EntityKeyProperty)
					{
						list.Add(propertyInfo);
					}
				}
			}
			return list;
		}
		private EntityKey GetEntityKey(params object[] keyValues)
		{
			List<PropertyInfo> keyColumns = EntityBusiness<TContext, TEntity>.GetKeyColumns(typeof(TEntity));
			if (keyColumns.Count == 0)
			{
				throw new ArgumentException("Entity has no key columns");
			}
			if (keyValues.Length != keyColumns.Count)
			{
				throw new ArgumentException("KeyValues count must be equal with key count");
			}
			KeyValuePair<string, object>[] array = new KeyValuePair<string, object>[keyValues.Length];
			for (int i = 0; i < keyValues.Length; i++)
			{
				array[i] = new KeyValuePair<string, object>(keyColumns[i].Name, Convert.ChangeType(keyValues[i], keyColumns[i].PropertyType));
			}
			return new EntityKey(this.FullyQualifiedEntitySetName, array);
		}
		public virtual IQueryable<TEntity> GetAll()
		{
			return this.Entities;
		}
		public virtual void Create(TEntity entity)
		{
			this.Entities.AddObject(entity);
			TContext context = this.Context;
			context.SaveChanges();
		}
		public virtual void Edit(TEntity entity)
		{
			TEntity entity2 = this.GetEntity(entity);
			this.Entities.ApplyCurrentValues(entity);
			TContext context = this.Context;
			context.SaveChanges();
		}
		public virtual void Delete(object key)
		{
			TEntity entity = this.GetEntity(key);
			this.Entities.DeleteObject(entity);
			TContext context = this.Context;
			context.SaveChanges();
		}
		public virtual TEntity GetEntity(object key)
		{
			EntityKey entityKey = this.GetEntityKey(new object[]
			{
				key
			});
			return this.GetEntity(entityKey);
		}
		public virtual TEntity GetEntity(TEntity entity)
		{
			EntityKey arg_3C_0;
			if ((arg_3C_0 = entity.EntityKey) == null)
			{
				TContext context = this.Context;
				arg_3C_0 = context.CreateEntityKey(this.Entities.EntitySet.Name, entity);
			}
			EntityKey entityKey = arg_3C_0;
			return this.GetEntity(entityKey);
		}
		public virtual TEntity GetEntity(EntityKey entityKey)
		{
			TContext context = this.Context;
			return (TEntity)((object)context.GetObjectByKey(entityKey));
		}
		public void Dispose()
		{
			TContext context = this.Context;
			context.Dispose();
		}
	}
}
