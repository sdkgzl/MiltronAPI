using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Operations.EnumOperations
{
    public static class EnumOperation
    {
        #region ExtensionMethods

        /// <summary>
        /// Extension method to return an enum value of type T for the given string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(this string str) where T : struct, IConvertible
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return Enum.TryParse<T>(str, true, out T val) ? val : default;
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(this int intValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return (T)Enum.ToObject(enumType, intValue);
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="longValue"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(this long longValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return (T)Enum.ToObject(enumType, longValue);
        }

        /// <summary>
        /// Extension method to return an enum value of type T for the given int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ulongValue"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(this ulong ulongValue) where T : struct, IConvertible
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return (T)Enum.ToObject(enumType, ulongValue);
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        /// <summary>
        /// This is secondary extension method for getting only description attribute from enum, it can be used on class properties also
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <example><![CDATA[string enumDesc = MyEnum.HereIsAnother.DescriptionAttr<T>();]]></example>
        /// <example><![CDATA[string classDesc = myInstance.SomeProperty.DescriptionAttr<T>();]]></example>
        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Enum value from description attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description">Description attribute value</param>
        /// <returns></returns>
        /// <example><![CDATA[ Animal x = "Giant Panda".GetValueFromDescription<Animal>();]]></example>
        public static T GetValueFromDescription<T>(this string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default;
        }

        /// <summary>
        /// It should be used for returning second non-enum value otherwise Description attribute
        /// It will return name attribute property
        /// </summary>
        /// <param name="enumValueP"></param>
        /// <returns></returns>
        /// <example><![CDATA[string enumDesc = MyEnum.HereIsAnother.GetDisplayOrValueFromEnum<T>();]]></example>
        public static string GetDisplayOrValueFromEnum<T>(this Enum enumValueP)
        {
            Type type = enumValueP.GetType();

            string name = Enum.GetName(type, enumValueP);

            if (name != null)
            {
                FieldInfo field = type.GetField(name);

                if (field != null)
                {
                    DisplayAttribute attr = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;

                    if (attr != null)
                    {
                        return attr.Name;
                    }
                    else
                    {
                        //If there is no display attribute return enum name
                        return name;
                    }
                }
                else
                {
                    //If there is no attribute return enum name
                    return name;
                }
            }
            else
            {
                //return not null because of using on queries
                return String.Empty;
            }
        }


        #endregion
    }
}
