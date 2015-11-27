using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ArtistInfoApi.UnitTests
{
    public class ReflectionHelper
    {
        public static string GetMethodName<T, U>(Expression<Func<T, U>> expression)
        {
            var method = expression.Body as MethodCallExpression;
            if (method != null)
            {
                if (method.Method.CustomAttributes.Any(x => x.AttributeType == typeof(ActionNameAttribute)))
                {
                    var attribute = method.Method.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ActionNameAttribute));
                    var name = attribute.ConstructorArguments[0].Value;
                    return (string)name;
                }

                return method.Method.Name;
            }

            throw new ArgumentNullException("Wrong expression");
        }
    }
}
