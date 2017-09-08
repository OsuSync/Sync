# osuSync

![osuSyncIcon](Sync/Resources/osu!%20001.ico)

## [Releases下载](/releases) [CN git地址](http://git.oschina.net/remilia/osuSync) [使用帮助文档(过期)](help_readme/README.md) 

以同步弹幕到你的游戏中。

本软件使用[MIT协议](LICENSE)授权

## 1.Client

- Client是具有发送/接收功能的目标源：  
> 1. 找到属于你的游戏的Client
> 2. 设置Client所需的账号密码，开始接入程序 
> 3. 接收来自游戏对弹幕的回复，并通过接口转发到直播源  

## 2.直播源

- 从大多数直播网站获取弹幕和评论!  
> 1. 找到你所喜好的直播网站的直播源
> 2. 填写需要的账号密码，更可登录到直播源，获得弹幕发射功能
> 3. 在Client接收弹幕的回复！如果直播源支持，可以直接在游戏内回复弹幕消息！

## 3.Frameworks

- 本软件实现了一套同步框架，开发者只需要将Sync.exe或者Sync工程引入您的插件中（插件必须编译为DLL，并放入Plugins才会被程序识别，）


