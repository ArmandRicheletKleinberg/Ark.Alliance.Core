using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Ark.App.Diagnostics;
using Ark.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ark.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    /// Ce contrôleur doit être hérité par tous les contrôleurs utilisé dans le projet.
    /// L'attribut ResponseCache est utilisé pour forcer les navigateurs à ne pas cacher les réponses de l'API (utile pour IE).
    /// </summary>
    [ApiController]
    [Route("api")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        #region Fields

        /// <summary>
        /// The logger used to log controllers failed results.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Whether to log only the unexpected result, otherwise log all the result that did not succeed.
        /// </summary>
        protected readonly bool IsLoggingOnlyUnexpectedResult;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="ControllerBase"/> instance.
        /// </summary>
        /// <param name="logger">The logger used to log the failed request results.</param>
        /// <param name="isLoggingOnlyUnexpectedResult">Whether to log only the unexpected result, otherwise log all the result that did not succeed.</param>
        protected ControllerBase(ILogger logger = null, bool isLoggingOnlyUnexpectedResult = true)
        {
            Logger = logger ?? DiagBase.Logs.GetValue(nameof(LoggersBase.Controllers));

            IsLoggingOnlyUnexpectedResult = isLoggingOnlyUnexpectedResult;
        }

        #endregion Constructors

        #region Methods (Protected)

        /// <summary>
        /// Execute some business logic asynchronously and returns a consistent HttpResponseDto which own the data to be returned along with some status.
        /// </summary>
        /// <typeparam name="TReturn">The type of the data returned by the Business Logic.</typeparam>
        /// <param name="blFunctionToExecute">The business logic function to execute.</param>
        /// <returns>A consistent HttpResponseDto filled with the returned data if any and the status.</returns>
        protected virtual async Task<ResultDto<TReturn>> ExecuteBlAsync<TReturn>(Func<Task<Result<TReturn>>> blFunctionToExecute)
        {
            try
            {
                var result = await blFunctionToExecute();
                LogResult(result);
                var response = CreateResultDtoFromResult(result);
                return response;
            }
            catch (Exception exception)
            {
                // If an unexpected error occurs then returns an unexpected response (should not occur as BL are included in a try/catch)
                Logger.LogCritical(exception, $"Exception while executing the business logic in the controller : {exception.Message}, User : {HttpContext.User.Identity?.Name}");
                return new ResultDto<TReturn>
                {
                    Status = ResultStatus.Unexpected,
                    Reason = exception.Message
                };
            }
        }

        /// <summary>
        /// Execute some business logic and returns a consistent HttpResponseDto which own the data to be returned along with some status.
        /// </summary>
        /// <typeparam name="TReturn">The type of the data returned by the Business Logic.</typeparam>
        /// <param name="blFunctionToExecute">The business logic function to execute.</param>
        /// <returns></returns>
        protected virtual ResultDto<TReturn> ExecuteBl<TReturn>(Func<Result<TReturn>> blFunctionToExecute)
            => ExecuteBlAsync(() => Task.Run(blFunctionToExecute)).Result;

        /// <summary>
        /// Execute some business logic asynchronously and returns a consistent HttpResponseDto which own the data to be returned along with some status.
        /// </summary>
        /// <param name="blFunctionToExecute">The business logic function to execute.</param>
        /// <returns>A consistent HttpResponseDto filled with the returned data if any and the status.</returns>
        protected virtual async Task<ResultDto> ExecuteBlAsync(Func<Task<Result>> blFunctionToExecute)
        {
            try
            {
                var result = await blFunctionToExecute();
                LogResult(result);
                var response = CreateResultDtoFromResult(result);
                return response;
            }
            catch (Exception exception)
            {
                // If an unexpected error occurs then returns an unexpected response (should not occur as BL are included in a try/catch)
                Logger.LogCritical(exception, $"Exception while executing the business logic in the controller : {exception.Message}, User : {HttpContext.User.Identity?.Name}");
                return new ResultDto
                {
                    Status = ResultStatus.Unexpected,
                    Reason = exception.Message
                };
            }
        }

        /// <summary>
        /// Execute some business logic and returns a consistent HttpResponseDto which own the data to be returned along with some status.
        /// </summary>
        /// <param name="blFunctionToExecute">The business logic function to execute.</param>
        /// <returns></returns>
        protected virtual ResultDto ExecuteBl(Func<Result> blFunctionToExecute)
            => ExecuteBlAsync(() => Task.Run(blFunctionToExecute)).Result;

        /// <summary>
        /// Executes a business logic method that returns a Result{FileDto} and return the file to the user.
        /// </summary>
        /// <param name="blFunctionToExecute">The business logic method to execute that returns a Result{FileDto}.</param>
        /// <param name="displayPdf">Whether this is a PDF file to display.</param>
        /// <returns>
        /// 200 : The file to download/display;
        /// 404 : The file has not been found;
        /// 500 : An unexpected error occurs;
        /// </returns>
        protected virtual async Task<IActionResult> ExecuteBlAndReturnFile(Func<Task<Result<FileDto>>> blFunctionToExecute, bool displayPdf = false)
        {
            try
            {
                // Tries to get the file and returns an HTTP code error in case of failure
                var fileResult = await blFunctionToExecute();
                if (fileResult.IsNotFound) return NotFound();
                if (fileResult.IsNotSuccess) return StatusCode(500);
                var file = fileResult.Data;

                if (displayPdf)
                {
                    Response.Headers.Append(
                        "Content-Disposition",
                        new ContentDisposition { FileName = file.Name, Inline = true }.ToString());
                    Response.Headers.Append("X-Content-Type-Options", "nosniff");
                    return new FileStreamResult(new MemoryStream(file.Content), file.MimeType);
                }

                return File(new MemoryStream(file.Content), file.MimeType, file.Name);
            }
            catch (Exception exception)
            {
                // If an unexpected error occurs then returns an unexpected response (should not occur as BL are included in a try/catch)
                Logger.LogCritical(exception, $"Exception while executing the business logic in the controller : {exception.Message}, User : {HttpContext.User.Identity?.Name}");
                if (EnvironmentHelper.IsEnvironment(EnvironmentEnum.Prod))
                    return Ok();

                return StatusCode(500, exception.ToDetailedString());
            }
        }

        /// <summary>
        /// Creates a new ResultDto given a Result.
        /// </summary>
        /// <param name="result">The result to convert in ResultDto.</param>
        /// <returns>The newly created ResultDto.</returns>
        protected virtual ResultDto CreateResultDtoFromResult(Result result)
            => new ResultDto
            {
                Status = result.Status,
                Reason = EnvironmentHelper.IsEnvironment(EnvironmentEnum.Prod) ? null : result.Reason,
                Exception = EnvironmentHelper.IsEnvironment(EnvironmentEnum.Prod) ? null : MapException(result.Exception)
            };

        /// <summary>
        /// Creates a new ResultDto given a Result{TResult}.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="result">The result to convert in ResultDto.</param>
        /// <returns>The newly created ResultDto.</returns>
        protected virtual ResultDto<TResult> CreateResultDtoFromResult<TResult>(Result<TResult> result)
            => new ResultDto<TResult>
            {
                Data = result.Data,
                Status = result.Status,
                Reason = EnvironmentHelper.IsEnvironment(EnvironmentEnum.Prod) ? null : result.Reason,
                Exception = EnvironmentHelper.IsEnvironment(EnvironmentEnum.Prod) ? null : MapException(result.Exception)
            };

        /// <summary>
        /// Logs the result if not success using the optional set logger.
        /// </summary>
        /// <param name="result">The result to log.</param>
        protected virtual void LogResult(Result result)
        {
            if (Logger == null || result.IsSuccess || IsLoggingOnlyUnexpectedResult && result.IsNotUnexpected)
                return;

            Logger.LogResult(result);
        }

        #endregion Methods (Protected)

        #region Methods (Helpers)

        /// <summary>
        /// Maps an Exception to a DTO.
        /// </summary>
        /// <param name="exception">The exception to map.</param>
        /// <returns>The exception DTO mapped.</returns>
        protected ExceptionDto MapException(Exception exception)
            => exception == null
                ? null
                : new ExceptionDto
                {
                    ExceptionType = exception.GetType().FullName,
                    Data = exception.Data,
                    HelpLink = exception.HelpLink,
                    InnerException = MapException(exception.InnerException),
                    Message = exception.Message,
                    Source = exception.Source,
                    StackTrace = exception.StackTrace
                };

        #endregion Methods (Helpers)
    }

    /// <inheritdoc />
    /// <summary>
    /// Ce contrôleur doit être hérité par tous les contrôleurs utilisé dans le projet.
    /// L'attribut ResponseCache est utilisé pour forcer les navigateurs à ne pas cacher les réponses de l'API (utile pour IE).
    /// </summary>
    /// <typeparam name="TUserProfileData">The type of the user profile data.</typeparam>
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class ControllerBase<TUserProfileData> : ControllerBase
        where TUserProfileData : new()
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="ControllerBase"/> instance.
        /// </summary>
        /// <param name="logger">The logger used to log the failed request results.</param>
        /// <param name="isLoggingOnlyUnexpectedResult">Whether to log only the unexpected result, otherwise log all the result that did not succeed.</param>
        protected ControllerBase(ILogger logger = null, bool isLoggingOnlyUnexpectedResult = true)
            : base(logger, isLoggingOnlyUnexpectedResult)
        { }

        #endregion Constructors

        #region Properties (Protected)

        /// <summary>
        /// Les informations de session utilisateur reprises du contexte HTTP (mise dans le contexte par le mécanisme d'authentification).
        /// </summary>
        private UserSession<TUserProfileData> _userSession;

        /// <summary>
        /// The user session taken from the HTTP context (set by the user authorization mechanism)./// Les informations de session utilisateur reprises du contexte HTTP (mise dans le contexte par le mécanisme d'authentification).
        /// Lazy loaded.
        /// </summary>
        protected UserSession<TUserProfileData> UserSession
            => _userSession ??= HttpContext.Items.GetValue(nameof(UserSession)) as UserSession<TUserProfileData>;

        #endregion Properties (Protected)
    }
}