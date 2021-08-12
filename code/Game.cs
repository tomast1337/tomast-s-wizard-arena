using Sandbox;
namespace Tomast1337 {
	public partial class Game : Sandbox.Game
	{
		public Game(){
			if(IsServer){
				Log.Info( "Created in Serverside" );
				new WizzadHudEntity();
			}
			if(IsClient){
				Log.Info("Created in Clientside");
			}
			new WizzadHudEntity();
		}
		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );
			var player = new WizzardPlayer();
			cl.Pawn = player;
			player.Respawn();
		}
	}
}
