## Rabbit ZooKeeper Extensions

该项目使用了 [Apache ZooKeeper .NET async Client](https://www.nuget.org/packages/ZooKeeperNetEx/) 组件，除提供了基本的zk操作还额外封装了常用的功能以更方便.net开发者更好的使用zookeeper。
## 提供的功能

1. session过期重连
2. 永久watcher
3. 递归删除节点
4. 递归创建节点
5. 跨平台（支持.net core）

## 使用说明
### 创建连接

    var client = new ZookeeperClient(new ZookeeperClientOptions
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
    await client.CreateEphemeralAsync("/year", Encoding.UTF8.GetBytes("2016"));
    await client.CreateEphemeralAsync("/year", Encoding.UTF8.GetBytes("2016"), ZooDefs.Ids.OPEN_ACL_UNSAFE);

    await client.CreatePersistentAsync("/year", Encoding.UTF8.GetBytes("2016"));
    await client.CreatePersistentAsync("/year", Encoding.UTF8.GetBytes("2016"), ZooDefs.Ids.OPEN_ACL_UNSAFE);

    await client.CreateAsync("/year", Encoding.UTF8.GetBytes("2016"), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT_SEQUENTIAL);
    
    //递归创建
    await client.CreateRecursiveAsync("/microsoft/netcore/aspnet", Encoding.UTF8.GetBytes("1.0.0"), CreateMode.PERSISTENT);
### 获取节点数据
    var data = await client.GetDataAsync("/year");
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
