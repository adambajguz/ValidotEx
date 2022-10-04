namespace WebApplicationExample.Extensions
{
    using Validot;
    using Validot.Settings;

    /// <summary>
    /// Extensions for <see cref="ValidatorSettings"/>.
    /// </summary>
    public static class SharedValidationSettingsExtensions
    {
        /// <summary>
        /// Applies shared settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ValidatorSettings ApplySharedSettings(this ValidatorSettings settings)
        {
            return settings
                .WithPolishTranslation();
        }
    }
}
