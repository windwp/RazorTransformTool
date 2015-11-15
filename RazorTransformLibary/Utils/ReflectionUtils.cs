using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RazorTransformLibary.Utils
{
    /// <summary>
    /// Collection of Reflection and type conversion related utility functions
    /// </summary>
    public static class ReflectionUtils
    {
        public enum PropertyAccessLevel
        {
            Public,
            Private,
            Static,
            All
        }
        public const BindingFlags MemberAccess =
          BindingFlags.Public | BindingFlags.NonPublic |
          BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;
        public static string[] PropertyNamesWithAccessFromType(object atype, PropertyAccessLevel access)
        {
            if (atype == null) return new string[] { };
            Type t = atype.GetType();
            PropertyInfo[] props;
            List<string> propNames = new List<string>();
            switch (access)
            {
                case PropertyAccessLevel.All:
                    props = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (PropertyInfo prp in props)
                        propNames.Add(prp.Name);
                    break;
                case PropertyAccessLevel.Public:
                    props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo prp in props)
                        propNames.Add(prp.Name);
                    break;
                case PropertyAccessLevel.Private:
                    props = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (PropertyInfo prp in props)
                        propNames.Add(prp.Name);
                    break;
                case PropertyAccessLevel.Static:
                    props = t.GetProperties(BindingFlags.Static | BindingFlags.Instance);
                    foreach (PropertyInfo prp in props)
                        propNames.Add(prp.Name);
                    break;
            }
            return propNames.ToArray();
        }
        public static string[] PropertyNamesFromType(object atype)
        {
            if (atype == null) return new string[] { };
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<string> propNames = new List<string>();
            foreach (PropertyInfo prp in props)
                propNames.Add(prp.Name);
            return propNames.ToArray();
        }

        public static PropertyInfo[] PropertysFromType(object atype)
        {
            if (atype == null) return new PropertyInfo[] { };
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<PropertyInfo> propNames = new List<PropertyInfo>();
            foreach (PropertyInfo prp in props)
                propNames.Add(prp);
            return propNames.ToArray();
        }
        public static object SetPropertyInternal(object parent, string property, object value)
        {
            if (property == "this" || property == "me")
                return parent;

            object result = null;
            string pureProperty = property;
            string indexes = null;
            bool isArrayOrCollection = false;

            // Deal with Array Property
            if (property.IndexOf("[") > -1)
            {
                pureProperty = property.Substring(0, property.IndexOf("["));
                indexes = property.Substring(property.IndexOf("["));
                isArrayOrCollection = true;
            }

            if (!isArrayOrCollection)
            {
                // Get the member
                MemberInfo member = parent.GetType().GetMember(pureProperty, MemberAccess)[0];
                if (member.MemberType == MemberTypes.Property)
                    ((PropertyInfo)member).SetValue(parent, value, null);
                else
                    ((FieldInfo)member).SetValue(parent, value);
                return null;
            }
            else
            {
                // Get the member
                MemberInfo member = parent.GetType().GetMember(pureProperty, MemberAccess)[0];
                if (member.MemberType == MemberTypes.Property)
                    result = ((PropertyInfo)member).GetValue(parent, null);
                else
                    result = ((FieldInfo)member).GetValue(parent);
            }
            indexes = indexes.Replace("[", string.Empty).Replace("]", string.Empty);

            if (result is Array)
            {
                int index = -1;
                int.TryParse(indexes, out index);
                result = CallMethod(result, "SetValue", value, index);
            }
            else if (result is ICollection)
            {
                if (indexes.StartsWith("\""))
                {
                    // String Index
                    indexes = indexes.Trim('\"');
                    result = CallMethod(result, "set_Item", indexes, value);
                }
                else
                {
                    // assume numeric index
                    int index = -1;
                    int.TryParse(indexes, out index);
                    result = CallMethod(result, "set_Item", index, value);
                }
            }

            return result;
        }
        /// <summary>
        /// Calls a method on an object dynamically. This version requires explicit
        /// specification of the parameter type signatures.
        /// </summary>
        /// <param name="instance">Instance of object to call method on</param>
        /// <param name="method">The method to call as a stringToTypedValue</param>
        /// <param name="parameterTypes">Specify each of the types for each parameter passed. 
        /// You can also pass null, but you may get errors for ambiguous methods signatures
        /// when null parameters are passed</param>
        /// <param name="parms">any variable number of parameters.</param>        
        /// <returns>object</returns>
        public static object CallMethod(object instance, string method, Type[] parameterTypes, params object[] parms)
        {
            if (parameterTypes == null && parms.Length > 0)
                // Call without explicit parameter types - might cause problems with overloads    
                // occurs when null parameters were passed and we couldn't figure out the parm type
                return instance.GetType().GetMethod(method, MemberAccess | BindingFlags.InvokeMethod).Invoke(instance, parms);
            else
                // Call with parameter types - works only if no null values were passed
                return instance.GetType().GetMethod(method, MemberAccess | BindingFlags.InvokeMethod, null, parameterTypes, null).Invoke(instance, parms);
        }

        /// <summary>
        /// Calls a method on an object dynamically. 
        /// 
        /// This version doesn't require specific parameter signatures to be passed. 
        /// Instead parameter types are inferred based on types passed. Note that if 
        /// you pass a null parameter, type inferrance cannot occur and if overloads
        /// exist the call may fail. if so use the more detailed overload of this method.
        /// </summary> 
        /// <param name="instance">Instance of object to call method on</param>
        /// <param name="method">The method to call as a stringToTypedValue</param>
        /// <param name="parameterTypes">Specify each of the types for each parameter passed. 
        /// You can also pass null, but you may get errors for ambiguous methods signatures
        /// when null parameters are passed</param>
        /// <param name="parms">any variable number of parameters.</param>        
        /// <returns>object</returns>
        public static object CallMethod(object instance, string method, params object[] parms)
        {
            // Pick up parameter types so we can match the method properly
            Type[] parameterTypes = null;
            if (parms != null)
            {
                parameterTypes = new Type[parms.Length];
                for (int x = 0; x < parms.Length; x++)
                {
                    // if we have null parameters we can't determine parameter types - exit
                    if (parms[x] == null)
                    {
                        parameterTypes = null;  // clear out - don't use types        
                        break;
                    }
                    parameterTypes[x] = parms[x].GetType();
                }
            }
            return CallMethod(instance, method, parameterTypes, parms);
        }
    }

}


