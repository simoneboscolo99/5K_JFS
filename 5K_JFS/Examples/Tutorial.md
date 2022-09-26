# Tutorial for the Input Scene File

In this file we provide the necessary instructions to build the input file describing the scene. We advise you not to skip this tutorial! At the end of this file you will be able to understand [these](https://github.com/simoneboscolo99/5K_JFS/tree/master/5K_JFS/Examples) examples and to build your own images. 

First of all, we note that all lines starting with the `#` symbol are considered comments, hence they are just skipped by the compiler. 

## Float variables

We can define float variables as follows:
```diff
float name(value)
``` 
where `value` is a floating-point number, while `name` is just the name of the variabile. Example: `float angle(35.2)` <br/>
Defining float variables is useful for particular transformations, as we will see later.

## Materials

A material characterizes the color of an object and is declared in the following way: 
```diff
material name = (Brdf, Pigment)
```
where `name` is a just the name assigned to the material. Then we must specify the BRDF type, which in turn needs a pigment. Finally, we need a Pigment type to specify the emitted radiance.

**BRDF types: diffuse, specular**

To define each type of BRDF we have to specify a Pigment between brackets, as follows
```diff
+ Brdf(Pigment)
```
**Pigment types: uniform, checkered, image**

In order to create a Pigment, we need to know how to define a color. Colors are represented by triplets of numbers enclosed by angular brackets
```diff
+ color = <R, G, B>
```
where `R` (red color), `G` (green color) and `B` (blue color) are float numbers from 0 to infinity. Example: `<0.9, 0.5, 0.1>` <br/>
For a correct execution of the image, we recommend to use values from 0 to 1 for the three color components.

We can now report how the three different pigments are defined, giving, as always, some examples
```diff
+ uniform(color)
```
Example: `uniform(<0.9, 0.5, 0.1>)`
```diff
+ checkered(color1, color2, numOfSteps)
```
where `numOfSteps` is the number of interations (integer) of the checkered pattern. <br/> Example: `checkered(<0.9, 0.5, 0.1>, <0.1, 0.9, 0.5>, 5)`
```diff
+ image(filename)
```
where the file must be in pfm format. Example: `image("texture.pfm")`

&emsp; ⚠️ &nbsp; **If the file of the image is not in the directory where you are executing the code, you must specify the path of the file** &nbsp; ⚠️

We can now show some examples of Brdf:

`diffuse(image("texture.pfm"))` <br/>
`specular(uniform(<0.9, 0.5, 0.1>))`

Finally, we have all the elements to define materials. Let's make two examples:

`material redMaterial(diffuse(uniform(<1, 0, 0>)), uniform(<0, 0, 0>))` <br/>
`material lightMaterial(diffuse(uniform(<1, 1, 1>)), uniform(<1, 1, 1>))`

If the second pigment is uniform with color `<0, 0, 0>` (black), the object does not radiate light. <br/> Once you have created a material, you can refer to it using its name.

## Shapes

Shapes are the objects in our world. We can declare a shape in the following way:
```diff
- shapeType = (materialName, transformation) 
```
We must specify the name of the material and the transformation. This is the way to proceed for the shapes: `Sphere`, `Cylinder`, `Disk`, `Plane`. 

&emsp; ⚠️ &nbsp; **`materialName` has to be already define above, you cannot directly construct a material here** &nbsp; ⚠️

The last shape left, `Box`, requires a slightly different definition:
```diff
- box = (minPoin, maxPoint, materialName, transformation)
```
where `minPoint` and `maxPoint` are 3D points representing respectively the minimum and maximum extent of each axis of the box.

&emsp; ⚠️ &nbsp; **each component of `minPoint` must be smaller than the corresponding one in `maxPoint`** &nbsp; ⚠️

Now we need to know what transformations are allowed and how each transformation is declared

**Transformation types:  scaling, translation, rotationX, rotationY, rotationZ, identity**

In order to create a transformation, we need to know how to define a vector. Vectors are represented by triplets of numbers enclosed by squared brackets
```diff
+ vector = [X, Y, Z]
```
where `X`, `Y` and `Z` are float numbers. Example: `[10.2, -2.7, 5.4]`

We can now report how the different transformations are defined, giving, as always, some examples
```diff
+ scaling(vector)
```
Example: `scaling([10.2, -2.7, 5.4])`
```diff
+ translation(vector)
```
Example: `translation([10.2, -2.7, 5.4])`
```diff
+ rotationX(angle)
```
Similarly to `RotationX` we can obtain `rotationY` and `rotationZ`. 

&emsp; ⚠️ &nbsp; **The angle must be specified in degrees, not radians!** &nbsp; ⚠️

Example: `rotationX(35.2)` or similarly `float angle(35.2)` <br/> 
&emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &emsp; &nbsp; &thinsp; `rotationX(angle)`

We now see the utility of defining float variables: the variable `angle` could be used in different rotations of different shapes. 

Transformations can be combined via the `*` symbol. Example: `rotationY(30) * translation([-4, 0, 0])` Pay attention to the order, since some trasformations are **not** commutative. If you don't need any transformation, just write `identity`. <br/>
The transformations are fundamental because they allow you to correctly place objects in the world. Moreover, you can zoom in, out or deform the shapes; for example, you can transform the sphere into an ellipsoid by scaling. To do this correctly, we need to know the characteristics of the default shapes:

- **Sphere** 🎱: it is centered at the origin and with unit radius.
- **Plane** ⬜: xy plane, passing through the origin.
- **Cylinder** 🎩: it is an **open** cylinder centered around the z axis and with unit radius. The z-coordinate range is 0 to 1.
- **Disk** 💿: it is a circular disk of unit radius, parallel to the x and y axis and passing through the origin.
- **Box** 🎁: it has faces parallel to the axes; the extent of each face is set in its definition, as we have seen above.

We have all the elements to define basic shapes. Let's make two examples:

`box([-5, 3, -3], [3, 8, 5], redMaterial, rotationZ(60))` <br/>
`cylinder(lightMaterial, scaling([0.7, 0.7, 1.5]) * translation([2, 0, -1]))`

We can construct more complicated shapes through the Constructive Solid Geometry (*CSG*) just doing the following:
```diff
- CSGOperation = (shape1, shape2, transformation)
```
**CSGOperation: union** (1 + 2), **difference** (1 - 2), **intersection** (1 ∩ 2)

Example: `difference(sphere(redMaterial, identity), sphere(greenMaterial, scaling([0.8, 0.8, 1.2])))`

You can also combine different CSG instructions: eache shape could be the result of a CSG operation. Example: 
<pre>
union( 
intersection(sphere(greeMaterial, translation([0, 0, 1])), cylinder(greenMaterial, identity), identity), 
difference(box([-1, -1, -1], [1, 1, 1], redMaterial, translation([0, 0, 1])), disk(redMaterial, identity), identity), 
identity
) </pre>

## Cameras

The camera describes the position of the obeserver and the viewing direction. 

&emsp; ⚠️ &nbsp; **For each scene you must define one and only one camera!** &nbsp; ⚠️

We can declare a camera in the following way:

```diff
- camera(type, transformation, aspectRatio, distance)
```

where`aspectRatio` is a float number definining how larger than the height is the image, while `distance` is a float number representing the distance between the obersver and the screen. We must specify the type of the camera

**Type: perspective, orthogonal**

Examples:
 
`camera(perspective, translation([-1, 0, 1]), 1.78, 1)` <sub> &emsp;   1.78 is the number corresponding to the aspect ratio 16:9 </sub> <br/>
`camera(orthogonal, translation([-1, 0, 0]), 1.78, 1)`

That's all you need to know! Now it's time to let your imagination run wild and create your first images! 💥
