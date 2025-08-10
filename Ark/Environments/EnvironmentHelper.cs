namespace Ark
{
    /// <summary>
    /// This helper class allows to get the current environment the app is working on.
    /// </summary>
    public static class EnvironmentHelper
    {
        #region Methods (Static)

        /// <summary>
        /// Initializes the application environment by settings it manually.
        /// </summary>
        public static void Initialize(EnvironmentEnum environment, string moduleName = null)
        {
            Current = environment;
            ModuleName = moduleName;
            switch (environment)
            {
                case EnvironmentEnum.Int:
                case EnvironmentEnum.Qa:
                    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
                    break;
                case EnvironmentEnum.Prod:
                    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
                    break;
                default:
                    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
                    break;
            }
        }

        /// <summary>
        /// Checks whether the current environment corresponds to the list of EnvironmentEnum parameters.
        /// </summary>
        /// <param name="envEnums">The list of EnvironmentEnum parameters</param>
        /// <returns></returns>
        public static bool IsEnvironment(params EnvironmentEnum[] envEnums)
            => envEnums.Any(e => e == Current);

        #endregion Methods (Static)

        #region Properties (Static)

        /// <summary>
        /// The environment type.
        /// </summary>
        public static EnvironmentEnum Current { get; private set; } = EnvironmentEnum.Debug;

        /// <summary>
        /// The module name if any.
        /// A module is used by some project to specify a specific instance of the same code.
        /// </summary>
        public static string ModuleName { get; private set; }

        #endregion Properties (Static)
    }
}