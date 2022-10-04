namespace Validot.DependencyInjection
{
    using System.Reflection;
    using Validot;
    using Validot.DependencyInjection.Internal;
    using Validot.Factory;

    /// <summary>
    /// Validot-related <see cref="ValidotBuilder"/> extensions.
    /// </summary>
    public static class ValidotBuilderExtensions
    {
        /// <summary>
        /// Adds validators to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static ValidotBuilder AddValidators(this ValidotBuilder builder, IEnumerable<Assembly> assemblies)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(assemblies);

            return builder.AddValidators(assemblies.ToArray());
        }

        /// <summary>
        /// Adds validators to the application.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static ValidotBuilder AddValidators(this ValidotBuilder builder, params Assembly[] assemblies)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(assemblies);

            Assembly[] assembliesToScan = assemblies is { Length: > 0 }
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies();

            IEnumerable<HolderInfo> holders = Validator.Factory.FetchHolders(assembliesToScan)
                .GroupBy(h => h.SpecifiedType)
                .Select(s => s.First());

            IServiceCollection services = builder.Services;

            foreach (HolderInfo holder in holders)
            {
                Type modelType = holder.SpecifiedType;
                Type validotValidatorType = typeof(IValidotValidator<>).MakeGenericType(modelType);

                object validator = holder.CreateValidator();
                services.AddSingleton(holder.ValidatorType, validator);

                services.AddSingleton(validotValidatorType, typeof(ValidotValidator<>).MakeGenericType(modelType));
                services.AddSingleton(provider => (IValidotValidator)provider.GetRequiredService(validotValidatorType));
            }

            return builder;
        }

        /// <summary>
        /// Adds a validator to the application.
        /// </summary>
        /// <typeparam name="TValidator"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="builder"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static ValidotBuilder AddValidator<TValidator, TModel>(this ValidotBuilder builder, IValidator<TModel> validator)
            where TValidator : ISpecificationHolder<TModel>
            where TModel : class
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(validator);

            IServiceCollection services = builder.Services;

            services.AddSingleton(validator);

            services.AddSingleton<IValidotValidator<TModel>, ValidotValidator<TModel>>();
            services.AddSingleton<IValidotValidator>(provider => provider.GetRequiredService<IValidotValidator<TModel>>());

            return builder;
        }
    }
}