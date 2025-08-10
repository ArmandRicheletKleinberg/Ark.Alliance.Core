using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Ark.Data;
using IBM.WMQ;
using MQMessage = IBM.WMQ.MQMessage;
using MQPutMessageOptions = IBM.WMQ.MQPutMessageOptions;
using MQQueue = IBM.WMQ.MQQueue;

namespace Ark.Net.MqSeries
{
    /// <summary>
    /// This is the base class to manage the communication towards the IBM MQ series server/channel/queues.
    /// </summary>
    public abstract class MqSeriesRepositoryBase
    {
        #region Static

        /// <summary>
        /// The connection pools that can bu used set by the Initialize method.
        /// </summary>
        private static readonly ConcurrentDictionary<string, MQueueConnectionPool> ConnectionPools = new ConcurrentDictionary<string, MQueueConnectionPool>();

        /// <summary>
        /// Initializes the connection pools to use with these repositories.
        /// Must be done once before using the repositories.
        /// </summary>
        /// <param name="connectionPoolsSettings">The connection pools settings to use.</param>
        public static void Initialize(MQueueConnectionPoolSettings[] connectionPoolsSettings)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            connectionPoolsSettings.ForEach(settings =>
            {
                if (!ConnectionPools.ContainsKey(settings.ConnectionPoolName))
                    ConnectionPools.TryAdd(settings.ConnectionPoolName, new MQueueConnectionPool(settings));
            });
        }

        #endregion Static

        #region Fields

        /// <summary>
        /// The name of the MQ connection pool to use with this repository.
        /// </summary>
        private readonly string _connectionPoolName;

        /// <summary>
        /// The connection pool to use by this repository.
        /// </summary>
        private readonly MQueueConnectionPool _connectionPool;

        /// <summary>
        /// The timeout laps to access a queue when a queue is being used.
        /// Default to 5 minutes.
        /// </summary>
        private readonly TimeSpan _queueAccessTimeout;

        /// <summary>
        /// The mainframe serializer is needed to serialize/deserialize complex messages.
        /// </summary>
        internal MainFrameSerializer MainFrameSerializer = new MainFrameSerializer();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="MqSeriesRepositoryBase"/> instance.
        /// </summary>
        /// <param name="connectionPoolName">The name of the MQ connection pool to use with this repository.</param>
        /// <param name="queueAccessTimeout">The maximum duration to access a queue from the connection pool. Default to 5 minutes.</param>
        protected MqSeriesRepositoryBase(string connectionPoolName, TimeSpan queueAccessTimeout = default)
        {
            _connectionPoolName = connectionPoolName;
            _connectionPool = ConnectionPools.GetValue(connectionPoolName);
            _queueAccessTimeout = queueAccessTimeout;
        }

        #endregion Constructors

        #region Methods (Helpers)

        /// <summary>
        /// Executes an action on a queue using a connection given by the connection pools.
        /// The action should not return data.
        /// </summary>
        /// <param name="queueAccessOptions">The options needed to access and to open the queue.</param>
        /// <param name="action">The action to execute on the queue. Synchronous as queue method are synchronous.</param>
        /// <returns>
        /// Success : The execution has succeeded.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// ... and also all the results returned by the action itself.
        /// </returns>
        protected virtual async Task<Result> Execute(MQueueAccessOptions queueAccessOptions, Func<MQQueue, Result> action)
        {
            if (_connectionPool == null)
                return Result.BadPrerequisites.WithReason($"Unable to find the connection pool {_connectionPoolName} settings. " +
                                                           "You must first use MqSeriesRepositoryBase.Initialize method with the connection pools settings to use.");

            return await _connectionPool.Execute(queueAccessOptions, action);
        }

        /// <summary>
        /// Executes an action on a queue using a connection given by the connection pools.
        /// The action should return some data.
        /// </summary>
        /// <param name="queueAccessOptions">The options needed to access and to open the queue.</param>
        /// <param name="action">The action to execute on the queue. Synchronous as queue method are synchronous.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// ... and also all the results returned by the action itself.
        /// </returns>
        protected async Task<Result<T>> Execute<T>(MQueueAccessOptions queueAccessOptions, Func<MQQueue, Result<T>> action)
        {
            if (_connectionPool == null)
                return Result<T>.BadPrerequisites.WithReason($"Unable to find the connection pool {_connectionPoolName} settings. " +
                                                              "You must first use MqSeriesRepositoryBase.Initialize method with the connection pools settings to use.");
            
            return await  _connectionPool.Execute(queueAccessOptions, action);
        }

