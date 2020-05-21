using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EK.TPM.Data
{
    /// <summary>
    /// Contains various utility methods.
    /// </summary>
    public class Util
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================

        //======================================================
        //      Public methods
        //======================================================
        public static Type GetMemberInfoVariableType( MemberInfo memberInfo )
        {
            if (memberInfo is PropertyInfo propertyInfo)
                return propertyInfo.PropertyType;

            if (memberInfo is FieldInfo fieldInfo)
                return fieldInfo.FieldType;

            return null;
        }

        public static void SetMemberValue<T>(object obj, string memberName, T value)
        {
            var memberInfo = obj?
                .GetType()
                .GetMember(
                    memberName, 
                    MemberTypes.Property | MemberTypes.Field, 
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.FirstOrDefault();

            if (memberInfo != null)
                SetMemberValue(obj, memberInfo, value);
        }

        public static void SetMemberValue<T>( object obj, MemberInfo memberInfo, T value )
        {
            if (memberInfo is PropertyInfo propertyInfo)
                propertyInfo.SetValue(obj, value);

            if (memberInfo is FieldInfo fieldInfo)
                fieldInfo.SetValue(obj, value);
        }

        //======================================================
        //      Properties
        //======================================================

        //======================================================
        //      Events
        //======================================================

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.StateMachine
