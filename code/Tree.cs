namespace Tomast1337
{
	using Sandbox;
	using System.Threading.Tasks;

	enum CurveStates
	{
		InvalidPoint,
		ValidPoint,
		Build
	}

	abstract class CubicCurve
	{
		protected Vector3 Point0 { get; set; } = Vector3.Zero;

		protected Vector3 Point1 { get; set; } = Vector3.Zero;

		protected Vector3 Point2 { get; set; } = Vector3.Zero;

		protected Vector3 Point3 { get; set; } = Vector3.Zero;

		protected float MaxDistance, MinDistance;

		public override string ToString()
		{
			return $"P0:{Point0},P1:{Point1},P2:{Point2},P3:{Point3}";
		}

		public bool IsAllSet()
		{
			return Point0 != Vector3.Zero && Point1 != Vector3.Zero && Point2 != Vector3.Zero && Point3 != Vector3.Zero;
		}

		protected bool IsaValidDistace( Vector3 pointA, Vector3 pointB )
		{
			float distance = Vector3.DistanceBetween( pointA, pointB );
			return distance >= MaxDistance && distance <= MinDistance;
		}


		public float Length() {
			float sum = 0f;
			Vector3 previousPoint = Point0;
			Vector3 nextPoint;
			float TSize = 0.025f;
			for ( float t = 0; t < 1; t += TSize )
			{
				nextPoint = getTPoint( t );
				sum += Vector3.DistanceBetween( previousPoint, nextPoint );
				previousPoint = nextPoint;
			}
			return sum;
		}

		public Vector3 getTNormal( float t )
		{
			float tt = t * t;
			return (Point0 * (-3 * tt + 6 * t - 3) +
				Point1 * (9 * tt - 12 * t + 3) +
				Point2 * (-9 * tt + 6 * t) +
				Point3 * 3 * tt).Normal;
		}

		public Vector3 getTPoint( float t )
		{
			float ttt = t * t * t;
			float tt = t * t;
			return Point0 * (-ttt + 3 * tt - 3 * t + 1) +
				Point1 * (3 * ttt - 6 * tt + 3 * t) +
				Point2 * (-3 * ttt + 3 * tt) +
				Point3 * ttt;
		}
	}

	class Tree : CubicCurve
	{
		private static string[] models = { "models/tree/log0.vmdl_c",
		"models/tree/log1.vmdl_c",
		"models/tree/log2.vmdl_c",
		"models/tree/log3.vmdl_c"};

		public Tree(float maxDistance, float minDistance )
		{
			MaxDistance = maxDistance;
			MinDistance = minDistance;
		}

		protected void BuildSegment( float t )
		{
			Vector3 position = getTPoint( t );
			var prop = new ModelEntity();


			prop.SetModel( Rand.FromArray<string>( models ) );
			prop.Position = position;
			prop.SetupPhysicsFromModel( PhysicsMotionType.Static, false );
			Vector3 normal = getTNormal( t );
			prop.Rotation = Rotation.LookAt( normal ).RotateAroundAxis( Vector3.Up, 90 );

			prop.DeleteAsync( 60 );
			Particles.Create( "particles/smoke_spawn.vpcf", prop.Position );
			Sound.FromEntity( "tree.spawn", prop );
		}

		private async void BuildTree( Vector3 PlayerPosition )
		{
			float MaxSize = MaxDistance * 4;
			float length = Length();
			float TSize = MaxSize / length /20;
			if ( Vector3.DistanceBetween( PlayerPosition, Point0 ) < Vector3.DistanceBetween( PlayerPosition, Point3 ) )

				for ( float t = 0; t < 1; t += TSize )
				{
					BuildSegment( t );
					await Task.Delay( 25 );
				}

			else
				for ( float t = 1; t > 0; t -= TSize )
				{
					BuildSegment( t );
					await Task.Delay( 25 );
				}

			Point0 = Vector3.Zero;
			Point1 = Vector3.Zero;
			Point2 = Vector3.Zero;
			Point3 = Vector3.Zero;
		}

		public CurveStates SetNextPoint( Vector3 point, Entity Owner )
		{
			if ( Point0 == Vector3.Zero )
			{
				Point0 = point;
				return CurveStates.ValidPoint;
			}
			else if ( Point1 == Vector3.Zero )
			{
				if ( IsaValidDistace( point, Point0 ) )
				{
					Point1 = point;
					return CurveStates.ValidPoint;
				}
				else
					return CurveStates.InvalidPoint;
			}

			else if ( Point2 == Vector3.Zero )
			{
				if ( IsaValidDistace( point, Point1 ) )
				{
					Point2 = point;
					return CurveStates.ValidPoint;
				}

				else
					return CurveStates.InvalidPoint;
			}
			else if ( Point3 == Vector3.Zero )
				if ( IsaValidDistace( point, Point2 ) )
				{
					Point3 = point;
					BuildTree( Owner.Position );
					return CurveStates.Build;
				}
			return CurveStates.InvalidPoint;
		}
	}
}
