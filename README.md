# RA2 AI 工具集
为红色警戒2、尤里的复仇（支持MOD）编写的几个小工具，包含 AI 编辑器、 ini 文件合并工具和 mix 文件读取类。

## AI 编辑器
### 特点
- [x] 支持rules文件导入（导入至当前游戏模板）
    - 支持导入阵营、国家、建筑、单位等信息
    - 支持分析TechLevel标签
- [x] 支持csf文件导入（导入至当前游戏模板）
    - 支持导入阵营、国家、建筑、单位的翻译语言
- [x] 自动填充，方便编辑
    - 关联标签、单位名等可自动填充
- [x] 快捷跳转，可在触发、作战小队、脚本、特遣部队之间自由跳转
- [x] MOD支持
    - 可通过添加自定义游戏，建立当前游戏模板副本（红色警戒2、尤里的复仇、其他支持平台），并在Data/Custom目录下生成自定义游戏模板目录
    - 在自定义游戏模板目录内，编辑scripts.xml文件可以任意添加平台支持的脚本
- [x] 触发、作战小队、特遣部队支持扩展
    - 触发：可为触发指定多个所属方、触发条件，注意滥用该扩展会造成发布时的触发条数大幅增加
    - 作战小队、特遣部队：可编辑简单、中等、困难不同难度下的关联信息（作战小队的扩展会覆盖特遣部队的扩展）
    - 【保存】时会在文件中增加额外标签信息，但不会
    - 【保存并发布时】会根据扩展标签自动生成特遣部队、作战小队和触发，输出最终文件，并删除输出文件中所有额外标签信息
- [x] 可将触发条件为“1-己方科技满足条件”的AI触发自动生成全阵营触发
- [x] AI文件对比，方便分析不同版本的区别
- [x] 支持直接编辑地图文件中的脚本、小队
- [x] 特殊脚本参数
    - 【#REF:Waypoints#】地图文件中的路径点
    - 【#REF:Scripts#】文件中在注册列表中的脚本
    - 【#REF:Teams#】文件中在注册列表中的作战小队
    - 【#REF:Countries#】导入的国家信息
    - 【#REF:BuildingsID#】导入的建筑清单
    - 【#REF:Parameters#】与另一脚本使用同种参数，使用Value2属性指定脚本号
### 界面
![](https://github.com/saralmira/ra2-AIwidgets/blob/main/pics/prtsc01.jpg)
![](https://github.com/saralmira/ra2-AIwidgets/blob/main/pics/prtsc02.jpg)
![](https://github.com/saralmira/ra2-AIwidgets/blob/main/pics/prtsc03.jpg)
![](https://github.com/saralmira/ra2-AIwidgets/blob/main/pics/prtsc04.jpg)


## ini 文件合并工具
可以方便地将多个ini合并为一个，也可以简易查看不同版本间ini文件存在的区别

## mix 文件读取类
可以读取 mix 文件
