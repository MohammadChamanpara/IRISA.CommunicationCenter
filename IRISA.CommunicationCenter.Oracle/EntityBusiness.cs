using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IRISA.CommunicationCenter.Oracle
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
                if (entities == null)
                {
                    TContext context = Context;
                    entities = context.CreateObjectSet<TEntity>();
                }
                return entities;
            }
        }
        public string FullyQualifiedEntitySetName
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
                {
                    Entities.EntitySet.EntityContainer.Name,
                    Entities.EntitySet.Name
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
                try
                {
                    TContext context = Context;
                    if (context.Connection.State != ConnectionState.Open)
                    {
                        context = Context;
                        Action action = new Action(context.Connection.Open);
                        context = Context;
                        CallWithTimeout(action, context.Connection.ConnectionTimeout);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public EntityBusiness() : this(Activator.CreateInstance<TContext>())
        {
        }
        public EntityBusiness(TContext context)
        {
            Context = context;
            TContext context2 = Context;
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
                IEnumerable<Attribute> source = propertyInfo.GetCustomAttributes(true).OfType<Attribute>().AsEnumerable();
                EdmScalarPropertyAttribute edmScalarPropertyAttribute = source.OfType<EdmScalarPropertyAttribute>().FirstOrDefault();
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
            List<PropertyInfo> keyColumns = GetKeyColumns(typeof(TEntity));
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
            return new EntityKey(FullyQualifiedEntitySetName, array);
        }
        public virtual IQueryable<TEntity> GetAll()
        {
            return Entities;
        }
        public virtual void Create(TEntity entity)
        {
            Entities.AddObject(entity);
            TContext context = Context;
            context.SaveChanges();
        }
        public virtual void Edit(TEntity entity)
        {
            _ = GetEntity(entity);
            Entities.ApplyCurrentValues(entity);
            TContext context = Context;
            context.SaveChanges();
        }
        public virtual void Delete(object key)
        {
            TEntity entity = GetEntity(key);
            Entities.DeleteObject(entity);
            TContext context = Context;
            context.SaveChanges();
        }
        public virtual TEntity GetEntity(object key)
        {
            EntityKey entityKey = GetEntityKey(new object[]
            {
                key
            });
            return GetEntity(entityKey);
        }
        public virtual TEntity GetEntity(TEntity entity)
        {
            EntityKey arg_3C_0;
            if ((arg_3C_0 = entity.EntityKey) == null)
            {
                TContext context = Context;
                arg_3C_0 = context.CreateEntityKey(Entities.EntitySet.Name, entity);
            }
            EntityKey entityKey = arg_3C_0;
            return GetEntity(entityKey);
        }
        public virtual TEntity GetEntity(EntityKey entityKey)
        {
            TContext context = Context;
            return (TEntity)context.GetObjectByKey(entityKey);
        }
        public void Dispose()
        {
            TContext context = Context;
            context.Dispose();
        }
    }
}
