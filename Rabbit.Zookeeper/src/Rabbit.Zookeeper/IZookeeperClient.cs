using org.apache.zookeeper;
using org.apache.zookeeper.data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rabbit.Zookeeper
{
    public interface IZookeeperClient : IDisposable
    {
        /// <summary>
        /// 具体的zookeeper连接。
        /// </summary>
        ZooKeeper ZooKeeper { get; }

        /// <summary>
        /// 客户端选项。
        /// </summary>
        ZookeeperClientOptions Options { get; }

        /// <summary>
        /// 等待zk连接到具体的某一个状态。
        /// </summary>
        /// <param name="states">希望达到的状态。</param>
        /// <param name="timeout">最长等待时间。</param>
        /// <returns>如果成功则返回true，否则返回false。</returns>
        bool WaitForKeeperState(Watcher.Event.KeeperState states, TimeSpan timeout);

        /// <summary>
        /// 重试直到zk连接上。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="callable">执行的zk操作。</param>
        /// <returns>执行结果。</returns>
        Task<T> RetryUntilConnected<T>(Func<Task<T>> callable);

        Task<IEnumerable<byte>> GetDataAsync(string path);

        Task<IEnumerable<string>> GetChildrenAsync(string path);

        Task<bool> ExistsAsync(string path);

        Task CreateAsync(string path, byte[] data, List<ACL> acls, CreateMode createMode);

        Task<Stat> SetDataAsync(string path, byte[] data, int version = -1);

        Task DeleteAsync(string path, int version = -1);

        Task SubscribeDataChange(string path, NodeDataChangeHandler listener);

        void UnSubscribeDataChange(string path, NodeDataChangeHandler listener);

        void SubscribeStatusChange(ConnectionStateChangeHandler listener);

        void UnSubscribeStatusChange(ConnectionStateChangeHandler listener);

        Task<IEnumerable<string>> SubscribeChildrenChange(string path, NodeChildrenChangeHandler listener);

        void UnSubscribeChildrenChange(string path, NodeChildrenChangeHandler listener);
    }

    public static class ZookeeperClientExtensions
    {
        public static Task CreateEphemeralAsync(this IZookeeperClient client, string path, byte[] data)
        {
            return client.CreateEphemeralAsync(path, data, ZooDefs.Ids.OPEN_ACL_UNSAFE);
        }

        public static Task CreateEphemeralAsync(this IZookeeperClient client, string path, byte[] data, List<ACL> acls)
        {
            return client.CreateAsync(path, data, acls, CreateMode.EPHEMERAL);
        }

        public static Task CreatePersistentAsync(this IZookeeperClient client, string path, byte[] data)
        {
            return client.CreatePersistentAsync(path, data, ZooDefs.Ids.OPEN_ACL_UNSAFE);
        }

        public static Task CreatePersistentAsync(this IZookeeperClient client, string path, byte[] data, List<ACL> acls)
        {
            return client.CreateAsync(path, data, acls, CreateMode.PERSISTENT);
        }

        public static async Task<bool> DeleteRecursiveAsync(this IZookeeperClient client, string path)
        {
            IEnumerable<string> children;
            try
            {
                children = await client.GetChildrenAsync(path);
            }
            catch (KeeperException.NoNodeException)
            {
                return true;
            }

            foreach (string subPath in children)
            {
                if (!await client.DeleteRecursiveAsync(path + "/" + subPath))
                {
                    return false;
                }
            }
            await client.DeleteAsync(path);
            return true;
        }

        public static Task CreateRecursiveAsync(this IZookeeperClient client, string path, byte[] data, CreateMode createMode)
        {
            return client.CreateRecursiveAsync(path, data, ZooDefs.Ids.OPEN_ACL_UNSAFE, createMode);
        }

        public static async Task CreateRecursiveAsync(this IZookeeperClient client, string path, byte[] data, List<ACL> acls, CreateMode createMode)
        {
            try
            {
                await client.CreateAsync(path, data, acls, createMode);
            }
            catch (KeeperException.NodeExistsException)
            {
            }
            catch (KeeperException.NoNodeException)
            {
                var parentDir = path.Substring(0, path.LastIndexOf('/'));
                await CreateRecursiveAsync(client, parentDir, null, acls, CreateMode.PERSISTENT);
                await client.CreateAsync(path, data, acls, createMode);
            }
        }

        /// <summary>
        /// 等待直到zk连接成功，超时时间为zk选项中的操作超时时间配置值。
        /// </summary>
        /// <param name="client">zk客户端。</param>
        public static void WaitForRetry(this IZookeeperClient client)
        {
            client.WaitUntilConnected(client.Options.OperatingTimeout);
        }

        /// <summary>
        /// 等待直到zk连接成功。
        /// </summary>
        /// <param name="client">zk客户端。</param>
        /// <param name="timeout">最长等待时间。</param>
        /// <returns>如果成功则返回true，否则返回false。</returns>
        public static bool WaitUntilConnected(this IZookeeperClient client, TimeSpan timeout)
        {
            return client.WaitForKeeperState(Watcher.Event.KeeperState.SyncConnected, timeout);
        }
    }
}