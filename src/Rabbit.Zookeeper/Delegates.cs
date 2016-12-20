using org.apache.zookeeper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rabbit.Zookeeper
{
    /// <summary>
    /// 连接状态变更事件参数。
    /// </summary>
    public class ConnectionStateChangeArgs
    {
        public Watcher.Event.KeeperState State { get; set; }
    }

    /// <summary>
    /// 节点变更参数。
    /// </summary>
    public abstract class NodeChangeArgs
    {
        protected NodeChangeArgs(string path, Watcher.Event.EventType type)
        {
            Path = path;
            Type = type;
        }

        /// <summary>
        /// 变更类型。
        /// </summary>
        public Watcher.Event.EventType Type { get; private set; }

        /// <summary>
        /// 节点路径。
        /// </summary>
        public string Path { get; private set; }
    }

    public sealed class NodeDataChangeArgs : NodeChangeArgs
    {
        public NodeDataChangeArgs(string path, Watcher.Event.EventType type, IEnumerable<byte> currentData) : base(path, type)
        {
            CurrentData = currentData;
        }

        /// <summary>
        /// 当前节点数据（最新的）
        /// </summary>
        public IEnumerable<byte> CurrentData { get; private set; }
    }

    public sealed class NodeChildrenChangeArgs : NodeChangeArgs
    {
        public NodeChildrenChangeArgs(string path, Watcher.Event.EventType type, IEnumerable<string> currentChildrens) : base(path, type)
        {
            CurrentChildrens = currentChildrens;
        }

        /// <summary>
        /// 当前节点的子节点数据（最新的）
        /// </summary>
        public IEnumerable<string> CurrentChildrens { get; private set; }
    }

    public delegate Task NodeDataChangeHandler(IZookeeperClient client, NodeDataChangeArgs args);

    public delegate Task NodeChildrenChangeHandler(IZookeeperClient client, NodeChildrenChangeArgs args);

    public delegate Task ConnectionStateChangeHandler(IZookeeperClient client, ConnectionStateChangeArgs args);
}