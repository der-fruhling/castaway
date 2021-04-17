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
uniform vec3 lightPos;

// Camera position.
uniform vec3 viewPos;

// Currently bound texture.
uniform sampler2D tex;

// Used by `Texture` to disable textures when needed.
uniform bool shouldTexture;

void main()
{
    // Do some math to find some directions for later math.
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(-viewPos - fragPos);
    
    // Do not render unseen pixels.
    if(dot(viewDir, norm) < 0) discard;
    
    // Some constant values.
    const float ambientStrength = 0.1;
    const float specularStrength = 0.5;
    const vec4 defaultColor = vec4(1, 1, 1, 1);
    
    // Calculate Ambient lighting
    vec3 ambient = ambientStrength * material.ambient;
    
    // Calculate Diffuse lighting.
    vec3 lightDir = normalize(lightPos - fragPos);
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * material.diffuse;
    
    // Calculate Specular lighting.
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), material.specularExp);
    vec3 specular = specularStrength * spec * material.specular;
    
    // Combine all the lightings!
    outColor = vec4(ambient + diffuse + specular, 1) * ((shouldTexture ? texture(tex, texCoords) : defaultColor) * color);
}
