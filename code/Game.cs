namespace Tomast1337
{
	using Sandbox;

	public partial class Game : Sandbox.Game
	{
		public Game()
		{
			Precache.Add( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" );
			Precache.Add( "materials/effects/lightcookie.vtex" );

			if ( IsServer )
			{
				Log.Info( "Created in Serverside" );
				new WizzadHudEntity();
			}
			if ( IsClient )
			{
				Log.Info( "Created in Clientside" );
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
