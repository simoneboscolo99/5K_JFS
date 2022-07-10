# Tutorial input scene file

(instruction)

In this file we present the different elements that can be inserted in the input file that describes the scene. At the end of this file you will be able to understand the examples (which you can find here), the images of which are shown in the readme gallery, and to build your own images. 

All lines starting with the symbol `#` are considered comments, hence they are skipped by the compiler.

## Float variables

We can define float variables in the input file describing the scene. The way to do this is as follows:

`float name(value)`

value is a floating-point number, while name is just the name of the variabile. Example: *float angle(35.2)*. Defininf float variables is useful for particular transformations, as we will see later.

## Materials

A material, which characterize the and the emission of an object, is declared in the following way: 

`material name = (Brdf(Pigment), Pigment)`

name is a just the name of the variabile, then we specify the Brdf type and the pigment type; finally, we need a second pigment to specigy the emitted radiance. If the second pigment is uniform with color <0, 0, 0> (black), the objetc does not radiate light.

Brdf can be diffuse or specular. Pigment types: uniform, checkered and image

Colors are represented by triplets of numbers enclosed by angular brackets

`color = <R, G, B>`

where R (red color), G (green color) and B (blue color) are float numbers from 0 to infinity. For a correct execution of the image, we recommend to use values from 0 to 1 for the three color components.

`uniform(color)`

`checkered(color1, color2, numberOfSteps)`

first color, second color and the number of interations of the checkered pattern.

`image(filename)`

where the file must be in pfm format. Be careful: if the file of the image is not in the directory where you are executing the code, you must specify the path of the file.

## Shapes

The shapes are the objects in our world. We can declare a shape in the following way:

`shapeType = (materialName, transformation)` -> for regular shape, except for boxes -> `box = (pointMin, pointMax, materialName, transformation)`

materialName has to be already define above, I cannot directly define a material here

**transformation = {scaling, translation, rotationX, rotationY, rotationZ, identity}**

`scaling(vector)`

`vector = [X, Y, Z]`

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

`camera(type, transformation, aspectRatio, screenDistance)`

where 'type' can be one of this two words: 'perspective' or 'orthogonal'. 'ScreenDistance' is a float number representing the distance between the obersver and the screen.

example: *camera(perspective, translation([-1, 0, 1]), 1.78, 1)* where 1.78 is the number corresponding to the aspect ratio 16:9

example: *camera(orthogonal, rotationY(30) * translation([-4, 0, 0]), 1, 1)*
