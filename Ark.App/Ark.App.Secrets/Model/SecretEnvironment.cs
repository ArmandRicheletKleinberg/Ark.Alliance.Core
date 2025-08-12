namespace Ark.App.Secrets.Model
{
    /// <summary>
    /// Normalized environments for secrets resolution.
    /// </summary>
    public enum SecretEnvironment
    {
        /// <summary>Production environment.</summary>
        Production = 0,
        /// <summary>Sandbox / pre-production environment.</summary>
        Sandbox = 1,
        /// <summary>Functional or QA testing environment.</summary>
        Test = 2,
        /// <summary>Developer local environment.</summary>
        Development = 3,
    }
}
