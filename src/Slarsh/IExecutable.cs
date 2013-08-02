namespace Slarsh
{
    /// <summary>
    /// The Executable interface.
    /// An executable is anything that can be executed inside a context.
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">
        /// The current context.
        /// </param>
        void Execute(IContext context);
    }

    /// <summary>
    /// The Executable interface, with a result type.
    /// An executable is anything that can be executed inside a context.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result.
    /// </typeparam>
    public interface IExecutable<out T> : IExecutable
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">
        /// The current context.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        new T Execute(IContext context);
    }
}
