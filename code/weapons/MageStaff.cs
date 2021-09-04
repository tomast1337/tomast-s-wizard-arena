namespace Tomast1337
{
	using Sandbox;

	[Library( "weapon_magestaff", Title = "MageStaff", Spawnable = true )]
	public partial class MageStaff : BaseWeapon
	{
		// Elements
		public bool Fire { get; set; } = false;

		public bool Earth { get; set; } = false;

		public bool Lightning { get; set; } = false;

		public bool Life { get; set; } = false;

		int selectedAttack = 0;

		// Mana / Ammo
		public float Mana { get; set; } = 100;

		// Flashligh stuff
		bool FlashlightOn = false;

		protected virtual Vector3 LightOffset => Vector3.Forward * 10;

		private SpotLightEntity worldLight;

		private SpotLightEntity viewLight;

		// fire rate
		public override float PrimaryRate => 1 / 1.5f;

		public override float SecondaryRate => 2f;

		public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";

		// tree
		Tree curve;

		public override void Reload()
		{
		}

		private SpotLightEntity CreateLight()
		{
			var light = new SpotLightEntity
			{
				Enabled = true,
				DynamicShadows = true,
				Range = 1024,
				Falloff = 1.0f,
				LinearAttenuation = 0.0f,
				QuadraticAttenuation = 1.0f,
				Brightness = 3,
				Color = Color.White,
				InnerConeAngle = 20,
				OuterConeAngle = 40,
				FogStength = 0.8f,
				Owner = Owner,
				LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
			};

			return light;
		}

		public override void Spawn()
		{
			base.Spawn();
			CollisionGroup = CollisionGroup.Weapon;
			SetInteractsAs( CollisionLayer.Debris );
			SetModel( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" );

			worldLight = CreateLight();
			worldLight.SetParent( this, "slide", new Transform( LightOffset ) );
			worldLight.EnableHideInFirstPerson = true;
			worldLight.Enabled = FlashlightOn;

			curve = new Tree( 256, 1536 );
		}

		public override void CreateViewModel()
		{
			base.CreateViewModel();

			viewLight = CreateLight();
			viewLight.SetParent( ViewModelEntity, "light", new Transform( LightOffset ) );
			viewLight.EnableViewmodelRendering = true;
			viewLight.Enabled = FlashlightOn;
		}

		private void processCurrentPower()
		{
			selectedAttack = 0;
			if ( Fire ) selectedAttack += 1;
			if ( Earth ) selectedAttack += 2;
			if ( Lightning ) selectedAttack += 4;
			if ( Life ) selectedAttack += 8;
		}

		private void ProcessSlotstButtons()
		{
			if ( Input.Pressed( InputButton.Slot1 ) )
			{
				Fire = !Fire;
				processCurrentPower();
				if ( Fire )
					Sound.FromEntity( "staff.fireenable", this );
				else
					Sound.FromEntity( "staff.powerdown", this );
			}

			if ( Input.Pressed( InputButton.Slot2 ) )
			{
				Earth = !Earth;
				processCurrentPower();
				if ( Earth )
					Sound.FromEntity( "staff.rockenable", this );
				else
					Sound.FromEntity( "staff.powerdown", this );
			}

			if ( Input.Pressed( InputButton.Slot3 ) )
			{
				Lightning = !Lightning;
				processCurrentPower();
				if ( Lightning )
					Sound.FromEntity( "staff.lightningenable", this );
				else
					Sound.FromEntity( "staff.powerdown", this );
			}

			if ( Input.Pressed( InputButton.Slot4 ) )
			{
				Life = !Life;
				processCurrentPower();
				if ( Life )
					Sound.FromEntity( "staff.lifeenable", this );
				else
					Sound.FromEntity( "staff.powerdown", this );
			}
		}

		private void ProcessMana()
		{
			if ( Mana > 100 )
				Mana = 100;
			if ( Mana < 0 )
				Mana = 0;
			if ( Mana < 100 )
				Mana += .08f;
		}

		public override bool CanPrimaryAttack()
		{
			if ( Owner.Health <= 0 )
				return false;

			return base.CanPrimaryAttack();
		}

		public override bool CanSecondaryAttack()
		{
			if ( Owner.Health <= 0 )
				return false;

			return base.CanSecondaryAttack();
		}

		public virtual void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
		{
			var forward = dir;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;
			foreach ( var tr in TraceBullet( pos, pos + forward * 50000, bulletSize ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;
				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}
		}

		public override void AttackPrimary()
		{
			processAttack( true );
		}

		public override void AttackSecondary()
		{
			processAttack( false );
		}

		private void processAttack( bool isPrimary )
		{
			switch ( selectedAttack )
			{
				case 0:// Base -> wind gust
					if ( Mana > 12f )
					{
						WindGust( isPrimary );
					}
					break;
				case 1:// Fire -> flame thrower
					if ( Mana > 0 )
					{
						Flamethrower( isPrimary );
					}
					break;
				case 2:// Earth -> rock boulder
					if ( Mana > 0 )
					{
						Rockboulder( isPrimary );
					}
					break;
				case 3:// Fire Earth -> meteor
					if ( Mana > 0 )
					{
						Meteor( isPrimary );
					}
					break;
				case 4:// Lightning -> Lightning strike
					if ( Mana > 0 )
					{
						Lightningstrike( isPrimary );
					}
					break;
				case 5:// Fire Lightning -> Laser shot
					if ( Mana > 0 )
						LaserShot( isPrimary );
					break;
				case 6:// Earth Lightning -> Fast rocks shots, Submachine gun like
					if ( Mana > 0 )
					{
						StoneSMG( isPrimary );
					}
					break;
				case 7:// Fire Earth Lightning -> muliple rocks shots, Shotgun like
					if ( Mana > 10 )
					{
						StoneShotgun( isPrimary );
					}
					break;
				case 8:// Life -> heal
					if ( Mana > 0 )
					{
						Heal( isPrimary );
					}
					break;
				case 9:// Fire Life -> flame shield
					if ( Mana > 0 )
					{
						FlameShield( isPrimary );
					}
					break;
				case 10:// Earth Life -> Rock Wall
					if ( Mana > 0 )
					{
						RockWall( isPrimary );
					}
					break;
				case 11:// Fire Earth Life -> Fire wall
					if ( Mana > 0 )
					{
						FireWall( isPrimary );
					}
					break;
				case 12:// Lightning Life -> force field
					if ( Mana > 0 )
					{
						ForceField( isPrimary );
					}
					break;
				case 13:// Fire Lightning Life -> Vampirism Shot, Awp like , Hit or die
					if ( Mana > 0 )
					{
						VampirismShot( isPrimary );
					}
					break;
				case 14:// Earth Lightning Life -> Tree Spawn
					if ( Mana > 80 )
					{
						Tree( isPrimary );
					}
					break;
				case 15:// Fire Earth Lightning Life -> suicide explosion
					if ( Mana > 50 )
					{
						Suicide( isPrimary );
					}
					break;
				default:
					break;
			}
		}

		void creatPushSphere()
		{
		}

		private void WindGust( bool isPrimary )
		{
			float manacost = 12f;

			if ( isPrimary )
			{
				TimeSinceSecondaryAttack = -1f; // Slow fire rate
				TimeSincePrimaryAttack = -1f; // Slow fire rate
			}
			else
			{
				Sound.FromEntity( "staff.windselfpush", this );
				Owner.Velocity += Owner.EyeRot.Forward * 800; //Knockback
				TimeSinceSecondaryAttack = -1f; // Slow fire rate
				TimeSincePrimaryAttack = -1f; // Slow fire rate
			}

			Mana -= manacost;
		}

		private void Flamethrower( bool isPrimary )
		{
			if ( !isPrimary )
			{
				TimeSinceSecondaryAttack = -1f; // Slow fire rate
				TimeSincePrimaryAttack = .35697f; // penalty for changinf fire type
			}
			else
			{
				TimeSinceSecondaryAttack = -0.2f; // penalty for changing fire type
				TimeSincePrimaryAttack = 1.5f; // Fast fire rate
			}

			var forward = Owner.EyeRot.Forward;
			forward = forward.Normal;

			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 250, 20.0f ) )
			{
				if ( !tr.Entity.IsValid() ) continue;

				//tr.Surface.DoBulletImpact( tr );

				if ( !IsServer ) continue;

				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100, 1 )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}
			Particles.Create( "particles/flame.vpcf", EffectEntity, "muzzle" );
		}

		void lauchProp( string model, float force )
		{
			var prop = new ModelEntity();
			prop.SetModel( model );
			prop.Position = Owner.EyePos + Owner.EyeRot.Forward * 40;
			prop.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			prop.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			prop.PhysicsGroup.Velocity = Owner.EyeRot.Forward * force;
			prop.DeleteAsync( 10 );
		}

		private void Rockboulder( bool isPrimary )
		{
			lauchProp( "models/rust_props/wooden_crates/wooden_crate_a.vmdl_c", 1000 );
		}

		private void Meteor( bool isPrimary )
		{
			lauchProp( "models/citizen/citizen.vmdl", 1000 );
		}

		private void Lightningstrike( bool isPrimary )
		{
			if ( !isPrimary )
			{

			}
			else
			{
				TimeSinceSecondaryAttack = -1f;
				ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, .1f, 20.0f, 50, 3.0f );
				new Sandbox.ScreenShake.Random( 0.1f, 1, 1 );
			}
		}

		private void LaserShot( bool isPrimary )
		{
			float manaCost = 0.01f;

			float damage = 1.0f;
			float spread = .01f;
			float force = 20.0f;


			if ( !isPrimary )
			{
				manaCost = 1f;
				TimeSinceSecondaryAttack = -1f; // Slow fire rate
				TimeSincePrimaryAttack = .35697f; // penalty for changinf fire type
			}
			else
			{
				TimeSinceSecondaryAttack = -0.2f; // penalty for changing fire type
				TimeSincePrimaryAttack = 1.5f; // Fast fire rate

			}
			ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, spread, force, damage, 3.0f );

			Mana -= manaCost;
		}

		private void StoneSMG( bool isPrimary )
		{
			float manacost = .1f;
			float damage = 3.0f;
			float spread = .1f;

			float length = .5f;
			float speed = 5.0f;
			float size = .5f;

			if ( !isPrimary )
			{
				manacost = .5f;

				damage = 5.0f;
				spread = 0.02f;

				length = .5f;
				speed = 5.0f;
				size = 3f;

				TimeSinceSecondaryAttack = -1f; // Slow fire rate
				TimeSincePrimaryAttack = .35697f; // penalty for changinf fire type
			}
			else
			{
				TimeSinceSecondaryAttack = -0.2f; // penalty for changing fire type
				TimeSincePrimaryAttack = 1.35697f; // Fast fire rate
			}
			ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, spread, 20.0f, damage, 3.0f );

			new Sandbox.ScreenShake.Perlin( length, speed, size );

			Mana -= manacost;
		}

		private void StoneShotgun( bool isPrimary )
		{
			float manacost = 5;
			int quant = 10;
			float damage = 3f;
			float spread = .1f;
			float KnockbackPower = 280f;
			float force = 10;

			float length = 1.0f;
			float speed = 1.5f;
			float size = 2.0f;


			if ( !isPrimary )
			{
				manacost = 18;

				quant = 20;
				damage = 2f;
				spread = .4f;
				KnockbackPower = 600;
				force = 20f;

				length = 3.0f;
				speed = 3.0f;
				size = 3.0f;

				TimeSinceSecondaryAttack = -1f; // Slow fire rate
				TimeSincePrimaryAttack = 1;  // penalty for changing fire type
			}

			else
			{

				TimeSinceSecondaryAttack = -1.9f; // penalty for changing fire type
				TimeSincePrimaryAttack = 0.5f; // Not so slow fire rate 
			}

			for ( int i = 0; i < quant; i++ )
			{
				ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, spread, force, damage, 3.0f );
			}

			Owner.Velocity += -Owner.EyeRot.Forward * KnockbackPower; //Knockback

			if ( IsLocalPawn )
			{
				new Sandbox.ScreenShake.Perlin( length, speed, size );
			}

			Mana -= manacost;
		}

		private void Heal( bool isPrimary )
		{
			TimeSinceSecondaryAttack = -1f; // Slow fire rate
		}

		private void FlameShield( bool isPrimary )
		{
		}

		private void RockWall( bool isPrimary )
		{
			var forward = Owner.EyeRot.Forward;
			forward = forward.Normal;
			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 2*512/3, 20.0f ) )
			{

				if ( tr.Hit )
				{
					Vector3 pos = tr.EndPos;
					if ( Vector3.DistanceBetween( pos, Owner.Position ) > 128 )
					{
						var prop = new ModelEntity();
						prop.SetModel( "models/RockWall/rockwall.vmdl_c" );
						prop.Position = pos;
						prop.Rotation = Rotation.LookAt( forward );
						prop.SetupPhysicsFromModel( PhysicsMotionType.Static, false );
						prop.DeleteAsync( 60 );
						Particles.Create( "particles/smoke_spawn.vpcf", Vector3.Lerp( Owner.Position, prop.Position, 0.5f ) );
						Sound.FromEntity( "rockwall.set", prop );
					}
					else {
						Sound.FromEntity( "", this );
					}
				}

			}
		}

		private void FireWall( bool isPrimary )
		{
		}

		private void ForceField( bool isPrimary )
		{
		}

		private void VampirismShot( bool isPrimary )
		{
		}

		private void Tree( bool isPrimary )
		{
			var forward = Owner.EyeRot.Forward;
			forward = forward.Normal;
			CurveStates state;
			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * 5000, 20.0f ) )
			{
				Vector3 pos = tr.EndPos;
				if ( curve != null && tr.Hit )
				{
					state = curve.SetNextPoint( pos, Owner );
					switch ( state )
					{
						case CurveStates.ValidPoint:
							DebugOverlay.Sphere( pos, 32, Color.Green, true, 5f );
							break;
						case CurveStates.InvalidPoint:
							DebugOverlay.Sphere( pos, 32, Color.Red, true, 5f );
							break;
						case CurveStates.Build:
							DebugOverlay.Sphere( pos, 32, Color.Blue, true, 5f );
							Mana = -50;
							break;
					}
				}

			}
		}

		private void Suicide( bool isPrimary )
		{
			Vector3 location = Owner.Position;
			float factor = 3 * Mana / 4;
			createExplosion( location, factor, factor );
			Mana = 0;
		}

		public void createExplosion( Vector3 position, float size, float power )
		{
			using ( Prediction.Off() )
			{
				Particles.Create( "particles/explosion.vpcf", position );
			}
		}

		private void ProcessFlashlight()
		{
			FlashlightOn = !FlashlightOn;
			PlaySound( FlashlightOn ? "flashlight-on" : "flashlight-off" );

			if ( worldLight.IsValid() )
				worldLight.Enabled = FlashlightOn;
			if ( viewLight.IsValid() )
				viewLight.Enabled = FlashlightOn;
		}

		public override void Simulate( Client player )
		{
			base.Simulate( player );
			ProcessMana();
			ProcessSlotstButtons();
			if ( Input.Pressed( InputButton.Flashlight ) )
			{
				ProcessFlashlight();
			}
		}
	}
}
