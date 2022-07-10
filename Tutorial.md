# Tutorial input scene file

In this file we present the different elements that can be inserted in the input file that describes the scene. At the end of this file you will be able to understand the examples (which you can find here), the images of which are shown in the readme gallery, and to build your own images.

## Materials

(color of objects)

`material name = (Brdf(Pigment), Pigment)`

name is a string that identifies the material. 

the second pigment is the emitted radiance.

Brdf can be diffuse or specular. Pigment types: uniform, checkered and image

`color = <R, G, B>`

where R (red color), G (green color) and B (blue color) are float numbers from 0 to infinity. For a correct execution of the image, we recommend to use values from 0 to 1 for the three color components.

`uniform(color)`

`checkered(color1, color2, numberOfSteps)`

`image(filename)`

where the file must be in pfm format. Be careful: if the file of the image is not in the directory where you are executing the code, you must specify the path of the file.

## Shapes

(objects)

`shapeType = (materialName, transformation)` -> for regular shape, except for boxes -> `box = (pointMin, pointMax, materialName, transformation)`

materialName has to be already define above, I cannot directly define a material here

**transformation = {scaling, translation, rotationX, rotationY, rotationZ, identity}**

CSG

`CSGOperation = (shape1, shape2, transformation)`

**CSGOperation = {union, difference, intersection}**

`scaling(vector)`

`vector = [X, Y, Z]`

`translation(vector)`

`rotationX(value)`

analogous for `rotationY` and `rotationZ`. Transformations can be combined via a `*`. Pay attention to the order, since some trasformations are not commutative. If you don't need any transformation, just write the word 'identity'.

example: 
  rotationX(35.2)     or similarly      float angle(35.2) 
                                        rotationX(angle)

## Cameras

`camera(type, transformation, aspectRatio, distance)`

where type can be one of this two words: perspective or orthogonal. Distance is a float number representing the distance between the obersver and the screen.

example: camera(perspective, translation([-1, 0, 1]), 1.78, 1) where 1.78 is the number corresponding to the aspect ratio 16:9
example: camera(orthogonal, rotationY(30) * translation([-4, 0, 0]), 1, 1)
