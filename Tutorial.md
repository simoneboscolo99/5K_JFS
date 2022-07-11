# Tutorial input scene file

In this file we provide the necessary instructions to build the input file describing the scene. We advise you not to skip this tutorial! At the end of this file you will be able to understand [these](https://github.com/simoneboscolo99/5K_JFS/tree/master/5K_JFS/Examples) examples and to build your own images. 

First of all, we note that all lines starting with the `#` symbol are considered comments, hence they are just skipped by the compiler. 

## Float variables

We can define float variables as follows:
```diff
- float name(value)
``` 
where `value` is a floating-point number, while `name` is just the name of the variabile. Example: `float angle(35.2)` <br/>
Defining float variables is useful for particular transformations, as we will see later.

## Materials

A material characterizes the color of an object and is declared in the following way: 
```diff
- material name = (Brdf(Pigment), Pigment)
```
where `name` is a just the name assigned to the material. Then we must specify the Brdf type, which in turn needs pigment type. Finally, we need a second pigment to specify the emitted radiance. If the second pigment is uniform with color <0, 0, 0> (black), the objetc does not radiate light.

**Brdf types: diffuse, specular**

**Pigment types: uniform, checkered, image**

In order to create a Pigment, we need to know how to define a color. Colors are represented by triplets of numbers enclosed by angular brackets
```diff
+ color = <R, G, B>
```
where R (red color), G (green color) and B (blue color) are float numbers from 0 to infinity. For a correct execution of the image, we recommend to use values from 0 to 1 for the three color components.

```diff
+ uniform(color)
```
Example: `uniform(<0.9, 0.5, 0.1>)`
```diff
+ checkered(color1, color2, numOfSteps)
```
where `numOfSteps` is the number of interations of the checkered pattern. Example: `checkered(<0.9, 0.5, 0.1>, <0.1, 0.9, 0.5>, 5)`
```diff
+ image(filename)
```

where the file must be in pfm format. Example: `image("texture.pfm")` <br/> 
Be careful: if the file of the image is not in the directory where you are executing the code, you must specify the path of the file.

We thus have all the elements to define materials. We report two examples:

`material redMaterial(diffuse(uniform(<1, 0, 0>)), uniform(<0, 0, 0>))` <br/>
`material lightMaterial(diffuse(uniform(<1, 1, 1>)), uniform(<1, 1, 1>))`

## Shapes

The shapes are the objects in our world. We can declare a shape in the following way:

```diff
- shapeType = (materialName, transformation) 
```
for regular shape, except for boxes

```diff
- box = (pointMin, pointMax, materialName, transformation)
```


materialName has to be already define above, I cannot directly define a material here

**Transformation types:  scaling, translation, rotationX, rotationY, rotationZ, identity**

In order to create a transformation, we need to know how to define a vector. Vectors are represented by triplets of numbers enclosed by squared brackets
```diff
+ vector = [X, Y, Z]
```

```diff
+ scaling(vector)
```


```diff
+ translation(vector)
```

```diff
+ rotationX(angle)
```

analogous for `rotationY` and `rotationZ`. Be careful: the angle must be specified in degrees, not radians! Example: `rotationX(35.2)` or similarly `float angle(35.2) rotationX(angle)`

Transformations can be combined via the `*` symbol. Example: `rotationY(30) * translation([-4, 0, 0])` Pay attention to the order, since some trasformations are not commutative. If you don't need any transformation, just write the word 'identity'.

We can construct more complicated shapes through the Constructive Solid Geometry (*CSG*) just doing the following:

`CSGOperation = (shape1, shape2, transformation)`

where **CSGOperation: union, difference, intersection**


## Cameras

The camera describes the position of the obeserver and the direction in which he observes. Be careful: for each scene you must define one and only one camera! We can declare a camera in the following way:

```diff
- camera(type, transformation, aspectRatio, distance)
```

where `type` can be one of this two words: 'perspective' or 'orthogonal'. `distance` is a float number representing the distance between the obersver and the screen.

Examples:
 
 `camera(perspective, translation([-1, 0, 1]), 1.78, 1)` <sub> &emsp;   1.78 is the number corresponding to the aspect ratio 16:9 </sub> <br/>
`camera(orthogonal, translation([-1, 0, 0]), 1.78, 1)`
