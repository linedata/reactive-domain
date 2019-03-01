﻿using System;
using System.Threading;
using Newtonsoft.Json;

namespace ReactiveDomain.Messaging {
    /// <summary>
    /// A correlated command that is optionally cancellable using a CancellationToken. 
    /// </summary>
    /// <inheritdoc cref="Message"/>
    public class Command : CorrelatedMessage {

        public bool TimeoutTcpWait = true;

        /// <summary>
        /// The CancellationToken for this command
        /// </summary>
        [JsonIgnore]
        public CancellationToken? CancellationToken;

        /// <summary>
        /// Has this command been canceled?
        /// </summary>
        public bool IsCanceled => CancellationToken?.IsCancellationRequested ?? false;

        /// <summary>
        /// Does this command allow cancellation?
        /// </summary>
        public bool IsCancelable => CancellationToken != null;
        
        public virtual void RegisterOnCancellation(Action action) {
            if(!IsCancelable) {
                throw new InvalidOperationException("Command cannot be canceled");
            }
            CancellationToken?.Register(action);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The source command, the cancellation token will be propagated.</param>
        public Command(Command source) : this(source, source.CancellationToken) { }
        public Command(CorrelatedMessage source, CancellationToken? token = null) : this(new CorrelationId(source), new SourceId(source), token) { }

        [JsonConstructor]
        public Command(CorrelationId correlationId, SourceId sourceId, CancellationToken? token = null) : base(correlationId, sourceId) {
            CancellationToken = token;
        }

        /// <summary>
        /// Create a CommandResponse indicating that this command has succeeded.
        /// </summary>
        public CommandResponse Succeed() {
            return new Success(this);
        }

        /// <summary>
        /// Create a CommandResponse indicating that this command has failed.
        /// </summary>
        public CommandResponse Fail(Exception ex = null) {
            return new Fail(this, ex);
        }

        /// <summary>
        /// Create a CommandResponse indicating that this command has been canceled.
        /// </summary>
        public CommandResponse Canceled() {
            return new Canceled(this);
        }
    }

    /// <summary>
    /// Indicates receipt of a command message.
    /// Does not indicate success or failure of command processing.
    /// </summary>
    /// <inheritdoc cref="Message"/>
    public class AckCommand : CorrelatedMessage {
       /// <summary>
        /// The Command whose receipt is being acknowledged.
        /// </summary>
        public Command SourceCommand { get; }
        /// <summary>
        /// The unique ID of the Command whose receipt is being acknowledged.
        /// </summary>
        public Guid CommandId => SourceCommand.MsgId;
        /// <summary>
        /// The Type of the Command whose receipt is being acknowledged.
        /// </summary>
        public Type CommandType => SourceCommand.GetType();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceCommand">The Command whose receipt is being acknowledged.</param>
        public AckCommand(Command sourceCommand) : base(new CorrelationId(sourceCommand), new SourceId(sourceCommand)) {
            SourceCommand = sourceCommand;
        }

    }
}
