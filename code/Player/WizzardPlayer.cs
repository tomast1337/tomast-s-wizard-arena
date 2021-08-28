namespace Tomast1337
{
	using Sandbox;
	using System;

	public partial class WizzardPlayer : Player
	{
		private DamageInfo lastDamage;

		public WizzardPlayer()
		{
			Inventory = new BaseInventory( this );
		}

		public void Dress( String modelPath )
		{
			ModelEntity clothe = new ModelEntity();

			clothe.SetModel( modelPath );
			clothe.SetParent( this, true );

			clothe.EnableShadowInFirstPerson = true;
			clothe.EnableHideInFirstPerson = true;
		}

		public override void Respawn()
		{

			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WizzadWalkController();
			Animator = new StandardPlayerAnimator();
			//Camera = new ThirdPersonCamera();
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Tags.Add( "player" );

			//head
			Dress( "models/citizen_clothes/hat/hat_leathercap.vmdl" );
			//Chest
			Dress( "models/citizen_clothes/jacket/jacket_heavy.vmdl" );
			//Legs
			Dress( "models/citizen_clothes/trousers/trousers.jeans.vmdl" );
			//Feet
			Dress( "models/citizen_clothes/shoes/shoes.workboots.vmdl" );

			Inventory.Add( Library.Create<Entity>( "weapon_magestaff" ), true );

			base.Respawn();
		}

		private void spawnRagdoll( Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone )
		{
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

			if ( damageFlags.HasFlag( DamageFlags.Bullet ) ||
			 damageFlags.HasFlag( DamageFlags.PhysicsImpact ) )
			{
				PhysicsBody body = bone > 0 ? ragdoll.GetBonePhysicsBody( bone ) : null;

				if ( body != null )
				{
					body.ApplyImpulseAt( forcePos, force * body.Mass );
				}
				else
				{
					ragdoll.PhysicsGroup.ApplyImpulse( force );
				}
			}

			if ( damageFlags.HasFlag( DamageFlags.Blast ) )
			{
				if ( ragdoll.PhysicsGroup != null )
				{
					ragdoll.PhysicsGroup.AddVelocity( (Position - (forcePos + Vector3.Down * 100.0f)).Normal * (force.Length * 0.2f) );
					var angularDir = (Rotation.FromYaw( 90 ) * force.WithZ( 0 ).Normal).Normal;
					ragdoll.PhysicsGroup.AddAngularVelocity( angularDir * (force.Length * 0.02f) );
				}
			}

			Corpse = ragdoll;

			ragdoll.DeleteAsync( 30 );
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
			{
				info.Damage *= 10.0f;
			}

			lastDamage = info;

			TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

			base.TakeDamage( info );
		}

		[ClientRpc]
		public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
		{
		}

		public override void OnKilled()
		{
			base.OnKilled();

			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
			PlaySound( "kersplat" );

			spawnRagdoll( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
			Camera = new SpectateRagdollCamera();
			Controller = null;
			EnableAllCollisions = false;
			EnableDrawing = false;
			Inventory.DeleteContents();
		}

		public override void Simulate( Client cl )
		{
			SimulateActiveChild( cl, ActiveChild );
			base.Simulate( cl );
		}
	}
}
