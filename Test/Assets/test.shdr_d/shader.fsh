/*
 * Phong lighting model.
 * Combines three types of lighting: Ambient, Diffuse, and Specular.
 *
 * Fragment shader.
 */

#version 150 core

// Structure to contain some info about what materials should look like.
struct Material
{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float specularExp;
};

// Passed from vertex shader.
in vec4 color;
in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

// Output color.
out vec4 outColor;

// Filled with knowledge.
uniform Material material;

// Some light settings.
uniform int lightCount = 0;
uniform vec3 lightPositions[128];

// Camera position.
uniform vec3 viewPos;

// Currently bound texture.
uniform sampler2D tex;

// Used by `Texture` to disable textures when needed.
uniform bool shouldTexture;

const float specularStrength = 0.5;
const float ambientStrength = 0.1;

// Calculate Diffuse lighting.
vec3 calculateDiffuse(vec3 lightPos, vec3 norm, vec3 lightDir)
{
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * material.diffuse;
    
    return diffuse;
}

// Calculate Specular lighting.
vec3 calculateSpecular(vec3 lightPos, vec3 norm, vec3 lightDir)
{
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), material.specularExp);
    vec3 specular = specularStrength * spec * material.specular;
    
    return specular;
}

// Combine Diffuse and Specular lighting.
vec3 calculateLighting(vec3 lightPos)
{
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - fragPos);

    return calculateDiffuse(lightPos, norm, lightDir) + calculateSpecular(lightPos, norm, lightDir);
}

void main()
{
    // Do not render unseen pixels.
    if(dot(normalize(viewPos - fragPos), normalize(normal)) < 0) discard;
    
    // Calculate all diffuse and specular lightings.
    vec3 lighting = vec3(0, 0, 0);
    for(int i = 0; i < min(lightCount, 128); i++) {
        lighting += calculateLighting(lightPositions[i]);
    }

    // Calculate ambient lighting.
    vec3 ambient = ambientStrength * material.ambient;
    
    // Constant.
    const vec4 defaultColor = vec4(1, 1, 1, 1);
    
    // Combine all the lightings!
    outColor = vec4(ambient + lighting, 1) * ((shouldTexture ? texture(tex, texCoords) : defaultColor) * color);
}
