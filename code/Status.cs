using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Tomast1337
{
	
	
	public class Status:Panel
	{
		public Label HealthLabel;
		public Label ManaLabel;
		public Label StaminaLabel;
		public Status()
		{
			HealthLabel = Add.Label( "0", "healthLabel" );
			ManaLabel = Add.Label( "0", "manaLabel" );
			StaminaLabel = Add.Label( "0", "staminaLabel" );
		}
		public override void Tick()
		{
			WizzardPlayer player = (WizzardPlayer) Local.Pawn;
			if ( player == null )
				return;

			if ( Style.Display != DisplayMode.Flex )
			{
				Style.Display = DisplayMode.Flex;
				Style.Dirty();
			}

			WizzadWalkController wwC = (WizzadWalkController)player.Controller;
			HealthLabel.Text = $"❤{player.Health}";
			ManaLabel.Text = $"🔮{player.Mana}";
			StaminaLabel.Text = $"💪{(wwC == null?0:( int)wwC.Stamina)}";
		}
	}
}
