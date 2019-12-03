# MirrorCaster [![Build status](https://ci.appveyor.com/api/projects/status/lbn627rj6fw20gvd/branch/master?svg=true)](https://ci.appveyor.com/project/TGSAN/mirrorcaster/branch/master)
开源、高效、低延迟的Android投屏工具  
# Demo
![Demo animation](https://raw.githubusercontent.com/TGSAN/MirrorCaster/master/images/demo.webp)  
* 低延迟高质量的投屏效果  
* 1920x1080分辨率下轻松达到60FPS  
* 支持网络ADB投屏传输  
* 投屏码率可控  
# 性能
显示延迟测量方法：UFOTEST中丢帧测试进行投屏，使用慢动作摄影，随机选取拍摄视频的三帧画面，将计算机画面与手机画面格数取相差格数的绝对值乘每帧持续时间（1000/FPS）。
## Xiaomi MIX 3
* SOC: SDM845 (Qualcomm Snapdragon 845)
* OS: MIUI11 Base Android 10
* Resolution: 1080 x 2340
* FPS: 60
* Link: USB 2.0
> Display Lag (AVG): 28.42ms
## Google Pixel 3 XL
* SOC: SDM845 (Qualcomm Snapdragon 845)
* OS: Android 10
* Resolution: 1440 x 2960
* FPS: 60
* Link: USB 2.0
> Display Lag (AVG): 57.38ms
## Xiaomi MIX 2
* SOC: MSM8998 (Qualcomm Snapdragon 835)
* OS: MIUI11 Base Android 9
* Resolution: 1080 x 2160
* FPS: 60
* Link: USB 2.0
> Display Lag (AVG): 41.66ms
# 依赖
.Net Framework 4.5  
adb - Android Debug Bridge  
mpv - a free, open source, and cross-platform media player  
