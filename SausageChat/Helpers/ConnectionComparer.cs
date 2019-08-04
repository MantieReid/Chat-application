using System.Collections.Generic;
using SausageChat.Core;
using SausageChat.Networking;

namespace SausageChat.Helpers
{
    /// <summary>
    /// This class is used to compare the names of the connections (NOT the guid)
    /// </summary>
    class ConnectionComparer : IComparer<SausageConnection>
    {
        public int Compare(SausageConnection x, SausageConnection y)
        {
            if (x == null || y == null)
                return 0;

            return x.UserInfo.Name.CompareTo(y.UserInfo.Name);
        }
    }
}
