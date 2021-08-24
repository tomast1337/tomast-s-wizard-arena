using Sandbox;

namespace Tomast1337
{
	partial class MageStaff : BaseCarriable
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
			Log.Info( "test" );
			base.Simulate( player );

		}

		public void AttackPrimary()
		{
			Log.Info( "Pew" );
		}
		public void AttackSecondary()
		{

			Log.Info( "Pew" );
		}

	}
}
