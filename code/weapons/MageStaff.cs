using Sandbox;

namespace Tomast1337
{
	[Library( "weapon_magestaff", Title = "MageStaff") ]
	public partial class MageStaff : BaseWeapon
	{

		public override void Spawn()
		{
			base.Spawn();
			CollisionGroup = CollisionGroup.Weapon;
			SetInteractsAs( CollisionLayer.Debris );
			Log.Info( "Staff added" );
		}
		public override void Simulate( Client player )
		{
			base.Simulate( player );
			WizzardPlayer playerEnt = (WizzardPlayer)Local.Pawn;


		}

		public override void AttackPrimary()
		{
			Log.Info( "Pew" );
		}
		public override void AttackSecondary()
		{

			Log.Info( "Pew" );
		}

		private void processAttack( WizzardPlayer playerEnt )
		{
			int attackSum = 0;
			if ( playerEnt.Fire ) attackSum += 1;
			if ( playerEnt.Earth ) attackSum += 2;
			if ( playerEnt.Lightning ) attackSum += 4;
			if ( playerEnt.Life ) attackSum += 8;
			switch ( attackSum )
			{
				case 0:// Base -> wind gust
					WindGust();
					break;
				case 1:// Fire -> flame thrower
					Flamethrower();
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
					StoneSMG();
					break;
				case 7:// Fire Earth Lightning -> muliple rocks shots, Shotgun like
					StoneShotgun();
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

		private void WindGust()
		{

		}
		private void Flamethrower()
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
		private void StoneSMG()
		{

		}
		private void StoneShotgun()
		{

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

		}

	}
}
