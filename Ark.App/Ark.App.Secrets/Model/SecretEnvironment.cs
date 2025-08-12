namespace Ark.App.Secrets.Model
{
    /// <summary>
    /// Defines standardized environments used when resolving secrets.
    /// + Enables consistent provider routing across deployments.
    /// - Misclassification may load incorrect credentials.
    /// </summary>
    public enum SecretEnvironment
    {
        #region Values
        /// <summary>Production environment handling live traffic.</summary>
        Production = 0,
        /// <summary>Sandbox or staging environment for pre-production validation.</summary>
        Sandbox = 1,
        /// <summary>Functional or QA testing environment executing automated suites.</summary>
        Test = 2,
        /// <summary>Developer local environment for experiments and debugging.</summary>
        Development = 3,
        #endregion Values
    }
}
