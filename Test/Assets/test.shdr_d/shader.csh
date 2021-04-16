#input inPosition = Position
#input inColor = Color
#input inNormal = Normal
#input inTexCoords = Texture

#output outColor = 0

#transform model = model
#transform view = view
#transform projection = proj

#use viewPos as ViewPosition

#use material.ambient as Materials.Ambient
#use material.diffuse as Materials.Diffuse
#use material.specular as Materials.Specular
#use material.specularExp as Materials.SpecularExp

#use shouldTexture as Textures[IsActive]
