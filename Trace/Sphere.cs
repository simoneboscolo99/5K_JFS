namespace Trace;

public class Sphere : Shape
{

    public Sphere(Transformation? T = null)
        : base(T) { }

    public override HitRecord? Ray_Intersection(Ray r)
    {
        var invRay = Tr.Inverse * r;
        var originVec = invRay.Origin.To_Vec();
        var a = invRay.Dir.Squared_Norm();
        var b = 2.0f * originVec.Dot(invRay.Dir);
        var c = originVec.Squared_Norm() - 1.0f;

        var delta = b * b - 4.0f * a * c;
        
        if (delta <= 0.0f) return null;
        // delta = 0 is equal to a tangent ray, unnecessary

        var sqrtDelta = (float) Math.Sqrt(delta);
        var tmin = (float) (-b - sqrtDelta) / (2.0f * a);
        tmax = (-b + sqrt_delta) / (2.0 * a)
        
        


    }

    
    /*origin_vec = inv_ray.origin.to_vec()
    a = inv_ray.dir.squared_norm()
    b = 2.0 * origin_vec.dot(inv_ray.dir)
    c = origin_vec.squared_norm() - 1.0

    delta = b * b - 4.0 * a * c
    if delta <= 0.0:
    return None

    sqrt_delta = sqrt(delta)
    tmin = (-b - sqrt_delta) / (2.0 * a)
    tmax = (-b + sqrt_delta) / (2.0 * a)

    if (tmin > inv_ray.tmin) and (tmin < inv_ray.tmax):
    first_hit_t = tmin
    elif (tmax > inv_ray.tmin) and (tmax < inv_ray.tmax):
    first_hit_t = tmax
    else:
    return None

    hit_point = inv_ray.at(first_hit_t)
    return HitRecord(
        world_point=self.transformation * hit_point,
        normal=self.transformation * _sphere_normal(hit_point, inv_ray.dir),
        surface_point=_sphere_point_to_uv(hit_point),
        t=first_hit_t,
        ray=ray,
        material=self.material,
    ) */  
    }

      
}