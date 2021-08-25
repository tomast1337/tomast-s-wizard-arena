using Sandbox;

namespace Tomast1337
{
	[Library( "weapon_magestaff", Title = "MageStaff", Spawnable = true) ]
	public partial class MageStaff : BaseWeapon
	{
		public bool Fire { get; set; } = false;
		public bool Earth { get; set; } = false;
		public bool Lightning { get; set; } = false;
		public bool Life { get; set; } = false;
		public float Mana { get; set; } = 100;

		public override float PrimaryRate => 1 / 1.5f;
		public override float SecondaryRate => 2f;

		int selectedAttack = 0;

		public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";

		public override void Reload() { }

		public override void Spawn()
		{
			base.Spawn();
			CollisionGroup = CollisionGroup.Weapon;
			SetInteractsAs( CollisionLayer.Debris );
			SetModel( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" );
			Log.Info( "Staff added" );
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
			}
				
			if ( Input.Pressed( InputButton.Slot2 ) )
			{
				Earth = !Earth;
				processCurrentPower();
			}
				
			if ( Input.Pressed( InputButton.Slot3 ) ) 
			{
				Lightning = !Lightning;
				processCurrentPower();
			}
				
			if ( Input.Pressed( InputButton.Slot4 ) )
			{
				Life = !Life;
				processCurrentPower();
			}
				
		}
		
		private void ProcessMana()
		{
			if ( Mana > 100 )
				Mana = 100;
			if ( Mana < 0 )
				Mana = 0;
			if ( Mana < 100 )
				Mana += .008f;
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
			foreach ( var tr in TraceBullet( pos, pos + forward * 5000, bulletSize ) )
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
			processAttack(true);
		}
		
		public override void AttackSecondary()
		{
			processAttack( false );
		}
		
		private void processAttack(bool isPrimary)
		{
			switch ( selectedAttack )
			{
				case 0:// Base -> wind gust
					WindGust();
					break;
				case 1:// Fire -> flame thrower
					Flamethrower( isPrimary );
					break;
				case 2:// Earth -> rock boulder
					Rockboulder();
					break;
				case 3:// Fire Earth -> meteor
					Meteor();
					break;
				case 4:// Lightning -> Lightning strike
					Lightningstrike();
					break;
				case 5:// Fire Lightning -> Laser shot
					LaserShot();
					break;
				case 6:// Earth Lightning -> Fast rocks shots, Submachine gun like
					StoneSMG( isPrimary );
					break;
				case 7:// Fire Earth Lightning -> muliple rocks shots, Shotgun like
					StoneShotgun( isPrimary );
					break;
				case 8:// Life -> heal
					Heal();
					break;
				case 9:// Fire Life -> flame shield
					FlameShield();
					break;
				case 10:// Earth Life -> Rock Wall
					RockWall();
					break;
				case 11:// Fire Earth Life -> Fire wall
					FireWall();
					break;
				case 12:// Lightning Life -> force field
					ForceField();
					break;
				case 13:// Fire Lightning Life -> Vampirism Shot, Awp like , Hit or die
					VampirismShot();
					break;
				case 14:// Earth Lightning Life -> Tree Spawn
					Tree();
					break;
				case 15:// Fire Earth Lightning Life -> suicide explosion
					Suicide();
					break;
				default:
					break;
			}
		}

		void creatPushSphere() { 
		
		}

		private void WindGust()
		{

		}

		private void Flamethrower( bool isPrimary )
		{

		}

		private void Rockboulder()
		{

		}

		private void Meteor()
		{

		}

		private void Lightningstrike()
		{

		}

		private void LaserShot()
		{

		}

		private void StoneSMG( bool isPrimary )
		{
			
		}

		private void StoneShotgun( bool isPrimary )
		{
			//TODO play sound ,animation and particle system
			int quant = 8;
			float damage = 9.0f;
			float spread = .2f;
			float KnockbackPower = 500f;
			if ( !isPrimary )
			{
				quant = 16;
				damage = 7.0f;
				spread = .8f;
				
				TimeSinceSecondaryAttack = -0.7f;
				TimeSincePrimaryAttack = -2;
				KnockbackPower = 600;
			}
			else
			{
				TimeSincePrimaryAttack = 0.5f;
				TimeSinceSecondaryAttack = 1.9f;
			}
			for ( int i = 0; i < quant; i++ ){
				ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, spread, 20.0f, damage, 3.0f );
			}
			Owner.Velocity += -Owner.EyeRot.Forward * KnockbackPower; //Knockback
		}

		private void Heal()
		{

		}

		private void FlameShield()
		{

		}

		private void RockWall()
		{

		}

		private void FireWall()
		{

		}

		private void ForceField()
		{

		}

		private void VampirismShot()
		{

		}

		private void Tree()
		{

		}

		private void Suicide()
		{
			Vector3 location = Owner.Position;
			float factor = 3 * Mana / 4;
			createExplosion( location, factor, factor );
		}
		public void createExplosion( Vector3 position, float size, float power) {
		
		
		}
		
		public override void Simulate( Client player )
		{
			base.Simulate( player );
			ProcessMana();
			ProcessSlotstButtons();
			//Log.Info( attackSum );
		}
	}
}
