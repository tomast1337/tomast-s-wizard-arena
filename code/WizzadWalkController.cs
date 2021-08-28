namespace Tomast1337
{
	using Sandbox;

	[Library]
	public partial class WizzadWalkController : WalkController
	{
		[Net, Predicted] public float Stamina { get; set; } = 0;

		public WizzadWalkController()
		{
			AirAcceleration = 500.0f;
			AutoJump = true;
			Stamina = 100;
			Acceleration = Acceleration * 2;
		}

		public override void Simulate()
		{
			base.Simulate();

			if ( Stamina < 100 && !(Input.Down( InputButton.Run ) && Velocity != Vector3.Zero)
				&& !Input.Down( InputButton.Jump ) && GroundEntity != null )
				Stamina += .25f;
			if ( Stamina > 100 )
				Stamina = 100;
			if ( Stamina < 0 )
				Stamina = 0;
		}

		public override void CheckJumpButton()
		{
			if ( Swimming )
			{
				ClearGroundEntity();
				Velocity = Velocity.WithZ( 100 );
				return;
			}
			if ( GroundEntity == null || Stamina < 5 )
				return;
			ClearGroundEntity();
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;
			float startz = Velocity.z;

			if ( Duck.IsActive )
				flMul *= 0.8f;

			Velocity = Velocity.WithZ( startz + flMul * flGroundFactor );

			WizzardPlayer player = (WizzardPlayer)Client.Pawn;
			var activeWeapon = player.ActiveChild as MageStaff;
			if ( activeWeapon == null )
				return;
			//Special jump
			if ( Input.Down( InputButton.Run ) && Stamina > 6.4f && activeWeapon.Mana > 1.2f )
			{
				float horizontalVelocity = new Vector3( Velocity.x, Velocity.y, 0 ).Length;
				float maximunHV = 400;
				Velocity += new Vector3( Velocity.x / 4,
										 Velocity.y / 4,
										 horizontalVelocity > maximunHV ? maximunHV : horizontalVelocity );

				Stamina -= 6.4f;
				activeWeapon.Mana -= 5f;
			}
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			Stamina -= 1.75f;

			AddEvent( "jump" );
		}

		public override float GetWishSpeed()
		{
			var ws = Duck.GetWishSpeed();
			if ( ws >= 0 ) return ws;
			if ( Input.Down( InputButton.Run ) && Stamina > 0 )
			{
				if ( !Input.Down( InputButton.Jump ) && Velocity != Vector3.Zero )
					Stamina -= 0.012f;
				return SprintSpeed;
			}
			if ( Input.Down( InputButton.Walk ) ) return WalkSpeed;
			return DefaultSpeed;
		}
	}
}
