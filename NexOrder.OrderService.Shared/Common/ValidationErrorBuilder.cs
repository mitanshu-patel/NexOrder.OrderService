using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Shared.Common
{
    public class ValidationErrorBuilder
    {
        private Dictionary<string, List<string>> errors = new();

        public static ValidationErrorBuilder Create()
        {
            return new ValidationErrorBuilder();
        }

        public ValidationErrorBuilder AddPropertyError(string propertyName, string errorMessage)
        {
            if (!errors.ContainsKey(propertyName))
            {
                errors[propertyName] = new List<string>();
            }

            errors[propertyName].Add(errorMessage);
            return this;
        }

        public ValidationErrorBuilder AddObjectError(string errorMessage)
        {
            if (!errors.ContainsKey(string.Empty))
            {
                errors[string.Empty] = new List<string>();
            }

            errors[string.Empty].Add(errorMessage);
            return this;
        }

        public bool HasErrors()
        {
            return errors.Any();
        }

        public CustomResponse<T> Build<T>()
        {
            return CustomHttpResult.BadRequest<T>("One or more validations failed", errors);
        }
    }
}
