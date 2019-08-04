/*
 * the reason we don't have messaging here is because Client doesn't contain User which is a part of
 * the server side UserMessage class and both projects work with the messages differently
 * so I've decided to keep them separated
 * - Archy (Disco)
 */

using System.Threading;

namespace SausageChat.Core
{
    // TODO: add StripData to this class and use it at both projects from here
    public static class SausageHelper
    {
        /// <summary>
        /// wraps SynchronizationContext.Send() function with a null object state
        /// </summary>
        /// <param name="UiCtx">Thread context</param>
        /// <param name="d">Callback</param>
        public static void Send(this SynchronizationContext UiCtx, SendOrPostCallback d) => UiCtx.Send(d, null);
    }
}
