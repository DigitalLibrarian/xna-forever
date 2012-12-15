using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Forever.Interface; //HACK

namespace Forever
{
    public class EntityInspectorAttribute : Attribute
    {
        public string InspectorLabel { get; set; }
        public EntityInspectorAttribute(string inspectorLabel)
        {
            InspectorLabel = inspectorLabel;
        }


        public static List<string[]> GetDisplayData(object entity)
        {

            List<string[]> result = new List<string[]>();

            Type entityType = entity.GetType();
            Type[] interfaces = entityType.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                if (interfaceType == typeof(IHasRigidBody))
                {
                    IHasRigidBody bodyHaver = entity as IHasRigidBody;
                    List<string[]> bodyProps = GetDisplayData(bodyHaver.Body);
                    result.AddRange(bodyProps);
                }

                PropertyInfo[] intProperties = interfaceType.GetProperties();
                foreach (PropertyInfo property in intProperties)
                {
                    ExamineProperty(property, ref entity, ref result);
                }
            }
            PropertyInfo[] properties = entityType.GetProperties();

                    
            foreach (PropertyInfo property in properties)
            {
                ExamineProperty(property, ref entity, ref result);

            }
            
            return result;
        }

        public static void ExamineProperty(PropertyInfo property, ref object entity, ref List<string[]> result)
        {
            object[] customAttrs = property.GetCustomAttributes(typeof(EntityInspectorAttribute), true);
            if (customAttrs.Length > 0)
            {
                EntityInspectorAttribute[] attrs = customAttrs as EntityInspectorAttribute[];
                // Should only be one
                if (attrs.Length > 1)
                {
                    throw new InvalidOperationException("Property was given multiple EntityInspector attributes.");
                }
                else
                {
                    string label = attrs[0].InspectorLabel;
                    object value = property.GetValue(entity, null);
                    if (value == null)
                    {
                        value = "(null)";
                    }
                    result.Add(new string[] { label, value.ToString() });

                }
            }
        }
      
    }
}
