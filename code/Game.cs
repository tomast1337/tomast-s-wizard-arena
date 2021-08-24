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
		}
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );
			
			var player = new WizzardPlayer();
			client.Pawn = player;
			
			player.Respawn();
		}
	}
}
