using Sandbox;
using System;

namespace Tomast1337
{
	public partial class WizzardPlayer : Player
	{
		public float Mana { get; set; } = 100;

		public WizzardPlayer()
		{
			
		}

		public override void Respawn()
		{
			Mana = 100;
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WizzadWalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}
		
		private void spawnRagdoll() {
			var ragdoll = new ModelEntity();
			ragdoll.SetModel( GetModelName() );
			ragdoll.Rotation = Rotation;
			ragdoll.Position = Position;
			ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			ragdoll.PhysicsGroup.Velocity = Vector3.Up * 500;
			ragdoll.DeleteAsync(30);
		}

		public override void OnKilled()
		{
			base.OnKilled();
			EnableDrawing = false;
			spawnRagdoll();
		}

	}
}
