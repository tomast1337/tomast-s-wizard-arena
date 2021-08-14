using Sandbox;

namespace Tomast1337
{
	public class WizzadWalkController : WalkController {
		public float Stamina { get; set; } = 0;
		public WizzadWalkController() {
			AirAcceleration = 500.0f;
			AutoJump = true;
			Stamina = 100;
			Acceleration = Acceleration * 2;
		}
		public override void Simulate() {
			base.Simulate();
			
			if ( Stamina < 100 && !(Input.Down( InputButton.Run ) && Velocity != Vector3.Zero)
				&& !Input.Down( InputButton.Jump ) && GroundEntity != null)
				Stamina += .25f;
			if ( Stamina > 100)
				Stamina += 100;
			if ( Stamina < 0 )
				Stamina += 0;
		}

		public override void CheckJumpButton()
		{
			if ( Swimming )
			{
				ClearGroundEntity();
				Velocity = Velocity.WithZ( 100 );
				return;
			}
			if ( GroundEntity == null || Stamina < 5)
				return;
			ClearGroundEntity();
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;
			float startz = Velocity.z;
			
			if ( Duck.IsActive )
				flMul *= 0.8f;
			
			Velocity = Velocity.WithZ( startz + flMul * flGroundFactor );

			//Special jump
			if ( Input.Down( InputButton.Run ) && Stamina > 10)
			{
				Velocity += new Vector3( -2 * Velocity.x/5, -2 * Velocity.y/5, 2* new Vector3( Velocity.x, Velocity.y, 0 ).Length/3 );
				Stamina -= 10f;
			}
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			Stamina -= 2.75f;
						
			AddEvent( "jump" );
		}

		public override float GetWishSpeed()
		{
			var ws = Duck.GetWishSpeed();
			if ( ws >= 0 ) return ws;
			if ( Input.Down( InputButton.Run ) && Stamina > 0 ) {
				if ( !Input.Down( InputButton.Jump ) && Velocity != Vector3.Zero)
					Stamina -= 0.25f;
				return SprintSpeed;
			} 
			if ( Input.Down( InputButton.Walk )) return WalkSpeed;
			return DefaultSpeed;
		}

	}
}
