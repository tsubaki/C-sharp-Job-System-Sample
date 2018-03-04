using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public class BounceCube1 : MonoBehaviour 
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
		var commands = new NativeArray<RaycastCommand>(targets.Length, Allocator.TempJob);
		var results = new NativeArray<RaycastHit>(targets.Length, Allocator.Temp);

		for(int i=0; i<targets.Length; i++)
		{
			var targetPosition = targets[i].position;
			var direction = Vector3.down;
			var command = new RaycastCommand(targetPosition, direction);
			commands[i] = command;
		}

		RaycastCommand.ScheduleBatch(commands, results, 20).Complete();
		commands.Dispose();

		for(int i=0; i<targets.Length; i++)
		{
			if(	velocity[i] < 0 && results[i].distance < 0.5f)
				velocity[i] = 2;
			velocity[i] -= 0.098f;
		}
		results.Dispose();

		for(int i=0; i<targets.Length; i++)
		{
			targets[i].localPosition += Vector3.up * velocity[i];
		}
	}
}