// Phong lighting model.
// Combines three types of lighting: Ambient, Diffuse, and Specular.
// 
// Configuration.

// Vertex inputs defined in shader.vsh
input inPosition = Position
input inColor = Color
input inNormal = Normal
input inTexCoords = Texture

// Fragment outputs defined in shader.fsh
output outColor = 0

// Transformation matrices.
transform model = model
transform view = view
transform projection = proj

// Tells the camera to store it's position into this uniform.
use viewPos as ViewPosition

// Material settings.
use material.ambient as Materials.Ambient
use material.diffuse as Materials.Diffuse
use material.specular as Materials.Specular
use material.specularExp as Materials.SpecularExp

// Texture settings.
use shouldTexture as Textures[IsActive]
