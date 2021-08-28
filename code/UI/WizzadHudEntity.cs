namespace Tomast1337
{
	using Sandbox;
	using Sandbox.UI;

	public partial class WizzadHudEntity : HudEntity<RootPanel>
	{
		public static WizzadHudEntity Singleton;

		public WizzadHudEntity()
		{
			if ( !IsClient )
				return;
			Singleton = this;

			RootPanel.StyleSheet.Load( "/UI/Hud.scss" );

			RootPanel.AddChild<NameTags>();
			RootPanel.AddChild<CrosshairCanvas>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<Status>();
			RootPanel.AddChild<PowerBar>();
		}
	}
}
