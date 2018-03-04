# C-sharp-Job-System-Sample

C# Job Systemのサンプルです。
基本的にすべて同じ挙動をするはずです。
段階的にC# Job Systemへ移行しています

##Bounce Cube 0 : 

C# Job Systemとか全く使わない方法

## Bounce Cube 1 :

**RaycastCommand** を使用してRaycast部分を並列化

## Bounce Cube 2 : 

**IJobParallelFor** を使用して、Cubeがバウンドする部分の速度制御を並列化

## Bounce Cube 3 : 

**IJobParallelForTransform** を使用して、Cubeのtransformが動く部分をjob化

##Bounce Cube 4 : 

追加機能。
**IJob** を使用して、地面に接触してるCubeがあるかどうかを判定する処理をJob化
