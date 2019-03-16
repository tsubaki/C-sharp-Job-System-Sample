using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public sealed class BounceCube4 : MonoBehaviour 
{
	[SerializeField] Transform[] targets;

	NativeArray<float> velocity;
	NativeArray<RaycastCommand> commands;
	NativeArray<RaycastHit> results;
	TransformAccessArray transformArray ;

	IsHitGroundJob hitCheckJob;

	JobHandle handle, hitcheckHandle;

	[SerializeField] CanvasGroup group;

	void OnEnable()
	{
		velocity = new NativeArray<float>(targets.Length, Allocator.Persistent);
		commands = new NativeArray<RaycastCommand>(targets.Length, Allocator.Persistent);
		results = new NativeArray<RaycastHit>(targets.Length, Allocator.Persistent);
		for(int i=0; i< targets.Length; i++)
		{
			velocity[i] = -1;
		}
		transformArray = new TransformAccessArray(targets);

		hitCheckJob = new IsHitGroundJob()
		{
			raycastResults = results,
			result = new NativeArray<int>(1, Allocator.Persistent)
		};

	}

	void OnDisable()
	{
		handle.Complete();
		hitcheckHandle.Complete();
		velocity.Dispose();
		commands.Dispose();
		results.Dispose();
		hitCheckJob.result.Dispose();
		transformArray.Dispose();
	}

	void Update()
	{
		handle.Complete();
		hitcheckHandle.Complete();

		group.alpha = hitCheckJob.result[0];

		// Raycastの開始点と位置を設定
		for(int i=0; i<transformArray.length; i++)
		{
			var targetPosition = transformArray[i].position;
			var direction = Vector3.down;
			var command = new RaycastCommand(targetPosition, direction);
			commands[i] = command;
		}

		// 移動のコマンドを設定
		var updatePositionJob = new UpdatePosition()
		{
			raycastResults = results,
			velocitys = velocity
		};

		var applyPosition = new ApplyPosition()
		{
			velocitys = velocity
		};


		// 並列処理を実行（即完了待ち）
		// 終わったらコマンドに使ったバッファは不要なので破棄
		var raycastJobHandle = RaycastCommand.ScheduleBatch(commands, results, 20);
		hitcheckHandle = hitCheckJob.Schedule(raycastJobHandle);
		var updatePositionHandle = updatePositionJob.Schedule(transformArray.length, 20, raycastJobHandle );
		handle = applyPosition.Schedule(transformArray, updatePositionHandle);

        JobHandle.ScheduleBatchedJobs();
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

    struct ApplyPosition : IJobParallelForTransform
    {
		public NativeArray<float> velocitys;

        void IJobParallelForTransform.Execute(int index, TransformAccess transform)
        {
			transform.localPosition += Vector3.up * velocitys[index];
        }
    }

    struct IsHitGroundJob : IJob
    {
		[ReadOnly] public NativeArray<RaycastHit> raycastResults;
		[WriteOnly] public NativeArray<int> result;
		
        void IJob.Execute()
        {
			for(int i=0; i<raycastResults.Length; i++)
			{
				if( raycastResults[i].distance < 1f)
				{
					result[0] = 0;
					return;
				}
			}
			result[0] = 1;
        }
    }
}