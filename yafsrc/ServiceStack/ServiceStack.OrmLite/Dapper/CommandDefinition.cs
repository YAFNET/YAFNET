// ***********************************************************************
// <copyright file="CommandDefinition.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace ServiceStack.OrmLite.Dapper;

/// <summary>
/// Represents the key aspects of a sql operation
/// </summary>
public struct CommandDefinition
{
    /// <summary>
    /// Fors the callback.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>CommandDefinition.</returns>
    internal static CommandDefinition ForCallback(object parameters)
    {
        return parameters is DynamicParameters ? new CommandDefinition(parameters) : default;
    }

    /// <summary>
    /// Called when [completed].
    /// </summary>
    internal void OnCompleted()
    {
        (Parameters as SqlMapper.IParameterCallbacks)?.OnCompleted();
    }

    /// <summary>
    /// The command (sql or a stored-procedure name) to execute
    /// </summary>
    /// <value>The command text.</value>
    public string CommandText { get; }

    /// <summary>
    /// The parameters associated with the command
    /// </summary>
    /// <value>The parameters.</value>
    public object Parameters { get; }

    /// <summary>
    /// The active transaction for the command
    /// </summary>
    /// <value>The transaction.</value>
    public IDbTransaction Transaction { get; }

    /// <summary>
    /// The effective timeout for the command
    /// </summary>
    /// <value>The command timeout.</value>
    public int? CommandTimeout { get; }

    /// <summary>
    /// The type of command that the command-text represents
    /// </summary>
    /// <value>The type of the command.</value>
    public CommandType? CommandType { get; }

    /// <summary>
    /// Should data be buffered before returning?
    /// </summary>
    /// <value><c>true</c> if buffered; otherwise, <c>false</c>.</value>
    public bool Buffered => (Flags & CommandFlags.Buffered) != 0;

    /// <summary>
    /// Should the plan for this query be cached?
    /// </summary>
    /// <value><c>true</c> if [add to cache]; otherwise, <c>false</c>.</value>
    internal bool AddToCache => (Flags & CommandFlags.NoCache) == 0;

    /// <summary>
    /// Additional state flags against this command
    /// </summary>
    /// <value>The flags.</value>
    public CommandFlags Flags { get; }

    /// <summary>
    /// Can async queries be pipelined?
    /// </summary>
    /// <value><c>true</c> if pipelined; otherwise, <c>false</c>.</value>
    public bool Pipelined => (Flags & CommandFlags.Pipelined) != 0;

    /// <summary>
    /// Initialize the command definition
    /// </summary>
    /// <param name="commandText">The text for this command.</param>
    /// <param name="parameters">The parameters for this command.</param>
    /// <param name="transaction">The transaction for this command to participate in.</param>
    /// <param name="commandTimeout">The timeout (in seconds) for this command.</param>
    /// <param name="commandType">The <see cref="CommandType" /> for this command.</param>
    /// <param name="flags">The behavior flags for this command.</param>
    /// <param name="cancellationToken">The cancellation token for this command.</param>
    public CommandDefinition(string commandText, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null,
                             CommandType? commandType = null, CommandFlags flags = CommandFlags.Buffered
                             , CancellationToken cancellationToken = default(CancellationToken)
    )
    {
        CommandText = commandText;
        Parameters = parameters;
        Transaction = transaction;
        CommandTimeout = commandTimeout;
        CommandType = commandType;
        Flags = flags;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDefinition"/> struct.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    private CommandDefinition(object parameters) : this()
    {
        Parameters = parameters;
    }

    /// <summary>
    /// For asynchronous operations, the cancellation-token
    /// </summary>
    /// <value>The cancellation token.</value>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Setups the command.
    /// </summary>
    /// <param name="cnn">The CNN.</param>
    /// <param name="paramReader">The parameter reader.</param>
    /// <returns>IDbCommand.</returns>
    internal IDbCommand SetupCommand(IDbConnection cnn, Action<IDbCommand, object> paramReader)
    {
        var cmd = cnn.CreateCommand();
        var init = GetInit(cmd.GetType());
        init?.Invoke(cmd);
        if (Transaction != null)
            cmd.Transaction = Transaction;
        cmd.CommandText = CommandText;
        if (CommandTimeout.HasValue)
        {
            cmd.CommandTimeout = CommandTimeout.Value;
        }
        else if (SqlMapper.Settings.CommandTimeout.HasValue)
        {
            cmd.CommandTimeout = SqlMapper.Settings.CommandTimeout.Value;
        }
        if (CommandType.HasValue)
            cmd.CommandType = CommandType.Value;
        paramReader?.Invoke(cmd, Parameters);
        return cmd;
    }

    /// <summary>
    /// The command initialize cache
    /// </summary>
    private static SqlMapper.Link<Type, Action<IDbCommand>> commandInitCache;

    /// <summary>
    /// Gets the initialize.
    /// </summary>
    /// <param name="commandType">Type of the command.</param>
    /// <returns>Action&lt;IDbCommand&gt;.</returns>
    private static Action<IDbCommand> GetInit(Type commandType)
    {
        if (commandType == null)
            return null; // GIGO
        if (SqlMapper.Link<Type, Action<IDbCommand>>.TryGet(commandInitCache, commandType, out Action<IDbCommand> action))
        {
            return action;
        }
        var bindByName = GetBasicPropertySetter(commandType, "BindByName", typeof(bool));
        var initialLongFetchSize = GetBasicPropertySetter(commandType, "InitialLONGFetchSize", typeof(int));

        action = null;
        if (bindByName != null || initialLongFetchSize != null)
        {
            var method = new DynamicMethod(commandType.Name + "_init", null, new Type[] { typeof(IDbCommand) });
            var il = method.GetILGenerator();

            if (bindByName != null)
            {
                // .BindByName = true
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, commandType);
                il.Emit(OpCodes.Ldc_I4_1);
                il.EmitCall(OpCodes.Callvirt, bindByName, null);
            }
            if (initialLongFetchSize != null)
            {
                // .InitialLONGFetchSize = -1
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, commandType);
                il.Emit(OpCodes.Ldc_I4_M1);
                il.EmitCall(OpCodes.Callvirt, initialLongFetchSize, null);
            }
            il.Emit(OpCodes.Ret);
            action = (Action<IDbCommand>)method.CreateDelegate(typeof(Action<IDbCommand>));
        }
        // cache it
        SqlMapper.Link<Type, Action<IDbCommand>>.TryAdd(ref commandInitCache, commandType, ref action);
        return action;
    }

    /// <summary>
    /// Gets the basic property setter.
    /// </summary>
    /// <param name="declaringType">Type of the declaring.</param>
    /// <param name="name">The name.</param>
    /// <param name="expectedType">The expected type.</param>
    /// <returns>MethodInfo.</returns>
    private static MethodInfo GetBasicPropertySetter(IReflect declaringType, string name, Type expectedType)
    {
        var prop = declaringType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (prop?.CanWrite == true && prop.PropertyType == expectedType && prop.GetIndexParameters().Length == 0)
        {
            return prop.GetSetMethod();
        }
        return null;
    }
}