# Tutorial input scene file

In this file we provide the necessary instructions to build the input file describing the scene. We advise you not to skip this tutorial! At the end of this file you will be able to understand [these](https://github.com/simoneboscolo99/5K_JFS/tree/master/5K_JFS/Examples) examples and to build your own images. 

```diff
+ this text is highlighted in green
- this text is highlighted in red
```

First of all, we note that all lines starting with the `#` symbol are considered comments, hence they are just skipped by the compiler. 

## Float variables

We can define float variables as follows:

```diff
- float name(value)
``` 

where value is a floating-point number, while name is just the name of the variabile. Example: `float angle(35.2)`. Defining float variables is useful for particular transformations, as we will see later.

## Materials

A material, which characterizes the and the emission of an object, is declared in the following way: 

```diff
- material name = (Brdf(Pigment), Pigment)
```

where name is a just the name assigned to the material, then we specify the Brdf type and the pigment type; finally, we need a second pigment to specigy the emitted radiance. If the second pigment is uniform with color <0, 0, 0> (black), the objetc does not radiate light.

**Brdf types: diffuse, specular**

**Pigment types: uniform, checkered, image**

```diff
+ uniform(color)
```

```diff
+ checkered(color1, color2, numberOfSteps)
```

first color, second color and the number of interations of the checkered pattern.

```diff
+ image(filename)
```

where the file must be in pfm format. Be careful: if the file of the image is not in the directory where you are executing the code, you must specify the path of the file.

Colors are represented by triplets of numbers enclosed by angular brackets

```diff
+ color = <R, G, B>
```

where R (red color), G (green color) and B (blue color) are float numbers from 0 to infinity. For a correct execution of the image, we recommend to use values from 0 to 1 for the three color components.

Examples of materials:
` `

## Shapes

The shapes are the objects in our world. We can declare a shape in the following way:

```diff
- shapeType = (materialName, transformation)` -> for regular shape, except for boxes -> `box = (pointMin, pointMax, materialName, transformation)
```

materialName has to be already define above, I cannot directly define a material here

**transformation = {scaling, translation, rotationX, rotationY, rotationZ, identity}**

`scaling(vector)`

```diff
vector = [X, Y, Z]
```

`translation(vector)`

`rotationX(angle)`

analogous for `rotationY` and `rotationZ`. Be careful: the angle must be specified in degrees, not radians!

example: 
  *rotationX(35.2)*     or similarly      *float angle(35.2)* 
                                        *rotationX(angle)*

Transformations can be combined via the `*` symbol. Pay attention to the order, since some trasformations are not commutative. If you don't need any transformation, just write the word 'identity'.

We can construct more complicated shapes through the Constructive Solid Geometry (*CSG*) just doing the following:

`CSGOperation = (shape1, shape2, transformation)`

where **CSGOperation = {union, difference, intersection}**


## Cameras

The camera describes the position of the obeserver and the direction in which he observes. Be careful: for each scene you must define one and only one camera! We can declare a camera in the following way:

```diff
- camera(type, transformation, aspectRatio, screenDistance)
```

where 'type' can be one of this two words: 'perspective' or 'orthogonal'. 'ScreenDistance' is a float number representing the distance between the obersver and the screen.

example: *camera(perspective, translation([-1, 0, 1]), 1.78, 1)* where 1.78 is the number corresponding to the aspect ratio 16:9

example: *camera(orthogonal, rotationY(30) * translation([-4, 0, 0]), 1, 1)*
