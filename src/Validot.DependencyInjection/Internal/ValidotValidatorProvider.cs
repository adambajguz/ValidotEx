namespace Validot.DependencyInjection.Internal
{
    using System.Collections.Generic;
    using Validot.DependencyInjection;

    /// <summary>
    /// Default implementation of <see cref="IValidotValidatorProvider"/>.
    /// </summary>
    internal sealed class ValidotValidatorProvider : IValidotValidatorProvider
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<Type, IValidotValidator?> _cache = new(); // We can cache validator because they are singletons.
        private IEnumerable<IValidotValidator>? _validatorsCache;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidotValidatorProvider"/>.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ValidotValidatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public IValidotValidator<T>? GetValidator<T>()
        {
            IValidotValidator? validotValidator = _cache.GetOrAdd(typeof(T), static (modelType, serviceProvider) =>
            {
                return serviceProvider.GetService<IValidotValidator<T>>();
            }, _serviceProvider);

            return validotValidator as IValidotValidator<T>;
        }

        /// <inheritdoc/>
        public IValidotValidator? GetValidator(Type modelType)
        {
            ArgumentNullException.ThrowIfNull(modelType);

            IValidotValidator? validotValidator = _cache.GetOrAdd(modelType, static (modelType, serviceProvider) =>
            {
                Type validotValidatorType = typeof(IValidotValidator<>).MakeGenericType(modelType);

                return serviceProvider.GetService(validotValidatorType) as IValidotValidator;
            }, _serviceProvider);

            return validotValidator;
        }

        /// <inheritdoc/>
        public IEnumerable<IValidotValidator> GetValidators()
        {
            if (_validatorsCache is null)
            {
                IEnumerable<IValidotValidator> validators = _serviceProvider.GetServices<IValidotValidator>();

                if (!validators.TryGetNonEnumeratedCount(out int count))
                {
                    count = validators.Count();
                }

                if (count != _cache.Count)
                {
                    foreach (IValidotValidator validator in validators)
                    {
                        _cache.TryAdd(validator.ModelType, validator);
                    }
                }

                _validatorsCache = validators;
            }

            return _validatorsCache;
        }
    }
}
