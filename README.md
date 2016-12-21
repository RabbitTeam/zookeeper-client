## Rabbit ZooKeeper Extensions

该项目使用了 [Apache ZooKeeper .NET async Client](https://www.nuget.org/packages/ZooKeeperNetEx/) 组件，除提供了基本的zk操作，还额外封装了常用的功能以便让.net开发者更好的使用zookeeper。
## 提供的功能

1. session过期重连
2. 永久watcher
3. 递归删除节点
4. 递归创建节点
5. 跨平台（支持.net core）

## 使用说明
### 创建连接

    IZookeeperClient client = new ZookeeperClient(new ZookeeperClientOptions
            {
                ConnectionString = "172.18.20.132:2181",
                BasePath = "/", //default value
                ConnectionTimeout = TimeSpan.FromSeconds(10), //default value
                SessionTimeout = TimeSpan.FromSeconds(20), //default value
                OperatingTimeout = TimeSpan.FromSeconds(60), //default value
                ReadOnly = false, //default value
                SessionId = 0, //default value
                SessionPasswd = null //default value
            });
### 创建节点
    var data = Encoding.UTF8.GetBytes("2016");
    
    //快速创建临时节点
    await client.CreateEphemeralAsync("/year", data);
    await client.CreateEphemeralAsync("/year", data, ZooDefs.Ids.OPEN_ACL_UNSAFE);
    
    //快速创建永久节点
    await client.CreatePersistentAsync("/year", data);
    await client.CreatePersistentAsync("/year", data, ZooDefs.Ids.OPEN_ACL_UNSAFE);
    
    //完整调用
    await client.CreateAsync("/year", data, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT_SEQUENTIAL);
    
    //递归创建
    await client.CreateRecursiveAsync("/microsoft/netcore/aspnet", Encoding.UTF8.GetBytes("1.0.0"), CreateMode.PERSISTENT);
### 获取节点数据
    IEnumerable<byte> data = await client.GetDataAsync("/year");
    Encoding.UTF8.GetString(data.ToArray());
### 获取子节点
    IEnumerable<string> children= await client.GetChildrenAsync("/microsoft");
### 判断节点是否存在
    bool exists = await client.ExistsAsync("/year");
### 删除节点
    await client.DeleteAsync("/year");

    //递归删除
    bool success = await client.DeleteRecursiveAsync("/microsoft");
### 更新数据
    Stat stat = await client.SetDataAsync("/year", Encoding.UTF8.GetBytes("2017"));
### 订阅数据变化
    await client.SubscribeDataChange("/year", (ct, args) =>
    {
        IEnumerable<byte> currentData = args.CurrentData;
        string path = args.Path;
        Watcher.Event.EventType eventType = args.Type;
        return Task.CompletedTask;
    });
### 订阅子节点变化
    await client.SubscribeChildrenChange("/microsoft", (ct, args) =>
    {
        IEnumerable<string> currentChildrens = args.CurrentChildrens;
        string path = args.Path;
        Watcher.Event.EventType eventType = args.Type;
        return Task.CompletedTask;
    });
## FAQ
### 什么时候会触发 "SubscribeDataChange" 事件 ?
在以下情况下会触发通过 "SubscribeDataChange" 方法订阅的事件：

1. 节点被创建
2. 节点被删除
3. 节点数据发生改变
4. zk连接重连成功

### 什么时候会触发 "SubscribeChildrenChange" 事件 ?
在以下情况下会触发通过 "SubscribeChildrenChange" 方法订阅的事件：

1. 节点被创建
2. 节点被删除
3. 节点子节点发生改变
4. zk连接重连成功

### 如何在 "xxxxChange" 事件中区分节点的状态 ?
在事件触发参数会有个类型为 "EventType" 的属性 "Type"，通过该属性可以清楚的区分出节点变更的原因。

### 为什么要写这个程序，它与 "ZooKeeperEx" 有什么区别 ?
官方提供的组件，只提供了基本的api，在正常的zk使用情景中需要做非常复杂的事情，滋生出很多额外的代码并且不能保证其执行的正确性。

在java语言中也有对官方zk进行封装的包 ZKClient，当前组件也是参考了这个项目。具体组件包提供了什么功能请参考 "提供的功能" 这一节。
