using Common.Core.Exceptions;

namespace Common.Core.Validation
{
    public static class PolicyIdValidator
    {
        /// <summary>
        /// Validator for PolicyId
        /// </summary>
        public static void EnsureValid(string data, string parent = null)
        {
            var property = "policyId"._path(parent);
            if (!ValidationFunctions.StringRequired(data, out var messageRequired))
            {
                throw new ValidationException(property, messageRequired);
            }

            if (!ValidationFunctions.GuidNullable(data, out var guid, out var messageGuid))
            {
                throw new ValidationException(property, messageGuid);
            }

        }
    }
}
