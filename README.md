# osuSync

![osuSyncIcon](Sync/Resources/osu! 001.ico)

## [Releases下载](http://git.oschina.net/remilia/osuSync/releases) [git地址](http://git.oschina.net/remilia/osuSync) 

是的，你可以同步弹幕到osu中，同时将你的智商同步到osu里。  
本软件使用[WTFPL协议](http://git.oschina.net/remilia/osuSync/blob/master/LICENSE)授权

## 1.IRC

- IRC使用Meebey的[SmartIrc4net (on github)](https://github.com/meebey/SmartIrc4net)作为基础  
  SmartIrc4net使用[LGPL2.1](https://github.com/meebey/SmartIrc4net/blob/master/LICENSE)授权，本工程以动态链接库形式调用。  

- 针对osu!bancho进行二次IRC开发，IRC功能如下：  
> 1. 在游戏内直接/np进行指定曲目pp查询  
> 2. 接收来自直播流的弹幕，并同步到指定的IRC账号中  
> 3. 接收来自游戏对弹幕的回复，并通过接口转发到弹幕服务器  

## 2.直播源

- 弹幕获取  
    使用了copyliu的[bilibili_dm](https://github.com/copyliu/bililive_dm)开源项目，并通过源文件的形式加入工程中
    (参见: [bilibili_dm](Sync/Source/BiliBili/Bilibili_dm/DanmakuLoader.cs))  
    
    该项目以[WTFPL协议](https://github.com/copyliu/bililive_dm/blob/master/LICENSE.txt)授权

- 弹幕发送
    弹幕发送在BiliBili上直接抓取接口，发送带Cookies的HTTP Request即可发送。  
    由于BiliBili部分Cookies携带HTTPOnly属性，故此处使用WIN32API来获取存储的Cookies。

## 3.Frameworks

>  本软件实现了一套可扩展的弹幕同步框架，只需要简单的实现接口就能将弹幕同步到osu中。  
>  本软件完全开源，您可通过Pull Request提交您的实现。

### `namespace Sync.Source`   

- `ISourceBase` 接口  
    
    源服务器接收实现接口

```csharp
public interface ISourceBase
{
    event ConnectedEvt onConnected;
    // 连接成功的事件
    event DisconnectedEvt onDisconnected;
    //断开连接的事件
    event DanmukuEvt onDanmuku;
    //接收到弹幕的事件
    event GiftEvt onGift;
    //接收到礼物的事件
    event CurrentOnlineEvt onOnlineChange;
    //在线人数改变的事件

    // 获得原始类型
    Type getSourceType();
    // 开始源服务器连接
    bool Connect(int roomID);
    // 断开连接
    bool Disconnect();
    // 连接状态
    bool Stauts();
}
```


- `ISendable` 接口  
    
    当前弹幕源是支持发送弹幕时实现

```csharp
interface ISendable
{
    void Send(string str);
    void Login(string user, string password);
    bool LoginStauts();
}
```