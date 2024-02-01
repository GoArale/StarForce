### 编辑器模式:

1. Launch
2. Splash: todo 公司logo闪屏
3. Preload: 加载配置/字体/协议等数据
4. ChangeScene: Scene.Menu

### 单机模式:

1. Launch
2. Splash: 公司logo闪屏
3. InitResources: 加载资源???
4. Preload: 加载配置/字体/协议等数据
5. ChangeScene: Scene.Menu

### 可更新模式:

1. 看 draw.io 流程图
2. todo 该模式下 加载对话框会报错, 暂时没搞明白 MenuForm 为什么加载 UIForm 脚本失败了

### 切换模式

1. `ProcedureSplash` 流程中决定的
2. `Unity` 编辑 `Game Framework.Builtin` 的 `Base` 组件, `Editor Resource Mode`

### 登录流程规划

1. Splash: 增加公司logo动画
2. 中间检查更新流程暂时省略
    1. 检查资源
    2. 更新资源
    3. 加载资源
    4. 校验资源
3. 拉取公告
4. 登录界面
5. 连接服务器, 进度条界面, tips等