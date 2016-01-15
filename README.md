OneClick软件离线安装
-------------------

下载微软OneClick方式部署的软件安装包到本地磁盘，方便墙内的同学安装GitHub Desktop for Windows

### 使用方法:

创建任务:
OneClickDownloader.exe CreateTask GitHubSetup http://github-windows.s3.amazonaws.com/GitHub.application

下载文件:
OneClickDownloader.exe RunTask GitHubSetup

网络不稳定的情况下部分文件可能下载失败，反复运行下载文件命令，直到提示所有文件下载完成，然后双击.application文件，安装软件

