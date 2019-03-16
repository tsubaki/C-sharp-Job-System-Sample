using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public sealed class BounceCube5 : MonoBehaviour 
{
	[SerializeField] Transform[] targets;

	NativeArray<float> velocity;
	NativeArray<RaycastCommand> commands;
    NativeArray<RaycastHit> results;
    NativeQueue<int> hitQueue;
	TransformAccessArray transformArray ;

	IsHitGroundJob hitCheckJob;

	JobHandle handle;

	[SerializeField] CanvasGroup group;

	void OnEnable()
	{
		velocity = new NativeArray<float>(targets.Length, Allocator.Persistent);
		commands = new NativeArray<RaycastCommand>(targets.Length, Allocator.Persistent);
        results = new NativeArray<RaycastHit>(targets.Length, Allocator.Persistent);
        hitQueue = new NativeQueue<int>(Allocator.Persistent);

        transform.DetachChildren();

        for (int i=0; i< targets.Length; i++)
		{
			velocity[i] = -1;
		}
		transformArray = new TransformAccessArray(targets);

		hitCheckJob = new IsHitGroundJob()
		{
			raycastResults = results,
			result = hitQueue.ToConcurrent()
        };

	}

	void OnDisable()
	{
		handle.Complete();

		velocity.Dispose();
		commands.Dispose();
		results.Dispose();
        hitQueue.Dispose();
        transformArray.Dispose();
	}

	void LateUpdate()
	{
		handle.Complete();

		// Raycastの開始点と位置を設定
		for(int i=0; i<transformArray.length; i++)
		{
			var targetPosition = transformArray[i].position;
			var direction = Vector3.down;
			var command = new RaycastCommand(targetPosition, direction);
			commands[i] = command;
		}

		// 移動のコマンドを設定
		var updatePositionJob = new UpdateVelocity()
		{
			velocitys = velocity
		};

		var applyPosition = new ApplyPosition()
		{
			velocitys = velocity
		};


        // 並列処理を実行（即完了待ち）
        // 終わったらコマンドに使ったバッファは不要なので破棄
        handle = RaycastCommand.ScheduleBatch(commands, results, 20);
        handle = hitCheckJob.Schedule(transformArray.length, 20, handle);
        handle = new ReflectionJob { velocitys = velocity, result=hitQueue }.Schedule(handle);
        handle = updatePositionJob.Schedule(transformArray.length, 20, handle);
		handle = applyPosition.Schedule(transformArray, handle);

        handle.Complete();
	}

    struct UpdateVelocity : IJobParallelFor
    {
		public NativeArray<float> velocitys;

        void IJobParallelFor.Execute(int index)
        {
			velocitys[index] -= 0.098f ;
        }
    }

    struct ApplyPosition : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float> velocitys;

        void IJobParallelForTransform.Execute(int index, TransformAccess transform)
        {
			transform.localPosition += Vector3.up * velocitys[index];
        }
    }

    struct IsHitGroundJob : IJobParallelFor
    {
		[ReadOnly] public NativeArray<RaycastHit> raycastResults;
		[WriteOnly] public NativeQueue<int>.Concurrent result;

        void IJobParallelFor.Execute(int index)
        {
            if (raycastResults[index].distance < 1f)
            {
                result.Enqueue(index);
            }
        }
    }

    struct ReflectionJob : IJob
    {
        public NativeQueue<int> result;
        public NativeArray<float> velocitys;

        public void Execute()
        {
            while(result.TryDequeue(out int index))
            {
                velocitys[index] = 2;
            }
        }
    }
}