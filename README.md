# wechat_spider_core
dotnet core开发的微信公众号历史文章爬虫

------



| 操作系统 | Win10 2004 |
| -------- | ---------- |
| IDE      | VS 2019    |
| 数据库   | PostgreSQL |



| dotnet core         | 3.1      | [3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) |
| ------------------- | -------- | ------------------------------------------------------------ |
| CefSharp            | 31.3.100 | [CefSharp](https://github.com/cefsharp/CefSharp)             |
| EntityFramewordCore | 3.1.5    |                                                              |
| NLog                | 4.7.2    |                                                              |

# 实现思路

## 准备工作

1. 注册一个公众号，个人企业无所谓，能登录微信公众平台就行。

NetCore DeskTop.WindowsForms程序内嵌CefSharp[打开微信公众平台](https://mp.weixin.qq.com/)，正常扫码登录以后，使用继承CefSharp的IResourceRequestHandlerFactory和IResourceRequestHandler监听request请求拿到token。模拟请求 [查询公众号](https://mp.weixin.qq.com/cgi-bin/searchbiz) 来获取公众号fackid，使用fackid调用 [搜索历史文章](https://mp.weixin.qq.com/cgi-bin/appmsg) 。

**需要注意的是，一个公众号每天请求文章列表次数大概在200次左右，再多就会被封掉一天。**