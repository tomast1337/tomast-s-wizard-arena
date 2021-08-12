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
		
		private void spawnRagdoll(Vector3 velocity) {
			ModelEntity ragdoll = new ModelEntity();
			ragdoll.Position = Position;
			ragdoll.Rotation = Rotation;
			ragdoll.Scale = Scale;
			ragdoll.MoveType = MoveType.Physics;
			ragdoll.UsePhysicsCollision = true;
			ragdoll.EnableAllCollisions = true;
			ragdoll.CollisionGroup = CollisionGroup.Debris;
			ragdoll.SetModel( GetModelName() );
			ragdoll.CopyBonesFrom( this );
			ragdoll.CopyBodyGroups( this );
			ragdoll.CopyMaterialGroup( this );
			ragdoll.TakeDecalsFrom( this );
			ragdoll.EnableHitboxes = true;
			ragdoll.EnableAllCollisions = true;
			ragdoll.SurroundingBoundsMode = SurroundingBoundsType.Physics;
			ragdoll.RenderColorAndAlpha = RenderColorAndAlpha;
			ragdoll.PhysicsGroup.Velocity = velocity;

			ragdoll.SetInteractsAs( CollisionLayer.Debris );
			ragdoll.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			ragdoll.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

			foreach ( Entity child in Children )
			{
				if ( child is ModelEntity ent )
				{
					string model = ent.GetModelName();

					if ( model != null && !model.Contains( "clothes" ) )
					{
						continue;
					}

					ModelEntity clothing = new ModelEntity();
					clothing.SetModel( model );
					clothing.SetParent( ragdoll, true );
					clothing.RenderColorAndAlpha = ent.RenderColorAndAlpha;
				}
			}

			Corpse = ragdoll;

			ragdoll.DeleteAsync( 30 );

		}

		public override void OnKilled()
		{
			base.OnKilled();
			spawnRagdoll( Velocity );
			Camera = new SpectateRagdollCamera();
			Controller = null;
			EnableAllCollisions = false;
			EnableDrawing = false;
			
		}

	}
}
