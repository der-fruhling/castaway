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

// Directional light settings
use dirCount as Lights[D].Count
use dirLights[$].direction as Lights[D].Direction
use dirLights[$].ambient as Lights[D].Ambient
use dirLights[$].diffuse as Lights[D].Diffuse
use dirLights[$].specular as Lights[D].Specular

// Point light settings
use pointCount as Lights[P].Count
use pointLights[$].position as Lights[P].Position
use pointLights[$].constant as Lights[P].Constant
use pointLights[$].linear as Lights[P].Linear
use pointLights[$].quadratic as Lights[P].Quadratic
use pointLights[$].ambient as Lights[P].Ambient
use pointLights[$].diffuse as Lights[P].Diffuse
use pointLights[$].specular as Lights[P].Specular

// Spotlight settings
use spotCount as Lights[S].Count
use spotlights[$].position as Lights[S].Position
use spotlights[$].direction as Lights[S].Direction
use spotlights[$].cutOff as Lights[S].CutOff
use spotlights[$].outerCutOff as Lights[S].OuterCutOff
use spotlights[$].ambient as Lights[S].Ambient
use spotlights[$].diffuse as Lights[S].Diffuse
use spotlights[$].specular as Lights[S].Specular
