# Tutorial input scene file

## Materials

(color of objects)

`material 'name' = ('Brdf'('Pigment'), 'Pigment')`

the second pigment is the emitted radiance.

Brdf can be diffuse or specular. Pigment types: uniform, checkered and image

`uniform('color')`

`checkered('color1', 'color2', 'numberOfSteps')`

`image('filename')`

where the file must be in pfm format. For a correct execution of the image, we recommend to use values from 0 to 1 for the three color components.

## Shapes

(objects)

`shapeType = (materialName, transformation)` -> for regular shape, except for boxes -> `box = (pointMin, pointMax, materialName, transformation)`

materialName has to be already define above, I cannot directly define a material here

**transformation = {scaling, translation, rotationX, rotationY, rotationZ}**

CSG

`CSGOperation = (shape1, shape2, transformation)`

**CSGOperation = {union, difference, intersection}**

`scaling('vector')`

`translation('vector')`

`rotationX('value')`

analogous for `rotationY` and `rotationZ`

example: rotationX(35.2) or similarly float angle(35.2) rotationX(angle)

## Cameras

`camera(type, transformation, aspectRatio, distance)`

where type can be one of this two words: perspective or orthogonal

example: camera(perspective, translation([-1, 0, 1]), 1.78, 1) where 1.78 is the number corresponding to the aspect ratio 16:9
example: camera(orthogonal, , 1, 1)
