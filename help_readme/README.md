##osuSync 
----------------------------------
####使用这个软件前，你必须要有一下东西:<br>
*  osu小号(能使用irc的) <br>
*  直播间(废话) <br>


###config配置
1.	获取小号的IRC密码 
在osu官网登录你小号的账号，然后在首页进入[IRC页面](https://osu.ppy.sh/p/irc)获取IRC密码(Server Password)。
2. 打开osuSync.exe，如果是第一次打开的话它会自动跳出config.ini让你填写配置内容.如果已经写好再改，请到程序根目录手动打开config.ini。

配置文件的名称以及解释如下:
LiveRoomID : 直播间ID(比如http://live.bilibili.com/302503 的 302503就是直播间ID,不同直播源的ID可能不同)
TargetIRC : 要接收信息的大号,也就是你要正在玩的大号，直播间发来的信息将会发送到这个账号上
BotIRC : 中间人小号，用于收发信息，直播间发来的信息通过这个小号代发到你TargetIRC大号那里去
BotIRCPassword : 小号的IRC密码
Provider :  要钦定的直播源，即你直播的网址地方名称,如BiliBili,DouYu,Twitch,默认BiliBili

Certification : B站网页的cookie，如果你想在IRC发送消息到直播间就需要它,在osuSync里面用login命令即可快速获取cookie

一个完整的配置应该是这样的:
![Example](http://git.oschina.net/remilia/osuSync/raw/dpdev/help_readme/images/a.png)

###启用osuSync
如果已经配置好了，那么请在程序输入start指令.
![Alt text](http://git.oschina.net/remilia/osuSync/raw/dpdev/help_readme/images/b.png)

如果成功的话你可以发现你的小号已经登录在IRC了
![Alt text](http://git.oschina.net/remilia/osuSync/raw/dpdev/help_readme/images/c.png)

这时候你可以测试是否流通了(其实不用
![Alt text](http://git.oschina.net/remilia/osuSync/raw/dpdev/help_readme/images/d.png)
默认情况下，自己发送的消息会回流到irc，如刚才情况。如果想要禁止回流的话请在IRC里面输入:
?ban (你直播间ID名)

###账号登录
![Alt text](http://git.oschina.net/remilia/osuSync/raw/dpdev/help_readme/images/e.png)

从IRC发送弹幕到直播间需要账号登录，并提取cookie.那么就拿BILIBILI来举例，你可以在你浏览器打开B站直播首页。按F12弄到控制台(Console).输入"doucument.cookie"并回车，将出现的红色内容复制，打开config.ini，在Provider变量下面起一行写Certification=红色内容,如之前的图所示；或者你也可以直接在软件内容输入
login B站用户名 密码(这个可不写)
输入回车后会跳出一个框，然后会跳到B站登陆界面，请在那里输入账号密码和验证码，登陆，成功后自动消失。

![Alt text](http://git.oschina.net/remilia/osuSync/raw/dpdev/help_readme/images/f.png)


###关闭osuSync
别怂，右上角,或者在程序输入:
stop

###关于以及声明
1.	本程序本质是一个消息收发程序，其对OSU频道的收发均是通过IRC协议编写轮子使用。对于直播间也是通过模拟环境以及Client接受直播间弹幕信息。所以本软件并非是非法软件( <br> 
2.	本程序支持插件使用，并留有插件的接口。如果可以，你可以自己编写一个插件来给本程序添加额外功能，通常程序启动时候会自动加载程序根目录的Plugin文件夹里面的dll文件，本软件本体可以保证自身清白有源码，但来历不明插件就未必了(逃



###最后
有什么问题或者建议请在[项目](http://git.oschina.net/remilia/osuSync)提交issue或者发送[邮件](mailto:mikirasora0409@126.com)