        /// <summary>
        /// Puts a raw string message into a queue.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="rawStringMessage">The raw string message which will be put onto the queue.</param>
        /// <param name="correlationId">The correlation identifier is used to manage bi directional messages in queue. Limit of 24 bytes.</param>
        /// <returns>
        /// Success : The execution has succeeded.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result> PutRawStringMessage(string queueKey, string rawStringMessage, byte[] correlationId = null)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { OutputAllowed = true }, queue =>
            {
                var message = new MQMessage { Format = MQC.MQFMT_STRING };
                if (correlationId != null)
                    message.CorrelationId = correlationId.Take(24).ToArray();

                message.WriteString(rawStringMessage);
                queue.Put(message, new MQPutMessageOptions());

                return Result.Success;
            });

        /// <summary>
        /// Puts a main frame object into a queue.
        /// </summary>
        /// <typeparam name="TMfo">The type of the main frame object.</typeparam>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="mainframeObject">The main frame object which will be serialized into string.</param>
        /// <param name="correlationId">The correlation identifier is used to manage bi directional messages in queue. Limit of 24 bytes.</param>
        /// <returns>
        /// Success : The execution has succeeded.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result> PutMainFrameObject<TMfo>(string queueKey, TMfo mainframeObject, byte[] correlationId = null)
            where TMfo : class, new()
            {
                var rawStringMessage = MainFrameSerializer.Serialize(mainframeObject);
                return PutRawStringMessage(queueKey, rawStringMessage, correlationId);
            }

        /// <summary>
        /// Gets a raw string message from a queue.
        /// It removes the object of the queue in the same atomic action.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="options">The options that specify how and what message to get.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<string>> GetRawStringMessage(string queueKey, MQueueGetMessageOptions options = null)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.QueueDefault, BrowsingAllowed = options?.IsBrowsing ?? false}, queue =>
            {
                var message = GetQueueMessage(queue, options);
                var rawStringMessage = message.ReadString(message.MessageLength);

                return new Result<string>(rawStringMessage);
            });

        /// <summary>
        /// Gets a main frame object from a queue.
        /// It removes the object of the queue in the same atomic action.
        /// </summary>
        /// <typeparam name="TMfo">The type of the main frame object.</typeparam>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="options">The options that specify how and what message to get.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<TMfo>> GetMainFrameObject<TMfo>(string queueKey, MQueueGetMessageOptions options = null)
            where TMfo : class, new()
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.QueueDefault, BrowsingAllowed = options?.IsBrowsing ?? false}, queue =>
            {
                var message = GetQueueMessage(queue, options);

                var messageString = message.ReadString(message.MessageLength);
                var mainframeObject = MainFrameSerializer.Deserialize<TMfo>(messageString);

                return new Result<TMfo>(mainframeObject);
            });

        /// <summary>
        /// Gets one of many defined main frames object from a queue.
        /// The type of the main frame object is conditioned to an identifier read in the incoming raw string message.
        /// The type is chosen from a correspondance dictionary between the identifier and the type.
        /// It removes the object of the queue in the same atomic action.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="getModelIdentifier">The function used to get the identifier from the raw message string. ie. message => message.SubString(0, 2)</param>
        /// <param name="identifiersToMainFrameObjectTypes">The corresponding main frame object types given the identifiers.</param>
        /// <param name="options">The options that specify how and what message to get.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data in correlated type is returned if type is found or raw string message is returned if not.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<object>> GetMultiMainFrameObject(string queueKey, Func<string, string> getModelIdentifier, Dictionary<string, Type> identifiersToMainFrameObjectTypes, MQueueGetMessageOptions options = null)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.QueueDefault, BrowsingAllowed = options?.IsBrowsing ?? false }, queue =>
            {
                var message = GetQueueMessage(queue, options);

                var messageString = message.ReadString(message.MessageLength);
                var mainFrameObjectTypeIdentifier = getModelIdentifier(messageString);
                var mainFrameObjectType = identifiersToMainFrameObjectTypes.GetValue(mainFrameObjectTypeIdentifier);
                if (mainFrameObjectType == null)
                    return new Result<object>(messageString);

                var mainframeObject = MainFrameSerializer.Deserialize(messageString, mainFrameObjectType);

                return new Result<object>(mainframeObject);
            });

        /// <summary>
        /// Gets one of many defined main frames object from a queue depending on the action code received in the first 2 characters of the message.
        /// The type of the main frame object is conditioned to an identifier read in the incoming raw string message.
        /// The type is chosen from a correspondance dictionary between the action code and the type.
        /// It removes the object of the queue in the same atomic action.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="codeActionsToMainFrameObjectTypes">The correspondence between code actions and main frame object types.</param>
        /// <param name="options">The options that specify how and what message to get.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data in correlated type is returned if type is found or raw string message is returned if not.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<object>> GetMultiMainFrameObjectUsingCodeAction(string queueKey, Dictionary<string, Type> codeActionsToMainFrameObjectTypes, MQueueGetMessageOptions options = null)
            => GetMultiMainFrameObject(queueKey, message => message.Substring(2), codeActionsToMainFrameObjectTypes, options);

        /// <summary>
        /// Browses a raw string message from the queue, executes an custom action and removes the message from the queue if success.
        /// It opens the queue exclusively to prevent another services to change the queue.
        /// As the queue is open exclusively, the action code should not take too much time.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="action">The asynchronous action to execute which returns a result which will be tested to remove the messages from the queue.</param>
        /// <param name="waitInterval">An interval to wait for the message. BEWARE ! this will keep a connection active during this time so use it parsimoniously.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<string>> BrowseFirstRawStringMessageAndRemoveOnActionSuccess(string queueKey, Func<string, Task<Result>> action, TimeSpan waitInterval = default)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.Exclusive, BrowsingAllowed = true }, queue =>
            {
                var message = GetQueueMessage(queue, new MQueueGetMessageOptions { WaitInterval = waitInterval, IsBrowsing = true });
               
                var messageString = message.ReadString(message.MessageLength);

                var result = action(messageString).Result;
                if (result.IsSuccess)
                    GetQueueMessage(queue);

                return new Result<string>(result).WithData(messageString);
            });


        /// <summary>
        /// Browses a raw string message from the queue, executes an custom action and removes the message from the queue if success.
        /// It opens the queue exclusively to prevent another services to change the queue.
        /// As the queue is open exclusively, the action code should not take too much time.
        /// /!\ In this case, the action take the message and the the PutDateTime of the message as parameter.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="action">The asynchronous action to execute which returns a result which will be tested to remove the messages from the queue.</param>
        /// <param name="waitInterval">An interval to wait for the message. BEWARE ! this will keep a connection active during this time so use it parsimoniously.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<string>> BrowseFirstRawStringMessageAndRemoveOnActionSuccess(string queueKey, Func<string, DateTime, Task<Result>> action, TimeSpan waitInterval = default)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.Exclusive, BrowsingAllowed = true }, queue =>
            {
                var message = GetQueueMessage(queue, new MQueueGetMessageOptions { WaitInterval = waitInterval, IsBrowsing = true });

                var messageString = message.ReadString(message.MessageLength);
                var putDatetime = message.PutDateTime.ToLocalTime().ToUniversalTime(); // By default, it is unspecified, force it to universal time

                var result = action(messageString, putDatetime).Result;
                if (result.IsSuccess)
                    GetQueueMessage(queue);

                return new Result<string>(result).WithData(messageString);
            });


        /// <summary>
        /// Browses a main frame object from the queue, executes an custom action and removes the message from the queue if success.
        /// It opens the queue exclusively to prevent another services to change the queue.
        /// As the queue is open exclusively, the action code should not take too much time.
        /// </summary>
        /// <typeparam name="TMfo">The type of the main frame object.</typeparam>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="action">The asynchronous action to execute which returns a result which will be tested to remove the messages from the queue.</param>
        /// <param name="waitInterval">An interval to wait for the message. BEWARE ! this will keep a connection active during this time so use it parsimoniously.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<TMfo>> BrowseFirstMainFrameObjectAndRemoveOnActionSuccess<TMfo>(string queueKey, Func<TMfo, Task<Result>> action, TimeSpan waitInterval = default)
            where TMfo : class, new()
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.Exclusive, BrowsingAllowed = true }, queue =>
            {
                    var message = GetQueueMessage(queue, new MQueueGetMessageOptions { WaitInterval = waitInterval, IsBrowsing = true });
                    var messageString = message.ReadString(message.MessageLength);
                    var mainframeObject = MainFrameSerializer.Deserialize<TMfo>(messageString);

                    var result = action(mainframeObject).Result;
                    if (result.IsSuccess)
                        GetQueueMessage(queue);

                    return new Result<TMfo>(result).WithData(mainframeObject);
            });

        /// <summary>
        /// Browses a main frame object from the queue depending on its action code, executes an custom action and removes the message from the queue if success.
        /// It opens the queue exclusively to prevent another services to change the queue.
        /// As the queue is open exclusively, the action code should not take too much time.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <param name="codeActionsToMainFrameObjectTypes">The correspondence between code actions and main frame object types.</param>
        /// <param name="action">The asynchronous action to execute which returns a result which will be tested to remove the messages from the queue.</param>
        /// <param name="waitInterval">An interval to wait for the message. BEWARE ! this will keep a connection active during this time so use it parsimoniously.</param>
        /// <returns>
        /// Success : The execution has succeeded and the data is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<object>> BrowseFirstMainFrameObjectWithActionCodeAndRemoveOnActionSuccess(string queueKey, Dictionary<string, Type> codeActionsToMainFrameObjectTypes, Func<object, Task<Result>> action, TimeSpan waitInterval = default)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.Exclusive, BrowsingAllowed = true }, queue =>
            {
                var message = GetQueueMessage(queue, new MQueueGetMessageOptions { WaitInterval = waitInterval, IsBrowsing = true });
                var messageString = message.ReadString(message.MessageLength);
                var mainFrameObjectType = codeActionsToMainFrameObjectTypes.GetValue(messageString.Substring(0, 2));

                var mainframeObject = mainFrameObjectType != null
                    ? MainFrameSerializer.Deserialize(messageString, mainFrameObjectType)
                    : messageString;

                var result = action(mainframeObject).Result;
                if (result.IsSuccess)
                    GetQueueMessage(queue);

                return new Result<object>(result).WithData(mainframeObject);
            });

        /// <summary>
        /// Whether the queue has at least one message in it.
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <returns>
        /// Success : The queue has at least one message.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result> HasMessages(string queueKey)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { BrowsingAllowed = true }, queue =>
            {
                // Throws an exception if no message if found
                GetQueueMessage(queue, new MQueueGetMessageOptions { IsBrowsing =  true});
                return Result.Success;
            });

        /// <summary>
        /// Gets the number of message in a queue (depth).
        /// </summary>
        /// <remarks>This may not work with the remote or cluster queues > Error 2068.</remarks>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// <returns>
        /// Success : The execution has succeeded and the number of messages is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result<int>> GetMessagesNumber(string queueKey)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InquiringAllowed = true }, queue => Result<int>.Success.WithData(queue.CurrentDepth));

        /// <summary>
        /// Requests some data from the mainframe.
        /// The request process works by sending a main frame object on one queue and then another queue is listened to receive the response.
        /// This request uses the CorrelationId to manage properly the receiving of the response so the mainframe must add the given correlation id to the message it send back.
        /// </summary>
        /// <typeparam name="TMfoSend">The type of the message to send to the mainframe.</typeparam>
        /// <typeparam name="TMfoReceive">The type of the message that should be received.</typeparam>
        /// <param name="sendQueueKey">The key of the queue where to send the out message to the main frame.</param>
        /// <param name="messageOut">The message to send to the mainframe.</param>
        /// <param name="receiveQueueKey">The queue key to listen the response from the mainframe.</param>
        /// <param name="waitInterval">
        /// The interval to wait for the mainframe response.
        /// returns NotFound if the response is not fetched within this time interval.
        /// BEWARE ! during this wait time, a connection from the pool is used and held so use this with parsimony.
        /// </param>
        /// <returns>
        /// Success : The execution has succeeded and the mainframe response is returned.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue in the interval waiting time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected async Task<Result<TMfoReceive>> RequestMainFrameWithCorrelationId<TMfoSend, TMfoReceive>(string sendQueueKey, TMfoSend messageOut, string receiveQueueKey, TimeSpan waitInterval)
            where TMfoReceive : class, new()
            where TMfoSend : class, new()
        {
            // First PUT the output message ot the output queue
            var correlationId = Guid.NewGuid().ToByteArray();
            var putResult = await PutMainFrameObject(sendQueueKey, messageOut, correlationId);
            if (putResult.IsNotSuccess)
                return new Result<TMfoReceive>(putResult);

            // Then GET the input message during the wait interval
            var getResult = await GetMainFrameObject<TMfoReceive>(receiveQueueKey, new MQueueGetMessageOptions { CorrelationId = correlationId, WaitInterval = waitInterval });
            return getResult;
        }

        /// <summary>
        /// Read the X First messages of the queue. 
        /// </summary>
        /// <param name="queueKey">The key of the queue with is linked to the queue name in the settings.</param>
        /// /// <param name="maxMessagesNumber">Number of messages to read.</param>
        /// <returns>
        /// Success : The execution has succeeded and the list of messages has been returner.
        /// BadPrerequisites : The connection pool name has not been found in the pre initialized settings.
        /// NotFound : No message was found in the queue event with the optional wait time.
        /// Timeout : No available connection could be used to open the queue in the given laps.
        /// NoConnection : No connection can be made to the MQ Series Server.
        /// Unexpected : An unexpected error occurs. 
        /// </returns>
        protected Task<Result<List<string>>> GetQueueXFirstMessages(string queueKey, int maxMessagesNumber)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.Exclusive, BrowsingAllowed = true }, queue =>
            {
                var messages = new List<string>();

                var optionsNextMessage = new MQueueGetMessageOptions { IsBrowsingNext = true };

                for (var i = 0; i < maxMessagesNumber; i++)
                {
                    try
                    {
                        var message = new MQMessage {Format = MQC.MQFMT_STRING};
                        queue.Get(message, optionsNextMessage.GetMqGetMessageOptions());
                        messages.Add(message.ReadString(message.MessageLength));
                    }
                    catch (MQException exception)
                    {
                        if (exception.ReasonCode == 2033)
                        {
                            if (messages.Count == 0)
                                messages.Add("Queue is empty");

                            break;
                        }

                        throw;
                    }
                }

                return new Result<List<string>>(messages);

            });

        /// <summary>
        /// Cleans some old messages to clean up a queue.
        /// </summary>
        /// <param name="queueKey">The key of the queue to clean.</param>
        /// <param name="validityTimeSpan">The timespan during which the message is still valid and should not be deleted.</param>
        /// <returns>
        /// Success : The queue has been cleaned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected Task<Result> CleanOldMessages(string queueKey, TimeSpan validityTimeSpan)
            => Execute(new MQueueAccessOptions(queueKey, _queueAccessTimeout) { InputModeModeAllowed = MQueueAccessInputModeEnum.Exclusive, BrowsingAllowed = true }, queue =>
                {
                    try
                    {
                        while (true)
                        {
                            var optionsNextMessage = new MQueueGetMessageOptions { IsBrowsingNext = true };
                            var message = new MQMessage { Format = MQC.MQFMT_STRING };
                            queue.Get(message, optionsNextMessage.GetMqGetMessageOptions());
                            if (DateTime.UtcNow.Subtract(message.PutDateTime) <= validityTimeSpan)
                                continue;

                            queue.Get(message, new MQGetMessageOptions { MatchOptions = MQC.MQMO_MATCH_MSG_ID });
                        }
                    }
                    catch (MQException exception)
                    {
                        if (exception.ReasonCode != 2033)
                            return new Result(exception);
                    }

                    return Result.Success;
                });

        #endregion Methods (Helpers)

        #region Methods (Private)

        /// <summary>
        /// Gets a message from the queue depending of get message options.
        /// </summary>
        /// <param name="queue">The queue to get the first message from?</param>
        /// <param name="options">The options that specify how and what message to get.</param>
        /// <returns>The message if found, a </returns>
        private static MQMessage GetQueueMessage(MQDestination queue, MQueueGetMessageOptions options = null)
        {
            options = options ?? new MQueueGetMessageOptions();
            var message = new MQMessage { Format = MQC.MQFMT_STRING };

            if (options.CorrelationId != null)
                message.CorrelationId = options.CorrelationId;

            queue.Get(message, options.GetMqGetMessageOptions());
            return message;
        }

        #endregion Methods (Private)
    }
}