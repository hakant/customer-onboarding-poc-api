using System;

namespace Origin08.CustomerOnboarding.Features.Shared
{
    public static class Validations
    {
        public static bool BeValidGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }
    }
}