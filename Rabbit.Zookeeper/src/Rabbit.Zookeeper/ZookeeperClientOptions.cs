using System;

namespace Rabbit.Zookeeper
{
    public class ZookeeperClientOptions
    {
        public ZookeeperClientOptions()
        {
            ConnectionTimeout = TimeSpan.FromSeconds(10);
            SessionTimeout = TimeSpan.FromSeconds(20);
            OperatingTimeout = TimeSpan.FromSeconds(60);
            ReadOnly = false;
            SessionId = 0;
            SessionPasswd = null;
        }

        /// <summary>
        /// 连接字符串。
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 等待zookeeper连接的时间。
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; }

        /// <summary>
        /// 执行zookeeper操作的重试等待时间。
        /// </summary>
        public TimeSpan OperatingTimeout { get; set; }

        /// <summary>
        /// zookeeper会话超时时间。
        /// </summary>
        public TimeSpan SessionTimeout { get; set; }

        /// <summary>
        /// 是否只读，默认为false。
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// 会话Id。
        /// </summary>
        public long SessionId { get; set; }

        /// <summary>
        /// 会话密码。
        /// </summary>
        public byte[] SessionPasswd { get; set; }

        /// <summary>
        /// 基础路径，会在所有的zk操作节点路径上加入此基础路径。
        /// </summary>
        public string BasePath { get; set; }
    }
}