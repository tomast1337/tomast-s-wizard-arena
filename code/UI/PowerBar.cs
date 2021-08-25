using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Tomast1337 {

	internal class PowerBar : Panel
	{
		public Label FireLabel;
		public Label EarthLabel;
		public Label LightningLabel;
		public Label LifeLabel;

		public PowerBar()
		{
			FireLabel = Add.Label( "🔥", "fireLabel" );
			EarthLabel = Add.Label( "🌎", "earthLabel" );
			LightningLabel = Add.Label( "⚡", "lightningLabel" );
			LifeLabel = Add.Label( "❤️", "lifeLabel" );

			FireLabel.SetClass( "active", true );

		}
		public override void Tick()
		{
			base.Tick();

			WizzardPlayer player = (WizzardPlayer)Local.Pawn;
			if ( player == null)
				return;
			var activeWeapon = player.ActiveChild as MageStaff;

			if ( activeWeapon.Fire )
				FireLabel.Style.Set( "text-shadow", "5px 5px 10px orange, -5px -5px 10px red" );
			else
				FireLabel.Style.Set( "text-shadow", "5px 5px 10px black, -5px -5px 10px black" );
			FireLabel.Style.Dirty();

			if ( activeWeapon.Earth )
				EarthLabel.Style.Set( "text-shadow", "5px 5px 10px green, -5px -5px 10px blue" );
			else
				EarthLabel.Style.Set( "text-shadow", "5px 5px 10px black, -5px -5px 10px black" );
			EarthLabel.Style.Dirty();

			if ( activeWeapon.Lightning )
				LightningLabel.Style.Set( "text-shadow", "5px 5px 10px black, -5px -5px 10px yellow" );
			else
				LightningLabel.Style.Set( "text-shadow", "5px 5px 10px black, -5px -5px 10px black" );
			LightningLabel.Style.Dirty();

			if ( activeWeapon.Life )
				LifeLabel.Style.Set( "text-shadow", "5px 5px 10px red, -5px -5px 10px pink" );
			else
				LifeLabel.Style.Set( "text-shadow", "5px 5px 10px black, -5px -5px 10px black" );
			LifeLabel.Style.Dirty();
		}
	}
}
