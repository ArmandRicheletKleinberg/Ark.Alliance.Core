namespace Ark
{
    /// <summary>
    /// The different environments used in the app.
    /// </summary>
    public enum EnvironmentEnum
    {
        /// <summary>
        /// Debug is the environment on the developer computer.
        /// </summary>
        Debug,

        /// <summary>
        /// Dev is the internal environment for the developers team.
        /// </summary>
        Dev,

        /// <summary>
        /// Integration on a test server is used by analyst to test development iteration.
        /// </summary>
        Int,

        /// <summary>
        /// Quality Assurance is used as pre prod environment to make the last tests before going to production.
        /// </summary>
        Qa,

        /// <summary>
        /// Production environment.
        /// </summary>
        Prod
    }
}