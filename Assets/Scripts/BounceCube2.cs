using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public sealed class BounceCube2 : MonoBehaviour 
{
	[SerializeField] Transform[] targets;

	NativeArray<float> velocity;

	void OnEnable()
	{
		velocity = new NativeArray<float>(targets.Length, Allocator.Persistent);
		for(int i=0; i< targets.Length; i++)
		{
			velocity[i] = -1;
		}
	}

	void OnDisable()
	{
		velocity.Dispose();
	}

	void Update()
	{
		// バッファの準備
		var commands = new NativeArray<RaycastCommand>(targets.Length, Allocator.TempJob);
		var results = new NativeArray<RaycastHit>(targets.Length, Allocator.Temp);

		// Raycastの開始点と位置を設定
		for(int i=0; i<targets.Length; i++)
		{
			var targetPosition = targets[i].position;
			var direction = Vector3.down;
			var command = new RaycastCommand(targetPosition, direction);
			commands[i] = command;
		}

		// 移動のコマンドを設定
		UpdatePosition updatePositionJob = new UpdatePosition()
		{
			raycastResults = results,
			velocitys = velocity
		};


		// 並列処理を実行（即完了待ち）
		// 終わったらコマンドに使ったバッファは不要なので破棄
		var raycastJobHandle = RaycastCommand.ScheduleBatch(commands, results, 20);
		updatePositionJob.Schedule(targets.Length, 20, raycastJobHandle ).Complete();

		commands.Dispose();
		results.Dispose();

		for(int i=0; i<targets.Length; i++)
		{
			targets[i].localPosition += Vector3.up * velocity[i];
		}
	}

    struct UpdatePosition : IJobParallelFor
    {
		[ReadOnly] public NativeArray<RaycastHit> raycastResults;
		public NativeArray<float> velocitys;

        void IJobParallelFor.Execute(int index)
        {
			if(	velocitys[index] < 0 && 
				raycastResults[index].distance < 0.5f)
			{
				velocitys[index] = 2;
			}
			velocitys[index] -= 0.098f ;
        }
    }
}