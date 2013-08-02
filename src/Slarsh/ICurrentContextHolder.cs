﻿namespace Slarsh
{
    /// <summary>
    /// The CurrentContextHolder interface, that must be implemented in order to provide <see cref="Context.Current"/> functionality.
    /// </summary>
    public interface ICurrentContextHolder
    {
        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        IContext GetCurrentContext();

        /// <summary>
        /// Sets the current context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        void SetCurrentContext(IContext context);
    }
}
