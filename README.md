# C-sharp-Job-System-Sample

C# Job Systemのサンプルです。  
基本的にすべて同じ挙動をするはずです。  
段階的にC# Job Systemへ移行しています  

このプロジェクトは、下のブログ記事説明用です。  
[【Unity】C# Job Systemを自分なりに解説してみる](http://tsubakit1.hateblo.jp/entry/2018/03/04/223804)

![ジョブ氏のサンプル](https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/img/image.gif?raw=true)

## Bounce Cube 0 : 

C# Job Systemとか全く使わない方法  
https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/Assets/Scripts/BounceCube0.cs  

![cube0](https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/img/cube0.jpg?raw=true)

## Bounce Cube 1 :

**RaycastCommand** を使用してRaycast部分を並列化  
https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/Assets/Scripts/BounceCube1.cs  

![cube1](https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/img/cube1.jpg?raw=true)

## Bounce Cube 2 : 

**IJobParallelFor** を使用して、Cubeがバウンドする部分の速度制御を並列化  
https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/Assets/Scripts/BounceCube2.cs  

![cube2](https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/img/cube2.jpg?raw=true)

## Bounce Cube 3 : 

**IJobParallelForTransform** を使用して、Cubeのtransformが動く部分をjob化  
https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/Assets/Scripts/BounceCube3.cs  

![cube3](https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/img/cube3.jpg?raw=true)


## Bounce Cube 4 : 

追加機能。
**IJob** を使用して、地面に接触してるCubeがあるかどうかを判定する処理をJob化  
https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/Assets/Scripts/BounceCube4.cs  

![cube4](https://github.com/tsubaki/C-sharp-Job-System-Sample/blob/master/img/cube4.jpg?raw=true)
