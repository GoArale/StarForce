### 功能列表：
* 启动界面加载
* 地图管理-解锁和锁定
* 关卡管理-解锁和锁定-和加载 
* 游戏暂停 - 继续 - 重新开始 - 返回关卡列表 
* 背景
   1. 上背景 
   2. 下背景 
   3. 草地 
* 小鸟管理
   1. 加载小鸟
* 小鸟飞行 - 拖尾效果
* 弹弓皮筋  
* 可销毁物体的碰撞 - `Collision.relitiveVelocity`
* 小猪 - 受伤
* 小鸟 - 受伤
* 爆炸效果 - 得分效果
* 游戏结果判定
  * 杀死了多少只猪猪
  * 胜利动画：1星 2星 3星   
  * 失败动画
* 镜头跟随
* 音效播放
  * 小鸟：select fly
  * 小猪：碰撞 死亡  和小鸟碰撞上
  * 背景音乐
* 小鸟分类
  * 红色：经典-无特殊功能 
  * 黄色：飞行中可以加速一次 
  * 长嘴鸟：可以掉头 
  * 黑色：可以爆炸 
* 技能分类： 
  * 只能在空中执行的 
  * 随时可以执行的 
* 游戏发布


2D SortingLayer分类
Background
Game
Foreground

知识点
```C#
1
LineRenderer - line.SetPositon(index,position)

2
collision.relativeVelocity

3-拖尾
TrailRenderer

4-2D检测一定范围的碰撞
Collider2D[] colliders = Physics2D.OverlapCircleAll(center,radius,layerMask)

5-使用ScriptObject保存数据
ScriptObject
[CreateAssetMenu()]
public List<int> a = new List<int>{ -1 };//给集合默认值

6-代码设置分辨率
Screen.SetResolution(x,x);
```


* 游戏进度-通关数据的保存：
* 地图关卡数据保存
  * 1-哪些地图和那些关卡是否通关
  * 2-通关的关卡和地图是几个星星

* 使用二维数组
  * int[7,32] mapData;
  * 第一维代表 第几地图
  * 第二维代表 第几关 
  * 数字代表是否通关  -1 未解锁 0 123

* 数据的存储使用ScriptObject

