namespace Tomast1337
{
	using Sandbox;
	using System.Threading.Tasks;

	class Curve
	{
		private Vector3 Point0 { get; set; } = Vector3.Zero;

		private Vector3 Point1 { get; set; } = Vector3.Zero;

		private Vector3 Point2 { get; set; } = Vector3.Zero;

		private Vector3 Point3 { get; set; } = Vector3.Zero;

		private int Segments;

		private float MaxDistance, MinDistance;

		public override string ToString()
		{
			return $"P0:{Point0},P1:{Point1},P2:{Point2},P3:{Point3}";
		}

		public bool IsAllSet()
		{

			return Point0 != Vector3.Zero && Point1 != Vector3.Zero && Point2 != Vector3.Zero && Point3 != Vector3.Zero;
		}

		public Curve( int segments, float maxDistance, float minDistance )
		{
			Segments = segments;
			MaxDistance = maxDistance;
			MinDistance = minDistance;
		}

		private bool IsaValidDistace( Vector3 pointA, Vector3 pointB )
		{
			float distance = Vector3.DistanceBetween( pointA, pointB );
			return distance >= MaxDistance && distance <= MinDistance;
		}

		public bool SetNextPoint( Vector3 point, Vector3 PlayerPosition )
		{
			if ( Point0 == Vector3.Zero )
			{
				Point0 = point;
				return true;
			}
			else if ( Point1 == Vector3.Zero )
			{
				if ( IsaValidDistace( point, Point0 ) )
				{
					Point1 = point;
					return true;
				}

				else
					return false;
			}

			else if ( Point2 == Vector3.Zero )
			{
				if ( IsaValidDistace( point, Point1 ) )
				{
					Point2 = point;
					return true;
				}

				else
					return false;
			}
			else if ( Point3 == Vector3.Zero )
			{
				if ( IsaValidDistace( point, Point2 ) )
				{
					Point3 = point;
					BuildCurve( PlayerPosition );
					return true;
				}

				else
					return false;
			}

			else
			{
				// Reset
				Point0 = point;
				Point1 = Vector3.Zero;
				Point2 = Vector3.Zero;
				Point3 = Vector3.Zero;
				return true;
			}
		}

		private void BuildSegment( float t )
		{
			Vector3 position = getPoint( t );
			var prop = new ModelEntity();
			prop.SetModel( "models/Tree.vmdl_c" );
			prop.Position = position;
			prop.SetupPhysicsFromModel( PhysicsMotionType.Static, false );
			Vector3 normal = getNormal( t );
			prop.Rotation = Rotation.LookAt( new Vector3( normal.x, normal.y ) ).RotateAroundAxis( Vector3.Up, 90 );
			prop.DeleteAsync( 30 );
			Particles.Create( "particles/explosion_smoke.vpcf", prop.Position );
			Sound.FromEntity( "tree.spawn", prop );
		}

		private async void BuildCurve( Vector3 PlayerPosition )
		{

			float TSize = 0.025f;
			if ( Vector3.DistanceBetween( PlayerPosition, Point0 ) < Vector3.DistanceBetween( PlayerPosition, Point3 ) )

				for ( float t = 0; t < 1; t += TSize )
				{
					BuildSegment( t );
					await Task.Delay( 50 );
				}

			else
				for ( float t = 1; t > 0; t -= TSize )
				{
					BuildSegment( t );
					await Task.Delay( 50 );
				}
		}

		private Vector3 getNormal( float t )
		{
			float tt = t * t;
			return (Point0 * (-3 * tt + 6 * t - 3) +
				Point1 * (9 * tt - 12 * t + 3) +
				Point2 * (-9 * tt + 6 * t) +
				Point3 * 3 * tt).Normal;
		}

		private Vector3 getPoint( float t )
		{
			float ttt = t * t * t;
			float tt = t * t;
			return Point0 * (-ttt + 3 * tt - 3 * t + 1) +
				Point1 * (3 * ttt - 6 * tt + 3 * t) +
				Point2 * (-3 * ttt + 3 * tt) +
				Point3 * ttt;
		}
	}
}
