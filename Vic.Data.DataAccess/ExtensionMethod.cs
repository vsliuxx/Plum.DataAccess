using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection.Emit;
using System.Reflection;

namespace Vic.Data
{
    public static class ExtensionMethod
    {
        /// <summary>
        /// 返回一个DataTable的List&lt;TResult&gt;实例
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class, new()
        {
            List<TResult> list = new List<TResult>();
            if (dt == null) 
                return list;
            DataTableEntityBuilder<TResult> eblist = DataTableEntityBuilder<TResult>.CreateBuilder(dt.Rows[0]);
            foreach (DataRow info in dt.Rows) 
                list.Add(eblist.Build(info));
            dt.Dispose(); 
            dt = null;
            return list;
        }

        public class DataTableEntityBuilder<Entity>
        {
            private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
            private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
            private delegate Entity Load(DataRow dataRecord);
            private Load handler;
            private DataTableEntityBuilder() { }
            public Entity Build(DataRow dataRecord)
            {
                return handler(dataRecord);
            }
            public static DataTableEntityBuilder<Entity> CreateBuilder(DataRow dataRecord)
            {
                DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
                DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(Entity), new Type[] { typeof(DataRow) }, typeof(Entity), true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(typeof(Entity));
                generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                for (int i = 0; i < dataRecord.ItemArray.Length; i++)
                {
                    PropertyInfo propertyInfo = typeof(Entity).GetProperty(dataRecord.Table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    Label endIfLabel = generator.DefineLabel();
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, i);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
                dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
                return dynamicBuilder;
            }
        }
    }
}
